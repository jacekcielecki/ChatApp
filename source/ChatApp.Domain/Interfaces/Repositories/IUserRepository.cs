using ChatApp.Domain.Entities;

namespace ChatApp.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmail(string email);
}