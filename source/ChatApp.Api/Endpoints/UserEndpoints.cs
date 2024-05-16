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
            var result = await userHelper.GetLoggedUser();

            return result.Match(
                user => Results.Ok(user.ToUserResponse()),
                validationErrors => Results.BadRequest(new HttpValidationProblemDetails(validationErrors.Errors))
                );
        })
        .RequireAuthorization();

        app.MapGet("/api/users/search/{searchPhrase}",
        async (IUserService userService, string searchPhrase) =>
        {
            var emails = await userService.GetEmailsBySearchPhrase(searchPhrase);
            return Results.Ok(emails);
        })
        .RequireAuthorization();
    }
}