using ChatApp.Functions.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;

namespace ChatApp.Functions.Queries;

public class GetUser
{
    [Function(nameof(GetUser))]
    public IResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
    {
        var user = req.User();
        return Results.Ok(user);
    }
}