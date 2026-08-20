# Customer Journeys — SME Review

**Status:** Draft — validate with homeowner / customer SMEs  
**Persona:** Customer / homeowner (e.g. **Lauren** — hired Riverside for a kitchen remodel)  
**Related:** [Contractor journeys](./contractor-journeys.md) · [Subcontractor journeys](./subcontractor-journeys.md) · [Full detail](../user-journeys.md)

**Design principles:**
- Lightweight portal — schedule visibility, calendar sync (optional), and private GC↔customer comms
- Customers do **not** see subs, sub pricing, or sub-only conversations
- Customers **cannot change or request changes** to the schedule — they **accept or reject** updates (same calendar rule as subs)
- **Primary customer** is entered by the **contractor during project creation** (name, email, phone) — not a separate invite step
- **Dual-channel confirm at connect:** system sends **email and MMS**; each channel must be **confirmed in our system**; **poke** until **both** are confirmed
- **Customer invite (MMS)** is **secondary** — for additional customer-side members (spouse, family, second contact on the job)

**Accept/Reject + calendar** (same rule as subcontractors): always recorded in ContractorPro; linked personal calendar updated when calendar is linked. See [full detail](../user-journeys.md#design-principle-acceptreject--calendar-subs-and-customers).

**Calendar providers:** **Google only in MVP** — attendee invite on accept when email on file. Apple connect → v0.1.1.

Use this list in SME workshops. Ask: *"What do you actually want to know during a remodel? What's TMI?"*

---

## Joining a project

### Primary customer (default — contractor sets up at project creation)

### H-1: Primary customer added — email + MMS sent
- **Trigger:** Ryan creates Maple St Kitchen; enters Lauren's **name, email, phone** as primary customer on the project form
- Lauren is on the project record immediately — Ryan does not run a separate invite step
- System sends **both** at once:
  - **Email** — confirm link to verify email reachability
  - **MMS** — confirm link to verify phone / text reachability
- System tracks **two confirmations** independently: `email_confirmed`, `phone_confirmed` (or `mms_confirmed`)
- **Fully connected** only when **both** channels are confirmed in our system
- **Success:** Ryan knows Lauren can be reached on email and text before relying on either channel
- **SME check:** Is dual confirm too much friction, or worth it for deliverability?

### H-2: Confirm email channel
- **Trigger:** Lauren receives connect email from Riverside Remodeling
- Taps **Confirm** in email → `email_confirmed` recorded in ContractorPro
- May land on same connect/portal flow as MMS link (one visit can satisfy both if she uses both links)
- **Success:** Email channel verified; Ryan dashboard shows ✉️ ✅ (or ⏳ if MMS still pending)
- **SME check:** Email confirm enough, or also need "open and read" tracking?

### H-3: Confirm MMS / phone channel
- **Trigger:** Lauren receives connect MMS/text
- Taps link on phone → `phone_confirmed` recorded in ContractorPro
- **Success:** Text channel verified; Ryan dashboard shows 📱 ✅ (or ⏳ if email still pending)
- **SME check:** Same link content as email, or shorter SMS copy?

### H-4: Poke until both channels confirmed
- **Trigger:** Lauren confirmed email but not MMS (or vice versa), or neither yet
- System sends automated reminders on **unconfirmed channel(s) only** — same poke cadence as subs (+24h, +48h, daily default)
- Ryan dashboard: **Email** ⏳/✅ · **MMS** ⏳/✅ — clear which channel still needs action
- Ryan can **Send reminder now** or **Snooze** per channel or both
- Poke stops when **both** confirmed (or Ryan intervenes)
- **Success:** Both channels verified without Ryan manually chasing "did you get my email?"
- **SME check:** Daily poke on both channels — too aggressive for homeowners?

### H-5: Primary customer — already in the system
- **Trigger:** Lauren's phone/email known from a prior project; both confirms may complete in one visit
- Taps email and/or MMS link → confirms channels + **Accept** connection to **this** project
- Pre-filled info; no full re-registration
- **Success:** Connected to Maple St when **both** channels confirmed
- **SME check:** Repeat customers — skip dual confirm? `[OPEN: trust prior confirms?]`

### H-6: Primary customer — new to the system
- **Trigger:** Lauren has never used ContractorPro; completes channel confirms (H-2/H-3)
- Confirms or completes profile: **name**, **email**, **phone**, optional **calendar link**
- Lands in customer portal after **both** channel confirms
- **Success:** Onboarded, both channels verified, connected in one or two visits
- **SME check:** Calendar link at first connect — too early?

### Additional customer members (secondary — MMS invite)

### H-7: Invite family member or second contact
- **Trigger:** Ryan wants Lauren's spouse, partner, or family member on the project too
- Ryan invites additional person as **Customer** via **MMS** (group or direct with project handle #)
- Primary customer (Lauren) may already be connected — this is an **extra** customer-side account
- **Success:** Second person gets portal + MMS access without replacing Lauren as primary
- **SME check:** How common? Spouse on every job, or only when both work from home?

### H-8: Additional member — accept or reject connection
- **Trigger:** Family member received H-7 invite
- Already in system → **Accept** or **Reject** project connection
- New to system → register on link (same dual-channel pattern as H-1–H-4 — `[OPEN: required for family too?]`)
- **Success:** Additional member on project with same customer visibility rules as Lauren
- **SME check:** Should family see the same schedule, or a reduced view? `[OPEN]`

### H-9: Reject connection
- **Trigger:** Wrong invite, wrong project, or person declines
- Taps **Reject** on link
- No membership created; Ryan notified `[OPEN: Ryan alert on reject?]`
- **Success:** Person not tied to a project they didn't want
- **SME check:** Wrong-number invites — how often?

### H-10: What customers see (and don't)
- **Sees:** Project schedule (customer-appropriate view), messages with Ryan, calendar-linked events (if linked)
- **Does NOT see:** Sub list, sub contact info, sub pricing, sub↔contractor threads, internal GC notes
- **Success:** Informed homeowner without construction admin overload
- **SME check:** What do they wish they could see? What would freak them out?

---

## Calendar

### H-11: Receive calendar events via Google invite
- **Trigger:** Lauren accepts a schedule update; email confirmed (H-2)
- System adds Lauren as **attendee** on the shared project Google Calendar event
- Lauren accepts invite in Google Calendar — no separate OAuth connect in MVP
- **Success:** Project dates on personal calendar when she accepts updates
- **SME check:** iPhone users — Google invite vs Apple-only → v0.1.1

### H-12: View schedule in the app
- **Trigger:** Any time after joining
- Opens customer portal → sees project schedule (plain language, no Gantt)
- Shows current dates and pending items awaiting accept
- **Success:** Single place to see what's planned without calling Ryan
- **SME check:** Mobile browser enough, or do they want a bookmark/PWA?

### H-13: Calendar linked — events sync to personal calendar
- **Trigger:** Calendar linked (H-11); Ryan publishes or updates customer-visible schedule items
- All customer-visible calendar items appear on linked calendar
- New items and updates flow after **accept** (H-14)
- **Success:** Remodel milestones on the calendar they already check daily
- **SME check:** Every task or only milestones (cabinets, inspection, move-in)?

### H-14: Schedule change — accept or reject
- **Trigger:** Ryan moves cabinet install Oct 1 → Oct 5
- Gets MMS: schedule change notification + link
- Taps link → sees what changed → **Accept** or **Reject**
- **Always** recorded in ContractorPro (Ryan sees ✅ or ❌)
- **Calendar linked:** accept/reject also updates linked calendar
- **Calendar not linked:** in-system only; view current schedule in app (H-12)
- **Success:** Ryan has accept/reject tracking; calendar current when linked
- **SME check:** Is "Reject" right for customers, or only "Accept" / "Got it"?

### H-15: Cascade / multi-task slip (customer view)
- **Trigger:** Framing delay pushes several customer-visible milestones
- Gets MMS notification(s) summarizing what changed (not every sub task)
- Accepts or rejects each change (or batched — `[OPEN]`) per H-14
- **Success:** Understands impact on inspection or move-in dates without sub-level detail
- **SME check:** One MMS per change vs. one digest?

---

## Messaging & communication

### H-16: Communicate via MMS with contractor
- **Trigger:** Day-to-day questions during the remodel
- Texts in MMS thread with Ryan (+ project handle # if group)
- Does **not** see sub group threads
- **Success:** Same texting comfort as today; contractor stays hub
- **SME check:** Do homeowners prefer text, email, or portal?

### H-17: Communicate via app with contractor
- **Trigger:** Customer opens portal to ask a question or follow up on a schedule change
- Messages Ryan in contractor↔customer thread inside the app
- Same thread may be mirrored from MMS (`[OPEN: unified inbox?]`)
- **Success:** Question captured on project; Ryan responds when available
- **SME check:** Will they open the app, or only reply in MMS?

### H-18: Photo upload (customer thread)
- **Trigger:** Customer wants to show Ryan a concern (e.g. existing damage before demo)
- Uploads photo via portal or sends in MMS customer thread
- **Success:** Photo on project record in customer-appropriate thread
- **SME check:** Do homeowners send photos often? Before, during, or after?

### H-19: What customers cannot see
- Jesse's photo of issue behind the wall (sub-only thread)
- Sub pricing, sub schedules, who's confirmed vs. pending
- Internal cascade preview ("4 tasks move · Jesse + Nate must re-confirm")
- **Success:** Right-to-know without construction chaos
- **SME check:** Any visibility gaps that cause surprise phone calls?

---

## Cross-project identity

### H-20: Same person, different roles
- **Trigger:** Jesse is remodeling his own bathroom with Oak Lane Builders (Customer) while subbing for Riverside elsewhere
- Gets MMS from both contractors — different links, different roles
- Customer portal on Oak Lane job; sub portal on Riverside job
- **Success:** No single "account type"; context per project
- **SME check:** N/A for most homeowners — relevant if SME is also a tradesperson

---

## What customers do NOT do (v0.1)

- Create a password or download a native app
- **Change** the schedule or **request** a schedule change (view + accept/reject only)
- Message subs directly (contractor is hub)
- See Gantt charts, cascade engine, or sub roster
- Negotiate dates like subs (no counter-propose)
- Manage billing or change orders in portal (out of v0.1 scope)

---

## Accept/Reject + calendar (subs and customers)

| Calendar linked? | Accept | Reject |
|------------------|--------|--------|
| **No** | Recorded in ContractorPro only | Recorded in ContractorPro only |
| **Yes** | Recorded in ContractorPro **+** linked calendar updated | Recorded in ContractorPro **+** linked calendar updated |

View schedule in portal/app either way.

---

## Connect confirm summary (primary customer at project create)

| Channel | Sent when | Confirmed in system | Poke if pending |
|---------|-----------|---------------------|-----------------|
| **Email** | Project create | `email_confirmed` | Yes — until confirmed |
| **MMS / phone** | Project create | `phone_confirmed` | Yes — until confirmed |

**Gate:** Customer **fully connected** when **both** rows are ✅. Schedule notifications should not rely on a channel until that channel is confirmed — see **H-23** · `[BL-1]`

---

## Visibility & competitive wedge

### H-21: "What changed" timeline
- **Trigger:** Lauren opens portal any time during remodel
- Sees rolling **timeline** (not Gantt): "Cabinets Oct 1→5", "Inspection still Sept 20"
- Each entry links to accept/reject if still pending
- **Contrast:** Buildertrend AI weekly digest from daily logs — ours is **event-triggered**, simpler
- **Success:** Lauren answers "where are we?" without calling Ryan
- **SME check:** How far back? Show cancelled milestones?

### H-22: Milestone-only notifications
- **Trigger:** Ryan moves internal task (rough-in inspection prep) — Lauren **not** notified
- Ryan marks tasks **customer-visible** or **milestone** (cabinets, inspection, walkthrough)
- Lauren only gets MMS for milestone-class changes
- **Success:** Less homeowner anxiety from sub-level noise
- **SME check:** Default all customer-visible or opt-in per task? `[BL-2]`

### H-23: Channel gating before schedule comms
- **Trigger:** Lauren confirmed email ✉️ but not phone 📱 yet
- Schedule **email** updates allowed; schedule **MMS** held until `phone_confirmed`
- Portal always shows full schedule after partial connect
- **Success:** Don't text wrong number; don't email unverified address
- **SME check:** Hard gate vs warn Ryan only? `[BL-1]`

### H-24: Returning customer — fast connect
- **Trigger:** Lauren was customer on Oak Ave last year; new Maple St project
- System recognizes phone/email; may skip re-verifying both channels `[BL-4]`
- One link → accept project connection + optional calendar refresh
- **Success:** Less friction for repeat clients
- **SME check:** Trust prior confirms for how long? Same GC only?

---

## Workshop prompts (customer / homeowner)

1. **Dual confirm:** Email + MMS both required — too much friction for homeowners?
2. **Poke:** Daily reminders on unconfirmed channels — acceptable?
3. **Milestones:** H-22 — what should Lauren actually get pinged for?
4. **Timeline:** H-21 — weekly digest vs event feed?
5. **Boundaries:** Confirm they should **not** be able to request date changes?

**Suggested review order:** H-1 → H-2/H-3 → H-4 → H-23 → H-11 → H-12 → H-14 → H-21 → H-22 → H-16/H-17 → H-7 (family)

Log decisions in [discovery-log.md](../../../discovery-log.md) · Open items: [backlog.md](./backlog.md)
