using Dawn;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Retry;
using System.Net;
using System.Text.RegularExpressions;
using WebCrawler.BusinessLayer.Services;
using WebCrawler.DataAccessLayer.Context;
using WebCrawler.DataAccessLayer.Models;
using WebsiteCrawler.Infrastructure.interfaces;
using WebsiteCrawler.Infrastructure.Storage;

namespace WebsiteCrawler.Service
{
    public class SingleThreadedWebSiteCrawler : IWebSiteCrawler
    {
        public const int RetryAttempts = 2;
        private readonly IHtmlParser htmlParser;
        private readonly HttpClient httpClient;
        private readonly Queue<Node> jobQueue;
        private readonly AppDbContext db;
        private readonly DateTime startTime;
        private ICrawlingNodeStorage crawlingNodeStorage;

        public StartingNode StartingNode { get; set; }

        public SingleThreadedWebSiteCrawler(IHtmlParser htmlParser, HttpClient httpClient, AppDbContext db)
        {
            this.startTime = DateTime.Now;

            this.htmlParser = htmlParser;
            this.httpClient = httpClient;
            this.db = db;

            jobQueue = new Queue<Node>();
            this.db = db;
        }

        public async Task<StartingNode> Run(WebsiteRecord record, int executionId, ICrawlingNodeStorage crawlingNodeStorage, int? maximumCountOfNodes = null)
        {
            this.crawlingNodeStorage = crawlingNodeStorage;

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Other");

            var node = new Node()
            {
                Url = record.URL,
                Domain = GetDomainFromUrl(record.URL),
                Children = new List<Node>(),
                WebsiteRecordId = record.Id,
                ExecutionId = executionId,
                Title = ""
            };

            StartingNode = new StartingNode()
            {
                Node = node
            };

            var url = new Uri(record.URL, UriKind.RelativeOrAbsolute);
            try
            {
                string? pageContents = await DownloadPage(url);
                node.Title = Regex.Match(pageContents, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase)?.Groups["Title"]?.Value ?? "";
            }
            catch (Exception ex) 
            {
                await crawlingNodeStorage.FinalizeCrawlingAsync(record.Id);
                StartingNode.NumberOfSites = await db.Nodes.CountAsync(x => x.ExecutionId == executionId);
                return StartingNode;
            }

            crawlingNodeStorage.CreateNewExecution(record.Id);
            await crawlingNodeStorage.AddOrUpdateNodeAsync(node, record.Id);

            jobQueue.Enqueue(node);

            while (jobQueue.Count > 0 && (maximumCountOfNodes != null ? (await db.Nodes.CountAsync(x => x.ExecutionId == executionId) < maximumCountOfNodes) : true))
            {
                await DiscoverLinks(string.IsNullOrEmpty(record.RegExp) ? null : new Regex(record.RegExp!), record.Id, executionId);
            }

            await crawlingNodeStorage.FinalizeCrawlingAsync(record.Id);
            StartingNode.NumberOfSites = await db.Nodes.CountAsync(x => x.ExecutionId == executionId);
            return StartingNode;
        }

        private static AsyncRetryPolicy<HttpResponseMessage> CreateExponentialBackoffPolicy()
        {
            var unAcceptableResponses = new HttpStatusCode[] { HttpStatusCode.GatewayTimeout, HttpStatusCode.GatewayTimeout };
            return Policy
                .HandleResult<HttpResponseMessage>(resp => unAcceptableResponses.Contains(resp.StatusCode))
                .WaitAndRetryAsync(
                RetryAttempts,
                attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));
        }

