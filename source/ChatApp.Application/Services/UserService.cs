using ChatApp.Application.Interfaces;
using ChatApp.Contracts.Request;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Interfaces.Repositories;

namespace ChatApp.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User?> GetByEmail(string email)
    {
        var user = await _userRepository.GetByEmail(email);
        return user;
    }

    public async Task<Guid?> Create(CreateUserRequest request)
    {
        var userId = await _userRepository.Create(request);
        return userId;
    }
}