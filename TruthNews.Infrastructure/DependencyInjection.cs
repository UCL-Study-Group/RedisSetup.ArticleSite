using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using TruthNews.Infrastructure.Context;
using TruthNews.Infrastructure.Services;

namespace TruthNews.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        const string redisConnection = "localhost:6379";

        services.AddSingleton<IConnectionMultiplexer>(conn =>
        {
            var configOptions = ConfigurationOptions.Parse(redisConnection);
            return ConnectionMultiplexer.Connect(configOptions);
        });

        services.AddSingleton<RedisContext>(r =>
        {
            var connection = r.GetRequiredService<IConnectionMultiplexer>();
            var expiration = TimeSpan.FromMinutes(1);
            
            return new RedisContext(connection, expiration);
        });

        services.AddScoped<DbContext>();
        
        return services;
    }
}