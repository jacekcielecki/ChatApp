using ChatApp.Users.Core.Details;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Azure.Functions.Worker;
using Core = ChatApp.Messages.Commands;

namespace ChatApp.Functions.Commands;

public class DeleteMessageById
{
    private readonly Core.DeleteMessageById _deleteMessageById;
    private readonly ILoggedUserProvider _loggedUserProvider;

    public DeleteMessageById(Core.DeleteMessageById deleteMessageById, ILoggedUserProvider loggedUserProvider)
    {
        _deleteMessageById = deleteMessageById;
        _loggedUserProvider = loggedUserProvider;
    }

    [Function(nameof(DeleteMessageById))]
    public async Task<Results<Ok, NotFound, ForbidHttpResult>> Run(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "DeleteMessageById/{messageId}")] HttpRequest req, Guid messageId)
    {
        var user = await _loggedUserProvider.Get();

        var result = await _deleteMessageById.Delete(messageId, user.Id);

        var response = result.Match<Results<Ok, NotFound, ForbidHttpResult>>(
            success => TypedResults.Ok(),
            notFound => TypedResults.NotFound(),
            forbidden => TypedResults.Forbid());

        return response;
    }
}