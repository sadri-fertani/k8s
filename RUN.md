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