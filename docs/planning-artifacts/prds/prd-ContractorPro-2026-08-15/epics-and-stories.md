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

**MVP success:** Ryan self-serves signup → plan job → publish prelim → finalize → first sub confirmed — without Thomas provisioning anything.

**Correct-course (2026-08-20):** Company # + SMS relay replaces per-project group MMS; job planning promoted to MVP (E13–E15). See [sprint-change-proposal-2026-08-20.md](../../sprint-change-proposal-2026-08-20.md).

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

### E1-S7: Auth/session E2E coverage (CI-safe) — **MVP**

**As a** product team, **we can** run deterministic end-to-end auth/session tests in CI **so that** onboarding regressions are caught before release.

**Acceptance:**
- [ ] Playwright suite runs in CI without requiring real Google interaction
- [ ] Coverage includes: anonymous guard to login, authenticated dashboard load, `/api/v1/team/me` success and unauthorized states, logout behavior
- [ ] Failed runs publish screenshots/traces/videos for triage
- [ ] Suite is stable enough for release gating (no flaky retries required for pass)

**FR:** FR-1 · **Journey:** C-1 step 1 · **Type:** quality enablement

---

### E1-S8: Deterministic test login bridge for E2E — **MVP**

**As a** product team, **we can** establish authenticated sessions in test environments without third-party IdP automation **so that** auth flows are testable and repeatable.

**Acceptance:**
- [ ] Test-only login mechanism is enabled only in `Test` environment
- [ ] Mechanism can mint a valid app session cookie for seeded user/contractor/team-member test identities
- [ ] Mechanism is blocked in Development/Production unless explicitly enabled for local test runs
- [ ] Security review confirms no production auth bypass risk

**FR:** FR-1 · **Journey:** C-1 step 1 · **Type:** quality enablement

---

### E1-S9: Manual Google OAuth release smoke — **MVP**

**As a** release owner, **I can** run a short manual Google OAuth smoke checklist **so that** external IdP and first-login provisioning remain verified before launch.

**Acceptance:**
- [ ] First-time Google sign-in creates expected user/contractor/team-member/auth-identity rows
- [ ] Repeat sign-in with same Google account does not duplicate provisioning
- [ ] Sign-in with second Google account creates separate contractor tenant
- [ ] Logout clears session and protected route returns unauthorized until next login
- [ ] Checklist can be completed in under 10 minutes with explicit pass/fail evidence

**FR:** FR-1 · **Journey:** C-1 step 1 · **Type:** release quality gate

---

## Epic E2 — Projects & Tasks

*Team members organize work on jobs.*

### E2-S1: Create a project

**As a** team member, **I can** create a Project (name, address, basic details) **so that** I have a container for schedule and comms.

**Acceptance:**
- [ ] Project belongs to my Contractor subscription only
- [ ] Subcontractor/Customer memberships cannot create projects
- [ ] Project created with status **`planning`** — no telco provisioning on create
- [ ] Company number provisioned at contractor level via **E8-S1** (not per project)

**FR:** FR-2, FR-21

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

## Epic E8 — Company Number, SMS Relay & Inbox

*One company # per contractor; SMS relay to staff + shared app inbox. Supersedes group MMS / per-project handle model — [company-number-messaging.md](../../technical-exploration/company-number-messaging.md).*

### E8-S1: Provision company number — **MVP**

**As the** system, **I must** provision one Twilio 10DLC number per Contractor **so that** all subs/customers text a single company line.

**Acceptance:**
- [ ] One active E.164 per contractor subscription (`contractor_phone_numbers`)
- [ ] Provision trigger TBD — interim: first paid / comms enabled (see SME follow-up 1b)
- [ ] Sandbox signup: no number until entitled
- [ ] Churn: release number immediately; DB history retained; return customer gets new number

**FR:** FR-14 · **Ref:** [company-number-messaging.md](../../technical-exploration/company-number-messaging.md)

---

