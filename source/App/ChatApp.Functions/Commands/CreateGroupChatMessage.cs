using ChatApp.Shared.Model.Messages;
using ChatApp.Users.Core.Details;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Core = ChatApp.Messages.Commands;

namespace ChatApp.Functions.Commands;

public class CreateGroupChatMessage
{
    private readonly Core.CreateGroupChatMessage _createGroupChatMessage;
    private readonly ILoggedUserProvider _loggedUserProvider;

    public CreateGroupChatMessage(ILoggedUserProvider loggedUserProvider, Core.CreateGroupChatMessage createGroupChatMessage)
    {
        _loggedUserProvider = loggedUserProvider;
        _createGroupChatMessage = createGroupChatMessage;
    }

    [Function(nameof(CreateGroupChatMessage))]
    public async Task<Results<Ok, ForbidHttpResult, BadRequest<HttpValidationProblemDetails>>> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req,
        [FromBody] CreateGroupChatMessageRequest request)
    {
        var user = await _loggedUserProvider.Get();

        var result = await _createGroupChatMessage.Create(request, user.Id);

        var response = result.Match<Results<Ok, ForbidHttpResult, BadRequest<HttpValidationProblemDetails>>>(
            success => TypedResults.Ok(),
            forbidden => TypedResults.Forbid(),
            errors => TypedResults.BadRequest(new HttpValidationProblemDetails(errors.Errors)));

        return response;

    }
}