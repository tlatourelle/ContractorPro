# Schedule Confirmation Workflow — Propose, Accept, Sync

Status: **Exploratory** (2026-08-14)  
Related: [product-vision.md](../product-vision.md), [google-calendar-integration.md](./google-calendar-integration.md), [invite-join-flow.md](./invite-join-flow.md), [messaging-and-media.md](./messaging-and-media.md), [job-planning-workflow.md](./job-planning-workflow.md)

## Product intent (from stakeholder)

When the GC sets or changes a date for an assigned sub, the sub must **actively accept** before the date is considered agreed. On accept, **both calendars update**. The GC is notified of accept/decline and can easily see **who has pending confirmations**.

> “I set Painting — XYZ Kitchen on Sept 10. Sub is notified and needs to accept. Then it's on my calendar and theirs. If I bump to Sept 11, they need to be notified, accept again, and both calendars update.”

This is the **schedule coordination layer** — commitment + visibility, not passive calendar pushes.

**Calendar invites are not enough.** Google Calendar ACL invites and event updates do **not** chase subs who ignore them. Buildertrend wins here with **automated daily reminders** until subs respond. ContractorPro must own **poke / follow-up automation** in our system — SMS and/or email on a schedule until accept, decline, or GC intervenes.

---

## Core principle: propose → accept → sync

ContractorPro is the **source of truth** for project schedule. Google Calendar reflects **agreed** dates only.

**Planning happens first.** Sold jobs are built in **plan mode** (phases, durations, buffers, what-if dates) with no sub notifications and no Google writes until the GC **finalizes** — see [job-planning-workflow.md](./job-planning-workflow.md). This document covers everything **after finalize**.

| Step | Who | What happens |
|------|-----|--------------|
| 1. Propose | GC | Sets or changes a task date; status → `proposed` or `proposed_change` |
| 2. Notify | System | SMS and/or email to assigned sub with signed magic link |
| 3. Decide | Sub | Taps **Accept** or **Decline** on mobile-friendly page (no “reply YES” to SMS) |
| 4. Sync | System | On **Accept** only → update shared project Google Calendar event |
| 5. Inform GC | System | In-app notification; optional SMS/email to GC (declines higher priority) |

**Not in scope for v0.1:** SMS reply parsing (“text YES to confirm”) — link-based accept/decline only.

---

## Task assignment status model

Each **task assignment** (task + sub) has its own confirmation state:

| Status | Meaning | GC calendar | Sub calendar |
|--------|---------|-------------|--------------|
| `proposed` | New date set; awaiting sub | Tentative / pending indicator | Not updated yet |
| `confirmed` | Sub accepted | Confirmed event | Confirmed event |
| `proposed_change` | GC rescheduled; awaiting re-confirm | New date shown as pending | **Last confirmed date** until accept |
| `declined` | Sub declined | Flagged; needs GC action | Unchanged |

### Date fields

```
task_assignments
  proposed_start, proposed_end     -- current proposal (may differ from confirmed)
  confirmed_start, confirmed_end   -- last agreed dates (null until first accept)
  proposed_at, confirmed_at, declined_at
  status                           -- proposed | confirmed | proposed_change | declined
```

**Rule:** Do **not** patch the shared Google Calendar to a new date until status returns to `confirmed`.

---

## Flow 1 — GC proposes a new date

### 1. GC sets date

From project schedule: “Painting — XYZ Kitchen — Sept 10” → assign to Mike (Tile).

### 2. System actions

- Create/update `task_assignment` with `status = proposed`
- Show on **GC schedule view** as *Pending sub confirmation*
- **Do not** write confirmed event to shared project calendar yet (optional: GC-only tentative marker — TBD)
- Queue notification(s) per sub `notify_via` preference
- Log in `notification_log`

### 3. Sub notification (SMS and/or email)

**SMS example:**

```
[Smith Remodeling] New date proposed: Painting — XYZ Kitchen on Sept 10.
Accept or decline: https://app.contractorpro.com/c/abc123
```

