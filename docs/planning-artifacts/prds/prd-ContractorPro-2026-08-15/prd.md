---
title: ContractorPro
status: draft
created: 2026-08-15
updated: 2026-08-15
version: v0.1-mvp
---

# PRD: ContractorPro (v0.1 MVP)

## 0. Document Purpose

This PRD defines **what** ContractorPro v0.1 must do for users and **why** it matters. It is written for the product owner (solo founder), downstream epic/story creation, and future UX work.

**Technical design is intentionally out of scope here.** Stack, integrations, data model, and implementation live in separate documents (see [addendum.md](./addendum.md) and `docs/planning-artifacts/technical-exploration/`).

This PRD consolidates prior discovery: `product-vision.md`, `schedule-confirmation-workflow.md`, `invite-join-flow.md`, and related planning artifacts.

**Identity principle:** The only fixed account type is the **Contractor** (subscription owner). **Subcontractor** and **Customer** are **project-scoped roles**, not global user types. The same person may hold different roles on different projects.

---

## 1. Vision

Small residential contractors lose hours to **phone tag** when schedules move. Enterprise construction platforms solve coordination but are **expensive**, **heavy to onboard**, and **force subs into another app**. Subs and customers already live in **text messages** and **Google Calendar** on their phones.

ContractorPro is a **schedule coordination layer** — not a full ERP. The **Contractor** (subscription owner) remains the hub on each project. When a date changes, assigned subs are **asked to confirm**, **reminded until they respond**, and **agreed dates appear on Google Calendar**. Subcontractors and Customers access the job through **magic links** — no app install, no password for invitees.

**Wedge:** Cheaper and simpler than Buildertrend-class tools, with **integrate-don't-replace** calendar behavior and **low-friction sub adoption**.

---

## 2. Target User

### 2.1 Jobs To Be Done

**Contractor / Team member (pays — subscription):**

- When I move a trade date, I need subs to **actually see it and commit** — not claim they never got the text.
- I need one place to see **who confirmed vs who is still pending** across active jobs.
- I want schedule changes to show up where I already work (**Google Calendar** + phone), without re-entering everything in a bloated PM tool.
- I need to message subs and customers **separately** and keep a project record (including photos).

**Subcontractor (project role — free):**

- Tell me when my date is and let me **accept or decline in one tap** — no new app, no password.
- Don't make me hunt through group texts for the current date.
- `[ASSUMPTION: On another Contractor's project I may be a Customer instead — role is per project, not who I am globally.]`

**Customer (project role — free):**

- Show me **what changed** on my project and let me message the Contractor and share photos — without seeing sub-only details.
- `[ASSUMPTION: I may be a Customer on one project and a Subcontractor on another.]`

### 2.2 Non-Users (v0.1)

- Solo specialty trades with no sub coordination (Jobber / Housecall Pro lane)
- Large commercial contractors, service-ticket workflows
- Enterprise crews requiring full ERP (estimating, selections, time cards, safety modules)

### 2.3 Key User Journeys

**UJ-1. Dana proposes a paint date and Mike confirms.**

Dana is a **team member** at **Riverside Remodeling** (a **Contractor** subscription). She is at her desk, logged into ContractorPro. She assigns "Painting — Maple St kitchen" to Mike for Sept 10 and saves. Mike is a **Subcontractor** on this project.

Mike receives an SMS (his preferred channel) with a link. On his phone he taps **Accept**. Dana sees "Mike confirmed" on her dashboard. The agreed date appears on the **shared project Google Calendar** — visible to Dana in Google and on Mike's phone calendar. No one had to install an app.

*Edge case:* Mike ignores the text. ContractorPro sends daily reminders (batched if multiple pending items). Dana sees "Pending 2 days" and optional escalation alert.

**UJ-2. Dana bumps the date; Mike must re-confirm.**

Dana moves painting from Sept 10 → Sept 11. Mike's calendar **still shows Sept 10** until he accepts. He gets SMS/email with old → new date and a link. On **Accept**, both calendars update to Sept 11. On **Decline**, Dana is notified immediately and the last agreed date stands until she resolves it.

