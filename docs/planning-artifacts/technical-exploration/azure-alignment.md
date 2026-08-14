# Azure Alignment — Exploration

Status: **Preference noted** (2026-08-13) — not a full platform commitment  
Team context: **More likely Azure than AWS** given skillset.

Related: [auth-and-data.md](./auth-and-data.md), [database-options.md](./database-options.md)

---

## What this means (and doesn’t)

| This **is** | This **is not** |
|-------------|-----------------|
| Default **growth path** for hosting, DB, and Microsoft-centric auth | “Must use Azure on day one” |
| Reason to **favor** Entra, App Service, Azure PostgreSQL in comparisons | A ban on Neon/Supabase for dev/MVP |
| Alignment if stack is **.NET** | Final decision on framework or auth vendor |

You can still **develop cheaply** (Neon free, Clerk free, local Docker) and **land on Azure** when paying customers need SLA.

---

## Suggested Azure-shaped architecture (future reference)

```
                    ┌─────────────────────────────────────┐
                    │  Azure Front Door / CDN (later)      │
                    └──────────────────┬──────────────────┘
                                       │
                    ┌──────────────────▼──────────────────┐
                    │  Azure App Service or Container Apps   │
                    │  (.NET or Node API + web)              │
                    └──────────────────┬──────────────────┘
           ┌───────────────────────────┼───────────────────────────┐
           │                           │                           │
┌──────────▼──────────┐   ┌────────────▼────────────┐   ┌─────────▼─────────┐
│ Entra External ID   │   │ Azure Database for       │   │ Azure Key Vault   │
│ or Clerk/Auth.js    │   │ PostgreSQL Flexible      │   │ secrets, conn     │
│ (GC OAuth BYOA)     │   │ Server                   │   │ strings           │
└─────────────────────┘   └──────────────────────────┘   └───────────────────┘
           │
┌──────────▼──────────┐   ┌──────────────────────────┐
│ Magic links (custom)│   │ Application Insights      │
│ subs / homeowners   │   │ logging & monitoring      │
└─────────────────────┘   └──────────────────────────┘

External (not Azure): Google Calendar API, Twilio SMS, Chargebee/Stripe
```

---

## By concern area

### Hosting (app)

| Phase | Azure option | Notes |
|-------|--------------|-------|
| **Dev / spike** | Local + Neon **or** App Service free/low tier | Don’t over-provision early |
| **MVP prod** | **Azure App Service** (Linux) or **Container Apps** | Familiar ops; easy scale up |
| **Later** | App Service scale-out, Front Door, staging slots | Blue/green deploys |

**AWS equivalent deferred** unless a specific service forces it (none identified yet).

---

### Database

| Phase | Option | Notes |
|-------|--------|-------|
| **Dev** | Neon free, Docker Postgres locally, or Azure PG with credits | Portable Postgres either way |
| **First paying customers** | **Azure Database for PostgreSQL — Flexible Server** (Burstable B1ms) | US region, automated backup on paid SKU |
| **Scale** | Zone-redundant HA, read replica, PgBouncer | Standard Azure PG path |

See [database-options.md](./database-options.md) for Neon/Supabase comparison. **Postgres on Azure** is the natural long-term host given team skills.

---

### Authentication

| Approach | Azure fit |
|----------|-----------|
| **[Microsoft Entra External ID](https://learn.microsoft.com/en-us/entra/external-id/)** | **Strong** — native Microsoft; GC “work account” login; enterprise SSO later |
| **Auth.js / OpenIddict on App Service** | **Strong** if .NET — sessions in Azure PG |
| **Clerk** | Works fine; auth stays SaaS, app on Azure |
| **Supabase Auth** | Possible but splits platform (Supabase cloud + Azure app) — less aligned |

**Draft lean (not decided):** Entra External ID or **.NET Identity + OpenIddict** for max Azure alignment; Clerk if speed beats platform purity.

Invitee **magic links** remain custom in app + Azure PG regardless.

---

### Observability & secrets

| Service | Use |
|---------|-----|
| **Application Insights** | Traces, errors, usage |
| **Azure Key Vault** | DB connection strings, API keys (Google, Twilio, Chargebee) |
| **Log Analytics** | Central logs at scale |

---

### Integrations (unchanged, Azure-adjacent)

| Integration | Notes |
|-------------|-------|
| **Google Calendar** | Still v0.1 — OAuth to Google, not Entra |
| **Microsoft 365 Calendar** | Natural **phase 2** on Azure + Entra skillset |
| **Chargebee / Stripe** | External SaaS; no AWS/Azure requirement |
| **QuickBooks Online** | External; explore post-MVP |

---

## Dev-now vs Azure-later (pragmatic path)

Many teams with Azure skills still start lean:

```
Planning     →  docs only (now)
Dev spike    →  Neon free OR local Postgres + Clerk/Auth.js trial
Early MVP    →  App Service (small) + Azure PG Burstable OR Neon paid
Growth       →  Full Azure: App Service/ACA + PG HA + Entra + Key Vault + App Insights
```

**Migration:** Postgres `pg_dump` from Neon → Azure PG when ready. App code stays the same if you avoid vendor-specific DB features.

---

## Cost awareness (rough)

| Resource | Early ballpark |
|----------|----------------|
| App Service B1 | ~$13–25/mo |
| Azure PG Burstable B1ms | ~$15–35/mo |
| Entra External ID | MAU-based — check current Entra pricing |
| Application Insights | Low volume often negligible |

**Total early Azure prod:** ~$30–60/mo before SMS/auth SaaS — still fine for a paid SaaS with a few GC subscribers.

Free tier for **pure planning** remains Neon + local dev; Azure credits can offset first deploy experiments.

---

## Open questions (Azure-specific)

- [ ] **Frontend** — React vs Blazor on App Service — see stack-web-api-db.md
- [ ] **Entra External ID** vs Clerk for MVP — complexity vs Azure-native
- [ ] **Single region** (e.g. East US 2) for US-only product
- [ ] **Bicep or Terraform** for infra-as-code when deploy starts
- [ ] **Azure Communication Services** for SMS vs Twilio
- [ ] **Static web** (Azure Static Web Apps) + API split vs monolith App Service

---

## AWS

**Deprioritized** for ContractorPro unless a specific integration requires it. No current product requirement points to AWS.

Log changes in [discovery-log.md](../discovery-log.md).
