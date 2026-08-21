# ContractorPro — Implementation Epics (Sprint 1)

Implementation breakdown for **Phase 1 MVP build**. Product epics remain in [epics-and-stories.md](../planning-artifacts/prds/prd-ContractorPro-2026-08-15/epics-and-stories.md).

Maps product **E1-S1** (Google OAuth + auto-provision Contractor) into dev-sized stories.

---

## Epic 1: Foundation & team member auth

**Goal:** Runnable solution + Ryan can sign in with Google and land in his own company workspace.

**Product refs:** E1-S1, FR-1, C-1 step 1 · **Architecture:** §4, §5.2, §7

### Story 1.0: Solution scaffold

Baseline .NET 9 + React 19 + PostgreSQL solution, health checks, local dev workflow, and CI skeleton. No auth.

**Depends:** — · **Unlocks:** 1.1+

### Story 1.1: Google OAuth BFF session and contractor auto-provision

Entra External ID (Google) sign-in, BFF HTTP-only session cookie, first-login creates `contractors` + owner `team_member`, `/team/me`, protect team API routes, minimal React login + dashboard stub.

**Depends:** 1.0 · **Product:** E1-S1

### Story 1.1a: Closeout hardening and auth readiness

Post-implementation hardening and DX closeout for 1.1: local scripts automation, dependency/security remediation, lint modernization, runtime auth toggle controls, and auth readiness documentation.

**Depends:** 1.1 · **Product:** E1-S1, E1-S7, E1-S8 · **Milestone:** M1

### Story 1.2: Company profile and session context

Company name/timezone display and edit; team member profile on `/team/me`; tenancy context on all team queries (E10-S1 prep).

**Depends:** 1.1 · **Product:** E1-S1, E10-S1 · **Milestone:** M2

### Story 1.3: Guided onboarding checklist shell

Empty-state checklist widget (steps stubbed); progress persistence hook — full steps wired in later stories.

**Depends:** 1.2 · **Product:** E1-S4 · **Milestone:** M5

### Story 1.4: Playwright E2E foundation (CI-safe)

Initialize Playwright test harness, browser projects, reporter/artifacts, and deterministic app boot for local and CI runs.

**Depends:** 1.0 · **Product:** E1-S7 · **Milestone:** M1

### Story 1.5: Test-auth bridge for deterministic session E2E

Add a test-only auth bridge in `Test` environment so E2E can establish valid session cookies without automating third-party Google UI.

**Depends:** 1.1, 1.4 · **Product:** E1-S8 · **Milestone:** M1

### Story 1.6: Auth/session E2E suite and CI integration

Implement Playwright tests for login guard, `/team/me` session behavior, dashboard rendering, logout, and session persistence; wire into setup and CI test stages with failure artifacts.

**Depends:** 1.5 · **Product:** E1-S7, E1-S8 · **Milestone:** M1

### Story 1.7: Manual Google OAuth smoke runbook

Define a strict manual smoke checklist for real Google sign-in and provisioning behavior, with evidence capture and release gate criteria.

**Depends:** 1.1 · **Product:** E1-S9 · **Milestone:** M1

---

## Epic 2: Projects & tasks

*(Backlog — generated in sprint planning pass 2 after Epic 1 stories complete.)*

### Story 2.1: Create project placeholder

Backlog placeholder for sprint-status continuity — replace when Epic 2 stories are authored.

---

*Add new implementation epics here as Phase 1 progresses. Use heading format `## Epic N:` and `### Story N.M:` for sprint_plan.py.*
