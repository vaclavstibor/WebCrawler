using Microsoft.EntityFrameworkCore;
using Polly.Caching;
using System;
using System.Collections.Concurrent;
using WebCrawler.DataAccessLayer.Context;
using WebCrawler.DataAccessLayer.Migrations;
using WebCrawler.DataAccessLayer.Models;
using WebsiteCrawler.Infrastructure.DataTransferObjects;
using WebsiteCrawler.Infrastructure.Storage;

namespace WebsiteCrawler.Services.Storage;

public class CrawlingNodeStorage : ICrawlingNodeStorage
{
    /// <summary>
    /// Caching Nodes inside Dictionary with websiteRecordId as the key.
    /// Each Node is also stored inside dictionary with it's id as a key for faster lookups
    /// </summary>
    private readonly Dictionary<int, ExecutionState> cache;
    private readonly IDbContextFactory<AppDbContext> dbContextFactory;

    private const int cachingLimit = 15;
    private const int magicStateLimit = 7;

    public CrawlingNodeStorage(IDbContextFactory<AppDbContext> dbContextFactory)
    {
        cache = new Dictionary<int, ExecutionState>();
        this.dbContextFactory = dbContextFactory;
    }

    public void CreateNewExecution(int websiteRecordId)
    {
        cache.Add(websiteRecordId, new ExecutionState());
    }

    public async Task AddOrUpdateNodeAsync(Node node, int websiteRecordId)
    {
        var state = cache[websiteRecordId];

        bool nodeAlreadyCached = false;

        for (int i = 0; i < state.NodesToSave.Count(); i++)
        {
            if (state.NodesToSave[i].Url == node.Url)
            { 
                nodeAlreadyCached = true;
                state.NodesToSave[i] = node;

                break;
            }
        }
        if (!nodeAlreadyCached)
        {
            state.NodesToSave.Add(node);
        }

        if (state.CrawlingUpdateState >= cachingLimit)
        {
            await Replace(websiteRecordId);
        }

        state.CrawlingUpdateState++;
    }

    private async Task Replace(int websiteRecordId)
    {
        bool isRecordThere = cache.TryGetValue(websiteRecordId, out var state);

        if (!isRecordThere)
        {
            return;
        }

        var existing = state!.NodesToSave.Where(x => x.Id != 0);
        var newNodes = state!.NodesToSave.Except(existing);

        using (var db = dbContextFactory.CreateDbContext())
        {
            try
            {
                db.Nodes.AddRange(newNodes);

                db.SaveChanges();

                var newExistingNodes = new List<Node>();

                foreach (var node in existing)
                {
                    var nodeInDb =  db.Nodes.SingleOrDefault(x => x.Id == node.Id);

                    if (nodeInDb != null)
                    { 
                        foreach (var child in )
                        newExistingNodes.Add(nodeInDb);
                    }
                }

                db.Nodes.UpdateRange(newExistingNodes);

                db.SaveChanges();
            }
            catch (Exception ex) { }
        }

        state.CachedNodes = state.CachedNodes.ExceptBy(state.NodesToSave.Select(x => x.Id), y => y.Id).ToList();

        state.CachedNodes.AddRange(state.NodesToSave);

        state.NodesToSave = new List<Node>();

        if (state.CacheUpdateState - state.CacheLastEmptying > magicStateLimit)
        {
            state.CachedNodes = state.CachedNodes.TakeLast(cachingLimit).ToList();

            state.CacheLastEmptying = state.CacheUpdateState;
        }

        state.CrawlingUpdateState = 0;
        state.CacheUpdateState++;
    }

    public async Task<ExecutionStateDto> GetNodesAsync(int websiteRecordId, long lastUpdateState, int executionId)
    {
        if (!cache.ContainsKey(websiteRecordId))
        {
            return new ExecutionStateDto();
        }

        var state = cache[websiteRecordId];

        if (lastUpdateState >= state.CacheLastEmptying)
        {
            return new ExecutionStateDto()
            {
                Nodes = state.CachedNodes,
                UpdateState = state.CrawlingUpdateState
            };
        }

        using (var db = await dbContextFactory.CreateDbContextAsync())
        {
            return new ExecutionStateDto()
            {
                UpdateState = state.CrawlingUpdateState,
                Nodes = await db.Nodes.Where(x => x.WebsiteRecordId == websiteRecordId && x.ExecutionId == executionId).ToListAsync()
            };
        }
    }

    private void DeleteCache(int websiteRecordId)
    {
        cache.Remove(websiteRecordId, out var all);
    }

    public async Task FinalizeCrawlingAsync(int websiteRecordId)
    {
        await Replace(websiteRecordId);

        DeleteCache(websiteRecordId);
    }

    public async Task<Node?> GetNodeOrDefaultAsync(int websiteRecordId, string url, int executionId)
    {
        using (var db = await dbContextFactory.CreateDbContextAsync())
        {
            var executionState = cache[websiteRecordId];
            return executionState.NodesToSave.SingleOrDefault(x => x.Url == url) ??
                (await db.Nodes.Include(x => x.Children).SingleOrDefaultAsync(x => x.Url == url && x.ExecutionId == executionId));
        }
    }

    public async Task RemoveNodeAsync(Node node, int websiteRecordId)
    {
        var state = cache[websiteRecordId];

        var nodeInNewCache = state.NodesToSave.SingleOrDefault(x => x.Id == node.Id);

        if (nodeInNewCache != null)
        { 
            state.NodesToSave.Remove(nodeInNewCache);
        }

        var nodeInCache = state.CachedNodes.SingleOrDefault(x => x.Id == node.Id);
        if (nodeInCache != null)
        { 
            state.CachedNodes.Remove(nodeInCache);
        }

        using (var db = await dbContextFactory.CreateDbContextAsync())
        {
            var nodeInDb = await db.Nodes.SingleOrDefaultAsync(x => x.Id == node.Id);
            if (nodeInDb != null)
            {
                db.Nodes.Remove(node);
                await db.SaveChangesAsync();
            }
        }
    }
}
