---
title: ContractorPro
status: draft
created: 2026-08-15
updated: 2026-08-20
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

**UJ-2. Either party can propose a reschedule; the other must re-confirm.**

Dana moves painting Sept 10 → Sept 11 (**contractor-initiated**). Mike's calendar **still shows Sept 10** until he accepts. He gets SMS/email with old → new date and a link. On **Accept**, both calendars update to Sept 11.

Mike can also **request** a different date (**sub-initiated**): he picks Sept 12 from his portal; Dana gets notified. Either party can **counter-propose** (e.g. Dana counters Sept 11) until someone **Accepts** or **Declines** — calendars keep the last confirmed date through the whole thread.

On **Decline**, Dana is notified immediately. She can negotiate with Mike again, or **reassign the task to a different Subcontractor** (Jose) — Mike's assignment closes, calendar event removed, Jose enters the standard propose flow.

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
| Team member at Contractor X invited as Subcontractor on Contractor Y's project | Yes — v0.1 via phone invite + magic link; OAuth session not merged (v0.2 optional) |
| **Contractor subscription holder** (self-registered owner) invited as Subcontractor on another Contractor's project | Yes — subscription role does not block project membership |
| **Contractor subscription holder** invited as Customer on another Contractor's project | Yes — same rule |
| Contractor subscription holder also participates on someone else's project | Yes — via `persons` + project membership, not subscription role |

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
- **Propose** — Either party sets or changes a date on an assignment; the other has not yet accepted.
- **Reschedule** — Change to a previously **confirmed** date; always enters propose/re-confirm flow regardless of who initiated.
- **Confirm / Accept** — Subcontractor or Team member agrees to proposed dates via magic link or dashboard; triggers calendar sync.
- **Counter-propose** — Pending party offers a different date instead of Accept/Decline; pending party flips; negotiation continues.
- **Poke** — Automated reminder (SMS and/or email) until Subcontractor accepts, declines, or Team member stops reminders.
- **Project handle #** — Dedicated phone number per **Project** for MMS routing; shared across all Dana↔sub/customer groups on that job; inbound `To` identifies project.
- **Magic Link** — Signed URL for passwordless access (join, confirm date, view portal).
- **Cascade** — Optional shift of dependent tasks by the same delta when a predecessor moves.
- **Portal** — Mobile-first web experience for project memberships (not a native app).

---

## 5. Features

### 5.1 Contractor Account & Project Setup

**Description:** Team members can sign in (OAuth preferred), work under a **Contractor** subscription, connect Google Calendar, create **Projects**, and add **Tasks** with dates and optional dependencies. Desktop-first experience. Realizes foundation for all UJs.

**Functional Requirements:**

#### FR-1: Team member authentication

Team members can sign in to ContractorPro using **Google OAuth** (Entra External ID) in MVP. Apple, Microsoft, and native passkey/TOTP accounts are **v0.1.1**. Each authenticated user belongs to exactly one **Contractor** subscription in v0.1.

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

Team members connect a Google account on behalf of the Contractor. ContractorPro **creates one shared Google calendar per project** (pro-provided). On Subcontractor or Customer **accept**, agreed dates are written as events on that calendar; subs/customers with email on file receive **Google event attendee invites**. A **portfolio calendar view** in the app shows all projects in one schedule.

**Consequences (testable):**
- Until a Task Assignment is **confirmed**, no agreed-date event is written to the shared project calendar for that assignment.
- Team member can see connection status (connected / disconnected / error).
- Subcontractor/Customer **Apple Calendar connect** is **out of MVP** — v0.1.1.

`[DECISION 2026-08-20: Pro-provided per project; attendee invites for invitees; portfolio UI in app — architecture-v0.1.md §1.6]`

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

**Description:** Core coordination loop. Team member **proposes** dates; either party can **accept, counter-propose, or decline**; calendars update on accept only. On decline, team member may **reassign** to another sub. Realizes UJ-1, UJ-2.

**Functional Requirements:**

#### FR-7: Propose date to assigned subcontractor

Team member can assign a Subcontractor project membership to a Task and **propose** a date. System notifies the Person per `notify_via` on that membership.

**Consequences (testable):**
- Task Assignment status becomes `proposed` (or `proposed_change` on reschedule).
- Notification includes task name, project name, date(s), and Accept/Decline link.
- No "reply YES to SMS" — link-based only.

#### FR-8: Accept, counter-propose, or decline

Person with Subcontractor role (or Team member when sub-initiated) can open magic link or dashboard and respond to a pending proposal on a mobile-friendly page without installing an app.

