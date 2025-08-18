using ChatApp.Shared.Model.Messages;
using ChatApp.Users.Core.Details;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Core = ChatApp.Messages.Commands;

namespace ChatApp.Functions.Commands;

public class CreatePrivateChatMessage
{
    private readonly Core.CreatePrivateChatMessage _createPrivateChatMessage;
    private readonly ILoggedUserProvider _loggedUserProvider;

    public CreatePrivateChatMessage(Core.CreatePrivateChatMessage createPrivateChatMessage, ILoggedUserProvider loggedUserProvider)
    {
        _createPrivateChatMessage = createPrivateChatMessage;
        _loggedUserProvider = loggedUserProvider;
    }

    [Function(nameof(CreatePrivateChatMessage))]
    public async Task<Results<Ok, ForbidHttpResult, BadRequest<HttpValidationProblemDetails>>> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req,
        [FromBody] CreatePrivateChatMessageRequest request)
    {
        var user = await _loggedUserProvider.Get();

        var result = await _createPrivateChatMessage.Create(request, user.Id);

        var response = result.Match<Results<Ok, ForbidHttpResult, BadRequest<HttpValidationProblemDetails>>>(
            success => TypedResults.Ok(),
            forbidden => TypedResults.Forbid(),
            errors => TypedResults.BadRequest(new HttpValidationProblemDetails(errors.Errors))
        );

        return response;
    }
}