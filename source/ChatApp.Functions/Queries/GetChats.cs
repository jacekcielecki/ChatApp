using ChatApp.Users.Core.Details;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Core = ChatApp.Chats.Queries;

namespace ChatApp.Functions.Queries;

public class GetChats
{
    private readonly ILoggedUserProvider _loggedUserProvider;
    private readonly Core.GetChats _getChats;

    public GetChats(ILoggedUserProvider loggedUserProvider, Core.GetChats getChats)
    {
        _loggedUserProvider = loggedUserProvider;
        _getChats = getChats;
    }

    [Function(nameof(GetChats))]
    public async Task<IResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        var user = await _loggedUserProvider.Get();
        var chats = await _getChats.Get(user.Id);

        return Results.Ok(chats);
    }
}