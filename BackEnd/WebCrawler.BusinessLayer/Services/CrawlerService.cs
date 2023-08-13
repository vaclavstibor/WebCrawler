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

        public async Task<List<NodeDto>> GetAllNodes(int websiteRecordId)
        {
            return await db.Nodes.Where(x => x.WebsiteRecordId == websiteRecordId)
                .Include(x => x.Children)
                .Select(x => new NodeDto
                { 
                    Id = x.Id,
                    Url = x.Url,
                    Domain = x.Domain,
                    CrawlTime = x.CrawlTime,
                    RegExpMatch = x.RegExpMatch,
                    Children = x.Children.Select(y => new NodeDto()
                    { 
                        Id = y.Id,
                        Url = y.Url,
                        Domain = y.Domain,
                        CrawlTime = y.CrawlTime,
                        RegExpMatch = y.RegExpMatch,
                        WebsiteRecordId = websiteRecordId,
                        ExecutionId = y.ExecutionId,
                        Children = new List<NodeDto>()
                    }).ToList(),
                    WebsiteRecordId = x.WebsiteRecordId,
                    ExecutionId = x.ExecutionId
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
