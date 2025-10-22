using ChatApp.Functions.Authorization;
using ChatApp.Shared.Model.Chats;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Core = ChatApp.Chats.Commands;

namespace ChatApp.Functions.Commands;

public class CreatePrivateChat
{
    private readonly Core.CreatePrivateChat _createPrivateChat;

    public CreatePrivateChat(Core.CreatePrivateChat createPrivateChat)
    {
        _createPrivateChat = createPrivateChat;
    }

    [Function(nameof(CreatePrivateChat))]
    public async Task<IResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req, [FromBody] CreatePrivateChatRequest request)
    {
        var result = await _createPrivateChat.Create(request, req.User().Id);

        var response = result.Match<Results<Ok<Guid?>, BadRequest<HttpValidationProblemDetails>>>(
            success => TypedResults.Ok(success.Value),
            errors => TypedResults.BadRequest(new HttpValidationProblemDetails(errors.Errors)));

        return response;
    }
}