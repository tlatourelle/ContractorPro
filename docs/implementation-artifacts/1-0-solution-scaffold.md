# Story 1.0: Solution scaffold

Status: ready-for-dev

Epic: 1 · FR: (foundation) · Journey: (none — enabler) · Depends: — · Product: unlocks E1-S1

---

## Story

As a **developer**,  
I want a **runnable .NET + React + PostgreSQL solution** with health checks and local dev workflow,  
so that **auth and features can be built on a consistent foundation without re-deciding structure each story**.

---

## Reasoning (the whys)

### Why this story now

The repo has **no application code**. Every subsequent story (1.1 auth, projects, MMS) needs shared structure: solution layout, EF Core + Postgres, React SPA shell, test projects, and a provable “it runs” checkpoint. Building auth on an ad-hoc folder layout would force a painful refactor after story 1.1.

### Why this approach

**Modular monolith** per [architecture-v0.1.md §7](../planning-artifacts/architecture-v0.1.md): `Api`, `Application`, `Domain`, `Infrastructure`, `Web`, plus `Application.Tests` and `Api.Tests`. This matches Thomas’s .NET strength and BMAD dev-story expectations.

**Alternatives considered:**

| Alternative | Why not now |
|-------------|-------------|
| Single `WebApi` project only | Would split later anyway; architecture already decided |
| Blazor frontend | Rejected 2026-08-20 — React for AI-assisted UI |
| SQL Server | Postgres locked in architecture |

### Out of scope (this story)

- Entra / Google OAuth, cookies, sessions
- Business entities beyond minimal `platform_settings` seed (optional) or empty migration
- Twilio, Resend, Stripe, Google Calendar
- CI deploy to Azure (only **build + test** CI skeleton)
- shadcn full design system (Tailwind + placeholder layout only)
- Docker Compose is **in scope** for local Postgres

### Tradeoffs

- **Minimal domain model** in 1.0 — only what’s needed to prove EF + migrations work; full schema lands incrementally (auth tables in 1.1).
- **Api hosts React in dev** via proxy or separate Vite dev server — document chosen approach in Dev Agent Record; production pattern can evolve.

### Planning references

- [architecture-v0.1.md §1.1, §7](../planning-artifacts/architecture-v0.1.md)
- [epics-and-stories.md M1–M21](../planning-artifacts/prds/prd-ContractorPro-2026-08-15/epics-and-stories.md) — M1 depends on this scaffold
- [story-standard.md](./story-standard.md)

---

## Details

### Solution layout

Create under repository root:

```
src/
  ContractorPro.Api/              # ASP.NET Core 9 host
  ContractorPro.Application/      # Application services (empty modules folder)
  ContractorPro.Domain/           # Entities (placeholder or empty)
  ContractorPro.Infrastructure/   # EF Core, DbContext, migrations
  ContractorPro.Web/              # Vite + React 19 + TypeScript
tests/
  ContractorPro.Application.Tests/
  ContractorPro.Api.Tests/
ContractorPro.sln
docker-compose.yml                # postgres:16 for local dev
.github/workflows/ci.yml          # build + test on push (optional if no GitHub yet — document manual run)
```

### API

| Route | Method | Auth | Response |
|-------|--------|------|----------|
| `/api/v1/health` | GET | None | `200` `{ "status": "healthy", "database": "healthy" \| "unhealthy" }` |
| `/api/v1/health/live` | GET | None | `200` `{ "status": "alive" }` — no DB check |

- CORS configured for Vite dev origin (`http://localhost:5173` default).
- Structured logging to console (App Insights later).
- `appsettings.Development.json` — connection string via env `ConnectionStrings__Default` or User Secrets template (no secrets committed).

### Data model (minimal)

- `ContractorProDbContext` registered with Npgsql.
- **Optional:** empty initial migration **or** single `platform_settings` table + seed row (`dashboard_poll_interval_seconds` = 60) to prove migrations — **no auth tables yet** (story 1.1).

### Frontend

- Vite + React 19 + TypeScript + React Router.
- Routes (placeholders): `/` → redirect `/app`, `/app` → “ContractorPro — scaffold” stub, `/p` → “Portal — scaffold” stub.
- Hand-typed `fetch` helper targeting `/api/v1` with `credentials: 'include'` (for 1.1).
- Tailwind CSS installed; shadcn can be initialized lightly or deferred to 1.1 UI polish — **at minimum** Tailwind works.

### Local dev workflow

Document in `src/README.md` (NEW):

1. `docker compose up -d` (Postgres)
2. `dotnet ef database update` (from Infrastructure project)
3. `dotnet run --project src/ContractorPro.Api` (API on e.g. 5001)
4. `npm run dev` in `ContractorPro.Web` (Vite on 5173)

### Files to create or modify

| Path | NEW/UPDATE | Purpose |
|------|------------|---------|
| `ContractorPro.sln` | NEW | Solution |
| `src/ContractorPro.Api/*` | NEW | Host, Program.cs, health endpoints |
| `src/ContractorPro.Application/*` | NEW | Project shell |
| `src/ContractorPro.Domain/*` | NEW | Project shell |
| `src/ContractorPro.Infrastructure/*` | NEW | DbContext, DI extension |
| `src/ContractorPro.Web/*` | NEW | React SPA |
| `tests/ContractorPro.Api.Tests/*` | NEW | Integration tests |
| `tests/ContractorPro.Application.Tests/*` | NEW | Unit test shell |
| `docker-compose.yml` | NEW | Postgres 16 |
| `.gitignore` | UPDATE | .NET, Node, secrets |
| `src/README.md` | NEW | Dev workflow |

