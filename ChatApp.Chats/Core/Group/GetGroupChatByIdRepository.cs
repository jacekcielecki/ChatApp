using ChatApp.Shared.Data.Adapters.DbConnectionFactory;
using ChatApp.Shared.Data.Adapters.Entities;
using Dapper;
using System.Data;

namespace ChatApp.Chats.Core.Group;

public class GetGroupChatByIdRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GetGroupChatByIdRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<GroupChat?> Get(Guid id)
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;

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

        await using var connection = _dbConnectionFactory.Create();

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
            }).FirstOrDefault();

        return result;
    }
}