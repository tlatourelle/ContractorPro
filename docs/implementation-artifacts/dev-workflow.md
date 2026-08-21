# ContractorPro — Development Workflow

**Status:** Standard (locked 2026-08-21)  
**Applies to:** All stories implemented by bmad-agent-dev  

## Story Lifecycle

```
Backlog → Ready-for-dev → In-progress → Review → Code Review → Done
          (Amelia picks)  (Amelia)       (marked)  (reviewer)     (verified)
```

---

## Stages & Responsibilities

### 1. Ready-for-dev
- Story file complete per [story-standard.md](./story-standard.md)
- All AC, reasoning, tests, and manual QA defined
- No implementation code yet

**Next:** Developer (Amelia) picks story and transitions to `in-progress`

---

### 2. In-progress
- **Who:** bmad-agent-dev (Amelia)
- **How:** Test-first discipline (red → green → refactor)
- **Output:** Code that passes all unit, integration, E2E tests locally
- **Completion:** All manual QA steps verified; Dev Agent Record filled

**Next:** Amelia moves story to `review` status (via sprint-status.yaml)

---

### 3. Review Status
- Story file marked `status: in-review`
- Baseline commit recorded (for diff construction)
- Code ready for adversarial review

**Next:** Code review initiated

---

### 4. Code Review

**Mandatory Process:**

1. **Trigger:** Story reaches `review` status
2. **Who:** bmad-code-review (independent review agent)
3. **Model requirement:** **DIFFERENT model than build agent** (Amelia)
   - Build: Claude Haiku (or assigned dev model)
   - Review: Claude Opus (or other model with different training/perspective)
4. **Scope:** All code, tests, configuration, documentation per spec
5. **Findings:** Markdown list of issues (blind review + edge-case + verification-gap layers)
6. **Output:** Story file updated with review findings and remediation

**Findings resolution:**

- **Blocking issues** (breaks acceptance criteria, security risk):
  - Story moves back to `in-progress`
  - Amelia addresses findings
  - Re-submit to review
- **Non-blocking issues** (style, optimizations, follow-ups):
  - Amelia files follow-up story or notes decision in story file
  - Story proceeds to `done`

---

### 5. Done
- All AC satisfied
- All tests passing
- Security review completed
- **Code review completed and findings addressed**
- Dev Agent Record signed off
- Sprint status updated to `done`

---

## Model-Swap Requirement

**Why:** Different models catch different issues. Blind review with a different training set reduces groupthink.

**Implementation:**

- **Build:** Amelia is the assigned dev (default model per skill config)
- **Code Review:** Always use a different model from the build agent
  - If Amelia built it: use Claude Opus or GPT-5.4 for review
  - If Amelia built it: explicitly call bmad-code-review with `--model <different-model>`
- **Document in story:** Record both models in Dev Agent Record

**Current sprint (1.0 baseline):**
- Build: Claude Haiku 4.5 (Amelia default)
- Review: Claude Opus 4.8 (different model)

---

## Sprint-Status Workflow

Story progression updates `sprint-status.yaml`:

| Status | Meaning | Who sets | Next |
|--------|---------|----------|------|
| `ready-for-dev` | Planning complete, ready to pick up | Sprint planning | Dev picks → `in-progress` |
| `in-progress` | Dev actively building | Dev (Amelia) | Dev marks → `review` |
| `review` | Code complete, awaiting code review | Dev (Amelia) | Review agent runs; status updates after |
| `code-review-findings` | Review found blocking issues | Code review | Dev addresses → back to `in-progress` |
| `done` | All AC + tests + review complete | Dev or review agent | Epic rolls up |

*Note:* Sprint planning logic may auto-transition states based on story file status frontmatter. Keep sync'd.

---

## Cross-functional Handoff Notes

### Dev → Code Review

When moving story to `review`:

1. Update story file frontmatter: `status: in-review`
2. Record `baseline_commit` (the commit hash before this story)
3. Ensure all local tests passing
4. Push or mark changes ready for review

Code review agent reads:
- Story spec (acceptance criteria, reasoning, security review)
- Diff from `baseline_commit` to current HEAD
- Spec context files (architecture, planning decisions)

### Code Review → Dev (if rework needed)

Review agent updates story file with findings and moves to `code-review-findings` status.

Dev:
1. Reads findings in story file
2. Addresses blocking issues
3. Moves back to `in-progress`
4. Repeats until code review passes

### Code Review → Done (if approved)

Review agent marks story `status: done` and syncs sprint-status.yaml.

---

## Manual Verification

Before a story can be marked `done`, Thomas (or delegate) walks the manual QA checklist from the story file.

- Happy path: ✅
- Error case: ✅
- Regression spot-check: ✅
- No regressions introduced: ✅

Only then does manual QA sign-off happen.

---

## CI Signal

- GitHub Actions runs on push: `dotnet build`, `dotnet test`, `npm run build`, linting
- CI must be green for story to move to `done`
- Code review findings that fail CI are prioritized

---

## Metrics & Cadence

- **Review time:** Target < 2 hours between story marked `review` and findings delivered
- **Rework:** Blocking issues should be rare (< 5% of stories). If frequent, review process or story definition is weak.
- **Story duration:** Varies by scope; target 1–3 days per story for Phase 1 scaffold

---

## Customization & Overrides

- Override code review layer (e.g., skip edge-case for simple stories): document in story file + get Thomas approval
- Override model swap (e.g., use same model for paired review): document + note in Dev Agent Record
- Add new review layers (e.g., performance, accessibility): update this doc and notify team

---

**Owner:** Thomas  
**Last updated:** 2026-08-21
