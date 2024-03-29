﻿using WebCrawler.DataAccessLayer.Context;
using WebCrawler.BusinessLayer.DataTransferObjects;
using Microsoft.EntityFrameworkCore;
using WebCrawler.DataAccessLayer.Models;
using WebCrawler.BusinessLayer.GraphQLModels;

namespace WebCrawler.BusinessLayer.Services
{
    public class RecordsService
    {
        private readonly AppDbContext db;
        private readonly CrawlerService crawlerService;

        public RecordsService(AppDbContext db, CrawlerService crawlerService)
        {
            this.db = db;
            this.crawlerService = crawlerService;
        }

        public string ComputePeriodicity(int minutes, int hours, int days)
        {
            string periodicity = "";
            if (days != 0)
                periodicity += days;
            else
                periodicity += 0;
            if (hours != 0)
                periodicity += ":" + hours;
            else
                periodicity += ":" + 0;
            if (minutes != 0)
                periodicity += ":" + minutes;
            else
                periodicity += ":" + 0;
            if (periodicity == "0:0:0")
            {
                return "";
            }
            else
                return periodicity;
        }

        public async Task<WebsiteRecordDTO> GetARecord(int recordId)
        {
            var record = await db.Records
                .SingleAsync(x => x.Id == recordId);
            
            var tags = await db.Tags.Where(x => x.WebsiteRecordId == recordId).ToListAsync();
            
            var recordDtO = new WebsiteRecordDTO()
            {
                Id = recordId,
                URL = record.URL,
                RegExp = record.RegExp,
                Hours = record.Hours,
                Minutes = record.Minutes,
                Days = record.Days,
                Label = record.Label,
                Active = record.Active,
                LastExecution = record.LastExecution,
                ExecutionStatus = record.ExecutionStatus.EnumToString(),
            };

            recordDtO.tagDTOs = tags.Select(x => new TagDTO()
            {
                Id = x.Id,
                Content = x.Content
            }).ToList();

            return recordDtO;
        }

        public async Task<List<WebPage>> GetWebPages()
        {
            return await db.Records
                .Include(x => x.Tags)
                .Select(x => new WebPage()
                {
                    Identifier = x.Id.ToString(),
                    Label = x.Label,
                    Url = x.URL,
                    Regexp = x.RegExp,
                    Tags = x.Tags.Select(y => y.Content).ToList(),
                    Active = x.Active
                })
                .ToListAsync();
        }

        public WebPage GetWebPage(int id)
        {
            var record = db.Records.SingleOrDefault(x => x.Id == id);

            return new WebPage()
            {
                Identifier = record.Id.ToString(),
                Label = record.Label,
                Url = record.URL,
                Regexp = record.RegExp,
                Tags = record.Tags.Select(y => y.Content).ToList(),
                Active = record.Active
            };
        }

        public async Task<List<WebsiteRecordDTO>> GetAllRecords()
        {
            return await db.Records
            .Include(x => x.Tags)
            .Select(x => new WebsiteRecordDTO
            {
                URL = x.URL,
                Id = x.Id,
                Hours = x.Hours,
                Minutes = x.Minutes,
                Days = x.Days,
                Label = x.Label,
                tagDTOs = x.Tags.Select(x => new TagDTO
                {
                    Id = x.Id,
                    WebsiteRecordId = x.WebsiteRecordId,
                    Content = x.Content
                }).ToList(),
                LastExecution = x.LastExecution,
                ExecutionStatus = x.ExecutionStatus.EnumToString(),
            }).ToListAsync();
        }

        public List<Tag> TagDtoesToTags(List<TagDTO> tagDTOes)
        {
            var tags = new List<Tag>();
            foreach (var tag in tagDTOes)
            {
                tags.Add(new Tag
                {
                    Content = tag.Content
                });
            }
            return tags;
                
        }

        public async Task<bool> DeleteWebsiteRecord(int recordId)
        {
            if (!await db.Records.AnyAsync(x => x.Id == recordId))
                return false;

            db.Tags.RemoveRange(db.Tags.Where(x => x.WebsiteRecordId == recordId));
            db.Records.Remove(await db.Records.SingleAsync(x => x.Id == recordId));

            var executions = db.Executions.Where(x => x.WebsiteRecordId == recordId);
            var nodes = db.Nodes.Where(x => x.WebsiteRecordId == recordId);

            db.Executions.RemoveRange(executions);
            db.Nodes.RemoveRange(nodes);

            await db.SaveChangesAsync();
            return true;
        }

        public async Task<WebsiteRecordDTO> UpdateWebsiteRecord(WebsiteRecordDTO record)
        {
            var recordInDb = await db.Records.SingleOrDefaultAsync(x => x.Id == record.Id);

            if (recordInDb != null)
            {
                recordInDb.Minutes = record.Minutes ?? 0;
                recordInDb.Hours = record.Hours ?? 0;
                recordInDb.Days = record.Days ?? 0;
                recordInDb.Active = record.Active ?? false;
                recordInDb.URL = record.URL;
                recordInDb.Label = record.Label;
                recordInDb.RegExp = record.RegExp;
                recordInDb.ExecutionStatus = record.ExecutionStatus.StringToEnum();

                var tags = await db.Tags.Where(x => x.WebsiteRecordId == record.Id).ToListAsync();
                db.Tags.RemoveRange(tags);

                await db.SaveChangesAsync();

                recordInDb.Tags = TagDtoesToTags(record.tagDTOs);

                await db.SaveChangesAsync();
            }
            else
            {
                recordInDb = new WebsiteRecord
                {
                    Minutes = record.Minutes,
                    Hours = record.Hours,
                    Days = record.Days,
                    Active = record.Active ?? false,
                    URL = record.URL,
                    Label = record.Label,
                    RegExp = record.RegExp,
                    Tags = record.tagDTOs.Select(x => new Tag
                    {
                        Content = x.Content,
                    }).ToList(),
                    ExecutionStatus = ExecutionStatus.Created
                };

                await db.AddAsync(recordInDb);
                await db.SaveChangesAsync();
                await crawlerService.StartExecution(recordInDb.Id);
            }

            return new WebsiteRecordDTO()
            {
                Id = recordInDb.Id,
                URL = recordInDb.URL,
                RegExp = recordInDb.RegExp,
                Hours = recordInDb.Hours,
                Minutes = recordInDb.Minutes,
                Days = recordInDb.Days,
                Label = recordInDb.Label,
                Active = recordInDb.Active,
                LastExecution = recordInDb.LastExecution,
                ExecutionStatus = recordInDb.ExecutionStatus.EnumToString(),
            };
        }

        public async Task AddNewTag(TagDTO tag)
        {
            await db.Tags.AddAsync(new Tag
            {
                Content = tag.Content,
                WebsiteRecordId = tag.WebsiteRecordId
            });
            await db.SaveChangesAsync();
        }

        public async Task DeleteTag(TagDTO tag)
        {
            var tagInDb = await db.Tags.FirstOrDefaultAsync(x => x.Id == tag.Id);
            if (tagInDb == null)
                return;
            db.Tags.Remove(tagInDb);
            await db.SaveChangesAsync();    
        }
    }
}
