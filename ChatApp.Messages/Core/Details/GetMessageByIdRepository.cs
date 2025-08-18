using ChatApp.Shared.Data.Adapters.DbConnectionFactory;
using ChatApp.Shared.Data.Adapters.Entities;
using Dapper;

namespace ChatApp.Messages.Core.Details;

public class GetMessageByIdRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GetMessageByIdRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<Message?> Get(Guid id)
    {
        const string sql =
            """
            SELECT id, chat_id, created_at, created_by_id, content
            FROM messages
            WHERE id = @id
            """;

        await using var connection = _dbConnectionFactory.Create();

        var message = await connection.QueryFirstOrDefaultAsync<Message>(sql, new { id });
        return message;
    }
}
