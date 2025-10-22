using ChatApp.Shared.Data.Adapters.Entities;
using System.Security.Claims;

namespace ChatApp.Users.Core.Details;

public class GetOrCreateUserFromClaims
{
    private readonly GetUserByEmailRepository _getUserByEmailRepository;
    private readonly CreateUserRepository _createUserRepository;

    public GetOrCreateUserFromClaims(GetUserByEmailRepository getUserByEmailRepository, CreateUserRepository createUserRepository)
    {
        _getUserByEmailRepository = getUserByEmailRepository;
        _createUserRepository = createUserRepository;
    }

    public async Task<User> Run(List<Claim> claims)
    {
        var email = claims.First(x => x.Type == "emails").Value;

        var user = await _getUserByEmailRepository.Get(email);
        if (user is null)
        {
            user = new User
            {
                Id = Guid.NewGuid(),
                Email = email.Trim(),
                GivenName = claims.First(x => x.Type == ClaimTypes.GivenName).Value.Trim(),
                FamilyName = claims.First(x => x.Type == ClaimTypes.Surname).Value.Trim(),
                CreatedAt = DateTime.UtcNow
            };
            await _createUserRepository.Create(user);
            user = await _getUserByEmailRepository.Get(email);
        }

        if (user is null)
        {
            throw new Exception("Failed to create user based on Claims.");
        }

        return user;
    }
}
