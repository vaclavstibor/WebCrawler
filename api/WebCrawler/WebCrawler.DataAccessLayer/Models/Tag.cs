

namespace WebCrawler.DataAccessLayer.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public int WebsiteRecordId { get; set; }
        public string Content { get; set; }
        public virtual WebsiteRecord WebsiteRecord { get; set; }
    }
}
