using System.Text.Json;
using StackExchange.Redis;

namespace TruthNews.Infrastructure.Context;

public class RedisContext
{
    private readonly IDatabase _database;
    private readonly IServer _server;
    private readonly TimeSpan _expirationTime;
    
    public RedisContext(IConnectionMultiplexer connection, TimeSpan? expirationTime)
    {
        _database = connection.GetDatabase();
        _server = connection.GetServer(connection.GetEndPoints().First());
        _expirationTime = expirationTime ?? TimeSpan.FromMinutes(1);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new InvalidOperationException("Key cannot be null or whitespace.");
        
        var redisObject = await _database.StringGetAsync(key);
        
        return !redisObject.HasValue ? default : JsonSerializer.Deserialize<T>(redisObject.ToString());
    }

    public async Task<string?> GetAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new InvalidOperationException("Key cannot be null or whitespace.");
        
        var redisObject = await _database.StringGetAsync(key);
        
        return !redisObject.HasValue ? null : redisObject.ToString();
    }

    public async Task SetAsync<T>(string key, T value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new InvalidOperationException("Key cannot be null or whitespace.");
        
        var serialized = JsonSerializer.Serialize(value);
        
        await _database.StringSetAsync(key, serialized, _expirationTime);
    }

    public async Task SetAsync(string key, object value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new InvalidOperationException("Key cannot be null or whitespace.");
        
        await _database.StringSetAsync(key, JsonSerializer.Serialize(value), _expirationTime);
    }

    public async Task DeleteAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new InvalidOperationException("Key cannot be null or whitespace.");
        
        await _database.KeyDeleteAsync(key);
    }
}