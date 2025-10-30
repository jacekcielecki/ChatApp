using ChatApp.Functions.Authorization;
using ChatApp.Shared.Model.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Azure.Functions.Worker;
using Core = ChatApp.Users.Commands;

namespace ChatApp.Functions.Commands;

public class UpdateUser
{
    private readonly Core.UpdateUser _updateUser;

    public UpdateUser(Core.UpdateUser updateUser)
    {
        _updateUser = updateUser;
    }

    [Function(nameof(UpdateUser))]
    public async Task<IResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "put")] HttpRequest req)
    {
        var form = await req.ReadFormAsync();
        var dto = new UserUpdateDto { ProfilePicture = form.Files.GetFile("profilePicture"), Bio = form["bio"], DeleteProfilePicture = (form.ContainsKey("deleteProfilePicture") && form["deleteProfilePicture"] == "1") };

        var result = await _updateUser.Update(req.User().Id, dto);

        var response = result.Match<Results<Ok, BadRequest<HttpValidationProblemDetails>>>(
            _ => TypedResults.Ok(),
            errors => TypedResults.BadRequest(new HttpValidationProblemDetails(errors.Errors)));

        return response;
    }
}