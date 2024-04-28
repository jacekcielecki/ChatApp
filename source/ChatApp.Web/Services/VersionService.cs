using ChatApp.Web.Interfaces.Services;

namespace ChatApp.Web.Services;

public class VersionService : IVersionService
{
    private readonly HttpClient _httpClient;

    public VersionService(IHttpClientFactory clientFactory)
    {
        _httpClient = clientFactory.CreateClient("ChatAppApi");
    }

    public async Task<string?> Get()
    {
        return await _httpClient.GetStringAsync("/api/version");
    }
}
