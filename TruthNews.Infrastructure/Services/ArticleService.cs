using TruthNews.Common.Models;
using TruthNews.Infrastructure.Context;

namespace TruthNews.Infrastructure.Services;

public class ArticleService
{
    private readonly DbContext _context;
    
    public ArticleService(DbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Article>> GetArticlesAsync()
    {
        try
        {
            var response = await _context.GetAllAsync<Article>();

            return response;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{nameof(GetArticlesAsync)}] Encountered an exception when getting users: {ex.Message}");
            throw;
        }
    }

    public async Task<Article?> GetArticleAsync(int id)
    {
        try
        {
            var response = await _context.GetByIdAsync<Article>(id);
            
            return response;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{nameof(GetArticleAsync)}] Encountered an exception when getting user: {ex.Message}");
            throw;
        }
    }
}