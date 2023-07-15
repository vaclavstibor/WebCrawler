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
        public virtual WebsiteRecord WebsiteRecord { get; set; }
        public Node Node { get; set; }
        public int WebsiteRecordId { get; set; }
        public int NumberOfSites { get; set; }
    }
}
