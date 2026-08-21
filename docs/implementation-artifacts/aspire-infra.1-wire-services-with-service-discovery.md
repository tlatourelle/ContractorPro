---
status: backlog
---

# Story Aspire-Infra.1: Wire services with service discovery

Status: backlog

Epic: Aspire Infrastructure · FR: (infrastructure) · Journey: (none — enabler) · Depends: Aspire-Infra.0 · Product: unlocks Aspire-Infra.2+

---

## Story

As a **developer**,  
I want **API and Web services to auto-discover each other via Aspire service discovery**,  
so that **I can remove hardcoded `localhost` URLs and run full-stack orchestration with inter-service communication working**.

---

## Reasoning (the whys)

### Why this story now

Story Aspire-Infra.0 created the skeleton; services exist but are isolated. Without service discovery, Web (React) can't call API, and API can't reach the database reliably. Aspire's built-in service discovery (via `IServiceCollection.AddServiceDiscovery()` and connection string templating) is the foundation for a working local dev experience.

This story is **critical-path** for F5 to work end-to-end.

### Why this approach

**Aspire service discovery** uses environment variable injection to wire connection strings and HTTP endpoints. On startup, Aspire populates:
- `ConnectionStrings__postgres` → PostgreSQL connection string
- `Services__api__http__0` → API endpoint (e.g., `http://localhost:5000`)

This pattern requires:
1. Adding `Aspire.ServiceDiscovery` to the API csproj
2. Updating `appsettings.json` to use `{postgres}` templating for the database connection string
3. Configuring the Web app (React) to call the API via the discoverable endpoint
4. No code changes to core API logic — only configuration injection

**Alternatives considered:**

| Alternative | Why not now |
|-------------|-------------|
| Hardcoded localhost URLs in React/API config | Breaks on port changes, not repeatable, defeats Aspire value prop |
| Manual environment variable setup in launchSettings.json | Fragile, doesn't scale; Aspire already manages this |
| Service-to-service HTTPS with certs | Out of scope for local dev; can add in deployment story |

### Out of scope (this story)

- **Health checks** — Story Aspire-Infra.2
- **API response caching or optimization** — Handled in feature stories
- **PostgreSQL connection pooling tuning** — Handled in infrastructure hardening after MVP
- **Secrets management** — Still uses local User Secrets; Key Vault integration in deployment phase

### Tradeoffs

- **Configuration complexity:** Must update both Api/appsettings and Web (React) .env to use the service discovery pattern. Web config is in `.env.local` (Vite convention), which must not be committed.
- **Database migrations:** Aspire doesn't auto-run migrations; they must run as a startup task in the App Host or manually. Deferred to Aspire-Infra.2 or story-specific migration handling.

### Planning references

- [architecture-v0.1.md §9](../planning-artifacts/architecture-v0.1.md) — service discovery design
- .NET Aspire docs: [Service discovery](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/service-discovery)
- Vite environment docs: [.env and environment variables](https://vitejs.dev/guide/env-and-mode)

---

## Details

### API configuration updates

#### ContractorPro.Api.csproj

Add package reference:

```xml
<ItemGroup>
  <PackageReference Include="Aspire.ServiceDiscovery.Yarp" Version="9.0.0" />
</ItemGroup>
```

#### Program.cs (update existing `builder` setup)

Add service discovery registration:

```csharp
builder.AddServiceDiscovery();
builder.ConfigureHttpClientDefaults(http =>
{
    http.AddStandardResilienceHandler();
});
```

This goes after existing service registrations, before `var app = builder.Build()`.

#### appsettings.json (update Database connection string)

Replace hardcoded `localhost` with Aspire template:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "User ID=postgres;Password=postgres;Host={postgres};Port=5432;Database=contractorpro_db;..."
  }
}
```

The `{postgres}` token is replaced by Aspire at runtime with the actual PostgreSQL container host/port.

### Web (React) configuration updates

#### .env.local (new file — DO NOT COMMIT)

Create `.env.local` in `src/ContractorPro.Web/`:

```
VITE_API_BASE_URL=http://localhost:5000
```

Update this after verifying the actual port assigned by Aspire Dashboard.

**Note:** Add `.env.local` to `.gitignore` if not already present.

#### src/api/client.ts (new or update existing)

Create or update HTTP client factory to use the discovery endpoint:

```typescript
const apiBaseUrl = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000';

export const apiClient = new HttpClient({
  baseURL: apiBaseUrl,
  withCredentials: true, // for session cookie
});
```

All API calls use `apiClient` (e.g., `apiClient.get('/auth/me')`).

### AppHost program updates

Update Program.cs to wire Web service with API reference:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin()
    .AddDatabase("contractorpro_db");

var api = builder.AddProject<Projects.ContractorPro_Api>("api")
    .WithReference(postgres)
    .WithExternalHttpEndpoints();

var web = builder.AddNpmApp("web", "../ContractorPro.Web", "dev")
    .WithReference(api)  // Web gets API endpoint via VITE_SERVICES_API_HTTP_0
    .WithExternalHttpEndpoints();

await builder.Build().RunAsync();
```

The `.WithReference(api)` call injects the API service endpoint into the Web app's environment.

### Environment variable mappings

When Aspire launches the Web app, it injects:

```
SERVICES__API__HTTP__0=http://api:5000  (internal, or http://localhost:5001 external)
```

The npm dev server must map this to `VITE_API_BASE_URL`. Update `src/ContractorPro.Web/package.json` dev script or create a `.env.development.local` helper:

```json
{
  "scripts": {
    "dev": "vite"
  }
}
```

And in `.env.local`:

