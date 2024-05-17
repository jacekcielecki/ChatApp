using Npgsql;

namespace ChatApp.Infrastructure.Database.DbConnectionFactory;

public interface IDbConnectionFactory
{
    NpgsqlConnection Create();
}