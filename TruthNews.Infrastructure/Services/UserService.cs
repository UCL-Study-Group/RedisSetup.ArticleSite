using TruthNews.Common.Models;
using TruthNews.Infrastructure.Context;

namespace TruthNews.Infrastructure.Services;

public class UserService
{
    private readonly DbContext _context;
    
    public UserService(DbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetUsersAsync(CancellationToken cancellationToken)
    {
        try
        {
            var response = await _context.GetAllAsync<User>();

            return response;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{nameof(GetUsersAsync)}] Encountered an exception when getting users: {ex.Message}");
            throw;
        }
    }

    public async Task<User?> GetUserAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _context.GetByIdAsync<User>(id);
            
            return response;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{nameof(GetUserAsync)}] Encountered an exception when getting user: {ex.Message}");
            throw;
        }
    }
}