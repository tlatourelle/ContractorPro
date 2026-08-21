---
status: backlog
---

# Story Aspire-Infra.3: Integration test harness with Aspire components

Status: backlog

Epic: Aspire Infrastructure · FR: (infrastructure, testing) · Journey: (none — enabler) · Depends: Aspire-Infra.1, Aspire-Infra.2 · Product: unlocks E1 integration tests+

---

## Story

As a **developer**,  
I want **integration tests that spin up the full Aspire stack (API, Web, Database) for test isolation**,  
so that **I can write end-to-end tests without mocking services and verify real inter-service behavior**.

---

## Reasoning (the whys)

### Why this story now

Stories Aspire-Infra.0–2 built the orchestration and dev experience. But tests are still mocking the database and HTTP clients. For E1 stories (auth flow, session handling) and beyond, we need **real integration tests** that:
- Spin up a fresh PostgreSQL instance per test run
- Run database migrations
- Call the real API endpoints
- Verify session cookies, redirects, and database state atomically

Aspire provides a test harness (`.WithWaitFor()`, `IResourcesClient`) to await service readiness and call endpoints.

This story enables **confident feature development** for E1.1+ and establishes the testing pattern for all future stories.

### Why this approach

**Aspire test harness** uses `Aspire.Hosting.Testing` + xUnit fixtures:
1. Define a test AppHost in the test project (inherits from `IAsyncLifetime`)
2. AppHost spins up services on test startup, tears down on test cleanup
3. Tests use `IResourceClient` to get service endpoints and call HTTP clients
4. Use `.WithWaitFor()` to wait for database migrations to complete

This pattern is **much cleaner** than manually managing Docker containers or mocking everything.

**Alternatives considered:**

| Alternative | Why not now |
|-------------|-------------|
| Keep mocking everything | Doesn't test real session/cookie behavior, can't verify database migrations, fragile to refactoring |
| Use docker-compose in tests | Manual cleanup, slow, tied to Docker Desktop, doesn't scale in CI/CD without Docker-in-Docker |
| Manual database fixture with SQL files | Brittle; doesn't scale as schema evolves; Aspire handles this automatically |
| Aspire.Components.Qdrant/Redis only, mock HTTP | Misses the main value: testing real HTTP + session behavior |

### Out of scope (this story)

- **Load testing or performance benchmarks** — Aspire test harness is for functional correctness, not perf.
- **CI/CD integration** — Tests run locally in this story; Azure Pipelines integration deferred.
- **WebDriver/Playwright tests** — Aspire harness is API-level; E2E UI tests in a separate test category.
- **Multi-database testing** — Tests use one PostgreSQL per run; schema variance testing deferred.
- **Test data seeding helpers** — Basic SQL script for test data; elaborate fixtures deferred.

### Tradeoffs

- **Test startup latency:** Spinning up PostgreSQL + API for each test class adds ~5–10s overhead. Trade-off: **correct vs. fast**. We choose correct; can optimize with shared fixtures later.
- **No test parallelization in v1:** Aspire test harness doesn't support parallel test runs in the same process (port conflicts). Each test class gets its own AppHost. This is **acceptable for MVP**; optimization in v0.1.1.
- **Requires Docker Desktop running:** Tests assume Docker is available. CI/CD must have Docker enabled (standard in Azure Pipelines).

### Planning references

