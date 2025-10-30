using ChatApp.Shared.Data.Adapters.DbConnectionFactory;
using ChatApp.Shared.Data.Adapters.Entities;
using Dapper;

namespace ChatApp.Users.Core.Details;

public class UpdateUserRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public UpdateUserRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task Update(User user)
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        const string sql =
            """
               UPDATE users
               SET profile_picture_url = @ProfilePictureUrl, bio = @Bio
               WHERE id = @Id;
            """;

        await using var connection = _dbConnectionFactory.Create();

        await connection.QuerySingleOrDefaultAsync<User>(sql, new { user.ProfilePicUrl, user.Bio, user.Id });
    }
}