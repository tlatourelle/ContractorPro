---
status: in-review
baseline_commit: 8c0518ef5d2fa035869a37c7c928a0a2a758daef
---

# Story 1.1: Google OAuth BFF session and contractor auto-provision

Status: in-review

Epic: 1 · FR: FR-1 · Journey: C-1 step 1 · Depends: 1.0 · Product: E1-S1 · Milestone: M1

Closeout companion: [1-1a-closeout-hardening-and-auth-readiness.md](./1-1a-closeout-hardening-and-auth-readiness.md)

---

## Story

As a **new contractor owner (Ryan)**,  
I want to **sign in with Google and have the system create my company automatically**,  
so that **I can use ContractorPro without manual provisioning or an invite code**.

---

## Reasoning (the whys)

### Why this story now

Story **1.0** provides the runnable stack. **M1** and all team-member features require a authenticated **Contractor tenant** context. Without this story, nothing in `/app/*` can be built securely.

This story delivers the **MVP auth vertical slice**: OAuth → session → tenant provisioning → protected API.

### Why this approach

| Decision | Why |
|----------|-----|
| **Entra External ID (CIAM)** | Managed Google federation; no password storage; aligns with Azure hosting path |
| **BFF HTTP-only session cookie** | Avoids exposing tokens to React; `credentials: 'include'`; XSS cannot exfiltrate bearer JWT from localStorage |
| **Google only (M1)** | Locked A-3; reduces IdP matrix for first ship |
| **Auto-provision on first login** | FR-1 / E1-S1; Ryan self-serves; no admin UI (A-4) |
| **Provisioning in one DB transaction** | Prevent orphan `users` without `contractors` |

**Alternatives rejected:**

| Alternative | Why rejected |
|-------------|--------------|
| SPA + MSAL bearer tokens | Architecture locked BFF cookie 2026-08-20 |
| ASP.NET Identity + passwords | Defer E1-S2; OAuth-first |
| Manual admin creates contractor | Violates self-serve MVP |

### Out of scope (this story)

- Apple / Microsoft OAuth (v0.1.1)
- Google **Calendar** OAuth (M6 / E3-S1 — separate consent + tokens in `calendar_connections`)
- Magic links for subs/customers (M7)
- Stripe, billing tiers, entitlements enforcement (Phase 2; schema hooks OK if migration groups naturally)
- Full onboarding checklist (story 1.3)
- Company profile edit UI beyond displaying name from Google/email (story 1.2)
- `/admin/*` platform routes
- Playwright E2E against real Google (deferred — integration + manual QA)

### Tradeoffs

- **Company name from Google display name or email domain** on first login — Ryan edits in 1.2; avoids blocking login on profile form.
- **Server-side session store vs encrypted cookie** — start with **distributed cache or DB session table** if multi-instance later; single-instance can use `AddSession` + data protection for MVP.
- **Entra dev tenant setup** is manual Thomas work — story documents required app registrations and redirect URIs.

### Planning references

- [architecture-v0.1.md §1.4, §4, §5.2, §3.3](../planning-artifacts/architecture-v0.1.md)
- [planning-decision-checklist.md A-3, A-4](../planning-artifacts/planning-decision-checklist.md)
- [epics-and-stories.md E1-S1](../planning-artifacts/prds/prd-ContractorPro-2026-08-15/epics-and-stories.md)
- [contractor-journeys.md C-1](../planning-artifacts/prds/prd-ContractorPro-2026-08-15/user-journeys/contractor-journeys.md)
- [story-standard.md](./story-standard.md)

---

## Details

### Prerequisites (Thomas — before dev)

1. **Entra External ID tenant** (CIAM) with Google as identity provider.
2. **App registration** for ContractorPro API (web app / confidential client or CIAM user flow per Microsoft.Identity.Web CIAM docs).
3. **Redirect URIs:** `https://localhost:{apiPort}/signin-oidc` (dev); prod URI placeholder in config.
4. **Client secret** in user-secrets / Key Vault — not in repo.
5. Google Cloud project linked in Entra IdP (Google sign-in enabled).

