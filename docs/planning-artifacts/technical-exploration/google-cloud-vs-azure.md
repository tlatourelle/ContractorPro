# Google Cloud vs Azure — Hosting Decision

Status: **Exploratory** (2026-08-13)  
Related: [azure-alignment.md](./azure-alignment.md), [google-calendar-integration.md](./google-calendar-integration.md), [stack-web-api-db.md](./stack-web-api-db.md)

## Question

> If we use Google Calendar, does using **Google Cloud Platform (GCP)** for hosting make sense?

## Short answer

**A Google Cloud *project* — yes** (for Calendar API, OAuth, consent screen).  
**Running the whole app on GCP — optional, not required.**

Most teams host the **.NET API on Azure** and use a **Google Cloud project only for Google APIs**. Calendar integration does **not** force GCP hosting.

---

## What actually requires Google

| Need | Google surface | Requires GCP hosting? |
|------|----------------|----------------------|
| Google Calendar API | Google Cloud project + OAuth client | **No** |
| “Sign in with Google” | Same or linked Cloud project | **No** |
| Google OAuth token refresh | Your API stores tokens; calls `googleapis.com` | **No** |
| Push notifications (calendar webhooks) | Public HTTPS endpoint — **any host** | **No** |
| Google app verification (sensitive scopes) | Cloud Console / OAuth consent | **No** |

Your ASP.NET Core API on **Azure App Service** calls `https://www.googleapis.com/calendar/v3/...` like any HTTP client.

---

## Pattern A — Azure app + Google Cloud project (recommended lean)

```
┌─────────────────────────────┐     ┌──────────────────────────────┐
│  Azure                       │     │  Google Cloud (APIs only)     │
│  App Service (.NET API)      │────▶│  OAuth client                 │
│  Azure PostgreSQL            │     │  Calendar API enabled         │
│  Key Vault                   │     │  OAuth consent screen         │
│  Application Insights        │     │  (no App Engine required)     │
└─────────────────────────────┘     └──────────────────────────────┘
         ▲
         │ HTTPS
┌────────┴────────┐
│  Web frontend   │
│  (React/Blazor) │
└─────────────────┘
```

**Pros:**
- Matches **.NET + Azure skillset**
- One ops model for compute, DB, secrets, monitoring
- Google Cloud project is **free/low cost** for API credentials only
- Industry-standard split (many SaaS apps are Azure/AWS + Google APIs)

**Cons:**
- Two vendor consoles (Azure + Google Cloud Console)
- OAuth redirect URIs must list Azure app URLs

---

## Pattern B — Full GCP hosting

```
Cloud Run or GKE (.NET) + Cloud SQL (Postgres) + Google Calendar same org
```

**Pros:**
- Single cloud vendor for **Google APIs + hosting**
- Cloud Run scales to zero (cost)
- Same billing account as Google Workspace if GC uses it (uncommon for your SaaS)

**Cons:**
- **New platform** vs your Azure/.NET comfort
- Cloud SQL ≠ Azure PG skill reuse
- Entra / Azure-native auth story weaker
- You already documented **Azure over AWS** — adding GCP is a second full platform

**When it might make sense:**
- Team equally strong on GCP
- Heavy Google stack (BigQuery, Pub/Sub, Firebase)
- Google Workspace Marketplace distribution

**For ContractorPro today:** Weak fit given Azure + .NET lean.

---

## Pattern C — Firebase / Google-centric mobile-first (not current direction)

Firebase Auth + Firestore — fights relational cascade model and .NET API plan. **Not recommended** for this architecture.

---

## Comparison table

| Criterion | Azure + Google Cloud project | Full GCP |
|-----------|------------------------------|----------|
| Google Calendar integration | ✅ Same | ✅ Same |
| .NET team skillset | ✅ | ⭐⭐ (Cloud Run supports .NET) |
| Postgres + EF Core | Azure PG | Cloud SQL |
| Auth (Entra, OpenIddict) | ✅ Natural | Less natural |
| Vendor count for **ops** | 2 (Azure + Google APIs) | 1 |
| Vendor count for **billing** | Azure + ~$0 Google API | GCP only |
| Calendar API latency | Irrelevant difference | Irrelevant difference |
| Vibe-coded .NET API | ✅ App Service familiar | New deploy path |

---

## Costs (rough)

| Item | Cost |
|------|------|
| **Google Cloud project** (Calendar API, OAuth) | **$0** for API usage at MVP scale; Calendar API has generous quotas |
| **Google OAuth verification** | Time/process, not hosting — required for production sensitive scopes |
| **Azure App Service + PG** | ~$30–60/mo early prod (unchanged) |
| **Full GCP equivalent** | Similar $ — not cheaper by virtue of Calendar alone |

**Calendar integration does not save money by moving to GCP.**

---

## What to create in Google Cloud (minimal)

Even on Azure hosting, create:

1. **Google Cloud project** (e.g. `contractorpro-prod`)
2. Enable **Google Calendar API**
3. **OAuth consent screen** (app name, logo, privacy policy URL)
4. **OAuth 2.0 Client ID** (Web application) — redirect URIs point to **Azure** API/auth callback URLs
5. Store `client_id` / `client_secret` in **Azure Key Vault**
6. (Later) Pub/Sub or webhook URL → Azure API endpoint for calendar push notifications

Same project can host **Google Sign-In** client if desired.

---

## Open questions

- [ ] One Google Cloud project for dev + prod, or separate?
- [ ] Google Sign-In and Calendar — same OAuth client or two?
- [ ] Workspace domain-wide delegation — needed? (Usually **no** for small GC SaaS; per-user OAuth yes)
- [ ] Customer discovery: any GC insist on “everything in Microsoft” including calendar? (Already deprioritized M365 calendar)

---

## Draft lean (not final)

| Layer | Where |
|-------|--------|
| **Hosting** | **Azure** (App Service, Azure PG) |
| **Google Calendar + Google OAuth** | **Google Cloud project** (APIs only) |
| **Microsoft sign-in** | Entra (Azure) — optional, separate from calendar |

**Do not move hosting to GCP solely because of Google Calendar.**

Log decisions in [discovery-log.md](../discovery-log.md).
