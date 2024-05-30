using ChatApp.Application.Interfaces;
using ChatApp.Application.Mapping;
using ChatApp.Contracts.Request;
using ChatApp.Contracts.Response;
using ChatApp.Domain.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Api.Endpoints;

public static class MessageEndpoints
{
    public static void MapMessageEndpoints(this WebApplication app)
    {
        var messageEndpoints = app.MapGroup("/api/messages").WithTags("Messages");

        messageEndpoints.MapGet("/paged",
            async (IGetLoggedUserHelper loggedUserHelper, IMessageService messageService, [FromBody] GetPagedMessagesRequest request) =>
            {
                var user = await loggedUserHelper.GetLoggedUser();
                var result = await messageService.GetPaged(request, user);

                return result.Match<Results<Ok<PagedResult<MessageResponse>>, NotFound, ForbidHttpResult, BadRequest<HttpValidationProblemDetails>>>(
                    res => TypedResults.Ok(new PagedResult<MessageResponse>(
                        res.Value.Item1.ToMessageResponse(), (uint)res.Value.Item2, request.PageSize, request.PageNumber)),
                    _ => TypedResults.NotFound(),
                    _ => TypedResults.Forbid(),
                    err => TypedResults.BadRequest(new HttpValidationProblemDetails(err.Errors))
                );
            })
            .RequireAuthorization();

        messageEndpoints.MapPost("/group",
            async (IGetLoggedUserHelper loggedUserHelper, IMessageService messageService, CreateGroupMessageRequest request) =>
            {
                var user = await loggedUserHelper.GetLoggedUser();
                var result = await messageService.CreateGroup(request, user);

                return result.Match<Results<Ok, NotFound, BadRequest<HttpValidationProblemDetails>>>(
                    _ => TypedResults.Ok(),
                    _ => TypedResults.NotFound(),
                    err => TypedResults.BadRequest(new HttpValidationProblemDetails(err.Errors))
                );
            })
            .RequireAuthorization();

        messageEndpoints.MapPost("/private",
            async (IGetLoggedUserHelper loggedUserHelper, IMessageService messageService, CreatePrivateMessageRequest request) =>
            {
                var user = await loggedUserHelper.GetLoggedUser();
                var result = await messageService.CreatePrivate(request, user);

                return result.Match<Results<Ok, NotFound, BadRequest<HttpValidationProblemDetails>>>(
                    _ => TypedResults.Ok(),
                    _ => TypedResults.NotFound(),
                    err => TypedResults.BadRequest(new HttpValidationProblemDetails(err.Errors))
                );
            })
            .RequireAuthorization();

        messageEndpoints.MapPut("/",
            async (IGetLoggedUserHelper loggedUserHelper, IMessageService messageService, UpdateMessageRequest request) =>
            {
                var user = await loggedUserHelper.GetLoggedUser();
                var result = await messageService.Update(request, user);

                return result.Match<Results<Ok, NotFound, ForbidHttpResult, BadRequest<HttpValidationProblemDetails>>>(
                    _ => TypedResults.Ok(),
                    _ => TypedResults.NotFound(),
                    _ => TypedResults.Forbid(),
                    err => TypedResults.BadRequest(new HttpValidationProblemDetails(err.Errors))
                );
            })
            .RequireAuthorization();

        messageEndpoints.MapDelete("/{id:guid}",
            async (IGetLoggedUserHelper loggedUserHelper, IMessageService messageService, Guid id) =>
            {
                var user = await loggedUserHelper.GetLoggedUser();
                var result = await messageService.Delete(id, user);

                return result.Match<Results<Ok, NotFound, ForbidHttpResult>>(
                    _ => TypedResults.Ok(),
                    _ => TypedResults.NotFound(),
                    _ => TypedResults.Forbid()
                );
            })
            .RequireAuthorization();
    }
}