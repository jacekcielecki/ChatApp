namespace ChatApp.Api.Endpoints;

public static class VersionEndpoints
{
    public static void MapVersionEndpoints(this WebApplication app)
    {
        app.MapGet("/api/version",
            () => TypedResults.Text("1.0.0")).WithTags("Version");
    }
}