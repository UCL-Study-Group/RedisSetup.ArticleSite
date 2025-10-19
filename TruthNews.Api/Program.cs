using TruthNews.Api.Middleware;
using TruthNews.Infrastructure;

namespace TruthNews.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container
        builder.Services.AddInfrastructure(builder.Configuration);
        
        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }
        
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/openapi/v1.json", "TruthNews API v1");
            options.RoutePrefix = string.Empty; // Serve Swagger UI at the app's root
        });

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.UseCacheMiddleware();

        app.Run();
    }
}