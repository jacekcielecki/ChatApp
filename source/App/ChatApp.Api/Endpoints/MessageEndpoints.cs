using ChatApp.Messages.Commands;
using ChatApp.Messages.Queries;
using ChatApp.Shared.Data.Adapters.Entities;
using ChatApp.Shared.Model.Messages;
using ChatApp.Shared.Model.ValueObjects;
using ChatApp.Users.Core.Details;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ChatApp.Api.Endpoints;

public static class MessageEndpoints
{
    public static void MapMessageEndpoints(this WebApplication app)
    {
        var messageEndpoints = app
            .MapGroup("/api/messages")
            .WithTags("Messages");

        messageEndpoints.Map("/GetWelcomeMsg",
            () => Task.FromResult(TypedResults.Ok("This is a secure area only for authenticated users.")));

        messageEndpoints.MapPost("/",
            async (ILoggedUserProvider loggedUserProvider, GetMessages getMessages, GetMessagesRequest request) =>
            {
                var user = await loggedUserProvider.Get();

                var result = await getMessages.Get(request, user.Id);

                var response = result.Match<Results<Ok<PagedResult<MessageResponse>>, NotFound, ForbidHttpResult, BadRequest<HttpValidationProblemDetails>>>(
                    success => TypedResults.Ok(success.Value),
                    _ => TypedResults.NotFound(),
                    _ => TypedResults.Forbid(),
                    err => TypedResults.BadRequest(new HttpValidationProblemDetails(err.Errors))
                );

                return response;
            })
            .RequireAuthorization();

        messageEndpoints.MapPost("/group",
            async (ILoggedUserProvider loggedUserProvider, CreateGroupChatMessage createGroupChat, CreateGroupChatMessageRequest request) =>
            {
                var user = await loggedUserProvider.Get();

                var result = await createGroupChat.Create(request, user.Id);

                var response = result.Match<Results<Ok, ForbidHttpResult, BadRequest<HttpValidationProblemDetails>>>(
                    success => TypedResults.Ok(),
                    forbidden => TypedResults.Forbid(),
                    errors => TypedResults.BadRequest(new HttpValidationProblemDetails(errors.Errors)));

                return response;
            })
            .RequireAuthorization();

        messageEndpoints.MapPost("/private",
            async (ILoggedUserProvider loggedUserProvider, CreatePrivateChatMessage createPrivateChatMessage, CreatePrivateChatMessageRequest request) =>
            {
                var user = await loggedUserProvider.Get();

                var result = await createPrivateChatMessage.Create(request, user.Id);

                var response = result.Match<Results<Ok, ForbidHttpResult, BadRequest<HttpValidationProblemDetails>>>(
                    success => TypedResults.Ok(),
                    forbidden => TypedResults.Forbid(),
                    errors => TypedResults.BadRequest(new HttpValidationProblemDetails(errors.Errors)));

                return response;
            })
            .RequireAuthorization();

        messageEndpoints.MapPut("/",
            async (ILoggedUserProvider loggedUserProvider, UpdateMessage updateMessage, UpdateMessageRequest request) =>
            {
                var user = await loggedUserProvider.Get();

                var result = await updateMessage.Update(request, user.Id);

                var response = result.Match<Results<Ok, NotFound, ForbidHttpResult, BadRequest<HttpValidationProblemDetails>>>(
                    _ => TypedResults.Ok(),
                    _ => TypedResults.NotFound(),
                    _ => TypedResults.Forbid(),
                    errors => TypedResults.BadRequest(new HttpValidationProblemDetails(errors.Errors)));

                return response;
            })
            .RequireAuthorization();

        messageEndpoints.MapDelete("/{id:guid}",
            async (ILoggedUserProvider loggedUserProvider, DeleteMessageById deleteMessageById, Guid id) =>
            {
                var user = await loggedUserProvider.Get();

                var result = await deleteMessageById.Delete(id, user.Id);

                var response = result.Match<Results<Ok, NotFound, ForbidHttpResult>>(
                    success => TypedResults.Ok(),
                    notFound => TypedResults.NotFound(),
                    forbidden => TypedResults.Forbid());

                return response;
            })
            .RequireAuthorization();
    }
}