Document actual IDs in `src/README.md` under “Auth setup” (placeholders only in repo).

### Auth flow

```text
1. React /app/login → user clicks "Sign in with Google"
2. Browser → GET /api/v1/auth/login (or /signin — pick one, document)
3. API → Challenge OpenIdConnect (Entra CIAM → Google)
4. Callback → OnTokenValidated:
     a. Resolve auth_identities by (provider=google, provider_subject)
     b. If missing: provision user + contractor + team_member (owner) in transaction
     c. If exists: load user + team_member + contractor
     d. Issue session (cookie)
5. Redirect → React /app/dashboard
6. React → GET /api/v1/team/me (credentials: include) → { user, contractor, teamMember }
```

### API endpoints

| Route | Method | Auth | Behavior |
|-------|--------|------|----------|
| `/api/v1/auth/login` | GET | None | Initiate OIDC challenge; redirect to Entra/Google |
| `/api/v1/auth/logout` | POST | Session | Clear session + OIDC sign-out (best effort) |
| `/api/v1/team/me` | GET | Session | Current user, team member, contractor summary |
| `/api/v1/team/*` (other) | * | Session | **401/403** without valid session (middleware) |

**Response `GET /api/v1/team/me` (200):**

```json
{
  "user": { "id": "uuid", "displayName": "Ryan", "email": "ryan@..." },
  "teamMember": { "id": "uuid", "role": "owner", "isOwner": true },
  "contractor": { "id": "uuid", "name": "Riverside Remodeling", "status": "active" }
}
```

### Data model (EF migration — add to 1.0 baseline)

Tables per [architecture §5.2](../planning-artifacts/architecture-v0.1.md):

- `contractors` — `id`, `name`, `status`, `timezone` (default `America/Chicago`), timestamps
- `users` — `id`, `email`, `display_name`, `status`
- `auth_identities` — `user_id`, `provider` (`google`), `provider_subject`, `email_at_provider`, `last_login_at`; UNIQUE `(provider, provider_subject)`
- `team_members` — `contractor_id`, `user_id`, `role` (`owner`), `is_owner=true`; UNIQUE `(contractor_id, user_id)`

**Phase 2 hooks (optional same migration):** `subscription_entitlements` with `tier=beta_full_access`, `billing_enforcement=false` — create row on contractor provision.

**Provisioning rules:**

| Case | Action |
|------|--------|
| New Google sub | Create user + contractor (name from claim) + team_member owner + auth_identity + optional entitlement |
| Existing Google sub | Update `last_login_at`; load existing contractor via team_member |
| Same email, different provider | **Out of scope** — only Google in M1 |

### Session cookie requirements

| Attribute | Value |
|-----------|-------|
| Name | e.g. `.ContractorPro.Session` |
| HttpOnly | `true` |
| Secure | `true` in non-Development |
| SameSite | `Lax` (or `Strict` if OAuth redirect chain allows — test) |
| Path | `/` |
| Max age | 14 days sliding (configurable) |

Store in session: `UserId`, `TeamMemberId`, `ContractorId` (UUIDs).

### Frontend

| Route | Behavior |
|-------|----------|
| `/app/login` | “Sign in with Google” button → navigates to `/api/v1/auth/login` (full page redirect) |
| `/app/dashboard` | Protected: fetch `/team/me`; show welcome + company name; redirect to login if 401 |
| `/app/*` | Auth guard wrapper |

Use hand-typed `fetch` with `credentials: 'include'`.

### Middleware / authorization

- `TeamMemberAuthorizationMiddleware` or `[Authorize(Policy = "TeamMember")]` on `/api/v1/team/*`.
- Unauthenticated → `401` JSON `{ "error": "unauthorized" }` (not redirect for API).
- React handles redirect to login on 401.

### Files to create or modify

