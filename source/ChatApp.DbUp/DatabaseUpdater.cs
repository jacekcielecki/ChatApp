using DbUp;
using System.Reflection;

namespace ChatApp.DbUp;

public class DatabaseUpdater
{
    private readonly string _dbConnectionString;

    public DatabaseUpdater(string dbConnectionString)
    {
        _dbConnectionString = dbConnectionString;
    }

    public void UpdateDatabase()
    {
        var upgrader = DeployChanges.To
            .PostgresqlDatabase(_dbConnectionString)
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
