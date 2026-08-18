---
title: ContractorPro v0.1 — Epics & User Stories
created: 2026-08-15
status: draft
source_prd: prd.md
---

# ContractorPro v0.1 — Epics & User Stories

Derived from [prd.md](./prd.md). Product-facing stories only — implementation detail belongs in architecture/TRD.

**Actors:** **Team member** (Contractor subscription), **Subcontractor** (project role), **Customer** (project role).

**Story ID format:** `E{epic}-S{story}` · maps to `FR-N` where noted.

---

## Epic E1 — Contractor Onboarding & Auth

*Team members can access the product under a paying Contractor subscription.*

### E1-S1: Sign in with OAuth

**As a** team member, **I can** sign in with Google, Apple, or Microsoft **so that** I access my Contractor's workspace without a new password.

**Acceptance:**
- [ ] OAuth sign-in creates or links a team member to exactly one Contractor subscription
- [ ] Unauthenticated users cannot reach project management routes
- [ ] Session persists across browser restarts (secure cookie)

**FR:** FR-1

---

### E1-S2: Sign in with native account (fallback)

**As a** team member without a preferred OAuth provider, **I can** use a native account with passkey or TOTP **so that** I can still access ContractorPro.

**Acceptance:**
- [ ] Passkey/TOTP offered; password-only is fallback
- [ ] Same Contractor subscription binding as OAuth

**FR:** FR-1 · **Priority:** Should (v0.1 if auth vendor supports; else v0.1.1)

---

### E1-S3: Subscribe to ContractorPro

**As a** Contractor subscription owner, **I can** choose a plan and pay **so that** my team can run projects beyond trial limits.

**Acceptance:**
- [ ] Paid tier unlocks configured limits; free tier enforced when applicable
- [ ] Subcontractor and Customer project memberships are never billed
- [ ] Inactive subscription blocks or limits create/notify actions per tier rules

**FR:** FR-18

---

## Epic E2 — Projects & Tasks

*Team members organize work on jobs.*

### E2-S1: Create a project

**As a** team member, **I can** create a Project (name, address, basic details) **so that** I have a container for schedule and comms.

**Acceptance:**
- [ ] Project belongs to my Contractor subscription only
- [ ] Subcontractor/Customer memberships cannot create projects
- [ ] System provisions **project handle #** (dedicated MMS phone number) on create

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

### E3-S2: Link or create project calendar

**As a** team member, **I can** link an existing Google calendar or have ContractorPro create one per project **so that** subs see agreed dates in Google.

**Acceptance:**
- [ ] One shared calendar per project (v0.1 lean)
- [ ] No calendar write until Task Assignment is **confirmed**

**FR:** FR-3, FR-8

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
- [ ] Status → `declined` (or last confirmed preserved per product rule on reschedule decline)
- [ ] Calendar not updated to proposed date
- [ ] Team member notified in-app (decline alert on by default)
- [ ] Team member can reassign per E5-S3b

**FR:** FR-8 · **UJ:** UJ-2b

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

## Epic E7 — Schedule Cascade

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

### E10-S2: Team member as Sub on another Contractor's project (v0.1 lean)

**As a** team member invited by phone to another Contractor's project as Subcontractor, **I can** act as Sub on that project via magic link **without** merging accounts.

**Acceptance:**
- [ ] Phone invite creates separate membership under other Contractor's project
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

## Suggested build order (solo founder)

| Phase | Epics | Outcome |
|-------|-------|---------|
| **1 — Skeleton** | E1 (OAuth lean), E2-S1/S2, E10-S1 | Login, create project + tasks |
| **2 — People** | E4 | Invite sub + customer, join |
| **3 — Core loop** | E5, E6 | Propose, accept, poke |
| **4 — Calendar** | E3 | Google sync on confirm |
| **5 — Comms** | E8, E9 | Messages, photos, customer feed |
| **6 — Power** | E7, E2-S3/S4 | Cascade |
| **7 — Ship** | E1-S3, E11 | Billing, AI stretch |

---

## Story count summary

| Epic | Stories | Must for MVP |
|------|---------|--------------|
| E1 Auth & billing | 3 | 2 (S1, S3; S2 if OAuth-only acceptable defer native) |
| E2 Projects & tasks | 4 | 3 (S1–S3; S4 with E7) |
| E3 Calendar | 2 | 2 |
| E4 Invite & join | 3 | 3 |
| E5 Propose/confirm | 5 | 5 |
| E6 Poke | 4 | 3 (S1–S3; S4 should) |
| E7 Cascade | 2 | 1 (S1–S2 optional toggle — S2 if cascade in MVP) |
| E8 Messaging | 3 | 3 |
| E9 Customer feed | 2 | 2 |
| E10 Identity | 2 | 1 (S1 must; S2 lean) |
| E11 AI stretch | 1 | 0 |
| **Total** | **31** | **~24 must-have** |

---

## Open story-level questions

1. **E3-S2:** BYO calendar only for v0.1 build, or pro-provided create?
2. **E5-S3:** On decline, hard `declined` vs revert to last confirmed?
3. **E1-S2:** Native auth in v0.1 or OAuth-only first ship?
4. **E7:** Is cascade required for MVP or fast-follow?

Log changes in [discovery-log.md](../../discovery-log.md).