### Tasks / subtasks

- [ ] Task 1: Create solution and projects (AC: 1, 2)
  - [ ] `dotnet new sln` + classlib/web projects per §7
  - [ ] Project references: Api → Application, Infrastructure; Infrastructure → Domain
- [ ] Task 2: Postgres + EF Core (AC: 3, 4)
  - [ ] docker-compose Postgres 16
  - [ ] DbContext + initial migration
  - [ ] Health check includes DB connectivity
- [ ] Task 3: React SPA shell (AC: 5)
  - [ ] Vite React TS + Router + Tailwind
  - [ ] Stub `/app` and `/p` routes
- [ ] Task 4: Tests + CI skeleton (AC: 6, 7)
  - [ ] Api integration test for `/health`
  - [ ] CI workflow: `dotnet build`, `dotnet test`, `npm ci && npm run build` in Web
- [ ] Task 5: Developer docs (AC: 8)
  - [ ] `src/README.md` with steps above

---

## Acceptance criteria

1. **AC-1:** `dotnet build` on solution succeeds with zero errors.
2. **AC-2:** Solution contains Api, Application, Domain, Infrastructure, Web, and two test projects per architecture §7.
3. **AC-3:** `docker compose up` starts Postgres; API connects via configured connection string.
4. **AC-4:** `GET /api/v1/health` returns `200` with `database: healthy` when Postgres is up; `database: unhealthy` when Postgres is down (not 500 unhandled).
5. **AC-5:** `npm run dev` serves React app; navigating to `/app` shows scaffold page; browser console has no fatal errors.
6. **AC-6:** `dotnet test` runs at least one passing integration test for health endpoint (WebApplicationFactory).
7. **AC-7:** GitHub Actions (or documented equivalent) workflow file runs build + test — or `src/README.md` documents manual CI commands if Actions not wired yet.
8. **AC-8:** No secrets, connection strings with passwords, or API keys committed to git; `.gitignore` covers `appsettings.*.local`, `.env`, user-secrets.

---

## Security & vulnerability review

| Check | Applicable? | Mitigation / notes |
|-------|-------------|-------------------|
| Authentication | N/A | No auth in this story |
| Authorization / tenancy | N/A | No tenant data yet |
| Input validation | Low | Health endpoints have no user input |
| Secrets handling | **Yes** | Connection string via env/user-secrets; document in README; scan git for accidental commits |
| Injection (SQL/XSS) | Low | EF only; no user SQL; React default escaping |
| CSRF | N/A | No state-changing public endpoints |
| Rate limiting | N/A | Health only |
| Dependency / supply chain | **Yes** | Pin major versions (.NET 9, React 19); run `dotnet list package --vulnerable` once |
| Privacy / logging | Low | No PII logged |

**Identified risks:**

1. **Default Postgres password in docker-compose** → **Mitigation:** dev-only; bind to localhost; document “never use in prod”; add comment in compose file.
2. **CORS `*` in dev** → **Mitigation:** restrict to `http://localhost:5173` (and API origin), not `AllowAnyOrigin` with credentials.

**Follow-up stories:** 1.1 adds auth middleware, secure cookies, CSRF considerations for BFF.

---

## Unit tests

| Test | AC | Description |
|------|-----|-------------|
| *(none required)* | — | No business logic yet |

**Run:** `dotnet test tests/ContractorPro.Application.Tests`

*N/A reason:* Application layer is empty shell; health logic lives in API integration test.

---

## Integration tests

| Test | AC | Description |
|------|-----|-------------|
| `HealthEndpoint_ReturnsOk_WhenDatabaseAvailable` | AC-4, AC-6 | WebApplicationFactory + test container or mocked DbContext health |
| `HealthEndpoint_ReturnsDatabaseUnhealthy_WhenConnectionFails` | AC-4 | Invalid connection string or stopped DB |

**Run:** `dotnet test tests/ContractorPro.Api.Tests`

*Note:* Use Testcontainers.PostgreSql or EF InMemory only for non-health tests — prefer real Postgres or health check mock for AC-4 accuracy.

---

## E2E tests

| Test | AC | Description |
|------|-----|-------------|
| *(deferred)* | AC-5 | Playwright: load `/app` scaffold |

**Run:** N/A this story

*Deferred reason:* Playwright harness added when auth flow exists (story 1.1). **Compensated by** manual QA MQ-3 and `npm run build` in CI.

---

## Manual QA checklist

- [ ] **MQ-1:** Clone repo fresh; follow `src/README.md` from scratch.
  - **Expected:** Postgres up, migrations apply, API + Vite start without undocumented steps.
- [ ] **MQ-2:** `curl http://localhost:5001/api/v1/health` (or configured port).
  - **Expected:** JSON with `"status":"healthy"` and `"database":"healthy"`.
- [ ] **MQ-3:** Open `http://localhost:5173/app` in browser.
  - **Expected:** Scaffold page visible; no red errors in devtools console.
- [ ] **MQ-4:** Stop Postgres; hit `/api/v1/health` again.
  - **Expected:** Still `200` with `database: unhealthy` OR documented behavior — not unhandled 500.
- [ ] **MQ-5:** `git grep -i password` on staged files before commit.
  - **Expected:** No real credentials in tracked files.

**Sign-off:** Thomas · Date: ___

---

## Definition of done

- [ ] All AC met
- [ ] Security review complete
- [ ] Integration tests passing
- [ ] Manual QA complete
- [ ] Dev Agent Record updated
- [ ] CI green (or manual equivalent documented)

---

## Dev Agent Record

### Agent model

*(filled on implementation)*

### Completion notes

### File list

### Test results
