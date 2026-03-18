using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Text;
using System.Text.Json;

namespace Kernel.Utils;

/// <summary>
/// Provides helper methods for interacting with a distributed cache and Redis, including operations to get, set, and
/// remove cached items by key or prefix.
/// </summary>
public class CacheHelper
{
    private readonly IDistributedCache _cache;
    private readonly IConnectionMultiplexer _redis;
    private readonly TimeSpan _defaultExpiry = 10.ToMinutes();

    public CacheHelper(IDistributedCache cache, IConnectionMultiplexer redis)
    {
        _cache = cache;
        _redis = redis;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var cachedData = await _cache.GetAsync(key);
        if (cachedData == null)
            return default;

        var result = JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(cachedData));

        if (result != null)
        {
            await _cache.RefreshAsync(key);
        }

        return result;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiry ?? _defaultExpiry
        };

        var jsonData = JsonSerializer.Serialize(value);
        await _cache.SetAsync(key, Encoding.UTF8.GetBytes(jsonData), options);
    }

    public Task RemoveAsync(string key)
    {
        return _cache.RemoveAsync(key);
    }

    public async Task RemoveByPrefix(string prefix)
    {
        // Get all endpoints (Master/Slave) of Redis
        var endpoints = _redis.GetEndPoints();
        var server = _redis.GetServer(endpoints.First());

        // Fid key match pattern (ex: "User_*")
        // Note: IDistributedCache maybe add prefix if you config InstanceName
        var pattern = $"{prefix}*";

        var keys = server.Keys(pattern: pattern);

        foreach (var key in keys)
        {
            await _cache.RemoveAsync(key);
        }
    }
}
