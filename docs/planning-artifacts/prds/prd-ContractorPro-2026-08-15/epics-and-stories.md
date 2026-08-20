---
title: ContractorPro v0.1 — Epics & User Stories
created: 2026-08-15
updated: 2026-08-20
status: draft
source_prd: prd.md
---

# ContractorPro v0.1 — Epics & User Stories

Derived from [prd.md](./prd.md). Product-facing stories only — implementation detail belongs in architecture/TRD.

**Actors:** **Team member** (Contractor subscription), **Subcontractor** (project role), **Customer** (project role).

**Story ID format:** `E{epic}-S{story}` · maps to `FR-N` where noted.

## Build phasing

| Phase | Goal | Billing |
|-------|------|---------|
| **MVP (Phase 1)** | Self-enrolling Contractors run real jobs end-to-end | **None** — all features open for beta |
| **Post-MVP (Phase 2)** | Monetize + telco-aligned gates | **Stripe Billing** — sandbox vs paid tiers (FR-18) |

**MVP success:** Ryan self-serves signup → first project → first sub confirmed — without Thomas provisioning anything.

---

## Epic E1 — Contractor Onboarding, Auth & Billing

*Team members self-serve into a Contractor workspace; billing gates ship Phase 2.*

### E1-S1: Self-serve sign in with OAuth — **MVP**

**As a** new contractor owner, **I can** sign in with **Google OAuth** **so that** I get a Contractor workspace without manual provisioning.

**Acceptance:**
- [ ] Google OAuth sign-in on empty account → creates **Contractor company** + first team member as owner
- [ ] Google OAuth on existing account → links to existing Contractor subscription
- [ ] Unauthenticated users cannot reach project management routes
- [ ] Session persists across browser restarts (secure cookie via BFF)
- [ ] No payment or invite code required in MVP
- [ ] Apple/Microsoft OAuth **not** in MVP (v0.1.1)

**FR:** FR-1 · **Journey:** C-1 step 1

---

### E1-S4: Guided onboarding checklist — **MVP**

**As a** new team member, **I can** follow an in-app checklist **so that** I reach first value in one session (~20–30 min).

**Acceptance:**
- [ ] Checklist steps: connect calendar (optional skip) → create first project → add tasks → invite first sub OR add customer → propose first date
- [ ] Progress persisted; dismissible after complete
- [ ] Empty dashboard shows checklist, not blank state
- [ ] Completing checklist satisfies SM-1 onboarding metric

**FR:** FR-2, FR-3, FR-4, FR-7 · **Journey:** C-1

---

### E1-S2: Sign in with native account (fallback) — **Should / v0.1.1**

**As a** team member without a preferred OAuth provider, **I can** use a native account with passkey or TOTP **so that** I can still access ContractorPro.

**Acceptance:**
- [ ] Passkey/TOTP offered; password-only is fallback
- [ ] Same Contractor subscription binding as OAuth

**FR:** FR-1 · **Priority:** Should (defer if OAuth-only acceptable for MVP)

---

### E1-S3: Subscribe via Stripe Billing — **Post-MVP (Phase 2)**

**As a** Contractor subscription owner, **I can** choose a paid plan and pay **so that** I unlock outbound coordination and concurrent active project slots.

**Acceptance:**
- [ ] Stripe Checkout from in-app upgrade prompt (C-27)
- [ ] **Products/prices:** Pro 5 $100/mo · Pro 10 $200/mo · linear +5 slots per +$100
- [ ] Webhooks sync `stripe_customer_id`, `subscription_status`, `tier`, `active_project_cap`
- [ ] Stripe Customer Portal link in Settings (update card, cancel, invoices)
- [ ] Subcontractor and Customer project memberships are never billed
- [ ] Downgrade/cancel at period end; data retained read-only per policy

**FR:** FR-18 · **Journey:** C-27, A-5

---

### E1-S5: Sandbox vs paid entitlements — **Post-MVP (Phase 2)**

