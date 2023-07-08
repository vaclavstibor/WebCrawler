using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.DataAccessLayer.Context;
using Newtonsoft.Json;
using WebCrawler.DataAccessLayer.Models;
using WebCrawler.BusinessLayer.DataTransferObjects;
using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;

namespace WebCrawler.BusinessLayer.Services
{
    public class CrawlerService
    {
        private readonly AppDbContext db;
        private readonly HttpClient httpClient;
        public CrawlerService(AppDbContext db, HttpClient httpclient)
        { 
            this.db = db;
            this.httpClient = httpclient;
        }

        public async Task PostNewCrawling(WebsiteRecordDTO record)
        {
            using (var client = new HttpClient())
            {
                var url = "";
                var content = new CrawlerPostDTO
                {
                    Id = record.Id.Value,
                    URL = url,
                    RegExp = record.RegExp
                };
                string contentInJson = JsonConvert.SerializeObject(content);
                var httpContent = new StringContent(contentInJson);
                HttpResponseMessage response = await client.PostAsync(url,httpContent);
                response.EnsureSuccessStatusCode();
            }
        }

        public async Task PostCrawledData(WebsiteRecordDTO record)
        {
            if (db.StartingNodes.Any(x => x.WebsiteRecordId == record.Id))
            {
                using (var client = new HttpClient())
                {
                    var url = "";
                    var content = await db.StartingNodes.Where(x => x.WebsiteRecordId == record.Id).ToListAsync();
                    string contentInJson = JsonConvert.SerializeObject(content);
                    var httpContent = new StringContent(contentInJson);
                    HttpResponseMessage response = await client.PostAsync(url, httpContent);
                    response.EnsureSuccessStatusCode();
                }
            }
        }

        public async Task<List<ExecutionDto>> GetAllExecutions()
        {
            return await db.Executions
                .Include(x => x.WebsiteRecord)
                .Select(x => new ExecutionDto()
                { 
                    WebsiteRecordId = x.Id,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    ExecutionStatus = x.ExecutionStatus,
                    NumberOfSitesCrawled = x.NumberOfSites,
                    WebsiteRecordLabel = x.WebsiteRecord.Label
                })
                .OrderByDescending(x => x.StartTime)
                .ToListAsync();
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
