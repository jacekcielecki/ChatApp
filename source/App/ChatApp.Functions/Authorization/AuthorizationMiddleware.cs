using ChatApp.Shared.Data.Adapters.Entities;
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
    private readonly GetUserByEmailRepository _getUserByEmailRepository;
    private readonly CreateUserRepository _createUserRepository;

    public AuthorizationMiddleware(GetUserByEmailRepository getUserByEmailRepository, CreateUserRepository createUserRepository)
    {
        _getUserByEmailRepository = getUserByEmailRepository;
        _createUserRepository = createUserRepository;
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
        var claimsPrincipal = tokenHandler.ValidateToken(authorizationToken, validationParameters, out SecurityToken jwt);

        if (claimsPrincipal.Identity is { IsAuthenticated: true })
        {
            return new(true, claimsPrincipal.Claims);
        }

        return new (false, []);
    }

    private async Task EnrichFunctionContext(FunctionContext context, List<Claim> claims)
    {
        var email = claims.First(x => x.Type == "emails").Value;

        var user = await _getUserByEmailRepository.Get(email);
        if (user is null)
        {
            user = new User
            {
                Id = Guid.NewGuid(),
                Email = email.Trim(),
                GivenName = claims.First(x => x.Type == ClaimTypes.GivenName).Value.Trim(),
                FamilyName = claims.First(x => x.Type == ClaimTypes.Surname).Value.Trim(),
                CreatedAt = DateTime.UtcNow
            };
            await _createUserRepository.Create(user);
            user = await _getUserByEmailRepository.Get(email);
        }

        if (user is null)
        {
            throw new Exception("User context not found in Function context.");
        }

        context.Items.Add(new KeyValuePair<object, object>("User", user.ToDto()));
        context.GetHttpContext()?.Items.Add(new KeyValuePair<object, object?>("User", user.ToDto()));
    }
}
