using ChatApp.Application.Interfaces;
using ChatApp.Application.Mapping;

namespace ChatApp.Api.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        app.MapGet("/api/users/{email}",
            async (IUserService userService, string email) =>
        {
            var user = await userService.GetByEmail(email);
            return user == null ? Results.NotFound() : Results.Ok(user.ToUserResponse());
        });
    }
}