**Email example:**

- **Subject:** `Smith Remodeling — confirm Sept 10 for Painting (XYZ Kitchen)`
- Body: task, project, proposed date, **Accept** / **Decline** buttons (signed HTTPS links)
- Plain link fallback for clients that strip buttons

Same confirm page regardless of channel.

### 4. Sub lands on confirm page (mobile-first)

```
Smith Remodeling — Maple St remodel

Painting — XYZ Kitchen
Proposed: Wednesday, Sept 10

[ Accept ]     [ Decline ]
```

No app install. No password. Magic link doubles as re-auth (see [invite-join-flow.md](./invite-join-flow.md)).

### 5a. Sub accepts

1. `status` → `confirmed`; copy `proposed_*` → `confirmed_*`
2. `CalendarSyncService.upsertEvent()` on project calendar (see [google-calendar-integration.md](./google-calendar-integration.md))
3. Sub sees event on phone calendar (Google / Samsung / Apple via Google sync)
4. Notify GC (in-app; optional SMS/email)
5. Invalidate confirmation token(s)

**Success screen:** “You’re confirmed for Sept 10. This is on your calendar.”

### 5b. Sub declines

1. `status` → `declined`
2. **No** calendar update to proposed date
3. Notify GC immediately (in-app + recommend SMS on by default)
4. GC dashboard shows ❌ — needs new date or conversation

---

## Flow 2 — GC reschedules (bump Sept 10 → Sept 11)

### 1. GC changes date

Drag task or edit date in app.

### 2. System actions

- If previously `confirmed`: `status` → `proposed_change`
- Update `proposed_start` / `proposed_end` to Sept 11
- **Keep** `confirmed_*` at Sept 10 until sub accepts
- GC view: Sept 11 as *Pending sub confirmation*
- Sub calendar: **still Sept 10** (last agreed date)
- Notify sub via SMS and/or email

**SMS example:**

```
[Smith Remodeling] Date change: Painting — XYZ Kitchen
Sept 10 → Sept 11. Please confirm: https://app.contractorpro.com/c/xyz789
```

### 3. Sub accepts reschedule

- `status` → `confirmed`; `confirmed_*` → Sept 11
- `events.patch` on Google → both calendars show Sept 11
- Notify GC

### 4. Sub declines reschedule

- `status` → `declined` (or revert to `confirmed` at Sept 10 — **product decision**; lean: stay `confirmed` at last agreed dates, flag decline for GC)
- Notify GC urgently
- GC must negotiate new date

---

## Notification channels (sub)

Accept/decline is always **link-based**. SMS and email are **delivery options**, not different workflows.

### Per-participant preference

Captured at invite or editable by GC:

| Field | Required | Notes |
|-------|----------|-------|
| `phone_e164` | ✅ | Identity + magic links (see invite flow) |
| `email` | Optional | Enables email notifications |
| `notify_via` | ✅ | `sms` \| `email` \| `both` |

**Defaults:**

- Phone only on file → `sms`
- Phone + email, no preference → `both` or company default
- Company setting: default `notify_via` for new subs

### On propose / reschedule

```
NotificationService.notifyAssignmentProposal(assignment)
  → read participant.notify_via
  → send SMS and/or email with same-purpose magic link
  → log channel(s) in notification_log
```

| `notify_via` | Delivery |
|--------------|----------|
| `sms` | Text with link |
| `email` | Email with Accept/Decline buttons |
| `both` | Both; first valid Accept/Decline wins |

---

## Automated follow-up (“poke”) engine

**Product requirement:** When a sub does not respond to a proposed or rescheduled date, ContractorPro **keeps reminding them** — daily by default — until they accept, decline, or the GC cancels/snoozes. This is **not** delegated to Google Calendar, iCal feeds, or email clients.

### Why calendar alone fails

| Mechanism | Reminds non-responders? | Accept/decline capture? |
|-----------|-------------------------|-------------------------|
| Google Calendar invite / ACL | ❌ No | ❌ No |
| iCal / webcal subscribe | ❌ No (passive feed) | ❌ No |
| **ContractorPro poke engine** | ✅ Yes | ✅ Yes (magic link) |

