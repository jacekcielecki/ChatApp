using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace ChatApp.Functions.Queries;

public class GetWelcomeMsg
{
    [Function(nameof(GetWelcomeMsg))]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteStringAsync("Azure Functions status: green.");

        return response;
    }
}