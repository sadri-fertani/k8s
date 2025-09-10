# N.B
Pour qu'un service soit accessible que depuis l’intérieur du cluster, alors il faut remplacer le type NodePort.
Si le type n'est pas defenit, alors il sera ClusterIP : accessible seulement à l'inmterieur du cluster.

Ingress : de l'ext vers l'interieur [service : clusterIP] (avec des num port < 30000) moyenant des rules
On dit Ingress controller (il joue le role de reverse-proxy)
Il peut etre : kong, nginx, trfaek

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

### Deployments
```
kubectl get deploy --all-namespaces
```

### Replicas
```
kubectl get replicaset --all-namespaces
```

```
kubectl get replicaset -o wide --all-namespaces
NAMESPACE      NAME                                DESIRED   CURRENT   READY   AGE    CONTAINERS               IMAGES                                    SELECTOR
api-space      monapi-deployment-64d89785b6        0         0         0       20h    monapi                   sadrifertani/monapi:latest                app=monapi,pod-template-hash=64d89785b6
api-space      monapi-deployment-65977c48          0         0         0       20h    monapi                   sadrifertani/monapi:latest                app=monapi,pod-template-hash=65977c48
api-space      monapi-deployment-687c59fdc7        1         1         1       19h    monapi                   sadrifertani/monapi:latest                app=monapi,pod-template-hash=687c59fdc7
api-space      monapi-deployment-68cc67bdbf        0         0         0       20h    monapi                   sadrifertani/monapi:latest                app=monapi,pod-template-hash=68cc67bdbf
api-space      monapi-deployment-77c4559d7c        0         0         0       20h    monapi                   sadrifertani/monapi:latest                app=monapi,pod-template-hash=77c4559d7c
api-space      monapi-deployment-7d4f98c975        0         0         0       20h    monapi                   sadrifertani/monapi:latest                app=monapi,pod-template-hash=7d4f98c975
api-space      monapi-deployment-fd789dfdf         0         0         0       20h    monapi                   sadrifertani/monapi:latest                app=monapi,pod-template-hash=fd789dfdf
api-space      monapi2-deployment-6c5455c8f4       0         0         0       20h    monapi2                  sadrifertani/monapi:latest                app=monapi2,pod-template-hash=6c5455c8f4
api-space      monapi2-deployment-74d4c88fc8       0         0         0       20h    monapi2                  sadrifertani/monapi:latest                app=monapi2,pod-template-hash=74d4c88fc8
api-space      monapi2-deployment-7c96f4b5f7       0         0         0       20h    monapi2                  sadrifertani/monapi:latest                app=monapi2,pod-template-hash=7c96f4b5f7
api-space      monapi2-deployment-844c57d8bc       0         0         0       20h    monapi2                  sadrifertani/monapi:latest                app=monapi2,pod-template-hash=844c57d8bc
api-space      monapi2-deployment-c898fcccc        1         1         1       19h    monapi2                  sadrifertani/monapi:latest                app=monapi2,pod-template-hash=c898fcccc
app-space      monapp-deployment-7d747c6f6         1         1         1       19h    monapp                   sadrifertani/monapp:latest                app=monapp,pod-template-hash=7d747c6f6
default        redis-94c488678                     1         1         1       4d6h   redis                    redis:6.2-alpine                          io.kompose.service=redis,pod-template-hash=94c488678
istio-system   istio-ingressgateway-7b7d7878c6     1         1         1       19h    istio-proxy              docker.io/istio/proxyv2:1.27.0            app=istio-ingressgateway,istio=ingressgateway,pod-template-hash=7b7d7878c6
istio-system   istiod-566955fbd5                   1         1         1       19h    discovery                docker.io/istio/pilot:1.27.0              istio=pilot,pod-template-hash=566955fbd5
kube-system    coredns-5688667fd4                  1         1         1       10d    coredns                  rancher/mirrored-coredns-coredns:1.12.1   k8s-app=kube-dns,pod-template-hash=5688667fd4
kube-system    local-path-provisioner-774c6665dc   1         1         1       10d    local-path-provisioner   rancher/local-path-provisioner:v0.0.31    app=local-path-provisioner,pod-template-hash=774c6665dc
kube-system    metrics-server-6f4c6675d5           1         1         1       10d    metrics-server           rancher/mirrored-metrics-server:v0.7.2    k8s-app=metrics-server,pod-template-hash=6f4c6675d5
kube-system    traefik-c98fdf6fb                   1         1         1       10d    traefik                  rancher/mirrored-library-traefik:3.3.6    app.kubernetes.io/instance=traefik-kube-system,app.kubernetes.io/name=traefik,pod-template-hash=c98fdf6fb   
```

### Scaling
```
kubectl create deployment --image=nginx:1.18.0 nginx-deploy

kubectl get deployments
NAME           READY   UP-TO-DATE   AVAILABLE   AGE
nginx-deploy   1/1     1            1           54s

kubectl scale --replicas=2 deployment/nginx-deploy
deployment.apps/nginx-deploy scaled

kubectl get deployments
NAME           READY   UP-TO-DATE   AVAILABLE   AGE
nginx-deploy   2/2     2            2           108s

kubectl get deployment -o wide
NAME           READY   UP-TO-DATE   AVAILABLE   AGE    CONTAINERS   IMAGES             SELECTOR
nginx-deploy   2/2     2            2           4m1s   nginx        nginx:1.18.0       app=nginx-deploy

kubectl set image deployment/nginx-deploy nginx=nginx
deployment.apps/nginx-deploy image updated

kubectl get deployment -o wide
NAME           READY   UP-TO-DATE   AVAILABLE   AGE     CONTAINERS   IMAGES             SELECTOR
nginx-deploy   2/2     2            2           5m51s   nginx        nginx              app=nginx-deploy

kubectl get replicasets -o wide
NAME                      DESIRED   CURRENT   READY   AGE     CONTAINERS   IMAGES             SELECTOR
nginx-deploy-689f576cfc   2         2         2       2m4s    nginx        nginx              app=nginx-deploy,pod-template-hash=689f576cfc
nginx-deploy-76f77d946c   0         0         0       7m38s   nginx        nginx:1.18.0       app=nginx-deploy,pod-template-hash=76f77d946c
```

### Clean
```
kubectl get deployments
kubectl delete deployment nginx-deploy

kubectl get pods
kubectl delete pod simple-web-app
```