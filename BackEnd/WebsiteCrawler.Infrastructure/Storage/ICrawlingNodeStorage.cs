using WebCrawler.DataAccessLayer.Models;
using WebsiteCrawler.Infrastructure.DataTransferObjects;

namespace WebsiteCrawler.Infrastructure.Storage;
public interface ICrawlingNodeStorage
{
    Task AddOrUpdateNodeAsync(Node node, int websiteRecordId);
    Task<ExecutionStateDto> GetNodesAsync(int websiteRecordId, long lastUpdateState, int executionId);
    Task FinalizeCrawlingAsync(int websiteRecordId);
    Task<Node?> GetNodeOrDefaultAsync(int websiteRecordId, string url, int executionId);
    Task RemoveNodeAsync(Node node, int websiteRecordId);
    void CreateNewExecution(int websiteRecordId);
}
