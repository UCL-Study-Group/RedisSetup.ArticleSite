using Microsoft.AspNetCore.Mvc;
using TruthNews.Common.Models;
using TruthNews.Infrastructure.Services;

namespace TruthNews.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly UserService  _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet("User")]
    public async Task<ActionResult<IEnumerable<User>>> GetUsersAsync()
    {
        var response = await _userService.GetUsersAsync(CancellationToken.None);

        if (!response.Any())
            return NoContent();
        
        return Ok(response);
    }

    [HttpGet("User/{id}")]
    public async Task<ActionResult<User>> GetUserAsync([FromRoute] int userId)
    {
        var response = await _userService.GetUserAsync(userId, CancellationToken.None);

        if (response is null)
            return NoContent();
        
        return Ok(response);
    }
}