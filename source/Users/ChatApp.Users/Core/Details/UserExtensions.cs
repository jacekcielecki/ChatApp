using ChatApp.Shared.Data.Adapters.Entities;
using ChatApp.Shared.Model.Users;

namespace ChatApp.Users.Core.Details;

public static class UserExtensions
{
    public static UserDto ToDto(this User entity)
    {
        var dto = new UserDto
        {
            Id = entity.Id,
            Email = entity.Email,
            GivenName = entity.GivenName,
            FamilyName = entity.FamilyName,
            ProfilePictureUrl = entity.ProfilePictureUrl,
            Bio = entity.Bio,
            CreatedAt = entity.CreatedAt
        };

        return dto;
    }
}