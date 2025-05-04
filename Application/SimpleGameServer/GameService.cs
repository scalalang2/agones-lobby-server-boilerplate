using System.Net;
using System.Net.Sockets;
using System.Text;
using Agones;
using Grpc.Core;
using Microsoft.Extensions.Hosting;

namespace SampleServer;

public class GameService : BackgroundService
{
    private IAgonesSDK _agones = new AgonesSDK();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        const int port = 9000;
        using var server = new UdpClient(port);

        Console.WriteLine($"Server started on port {port}. Waiting for client...");

        this.RunHealthCheckAsync(stoppingToken);
        await this.DoMustReady(stoppingToken);
        while (!stoppingToken.IsCancellationRequested)
        {
            var recv = await server.ReceiveAsync(stoppingToken);
            var clientChoice = Encoding.UTF8.GetString(recv.Buffer).Trim();

            string[] choices = { "Scissors", "Paper", "Rock" };
            var rnd = new Random();
            var serverChoice = choices[rnd.Next(choices.Length)];

            var result = GetResult(clientChoice, serverChoice);
            var response = $"Server: {serverChoice}, Result: {result}\n";
            var sendBytes = Encoding.UTF8.GetBytes(response);
            await server.SendAsync(sendBytes, sendBytes.Length, recv.RemoteEndPoint);
            await this.DoMustReady(stoppingToken);
        }
    }
    
    private async Task DoMustReady(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            var status = await _agones.ReadyAsync();
            if (status.StatusCode == StatusCode.OK)
            {
                Console.WriteLine("GameServer marked as Ready.");
                break;
            }
            else
            {
                Console.WriteLine($"ReadyAsync failed: {status.Detail}, retrying...");
            }
            await Task.Delay(1000, token);
        }
    }


    private async Task RunHealthCheckAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var health = await _agones.HealthAsync();
            if (health.StatusCode == StatusCode.OK)
            {
                Console.WriteLine("GameServer is Healthy.");
            }
            else
            {
                Console.WriteLine($"HealthAsync failed: {health.Detail}");
            }
            
            await Task.Delay(2000, stoppingToken);
        }
    }

    private string GetResult(string client, string server)
    {
        if (client == server)
            return "Draw";

        return (client, server) switch
        {
            ("Scissors", "Rock") => "Server Win",
            ("Paper", "Scissors") => "Server Win",
            ("Rock", "Paper") => "Server Win",
            ("Scissors", "Paper") => "Client Win",
            ("Paper", "Rock") => "Client Win",
            ("Rock", "Scissors") => "Client Win",
            _ => "Invalid Choice from Client, Available Choices are [Rock, Paper, Scissors]"
        };
    }
}
