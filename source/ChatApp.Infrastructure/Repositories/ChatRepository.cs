using ChatApp.Contracts.Request;
using ChatApp.Domain.Interfaces.Repositories;
using ChatApp.Infrastructure.Database.DbConnectionFactory;
using Dapper;

namespace ChatApp.Infrastructure.Repositories;

internal class ChatRepository : IChatRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ChatRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Guid?> CreateGroup(CreateGroupChatRequest request, Guid creatorId)
    {
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        const string sql =
            """
            INSERT INTO group_chats (name, created_at, created_by_id)
            VALUES (@name, @created_at, @created_by_id)
            RETURNING id;
            """;

        using var connection = _connectionFactory.Create();
        var chatId = await connection.QuerySingleOrDefaultAsync<Guid?>(sql, new
        {
            name = request.Name,
            created_at = DateTime.Now,
            created_by_id = creatorId
        });

        return chatId;
    }
}