| Path | NEW/UPDATE | Purpose |
|------|------------|---------|
| `src/ContractorPro.Infrastructure/Persistence/Entities/*` | NEW | User, Contractor, TeamMember, AuthIdentity |
| `src/ContractorPro.Infrastructure/Persistence/Migrations/*` | NEW | Auth tables migration |
| `src/ContractorPro.Application/Identity/*` | NEW | `IProvisioningService`, `ITeamContext` |
| `src/ContractorPro.Application/Identity/ProvisioningService.cs` | NEW | First-login transaction |
| `src/ContractorPro.Api/Auth/*` | NEW | OIDC config, callbacks |
| `src/ContractorPro.Api/Controllers/TeamController.cs` | NEW | `/team/me` |
| `src/ContractorPro.Api/Middleware/TeamMemberAuthMiddleware.cs` | NEW | Session validation |
| `src/ContractorPro.Web/src/pages/Login.tsx` | NEW | Login page |
| `src/ContractorPro.Web/src/pages/Dashboard.tsx` | NEW | Dashboard stub |
| `src/ContractorPro.Web/src/lib/api.ts` | UPDATE | `getTeamMe()` |
| `tests/ContractorPro.Application.Tests/Identity/ProvisioningServiceTests.cs` | NEW | Unit tests |
| `tests/ContractorPro.Api.Tests/Auth/TeamMeEndpointTests.cs` | NEW | Integration tests |
| `src/README.md` | UPDATE | Entra setup + test login steps |

### Tasks / subtasks

- [ ] Task 1: EF entities + migration (AC: 2, 3)
- [ ] Task 2: ProvisioningService with transactional create (AC: 3, 4)
- [ ] Task 3: Entra CIAM + Microsoft.Identity.Web OIDC + callback (AC: 1, 5)
- [ ] Task 4: Session cookie + `/team/me` (AC: 5, 6)
- [ ] Task 5: Protect `/api/v1/team/*` (AC: 6, 7)
- [ ] Task 6: React login + dashboard + auth guard (AC: 1, 8)
- [ ] Task 7: Tests + manual QA doc (AC: all)

---

## Acceptance criteria

1. **AC-1:** Unauthenticated user visiting `/app/dashboard` is redirected to `/app/login`.
2. **AC-2:** “Sign in with Google” completes OAuth and lands on dashboard showing **display name** and **company name**.
3. **AC-3:** **First** Google account sign-in creates exactly one `contractors`, one `users`, one `team_members` (owner), one `auth_identities` row.
4. **AC-4:** **Second** sign-in with same Google account reuses same contractor — **no duplicate** contractor or user.
5. **AC-5:** **Different** Google account creates a **second** contractor tenant.
6. **AC-6:** `GET /api/v1/team/me` without session cookie returns `401`.
7. **AC-7:** `GET /api/v1/team/me` with valid session returns correct user + contractor IDs matching DB.
8. **AC-8:** Session persists across browser restart (close tab, reopen `/app/dashboard` — still authenticated).
9. **AC-9:** Logout clears session; subsequent `/team/me` returns `401`.
10. **AC-10:** No Apple/Microsoft login options in UI.

---

## Security & vulnerability review

| Check | Applicable? | Mitigation / notes |
|-------|-------------|-------------------|
| Authentication | **Yes** | OIDC via Entra; validate issuer, audience, nonce/state on callback |
| Authorization / tenancy | **Yes** | Session binds `ContractorId`; `/team/me` only returns own tenant; no cross-tenant ID in URL |
| Input validation | Low | OIDC claims validated by middleware; sanitize display name length (e.g. 200 chars) |
| Secrets handling | **Yes** | Client secret in user-secrets/Key Vault; config keys documented; never log tokens |
| Injection (SQL/XSS) | **Yes** | EF parameterized; React escapes display name; no `dangerouslySetInnerHTML` |
| CSRF | **Yes** | OAuth state parameter; SameSite cookie; POST logout should use antiforgery or same-site-only cookie pattern |
| Rate limiting | **Medium** | Consider basic rate limit on `/auth/login` callback — optional follow-up if abuse |
| Dependency / supply chain | **Yes** | `Microsoft.Identity.Web` current stable; run vulnerable package check |
| Privacy / logging | **Yes** | Log user id + contractor id on login; **do not** log id_token, access_token, or cookie value |

**Identified risks:**

