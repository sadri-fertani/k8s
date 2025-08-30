Parfait, Sadri ! Voici une version que tu peux utiliser directement comme **README.md** pour ton projet GitHub 🚀

---

```markdown
# 🚀 Déploiement local d’une application .NET 8 sur Kubernetes avec Kind

Ce guide explique comment créer, dockeriser et déployer une application .NET 8 sur un cluster Kubernetes local à l’aide de [Kind](https://kind.sigs.k8s.io/).

---

## 🧰 Prérequis

- [.NET SDK 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Docker](https://www.docker.com/)
- [Kind](https://kind.sigs.k8s.io/)
- [kubectl](https://kubernetes.io/docs/tasks/tools/)

---

## 🛠️ Création de l’API .NET

```bash
dotnet new webapi -n MonApiK8s
cd MonApiK8s
dotnet build
```

---

## 🐳 Dockerisation

Crée le fichier `Dockerfile` :

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MonApiK8s.dll"]
```

Puis construis l’image :

```bash
docker build -t monapi:latest .
```

---

## 🧱 Cluster Kind

```bash
kind create cluster
kind load docker-image monapi:latest
```

---

## 📄 Déploiement Kubernetes

### `deployment.yaml`

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: monapi-deployment
spec:
  replicas: 1
  selector:
    matchLabels:
      app: monapi
  template:
    metadata:
      labels:
        app: monapi
    spec:
      containers:
      - name: monapi
        image: monapi:latest
        ports:
        - containerPort: 80
```

### `service.yaml`

```yaml
apiVersion: v1
kind: Service
metadata:
  name: monapi-service
spec:
  selector:
    app: monapi
  ports:
  - protocol: TCP
    port: 80
    targetPort: 80
  type: NodePort
```

---

## 🚀 Déploiement

```bash
kubectl apply -f deployment.yaml
kubectl apply -f service.yaml
```

---

## 🌐 Accès à l’API

```bash
kubectl get service monapi-service
curl http://localhost:<nodePort>
```

---

## 🧪 Vérifications

```bash
kubectl get pods
kubectl logs <nom-du-pod>
```

---

## 💡 Aller plus loin

- 🔧 Ajouter Helm pour la gestion de chart
- 🛡️ Intégrer NGINX Ingress
- 📦 Automatiser avec GitHub Actions
```

---

Tu veux que je te prépare directement une version Helm pour ce projet ? Ou que je transforme tout ça en une page prête à copier-coller dans Notion ? 😄


Absolutely, Sadri! Let's turn your .NET 8 app into a deployable Helm chart 🪖

---

## 📦 Structure du Helm Chart

Crée un dossier `helm/monapi` avec cette structure :

```
helm/
└── monapi/
    ├── Chart.yaml
    ├── values.yaml
    └── templates/
        ├── deployment.yaml
        └── service.yaml
```

---

## 🧾 `Chart.yaml`

```yaml
apiVersion: v2
name: monapi
description: Déploiement de l'API .NET 8 avec Helm
type: application
version: 0.1.0
appVersion: "1.0"
```

---

## ⚙️ `values.yaml`

```yaml
replicaCount: 1

image:
  repository: monapi
  tag: latest
  pullPolicy: IfNotPresent

service:
  type: NodePort
  port: 80

containerPort: 80

resources: {}
```

---

## 🚀 `templates/deployment.yaml`

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: {{ .Chart.Name }}-deployment
spec:
  replicas: {{ .Values.replicaCount }}
  selector:
    matchLabels:
      app: {{ .Chart.Name }}
  template:
    metadata:
      labels:
        app: {{ .Chart.Name }}
    spec:
      containers:
        - name: {{ .Chart.Name }}
          image: "{{ .Values.image.repository }}:{{ .Values.image.tag }}"
          imagePullPolicy: {{ .Values.image.pullPolicy }}
          ports:
            - containerPort: {{ .Values.containerPort }}
```

---

## 🌐 `templates/service.yaml`

```yaml
apiVersion: v1
kind: Service
metadata:
  name: {{ .Chart.Name }}-service
spec:
  selector:
    app: {{ .Chart.Name }}
  ports:
    - protocol: TCP
      port: {{ .Values.service.port }}
      targetPort: {{ .Values.containerPort }}
  type: {{ .Values.service.type }}
