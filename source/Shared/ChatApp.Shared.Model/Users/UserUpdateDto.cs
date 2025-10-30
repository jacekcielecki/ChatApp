using Microsoft.AspNetCore.Http;

namespace ChatApp.Shared.Model.Users;

public class UserUpdateDto
{
    public string? Bio { get; set; }
    public IFormFile? ProfilePicture { get; set; }
    public bool DeleteProfilePicture  { get; set; }
}
