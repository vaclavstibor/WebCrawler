using WebCrawler.DataAccessLayer.Context;
using WebCrawler.DataAccessLayer.Models;
using WebCrawler.BusinessLayer.DataTransferObjects;
using Microsoft.EntityFrameworkCore;
using WebCrawler.DataAccessLayer.Cache;
using WebCrawler.BusinessLayer.Mappings;

namespace WebCrawler.BusinessLayer.Services
{
    public class CrawlerService
    {
        private readonly AppDbContext db;
        public CrawlerService(AppDbContext db)
        { 
            this.db = db;
        }

        public async Task<List<NodeDto>> GetAllNodes(int websiteRecordId)
        {
            return await db.Nodes.Where(x => x.WebsiteRecordId == websiteRecordId)
                .Include(x => x.Children)
                .Select(x => x.MapToDto())
                .ToListAsync();
        }

        public List<NodeDto> GetNewNodesLive(int websiteRecordId)
        {
            return CrawlingCache.GetAndDeleteCachedNodes(websiteRecordId)
                .Select(x => x.MapToDto())
                .ToList();
        }

        public List<NodeDto> GetAllNodesLive(int websiteRecordId)
        {
            return CrawlingCache.GetAllNodes(websiteRecordId)
                .Select(x => x.MapToDto())
                .ToList();
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
