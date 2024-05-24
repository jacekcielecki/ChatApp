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
            async (IGetLoggedUserHelper loggedUserHelper, IMessageService messageService, CreateMessageRequest request) =>
            {
                var user = await loggedUserHelper.GetLoggedUser();
                var result = await messageService.Create(request, user);

                return result.Match<Results<Ok, BadRequest<HttpValidationProblemDetails>>>(
                    _ => TypedResults.Ok(),
                    validationErrors => TypedResults.BadRequest(new HttpValidationProblemDetails(validationErrors.Errors))
                );

            })
            .RequireAuthorization();
    }
}