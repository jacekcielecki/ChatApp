using ChatApp.Shared.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;

namespace ChatApp.Functions.Queries;

public class GetVersion
{
    [Function(nameof(GetVersion))]
    public IResult Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        return Results.Ok(Environment.GetEnvironmentVariable(Envars.Version));
    }
}