using WebCrawler.DataAccessLayer.Models;
using WebCrawler.BusinessLayer.DataTransferObjects;
using AutoMapper;
namespace WebCrawler.BusinessLayer.Mappings
{
    public class TagMapping : Profile
    {
        public TagMapping()
        {
            CreateMap<Tag, TagDTO>()
                .ForMember(d => d.Id, a => a.MapFrom(s => s.Id))
                .ForMember(d => d.Content, a => a.MapFrom(s => s.Content))
                .ForMember(d => d.WebsiteRecordId, a => a.MapFrom(s => s.WebsiteRecordId))
                .ReverseMap();
        }
    }
}
