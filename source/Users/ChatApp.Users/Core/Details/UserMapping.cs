using ChatApp.Shared.Data.Adapters.Entities;
using ChatApp.Shared.Model.Users;

namespace ChatApp.Users.Core.Details;

public static class UserMapping
{
    public static UserResponse ToResponse(this User user)
    {
        return new UserResponse(user.Id, user.Email, user.GivenName, user.FamilyName, user.CreatedAt);
    }
}