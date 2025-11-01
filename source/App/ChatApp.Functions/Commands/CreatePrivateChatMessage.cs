using ChatApp.Functions.Authorization;
using ChatApp.Shared.Model.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Core = ChatApp.Messages.Commands;

namespace ChatApp.Functions.Commands;

public class CreatePrivateChatMessage
{
    private readonly Core.CreatePrivateChatMessage _createPrivateChatMessage;

    public CreatePrivateChatMessage(Core.CreatePrivateChatMessage createPrivateChatMessage)
    {
        _createPrivateChatMessage = createPrivateChatMessage;
    }

    [Function(nameof(CreatePrivateChatMessage))]
    public async Task<Results<Ok, ForbidHttpResult, BadRequest<HttpValidationProblemDetails>>> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "messages/private")] HttpRequest req,
        [FromBody] MessageCreateApiDto dto)
    {
        var result = await _createPrivateChatMessage.Create(dto, req.User().Id);

        var response = result.Match<Results<Ok, ForbidHttpResult, BadRequest<HttpValidationProblemDetails>>>(
            _ => TypedResults.Ok(),
            _ => TypedResults.Forbid(),
            errors => TypedResults.BadRequest(new HttpValidationProblemDetails(errors.Errors)));

        return response;
    }
}