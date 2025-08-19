using ChatApp.Shared.Data.Adapters.DbConnectionFactory;
using Dapper;

namespace ChatApp.Messages.Core.Details;

public class DeleteMessageByIdRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public DeleteMessageByIdRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task Delete(Guid id)
    {
        const string sql = "DELETE FROM messages WHERE id = @id";

        await using var connection = _dbConnectionFactory.Create();

        await connection.ExecuteAsync(sql, new { id });
    }
}
