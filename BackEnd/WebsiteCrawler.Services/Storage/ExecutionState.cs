using System;
using System.Collections.Concurrent;
using WebCrawler.DataAccessLayer.Models;

namespace WebsiteCrawler.Services.Storage
{
    public class ExecutionState
    {
        public List<Node> NodesToSave { get; set; }
        public List<Node> CachedNodes { get; set; }

        public long CrawlingUpdateState { get; set; }
        public long CacheUpdateState { get; set; }
        public long CacheLastEmptying { get; set; }

        public ExecutionState()
        {
            NodesToSave = new List<Node>();
            CachedNodes = new List<Node>();
        }
    }
}
