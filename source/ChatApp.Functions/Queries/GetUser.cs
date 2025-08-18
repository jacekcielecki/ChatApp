using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Core = ChatApp.Users.Queries;

namespace ChatApp.Functions.Queries;

public class GetUser
{
    private readonly Core.GetUser _getUser;

    public GetUser(Core.GetUser getUser)
    {
        _getUser = getUser;
    }

    [Function(nameof(GetUser))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        var user = await _getUser.Get();
        return new OkObjectResult(user);
    }
}