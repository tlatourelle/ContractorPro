# Stack Exploration — Web + API + DB

Status: **Exploratory** (2026-08-13)  
Context: [azure-alignment.md](./azure-alignment.md), [auth-and-data.md](./auth-and-data.md), [database-options.md](./database-options.md)

## Direction from team

| Input | Notes |
|-------|-------|
| **Personal strength** | **.NET** over other stacks |
| **Architecture** | **Web frontend** ↔ **API backend** ↔ **database** |
| **Build style** | **“Vibe coded”** — AI-assisted; open to tools that accelerate, not dogmatic |
| **Cloud lean** | Azure over AWS ([azure-alignment.md](./azure-alignment.md)) |

**No final stack decision** — this doc compares realistic options.

---

## Target architecture (logical)

```
┌─────────────────────────────────────────────────────────────┐
│  Browser / mobile web                                        │
│  Web frontend (SPA or SSR)                                   │
└──────────────────────────┬──────────────────────────────────┘
                           │ HTTPS / JSON REST (or minimal APIs)
┌──────────────────────────▼──────────────────────────────────┐
│  API backend (.NET — preferred)                              │
│  Auth validation · business logic · cascade engine · messaging│
└──────────────────────────┬──────────────────────────────────┘
                           │
┌──────────────────────────▼──────────────────────────────────┐
│  PostgreSQL                                                  │
│  Dev: Neon / Docker · Prod: Azure PostgreSQL (likely)         │
└─────────────────────────────────────────────────────────────┘

External: Google Calendar, Twilio SMS, Chargebee/Stripe, (later) QBO
```

**Principle:** Frontend stays thin — scheduling rules, permissions, and cascade logic live in the **API**, not duplicated in the browser.

---

## Backend — .NET (leading candidate)

**ASP.NET Core Web API** (.NET 8/9) aligns with team skill and Azure.

| Piece | Typical choice | Notes |
|-------|----------------|-------|
| **Framework** | ASP.NET Core Minimal APIs or Controllers | Minimal APIs fine for vibe-coded CRUD; controllers if structure helps |
| **ORM** | **EF Core** + PostgreSQL provider (`Npgsql`) | Migrations built-in; AI tools know EF well |
| **Auth** | See auth section below | Validate JWT/session from Entra, OpenIddict, or Clerk |
| **Background jobs** | Azure Functions or `IHostedService` + queue later | SMS retries, cascade notifications |
| **API style** | REST + OpenAPI (Swagger) | Clear contracts for frontend + AI codegen |

**Pros:** Your expertise; first-class Azure; strong typing helps AI refactors; Entra integration native.  
**Cons:** Heavier than a single Next.js monolith for a solo MVP (two deployables).

---

## Frontend — options (open)

Vibe coding often favors **React** ecosystems (large AI training corpus), but **.NET familiarity** matters for who reviews code.

### Option A — React SPA + .NET API (common split)

| Layer | Tech |
|-------|------|
| UI | **React** + TypeScript (Vite or Next.js as static export) |
| API | ASP.NET Core |
| Deploy | Azure Static Web Apps (frontend) + App Service (API) **or** both on App Service |

**Pros:** Huge AI/example surface; modern component libs (shadcn, etc.); clear separation.  
**Cons:** Two languages; CORS/auth cookie setup between origins.

**Vibe-coding fit:** ⭐⭐⭐ — most examples online.

---

### Option B — Blazor WebAssembly or Auto + .NET API

| Layer | Tech |
|-------|------|
| UI | **Blazor** (WASM or Server) |
| API | ASP.NET Core (can share DTOs/models in solution) |

