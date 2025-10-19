using TruthNews.Api.Middleware;
using TruthNews.Infrastructure;

namespace TruthNews.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // So it uses our infrastructure... very important!
        builder.Services.AddInfrastructure(builder.Configuration);

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        var app = builder.Build();
        
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        // I like swagger, what can I say ¯\_(ツ)_/¯
        // It is also so you can just access it from the base ip, no need for /swagger!!
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/openapi/v1.json", "TruthNews API v1");
            options.RoutePrefix = string.Empty;
        });

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        // Since we're using our own middleware, we need to declare it so it uses it :)
        app.UseCacheMiddleware();

        app.Run();
    }
}