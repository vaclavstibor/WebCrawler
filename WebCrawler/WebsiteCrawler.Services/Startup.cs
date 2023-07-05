using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.DataAccessLayer.Context;
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

        public async Task Run()
        {
            while (true)
            {
                Thread.Sleep(1000);
                try
                {
                    var crawler = provider.GetService<IWebSiteCrawler>();
                    var db = provider.GetService<AppDbContext>();

                    var unscheduledRecords = await db.Records.Where(x => x.ExecutionStatus != WebCrawler.DataAccessLayer.Models.ExecutionStatus.Executing)
                        .Include(x => x.StartingNode)
                        .ToListAsync();

                    foreach (var record in unscheduledRecords)
                    {
                        record.Days ??= 0;
                        record.Hours ??= 0;
                        record.Minutes ??= 0;
                        var frequency = new TimeSpan(record.Days.Value, record.Hours.Value, record.Minutes.Value, 0);
                        var timeDifference = DateTime.Now - record.LastExecution;


                        if (record.ExecutionStatus == WebCrawler.DataAccessLayer.Models.ExecutionStatus.Created
                            || (timeDifference >= frequency && frequency != new TimeSpan(0,0,0,0))
                        )
                        {
                            await Task.Run(async () => {
                                var result = await crawler.Run(record, 1000);
                                record.StartingNode = result;
                                db.Update(record);
                                await db.SaveChangesAsync();
                            });
                            
                        }
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