```
VITE_API_BASE_URL=${SERVICES__API__HTTP__0 || http://localhost:5000}
```

Or manually set after starting Aspire.

### Files to create or modify

| Path | NEW/UPDATE | Purpose |
|------|------------|---------|
| `src/ContractorPro.Api/ContractorPro.Api.csproj` | UPDATE | Add `Aspire.ServiceDiscovery.Yarp` package |
| `src/ContractorPro.Api/Program.cs` | UPDATE | Add `AddServiceDiscovery()` and resilience handler |
| `src/ContractorPro.Api/appsettings.json` | UPDATE | Connection string uses `{postgres}` template |
| `src/ContractorPro.Web/.env.local` | NEW | API endpoint mapping (local only) |
| `src/ContractorPro.Web/src/api/client.ts` | NEW/UPDATE | HTTP client uses environment variable |
| `src/ContractorPro.Web/.gitignore` | UPDATE | Ensure `.env.local` is ignored |
| `src/ContractorPro.AppHost/Program.cs` | UPDATE | Add `.WithReference()` calls for inter-service links |

### Tasks / subtasks

- [ ] Update API for service discovery (AC: 1)
  - [ ] Add Aspire.ServiceDiscovery.Yarp NuGet package
  - [ ] Register service discovery in Program.cs
  - [ ] Update appsettings.json connection string to use `{postgres}` template
  - [ ] Verify API compiles
- [ ] Update AppHost Program.cs (AC: 2)
  - [ ] Add `.WithReference(postgres)` to API service definition
  - [ ] Add `.WithReference(api)` to Web service definition
  - [ ] Verify AppHost compiles and loads service metadata
- [ ] Wire Web environment variables (AC: 3)
  - [ ] Create `.env.local` in Web project root
  - [ ] Set `VITE_API_BASE_URL` to localhost port or use dynamic injection
  - [ ] Add `.env.local` to `.gitignore`
- [ ] Create HTTP client for React (AC: 4)
  - [ ] Create `src/api/client.ts` with `apiClient` factory
  - [ ] Use `VITE_API_BASE_URL` from environment
  - [ ] Export for use in React components
- [ ] Test inter-service connectivity (AC: 5)
  - [ ] Launch AppHost (F5 or `dotnet run --project src/ContractorPro.AppHost`)
  - [ ] Verify API is reachable from Web: `GET /health` returns 200
  - [ ] Verify database connection: API logs show successful Postgres connection

---

## Acceptance criteria

1. **AC-1:** API project includes `Aspire.ServiceDiscovery.Yarp` NuGet package and calls `AddServiceDiscovery()` in Program.cs. Compilation succeeds with no warnings.

2. **AC-2:** AppHost Program.cs defines `.WithReference(postgres)` on the API service and `.WithReference(api)` on the Web service. AppHost compiles and loads without errors.

3. **AC-3:** Web project has `.env.local` (not committed) with `VITE_API_BASE_URL` set to the API endpoint (e.g., `http://localhost:5000`). React dev server reads this value on startup.

4. **AC-4:** `src/api/client.ts` exports an `apiClient` that uses `VITE_API_BASE_URL` as baseURL. All subsequent React components import and use this client (verified by at least one sample API call, e.g., in a dev fixture or test).

5. **AC-5:** Running `dotnet run --project src/ContractorPro.AppHost` launches API, Web, and PostgreSQL. Web app loads successfully in browser and can reach API via the client (e.g., `GET /health` returns 200; visible in browser dev tools Network tab or Aspire Dashboard).

---

## Security & vulnerability review

| Check | Notes |
|-------|-------|
| **Authentication** | Aspire service discovery is unencrypted on localhost (OK for dev). Session cookie transport to API is still over HTTP locally; will be HTTPS in ACA deployment. |
| **Authorization** | No authorization changes in this story — API remains unprotected at the HTTP level (auth middleware handles this). |
| **Secrets** | Database password still in appsettings.json (local dev only). No secrets in React env file (.env.local is gitignored). PostgreSQL password from Docker / Aspire container defaults (acceptable for dev). |
| **Input validation** | No new user input vectors. HTTP client respects CORS if API enforces it. |
| **Injection** | `.env.local` is read by Vite build; no eval() or string interpolation in connection strings (using Aspire templates). |
| **Dependency risk** | `Aspire.ServiceDiscovery.Yarp` is official Microsoft package; monitor for updates. No third-party HTTP clients exposed. |
| **CORS** | Verify API allows `http://localhost:*` in CORS policy during local dev. Update policy to deny `*` in production. |

**Findings:**
- Recommend: Add CORS middleware check to API to deny `*` on non-local environments.
- Recommend: Document that `.env.local` must not be committed; add pre-commit hook if useful.

---

## Unit tests

N/A — This is infrastructure configuration. Verification is manual (AC-5 test above).

For end-to-end smoke testing, see Aspire-Infra.3 (integration test harness).

---

## Manual verification checklist

- [ ] `dotnet build ContractorPro.sln` succeeds
- [ ] AppHost launches: `dotnet run --project src/ContractorPro.AppHost`
- [ ] Aspire Dashboard opens automatically (or navigate to `http://localhost:15000`)
- [ ] Dashboard shows three resources: `postgres`, `api`, `web`
- [ ] All three resources show "Running" status within 30 seconds
- [ ] Click API resource → verify `/health` endpoint returns 200
- [ ] Open Web in browser → React dev server loads (`http://localhost:5173` or assigned port)
- [ ] React app console shows no CORS errors when calling API
- [ ] (Optional) Open React dev tools → Network tab → one GET `/health` call to API succeeds

---
