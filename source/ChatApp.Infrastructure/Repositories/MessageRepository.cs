using ChatApp.Domain.Entities;
using ChatApp.Domain.Interfaces.Repositories;
using ChatApp.Infrastructure.Database.DbConnectionFactory;
using Dapper;

namespace ChatApp.Infrastructure.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public MessageRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Message?> GetById(Guid id)
    {
        const string sql =
            """
            SELECT id, chat_id, created_at, created_by_id, content
            FROM messages
            WHERE id = @id
            """;

        await using var connection = _connectionFactory.Create();

        var message = await connection.QueryFirstOrDefaultAsync<Message>(sql, new { id });
        return message;
    }

    public async Task<Guid?> Insert(Message message)
    {
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        const string sql =
            """
            INSERT INTO messages (id, chat_id, created_at, created_by_id, content)
            VALUES (@id, @chat_id, @created_at, @created_by_id, @content)
            RETURNING id
            """;

        await using var connection = _connectionFactory.Create();

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

    public async Task Update(Message message)
    {
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        const string sql =
            """
            UPDATE messages
            SET content = @content
            WHERE id = @id
            """;

        await using var connection = _connectionFactory.Create();

        await connection.ExecuteScalarAsync(sql, new { content = message.Content, id = message.Id });
    }

    public async Task Delete(Guid id)
    {
        const string sql = "DELETE FROM messages WHERE id = @id";

        await using var connection = _connectionFactory.Create();

        await connection.ExecuteAsync(sql, new { id });
    }
}
