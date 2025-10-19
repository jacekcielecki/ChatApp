using ChatApp.Api.Endpoints;
using ChatApp.Chats;
using ChatApp.Messages;
using ChatApp.Users;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ChatApp.IntegrationTests")]

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterChatsCore();
builder.Services.RegisterMessagesCore();
builder.Services.RegisterUsersCore();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapUserEndpoints();
app.MapChatEndpoints();
app.MapVersionEndpoints();
app.MapMessageEndpoints();

app.Run();
