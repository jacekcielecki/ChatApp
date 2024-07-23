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
            async (IChatHandler chatHandler, IGetLoggedUserHelper loggedUserHelper) =>
            {
                var user = await loggedUserHelper.GetLoggedUser();
                var groupChats = chatHandler.GetGroupChats(user);
                var privateChats = chatHandler.GetPrivateChats(user);

                var fetchData = new Task[] { groupChats, privateChats };
                await Task.WhenAll(fetchData);

                return TypedResults.Ok(
                    new GetChatResponse(
                        privateChats.Result.ToPrivateChatResponse(),
                        groupChats.Result.ToGroupChatResponse())
                    );
            })
            .RequireAuthorization();

        chatEndpoints.MapPost("/group",
            async (IChatHandler chatHandler, IGetLoggedUserHelper loggedUserHelper, CreateGroupChatRequest request) =>
            {
                var user = await loggedUserHelper.GetLoggedUser();
                var result = await chatHandler.CreateGroup(request, user);

                return result.Match<Results<Ok<Guid>, BadRequest<HttpValidationProblemDetails>>>(
                    success => TypedResults.Ok(success.Value),
                    err => TypedResults.BadRequest(new HttpValidationProblemDetails(err.Errors))
                );
            })
            .RequireAuthorization();

        chatEndpoints.MapPost("/private",
            async (IChatHandler chatHandler, IGetLoggedUserHelper loggedUserHelper, CreatePrivateChatRequest request) =>
            {
                var user = await loggedUserHelper.GetLoggedUser();
                var result = await chatHandler.CreatePrivate(request, user);

                return result.Match<Results<Ok<Guid>, BadRequest<HttpValidationProblemDetails>>>(
                    success => TypedResults.Ok(success.Value),
                    err => TypedResults.BadRequest(new HttpValidationProblemDetails(err.Errors))
                );
            })
            .RequireAuthorization();

        chatEndpoints.MapPut("/",
            async (IChatHandler chatHandler, IGetLoggedUserHelper loggedUserHelper, UpdateGroupChatRequest request) =>
            {
                var user = await loggedUserHelper.GetLoggedUser();
                var result = await chatHandler.UpdateGroup(request, user);

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
