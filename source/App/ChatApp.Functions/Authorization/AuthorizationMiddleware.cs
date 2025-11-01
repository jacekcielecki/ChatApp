using ChatApp.Users.Core.Details;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace ChatApp.Functions.Authorization;

public class AuthorizationMiddleware : IFunctionsWorkerMiddleware
{
    private readonly HandleFirstLogin _handleFirstLogin;

    public AuthorizationMiddleware(HandleFirstLogin handleFirstLogin)
    {
        _handleFirstLogin = handleFirstLogin;
    }

    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        var httpRequest = context.GetHttpContext()?.Request;
        var authorizationHeader = httpRequest?.Headers["Authorization"];

        var authorizationToken = authorizationHeader?.ToString().Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);

        if (string.IsNullOrWhiteSpace(authorizationToken))
        {
            await ReturnResponse(context, HttpStatusCode.Unauthorized, new { Error = "Authorization token not found." });
            return;
        }

        var authorizationResult = Authorize(authorizationToken);
        if (authorizationResult.IsAuthorized)
        {
            await EnrichFunctionContext(context, authorizationResult.Claims.ToList());
            await next(context);
            return;
        }

        await ReturnResponse(context, HttpStatusCode.Unauthorized, new { Error = "Authorization failed." });
    }

    private async Task ReturnResponse(FunctionContext context, HttpStatusCode statusCode, object body)
    {
        var httpReqData = await context.GetHttpRequestDataAsync();
        if (httpReqData != null)
        {
            var httpResponse = httpReqData.CreateResponse(statusCode);
            await httpResponse.WriteAsJsonAsync(body);
            context.GetInvocationResult().Value = httpResponse;
        }
    }

    private (bool IsAuthorized, IEnumerable<Claim> Claims) Authorize(string authorizationToken)
    {
        var tenantId = Environment.GetEnvironmentVariable("AzureAdB2C__TenantId");
        var audience = Environment.GetEnvironmentVariable("AzureAdB2C__Audience");
        var tenant = Environment.GetEnvironmentVariable("AzureAdB2C__Tenant");
        var policy = Environment.GetEnvironmentVariable("AzureAdB2C__Policy");

        var issuer = $"https://{tenant}.b2clogin.com/{tenantId}/v2.0/";

        var metaDataEndpoint = $"https://{tenant}.b2clogin.com/{tenant}.onmicrosoft.com/{policy}/v2.0/.well-known/openid-configuration";

        var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(metaDataEndpoint, new OpenIdConnectConfigurationRetriever());

        OpenIdConnectConfiguration config = configManager.GetConfigurationAsync().Result;

        var validationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            RequireSignedTokens = true,
            ClockSkew = TimeSpan.Zero,
            ValidAudience = audience,
            IssuerSigningKeys = config.SigningKeys,
            ValidIssuer = issuer
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var claimsPrincipal = tokenHandler.ValidateToken(authorizationToken, validationParameters, out SecurityToken jwt);
            if (claimsPrincipal.Identity is { IsAuthenticated: true })
            {
                return new(true, claimsPrincipal.Claims);
            }
        }
        catch (SecurityTokenExpiredException)
        {
            return new(false, []);
        }

        return new (false, []);
    }

    private async Task EnrichFunctionContext(FunctionContext context, List<Claim> claims)
    {
        var user = await _handleFirstLogin.Handle(claims);

        context.Items.Add(new KeyValuePair<object, object>("User", user.ToDto()));

        if (context.GetHttpContext()?.Items is not null)
            context.GetHttpContext()?.Items.Add(new KeyValuePair<object, object?>("User", user.ToDto()));
    }
}
