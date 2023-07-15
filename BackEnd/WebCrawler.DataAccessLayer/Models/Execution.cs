using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler.DataAccessLayer.Models
{
    public class Execution
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public ExecutionStatus ExecutionStatus { get; set; }
        public WebsiteRecord WebsiteRecord { get; set; }
        public int WebsiteRecordId { get; set; }
        public int? NumberOfSites { get; set; }
    }
}
