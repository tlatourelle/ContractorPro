---
status: backlog
---

# Story Aspire-Infra.4: Deployment manifest generation and documentation

Status: backlog

Epic: Aspire Infrastructure · FR: (infrastructure, deployment) · Journey: (none — enabler) · Depends: Aspire-Infra.2, Aspire-Infra.3 · Product: unlocks deployment phase+

---

## Story

As a **DevOps / deployment engineer**,  
I want **Aspire to generate Azure Container Apps manifest from AppHost** and **clear documentation on how to deploy to ACA**,  
so that **deployment to production is repeatable and we can validate the Aspire → ACA flow before E2 work**.

---

## Reasoning (the whys)

### Why this story now

Stories Aspire-Infra.0–3 built a working local dev and test harness. Now we validate that Aspire's **native deployment target** (Azure Container Apps) works end-to-end. This is a **pre-MVP gate**: before building features, we must prove the deployment path is solid.

This story also **documents the dev-to-prod workflow** so future stories don't have to re-discover deployment mechanics.

### Why this approach

**Aspire manifest generation** is built-in via `dotnet aspire build`:

```bash
dotnet aspire build --output ./aspire-manifest
```

This generates YAML manifests for:
- API service (image, environment variables, resource requests)
- Web service (static site or container)
- PostgreSQL database (Flexible Server connection string)

The manifests are then deployed to Azure Container Apps via:
```bash
az containerapp env create --name ${env} ...
az containerapp create --yaml ./aspire-manifest/api.yaml --name api ...
```

This approach **validates that Aspire's deployment model matches our architecture** and ensures future E1+ stories don't hit deployment surprises.

**Alternatives considered:**

| Alternative | Why not now |
|-------------|-------------|
| Skip deployment validation, do it later | Risk: discover breakage at release time; Aspire manifest gen is trivial now |
| Manually write Azure IaC (Terraform, Bicep) | Aspire already generates IaC; using Aspire's output is DRY and aligned |
| Deploy to App Service instead of ACA | Contradicts architecture decision (ACA chosen 2026-08-20); Aspire's native target is ACA |

### Out of scope (this story)

- **Actual Azure deployment** — Story documents the flow; deployment to prod deferred to release phase.
- **CI/CD pipeline integration** — Azure Pipelines setup deferred; this story is manual validation.
- **Secrets management in ACA** — Aspire placeholders User Secrets locations; Key Vault linkage deferred.
- **Monitoring / Application Insights setup** — Logging config deferred.
- **Database backup / recovery** — DBA topic; deferred.
- **SSL/TLS certificate management** — Infrastructure deferred.

### Tradeoffs

- **Manual deployment in this story** — Not automated in CI/CD yet. Trade-off: **validation > automation** for MVP. Can automate in v0.1.1.
- **Manifest must be reviewed before deployment** — Aspire output is generated code; should be reviewed (not blindly applied). Adds governance step.

### Planning references

