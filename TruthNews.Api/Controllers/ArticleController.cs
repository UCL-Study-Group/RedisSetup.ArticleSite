using Microsoft.AspNetCore.Mvc;
using TruthNews.Common.Models;
using TruthNews.Infrastructure.Services;

namespace TruthNews.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ArticleController : ControllerBase
{
    private readonly ArticleService _articleService;

    public ArticleController(ArticleService articleService)
    {
        _articleService = articleService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Article>>> GetArticlesAsync()
    {
        var response = await _articleService.GetArticlesAsync();

        if (!response.Any())
            return NoContent();
        
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Article>> GetArticleAsync([FromRoute] int id)
    {
        var response = await _articleService.GetArticleAsync(id);

        if (response is null)
            return NotFound();
        
        return Ok(response);
    }
}