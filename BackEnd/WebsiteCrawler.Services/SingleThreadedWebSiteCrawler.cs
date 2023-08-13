using Dawn;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Retry;
using System.Net;
using System.Text.RegularExpressions;
using WebCrawler.DataAccessLayer.Context;
using WebCrawler.DataAccessLayer.Models;
using WebsiteCrawler.Infrastructure.interfaces;

namespace WebsiteCrawler.Service
{
    public class SingleThreadedWebSiteCrawler : IWebSiteCrawler
    {
        public const int RetryAttempts = 2;
        private readonly IHtmlParser htmlParser;
        private readonly HttpClient httpClient;
        private readonly Queue<Node> jobQueue;
        private readonly HashSet<Node> nodes;
        private readonly AppDbContext db;   

        public StartingNode StartingNode { get; set; }

        public SingleThreadedWebSiteCrawler(IHtmlParser htmlParser, HttpClient httpClient, AppDbContext db)
        {
            this.htmlParser = htmlParser;
            this.httpClient = httpClient;
            this.db = db;

            jobQueue = new Queue<Node>();
            nodes = new HashSet<Node>();
        }

        public async Task<StartingNode> Run(WebsiteRecord record, int executionId, int? maximumCountOfNodes = null)
        {
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Other");

            var node = new Node()
            {
                Url = record.URL,
                Domain = GetDomainFromUrl(record.URL),
                Children = new List<Node>(),
                WebsiteRecordId = record.Id,
                ExecutionId = executionId
            };

            await db.Nodes.AddAsync(node);
            await db.SaveChangesAsync();

            nodes.Add(node);

            StartingNode = new StartingNode()
            {
                Node = node
            };

            jobQueue.Enqueue(node);

            while (jobQueue.Count > 0 && (maximumCountOfNodes != null ? (nodes.Count < maximumCountOfNodes) : true))
            {
                await DiscoverLinks(DateTime.Now, new Regex(record.RegExp ?? ""), record.Id, executionId);
            }

            StartingNode.NumberOfSites = nodes.Count;
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

        private async Task DiscoverLinks(DateTime startCrawlTime, Regex? regex, int websiteRecordId, int executionId)
        {
            var parentNode = jobQueue.Dequeue();
            var parentUri = new Uri(parentNode.Url, UriKind.RelativeOrAbsolute);

            string? pageContents = await DownloadPage(parentUri);

            if (pageContents == null)
            {
                return;
            }
            var links = FindLinksWithinHtml(pageContents);

            var currentNewNodes = new List<Node>();

            foreach (var rawLink in links)
            {
                Node? node = null;
                try
                {
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

                    node = nodes.SingleOrDefault(x => x.Url == link);

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
                            ExecutionId = executionId
                        };

                        nodes.Add(node);
                        currentNewNodes.Add(node);
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
                            pageContents = await DownloadPage(uri);
                        }
                    }
                    catch
                    {
                        pageContents = null;
                    }

                    if (!parentNode.Children.Any(x => x == node))
                    {
                        parentNode.Children.Add(node);

                        if (node.RegExpMatch != false && pageContents != null && !nodeAlreadyVisited)
                        {
                            jobQueue.Enqueue(node);
                        }
                    }
                }
                catch
                {
                    currentNewNodes.Remove(node);
                    nodes.Remove(node);
                }
            }

            await db.Nodes.AddRangeAsync(currentNewNodes);
            await db.SaveChangesAsync();
            parentNode.CrawlTime = GetCrawlTime(startCrawlTime);
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
                //throw new NotImplementedException("How do we handle errors? Think"); //TODO handle non-sucess response, Polly retry
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

        private TimeSpan GetCrawlTime(DateTime startTime)
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