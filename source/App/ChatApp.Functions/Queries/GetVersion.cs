using ChatApp.Shared.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;

namespace ChatApp.Functions.Queries;

public class GetVersion
{
    [Function(nameof(GetVersion))]
    public IResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "version")] HttpRequest req)
    {
        return Results.Text(Environment.GetEnvironmentVariable(Envars.Version));
    }
}