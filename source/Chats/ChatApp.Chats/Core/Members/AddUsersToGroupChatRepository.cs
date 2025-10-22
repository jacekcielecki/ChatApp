using ChatApp.Shared.Data.Adapters.DbConnectionFactory;

namespace ChatApp.Chats.Core.Members;

public class AddUsersToGroupChatRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public AddUsersToGroupChatRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task Add(List<Guid> members, Guid chatId)
    {
        const string sql =
            """
            COPY group_chats_users (group_chat_id, user_id, created_at)
            FROM STDIN (FORMAT BINARY)
            """;

        await using var connection = _dbConnectionFactory.Create();

        connection.Open();
        await using var writer = await connection.BeginBinaryImportAsync(sql);

        foreach (var memberId in members)
        {
            await writer.StartRowAsync();
            await writer.WriteAsync(chatId, NpgsqlTypes.NpgsqlDbType.Uuid);
            await writer.WriteAsync(memberId, NpgsqlTypes.NpgsqlDbType.Uuid);
            await writer.WriteAsync(DateTime.Now, NpgsqlTypes.NpgsqlDbType.Timestamp);
        }
        await writer.CompleteAsync();
    }
}
