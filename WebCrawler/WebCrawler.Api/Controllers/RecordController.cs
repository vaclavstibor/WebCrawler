using Microsoft.AspNetCore.Mvc;
using WebCrawler.BusinessLayer.Services;

namespace WebCrawler.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RecordController : ControllerBase
    {
        private readonly ILogger<RecordController> logger;
        private readonly RecordsService recordsService;

        public RecordController(ILogger<RecordController> logger, RecordsService recordsService)
        {
            this.logger = logger;   
            this.recordsService = recordsService;
        }

        [HttpGet(Name = "EditRecord/{id}")]
        public async Task<JsonResult> ShowRecord(int id)
        {
            return new JsonResult(await recordsService.GetARecord(id));
        }
    }
}
