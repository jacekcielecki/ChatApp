using ChatApp.Application;
using ChatApp.Application.Interfaces;
using ChatApp.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddInfrastructure(builder.Configuration).AddApplication();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapGet("/User/GetByName", async (IUserService userService) =>
{
    var user = await userService.GetByName("Johny");
    return user;
});

app.Run();