1. **Open redirect after login** → **Mitigation:** allowlist redirect targets (`/app/dashboard` only); no user-supplied `returnUrl` without validation.
2. **Session fixation** → **Mitigation:** regenerate session ID on successful login.
3. **Duplicate provisioning race** → **Mitigation:** UNIQUE on `auth_identities(provider, provider_subject)`; catch duplicate and retry load path.
4. **XSS stealing session** → **Mitigation:** HttpOnly cookie; CSP baseline in API headers (report-only OK initially).

**Follow-up stories:** 1.2 company profile; E10-S1 hard tenancy query filters on all modules.

---

## Unit tests

| Test | AC | Description |
|------|-----|-------------|
| `ProvisionNewUser_CreatesContractorOwnerAndAuthIdentity` | AC-3 | Mock DbContext/UoW; assert 4 entities created, `is_owner=true` |
| `ProvisionExistingUser_DoesNotCreateDuplicateContractor` | AC-4 | Pre-seed auth_identity; second call returns same IDs |
| `ProvisionNewUser_RollsBackOnFailure` | AC-3 | Simulate failure after user insert; no partial contractor |

**Run:** `dotnet test tests/ContractorPro.Application.Tests --filter Provisioning`

---

## Integration tests

| Test | AC | Description |
|------|-----|-------------|
| `TeamMe_Returns401_WhenAnonymous` | AC-6 | WebApplicationFactory, no cookie |
| `TeamMe_Returns200_WhenAuthenticated` | AC-7 | Test auth handler or cookie auth scheme stub with seeded session |
| `TeamRoutes_Return401_WhenAnonymous` | AC-6, AC-7 | Any `/api/v1/team/*` route |

*OAuth callback integration:* use **TestAuthHandler** scheme in test environment to simulate authenticated principal with Google claims — do not call real Entra in CI.

**Run:** `dotnet test tests/ContractorPro.Api.Tests --filter Team`

---

## E2E tests

| Test | AC | Description |
|------|-----|-------------|
| `AnonymousDashboard_RedirectsToLogin` | AC-1 | Playwright, CI-safe |
| `AuthenticatedTeamMe_ShowsDashboard` | AC-2, AC-7 | Playwright with test-auth bridge |
| `Logout_ClearsSession` | AC-6, AC-9 | Playwright + cookie/session verification |
| `GoogleLogin_CreatesDashboard` | AC-1, AC-2 | Manual smoke with real Google account |

**Run:**
- CI-safe Playwright suite: planned in implementation stories 1.4-1.6
- Manual real Google smoke: planned in implementation story 1.7

*Execution note:* Real Google UI automation remains out of CI scope; coverage is split between deterministic Playwright session tests and manual provider smoke.

---

## Manual QA checklist

- [ ] **MQ-1:** Open `/app/login` logged out → click Sign in with Google → complete Google consent.
  - **Expected:** Redirect to `/app/dashboard` with welcome text and company name.
- [ ] **MQ-2:** Query DB after MQ-1.
  - **Expected:** 1 contractor, 1 user, 1 team_member (owner), 1 auth_identity for your Google sub.
- [ ] **MQ-3:** Sign out → sign in again with **same** Google account.
  - **Expected:** Same company name; DB still 1 contractor row for that identity.
- [ ] **MQ-4:** Sign out → sign in with **different** Google account.
  - **Expected:** Different company on dashboard; 2 contractors in DB.
- [ ] **MQ-5:** While logged in, close browser completely → reopen `/app/dashboard`.
  - **Expected:** Still logged in (session cookie persists within max age).
- [ ] **MQ-6:** Logout → navigate to `/app/dashboard`.
  - **Expected:** Redirect to login; `curl /api/v1/team/me` without cookie → 401.
- [ ] **MQ-7:** DevTools → Application → Cookies.
  - **Expected:** Session cookie has HttpOnly; Secure in HTTPS profile.
- [ ] **MQ-8:** Confirm login page shows **Google only** — no Apple/Microsoft buttons.

**Sign-off:** Thomas · Date: ___

---

## Definition of done

- [ ] All AC met
- [ ] Security review complete
- [ ] Unit + integration tests passing
- [ ] Manual QA checklist completed
- [ ] Entra dev setup documented in `src/README.md`
- [ ] Dev Agent Record updated
- [ ] No secrets in git; CI green

