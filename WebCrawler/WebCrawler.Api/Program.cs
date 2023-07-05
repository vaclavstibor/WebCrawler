using WebCrawler.BusinessLayer.Services;
using WebCrawler.DataAccessLayer.Context;
using WebsiteCrawler.Services;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace WebCrawler.Api
{
    public class Program
    {
        public static void Main()
        {
            Startup webStartup = new Startup();
            WebsiteCrawler.Services.Startup crawlerStartup = new WebsiteCrawler.Services.Startup();

            Task.Run(() => webStartup.Run());
            Task.Run(() => crawlerStartup.Run());
        }
    }
}