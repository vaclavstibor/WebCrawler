using WebsiteCrawler.Services;
using Microsoft.AspNetCore;
using WebCrawler.DataAccessLayer.Context;

namespace WebCrawler.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().MigrateDatabase<AppDbContext>().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}