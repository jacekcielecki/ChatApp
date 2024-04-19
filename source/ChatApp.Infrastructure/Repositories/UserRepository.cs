using ChatApp.Domain.Entities;
using ChatApp.Domain.Interfaces.Repositories;
using ChatApp.Infrastructure.Database.DbConnectionFactory;
using Dapper;

namespace ChatApp.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UserRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<User?> GetByName(string name)
    {
        const string sql =
            """
            SELECT name
            FROM public."users"
            WHERE Name = @Name
            """;

        using var connection = _connectionFactory.Create();
        var user = await connection.QuerySingleOrDefaultAsync<User>(sql, new { name });

        return user;
    }
}