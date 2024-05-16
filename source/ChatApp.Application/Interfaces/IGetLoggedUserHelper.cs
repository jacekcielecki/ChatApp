using ChatApp.Domain.Entities;
using ChatApp.Domain.ResultTypes;
using OneOf;

namespace ChatApp.Application.Interfaces;

public interface IGetLoggedUserHelper
{
    Task<OneOf<User, ValidationErrors>> GetLoggedUser();
}
