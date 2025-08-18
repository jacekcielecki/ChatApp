using ChatApp.Messages.Commands;
using ChatApp.Messages.Core.Create;
using ChatApp.Shared.Data;
using ChatApp.Shared.Data.Adapters.DbConnectionFactory;
using Microsoft.Extensions.DependencyInjection;

namespace ChatApp.Messages;

public static class TypeRegister
{
    public static IServiceCollection RegisterMessagesCore(this IServiceCollection services)
    {
        var dbConnectionString = Environment.GetEnvironmentVariable(Envars.DatabaseConnectionString);
        services.AddTransient<IDbConnectionFactory, DbConnectionFactory>(_ =>
        {
            if (string.IsNullOrWhiteSpace(dbConnectionString))
                throw new NullReferenceException(nameof(Envars.DatabaseConnectionString));

            return new DbConnectionFactory(dbConnectionString);
        });

        services.AddTransient<CreateMessageRepository>();
        services.AddTransient<CreatePrivateChatMessage>();

        return services;
    }
}
