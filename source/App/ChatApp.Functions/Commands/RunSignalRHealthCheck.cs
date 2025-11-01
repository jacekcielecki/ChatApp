using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;

namespace ChatApp.Functions.Commands;

public class RunSignalRHealthCheck
{
    [Function(nameof(RunSignalRHealthCheck))]
    public async Task<IResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "messages/RunSignalRHealthCheck")] HttpRequest req)
    {
        throw new NotImplementedException();
    }
}