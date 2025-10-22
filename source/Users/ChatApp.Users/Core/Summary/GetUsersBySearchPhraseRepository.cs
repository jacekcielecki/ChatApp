using ChatApp.Shared.Data.Adapters.DbConnectionFactory;
using ChatApp.Shared.Model.Users;
using Dapper;

namespace ChatApp.Users.Core.Summary;

public class GetUsersBySearchPhraseRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GetUsersBySearchPhraseRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<IEnumerable<UserSummaryDto>> Get(string searchPhrase)
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        const string sql =
            """
           SELECT id, email, given_name, family_name
           FROM users
           WHERE 
               email ILIKE @Phrase OR 
               given_name ILIKE @Phrase OR 
               family_name ILIKE @Phrase
           ORDER BY created_at DESC
           LIMIT 15;
           """;

        await using var connection = _dbConnectionFactory.Create();
        var summaries = await connection.QueryAsync<UserSummaryDto>(sql, new { Phrase = $"%{searchPhrase}%" });

        return summaries;
    }
}
