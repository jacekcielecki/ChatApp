using ChatApp.Api;
using ChatApp.Api.Endpoints;
using ChatApp.Application;
using ChatApp.DbUp;
using ChatApp.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Runtime.CompilerServices;
using System.Security.Claims;

[assembly: InternalsVisibleTo("ChatApp.IntegrationTests")]

var builder = WebApplication.CreateBuilder(args);
var dbConnectionString = builder.Configuration.GetValue<string>("Database:ConnectionString");
var keyCloakRealm = builder.Configuration.GetValue<string>("KeyCloak:RealmUrl");

// Add services to the container.
builder.Services.AddInfrastructure(builder.Configuration).AddApplication().AddApi();
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

var dbUp = new DatabaseUpdater(dbConnectionString!);
dbUp.UpdateDatabase();

var app = builder.Build();

if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapUserEndpoints();
app.MapChatEndpoints();
app.MapVersionEndpoints();

app.Run();
