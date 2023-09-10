using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using WebCrawler.BusinessLayer.GraphQLModels;
using WebCrawler.BusinessLayer.Services;

namespace WebCrawler.Api.Controllers
{
    public class GraphQLController : GraphController
    {
        private readonly CrawlerService crawlerService;
        private readonly RecordsService recordsService;

        public GraphQLController(CrawlerService crawlerService, RecordsService recordsService)
        {
            this.recordsService = recordsService;
            this.crawlerService = crawlerService;
        }

        [QueryRoot("websites")]
        public async Task<List<WebPage>> GetWebsites()
        {
            return await recordsService.GetWebPages();
        }

        [QueryRoot("nodes")]
        public async Task<List<Node>> GetNodes(List<string> webPages)
        {
            return await crawlerService.GetALlNodes(webPages);
        }
    }
}
