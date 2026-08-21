---
status: backlog
---

# Story Aspire-Infra.2: Dashboard, health checks, and launch experience

Status: backlog

Epic: Aspire Infrastructure · FR: (infrastructure) · Journey: (none — enabler) · Depends: Aspire-Infra.1 · Product: unlocks Aspire-Infra.3+

---

## Story

As a **developer**,  
I want **health checks integrated with Aspire Dashboard so I can monitor service status and launch from VS with a single click**,  
so that **I get immediate visibility into stack health and can iterate faster without manual service checks**.

---

## Reasoning (the whys)

### Why this story now

Stories Aspire-Infra.0 and .1 built the scaffolding and service discovery. Services now talk to each other, but there's no visibility. A developer starting the app doesn't know if each service is healthy or what port it's on. Aspire Dashboard is the canonical tool for this; wiring health checks into the Dashboard gives real-time feedback.

This story is **developer experience critical** — F5 must be the only command needed to launch the full stack.

### Why this approach

**Health checks** are already partly implemented (story 1.0 added `/health` endpoints). This story:
1. Extends health checks to include database readiness (EF Core can connect + migrations run)
2. Wires health check results into Aspire service definitions so Dashboard shows "Healthy" / "Degraded"
3. Configures AppHost as the startup project so F5 launches Aspire, which orchestrates everything
4. Optionally adds database migration as a startup task (or deferred to a separate database-init story)

**Alternatives considered:**

| Alternative | Why not now |
|-------------|-------------|
| Skip health checks, rely on logs | Aspire Dashboard becomes useless; developers must dig through console spam |
| Add health checks only to API, not DB | Incomplete; Dashboard won't show database readiness, only "looks good" if API is running |
| Manual `dotnet run` + `npm run dev` separately | Defeats the purpose of Aspire orchestration; not a single "start" point |
| Add health check to Web (React) | Web is a static asset server; health check less meaningful; API health is sufficient |

### Out of scope (this story)

- **Database migrations auto-run** — If migrations are required on startup, this is a follow-up (Aspire-Infra.3 or a separate DB-init story). For now, migrations run manually or as a documented step.
- **Custom health check policies** — AC requires "Healthy" status only; threshold tuning deferred.
- **Liveness vs. readiness probes** — Aspire Dashboard distinction; can be added later.
- **Logging configuration** — Structured logging / Application Insights integration deferred.

### Tradeoffs

- **Health checks add latency to API startup** — Aspire Dashboard waits for health check to pass before marking service "Running". Acceptable for local dev (usually <3s total).
- **Database connection string must be valid** — If PostgreSQL is not running, health checks will fail immediately. This is **correct behavior** but requires clear error messaging.

### Planning references

- [1-0-solution-scaffold.md](./1-0-solution-scaffold.md) — existing `/health` endpoints
- [architecture-v0.1.md §9](../planning-artifacts/architecture-v0.1.md) — health check design
- .NET Aspire docs: [Health checks integration](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/health-checks)

---

## Details

### API health check updates

#### ContractorPro.Api/HealthEndpoints.cs (extend existing)

Current file has basic `/health` endpoint. Extend to include database check:

```csharp
// Existing /health and /health/live endpoints...

// NEW: /health/ready endpoint (includes database)
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = WriteResponse
});
```

#### ContractorPro.Api/Program.cs (extend health check registration)

Add database readiness check:

```csharp
builder.Services
    .AddHealthChecks()
    .AddCheck("api-online", () => HealthCheckResult.Healthy("API is running"))
    .AddNpgSql(
        connectionString: connectionString,
        name: "postgres",
        tags: new[] { "ready" }
    );
```

The `AddNpgSql()` call verifies that:
- Connection string is valid
- PostgreSQL is reachable and responding
- (Optionally) runs a test query

If PostgreSQL is down, the check fails, and Aspire Dashboard marks the API service as "Degraded" or "Unhealthy".

### AppHost orchestration updates

#### Program.cs (add health check mapping)

```csharp
var api = builder.AddProject<Projects.ContractorPro_Api>("api")
    .WithReference(postgres)
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health/ready"); // ← NEW: Map readiness probe
```

Aspire will poll `/health/ready` and update the Dashboard accordingly.

#### PostgreSQL service health

```csharp
var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin()
    .AddDatabase("contractorpro_db")
    .WithHealthCheck(); // ← NEW: Enable Postgres health check
```

This polls the PostgreSQL container for responsiveness.

### Launch configuration

#### Set AppHost as startup project in VS

Right-click `ContractorPro.AppHost` → **Set as Startup Project**.

Now F5 launches AppHost, which orchestrates:
1. PostgreSQL (container)
2. API (.NET service)
3. Web (npm dev server)
4. Aspire Dashboard (automatic)

#### Debug output expectations

When launching, console should show:

```
Aspire.Hosting.DistributedApplication[0]
  Services running at:
  - postgres: "postgres://localhost:5432"
  - api: "http://localhost:5000"
  - web: "http://localhost:5173"
  - dashboard: "http://localhost:15000"

Aspire.Hosting[0]
  Aspire Dashboard running at "http://localhost:15000"
```

