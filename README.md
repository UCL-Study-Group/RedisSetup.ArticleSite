# Redis Implementation

## Introduction

This project was made as a task from our glorious teacher, where we needed to implement Redis to
cache objects from our backend, rather than call the database each time, so we could prove how
much faster it would be. Spoiler alert: it is marginally faster!

## Disclosure

As a disclosure beforehand, the project primarily follows traits from the Clean Architecture approach.
Though we decided to skip out on the Application layer, just to save time on having to add three projects.
So when you look at it, imagine there was an `.Application` project, please?

## Point of interests

### The Cache Middleware

So looking through the project. One of the first point of interests that comes, would be the `CacheMiddleware.cs`. 
The middleware is used as a way to intercept the request, before it tries to talk with our MsSQL database.
Here it check it the entry exists inside the Redis database, and if it does, it will simply with that response

```csharp
var cachedResponse = await _cacheService.GetAsync(cacheKey);

if (!string.IsNullOrEmpty(cachedResponse))
{
    Console.WriteLine($"[CacheMiddleware] Found hit for key: {cacheKey}, within {totalStopwatch.ElapsedMilliseconds}ms");
    context.Response.ContentType = "application/json";
    await context.Response.WriteAsync(cachedResponse);
    return;
}
```

If it does not found an entry inside the Redis, it will than go on to continue the requests to our MsSQL, but we
also decide to listen in, so we can save it to our cache ;)

```csharp
var originalBodyStream = context.Response.Body;
using var responseBody = new MemoryStream();
context.Response.Body = responseBody;

await _next(context);

responseBody.Seek(0, SeekOrigin.Begin);
var responseText = await new StreamReader(responseBody).ReadToEndAsync();

if (context.Response.StatusCode == 200 && !string.IsNullOrEmpty(responseText))
{
    await _cacheService.SetAsync(cacheKey, responseText);
}
```

### Redis Context

To allow us to make requests to the Redis Database, we decided to go with the NuGet Package `NRedisStack`. Its implementation
is rather straightforward. In our `DependencyInjection.cs`, we declare the needed IConnectionMultiplexer as well as the
context itself. Here we've taken the liberty to define a expiration time of 1 minute, as to make it easier to show it.

```csharp
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
```

The `RedisContext` just wraps most of the basic calls to the redis database.

```csharp
public async Task GetAsync(string key)
{
    if (string.IsNullOrWhiteSpace(key))
        throw new InvalidOperationException("Key cannot be null or whitespace.");
    
    var redisObject = await _database.StringGetAsync(key);
    
    return !redisObject.HasValue ? default : JsonSerializer.Deserialize(redisObject.ToString());
}

public async Task SetAsync(string key, T value)
{
    if (string.IsNullOrWhiteSpace(key))
        throw new InvalidOperationException("Key cannot be null or whitespace.");
    
    var serialized = JsonSerializer.Serialize(value);
    
    await _database.StringSetAsync(key, serialized, _expirationTime);
}
```