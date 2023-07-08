using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.DataAccessLayer.Models;

namespace WebCrawler.BusinessLayer.DataTransferObjects
{
    public class ExecutionDto
    {
        public int WebsiteRecordId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public ExecutionStatus ExecutionStatus { get; set; }
        public string WebsiteRecordLabel { get; set; }
        public int? NumberOfSitesCrawled { get; set; }
    }
}
