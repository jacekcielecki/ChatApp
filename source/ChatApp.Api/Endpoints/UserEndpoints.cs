using ChatApp.Users.Queries;

namespace ChatApp.Api.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var userEndpoints = app
            .MapGroup("/api/users")
            .WithTags("Users");

        userEndpoints.MapGet("/me",
            async (GetUser getUser) =>
            {
                var user = await getUser.Get();
                return Results.Ok(user);
            })
            .RequireAuthorization();

        userEndpoints.MapGet("/search/{searchPhrase}",
            async (GetUsersBySearchPhrase getUsers, string searchPhrase) =>
            {
                var users = await getUsers.Get(searchPhrase);
                return Results.Ok(users);
            })
            .RequireAuthorization();
    }
}