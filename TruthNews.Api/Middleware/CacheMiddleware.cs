using System.Diagnostics;
using TruthNews.Infrastructure.Services;

namespace TruthNews.Api.Middleware;

public class CacheMiddleware
{
    private readonly RequestDelegate _next;
    private readonly CacheService  _cacheService; 
    
    public CacheMiddleware(RequestDelegate next, CacheService cacheService)
    {
        _next = next;
        _cacheService = cacheService;
    }

    /// <summary>
    /// This is the method it runs though when we make a request to the API
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        // First off, I check if it is a GET Method, since it is the only
        // methods we're interested in, when using caching
        if (context.Request.Method != HttpMethods.Get)
        {
            await _next(context);
            return;
        }
        
        // Seems like it ran into an issue where it also tried to cache the swagger documentation O_o
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? string.Empty;
        if (path.StartsWith("/swagger") || 
            path.StartsWith("/openapi") || 
            path.Contains(".json") ||
            path.Contains(".css") ||
            path.Contains(".js"))
        {
            await _next(context);
            return;
        }
        
        // Here we make it into a cacheKey. It will be the one used on the redis database
        var cacheKey = GenerateCacheKey(context.Request);
        var totalStopwatch = Stopwatch.StartNew();
        
        // We check if we have it in cache
        var cachedResponse = await _cacheService.GetAsync(cacheKey);

        // In case we do, we just respond with the response. No need for more :D
        if (!string.IsNullOrEmpty(cachedResponse))
        {
            totalStopwatch.Stop();
            Console.WriteLine($"[CacheMiddleware] Found hit for key: {cacheKey}, within {totalStopwatch.ElapsedMilliseconds}ms");
            
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(cachedResponse);
            return;
        }
        
        // In case we don't. Then we can read the response and save it in cache, for the next time!
        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;
        
        // Let the request continue to the controller
        await _next(context);
        
        // Read what the controller returned
        responseBody.Seek(0, SeekOrigin.Begin);
        var responseText = await new StreamReader(responseBody).ReadToEndAsync();
        
        // We wouldn't want to save if it failed, so we check for status code!
        if (context.Response.StatusCode == 200 && !string.IsNullOrEmpty(responseText))
        {
            await _cacheService.SetAsync(cacheKey, responseText);
        }
        
        totalStopwatch.Stop();
        Console.WriteLine($"[CacheMiddleware] No hit for key: {cacheKey}, took {totalStopwatch.ElapsedMilliseconds}ms");
        
        // Send the response to the client
        responseBody.Seek(0, SeekOrigin.Begin);
        await responseBody.CopyToAsync(originalBodyStream);
        context.Response.Body = originalBodyStream;
    }

    /// <summary>
    /// Simple private method used for making the key into a cache key
    /// Pssst it is the one we use for redis 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    private static string GenerateCacheKey(HttpRequest request)
    {
        var path = request.Path.ToString().ToLowerInvariant();
        var queryString = request.QueryString.ToString();
        return $"cache:{path}{queryString}";
    }
}

/// <summary>
/// This is so we can call it in our Program.cs
/// </summary>
public static class CacheMiddlewareExtensions
{
    public static IApplicationBuilder UseCacheMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<CacheMiddleware>();
    }
}