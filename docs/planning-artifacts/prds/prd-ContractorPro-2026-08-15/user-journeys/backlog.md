# User Journeys — Backlog & Discovery

Journeys drafted but **not ready to build** without more discovery, SME validation, or explicit v0.2 scope lock.

Related: [README.md](./README.md) · [discovery-log.md](../../../discovery-log.md)

---

## Team roles (post-POC — not v0.1)

| Future role | Intent | v0.1 today |
|-------------|--------|------------|
| **Contractor-admin** (Ryan) | Subscription, billing, company settings, destructive actions | Ryan and Maci share all capabilities |
| **Contractor-office** (Maci) | Projects, customers, subs, scheduling, messaging | Same login powers as Ryan |

**v0.1 rule:** Maci journeys ([contractor-journeys.md](./contractor-journeys.md) § Maci) describe **typical task focus**, not permission differences.

---

## Backlog by build bucket (2026-08-20)

Decided in [planning-decision-checklist.md](../../../planning-decision-checklist.md) §B.

### MVP Phase 1 — build rules locked

| ID | Topic | MVP behavior |
|----|-------|--------------|
| **BL-1** | Customer channel gating | Gate schedule MMS until `phone_confirmed`; gate email until `email_confirmed` |
| **BL-3** | Family dual-channel confirm | **MMS-only** for family invite (Erin) — not full dual-channel |
| **BL-5** | Batch cascade confirm (subs) | **One batched SMS** per sub per project when cascade moves multiple tasks |
| **BL-6** | Batch cascade confirm (customer) | **One digest** per cascade event |
| **BL-7** | Quiet hours | **Company default** 8pm–8am local; queue SMS |
| **BL-8** | `notify_via` per sub | **GC sets at invite**; editable by GC |
| ~~**BL-13**~~ | ~~MMS before handle ready~~ | **Retired 2026-08-20** — company # model; no per-project handle |
| ~~**BL-14**~~ | ~~Unified inbox mirror-only~~ | **Superseded 2026-08-20** — app inbox + SMS relay (E8) |
| **BL-18** | Courtesy SMS on reassignment | **Auto-send** when sub replaced on task |

### v0.1.1

| ID | Topic | Notes |
|----|-------|-------|
| **BL-2** | Customer milestone filter | Which tasks customer-visible |
| **BL-4** | Returning customer fast path | Skip dual confirm if verified before |
| **BL-9** | Poke Ryan on sub-request | Auto-poke GC at 48h pending |
| **BL-11** | Partial cascade | Full cascade only until then |
| **BL-12** | Business days / holidays | Calendar days in MVP |
| **BL-15** | Project photo timeline | Cross-thread photo feed |
| ~~**BL-23**~~ | ~~Handle # reuse~~ | **Retired 2026-08-20** — one company # per contractor; no per-project pool |

### v0.2+

| ID | Topic | Notes |
|----|-------|-------|
| **BL-10** | Draft schedule mode | FJ-1 plan mode |
| **BL-17** | Sub "my jobs" portal | FJ-4 |

### When hired (ops policy)

| ID | Topic |
|----|-------|
| **BL-19** | Platform admin role split (Alex vs Thomas) |

### Previously decided

| ID | Decision |
|----|----------|
| ~~**BL-16**~~ | Stripe Phase 2; MVP no gates |
| ~~**BL-20–22**~~ | Admin impersonation, STOP scope, kill switch |

---

## Legacy reference (original open questions)

| ID | Topic | Journey ref | Original question |
|----|-------|-------------|-------------------|
| **BL-1** | Customer channel gating | H-23, UJ-3b | Block schedule MMS until `phone_confirmed`? Block email digests until `email_confirmed`? |
| **BL-2** | Customer milestone filter | H-22 | Which tasks are customer-visible vs internal-only? Who marks them? |
| **BL-3** | Family dual-channel confirm | H-7, H-8 | Same email+MMS+poke as primary, or MMS-only for Erin? |
| **BL-4** | Returning customer fast path | H-24, H-5 | Skip dual channel confirm if both channels verified on prior project? |
| **BL-5** | Batch cascade confirm (subs) | S-18, C-12 | One SMS with all task moves vs one per assignment? |
| **BL-6** | Batch cascade confirm (customer) | H-15 | One digest vs per-milestone MMS? |
| **BL-7** | Quiet hours | S-19 | Company default vs per-participant? Timezone source? |
| **BL-8** | `notify_via` per sub | S-16 | Set at invite, editable by sub, or GC-only? |
| **BL-9** | Poke Ryan on sub-request | C-23 | Auto-poke GC if sub reschedule request pending 48h? |
| **BL-10** | Draft schedule mode | C-21 | Per-project toggle or per-edit? How long can draft sit? |
| **BL-11** | Partial cascade | C-20 | Move only selected dependents? Fixed duration vs end date? |
| **BL-12** | Business days / holidays | C-20 | Calendar days vs business days for cascade delta? GC blackout dates? |
| **BL-13** | MMS ingest before handle ready | C-13 | Queue messages, warn Ryan, or drop? |
| **BL-14** | Unified inbox (MMS + app) | C-14, H-17 | Single thread view or mirror-only? |
| **BL-15** | Project photo timeline | C-26 | Chronological all-project photos across threads — MVP or v0.1.1? |
| ~~**BL-16**~~ | ~~Billing / trial limits~~ | C-27 | **Decided 2026-08-19:** Stripe Billing; sandbox = plan-only (Phase 2); paid ~$100/5 concurrent active projects; **MVP = no billing/gates** |
| **BL-17** | Sub "my jobs" landing | S-17 | v0.1 per-link only; when does unified portal ship? |
| **BL-18** | Courtesy SMS on reassignment | S-20, UJ-2e | Auto-send when Nate replaces Jesse? |
| **BL-19** | Platform admin role split | A-* | Super-admin vs support ops — what can Alex do without Thomas? |
| ~~**BL-20**~~ | ~~Admin impersonation~~ | A-9, A-1 | **Decided 2026-08-19:** v0.1 drill-down only; A-9 deferred v0.1.1 |
| ~~**BL-21**~~ | ~~SMS opt-out scope~~ | A-11 | **Decided 2026-08-19:** platform-global STOP + re-opt-in via link/START/admin audit |
| ~~**BL-22**~~ | ~~Kill switch granularity~~ | A-10, A-6 | **Decided 2026-08-19:** platform kill + per-tenant suspend; per-project v0.2 |
| **BL-23** | Handle # same-company reuse | E8-S5, C-25 | **Decided 2026-08-20:** deferred **v0.1.1** — MVP always JIT fresh; cooling → release. Spike: history routing |

