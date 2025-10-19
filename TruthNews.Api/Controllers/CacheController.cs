using Microsoft.AspNetCore.Mvc;
using TruthNews.Infrastructure.Services;

namespace TruthNews.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CacheController : ControllerBase
{
    private readonly CacheService  _cacheService;

    public CacheController(CacheService cacheService)
    {
        _cacheService = cacheService;
    }
    
    [HttpDelete("Reset")]
    public async Task<ActionResult> ResetAsync()
    {
        try
        {
            await _cacheService.ResetAsync();
            
            return Ok();
        }
        catch (Exception)
        {
            return Problem("Failed to reset cache");
        }
    }
}