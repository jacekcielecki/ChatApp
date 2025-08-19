using ChatApp.Shared.Data.Adapters.DbConnectionFactory;
using ChatApp.Shared.Data.Adapters.Entities;
using Dapper;

namespace ChatApp.Messages.Core.Details;

public class CreateMessageRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public CreateMessageRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<Guid?> Create(Message message)
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        const string sql =
            """
            INSERT INTO messages (id, chat_id, created_at, created_by_id, content)
            VALUES (@id, @chat_id, @created_at, @created_by_id, @content)
            RETURNING id
            """;

        await using var connection = _dbConnectionFactory.Create();

        var messageId = await connection.QuerySingleOrDefaultAsync<Guid>(sql, new
        {
            id = message.Id,
            chat_id = message.ChatId,
            created_at = message.CreatedAt,
            created_by_id = message.CreatedById,
            content = message.Content
        });

        return messageId;
    }
}
