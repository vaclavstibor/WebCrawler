using System;
using System.Collections.Concurrent;
using WebCrawler.DataAccessLayer.Models;

namespace WebCrawler.DataAccessLayer.Cache
{
    public static class CrawlingCache
    {
        private static readonly ConcurrentDictionary<int, ConcurrentDictionary<int, Node>> cache;
        private static readonly ConcurrentDictionary<int, ConcurrentDictionary<int, Node>> recentCache;

        static CrawlingCache()
        {
            cache = new ConcurrentDictionary<int, ConcurrentDictionary<int, Node>>();
            recentCache = new ConcurrentDictionary<int, ConcurrentDictionary<int, Node>>();
        }

        public static void AddOrUpdateNode(Node node, int websiteRecordId)
        {
            if (!recentCache.ContainsKey(websiteRecordId))
            {
                recentCache.TryAdd(websiteRecordId, new ConcurrentDictionary<int, Node>());
            }

            if (recentCache[websiteRecordId].Any(x => x.Key == node.Id))
            {
                recentCache[websiteRecordId][node.Id] = node;
            }
            else
            {
                recentCache[websiteRecordId].TryAdd(node.Id, node);
            }
        }

        public static List<Node> GetAndDeleteCachedNodes(int websiteRecordId)
        {
            if (recentCache.ContainsKey(websiteRecordId))
            {
                var nodes = recentCache[websiteRecordId].Select(x => x.Value).ToList();
                recentCache[websiteRecordId] = new ConcurrentDictionary<int, Node>();

                Task.Run(() => {
                    foreach (var node in nodes)
                    {
                        cache[websiteRecordId][node.Id] = node;
                    }
                });

                return nodes;
            }

            return new List<Node>();
        }

        public static List<Node> GetAllNodes(int websiteRecordId)
        {
            if (cache.ContainsKey(websiteRecordId))
            {
                var nodes = cache[websiteRecordId].Select(x => x.Value).ToList();
                cache[websiteRecordId] = new ConcurrentDictionary<int, Node>();
                return nodes;
            }

            return new List<Node>();
        }

        public static void DeleteCache(int websiteRecordId)
        {
            recentCache.Remove(websiteRecordId, out var recent);
            cache.Remove(websiteRecordId, out var all);
        }
    }
}
