namespace WebCrawler.DataAccessLayer.Models
{
    public class Node
    {
        public int Id { get; set; } 
        public string Url { get; set; }
        public string Domain { get; set; }
        public TimeSpan CrawlTime { get; set; }
        public bool? RegExpMatch { get; set; } // none RegExp filled -> null
        public virtual ICollection<Node>? Children { get; set; }
    }
}
