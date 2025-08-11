using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace ChatApp.Functions.Commands;

public class GetWelcomeMsg
{
    [Function(nameof(GetWelcomeMsg))]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
       return new OkObjectResult("Welcome to Dashboard!");
    }
}