using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SampleServer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<GameService>();
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5000);
});

var app = builder.Build();
app.MapGet("/", () => "Hello :)");
app.Run();