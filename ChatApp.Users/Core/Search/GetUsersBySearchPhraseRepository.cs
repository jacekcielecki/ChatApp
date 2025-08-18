using ChatApp.Shared.Data.Adapters.DbConnectionFactory;
using Dapper;

namespace ChatApp.Users.Core.Search;

public class GetUsersBySearchPhraseRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GetUsersBySearchPhraseRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<string[]> Get(string searchPhrase)
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        const string sql =
            """
            SELECT email
            FROM users
            WHERE email LIKE @Phrase
            LIMIT 5;
            """;

        await using var connection = _dbConnectionFactory.Create();
        var emails = await connection.QueryAsync<string>(sql, new { Phrase = $"%{searchPhrase}%" });

        return emails.ToArray();
    }
}
