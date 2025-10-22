using ChatApp.Shared.Model.Users;
using ChatApp.Users.Core.Details;

namespace ChatApp.Api;

public class LoggedUserProvider : ILoggedUserProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly GetOrCreateUserFromClaims _getOrCreateUserFromClaims;

    public LoggedUserProvider(IHttpContextAccessor httpContextAccessor, GetOrCreateUserFromClaims getOrCreateUserFromClaims)
    {
        _httpContextAccessor = httpContextAccessor;
        _getOrCreateUserFromClaims = getOrCreateUserFromClaims;
    }

    public async Task<UserDto> Get()
    {
        var claimsPrincipal = _httpContextAccessor.HttpContext?.User;
        var claims = claimsPrincipal?.Claims.ToList();

        if (claims is not null)
        {
            var user = await _getOrCreateUserFromClaims.Run(claims);

            return user.ToDto();
        }
        
        throw new Exception("Unable to create user. ClaimsPrincipal is null.");
    }
}
