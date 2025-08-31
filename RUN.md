# N.B
Pour qu'un service soit accessible que depuis l’intérieur du cluster, alors il faut remplacer le type NodePort.
Si le type n'est pas defenit, alors il sera ClusterIP : accessible seulement à l'inmterieur du cluster.

# API DEPLOY
## Dockerfile image
Se positionner dans le dossier contenant *Dockerfile*
```
cd C:\Dev\TutoK8s\MonApiK8s\MonApiK8s
```
## Deploiement de l'image
### Build
```
docker build -t monapi:latest .
```
### Tag
```
docker tag monapi:latest sadrifertani/monapi:latest
```
### Deploiement
L'image sera deployer sur ton compte public de *DockerHub*
```
docker push sadrifertani/monapi:latest
```

## Helm
Se positionner dans le dossier contenant le dossier *helm*
```
cd C:\Dev\TutoK8s
```
Verifier
```
helm lint ./helm/monapi
```
Supprimer l'ancienne version
```
helm uninstall monapi
```
Check install
```
#helm install monapi ./helm/monapi --dry-run --debug
```
Install (using namspace)
```
helm install monapi ./helm/monapi -n api-space
```
Update configuration
```
helm upgrade monapi ./helm/monapi -n api-space
```

# Verification
```
kubectl get pods
kubectl logs monapi-deployment-6fc5bb55f-dnr5h
kubectl get svc
```
Forward port
```
kubectl port-forward service/monapi-service 30080:80
```
Test CURL
```
curl http://localhost:30080/get-config
```

## Infos & Commandes utiles
### URL
Si meme namespace
http://<service-name>:80
Sinon
http://<service-name>.<namespace>.svc.cluster.local:80

### Secret
kubectl create secret generic my-secret --from-literal=REDIS_KEY=eYVX7EwVmmxKPCDmwMtyKVge8oLd2t81 -n app-space

### Create new Namespace from file
```
kubectl apply -f ./helm/monapi2/templates/namespace.yaml
```

### Create new namespace (Advanced)
Creation du namespace manuellement pour eviter le conflit du proprietaire avec helm
```
kubectl create namespace api-space
kubectl label namespace api-space istio-injection=enabled

kubectl create namespace api-space --dry-run=client -o yaml | kubectl label -f - istio-injection=enabled
```

### Get all namespace
```
kubectl get pods --all-namespaces
```

### Check Authorization Policy
```
kubectl get authorizationpolicy -A 
```

### Restart deployment
```
kubectl rollout restart deployment monapi2-deployment -n api-space
```

### Definir label pour les namesapce
```
kubectl get namespace default --show-labels

kubectl label namespace api-space name=api-space      
kubectl label namespace app-space name=app-space      
kubectl label namespace istio-system name=istio-system
kubectl label namespace kube-system name=kube-system 
```

### Add Policy
```
kubectl apply -f .\helm\monapp\templates\AuthorizationPolicy.yaml
```

### Get config Service
```
kubectl get svc monapp-service -n app-space -o yaml
```

### Debug
#### Pod #1
Ceci est pod qui contient des tools comme curl
```
kubectl apply -f net-debug.yaml
kubectl exec -it net-debug -n app-space -c tools -- sh
```

#### Pod #2
Un autre pod
```
kubectl run istio-test-manual --image=nginx --restart=Never -n api-space
```

### Delete pod
```
kubectl delete pod <nom-du-pod> -n api-space
```

### Label namespace (Inject Istio)
```
kubectl label namespace api-space istio-injection-
kubectl label namespace api-space istio.io/rev=default
```

### log istio
```
kubectl logs -n istio-system -l app=istiod --tail=100
```