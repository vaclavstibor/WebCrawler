﻿using WebCrawler.DataAccessLayer.Context;
using WebCrawler.BusinessLayer.DataTransferObjects;
using Microsoft.EntityFrameworkCore;
using WebCrawler.DataAccessLayer.Models;
using WebCrawler.BusinessLayer.Options;

namespace WebCrawler.BusinessLayer.Services
{
    public class RecordsService
    {
        private readonly AppDbContext db;

        public RecordsService(AppDbContext db)
        {
            this.db = db;
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
                ExecutionStatus = record.ExecutionStatus,
            };
            recordDtO.tagDTOs = tags.Select(x => new TagDTO()
            {
                Id = x.Id,
                Content = x.Content
            }).ToList();
            return recordDtO;
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
                ExecutionStatus = x.ExecutionStatus
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

        public async Task AddWebsiteRecord(WebsiteRecordDTO record, List<TagDTO> tags)
        {
            record.Hours += record.Minutes / 60;
            record.Minutes = record.Minutes % 60;

            record.Days += record.Hours / 24;
            record.Hours = record.Hours % 24;
            
            var newRecord = new WebsiteRecord
            {
                Hours = record.Hours,
                Days = record.Days,
                Label = record.Label,
                URL = record.URL,
                Active = record.Active,
                Minutes = record.Minutes,
                Tags = TagDtoesToTags(tags),
                RegExp = record.RegExp,
                ExecutionStatus = record.ExecutionStatus,
                LastExecution = null
            };
            await db.Records.AddAsync(newRecord);
            await db.SaveChangesAsync();
        }

        public async Task<bool> DeleteWebsiteRecord(int recordId)
        {
            if (!await db.Records.AnyAsync(x => x.Id == recordId))
                return false;

            db.Tags.RemoveRange(db.Tags.Where(x => x.WebsiteRecordId == recordId).ToList());
            db.Records.Remove(await db.Records.SingleAsync(x => x.Id == recordId));
            await db.SaveChangesAsync();
            return true;
        }

        public async Task UpdateWebsiteRecord(WebsiteRecordDTO record)
        {
            var recordInDb = await db.Records.SingleOrDefaultAsync(x => x.Id == record.Id);
            recordInDb.Minutes = record.Minutes;
            recordInDb.Hours = record.Hours;
            recordInDb.Days = record.Days;
            recordInDb.Active = record.Active;
            recordInDb.URL = record.URL;
            recordInDb.Label = record.Label;
            recordInDb.RegExp = record.RegExp;

            if (recordInDb != null)
            {
                
                var tags = await db.Tags.Where(x => x.WebsiteRecordId == record.Id).ToListAsync();
                db.Tags.RemoveRange(tags);

                await db.SaveChangesAsync();

                recordInDb.Tags = TagDtoesToTags(record.tagDTOs);
                
                await db.SaveChangesAsync();
            }
            else
            { 
                
            }
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