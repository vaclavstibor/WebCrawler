﻿using System.Collections.Generic;
using System.Threading.Tasks;
using WebCrawler.DataAccessLayer.Models;

namespace WebsiteCrawler.Infrastructure.interfaces
{
    /// <summary>
    /// Abstracts the implementation of a web site crawler which will search for links in the given page and subsequent pages
    /// </summary>
    public interface IWebSiteCrawler
    {
        /// <summary>
        /// Crawls the specified web page and discovered child page which falls in the same domain as the starting url.
        /// </summary>
        /// <param name="url">The URL of the web page</param>
        /// <param name="maxPagesToSearch">An upper limit on the maximum number of discovered pages to crawl. This becomes a stopping criteria</param>
        /// <returns></returns>
        public Task<StartingNode> Run(WebsiteRecord record, int executionId, int? maximumCountOfNodes = null);
    }
}