- [architecture-v0.1.md §9](../planning-artifacts/architecture-v0.1.md) — deployment architecture
- .NET Aspire docs: [Build and deploy Aspire applications](https://learn.microsoft.com/en-us/dotnet/aspire/deployment/overview)
- Azure CLI docs: [Container Apps](https://learn.microsoft.com/en-us/cli/azure/containerapp)

---

## Details

### Manifest generation

#### Prerequisites

- Azure CLI installed and authenticated: `az login`
- Azure subscription with Container Registry and Container Apps permissions
- Docker Desktop running locally (for image pushes)

#### Generate manifests

```bash
cd src/ContractorPro.AppHost
dotnet aspire build --output ./manifests
```

Output:
```
./ContractorPro.AppHost/manifests/
  aspire-manifest.yaml  ← Service definitions
  postgres-init.sql     ← Database initialization (if applicable)
```

### Review generated manifest

#### aspire-manifest.yaml structure (example)

```yaml
version: 1
resources:
  api:
    type: container
    image: contractorpro.azurecr.io/api:latest
    env:
      - name: ConnectionStrings__DefaultConnection
        value: "User ID=postgres;Host={postgres};..."
      - name: ASPNETCORE_ENVIRONMENT
        value: Development
    ports:
      - containerPort: 5000
        protocol: tcp
    resources:
      cpu: 0.5
      memory: 1Gi
    
  postgres:
    type: postgres
    image: postgres:16
    env:
      - name: POSTGRES_PASSWORD
        secretRef: postgres-password
    volumes:
      - target: /var/lib/postgresql/data
        path: postgres_data
    resources:
      cpu: 0.5
      memory: 1Gi
      
  web:
    type: static
    build:
      context: ../ContractorPro.Web
      dockerfile: Dockerfile
    env: {}
    ports:
      - containerPort: 5173
```

**Review checklist:**
- [ ] Image references use ACR (Azure Container Registry) URLs
- [ ] Environment variables are templated correctly
- [ ] Secrets are referenced (not hardcoded)
- [ ] Resource CPU/memory are reasonable for development

### Deployment flow (manual, documented)

#### Step 1: Set up Azure resources

```bash
# Create resource group
az group create --name contractorpro-rg --location eastus

# Create container registry
az acr create --resource-group contractorpro-rg --name contractorpro --sku Basic

# Create container app environment
az containerapp env create \
  --name contractorpro-env \
  --resource-group contractorpro-rg \
  --location eastus
```

#### Step 2: Build and push images to ACR

```bash
# Login to ACR
az acr login --name contractorpro

# Build API image
docker build -t contractorpro.azurecr.io/api:latest ./src/ContractorPro.Api
docker push contractorpro.azurecr.io/api:latest

# Build Web image (if not using Static Web Apps)
docker build -t contractorpro.azurecr.io/web:latest ./src/ContractorPro.Web
docker push contractorpro.azurecr.io/web:latest
```

#### Step 3: Deploy manifests to ACA

```bash
# Deploy API container app
az containerapp create \
  --resource-group contractorpro-rg \
  --environment contractorpro-env \
  --name api \
  --yaml ./manifests/aspire-manifest.yaml

# Deploy Web (or use Azure Static Web Apps)
az containerapp create \
  --resource-group contractorpro-rg \
  --environment contractorpro-env \
  --name web \
  --yaml ./manifests/aspire-manifest.yaml
```

#### Step 4: Verify deployment

```bash
# Check container app status
az containerapp show \
  --name api \
  --resource-group contractorpro-rg

# Get API endpoint
az containerapp show \
  --name api \
  --resource-group contractorpro-rg \
  --query properties.latestRevisionFqdn
```

### Documentation artifact

#### docs/deployment/aspire-to-aca-deployment-guide.md (new)

Comprehensive guide covering:

1. **Prerequisites:** Azure CLI, Docker, ACR access, ACA quotas
2. **Local validation:** Run AppHost locally, verify all services healthy
3. **Manifest generation:** `dotnet aspire build` command and output review
4. **Secrets management:** User Secrets (local) → Key Vault (production)
5. **Image build/push:** Docker commands to ACR
6. **Container Apps deployment:** `az containerapp create` with manifest
7. **Post-deployment validation:** Health checks, logging, monitoring
8. **Rollback procedures:** Revert to previous revision
9. **Troubleshooting:** Common issues and fixes

(Full content ~300 lines, follows pattern of existing docs)

### Files to create or modify

| Path | NEW/UPDATE | Purpose |
|------|------------|---------|
| `src/ContractorPro.AppHost/ContractorPro.AppHost.csproj` | UPDATE | Ensure publish profile includes manifest generation (if needed) |
| `docs/deployment/aspire-to-aca-deployment-guide.md` | NEW | Full deployment walkthrough and troubleshooting |
| `docs/deployment/aspire-manifest-checklist.md` | NEW | Pre-deployment review checklist |
| `docs/deployment/.gitignore` | UPDATE | Ignore generated manifests (sensitive data) |

### Tasks / subtasks

- [ ] Test manifest generation (AC: 1)
  - [ ] Run `dotnet aspire build --output ./manifests` from AppHost directory
  - [ ] Verify `aspire-manifest.yaml` is generated correctly
  - [ ] Review manifest for hardcoded secrets (none should be present)
  - [ ] Save manifest as reference (do not commit to git)
- [ ] Create deployment guide (AC: 2)
  - [ ] Write `aspire-to-aca-deployment-guide.md` with all sections
  - [ ] Include step-by-step commands
  - [ ] Add troubleshooting section with common errors
  - [ ] Get peer review from DevOps team or architect
- [ ] Create pre-deployment checklist (AC: 3)
  - [ ] Write `aspire-manifest-checklist.md` for manual review
  - [ ] Include security review points (no hardcoded secrets, env vars correct, resource limits sensible)
  - [ ] Include functional check points (image URLs, ports, health checks)
- [ ] Dry-run deployment to test subscription (AC: 4) ← Optional but recommended
  - [ ] Create test resource group in Azure
  - [ ] Follow guide step-by-step
  - [ ] Verify API is reachable at FQDN
  - [ ] Test health endpoint from outside ACA
  - [ ] Document any issues and update guide
- [ ] Update solution README (AC: 5)
  - [ ] Link to deployment guide from main README.md
  - [ ] Add quickstart: "To deploy to production, see docs/deployment/..."

---

## Acceptance criteria

1. **AC-1:** `dotnet aspire build` generates manifest YAML without errors. Manifest includes all three services (api, postgres, web) and no hardcoded secrets (connection strings use placeholders or environment variable references).

2. **AC-2:** `docs/deployment/aspire-to-aca-deployment-guide.md` exists and covers:
   - Prerequisites and setup steps
   - Manifest generation command and review process
   - Azure resource creation (resource group, ACR, ACA environment)
   - Image build and push to ACR
   - Container app deployment via manifest
   - Post-deployment verification (health checks, endpoint access)
   - Rollback procedure
   - Troubleshooting (at least 3 common issues with solutions)

3. **AC-3:** `docs/deployment/aspire-manifest-checklist.md` provides a security and functional review checklist (at least 10 items) that a reviewer can follow before approving a manifest for deployment.

4. **AC-4 (optional):** Dry-run deployment to test subscription succeeds. API is reachable at assigned FQDN. Health endpoint returns 200. Deployment issues (if any) are documented and guide is updated.

5. **AC-5:** Main README.md or a new `DEPLOYMENT.md` file links to deployment guide. Contributors know where to find deployment instructions.

---

## Security & vulnerability review

| Check | Notes |
|-------|-------|
| **Manifest review** | Manifests must never include hardcoded secrets; Aspire should use placeholders or env var refs. Add pre-deployment checklist to enforce this. |
| **Image security** | Container images pushed to ACR should be scanned for CVEs. Aspire manifest generation doesn't auto-scan; must be added to build pipeline. |
| **Key Vault integration** | Guide documents User Secrets (local) → Key Vault (prod). Actual KV setup is a follow-up. |
| **RBAC** | Container Apps deployment requires ACR pull and ACA write permissions. Guide should document required roles. |
| **Network** | ACA deployment defaults to public endpoint. Guide should note that private endpoints can be configured later. |
| **Secrets in logs** | Ensure manifest review catches any connection strings in plain text. |

**Findings:**
- Add to checklist: "No secrets in manifest. Sensitive values use Key Vault references or env var substitution."
- Recommend: Add a CI/CD pre-check that scans generated manifests for common secret patterns (passwords, API keys).

---

## Unit tests

N/A — This is documentation and deployment validation. Verification is manual (AC-4 dry-run).

---

## Manual verification checklist

- [ ] `dotnet aspire build --output ./manifests` runs without errors
- [ ] `./manifests/aspire-manifest.yaml` exists and is valid YAML
- [ ] Manifest contains 3 resources: api, postgres, web
- [ ] No hardcoded passwords or connection strings in manifest (use `{service}` or `$()` templating)
- [ ] `docs/deployment/aspire-to-aca-deployment-guide.md` is comprehensive and well-formatted
- [ ] Checklist file exists and covers security + functional concerns
- [ ] (Optional) Follow deployment guide on test subscription:
  - [ ] All Azure CLI commands run without errors
  - [ ] Container apps are created successfully
  - [ ] API is accessible at FQDN and returns 200 on `/health`
  - [ ] Web loads successfully
  - [ ] Teardown (delete resource group) leaves no orphaned resources
- [ ] README.md links to deployment guide

---

## Dependencies and follow-ups

**This story is the gate for deployment readiness.** Subsequent phases (release, scaling, monitoring) depend on successful dry-run validation (AC-4).

**Future stories may extend this:**
- **CI/CD automation:** Azure Pipelines to auto-build and push images
- **Key Vault integration:** Automatic secret injection into ACA
- **Monitoring setup:** Application Insights configuration in manifest
- **Multi-environment:** Separate manifests for staging/prod with different resource allocations

---