### E8-S2: Inbound webhook + thread routing — **MVP**

**As the** system, **when** external SMS/MMS arrives at the company #, **I must** route to the correct thread or orphan queue **so that** traffic is logged and staff notified.

**Acceptance:**
- [ ] Twilio webhook validates signature; idempotent by `MessageSid`
- [ ] Thread model: `(contractor_id, person_id, project_id, audience)`
- [ ] 0 project matches → orphan queue; 1 match → attach; N matches → orphan + suggest in app
- [ ] Fan-out SMS relay notification to team member phones from company #
- [ ] Staff phone allowlist bypasses carrier STOP for relay copies (see RC-5)

**FR:** FR-14 · **UJ:** UJ-8

---

### E8-S3: Staff SMS relay — **MVP**

**As a** team member, **I can** reply to subs/customers by texting the company # **so that** field coordination works without opening the app.

**Acceptance:**
- [ ] Staff inbound from allowlisted phone → `StaffSmsRouter`
- [ ] Lenient ref token: token match → route; else single open thread → route; else disambiguation SMS
- [ ] Never auto-send to external participant on ambiguous staff inbound
- [ ] Outbound to external participant + copy to other staff from company #
- [ ] Open thread TTL default 72h (see RC-5)

**FR:** FR-14 · **Spike:** SP-4 SME validation before build lock

---

### E8-S4: App shared inbox — **MVP**

**As a** team member, **I can** view and reply in a shared inbox **so that** Ryan and Maci see the same threads and can assign orphans.

**Acceptance:**
- [ ] Unified inbox with filters (project, participant, audience, unassigned)
- [ ] Reply from app binds `thread_id` directly (no token)
- [ ] Optional thread claim; show last replier
- [ ] All team members see all threads in MVP
- [ ] Polling refresh ~60s (SignalR deferred — RC-5)

**FR:** FR-14, FR-15 · **UJ:** UJ-8

---

### E8-S5: Inbound MMS ingest — **MVP**

**As the** system, **I must** store inbound MMS media **so that** photos in SMS threads appear in the app.

**Acceptance:**
- [ ] `InboundMediaIngestWorker` persists MMS to blob + thread
- [ ] Staff MMS outbound from inbox **deferred v1** (RC-5)

**FR:** FR-15

---

### E8-S6: Platform STOP on company # — **MVP (pre-prod SMS)**

**As the** system, **when** a person texts STOP on the company line, **I must** block automated outbound **so that** we comply with TCPA.

**Acceptance:**
- [ ] Same platform-global opt-out as E6-S5 / E12-S1
- [ ] Staff allowlisted phones do not opt out company line when texting STOP on relay copies

**FR:** FR-14 · **Journey:** A-11 · **Depends:** E12-S1

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

### E9-S3: Customer preliminary schedule view — **MVP**

**As a** Customer, **I can** view a read-only preliminary schedule after GC publishes **so that** I see the plan before subs are invited.

**Acceptance:**
- [ ] Triggered by **Publish prelim** action (E13-S4) — separate from finalize
- [ ] Optional customer gate slot disabled by default until SME 2B

**FR:** FR-21 · **UJ:** UJ-4

---

### E9-S4: Automated milestone prep comms — **MVP**

**As a** Customer, **I receive** automated prep messages before milestones **so that** I know how to prepare (e.g. demo day).

**Acceptance:**
- [ ] Sends N days before customer-visible milestone; N = contractor setting (FR-24)
- [ ] Respects `notify_via`; from company # / email
- [ ] Distinct from schedule-change notifications (RC-2)

**FR:** FR-24 · **Epic overlap:** E15

---

## Epic E13 — Job Planning & Finalize — **MVP**

*Plan-first workflow promoted from v0.2 — [job-planning-workflow.md](../../technical-exploration/job-planning-workflow.md).*

### E13-S1: Project templates — **MVP**

**As a** team member, **I can** save and apply job templates **so that** phases, durations, buffers, and cascade defaults reuse across jobs.

