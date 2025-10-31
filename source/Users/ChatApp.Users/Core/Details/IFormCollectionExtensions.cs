using ChatApp.Shared.Model.Users;
using Microsoft.AspNetCore.Http;

namespace ChatApp.Users.Core.Details;

public static class FormCollectionExtensions
{
    public static UserUpdateDto GetUserUpdateDto(this IFormCollection form)
    {
        var dto = new UserUpdateDto
        {
            ProfilePicture = form.Files.FirstOrDefault(x => x.Name == "ProfilePicture"),
            Bio = form["Bio"],
            DeleteProfilePicture = (form.ContainsKey("DeleteProfilePicture") && form["DeleteProfilePicture"] == "1")
        };

        return dto;
    }
}
