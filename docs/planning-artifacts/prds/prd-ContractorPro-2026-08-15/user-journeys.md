# ContractorPro v0.1 — User Journeys (Working)

Status: **Draft** — walk through and refine with product owner  
Related: [prd.md](./prd.md) · [epics-and-stories.md](./epics-and-stories.md) (implementation tickets — separate concern)

## SME review (persona-split)

For workshop review with SMEs, use the bulleted lists in separate files:

| Persona | File |
|---------|------|
| Contractor (GC team member) | [user-journeys/contractor-journeys.md](./user-journeys/contractor-journeys.md) |
| Subcontractor | [user-journeys/subcontractor-journeys.md](./user-journeys/subcontractor-journeys.md) |
| Customer / homeowner | [user-journeys/customer-journeys.md](./user-journeys/customer-journeys.md) |
| App / site admin (platform ops) | [user-journeys/admin-journeys.md](./user-journeys/admin-journeys.md) |

Index: [user-journeys/README.md](./user-journeys/README.md) · Backlog: [user-journeys/backlog.md](./user-journeys/backlog.md)

This document is the **full-detail** reference (step tables, system behavior, epic mapping). The persona files are the **review pack**.

**Example cast:** Ryan (contractor), Maci (office manager), Jesse & Marcus (subs), Lauren & Erin (customers). See [user-journeys/README.md](./user-journeys/README.md).

## How to read this doc

**User journeys** = step-by-step stories of real people moving through the product — what they see, tap, and feel at each moment.

This is **not** the same as dev user stories (E5-S2, etc.). Journeys inform the PRD and UX; stories break journeys into buildable slices.

Each journey includes:
- **Cast** — who is involved and their role *on this project*
- **Trigger** — what kicked it off
- **Steps** — numbered, multi-party where needed
- **Success** — how they know it worked
- **Branches** — what if it goes wrong

---

## Journey map (v0.1 MVP)

**Review order:** Start with **sub invite** (UJ-3) — nothing else works if subs won't join.

