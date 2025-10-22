using ChatApp.Shared.Model.Users;
using ChatApp.Users.Core.Details;

namespace ChatApp.Shared.Tests.Setup;

public class IntegrationTestLoggedUserProvider : ILoggedUserProvider
{
    private readonly GetUserByEmailRepository _getUserByEmailRepository;

    public IntegrationTestLoggedUserProvider(GetUserByEmailRepository getUserByEmailRepository)
    {
        _getUserByEmailRepository = getUserByEmailRepository;
    }

    public async Task<UserDto> Get()
    {
        var email = "david@example.com";

        var user = await _getUserByEmailRepository.Get(email);
        if (user is null)
        {
            throw new KeyNotFoundException(email);
        }

        return user.ToDto();
    }
}