using ChatApp.Shared.Model.Users;

namespace ChatApp.Users.Core.Details;

public interface ILoggedUserProvider
{
    Task<UserDto> Get();
}
