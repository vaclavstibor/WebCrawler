using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using WebCrawler.DataAccessLayer.Models;
using WebCrawler.BusinessLayer.DataTransferObjects;

namespace WebCrawler.BusinessLayer.Mappings
{
    public class WebsiteRecordMapping : Profile
    {
        public WebsiteRecordMapping()
        {
            CreateMap<WebsiteRecord, WebsiteRecordDTO>()
                .ForMember(d => d.Minutes, a => a.MapFrom(s => s.Minutes))
                .ForMember(d => d.Hours, a => a.MapFrom(s => s.Hours))
                .ForMember(d => d.Days, a => a.MapFrom(s => s.Days))
                .ForMember(d => d.Active, a => a.MapFrom(s => s.Active))
                .ForMember(d => d.Label, a => a.MapFrom(s => s.Label))
                .ForMember(d => d.URL, a => a.MapFrom(s => s.URL))
                .ForMember(d => d.RegExp, a => a.MapFrom(s => s.RegExp))
                .ForMember(d => d.Id, a => a.MapFrom(s => s.Id))
                .ForMember(d => d.LastExecution, a => a.MapFrom(s => s.LastExecution))
                .ForMember(d => d.ExecutionStatus, a => a.MapFrom(s => s.ExecutionStatus));
        }
    }
}
