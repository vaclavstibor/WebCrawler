using System;
using System.Collections.Generic;

namespace WebsiteCrawler.Infrastructure.entity
{
	public class CrawlingNode
	{
		public CrawlingNode()
		{
		}

		public int Id { get; set; }

		public string Url { get; set; }

		public string Domain { get; set; }

		public DateTime CrawlTime { get; set; }

		public bool? RegExpMatch { get; set; }

		public List<int> Children { get; set; }

        public override string ToString()
        {
            return $"Id={this.Id}" +
				   $"Url={this.Url}" +
				   $"Domain={this.Domain}" +
				   $"CrawlTime={this.CrawlTime}" +
				   $"RegExpMatch={this.RegExpMatch}" +
				   $"Children={this.Children}";
        }
    }
}
