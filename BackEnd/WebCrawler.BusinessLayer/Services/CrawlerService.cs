using WebCrawler.DataAccessLayer.Context;
using Newtonsoft.Json;
using WebCrawler.DataAccessLayer.Models;
using WebCrawler.BusinessLayer.DataTransferObjects;
using Microsoft.EntityFrameworkCore;
using WebCrawler.DataAccessLayer.Migrations;

namespace WebCrawler.BusinessLayer.Services
{
    public class CrawlerService
    {
        private readonly AppDbContext db;
        public CrawlerService(AppDbContext db)
        { 
            this.db = db;
        }

        public async Task<List<Node>> GetAllNodes(int websiteRecordId)
        {
            return await db.Nodes.Where(x => x.WebsiteRecordId == websiteRecordId).Include(x => x.Children).ToListAsync();
        }

        public async Task<List<ExecutionDto>> GetAllExecutions()
        {
            return (await db.Executions
                .Include(x => x.WebsiteRecord)
                .ToListAsync())
                .OrderByDescending(x => x.StartTime)
                .Select(x => new ExecutionDto()
                {
                    WebsiteRecordId = x.Id,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    ExecutionStatus = x.ExecutionStatus,
                    NumberOfSitesCrawled = x.NumberOfSites,
                    WebsiteRecordLabel = x.WebsiteRecord.Label
                })
                .ToList();
        }   

        public async Task StartExecution(int websiteRecordId)
        {
            var newExecution = new Execution()
            { 
                WebsiteRecordId = websiteRecordId,
                ExecutionStatus = ExecutionStatus.Created,
            };

            db.Executions.Add(newExecution);

            await db.SaveChangesAsync();
        }
    }
}
