using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler.DataAccessLayer.Models
{
    public class StartingNode
    {
        public int Id { get; set; }
        public int StartingNodeId { get; set; }
        public virtual WebsiteRecord WebsiteRecord { get; set; }
        public int WebsiteRecordId { get; set; }
    }
}
