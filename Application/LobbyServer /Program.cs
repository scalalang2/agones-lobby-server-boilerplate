using k8s;
using LobbyServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IKubernetes>(sp =>
{
    var config = KubernetesClientConfiguration.InClusterConfig();
    return new Kubernetes(config);
});
builder.Services.AddSingleton<GameServerAllocator>();
builder.Services.AddControllers();

var app = builder.Build();
app.MapControllers();
app.Run();