using Microsoft.AspNetCore.Mvc;
using WebCrawler.BusinessLayer.Services;
using System.Threading.Tasks;

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
            var result = await crawlerService.GetAllNodes(id, DataAccessLayer.Models.ExecutionStatus.Executed);
            var json = new JsonResult(result);
            return json;
        }

        [HttpGet("getGraphLive/{id:int}")]
        public async Task<JsonResult> GetGraphLive(int id)
        {
            var result = await crawlerService.GetAllNodes(id, DataAccessLayer.Models.ExecutionStatus.Executing);
            var json = new JsonResult(result);
            return json;
        }
    }
}
