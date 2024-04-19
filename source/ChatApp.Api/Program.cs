using ChatApp.Api.Endpoints;
using ChatApp.Application;
using ChatApp.DbUp;
using ChatApp.Infrastructure;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ChatApp.IntegrationTests")]

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddInfrastructure(builder.Configuration).AddApplication();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var dbUp = new DatabaseUpdater(builder.Configuration);
dbUp.UpdateDatabase();

var app = builder.Build();

if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapUserEndpoints();

app.Run();
