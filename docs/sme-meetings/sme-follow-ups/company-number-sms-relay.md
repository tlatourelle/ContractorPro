# SME Follow-Up: Company # SMS Relay (Ryan + Maci)

| Field | Value |
|-------|-------|
| **Status** | Open — validate flows with Ryan + Maci before build lock |
| **Session type** | Dedicated 30–45 min — **company texting only** |
| **Blocked by** | Nothing (architecture direction decided) |
| **Blocks** | Final inbox UX copy; staff onboarding; E6 messaging epic scope |
| **Workbook** | [decision-workbook.md](../decision-workbook.md) §1 |
| **Architecture** | [company-number-messaging.md](../../planning-artifacts/technical-exploration/company-number-messaging.md) |
| **Source** | SME Meeting 01 (ReviewWave mental model); Winston architect session 2026-08-20 |

---

## The one thing we're validating

> **Will Ryan and Maci actually text Acme's company # back (not the sub's cell) — and is the relay UX acceptable when two subs are active at once?**

Architecture is **Pattern A (SMS relay hub)** with **lenient token mode**: no codes on most replies; system asks when ambiguous.

---

## What we're **not** re-litigating

- One company # per contractor (not per project)
- Subs/customers get **one number** to save — Acme's line
- Personal-cell job texts stay **out of scope** for logging
- Schedule accept/decline stays **magic link**, not "text YES"

---

## How it works (30-second pitch for the room)

```text
Mike texts Acme's number → Ryan AND Maci get a text FROM Acme with Mike's message.

Ryan replies TO Acme's number → Mike gets the reply FROM Acme;
                              → Maci gets a copy ("Ryan → Mike: …").

Nobody gives subs their personal cells for job coordination.
```

---

## Example flows — walk through live

Use **Acme Remodeling**, **(555) 200-ACME**, Ryan, Maci, Mike (painter), Jose (electrician).

### Flow 1 — Happy path (one active sub)

| Step | Who | Message |
|------|-----|---------|
| 1 | Mike → Acme # | "Can I start Tuesday?" |
| 2 | Acme # → Ryan | `Acme [7K2M] Mike·Maple: Can I start Tuesday?` |
| 3 | Acme # → Maci | same |
| 4 | Ryan → Acme # | `Yes Tuesday works` *(no code — only Mike active)* |
| 5 | Acme # → Mike | `Yes Tuesday works` |
| 6 | Acme # → Maci | `[Ryan→Mike] Yes Tuesday works` |

**Ask Ryan/Macie:** Is step 4 natural? Would you remember to reply to **Acme #**, not Mike?

---

### Flow 2 — Two subs text close together (disambiguation)

| Step | Who | Message |
|------|-----|---------|
| 1 | Mike → Acme # | "Can I start Tuesday?" |
| 2 | Jose → Acme # | "Need to push electrical to Friday" |
| 3 | Acme # → Ryan | Two forwards with `[7K2M]` and `[4NPQ]` |
| 4 | Ryan → Acme # | `Yes Tuesday` *(no code — ambiguous!)* |
| 5 | Acme # → Ryan | `Acme: 2 active chats — 7K2M Mike (Maple) / 4NPQ Jose (Oak). Reply with code + message` |
| 6 | Ryan → Acme # | `7K2M Yes Tuesday works` |
| 7 | Acme # → Mike | `Yes Tuesday works` |
| 8 | Acme # → Maci | `[Ryan→Mike] Yes Tuesday works` |

**Ask Ryan/Macie:** Is step 5–6 acceptable friction? How often does this happen in real life?

---

### Flow 3 — Maci replies instead of Ryan

| Step | Who | Message |
|------|-----|---------|
| 1 | Mike → Acme # | "What time Tuesday?" |
| 2 | Acme # → Ryan + Maci | forward with `[7K2M]` |
| 3 | Maci → Acme # | `8am sharp` |
| 4 | Acme # → Mike | `8am sharp` |
| 5 | Acme # → Ryan | `[Maci→Mike] 8am sharp` |

**Ask Ryan:** Do you want to see Maci's replies in real time like this? Any "step on toes" concern?

---

### Flow 4 — Double reply (both answer)

| Step | Who | Message |
|------|-----|---------|
| 1 | Mike → Acme # | "Tuesday still good?" |
| 2 | Acme # → Ryan + Maci | forward |
| 3 | Ryan → Acme # | `Yes` *(within 1 min)* |
| 4 | Maci → Acme # | `Yes confirmed` *(within 2 min)* |
| 5 | Acme # → Mike | **Two separate texts** from Acme |

