using ChatApp.Shared.Data.Adapters.DbConnectionFactory;
using ChatApp.Shared.Data.Adapters.Entities;
using Dapper;

namespace ChatApp.Messages.Core.Details;

public class UpdateMessageRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public UpdateMessageRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task Update(Message message)
    { 
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        const string sql =
            """
            UPDATE messages
            SET content = @content
            WHERE id = @id
            """;

        await using var connection = _dbConnectionFactory.Create();

        await connection.ExecuteScalarAsync(sql, new { content = message.Content, id = message.Id });
    }
}
