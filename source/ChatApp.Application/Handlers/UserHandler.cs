using ChatApp.Application.Interfaces;
using ChatApp.Contracts.Request;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Interfaces.Repositories;

namespace ChatApp.Application.Handlers;

public class UserHandler : IUserHandler
{
    private readonly IUserRepository _userRepository;

    public UserHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User?> GetById(Guid id)
    {
        var user = await _userRepository.GetById(id);
        return user;
    }

    public async Task<User?> GetByEmail(string email)
    {
        var user = await _userRepository.GetByEmail(email);
        return user;
    }

    public async Task<string[]> GetEmailsBySearchPhrase(string searchPhrase)
    {
        var emails = await _userRepository.GetEmailsBySearchPhrase(searchPhrase);
        return emails;
    }

    public async Task<Guid?> Create(CreateUserRequest request)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Email = request.Email,
        };

        await _userRepository.Insert(user);
        return user.Id;
    }
}