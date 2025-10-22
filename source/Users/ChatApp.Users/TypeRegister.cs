using ChatApp.Shared.Data;
using ChatApp.Shared.Data.Adapters.DbConnectionFactory;
using ChatApp.Users.Core.Details;
using ChatApp.Users.Core.Summary;
using ChatApp.Users.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace ChatApp.Users;

public static class TypeRegister
{
    public static IServiceCollection RegisterUsersCore(this IServiceCollection services)
    {
        var dbConnectionString = Environment.GetEnvironmentVariable(Envars.DatabaseConnectionString);
        services.AddTransient<IDbConnectionFactory, DbConnectionFactory>(_ =>
        {
            if (string.IsNullOrWhiteSpace(dbConnectionString))
                throw new NullReferenceException(nameof(Envars.DatabaseConnectionString));

            return new DbConnectionFactory(dbConnectionString);
        });

        services.AddTransient<GetUserByEmailRepository>();
        services.AddTransient<GetUserByIdRepository>();
        services.AddTransient<GetUsersBySearchPhrase>();
        services.AddTransient<GetUsersBySearchPhraseRepository>();
        services.AddTransient<GetUser>();
        services.AddTransient<CreateUserRepository>();
        services.AddTransient<GetOrCreateUserFromClaims>();

        return services;
    }
}
