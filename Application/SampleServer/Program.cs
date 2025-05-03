using System.Net;
using System.Net.Sockets;
using System.Text;

const int port = 9000;
var server = new UdpClient(port);

Console.WriteLine($"Server started on port {port}. Waiting for client...");

while (true)
{
    var remoteEp = new IPEndPoint(IPAddress.Any, 0);
    var recv = server.Receive(ref remoteEp);
    var clientChoice = Encoding.UTF8.GetString(recv).Trim();
    
    string[] choices = { "Scissors", "Paper", "Rock" };
    var rnd = new Random();
    var serverChoice = choices[rnd.Next(choices.Length)];

    var result = GetResult(clientChoice, serverChoice);
    var response = $"Server: {serverChoice}, Result: {result}\n";
    var sendBytes = Encoding.UTF8.GetBytes(response);
    server.Send(sendBytes, sendBytes.Length, remoteEp);
}
string GetResult(string client, string server)
{
    if (client == server)
        return "Draw";
    
    switch (client)
    {
        case "Scissors" when server == "Rock":
        case "Paper" when server == "Scissors":
        case "Rock" when server == "Paper":
            return "Client Win";
        case "Scissors" when server == "Paper":
        case "Paper" when server == "Rock":
        case "Rock" when server == "Scissors":
            return "Server Win";
        default:
            return "Invalid Choice from Client, Available Choices are [Rock, Paper, Scissors]";
    }
}