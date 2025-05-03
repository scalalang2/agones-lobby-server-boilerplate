using k8s;

namespace LobbyServer;

public class GameServerAllocator(IKubernetes kubernetesClient)
{
    private const string Namespace = "default";

    public async Task<AllocateGameServerResponse> AllocateAsync()
    {
        var allocation = new AllocateGameServerRequest();
        var response = await kubernetesClient.CustomObjects.CreateNamespacedCustomObjectAsync(
            allocation,
            group: "allocation.agones.dev",
            version: "v1",
            namespaceParameter: Namespace,
            plural: "gameserverallocations"
        );
        
        dynamic resp = response;
        if (resp.status?.state == "Allocated")
        {
            return new AllocateGameServerResponse
            {
                GameServerName = resp.status.gameServerName,
                Address = resp.status.address,
                Port = resp.status.ports[0].port,
                NodeName = resp.status.nodeName
            };
        }
        
        throw new Exception("Failed to allocate game server");
    }
}