﻿using System;
using System.Collections.Concurrent;
using WebCrawler.DataAccessLayer.Models;

namespace WebCrawler.DataAccessLayer.Cache
{
    public static class CrawlingCache
    {
        private static readonly ConcurrentDictionary<int, ConcurrentDictionary<int, Node>> cache;

        static CrawlingCache()
        {
            cache = new ConcurrentDictionary<int, ConcurrentDictionary<int, Node>>();
        }

        public static void AddOrUpdateNode(Node node, int websiteRecordId)
        {
            if (!cache.ContainsKey(websiteRecordId))
            {
                cache.TryAdd(websiteRecordId, new ConcurrentDictionary<int, Node>());
                
            }
            if (cache[websiteRecordId].Any(x => x.Key == node.Id))
            {
                cache[websiteRecordId][node.Id] = node;
            }
            else
            {
                cache[websiteRecordId].TryAdd(node.Id, node);
            }
        }

        public static List<Node> GetNodes(int websiteRecordId)
        {
            if (cache.ContainsKey(websiteRecordId))
            {
                var nodes = cache[websiteRecordId].Select(x => x.Value).ToList();
                return nodes;
            }

            return new List<Node>();
        }

        public static void DeleteCache(int websiteRecordId)
        {
            cache.Remove(websiteRecordId, out var all);
        }
    }
}
