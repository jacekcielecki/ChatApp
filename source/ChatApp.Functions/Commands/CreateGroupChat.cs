using ChatApp.Shared.Model.Chats;
using ChatApp.Users.Core.Details;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Core = ChatApp.Chats.Commands;

namespace ChatApp.Functions.Commands;

public class CreateGroupChat
{
    private readonly Core.CreateGroupChat _createGroupChat;
    private readonly ILoggedUserProvider _loggedUserProvider;

    public CreateGroupChat(Core.CreateGroupChat createGroupChat, ILoggedUserProvider loggedUserProvider)
    {
        _createGroupChat = createGroupChat;
        _loggedUserProvider = loggedUserProvider;
    }

    [Function(nameof(CreateGroupChat))]
    public async Task<Results<Ok<Guid?>, BadRequest<HttpValidationProblemDetails>>> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req,
        [FromBody] CreateGroupChatRequest request)
    {
        var user = await _loggedUserProvider.Get();

        var result = await _createGroupChat.Create(request, user.Id);

        var response = result.Match<Results<Ok<Guid?>, BadRequest<HttpValidationProblemDetails>>>(
            success => TypedResults.Ok(success.Value),
            errors => TypedResults.BadRequest(new HttpValidationProblemDetails(errors.Errors))
        );

        return response;
    }
}