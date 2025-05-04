## Agones Lobby Server Boilerplate
A sample lobby server built with .NET, capable of managing dedicated game server hosting using Agones. ☸️

## Project Layout
```
- Application consists of a sample lobby server and a simple rock-paper-scissors game server.
- chart : a helm chart for installing Agones & deploying a Fleet.
```

## prerequisites
- [Minikube](https://minikube.sigs.k8s.io/docs/start/?arch=%2Fmacos%2Farm64%2Fstable%2Fbinary+download)
- [Helm](https://helm.sh/docs/intro/install/)

## 1. Create a kubernetes cluster
```sh
# Create a kuberentes cluster
$ minikube start --kubernetes-version v1.31.0 -p agones --ports 7000-7100:7000-7100/udp 5000:5000

# Open the kubernetes dashboard
$ minikube dashboard -p agones

# Stop the cluster
$ minikube stop -p agones

# Fetch profiles
$ minikube profile list
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
$ helm install apps ./apps
$ kubectl get pods
```