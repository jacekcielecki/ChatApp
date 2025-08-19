using ChatApp.Shared.Data.Adapters.DbConnectionFactory;
using ChatApp.Shared.Data.Adapters.Entities;
using Dapper;

namespace ChatApp.Chats.Core.Private;

public class CreatePrivateChatRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public CreatePrivateChatRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<Guid?> Create(PrivateChat privateChat)
    {
        const string sql =
            """
            INSERT INTO private_chats (id, created_at, first_user_id, second_user_id)
            VALUES (@id, @created_at, @first_user_id, @second_user_id)
            RETURNING id;
            """;

        await using var connection = _dbConnectionFactory.Create();
        var chatId = await connection.QuerySingleOrDefaultAsync<Guid>(sql, new
        {
            id = privateChat.Id,
            created_at = privateChat.CreatedAt,
            first_user_id = privateChat.FirstUserId,
            second_user_id = privateChat.SecondUserId
        });

        return chatId;
    }
}
