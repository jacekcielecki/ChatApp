using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace ChatApp.Functions.Commands;

public class GetErrorMsg
{
    [Function(nameof(GetErrorMsg))]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        throw new ArgumentOutOfRangeException(nameof(req), "There was an error when processing your request.");
    }
}