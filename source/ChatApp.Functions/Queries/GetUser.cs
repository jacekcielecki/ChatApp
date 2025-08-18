using ChatApp.Users.Core.Details;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace ChatApp.Functions.Queries;

public class GetUser
{
    private readonly ILoggedUserProvider _loggedUserProvider;

    public GetUser(ILoggedUserProvider loggedUserProvider)
    {
        _loggedUserProvider = loggedUserProvider;
    }

    [Function(nameof(GetUser))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        var user = await _loggedUserProvider.Get();

        return new OkObjectResult(user.ToResponse());
    }
}