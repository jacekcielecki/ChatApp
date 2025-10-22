using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace ChatApp.Functions.Queries;

public class GetWelcomeMsg
{
    [Function(nameof(GetWelcomeMsg))]
    public IResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
    {
        return Results.Text("Azure Functions status: green");
    }
}