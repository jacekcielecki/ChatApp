using ChatApp.Contracts.Request;
using ChatApp.Domain.Entities;

namespace ChatApp.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmail(string email);
    Task<Guid?> Create(CreateUserRequest request);
}