---

## Engineering spikes (post-MVP)

| ID | Spike | Delivers | Prereq in MVP | Ref |
|----|-------|----------|---------------|-----|
| ~~**SP-1**~~ | ~~Number reuse + history routing~~ | **Retired 2026-08-20** — per-project handle model removed |
| ~~**SP-2**~~ | ~~Twilio vs Telnyx group MMS E2E~~ | **Retired 2026-08-20** — Programmable Messaging 1:1 only; see [company-number-messaging.md](../../../technical-exploration/company-number-messaging.md) |
| **SP-3** | Google Drive API + QR resource page | E14 prereq | OAuth scopes, folder create, portal proxy, preview strategy | [decision-workbook.md](../../../sme-meetings/decision-workbook.md) §4 |
| **SP-4** | SMS relay SME validation | Pre-build gate | Scripted flows with Ryan/Macie | [company-number-sms-relay.md](../../../sme-meetings/sme-follow-ups/company-number-sms-relay.md) |

**SP-1 acceptance (spike output):** sequence diagram for Marcus texting old Maple # after number reassigned to Oak; test cases; decision on reuse gate vs pure history routing.

---

## Roadmap — post correct-course thin spots (2026-08-20)

Approved in [sprint-change-proposal-2026-08-20.md](../../../sprint-change-proposal-2026-08-20.md). Decisions locked in [decision-workbook.md](../../../sme-meetings/decision-workbook.md); items below need **more discussion or UX detail** before build lock.

| ID | Topic | Bucket | Notes |
|----|-------|--------|-------|
| **RC-1** | Single QR vs two QRs (check-in / check-out) | MVP UX | Workbook intent: one laminated QR → resource page with Check in + Upload actions; validate with Ryan |
| **RC-2** | Milestone comms vs date-change notifications | MVP copy/rules | Automated prep messages (FR-24) distinct from event-driven schedule-change alerts (FR-16); template boundaries TBD |
| **RC-3** | Default `notify_via` at join | MVP defaults | Workbook: subs SMS, customers both — confirm editable-by-participant vs GC-only (BL-8 overlap) |
| **RC-4** | GC staff notification defaults | MVP settings | Email + in-app for Ryan/Macie — separate from participant `notify_via`; relay SMS alert rules already locked |
| **RC-5** | SMS relay implementation tuning | v0.1 polish | Staff STOP allowlist; 72h open-thread TTL; polling 60s vs SignalR; staff MMS outbound deferred — see [company-number-messaging.md](../../../technical-exploration/company-number-messaging.md) |
| **RC-6** | Planning stretch features | v0.1.1 | Reverse-schedule from anchor trade; multi-job portfolio balance panel (FJ-2) |
| **RC-7** | Secondary doc sync | Pre-dev | Align [messaging-and-media.md](../../../technical-exploration/messaging-and-media.md), [invite-join-flow.md](../../../technical-exploration/invite-join-flow.md), [schedule-confirmation-workflow.md](../../../technical-exploration/schedule-confirmation-workflow.md) with company # + plan-first model |

**Also deferred to SME follow-up (not RC):** 2B customer gate, 1b company # provision timing, 1B SMS relay validation, #9 Twilio portability research.

---

## v0.2+ future journeys (drafted — not in v0.1 scope)

Documented in [future-journeys-v02.md](./future-journeys-v02.md):

| ID | Journey | Persona | Notes |
|----|---------|---------|-------|
| ~~**FJ-1**~~ | ~~Plan mode → Finalize schedule~~ | Ryan / Maci | **Promoted to MVP** (2026-08-20 correct-course) — Epic E13 |
| **FJ-2** | Portfolio balance / sub conflict across jobs | Ryan | RC-6 v0.1.1 |
| **FJ-3** | AI draft "what changed" on schedule shift | Ryan |
| **FJ-4** | Unified person portal (all projects, one phone) | Jesse / Lauren |
| **FJ-5** | Project template (kitchen remodel phases) | Ryan / Maci |
| **FJ-6** | Role-based permissions (admin vs office) | Ryan / Maci |

---

## Competitive / wedge journeys (documented in v0.1 files)

These are **intentionally in scope** for differentiation — see contractor C-19–C-25, customer H-21–H-24, UJ-9:

- Portfolio triage home screen (vs BT dashboard sprawl)
- Event-triggered customer updates (vs BT weekly AI digest)
- Persistence / poke layer (vs passive iCal)
- Google attendee invites on accept (vs BT one-way iCal); Apple CalDAV v0.1.1

Log decisions in [discovery-log.md](../../../discovery-log.md).
