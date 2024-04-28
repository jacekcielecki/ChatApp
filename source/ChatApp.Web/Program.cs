using ChatApp.Web.Components;
using ChatApp.Web.Interfaces.Services;
using ChatApp.Web.Services;

var builder = WebApplication.CreateBuilder(args);

var apiUrl = builder.Configuration.GetValue<string>("Api:Url");

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient("ChatAppApi", (_, client) =>
{
    client.Timeout = new TimeSpan(0, 0, 60);
    client.BaseAddress = new Uri(apiUrl!);
});
builder.Services.AddTransient<IVersionService, VersionService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