**Pros:** **One language (C#)** end-to-end; shared types; plays to .NET strength.  
**Cons:** Smaller UI ecosystem vs React; WASM bundle size; fewer copy-paste AI snippets for fancy UI.

**Vibe-coding fit:** ⭐⭐ — improving, but React wins for generic AI output.

---

### Option C — Next.js frontend + .NET API

| Layer | Tech |
|-------|------|
| UI | Next.js (App Router) |
| API | ASP.NET Core (separate repo or folder) |

**Pros:** Best of React SSR + your .NET backend.  
**Cons:** Two stacks; you know .NET more than Node — frontend still JS/TS.

**Vibe-coding fit:** ⭐⭐⭐ for UI, ⭐ for backend alignment with your skills.

---

### Option D — ASP.NET Core serves API + minimal Razor/HTMX (later alternative)

Server-rendered pages with **htmx** or Razor Pages for simple portals (homeowner/sub views).

**Pros:** One deployable; great for **lightweight invitee portals**.  
**Cons:** Less “app-like” for GC dashboard; may outgrow for rich scheduling UI.

**Vibe-coding fit:** ⭐⭐ — good for sub/homeowner magic-link pages specifically.

**Hybrid idea (not decided):** React (or Blazor) for **GC app**; simpler Razor/HTMX or small React bundle for **invitee portals**.

---

## Draft lean (not final)

| Layer | Lean | Why |
|-------|------|-----|
| **API** | **ASP.NET Core** | Your skill + Azure |
| **DB** | **PostgreSQL** + EF Core | Fits data model; Azure PG later |
| **GC web app** | **React + TypeScript** (Vite SPA) | Vibe coding + UI flexibility |
| **Invitee portals** | **Simple React pages or Razor** | Keep magic-link views thin |
| **Hosting** | App Service (API) + Static Web Apps or same App Service (UI) | Azure-aligned |

**Blazor** remains a valid swap if you want single-language and accept smaller UI ecosystem.

---

## Auth with .NET API

| Approach | .NET integration | Azure fit |
|----------|------------------|-----------|
| **Entra External ID** | `Microsoft.Identity.Web` | ⭐⭐⭐ |
| **OpenIddict** (self-hosted OIDC) | Native ASP.NET | ⭐⭐ |
| **Clerk** | JWT validation middleware | ⭐ (SaaS) |
| **Auth.js** | ❌ Node-centric — poor fit for .NET API | |

**Draft lean:** Entra External ID **or** OpenIddict on the API; magic links implemented in .NET for invitees.

Auth.js is a weak match if API is .NET — deprioritize unless frontend-only auth (unusual for this architecture).

---

## Solution structure (sketch)

```
ContractorPro/
  src/
    ContractorPro.Api/          # ASP.NET Core Web API
    ContractorPro.Core/         # Domain: projects, tasks, cascade, messages
    ContractorPro.Infrastructure/ # EF Core, external APIs (Google, Twilio)
    ContractorPro.Web/            # React SPA (or Blazor.Client)
  tests/
  docs/
```

Monorepo in one git repo; two deployables (API + static web).

---

## Vibe-coding practices (process, not tech)

| Practice | Why |
|----------|-----|
| **OpenAPI/Swagger** on API | Frontend + AI can generate clients |
| **Thin controllers** | Business logic in testable services |
| **EF migrations** | Schema changes traceable |
| **Strong DTOs** | Clear API contracts for AI edits |
| **Feature folders** | `Features/Scheduling/`, `Features/Messaging/` — easier to prompt in chunks |
| **Don’t vibe-coding auth/security** | Hand-review OAuth, magic links, tenant isolation |

---

## Azure deployment sketch

| Component | Azure service |
|-----------|---------------|
| API | App Service (Linux, .NET 8) |
| Frontend | Azure Static Web Apps **or** App Service static |
| DB | PostgreSQL Flexible Server |
| Secrets | Key Vault |
| SMS | Twilio (external) or Azure Communication Services |
| CI/CD | GitHub Actions → Azure |

---

## Open questions — stack

- [ ] **React vs Blazor** for GC dashboard — try a 1-day UI spike?
- [ ] **Minimal APIs vs Controllers** — team preference?
- [ ] **Separate subdomain** — `app.contractorpro.com` + `api.contractorpro.com`?
- [ ] **Real-time messaging** — SignalR on API vs polling vs SSE?
- [ ] **Invitee portal** — same React app (role-based) vs separate thin app?
- [ ] **EF Core vs Dapper** for hot paths (probably EF until proven slow)
- [ ] **OpenAPI client generation** for frontend (NSwag, Kiota)

---

## What we’re not doing (for now)

- Full Next.js monolith (API in Node) — fights .NET strength
- Auth.js as primary — Node-oriented
- Mobile native apps
- Microservices — single API is enough for years

Log decisions in [discovery-log.md](../discovery-log.md).
