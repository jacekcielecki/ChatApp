using ChatApp.Functions.Authorization;
using ChatApp.Shared.Model.Chats;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Core = ChatApp.Chats.Commands;

namespace ChatApp.Functions.Commands;

public class UpdateGroupChat
{ 
    private readonly Core.UpdateGroupChat _updateGroupChat;

    public UpdateGroupChat(Core.UpdateGroupChat updateGroupChat)
    { 
        _updateGroupChat = updateGroupChat;
    }

    [Function(nameof(UpdateGroupChat))]
    public async Task<IResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "chats/group")] HttpRequest req, [FromBody] GroupChatUpdateApiDto dto)
    {
        var result = await _updateGroupChat.Update(dto, req.User().Id);

        var response = result.Match<Results<Ok, NotFound, ForbidHttpResult, BadRequest<HttpValidationProblemDetails>>>(
            _ => TypedResults.Ok(),
            _ => TypedResults.NotFound(),
            _ => TypedResults.Forbid(),
            err => TypedResults.BadRequest(new HttpValidationProblemDetails(err.Errors))
        );

        return response;
    }
}