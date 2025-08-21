using ChatApp.Shared.Data.Adapters.Entities;

namespace ChatApp.Users.Core.Details;

public interface ILoggedUserProvider
{
    Task<User> Get();
}
