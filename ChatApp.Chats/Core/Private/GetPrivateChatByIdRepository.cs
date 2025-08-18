using ChatApp.Shared.Data.Adapters.DbConnectionFactory;
using ChatApp.Shared.Data.Adapters.Entities;
using Dapper;

namespace ChatApp.Chats.Core.Private;

public class GetPrivateChatByIdRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GetPrivateChatByIdRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<PrivateChat?> Get(Guid id)
    {
        const string sql =
            """
            SELECT id, created_at, first_user_id, second_user_id
            FROM private_chats
            WHERE id = @id
            """;

        await using var connection = _dbConnectionFactory.Create();
        var chat = await connection.QuerySingleOrDefaultAsync<PrivateChat>(sql, new { id });

        return chat;
    }
}