**As the** system, **I must** enforce tier rules **so that** free users plan without telco cost and paid users get comms within their cap.

**Acceptance:**
- [ ] Default new signup → **sandbox** tier (MVP override: `billing_enforcement=off` env flag grants full access)
- [ ] **Sandbox blocks** (server-side): sub invite, customer outbound notify, propose-notify, poke send, cascade publish, MMS/SMS send
- [ ] Customer contact saved on project does **not** trigger H-1 email/MMS until subscribed
- [ ] **Paid tier:** `comms_enabled` per project up to concurrent **active** cap; 6th project → plan-only or upgrade prompt
- [ ] Cascade preview allowed on sandbox; publish blocked
- [ ] Central entitlement service checked by API — not UI-only
- [ ] Upgrade modal copy: *"Subscribe to invite subs and notify customers"*

**FR:** FR-18 · **Journey:** C-27

---

### E1-S6: Dunning and messaging suspend — **Post-MVP (Phase 2)**

**As the** system, **when** Stripe reports failed payment, **I must** degrade gracefully **so that** telco isn't burned on delinquent accounts.

**Acceptance:**
- [ ] `invoice.payment_failed` → in-app banner + email to owner
- [ ] Grace period (default 14 days) then `messaging_suspended` on tenant (A-6)
- [ ] Suspended tenant: outbound SMS/MMS blocked; in-app read-only scheduling OK
- [ ] Payment restored → auto-lift suspend via webhook

**FR:** FR-18 · **Journey:** A-6, A-17, C-27

---

## Epic E2 — Projects & Tasks

*Team members organize work on jobs.*

### E2-S1: Create a project

**As a** team member, **I can** create a Project (name, address, basic details) **so that** I have a container for schedule and comms.

**Acceptance:**
- [ ] Project belongs to my Contractor subscription only
- [ ] Subcontractor/Customer memberships cannot create projects
- [ ] Project create triggers handle assignment via **E8-S4** (JIT from company pool)

**FR:** FR-2, FR-14, FR-20

---

### E2-S2: Add and edit tasks

**As a** team member, **I can** add Tasks with dates (single-day or range) **so that** I can build a job schedule.

**Acceptance:**
- [ ] Task list/timeline visible per project
- [ ] Tasks can exist without a Subcontractor assignment (Contractor-only milestones)

**FR:** FR-2

---

### E2-S3: Task dependencies

**As a** team member, **I can** mark tasks as dependent on other tasks **so that** cascade can shift the chain later.

**Acceptance:**
- [ ] Dependency is directional (predecessor → successor)
- [ ] Circular dependencies prevented or warned

**FR:** FR-2 · **Feeds:** E6

---

### E2-S4: Enable cascade per project

**As a** team member, **I can** turn cascade on or off for a project **so that** I control whether slips ripple automatically.

**Acceptance:**
- [ ] Toggle stored per project; default documented
- [ ] When off, moving a task does not auto-shift dependents

**FR:** FR-13

---

## Epic E3 — Google Calendar Connection

*Agreed dates appear in Google after Subcontractor confirmation.*

### E3-S1: Connect Google Calendar

**As a** team member, **I can** connect our Contractor's Google account **so that** confirmed dates sync to a shared calendar.

**Acceptance:**
- [ ] Connection status shown: connected / disconnected / error
- [ ] Reconnect flow if token expires

**FR:** FR-3

---

### E3-S2: Pro-provided project calendar — **MVP**

**As a** team member, **when** I create a project, **ContractorPro creates a Google calendar** under our connected account **so that** agreed dates sync in one place per job.

**Acceptance:**
- [ ] On project create: `calendars.insert` under GC Google OAuth (pro-provided)
- [ ] One shared calendar per project
- [ ] No calendar write until Task Assignment is **confirmed**
- [ ] On confirm: event on project calendar + **attendee invite** to sub/customer email when on file

**FR:** FR-3, FR-8 · **Decision:** 2026-08-20 §A-2, C-4

---

### E3-S3: Portfolio calendar view — **MVP**

