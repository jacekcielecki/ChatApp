using ChatApp.Contracts.Request;
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

        using var connection = _connectionFactory.Create();
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

        using var connection = _connectionFactory.Create();
        var user = await connection.QuerySingleOrDefaultAsync<User>(sql, new { email });

        return user;
    }

    public async Task<string[]> GetEmailsBySearchPhrase(string searchPhrase)
    {
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        const string sql =
            """
            SELECT email
            FROM public.users as u
            WHERE u.email LIKE @Phrase
            """;

        using var connection = _connectionFactory.Create();
        var emails = await connection.QueryAsync<string>(sql, new { Phrase = $"%{searchPhrase}%" });

        return emails.ToArray();
    }

    public async Task<Guid?> Create(CreateUserRequest request)
    {
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        const string sql =
            """
            INSERT INTO users (email, created_at)
            VALUES (@email, @created_at)
            RETURNING id;
            """;

        using var connection = _connectionFactory.Create();
        var userId = await connection.QuerySingleOrDefaultAsync<Guid?>(sql, new { email = request.Email, created_at = DateTime.Now });

        return userId;
    }
}