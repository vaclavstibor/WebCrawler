using System;
using WebCrawler.DataAccessLayer.Models;

namespace WebsiteCrawler.Service.entity
{
    /// <summary>
    /// Defines a single URL which will be crawled for further discover
    /// </summary>
    internal class SearchJob
    {
        public SearchJob(string url, Node node)
        {
            this.Url = url;
            this.Uri = new Uri(url);
            this.Node = node;
        }

        public Node Node { get; set; }
        /// <summary>
        /// Gets the Uri model from the raw text based Url
        /// </summary>
        public Uri Uri { get; }

        /// <summary>
        /// Gets or sets the fully qualified URL which will be searched
        /// </summary>
        public string Url { get; private set; }
    }
}