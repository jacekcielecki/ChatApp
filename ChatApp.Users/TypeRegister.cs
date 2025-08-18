using ChatApp.Shared.Data;
using ChatApp.Shared.Data.Adapters.DbConnectionFactory;
using ChatApp.Users.Core.Details;
using Microsoft.Extensions.DependencyInjection;

namespace ChatApp.Users;

public static class TypeRegister
{
    public static void RegisterUsersCore(this IServiceCollection services)
    {
        var dbConnectionString = Environment.GetEnvironmentVariable(Envars.DatabaseConnectionString);
        services.AddTransient<IDbConnectionFactory, DbConnectionFactory>(_ =>
        {
            if (string.IsNullOrWhiteSpace(dbConnectionString))
                throw new NullReferenceException(nameof(Envars.DatabaseConnectionString));

            return new DbConnectionFactory(dbConnectionString);
        });

        services.AddTransient<GetUserByEmail>();
        services.AddTransient<GetUserByEmailRepository>();
        services.AddTransient<ILoggedUserProvider, DummyLoggedUserProvider>();
    }
}
