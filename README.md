# ContractorPro

Personal project for contractor business management.

## Status

**Discovery & planning** — product vision and research in progress. No application code yet.

## Planning docs

Start here: [docs/planning-artifacts/README.md](docs/planning-artifacts/README.md)

- [Product vision](docs/planning-artifacts/product-vision.md)
- [Discovery log](docs/planning-artifacts/discovery-log.md) — ideas, questions, decisions
- [Competitor research](docs/planning-artifacts/competitor-research.md)
- [Customer discovery questions](docs/planning-artifacts/customer-discovery.md)

BMAD Method is installed (`_bmad/`, `.agents/skills/`) for structured planning when ready.

## Local helper scripts

Use these PowerShell scripts from the repo root to simplify local validation.

### Setup and test everything

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\setup-and-test.ps1
```

What it does:

- Starts Postgres with Docker Compose
- Restores and builds the .NET solution
- Runs .NET tests
- Installs frontend dependencies
- Runs frontend lint and production build

Options:

```powershell
# Skip Docker startup
powershell -ExecutionPolicy Bypass -File .\scripts\setup-and-test.ps1 -SkipDocker

# Skip frontend lint/build
powershell -ExecutionPolicy Bypass -File .\scripts\setup-and-test.ps1 -SkipFrontend
```

### Setup and run the app

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\run-local.ps1
```

What it does:

- Starts Docker Desktop daemon when needed, then starts Postgres with Docker Compose
- Applies EF migrations
- Installs frontend dependencies
- Opens two new PowerShell windows:
	- API: `dotnet run --project src/ContractorPro.Api`
	- Web: `npm run dev` in `src/ContractorPro.Web`

Options:

```powershell
# Skip Docker startup
powershell -ExecutionPolicy Bypass -File .\scripts\run-local.ps1 -SkipDocker

# Skip migrations
powershell -ExecutionPolicy Bypass -File .\scripts\run-local.ps1 -SkipMigrations

# Skip npm install
powershell -ExecutionPolicy Bypass -File .\scripts\run-local.ps1 -SkipNpmInstall

# Increase Docker daemon startup wait time (first run can be slow)
powershell -ExecutionPolicy Bypass -File .\scripts\run-local.ps1 -DockerTimeoutSeconds 360
```

### Stop local app and services

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\stop-local.ps1
```

What it does:

- Stops API process started with `dotnet run --project src/ContractorPro.Api`
- Stops frontend dev server process (`npm run dev` / Vite)
- Runs `docker compose down` from repo root
- Stops Docker Desktop processes (unless `-KeepDocker` is set)

Options:

```powershell
# Keep Docker services running
powershell -ExecutionPolicy Bypass -File .\scripts\stop-local.ps1 -KeepDocker

# Force kill matching processes
powershell -ExecutionPolicy Bypass -File .\scripts\stop-local.ps1 -Force

# Increase teardown wait time
powershell -ExecutionPolicy Bypass -File .\scripts\stop-local.ps1 -DockerShutdownTimeoutSeconds 180
```
