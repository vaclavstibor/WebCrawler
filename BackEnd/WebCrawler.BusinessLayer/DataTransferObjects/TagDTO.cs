using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler.BusinessLayer.DataTransferObjects
{
    public class TagDTO
    {
        public string Content { get; set; }
        public int Id { get; set; }
        public int WebsiteRecordId { get; set; }
    }
}
