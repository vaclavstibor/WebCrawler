using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotVVM.Framework.ViewModel;
using WebCrawler.BusinessLayer.Services;
using WebCrawler.BusinessLayer.DataTransferObjects;

namespace WebCrawler.Web.ViewModels
{
    public class RecordEditViewModel : MasterPageViewModel
    {
        [FromRoute("Id")]
        public int PersonId { get; set; }
        private readonly RecordsService recordsService;
        public WebsiteRecordDTO Record { get; set; }
        public TagDTO newTag { get; set; }
        public RecordEditViewModel(RecordsService recordsService)
        {
            this.recordsService = recordsService;
        }
        public override async Task PreRender()
        {
            if (!Context.IsPostBack)
            {
                Record = await recordsService.GetARecord(PersonId);
                newTag = new TagDTO();
            }
            await base.PreRender();
        }


        public async Task Cancel()
        {
            Record = await recordsService.GetARecord(PersonId);
        }

        public async Task Save()
        {
            await recordsService.UpdateWebsiteRecord(Record);
        }

        public void DeleteTag(TagDTO tag)
        {
            Record.tagDTOs.Remove(tag);
        }
        public void SaveNewTag()
        {
            if (newTag != null)
            {
                Record.tagDTOs.Add(newTag);
                newTag = new TagDTO();
            }
        }
    }
}

