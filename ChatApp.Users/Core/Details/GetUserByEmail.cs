using ChatApp.Shared.Data.Adapters.Entities;

namespace ChatApp.Users.Core.Details;

public class GetUserByEmail
{
    private readonly GetUserByEmailRepository _getUserByEmailRepository;

    public GetUserByEmail(GetUserByEmailRepository getUserByEmailRepository)
    {
        _getUserByEmailRepository = getUserByEmailRepository;
    }

    public async Task<User?> GetByEmail(string email)
    {
        var user = await _getUserByEmailRepository.Get(email);
        return user;
    }
}