**As a** team member, **I can** view all active projects on one calendar in the app **so that** I see cross-job conflicts without switching Google calendars.

**Acceptance:**
- [ ] Unified calendar UI aggregating confirmed/proposed dates across projects
- [ ] Filter by project; default shows all active projects
- [ ] Read-only overlay of Google events per project calendar (no duplicate write path)

**FR:** FR-3 · **Decision:** C-4 · **Ref:** architecture-v0.1.md §1.6

---

## Epic E4 — Project Membership: Invite & Join

*People join a project with a role — not a global account type.*

### E4-S1: Invite Subcontractor or Customer

**As a** team member, **I can** invite someone by name, phone, role (Subcontractor or Customer), and notify preference **so that** they can join this project only.

**Acceptance:**
- [ ] SMS sent when phone provided; email when configured
- [ ] Role is Subcontractor or Customer — required
- [ ] Same phone can be invited to another project with a different role (separate membership)

**FR:** FR-4, FR-20

---

### E4-S2: Join project via magic link

**As a** person invited to a project, **I can** confirm my name and phone on one screen **so that** I access the portal without a password.

**Acceptance:**
- [ ] Join completes in under 60 seconds on mobile
- [ ] No Contractor subscription created for invitee
- [ ] Phone verification via invite token or OTP

**FR:** FR-5

---

### E4-S3: Role-scoped portal access

**As a** Subcontractor, **I see** only my tasks and Contractor↔sub thread; **as a** Customer, **I see** schedule slice and Contractor↔customer thread **so that** privacy is preserved.

**Acceptance:**
- [ ] Customer cannot see sub-only threads or internal notes
- [ ] Subcontractor cannot see customer-private thread
- [ ] Magic links scoped to project membership

**FR:** FR-6, FR-20

---

## Epic E5 — Schedule Proposal & Confirmation

*Core coordination loop.*

### E5-S1: Assign Subcontractor and propose date

**As a** team member, **I can** assign a Subcontractor membership to a task and propose a date **so that** they know when to show up.

**Acceptance:**
- [ ] Task Assignment status → `proposed`
- [ ] Notification sent per membership `notify_via` with Accept/Decline link
- [ ] No Google calendar write yet

**FR:** FR-7 · **UJ:** UJ-1

---

### E5-S2: Accept proposed date

**As a** Subcontractor on a project, **I can** tap Accept on the magic-link page **so that** the date is agreed and on my calendar.

**Acceptance:**
- [ ] Status → `confirmed`
- [ ] Shared project Google Calendar event created/updated
- [ ] Team member notified in-app

**FR:** FR-8 · **UJ:** UJ-1

---

### E5-S2b: Counter-propose a date

**As a** Subcontractor or team member, **I can** suggest a different date on a pending proposal **so that** we can negotiate without a phone call.

**Acceptance:**
- [ ] Proposed dates update; `pending_party` flips
- [ ] Negotiation history visible on confirm page and dashboard
- [ ] Other party notified; poke timer resets for new pending party
- [ ] Last confirmed calendar dates unchanged until Accept

**FR:** FR-8, FR-9 · **UJ:** UJ-2d

---

### E5-S3: Decline proposed date

**As a** Subcontractor, **I can** tap Decline **so that** the Contractor knows I cannot make that date.

**Acceptance:**
- [ ] Status → `declined` (**hard decline** — does not revert to last confirmed)
- [ ] Calendar not updated to proposed date; last confirmed event unchanged if any
- [ ] Team member notified in-app (decline alert on by default)
- [ ] Team member can reassign per E5-S3b

**FR:** FR-8 · **UJ:** UJ-2b · **Decision:** C-1

---

### E5-S3b: Reassign task after decline

**As a** team member, **I can** assign a declined task to a different Subcontractor **so that** the schedule keeps moving.

**Acceptance:**
- [ ] Declined assignment closed; poke stopped
- [ ] Confirmed calendar event removed from declined sub (if any)
- [ ] New assignment → `proposed` for replacement sub
- [ ] Assignment history shows decline + reassignment