**Acceptance:**
- [ ] Template includes phase list, deps, default cascade waves (parallel/sequential)
- [ ] Apply template to new project in `planning` status

**FR:** FR-21, FR-22

---

### E13-S2: Planning workspace — **MVP**

**As a** team member, **I can** edit the plan (durations, buffers, start date, overlay vs existing calendar) **so that** I finalize a realistic schedule before notifying anyone.

**Acceptance:**
- [ ] No Google writes or outbound SMS in planning mode
- [ ] Overlay single-job vs existing commitments
- [ ] Dashed / planning UI styling

**FR:** FR-21

---

### E13-S3: Contract / customer constraints — **MVP**

**As a** team member, **I can** record blackouts and access notes at project setup **so that** planning respects customer constraints.

**Acceptance:**
- [ ] Structured or freeform fields TBD (RC-7)
- [ ] Constraints visible in planning workspace

**FR:** FR-21

---

### E13-S4: Publish preliminary schedule to customer — **MVP**

**As a** team member, **I can** publish a skeletal schedule to the customer **so that** they preview before subs are invited.

**Acceptance:**
- [ ] **Separate button** from finalize
- [ ] Customer sees read-only prelim (E9-S3)
- [ ] Optional gate slot off by default (2B)

**FR:** FR-21

---

### E13-S5: Finalize plan & start sub cascade — **MVP**

**As a** team member, **I can** finalize the plan **so that** sub approval cascade begins and project becomes `active`.

**Acceptance:**
- [ ] **Separate button** from publish prelim
- [ ] Triggers configurable cascade (E13-S6 / FR-22)
- [ ] Sub assignments in planning become invite targets

**FR:** FR-21, FR-22

---

### E13-S6: Runtime cascade override — **MVP**

**As a** team member, **I can** adjust cascade waves at runtime **so that** I send parallel invites or skip gates without rebuilding the plan.

**Acceptance:**
- [ ] Override who goes next, skip gate, open parallel wave
- [ ] Outbound from company # with project prefix

**FR:** FR-22

---

## Epic E14 — Project Resources & QR — **MVP**

*Google Drive backend; app-only portal — [decision-workbook.md](../../../sme-meetings/decision-workbook.md) §4, §5.*

### E14-S1: Google Drive folder per project — **MVP**

**As a** team member, **I can** link or auto-create a Drive folder **so that** job docs live where Ryan already works.

**Acceptance:**
- [ ] OAuth scope for Drive (calendar + drive minimum)
- [ ] Folder create/link on project setup or finalize
- [ ] Degradation path when GC revokes Google (SP-3)

**FR:** FR-23 · **Spike:** SP-3

---

### E14-S2: Doc list + portal file view — **MVP**

**As a** sub/customer, **I can** view project files in the app portal **so that** I never open drive.google.com.

**Acceptance:**
- [ ] GC manages doc list in app; sync to Drive folder
- [ ] In-portal list + preview (strategy TBD — SP-3)

**FR:** FR-23

---

### E14-S3: Printable QR → resource page — **MVP**

**As a** team member, **I can** print a QR sheet for the job site **so that** subs scan to reach project resources.

**Acceptance:**
- [ ] QR URL is app resource page — not raw Drive link
- [ ] Single QR with Check in + Upload actions (RC-1)

**FR:** FR-23

---

### E14-S4: QR check-in & check-out — **MVP**

**As a** sub, **I can** scan QR to check in and upload end-of-day photos **so that** progress is captured on the job.

**Acceptance:**
- [ ] First scan: verify phone + bind to sub on project roster
- [ ] Return scan: recognize phone + sub
- [ ] Check-out: photos/notes upload via app → Drive folder

**FR:** FR-23, FR-5

---

## Epic E15 — Customer Milestone Comms — **MVP**

*Automated prep messages — FR-24; overlaps E9-S4.*

### E15-S1: Contractor milestone lead-time setting — **MVP**

