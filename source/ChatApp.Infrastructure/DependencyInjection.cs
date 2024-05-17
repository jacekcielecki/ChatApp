using ChatApp.Domain.Interfaces.Repositories;
using ChatApp.Infrastructure.Database.DbConnectionFactory;
using ChatApp.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChatApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration["Database:ConnectionString"];

        services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>(_ =>
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new NullReferenceException("Database:ConnectionString value cannot be null or empty");

            return new DbConnectionFactory(connectionString);
        });
        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<IChatRepository, ChatRepository>();
        return services;
    }
}