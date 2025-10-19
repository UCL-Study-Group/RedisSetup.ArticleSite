using Microsoft.AspNetCore.Mvc;
using TruthNews.Common.Models;
using TruthNews.Infrastructure.Services;

namespace TruthNews.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsersAsync(CancellationToken cancellationToken)
    {
        var response = await _userService.GetUsersAsync(cancellationToken);

        if (!response.Any())
            return NoContent();
        
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<User>> GetUserAsync([FromRoute] int id, CancellationToken cancellationToken)
    {
        var response = await _userService.GetUserAsync(id, cancellationToken);

        if (response is null)
            return NotFound();
        
        return Ok(response);
    }
}