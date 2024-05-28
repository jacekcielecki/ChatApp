using ChatApp.Domain.Entities;
using ChatApp.Domain.Interfaces.Repositories;
using ChatApp.Infrastructure.Database.DbConnectionFactory;
using Dapper;

namespace ChatApp.Infrastructure.Repositories;

public class GroupChatMembersRepository : IGroupChatMembersRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public GroupChatMembersRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task BulkInsert(List<User> members, Guid groupChatId)
    {
        const string sql =
            """
            COPY group_chats_users (group_chat_id, user_id, created_at)
            FROM STDIN (FORMAT BINARY)
            """;

        await using var connection = _connectionFactory.Create();

        connection.Open();
        await using var writer = await connection.BeginBinaryImportAsync(sql);

        foreach (var member in members)
        {
            await writer.StartRowAsync();
            await writer.WriteAsync(groupChatId, NpgsqlTypes.NpgsqlDbType.Uuid);
            await writer.WriteAsync(member.Id, NpgsqlTypes.NpgsqlDbType.Uuid);
            await writer.WriteAsync(DateTime.Now, NpgsqlTypes.NpgsqlDbType.Timestamp);
        }
        await writer.CompleteAsync();
    }

    public async Task DeleteByChatId(Guid groupChatId)
    {
        const string sql =
            """
            DELETE FROM group_chats_users
            WHERE group_chat_id = @groupChatId
            """;

        await using var connection = _connectionFactory.Create();

        await connection.ExecuteAsync(sql, new { groupChatId });
    }
}
