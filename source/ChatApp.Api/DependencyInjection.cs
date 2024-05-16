using ChatApp.Api.Helpers;
using ChatApp.Application.Interfaces;

namespace ChatApp.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        services.AddTransient<IGetLoggedUserHelper, GetLoggedUserHelper>();
        return services;
    }
}