**Response options:** **Accept** | **Counter-propose** (suggest different date) | **Decline**.

**Consequences (testable):**
- **Accept:** status → `confirmed`; shared project calendar event created/updated; other party notified in-app.
- **Counter-propose:** proposed dates update; `pending_party` flips to the other party; negotiation history appended; poke timer resets for new pending party; other party notified.
- **Decline:** status → `declined`; other party notified in-app (decline alert on by default); calendar unchanged for proposed date; Team member may reassign per FR-9a.

#### FR-9: Reschedule requires re-confirmation (either direction)

When **either** Team member or Subcontractor changes dates on a previously **confirmed** assignment, the **other party** must re-confirm. Calendars show last **confirmed** date until re-accept.

**Consequences (testable):**
- Status → `proposed_change` after date edit on confirmed assignment (regardless of initiator).
- Assignment records **who proposed** the change (`team_member` | `subcontractor`).
- **Team member initiated:** Subcontractor notified via SMS/email with old → new date and magic link; poke reminders per FR-11.
- **Subcontractor initiated:** Team member notified in-app (optional SMS); Subcontractor sees "pending contractor approval."
- **Accept:** status → `confirmed`; calendars update to new dates; other party notified.
- **Counter-propose:** same as FR-8 counter-propose (applies to `proposed_change` and initial `proposed`).
- **Decline:** other party notified; last confirmed dates remain on calendars until reassignment or new proposal is agreed.

#### FR-9a: Reassign task after decline

When a Task Assignment is `declined`, Team member can assign the same Task to a **different** Subcontractor project membership without creating a duplicate open assignment.

**Consequences (testable):**
- Declined assignment is closed (terminal); poke reminders stop.
- Any **confirmed** calendar event for the declined Subcontractor on that Task is removed.
- New Task Assignment created for replacement Sub → `proposed`; standard notify + poke cycle (UJ-3+ if first task for that sub on project).
- Assignment history preserved on Task (declined sub + replacement visible to Team member).
- Declined Subcontractor remains on project if they have other tasks; only this assignment closes.

#### FR-10: Contractor confirmation dashboard

Team member can view all Task Assignments filtered by status: pending, confirmed, declined; and see per-sub pending summary ("who is holding me up").

**Consequences (testable):**
- Pending items show time since last notification, reminder count, and **negotiation thread** (counter-propose history) where applicable.
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

### 5.6 Messaging & Photos (MMS-first) + Scheduling (app-first)

**Description:** **Two lanes:** (1) General conversation via group MMS per relationship — ingested and mirrored in web. (2) **Scheduling** — Team member manages dates, dependencies, cascade, and reassignment in the **web app** (multi-job portfolio); system sends confirmation MMS/SMS with magic links after commits. Realizes UJ-8, UJ-4.

**Functional Requirements:**

#### FR-14: MMS group thread per relationship

When Team member invites or assigns a Subcontractor or Customer, system provisions a **project handle #** (dedicated phone number per project) and documents a **group MMS thread**: Team member's phone + that membership's phone + project handle. All MMS in the group is ingested and visible in the web app under that project and thread.

