using ChatApp.Application.Interfaces;

namespace ChatApp.Api.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        app.MapGet("/api/users/{name}", async (IUserService userService, string name) =>
        {
            var user = await userService.GetByName(name);
            return user == null ? Results.NotFound() : Results.Ok(user);
        });
    }
}