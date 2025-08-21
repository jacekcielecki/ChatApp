using ChatApp.Api.Endpoints;
using ChatApp.Chats;
using ChatApp.Messages;
using ChatApp.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Runtime.CompilerServices;
using System.Security.Claims;

[assembly: InternalsVisibleTo("ChatApp.IntegrationTests")]

var builder = WebApplication.CreateBuilder(args);
var keyCloakRealm = builder.Configuration.GetValue<string>("KeyCloak:RealmUrl");

builder.Services.RegisterChatsCore();
builder.Services.RegisterMessagesCore();
builder.Services.RegisterUsersCore();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = keyCloakRealm;
        options.MetadataAddress = $"{keyCloakRealm}/.well-known/openid-configuration";
        options.RequireHttpsMetadata = false;
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("Authentication failed: " + context.Exception.Message);
                return Task.CompletedTask;
            },
        };
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role,
            ValidateIssuer = true,
            ValidIssuers = new[] { keyCloakRealm },
            ValidateAudience = true,
            ValidAudiences = new[] { "chatAppApi", "frontend", "mobile", "swagger", },
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapUserEndpoints();
app.MapChatEndpoints();
app.MapVersionEndpoints();
app.MapMessageEndpoints();

app.Run();
