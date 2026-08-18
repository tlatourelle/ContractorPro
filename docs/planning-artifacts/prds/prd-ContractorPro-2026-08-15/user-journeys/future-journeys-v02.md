# Future User Journeys (v0.2+)

**Status:** Draft — not v0.1 MVP scope  
**Purpose:** Capture journeys that need discovery or depend on job planning, AI, or role model work.

See [backlog.md](./backlog.md) for open questions. Technical detail: [job-planning-workflow.md](../../../technical-exploration/job-planning-workflow.md).

---

## FJ-1: Plan mode → Finalize schedule

**Cast:** Ryan or Maci  
**Trigger:** Sold job (Maple St) — contract signed, not yet live with subs.

### Ryan / Maci does
- Creates project in **`planning`** status
- Adds **work phases**: Demo (2d), buffer (1d), Rough electric (2d), …
- Picks start date; sees **in-app preview only** — no Google writes, no SMS
- Adjusts durations, buffers, drag phases — fully reversible
- Taps **Finalize schedule** → project becomes **`active`**
- System creates live tasks + enters propose/confirm workflow (UJ-1, UJ-3+)

### Success
- Internal what-if planning without bothering Jesse or Lauren
- Clear commit boundary before external notifications

**SME check:** How long do jobs sit in planning? Who finalizes — Ryan only or Maci too?

---

## FJ-2: Portfolio balance / sub conflict across jobs

**Cast:** Ryan  
**Trigger:** Finalizing Maple St while Jesse is already confirmed on Oak Ave same week.

### Steps
- During plan or active portfolio view, sees **sub conflict panel**
- "Jesse: Paint Maple St Sept 10–12 overlaps Electric Oak Ave Sept 11"
- Adjusts one job's dates in plan mode, or messages Jesse before proposing
- **Success:** Conflicts visible before proposals go out

**Depends on:** FJ-1 plan mode, cross-project assignment visibility

---

## FJ-3: AI draft "what changed" for customer

**Cast:** Ryan  
**Trigger:** Cascade moves cabinets Oct 1 → Oct 5 (UJ-5 / UJ-9).

### Steps
- On save, system drafts plain-language message for Lauren
- Ryan reviews, edits, approves send
- Lauren gets MMS/email per H-14 — not a weekly AI digest (BT-style)

**SME check:** Draft always, or only on cascade / multi-task moves?

**PRD:** FR-19 (deferred v0.2+)

---

## FJ-4: Unified person portal

**Cast:** Jesse or Lauren  
**Trigger:** Same phone on 3 projects across 2 contractors.

### v0.1 (lean)
- Each magic link is project-scoped — no global list

### v0.2 (target)
- One phone verify → "Your projects" list with role per row
- Open confirmations across all GCs in one place

**Journey ref:** S-17, H-20, UJ-7

---

## FJ-5: Project template

**Cast:** Maci (typical) or Ryan  
**Trigger:** Another kitchen remodel — same phase sequence as Maple St.

### Steps
- **New project from template:** Kitchen remodel
- Pre-wired phases, default buffers, optional cascade deps
- Edit names/dates; enter customer; finalize or go live
- **Success:** C-1 onboarding in &lt;10 min for repeat job types

---

## FJ-6: Role-based permissions (post-POC)

**Cast:** Ryan (contractor-admin), Maci (contractor-office)

| Action | Admin (Ryan) | Office (Maci) |
|--------|--------------|---------------|
| Subscription / billing | ✅ | ❌ |
| Company calendar connect | ✅ | `[OPEN]` |
| Create project, invite subs/customers | ✅ | ✅ |
| Propose / cascade / reassign | ✅ | ✅ |
| Send poke / snooze | ✅ | ✅ |
| Archive project | ✅ | `[OPEN]` |

**v0.1:** Not implemented — Ryan and Maci identical. See [backlog.md](./backlog.md).

---

Log decisions in [discovery-log.md](../../../discovery-log.md).
