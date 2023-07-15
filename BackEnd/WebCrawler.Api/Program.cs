using WebCrawler.BusinessLayer.Services;
using WebsiteCrawler.Services;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;

namespace WebCrawler.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebsiteCrawler.Services.Startup crawlerStartup = new WebsiteCrawler.Services.Startup();
            
            Task.Run(() => crawlerStartup.Run());

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}