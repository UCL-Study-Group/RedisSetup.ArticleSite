namespace TruthNews.Api.Middleware;

public class RedisMiddleware
{
    private readonly RequestDelegate _next;
    
    public RedisMiddleware(RequestDelegate next)
    {
        _next = next;
    }
}

public static class RedisMiddlewareExtensions
{
    public static IApplicationBuilder UseRedis(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RedisMiddleware>();
    }
}