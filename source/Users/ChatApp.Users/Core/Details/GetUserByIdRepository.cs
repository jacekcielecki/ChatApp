using ChatApp.Shared.Data.Adapters.DbConnectionFactory;
using ChatApp.Shared.Data.Adapters.Entities;
using Dapper;

namespace ChatApp.Users.Core.Details;

public class GetUserByIdRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GetUserByIdRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<User?> Get(Guid id)
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        const string sql =
            """
            SELECT id, email, given_name, family_name, created_at
            FROM users
            WHERE id = @id
            """;

        await using var connection = _dbConnectionFactory.Create();
        var user = await connection.QuerySingleOrDefaultAsync<User>(sql, new { id });

        return user;
    }
}
