using ChatApp.Contracts.Request;
using ChatApp.Domain.Entities;

namespace ChatApp.Application.Interfaces;

public interface IUserHandler
{
    Task<User?> GetById(Guid id);
    Task<User?> GetByEmail(string email);
    Task<string[]> GetEmailsBySearchPhrase(string searchPhrase);
    Task<Guid?> Create(CreateUserRequest request);
}