Buildertrend ships with **auto-notify, schedule reminders, and confirmation requests** as default behavior. We match or beat that persistence without requiring subs to install an app.

### Default reminder cadence (v0.1)

Configurable per company; these are **defaults**:

| When | Who gets poked | Channel | Notes |
|------|----------------|---------|-------|
| **Immediately** | Sub | SMS and/or email per `notify_via` | Initial proposal or reschedule |
| **+24h** no response | Sub | Preferred channel | “Still waiting for your confirmation…” |
| **+48h** | Sub | **Both** if available | Stronger nudge |
| **Daily thereafter** | Sub | Preferred channel (or both after day 3) | **BT-style daily poke** until resolved |
| **+48h** | GC | In-app (+ optional SMS) | “Mike hasn’t confirmed Painting — 2 days” |
| **+72h** | GC | In-app + recommend SMS | Escalation — GC may call sub |
| **Each poke** | GC | In-app only | “Reminder #3 sent to Mike” (audit, not spam) |

**Stop conditions** (cancel pending reminders):

- Sub **accepts** or **declines**
- GC **cancels** proposal or **reassigns** task
- GC **snoozes** reminders (e.g. “I talked to him, poke again Friday”)
- Task or project **archived**

### Quiet hours

- Default: **no SMS to subs 8pm–8am** in project timezone (queue for next morning)
- Email reminders may still send overnight (lower intrusion) — configurable
- GC can override per company

### Don’t spam: batching rules

If one sub has **multiple pending** tasks on the same project:

| Rule | Behavior |
|------|----------|
| **Same day, multiple pokes** | **One SMS** listing all pending items + single link to batch confirm page |
| **Reschedule + new proposal** | Include all open items in next daily digest |
| **Max SMS per sub per day** | Default **1** reminder SMS/day (plus initial notification on propose) |

**SMS example (daily poke, batched):**

```
[Smith Remodeling] Still need your confirmation (2 items):
• Painting — XYZ Kitchen — Sept 10
• Touch-up — Sept 12
Confirm: https://app.contractorpro.com/c/batch789
```

Sub lands on a page listing all pending assignments with Accept/Decline per task (or Accept all).

### GC controls

| Action | Effect |
|--------|--------|
| **Send reminder now** | Manual poke outside schedule |
| **Snooze reminders** | Pause auto-poke for N days |
| **Stop reminding** | GC owns follow-up offline (logged) |
| **Change cadence** | Company setting: daily / every 2 days / aggressive (12h) |

Company settings example:

```
Confirmation reminders: [ Daily until resolved ▼ ]
Escalate to GC after: [ 2 days ▼ ]
Quiet hours: 8pm – 8am
```

### GC dashboard: poke visibility

```
Mike (Tile) — 3 reminders sent — last: today 9:00am — STILL PENDING
  Painting — XYZ Kitchen — Sept 10 (proposed 4 days ago)
  [ Send reminder now ]  [ Snooze 2 days ]  [ Call logged ]
```

Filter: **Needs follow-up** = pending + last poke > 24h OR any pending > 48h.

### Technical sketch

```
reminder_schedules
  id
  task_assignment_id
  next_send_at
  reminder_count
  snoozed_until
  stopped_at
  stop_reason              -- accepted | declined | gc_snooze | gc_stop | reassigned

ConfirmationReminderWorker (Azure Function / background job)
  → every 15–60 min: find due reminders
  → respect quiet hours + batching
  → send via NotificationService
  → increment reminder_count; schedule next (default +24h)
  → if threshold: notify GC (escalation)
  → log notification_log event_type = reminder | escalation
```

On **propose** or **reschedule**: create `reminder_schedule` with `next_send_at = now + 24h` (after initial notification).

On **accept/decline**: `stopped_at = now`.

