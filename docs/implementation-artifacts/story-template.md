# Story {{epic}}.{{story}}: {{title}}

Status: ready-for-dev

Epic: {{epic_ref}} · FR: {{fr_refs}} · Journey: {{journey_refs}} · Depends: {{dependencies}}

---

## Story

As a {{role}},  
I want {{action}},  
so that {{benefit}}.

---

## Reasoning (the whys)

### Why this story now

{{why_now}}

### Why this approach

{{approach_and_alternatives}}

### Out of scope (this story)

{{explicit_out_of_scope}}

### Tradeoffs

{{tradeoffs}}

### Planning references

- [architecture-v0.1.md](../planning-artifacts/architecture-v0.1.md) — {{sections}}
- [epics-and-stories.md](../planning-artifacts/prds/prd-ContractorPro-2026-08-15/epics-and-stories.md) — {{epic_story}}
- [discovery-log.md](../planning-artifacts/discovery-log.md) — {{decisions if any}}

---

## Details

### API

{{routes_methods_contracts}}

### Data model

{{entities_migrations}}

### UI

{{routes_states_copy}}

### Integrations / configuration

{{external_services_env_vars}}

### Files to create or modify

| Path | NEW/UPDATE | Purpose |
|------|------------|---------|
| … | … | … |

### Tasks / subtasks

- [ ] Task 1 (AC: 1, 2)
  - [ ] Subtask …
- [ ] Task 2 (AC: 3)

---

## Acceptance criteria

1. **AC-1:** …
2. **AC-2:** …

---

## Security & vulnerability review

| Check | Applicable? | Mitigation / notes |
|-------|-------------|-------------------|
| Authentication | Yes/No/N/A | … |
| Authorization / tenancy | Yes/No/N/A | … |
| Input validation | Yes/No/N/A | … |
| Secrets handling | Yes/No/N/A | … |
| Injection (SQL/XSS) | Yes/No/N/A | … |
| CSRF | Yes/No/N/A | … |
| Rate limiting / abuse | Yes/No/N/A | … |
| Dependency / supply chain | Yes/No/N/A | … |
| Privacy / logging | Yes/No/N/A | … |

**Identified risks:**

1. … → **Mitigation:** …

**Follow-up stories (if any):** …

---

## Unit tests

| Test | AC | Description |
|------|-----|-------------|
| … | AC-n | … |

**Run:** `{{command}}`

*N/A reason (if not applicable):* …

---

## Integration tests

| Test | AC | Description |
|------|-----|-------------|
| … | AC-n | … |

**Run:** `{{command}}`

*N/A reason (if not applicable):* …

---

## E2E tests

| Test | AC | Description |
|------|-----|-------------|
| … | AC-n | … |

**Run:** `{{command}}`

*Deferred / N/A reason:* …

---

## Manual QA checklist

- [ ] **MQ-1:** …
  - **Expected:** …
- [ ] **MQ-2:** …
  - **Expected:** …
- [ ] **MQ-3 (negative):** …
  - **Expected:** …
- [ ] **Regression:** …
  - **Expected:** …

**Sign-off:** Thomas · Date: ___

---

## Definition of done

- [ ] All AC met
- [ ] Security review complete
- [ ] Unit / integration / e2e per above (or N/A documented)
- [ ] Manual QA complete
- [ ] Dev Agent Record updated
- [ ] CI green

---

## Dev Agent Record

### Agent model

### Completion notes

### File list

### Test results
