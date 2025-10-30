using ChatApp.Shared.Data.Adapters.DbConnectionFactory;
using ChatApp.Shared.Data.Adapters.Entities;
using Dapper;
using System.Data;

namespace ChatApp.Chats.Core.Group;

public class GetGroupChatsRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GetGroupChatsRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<IEnumerable<GroupChat>> Get(Guid userId)
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        const string sql = """
            SELECT
                gc.id, gc.name, gc.created_at, gc.created_by_id,
                u.id, u.email, u.given_name, u.family_name, u.profile_picture_url, u.bio, u.created_at,
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
            ORDER BY gc.created_at DESC, me.created_at ASC
            """;

        await using var connection = _dbConnectionFactory.Create();

        var groupChats = await connection.QueryAsync<GroupChat, User?, Message?, GroupChat>(
            sql,
            (groupChat, member, message) =>
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
            },
            new { UserId = userId },
            splitOn: "id,id",
            commandType: CommandType.Text
        );

        var result = groupChats
            .GroupBy(x => x.Id)
            .Select(g =>
            {
                var chat = g.First();

                chat.Members = g.SelectMany(x => x.Members)
                    .GroupBy(m => m.Id)
                    .Select(grp => grp.First())
                    .ToList();

                chat.Messages = g.SelectMany(x => x.Messages)
                    .GroupBy(m => m.Id)
                    .Select(grp => grp.First())
                    .OrderByDescending(m => m.CreatedAt)
                    .ToList();

                return chat;
            });

        return result;
    }
}
