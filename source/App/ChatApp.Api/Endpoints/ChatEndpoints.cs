using ChatApp.Chats.Commands;
using ChatApp.Chats.Queries;
using ChatApp.Shared.Model.Chats;
using ChatApp.Users.Core.Details;

namespace ChatApp.Api.Endpoints;

public static class ChatEndpoints
{
    public static void MapChatEndpoints(this WebApplication app)
    {
        var api = app
            .MapGroup("/api/chats")
            .WithTags("Chats")
            .RequireAuthorization();

        // GET /api/chats/me
        api.MapGet("/me", GetChats)
            .WithName("GetMyChats")
            .WithSummary("Get authenticated user chats.")
            .Produces<List<ChatDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

        // POST /api/chats/group
        api.MapPost("/group", CreateGroupChat)
            .WithName("CreateGroupChat")
            .WithSummary("Create a new group chat.")
            .Produces<Guid?>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem();

        // POST /api/chats/private
        api.MapPost("/private", CreatePrivateChat)
            .WithName("CreatePrivateChat")
            .WithSummary("Create a new private chat.")
            .Produces<Guid?>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem();

        // PUT /api/chats/group
        api.MapPut("/group", UpdateGroupChat)
            .WithName("UpdateGroupChat")
            .WithSummary("Update existing group chat details.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesValidationProblem();

        // -------------------- Handlers --------------------

        async Task<IResult> GetChats(GetChats getChats, ILoggedUserProvider loggedUserProvider)
        {
            var user = await loggedUserProvider.Get();
            var chats = await getChats.Get(user.Id);

            return TypedResults.Ok(chats);
        }

        async Task<IResult> CreateGroupChat(
            CreateGroupChat createGroupChat,
            ILoggedUserProvider loggedUserProvider,
            GroupChatCreateApiDto dto)
        {
            var user = await loggedUserProvider.Get();

            var result = await createGroupChat.Create(dto, user.Id);

            return result.Match<IResult>(
                success => TypedResults.Ok(success.Value),
                errors => TypedResults.BadRequest(new HttpValidationProblemDetails(errors.Errors))
            );
        }

        async Task<IResult> CreatePrivateChat(
            CreatePrivateChat createPrivateChat,
            ILoggedUserProvider loggedUserProvider,
            PrivateChatCreateApiDto dto)
        {
            var user = await loggedUserProvider.Get();

            var result = await createPrivateChat.Create(dto, user.Id);

            return result.Match<IResult>(
                success => TypedResults.Ok(success.Value),
                errors => TypedResults.BadRequest(new HttpValidationProblemDetails(errors.Errors))
            );
        }

        async Task<IResult> UpdateGroupChat(
           UpdateGroupChat updateGroupChat,
            ILoggedUserProvider loggedUserProvider,
            GroupChatUpdateApiDto dto)
        {
            var user = await loggedUserProvider.Get();

            var result = await updateGroupChat.Update(dto, user.Id);

            return result.Match<IResult>(
                _ => TypedResults.Ok(),
                _ => TypedResults.NotFound(),
                _ => TypedResults.Forbid(),
                err => TypedResults.BadRequest(new HttpValidationProblemDetails(err.Errors))
            );
        }
    }
}
