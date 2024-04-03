using Npgsql;
using System.Data;

namespace ChatApp.Infrastructure.Database.DbConnectionFactory
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        public IDbConnection Create()
        {
            var connectionString = "";
            return new NpgsqlConnection(connectionString);
        }
    }
}
