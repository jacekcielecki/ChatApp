using ChatApp.Shared.Model.Users;
using ChatApp.Users.Core.Details;

namespace ChatApp.Users.Queries;

public class GetUser
{
    private readonly ILoggedUserProvider _loggedUserProvider;

    public GetUser(ILoggedUserProvider loggedUserProvider)
    {
        _loggedUserProvider = loggedUserProvider;
    }

    public async Task<UserDto> Get()
    {
        var user = await _loggedUserProvider.Get();
        var response = user.ToDto();

        return response;
    }
}