| ID | Journey | Primary actor | MVP? |
|----|---------|---------------|------|
| **UJ-0** | Contractor gets started | Team member | Yes (assume Ryan exists) |
| **UJ-3** | **Sub invited — first time ever** | Team member + Subcontractor | Yes — **critical path** |
| **UJ-3r** | **Sub invited — phone already known** | Team member + Subcontractor | Yes — **critical path** |
| **UJ-3+** | Invite + propose date in one step | Team member + Subcontractor | Yes — recommended default |
| **UJ-1** | Propose a date → Sub confirms | Team member + Subcontractor | Yes — core |
| **UJ-1b** | Sub doesn't respond (poke) | Team member + Subcontractor | Yes — core |
| **UJ-2** | Reschedule → other party re-confirms (**either direction**) | Team member + Subcontractor | Yes |
| **UJ-2a** | Contractor reschedules → Sub re-confirms | Team member + Subcontractor | Yes |
| **UJ-2c** | Sub requests reschedule → Contractor re-confirms | Team member + Subcontractor | Yes |
| **UJ-2b** | Sub declines a date | Team member + Subcontractor | Yes |
| **UJ-2d** | Counter-propose (negotiate dates) | Team member + Subcontractor | Yes |
| **UJ-2e** | Sub declines → Contractor assigns different sub | Team member + Subcontractor | Yes |
| **UJ-3b** | Primary customer — project create + dual email/MMS confirm | Team member + Customer | Yes |
| **UJ-3c** | Additional customer member invite (family) | Team member + Customer | Yes |
| **UJ-4** | Customer accepts schedule change (calendar or in-system) | Team member + Customer | Yes |
| **UJ-5** | Cascade after a slip | Team member + Subcontractors | Yes (if cascade in MVP) |
| **UJ-6** | Photo in thread (MMS or web) | Any project role | Yes |
| **UJ-7** | Same person, different roles on different jobs | Person (cross-project) | v0.1 lean / v0.2 polish |
| **UJ-8** | MMS group thread (primary comms) | Team member + Subcontractor / Customer | Yes — **critical path** |
| **UJ-9** | **End-to-end schedule slip** (cross-persona; composes UJ-4, UJ-5, UJ-8) | Team member + Sub + Customer | Yes — **SME walkthrough** → [§ UJ-9](#uj-9-end-to-end-schedule-slip-cross-persona) |

**Note:** UJ-9 is not a separate feature — it is a **walkthrough** that stitches other journeys together. Review it last, after UJ-3+ → UJ-1 → UJ-3b → UJ-4 → UJ-5.

---

## Design principle: Subs are never "in the system"

Jesse does **not** create an account. He does **not** download an app. He does **not** get a global login.

| What Jesse has | What Jesse does **not** have |
|---------------|-----------------------------|
| A **project membership** on Maple St as **Subcontractor** | A ContractorPro "user account" |
| A verified **phone** on that membership | A password |
| Magic links that work for **this project** | Access to other contractors' projects |

**"Already in the system"** really means: **we've seen this phone before** on some other project membership. UX can be faster — but it's always a **new join to this project**, not "log into your account."

Ryan never needs to know or care whether Jesse is new. He enters name + phone → Send.

### Design principle: **two lanes** — MMS for talk, app for schedule

| Lane | Channel | Who | What |
|------|---------|-----|------|
| **Conversation** | Group MMS (Ryan + sub/customer + handle #) | Everyone in the field | Questions, delays, photos, "can't make it," scope chat |
| **Scheduling** | Web app (+ confirm links in MMS) | **Ryan (contractor)** | Propose, reschedule, cascade, reassign, portfolio view |

**Why scheduling stays in the app:** Ryan runs **multiple jobs and teams** at once. Dates, dependencies, who's confirmed, cascade impact — that's too much to manage in text threads. MMS signals problems; the app is where the schedule lives.

**Subs don't schedule in the app in v0.1** — they **confirm** dates via magic link when Ryan acts. Sub-initiated change requests (UJ-2c) are optional/secondary; default pattern is sub texts in MMS → Ryan reschedules in app.

```text
MMS:  "Can't start Thursday — supplier delay"
App:  Ryan moves Flooring → Tuesday, previews cascade, saves
MMS:  [Maple St] Confirm Tuesday: [link]   ← system message after Ryan commits
```

### Design principle: **Accept/Reject + calendar** (subs and customers)

Applies to **subcontractors** and **customers** whenever they Accept or Reject a schedule item (propose, reschedule, cascade, customer-visible change).

| Calendar linked? | Accept | Reject |
|------------------|--------|--------|
| **No** | Recorded in ContractorPro only | Recorded in ContractorPro only |
| **Yes** | Recorded in ContractorPro **+** linked calendar updated | Recorded in ContractorPro **+** linked calendar updated |

- **Always in our system** — Ryan sees ✅ accepted, ❌ declined, or ⏳ pending regardless of calendar link.
- **Calendar is optional** for invitees — link during join or later from portal; view schedule in app either way.
- **Calendar providers (v0.1):** **Google Calendar only** — subs/customers receive **Google event attendee invites** on accept when email is on file. **Apple Calendar connect → v0.1.1.** Portal schedule view works without calendar link.
- **Contractor calendar** (Ryan's company Google) is separate — syncs on sub/customer accept per existing workflow.

Subs may also **counter-propose** or **request a different date** — those are negotiation states, not simple accept/reject. Customers do **not** negotiate; accept or reject only.

### Product decision: **bundle join + first date (default)**

| Approach | When | Jesse's SMS count |
|----------|------|------------------|
| **Bundled (default)** — UJ-3+ | Ryan assigns first task + date at invite | **1** — join + Accept/Decline on same link |
| **Separate** — UJ-3 then UJ-1 | Ryan adds sub to roster before dates are known | **2** — "join project" then later "confirm Sept 10" |

**Recommendation:** First scheduled task for a sub **is** their invite. Magic link join and UJ-1 confirm happen in **one visit** when Jesse is new; returning subs skip join and go straight to Accept/Decline.

Separate invite-only (UJ-3) remains available when Ryan is building the roster early — not the happy path.

```text
NEW SUB + FIRST TASK DATE     →  one SMS  →  [Join if needed] → Accept/Decline  (UJ-3+)
SUB ALREADY ON THIS PROJECT   →  one SMS  →  Accept/Decline only             (UJ-1)
SUB ON ROSTER, NO DATE YET    →  invite SMS only, date later                   (UJ-3 → UJ-1)
```

---

## UJ-0: Contractor gets started

**Cast:** Ryan — team member, owner of **Riverside Remodeling** (Contractor subscription)  
**Trigger:** Ryan signed up after churning from Buildertrend; has one new kitchen job starting.

### Steps

| # | Who | Does what | Sees / system |
|---|-----|-----------|---------------|
| 1 | Ryan | Opens ContractorPro on laptop, signs in with Google | Contractor workspace (empty) |
| 2 | Ryan | Connects Google Calendar for Riverside Remodeling | "Connected" + short explanation: dates sync after subs confirm |
| 3 | Ryan | Creates project **Maple St Kitchen** + enters primary customer Lauren (name, email, phone) | Project dashboard; Lauren gets **email + MMS** confirm links |
| 4 | Ryan | Adds tasks: Demo, Rough electric, Drywall, Paint (dates optional for now) | Task list |
| 5 | Ryan | Invites Jesse (Sub, paint) — see UJ-3 / UJ-3+ | Invite sent |
| 6 | Ryan | Assigns Paint to Jesse, proposes **Sept 10** | Jesse → pending; Ryan dashboard shows ⏳ |

**Success:** Ryan has a live project, calendar connected, primary customer on project, one sub invited, one date proposed — in one sitting (~20–30 min).

**Branches:**
- Ryan skips calendar connect → warned: "Subs won't see dates in Google until you connect"
- Ryan proposes before Jesse joins → Jesse gets invite + proposal in one SMS after join

---

## UJ-1: Propose a date → Sub confirms (happy path)

**Prerequisite:** Jesse has **project membership** on Maple St (UJ-3 or UJ-3+).  
**Cast:** Ryan (team member), Jesse (Subcontractor on Maple St only)  
**Realizes:** Core wedge — propose → accept → calendar.

**Trigger:** Ryan sets Jesse's paint date (Jesse already joined via UJ-3, or joined inline via UJ-3+).

### Steps

| # | Who | Does what | Sees / system |
|---|-----|-----------|---------------|
| 1 | Ryan | Opens Maple St → Paint task → assigns Jesse → date **Sept 10** → Save | Preview: "Jesse will be notified" |
| 2 | System | Sends SMS to Jesse | `[Riverside Remodeling] Painting — Maple St, Sept 10. Accept or decline: [link]` |
| 3 | Jesse | Taps link on phone (between jobs) | One screen: task, date, **Accept** / **Decline** |
| 4 | Jesse | Taps **Accept** | Recorded in system; linked calendar updated **if** Jesse has calendar linked |
| 5 | System | Writes event to contractor + project Google Calendar | Jesse's personal calendar updated only when linked (see accept/reject rule above) |
| 6 | Ryan | (moments later) | In-app: "Jesse confirmed Paint — Sept 10" · dashboard ✅ |

**Success:** Ryan sees confirmed status in app. Jesse's personal calendar updates when linked; always viewable in portal.

**Branches:**
- Jesse already joined vs first-time join on same link
- Jesse uses email notify → same page, email buttons
- Calendar not linked → accept recorded in system only; Jesse views schedule in portal

---

## UJ-1b: Sub doesn't respond (poke)

**Cast:** Ryan, Jesse  
**Trigger:** Jesse got UJ-1 SMS but didn't open it.

### Steps

| # | Who | Does what | Sees / system |
|---|-----|-----------|---------------|
| 1 | Jesse | Ignores day 1 | Ryan dashboard: ⏳ Pending |
| 2 | System | +24h reminder SMS | "Still need your confirmation…" |
| 3 | Jesse | Still ignores | Ryan: ⏳ Pending 2 days |
| 4 | System | +48h stronger reminder; optional Ryan alert | Dashboard escalation badge |
| 5 | System | Daily poke (batched if multiple tasks) | Jesse gets one SMS/day max |
| 6a | Jesse | Eventually taps Accept | → UJ-1 steps 4–6 |
| 6b | Ryan | Taps **Send reminder now** or calls Jesse, then **Snooze 2 days** | Pokes paused; logged |

**Success:** Either Jesse confirms without Ryan chasing, or Ryan has visibility to chase intentionally.

**Design note:** Google Calendar invite alone would **not** do steps 2–5 — this is ContractorPro's job.

---

## UJ-2: Reschedule → other party re-confirms (either direction)

**Core rule:** Nobody moves a **confirmed** date unilaterally. Either party can **propose** a change; the **other** must Accept, **Counter-propose**, or Decline. Until agreement, calendars keep the **last confirmed** date.

| Who proposes | Who must respond | Journey |
|--------------|------------------|---------|
| Ryan (team member) | Jesse (sub) | **UJ-2a** |
| Jesse (sub) | Ryan (team member) | **UJ-2c** |
| Either (back-and-forth) | The other party | **UJ-2d** |

**Response options** (when you are the pending party):

| Action | Effect |
|--------|--------|
| **Accept** | Dates confirmed; calendars sync |
| **Counter-propose** | New date offered; pending party **flips**; negotiation continues |
| **Decline** | Hard no on current proposal → **UJ-2b** (Ryan may reassign → **UJ-2e**) |

---

## UJ-2a: Contractor reschedules → Sub re-confirms

**Cast:** Ryan, Jesse (previously confirmed Sept 10)  
**Trigger:** Cabinet delay pushes paint to Sept 11.

### Steps

| # | Who | Does what | Sees / system |
|---|-----|-----------|---------------|
| 1 | Ryan | Drags Paint to **Sept 11** (or edits date) | Preview: "Jesse must re-confirm" |
| 2 | Ryan | Confirms save | Status → `proposed_change`; initiator = team member |
| 3 | System | SMS to Jesse | `Sept 10 → Sept 11. Please confirm: [link]` |
| 4 | Jesse | Calendar still shows **Sept 10** until he acts | Old agreed date preserved |
| 5 | Jesse | Taps **Accept**, **Counter-propose**, or **Decline** | Accept → Sept 11; counter → **UJ-2d**; decline → **UJ-2b** |

**Success:** No silent calendar move — Jesse explicitly agreed to the new date.

---

## UJ-2c: Sub requests reschedule → Contractor re-confirms

**Cast:** Ryan, Jesse (previously confirmed Sept 10)  
**Trigger:** Jesse gets booked on another job that week; he can't make Sept 10.

### Steps

| # | Who | Does what | Sees / system |
|---|-----|-----------|---------------|
| 1 | Jesse | Opens assignment from portal or SMS link | Confirmed: Paint — Sept 10 |
| 2 | Jesse | Taps **Request different date** → picks **Sept 12** (or range) | Optional note: "Conflict on another job" |
| 3 | System | Status → `proposed_change`; initiator = subcontractor | Proposed: Sept 12; confirmed still Sept 10 |
| 4 | System | Notifies Ryan (in-app + optional SMS) | "Jesse requested Paint move: Sept 10 → Sept 12" |
| 5 | Ryan | Opens dashboard or link | **Accept** / **Counter-propose** / **Decline** / Message Jesse |
| 6a | Ryan | **Accept** | Both calendars → Sept 12; Jesse notified |
| 6b | Ryan | **Counter-propose Sept 11** | Pending flips to Jesse; see **UJ-2d** |
| 6c | Ryan | **Decline** | Stays Sept 10 confirmed; Jesse notified to call or propose again |

**Success:** Jesse surfaced the conflict in-app instead of no-showing; Ryan chose whether Sept 12 works.

**v0.1 lean:** Poke reminders target **subs** (FR-11). Ryan gets immediate notification; no automated chase if he doesn't respond — he's the paying user and usually at a desk. `[OPEN: poke Ryan if sub-request pending 48h?]`

**Branches:**
- Jesse taps **Decline** instead of requesting a date → **UJ-2b**
- Ryan **counter-proposes** instead of accept/decline → **UJ-2d**
- Ryan accepts but cascade would affect other tasks → preview before confirm (UJ-5 overlap)

---

## UJ-2d: Counter-propose (date negotiation)

**Cast:** Ryan, Jesse  
**Trigger:** Jesse requested Sept 12; Ryan can't do Sept 12 but Sept 11 works.

### Steps

| # | Who | Does what | Sees / system |
|---|-----|-----------|---------------|
| 1 | Jesse | Requested Sept 12 (from UJ-2c) | Pending Ryan |
| 2 | Ryan | Opens request, taps **Suggest different date** → **Sept 11** | Optional note: "Cabinets land Sept 10" |
| 3 | System | Updates proposed dates; `pending_party` → Jesse | Thread: "Jesse: Sept 12 → Ryan: Sept 11" |
| 4 | System | SMS to Jesse | `Ryan suggested Sept 11 instead of Sept 12. Confirm: [link]` |
| 5 | Jesse | **Accept** Sept 11 | Confirmed; both calendars → Sept 11 |
| — | Jesse | **Counter-propose Sept 13** | Pending flips back to Ryan; loop continues |
| — | Jesse | **Decline** | **UJ-2b** |

**Success:** Real-world scheduling happens in 2–3 rounds without phone tag.

**Rules:**
- Calendars stay on **last confirmed** date through the whole thread (Sept 10 in this example).
- Each counter resets poke timer for whoever is now pending.
- Negotiation history visible on Ryan's dashboard (audit trail, not a chat).
- No arbitrary round cap in v0.1 — ends on Accept or Decline.

**Symmetric:** Jesse can counter when Ryan reschedules (UJ-2a) the same way — "Can't do Sept 11, how about Sept 13?"

---

## UJ-2b: Sub declines a date

**Cast:** Ryan, Jesse  
**Trigger:** Jesse can't do Sept 11 — booked on another Contractor's job (or he won't counter-propose).

### Steps

| # | Who | Does what | Sees / system |
|---|-----|-----------|---------------|
| 1 | Jesse | Opens link, taps **Decline** | "Decline recorded. [Contractor] will be in touch." |
| 2 | System | Alerts Ryan immediately (in-app + optional SMS) | "Jesse DECLINED Paint — Sept 11" |
| 3 | Ryan | Opens dashboard | ❌ Declined — **Reassign** / propose new date to Jesse / message |
| 4 | Ryan | (optional) Messages Jesse | Offline resolution |
| 5a | Ryan | Proposes new date to Jesse | New propose cycle (UJ-1) |
| 5b | Ryan | **Assigns Nate instead** | → **UJ-2e** |

**Success:** Ryan knew within minutes, not on Sept 11 when Jesse no-shows.

**Calendar on decline:**
| Situation | Jesse's calendar |
|-----------|-----------------|
| Declined a **proposed** date (never confirmed) | No event (nothing was ever agreed) |
| Declined a **reschedule** (was confirmed Sept 10) | Stays **Sept 10** until Ryan resolves; removed on reassignment (UJ-2e) |

---

## UJ-2e: Sub declines → Contractor assigns different sub

**Cast:** Ryan, Jesse (declined), Nate (replacement painter)  
**Trigger:** Jesse can't do the job; Ryan needs paint covered.

### Steps

| # | Who | Does what | Sees / system |
|---|-----|-----------|---------------|
| 1 | — | (from UJ-2b) Jesse's assignment → `declined` | Paint task shows ❌ Jesse declined |
| 2 | Ryan | Paint task → **Assign to Nate** → date **Sept 10** (same or new) | Preview: "Jesse removed · Nate will be notified" |
| 3 | System | Closes Jesse's assignment; stops poke | Jesse's calendar event **removed** (if any confirmed) |
| 4 | System | Creates assignment for Nate → `proposed` | If Nate new to project → **UJ-3+** bundled invite |
| 5 | Nate | Gets SMS, Accept/Decline | Standard propose flow (UJ-1) |
| 6 | Ryan | Dashboard | Paint: ⏳ pending Nate (Jesse shown as declined / past assignee) |

**Success:** Ryan moved on in one action; schedule didn't stall on Jesse.

**Design notes:**
- Jesse is **not** deleted from the project — he may still be on other tasks. Only this assignment closes.
- Assignment history preserved: "Jesse declined Sept 11 → reassigned to Nate."
- Optional courtesy SMS to Jesse: "You're off Paint on Maple St. Other assignments unchanged." `[OPEN]`
- Ryan can pick someone **not yet on project** → invite + propose in one step (UJ-3+).

---

## UJ-3+: Invite + first scheduled task — **default path**

**This is the primary sub onboarding journey.** It bundles magic-link join (if needed) with the first propose/confirm (UJ-1) in **one SMS**.

**Cast:** Ryan, Jesse (new or returning to *this* project)  
**Trigger:** Ryan assigns Jesse his **first task** on Maple St with a date — e.g. Rough electric, Sept 10.

### Ryan's side

| # | Ryan does |
|---|-----------|
| 1 | Maple St → assign **Rough electric** to Jesse (pick phone from roster or enter new) |
| 2 | Sets date **Sept 10** |
| 3 | **Save** — system treats this as invite + propose when Jesse is not yet on project |

If Jesse's phone is new to this project → create membership + send bundled SMS.  
If Jesse already joined this project → UJ-1 only (no join step).

### Jesse's side — one link, ordered steps

**SMS (single):**
```
[Riverside Remodeling] Maple St Kitchen — Rough electric, Sept 10.
Tap to join and confirm: [link]
```

| Step | Jesse (new to this project) | Jesse (already on project) |
|------|---------------------------|---------------------------|
| 1 | Opens link | Opens link |
| 2 | **Join** screen — confirm name + phone (one screen) | *(skip)* |
| 3 | **Accept** or **Decline** Sept 10 | **Accept** or **Decline** |
| 4 | Confirmed → calendar sync | Same |

**Success:** One text, one visit, sub is on the job and date is agreed (or declined).

**Why bundle:** Matches how Ryan already works — he doesn't text "download this" and then "be there Tuesday." He texts "you're on Maple St, electric Sept 10, confirm."

---

## UJ-3: Invite only (no date yet) — secondary path

**Cast:** Ryan, Jesse  
**Trigger:** Ryan adds Jesse to the project roster before any dates are set (e.g. planning phase).

Jesse gets invite-only SMS → join portal → waits. When Ryan later assigns a dated task → **UJ-1** (second SMS).

Use when dates aren't ready yet — not the default.

---

## UJ-3r: Sub invited — phone already known (returning Jesse)

**Cast:** Ryan (Riverside), Jesse (was Sub on **Main St** last year — same Contractor)  
**Trigger:** Ryan adds Jesse to **Oak Ave** — new project.

### What Ryan sees

| # | Ryan does | System helps |
|---|-----------|--------------|
| 1 | Starts invite, types Jesse's phone | **"Jesse Torres — was on Main St"** (company rolodex suggestion) |
| 2 | Confirms name, assigns task + date (UJ-3+) or invite only (UJ-3) | New **project membership** for Oak Ave |

### What Jesse experiences

**Path A — Trusted device:** link may skip join → straight to Accept/Decline or portal.  
**Path B — New device:** join with name pre-filled — one tap.  
**Path C — Different Contractor:** same bundled flow; SMS clearly from Riverside.

**Success:** Jesse thinks "new job text," not "log into my account."

---

## UJ-3 edge cases

| Scenario | Behavior |
|----------|----------|
| Ryan typo'd phone | Wrong person opens link → phone verify fails → cannot join |
| Jesse forwards invite | Token may require OTP match to invited phone |
| Ryan re-invites same phone to same project | "Already joined" or resend link |
| Jesse is **Customer** on another project (same phone) | New membership on Maple St as **Sub** — roles don't collide |
| Jesse declines to join | Rare — no membership; Ryan sees invite not accepted `[OPEN]` |
| Ryan invites before Jesse has smartphone | Email path if email provided |

---

## UJ-3 vs UJ-1 vs UJ-3+ — when to use which

```text
First task + date for Jesse on THIS project   →  UJ-3+  (default) — join + confirm, one SMS
Jesse already on project, new date             →  UJ-1 only
Roster Jesse early, no dates yet               →  UJ-3, then UJ-1 later (two SMS)
```

---

## UJ-3b: Primary customer — set up at project creation

**Cast:** Ryan, Lauren  
**Trigger:** Ryan creates Maple St Kitchen; Lauren is the homeowner from contract intake.

**Dual-channel confirm:** System sends **email and MMS** on project create. Each channel must be **confirmed in our system**. **Poke** unconfirmed channel(s) until **both** are ✅.

### Steps

| # | Who | Does what | Sees / system |
|---|-----|-----------|---------------|
| 1 | Ryan | Creates project; enters Lauren as **primary customer** — name, email, phone | Customer on project record; both channels pending |
| 2 | System | Sends **email** + **MMS** — each with confirm link | `email_confirmed` ⏳ · `phone_confirmed` ⏳ |
| 3 | Lauren | Taps **email** link → Confirm | `email_confirmed` ✅ |
| 4 | Lauren | Taps **MMS** link → Confirm | `phone_confirmed` ✅ |
| 5 | System | If either still ⏳ → poke on unconfirmed channel(s) only | +24h, +48h, daily (same engine as sub poke) |
| 6a | Lauren (already in system) | After both confirms → **Accept** connection | Pre-filled; portal access |
| 6b | Lauren (new) | After both confirms → confirm profile, optional **calendar link** | Customer portal |
| 7 | Ryan | Dashboard | **Email** ✅/⏳ · **MMS** ✅/⏳ until both done |
| 8 | Lauren | — | Does **not** see sub list, sub pricing, or sub threads |

**Success:** Primary customer on project from creation. **Both** email and phone verified in system before fully connected.

**Comms:** Lauren can message Ryan via **MMS** or **app** after connect. Lauren **cannot** change or request schedule changes.

`[OPEN: Block schedule MMS to customer until phone_confirmed? Block email schedule digests until email_confirmed?]`

---

## UJ-3c: Additional customer member invite (family)

**Cast:** Ryan, Lauren (primary), Erin (spouse — additional customer)  
**Trigger:** Ryan wants a second customer-side contact on the project (spouse, family, co-owner).

### Steps

| # | Who | Does what | Sees / system |
|---|-----|-----------|---------------|
| 1 | Ryan | Invites Erin as **Customer** via **MMS** | Additional membership; Lauren unchanged as primary |
| 2a | Erin (already in system) | Taps link → **Accept** or **Reject** | Same visibility as Lauren |
| 2b | Erin (new) | Taps link → register on link | Customer portal |
| 3 | Erin | — | Same customer visibility rules; no sub threads |

**Success:** Family member has portal + MMS access. **Secondary path** — primary customer is always set at project creation (UJ-3b).

---

## UJ-4: Customer schedule change → accept

**Cast:** Ryan, Lauren (Customer on Maple St)  
**Trigger:** Ryan moves cabinet install from Oct 1 → Oct 5.

### Steps

| # | Who | Does what | Sees / system |
|---|-----|-----------|---------------|
| 1 | Ryan | Updates cabinet task date (customer-visible) | — |
| 2 | System | MMS to Lauren | `Schedule update on Maple St: Cabinets moved to Oct 5. Accept: [link]` |
| 3 | Lauren | Taps link | **What changed** — plain language, no Gantt |
| 4 | Lauren | Taps **Accept** or **Reject** | Always recorded in system |
| 5a | Lauren (calendar **linked**) | Accept/Reject | Linked calendar updated |
| 5b | Lauren (calendar **not** linked) | Accept/Reject | In-system only; view schedule in app |
| 6 | Ryan | Dashboard | ✅ accepted, ❌ rejected, or ⏳ pending |
| 6 | Lauren | Optional: replies in Contractor↔customer thread (app or MMS) | "Thanks, will the 5th work for the inspection?" |

**Success:** Lauren acknowledged the change; Ryan has accept tracking. Lauren never sees that Jesse's paint slipped.

**Rules:**
- Customer **accepts or rejects** schedule updates — does **not** negotiate, counter-propose, or request changes (unlike subs).
- Follows **accept/reject + calendar** rule (subs and customers): always recorded in system; linked calendar updated when calendar linked.

---

## UJ-5: Cascade after a slip

**Cast:** Ryan, Jesse (electric), Nate (drywall) — multiple subs  
**Trigger:** Framing finishes 3 days late; everything after must move.

### Steps

| # | Who | Does what | Sees / system |
|---|-----|-----------|---------------|
| 1 | Ryan | Moves **Framing** +3 days on Maple St (cascade on) | — |
| 2 | System | Shows **preview** | "4 tasks move · Jesse + Nate must re-confirm" |
| 3 | Ryan | Reviews list, taps **Confirm** | Assignments → proposed_change |
| 4 | System | Notifies Jesse, Nate (and poke cycle starts) | Each gets old → new + link |
| 5 | Jesse, Nate | Accept individually | Calendars update per sub as each accepts |
| 6 | Ryan | Dashboard | Mixed ✅ / ⏳ until all confirm |

**Success:** One edit by Ryan, controlled ripple, no silent calendar chaos.

**Branch:** Cascade off → only framing moves; Ryan manually moves downstream tasks.

---

---

## UJ-8: MMS group thread — how field comms actually work (v0.1)

**Cast:** Ryan, Marcus (flooring sub)  
**Trigger:** Ryan needs to coordinate Maple St with Marcus; comms stay in text like today.

### Setup (when Ryan invites / assigns Marcus)

| # | Who | Does what | Sees / system |
|---|-----|-----------|---------------|
| 1 | Ryan | Creates Maple St project (UJ-0) | System provisions **Maple handle #** `(555) 100-0001` |
| 2 | Ryan | Invites Marcus (UJ-3+) | UI shows: add Marcus + **Maple handle #** to group text |
| 3 | Ryan | Creates group MMS: **Ryan + Marcus + Maple handle #** | Native Messages; same handle # used for every sub on this job |
| 4 | System | Records `mms_thread` (Maple + Marcus); ingests MMS | Web app: Maple St → Marcus thread |

**Routing:** inbound `To` = Maple # → project; `From` = Marcus → his membership. Jesse on Maple uses the **same** Maple # in a **different** group.

**Note:** Ryan creates the group. ContractorPro cannot join his old iMessage thread — this is a **new** group with the handle.

### Day-to-day conversation (MMS-primary)

| # | Who | Does what | Sees / system |
|---|-----|-----------|---------------|
| 1 | Marcus | Texts group: *"Supplier slipped — can't start flooring Thursday"* | Logged in app; Ryan sees in dashboard |
| 2 | Ryan | Reads on phone **or** web thread mirror | Same content |
| 3 | Ryan | (optional) Replies in group MMS | Logged |

**Success:** Conversation captured on project without Marcus opening a portal.

### Schedule commit (web app + confirmation MMS)

| # | Who | Does what | Sees / system |
|---|-----|-----------|---------------|
| 4 | Ryan | Opens **web app**, moves Flooring to next Tuesday (UJ-2a) | Reschedule preview |
| 5 | System | Sends MMS to group (or from handle): `[Maple St] Flooring → Tuesday. Confirm: [link]` | Marcus gets text in same thread |
| 6 | Marcus | Taps link, Accept | Calendars sync; Ryan ✅ |

**Key split:**
- **Talk** in MMS (can't start, why, photos)
- **Commit** in app (Ryan reschedules) + magic link (Marcus confirms)

Jesse (painter) has a **different** group: Ryan + Jesse + **same Maple handle #**. Marcus never sees Jesse's thread.

### Customer variant

Ryan + Lauren + handle # for schedule questions. Lauren does not see sub threads.

---

## UJ-6: Photo in thread (MMS or web)

**Cast:** Jesse (Sub), Ryan  
**Trigger:** Jesse finds an issue behind the wall; needs to show Ryan.

### Path A — MMS (default)

| # | Who | Does what | Sees / system |
|---|-----|-----------|---------------|
| 1 | Jesse | Sends photo in **group MMS** (Ryan + Jesse + handle) | MMS ingested → blob storage |
| 2 | Ryan | Sees photo in phone thread + web mirror | Project record |

### Path B — Web (optional)

| # | Who | Does what | Sees / system |
|---|-----|-----------|---------------|
| 1 | Jesse | Opens magic link → Messages → camera upload | Same thread in app |

**Success:** Field photo on project record — not lost in a personal Ryan↔Jesse thread without the handle.

**Branch:** Lauren cannot see this thread (sub-only).

---

## UJ-7: Same person, different roles (identity)

**Cast:** Jesse  
**Trigger:** Jesse does tile for Riverside (Sub) and is also remodeling his own bathroom hired **Oak Lane Builders** (Customer).

### Steps

| # | Who | Does what | Sees / system |
|---|-----|-----------|---------------|
| 1 | Jesse | Sub on Maple St (Riverside) | Sub portal: tasks, confirm dates |
| 2 | Jesse | Customer on Oak Lane job (different Contractor) | Customer portal: what-changed, message Oak Lane |
| 3 | Jesse | Gets SMS from both Contractors | Different links, different roles |
| 4 | Jesse | — | No single "account type"; two memberships |

**v0.1:** Two separate magic-link contexts.  
**v0.2 (optional):** One phone login listing "Your projects."

**Success:** System never assumes Jesse is "always a sub."

---

## UJ-9: End-to-end schedule slip (cross-persona)

**Cast:** Ryan, Maci (optional), Jesse, Marcus, Lauren  
**Trigger:** Framing on Maple St finishes **3 days late** — cascade affects paint, flooring, and customer-visible cabinet milestone.

**Purpose:** Single SME walkthrough tying confirm, cascade, MMS, customer accept, and poke. Bullets: [contractor C-19–C-20](./user-journeys/contractor-journeys.md), [sub S-18](./user-journeys/subcontractor-journeys.md), [customer H-21/H-14](./user-journeys/customer-journeys.md).

### Timeline

| # | Who | Does what | System |
|---|-----|-----------|--------|
| 1 | Marcus | MMS group: "Supplier slipped — can't start flooring Thursday" | Ingested → Ryan/Maci see in app (UJ-8) |
| 2 | Ryan | Calls Maci: push everything from framing +3 days | — |
| 3 | Maci | Opens Maple St; **draft** or direct edit with cascade on (C-20/C-21) | Preview: 4 tasks, Jesse + Marcus re-confirm, Lauren milestone |
| 4 | Maci | Ryan approves on phone; Maci **confirms** cascade publish | Assignments → `proposed_change` |
| 5 | Jesse | SMS: 3 tasks old→new — batch or separate `[BL-5]` | Poke if no response (UJ-1b) |
| 6 | Marcus | SMS: flooring old→new | Poke if no response |
| 7 | Lauren | MMS: "Cabinets moved Oct 1 → Oct 5" + accept link (UJ-4) | Only if milestone-visible `[BL-2]`; channel gating `[BL-1]` |
| 8 | Jesse | Accepts all / each; calendars update when linked | Ryan dashboard → ✅ |
| 9 | Marcus | Accepts | ✅ |
| 10 | Lauren | Accepts milestone | ✅; timeline updated (H-21) |
| 11 | Ryan | **C-19** queue clears for Maple St slip | Portfolio triage green |

**Success:** One slip handled without phone tag — talk in MMS, commit in app, confirm via links, customer sees milestones only.

**Branches:**
- Jesse requests counter-propose on one task → UJ-2d thread; rest can still confirm
- Lauren doesn't accept milestone → poke on MMS channel; Ryan sees ⏳
- Partial cascade → only selected tasks move `[BL-11]`

---

## Cross-journey timeline (example week)

```text
Mon    Ryan: UJ-0 onboarding, invites Jesse + Lauren
Tue    Nate: UJ-3 join · Ryan proposes paint UJ-1
Wed    Jesse: accepts UJ-1 · calendars sync
Thu    Ryan: messages Jesse photo UJ-6
Fri    Ryan/Maci: slip framing UJ-9 (cascade) · Jesse + Marcus re-confirm
       Lauren: UJ-4 / H-21 cabinet milestone accept
```

---

## Journeys vs dev stories

| User journey | Rough story coverage |
|--------------|---------------------|
| UJ-0 | E1, E2, E3, E4 |
| UJ-1, UJ-1b, UJ-2, UJ-2a, UJ-2b, UJ-2c, UJ-2d, UJ-2e | E5, E6, E3 |
| UJ-3, UJ-3r, UJ-3+ | E4 |
| UJ-3b, UJ-3c | E4, E9 |
| UJ-4 | E9 |
| UJ-5, UJ-9 | E7, E5, E6, E9 |
| UJ-8, UJ-6 | E8 |
| UJ-7 | E10 |

**Backlog / v0.2 journeys:** [user-journeys/backlog.md](./user-journeys/backlog.md) · [future-journeys-v02.md](./user-journeys/future-journeys-v02.md)

---

## Workshop — refine together

Walk each journey and ask:

1. **Missing step?** (e.g., Ryan adds subs before tasks exist?)
2. **Wrong actor?** (owner vs office manager?)
3. **Too much for v0.1?** (cut cascade to v0.1.1?)
4. **Emotional beat** — where does Ryan lose patience today?

**Suggested order to review live:** **UJ-3+ → UJ-3r → UJ-1 → UJ-1b → UJ-2a / UJ-2c / UJ-2d → UJ-2b / UJ-2e → UJ-3b → UJ-4 → UJ-9** (full slip)

Log decisions in [discovery-log.md](../../discovery-log.md).
