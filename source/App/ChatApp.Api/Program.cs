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
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient",
    policy =>
    {
        policy
            .WithOrigins("https://localhost:7206")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
    options.AddPolicy("AllowReactClient",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:5173/")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowBlazorClient");
app.UseCors("AllowReactClient");

app.UseHttpsRedirection();
app.MapUserEndpoints();
app.MapChatEndpoints();
app.MapVersionEndpoints();
app.MapMessageEndpoints();

app.Run();