**UJ-3. Jose joins a job in under 60 seconds.**

Dana invites Jose (electric) by name + phone from the Maple St project. Jose taps the SMS link, confirms name and phone on one screen, and joins — no password. He can see his assigned tasks, confirm dates, message Dana privately, and upload photos from his mobile browser.

**UJ-4. Customer Carol sees what changed.**

Dana invites Carol as a **Customer** on the Maple St project. Carol joins via magic link. When Dana shifts the cabinet install, Carol gets SMS with a link to a simple "what changed" view and can reply in her **private Contractor↔customer** thread. She never sees sub pricing or Contractor↔sub messages.

**UJ-5. Dana cascades a slip and previews impact.**

Framing on Maple St slips 3 days. Dana enables cascade (per-project setting), previews which dependent tasks and which subs will be affected, and confirms. Affected subs receive new **proposals** requiring re-confirmation; poke reminders run until they respond.

---

## 3. Identity & Roles Model

**Core rule:** Identity is separate from project role. The only inflexible membership is **Contractor** (SaaS subscription owner). Everything else is contextual.

### 3.1 Layers

| Layer | What it is | Scope | Pays? |
|-------|------------|-------|-------|
| **Contractor** | Subscription tenant; the business paying for ContractorPro | Account-wide | Yes |
| **Team member** | Authenticated user who works for a Contractor | One Contractor subscription | No (employed by Contractor) |
| **Person** | Real human, identified primarily by phone | Cross-project (linkable) | — |
| **Project membership** | Role of a Person on one Project | Single project | No |

### 3.2 Project membership roles (not global identity)

On each **Project**, a Person is invited as exactly one of:

| Role | Can create projects? | Typical access |
|------|----------------------|----------------|
| **Subcontractor** | No | Assigned tasks, confirm dates, Contractor↔sub messages |
| **Customer** | No | Schedule slice, what-changed, Contractor↔customer messages |

A Person may be **Subcontractor** on Project A and **Customer** on Project B — even under different Contractors. The system uses **separate project membership records**, not a global "user type."

### 3.3 Cross-subscription cases

| Scenario | Supported |
|----------|-----------|
| Same phone: Sub on one project, Customer on another | Yes |
| Same phone: Customer on projects from two different Contractors | Yes |
| Team member at Contractor X invited as Subcontractor on Contractor Y's project | Yes `[ASSUMPTION: v0.1 via phone invite; same login identity optional v0.2]` |
| Contractor subscription holder also participates on someone else's project | Yes — via project membership, not subscription role |

**Permissions derive from context:** subscription (Team member), project role (Subcontractor / Customer), or both — never a single global role label.

### 3.4 Naming note

**Contractor** means the **paying subscription owner** (remodeler, builder, design-build firm, etc.) — not necessarily a licensed "general contractor." Avoid "GC" in product copy. **Customer** is preferred over "homeowner" in the product (residential marketing may still say homeowners).

---

## 4. Glossary

- **Contractor** — The **subscription owner**; the only fixed SaaS account type. Owns billing, projects, and team members. Not a project role.
- **Team member** — Authenticated user belonging to a Contractor subscription; can create projects and invite people.
- **Person** — A real individual, keyed by verified phone (and optional email). May appear on many projects under different roles.
- **Project** — A job run by a Contractor (e.g., kitchen remodel at an address).
- **Project membership** — A Person's participation on one Project with role **Subcontractor** or **Customer**.
- **Subcontractor** — Project membership role: performs work; cannot create projects.
- **Customer** — Project membership role: hired the Contractor for the project; cannot create projects. Multiple Customers allowed per project.
- **Task** — A schedulable unit of work on a Project (e.g., "Rough electric").
- **Task Assignment** — Link between a Task and a Subcontractor project membership with **confirmation status** (proposed, confirmed, proposed_change, declined).
- **Propose** — Team member sets or changes a date; Subcontractor has not yet accepted.
- **Confirm / Accept** — Subcontractor agrees to proposed dates via magic link; triggers calendar sync.
- **Poke** — Automated reminder (SMS and/or email) until Subcontractor accepts, declines, or Team member stops reminders.
- **Magic Link** — Signed URL for passwordless access (join, confirm date, view portal).
- **Cascade** — Optional shift of dependent tasks by the same delta when a predecessor moves.
- **Portal** — Mobile-first web experience for project memberships (not a native app).