**Ask them:** Is this OK occasionally? Should second reply warn Maci "Ryan already replied 1m ago"?

---

### Flow 5 — System outbound (approval) + sub replies

| Step | Who | Message |
|------|-----|---------|
| 1 | App → Mike (from Acme #) | `Maple St · Acme — Confirm electrical Aug 25: https://app…/c/abc` |
| 2 | Mike → Acme # | "Link didn't work, call me" |
| 3 | Acme # → Ryan + Maci | forward with project context |
| 4 | Ryan → Acme # | `I'll call you in 5` |
| 5 | Acme # → Mike + copy Maci | as in Flow 1 |

**Ask:** Does the **project prefix** in step 1 help when Mike's reply lands on the shared line?

---

### Flow 6 — Wrong behavior (out of scope)

| Step | Who | What happens |
|------|-----|--------------|
| 1 | Ryan texts **Mike's personal cell** directly | Maci **does not** see it; app **does not** log it |

**Confirm:** Product rule — job talk goes through Acme #. Personal cells for emergencies only?

---

## Questions for Ryan + Maci

### Adoption

| # | Question | Ryan | Maci |
|---|----------|------|------|
| 1 | Will you **give subs Acme's #** instead of your cell for job texts? | | |
| 2 | Will you **reply to Acme #** when you get a forward — not Reply-to-Mike? | | |
| 3 | Is seeing each other's replies via `[Ryan→Mike]` copies **enough visibility**? | | |

### Friction

| # | Question | Ryan | Maci |
|---|----------|------|------|
| 4 | Flow 2 disambiguation (`7K2M Yes…`) — **too annoying** or fine? | | |
| 5 | Would you rather **open the app** to reply when 2+ subs are active? | | |
| 6 | Double-reply (Flow 4) — how bad is that in practice? | | |

### Alerts

| # | Question | Ryan | Maci |
|---|----------|------|------|
| 7 | SMS forward from Acme # enough, or also **email** when someone texts? | | |
| 8 | Quiet hours — stop SMS forwards at night? | | |

### Customers vs subs

| # | Question | Ryan | Maci |
|---|----------|------|------|
| 9 | Same Acme # for **homeowners** and **subs** — OK? | | |
| 10 | Homeowner texts "when is demo?" — same relay rules? | | |

---

## What NOT to discuss in this session

| Off-topic | Why |
|-----------|-----|
| Customer approval gate (2B) | Separate follow-up |
| Per-project phone numbers | Retired |
| Group MMS with Ryan's personal phone in thread | Retired |
| Magic link / OTP details | Separate (#6) |

---

## Session todo

- [ ] Schedule SME touchpoint — Ryan + Maci — **company # SMS relay only**
- [ ] Print or share this doc (Flows 1–6 on one page)
- [ ] Walk each flow; mark questions table
- [ ] Confirm "Acme # everywhere" behavior change is acceptable
- [ ] Record decisions in **Decision** section below
- [ ] Update [decision-workbook.md](../decision-workbook.md) §1 notes if copy/UX changes
- [ ] Update [company-number-messaging.md](../../planning-artifacts/technical-exploration/company-number-messaging.md) if SMEs reject disambiguation or relay copies
- [ ] Propagate to PRD FR-14 + E6 epics (after correct-course)

---

## Decision (fill in after SME session)

| Field | Value |
|-------|-------|
| **Relay accepted?** | _TBD_ |
| **Lenient + ask when ambiguous?** | _TBD_ |
| **Staff SMS forward format** | _TBD_ (token in forward — confirm copy) |
| **App vs SMS for reply preference** | _TBD_ |
| **Double-reply mitigation** | _TBD_ |
| **Email alerts?** | _TBD_ |
| **Decided by** | |
| **Date** | |
| **Rationale** | |

---

## Outcome checklist

When SMEs validate:

- [ ] This doc → status **DECIDED** with table above complete
- [ ] [decision-workbook.md](../decision-workbook.md) §1 SME notes updated
- [ ] Onboarding copy drafted ("Text Acme at … / Reply to this number only")
- [ ] E6 stories updated for relay + disambiguation
- [ ] PRD FR-14 (via correct-course when ready)
