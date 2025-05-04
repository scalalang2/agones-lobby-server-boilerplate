## Agones Lobby Server Boilerplate
This repository provides a sample lobby server developed with .NET, designed to manage dedicated game server hosting through Agones. ☸️

## Project Layout
```
- Application/ : This includes a sample lobby server and a basic rock-paper-scissors game server.
- chart/ : Contains a Helm chart for installing Agones and deploying a Fleet.
```

## prerequisites
- [Minikube](https://minikube.sigs.k8s.io/docs/start/?arch=%2Fmacos%2Farm64%2Fstable%2Fbinary+download)
- [Helm](https://helm.sh/docs/intro/install/)

## 1. Create a kubernetes cluster
```sh
# Create a kuberentes cluster
$ minikube start --kubernetes-version v1.31.0 -p agones --ports 7000-7100:7000-7100/udp

# Open the kubernetes dashboard
$ minikube dashboard -p agones

# Stop the cluster
$ minikube stop -p agones

# Fetch profiles
$ minikube profile list

# Start Tunneling
$ minikube tunnel -p agones
```

## 2. Install Agones
```sh
$ helm repo add agones https://agones.dev/chart/stable
$ helm repo update
$ helm install my-release --namespace agones-system --create-namespace agones/agones
```

```sh
$ kubectl get pods -n agones-system
NAME                                 READY   STATUS    RESTARTS   AGE
agones-allocator-676586f455-cc84w    1/1     Running   0          11h
agones-allocator-676586f455-nk7xm    1/1     Running   0          11h
agones-allocator-676586f455-s8knm    1/1     Running   0          11h
agones-controller-5b948cfcf7-5bttk   1/1     Running   0          11h
agones-controller-5b948cfcf7-ct8ch   1/1     Running   0          11h
agones-extensions-5f84d45b8c-9rbrc   1/1     Running   0          11h
agones-extensions-5f84d45b8c-wn8sn   1/1     Running   0          11h
agones-ping-855f486748-226mr         1/1     Running   0          11h
agones-ping-855f486748-zmnmd         1/1     Running   0          11
```

## 3. Build a Docker image
```sh
$ cd ./Application
$ docker built -t sample-server .
```

## 4. Deploy apps 
```sh
$ helm install apps ./chart/apps
$ kubectl get pods
NAME                                 READY   STATUS    RESTARTS      AGE
apps-lobby-server-69d9cc64ff-h6dqj   1/1     Running   2 (21s ago)   23s
simple-game-server-jccjv-2gw8z       2/2     Running   0             22s
simple-game-server-jccjv-fxb5q       2/2     Running   0             22

$ kubectl get fleee
NAME                 SCHEDULING   DESIRED   CURRENT   ALLOCATED   READY   AGE
simple-game-server   Packed       2         2         0           2       4m9s

$ kubectl get fleeeautoscaler
NAME                            AGE
simple-game-server-autoscaler   4m29s
```

## 5. Tunneling for the lobby server
```sh
$ kubectl port-forward service/apps-service 5000:5000
```

Let's visit to the http://localhost:5000/api/hello

## 6. Allocate a Game Server
Visiting https://localhost:5000/api/game will allocate a new game server.

```json
{
  "kind": "GameServerAllocation",
  "apiVersion": "allocation.agones.dev/v1",
  "metadata": {
    "name": "simple-game-server-vhhq4-gjnmd",
    "namespace": "default",
    "creationTimestamp": "2025-05-04T04:33:42Z"
  },
  "spec": {...},
  "status": {
    "state": "Allocated",
    "gameServerName": "simple-game-server-vhhq4-gjnmd",
    "ports": [
      {
        "name": "default",
        "port": 7050
      }
    ],
    "address": "192.168.49.2",
    "addresses": [
      {
        "type": "InternalIP",
        "address": "192.168.49.2"
      },
      {
        "type": "Hostname",
        "address": "agones"
      },
      {
        "type": "PodIP",
        "address": "10.244.0.79"
      }
    ],
    "nodeName": "agones",
    "source": "local",
    "metadata": {
      "labels": {
        "agones.dev/fleet": "simple-game-server",
        "agones.dev/gameserverset": "simple-game-server-vhhq4"
      },
      "annotations": {
        "agones.dev/last-allocated": "2025-05-04T04:33:42.22290284Z",
        "agones.dev/ready-container-id": "docker://4999e9151590562492076941844dce6128c6b039df5bb05526705f80fbdd0099",
        "agones.dev/sdk-version": "1.48.0"
      }
    }
  }
```

A new game server will be provisioned according to the policy defined in the FleetAutoScaler.
```sh
$ kubectl get gs
NAME                             STATE       ADDRESS        PORT   NODE     AGE
simple-game-server-vhhq4-bp8bq   Ready       192.168.49.2   7025   agones   87s
simple-game-server-vhhq4-gjnmd   Allocated   192.168.49.2   7050   agones   2m57s
simple-game-server-vhhq4-pgw6h   Ready       192.168.49.2   7073   agones   2m57s
```

Let's play game
```sh
$ nc -u localhost 7050
Rock
Server: Scissors, Result: Client Win
```

After finishing the game, the game server is returned to the pool.
```sh
$ kubectl get gs
NAME                             STATE   ADDRESS        PORT   NODE     AGE
simple-game-server-vhhq4-gjnmd   Ready   192.168.49.2   7050   agones   4m22s
simple-game-server-vhhq4-pgw6h   Ready   192.168.49.2   7073   agones   4m22s
```