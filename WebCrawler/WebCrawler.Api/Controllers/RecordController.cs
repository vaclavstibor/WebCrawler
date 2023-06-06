using Microsoft.AspNetCore.Mvc;
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

        [HttpGet(Name = "GetRecord/{id}")]
        public async Task<JsonResult> GetRecord(int id)
        {
            return new JsonResult(await recordsService.GetARecord(id));
        }

        [HttpDelete(Name = "DeleteRecord/{id}")]
        public async Task<StatusCodeResult> DeleteRecord([FromHeader] int recordId)
        {
            if (await recordsService.DeleteWebsiteRecord(recordId))
            { 
                return new OkResult();
            }

            return new NotFoundResult();
        }
    }
}
