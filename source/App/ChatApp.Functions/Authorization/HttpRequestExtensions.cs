using ChatApp.Shared.Data.Adapters.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker.Http;

namespace ChatApp.Functions.Authorization;

public static class HttpRequestExtensions
{
    public static User User(this HttpRequestData req)
    {
        req.FunctionContext.Items.TryGetValue(nameof(User), out var userValue);
        return userValue as User ?? throw new NullReferenceException(nameof(User));
    }

    public static User User(this HttpRequest req)
    {
        req.HttpContext.Items.TryGetValue(nameof(User), out var userValue);
        return userValue as User ?? throw new NullReferenceException(nameof(User));
    }
}
