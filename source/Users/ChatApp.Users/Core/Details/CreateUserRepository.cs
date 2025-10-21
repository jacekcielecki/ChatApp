using ChatApp.Shared.Data.Adapters.DbConnectionFactory;
using ChatApp.Shared.Data.Adapters.Entities;
using Dapper;

namespace ChatApp.Users.Core.Details;

public class CreateUserRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public CreateUserRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<Guid?> Create(User user)
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        const string sql =
            """
            INSERT INTO users (id, email, given_name, family_name, created_at)
            VALUES (@id, @email, @given_name, @family_name, @created_at)
            RETURNING id
            """;

        await using var connection = _dbConnectionFactory.Create();

        var userId = await connection.QuerySingleOrDefaultAsync<Guid>(sql, new
        {
            id = user.Id,
            email = user.Email,
            given_name = user.GivenName,
            family_name = user.FamilyName,
            created_at = user.CreatedAt
        });

        return userId;
    }
}
