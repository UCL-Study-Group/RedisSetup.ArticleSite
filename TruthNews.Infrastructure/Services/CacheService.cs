using TruthNews.Infrastructure.Context;

namespace TruthNews.Infrastructure.Services;

/// <summary>
/// Pretty much a wrap service, but it is what it is ¯\_(ツ)_/¯
/// </summary>
public class CacheService
{
    private readonly RedisContext _redisContext;
    
    public CacheService(RedisContext redisContext)
    {
        _redisContext = redisContext;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        return await _redisContext.GetAsync<T>(key);
    }

    public async Task<string?> GetAsync(string key)
    {
        return await _redisContext.GetAsync(key);
    }

    public async Task SetAsync<T>(string key, T value)
    {
        await _redisContext.SetAsync(key, value);
    }

    public async Task SetAsync(string key, object value)
    {
        await _redisContext.SetAsync(key, value);
    }

    public async Task DeleteAsync(string key)
    {
        await _redisContext.DeleteAsync(key);
    }

    public async Task ResetAsync()
    {
        await _redisContext.FlushByPatternAsync();
    }
}