using Npgsql;

namespace ChatApp.Infrastructure.Database.DbConnectionFactory;

public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public NpgsqlConnection Create()
    {
        return new NpgsqlConnection(_connectionString);
    }
}