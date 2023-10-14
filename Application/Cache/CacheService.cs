using System.Text.Json;
using Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace Application.Cache;

public sealed class CacheService : ICacheService
{
    private static readonly SemaphoreSlim semaphoreSlim = new(1, 1);
    private readonly IDistributedCache _cache;
    private readonly DistributedCacheEntryOptions _cacheEntryOptions = new();

    public CacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task AddAsync<T>(string recordId, T data, CancellationToken cancellationToken)
    {
        var jsonData = JsonSerializer.Serialize(data);
        await semaphoreSlim.WaitAsync(cancellationToken);
        try
        {
            await _cache.SetStringAsync(recordId, jsonData, _cacheEntryOptions, cancellationToken);
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }

    public async Task<T?> GetAsync<T>(string recordId, CancellationToken cancellationToken)
    {
        string? data;
        await semaphoreSlim.WaitAsync(cancellationToken);
        try
        {
            data = await _cache.GetStringAsync(recordId, cancellationToken);
        }
        finally
        {
            semaphoreSlim.Release();
        }
        return data is null ? default : JsonSerializer.Deserialize<T>(data);
    }

    public async Task<T?> GetAndRemoveAsync<T>(string recordId, CancellationToken cancellationToken)
    {
        string? data;
        await semaphoreSlim.WaitAsync(cancellationToken);
        try
        {
            data = await _cache.GetStringAsync(recordId, cancellationToken);
            await _cache.RemoveAsync(recordId, cancellationToken);
        }
        finally
        {
            semaphoreSlim.Release();
        }
        var result = data is null ? default : JsonSerializer.Deserialize<T>(data);
        return result;
    }
}