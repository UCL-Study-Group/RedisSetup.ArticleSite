using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RepoDb;
using StackExchange.Redis;
using TruthNews.Infrastructure.Context;
using TruthNews.Infrastructure.Services;

namespace TruthNews.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// So apparently this is a way smarter way to use dependency injection for our
    /// services... who knew!!! So I've decided to use it B)
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Since we're using Repo, we need to declare so it knows it is using MsSQL ;(
        GlobalConfiguration.Setup().UseSqlServer();
        
        var redisConnection = configuration["Redis_Connection"] ??  "localhost:6379";
        var msConnection = configuration["MsSql_Connection"] ?? "Server=localhost,1433;Database=TruthNews;User Id=sa;Password=Password123!;TrustServerCertificate=True;";
        
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

        services.AddScoped<IDbConnection>(ms => new SqlConnection(msConnection));
        services.AddScoped<DbContext>();

        services.AddScoped<UserService>();
        services.AddScoped<ArticleService>();
        services.AddSingleton<CacheService>();
        
        return services;
    }
}