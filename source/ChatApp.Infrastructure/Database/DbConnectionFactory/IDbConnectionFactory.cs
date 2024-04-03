using System.Data;

namespace ChatApp.Infrastructure.Database.DbConnectionFactory
{
    public interface IDbConnectionFactory
    {
        IDbConnection Create();
    }
}
