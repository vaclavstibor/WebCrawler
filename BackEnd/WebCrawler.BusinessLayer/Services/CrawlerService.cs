using WebCrawler.DataAccessLayer.Context;
using WebCrawler.DataAccessLayer.Models;
using WebCrawler.BusinessLayer.DataTransferObjects;
using Microsoft.EntityFrameworkCore;
using WebCrawler.DataAccessLayer.Cache;
using WebCrawler.BusinessLayer.Mappings;
using WebCrawler.BusinessLayer.GraphQLModels;
using System.Text.RegularExpressions;
using System;

namespace WebCrawler.BusinessLayer.Services
{
    public class CrawlerService
    {
        private readonly AppDbContext db;
        public CrawlerService(AppDbContext db)
        { 
            this.db = db;
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
                    Title = x.Domain,
                    Url = x.Url,
                    CrawlTime = x.CrawlTime.ToString(),
                    Links = x.Children.Select(y => new GraphQLModels.Node
                    {
                        Title = y.Domain,
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
            return await db.Nodes.Where(x => x.WebsiteRecordId == websiteRecordId)
                .Include(x => x.Children)
                .Select(x => x.MapToDto())
                .ToListAsync();
        }

        public List<NodeDto> GetNewNodesLive(int websiteRecordId)
        {
            return CrawlingCache.GetNodes(websiteRecordId)
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
