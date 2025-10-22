using ChatApp.Functions.Authorization;
using ChatApp.Shared.Model.Messages;
using ChatApp.Shared.Model.ValueObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Core = ChatApp.Messages.Queries;

namespace ChatApp.Functions.Queries;

public class GetMessages
{
    private readonly Core.GetMessages _getMessages;

    public GetMessages(Core.GetMessages getMessages)
    {
        _getMessages = getMessages;
    }

    [Function(nameof(GetMessages))]
    public async Task<IResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req,
        [FromBody] GetMessagesParamsDto paramsDto)
    {
        var result = await _getMessages.Get(paramsDto, req.User().Id);

        var response = result.Match<Results<Ok<PagedResult<MessageDto>>, NotFound, ForbidHttpResult, BadRequest<HttpValidationProblemDetails>>>(
            success => TypedResults.Ok(success.Value),
            _ => TypedResults.NotFound(),
            _ => TypedResults.Forbid(),
            err => TypedResults.BadRequest(new HttpValidationProblemDetails(err.Errors))
        );

        return response;
    }
}