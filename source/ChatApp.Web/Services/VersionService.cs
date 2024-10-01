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
        return await Task.FromResult("23-09-2024 v3");
        //return await _httpClient.GetStringAsync("/api/version");
    }
}
