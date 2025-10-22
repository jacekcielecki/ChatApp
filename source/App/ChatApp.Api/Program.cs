using ChatApp.Api;
using ChatApp.Api.Endpoints;
using ChatApp.Chats;
using ChatApp.Messages;
using ChatApp.Shared.Data;
using ChatApp.Users;
using ChatApp.Users.Core.Details;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ChatApp.IntegrationTests")]

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAdB2C"));

builder.Services.AddAuthorization();

var dbConnectionString = builder.Configuration[Envars.DatabaseConnectionString];
Environment.SetEnvironmentVariable(Envars.DatabaseConnectionString, dbConnectionString);

builder.Services.AddCors();

builder.Services.RegisterChatsCore();
builder.Services.RegisterMessagesCore();
builder.Services.RegisterUsersCore();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<ILoggedUserProvider, LoggedUserProvider>();

var app = builder.Build();

if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(policy => policy
    .WithOrigins("https://localhost:7206", "http://localhost:5173")
    .AllowAnyHeader()
    .AllowAnyMethod());

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapUserEndpoints();
app.MapChatEndpoints();
app.MapVersionEndpoints();
app.MapMessageEndpoints();

app.Run();
