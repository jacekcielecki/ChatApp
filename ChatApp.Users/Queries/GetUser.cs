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

    public async Task<UserResponse> Get()
    {
        var user = await _loggedUserProvider.Get();
        var response = user.ToResponse();

        return response;
    }
}
