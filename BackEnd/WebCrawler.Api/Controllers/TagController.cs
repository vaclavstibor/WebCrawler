using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebCrawler.BusinessLayer.DataTransferObjects;
using WebCrawler.BusinessLayer.Services;

namespace WebCrawler.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly RecordsService recordsService;

        public TagController(RecordsService recordsService)
        { 
            this.recordsService = recordsService;
        }

        [HttpPost(Name = "SaveNewTag")]
        public async Task<StatusCodeResult> SaveNewTag([FromBody] TagDTO newTag)
        {
            if (newTag == null)
                return new NotFoundResult();

            await recordsService.AddNewTag(newTag);

            return new OkResult();
        }

        [HttpDelete(Name = "DeleteTag")]
        public async Task<StatusCodeResult> DeleteTag([FromBody] TagDTO newTag)
        {
            if (newTag == null)
                return new NotFoundResult();

            await recordsService.DeleteTag(newTag); 

            return new OkResult();
        }
    }
}
