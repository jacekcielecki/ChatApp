﻿using ChatApp.Application.Interfaces;
using ChatApp.Application.Mapping;
using ChatApp.Contracts.Request;
using ChatApp.Contracts.Response;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ChatApp.Api.Endpoints;

public static class ChatEndpoints
{
    public static void MapChatEndpoints(this WebApplication app)
    {
        var chatEndpoints = app.MapGroup("/api/chats").WithTags("Chats");

        chatEndpoints.MapPost("/group",
            async (IChatService chatService, IGetLoggedUserHelper loggedUserHelper, CreateGroupChatRequest request) =>
            {
                var user = await loggedUserHelper.GetLoggedUser();
                var result = await chatService.CreateGroup(request, user.Id);

                return result.Match<Results<Ok<Guid>, BadRequest<HttpValidationProblemDetails>>>(
                    success => TypedResults.Ok(success.Value),
                    validationErrors => TypedResults.BadRequest(new HttpValidationProblemDetails(validationErrors.Errors))
                );
            })
            .RequireAuthorization();

        chatEndpoints.MapGet("/me",
            async (IChatService chatService, IGetLoggedUserHelper loggedUserHelper) =>
            {
                var user = await loggedUserHelper.GetLoggedUser();
                var groupChats = await chatService.GetGroupChats(user.Id);
                var privateChats = await chatService.GetPrivateChats(user.Id);

                return TypedResults.Ok(new GetChatResponse(privateChats.ToPrivateChatResponse(), groupChats.ToGroupChatResponse()));
            })
            .RequireAuthorization();
    }
}
