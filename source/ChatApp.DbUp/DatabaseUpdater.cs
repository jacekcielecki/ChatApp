using DbUp;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace ChatApp.DbUp;

public class DatabaseUpdater
{
    private readonly IConfiguration _configuration;

    public DatabaseUpdater(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void UpdateDatabase()
    {
        var connectionString = _configuration["Database:ConnectionString"];

        var upgrader = DeployChanges.To
            .PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .LogToConsole()
            .Build();

        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(result.Error);
            Console.ResetColor();
            return;
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Database update was successful!");
        Console.ResetColor();
    }
}
