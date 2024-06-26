﻿using ChatApp.Application.Interfaces;
using ChatApp.Application.Mapping;
using ChatApp.Contracts.Request;
using ChatApp.Contracts.Response;
using ChatApp.Domain.Common;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ChatApp.Api.Endpoints;

public static class MessageEndpoints
{
    public static void MapMessageEndpoints(this WebApplication app)
    {
        var messageEndpoints = app.MapGroup("/api/messages").WithTags("Messages");

        messageEndpoints.MapGet("/paged/ChatId={chatId:Guid}&PageSize={pageSize:int}&PageNumber={pageNumber:int}",
            async (IGetLoggedUserHelper loggedUserHelper, IMessageHandler messageHandler, Guid chatId, uint pageSize, uint pageNumber) =>
            {
                var user = await loggedUserHelper.GetLoggedUser();
                var request = new GetPagedMessagesRequest(chatId, pageSize, pageNumber);
                var result = await messageHandler.GetPaged(request, user);

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
            async (IGetLoggedUserHelper loggedUserHelper, IMessageHandler messageHandler, CreateGroupMessageRequest request) =>
            {
                var user = await loggedUserHelper.GetLoggedUser();
                var result = await messageHandler.CreateGroup(request, user);

                return result.Match<Results<Ok, NotFound, BadRequest<HttpValidationProblemDetails>>>(
                    _ => TypedResults.Ok(),
                    _ => TypedResults.NotFound(),
                    err => TypedResults.BadRequest(new HttpValidationProblemDetails(err.Errors))
                );
            })
            .RequireAuthorization();

        messageEndpoints.MapPost("/private",
            async (IGetLoggedUserHelper loggedUserHelper, IMessageHandler messageHandler, CreatePrivateMessageRequest request) =>
            {
                var user = await loggedUserHelper.GetLoggedUser();
                var result = await messageHandler.CreatePrivate(request, user);

                return result.Match<Results<Ok, NotFound, BadRequest<HttpValidationProblemDetails>>>(
                    _ => TypedResults.Ok(),
                    _ => TypedResults.NotFound(),
                    err => TypedResults.BadRequest(new HttpValidationProblemDetails(err.Errors))
                );
            })
            .RequireAuthorization();

        messageEndpoints.MapPut("/",
            async (IGetLoggedUserHelper loggedUserHelper, IMessageHandler messageHandler, UpdateMessageRequest request) =>
            {
                var user = await loggedUserHelper.GetLoggedUser();
                var result = await messageHandler.Update(request, user);

                return result.Match<Results<Ok, NotFound, ForbidHttpResult, BadRequest<HttpValidationProblemDetails>>>(
                    _ => TypedResults.Ok(),
                    _ => TypedResults.NotFound(),
                    _ => TypedResults.Forbid(),
                    err => TypedResults.BadRequest(new HttpValidationProblemDetails(err.Errors))
                );
            })
            .RequireAuthorization();

        messageEndpoints.MapDelete("/{id:guid}",
            async (IGetLoggedUserHelper loggedUserHelper, IMessageHandler messageHandler, Guid id) =>
            {
                var user = await loggedUserHelper.GetLoggedUser();
                var result = await messageHandler.Delete(id, user);

                return result.Match<Results<Ok, NotFound, ForbidHttpResult>>(
                    _ => TypedResults.Ok(),
                    _ => TypedResults.NotFound(),
                    _ => TypedResults.Forbid()
                );
            })
            .RequireAuthorization();
    }
}