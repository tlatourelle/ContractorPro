# ContractorPro — Implementation Story Standard

**Owner:** Thomas  
**Applies to:** Every story file in `docs/implementation-artifacts/` before `dev-story` runs  
**Status:** Required — do not start implementation without a story that satisfies this doc.

Planning epics (`epics-and-stories.md`) define **what** and **why** at product level.  
Implementation stories define **how**, **proof**, and **risk** at build level.

---

## Required sections (every story)

Each story file MUST include all sections below. If a section is not applicable, include it with **N/A — {one-line reason}** (never omit the heading).

### 1. Story (user value)

- **As a** / **I want** / **so that** — copied or refined from epic
- **Epic / FR / Journey refs** — e.g. E1-S1, FR-1, C-1

### 2. Reasoning (the whys)

Explain decisions so future readers (and agents) do not re-litigate planning:

| Topic | Answer |
|-------|--------|
| **Why this story exists now** | Dependency order, risk reduction, unblock value |
| **Why this approach** | Chosen pattern vs alternatives (1–2 sentences each) |
| **Why not in scope** | Explicit deferrals to prevent scope creep |
| **Tradeoffs accepted** | Speed vs purity, manual vs automated, etc. |

Link to locked decisions: `architecture-v0.1.md`, `planning-decision-checklist.md`, `discovery-log.md`.

### 3. Details (implementation specification)

Enough detail that a dev agent does not invent architecture:

- **API contracts** — routes, methods, request/response shapes, status codes
- **Data model** — tables/entities touched, fields, constraints, migrations
- **UI behavior** — routes, states (loading/empty/error), copy where it matters
- **Integration points** — Entra, Google, Twilio, etc. with env vars / secrets notes
- **File structure** — modules/paths to create or modify (NEW vs UPDATE)
- **Tasks / subtasks** — mapped to acceptance criteria IDs

### 4. Acceptance criteria

Numbered, **testable**, unambiguous. Prefer Given/When/Then for user-visible behavior.

Example:

```text
AC-1: Given no session cookie, when GET /app/projects, then 401 and redirect to login.
AC-2: Given first Google sign-in, when OAuth callback completes, then one contractors row + owner team_member created in one transaction.
```

Every AC must be verifiable by at least one of: unit test, integration test, e2e test, or manual step.

### 5. Security & vulnerability review

Per-story threat pass (not a full pentest). Address what this change touches:

| Check | Story-specific notes |
|-------|---------------------|
| **Authentication** | Session/cookie flags, OAuth state/nonce, token storage |
| **Authorization / tenancy** | `contractor_id` scoping; no cross-tenant reads/writes |
| **Input validation** | Query/body/path params; max lengths; enum allowlists |
| **Secrets** | No secrets in repo; Key Vault / user-secrets; no logging tokens |
| **Injection** | Parameterized SQL (EF); no raw HTML injection in React |
| **CSRF** | BFF cookie + SameSite; state-changing POST requirements |
| **Rate limiting / abuse** | Login callback, magic-link issuance if applicable |
| **Dependency risk** | New packages — note supply-chain / known CVE posture |
| **Privacy** | PII fields; log redaction; retention |

**Findings:** List risks identified and **mitigations built into this story** or **follow-up story IDs**.

For auth, messaging, or admin stories: run mental OWASP ASVS L1 pass on the delta.

### 6. Unit tests (if applicable)

| Test | Covers AC | Notes |
|------|-----------|-------|
| … | AC-n | … |

- **Framework:** xUnit + (project TBD on scaffold)
- **Scope:** Business logic, validators, provisioning, middleware in isolation
- **Required when:** Pure logic, state machines, entitlement checks, parsers

If N/A: explain (e.g. “UI-only copy change, no logic”).

### 7. Integration tests (if applicable)

| Test | Covers AC | Notes |
|------|-----------|-------|
| … | AC-n | … |

- **Scope:** API + test DB, OAuth callback with test claims, webhook signatures
- **Required when:** DB writes, external boundaries (mock or test containers)

### 8. E2E tests (if applicable)

| Test | Covers AC | Notes |
|------|-----------|-------|
| … | AC-n | … |

- **Framework:** Playwright (when scaffold exists)
- **Required when:** Critical user path, regressions are expensive (auth, propose/accept)
- **OAuth note:** Real Google E2E may use test Entra tenant; until then mark **E2E: deferred** and rely on integration + manual

If N/A: state why and what compensates (integration + manual).

### 9. Manual validation checklist

**Always required.** Numbered steps a human runs before marking story done:

```markdown
## Manual QA

- [ ] Step 1: …
  - **Expected:** …
- [ ] Step 2: …
  - **Expected:** …
```

Include: happy path, one negative path, regression spot-check, browser refresh / session persistence where relevant.

### 10. Definition of done

Story is **done** only when:

- [ ] All AC satisfied
- [ ] Security section reviewed; mitigations implemented or follow-up filed
- [ ] Unit tests written and passing (or N/A documented)
- [ ] Integration tests written and passing (or N/A documented)
- [ ] E2E tests written and passing (or N/A documented with compensating manual QA)
- [ ] Manual QA checklist completed by Thomas (or delegate)
- [ ] Dev Agent Record updated: files changed, commands run, notes
- [ ] No secrets committed; linter/tests green in CI

### 11. Dev Agent Record

Filled during/after implementation:

- Agent model
- Completion notes (what was built, surprises)
- File list
- Test commands run + results

---

## Story file template

Copy `story-template.md` in this folder when creating a new story.

---

## Relationship to other artifacts

| Artifact | Role |
|----------|------|
| `planning-artifacts/.../epics-and-stories.md` | Product backlog; M1–M21 milestones |
| `implementation-artifacts/sprint-status.yaml` | Sprint tracking (from sprint-planning) |
| `implementation-artifacts/{epic}-{story}-*.md` | Dev-ready story per this standard |
| `architecture-v0.1.md` | Technical source of truth |

**Rule:** Never implement from an epic bullet alone. Always from a story file that meets this standard.
