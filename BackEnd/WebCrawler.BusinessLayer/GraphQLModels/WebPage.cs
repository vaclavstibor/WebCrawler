using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler.BusinessLayer.GraphQLModels
{
    public class WebPage
    {
        public string Identifier { get; set; }
        public string Label { get; set; }
        public string Url { get; set; }
        public string Regexp { get; set; }
        public List<string> Tags { get; set; }
        public bool Active { get; set; }
    }
}
