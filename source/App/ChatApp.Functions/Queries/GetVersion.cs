using ChatApp.Shared.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace ChatApp.Functions.Queries;

public class GetVersion
{
    [Function(nameof(GetVersion))]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        return new OkObjectResult(Environment.GetEnvironmentVariable(Envars.Version));
    }
}