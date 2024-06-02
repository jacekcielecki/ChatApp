using ChatApp.Infrastructure.Database.DbConnectionFactory;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace ChatApp.IntegrationTests.Tools;

internal sealed class TestApiFixture : WebApplicationFactory<Program>
{
    private readonly PostgreSqlFixture _postgreFixture;
    public const string TestUserEmail = "testUser@mail.com";
    private const string TestAuthSchemeName = "testAuthScheme";

    public TestApiFixture(PostgreSqlFixture postgreFixture)
    {
        _postgreFixture = postgreFixture;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureLogging(o => o.ClearProviders());
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IHostedService>();
            services.AddAuthentication(TestAuthSchemeName)
                .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                    TestAuthSchemeName, _ => { }
                );
            services.RemoveAll<IDbConnectionFactory>();
            services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>(_ =>
                new DbConnectionFactory(_postgreFixture.ConnectionString));
        });
    }

    private sealed class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder) : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            Claim[] claims = [new Claim(ClaimTypes.Email, TestUserEmail)];
            var identity = new ClaimsIdentity(claims, TestAuthSchemeName);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, TestAuthSchemeName);

            var result = AuthenticateResult.Success(ticket);

            return Task.FromResult(result);
        }
    }
}
