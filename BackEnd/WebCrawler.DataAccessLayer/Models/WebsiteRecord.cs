
namespace WebCrawler.DataAccessLayer.Models
{

    public enum ExecutionStatus
    { 
        Created,
        Executing,
        Executed
    }

    public class WebsiteRecord
    {
        public int Id { get; set; }
        public string URL { get; set; }
        public string? RegExp { get; set; }
        public int? Hours { get; set; }
        public int? Minutes { get; set; }
        public int? Days { get; set; }
        public string? Label { get; set; }
        public bool Active { get; set; }
        public DateTime? LastExecution { get; set; }
        public ExecutionStatus? ExecutionStatus { get; set; } 
        public virtual List<Tag> Tags { get; set; }
    }
}
