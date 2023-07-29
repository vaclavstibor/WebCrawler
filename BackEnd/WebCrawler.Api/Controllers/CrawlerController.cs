﻿using Microsoft.AspNetCore.Mvc;
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
        public async Task<JsonResult> GetGraph(int id)
        {
            var result = await crawlerService.GetAllNodes(id);
            var json = new JsonResult(result);
            return json;
        }

        [HttpGet("getGraph2/{id:int}")]
        public async Task<JsonResult> GetGraph2(int id)
        {
            var result = await crawlerService.GetAllNodesMinimal(id);
            var json = new JsonResult(result);
            return json;
        }
    }
}
