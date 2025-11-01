using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Core = ChatApp.Users.Queries;

namespace ChatApp.Functions.Queries;

public class GetUsersBySearchPhrase
{
    private readonly Core.GetUsersBySearchPhrase _getUsersBySearchPhrase;

    public GetUsersBySearchPhrase(Core.GetUsersBySearchPhrase getUsersBySearchPhrase)
    {
        _getUsersBySearchPhrase = getUsersBySearchPhrase;
    }

    [Function(nameof(GetUsersBySearchPhrase))]
    public async Task<IResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get",
        Route = "users/search/{searchPhrase}")]
        HttpRequest req,
        string searchPhrase)
    {
        var users = await _getUsersBySearchPhrase.Get(searchPhrase);
        return Results.Ok(users);
    }
}