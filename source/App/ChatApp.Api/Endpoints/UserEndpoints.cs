using ChatApp.Shared.Model.Users;
using ChatApp.Users.Queries;

namespace ChatApp.Api.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var api = app
            .MapGroup("/api/users")
            .WithTags("Users")
            .RequireAuthorization();

        // GET /api/users/me
        api.MapGet("/me", GetCurrentUser)
            .WithName("GetCurrentUser")
            .WithSummary("Get details of the authenticated user.")
            .Produces<UserDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

        // GET /api/users/search/{searchPhrase}
        api.MapGet("/search/{searchPhrase}", SearchUsers)
            .WithName("SearchUsers")
            .WithSummary("Search users by a search phrase (e.g., name or email).")
            .Produces<IEnumerable<UserDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

        // -------------------- Handlers --------------------

        async Task<IResult> GetCurrentUser(GetUser getUser)
        {
            var user = await getUser.Get();
            return TypedResults.Ok(user);
        }

        async Task<IResult> SearchUsers(GetUsersBySearchPhrase getUsers, string searchPhrase)
        {
            var users = await getUsers.Get(searchPhrase);
            return TypedResults.Ok(users);
        }
    }
}