---

## Dev Agent Record

### Agent model

- GPT-5.3-Codex

### Completion notes

- Implemented Entra External ID OIDC challenge + cookie session auth in API startup and auth controller.
- Implemented first-login provisioning flow to create `users`, `contractors`, `team_members` (owner), and `auth_identities` transactionally.
- Implemented authenticated `/api/v1/team/me` endpoint and protected team routes behavior.
- Implemented minimal React login and dashboard flow using cookie-based API calls.
- Verified local-dev fallback behavior when external auth is not configured (`Authentication:ExternalId:Enabled=false`) and test auth stubs for integration tests.
- Round 2 remediation applied after independent review: fixed provider subject extraction to use `ClaimTypes.NameIdentifier` with `sub` fallback; set persistent auth cookie on login; added duplicate-race recovery path in provisioning; added unit tests for external identity claim extraction.

### File list

- src/ContractorPro.Api/Program.cs
- src/ContractorPro.Api/Auth/ExternalIdAuthenticationOptions.cs
- src/ContractorPro.Api/Auth/ContractorProClaimTypes.cs
- src/ContractorPro.Api/Controllers/AuthController.cs
- src/ContractorPro.Api/Controllers/TeamController.cs
- src/ContractorPro.Api/Middleware/TeamMemberAuthMiddleware.cs
- src/ContractorPro.Application/Identity/IProvisioningService.cs
- src/ContractorPro.Application/Identity/ProvisioningRequest.cs
- src/ContractorPro.Application/Identity/ProvisioningResult.cs
- src/ContractorPro.Application/Identity/ProvisioningService.cs
- src/ContractorPro.Infrastructure/ContractorProDbContext.cs
- src/ContractorPro.Infrastructure/Entities/User.cs
- src/ContractorPro.Infrastructure/Entities/Contractor.cs
- src/ContractorPro.Infrastructure/Entities/TeamMember.cs
- src/ContractorPro.Infrastructure/Entities/AuthIdentity.cs
- src/ContractorPro.Infrastructure/Migrations/20260821120000_AddGoogleAuthSessionAndTeamContext.cs
- src/ContractorPro.Web/src/api.ts
- src/ContractorPro.Web/src/pages/Login.tsx
- src/ContractorPro.Web/src/pages/Dashboard.tsx
- src/ContractorPro.Web/src/pages/AppLayout.tsx
- tests/ContractorPro.Application.Tests/Identity/ProvisioningServiceTests.cs
- tests/ContractorPro.Api.Tests/Auth/TeamMeEndpointTests.cs
- src/ContractorPro.Api/Auth/ExternalIdentityClaims.cs
- tests/ContractorPro.Api.Tests/Auth/ExternalIdentityClaimsTests.cs

### Test results

- `dotnet build ContractorPro.sln`
  - PASS
  - Build succeeded in 2.1s
  - Warnings: `NU1903` on `SQLitePCLRaw.lib.e_sqlite3` in `ContractorPro.Application.Tests`
- `dotnet test ContractorPro.sln`
  - PASS
  - Total: 12, Failed: 0, Succeeded: 12, Skipped: 0
  - Build succeeded with warnings: `NU1903` on `SQLitePCLRaw.lib.e_sqlite3`
- `dotnet test tests/ContractorPro.Application.Tests/ContractorPro.Application.Tests.csproj`
  - PASS
  - Total: 4, Failed: 0, Succeeded: 4, Skipped: 0, Duration: 3.8s
- `dotnet test tests/ContractorPro.Api.Tests/ContractorPro.Api.Tests.csproj`
  - PASS
  - Total: 8, Failed: 0, Succeeded: 8, Skipped: 0, Duration: 6.2s
- `npm run lint` (from `src/ContractorPro.Web`)
  - PASS
  - No lint errors reported
  - Warning: TypeScript 5.9.3 is above `@typescript-eslint` officially supported range
- `npm run build` (from `src/ContractorPro.Web`)
  - PASS
  - Vite build completed successfully in 668ms
