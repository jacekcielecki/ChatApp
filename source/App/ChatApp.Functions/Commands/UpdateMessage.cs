using ChatApp.Functions.Authorization;
using ChatApp.Shared.Model.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Core = ChatApp.Messages.Commands;

namespace ChatApp.Functions.Commands;

public class UpdateMessage
{
    private readonly Core.UpdateMessage _updateMessage;

    public UpdateMessage(Core.UpdateMessage updateMessage)
    {
        _updateMessage = updateMessage;
    }

    [Function(nameof(UpdateMessage))]
    public async Task<IResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "put")] HttpRequest req,
        [FromBody] UpdateMessageRequest request)
    {
        var result = await _updateMessage.Update(request, req.User().Id);

        var response = result.Match<Results<Ok, NotFound, ForbidHttpResult, BadRequest<HttpValidationProblemDetails>>>(
            _ => TypedResults.Ok(),
            _ => TypedResults.NotFound(),
            _ => TypedResults.Forbid(),
            errors => TypedResults.BadRequest(new HttpValidationProblemDetails(errors.Errors)));

        return response;
    }
}