**Idempotency:** Worker uses `reminder_schedule.id` + date bucket so retries don’t double-send.

### Tier / cost note

Daily SMS pokes have **per-message cost** (~$0.01+). Factor into tier limits (e.g. free tier: initial + 2 reminders; paid: unlimited daily until resolved). See monetization open questions.

### MVP checklist (poke engine)

- [ ] `reminder_schedules` table + background worker
- [ ] Default daily poke until accept/decline
- [ ] Quiet hours for SMS
- [ ] Batch multiple pending tasks into one daily SMS
- [ ] GC escalation at 48h/72h
- [ ] Manual “Send reminder now” + snooze
- [ ] GC dashboard: reminder count + last poke time

---

## GC notifications (accept / decline)

GC must always know when subs respond.

| Event | In-app | SMS/email to GC |
|-------|--------|-----------------|
| Sub **accepts** | ✅ Always | Configurable (company setting) |
| Sub **declines** | ✅ Always | **Recommend on by default** |
| Sub **pending 24h+** | Dashboard badge + poke scheduled | Optional digest |
| Sub **pending 48h+** | Escalation alert | Recommend SMS |
| **Reminder sent to sub** | Audit log entry | — |

**In-app examples:**

- “Mike confirmed Painting — XYZ Kitchen, Sept 10”
- “Jose declined Rough electric — Sept 12”

**Optional SMS to GC:**

```
[ContractorPro] Mike confirmed: Painting — XYZ Kitchen, Sept 10.
```

```
[ContractorPro] Jose DECLINED: Rough electric — Sept 12. Open: [link]
```

---

## GC dashboard — pending vs confirmed

### Project schedule view

Each row shows assignment confirmation status:

```
Task                    Date        Assigned    Status
─────────────────────────────────────────────────────────
Painting — XYZ Kitchen  Sept 10     Mike        ✅ Confirmed (2h ago)
Rough electric          Sept 12     Jose        ⏳ Pending (sent 4h ago)
Drywall                 Sept 14     Carlos      ❌ Declined — needs new date
```

**Filters:** All | **Pending confirmation** | Confirmed | Declined

### “Who’s holding me up?” panel

Aggregate by sub:

```
Pending your subs (2)
  Jose (Electric)     1 task pending — Rough electric, Sept 12
  Carlos (Drywall)    1 declined — Drywall, Sept 14

All confirmed (3 subs, 8 tasks)
```

### Pre-save preview (cascade-aware)

Before GC confirms a schedule change that affects multiple tasks:

```
Saving will notify 2 subs:
  Mike — reschedule, needs re-confirm
  Jose — new date, needs confirm

[ Save & notify ]
```

Ties to async cascade worker (see [external-mvp-roadmap-review.md](./external-mvp-roadmap-review.md)).

---

## Calendar sync rules

Sync to Google happens **on Accept only** (not on propose).

| Event | Calendar action |
|-------|-----------------|
| First accept (`proposed` → `confirmed`) | `events.insert` on project calendar |
| Accept reschedule (`proposed_change` → `confirmed`) | `events.patch` to new dates |
| Decline | No change to shared calendar (or remove tentative if any) |
| GC proposes (pending) | **No** patch to shared confirmed event |

**Sub calendar access:** Email ACL on shared project calendar (no sub OAuth required for MVP). See [google-calendar-integration.md](./google-calendar-integration.md).

**Provider abstraction:** Calendar writes go through `CalendarProvider` adapter; MVP = Google only. Samsung / Apple calendars receive updates via Google sync on the sub’s device.

---

## Magic link security

| Property | Recommendation |
|----------|----------------|
| **Purpose** | Scoped to `assignment_id` + `action=confirm` |
| **Signing** | HMAC or JWT; include `participant_id`, `project_id`, `expires` |
| **TTL** | 72h default; refresh on resend/reminder |
| **Single use** | Invalidate on Accept/Decline; invalidate sibling tokens if `both` sent |
| **Scope** | Sub link ≠ homeowner link; assignment-scoped |

---

## Data model sketch

