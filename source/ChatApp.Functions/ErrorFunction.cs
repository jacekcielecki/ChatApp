using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ChatApp.Functions
{
    public class ErrorFunction
    {
        private readonly ILogger<ErrorFunction> _logger;

        public ErrorFunction(ILogger<ErrorFunction> logger)
        {
            _logger = logger;
        }

        [Function("ErrorFunction")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function 11processed a request.");
            throw new ArgumentOutOfRangeException("req", "test error!");
        }
    }
}
