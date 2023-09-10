using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler.BusinessLayer.DataTransferObjects
{
    public class UpdateStateDto
    {
        public List<NodeDto> Nodes { get; set; }    
        public long UpdateState { get; set; }
    }
}
