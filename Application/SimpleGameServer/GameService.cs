using System.Net;
using System.Net.Sockets;
using System.Text;
using Agones;
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

        while (!stoppingToken.IsCancellationRequested)
        {
            // assume this request is always successful
            var _ = await _agones.ReadyAsync();
            
            var recv = await server.ReceiveAsync(stoppingToken);
            var clientChoice = Encoding.UTF8.GetString(recv.Buffer).Trim();

            string[] choices = { "Scissors", "Paper", "Rock" };
            var rnd = new Random();
            var serverChoice = choices[rnd.Next(choices.Length)];

            var result = GetResult(clientChoice, serverChoice);
            var response = $"Server: {serverChoice}, Result: {result}\n";
            var sendBytes = Encoding.UTF8.GetBytes(response);
            await server.SendAsync(sendBytes, sendBytes.Length, recv.RemoteEndPoint);
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
