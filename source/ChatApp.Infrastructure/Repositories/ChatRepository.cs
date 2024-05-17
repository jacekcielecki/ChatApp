using ChatApp.Domain.Entities;
using ChatApp.Domain.Interfaces.Repositories;
using ChatApp.Infrastructure.Database.DbConnectionFactory;
using Dapper;
using System.Data;

namespace ChatApp.Infrastructure.Repositories;

internal class ChatRepository : IChatRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ChatRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<GroupChat>> GetGroupChats(Guid userId)
    {
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        const string sql =
            """
            SELECT gc.id, gc.name, gc.created_at, gc.created_by_id, u.id, u.email, u.created_at
            FROM group_chats gc
            LEFT JOIN group_chats_users gcu ON gcu.group_chat_id = gc.id
            LEFT JOIN users u ON u.id = gcu.user_id
            WHERE gc.id IN (
                SELECT gcu_inner.group_chat_id
                FROM group_chats_users gcu_inner
                WHERE gcu_inner.user_id = @UserId
            )
            ORDER BY gc.created_at DESC
            """;

        await using var connection = _connectionFactory.Create();

        var groupChats = await connection.QueryAsync<GroupChat, User?, GroupChat>(sql, (groupChat, member) =>
        {
            groupChat.Members = [];
            if (member != null)
            {
                groupChat.Members.Add(member);
            }
            return groupChat;
        },new { UserId = userId }, splitOn: "Id", commandType: CommandType.Text);

        var result = groupChats.GroupBy(x => x.Id).Select(y =>
        {
            var single = y.First();
            if (single.Members.Count != 0)
            {
                single.Members = y.Select(x => x.Members.Single()).ToList();
            }
            return single;
        });

        return result;
    }

    public async Task<Guid?> CreateGroup(string name, List<Guid> members, Guid creatorId)
    {
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        const string sql =
            """
            INSERT INTO group_chats (name, created_at, created_by_id)
            VALUES (@name, @created_at, @created_by_id)
            RETURNING id;
            """;

        await using var connection = _connectionFactory.Create();
        var chatId = await connection.QuerySingleOrDefaultAsync<Guid>(sql, new
        {
            name = name,
            created_at = DateTime.Now,
            created_by_id = creatorId
        });

        await BulkInsertGroupChatMembers(members, chatId);

        return chatId;
    }

    private async Task BulkInsertGroupChatMembers(List<Guid> userIds, Guid groupChatId)
    {
        await using var connection = _connectionFactory.Create();
        connection.Open();

        const string sql = "COPY group_chats_users (group_chat_id, user_id, created_at) FROM STDIN (FORMAT BINARY)";

        await using var writer = await connection.BeginBinaryImportAsync(sql);

        foreach (var userId in userIds)
        {
            await writer.StartRowAsync();
            await writer.WriteAsync(groupChatId, NpgsqlTypes.NpgsqlDbType.Uuid);
            await writer.WriteAsync(userId, NpgsqlTypes.NpgsqlDbType.Uuid);
            await writer.WriteAsync(DateTime.Now, NpgsqlTypes.NpgsqlDbType.Timestamp);
        }
        await writer.CompleteAsync();
    }
}
