using Microsoft.AspNetCore.Mvc;
using WebCrawler.BusinessLayer.DataTransferObjects;
using WebCrawler.BusinessLayer.Services;

namespace WebCrawler.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecordController : ControllerBase
    {
        private readonly RecordsService recordsService;

        public RecordController(RecordsService recordsService)
        {
            this.recordsService = recordsService;
        }

        [HttpGet("single/{id}")]
        public async Task<JsonResult> GetSingle(int id)
        {
            return new JsonResult(await recordsService.GetARecord(id));
        }

        [HttpDelete("deleteSingle/{id:int}")]
        public async Task<StatusCodeResult> DeleteSingle(int id)
        {
            if (await recordsService.DeleteWebsiteRecord(id))
            { 
                return new OkResult();
            }

            return new NotFoundResult();
        }

        [HttpPost("updateSingle")]
        public async Task<JsonResult> UpdateSingle([FromBody] WebsiteRecordDTO record)
        {
            return new JsonResult(await recordsService.UpdateWebsiteRecord(record));
        }

        [HttpGet("all")]
        public async Task<JsonResult> GetAll()
        {
            return new JsonResult(await recordsService.GetAllRecords());
        }
    }
}
