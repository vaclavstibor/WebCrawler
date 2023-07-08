using Microsoft.AspNetCore.Mvc;
using WebCrawler.BusinessLayer.Services;

namespace WebCrawler.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CrawlerController
    {
        private readonly CrawlerService crawlerService;

        public CrawlerController(CrawlerService crawlerService)
        {
            this.crawlerService = crawlerService;
        }

        [HttpGet("all")]
        public async Task<JsonResult> GetAll()
        { 
            return new JsonResult(await crawlerService.GetAllExecutions());
        }

        [HttpPost("execute")]
        public async Task Execute([FromBody] int websiteRecordId)
        {
            await crawlerService.StartExecution(websiteRecordId);
        }
    }
}
