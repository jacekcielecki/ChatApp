using ChatApp.Application.Interfaces;
using ChatApp.Contracts.Request;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ChatApp.Api.Endpoints;

public static class MessageEndpoints
{
    public static void MapMessageEndpoints(this WebApplication app)
    {
        var messageEndpoints = app.MapGroup("/api/messages").WithTags("Messages");

        messageEndpoints.MapPost("/group",
            async (IGetLoggedUserHelper loggedUserHelper, IMessageService messageService, CreateGroupMessageRequest request) =>
            {
                var user = await loggedUserHelper.GetLoggedUser();
                var result = await messageService.CreateGroup(request, user);

                return result.Match<Results<Ok, BadRequest<HttpValidationProblemDetails>>>(
                    _ => TypedResults.Ok(),
                    validationErrors => TypedResults.BadRequest(new HttpValidationProblemDetails(validationErrors.Errors))
                );

            })
            .RequireAuthorization();

        messageEndpoints.MapPost("/private",
            async (IGetLoggedUserHelper loggedUserHelper, IMessageService messageService, CreatePrivateMessageRequest request) =>
            {
                var user = await loggedUserHelper.GetLoggedUser();
                var result = await messageService.CreatePrivate(request, user);

                return result.Match<Results<Ok, BadRequest<HttpValidationProblemDetails>>>(
                    _ => TypedResults.Ok(),
                    validationErrors => TypedResults.BadRequest(new HttpValidationProblemDetails(validationErrors.Errors))
                );

            })
            .RequireAuthorization();

        messageEndpoints.MapPut("/",
            async (IGetLoggedUserHelper loggedUserHelper, IMessageService messageService, UpdateMessageRequest request) =>
            {
                var user = await loggedUserHelper.GetLoggedUser();
                var result = await messageService.Update(request, user);

                return result.Match<Results<Ok, BadRequest<HttpValidationProblemDetails>>>(
                    _ => TypedResults.Ok(),
                    validationErrors => TypedResults.BadRequest(new HttpValidationProblemDetails(validationErrors.Errors))
                );

            })
            .RequireAuthorization();

        messageEndpoints.MapDelete("/{id:guid}",
            async (IGetLoggedUserHelper loggedUserHelper, IMessageService messageService, Guid id) =>
            {
                var user = await loggedUserHelper.GetLoggedUser();
                var result = await messageService.Delete(id, user);

                return result.Match<Results<Ok, BadRequest<HttpValidationProblemDetails>>>(
                    _ => TypedResults.Ok(),
                    validationErrors => TypedResults.BadRequest(new HttpValidationProblemDetails(validationErrors.Errors))
                );

            })
            .RequireAuthorization();
    }
}