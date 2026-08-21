# Sprint Change Proposal — SME Meeting 01 Direction Change

| Field | Value |
|-------|-------|
| **Project** | ContractorPro |
| **Date** | 2026-08-20 |
| **Author** | Correct-course workflow (batch) |
| **Trigger** | SME Meeting 01 + architect session (Winston) |
| **Sources** | [decision-workbook.md](../sme-meetings/decision-workbook.md), [company-number-messaging.md](./technical-exploration/company-number-messaging.md) |
| **Scope classification** | **Major** |
| **Status** | **Approved** (Thomas, 2026-08-20) |

---

## Section 1: Issue Summary

### Problem statement

Planning artifacts (PRD v0.1, epics, architecture §1.7) assume **per-project Twilio handle numbers** and **native group MMS** (GC personal phone + handle # + sub/customer). Paying-customer SMEs (Ryan & Maci) rejected that model in SME Meeting 01: subs won't stay in per-project threads, and the GC team needs **one company number** with shared visibility (ReviewWave-style).

Parallel decisions from the SME workbook expand MVP scope: **plan-first job planning** (was v0.2), **Google Drive-backed file share**, **QR check-in/out**, **automated customer milestone comms**, and updated **auth/notify** rules.

Winston has documented the replacement architecture in [company-number-messaging.md](./technical-exploration/company-number-messaging.md). **PRD, epics, user journeys, UX mockups, and product-vision are not yet aligned.**

### Discovery context

| When | What |
|------|------|
| 2026-08-20 | SME lunch (Pocket transcript) — phone model tension, plan-first, QR/Drive |
| 2026-08-20 | Thomas + workbook sessions — decisions #0–#9 (2B deferred) |
| 2026-08-20 | Thomas + Winston — company # + SMS relay Pattern A |

### Evidence

- Ryan: per-project groups "clunky"; subs call personal cells; wants central company #
- Maci: needs visibility when Ryan handles something she didn't know about
- Buildertrend pain: "send all or nothing" sub invites → configurable cascade (2A)
- Architect doc supersedes [project-handle-numbers.md](./technical-exploration/project-handle-numbers.md)

---

## Section 2: Impact Analysis

### Epic impact

| Epic | Impact | Action |
|------|--------|--------|
| **E1** Billing/onboarding | Company # provision trigger TBD (1b); sandbox unchanged | Modify E1-S5 notes; add provision story |
| **E2** Projects & tasks | Remove JIT handle on create; add `planning` status | Modify E2-S1; **add E2 planning stories** |
| **E3** Calendar | Plan mode: no Google writes until finalize/sub accept | Modify E3 stories; tie to planning epic |
| **E4** Invite/join | `notify_via`; QR first-touch bind | Modify E4-S1/S2 |
| **E5** Schedule confirm | Magic-link only; cascade waves from plan finalize | Modify E5; add cascade invite stories |
| **E6** Poke | Sends from company # | Modify acceptance criteria |
| **E7** Cascade | Unchanged (2C); replan loop from planning | Clarify coupling to planning |
| **E8** **MMS group threads** | **Obsolete as written** | **Replace with E8 Company comms + inbox + relay** |
| **E9** Customer visibility | + milestone auto-send; prelim publish | Expand E9; **add E13 or E9 stories** |
| **E10** Identity | Trusted device 30d + OTP | Modify E10 / join stories |
| **E11** AI | Unchanged deferred | N/A |
| **E12** Admin | STOP on company #; orphan queue | Modify references |
| **NEW E13** Job planning | Was v0.2 | **New epic** — templates, plan UI, prelim, finalize |
| **NEW E14** Project resources | Drive + QR | **New epic** — folder, portal, check-in/out |
| **NEW E15** Milestone comms | Automated customer prep | **New epic** or fold into E9 |

### Story impact summary

- **Retire / rewrite:** E8-S1, E8-S2, E8-S3 (group MMS), E8-S4 (per-project pool), E8-S5 (reuse — defer or repurpose)
- **Modify:** E2-S1, E4-*, E5-*, E6-*, E9-*, build phasing table, milestones M3/M3a
- **Add:** ~15–25 new stories across planning, comms relay, Drive, QR, milestones, cascade templates

### Artifact conflicts

| Artifact | Conflict | Update needed |
|----------|----------|---------------|
| **prd.md** | FR-14 group MMS; handle # glossary; job planning v0.2; no milestone auto-send | Major §3–§6 rewrite |
| **epics-and-stories.md** | E8 entire epic; missing planning/resources epics | Restructure |
| **architecture-v0.1.md** | §1.7 per-project pool (partially updated?) | Align with company-number-messaging.md |
| **project-handle-numbers.md** | Superseded | Archive banner → point to company-number-messaging.md |
| **job-planning-workflow.md** | Status exploratory v0.2 | Promote to MVP; add prelim/finalize steps |
| **product-vision.md** | v0.2 job planning; per-project messaging | Update wedge + MVP table |
| **User journeys** | C-12 group MMS, handle # throughout | Rewrite comms journeys; add planning flow |
| **UX mockups** | Group MMS setup screens | Replace with inbox + planning mode + resource page |
| **finances/monthly-run-rate.md** | ~$10/project telco | ~$1.15/mo + volume |
| **sprint-status.yaml** | Epic/story IDs if renumbered | Update after approval |

### Technical impact

- Twilio: Programmable Messaging only; drop Conversations/group MMS
- New workers: `StaffSmsRouter`, `InboundRouter`, `InboundMediaIngestWorker`
- New tables: `contractor_phone_numbers`, `comm_threads`, `staff_sms_sessions`
- Google Drive API (OAuth scope expansion)
- Scheduled worker for milestone comms
- No code rollback required — planning-phase change before implementation

---

## Section 3: Recommended Approach

### Selected path: **Hybrid — Direct Adjustment + MVP scope expansion (Option 1 + Option 3)**

| Option | Viable? | Notes |
|--------|---------|-------|
| Direct adjustment | ✅ | Primary — rewrite affected stories/epics |
| Rollback | ❌ | No shipped code to revert |
| MVP scope reduction | ❌ | Opposite — SMEs expanded MVP (planning, Drive, milestones) |

### Rationale

- SMEs and architect have **already decided**; delay creates drift between `company-number-messaging.md` and PRD
- Per-project model is **not salvageable** with SME buy-in — must replace, not patch
- Job planning promotion is **core to Ryan's workflow**, not optional polish
- Deferred items (2B, 1b, 1B validation, #9) use **stubs/flags** so build can proceed

### Effort & risk

| Dimension | Assessment |
|-----------|------------|
| **Effort** | High — 2–3 doc sprints before dev parity |
| **Timeline** | +4–8 weeks MVP vs prior plan (planning + Drive + relay) |
| **Risk** | Medium — SMS relay adoption (Ryan behavior change); Drive API |
| **Mitigation** | SME validation session (1B); onboarding copy; Winston Drive spike |

---

## Section 4: Detailed Change Proposals

### 4.1 PRD (`prd.md`)

#### Glossary — Project handle #

**OLD:**
> **Project handle #** — Dedicated phone number per **Project** for MMS routing…

**NEW:**
> **Company number** — One Twilio 10DLC number per **Contractor subscription**. All subs/customers text this number. Staff coordinate via SMS relay + app inbox. Outbound system messages include project prefix. See [company-number-messaging.md](../technical-exploration/company-number-messaging.md).

**Rationale:** Core model change.

---

#### FR-14 (replace entirely)

**OLD:** FR-14: MMS group thread per relationship — project handle #, group MMS ingest…

**NEW:** FR-14: Company number messaging & shared inbox

- One company number per contractor (provision timing: **TBD** — interim: first paid/comms enabled)
- External participants text company # only
- Staff SMS relay: inbound fan-out to team phones from company #; staff reply **to** company # (lenient ref token routing)
- App inbox: shared threads per `(person, project, audience)`; orphan queue + manual project assign
- Hybrid logging: all company # traffic logged; outbound project-tagged; inbound best-effort
- Schedule accept/decline: **magic links only** (not SMS YES/NO)
- Personal cell traffic: out of scope

**Rationale:** Winston architecture + workbook #0/#1.

---

#### New FR-20: Job planning & finalize (promote from v0.2)

- Project status `planning` → `active`
- Templates: phases, durations, buffers, cascade config (parallel/sequential)
- Contract constraints (blackouts, access notes)
- Overlay vs existing calendar / jobs
- **Publish prelim to customer** (separate action)
- **Finalize & start sub cascade** (separate action; 2B gate optional/off by default)
- No external notify until finalize

**Rationale:** Workbook #3, #2A.

---

#### New FR-21: Configurable approval cascade

- Template-driven invite waves after finalize
- Sequential gates + parallel waves configurable at template and runtime
- Sub confirm → optional auto-invite next wave

**Rationale:** Workbook #2A; Buildertrend pain.

---

#### New FR-22: Project resources (Drive + QR)

- Google Drive folder per project (app-managed metadata; files in Drive)
- Sub/customer interact via app portal only (not drive.google.com)
- QR → app resource page; check-in (phone→sub bind); check-out (photo/note upload to Drive)

**Rationale:** Workbook #4, #5.

---

#### New FR-23: Customer milestone comms

- Automated SMS/email N days before customer-visible milestones
- N = contractor setting (company default)
- Prep message templates per milestone type
- Respects `notify_via`

**Rationale:** Workbook #7.

---

#### FR participant auth (update invite/join section)

- Trusted device 30 days; OTP on new device or expired trust
- Same for subs and customers
- QR return: phone+sub recognition sufficient

**Rationale:** Workbook #6.

---

#### Out of scope table

**OLD:** Full job planning module — **v0.2**

**NEW:** Remove job planning from deferred list. Add: AI inbound project tagging, Twilio port-out (#9 research), customer approval gate rules (2B SME pending).

---

### 4.2 Epics (`epics-and-stories.md`)

#### Replace Epic E8 title and stories

**OLD:** Epic E8 — MMS Group Threads & Photos

**NEW:** Epic E8 — Company Number, SMS Relay & Inbox

| Story | Summary |
|-------|---------|
| E8-S1 | Provision company number (trigger TBD; interim first paid/comms) |
| E8-S2 | Inbound webhook + thread routing + orphan queue |
| E8-S3 | Staff SMS relay (fan-out, lenient token, disambiguation) |
| E8-S4 | App shared inbox (reply, claim, filters) |
| E8-S5 | Inbound MMS ingest to blob (staff relay MMS outbound deferred) |
| E8-S6 | Platform STOP/opt-out on company # (move from E6-S5 overlap — clarify) |

**Retire:** Old E8-S1–S5 group MMS / per-project pool stories.

---

#### New Epic E13 — Job Planning & Finalize — **MVP**

| Story | Summary |
|-------|---------|
| E13-S1 | Project templates (phases, deps, cascade defaults) |
| E13-S2 | Planning workspace UI (durations, buffers, overlay) |
| E13-S3 | Contract/customer constraints at setup |
| E13-S4 | Publish prelim to customer |
| E13-S5 | Finalize plan → trigger sub cascade |
| E13-S6 | Runtime cascade override (parallel/sequential waves) |

**Optional gate:** E13-S4 includes `customer_gate_enabled` flag — default **false** until 2B decided.

---

#### New Epic E14 — Project Resources & QR — **MVP**

| Story | Summary |
|-------|---------|
| E14-S1 | Google Drive OAuth + project folder link/create |
| E14-S2 | Doc list in app + portal file view (Drive proxy) |
| E14-S3 | Printable QR sheet → app resource page |
| E14-S4 | QR check-in (phone→sub bind) + check-out upload |

---

#### New Epic E15 — Customer Milestone Comms — **MVP**

| Story | Summary |
|-------|---------|
| E15-S1 | Contractor setting: days-before default |
| E15-S2 | Milestone prep templates on phases |
| E15-S3 | Scheduled send worker |

---

#### Modify E2-S1

**OLD:** Project create triggers handle assignment via E8-S4

**NEW:** Project create sets status `planning`; no telco action. Company # exists at contractor level (E8-S1).

---

#### Modify E4-S1

**ADD:** Participant selects `notify_via` (sms | email | both) at join; editable in portal.

---

#### Modify build phasing table

**OLD:** M3 Create project + JIT handle #

**NEW:** M3 Create project (planning mode); M3a Provision company #; M3b Planning template + prelim; M4 Finalize + cascade…

(Re-sequence milestones in full epic doc edit.)

---

### 4.3 Architecture

**Action:** Mark [project-handle-numbers.md](./technical-exploration/project-handle-numbers.md) **SUPERSEDED** — link to [company-number-messaging.md](./technical-exploration/company-number-messaging.md).

**Action:** Update [architecture-v0.1.md](./architecture-v0.1.md) §1.7, §5.5 telephony ERD, workers list to match company-number-messaging.md (if not fully done).

**Action:** Add §1.x Google Drive integration pointer (E14) — Winston spike doc TBD.

---

### 4.4 Product vision & job-planning-workflow

**product-vision.md:**
- Move job planning from v0.2 table → MVP
- Replace per-project handle messaging with company number + inbox
- Add QR/resources bullet

**job-planning-workflow.md:**
- Status: **MVP (promoted 2026-08-20)**
- Add explicit **Publish prelim** and **Finalize** steps before sub cascade
- Note 2B customer gate optional

---

### 4.5 User journeys (high level)

| File | Change |
|------|--------|
| contractor-journeys.md | Remove C-12 group MMS setup; add planning + inbox flows |
| subcontractor-journeys.md | Text company #; magic link confirm; QR check-in |
| customer-journeys.md | Prelim view; milestone comms; no group MMS |
| user-journeys.md UJ-8 | Rewrite two-lane comms model |
| backlog.md | Close/retire BL-13 MMS before handle; update SP-1/SP-2 |

---

### 4.6 UX mockups

| Screen | Action |
|--------|--------|
| Group MMS / handle setup | **Remove** or archive |
| **Add** | Planning workspace, prelim publish, finalize, inbox, orphan assign, resource/QR page |
| system-overview.html | Update comms diagram to company # relay |

---

### 4.7 Finances

**monthly-run-rate.md:** Replace ~$10/project number line with ~$1.15/mo + SMS/MMS volume per contractor.

---

### 4.8 Deferred — do NOT implement in artifact edits yet

| Item | Stub in artifacts |
|------|-------------------|
| **2B** Customer approval gate | `customer_gate_enabled` default false; note SME pending |
| **1b** Provision timing | TBD; interim first paid/comms in architecture only |
| **1B** SMS relay SME validation | Reference follow-up doc; no PRD change |
| **#9** Twilio portability | Research note in discovery-log |

---

## Section 5: Implementation Handoff

### Scope: **Major**

| Role | Responsibility |
|------|----------------|
| **Thomas (PM)** | Approve this proposal; schedule SME 2B + 1b + 1B |
| **Winston (Architect)** | Drive API spike; confirm architecture-v0.1 fully synced |
| **PM/Dev agent** | Apply approved edits to PRD, epics, journeys, vision |
| **UX** | Planning mode + inbox + resource page mockups |

### Sequencing (post-approval)

```text
1. Apply PRD + epic restructure (this proposal)
2. Update architecture + archive project-handle-numbers
3. Update user journeys + UX pack (parallel)
4. Update sprint-status.yaml
5. SME sessions: 2B, 1b, 1B (non-blocking for doc merge)
6. Dev: E8 relay first → E13 planning → E14 resources
```

### Success criteria

- [ ] No PRD/epic reference to per-project handle or group MMS as MVP
- [ ] FR-14 matches company-number-messaging.md
- [ ] Job planning in MVP scope with prelim + finalize buttons
- [ ] New epics E13–E15 (or equivalent) in epics-and-stories.md
- [ ] sprint-status.yaml reflects epic changes
- [ ] decision-workbook propagation checklist marked complete

---

## Section 6: Checklist completion (batch)

| Section | Status |
|---------|--------|
| 1 Trigger & context | [x] Done |
| 2 Epic impact | [x] Done |
| 3 Artifact conflicts | [x] Done |
| 4 Path forward | [x] Done — Hybrid Option 1+3 |
| 5 Proposal components | [x] Done |
| 6 User approval | [x] Approved — thin spots → backlog RC-1–RC-7 |
| 6.4 sprint-status.yaml | [x] Updated |
| 6.5 Handoff | [x] Artifact propagation in progress |

---

## Approval

**Thomas:** Approved 2026-08-20. Thin spots routed to backlog RC-1–RC-7. Artifact propagation applied to PRD, epics, product-vision, job-planning-workflow, sprint-status.yaml.

---

*Generated by bmad-correct-course (batch mode), 2026-08-20.*