- [architecture-v0.1.md §9](../planning-artifacts/architecture-v0.1.md) — testing strategy
- [aspire-infra.1-wire-services-with-service-discovery.md](./aspire-infra.1-wire-services-with-service-discovery.md) — service discovery
- .NET Aspire docs: [Test Aspire apps](https://learn.microsoft.com/en-us/dotnet/aspire/testing)
- xUnit: [Shared context](https://xunit.net/docs/shared-context)

---

## Details

### Test project setup

#### Create ContractorPro.Api.Integration.Tests project

```bash
dotnet new xunit -n ContractorPro.Api.Integration.Tests -o tests/ContractorPro.Api.Integration.Tests
dotnet sln add tests/ContractorPro.Api.Integration.Tests/ContractorPro.Api.Integration.Tests.csproj
```

#### Add NuGet packages

Add to `ContractorPro.Api.Integration.Tests.csproj`:

```xml
<ItemGroup>
  <PackageReference Include="Aspire.Hosting.Testing" Version="9.0.0" />
  <PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="10.0.0" />
  <PackageReference Include="xunit" Version="2.6.0" />
  <PackageReference Include="xunit.runner.visualstudio" Version="2.5.0" />
  <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
</ItemGroup>

<ItemGroup>
  <ProjectReference Include="../../src/ContractorPro.Api/ContractorPro.Api.csproj" />
  <ProjectReference Include="../../src/ContractorPro.AppHost/ContractorPro.AppHost.csproj" />
</ItemGroup>
```

### Test AppHost fixture

#### tests/ContractorPro.Api.Integration.Tests/Fixtures/AppHostFixture.cs (new)

```csharp
using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Xunit;

namespace ContractorPro.Api.Integration.Tests.Fixtures;

/// <summary>
/// Aspire AppHost fixture for integration tests.
/// Spins up PostgreSQL + API on test startup, tears down on test cleanup.
/// </summary>
public class AppHostFixture : IAsyncLifetime
{
    private DistributedApplication? _app;

    public string ApiBaseUrl { get; private set; } = string.Empty;
    public string PostgresConnectionString { get; private set; } = string.Empty;
    public IResourcesClient ResourcesClient { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        // Build AppHost (same as production AppHost, but without Web for tests)
        var builder = DistributedApplication.CreateBuilder();

        var postgres = builder.AddPostgres("postgres")
            .AddDatabase("contractorpro_db")
            .WithWaitFor(pg => pg.WaitForHealthy());

        var api = builder.AddProject<Projects.ContractorPro_Api>("api")
            .WithReference(postgres)
            .WithHttpHealthCheck("/health/ready")
            .WaitFor(postgres);

        _app = builder.Build();
        await _app.StartAsync();

        // Get resource endpoints
        ResourcesClient = _app.Services.GetRequiredService<IResourcesClient>();

        // Retrieve API endpoint
        var apiResource = await ResourcesClient.GetResourceAsync("api");
        ApiBaseUrl = apiResource.Endpoints
            .FirstOrDefault(e => e.Scheme == "http")?.Url 
            ?? throw new InvalidOperationException("API endpoint not found");

        // Retrieve PostgreSQL connection string
        var postgresResource = await ResourcesClient.GetResourceAsync("postgres");
        PostgresConnectionString = postgresResource.ConnectionString 
            ?? throw new InvalidOperationException("PostgreSQL connection string not found");

        // Wait for API to be healthy
        await WaitForHealthy(ApiBaseUrl, TimeSpan.FromSeconds(30));
    }

    public async Task DisposeAsync()
    {
        if (_app is not null)
        {
            await _app.StopAsync();
            await _app.DisposeAsync();
        }
    }

    private static async Task WaitForHealthy(string baseUrl, TimeSpan timeout)
    {
        using var httpClient = new HttpClient();
        var healthUrl = $"{baseUrl}/health/ready";
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        while (stopwatch.Elapsed < timeout)
        {
            try
            {
                var response = await httpClient.GetAsync(healthUrl);
                if (response.IsSuccessStatusCode)
                {
                    return;
                }
            }
            catch (HttpRequestException)
            {
                // Service not yet ready
            }

            await Task.Delay(500);
        }

        throw new TimeoutException($"Service at {baseUrl} did not become healthy within {timeout.TotalSeconds}s");
    }
}
```

### Collection fixture (xUnit convention)

#### tests/ContractorPro.Api.Integration.Tests/ApiIntegrationTestCollection.cs (new)

```csharp
using Xunit;
using ContractorPro.Api.Integration.Tests.Fixtures;

namespace ContractorPro.Api.Integration.Tests;

[CollectionDefinition("API Integration Tests")]
public class ApiIntegrationTestCollection : ICollectionFixture<AppHostFixture>
{
    // This class has no code, and never creates an instance of itself.
    // Its purpose is simply to define the traits on this collection.
}
```

### Sample test

#### tests/ContractorPro.Api.Integration.Tests/HealthEndpointTests.cs (new)

```csharp
using Xunit;
using ContractorPro.Api.Integration.Tests.Fixtures;

namespace ContractorPro.Api.Integration.Tests;

[Collection("API Integration Tests")]
public class HealthEndpointTests
{
    private readonly AppHostFixture _fixture;

    public HealthEndpointTests(AppHostFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task HealthEndpoint_ReturnsHealthy()
    {
        // Arrange
        using var httpClient = new HttpClient();

        // Act
        var response = await httpClient.GetAsync($"{_fixture.ApiBaseUrl}/health");

        // Assert
        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task HealthReadyEndpoint_ReturnsHealthy_WhenDatabaseConnected()
    {
        // Arrange
        using var httpClient = new HttpClient();

        // Act
        var response = await httpClient.GetAsync($"{_fixture.ApiBaseUrl}/health/ready");

        // Assert
        Assert.True(response.IsSuccessStatusCode);
    }
}
```

### Files to create or modify

| Path | NEW/UPDATE | Purpose |
|------|------------|---------|
| `tests/ContractorPro.Api.Integration.Tests/` | NEW | Integration test project |
| `tests/ContractorPro.Api.Integration.Tests/ContractorPro.Api.Integration.Tests.csproj` | NEW | Test project file |
| `tests/ContractorPro.Api.Integration.Tests/Fixtures/AppHostFixture.cs` | NEW | Aspire fixture for spinning up AppHost |
| `tests/ContractorPro.Api.Integration.Tests/ApiIntegrationTestCollection.cs` | NEW | xUnit collection fixture |
| `tests/ContractorPro.Api.Integration.Tests/HealthEndpointTests.cs` | NEW | Sample integration tests |
| `ContractorPro.sln` | UPDATE | Add integration test project |

### Tasks / subtasks

- [ ] Create integration test project (AC: 1)
  - [ ] `dotnet new xunit` for the test project
  - [ ] Add to solution
  - [ ] Add NuGet packages: Aspire.Hosting.Testing, Microsoft.AspNetCore.Mvc.Testing
- [ ] Implement AppHostFixture (AC: 2)
  - [ ] Create `Fixtures/AppHostFixture.cs` with test AppHost builder
  - [ ] Implement `IAsyncLifetime` for startup/teardown
  - [ ] Expose `ApiBaseUrl` and `ResourcesClient` for tests
  - [ ] Add `WaitForHealthy()` helper to poll health endpoint
- [ ] Create xUnit collection fixture (AC: 3)
  - [ ] Create `ApiIntegrationTestCollection.cs` with `[CollectionDefinition]`
  - [ ] Ensure all tests decorate with `[Collection("API Integration Tests")]`
- [ ] Write sample tests (AC: 4)
  - [ ] Create `HealthEndpointTests.cs` with at least 2 health check tests
  - [ ] Verify tests pass: `dotnet test tests/ContractorPro.Api.Integration.Tests`
- [ ] Document test patterns (AC: 5)
  - [ ] Add README or comments in Fixtures/ on how to write new integration tests
  - [ ] Include example of making authenticated API calls (for E1.1 follow-up)

---

## Acceptance criteria

1. **AC-1:** Integration test project compiles without errors. Project references AppHost and includes all required NuGet packages.

2. **AC-2:** AppHostFixture class implements `IAsyncLifetime` and successfully spins up PostgreSQL + API on `InitializeAsync()`. Both `ApiBaseUrl` and `ResourcesClient` are available to test methods. Cleanup completes without errors in `DisposeAsync()`.

3. **AC-3:** xUnit collection fixture is defined and all tests inherit from it via `[Collection]` attribute. Fixture is instantiated once per test class (not per test method).

4. **AC-4:** HealthEndpointTests runs successfully:
   - `HealthEndpoint_ReturnsHealthy()` verifies `GET /health` returns 200
   - `HealthReadyEndpoint_ReturnsHealthy_WhenDatabaseConnected()` verifies `GET /health/ready` returns 200 when database is connected
   - Running `dotnet test` shows all tests pass (2 passed, 0 failed)

5. **AC-5:** Integration test README or inline comments explain:
   - How to add a new integration test class
   - How to make authenticated API calls (with session cookie example)
   - How to query the database directly (via connection string)
   - How to wait for async operations (example: polling a resource)

---

## Security & vulnerability review

| Check | Notes |
|-------|-------|
| **Test isolation** | Each test class gets its own AppHost instance → fresh PostgreSQL container. No data leakage between tests. |
| **Secrets in tests** | Connection strings are retrieved from Aspire at runtime; no hardcoded credentials. Test database is ephemeral (destroyed on test cleanup). |
| **Authentication** | Tests can include session cookies, Bearer tokens, or other auth headers. When testing auth flows, verify tokens are correctly validated or rejected. |
| **Dependency risk** | Aspire.Hosting.Testing is official Microsoft package; monitor for CVEs. |
| **CI/CD risk** | Tests require Docker Desktop running in CI/CD; ensure container images are scanned for vulnerabilities. |
| **Data cleanup** | PostgreSQL container is destroyed at test cleanup; no orphaned data. Verify no stray files are left behind. |

**Findings:**
- Recommend: Add a `DatabaseTestHelper` class for seeding test data safely (e.g., contractor, team member, project fixtures). Follow-up story if needed.
- Recommend: Document that tests must not hardcode auth tokens; use login flow or mock identity provider for test auth.

---

## Unit tests

N/A — The integration test harness IS the test infrastructure. Samples (HealthEndpointTests) verify basic functionality.

For API-level unit tests (services, business logic), see existing `ContractorPro.Api.Tests` (unchanged).

---

## Manual verification checklist

- [ ] `dotnet sln list` shows ContractorPro.Api.Integration.Tests
- [ ] `dotnet build tests/ContractorPro.Api.Integration.Tests/` succeeds
- [ ] `dotnet test tests/ContractorPro.Api.Integration.Tests/` runs and shows 2 tests passing
- [ ] Test output includes startup logs showing Aspire spinning up Postgres and API
- [ ] Test completes within 30 seconds (including AppHost startup)
- [ ] Stop test execution (Ctrl+C); verify no Docker containers are left running: `docker ps | grep contractorpro` (should be empty)
- [ ] Re-run tests; verify fresh PostgreSQL container is created each time
- [ ] (Optional) Add a test that writes to the database, then verify rows are deleted on cleanup

---

## Integration with E1.1+ stories

This harness enables **E1.1 (Google OAuth)** and future stories to write tests like:

```csharp
[Collection("API Integration Tests")]
public class AuthFlowTests
{
    [Fact]
    public async Task GoogleOAuthCallback_CreatesContractorAndTeamMember_OnFirstLogin()
    {
        // Arrange
        var httpClient = new HttpClient();
        // Mock Google OAuth response or use test identity provider
        
        // Act
        var response = await httpClient.GetAsync($"{_fixture.ApiBaseUrl}/auth/callback?code=...");
        
        // Assert
        Assert.True(response.IsSuccessStatusCode);
        
        // Verify database state
        using var connection = new NpgsqlConnection(_fixture.PostgresConnectionString);
        await connection.OpenAsync();
        // ... verify contractors and team_members rows were created
    }
}
```

No additional test infrastructure needed; just add tests to the existing fixture.

---
