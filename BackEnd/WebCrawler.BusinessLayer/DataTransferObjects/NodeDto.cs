using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.DataAccessLayer.Models;

namespace WebCrawler.BusinessLayer.DataTransferObjects
{
    public class NodeDto
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Domain { get; set; }
        public string CrawlTime { get; set; }
        public bool? RegExpMatch { get; set; } // none RegExp filled -> null
        public virtual ICollection<NodeDto>? Children { get; set; }
        public int WebsiteRecordId { get; set; }
        public int ExecutionId { get; set; }
        public string Title { get; set; }
    }
}