```

---

## 🔧 Commandes Helm

### 1. Initialiser le chart local

```bash
helm install monapi ./helm/monapi
```

### 2. Mettre à jour

```bash
helm upgrade monapi ./helm/monapi
```

### 3. Supprimer le déploiement

```bash
helm uninstall monapi
```

---

Si tu veux que je t’aide à ajouter des probes, des secrets, des ingress ou un `values-production.yaml`, je suis chaud pour t’embarquer dans une version “real world” 😎


To add **health checks** to your Helm chart for the .NET 8 app, you'll need to configure Kubernetes **liveness** and **readiness probes**. These help the cluster know when your app is healthy and ready to serve traffic 🚦

Let’s update the `deployment.yaml` inside your Helm chart’s `templates/` folder:

---

## 🩺 Add Probes to `deployment.yaml`

Inside the container spec, update it like this:

```yaml
containers:
  - name: {{ .Chart.Name }}
    image: "{{ .Values.image.repository }}:{{ .Values.image.tag }}"
    imagePullPolicy: {{ .Values.image.pullPolicy }}
    ports:
      - containerPort: {{ .Values.containerPort }}
    readinessProbe:
      httpGet:
        path: /health
        port: {{ .Values.containerPort }}
      initialDelaySeconds: 5
      periodSeconds: 10
    livenessProbe:
      httpGet:
        path: /health
        port: {{ .Values.containerPort }}
      initialDelaySeconds: 15
      periodSeconds: 20
```

---

## 🛠️ Ensure Your App Exposes `/health`

In your .NET app, modify `Program.cs` to expose a health endpoint:

```csharp
app.MapGet("/health", () => Results.Ok("Healthy"));
```

Or use `Microsoft.AspNetCore.Diagnostics.HealthChecks` for a richer setup:

```csharp
builder.Services.AddHealthChecks();
app.MapHealthChecks("/health");
```

---

## 💡 Optional: Make Probes Configurable

You can move probe settings into `values.yaml` to customize per environment:

```yaml
readinessProbe:
  path: /health
  initialDelaySeconds: 5
  periodSeconds: 10

livenessProbe:
  path: /health
  initialDelaySeconds: 15
  periodSeconds: 20
```

Then use Helm templating to wire them in.

---

Want me to update the full `deployment.yaml` template with all that embedded logic and Helm variables? I can also add `values-production.yaml` if you’re deploying to real clusters 📦



You can customize health checks per environment by templating them in your Helm chart’s `values.yaml` file and dynamically inserting those values into your `deployment.yaml`. This gives you fine-grained control so dev, staging, and prod can each behave differently 💡

---

## 🗂️ Step 1: Define probe config in `values.yaml`

```yaml
readinessProbe:
  path: /health
  initialDelaySeconds: 5
  periodSeconds: 10
  failureThreshold: 3

livenessProbe:
  path: /health
  initialDelaySeconds: 15
  periodSeconds: 20
  failureThreshold: 3
```

You can now override these values in environment-specific files like:
- `values-dev.yaml`
- `values-prod.yaml`

---

## 🛠️ Step 2: Update `deployment.yaml` template

Add this under your container spec using Helm templating:

```yaml
readinessProbe:
  httpGet:
    path: {{ .Values.readinessProbe.path }}
    port: {{ .Values.containerPort }}
  initialDelaySeconds: {{ .Values.readinessProbe.initialDelaySeconds }}
  periodSeconds: {{ .Values.readinessProbe.periodSeconds }}
  failureThreshold: {{ .Values.readinessProbe.failureThreshold }}

livenessProbe:
  httpGet:
    path: {{ .Values.livenessProbe.path }}
    port: {{ .Values.containerPort }}
  initialDelaySeconds: {{ .Values.livenessProbe.initialDelaySeconds }}
  periodSeconds: {{ .Values.livenessProbe.periodSeconds }}
  failureThreshold: {{ .Values.livenessProbe.failureThreshold }}
