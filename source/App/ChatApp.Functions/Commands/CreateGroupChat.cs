using ChatApp.Functions.Authorization;
using ChatApp.Shared.Model.Chats;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Core = ChatApp.Chats.Commands;

namespace ChatApp.Functions.Commands;

public class CreateGroupChat
{
    private readonly Core.CreateGroupChat _createGroupChat;

    public CreateGroupChat(Core.CreateGroupChat createGroupChat)
    {
        _createGroupChat = createGroupChat;
    }

    [Function(nameof(CreateGroupChat))]
    public async Task<Results<Ok<Guid?>, BadRequest<HttpValidationProblemDetails>>> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "chats/group")] HttpRequest req,
        [FromBody] GroupChatCreateApiDto dto)
    {
        var result = await _createGroupChat.Create(dto, req.User().Id);

        var response = result.Match<Results<Ok<Guid?>, BadRequest<HttpValidationProblemDetails>>>(
            success => TypedResults.Ok(success.Value),
            errors => TypedResults.BadRequest(new HttpValidationProblemDetails(errors.Errors))
        );

        return response;
    }
}