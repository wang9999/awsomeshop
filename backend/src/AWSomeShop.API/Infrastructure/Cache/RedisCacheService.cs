using System.Text.Json;
using StackExchange.Redis;

namespace AWSomeShop.API.Infrastructure.Cache;

public class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _database;
    private readonly JsonSerializerOptions _jsonOptions;

    public RedisCacheService(IConnectionMultiplexer redis)
    {
        _redis = redis;
        _database = redis.GetDatabase();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await _database.StringGetAsync(key);
        if (value.IsNullOrEmpty)
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(value!, _jsonOptions);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        var serialized = JsonSerializer.Serialize(value, _jsonOptions);
        if (expiration.HasValue)
        {
            await _database.StringSetAsync(key, serialized, expiration.Value);
        }
        else
        {
            await _database.StringSetAsync(key, serialized);
        }
    }

    public async Task RemoveAsync(string key)
    {
        await _database.KeyDeleteAsync(key);
    }

    public async Task RemoveByPatternAsync(string pattern)
    {
        var server = _redis.GetServer(_redis.GetEndPoints().First());
        var keys = server.Keys(pattern: pattern).ToArray();
        
        if (keys.Length > 0)
        {
            await _database.KeyDeleteAsync(keys);
        }
    }

    public async Task<bool> ExistsAsync(string key)
    {
        return await _database.KeyExistsAsync(key);
    }

    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
    {
        var cached = await GetAsync<T>(key);
        if (cached != null)
        {
            return cached;
        }

        var value = await factory();
        await SetAsync(key, value, expiration);
        return value;
    }
}