---

## 5. Features

### 5.1 Contractor Account & Project Setup

**Description:** Team members can sign in (OAuth preferred), work under a **Contractor** subscription, connect Google Calendar, create **Projects**, and add **Tasks** with dates and optional dependencies. Desktop-first experience. Realizes foundation for all UJs.

**Functional Requirements:**

#### FR-1: Team member authentication

Team members can sign in to ContractorPro using OAuth (Google, Apple, or Microsoft) or a native account with passkey/TOTP preferred over passwords. Each authenticated user belongs to exactly one **Contractor** subscription in v0.1.

**Consequences (testable):**
- Unauthenticated users cannot access Contractor project management screens.
- Session persists across browser restarts per standard secure cookie policy.
- Project membership roles (Subcontractor, Customer) do not grant subscription-level access.

#### FR-2: Project and task management

Team members can create a **Project** under their Contractor, add **Tasks** with start/end (or single-day) dates, assign optional **dependencies** between tasks, and enable or disable **Cascade** per project.

**Consequences (testable):**
- Team member can view a project task list/timeline for all tasks on a project.
- Tasks without assignments can exist (Contractor-only milestones).
- Subcontractor and Customer project memberships cannot create projects.

#### FR-3: Google Calendar connection

Team members can connect a Google account on behalf of the Contractor and link or create a **shared project calendar** so agreed dates sync after Subcontractor confirmation.

**Consequences (testable):**
- Until a Task Assignment is **confirmed**, no agreed-date event is written to the shared project calendar for that assignment.
- Team member can see connection status (connected / disconnected / error).

`[ASSUMPTION: v0.1 ships one calendar mode — either BYO link or Pro-provided per project; not both required at launch.]`

---

### 5.2 Project Membership Invite & Join

**Description:** Team members invite people to a **Project** with a **project membership role** (Subcontractor or Customer). Invitees join with **name + phone confirm** only — no password. Role is scoped to that project. Realizes UJ-3, UJ-4.

**Functional Requirements:**

#### FR-4: Invite project membership

Team member can invite a Person to a Project with name, phone (required), role (**Subcontractor** or **Customer**), and notification preference (`sms`, `email`, or `both`).

**Consequences (testable):**
- Invitee receives SMS with join link when phone is provided.
- Invitee receives email when email is provided and `notify_via` includes email.
- Same phone may hold different roles on different projects (separate membership records).

#### FR-5: Passwordless join

Invitee can complete join on a single mobile-first screen (confirm/edit name, confirm phone) without creating a password or Contractor subscription.

**Consequences (testable):**
- Successful join creates a **project membership** record scoped to one Project and one role.
- Join requires phone verification (invite token match or OTP).

#### FR-6: Role-based visibility

Customer project memberships cannot view Subcontractor-only threads, sub pricing, or internal Contractor notes. Subcontractor memberships see only their assigned tasks and Contractor↔sub thread.

**Consequences (testable):**
- Customer magic link cannot access sub assignment detail beyond shared schedule slice.
- Subcontractor magic link cannot access customer-private thread.
- Permissions evaluated per project membership, not global identity.

---

### 5.3 Schedule Proposal & Confirmation

**Description:** Core coordination loop. Team member **proposes** dates; Subcontractor **accepts or declines** via link; calendars update on accept only. Realizes UJ-1, UJ-2.

**Functional Requirements:**

#### FR-7: Propose date to assigned subcontractor

