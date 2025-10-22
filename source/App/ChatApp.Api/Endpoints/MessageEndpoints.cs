using ChatApp.Messages.Commands;
using ChatApp.Messages.Queries;
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
            () => Task.FromResult(TypedResults.Text("Web Api status: green.")))
            .RequireAuthorization();

        messageEndpoints.MapPost("/",
            async (ILoggedUserProvider loggedUserProvider, GetMessages getMessages, GetMessagesParamsDto paramsDto) =>
            {
                var user = await loggedUserProvider.Get();

                var result = await getMessages.Get(paramsDto, user.Id);

                var response = result.Match<Results<Ok<PagedResult<MessageDto>>, NotFound, ForbidHttpResult, BadRequest<HttpValidationProblemDetails>>>(
                    success => TypedResults.Ok(success.Value),
                    _ => TypedResults.NotFound(),
                    _ => TypedResults.Forbid(),
                    err => TypedResults.BadRequest(new HttpValidationProblemDetails(err.Errors))
                );

                return response;
            })
            .RequireAuthorization();

        messageEndpoints.MapPost("/group",
            async (ILoggedUserProvider loggedUserProvider, CreateGroupChatMessage createGroupChat, GroupChatMessageCreateApiDto dto) =>
            {
                var user = await loggedUserProvider.Get();

                var result = await createGroupChat.Create(dto, user.Id);

                var response = result.Match<Results<Ok, ForbidHttpResult, BadRequest<HttpValidationProblemDetails>>>(
                    _ => TypedResults.Ok(),
                    _ => TypedResults.Forbid(),
                    errors => TypedResults.BadRequest(new HttpValidationProblemDetails(errors.Errors)));

                return response;
            })
            .RequireAuthorization();

        messageEndpoints.MapPost("/private",
            async (ILoggedUserProvider loggedUserProvider, CreatePrivateChatMessage createPrivateChatMessage, PrivateChatCreateApiDto dto) =>
            {
                var user = await loggedUserProvider.Get();

                var result = await createPrivateChatMessage.Create(dto, user.Id);

                var response = result.Match<Results<Ok, ForbidHttpResult, BadRequest<HttpValidationProblemDetails>>>(
                    _ => TypedResults.Ok(),
                    _ => TypedResults.Forbid(),
                    errors => TypedResults.BadRequest(new HttpValidationProblemDetails(errors.Errors)));

                return response;
            })
            .RequireAuthorization();

        messageEndpoints.MapPut("/",
            async (ILoggedUserProvider loggedUserProvider, UpdateMessage updateMessage, MessageUpdateApiDto dto) =>
            {
                var user = await loggedUserProvider.Get();

                var result = await updateMessage.Update(dto, user.Id);

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
                    _ => TypedResults.Ok(),
                    _ => TypedResults.NotFound(),
                    _ => TypedResults.Forbid());

                return response;
            })
            .RequireAuthorization();
    }
}