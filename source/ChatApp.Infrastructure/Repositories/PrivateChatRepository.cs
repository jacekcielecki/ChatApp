using ChatApp.Domain.Entities;
using ChatApp.Domain.Interfaces.Repositories;
using ChatApp.Infrastructure.Database.DbConnectionFactory;
using Dapper;

namespace ChatApp.Infrastructure.Repositories;

public class PrivateChatRepository : IPrivateChatRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public PrivateChatRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<IEnumerable<PrivateChat>> Get(Guid userId)
    {
        const string sql =
            """
            SELECT pc.id, pc.name, pc.created_at, pc.first_user_id, pc.second_user_id,
             me.id, me.chat_id, me.created_at, me.created_by_id, me.content
            FROM private_chats pc
            LEFT JOIN messages me ON me.chat_id = pc.id
            WHERE pc.first_user_id = @userId OR pc.second_user_id = @userId
            ORDER BY pc.created_at DESC
            """;

        await using var connection = _dbConnectionFactory.Create();
        var privateChats = await connection.QueryAsync<PrivateChat, Message?, PrivateChat>(sql, (chat, message) =>
        {
            chat.Messages = [];
            if (message != null)
            {
                chat.Messages.Add(message);
            }
            return chat;
        } ,new { userId });

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

    public async Task<PrivateChat?> GetByUserId(Guid receiverId, Guid userId)
    {
        const string sql =
            """
            SELECT id, name, created_at, first_user_id, second_user_id
            FROM private_chats pc
            WHERE pc.first_user_id = @userId OR pc.second_user_id = @userId
            AND pc.first_user_id = @receiverId OR pc.second_user_id = @receiverId
            """;

        await using var connection = _dbConnectionFactory.Create();
        var chat = await connection.QuerySingleOrDefaultAsync<PrivateChat>(sql, new { receiverId, userId });

        return chat;
    }

    public async Task<PrivateChat?> Insert(PrivateChat privateChat)
    {
        const string sql =
            """
            INSERT INTO private_chats (id, name, created_at, first_user_id, second_user_id)
            VALUES (@id, @name, @created_at, @first_user_id, @second_user_id)
            RETURNING id, name, created_at, first_user_id, second_user_id;
            """;

        await using var connection = _dbConnectionFactory.Create();
        var chat = await connection.QuerySingleOrDefaultAsync<PrivateChat>(sql, new
        {
            id = privateChat.Id,
            name = "name holder",
            created_at = privateChat.CreatedAt,
            first_user_id = privateChat.FirstUserId,
            second_user_id = privateChat.SecondUserId
        });

        return chat;
    }
}
