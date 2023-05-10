using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.DataAccessLayer.Models;

namespace WebCrawler.BusinessLayer.DataTransferObjects
{
    public class CrawlerPostDTO
    {
        public int Id { get; set; }
        public string URL { get; set; }
        public string RegExp { get; set; }
    }

    public class CrawlerRecordedDataDTO
    { 
        public int Id { get; set; }
        public Node StartingNode { get; set; }
        public List<Node> Nodes { get; set; }
    }
}
