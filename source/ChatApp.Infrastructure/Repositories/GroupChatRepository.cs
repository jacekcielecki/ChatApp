using ChatApp.Domain.Entities;
using ChatApp.Domain.Interfaces.Repositories;
using ChatApp.Infrastructure.Database.DbConnectionFactory;
using Dapper;
using System.Data;

namespace ChatApp.Infrastructure.Repositories;

public class GroupChatRepository : IGroupChatRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IGroupChatMembersRepository _groupChatMembersRepository;

    public GroupChatRepository(IDbConnectionFactory connectionFactory, IGroupChatMembersRepository groupChatMembersRepository)
    {
        _connectionFactory = connectionFactory;
        _groupChatMembersRepository = groupChatMembersRepository;
    }

    public async Task<IEnumerable<GroupChat>> Get(Guid userId)
    {
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        const string sql =
            """
            SELECT
             gc.id, gc.name, gc.created_at, gc.created_by_id,
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

        var groupChats = await connection.QueryAsync<GroupChat, User?, Message?, GroupChat>
        (sql, (groupChat, member, message) =>
        {
            groupChat.Members = [];
            if (member != null)
            {
                groupChat.Members.Add(member);
            }
            groupChat.Messages = [];
            if (message != null)
            {
                groupChat.Messages.Add(message);
            }
            return groupChat;
        },new { UserId = userId }, splitOn: "id,id", commandType: CommandType.Text);

        var result = groupChats.GroupBy(x => x.Id)
            .Select(y =>
        {
            var groupChat = y.First();
            if (groupChat.Members.Count != 0)
            {
                groupChat.Members = y.Select(x => x.Members.Single()).ToList();
            }
            if (groupChat.Messages.Count != 0)
            {
                groupChat.Messages = y.Select(x => x.Messages.Single()).ToList();
            }
            groupChat.Members = groupChat.Members.GroupBy(u => u.Id).Select(g => g.First()).ToList();
            groupChat.Messages = groupChat.Messages.GroupBy(u => u.Id).Select(g => g.First()).ToList();
            return groupChat;
        });

        return result;
    }

    public async Task<GroupChat?> GetById(Guid id)
    {
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        const string sql =
            """
            SELECT
             gc.id, gc.name, gc.created_at, gc.created_by_id,
             u.id, u.email, u.created_at
            FROM group_chats gc
            LEFT JOIN group_chats_users gcu ON gcu.group_chat_id = gc.id
            LEFT JOIN users u ON u.id = gcu.user_id
            WHERE gc.id = @id;
            """;

        await using var connection = _connectionFactory.Create();

        var chat = await connection.QueryAsync<GroupChat, User?, GroupChat>
        (sql, (groupChat, member) =>
        {
            groupChat.Members = [];
            if (member != null)
            {
                groupChat.Members.Add(member);
            }

            return groupChat;
        }, new { id }, splitOn: "id", commandType: CommandType.Text);

        var result = chat.GroupBy(x => x.Id)
            .Select(y =>
        {
            var groupChat = y.First();
            if (groupChat.Members.Count != 0)
            {
                groupChat.Members = y.Select(x => x.Members.Single()).ToList();
            }

            groupChat.Members = groupChat.Members.GroupBy(u => u.Id).Select(g => g.First()).ToList();
            return groupChat;
        });

        return result.FirstOrDefault();
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

        await _groupChatMembersRepository.BulkInsert(groupChat.Members, groupChat.Id);

        return chatId;
    }

    public async Task Update(GroupChat groupChat)
    {
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        const string sql =
            """
            UPDATE group_chats
            SET name = @name
            WHERE id = @id;
            """;

        await using var connection = _connectionFactory.Create();

        await connection.ExecuteScalarAsync(sql, new { name = groupChat.Name, id = groupChat.Id });

        await _groupChatMembersRepository.DeleteByChatId(groupChat.Id);
        await _groupChatMembersRepository.BulkInsert(groupChat.Members, groupChat.Id);
    }
}
