namespace LobbyServer;

public class Spec
{
    public Selector[] Selectors { get; set; } = [new Selector()];
}

public class Selector
{
    public Dictionary<string, string> MatchLabels { get; set; } = new()
    {
        { "agones.dev/fleet", "simple-game-server" }
    };
}

public class AllocateGameServerRequest
{
    public string ApiVersion { get; set; } = "allocation.agones.dev/v1";
    public string Kind { get; set; } = "GameServerAllocation";
    public Spec Spec { get; set; } = new Spec();
}

public class AllocateGameServerResponse
{
    public string GameServerName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int Port { get; set; }
    public string NodeName { get; set; } = string.Empty;
}
