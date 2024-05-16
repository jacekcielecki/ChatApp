using ChatApp.Domain.Entities;

namespace ChatApp.Application.Interfaces;

public interface IGetLoggedUserHelper
{
    Task<User> GetLoggedUser();
}
