using WebCrawler.DataAccessLayer.Context;
using WebCrawler.DataAccessLayer.Models;
using WebCrawler.BusinessLayer.DataTransferObjects;
using Microsoft.EntityFrameworkCore;
using WebCrawler.BusinessLayer.Mappings;
using WebCrawler.BusinessLayer.GraphQLModels;
using WebsiteCrawler.Infrastructure.Storage;

namespace WebCrawler.BusinessLayer.Services
{
    public class CrawlerService
    {
        private readonly AppDbContext db;
        private readonly ICrawlingNodeStorage crawlingNodeStorage;

        public CrawlerService(AppDbContext db, ICrawlingNodeStorage crawlingNodeStorage)
        {
            this.db = db;
            this.crawlingNodeStorage = crawlingNodeStorage;
        }

        private WebPage GetWebPage(int id)
        {
            var record = db.Records.SingleOrDefault(x => x.Id == id);

            return new WebPage()
            {
                Identifier = record?.Id.ToString() ?? "",
                Label = record?.Label ?? "",
                Url = record?.URL ?? "",
                Regexp = record?.RegExp ?? "",
                Tags = record?.Tags?.Select(y => y.Content).ToList() ?? new List<string>(),
                Active = record?.Active ?? false
            };
        }

        public async Task<List<GraphQLModels.Node>> GetALlNodes(List<string> ids)
        {
            var iDS = new List<int>();

            foreach (var id in ids)
            {
                if (int.TryParse(id, out var i))
                {
                    iDS.Add(i);
                }
            }

            return await db.Nodes.Where(x => iDS.Contains(x.WebsiteRecordId))
                .Include(x => x.Children)
                .Select(x => new GraphQLModels.Node
                {
                    Title = x.Title,
                    Url = x.Url,
                    CrawlTime = x.CrawlTime.ToString(),
                    Links = x.Children.Select(y => new GraphQLModels.Node
                    {
                        Title = y.Title,
                        Url = y.Url,
                        CrawlTime = y.CrawlTime.ToString(),
                        Links = new List<GraphQLModels.Node>(),
                        Owner = db.Records.SingleOrDefault(z => z.Id == x.WebsiteRecordId).ToWebPage()

                    }).ToList(),
                    Owner = db.Records.SingleOrDefault(z => z.Id == x.WebsiteRecordId).ToWebPage()
                })
                .ToListAsync();
        }

        public async Task<List<NodeDto>> GetAllNodes(int websiteRecordId)
        {
            var liveExecution = await db.Executions.SingleOrDefaultAsync(x => x.WebsiteRecordId == websiteRecordId);

            if (liveExecution != null)
            {
                return await db.Nodes.Where(x => x.WebsiteRecordId == websiteRecordId && x.ExecutionId != liveExecution.Id)
                .Include(x => x.Children)
                .Select(x => x.MapToDto())
                .ToListAsync();
            }

            return await db.Nodes.Where(x => x.WebsiteRecordId == websiteRecordId)
                .Include(x => x.Children)
                .Select(x => x.MapToDto())
                .ToListAsync();
        }

        public async Task<UpdateStateDto> GetNewNodesLive(int websiteRecordId, long updateState)
        {
            try
            {
                var execution = await db.Executions.SingleOrDefaultAsync(x => x.ExecutionStatus == ExecutionStatus.Executing);
                var result = new UpdateStateDto();

                if (execution != null)
                {
                    var executionState = (await crawlingNodeStorage.GetNodesAsync(websiteRecordId, updateState, execution.Id));

                    result.Nodes = executionState.Nodes
                        .Select(x => x.MapToDto())
                        .ToList();

                    result.UpdateState = executionState.UpdateState;

                    return result;
                }

                result.UpdateState = 0;
                result.Nodes = new List<NodeDto>();

                return result;
            }
            catch
            { 
                
            }
            return null;
        }

        public async Task<List<ExecutionDto>> GetAllExecutions()
        {
            return (await db.Executions
                .Include(x => x.WebsiteRecord)
                .ToListAsync())
                .OrderByDescending(x => x.StartTime)
                .Select(x => new ExecutionDto()
                {
                    WebsiteRecordId = x.WebsiteRecordId,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    ExecutionStatus = x.ExecutionStatus.EnumToString(),
                    NumberOfSitesCrawled = x.NumberOfSites,
                    WebsiteRecordLabel = x.WebsiteRecord.Label
                })
                .ToList();
        }

        public async Task StartExecution(int websiteRecordId)
        {
            if (await db.Executions.AnyAsync(x => x.WebsiteRecordId == websiteRecordId))
            {
                return;
            }

            var newExecution = new Execution()
            {
                WebsiteRecordId = websiteRecordId,
                ExecutionStatus = ExecutionStatus.Created,
            };

            var record = db.Records.SingleOrDefault(x => x.Id == websiteRecordId);

            db.Executions.Add(newExecution);
            db.Records.Update(record);

            await db.SaveChangesAsync();
        }
    }
}
