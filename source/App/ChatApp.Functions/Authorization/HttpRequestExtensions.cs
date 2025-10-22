using ChatApp.Shared.Model.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace ChatApp.Functions.Authorization;

public static class HttpRequestExtensions
{
    public static UserDto User(this HttpRequestData req)
    {
        var user = req.FunctionContext.GetHttpContext()?.Items.First(x => x.Key.Equals("User")).Value as UserDto;
        return user ?? throw new NullReferenceException(nameof(User));
    }

    public static UserDto User(this HttpRequest req)
    {
        var user = req.HttpContext.Items.First(x => x.Key.Equals("User")).Value as UserDto;
        return user ?? throw new NullReferenceException(nameof(User));
    }
}
