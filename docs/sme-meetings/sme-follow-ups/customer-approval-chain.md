# SME Follow-Up: Customer Role in Approval Chain (2B)

| Field | Value |
|-------|-------|
| **Status** | Open — needs Ryan + Maci alignment |
| **Session type** | Dedicated 30–45 min — **customer approval chain only** |
| **Blocked by** | Nothing |
| **Blocks** | Final PRD customer gate rules; cascade template `customer_gate` slot |
| **Workbook** | [decision-workbook.md](../decision-workbook.md) §2B |
| **Source** | SME Meeting 01 transcript; Maci vs Ryan tension |
| **Related flow** | Plan → **Publish prelim** → *(this decision)* → **Finalize & start sub cascade** |

---

## The one decision we're trying to get

> **Does a customer action ever hold up sub invites or schedule commits — and if so, when?**

Everything else in this doc is detail around that question.

---

## Why this is open

Two valid operating models came out of the lunch:

| Voice | Position |
|-------|----------|
| **Maci** | Customer should see schedule first; **approve** skeletal plan before subs go out; **approve/acknowledge** on meaningful changes — "industry standard" |
| **Ryan** | Customer constraints captured **at contract signing** (blackouts, access, surgery dates); ongoing per-trade customer approval gives too much veto; subs cascade after internal plan is set |

**Transcript anchors:**

- Ryan: customers can't approve/reject like subs; signing captures blackout/access constraints.
- Maci: "customer first should be industry standard"; wants action on first schedule and on changes; softened — "approval is hard because you don't want to give them that much say" → at least **acknowledgement**.
- Ryan today: skeletal calendar preview to customer → first sub → chain down; sometimes "are we good here?" on phase pushes (often rough trades), often by phone.

Current PRD leans **acknowledge**, not hard **approve**, on task dates. Do not lock product behavior until SMEs pick a lane (or hybrid).

---

## What we need from Ryan + Maci

### 1. At **Publish prelim** — what must happen before **Finalize & start subs**?

Pick one (or define hybrid):

| Option | Customer does | Sub cascade |
|--------|---------------|-------------|
| **Preview** | Sees schedule; no tap required | GC proceeds when ready |
| **Acknowledge once** | Taps "I've seen it" / "Looks good" | Blocked until tap |
| **Approve** | Can say no / request changes | Blocked until yes |
| **Already handled** | Constraints only at contract signing | GC proceeds; prelim is courtesy |

**Tension to resolve:**

- **Maci:** customer first, approve skeletal plan, then subs.
- **Ryan:** blackouts at signing; preview calendar; subs chain after; customers don't approve like subs.

**Decision needed:** Is **Publish prelim** informational or a **hard gate**?

---

### 2. After subs are live — when dates move, does customer act again?

| Scenario | Notify only? | Acknowledge? | Approve (can block)? |
|----------|--------------|--------------|----------------------|
| Minor slip (1–2 days, same week) | ☐ | ☐ | ☐ |
| Major shift (demo moves a week) | ☐ | ☐ | ☐ |
| Customer-visible milestone only | ☐ | ☐ | ☐ |
| Internal / sub-only task moves | ☐ | ☐ | ☐ |

**Ask them:** Which rows actually happen in real life vs "we'd just call them"?

**Tension to resolve:**

- **Maci:** wants approval/acknowledgement on schedule **changes**.
- **Ryan:** sometimes "are we good here?" on phase pushes; often phone, not systematic.

---

### 3. What is **customer-visible** on the prelim calendar?

Mark one (or describe):

| ☐ | Visibility |
|---|------------|
| ☐ | Every trade and date |
| ☐ | Milestones only (demo, cabinets, inspection, move-in) |
| ☐ | Everything except sub names / internal notes |
| ☐ | Other: _________________________________ |

**Why it matters:** "Customer approve" might mean veto on **paint Tuesday** or only **demo day** — completely different product behavior.

---

### 4. If customer **doesn't respond** to prelim

| Question | Answer |
|----------|--------|
| GC override after N days and start subs anyway? | ☐ Yes ☐ No — N = ___ days |
| Daily poke like subs until they respond? | ☐ Yes ☐ No |
| How many days before Ryan proceeds without hearing back? | ___ days |

---

### 5. **Words matter** — button copy

Maci backed off "approve" — don't want homeowners to feel they have veto power.

**What exact button copy would they use with a homeowner?**

| ☐ | Copy option |
|---|-------------|
| ☐ | "Confirm schedule" |
| ☐ | "Looks good — proceed" |
| ☐ | "I've reviewed the plan" |
| ☐ | Other: _________________________________ |

