using WebCrawler.BusinessLayer.GraphQLModels;
using WebCrawler.DataAccessLayer.Models;

namespace WebCrawler.BusinessLayer.Mappings
{
    public static class RecordToWebsite
    {
        public static WebPage ToWebPage(this WebsiteRecord? record) =>
            new WebPage
            {
                Identifier = record?.Id.ToString() ?? "",
                Tags = record?.Tags?.Select(x => x.Content).ToList() ?? new List<string>(),
                Url = record?.URL ?? "",
                Active = record?.Active ?? false,
                Label = record?.Label ?? "",
                Regexp = record?.RegExp ?? ""
            };
    }
}
