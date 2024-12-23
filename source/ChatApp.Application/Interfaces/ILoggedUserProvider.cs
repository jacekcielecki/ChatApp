using ChatApp.Domain.Entities;

namespace ChatApp.Application.Interfaces;

public interface ILoggedUserProvider
{
    Task<User> Get();
}
