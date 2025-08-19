using ChatApp.Messages.Commands;
using ChatApp.Messages.Core;
using ChatApp.Messages.Core.Create;
using ChatApp.Messages.Core.Delete;
using ChatApp.Messages.Core.Details;
using ChatApp.Messages.Core.Update;
using ChatApp.Messages.Queries;
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
        services.AddTransient<DeleteMessageById>();
        services.AddTransient<DeleteMessageByIdRepository>();
        services.AddTransient<GetMessageByIdRepository>();
        services.AddTransient<CreateGroupChatMessage>();
        services.AddTransient<UpdateMessage>();
        services.AddTransient<UpdateMessageRepository>();
        services.AddTransient<GetMessagesRepository>();
        services.AddTransient<GetMessages>();

        return services;
    }
}
