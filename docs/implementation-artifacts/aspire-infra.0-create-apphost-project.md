---
status: backlog
---

# Story Aspire-Infra.0: Create AppHost project

Status: backlog

Epic: Aspire Infrastructure · FR: (infrastructure) · Journey: (none — enabler) · Depends: 1.0 · Product: unlocks Aspire-Infra.1+

---

## Story

As a **developer**,  
I want a **ContractorPro.AppHost project** configured for .NET Aspire local orchestration,  
so that **I can launch the full stack (API, Web, Database) with a single F5 from VS**.

---

## Reasoning (the whys)

### Why this story now

Story 1.0 established the monolith structure. Now that API and Web compile, we can add orchestration without refactoring existing code. AppHost acts as the "entry point" for local dev, replacing manual docker-compose + separate dotnet watch commands.

**Windows-only dev** (per constraints) aligns perfectly with Aspire host requirements. Aspire also generates Azure Container Apps manifests, de-risking deployment readiness.

### Why this approach

**Aspire host project** is the canonical pattern: a lightweight console app that defines services (API, Web, Database) declaratively. No code changes to existing projects — AppHost references them as resource definitions.

**Alternatives considered:**

| Alternative | Why not now |
|-------------|-------------|
| Keep docker-compose only | Loses F5 integration, no ACA manifest generation, slower feedback loop |
| Use Aspire from a test project | AppHost must be runnable standalone as the "entry point" |
| Launchpad via launchSettings.json | Limited orchestration; can't express inter-service dependencies cleanly |

### Out of scope (this story)

- **Health checks** — added in Aspire-Infra.2
- **Dashboard configuration** — added in Aspire-Infra.2
- **Integration tests** — added in Aspire-Infra.3
- **Deployment manifest generation** — documented in Aspire-Infra.3
- **Docker container build optimization** — handled in deployment phase

### Tradeoffs

- **AppHost adds one new project** to the solution, but it's lightweight (~200 LOC target) and Windows-only, so no extra CI burden.
- **Local-only for now:** AppHost doesn't run in CI/CD (docker-compose remains the fallback); can be integrated later if needed.

### Planning references

- [architecture-v0.1.md §9](../planning-artifacts/architecture-v0.1.md) — orchestration strategy
- [1-0-solution-scaffold.md](./1-0-solution-scaffold.md) — baseline solution layout
- .NET Aspire docs: [Create a new Aspire app host project](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/setup-tooling)

---

## Details

### Project structure

Create `ContractorPro.AppHost` in `/src`:

```
src/
  ContractorPro.AppHost/
    ContractorPro.AppHost.csproj
    Program.cs                 ← Main entry point; defines services
    appsettings.json          ← AppHost-specific settings (e.g., port mappings)
```

### ContractorPro.AppHost.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <InvariantGlobalization>false</InvariantGlobalization>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Aspire.Hosting" Version="9.0.0" />
    <PackageReference Include="Aspire.Hosting.PostgreSQL" Version="9.0.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="./ContractorPro.Api/ContractorPro.Api.csproj" />
    <ProjectReference Include="./ContractorPro.Web/ContractorPro.Web.csproj" />
  </ItemGroup>

</Project>
```

### Program.cs (initial structure)

```csharp
using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// PostgreSQL database
var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin()
    .AddDatabase("contractorpro_db");

// API service
var api = builder.AddProject<Projects.ContractorPro_Api>("api")
    .WithReference(postgres)
    .WithExternalHttpEndpoints();

// Web (React) service
var web = builder.AddNpmApp("web", "../ContractorPro.Web", "dev")
    .WithReference(api)
    .WithExternalHttpEndpoints();

await builder.Build().RunAsync();
```

### appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "default": "Information",
      "Microsoft.Aspire": "Information"
    }
  }
}
```

### Add AppHost to .sln

```bash
dotnet sln ContractorPro.sln add src/ContractorPro.AppHost/ContractorPro.AppHost.csproj
```

### Files to create or modify

| Path | NEW/UPDATE | Purpose |
|------|------------|---------|
| `src/ContractorPro.AppHost/` | NEW | AppHost project root |
| `src/ContractorPro.AppHost/ContractorPro.AppHost.csproj` | NEW | AppHost project file |
| `src/ContractorPro.AppHost/Program.cs` | NEW | Service definitions (skeleton) |
| `src/ContractorPro.AppHost/appsettings.json` | NEW | AppHost logging config |
| `ContractorPro.sln` | UPDATE | Add AppHost project reference |
| `.gitignore` | UPDATE | Ignore AppHost build artifacts |

### Tasks / subtasks

- [ ] Create AppHost project scaffolding (AC: 1)
  - [ ] Run `dotnet new aspire-apphost -n ContractorPro.AppHost` or manual project creation
  - [ ] Add Aspire.Hosting NuGet packages (PostgreSQL, base Aspire)
  - [ ] Add project references to Api and Web
- [ ] Write Program.cs skeleton (AC: 2)
  - [ ] Define PostgreSQL service
  - [ ] Add API service reference
  - [ ] Add Web service reference (npm app)
  - [ ] Verify Projects.* auto-scaffolding for project references
- [ ] Add to solution file (AC: 3)
  - [ ] `dotnet sln add` command
  - [ ] Verify SLN parses and solution loads in VS
- [ ] Verify baseline compilation (AC: 4)
  - [ ] `dotnet build` succeeds with no errors
  - [ ] Solution loads in VS 2025 without warnings about missing projects

---

## Acceptance criteria

1. **AC-1:** AppHost project compiles without errors. Project file includes `Aspire.Hosting` and `Aspire.Hosting.PostgreSQL` NuGet packages (9.0.0 or later).

2. **AC-2:** Program.cs defines three services:
   - PostgreSQL database (named "postgres" with "contractorpro_db" database)
   - API project reference with PostgreSQL connection
   - Web (React npm app) with API reference

3. **AC-3:** AppHost project is added to ContractorPro.sln and loads successfully in VS 2025. Running `dotnet sln list` shows ContractorPro.AppHost.

4. **AC-4:** Solution builds cleanly: `dotnet build ContractorPro.sln` succeeds with no errors (warnings about implicit usings or node_modules OK).

---

## Security & vulnerability review

| Check | Notes |
|-------|-------|
| **Authentication** | N/A — AppHost is development-only tooling, no prod deployment. |
| **Authorization** | N/A |
| **Secrets** | AppHost uses User Secrets for API connection strings; no secrets in code or appsettings.json. PostgreSQL password handled by Docker/Aspire, not hardcoded. |
| **Input validation** | N/A — AppHost processes no user input. |
| **Dependencies** | Aspire.Hosting packages are from official Microsoft NuGet feed. Monitor for CVEs in minor updates. |
| **Local-only** | AppHost does not expose to public network (Aspire host runs on localhost only). |

**Findings:** None — AppHost is scaffolding-only in this story. No secrets to harden.

---

## Unit tests (N/A for scaffolding)

N/A — This is scaffolding / integration setup. Verification via manual build and compilation.

---

## Manual verification checklist

- [ ] Open ContractorPro.sln in VS 2025; Solution Explorer shows ContractorPro.AppHost
- [ ] Right-click AppHost project → Set as Startup Project
- [ ] Verify Debug → Run configuration can select AppHost
- [ ] `dotnet run --project src/ContractorPro.AppHost` does not crash on startup (API/Web may not be fully wired yet — expected to fail at service startup)

---
