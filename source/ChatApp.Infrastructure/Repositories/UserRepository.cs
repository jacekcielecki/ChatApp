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
        const string sql =
            """
            SELECT id, email
            FROM users
            WHERE id = @id
            """;

        using var connection = _connectionFactory.Create();
        var user = await connection.QuerySingleOrDefaultAsync<User>(sql, new { id });

        return user;
    }

    public async Task<User?> GetByEmail(string email)
    {
        const string sql =
            """
            SELECT id, email
            FROM users
            WHERE email = @email
            """;

        using var connection = _connectionFactory.Create();
        var user = await connection.QuerySingleOrDefaultAsync<User>(sql, new { email });

        return user;
    }
        
    public async Task<string[]> GetEmailsBySearchPhrase(string searchPhrase)
    {
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
        const string sql =
            """
            INSERT INTO users (email)
            VALUES (@email)
            RETURNING id;
            """;

        using var connection = _connectionFactory.Create();
        var userId = await connection.QuerySingleOrDefaultAsync<Guid?>(sql, new { email = request.Email });

        return userId;
    }
}