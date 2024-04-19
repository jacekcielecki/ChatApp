using ChatApp.Domain.Entities;

namespace ChatApp.Application.Interfaces;

public interface IUserService
{
    Task<User?> GetByEmail(string email);
}