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

    public async Task<User?> GetById(Guid id)
    {
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        const string sql =
            """
            SELECT id, email, created_at
            FROM users
            WHERE id = @id
            """;

        await using var connection = _connectionFactory.Create();
        var user = await connection.QuerySingleOrDefaultAsync<User>(sql, new { id });

        return user;
    }

    public async Task<User?> GetByEmail(string email)
    {
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        const string sql =
            """
            SELECT id, email, created_at
            FROM users
            WHERE email = @email
            """;

        await using var connection = _connectionFactory.Create();
        var user = await connection.QuerySingleOrDefaultAsync<User>(sql, new { email });

        return user;
    }

    public async Task<string[]> GetEmailsBySearchPhrase(string searchPhrase)
    {
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        const string sql =
            """
            SELECT email
            FROM users as
            WHERE email LIKE @Phrase
            LIMIT 5;
            """;

        await using var connection = _connectionFactory.Create();
        var emails = await connection.QueryAsync<string>(sql, new { Phrase = $"%{searchPhrase}%" });

        return emails.ToArray();
    }

    public async Task<Guid?> Insert(User user)
    {
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        const string sql =
            """
            INSERT INTO users (id, email, created_at)
            VALUES (@id, @email, @created_at)
            RETURNING id;
            """;

        await using var connection = _connectionFactory.Create();
        var userId = await connection.QuerySingleOrDefaultAsync<Guid>(sql, new
        {
            id = user.Id,
            email = user.Email,
            created_at = user.CreatedAt
        });

        return userId;
    }
}