Team member can assign a Subcontractor project membership to a Task and **propose** a date. System notifies the Person per `notify_via` on that membership.

**Consequences (testable):**
- Task Assignment status becomes `proposed` (or `proposed_change` on reschedule).
- Notification includes task name, project name, date(s), and Accept/Decline link.
- No "reply YES to SMS" — link-based only.

#### FR-8: Subcontractor accept or decline

Person with Subcontractor role on the project can open magic link and tap **Accept** or **Decline** on a mobile-friendly page without installing an app.

**Consequences (testable):**
- **Accept:** status → `confirmed`; shared project calendar event created/updated; Team member notified in-app.
- **Decline:** Team member notified in-app (decline notification on by default); calendar unchanged for proposed date.

#### FR-9: Reschedule requires re-confirmation

When Team member changes dates on a previously **confirmed** assignment, Subcontractor must re-confirm. Subcontractor calendar shows last **confirmed** date until re-accept.

**Consequences (testable):**
- Status → `proposed_change` after Team member edit on confirmed assignment.
- New proposal notification includes old → new date.

#### FR-10: Contractor confirmation dashboard

Team member can view all Task Assignments filtered by status: pending, confirmed, declined; and see per-sub pending summary ("who is holding me up").

**Consequences (testable):**
- Pending items show time since last notification and reminder count.
- Team member can manually "send reminder now" or snooze automated pokes.

---

### 5.4 Automated Poke (Reminders)

**Description:** ContractorPro — not Google Calendar — chases non-responding Subcontractors. Daily reminders until accept/decline or Team member stops. Matches Buildertrend-style persistence. Realizes UJ-1 edge case.

**Functional Requirements:**

#### FR-11: Automated reminder cadence

When a Task Assignment is `proposed` or `proposed_change`, system sends reminders on a schedule until resolved.

**Consequences (testable):**
- Default: initial notification, +24h, +48h, then daily until accept/decline/stop.
- Multiple pending assignments for same Sub on same project batch into **one daily SMS** where possible.
- SMS respects quiet hours (default 8pm–8am project timezone); queued for next window.
- Reminders stop on accept, decline, Team member snooze, reassignment, or project archive.

#### FR-12: Team member escalation

Team member receives in-app alert when Subcontractor has been pending beyond configurable threshold (default 48h); optional SMS to Team member on decline.

**Consequences (testable):**
- Dashboard shows reminder count and last poke timestamp per pending assignment.

---

### 5.5 Schedule Cascade (Optional)

**Description:** When enabled per project, moving a task shifts dependent tasks by the same delta. Team member previews affected Subcontractors before confirming. Realizes UJ-5.

**Functional Requirements:**

#### FR-13: Cascade preview and apply

Team member can preview which tasks and which Subcontractors will be affected before applying a cascade shift.

**Consequences (testable):**
- Dependent tasks update proposed dates by the same delta when cascade is confirmed.
- Each affected Subcontractor receives proposal notifications (and poke cycle restarts per assignment).

`[ASSUMPTION: v0.1 cascade shifts proposed/confirmed assignments; confirmed assignments become proposed_change requiring re-confirm.]`

---

### 5.6 Messaging & Photos

**Description:** Private messaging threads per relationship (Contractor↔sub, Contractor↔customer) with image upload from Team members, Subcontractors, and Customers. Realizes UJ-4 and project communication needs.

**Functional Requirements:**

#### FR-14: Private messaging threads

Team member can message each Subcontractor membership privately and each Customer membership privately. Subcontractors and Customers can reply via portal.

**Consequences (testable):**
- No shared sub↔customer thread.
- Message history visible to Team member and the project membership in that thread.

#### FR-15: Image upload in messages

Team member, Subcontractor, and Customer can attach photos to messages from mobile browser (camera capture supported).

**Consequences (testable):**
- Images display in thread; SMS for new messages uses link-back pattern (not MMS attachment by default).

`[ASSUMPTION: SMS = notification + link; conversation primary in web portal.]`

---

### 5.7 Notifications & Customer Feed

