using ChatApp.Chats;
using ChatApp.Functions.Authorization;
using ChatApp.Messages;
using ChatApp.Users;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.UseMiddleware<AuthorizationMiddleware>();

builder.Services
    .RegisterUsersCore()
    .RegisterChatsCore()
    .RegisterMessagesCore();

builder.Services.AddSingleton<IFunctionsWorkerMiddleware, AuthorizationMiddleware>();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

builder.Build().Run();
