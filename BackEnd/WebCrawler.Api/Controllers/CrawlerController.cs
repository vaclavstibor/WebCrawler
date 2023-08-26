using Microsoft.AspNetCore.Mvc;
using WebCrawler.BusinessLayer.Services;
using WebCrawler.DataAccessLayer.Cache;

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
            var result = new JsonResult(await crawlerService.GetAllExecutions());
            return result;
        }

        [HttpPost("execute")]
        public async Task Execute([FromBody] int websiteRecordId)
        {
            await crawlerService.StartExecution(websiteRecordId);
        }

        [HttpGet("getGraph/{id:int}")]
        public async Task<JsonResult> GetGraphStatic(int id)
        {
            return new JsonResult(await crawlerService.GetAllNodes(id));
        }

        [HttpGet("getGraph/live/{id:int}")]
        public JsonResult GetGraphStatisticLive(int id)
        {
            return new JsonResult(crawlerService.GetAllNodesLive(id));
        }
    }
}
