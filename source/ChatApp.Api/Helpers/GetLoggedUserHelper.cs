using ChatApp.Application.Interfaces;
using ChatApp.Contracts.Request;
using ChatApp.Domain.Entities;
using System.Security.Claims;

namespace ChatApp.Api.Helpers;

/// <summary>
/// Returns user matching user email present in claims from keycloak token <br/>
/// Should only be used on authorized endpoints
/// </summary>
public class GetLoggedUserHelper : IGetLoggedUserHelper
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserHandler _userHandler;

    public GetLoggedUserHelper(IHttpContextAccessor httpContextAccessor, IUserHandler userHandler)
    {
        _httpContextAccessor = httpContextAccessor;
        _userHandler = userHandler;
    }

    public async Task<User> GetLoggedUser()
    {
        var email = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        if (email != null)
        {
            var user = await _userHandler.GetByEmail(email);
            if (user != null)
            {
                return user;
            }

            var userId = await _userHandler.Create(new CreateUserRequest(email));
            if (userId != null)
            {
                return await _userHandler.GetById((Guid)userId) ?? throw new Exception("Something went wrong, user was not created");
            }
        }

        throw new Exception("User email not present in claims");
    }
}
