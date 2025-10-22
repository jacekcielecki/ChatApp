using ChatApp.Functions.Authorization;
using ChatApp.Shared.Model.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Core = ChatApp.Messages.Commands;

namespace ChatApp.Functions.Commands;

public class CreateGroupChatMessage
{
    private readonly Core.CreateGroupChatMessage _createGroupChatMessage;

    public CreateGroupChatMessage(Core.CreateGroupChatMessage createGroupChatMessage)
    {
        _createGroupChatMessage = createGroupChatMessage;
    }

    [Function(nameof(CreateGroupChatMessage))]
    public async Task<Results<Ok, ForbidHttpResult, BadRequest<HttpValidationProblemDetails>>> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req,
        [FromBody] GroupChatMessageCreateApiDto dto)
    {
        var result = await _createGroupChatMessage.Create(dto, req.User().Id);

        var response = result.Match<Results<Ok, ForbidHttpResult, BadRequest<HttpValidationProblemDetails>>>(
            _ => TypedResults.Ok(),
            _ => TypedResults.Forbid(),
            errors => TypedResults.BadRequest(new HttpValidationProblemDetails(errors.Errors)));

        return response;

    }
}