using System.Data;
using Microsoft.Data.SqlClient;
using RepoDb;

namespace TruthNews.Infrastructure.Context;

public class DbContext
{
    private readonly IDbConnection _connection;

    public DbContext(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<T?> GetByIdAsync<T>(object id) where T : class
    {
        await using var connection = new SqlConnection(_connection.ConnectionString);
        var result = await connection.QueryAsync<T>(id);
        
        return result.FirstOrDefault();
    }
    
    public async Task<IEnumerable<T>> GetAllAsync<T>() where T : class
    {
        await using var connection = new SqlConnection(_connection.ConnectionString);
        return await connection.QueryAllAsync<T>();
    }

    public async Task InsertAsync<T>(T item) where T : class
    {
        await using var connection = new SqlConnection(_connection.ConnectionString);
        await connection.InsertAsync(item);
    }

    public async Task UpdateAsync<T>(T item) where T : class
    {
        await using var connection = new SqlConnection(_connection.ConnectionString);
        await connection.UpdateAsync(item);
    }

    public async Task DeleteAsync<T>(T item) where T : class
    {
        await using var connection = new SqlConnection(_connection.ConnectionString);
        await connection.DeleteAsync(item);
    }
}