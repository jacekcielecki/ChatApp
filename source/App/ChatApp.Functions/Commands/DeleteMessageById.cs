using ChatApp.Functions.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Azure.Functions.Worker;
using Core = ChatApp.Messages.Commands;

namespace ChatApp.Functions.Commands;

public class DeleteMessageById
{
    private readonly Core.DeleteMessageById _deleteMessageById;

    public DeleteMessageById(Core.DeleteMessageById deleteMessageById)
    {
        _deleteMessageById = deleteMessageById;
    }

    [Function(nameof(DeleteMessageById))]
    public async Task<Results<Ok, NotFound, ForbidHttpResult>> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "messages/{id}")] HttpRequest req, Guid id)
    {
        var result = await _deleteMessageById.Delete(id, req.User().Id);

        var response = result.Match<Results<Ok, NotFound, ForbidHttpResult>>(
            _ => TypedResults.Ok(),
            _ => TypedResults.NotFound(),
            _ => TypedResults.Forbid());

        return response;
    }
}