**Follow-up:** Is that tap **"you may enter my home"** or just **"I saw the dates"**?

---

### 6. Ryan's **real workflow today** (walkthrough — one job)

Use a real project (e.g. Maple St). Not hypotheticals.

| # | Question | Notes |
|---|----------|-------|
| 1 | When does the customer first see dates — before **any** sub, or after some subs confirmed? | |
| 2 | Has a customer ever **stopped** a job because of a date in writing/portal, or is it always a phone call? | |
| 3 | When Ryan checks "are we good here?" on a phase push — **every** customer or **some** jobs? | |
| 4 | Walk through last kitchen/remodel: prelim → customer → sub #1 → chain. Where did customer actually engage? | |

---

### 7. Edge cases (quick yes/no)

| # | Question | Yes | No | N/A |
|---|----------|-----|-----|-----|
| 1 | **Second homeowner / spouse** — same gate or primary contact only? | ☐ | ☐ | ☐ |
| 2 | **Customer says no mid-job** — replan in app + customer tap again, or Ryan handles offline? | ☐ | ☐ | ☐ |
| 3 | **Subs confirming while customer prelim still pending** — allowed? (Product lean: **no**) | ☐ | ☐ | ☐ |

---

## Worksheet for the room (mark cells)

```text
                        Preview    Acknowledge    Approve/block
Prelim (before subs)       ☐            ☐              ☐
Date change (major)        ☐            ☐              ☐
Date change (minor)        ☐            ☐              ☐
Phase push ("are we good") ☐            ☐              ☐
Sub-only date moves        ☐            ☐              ☐
```

**Facilitator:** Have Ryan and Maci mark the same sheet independently, then reconcile deltas.

---

## Options summary (don't lead — present neutrally)

| Option | Summary | Maci fit | Ryan fit |
|--------|---------|----------|----------|
| **Preview only** | Customer sees calendar; subs proceed on GC say-so | Low | High |
| **Acknowledge once** | One tap at first publish; notify on changes | Medium | Medium |
| **Acknowledge on changes** | Tap when customer-visible tasks shift | High | Low–Medium |
| **Approve gates** | Customer can hold sub cascade | High | Low |
| **Signing only** | Blackout dates at setup; no portal gate | Low | High |

---

## Questions for Thomas **before** SME #2

Resolve internally so Ryan/Macie aren't designing the engine:

| # | Question | Thomas answer |
|---|----------|---------------|
| 1 | **Can GC always override a customer gate?** (Ryan will want an escape hatch.) | |
| 2 | **Is "Publish prelim" the only pre-sub gate**, or can templates also gate mid-plan (e.g. before rough trades)? | |

---

## What NOT to discuss in this session

Keep the room focused — redirect if conversation drifts:

| Off-topic | Why |
|-----------|-----|
| Phone number model / Twilio / inbox routing | Separate decision (#1) |
| OTP vs magic link | Separate decision (#6) |
| QR / Google Drive / check-in | Separate decisions (#4, #5) |
| Buildertrend bashing | Steer back to *their* process today |

---

## Interim engineering rule (until DECIDED)

- Cascade / template engine supports an **optional `customer_gate` step** — **off by default**
- No customer blocker in MVP UX until this doc is marked DECIDED
- Customer portal remains **read-only schedule view** + messaging regardless of gate outcome

---

## Session todo

- [ ] Schedule SME touchpoint — Ryan + Maci — **customer approval chain only**
- [ ] Resolve Thomas pre-questions (GC override? single prelim gate only?)
- [ ] Print or share worksheet + scenario table
- [ ] Facilitate independent mark-up, then reconcile
- [ ] Record decision in **Decision** section below
- [ ] Update [decision-workbook.md](../decision-workbook.md) §2B
- [ ] Propagate to PRD customer journeys + cascade templates (after correct-course)

---

## Decision (fill in after SME session)

| Field | Value |
|-------|-------|
| **Choice** | _TBD_ |
| **Prelim gate** | Preview / Acknowledge / Approve / Signing-only |
| **Change gate** | Notify / Acknowledge / Approve — which scenarios |
| **Customer-visible scope** | |
| **Non-response policy** | |
| **Button copy** | |
| **GC override allowed?** | |
| **Decided by** | |
| **Date** | |
| **Rationale** | |

---

## Outcome checklist

When SMEs decide:

- [ ] This doc → status **DECIDED** with table above complete
- [ ] [decision-workbook.md](../decision-workbook.md) §2B updated
- [ ] PRD customer journeys + FRs (via correct-course when ready)
- [ ] Cascade template model: `customer_gate` default on/off per template type
