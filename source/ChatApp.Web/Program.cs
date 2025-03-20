using ChatApp.Web;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Sysinfocus.AspNetCore.Components;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSysinfocus(false);

var accessTokenScopeUri = builder.Configuration["AccessTokenScopeUri"] ?? "";

builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("AzureAdB2C", options.ProviderOptions.Authentication);
    options.ProviderOptions.LoginMode = "redirect";
    options.ProviderOptions.DefaultAccessTokenScopes.Add(accessTokenScopeUri);
});

await builder.Build().RunAsync();
