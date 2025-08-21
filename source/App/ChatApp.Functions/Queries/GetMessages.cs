using ChatApp.Shared.Model.Messages;
using ChatApp.Shared.Model.ValueObjects;
using ChatApp.Users.Core.Details;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Core = ChatApp.Messages.Queries;

namespace ChatApp.Functions.Queries;

public class GetMessages
{
    private readonly ILoggedUserProvider _loggedUserProvider;
    private readonly Core.GetMessages _getMessages;

    public GetMessages(Core.GetMessages getMessages, ILoggedUserProvider loggedUserProvider)
    {
        _getMessages = getMessages;
        _loggedUserProvider = loggedUserProvider;
    }

    [Function(nameof(GetMessages))]
    public async Task<IResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req,
        [FromBody] GetMessagesRequest request)
    {
        var user = await _loggedUserProvider.Get();

        var result = await _getMessages.Get(request, user.Id);

        var response = result.Match<Results<Ok<PagedResult<MessageResponse>>, NotFound, ForbidHttpResult, BadRequest<HttpValidationProblemDetails>>>(
            success => TypedResults.Ok(success.Value),
            _ => TypedResults.NotFound(),
            _ => TypedResults.Forbid(),
            err => TypedResults.BadRequest(new HttpValidationProblemDetails(err.Errors))
        );

        return response;
    }
}