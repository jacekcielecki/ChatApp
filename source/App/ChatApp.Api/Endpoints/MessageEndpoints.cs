using ChatApp.Messages.Commands;
using ChatApp.Messages.Queries;
using ChatApp.Shared.Model.Messages;
using ChatApp.Shared.Model.ValueObjects;
using ChatApp.Users.Core.Details;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

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

        messageEndpoints.MapGet("/",
            async ([FromServices] ILoggedUserProvider loggedUserProvider, [FromServices] GetMessages getMessages, [FromBody] GetMessagesParamsDto paramsDto) =>
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
            async ([FromServices] ILoggedUserProvider loggedUserProvider, [FromServices] CreateGroupChatMessage createGroupChat, [FromBody] MessageCreateApiDto dto) =>
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
            async ([FromServices] ILoggedUserProvider loggedUserProvider, [FromServices] CreatePrivateChatMessage createPrivateChatMessage, [FromBody] MessageCreateApiDto dto) =>
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
            async ([FromServices] ILoggedUserProvider loggedUserProvider, [FromServices] UpdateMessage updateMessage, [FromBody] MessageUpdateApiDto dto) =>
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
            async ([FromServices] ILoggedUserProvider loggedUserProvider, [FromServices] DeleteMessageById deleteMessageById, Guid id) =>
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