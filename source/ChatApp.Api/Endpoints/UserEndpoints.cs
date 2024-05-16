using ChatApp.Application.Interfaces;
using ChatApp.Application.Mapping;

namespace ChatApp.Api.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        app.MapGet("/api/users/me",
        async (IGetLoggedUserHelper userHelper) =>
        {
            var user = await userHelper.GetLoggedUser();
            return Results.Ok(user.ToUserResponse());
        })
        .RequireAuthorization();
    }
}