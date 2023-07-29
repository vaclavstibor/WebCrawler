using WebCrawler.DataAccessLayer.Context;
using Newtonsoft.Json;
using WebCrawler.DataAccessLayer.Models;
using WebCrawler.BusinessLayer.DataTransferObjects;
using Microsoft.EntityFrameworkCore;
using WebCrawler.DataAccessLayer.Migrations;
using System.Security.Cryptography.X509Certificates;

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

        public class NodeDto
        { 
            public int Id { get; set; }
            public List<NodeDto> Children { get; set; }
        }

        public async Task<List<NodeDto>> GetAllNodesMinimal(int websiteRecordId)
        {
            return await db.Nodes.Where(x => x.WebsiteRecordId == websiteRecordId).Include(x => x.Children).Select(x => new NodeDto()
            {
                Id = x.Id,
                Children = x.Children.Select(y => 
                new NodeDto()
                { 
                    Id = y.Id
                }).ToList()
            }).ToListAsync();
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