### launchSettings.json (optional enhancement)

Update `.launchSettings.json` in AppHost project to auto-open Dashboard:

```json
{
  "profiles": {
    "AppHost": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "launchUrl": "http://localhost:15000",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

### Files to create or modify

| Path | NEW/UPDATE | Purpose |
|------|------------|---------|
| `src/ContractorPro.Api/HealthEndpoints.cs` | UPDATE | Add `/health/ready` with database check |
| `src/ContractorPro.Api/Program.cs` | UPDATE | Register `AddNpgSql()` health check |
| `src/ContractorPro.AppHost/Program.cs` | UPDATE | Add `.WithHttpHealthCheck()` to API; `.WithHealthCheck()` to Postgres |
| `src/ContractorPro.AppHost/.launchSettings.json` | UPDATE | Set as startup project; auto-open Dashboard (optional) |

### Tasks / subtasks

- [ ] Extend API health checks (AC: 1)
  - [ ] Add `/health/ready` endpoint that includes database check
  - [ ] Register `AddNpgSql()` to Program.cs health checks
  - [ ] Verify API compiles
  - [ ] Test manually: curl `http://localhost:5000/health/ready` (should 200 if DB connected)
- [ ] Update AppHost with health check mapping (AC: 2)
  - [ ] Add `.WithHttpHealthCheck("/health/ready")` to API service
  - [ ] Add `.WithHealthCheck()` to PostgreSQL service
  - [ ] Verify AppHost compiles
- [ ] Set AppHost as startup project (AC: 3)
  - [ ] Right-click AppHost project → Set as Startup Project
  - [ ] Verify Debug → Start Debugging launches AppHost
- [ ] Configure auto-launch of Dashboard (AC: 4)
  - [ ] Update launchSettings.json to open `http://localhost:15000` on F5
  - [ ] Verify Dashboard opens automatically when AppHost starts
- [ ] Test full launch experience (AC: 5)
  - [ ] Press F5 with AppHost as startup project
  - [ ] All services reach "Running" status in Dashboard within 30s
  - [ ] Health endpoints return expected status
  - [ ] Stop (Shift+F5) cleanly shuts down all services

---

## Acceptance criteria

1. **AC-1:** API project has `/health/ready` endpoint that includes PostgreSQL connection check via `AddNpgSql()`. Endpoint returns 200 when database is reachable, 503 if database is down.

2. **AC-2:** AppHost service definitions include `.WithHttpHealthCheck("/health/ready")` on API and `.WithHealthCheck()` on PostgreSQL. AppHost compiles without errors.

3. **AC-3:** ContractorPro.AppHost is set as the startup project. In VS, Debug → Start Debugging (F5) launches the AppHost process.

4. **AC-4:** Aspire Dashboard opens automatically (or is configured to do so via launchSettings.json). Dashboard is accessible at `http://localhost:15000` without manual navigation.

5. **AC-5:** After F5 launch:
   - Postgres resource shows "Running" within 10 seconds
   - API resource shows "Running" within 20 seconds and reports health via Dashboard
   - Web resource shows "Running" within 20 seconds
   - All three resources can be toggled on/off in Dashboard without crash
   - Pressing Shift+F5 (stop) cleanly terminates all services (no zombie processes)

---

## Security & vulnerability review

| Check | Notes |
|-------|-------|
| **Authentication** | AppHost is localhost-only; Dashboard does not require authentication in dev (acceptable). |
| **Health check endpoint access** | `/health/ready` is read-only and does not expose sensitive data beyond "connected/disconnected". Response does not include error details (e.g., no credentials in error messages). |
| **Database credentials** | Aspire PostgreSQL container uses default password; acceptable for local dev. Must use Azure Flexible Server + Entra auth in production. |
| **Port exposure** | Aspire binds to `127.0.0.1` only (not `0.0.0.0`); services are not accessible from other machines on the network. |
| **Dependency risk** | Health check uses Npgsql; ensure package is up-to-date and monitored for CVEs. |
| **Logging** | Health check failures logged to console; no sensitive data leak (no passwords in logs). |

**Findings:** None critical. Verify CORS policy still rejects `*` on API and enforces `localhost` for development.

---

## Unit tests

N/A — Health check integration is tested via manual launch (AC-5).

For automated health check tests, see Aspire-Infra.3 (integration test harness).

---

## Manual verification checklist

- [ ] `dotnet build ContractorPro.sln` succeeds
- [ ] F5 with AppHost as startup project launches and opens Dashboard at `http://localhost:15000`
- [ ] Dashboard shows 3–4 resources: `postgres`, `api`, `web`, (optional `pgadmin`)
- [ ] All resources reach "Running" status within 30 seconds
- [ ] Click API resource detail → verify `/health/ready` shows "Healthy"
- [ ] Stop AppHost (Shift+F5) cleanly terminates without errors
- [ ] Re-launch F5 → services start again without warnings
- [ ] (Stress test) Manually kill PostgreSQL container in Docker Desktop → Dashboard shows API as "Degraded" within 10 seconds
- [ ] (Stress test) Restart PostgreSQL container → API recovers to "Running" within 20 seconds

---
