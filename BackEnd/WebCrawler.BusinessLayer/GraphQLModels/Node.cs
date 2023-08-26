using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler.BusinessLayer.GraphQLModels
{
    public class Node
    {
        public string? Title { get; set; }
        public string Url { get; set; }
        public string? CrawlTime { get; set; }
        public List<Node> Links { get; set; }
        public WebPage Owner { get; set; }
    }
}
