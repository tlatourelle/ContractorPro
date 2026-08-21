# ContractorPro Development Guide

## Prerequisites

- **.NET 10 SDK** — [Download](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- **PostgreSQL 16** — [Download](https://www.postgresql.org/download/) or use Docker Compose
- **Node.js 20+** — [Download](https://nodejs.org/)
- **Docker & Docker Compose** (optional, for local Postgres)

## Quick Start

### 1. Start PostgreSQL

**Using Docker Compose (recommended):**

```bash
docker compose up -d
```

**Using local PostgreSQL:**

Ensure Postgres is running and update the connection string in `src/ContractorPro.Api/appsettings.json` if needed.

### 2. Run Database Migrations

From the repository root:

```bash
cd src/ContractorPro.Infrastructure
dotnet ef database update
cd ../..
```

Or from the API project:

```bash
cd src/ContractorPro.Api
dotnet ef database update -s . -p ../ContractorPro.Infrastructure
cd ../..
```

### 3. Start the API

```bash
dotnet run --project src/ContractorPro.Api
```

The API will run on `http://localhost:5000` (or configured port).

**Health Check:**

```bash
curl http://localhost:5000/api/v1/health
```

Expected response:

```json
{
  "status": "healthy",
  "database": "healthy"
}
```

### 4. Start the React Frontend

In a new terminal, from the repository root:

```bash
cd src/ContractorPro.Web
npm install
npm run dev
```

The frontend will run on `http://localhost:5173`.

### 5. Access the Application

- **App (team member):** http://localhost:5173/app
- **Portal (subcontractor/customer):** http://localhost:5173/p

## Development Workflow

### Build the entire solution

```bash
dotnet build
```

### Run all tests

```bash
dotnet test
```

### Run only API tests

```bash
dotnet test tests/ContractorPro.Api.Tests
```

### Run only Application tests

```bash
dotnet test tests/ContractorPro.Application.Tests
```

### Frontend lint

```bash
cd src/ContractorPro.Web
npm run lint
```

### Frontend build

```bash
cd src/ContractorPro.Web
npm run build
```

## Configuration

### API Configuration

**Connection String:**

- Default: `appsettings.json` has a local Postgres string
- Override via environment variable: `ConnectionStrings__Default`
- Use User Secrets for development (never commit secrets):

  ```bash
  dotnet user-secrets init --project src/ContractorPro.Api
  dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;..." --project src/ContractorPro.Api
  ```

**Logging:**

- Defaults to console in development
- Configure in `appsettings.Development.json`

### Entra External ID + Google Auth Setup (Story 1.1)

Real Google sign-in requires Entra External ID configuration. Without this setup, login endpoints return auth configuration errors.

1. Create or use an Entra External ID tenant.
2. Configure Google as an identity provider in Entra.
3. Register the ContractorPro API as a confidential client app.
4. Add local redirect URI for OIDC callback:
  - `https://localhost:5000/signin-oidc` (or your local API origin + callback path)
5. Create a client secret and store it in user-secrets.

Set local secrets:

```bash
dotnet user-secrets set "Authentication:ExternalId:Enabled" "true" --project src/ContractorPro.Api
dotnet user-secrets set "Authentication:ExternalId:Authority" "https://<tenant>.ciamlogin.com/<tenant-id>/v2.0" --project src/ContractorPro.Api
dotnet user-secrets set "Authentication:ExternalId:ClientId" "<client-id>" --project src/ContractorPro.Api
dotnet user-secrets set "Authentication:ExternalId:ClientSecret" "<client-secret>" --project src/ContractorPro.Api
dotnet user-secrets set "Authentication:ExternalId:CallbackPath" "/signin-oidc" --project src/ContractorPro.Api
```

Validation checklist:

- Start API and Web.
- Open `/app/login`.
- Click Sign in with Google.
- Confirm browser redirects to Entra, then Google.
- Complete login and confirm redirect to `/app/dashboard`.

### Frontend Configuration

**API Proxy:**

- Dev server proxies `/api` requests to `http://localhost:5000`
- Configure in `src/ContractorPro.Web/vite.config.ts`

**CORS:**

- API allows `http://localhost:5173` and `http://localhost:3000` in development
- Production CORS locked to specific origins (set via environment)

## Database Migrations

### Create a new migration

```bash
cd src/ContractorPro.Infrastructure
dotnet ef migrations add <MigrationName>
```

### Apply migrations

```bash
cd src/ContractorPro.Api
dotnet ef database update
```

### Remove last migration (dev only)

```bash
cd src/ContractorPro.Infrastructure
dotnet ef migrations remove
```

## Testing

### Test Philosophy

- **Red, Green, Refactor:** Write failing test first, implement feature, refactor for clean code.
- **Integration tests over unit tests** for API endpoints (WebApplicationFactory).
- **Unit tests** for Application/Domain business logic.

### Running Tests in CI

```bash
# All tests
dotnet test

# With coverage (requires coverlet)
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover
```

GitHub Actions CI runs tests on every push to `main` and `develop`.

## Security Notes

### Secrets Management

- **Never commit** `appsettings.*.local.json`, `.env`, or User Secrets.
- `.gitignore` covers these — verify with `git status`.
- Use Azure Key Vault in production.

### Default Database Password

- Docker Compose uses `postgres` / `postgres` — **dev only**.
- **Never use in production.**
- Local instance should use strong password.

### CORS Configuration

- Development allows `localhost:5173` and `3000`.
- Production CORS restricted to specific domains (set via environment).

### Connection String

- No hardcoded passwords in source code.
- Use environment variables or User Secrets.

## Stopping Services

### Stop PostgreSQL (Docker)

```bash
docker compose down
```

To keep data: `docker compose stop`

### Stop API and Frontend

- Press `Ctrl+C` in each terminal.

## Troubleshooting

### "Connection refused" to Postgres

1. Verify `docker compose ps` shows postgres running.
2. Check connection string in `appsettings.json`.
3. Test manually: `psql -h localhost -U postgres -d contractorpro`

### Health endpoint returns "database: unhealthy"

1. Verify Postgres is running.
2. Check the connection string and credentials.
3. Review API logs in the terminal.

### React app won't connect to API

1. Ensure API is running on `http://localhost:5000`.
2. Check browser console for CORS errors.
3. Verify `vite.config.ts` proxy is correctly configured.

### Migrations fail

1. Ensure `ContractorPro.Infrastructure` is the startup project.
2. Verify Postgres is accessible.
3. Check that `.csproj` files reference Entity Framework tools.

## Next Steps

After successfully running Story 1.0:

- **Story 1.1:** Add Google OAuth, BFF session cookie, contractor auto-provision.
- **Story 1.2:** Add company profile and session context.
- **Story 1.3:** Add onboarding checklist.

See [implementation-epics.md](../../docs/implementation-artifacts/implementation-epics.md) for the full roadmap.
