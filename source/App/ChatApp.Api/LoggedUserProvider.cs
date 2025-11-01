using ChatApp.Shared.Model.Users;
using ChatApp.Users.Core.Details;

namespace ChatApp.Api;

public class LoggedUserProvider : ILoggedUserProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly HandleFirstLogin _handleFirstLogin;

    public LoggedUserProvider(IHttpContextAccessor httpContextAccessor, HandleFirstLogin handleFirstLogin)
    {
        _httpContextAccessor = httpContextAccessor;
        _handleFirstLogin = handleFirstLogin;
    }

    public async Task<UserDto> Get()
    {
        var claimsPrincipal = _httpContextAccessor.HttpContext?.User;
        var claims = claimsPrincipal?.Claims.ToList();

        if (claims is not null)
        {
            var user = await _handleFirstLogin.Handle(claims);

            return user.ToDto();
        }
        
        throw new Exception("Unable to create user. ClaimsPrincipal is null.");
    }
}
