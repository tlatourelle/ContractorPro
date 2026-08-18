# Subcontractor Journeys — SME Review

**Status:** Draft — validate with trade subcontractor SMEs  
**Persona:** Subcontractor (e.g. Jesse — painter, Marcus — flooring; invited to one project at a time)  
**Related:** [Contractor journeys](./contractor-journeys.md) · [Customer journeys](./customer-journeys.md) · [Full detail](../user-journeys.md)

**Design principle:** Subs do **not** create accounts, download an app, or get a global login. They have a **project membership** + verified phone + magic links for **this project**.

**Accept/Reject + calendar** (same rule as customers): always recorded in ContractorPro; linked personal calendar updated when calendar is linked. See [full detail](../user-journeys.md#design-principle-acceptreject--calendar-subs-and-customers).

**Calendar providers:** Google Calendar + Apple iCal/iCloud at v0.1 — **Google preferred** (default connect option).

Use this list in SME workshops. Ask: *"Would you actually tap that link? What would make you ignore it?"*

---

## Joining a project

### S-1: First invite + confirm date (default — one SMS)
- **Trigger:** Ryan assigns Jesse his first task on Maple St with a date
- Gets one SMS: `[Riverside Remodeling] Maple St Kitchen — Rough electric, Sept 10. Tap to join and confirm: [link]`
- Taps link on phone (between jobs)
- **Join** screen — confirm name + phone (one screen)
- Taps **Accept** or **Decline** the date
- **Accept/Decline** always recorded in ContractorPro; linked calendar updated if calendar linked (see rule below)
- **Success:** One text, one visit, on the job and date agreed (or declined)
- **SME check:** Will they tap an unknown link? What does the SMS need to say to trust it?

### S-2: Join project only (no date yet)
- **Trigger:** Ryan added Jesse to roster before dates are set
- Gets invite-only SMS → join portal → waits
- Later gets second SMS when Ryan proposes a date (S-3)
- **Success:** On roster; no date commitment yet
- **SME check:** Is two-text flow annoying enough to avoid?

### S-3: Returning sub — phone already known
- **Trigger:** Jesse was on Main St last year; now invited to Oak Ave (same contractor)
- May skip join screen on trusted device → straight to Accept/Decline
- Or join with name pre-filled — one tap
- If different contractor: same flow but SMS clearly from Riverside
- **Success:** Feels like "new job text," not "log into my account"
- **SME check:** Do subs work for multiple GCs? How do they tell texts apart?

### S-4: Edge cases
- Wrong person got link → phone verify fails → cannot join
- Forwarded invite → may require OTP match to invited phone
- Already on project → "Already joined" or resend link
- Same phone is Customer on another project → new membership as Sub — roles don't collide
- **SME check:** Any deal-breakers in verify flow?

---

## Calendar (optional)

### S-5a: Link calendar to project
- **Trigger:** During join (S-1) or later from sub portal settings
- Chooses **Google Calendar** (preferred / shown first) or **Apple Calendar (iCal / iCloud)**
- Calendar linked to **this project** — optional, not required
- **Success:** Ready to receive assignment events on personal calendar when accepting
- **SME check:** Do subs use Google, Apple Calendar, or only texts?

---

## Confirming & responding to dates

### S-5: Accept or decline a proposed date
- **Trigger:** Ryan set paint date Sept 10 (Jesse already on project)
- Gets SMS: `[Riverside Remodeling] Painting — Maple St, Sept 10. Accept or decline: [link]`
- Taps link → one screen: task, date, **Accept** / **Decline**
- **Accept** or **Decline** → always recorded in ContractorPro
- If calendar linked → linked calendar updated on accept (event added) or decline (no event / unchanged per decline rules)
- If calendar **not** linked → view schedule in portal only
- **Success:** Confirmed or declined in system; Ryan sees status; calendar sync when linked
- **SME check:** Accept/Decline enough, or do they need "maybe" / "call me"?

### S-6: Ignore the text (poke reminders)
- **Trigger:** Got S-5 SMS but didn't open it
- Day 1: nothing (Ryan sees pending)
- +24h: reminder SMS "Still need your confirmation…"
- +48h: stronger reminder
- Then one SMS/day max (batched if multiple tasks)
- **Success:** Eventually confirms, or Ryan calls them
- **SME check:** Daily poke — helpful nudge or spam?

### S-7: Contractor rescheduled → must re-confirm
- **Trigger:** Ryan moved paint Sept 10 → Sept 11
- Gets SMS: `Sept 10 → Sept 11. Please confirm: [link]`
- Calendar still shows **Sept 10** until Jesse acts (when calendar linked)
- Taps **Accept**, **Counter-propose**, or **Decline**
- Accept/Decline recorded in system; linked calendar updated when calendar linked
- **Success:** No surprise calendar move — explicitly agreed to new date
- **SME check:** Do subs check calendar or rely on texts?

### S-8: Request a different date
- **Trigger:** Booked on another job; can't make Sept 10
- Opens assignment from SMS link
- Taps **Request different date** → picks Sept 12 (optional note: "Conflict on another job")
- Waits for Ryan to Accept / Counter-propose / Decline
- **Success:** Surfaced conflict without no-showing
- **SME check:** Would they use app, or just text Ryan in MMS?

### S-9: Counter-propose (negotiate)
- **Trigger:** Ryan suggested Sept 11 instead of Jesse's Sept 12
- Gets SMS: `Ryan suggested Sept 11 instead of Sept 12. Confirm: [link]`
- **Accept** Sept 11, **Counter-propose** Sept 13, or **Decline**
- Calendars stay on last confirmed date through whole thread
- **Success:** Real scheduling in 2–3 rounds without phone tag
- **SME check:** How many rounds before they just call?

### S-10: Decline a date
- **Trigger:** Can't do proposed date — booked elsewhere
- Taps **Decline** on link
- Decline recorded in ContractorPro; linked calendar updated if linked (e.g. no new event, or prior confirmed date unchanged on reschedule decline)
- Sees "Decline recorded. [Contractor] will be in touch."
- Ryan alerted immediately
- **Success:** Ryan knew within minutes, not on job day when Jesse no-shows
- **SME check:** Decline vs. ghost — which is more common?

### S-11: Cascade — multiple tasks move at once
- **Trigger:** Framing slip moved 4 downstream tasks
- Gets SMS per affected assignment: old → new + link
- Confirms each individually — accept/reject recorded in system; calendar updated per item when linked
- **Success:** Each task re-confirmed in system; calendar sync when linked
- **SME check:** One SMS with all changes, or one per task?

---

## Messaging & field comms

### S-12: Group MMS with contractor + project handle
- **Trigger:** Ryan set up Maple St group: Ryan + Jesse + project handle #
- Texts group like any normal group chat
- Questions, delays, photos, "can't make it"
- Does **not** need to open ContractorPro for conversation
- **Success:** Same texting habit as today; thread logged on project
- **SME check:** Will they add a third number to the group? Resistance?

### S-13: Report delay in MMS → confirm new date via link
- **Trigger:** Texts "Supplier slipped — can't start Thursday"
- Ryan reads in MMS, reschedules in app
- Gets MMS in same thread: `[Maple St] Flooring → Tuesday. Confirm: [link]`
- Taps link, Accept
- **Success:** Talked in text; committed via link
- **SME check:** Is the confirmation link in the group thread trusted?

### S-14: Send photo of issue
- **Trigger:** Found problem behind the wall
- **Path A (default):** Sends photo in group MMS
- **Path B (optional):** Opens magic link → Messages → camera upload
- **Success:** Ryan has photo on project record
- **SME check:** MMS photo is enough?

---

## Cross-project identity

### S-15: Same person, different roles on different jobs
- **Trigger:** Jesse is Sub on Maple St (Riverside) and Customer on Oak Lane (different contractor)
- Gets SMS from both — different links, different roles
- No single "account type"; two separate memberships
- **Success:** System never assumes Jesse is "always a sub"
- **SME check:** How confusing is multiple roles on one phone?

---

## Preferences & edge cases

### S-16: Notification channel preference (`notify_via`)
- **Trigger:** Jesse hates email; only reads texts
- At invite or in portal, preference: **SMS** | **email** | **both** for schedule proposals and pokes
- Proposals and reminders respect preference
- **Success:** Messages arrive where Jesse actually looks
- **SME check:** Who sets this — Jesse, Ryan, or default both? `[BL-8]`

### S-17: Open confirmations across jobs (v0.1 lean)
- **Trigger:** Jesse has pending confirm on Maple St and Oak Ave
- **v0.1:** Each SMS link is project-scoped — two texts, two taps
- **v0.2:** Unified "Your open items" portal → [future-journeys-v02.md](./future-journeys-v02.md) **FJ-4**
- **Success:** v0.1 works without global account; v0.2 reduces link fatigue
- **SME check:** How many concurrent GCs does a sub typically juggle?

### S-18: Batch confirm after cascade
- **Trigger:** Cascade moved 3 of Jesse's tasks on Maple St
- **Option A:** One SMS listing all old → new + single link to confirm all
- **Option B:** One SMS per task (current default lean)
- Jesse accepts all or reviews each
- **Success:** Fewer texts on big slips
- **SME check:** Batch vs per-task? `[BL-5]`

### S-19: Quiet hours
- **Trigger:** Poke would fire at 9pm; Jesse's on quiet hours
- SMS/email pokes deferred until window opens (e.g. 7am)
- Urgent GC escalation to Ryan still allowed `[OPEN]`
- **Success:** Less "stop texting me at night" backlash
- **SME check:** Company default or per-sub? `[BL-7]`

### S-20: Removed from task — courtesy message
- **Trigger:** Nate replaced Jesse on Paint (C-11 / UJ-2e)
- Optional SMS: "You're off Paint on Maple St. Other assignments unchanged."
- **Success:** Jesse not wondering why confirm link stopped
- **SME check:** Auto-send or Ryan taps send? `[BL-18]`

### S-21: Decline to join project
- **Trigger:** Jesse gets invite+date SMS but taps Decline on **join** (not date)
- No membership created; Ryan alerted
- Rare — usually wrong number or not interested
- **Success:** Ryan knows not to expect Jesse on site
- **SME check:** Different copy for "decline join" vs "decline date"?

### S-22: MMS before handle # ready
- **Trigger:** Jesse texts Ryan's personal cell about job; group not set up yet
- **v0.1 lean:** Message not on project record until group exists
- **Target:** Warn Ryan "Set up Marcus MMS group"; optional ingest after setup `[BL-13]`
- **Success:** Ryan nudged to complete C-13 setup

---

## Accept/Reject + calendar (subs and customers)

| Calendar linked? | Accept | Reject |
|------------------|--------|--------|
| **No** | Recorded in ContractorPro only | Recorded in ContractorPro only |
| **Yes** | Recorded in ContractorPro **+** linked calendar updated | Recorded in ContractorPro **+** linked calendar updated |

View schedule in portal/app either way. Counter-propose / request different date are sub-only negotiation flows — still recorded in system; calendar stays on last confirmed date until agreement.

---

## What subs do NOT do (v0.1)

- Create a ContractorPro account or password
- Download an app
- Propose cascade or move other subs' dates
- See customer threads, other subs' threads, or pricing
- Schedule in the app (they **confirm**; Ryan schedules)

---

## Workshop prompts (subcontractor)

1. What makes you **tap** vs. **ignore** a GC's scheduling link?
2. Accept/Decline/Counter-propose — enough buttons?
3. MMS group with a project number — would you use it?
4. Daily reminder texts — help or annoyance?
5. Calendar sync — Google, Apple, or texts only?

**Suggested review order:** S-1 → S-3 → S-5 → S-6 → S-7 → S-8/S-9 → S-10 → S-12/S-13 → S-14 → S-16 → S-18

Log decisions in [discovery-log.md](../../../discovery-log.md) · Open items: [backlog.md](./backlog.md)
