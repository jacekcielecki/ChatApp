using ChatApp.Contracts.Request;
using ChatApp.Domain.Entities;

namespace ChatApp.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetById(Guid id);
    Task<User?> GetByEmail(string email);
    Task<string[]> GetEmailsBySearchPhrase(string searchParam);
    Task<Guid?> Create(CreateUserRequest request);
}