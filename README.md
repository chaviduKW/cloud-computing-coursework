# Tech Salary Application 

This repository contains the Kubernetes manifests for deploying the Tech Salary application, a microservices for managing salary submissions, identity, community data, and more. It is structured using Kustomize for easy customization and deployment.

## Project Overview

The Tech Salary application consists of multiple microservices:

- **APIs**:
  - Gateway API: Entry point for the application
  - Identity API: Handles user authentication and identity management
  - Salary Submission API: Manages salary data submissions
  - Search API: Provides search functionality
  - Stats API: Generates statistics and reports
  - Vote API: Handles voting features

- **Frontend**: Web interface for user interaction

- **Databases**:
  - Community Database (PostgreSQL)
  - Identity Database (PostgreSQL)
  - Submissions Database (PostgreSQL)

- **Infrastructure**:
  - Ingress for external access
  - Persistent Volume Claims for data persistence
  - Secrets for sensitive data (database credentials, etc.)
  - Namespaces for seperation of application and the data

## Prerequisites

Before deploying, ensure you have:

- A Kubernetes cluster (local with Minikube/MicroK8s, or cloud-based like AKS/EKS/GKE)
- `kubectl` CLI installed and configured to access the cluster
- `ingress-nginx` installed 

## Deployment Instructions

1. **Clone and navigate to the repository**:
   ```
   cd cloud-computing-coursework/k8s/
   ```

2. **Customize the deployment (optional)**:
   - Edit `kustomization.yaml` to modify configurations
   - Update secrets in `security/secret.yaml` and `security/secret-database.yaml` with your actual credentials
   - Adjust resource limits in deployment files if needed

3. **Deploy using Kustomize**:
   ```
   kubectl apply -k .
   ```
   This will apply all manifests in the directory using the kustomization.yaml file.

4. **Verify deployment**:
   ```
   kubectl get pods -n techsalary
   kubectl get pods -n techsalary-data
   ```

5. **Check logs if needed**:
   ```
   kubectl logs -n techsalary-namespace <pod-name>
   ```

## Accessing the Application

- **Frontend**: Access via the ingress URL (check `ingress/ingress.yaml` for host configuration)
- **APIs**: Available through the Gateway API service
- **Databases**: Internal access only via services in the `techsalary-data` namespace

## Database Initialization

The databases are initialized using ConfigMaps with SQL scripts:
- `migrations/postgres-init-community-config.yaml`
- `migrations/postgres-init-identity-config.yaml`
- `migrations/postgres-init-submissions-config.yaml`

These run automatically during pod startup.

## Secrets Management

Sensitive data is stored in:
- `security/secret.yaml`: Application secrets
- `security/secret-database.yaml`: Database credentials

**Important**: Update these with secure, base64-encoded values before deployment in production.

## Persistent Storage

Persistent Volume Claims are defined for each database:
- `persistentvolumeclaim/community-postgres-pvc.yaml`
- `persistentvolumeclaim/identity-postgres-pvc.yaml`
- `persistentvolumeclaim/submissions-postgres-pvc.yaml`

Ensure your cluster has a default StorageClass configured.

## Troubleshooting

- **Pods not starting**: Check resource availability and logs
- **Services not accessible**: Verify namespace and service configurations
- **Ingress not working**: Ensure ingress controller is installed in the cluster
- **Database connection issues**: Check secrets and PVC status


## Cleanup

To remove the deployment:
```
kubectl delete -k .
```

This coursework demonstrates cloud-native application deployment using Kubernetes and microservices architecture.
