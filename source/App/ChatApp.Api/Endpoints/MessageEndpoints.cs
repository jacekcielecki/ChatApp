using ChatApp.Messages.Commands;
using ChatApp.Messages.Queries;
using ChatApp.Shared.Model.Messages;
using ChatApp.Shared.Model.ValueObjects;
using ChatApp.Users.Core.Details;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Api.Endpoints;

public static class MessageEndpoints
{
    public static void MapMessageEndpoints(this WebApplication app)
    {
        var api = app
            .MapGroup("/api/messages")
            .WithTags("Messages")
            .RequireAuthorization();

        // GET /api/messages/RunSignalRHealthCheck
        api.MapGet("/RunSignalRHealthCheck", RunSignalRHealthCheck)
            .WithName("RunSignalRHealthCheck")
            .WithSummary("Sends test web socket message to all clients.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

        // GET /api/messages/GetWelcomeMsg
        api.MapGet("/GetWelcomeMsg", () => TypedResults.Text("Web API status: green."))
            .WithName("GetWelcomeMessage")
            .WithSummary("Returns a basic health/status message.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

        // GET /api/messages
        api.MapGet("/", GetMessagesPaged)
            .WithName("GetMessages")
            .WithSummary("Get messages from specified chat with pagination.")
            .Produces<PagedResult<MessageDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesValidationProblem();

        // POST /api/messages/group
        api.MapPost("/group", CreateGroupMessage)
            .WithName("CreateGroupMessage")
            .WithSummary("Create a new message in a group chat.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .ProducesValidationProblem();

        // POST /api/messages/private
        api.MapPost("/private", CreatePrivateMessage)
            .WithName("CreatePrivateMessage")
            .WithSummary("Create a new message in a private chat.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .ProducesValidationProblem();

        // PUT /api/messages
        api.MapPut("/", UpdateMessage)
            .WithName("UpdateMessage")
            .WithSummary("Update an existing message.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesValidationProblem();

        // DELETE /api/messages/{id}
        api.MapDelete("/{id:guid}", DeleteMessage)
            .WithName("DeleteMessage")
            .WithSummary("Delete a message by its ID.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);

        // -------------------- Handlers --------------------

        async Task<IResult> GetMessagesPaged(
            ILoggedUserProvider loggedUserProvider,
            GetMessages getMessages,
            [FromBody] GetMessagesParamsDto paramsDto)
        {
            var user = await loggedUserProvider.Get();

            var result = await getMessages.Get(paramsDto, user.Id);

            return result.Match<IResult>(
                success => TypedResults.Ok(success.Value),
                _ => TypedResults.NotFound(),
                _ => TypedResults.Forbid(),
                err => TypedResults.BadRequest(new HttpValidationProblemDetails(err.Errors))
            );
        }

        async Task<IResult> CreateGroupMessage(
            ILoggedUserProvider loggedUserProvider,
            CreateGroupChatMessage createGroupChat,
            MessageCreateApiDto dto)
        {
            var user = await loggedUserProvider.Get();

            var result = await createGroupChat.Create(dto, user.Id);

            return result.Match<IResult>(
                _ => TypedResults.Ok(),
                _ => TypedResults.Forbid(),
                errors => TypedResults.BadRequest(new HttpValidationProblemDetails(errors.Errors))
            );
        }

        async Task<IResult> CreatePrivateMessage(
            ILoggedUserProvider loggedUserProvider,
            CreatePrivateChatMessage createPrivateChatMessage,
            MessageCreateApiDto dto)
        {
            var user = await loggedUserProvider.Get();

            var result = await createPrivateChatMessage.Create(dto, user.Id);

            return result.Match<IResult>(
                _ => TypedResults.Ok(),
                _ => TypedResults.Forbid(),
                errors => TypedResults.BadRequest(new HttpValidationProblemDetails(errors.Errors))
            );
        }

        async Task<IResult> UpdateMessage(
            ILoggedUserProvider loggedUserProvider,
            UpdateMessage updateMessage,
            MessageUpdateApiDto dto)
        {
            var user = await loggedUserProvider.Get();

            var result = await updateMessage.Update(dto, user.Id);

            return result.Match<IResult>(
                _ => TypedResults.Ok(),
                _ => TypedResults.NotFound(),
                _ => TypedResults.Forbid(),
                errors => TypedResults.BadRequest(new HttpValidationProblemDetails(errors.Errors))
            );
        }

        async Task<IResult> DeleteMessage(
            ILoggedUserProvider loggedUserProvider,
            DeleteMessageById deleteMessageById,
            Guid id)
        {
            var user = await loggedUserProvider.Get();

            var result = await deleteMessageById.Delete(id, user.Id);

            return result.Match<IResult>(
                _ => TypedResults.Ok(),
                _ => TypedResults.NotFound(),
                _ => TypedResults.Forbid()
            );
        }

        async Task<IResult> RunSignalRHealthCheck(
            RunSignalRHealthCheck runSignalRHealthCheck,
            string message)
        {
            await runSignalRHealthCheck.Run(message);

            return TypedResults.Ok();
        }
    }
}
