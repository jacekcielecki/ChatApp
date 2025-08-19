using ChatApp.Shared.Data.Adapters.DbConnectionFactory;
using ChatApp.Shared.Data.Adapters.Entities;
using Dapper;

namespace ChatApp.Messages.Core;

public class GetMessagesRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GetMessagesRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<(List<Message> Items, int TotalMessagesCount)> Get(Guid chatId, int skip, int take)
    {
        const string sql =
            """
            SELECT id, chat_id, created_at, created_by_id, content
            FROM messages
            WHERE chat_id = @chatId
            ORDER BY created_at DESC
            OFFSET @skip ROWS
            FETCH NEXT @take ROWS ONLY;
            """;

        const string countSql =
            """
            SELECT COUNT(*)
            FROM messages
            WHERE chat_id = @chatId;
            """;

        await using var connection = _dbConnectionFactory.Create();

        var messages = await connection.QueryAsync<Message>(sql, new { chatId, skip, take });
        var totalMessagesCount = await connection.QuerySingleOrDefaultAsync<int>(countSql, new { chatId });

        return (messages.ToList(), totalMessagesCount);
    }
}
