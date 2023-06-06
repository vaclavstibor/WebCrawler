using WebCrawler.BusinessLayer.Services;
using WebCrawler.BusinessLayer.DataTransferObjects;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebCrawler.BusinessLayer.Options;
using System.Linq;

namespace WebCrawler.Web.ViewModels
{
    public class RecordsViewModel : MasterPageViewModel
    {
        private readonly RecordsService recordsService;
        private SortOptions sortOptions { get; set; } = null;
        private FilterOptions filterOptions { get; set; } = null;
        public List<WebsiteRecordDTO> WebsiteRecords { get; set; }
        public WebsiteRecordDTO newRecord { get; set; }
        public List<TagDTO> tagDTOs { get; set; }
        public TagDTO newTag { get; set; }

        public bool URLSort { get; set; }
        public bool LastCrawlingSort { get; set; }
        public bool Alphabetically { get; set; }
        public bool NewestToOldest { get; set; }


        public List<string> URLsForFiltering { get; set; }
        public List<string> LabelsForFiltering { get; set; }
        public List<string> TagsForFiltering { get; set; }



        public List<string> ChosenURLsForFiltering { get; set; } = new List<string>();
        public List<string> ChosenLabelsForFiltering { get; set; } = new List<string>();
        public List<string> ChosenTagsForFiltering { get; set; } = new List<string>();

        public string SelectedURL { get; set; }
        public string SelectedLabel { get; set; }   
        public string SelectedTag { get; set; }
        public RecordsViewModel(RecordsService recordsService)
        {
            this.recordsService = recordsService;
        }
        public override async Task PreRender()
        {
            if (!Context.IsPostBack)
            {
                WebsiteRecords = await recordsService.GetAllRecords();
                URLsForFiltering = WebsiteRecords.Select(x => x.URL).Where(x => x != null && x != "").ToList();
                LabelsForFiltering = WebsiteRecords.Select(x => x.Label).Where(x => x != null && x != "").ToList();
                TagsForFiltering = WebsiteRecords.SelectMany(x => x.tagDTOs).Select(x => x.Content).Where(x => x != null && x != "").ToList();

                newRecord = new WebsiteRecordDTO();
                tagDTOs = new List<TagDTO>();
                newTag = new TagDTO();

            }
            await base.PreRender();
        }
        public void AddRecord()
        {

        }
        public async Task SaveNewRecord()
        {
            await recordsService.AddWebsiteRecord(newRecord, tagDTOs);
            newRecord = new WebsiteRecordDTO();
            tagDTOs = new List<TagDTO>();
            await SortFilter();
        }
        public void SaveNewTag()
        {
            if (newTag != null)
            {
                tagDTOs.Add(newTag);
                newTag = new TagDTO();
            }
        }
        public void CancelTag(TagDTO tag)
        {
            tagDTOs.Remove(tag);
        }
        public async Task DeleteRecord(WebsiteRecordDTO record)
        {
            await recordsService.DeleteWebsiteRecord(record);
            WebsiteRecords = await recordsService.GetAllRecords();
        }
        public async Task SortFilter()
        {
            if (URLSort || LastCrawlingSort)
            {
                sortOptions = new SortOptions
                {
                    AlphabeticalSorting = URLSort,
                    TimeSorting = LastCrawlingSort,
                    Alphabetically = Alphabetically,
                    NewestToOldest = NewestToOldest
                };
            }

            if (ChosenURLsForFiltering.Count != 0 || ChosenTagsForFiltering.Count != 0 || ChosenLabelsForFiltering.Count != 0)
            {
                filterOptions = new FilterOptions()
                {
                    URLs = ChosenURLsForFiltering,
                    Tags = ChosenTagsForFiltering,
                    Labels = ChosenLabelsForFiltering
                };
            }
            WebsiteRecords = await recordsService.GetAllRecords(sortOptions = sortOptions, filterOptions = filterOptions);
            sortOptions = null;
            filterOptions = null;
        }
        public void AddStringToFilter(int category)
        {
            if (category == 1 && !ChosenTagsForFiltering.Any(x => x == SelectedTag))
            {
                ChosenTagsForFiltering.Add(SelectedTag);
            }
            else if (category == 2 && !ChosenLabelsForFiltering.Any(x => x == SelectedLabel))
            {
                ChosenLabelsForFiltering.Add(SelectedLabel);
            }
            else if (!ChosenURLsForFiltering.Any(x => x == SelectedURL))
            {
                ChosenURLsForFiltering.Add(SelectedURL);
            }
        }
        public void CancelStringFromFilter(int category, string selected)
        {
            if (category == 1)
            {
                ChosenTagsForFiltering.Remove(selected);
            }
            else if (category == 2)
            {
                ChosenLabelsForFiltering.Remove(selected);
            }
            else
            {
                ChosenURLsForFiltering.Remove(selected);
            }
        }
    }
}