**FR:** FR-9a · **UJ:** UJ-2e

---

### E5-S4: Reschedule with re-confirmation (either direction)

**As a** team member or Subcontractor, **I can** propose a new date on a confirmed assignment **so that** the other party must accept before calendars change.

**Acceptance:**
- [ ] Status → `proposed_change`; records `change_initiator`
- [ ] Calendars keep last **confirmed** date until re-accept
- [ ] Notification shows old → new date
- [ ] **Team member initiated:** Sub notified via SMS/email + poke per FR-11
- [ ] **Sub initiated:** Team member notified in-app (+ optional SMS); sub sees pending state

**FR:** FR-9 · **UJ:** UJ-2, UJ-2a, UJ-2c, UJ-2d

---

### E5-S5: Confirmation dashboard

**As a** team member, **I can** see pending, confirmed, and declined assignments per project and per Subcontractor **so that** I know who is holding up the schedule.

**Acceptance:**
- [ ] Filters: all / pending / confirmed / declined
- [ ] "Who's holding me up" summary panel
- [ ] Shows time since notify, reminder count, and negotiation thread when applicable

**FR:** FR-10

---

## Epic E6 — Automated Poke (Reminders)

*ContractorPro chases non-responders — not Google Calendar.*

### E6-S1: Automatic reminder schedule

**As a** team member, **I want** the system to remind Subcontractors who haven't responded **so that** I don't have to chase them manually.

**Acceptance:**
- [ ] Default cadence: initial, +24h, +48h, then daily until resolved
- [ ] Quiet hours 8pm–8am (SMS queued)
- [ ] Stops on accept, decline, snooze, reassign, archive

**FR:** FR-11 · **UJ:** UJ-1 edge

---

### E6-S2: Batched daily poke

**As a** Subcontractor with multiple pending items on one project, **I receive** one SMS listing all pending confirmations **so that** I'm not spammed.

**Acceptance:**
- [ ] Max one reminder SMS per sub per project per day (configurable)
- [ ] Single link opens batch confirm page

**FR:** FR-11

---

### E6-S3: Manual reminder and snooze

**As a** team member, **I can** send a reminder now or snooze auto-pokes **so that** I control follow-up when I've talked to the sub offline.

**Acceptance:**
- [ ] "Send reminder now" on pending assignment
- [ ] Snooze for N days; logged in audit

**FR:** FR-10, FR-11

---

### E6-S4: Escalation to team member

**As a** team member, **I am** alerted when a Subcontractor has been pending 48h+ **so that** I can call them.

**Acceptance:**
- [ ] In-app escalation badge; optional SMS to team member on decline

**FR:** FR-12

---

### E6-S5: Platform-global STOP / opt-out enforcement — **MVP (pre-prod SMS)**

**As the** system, **when** a person texts STOP (or is opted out), **I must** block all ContractorPro automated SMS/MMS to that phone **so that** we comply with TCPA and BL-21.

**Acceptance:**
- [ ] Inbound STOP/START via Twilio webhook → `persons.sms_opted_out` (platform-global)
- [ ] All outbound paths check opt-out before send: poke, propose, invite, customer notify, system MMS
- [ ] Auto-reply on STOP per A-11; START or magic-link re-consent restores
- [ ] Admin restore API (no UI M1) with audit log
- [ ] Twilio opt-out list kept in sync

**FR:** FR-14 · **Journey:** A-11 · **Decision:** A-5, BL-21 · **Depends:** E12-S1

---

## Epic E7 — Schedule Cascade — **MVP**

*Optional ripple when a task slips.*

### E7-S1: Preview cascade impact

**As a** team member, **I can** preview which tasks and Subcontractors will move before I confirm a cascade **so that** I'm not surprised by notifications.

**Acceptance:**
- [ ] Preview lists tasks, date deltas, affected Subcontractors
- [ ] Confirm / cancel before apply

**FR:** FR-13 · **UJ:** UJ-5

