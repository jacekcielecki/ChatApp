using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace ChatApp.Web.Authorization;

public class AuthorizationDelegatingHandler : DelegatingHandler
{
    private readonly IAccessTokenProvider _accessTokenProvider;

    public AuthorizationDelegatingHandler(IAccessTokenProvider accessTokenProvider)
    {
        _accessTokenProvider = accessTokenProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var tokenResult = await _accessTokenProvider.RequestAccessToken();
        if (tokenResult.TryGetToken(out var accessToken))
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken.Value);
        }
        return await base.SendAsync(request, cancellationToken);
    }
}