        private async Task DiscoverLinks(Regex? regex, int websiteRecordId, int executionId)
        {
            var parentNode = jobQueue.Dequeue();
            var parentUri = new Uri(parentNode.Url, UriKind.RelativeOrAbsolute);

            string? pageContents = "";

            try
            {
                pageContents = await DownloadPage(parentUri);
            }
            catch (Exception ex) 
            {
                return;
            }

            var links = FindLinksWithinHtml(pageContents);

            foreach (var rawLink in links)
            {
                Node? node = null;
                var link = rawLink.Trim().Trim('\n');
                Uri uri;
                if (link.StartsWith('/') || link.StartsWith('\\'))
                {
                    uri = new Uri(parentUri, link);
                }
                else
                {
                    uri = new Uri(link, UriKind.RelativeOrAbsolute);
                }
                link = uri.ToString();

                node = await crawlingNodeStorage.GetNodeOrDefaultAsync(websiteRecordId, link, executionId);

                bool nodeAlreadyVisited = true;

                if (node is null)
                {
                    nodeAlreadyVisited = false;

                    if (!uri.IsAbsoluteUri)
                    {
                        continue;
                    }

                    node = new Node
                    {
                        Url = link,
                        Domain = uri.Host,
                        Children = new List<Node>(),
                        WebsiteRecordId = websiteRecordId,
                        ExecutionId = executionId,
                        Title = ""
                    };
                }

                if (regex is not null)
                {
                    node.RegExpMatch = regex.IsMatch(link);
                }

                bool isLinkAcceptable = IsLinkAcceptable(link);

                if (!isLinkAcceptable)
                {
                    continue;
                }

                if (
                (uri.Host.ToLower() == parentUri.Host.ToLower()) &&
                (uri.PathAndQuery.ToLower() == parentUri.PathAndQuery.ToLower())
                )
                {
                    continue;
                }

                try
                {
                    if (!nodeAlreadyVisited)
                    {
                        try
                        {
                            pageContents = await DownloadPage(uri);
                            node.Title = Regex.Match(pageContents, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value ?? "";
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
                catch
                {
                    pageContents = null;
                }

                if (!parentNode.Children.Any(x => x == node))
                {
                    parentNode.Children.Add(node);

                    node.CrawlTime = GetCrawlTime();

                    await crawlingNodeStorage.AddOrUpdateNodeAsync(parentNode, websiteRecordId);
                    await crawlingNodeStorage.AddOrUpdateNodeAsync(node, websiteRecordId);

                    if (node.RegExpMatch != false && pageContents != null && !nodeAlreadyVisited)
                    {
                        jobQueue.Enqueue(node);
                    }
                }
            }
        }

        private async Task<string> DownloadPage(Uri uri)
        {
            if ((uri.Scheme.ToLower() != "http") && (uri.Scheme.ToLower() != "https"))
            {
                return null;
            }

            var retryPolicy = CreateExponentialBackoffPolicy();

            var htmlResponse = await retryPolicy
                .ExecuteAsync(() => httpClient.GetAsync(uri));

            if (!htmlResponse.IsSuccessStatusCode)
            {
                return null;
            }

            var ctype = htmlResponse.Content.Headers.ContentType;
            if (!ctype.MediaType.Contains("text/html"))
            {
                return null;
            }

            var htmlContent = await htmlResponse.Content.ReadAsStringAsync();
            return htmlContent;
        }

        private List<string> FindLinksWithinHtml(string htmlContent)
        {
            return htmlParser.GetLinks(htmlContent);
        }

        private bool IsLinkAcceptable(string link)
        {
            if (string.IsNullOrEmpty((string)link))
            {
                return false;
            }
            var frags = link.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (!frags.Any())
            {
                return false;
            }

            if (link.StartsWith("#"))
            {
                //Book marks are not wanted
                return false;
            }

            if (link.StartsWith("mailto:"))
            {
                //Email links are not wanted
                return false;
            }

            if (link.StartsWith("tel:"))
            {
                //Phone links are not wanted
                return false;
            }

            if (link.StartsWith("sms:"))
            {
                //sms links are not wanted
                return false;
            }

            return true;
        }

        private TimeSpan GetCrawlTime()
        {
            TimeSpan duration = DateTime.Now.Subtract(startTime);

            return duration;  
        }

        public static string GetDomainFromUrl(string url)
        {
            if (url.ToLower().StartsWith("http:") || url.ToLower().StartsWith("https:"))
            {
                var uri = new Uri(url);
                string domain = uri.Host;

                return domain;
            }

            return "";
        }
    }
}