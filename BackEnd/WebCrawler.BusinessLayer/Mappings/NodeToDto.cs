using Microsoft.EntityFrameworkCore.Query;
using System.Security.Policy;
using WebCrawler.BusinessLayer.DataTransferObjects;
using WebCrawler.DataAccessLayer.Migrations;
using WebCrawler.DataAccessLayer.Models;

namespace WebCrawler.BusinessLayer.Mappings;

public static class NodeToDtoMapping
{
    public static NodeDto MapToDto(this Node x) =>
        new NodeDto
        {
            Id = x.Id,
            Url = x.Url,
            Domain = x.Domain,
            CrawlTime = x.CrawlTime.ToString(@"hh\:mm\:ss"),
            Title = x.Title,
            RegExpMatch = x.RegExpMatch,
            Children = x.Children.Select(y => new NodeDto()
            {
                Id = y.Id,
                Url = y.Url,
                Domain = y.Domain,
                CrawlTime = y.CrawlTime.ToString(@"hh\:mm\:ss"),
                RegExpMatch = y.RegExpMatch,
                ExecutionId = y.ExecutionId,
                Title = y.Title,
                Children = new List<NodeDto>()
            }).ToList(),
            WebsiteRecordId = x.WebsiteRecordId,
            ExecutionId = x.ExecutionId
        };
}