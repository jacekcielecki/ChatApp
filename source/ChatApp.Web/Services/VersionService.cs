namespace ChatApp.Web.Services;

public class VersionService 
{
    private readonly HttpClient _httpClient;

    public VersionService(IHttpClientFactory clientFactory)
    {
        _httpClient = clientFactory.CreateClient("ChatAppApi");
    }

    public async Task<string?> Get()
    {
        return await Task.FromResult("11-05-2024 v4");
        //return await _httpClient.GetStringAsync("/api/version");
    }
}
