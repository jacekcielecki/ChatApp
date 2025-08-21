using ChatApp.Shared.Model.Messages;
using ChatApp.Users.Core.Details;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Core = ChatApp.Messages.Commands;

namespace ChatApp.Functions.Commands;

public class UpdateMessage
{
    private readonly ILoggedUserProvider _loggedUserProvider;
    private readonly Core.UpdateMessage _updateMessage;

    public UpdateMessage(Core.UpdateMessage updateMessage, ILoggedUserProvider loggedUserProvider)
    {
        _updateMessage = updateMessage;
        _loggedUserProvider = loggedUserProvider;
    }

    [Function(nameof(UpdateMessage))]
    public async Task<IResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "put")] HttpRequest req,
        [FromBody] UpdateMessageRequest request)
    {
        var user = await _loggedUserProvider.Get();

        var result = await _updateMessage.Update(request, user.Id);

        var response = result.Match<Results<Ok, NotFound, ForbidHttpResult, BadRequest<HttpValidationProblemDetails>>>(
            _ => TypedResults.Ok(),
            _ => TypedResults.NotFound(),
            _ => TypedResults.Forbid(),
            errors => TypedResults.BadRequest(new HttpValidationProblemDetails(errors.Errors)));

        return response;
    }
}