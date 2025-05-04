using k8s;
using Microsoft.AspNetCore.Mvc;

namespace LobbyServer;

public class GameServerAllocator(IKubernetes kubernetesClient)
{
    private const string Namespace = "default";

    public async Task<object> AllocateAsync()
    {
        var allocation = new AllocateGameServerRequest();
        var response = await kubernetesClient.CustomObjects.CreateNamespacedCustomObjectAsync(
            allocation,
            group: "allocation.agones.dev",
            version: "v1",
            namespaceParameter: Namespace,
            plural: "gameserverallocations"
        );

        return response;
    }
}