using ChatApp.Application.Interfaces;
using ChatApp.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ChatApp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddTransient<IUserHandler, UserHandler>();
        services.AddTransient<IChatHandler, ChatHandler>();
        services.AddTransient<IMessageHandler, MessageHandler>();
        return services;
    }
}