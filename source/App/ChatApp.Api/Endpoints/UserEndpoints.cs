using ChatApp.Users.Queries;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Api.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var userEndpoints = app
            .MapGroup("/api/users")
            .WithTags("Users");

        userEndpoints.MapGet("/me",
            async ([FromServices] GetUser getUser) =>
            {
                var user = await getUser.Get();
                return Results.Ok(user);
            })
            .RequireAuthorization();

        userEndpoints.MapGet("/search/{searchPhrase}",
            async ([FromServices] GetUsersBySearchPhrase getUsers, string searchPhrase) =>
            {
                var users = await getUsers.Get(searchPhrase);
                return Results.Ok(users);
            })
            .RequireAuthorization();
    }
}