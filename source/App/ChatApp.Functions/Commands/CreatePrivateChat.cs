using ChatApp.Shared.Model.Chats;
using ChatApp.Users.Core.Details;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Core = ChatApp.Chats.Commands;

namespace ChatApp.Functions.Commands;

public class CreatePrivateChat
{
    private readonly ILoggedUserProvider _loggedUserProvider;
    private readonly Core.CreatePrivateChat _createPrivateChat;

    public CreatePrivateChat(ILoggedUserProvider loggedUserProvider, Core.CreatePrivateChat createPrivateChat)
    {
        _loggedUserProvider = loggedUserProvider;
        _createPrivateChat = createPrivateChat;
    }

    [Function(nameof(CreatePrivateChat))]
    public async Task<IResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req, [FromBody] CreatePrivateChatRequest request)
    {
        var user = await _loggedUserProvider.Get();

        var result = await _createPrivateChat.Create(request, user.Id);

        var response = result.Match<Results<Ok<Guid?>, BadRequest<HttpValidationProblemDetails>>>(
            success => TypedResults.Ok(success.Value),
            errors => TypedResults.BadRequest(new HttpValidationProblemDetails(errors.Errors)));

        return response;
    }
}