**Description:** SMS/email drive project memberships back to the portal. Customers get schedule-change visibility appropriate to their role.

**Functional Requirements:**

#### FR-16: Schedule change notifications

People with project memberships receive SMS and/or email when schedule proposals or cascades affect them, with magic link to act or view.

**Consequences (testable):**
- Notification content states what changed in plain language.
- Links are signed, scoped to project membership, and expire per security policy.

#### FR-17: Customer what-changed view

Customer project membership can view a simplified timeline of schedule changes for their project (not full Gantt).

**Consequences (testable):**
- Customer sees milestone-level changes; not sub-internal detail.

---

### 5.8 Subscription Billing

**Description:** Contractor subscription pays flat monthly tier; project memberships never pay.

**Functional Requirements:**

#### FR-18: Contractor subscription

Contractor can subscribe to a paid tier (with free tier limits) to use ContractorPro beyond trial/limits.

**Consequences (testable):**
- Billing blocks or limits apply when subscription inactive (exact limits TBD).
- Subcontractor and Customer project memberships are never charged.

`[ASSUMPTION: Stripe Billing or Chargebee; free tier includes at least 1 active project with SMS caps.]`

---

### 5.9 AI-Assisted Comms (Stretch)

**Description:** Reduce Team member admin when schedule moves — draft customer update, summarize thread. **Stretch for v0.1**; not blocking launch.

#### FR-19: Draft update on schedule change (stretch)

Team member can generate a draft message explaining what changed after cascade or major reschedule; Team member must approve before send.

**Consequences (testable):**
- Draft never sends without explicit Team member approval.

---

### 5.10 Identity & Permissions

**Description:** Enforces separation between subscription access and project-scoped roles. Realizes §3 Identity & Roles Model.

#### FR-20: Identity separate from project role

The system must evaluate permissions from **subscription context** (Team member on a Contractor) and **project membership context** (Subcontractor or Customer on a Project) independently. No global "user type" may grant cross-project or subscription access.

**Consequences (testable):**
- Subcontractor membership on Project A grants no access to Project B unless separately invited.
- Customer membership never grants Contractor subscription capabilities.
- Same verified phone may hold multiple project memberships with different roles under different Contractors.

---

## 6. Non-Goals (Explicit)

- Full **job planning** module (phases, durations, buffers, portfolio planner, finalize) — **v0.2**
- Estimating, invoicing, selections, time cards, safety/compliance
- Native iOS/Android apps (responsive web only)
- Microsoft 365 Calendar integration
- Homeowner or Customer subscription tier (Customers are always free project memberships)
- Deep QuickBooks sync
- Commercial construction workflows
- SMS "reply YES" to accept dates
- Subcontractor↔customer direct messaging
- Offline-first mobile DB
- AI estimating, takeoff, or document extraction

---

## 7. MVP Scope

### 7.1 In Scope (v0.1)

- Contractor subscription + Team member auth (OAuth + native fallback)
- Projects + tasks + optional dependencies + optional cascade with preview
- Google Calendar connect; sync **on Subcontractor accept**
- Invite project memberships: Subcontractor and Customer (name + phone join)
- Propose → accept/decline → poke until response
- Team member pending/confirmed/declined dashboard
- Contractor↔sub and Contractor↔customer messaging with image upload
- SMS/email notifications with magic links scoped to project membership
- Customer simplified schedule / what-changed view
- Contractor subscription billing
- Responsive web: Team member desktop-first; project membership mobile-first
- AI draft on schedule change (**stretch**)
- Identity: separate subscription role from per-project Subcontractor/Customer roles

### 7.2 Out of Scope for MVP

| Item | Reason / Target |
|------|-----------------|
| Job planning (phases, buffers, portfolio) | v0.2 — see job-planning-workflow.md |
| Microsoft Calendar | Post-MVP; Google covers most subs |
| Native apps | Web-only strategy |
| SMS relay / virtual group chat | v0.2 opt-in; higher cost/complexity |
| PWA / offline | v0.2+ if validated |
| Multi-team-member permissions | Simplify v0.1; owner + basic staff later |
| Unified Person profile across all projects | v0.2; v0.1 uses per-project memberships keyed by phone |