```

---

## 🧪 Step 3: Deploy with targeted config

Example for production:

```bash
helm install monapi ./helm/monapi -f values-prod.yaml
```

---

This lets you tweak health behavior per environment—like stricter checks in production and looser ones for dev debugging 🔧

If you want to add startup probes or switch to TCP or command probes, I can show you how to do that too. Want to explore readiness gates or mesh-friendly health configs next? 🧵

local register
```
docker run -d -p 5000:5000 --restart=always --name registry registry:2
```

```
docker tag monapi localhost:5000/monapi
docker push localhost:5000/monapi
```


```
Parfait Sadri 👌 ! Voici comment adapter ton fichier de déploiement Kubernetes pour utiliser ton image privée stockée dans un registre local (comme celui que tu as lancé sur `localhost:5000`) :

---

### 🛠️ Étapes à suivre

1. **Tague ton image et pousse-la dans ton registre local**
   ```bash
   docker tag monapi localhost:5000/monapi
   docker push localhost:5000/monapi
   ```

2. **Modifie ton YAML de déploiement**
   Remplace `image: monapi:latest` par l’image stockée dans ton registre :
   ```yaml
   image: localhost:5000/monapi
   ```

   Ton fichier corrigé devient :
   ```yaml
   apiVersion: apps/v1
   kind: Deployment
   metadata:
     name: monapi-deployment
   spec:
     replicas: 1
     selector:
       matchLabels:
         app: monapi
     template:
       metadata:
         labels:
           app: monapi
       spec:
         containers:
         - name: monapi
           image: localhost:5000/monapi
           ports:
           - containerPort: 80
   ```

---

### ⚠️ Si tu utilises Minikube ou kind...

Ton registre local `localhost:5000` ne sera pas accessible depuis les nodes du cluster par défaut. Deux solutions :

#### 🔁 Option 1: Charger l’image directement sur le node
```bash
docker save localhost:5000/monapi > monapi.tar
# Puis sur le node :
docker load < monapi.tar
```

#### 🚪 Option 2: Exposer le registre
Expose ton registre en ajoutant un port réseau partagé ou en configurant un proxy (ex. Nginx) pour que les nodes du cluster puissent accéder à `host.docker.internal:5000` ou à une IP publique locale.

---

💡 Tu veux que je t’aide à rendre ton registre accessible à Minikube ou à kind ? Je peux te guider pas à pas selon ton environnement. Fais-moi signe 😄

```

docker save localhost:5000/monapi > monapi.tar
# Puis sur le node :
docker load < monapi.tar

kubectl get pods
kubectl delete pod monapi-deployment-5c6dccb987-wc52q


kubectl config get-clusters
kind delete cluster --name kind-kind


kind delete cluster --name kind


kind load docker-image monapi:latest --name kind-dev

docker restart kind-dev-control-plane

kubectl port-forward service/monapi-service 8008:80

curl http://localhost:8008/weatherforecast
[{"date":"2025-07-28","temperatureC":20,"summary":"Scorching","temperatureF":67},{"date":"2025-07-29","temperatureC":49,"summary":"Warm","temperatureF":120},{"date":"2025-07-30","temperatureC":44,"summary":"Balmy","temperatureF":111},{"date":"2025-07-31","temperatureC":13,"summary":"Hot","temperatureF":55},{"date":"2025-08-01","temperatureC":2,"summary":"Chilly","temperatureF":35}]

helm lint ./helm/monapi
helm install monapi ./helm/monapi
# helm upgrade monapi ./helm/monapi
kubectl port-forward service/monapi-service 8008:80
curl http://localhost:8008/weatherforecast

kubectl patch deployment monapi-deployment -p "{\"metadata\":{\"labels\":{\"app.kubernetes.io/managed-by\":\"Helm\"},\"annotations\":{\"meta.helm.sh/release-name\":\"monapi\",\"meta.helm.sh/release-namespace\":\"default\"}}}"

## load internal docker image (otherwise : status will be pending)
kind load docker-image monapi:latest --name dev


## new deploy
kubectl get pods
kubectl delete pod monapi-deployment-5c6dccb987-wc52q


kind delete cluster --name kind
kind create cluster --name kind
docker build -t monapi:latest .
kind load docker-image monapi:latest --name kind

docker start kind-dev-control-plane

helm install monapi ./helm/monapi

kubectl port-forward service/monapi-service 8008:80
