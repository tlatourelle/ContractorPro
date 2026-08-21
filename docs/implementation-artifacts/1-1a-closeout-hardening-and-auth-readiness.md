---
status: done
parent_story: 1.1
created: 2026-08-21
---

# Story 1.1a: Closeout hardening and auth readiness

Status: done

Epic: 1 · Depends: 1.1 · Product: E1-S1, E1-S7, E1-S8 · Milestone: M1

---

## Story

As a delivery team,
we need to close out Story 1.1 with security, automation, and auth-readiness hardening,
so that local validation is reliable and real Google sign-in can be enabled cleanly when Entra config is ready.

---

## Scope completed

### 1) Local developer automation

- Added and stabilized local scripts:
  - `scripts/run-local.ps1`
  - `scripts/stop-local.ps1`
  - `scripts/setup-and-test.ps1`
- Automated Docker startup and Postgres health checks.
- Automated EF tool restore and migration update path.
- Added process cleanup to avoid Windows file-lock build failures.

### 2) Security and dependency remediation

- Fixed high severity SQLite package warning by pinning patched package in test project.
- Upgraded .NET package versions to compatible stable lines.
- Modernized frontend lint stack to ESLint 10 and migrated to flat config.
- Reduced setup noise by resolving deprecated lint dependency chain.

### 3) Auth usability and readiness

- Added runtime auth state service and dev-only toggle API endpoints:
  - `GET /api/v1/auth/config`
  - `POST /api/v1/auth/config`
- Added Login UI controls for enable/disable auth in development.
- Added explicit incomplete-config response handling (`auth_config_incomplete`) before challenge.

### 4) Test strategy and planning updates

- Added E2E planning stories (CI-safe Playwright plus manual provider smoke split).
- Updated Story 1.1 E2E section with deterministic CI strategy.

---

## Validation completed

- Backend build and tests pass under stabilized setup script workflow.
- Frontend lint and production build pass after ESLint 10 migration.
- Full `setup-and-test` run completes without prior deprecation and vulnerability blockers.

---

## Entra External ID setup required for real Google sign-in

Real Google sign-in in Story 1.1 requires Entra External ID configuration. Without this, the app correctly returns `auth_not_configured` or `auth_config_incomplete`.

### Required cloud setup

1. Create or use an Entra External ID tenant.
2. Configure Google as an identity provider in that tenant.
3. Register the ContractorPro API app as a confidential client.
4. Add redirect URI for local callback:
   - `https://localhost:5000/signin-oidc` (or your actual local API origin)
5. Generate a client secret and store it locally via user-secrets.

### Required local config keys

Set these for `src/ContractorPro.Api` user-secrets:

- `Authentication:ExternalId:Enabled`
- `Authentication:ExternalId:Authority`
- `Authentication:ExternalId:ClientId`
- `Authentication:ExternalId:ClientSecret`
- `Authentication:ExternalId:CallbackPath` (typically `/signin-oidc`)

### Example local commands

```powershell
dotnet user-secrets set "Authentication:ExternalId:Enabled" "true" --project src/ContractorPro.Api
dotnet user-secrets set "Authentication:ExternalId:Authority" "https://<tenant>.ciamlogin.com/<tenant-id>/v2.0" --project src/ContractorPro.Api
dotnet user-secrets set "Authentication:ExternalId:ClientId" "<client-id>" --project src/ContractorPro.Api
dotnet user-secrets set "Authentication:ExternalId:ClientSecret" "<client-secret>" --project src/ContractorPro.Api
dotnet user-secrets set "Authentication:ExternalId:CallbackPath" "/signin-oidc" --project src/ContractorPro.Api
```

### Manual verification steps

1. Start local stack.
2. Open `/app/login`.
3. Confirm auth status says enabled and configured.
4. Click Sign in with Google.
5. Confirm redirect to Entra and then Google.
6. Complete sign-in and confirm landing on `/app/dashboard`.

---

## Definition of done

- [x] Story 1.1 hardening changes documented
- [x] Security and dependency updates documented
- [x] Auth toggle behavior documented
- [x] Entra setup and local config requirements documented
- [x] Next story handoff ready (Story 1.2)
