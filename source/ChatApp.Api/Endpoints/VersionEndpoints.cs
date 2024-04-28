namespace ChatApp.Api.Endpoints;

public static class VersionEndpoints
{
    public static void MapVersionEndpoints(this WebApplication app)
    {
        app.MapGet("/api/version", () =>
        {
            var version = "1.0.0";
            return Results.Text(version);
        });
    }
}