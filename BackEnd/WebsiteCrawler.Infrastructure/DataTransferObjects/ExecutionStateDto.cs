using WebCrawler.DataAccessLayer.Models;

namespace WebsiteCrawler.Infrastructure.DataTransferObjects;

public class ExecutionStateDto
{
    public List<Node> Nodes { get; set; }
    public long UpdateState { get; set; }

    public ExecutionStateDto()
    { 
        Nodes = new List<Node>();
    }
}
