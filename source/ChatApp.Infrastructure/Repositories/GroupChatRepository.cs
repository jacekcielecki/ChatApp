using ChatApp.Domain.Entities;
using ChatApp.Domain.Interfaces.Repositories;
using ChatApp.Infrastructure.Database.DbConnectionFactory;
using Dapper;
using System.Data;

namespace ChatApp.Infrastructure.Repositories;

internal class GroupChatRepository : IGroupChatRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public GroupChatRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<GroupChat>> Get(Guid userId)
    {
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        const string sql =
            """
            SELECT gc.id, gc.name, gc.created_at, gc.created_by_id,
             u.id, u.email, u.created_at,
             me.id, me.chat_id, me.created_at, me.created_by_id, me.content
            FROM group_chats gc
            LEFT JOIN group_chats_users gcu ON gcu.group_chat_id = gc.id
            LEFT JOIN users u ON u.id = gcu.user_id
            LEFT JOIN messages me ON me.chat_id = gc.id
            WHERE gc.id IN (
                SELECT gcu_inner.group_chat_id
                FROM group_chats_users gcu_inner
                WHERE gcu_inner.user_id = @UserId
            )
            ORDER BY gc.created_at DESC
            """;

        await using var connection = _connectionFactory.Create();

        var groupChats = await connection.QueryAsync<GroupChat, User?, Message?, GroupChat>(sql, (groupChat, member, message) =>
        {
            groupChat.Members = [];
            groupChat.Messages = [];
            if (member != null)
            {
                groupChat.Members.Add(member);
            }
            if (message != null)
            {
                groupChat.Messages.Add(message);
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
            if (single.Messages.Count != 0)
            {
                single.Messages = y.Select(x => x.Messages.Single()).ToList();
            }
            return single;
        });

        return result;
    }

    public async Task<GroupChat?> GetById(Guid id)
    {
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        const string sql =
            """
            SELECT id, name, created_at, created_by_id
            FROM group_chats
            WHERE group_chats.id = @id;
            """;

        await using var connection = _connectionFactory.Create();
        var chat = await connection.QuerySingleOrDefaultAsync<GroupChat>(sql, new { id });

        return chat;
    }

    public async Task<Guid?> Insert(GroupChat groupChat)
    {
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        const string sql =
            """
            INSERT INTO group_chats (id, name, created_at, created_by_id)
            VALUES (@id, @name, @created_at, @created_by_id)
            RETURNING id;
            """;

        await using var connection = _connectionFactory.Create();
        var chatId = await connection.QuerySingleOrDefaultAsync<Guid>(sql, new
        {
            id = groupChat.Id,
            name = groupChat.Name,
            created_at = groupChat.CreatedAt,
            created_by_id = groupChat.CreatedById
        });

        await BulkInsertGroupChatMembers(groupChat.Members, groupChat.Id);

        return chatId;
    }

    private async Task BulkInsertGroupChatMembers(List<User> members, Guid groupChatId)
    {
        await using var connection = _connectionFactory.Create();
        connection.Open();

        const string sql = "COPY group_chats_users (group_chat_id, user_id, created_at) FROM STDIN (FORMAT BINARY)";

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
}
