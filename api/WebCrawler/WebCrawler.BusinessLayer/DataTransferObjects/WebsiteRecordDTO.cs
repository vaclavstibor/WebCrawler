using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler.BusinessLayer.DataTransferObjects
{
    public class WebsiteRecordDTO
    {
        public int Id { get; set; }
        public string URL { get; set; }
        public string? RegExp { get; set; }
        public int? Hours { get; set; }
        public int? Minutes { get; set; }
        public int? Days { get; set; }
        public string? Label { get; set; }
        public bool Active { get; set; }
        public DateTime? LastExecution { get; set; }
        public List<TagDTO> tagDTOs { get; set; }
        public bool? ExecutionStatus { get; set; }
        public string Periodicity { get; set; }

        public WebsiteRecordDTO()
        {
            tagDTOs = new List<TagDTO>();
        }
    }
}
