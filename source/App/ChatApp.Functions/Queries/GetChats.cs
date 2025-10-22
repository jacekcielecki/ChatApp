using ChatApp.Functions.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Core = ChatApp.Chats.Queries;

namespace ChatApp.Functions.Queries;

public class GetChats
{
    private readonly Core.GetChats _getChats;

    public GetChats(Core.GetChats getChats)
    {
        _getChats = getChats;
    }

    [Function(nameof(GetChats))]
    public async Task<IResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
    {
        var chats = await _getChats.Get(req.User().Id);
        return Results.Ok(chats);
    }
}