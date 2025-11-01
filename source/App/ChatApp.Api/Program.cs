using ChatApp.Api;
using ChatApp.Api.Endpoints;
using ChatApp.Chats;
using ChatApp.Chats.Core.Hubs;
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

var storageAccountConnectionString = builder.Configuration[Envars.StorageAccountConnectionString];
Environment.SetEnvironmentVariable(Envars.StorageAccountConnectionString, storageAccountConnectionString);

builder.Services.AddCors();

builder.Services.RegisterChatsCore();
builder.Services.RegisterMessagesCore();
builder.Services.RegisterUsersCore();

builder.Services.AddSignalR();
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
    .AllowCredentials()
    .AllowAnyHeader()
    .AllowAnyMethod());

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<ChatHub>("/chatHub")
    .RequireAuthorization();

app.MapUserEndpoints();
app.MapChatEndpoints();
app.MapVersionEndpoints();
app.MapMessageEndpoints();

app.Run();