namespace WebCrawler.DataAccessLayer.Models
{
    public class Node
    {
        public int Id { get; set; } 
        public string Domain { get; set; }
        public int Group { get; set; }
        public DateTime DateTime { get; set; }
        public virtual ICollection<Node> Children { get; set; }
        public int WebsiteRecordId { get; set; }
    }
}
