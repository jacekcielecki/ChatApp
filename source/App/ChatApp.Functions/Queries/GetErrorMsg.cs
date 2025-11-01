using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;

namespace ChatApp.Functions.Queries;

public class GetErrorMsg
{
    [Function(nameof(GetErrorMsg))]
    public IResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "messages/GetErrorMsg")] HttpRequest req)
    {
        throw new ArgumentOutOfRangeException(nameof(req), "There was an error when processing your request.");
    }
}