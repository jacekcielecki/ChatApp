using ChatApp.Chats.Commands;
using ChatApp.Chats.Core.Group;
using ChatApp.Chats.Core.Members;
using ChatApp.Chats.Core.Private;
using ChatApp.Chats.Queries;
using ChatApp.Shared.Data;
using ChatApp.Shared.Data.Adapters.DbConnectionFactory;
using Microsoft.Extensions.DependencyInjection;

namespace ChatApp.Chats;

public static class TypeRegister
{
    public static IServiceCollection RegisterChatsCore(this IServiceCollection services)
    {
        var dbConnectionString = Environment.GetEnvironmentVariable(Envars.DatabaseConnectionString);
        services.AddTransient<IDbConnectionFactory, DbConnectionFactory>(_ =>
        {
            if (string.IsNullOrWhiteSpace(dbConnectionString))
                throw new NullReferenceException(nameof(Envars.DatabaseConnectionString));

            return new DbConnectionFactory(dbConnectionString);
        });

        services.AddTransient<GetChats>();
        services.AddTransient<GetPrivateChatsRepository>();
        services.AddTransient<GetGroupChatsRepository>();
        services.AddTransient<GetGroupChatByIdRepository>();
        services.AddTransient<GetPrivateChatByIdRepository>();
        services.AddTransient<GetUserByIdRepository>();
        services.AddTransient<CreateGroupChat>();
        services.AddTransient<CreateGroupChatRepository>();
        services.AddTransient<AddUsersToGroupChatRepository>();
        services.AddTransient<CreatePrivateChatRepository>();
        services.AddTransient<CreatePrivateChat>();
        services.AddTransient<UpdateGroupChatRepository>();
        services.AddTransient<UpdateGroupChat>();
        services.AddTransient<RemoveUsersFromGroupChatRepository>();

        return services;
    }
}