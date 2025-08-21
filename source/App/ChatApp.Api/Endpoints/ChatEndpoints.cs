using ChatApp.Chats.Commands;
using ChatApp.Chats.Queries;
using ChatApp.Shared.Model.Chats;
using ChatApp.Users.Core.Details;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ChatApp.Api.Endpoints;

public static class ChatEndpoints
{
    public static void MapChatEndpoints(this WebApplication app)
    {
        var chatEndpoints = app
            .MapGroup("/api/chats")
            .WithTags("Chats");

        chatEndpoints.MapGet("/me",
            async (GetChats getChats, ILoggedUserProvider loggedUserProvider) =>
            {
                var user = await loggedUserProvider.Get();
                var chats = await getChats.Get(user.Id);

                return Results.Ok(chats);
            })
            .RequireAuthorization();

        chatEndpoints.MapPost("/group",
            async (CreateGroupChat createGroupChat, ILoggedUserProvider loggedUserProvider, CreateGroupChatRequest request) =>
            {
                var user = await loggedUserProvider.Get();

                var result = await createGroupChat.Create(request, user.Id);

                var response = result.Match<Results<Ok<Guid?>, BadRequest<HttpValidationProblemDetails>>>(
                    success => TypedResults.Ok(success.Value),
                    errors => TypedResults.BadRequest(new HttpValidationProblemDetails(errors.Errors))
                );

                return response;
            })
            .RequireAuthorization();

        chatEndpoints.MapPost("/private",
            async (CreatePrivateChat createPrivateChat, ILoggedUserProvider loggedUserProvider, CreatePrivateChatRequest request) =>
            {
                var user = await loggedUserProvider.Get();

                var result = await createPrivateChat.Create(request, user.Id);

                var response = result.Match<Results<Ok<Guid?>, BadRequest<HttpValidationProblemDetails>>>(
                    success => TypedResults.Ok(success.Value),
                    errors => TypedResults.BadRequest(new HttpValidationProblemDetails(errors.Errors)));

                return response;
            })
            .RequireAuthorization();

        chatEndpoints.MapPut("/",
            async (UpdateGroupChat updateGroupChat, ILoggedUserProvider loggedUserProvider, UpdateGroupChatRequest request) =>
            {
                var user = await loggedUserProvider.Get();

                var result = await updateGroupChat.Update(request, user.Id);

                var response = result.Match<Results<Ok, NotFound, ForbidHttpResult, BadRequest<HttpValidationProblemDetails>>>(
                    _ => TypedResults.Ok(),
                    _ => TypedResults.NotFound(),
                    _ => TypedResults.Forbid(),
                    err => TypedResults.BadRequest(new HttpValidationProblemDetails(err.Errors))
                );

                return response;
            })
            .RequireAuthorization();
    }
}
