using ChatApp.Shared.Model.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace ChatApp.Functions.Authorization;

public static class HttpRequestExtensions
{
    public static UserResponse User(this HttpRequestData req)
    {
        var user = req.FunctionContext.GetHttpContext()?.Items.First(x => x.Key.Equals("User")).Value as UserResponse;
        return user ?? throw new NullReferenceException(nameof(User));
    }

    public static UserResponse User(this HttpRequest req)
    {
        var user = req.HttpContext.Items.First(x => x.Key.Equals("User")).Value as UserResponse;
        return user ?? throw new NullReferenceException(nameof(User));
    }
}
