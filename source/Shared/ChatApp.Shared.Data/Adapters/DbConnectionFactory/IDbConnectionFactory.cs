using Npgsql;

namespace ChatApp.Shared.Data.Adapters.DbConnectionFactory;

public interface IDbConnectionFactory
{
    NpgsqlConnection Create();
}