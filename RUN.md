# Important
Si tu veux que ton API ne soit accessible que depuis l’intérieur du cluster, alors il faut remplacer NodePort par ClusterIP.

# API DEPLOY
```
cd C:\Dev\TutoK8s\MonApiK8s\MonApiK8s

docker build -t monapi:latest .
docker tag monapi:latest sadrifertani/monapi:latest
docker push sadrifertani/monapi:latest

cd C:\Dev\TutoK8s

helm lint ./helm/monapi
helm uninstall monapi
#helm install monapi ./helm/monapi --dry-run --debug
helm install monapi ./helm/monapi
kubectl get pods
kubectl logs monapi-deployment-6fc5bb55f-dnr5h
kubectl get svc
# Update config helm
helm upgrade monapi ./helm/monapi
# run in background
kubectl port-forward service/monapi-service 30080:80
```

```
curl http://localhost:30080/get-config
```

# APP WEB Deploy
```
cd C:\Dev\TutoK8s\MonAppK8s\MonAppK8s

docker build -t monapp:latest .
docker tag monapp:latest sadrifertani/monapp:latest
docker push sadrifertani/monapp:latest

cd C:\Dev\TutoK8s

helm lint ./helm/monapp
helm uninstall monapp
helm install monapp ./helm/monapp
helm upgrade monapp ./helm/monapp
```

kubectl get svc
NAME             TYPE        CLUSTER-IP      EXTERNAL-IP   PORT(S)        AGE
kubernetes       ClusterIP   10.43.0.1       <none>        443/TCP        5d18h
monapi-service   ClusterIP   10.43.152.53    <none>        80/TCP         20h
monapp-service   NodePort    10.43.178.208   <none>        80:30090/TCP   3m35s
redis            ClusterIP   10.43.211.179   <none>        6379/TCP       142m

si meme namespace
http://monapi-service:80
different
http://monapi-service.<namespace>.svc.cluster.local:80

# Secret
kubectl create secret generic my-secret --from-literal=REDIS_KEY=eYVX7EwVmmxKPCDmwMtyKVge8oLd2t81 -n app-space

# Namespace
kubectl apply -f ./helm/monapi2/templates/namespace.yaml


kubectl get pods --all-namespaces

kubectl get authorizationpolicy -A 

helm upgrade monapi2 ./helm/monapi2
kubectl rollout restart deployment monapi2-deployment -n api-space
kubectl rollout restart deployment monapp-deployment -n app-space

# Definir label pour les namesapce
kubectl get namespace default --show-labels

kubectl label namespace api-space name=api-space      
kubectl label namespace app-space name=app-space      
kubectl label namespace istio-system name=istio-system
kubectl label namespace kube-system name=kube-system 


kubectl apply -f .\helm\monapp\templates\AuthorizationPolicy.yaml

kubectl get svc monapp-service -n app-space -o yaml

# Debug
kubectl apply -f net-debug.yaml
kubectl exec -it net-debug -n app-space -c tools -- sh


helm install monapi ./helm/monapi --namespace api-space

# Remarque
creation du namespace manuellement pour eviter le conflit du proprietaire avec helm
```
kubectl create namespace api-space
kubectl label namespace api-space istio-injection=enabled

kubectl create namespace api-space --dry-run=client -o yaml | kubectl label -f - istio-injection=enabled
```

kubectl delete pod <nom-du-pod> -n api-space


kubectl label namespace api-space istio-injection-
kubectl label namespace api-space istio.io/rev=default

log istio
```
kubectl logs -n istio-system -l app=istiod --tail=100
```

run simple 
kubectl run istio-test-manual --image=nginx --restart=Never -n api-space