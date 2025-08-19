using ChatApp.Shared.Data.Adapters.DbConnectionFactory;
using Dapper;

namespace ChatApp.Chats.Core.Members;

public class RemoveUsersFromGroupChatRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public RemoveUsersFromGroupChatRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task Delete(Guid groupChatId)
    {
        const string sql =
            """
            DELETE FROM group_chats_users
            WHERE group_chat_id = @groupChatId
            """;

        await using var connection = _dbConnectionFactory.Create();

        await connection.ExecuteAsync(sql, new { groupChatId });
    }
}

