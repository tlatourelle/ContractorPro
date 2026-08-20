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

### Story 1.2: Company profile and session context

Company name/timezone display and edit; team member profile on `/team/me`; tenancy context on all team queries (E10-S1 prep).

**Depends:** 1.1 · **Product:** E1-S1, E10-S1 · **Milestone:** M2

### Story 1.3: Guided onboarding checklist shell

Empty-state checklist widget (steps stubbed); progress persistence hook — full steps wired in later stories.

**Depends:** 1.2 · **Product:** E1-S4 · **Milestone:** M5

---

## Epic 2: Projects & tasks

*(Backlog — generated in sprint planning pass 2 after Epic 1 stories complete.)*

### Story 2.1: Create project placeholder

Backlog placeholder for sprint-status continuity — replace when Epic 2 stories are authored.

---

*Add new implementation epics here as Phase 1 progresses. Use heading format `## Epic N:` and `### Story N.M:` for sprint_plan.py.*
