using WebCrawler.DataAccessLayer.Context;
using WebCrawler.BusinessLayer.DataTransferObjects;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AutoMapper;
using WebCrawler.BusinessLayer.Services.Base;
using WebCrawler.DataAccessLayer.Models;
using System;
using WebCrawler.BusinessLayer.Options;

namespace WebCrawler.BusinessLayer.Services
{
    public class RecordsService : ServiceBase<Tag, TagDTO>
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;

        public RecordsService(AppDbContext db, IMapper mapper) : base(mapper)
        {
            this.db = db;
            this.mapper = mapper;
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
            var recordDtO = mapper.Map<WebsiteRecordDTO>(record);
            recordDtO.tagDTOs = EntitiesToDTOes(tags);
            return recordDtO;
        }
        public async Task<List<WebsiteRecordDTO>> GetAllRecords(SortOptions sortOptions = null, FilterOptions filterOptions = null)
        {
            var DTOes = new List<WebsiteRecordDTO>();
            var records = await db.Records
                .Include(x => x.Tags)
                .Select(x => new
                {
                    URL = x.URL,
                    Id = x.Id,
                    Hours = x.Hours,
                    Minutes = x.Minutes,
                    Days = x.Days,
                    Label = x.Label,
                    tags = x.Tags,
                    LastExecution = x.LastExecution,
                    ExecutionStatus = x.ExecutionStatus
                }).ToListAsync();
            if (filterOptions != null)
            {
                if (filterOptions.URLs.Count != 0)
                {
                    records = records.Where(x => filterOptions.URLs.Any(y => y == x.URL)).ToList();
                }
                if (filterOptions.Labels.Count != 0)
                {
                    records = records.Where(x => filterOptions.Labels.Any(y => y == x.Label)).ToList();
                }
                if (filterOptions.Tags.Count != 0)
                {
                    records = records.Where(x => filterOptions.Tags.Any(y => x.tags.Any(z => z.Content == y))).ToList();

                }
            }

            foreach (var record in records)
            {
                DTOes.Add(new WebsiteRecordDTO
                {
                    URL = record.URL,
                    Id = record.Id,
                    Hours = record.Hours,
                    Minutes = record.Minutes,
                    Days = record.Days,
                    Label = record.Label,
                    tagDTOs = EntitiesToDTOes(record.tags),
                    LastExecution = record.LastExecution,
                    ExecutionStatus = record.ExecutionStatus,
                    Periodicity = ComputePeriodicity(record.Minutes ?? 0, record.Hours ?? 0, record.Days ?? 0)
                }) ;
            }

            if (sortOptions != null)
            {
                if (sortOptions.AlphabeticalSorting)
                    if (sortOptions.Alphabetically)
                        DTOes = DTOes.OrderBy(x => x.URL).ToList();
                    else
                        DTOes = DTOes.OrderByDescending(x => x.URL).ToList();
                if (sortOptions.TimeSorting)
                    if (sortOptions.NewestToOldest)
                        DTOes = DTOes.OrderBy(x => x.LastExecution).ToList();
                    else
                        DTOes = DTOes.OrderByDescending(x => x.LastExecution).ToList();
            }
            return DTOes;
            
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

        public async Task DeleteWebsiteRecord(WebsiteRecordDTO record)
        {
            db.Tags.RemoveRange(db.Tags.Where(x => x.WebsiteRecordId == record.Id).ToList());
            db.Records.Remove(await db.Records.SingleAsync(x => x.Id == record.Id));
            await db.SaveChangesAsync();
        }

        public async Task UpdateWebsiteRecord(WebsiteRecordDTO record)
        {
            var recordInDb = await db.Records.SingleAsync(x => x.Id == record.Id);
            recordInDb.Minutes = record.Minutes;
            recordInDb.Hours = record.Hours;
            recordInDb.Days = record.Days;
            recordInDb.Active = record.Active;
            recordInDb.URL = record.URL;
            recordInDb.Label = record.Label;
            recordInDb.RegExp = record.RegExp;
            var tags = await db.Tags.Where(x => x.WebsiteRecordId == record.Id).ToListAsync();
            db.Tags.RemoveRange(tags);
            await db.SaveChangesAsync();
            recordInDb.Tags = TagDtoesToTags(record.tagDTOs);
            await db.SaveChangesAsync();
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
    }
}
