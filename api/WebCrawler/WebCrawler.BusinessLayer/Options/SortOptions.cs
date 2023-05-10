using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler.BusinessLayer.Options
{
    public class SortOptions
    {
        public bool AlphabeticalSorting { get; set; }
        public bool Alphabetically { get; set; }
        public bool TimeSorting { get; set; }
        public bool NewestToOldest { get; set; }
    }

    public class FilterOptions
    { 
        public List<string> URLs { get; set; }
        public List<string> Tags { get; set; }
        public List<string> Labels { get; set; }
        public FilterOptions()
        {
            URLs = new List<string>();
            Tags = new List<string>();
            Labels = new List<string>();
        }
    }
}