**Consequences (testable):**
- **One handle # per project** — provisioned on project create; shared across all relationship groups on that job.
- Inbound routing: `To` (handle #) → project; `From` (sender phone) → project membership.
- Each relationship stores `conversation_sid` or internal `mms_thread_id` at provision time.
- Separate group per relationship — no shared sub↔sub or sub↔customer thread.
- Team member is decision maker; coordinates between threads manually.
- Subcontractors and Customers communicate via native Messages app, not portal-first.
- Team member can view full thread history in web dashboard; optional reply from web → MMS to group.
- Outbound system messages use `[Project · Contractor]` prefix plus handle # for delivery.

#### FR-15: MMS and web images

Photos sent as **MMS** in group threads are stored (blob) and displayed in the web thread mirror. Team member, Subcontractor, and Customer may also attach photos via web portal when using magic-link session.

**Consequences (testable):**
- MMS images ingested from group threads appear in app alongside text.
- System schedule messages (propose, poke, confirm) sent via MMS/SMS into the relationship thread or from handle #.

`[DECISION: v0.1 conversation primary in group MMS; web app for capture + Dana schedule actions; confirm via magic link in MMS.]`

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

**Description:** Contractor subscription pays flat monthly tier by **concurrent active projects with outbound comms**; project memberships never pay. **Billing vendor:** Stripe Billing (Checkout + Customer Portal + webhooks). Decision: 2026-08-19 — [discovery-log.md](../../discovery-log.md).

**Phasing:**
- **MVP (Phase 1):** Self-serve signup and full coordination — **no payment, no tier enforcement** (beta / design partners).
- **Post-MVP (Phase 2):** Stripe self-serve subscribe + entitlement gates (free sandbox vs paid).

**Functional Requirements:**

#### FR-18: Contractor subscription & entitlements

Contractor can self-serve sign up, use the product on a **free sandbox** tier (plan-only), and **subscribe via Stripe** to unlock outbound coordination and concurrent active project slots.

**Tier model (locked — Phase 2):**

| Tier | Price | Concurrent active projects (comms enabled) | Outbound comms |
|------|-------|------------------------------------------|----------------|
| **Sandbox** | $0 | Unlimited plan-only projects | Blocked |
| **Pro 5** | $100/mo | 5 | Enabled |
| **Pro 10** | $200/mo | 10 | Enabled |
| **Pro 15+** | +$100/mo per +5 slots | Linear | Enabled |

**Sandbox — allowed without payment:**
- OAuth sign-up, company profile, create/edit projects and tasks
- Internal schedule layout, dependencies, cascade **preview** (no publish)

**Sandbox — blocked until subscribed (Phase 2):**
- Sub invite (any path), customer outbound confirm (email/MMS), propose/notify dates, poke, cascade **publish**, MMS threads, any SMS/MMS send
- Entering customer contact on project **does not** auto-send until subscribed (defer H-1 notify)

**Paid tier — over limit:** 6th concurrent active project on Pro 5 → **plan-only mode** or prompt upgrade before enabling comms (locked 2026-08-20 §A).

**MVP exception (Phase 1):** All outbound coordination enabled for every signed-up Contractor — no Stripe, no gates — to validate core loop with design partners.

**Consequences (testable):**
- Subcontractor and Customer project memberships are never charged.
- Phase 2: `invoice.payment_failed` → grace banner → **messaging_suspended** on tenant per admin journeys A-6/A-17.
- Phase 2: Stripe Customer Portal linked from Settings for card update, cancel, invoices.
- Entitlement checks centralized (middleware/service) — not scattered in UI only.

---

### 5.9 AI-Assisted Comms (Deferred — v0.2+)

**Description:** Reduce Team member admin when schedule moves — draft customer update, summarize thread, interpret SMS intent. **Not in v0.1** (2026-08-17 decision).

#### FR-19: Draft update on schedule change (v0.2+)

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

### 7.1 In Scope (MVP — Phase 1)

**Goal:** Self-enrolling Contractors run real jobs end-to-end — **no payment or tier gates.**

- **Self-serve signup:** Google OAuth → create Contractor company → guided onboarding (C-1)
- Contractor + Team member auth (**Google OAuth only** in MVP; native fallback v0.1.1)
- Projects + tasks + optional dependencies + **cascade with preview** (MVP)
- Google Calendar connect; **pro-provided calendar per project**; sync **on accept** via event attendee invites
- **Portfolio calendar view** across all active projects (in-app)
- Invite project memberships: Subcontractor and Customer (name + phone join)
- Propose → accept/**hard decline** → poke until response; reassign after decline (E5-S3b)
- Team member pending/confirmed/declined dashboard
- Contractor↔sub and Contractor↔customer **MMS group threads** (handle #) with ingest + web mirror
- MMS/SMS confirmation and poke messages after Team member schedule actions
- Image capture from MMS + web upload
- Customer simplified schedule / what-changed view
- **Platform-global STOP/opt-out** handling (API + Twilio; no admin UI in M1)
- Responsive web: Team member desktop-first; magic-link pages mobile-first for confirm/join
- Identity: separate subscription role from per-project Subcontractor/Customer roles
- **Data model hooks** for `subscription_tier`, `comms_enabled`, Stripe IDs — defaults to full access in MVP

### 7.1b Post-MVP (Phase 2 — Billing & entitlements)

Ship immediately after MVP validates coordination loop:

- **Stripe Billing:** Checkout, webhooks, Customer Portal (FR-18)
- **Free sandbox tier:** plan-only; paywall on first outbound comms (C-27)
- **Paid tiers:** ~$100/mo per 5 concurrent active projects (comms enabled), linear
- Dunning → tenant **messaging_suspended** (A-6, A-17)
- Upgrade prompts at invite sub, notify customer, plan cap

### 7.2 Out of Scope for MVP

- AI (SMS intent parsing, draft messages, auto-schedule from chat) — **v0.2+**

| Item | Reason / Target |
|------|-----------------|
- **Stripe Billing & paid tiers** | Phase 2 — immediately post-MVP; design locked in FR-18 |
| **Apple Calendar (invitee connect)** | v0.1.1 — MVP uses Google attendee invites only |
| **Admin `/admin` UI** | Phase 2 — M1 uses API + Twilio/DB manual ops |
| **Free-tier outbound comms gate** | Phase 2 — MVP runs full access for beta |
| Job planning (phases, buffers, portfolio) | v0.2 — see job-planning-workflow.md |
| Microsoft Calendar | Post-MVP; Google covers most subs |
| Native apps | Web-only strategy |
| AI (drafts, SMS intent) | v0.2+ |
| PWA / offline | v0.2+ if validated |
| Multi-team-member permissions | Simplify v0.1; owner + basic staff later |
| Unified Person profile across all projects | v0.2 portal UI; v0.1 uses global `persons` by phone + per-project `project_memberships` (magic link per membership) |

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

1. ~~v0.1 calendar mode~~ — **Resolved 2026-08-20:** Pro-provided Google calendar per project; portfolio UI in app; subs/customers via event attendee invites — [architecture-v0.1.md](../../architecture-v0.1.md) §1.6
2. ~~Free tier limits~~ — **Resolved 2026-08-19:** Sandbox = plan-only; paid = ~$100/5 concurrent active projects with comms; MVP bypasses gates — FR-18
3. ~~Can Subcontractors **decline** without Team member phone call~~ — **Resolved:** Hard decline in-app; reassign (E5-S3b). `[OPEN: courtesy SMS to removed sub?]`
4. ~~Customer calendar invite~~ — **Resolved 2026-08-20:** Google event attendee invite when email on file; portal-only otherwise
5. Multiple Team members per Contractor in v0.1 or single owner account?
6. ~~AI draft~~ — **Deferred v0.2+** (2026-08-17)
7. Customer discovery: validate ICP (2–5 person contractor crews) before public launch.
8. ~~v0.2: link **Person** identity across project memberships~~ — **Resolved 2026-08-20:** Global `persons` row per `phone_e164` in v0.1; unified multi-project portal UI remains **v0.2** (FJ-4) — [architecture-v0.1.md](../../architecture-v0.1.md) §4.3
9. ~~GC auth providers~~ — **Resolved 2026-08-20:** Google only M1; Apple/Microsoft/native v0.1.1
10. ~~Cascade in MVP~~ — **Resolved 2026-08-20:** Yes — E7 in MVP build order
11. ~~6th project on Pro 5~~ — **Resolved 2026-08-20:** Plan-only mode (Phase 2)
12. ~~Annual pricing~~ — **Resolved 2026-08-20:** ~2 months free on annual at Phase 2 (tune later)

---

## 10. Assumptions Index

- `[ASSUMPTION: On another Contractor's project I may be a Customer instead — role is per project]` — JTBD
- `[ASSUMPTION: I may be a Customer on one project and a Subcontractor on another]` — JTBD
- `[ASSUMPTION: Contractor subscriber may be Sub or Customer on another Contractor's project — separate identity planes in v0.1]` — §3.3
- `[DECISION 2026-08-20: Pro-provided Google calendar per project; attendee invites for invitees]` — FR-3
- `[DECISION 2026-08-20: Google OAuth only for team members in MVP]` — FR-1
- `[DECISION 2026-08-20: Hard decline on E5-S3; reassign via E5-S3b]` — FR-8
- `[ASSUMPTION: v0.1 cascade shifts proposed/confirmed assignments; confirmed become proposed_change]` — FR-13
- `[ASSUMPTION: SMS = notification + link; conversation primary in web portal]` — FR-15
- `[DECISION: Stripe Billing; sandbox free plan-only; ~$100/5 concurrent active projects — Phase 2 enforcement]` — FR-18
- `[DECISION 2026-08-20: ~2 months free on annual at Phase 2]` — FR-18
- `[ASSUMPTION: Primary ICP is 2–5 person residential contractor crews; solo trades adjacent]` — Vision
- `[ASSUMPTION: US only, English, USD for v0.1]` — Scope
- `[DECISION 2026-08-20: Google Calendar for MVP; Apple device users may sync via Google invite to iOS Calendar app]` — Integrations

---

## 11. Related Documents (Technical — Out of Scope for This PRD)

| Document | Purpose |
|----------|---------|
| [addendum.md](./addendum.md) | Index to technical exploration + architecture |
| [../../architecture-v0.1.md](../../architecture-v0.1.md) | **Architecture / TRD v0.1** — consolidated system design |

---

*Status: **draft** — fast path from existing planning artifacts. Review and mark `final` when v0.1 scope is locked.*
