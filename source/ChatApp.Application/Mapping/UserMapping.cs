using ChatApp.Contracts.Response;
using ChatApp.Domain.Entities;

namespace ChatApp.Application.Mapping;

public static class UserMapping
{
    public static UserResponse ToUserResponse(this User user)
    {
        return new UserResponse(user.Id, user.Email, user.CreatedAt);
    }
}