using ChatApp.Application.Interfaces;
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

        chatEndpoints.MapGet("/me",
            async (IChatService chatService, IGetLoggedUserHelper loggedUserHelper) =>
            {
                var user = await loggedUserHelper.GetLoggedUser();
                var groupChats = await chatService.GetGroupChats(user);
                var privateChats = await chatService.GetPrivateChats(user);

                return TypedResults.Ok(new GetChatResponse(privateChats.ToPrivateChatResponse(), groupChats.ToGroupChatResponse()));
            })
            .RequireAuthorization();

        chatEndpoints.MapPost("/group",
            async (IChatService chatService, IGetLoggedUserHelper loggedUserHelper, CreateGroupChatRequest request) =>
            {
                var user = await loggedUserHelper.GetLoggedUser();
                var result = await chatService.CreateGroup(request, user);

                return result.Match<Results<Ok<Guid>, BadRequest<HttpValidationProblemDetails>>>(
                    success => TypedResults.Ok(success.Value),
                    err => TypedResults.BadRequest(new HttpValidationProblemDetails(err.Errors))
                );
            })
            .RequireAuthorization();

        chatEndpoints.MapPost("/private",
            async (IChatService chatService, IGetLoggedUserHelper loggedUserHelper, CreatePrivateChatRequest request) =>
            {
                var user = await loggedUserHelper.GetLoggedUser();
                var result = await chatService.CreatePrivate(request, user);

                return result.Match<Results<Ok<Guid>, BadRequest<HttpValidationProblemDetails>>>(
                    success => TypedResults.Ok(success.Value),
                    err => TypedResults.BadRequest(new HttpValidationProblemDetails(err.Errors))
                );
            })
            .RequireAuthorization();

        chatEndpoints.MapPut("/",
            async (IChatService chatService, IGetLoggedUserHelper loggedUserHelper, UpdateGroupChatRequest request) =>
            {
                var user = await loggedUserHelper.GetLoggedUser();
                var result = await chatService.UpdateGroup(request, user);

                return result.Match<Results<Ok, NotFound, ForbidHttpResult, BadRequest<HttpValidationProblemDetails>>>(
                    _ => TypedResults.Ok(),
                    _ => TypedResults.NotFound(),
                    _ => TypedResults.Forbid(),
                    err => TypedResults.BadRequest(new HttpValidationProblemDetails(err.Errors))
                );
            })
            .RequireAuthorization();
    }
}
