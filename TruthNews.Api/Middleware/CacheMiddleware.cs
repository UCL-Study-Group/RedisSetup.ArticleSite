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

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Method != HttpMethods.Get)
        {
            await _next(context);
            return;
        }
        
        var cacheKey = GenerateCacheKey(context.Request);
        
        var cachedResponse = await _cacheService.GetAsync(cacheKey);

        // This is used if the CacheService finds a match
        if (!string.IsNullOrEmpty(cachedResponse))
        {
            Console.WriteLine($"[CacheMiddleware] Found hit for key: {cacheKey}");
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(cachedResponse);

            return;
        }

        Console.WriteLine($"[CacheMiddleware] No hit for key: {cacheKey}");
        
        // Capture the response so we can cache it
        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;
        
        // Let the request continue to the controller
        await _next(context);
        
        // Read what the controller returned
        responseBody.Seek(0, SeekOrigin.Begin);
        var responseText = await new StreamReader(responseBody).ReadToEndAsync();
        
        // Cache it if successful
        if (context.Response.StatusCode == 200 && !string.IsNullOrEmpty(responseText))
        {
            await _cacheService.SetAsync(cacheKey, responseText);
            Console.WriteLine($"[CacheMiddleware] Cached response for key: {cacheKey}");
        }
        
        // Send the response to the client
        responseBody.Seek(0, SeekOrigin.Begin);
        await responseBody.CopyToAsync(originalBodyStream);
        context.Response.Body = originalBodyStream;
    }

    private static string GenerateCacheKey(HttpRequest request)
    {
        var path = request.Path.ToString().ToLowerInvariant();
        var queryString = request.QueryString.ToString();
        return $"cache:{path}{queryString}";
    }
}

public static class CacheMiddlewareExtensions
{
    public static IApplicationBuilder UseCacheMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<CacheMiddleware>();
    }
}