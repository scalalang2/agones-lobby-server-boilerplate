using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SampleServer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<GameService>();

var app = builder.Build();
app.MapGet("/", () => "Hello :)");
app.Run();