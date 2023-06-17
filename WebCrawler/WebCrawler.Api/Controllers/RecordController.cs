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

        [HttpDelete("deleteSingle/{id}")]
        public async Task<StatusCodeResult> DeleteSingle([FromHeader] int recordId)
        {
            if (await recordsService.DeleteWebsiteRecord(recordId))
            { 
                return new OkResult();
            }

            return new NotFoundResult();
        }

        [HttpDelete("updateSingle/{id}")]
        public async Task<StatusCodeResult> UpdateSingle([FromBody] WebsiteRecordDTO record)
        {
            await recordsService.UpdateWebsiteRecord(record);
            return new OkResult();
        }

        [HttpGet("all")]
        public async Task<JsonResult> GetAll()
        {
            return new JsonResult(await recordsService.GetAllRecords());
        }
    }
}
