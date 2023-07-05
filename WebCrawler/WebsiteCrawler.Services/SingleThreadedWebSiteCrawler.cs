using Dawn;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System.Net;
using System.Text.RegularExpressions;
using WebCrawler.DataAccessLayer.Context;
using WebCrawler.DataAccessLayer.Models;
using WebsiteCrawler.Infrastructure.entity;
using WebsiteCrawler.Infrastructure.extensions;
using WebsiteCrawler.Infrastructure.interfaces;
using WebsiteCrawler.Service.entity;

namespace WebsiteCrawler.Service
{
    public class SingleThreadedWebSiteCrawler : IWebSiteCrawler
    {
        public const int RetryAttempts = 2;
        private readonly List<HttpError> errors = new List<HttpError>();
        private readonly IHtmlParser htmlParser;
        private readonly HttpClient httpClient;
        private readonly Queue<SearchJob> jobQueue;
        private readonly HashSet<string> searchResults;

        public StartingNode StartingNode { get; set; }

        public SingleThreadedWebSiteCrawler(
            IHtmlParser htmlParser,
            HttpClient httpClient,
            AppDbContext db)
        {
            this.htmlParser = htmlParser;
            this.httpClient = httpClient;

            searchResults = new HashSet<string>();
            jobQueue = new Queue<SearchJob>();
        }

        public async Task<StartingNode> Run(WebsiteRecord record, int maxPagesToSearch)
        {
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Other");

            var node = new Node()
            {
                Children = new List<Node>(),
                Url = record.URL,
                Domain = GetDomainFromUrl(record.URL)
            };

            jobQueue.Enqueue(new SearchJob(record.URL, node));

            StartingNode = new StartingNode()
            {
                WebsiteRecord = record,
                Node = node
            };

            for (int pageCount = 0; pageCount < maxPagesToSearch; pageCount++)
            {
                if (jobQueue.Count == 0)
                {
                    break;
                }

                await DiscoverLinks(record.URL, DateTime.Now);
            }

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

        private async Task DiscoverLinks(string startingSite, DateTime startCrawlTime)
        {
            var searchJob = jobQueue.Dequeue();
            string pageContents = await DownloadPage(searchJob);
            if (pageContents == null)
            {
                return;
            }
            var links = FindLinksWithinHtml(pageContents);
            links.ForEach(rawLink =>
            {
                var link = rawLink.Trim().Trim('\n');

                var searchResult = new Node
                {
                    Url = startingSite,
                    Domain = GetDomainFromUrl(startingSite),
                    Children = new List<Node>()
                };
                bool isLinkAcceptable = IsLinkAcceptable(searchJob, link);
                Uri absoluteLink;
                if (!isLinkAcceptable)
                {
                    return;
                }
                else
                {
                    var parentLink = searchJob.Uri.GetParentUriString();
                    var absoluteUri = UrlExtensions.Combine(parentLink, link);
                    absoluteLink = absoluteUri;
                }

                if (
                (absoluteLink.Host.ToLower() == searchJob.Uri.Host.ToLower()) &&
                (absoluteLink.PathAndQuery.ToLower() == searchJob.Uri.PathAndQuery.ToLower())
                )
                {
                    return;
                }
                if (searchResults.Contains(absoluteLink.ToString()))
                {
                    return;
                }
                searchResult.CrawlTime = GetCrawlTime(startCrawlTime);
                searchResults.Add(absoluteLink.ToString());
                jobQueue.Enqueue(new SearchJob(absoluteLink.ToString(), searchResult));
                searchJob.Node.Children.Add(searchResult);
            });
        }

        private async Task<string> DownloadPage(SearchJob searchJob)
        {
            if ((searchJob.Uri.Scheme.ToLower() != "http") && (searchJob.Uri.Scheme.ToLower() != "https"))
            {
                return null;
            }

            var retryPolicy = CreateExponentialBackoffPolicy();

            var htmlResponse = await retryPolicy
                .ExecuteAsync(() => httpClient.GetAsync(searchJob.Url));

            if (!htmlResponse.IsSuccessStatusCode)
            {
                //throw new NotImplementedException("How do we handle errors? Think"); //TODO handle non-sucess response, Polly retry
                errors.Add(new HttpError { Url = searchJob.Url, HttpStatusCode = htmlResponse.StatusCode });
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

        private bool IsLinkAcceptable(SearchJob searchJob, string link)
        {
            var childLink = link;
            if (string.IsNullOrEmpty(childLink))
            {
                return false;
            }
            var frags = childLink.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (!frags.Any())
            {
                return false;
            }

            if (childLink.StartsWith("#"))
            {
                //Book marks are not wanted
                return false;
            }

            if (childLink.StartsWith("mailto:"))
            {
                //Email links are not wanted
                return false;
            }

            if (childLink.StartsWith("tel:"))
            {
                //Phone links are not wanted
                return false;
            }

            if (childLink.StartsWith("sms:"))
            {
                //sms links are not wanted
                return false;
            }

            return true;
        }

        private string GetDomainFromUrl(string url)
        {
            Uri uri = new Uri(url);
            string domain = uri.Host;

            return domain;
        }

        private bool? IsRegExpMatched(string url, string regExp)
        {
            if (string.IsNullOrEmpty(regExp))
            {
                return null;
            }

            if (Regex.IsMatch(url, regExp, RegexOptions.IgnoreCase))
            {
                return true;
            }

            return false;
        }

        private TimeSpan GetCrawlTime(DateTime startTime)
        {
            TimeSpan duration = DateTime.Now.Subtract(startTime);

            return duration;
        }
    }
}