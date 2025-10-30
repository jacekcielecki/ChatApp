using ChatApp.Shared.Data.Adapters.DbConnectionFactory;
using ChatApp.Shared.Data.Adapters.Entities;
using Dapper;

namespace ChatApp.Users.Core.Details;

public class GetUserByEmailRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GetUserByEmailRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<User?> Get(string email)
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        const string sql =
            """
            SELECT id, email, given_name, family_name, profile_picture_url, bio, created_at
            FROM users
            WHERE email = @email
            """;

        await using var connection = _dbConnectionFactory.Create();
        var user = await connection.QuerySingleOrDefaultAsync<User>(sql, new { email });

        return user;
    }
}
