using ChatApp.Shared.Data.Adapters.DbConnectionFactory;
using ChatApp.Shared.Data.Adapters.Entities;
using Dapper;

namespace ChatApp.Chats.Core.Private;

public class GetPrivateChatsRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GetPrivateChatsRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<IEnumerable<PrivateChat>> Get(Guid userId)
    {
        const string sql =
            """
            SELECT pc.id, pc.created_at, pc.first_user_id, pc.second_user_id,
             me.id, me.chat_id, me.created_at, me.created_by_id, me.content,
             u.id, u.email, u.given_name, u.family_name, u.created_at
            FROM private_chats pc
            LEFT JOIN messages me ON me.chat_id = pc.id
            LEFT JOIN users u ON u.id = 
                (CASE WHEN pc.first_user_id = @userId
                    THEN pc.second_user_id
                    ELSE pc.first_user_id
                END)
            WHERE pc.first_user_id = @userId OR pc.second_user_id = @userId
            ORDER BY pc.created_at DESC
            """;

        await using var connection = _dbConnectionFactory.Create();

        var privateChats = await connection.QueryAsync<PrivateChat, Message?, User, PrivateChat>(sql, (chat, message, receiver) =>
        {
            chat.Messages = [];
            if (message != null)
            {
                chat.Messages.Add(message);
            }
            chat.Receiver = receiver;
            return chat;
        }, new { userId }, splitOn: "id");

        var result = privateChats.GroupBy(x => x.Id).Select(y =>
        {
            var single = y.First();
            if (single.Messages.Count != 0)
            {
                single.Messages = y.Select(x => x.Messages.Single()).ToList();
            }
            return single;
        });

        return result;
    }
}
