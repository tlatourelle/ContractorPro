---
status: review
created: 2026-08-21
---

# Story 1.3: Guided onboarding checklist shell

Status: review

Epic: 1 · Depends: 1.2 · Product: E1-S4 · Milestone: M5

---

## Story

As a newly provisioned company owner,
I want a guided onboarding checklist shell in my dashboard,
so that I can track setup progress while deeper onboarding features are implemented in later stories.

---

## Scope implemented

### Frontend

- Added a production-quality onboarding checklist card to dashboard empty-state context:
  - visible in the dashboard "Getting Started" section
  - includes three stubbed onboarding steps
  - each step supports complete/incomplete toggling
  - progress summary displays completed count
- Preserved scope boundaries:
  - no deep feature wiring for invites/projects
  - step content is intentionally stubbed for future stories

### Persistence hook

- Added a lightweight local persistence seam backed by `localStorage` for MVP shell behavior.
- Persistence key is scoped per contractor and team member to avoid cross-account leakage in shared browsers.
- Hook exposes a reusable state API (`isLoaded`, `steps`, `completedCount`, `toggleStep`) for future component reuse.
- Added future API seam types (`OnboardingChecklistProgressStore`) in web API contracts so server-backed persistence can be introduced with minimal UI refactor.

### API/backend

- No backend endpoint changes in this story.
- Server-backed persistence intentionally deferred; current seam allows clean migration to API storage later.

---

## Acceptance criteria status

- [x] Minimal onboarding checklist shell is present in dashboard UI
- [x] Checklist can mark stubbed steps complete/incomplete
- [x] Progress persists for MVP shell via lightweight storage
- [x] Types and helper seam updated for future server persistence
- [x] Scope remains tight with no deep onboarding integrations

---

## Files changed

- `src/ContractorPro.Web/src/api.ts`
- `src/ContractorPro.Web/src/onboardingChecklist.ts`
- `src/ContractorPro.Web/src/pages/Dashboard.tsx`
- `docs/implementation-artifacts/1-3-guided-onboarding-checklist-shell.md`
- `docs/implementation-artifacts/sprint-status.yaml`

---

## Validation

- `npm run lint` in `src/ContractorPro.Web`
- `npm run build` in `src/ContractorPro.Web`

---

## Notes and follow-ups

- Follow-up candidate: add `/api/v1/team/onboarding-progress` with read/write operations and swap the storage implementation behind `OnboardingChecklistProgressStore`.
- Follow-up candidate: replace stubbed steps with live completion signals as Epic 2 features ship.
