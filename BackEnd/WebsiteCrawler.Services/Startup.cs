using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebCrawler.DataAccessLayer.Context;
using WebCrawler.DataAccessLayer.Models;
using WebsiteCrawler.Infrastructure.interfaces;
using WebsiteCrawler.Service;

namespace WebsiteCrawler.Services
{
    public class Startup
    {
        private IServiceProvider provider;

        public Startup()
        {
            provider = ConfigureServices();
        }

        private ServiceProvider ConfigureServices()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddTransient<IWebSiteCrawler, SingleThreadedWebSiteCrawler>();

            serviceCollection.AddTransient<IHtmlParser, HtmlAgilityParser>();

            serviceCollection.AddTransient<HttpClient>();

            serviceCollection.AddDbContext<AppDbContext>(options =>
               options.UseSqlServer("Data Source=localhost;Initial Catalog=CrawlerDB;Integrated Security=True"));

            return serviceCollection.BuildServiceProvider();
        }

        private async Task CreateNewExecution(WebsiteRecord record)
        {
            var db = provider.GetService<AppDbContext>();
            var crawler = provider.GetService<IWebSiteCrawler>();

            await Task.Run(async () => {
                record.ExecutionStatus = ExecutionStatus.Executing;
                record.LastExecution = DateTime.Now;

                var newExecution = new Execution()
                {
                    StartTime = DateTime.Now,
                    ExecutionStatus = ExecutionStatus.Executing,
                    WebsiteRecordId = record.Id
                };

                db!.Update(record);
                db.Add(newExecution);

                await db.SaveChangesAsync();

                var result = crawler!.Run(record, 1000);
                await result.ContinueWith(async x => 
                {
                    record.StartingNode = await result;
                    record.ExecutionStatus = ExecutionStatus.Executed;

                    newExecution.EndTime = DateTime.Now;
                    newExecution.ExecutionStatus = ExecutionStatus.Executed;
                    newExecution.NumberOfSites = record.StartingNode.NumberOfSites;

                    db.Update(newExecution);
                    db.Update(record);
                    await db.SaveChangesAsync();
                });
            });
        }

        public async Task Run()
        {
            while (true)
            {
                Thread.Sleep(1000);
                try
                {
                    var crawler = provider.GetService<IWebSiteCrawler>();
                    var db = provider.GetService<AppDbContext>();

                    var unscheduledRecords = await db.Records.Where(x => x.ExecutionStatus != ExecutionStatus.Executing)
                        .Include(x => x.StartingNode)
                        .ToListAsync();

                    var manualExecutions = await db.Executions.Where(x => x.ExecutionStatus == ExecutionStatus.Created)
                        .Include(x => x.WebsiteRecord)
                        .ToListAsync();

                    var manuallyExecutedRecords = manualExecutions
                        .Select(x => x.WebsiteRecord)
                        .ToList();

                    foreach (var record in unscheduledRecords.Union(manuallyExecutedRecords))
                    {
                        record.Days ??= 0;
                        record.Hours ??= 0;
                        record.Minutes ??= 0;
                        var frequency = new TimeSpan(record.Days.Value, record.Hours.Value, record.Minutes.Value, 0);
                        var timeDifference = DateTime.Now - record.LastExecution;

                        if (record.ExecutionStatus == ExecutionStatus.Created
                            || (timeDifference >= frequency && frequency != new TimeSpan(0,0,0,0))
                        )
                        {
                            await CreateNewExecution(record);
                        }
                    }

                    db.Executions.RemoveRange(manualExecutions);
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}