**As a** team member, **I can** set default days-before for milestone comms **so that** customers get prep notice on my schedule.

**Acceptance:**
- [ ] Company-level default (e.g. 1 day, 7 days)
- [ ] Optional per-phase override TBD (RC-2)

**FR:** FR-24

---

### E15-S2: Milestone prep templates — **MVP**

**As a** team member, **I can** attach prep message templates to customer-visible phases **so that** automated sends have useful content.

**Acceptance:**
- [ ] Template: milestone name + prep instructions
- [ ] Tied to plan phases from E13

**FR:** FR-24

---

### E15-S3: Scheduled milestone send worker — **MVP**

**As the** system, **I must** send milestone comms on schedule **so that** customers are notified without manual Ryan/Macie email.

**Acceptance:**
- [ ] Worker runs against plan phase dates minus lead-time
- [ ] Respects `notify_via`; from company # / email
- [ ] Does not duplicate schedule-**change** alerts (RC-2)

**FR:** FR-24

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
| **1 — Skeleton** | E1-S1, E2-S1/S2, E10-S1 | OAuth signup creates company; first project (`planning`) + tasks |
| **2 — Onboarding UX** | E1-S4, E3-S1, E3-S3 | Checklist; calendar connect; portfolio view |
| **3 — Planning** | E13-S1–S3 | Templates; planning workspace; constraints |
| **4 — People** | E4, E8-S1 | Invite sub + customer; provision company # |
| **5 — Resources** | E14 | Drive folder; QR; check-in/out |
| **6 — Finalize loop** | E13-S4–S6, E5, E6 | Prelim publish; finalize + cascade; propose/accept/poke |
| **7 — Calendar** | E3-S2 | Pro-provided calendar + attendee invites on confirm |
| **8 — Comms** | E8-S2–S6, E9, E15 | Inbox + relay; customer feed + milestone comms |
| **9 — Power** | E7, E2-S3/S4 | Cascade preview + apply (live reschedule) |
| **10 — Compliance** | E12-S1–S3, E8-S6 | STOP/opt-out + delivery trace (before prod SMS) |
| **11 — MVP ship** | E10-S2, E12-S4, polish | Cross-tenant edge case; pre-launch gate; beta ready |

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
| M3 | Create project (`planning` status) | E2-S1 | M2 |
| M3a | Provision company # | E8-S1 | M2 |
| M3b | Planning template + workspace | E13-S1, S2 | M3 |
| M4 | Task CRUD + timeline view | E2-S2 | M3 |
| M5 | Onboarding checklist widget | E1-S4 | M3 |
| M6 | Google Calendar OAuth connect | E3-S1 | M2 |
| M6a | Drive folder + QR resource page | E14-S1–S3 | M3, SP-3 |
| M7 | Sub/customer invite + magic link join | E4-S1, S2, S3 | M3 |
| M7a | Publish prelim + finalize + cascade start | E13-S4, S5, S6 | M7, M3b |
| M8 | Propose date + accept/decline + status | E5-S1–S3, S5 | M7a |
| M9 | Counter-propose + reschedule + reassign | E5-S2b, S3b, S4 | M8 |
| M10 | Poke scheduler + manual reminder/snooze | E6-S1–S3 | M8 |
| M11 | Confirmation dashboard / action queue | E5-S5, E6-S4 | M8 |
| M12 | Calendar write on confirm + attendee invites | E3-S2 | M6, M8 |
| M12a | Portfolio calendar UI | E3-S3 | M6, M8 |
| M13 | Company # inbound + SMS relay + inbox | E8-S2–S4 | M3a, M7 |
| M13a | Inbound MMS ingest | E8-S5 | M13 |
| M14 | QR check-in/out + Drive upload | E14-S4 | M6a, M7 |
| M15 | Customer schedule notify + timeline + prelim | E9-S1–S3 | M7, M8 |
| M15a | Milestone prep comms worker | E15, E9-S4 | M7a, M8 |
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
