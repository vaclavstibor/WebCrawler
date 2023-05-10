using Microsoft.AspNetCore.Mvc;
using WebCrawler.BusinessLayer.DataTransferObjects;
using WebCrawler.BusinessLayer.Services;

namespace WebCrawler.Api.Controllers
{
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
        public async Task<WebsiteRecordDTO> ShowRecord(int id)
        {
            return await recordsService.GetARecord(id);
        }
    }
}
