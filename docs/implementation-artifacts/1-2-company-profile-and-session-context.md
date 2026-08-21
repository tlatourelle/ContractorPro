---
status: review
created: 2026-08-21
---

# Story 1.2: Company profile and session context

Status: review

Epic: 1 · Depends: 1.1 · Product: E1-S1, E10-S1 · Milestone: M2

---

## Story

As a team member,
I want to view and update company profile details and see richer team session context,
so that my workspace reflects accurate contractor settings and future team APIs can enforce tenancy consistently.

---

## Scope implemented

### API

- Extended `GET /api/v1/team/me` response with richer profile data:
  - user status
  - team member ownership and metadata
  - contractor timezone
- Added `PUT /api/v1/team/company` endpoint:
  - owner-only update of contractor name and timezone
  - basic validation for name and timezone
- Introduced shared team context extraction/loading pattern in controller for tenancy-safe queries.

### Frontend

- Updated typed API contracts for expanded `/team/me` payload.
- Added API call for company profile update.
- Enhanced dashboard:
  - displays company timezone
  - owner-editable company name and timezone form
  - save flow with success and error messaging
  - read-only guard for non-owner users
  - team profile display fields

### Tests

- Expanded team endpoint tests to assert richer payload behavior.
- Added tests for company update endpoint:
  - owner success path
  - non-owner forbidden
  - invalid timezone bad request

---

## Acceptance criteria status

- [x] Company name and timezone visible on dashboard
- [x] Owner can edit and save company name/timezone
- [x] Non-owner cannot update company profile
- [x] `/api/v1/team/me` includes team member profile details
- [x] Team query paths use a consistent session/tenant context pattern

---

## Files changed

- `src/ContractorPro.Api/Controllers/TeamController.cs`
- `src/ContractorPro.Web/src/api.ts`
- `src/ContractorPro.Web/src/pages/Dashboard.tsx`
- `tests/ContractorPro.Api.Tests/Auth/TeamMeEndpointTests.cs`

---

## Validation

- `dotnet test tests/ContractorPro.Api.Tests/ContractorPro.Api.Tests.csproj -c Release`
  - PASS (11/11)
- `npm run lint` in `src/ContractorPro.Web`
  - PASS
- `npm run build` in `src/ContractorPro.Web`
  - PASS

---

## Notes and follow-ups

- Timezone validation is currently length and non-empty checks; full IANA validation can be added in a follow-up hardening story.
- E10-S1 tenancy hardening across all future modules should reuse the context extraction pattern introduced here.
