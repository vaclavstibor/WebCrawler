using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebCrawler.BusinessLayer.Services;
using WebCrawler.DataAccessLayer.Context;
using WebCrawler.DataAccessLayer.Models;
using WebsiteCrawler.Infrastructure.interfaces;
using WebsiteCrawler.Service;
using Microsoft.EntityFrameworkCore.SqlServer;

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
                //options.UseSqlServer("Data Source=localhost;Initial Catalog=CrawlerDB;Integrated Security=True; TrustServerCertificate=true"),
                options.UseSqlServer("Server=sql_server2022;Database=SalesDb;User Id=SA;Password=A&VeryComplex123Password;MultipleActiveResultSets=true;TrustServerCertificate=True"),
                contextLifetime: ServiceLifetime.Transient
            );

            return serviceCollection.BuildServiceProvider();
        }

        private void CreateNewExecution(WebsiteRecord record)
        {
            var db = provider.GetRequiredService<AppDbContext>();

            record.ExecutionStatus = ExecutionStatus.Executing;
            record.LastExecution = DateTime.Now;

            var newExecution = new Execution()
            {
                StartTime = DateTime.Now,
                ExecutionStatus = ExecutionStatus.Executing,
                WebsiteRecordId = record.Id
            };

            db.Add(newExecution);
            db.SaveChanges();

            Task.Run(async () => {
                var db = provider.GetRequiredService<AppDbContext>();

                var oldNodes = await db.Nodes.Where(x => x.WebsiteRecordId == record.Id).ToListAsync();

                var crawler = provider.GetRequiredService<IWebSiteCrawler>();
                
                await crawler!.Run(record, 10000)
                .ContinueWith(async x => 
                {
                    record.ExecutionStatus = ExecutionStatus.Executed;

                    newExecution.EndTime = DateTime.Now;
                    newExecution.ExecutionStatus = ExecutionStatus.Executed;
                    newExecution.NumberOfSites = (await x).NumberOfSites;

                    var db = provider.GetRequiredService<AppDbContext>();

                    db.Executions.Update(newExecution);
                    db.Records.Update(record);

                    foreach (var node in oldNodes)
                    {
                        node.Children = null;
                    }
                    db.Nodes.RemoveRange(oldNodes);

                    await db.SaveChangesAsync();
                });
            });
        }

        private void Execute(Execution execution)
        {
            var db = provider.GetRequiredService<AppDbContext>();

            execution.WebsiteRecord.ExecutionStatus = ExecutionStatus.Executing;
            execution.WebsiteRecord.LastExecution = DateTime.Now;
            execution.ExecutionStatus = ExecutionStatus.Executing;
            execution.StartTime = DateTime.Now;

            db.Executions.Update(execution);
            db.SaveChanges();

            Task.Run(async () => {
                var db = provider.GetRequiredService<AppDbContext>();

                var oldNodes = await db.Nodes.Where(x => x.WebsiteRecordId == execution.WebsiteRecordId).ToListAsync();

                var crawler = provider.GetService<IWebSiteCrawler>();

                await crawler!.Run(execution.WebsiteRecord, execution.Id, 10000)
                .ContinueWith(async x =>
                {
                    execution.WebsiteRecord.ExecutionStatus = ExecutionStatus.Executed;

                    execution.EndTime = DateTime.Now;
                    execution.ExecutionStatus = ExecutionStatus.Executed;
                    execution.NumberOfSites = (await x).NumberOfSites;

                    var db = provider.GetRequiredService<AppDbContext>();
                    db = provider.GetRequiredService<AppDbContext>();

                    db.Executions.Update(execution);

                    foreach (var node in oldNodes)
                    {
                        node.Children = null;
                        node.Parents = null;
                    }
                    db.Nodes.UpdateRange(oldNodes);

                    await db.SaveChangesAsync();

                    db.Nodes.RemoveRange(oldNodes);

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
        }
    }
}
