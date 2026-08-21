# Epic 1 Context: Foundation & team member auth

<!-- Compiled from planning artifacts. Edit freely. Regenerate with compile-epic-context if planning docs change. -->

## Goal

Build a runnable .NET + React + PostgreSQL solution scaffold and implement Google OAuth sign-in for contractors so they can land in a workspace. This epic provides the technical foundation for all subsequent features: a modular monolith architecture, database access layer, local development workflow, and team member authentication flow.

## Stories

- Story 1.0: Solution scaffold
- Story 1.1: Google OAuth BFF session and contractor auto-provision
- Story 1.2: Company profile and session context
- Story 1.3: Guided onboarding checklist shell

## Requirements & Constraints

**Technology Stack (MVP Phase 1):**

- **Paradigm:** Modular monolith (one deployable; clear module boundaries; extract later if needed)
- **API:** ASP.NET Core (.NET 10) with strong typing for AI-assisted development
- **ORM:** EF Core + Npgsql for PostgreSQL 16
- **Database:** PostgreSQL 16 (Neon or Docker locally; Azure Flexible Server in production)
- **Frontend:** React 19 + TypeScript + Vite + shadcn/ui
- **Frontend layout:** Single SPA — `/app/*` (team member, desktop-first) + `/p/*` (portal, mobile-first)
- **Hosting:** Azure App Service (API) + Azure Static Web Apps or same App Service (UI)
- **Secrets:** Azure Key Vault; no secrets committed to git
- **Observability:** Application Insights (structured logging)
- **Transactional email:** Resend (magic links, invites, notifications)
- **Team member auth:** Entra External ID with Google OAuth via Microsoft.Identity.Web
- **Team member session:** BFF HTTP-only session cookie (not SPA bearer tokens in browser)

**Project Structure:**

```
src/
  ContractorPro.Api/              # ASP.NET Core host, health endpoints, middleware
  ContractorPro.Application/      # Application services (empty modules structure)
  ContractorPro.Domain/           # Entities, interfaces, constants
  ContractorPro.Infrastructure/   # EF Core, DbContext, migrations, external integrations
  ContractorPro.Web/              # React SPA with Vite
tests/
  ContractorPro.Application.Tests/
  ContractorPro.Api.Tests/
ContractorPro.sln
docker-compose.yml                # Local Postgres 16
```

**Application Features (Epic Scope):**

- Story 1.0: Health checks, local dev workflow, CI skeleton (no auth)
- Story 1.1: Google OAuth sign-in, HTTP-only session cookie, contractor auto-provision with team_members table
- Story 1.2: Company profile (name, timezone) and team member profile view
- Story 1.3: Empty-state onboarding checklist widget

**Non-Functional Requirements:**

- Zero secrets or API keys in git
- Production-ready code (clean, minimal, no AI-generated noise)
- All acceptance criteria verified by tests before marking done
- Responsive web only (no native apps; PWA optional future)

## Technical Decisions

**Architecture Patterns:**

- **Modular monolith boundaries:** Api (host, controllers), Application (service interfaces, commands), Domain (entities, constants), Infrastructure (data access, external integrations)
- **Dependency flow:** Api → Application, Infrastructure; Application → Domain; Infrastructure → Domain (clean architecture layers)
- **Database context:** Single `ContractorProDbContext` per EF Core best practices; migrations in Infrastructure; seeding of platform defaults (e.g., dashboard poll interval)

**Data Model (Epic 1):**

- `ContractorProDbContext` registered with Npgsql + EF Core
- Minimal schema: `contractors` (subscription owners), `team_members` (authenticated users), `platform_settings` (admin-only configuration like `dashboard_poll_interval_seconds`)
- EF Core migrations for schema versioning; initial migration either empty or with `platform_settings` seed
- No auth tables in Story 1.0; auth tables added in Story 1.1

**API Contract:**

- Base path: `/api/v1`
- Health endpoints: `GET /api/v1/health` (includes DB check) and `GET /api/v1/health/live` (no DB check)
- CORS: Restrict to development origin (`http://localhost:5173`); configure for 1.1 OAuth callback
- Structured logging to console (Application Insights in production)
- Connection string via environment variable `ConnectionStrings__Default` or User Secrets template

**Frontend Patterns:**

- Single entry point with React Router
- Routes: `/` → redirect `/app`, `/app` → team member dashboard (scaffold), `/p` → portal (scaffold)
- Hand-typed fetch helper targeting `/api/v1` with `credentials: 'include'` (for 1.1 session cookies)
- Tailwind CSS baseline; shadcn initialization optional in 1.0 or 1.1 UI polish

**Development Workflow:**

1. `docker compose up -d` → Start Postgres locally
2. `dotnet ef database update` → Apply migrations
3. `dotnet run --project src/ContractorPro.Api` → API on (e.g.) 5001
4. `npm run dev` (from Web folder) → Vite on 5173
5. `.gitignore` covers `appsettings.*.local`, `.env`, user-secrets, node_modules, build artifacts

**Security Baseline (Story 1.0):**

- No authentication endpoints yet (Story 1.0)
- Health endpoints have no input validation needed
- Connection string not hardcoded; environment-based or User Secrets
- CORS limited to localhost dev origin
- SQL injection prevented by EF Core (parameterized queries)
- XSS mitigated by React default escaping

## Cross-Story Dependencies

**Within Epic 1:**

- Story 1.0 (scaffold) unlocks 1.1+ (all subsequent stories depend on working solution)
- Story 1.1 (auth) depends on 1.0 scaffold; adds Entra/Google integration, `auth_identities` table, session middleware
- Story 1.2 (company profile) depends on 1.1 (assumes authenticated team members exist)
- Story 1.3 (onboarding) depends on 1.2 (assumes profile context available)

**External Dependencies:**

- Azure Key Vault (later phases for OAuth secrets; Story 1.0 uses local User Secrets)
- Google Cloud project (required for 1.1 OAuth; pre-configured separately)
- Entra External ID tenant (pre-configured; linked to Google provider in 1.1)
