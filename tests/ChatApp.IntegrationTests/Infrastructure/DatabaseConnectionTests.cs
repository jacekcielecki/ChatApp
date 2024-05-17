using ChatApp.IntegrationTests.Tools;

namespace ChatApp.IntegrationTests.Infrastructure;

[Collection("PostgreSql collection")]
public sealed class DatabaseConnectionTests
{
    private readonly PostgreSqlFixture _postgreFixture;

    public DatabaseConnectionTests(PostgreSqlFixture postgreFixture)
    {
        _postgreFixture = postgreFixture;
    }

    [Fact]
    public void TestDatabaseConnection()
    {
        // Use _postgreFixture.ConnectionString to connect to the PostgreSQL container
        using var connection = new Npgsql.NpgsqlConnection(_postgreFixture.ConnectionString);
        connection.Open();

        using var command = new Npgsql.NpgsqlCommand("SELECT 1", connection);
        var result = command.ExecuteScalar();

        result.Should().Be(1);
    }
}
