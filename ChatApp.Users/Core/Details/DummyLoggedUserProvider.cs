using ChatApp.Shared.Data.Adapters.Entities;

namespace ChatApp.Users.Core.Details;

public class DummyLoggedUserProvider : ILoggedUserProvider
{
    private readonly GetUserByEmailRepository _getUserByEmailRepository;

    public DummyLoggedUserProvider(GetUserByEmailRepository getUserByEmailRepository)
    {
        _getUserByEmailRepository = getUserByEmailRepository;
    }

    public async Task<User> Get()
    {
        var email = "david@example.com";

        var user = await _getUserByEmailRepository.Get(email);
        if (user is null)
        {
            throw new KeyNotFoundException(email);
        }

        return user;
    }
}