---

## 8. Success Metrics

**Primary**

- **SM-1:** Team member completes onboarding (connect calendar, create project, invite 1 Subcontractor + 1 Customer, propose 1 date) in **one session** (< 30 min). Validates FR-2, FR-3, FR-4, FR-7.
- **SM-2:** Team member publishes a schedule change (propose or cascade) and affected Subcontractors are notified in **< 2 minutes**. Validates FR-7, FR-11, FR-13.
- **SM-3:** **≥ 70%** of proposed assignments reach `confirmed` within **72 hours** without Team member manual phone chase (pilot cohort). Validates FR-8, FR-11.

**Secondary**

- **SM-4:** Project membership holder opens notification link within **24 hours** (median). Validates FR-16.
- **SM-5:** Qualitative: Contractor reports fewer "when are you coming?" messages vs baseline (discovery interviews). Validates overall wedge.

**Counter-metrics (do not optimize)**

- **SM-C1:** Raw SMS count per project — minimize cost, not maximize pings beyond effective poke cadence.
- **SM-C2:** Feature breadth (modules added) — resist ERP creep in v0.1.

---

## 9. Open Questions

1. v0.1 calendar mode: BYO only, Pro-provided only, or both?
2. Free tier limits: projects count, SMS/month, cascade on/off?
3. Can Subcontractors **decline** without Team member phone call — what UX nudges reschedule?
4. Customer calendar invite (Google ACL) in v0.1 or portal-only?
5. Multiple Team members per Contractor in v0.1 or single owner account?
6. AI draft: ship in v0.1 or defer to v0.2?
7. Customer discovery: validate ICP (2–5 person contractor crews) before public launch.
8. v0.2: link **Person** identity across project memberships (one portal listing all projects for a phone)?

---

## 10. Assumptions Index

- `[ASSUMPTION: On another Contractor's project I may be a Customer instead — role is per project]` — JTBD
- `[ASSUMPTION: I may be a Customer on one project and a Subcontractor on another]` — JTBD
- `[ASSUMPTION: Team member at Contractor X invited as Subcontractor on Contractor Y's project — v0.1 via phone]` — §3.3
- `[ASSUMPTION: v0.1 ships one calendar mode — either BYO link or Pro-provided per project]` — FR-3
- `[ASSUMPTION: v0.1 cascade shifts proposed/confirmed assignments; confirmed become proposed_change]` — FR-13
- `[ASSUMPTION: SMS = notification + link; conversation primary in web portal]` — FR-15
- `[ASSUMPTION: Stripe Billing or Chargebee; free tier with SMS caps]` — FR-18
- `[ASSUMPTION: Primary ICP is 2–5 person residential contractor crews; solo trades adjacent]` — Vision
- `[ASSUMPTION: US only, English, USD for v0.1]` — Scope
- `[ASSUMPTION: Google Calendar sufficient for MVP; Samsung/Apple via Google sync on device]` — Integrations

---

## 11. Related Documents (Technical — Out of Scope for This PRD)

| Document | Purpose |
|----------|---------|
| [addendum.md](./addendum.md) | Index to technical exploration + architecture placeholder |
| `../product-vision.md` | North star (source) |
| `../technical-exploration/schedule-confirmation-workflow.md` | Propose/accept/poke detail |
| `../technical-exploration/invite-join-flow.md` | Join flow detail |
| `../technical-exploration/google-calendar-integration.md` | Calendar integration detail |
| `../technical-exploration/job-planning-workflow.md` | v0.2 planning module |
| `../technical-exploration/messaging-and-media.md` | Messaging & images |
| **Architecture / TRD (TBD)** | Consolidated system design — to be created separately |

---

*Status: **draft** — fast path from existing planning artifacts. Review and mark `final` when v0.1 scope is locked.*