```
task_assignments
  id
  task_id
  participant_id              -- sub (project_participants)
  status                      -- proposed | confirmed | proposed_change | declined
  proposed_start, proposed_end
  confirmed_start, confirmed_end
  proposed_at, confirmed_at, declined_at
  google_event_id             -- set/updated on confirm only
  last_notification_at

project_participants
  ... (existing fields)
  notify_via                  -- sms | email | both

confirmation_tokens
  id
  task_assignment_id
  participant_id
  token_hash
  channel                     -- sms | email
  purpose                     -- propose | reschedule
  expires_at
  used_at

notification_log
  id
  task_assignment_id
  participant_id
  channel                     -- sms | email | in_app
  direction                   -- outbound | inbound_ack (future)
  event_type                  -- proposed | reminder | escalation | accepted | declined | gc_notified
  sent_at
  provider_message_id
```

---

## Integration with cascade

When cascade shifts multiple dependent tasks:

1. GC previews affected subs and pending re-confirms
2. GC confirms → worker updates `proposed_*` on affected assignments
3. Each affected sub gets notification(s); each assignment needs its own accept
4. Calendar events update **per assignment** as each sub accepts (not batch on GC save)

**Open:** Should cascade auto-propose all shifted tasks, or only tasks with assigned subs?

---

## Homeowners

**v0.1 lean:** Homeowners get **view-only** schedule in portal; no accept/decline workflow unless discovery demands it.

Optional later: milestone invites with read-only calendar ACL (no confirmation).

---

## MVP checklist

- [ ] `task_assignments` with status model + proposed/confirmed dates
- [ ] `notify_via` on `project_participants` + company default
- [ ] Propose flow: GC sets date → notify sub(s) → pending on dashboard
- [ ] Sub confirm page: Accept / Decline via magic link (mobile-first)
- [ ] On Accept: Google Calendar upsert + GC in-app notification
- [ ] On Decline: GC in-app + default SMS notification
- [ ] Reschedule → `proposed_change`; sub calendar holds last confirmed until accept
- [ ] GC dashboard: pending / confirmed / declined filters + “who’s holding me up”
- [ ] Pre-save preview for multi-sub changes
- [ ] **Automated poke engine** — daily reminders until accept/decline; quiet hours; batching
- [ ] GC escalation + manual “send reminder now” / snooze
- [ ] `reminder_schedules` + background worker
- [ ] `notification_log` for audit

### Later

- [ ] GC “force confirm” override (logged; emergency use)
- [ ] Batch accept for sub with multiple tasks on same day
- [ ] Homeowner milestone notifications
- [ ] GC email digest of pending confirmations

---

## Open questions

### Product

- [ ] On decline of reschedule: revert to `confirmed` at old dates automatically, or explicit `declined` state?
- [ ] Reminder cadence defaults: daily vs every 2 days — validate with GCs
- [ ] Tier limits on reminder SMS volume per project/month?
- [ ] GC tentative events on personal calendar while pending — show or app-only?
- [ ] Is accept **required** before GC considers schedule “locked”? (Lean: yes for assigned subs)
- [ ] Homeowner: any confirmation workflow in v0.2?

### Technical

- [ ] One confirmation token per notification vs reusable until expiry?
- [ ] Email provider (SendGrid, ACS, etc.) — align with auth magic-link email
- [ ] Idempotency: double-click Accept on slow network

---

## Relation to wedge

| Competitor pattern | ContractorPro |
|--------------------|---------------|
| Passive iCal feed (Buildertrend) | Active propose → accept with audit trail |
| Sub must use platform daily | Sub taps link once; calendar updates on accept |
| GC doesn’t know who saw the change | Dashboard: pending / confirmed / declined per sub |
| Calendar is source of truth | **ContractorPro** is source of truth; calendar = agreed dates |
| **BT auto-reminds until sub responds** | **Daily poke engine** (SMS/email) — not Google Calendar’s job |

Log decisions in [discovery-log.md](../discovery-log.md).
