using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebCrawler.BusinessLayer.DataTransferObjects;
using System.Threading.Tasks;
using DotVVM.Framework.ViewModel;
using WebCrawler.BusinessLayer.Services;

namespace WebCrawler.Web.ViewModels
{
    public class RecordDetailViewModel : MasterPageViewModel
    {
        [FromRoute("Id")]
        public int PersonId { get; set; }
        private readonly RecordsService recordsService;
        public WebsiteRecordDTO Record { get; set; }
        public RecordDetailViewModel(RecordsService recordsService)
        {
            this.recordsService = recordsService;
        }
        public override async Task PreRender()
        {
            if (!Context.IsPostBack)
            {
                Record = await recordsService.GetARecord(PersonId);
            }
            await base.PreRender();
        }

        public async Task DeleteRecord()
        {
            await recordsService.DeleteWebsiteRecord(Record);
            Context.RedirectToUrl("/");
        }
    }
}

