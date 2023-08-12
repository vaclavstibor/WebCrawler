using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography.X509Certificates;
using WebCrawler.BusinessLayer.Services;
using WebCrawler.DataAccessLayer.Context;
using WebCrawler.DataAccessLayer.Models;
using WebsiteCrawler.Infrastructure.interfaces;
using WebsiteCrawler.Service;

namespace WebsiteCrawler.Services
{
    public class Executor
    {
        private IServiceProvider provider;

        public Executor()
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

        private void CreateNewExecution(WebsiteRecord record)
        {
            Task.Run(async () => {
                var db = provider.GetRequiredService<AppDbContext>();
                var crawler = provider.GetRequiredService<IWebSiteCrawler>();


                record.ExecutionStatus = ExecutionStatus.Executing;
                record.LastExecution = DateTime.Now;

                var oldExecutions = await db.Executions.Where(x => x.WebsiteRecordId == record.Id).ToListAsync();

                var newExecution = new Execution()
                {
                    StartTime = DateTime.Now,
                    ExecutionStatus = ExecutionStatus.Executing,
                    WebsiteRecordId = record.Id
                };

                db.Add(newExecution);
                await db.SaveChangesAsync();

                var result = crawler!.Run(record, 100);

                await result.ContinueWith(async x => 
                {
                    record.ExecutionStatus = ExecutionStatus.Executed;

                    newExecution.EndTime = DateTime.Now;
                    newExecution.ExecutionStatus = ExecutionStatus.Executed;
                    newExecution.NumberOfSites = (await result).NumberOfSites;

                    db = provider.GetRequiredService<AppDbContext>();

                    db.Update(newExecution);
                    db.Update(record);

                    var oldNodes = db.Nodes.Where(x => oldExecutions.Any(y => y.Id == x.ExecutionId));
                    db.RemoveRange(oldNodes);

                    await db.SaveChangesAsync();
                });
            });
        }

        private void Execute(Execution execution)
        {
            Task.Run(async () => {
                var db = provider.GetRequiredService<AppDbContext>();
                var crawler = provider.GetService<IWebSiteCrawler>();


                execution.WebsiteRecord.ExecutionStatus = ExecutionStatus.Executing;
                execution.WebsiteRecord.LastExecution = DateTime.Now;
                execution.ExecutionStatus = ExecutionStatus.Executing;
                execution.StartTime = DateTime.Now;

                db.Executions.Update(execution);
                await db.SaveChangesAsync();

                var oldExecutions = await db.Executions.Where(x => x.WebsiteRecordId == execution.WebsiteRecordId).ToListAsync();

                var result = crawler!.Run(execution.WebsiteRecord, execution.Id, 100);
                await result.ContinueWith(async x =>
                {
                    execution.WebsiteRecord.ExecutionStatus = ExecutionStatus.Executed;

                    execution.EndTime = DateTime.Now;
                    execution.ExecutionStatus = ExecutionStatus.Executed;
                    execution.NumberOfSites = (await result).NumberOfSites;

                    db = provider.GetRequiredService<AppDbContext>();

                    db.Update(execution);
                    db.Update(execution.WebsiteRecord);

                    var oldNodes = db.Nodes.Where(x => oldExecutions.Any(y => y.Id == x.ExecutionId));
                    db.RemoveRange(oldNodes);

                    await db.SaveChangesAsync();
                });
            });
        }
        
        private async Task ReexecuteRecords()
        {
            var db = provider.GetRequiredService<AppDbContext>();

            var orphaned = await db.Executions
                .Include(x => x.WebsiteRecord)
                .Where(x => x.WebsiteRecord.ExecutionStatus == ExecutionStatus.Executing)
                .ToListAsync();

            foreach (var orphan in orphaned)
            {
                orphan.ExecutionStatus = ExecutionStatus.Created;
                orphan.WebsiteRecord.ExecutionStatus = ExecutionStatus.Created;
            }

            db.Executions.UpdateRange(orphaned);

            await db.SaveChangesAsync();
        }
        public async Task Run()
        {
            await ReexecuteRecords();

            while (true)
            {
                Thread.Sleep(1000);
                try
                {
                    var crawler = provider.GetService<IWebSiteCrawler>();
                    var db = provider.GetRequiredService<AppDbContext>();

                    var executed = await db.Records.Where(x => x.ExecutionStatus == ExecutionStatus.Executed && x.Active == true)
                        .ToListAsync();

                    foreach (var record in executed)
                    {
                        record.Days ??= 0;
                        record.Hours ??= 0;
                        record.Minutes ??= 0;
                        var frequency = new TimeSpan(record.Days.Value, record.Hours.Value, record.Minutes.Value, 0);
                        var timeDifference = DateTime.Now - record.LastExecution;

                        if (timeDifference >= frequency && frequency != new TimeSpan(0,0,0,0))
                        {
                            CreateNewExecution(record);
                        }
                    }

                    var manualExecutions = await db.Executions.Where(x => x.ExecutionStatus == ExecutionStatus.Created)
                        .Include(x => x.WebsiteRecord)
                        .ToListAsync();

                    foreach (var manualExecution in manualExecutions)
                    {
                        Execute(manualExecution);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}