---

### E7-S2: Apply cascade

**As a** team member, **when** I move a task with cascade enabled, **dependent tasks shift** by the same delta and affected Subcontractors get new proposals.

**Acceptance:**
- [ ] Dependent proposed/confirmed dates update per rules
- [ ] Confirmed assignments become `proposed_change` requiring re-confirm
- [ ] Poke cycle restarts per affected assignment

**FR:** FR-13

---

## Epic E8 — MMS Group Threads & Photos

*Group MMS per relationship (Dana + party + project handle #); ingest + web mirror.*

### E8-S1: MMS group thread per Subcontractor

**As a** team member, **I can** run field comms in a group MMS with a Subcontractor and the project handle # **so that** texts are logged to the project without subs using the portal.

**Acceptance:**
- [ ] **One handle # per project** provisioned on project create
- [ ] Inbound MMS: `To` → project, `From` → membership
- [ ] `mms_thread` record per relationship (`conversation_sid` when provisioned)
- [ ] Inbound MMS ingested and tied to project + sub membership
- [ ] Thread visible in web dashboard (mirror)
- [ ] Separate group per sub — no sub↔sub thread

**FR:** FR-14 · **UJ:** UJ-8

---

### E8-S2: MMS group thread per Customer

**As a** team member, **I can** use group MMS with a Customer and the project handle # **so that** homeowner comms are captured separately from subs.

**Acceptance:**
- [ ] Same ingest/mirror pattern as E8-S1
- [ ] Customer cannot see sub threads

**FR:** FR-14 · **UJ:** UJ-8, UJ-4

---

### E8-S3: MMS and web photos

**As a** team member, Subcontractor, or Customer, **I can** send photos via **MMS in the group** or web upload **so that** job photos live on the project record.

**Acceptance:**
- [ ] MMS images ingested to blob storage and shown in thread
- [ ] Optional camera upload via magic-link web session
- [ ] System schedule messages (propose/poke/confirm) sent via MMS/SMS to relationship thread

**FR:** FR-15 · **UJ:** UJ-6, UJ-8

---

### E8-S4: Project handle number pool lifecycle — **MVP**

**As the** system, **I must** JIT-provision, hold, cool, and release project handle numbers **so that** MMS routing stays correct, tenant-isolated, and telco cost matches usage.

**Acceptance:**
- [ ] **JIT assign** on project create: **always buy** new number from CPaaS in MVP (do not pull from `available` pool until E8-S5)
- [ ] **Tenant isolation:** number never moves from Company A to Company B while on platform
- [ ] **States (MVP):** `assigned` → `cooling` → `released`; `retired` on abuse; `available` unused until v0.1.1 (E8-S5)
- [ ] **Project archive (C-25):** number → `cooling` for **90 days** (default; per-contractor or platform override); inbound MMS/SMS still ingests to **archived** project; notify team member in-app
- [ ] **Cooling end (MVP):** deprovision at Twilio → `released`; E.164 on project is display-only; **not** returned to `available` pool
- [ ] **Churn / account closure:** release **all** company numbers to CPaaS immediately (assigned + cooling + available); message/media history **retained** in DB/blob
- [ ] **Return after churn:** reactivated company gets **new** JIT numbers; old E.164 on projects is historical display only
- [ ] `phone_number_pool` + `phone_number_assignments` history tables; inbound webhook resolves `To` → **current** assigned or cooling project only (history routing → **E8-S5 / SP-1**)
- [ ] Audit log on assign, enter cooling, release to vendor

**FR:** FR-14, FR-20 · **Journey:** C-25 · **Ref:** [project-handle-numbers.md](../../technical-exploration/project-handle-numbers.md) · **Reuse later:** [backlog.md](./user-journeys/backlog.md) BL-23, SP-1

---

### E8-S5: Same-company number reuse — **Post-MVP (v0.1.1)**

**As the** system, **when** a cooled number returns to the company pool, **I must** reassign it safely **so that** telco cost drops without misrouting Marcus's old group texts to the wrong project.

**Acceptance:**
- [ ] After `cooling_until`: transition `cooling` → `available` (same `contractor_id` only) instead of `released` — configurable per platform flag
- [ ] Project create may pull from `available` before buying new
- [ ] Inbound webhook: **history routing** — `(to_e164, from_phone)` matches prior `phone_number_assignments` → route to correct project (archived/cooling/current)
- [ ] Spike **SP-1** completed with test matrix before ship
- [ ] Admin metric: pool size and reuse rate per contractor

**Depends:** E8-S4 · **Backlog:** BL-23 · **Spike:** SP-1

---

## Epic E9 — Customer Schedule Visibility

*Customers see what changed — not sub internals.*

### E9-S1: Schedule change notification to Customer

**As a** Customer, **I receive** SMS/email when meaningful schedule changes occur **so that** I stay informed without calling the Contractor.

**Acceptance:**
- [ ] Plain-language what changed + magic link
- [ ] Scoped to customer membership only

**FR:** FR-16 · **UJ:** UJ-4

---

### E9-S2: What-changed timeline

**As a** Customer, **I can** view a simple timeline of schedule changes **so that** I understand the current plan without a Gantt chart.

**Acceptance:**
- [ ] Milestone-level changes only; no sub pricing or internal notes

**FR:** FR-17

---

## Epic E10 — Identity & Cross-Project Boundaries

*Subscription vs project role separation.*

### E10-S1: Isolated project memberships

**As the** system, **I must** enforce that project membership grants access only to that project **so that** a Sub on Job A cannot see Job B.

**Acceptance:**
- [ ] Magic links and sessions scoped to project + membership role
- [ ] Subcontractor membership never grants team member routes
- [ ] Same phone, two projects, two roles — independent records

**FR:** FR-20

---

### E10-S2: Contractor subscriber or team member as Sub/Customer on another project (v0.1 lean)

**As a** contractor who pays for ContractorPro (or any team member), **when** I am invited by phone to another Contractor's project as Subcontractor or Customer, **I can** act in that role via magic link **without** merging accounts or losing my subscription access.

**Acceptance:**
- [ ] Phone invite creates or reuses `persons` + `project_memberships` under the other Contractor's project
- [ ] Subscription / team-member session does not auto-grant invitee routes on other tenants' projects
- [ ] Invitee magic-link session does not grant team-member routes on own or other subscriptions
- [ ] No cross-tenant data leakage

**FR:** FR-20 · **ASSUMPTION:** v0.1 magic-link only; unified person portal v0.2

---

## Epic E11 — AI Draft (Stretch)

### E11-S1: Draft message on schedule change

**As a** team member, **I can** generate a draft customer update after a cascade **so that** I send a clear message faster.

**Acceptance:**
- [ ] Draft requires explicit approve before send
- [ ] Never auto-sends

**FR:** FR-19 · **Priority:** Stretch / v0.1.1

---

## Epic E12 — Platform Admin (minimal API) — **MVP API / Phase 2 UI**

*Compliance and ops hooks without `/admin` UI in M1 (A-4). Thomas operates via Twilio console, DB, and internal API until Phase 2.*

### E12-S1: STOP / START webhook ingestion — **MVP (pre-prod SMS)**

**As the** platform, **I must** ingest Twilio STOP/START and inbound opt-out keywords **so that** E6-S5 can enforce platform-global blocks.

**Acceptance:**
- [ ] Dedicated Twilio webhook route for inbound SMS on platform/long codes
- [ ] Parses STOP, STOPALL, UNSUBSCRIBE, CANCEL, END, QUIT, START
- [ ] Updates `persons.sms_opted_out` + `opt_out_audit` row
- [ ] Sends auto-reply copy per A-11

**Journey:** A-11 · **Decision:** A-5, BL-21

---

### E12-S2: Outbound delivery trace — **MVP (pre-prod SMS)**

**As** platform ops, **I need** message_sid and delivery status on every outbound SMS/MMS **so that** I can trace delivery failures (A-2).

**Acceptance:**
- [ ] Store Twilio `MessageSid` on send
- [ ] Status callback webhook updates `delivered` / `failed` / `undelivered`
- [ ] Queryable by phone + project for support (API or DB; no admin UI M1)

**Journey:** A-2

---

### E12-S3: Admin opt-out restore API — **MVP (pre-prod SMS)**

**As** platform ops, **I can** restore SMS consent for a phone via authenticated internal API **so that** wrongful STOP disputes are resolved (A-11).

**Acceptance:**
- [ ] Workforce Entra–protected endpoint (not CIAM)
- [ ] Requires reason + audit log entry
- [ ] Clears `sms_opted_out`; does not auto-resubscribe to threads without re-consent flow

**Journey:** A-11 · **Decision:** BL-21

---

### E12-S4: Pre-launch compliance checklist — **Pre-beta gate**

**As** the founder, **I can** track PL-1–PL-8 completion **so that** we do not send prod SMS/MMS until compliant.

**Acceptance:**
- [ ] Checklist doc or issue template tracks: 10DLC brand (PL-1), campaign (PL-2), number linkage (PL-3), opt-in copy (PL-4), Resend domain (PL-5), Google OAuth verification (PL-6), Entra prod tenant (PL-7), Telnyx spike (PL-8)
- [ ] `platform_settings.prod_sms_enabled` defaults `false` until all green
- [ ] E6-S5 and E12-S1 blocked when flag false

**Ref:** architecture-v0.1.md §10

---

## Suggested build order (solo founder)

### Phase 1 — MVP (self-serve coordination, no billing)

| Step | Stories | Outcome |
|------|---------|---------|
| **1 — Skeleton** | E1-S1, E2-S1/S2, E10-S1 | OAuth signup creates company; first project + tasks |
| **2 — Onboarding UX** | E1-S4, E3-S1, E3-S3 | Checklist; calendar connect; portfolio view |
| **3 — People** | E4 | Invite sub + customer, join, role portals |
| **4 — Core loop** | E5, E6 | Propose, accept/decline, poke, dashboard |
| **5 — Calendar** | E3-S2 | Pro-provided calendar + attendee invites on confirm |
| **6 — Comms** | E8 (incl. S4 pool), E9 | MMS threads, handle lifecycle, customer feed |
| **7 — Power** | E7, E2-S3/S4 | Cascade preview + apply |
| **8 — Compliance** | E12-S1–S3, E6-S5 | STOP/opt-out + delivery trace (before prod SMS) |
| **9 — MVP ship** | E10-S2, E12-S4, polish | Cross-tenant edge case; pre-launch gate; beta ready |

**MVP exit criteria:** 3–5 design-partner Contractors complete C-1 → first sub confirmed without admin help.

### Phase 2 — Billing (immediately post-MVP)

| Step | Stories | Outcome |
|------|---------|---------|
| **9 — Entitlements** | E1-S5 | Sandbox gates; project cap; MVP flag off |
| **10 — Stripe** | E1-S3 | Checkout, webhooks, Customer Portal |
| **11 — Dunning** | E1-S6 | Failed payment → suspend messaging |
| **12 — GA prep** | E1-S2 (if deferred) | Native auth fallback |

---

## MVP task checklist (implementation-facing)

Use as sprint backbone for Phase 1. Each row ≈ one deliverable slice.

| # | Task | Stories | Depends |
|---|------|---------|---------|
| M1 | Auth + auto-provision Contractor on first OAuth | E1-S1 | — |
| M2 | Company profile + team member session | E1-S1, E10-S1 | M1 |
| M3 | Create project + JIT handle # from company pool | E2-S1, E8-S4 | M2 |
| M3a | Number pool: assign, archive→cooling, inbound to archived project | E8-S4 | M3, M13 |
| M4 | Task CRUD + timeline view | E2-S2 | M3 |
| M5 | Onboarding checklist widget | E1-S4 | M3 |
| M6 | Google Calendar OAuth connect | E3-S1 | M2 |
| M7 | Sub/customer invite + magic link join | E4-S1, S2, S3 | M3 |
| M8 | Propose date + accept/decline + status | E5-S1–S3, S5 | M7 |
| M9 | Counter-propose + reschedule + reassign | E5-S2b, S3b, S4 | M8 |
| M10 | Poke scheduler + manual reminder/snooze | E6-S1–S3 | M8 |
| M11 | Confirmation dashboard / action queue | E5-S5, E6-S4 | M8 |
| M12 | Calendar write on confirm + attendee invites | E3-S2 | M6, M8 |
| M12a | Portfolio calendar UI | E3-S3 | M6, M8 |
| M13 | MMS ingest + thread mirror (sub) | E8-S1, S3 | M3, M7 |
| M14 | MMS thread (customer) + photos | E8-S2, S3 | M13 |
| M15 | Customer schedule notify + timeline | E9-S1, S2 | M7, M8 |
| M16 | Task dependencies + cascade toggle | E2-S3, S4 | M4 |
| M17 | Cascade preview + apply | E7-S1, S2 | M16, M8 |
| M18 | Schema placeholders: `tier`, `stripe_*`, `comms_enabled` (defaults open) | E1-S5 prep | M2 |
| M19 | STOP/opt-out + delivery trace | E12-S1–S3, E6-S5 | M13 |
| M20 | Pre-launch compliance gate | E12-S4 | M19 |
| M21 | Beta polish: error states, empty states, mobile confirm pages | all | M1–M20 |

**Phase 2 tasks (billing):**

| # | Task | Stories |
|---|------|---------|
| P1 | Entitlement middleware + sandbox blocks | E1-S5 |
| P2 | Stripe products/prices + Checkout + webhooks | E1-S3 |
| P3 | Customer Portal link + Settings billing UI | E1-S3 |
| P4 | Upgrade prompts at gated actions (C-27) | E1-S5 |
| P5 | Dunning webhooks + messaging_suspended | E1-S6 |
| P6 | Admin tenant snapshot billing fields (A-1, A-5) | admin journeys |
| P7 | Churn: release all CPaaS numbers + retain DB history | E8-S4, A-12 |

---

## Story count summary

| Epic | Stories | MVP (Phase 1) | Post-MVP (Phase 2) |
|------|---------|---------------|---------------------|
| E1 Onboarding, auth & billing | 6 | 2 (S1, S4) + S2 should | 3 (S3, S5, S6) |
| E2 Projects & tasks | 4 | 3–4 | — |
| E3 Calendar | 3 | 3 | — |
| E4 Invite & join | 3 | 3 | — |
| E5 Propose/confirm | 7 | 7 | — |
| E6 Poke + opt-out | 5 | 5 (incl. S5 pre-prod) | — |
| E7 Cascade | 2 | **2 (MVP)** | — |
| E8 Messaging | 5 | 5 (incl. S4 pool) | P7 churn release |
| E9 Customer feed | 2 | 2 | — |
| E10 Identity | 2 | 1–2 | — |
| E11 AI stretch | 1 | 0 | — |
| E12 Platform admin | 4 | 4 (API only; pre-prod SMS) | UI Phase 2 |
| **Total** | **44** | **~32–35** | **3 billing + P7 pool release** |

---

## Resolved story-level questions (2026-08-20)

1. ~~**E3-S2:** BYO vs pro-provided~~ → **Pro-provided** + attendee invites
2. ~~**E5-S3:** Hard decline vs revert~~ → **Hard decline** + E5-S3b reassign (C-1)
3. ~~**E1-S2:** Native auth in MVP~~ → **OAuth-only (Google) MVP**; E1-S2 v0.1.1
4. ~~**E7:** Cascade in MVP~~ → **Yes — MVP** (A-1)
5. ~~**E1-S5:** 6th active project on Pro 5~~ → **Plan-only** (A-8)

Log changes in [discovery-log.md](../../discovery-log.md).
