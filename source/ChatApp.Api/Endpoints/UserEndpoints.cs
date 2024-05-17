using ChatApp.Application.Interfaces;
using ChatApp.Application.Mapping;
using ChatApp.Contracts.Response;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ChatApp.Api.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var userEndpoints = app.MapGroup("/api/users").WithTags("Users");

        userEndpoints.MapGet("/me",
            async (IGetLoggedUserHelper userHelper) =>
            {
                var result = await userHelper.GetLoggedUser();

                return result.Match<Results<Ok<UserResponse>, BadRequest<HttpValidationProblemDetails>>>(
                    user => TypedResults.Ok(user.ToUserResponse()),
                    validationErrors => TypedResults.BadRequest(new HttpValidationProblemDetails(validationErrors.Errors))
                    );
            })
            .RequireAuthorization();

        userEndpoints.MapGet("/search/{searchPhrase}",
            async (IUserService userService, string searchPhrase) =>
            {
                var emails = await userService.GetEmailsBySearchPhrase(searchPhrase);
                return TypedResults.Ok(emails);
            })
        .RequireAuthorization();
    }
}