using ChatApp.Application.Interfaces;
using ChatApp.Application.Mapping;

namespace ChatApp.Api.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var userEndpoints = app.MapGroup("/api/users").WithTags("Users");

        userEndpoints.MapGet("/me",
            async (IGetLoggedUserHelper userHelper) =>
            {
                var user = await userHelper.GetLoggedUser();
                return TypedResults.Ok(user.ToUserResponse());
            })
            .RequireAuthorization();

        userEndpoints.MapGet("/search/{searchPhrase}",
            async (IUserHandler userHandler, string searchPhrase) =>
            {
                var emails = await userHandler.GetEmailsBySearchPhrase(searchPhrase);
                return TypedResults.Ok(emails);
            })
            .RequireAuthorization();
    }
}