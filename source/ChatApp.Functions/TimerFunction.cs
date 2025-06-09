using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ChatApp.Functions;

public class TimerFunction
{
    private readonly ILogger _logger;

    public TimerFunction(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<TimerFunction>();
    }

    [Function(nameof(TimerFunction))]
    public void Run([TimerTrigger("%Cron_TimerFunction%")] TimerInfo myTimer)
    {
        _logger.LogInformation("C# Timer trigger function executed at: {executionTime}", DateTime.Now);
        
        if (myTimer.ScheduleStatus is not null)
        {
            _logger.LogInformation("Next timer schedule at: {nextSchedule}", myTimer.ScheduleStatus.Next);
        }
    }
}