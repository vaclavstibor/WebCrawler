using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebCrawler.BusinessLayer.DataTransferObjects;

namespace WebCrawler.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        [HttpPost(Name = "SaveNewTag")]
        public async Task<StatusCodeResult> SaveNewTag([FromBody] TagDTO newTag)
        {
            if (newTag == null)
                return new NotFoundResult();

            return new OkResult();
        }   
    }
}
