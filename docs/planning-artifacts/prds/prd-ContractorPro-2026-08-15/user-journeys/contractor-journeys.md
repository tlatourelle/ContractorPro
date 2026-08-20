# Contractor Journeys — SME Review

**Status:** Draft — validate with GC / office manager SMEs  
**Persona:** **Ryan** (contractor / owner) or **Maci** (office manager) — **v0.1: identical access**; examples use Ryan unless noted. Maci-focused flows → [§ Maci](#maci--typical-office-tasks-v01-same-access-as-ryan).  
**Related:** [Subcontractor journeys](./subcontractor-journeys.md) · [Customer journeys](./customer-journeys.md) · [Full detail](../user-journeys.md) · [Backlog](./backlog.md)

Use this list in SME workshops. Ask: *"Is this how you actually work today? What's missing or wrong?"*

---

## Onboarding & project setup

### C-1: Get started on ContractorPro — **MVP self-serve**
- **Trigger:** Ryan finds ContractorPro; signs up without sales call or invite code
- Signs in on laptop (Google OAuth) → **system creates Riverside Remodeling company automatically** (E1-S1)
- Onboarding checklist (E1-S4) guides: connect calendar → create first project → add tasks → invite sub → propose date
- Creates first project (e.g. Maple St Kitchen) — enters **primary customer** name, email, phone on project form
- Adds tasks (Demo, Rough electric, Drywall, Paint — dates optional at first)
- Invites first sub; customer gets connect link automatically (not a separate invite step)
- Assigns a sub to a task and proposes a date
- **MVP:** Full coordination enabled — no payment step
- **Phase 2:** First **Invite sub** or customer notify on sandbox tier triggers upgrade (C-27)
- **Success:** Live project, calendar connected, primary customer on project + one sub invited, one date proposed — ~20–30 min
- **SME check:** Is 20–30 min realistic? What do they skip on day one?

### C-2: Connect company calendar
- **Trigger:** During onboarding or later from settings
- Connects company calendar — **Google OAuth**; ContractorPro **creates one calendar per project** (pro-provided)
- Sees confirmation that dates sync after subs/customers confirm
- **Success:** Contractor calendar linked; subs/customers receive **Google attendee invites** on accept (MVP)
- **SME check:** One calendar per company, or per user? Shared office calendar?

---

## Inviting & assigning subs

### C-3: Invite sub + first date (default path)
- **Trigger:** Assigning a sub their first task on this project with a date
- Picks sub from roster or enters name + phone
- Sets task and date (e.g. Rough electric, Sept 10)
- Saves — system sends one SMS: join + confirm in one link
- Dashboard shows pending ⏳
- **Success:** Sub on roster and date proposed in one action, one text to sub
- **SME check:** Is "assign + date = invite" how they work today?

### C-4: Add sub to roster early (no date yet)
- **Trigger:** Planning phase — knows who'll be on the job but dates aren't set
- Invites sub with name + phone only
- Sub joins; Ryan assigns dated task later (second SMS to sub)
- **Success:** Roster built before schedule is firm
- **SME check:** How often do they invite subs before dates exist?

### C-5: Invite returning sub (phone already known)
- **Trigger:** Adding Jesse to a new project — was sub on Main St last year
- Types phone; system suggests "Jesse Torres — was on Main St"
- Confirms name, assigns task + date (or invite only)
- **Success:** Faster invite; Ryan doesn't need to remember if Jesse is "in the system"
- **SME check:** Do they keep a mental rolodex? Excel? BT contact list?

---

## Scheduling & confirmations

### C-6: Propose a date → wait for sub confirm
- **Trigger:** Sub already on project; Ryan sets or changes a task date
- Assigns sub to task, sets date, saves
- Dashboard shows pending until sub accepts or declines
- Sub response **always** recorded in system; sub's linked calendar updated when calendar linked
- Gets in-app notification when sub confirms ✅ or declines ❌
- **Success:** Confirmed status in app; sub calendar sync when linked
- **SME check:** How do they chase subs today — call, text, ignore?

### C-7: Sub doesn't respond (poke)
- **Trigger:** Sub got SMS but didn't open it
- Dashboard shows ⏳ Pending (day 1, day 2…)
- System sends automatic reminder SMS at +24h, +48h, then daily
- Ryan can tap **Send reminder now** or **Snooze 2 days**
- **Success:** Either sub confirms without Ryan chasing, or Ryan has visibility to chase intentionally
- **SME check:** Is daily poke too aggressive? What cadence feels right?

### C-8: Reschedule → sub must re-confirm
- **Trigger:** Cabinet delay pushes paint from Sept 10 → Sept 11
- Drags task to new date (or edits)
- Sees preview: "Jesse must re-confirm"
- Saves; Jesse gets SMS with old → new date
- **Success:** No silent calendar move — sub explicitly agreed
- **SME check:** Do they always re-confirm, or sometimes just text "we moved to the 11th"?

### C-9: Sub requests reschedule → Ryan decides
- **Trigger:** Jesse texts in MMS "can't make Sept 10" or requests date in portal
- Gets in-app (+ optional SMS) alert: "Jesse requested Paint move: Sept 10 → Sept 12"
- Opens request; chooses **Accept**, **Counter-propose**, or **Decline**
- **Success:** Conflict surfaced in-app; Ryan chose whether Sept 12 works
- **SME check:** Does sub-initiated reschedule happen often? MMS-first or app-first?

### C-10: Counter-propose (date negotiation)
- **Trigger:** Jesse requested Sept 12; Ryan can do Sept 11 but not 12
- Suggests Sept 11 with optional note
- Pending flips back to Jesse; loop until Accept or Decline
- Negotiation history visible on dashboard
- **Success:** 2–3 rounds without phone tag; calendars stay on last confirmed date until agreement
- **SME check:** How many back-and-forth rounds are normal?

### C-11: Sub declines → reassign
- **Trigger:** Jesse declined Sept 11 — booked elsewhere
- Gets immediate alert: "Jesse DECLINED Paint — Sept 11"
- Options: propose new date to Jesse, message Jesse, or **Assign Nate instead**
- Reassigning closes Jesse's assignment, notifies Nate (invite + propose if new)
- **Success:** Moved on in one action; schedule didn't stall
- **SME check:** How fast do they find backup subs? Same trade or call around?

---

## Cascade & portfolio

### C-12: Cascade after a slip
- **Trigger:** Framing finishes 3 days late; downstream tasks must move
- Moves framing +3 days with cascade on
- Reviews preview: "4 tasks move · Jesse + Nate must re-confirm"
- Confirms; each affected sub gets old → new + link
- Dashboard shows mixed ✅ / ⏳ until all confirm
- **Success:** One edit, controlled ripple, no silent calendar chaos
- **SME check:** Is cascade always on, or per-job? Do they preview before committing?

---

## Messaging & field comms

### C-13: Set up MMS group thread per sub
- **Trigger:** Inviting or assigning Marcus on Maple St
- System provisions project handle # (e.g. (555) 100-0001)
- UI prompts: add Marcus + Maple handle # to group text
- Creates group MMS: Ryan + Marcus + handle # in native Messages
- **Success:** Conversation captured on project; same handle # for every sub on this job (separate groups)
- **SME check:** Will they create a new group, or try to use an existing thread?

### C-14: Read & reply in MMS (mirrored in app)
- **Trigger:** Marcus texts "supplier slipped — can't start Thursday"
- Reads on phone in group MMS or in web app thread mirror
- Replies in MMS if needed
- **Success:** Field comms logged without sub opening portal
- **SME check:** Do they want app-only, MMS-only, or both?

### C-15: Commit schedule change after MMS conversation
- **Trigger:** After discussing delay in MMS, Ryan moves Flooring to Tuesday in web app
- Reschedules in app (C-8 flow)
- System sends confirmation MMS to group: "Flooring → Tuesday. Confirm: [link]"
- **Success:** Talk in MMS; commit in app + magic link
- **SME check:** Is this two-lane pattern (MMS talk / app schedule) how they think?

### C-16: Photo in thread
- **Trigger:** Jesse sends photo of issue behind the wall
- Sees photo in phone thread + web mirror on project record
- **Success:** Field photo not lost in personal Ryan↔Jesse thread
- **SME check:** MMS photo enough, or do they need app upload too?

---

## Customer coordination

### C-17: Add primary customer at project creation
- **Trigger:** Ryan creates Maple St Kitchen
- Enters Lauren's **name, email, phone** as primary customer on the project form (from contract intake)
- System sends Lauren **email + MMS** immediately — each with its own confirm link
- Dashboard tracks **Email** ⏳/✅ and **MMS** ⏳/✅ separately
- **Poke engine** runs on unconfirmed channel(s) until **both** confirmed (+24h, +48h, daily default)
- Lauren fully connected per customer journeys H-2–H-6
- **Success:** Primary customer on project; both channels verified before Ryan relies on either
- **SME check:** Always have customer info at create? Chase if only one channel confirms?

### C-17b: Invite additional customer member (family)
- **Trigger:** Ryan wants spouse, partner, or family member on the project too
- Invites additional person as **Customer** via **MMS**
- Same accept/reject flows as primary; does not replace Lauren as primary contact
- **Success:** Extra customer-side account for family who need portal/MMS access
- **SME check:** How often? Every job or edge case?

### C-18: Customer schedule change — accept/reject tracking (Ryan's side)
- **Trigger:** Ryan moves cabinet install Oct 1 → Oct 5
- System sends MMS to Lauren with accept/reject link
- Ryan dashboard shows ✅ accepted, ❌ rejected, or ⏳ pending
- Lauren's accept/reject **always** recorded in system; her linked calendar updated only when calendar linked
- **Success:** Ryan knows Lauren's response; calendar sync when Lauren has linked
- **SME check:** What changes matter vs. noise? Chase if no response?

---

## Portfolio & operations

### C-19: Morning / portfolio triage
- **Trigger:** Ryan or Maci opens ContractorPro at start of day — 4 active jobs
- **Action queue** shows across all projects:
  - Sub confirms pending (⏳ by sub, by task)
  - Customer channel confirms pending (✉️/📱 per Lauren)
  - Sub-initiated reschedule awaiting Ryan (C-9)
  - Cascade partially confirmed (mixed ✅/⏳)
  - Poke escalations (day 2+, day 3+)
- Taps item → lands on project/task
- **Success:** One screen answers "what needs attention?" — not hunting per project
- **SME check:** Sort by urgency, project, or date? What rises to top?

### C-20: Cascade preview before commit (expanded)
- **Trigger:** Framing slip +3 days; cascade enabled (extends C-12)
- Before save, reviews **preview panel:**
  - Each task: old date → new date
  - Each sub: must re-confirm (yes/no)
  - Customer-visible milestones affected (Lauren notifications)
  - Optional: partial cascade — select which dependents move `[BL-11]`
- Edits preview list, adds slip reason note, confirms
- **Success:** No surprise ripple; Ryan knows full blast radius
- **SME check:** Preview mandatory or skippable for power users?

### C-21: Draft schedule change (mute notifications)
- **Trigger:** Ryan exploring dates before telling subs — supplier delay uncertain
- Toggles **Draft** (or "Don't notify yet") on schedule edit
- Moves tasks in app; subs and customer **not** notified; calendars unchanged
- Reviews draft impact (same preview as C-20)
- Taps **Publish** → live proposals go out (UJ-1 / UJ-2 / UJ-5)
- **Success:** Internal what-if without waking Jesse at 10pm
- **SME check:** Draft per session or persistent until publish? `[BL-10]`

### C-23: Sub requests reschedule → Ryan must respond (poke GC)
- **Trigger:** Jesse requested Sept 12 (C-9); Ryan hasn't acted in 48h
- Jesse already notified when he submitted request
- System **pokes Ryan** (in-app + optional SMS): "Jesse waiting on Paint reschedule"
- Ryan accepts, counter-proposes, or declines
- **Success:** Sub-initiated changes don't die on Ryan's desk
- **SME check:** Poke Ryan at 48h? Sooner? `[BL-9]`

### C-24: Wrong contact / typo recovery
- **Trigger:** Ryan entered Lauren's phone wrong; wrong person got MMS
- Edits customer email or phone on project
- System invalidates outstanding magic links for old contact
- Resends dual-channel confirm to corrected address
- Audit log: "Contact updated; confirms reset"
- **Success:** Recover without duplicate memberships or ghost users
- **SME check:** Who can edit customer PII — Ryan, Maci, both? (v0.1: both)

### C-25: Project complete / archive
- **Trigger:** Maple St punch list done; job closed
- Marks project **Complete** or **Archived**
- Stops all poke schedules; portal read-only for Lauren
- MMS handle remains for history but no new auto-messages
- Calendar events marked complete / removed per rules
- **Success:** Clean end-of-job; no stray reminders months later
- **SME check:** Can reopen archived project?

### C-26: Project photo timeline (needs discovery)
- **Trigger:** Ryan wants all field photos for Maple St in one place
- Chronological feed across sub threads, customer thread, MMS ingest
- Filter by trade, date, uploader
- **Status:** `[BL-15]` — MVP or v0.1.1?
- **Success:** Dispute resolution, progress review without scrolling threads

### C-27: Billing / upgrade to unlock coordination — **Post-MVP (Phase 2)**
- **Trigger:** Ryan finished laying out Maple St in free tier; taps **Invite sub** or saves customer contact that would notify Lauren
- **MVP (Phase 1):** All coordination features open — no upgrade prompt; validate core loop with design partners
- **Phase 2 — free tier allows:** sign-up, company setup, create projects, add tasks/dates, internal schedule editing, cascade **preview** (no publish)
- **Phase 2 — free tier blocks:** sub invite, customer outbound confirm (email/MMS), propose dates that notify, poke, cascade publish, MMS threads, any SMS/MMS send
- Upgrade prompt: *"Subscribe to invite subs and notify customers"* → Stripe Checkout
- **Paid tiers (draft):** ~$100/mo · up to 5 concurrent **active** projects with comms · $200/mo · up to 10 · linear
- At plan cap (e.g. 6th active project on $100 tier): plan-only mode or prompt upgrade — prefer plan-only
- **Success:** Ryan experiences product before paying; conversion at first real coordination moment; telco COGS only on paying tenants
- **Vendor:** Stripe Billing + Customer Portal for self-serve manage/cancel
- **Stories:** E1-S3, E1-S5, E1-S6 · **Decision:** [discovery-log.md](../../../discovery-log.md)

---

## Maci — typical office tasks (v0.1: same access as Ryan)

At Riverside's size, **Maci has the same product permissions as Ryan**. These journeys describe **who usually does the work**, not separate roles. Post-POC admin vs office-worker split (billing, subscriptions) → [backlog.md](./backlog.md) **FJ-6**.

### M-1: Morning desk triage
- **Typical actor:** Maci
- **Trigger:** Ryan is on site; Maci opens laptop first thing
- Runs **C-19** action queue: pending sub confirms, Lauren channel confirms, poke escalations
- Calls Jesse if day 3 pending; sends **Send reminder now**
- Flags reschedule requests for Ryan's call when needed
- **Success:** Office absorbs chase work; Ryan stays in field

### M-2: Customer intake at project create
- **Typical actor:** Maci
- **Trigger:** Contract signed; Ryan asks Maci to set up Maple St
- Creates project; enters Lauren name, email, phone (**C-17**)
- Monitors ✉️/📱 confirm status; pokes unconfirmed channels (**H-4**)
- **Success:** Customer fully connected before first schedule publish

### M-3: Sub roster & date entry
- **Typical actor:** Maci
- **Trigger:** Ryan texted "get Jesse on paint Sept 10"
- Adds Jesse via **C-3** or **C-5**; assigns task + date
- Sets up MMS group prompt for Ryan to create on phone (**C-13**)
- **Success:** Schedule action done without Ryan at desk

### M-4: MMS mirror & flag for Ryan
- **Typical actor:** Maci
- **Trigger:** Marcus MMS ingested overnight — supplier delay
- Reads thread in app (**C-14**); tags or notes for Ryan
- Does **not** move schedule until Ryan commits (**C-15**) unless team norm allows
- **Success:** Nothing lost from overnight texts

### M-5: Cascade publish after Ryan's call
- **Typical actor:** Maci (execute) + Ryan (decide)
- **Trigger:** Ryan calls in: "push everything 3 days from framing"
- Maci runs **C-20** preview, reads impact to Ryan on phone
- Ryan approves; Maci confirms cascade publish
- **Success:** Field decision + desk execution split

---

## Workshop prompts (contractor)

1. Which journeys happen **every week** vs. rarely?
2. Where does Ryan lose patience today? (chasing subs? explaining delays to homeowners?)
3. Desktop vs. phone — which actions must work from the truck?
4. Ryan vs. Maci — same buttons, different habits: who does M-1–M-5 in your shop?
5. Anything here that's **too much for v0.1**? → see [backlog.md](./backlog.md)

**Suggested review order:** C-3 → C-5 → C-6 → C-7 → C-8 → C-19 → C-20 → C-9/C-10 → C-11 → C-12 → C-13/C-15 → C-17 → C-18 → **UJ-9** (full detail)

Log decisions in [discovery-log.md](../../../discovery-log.md).
