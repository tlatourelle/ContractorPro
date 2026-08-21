# Job Planning Workflow — Plan, Balance, Finalize

Status: **MVP (promoted 2026-08-20)** — was exploratory v0.2  
Related: [product-vision.md](../product-vision.md), [schedule-confirmation-workflow.md](./schedule-confirmation-workflow.md), [google-calendar-integration.md](./google-calendar-integration.md), [sprint-change-proposal-2026-08-20.md](../sprint-change-proposal-2026-08-20.md)

## Product intent (from stakeholder)

Before subs are notified or Google Calendar is touched, the GC needs a **planning workspace** for each sold job: define work types, durations, buffers between phases, pick a start date, preview the timeline, overlay against existing commitments, cross-check sub availability across jobs, balance multiple sold contracts, then **finalize** to enter the live scheduling workflow.

> “For each job I’ve sold — what types of work are needed? How many days does each take? How much buffer between types (maybe different per type)? Pick a start date and show me what that looks like on a calendar — **not on calendar yet**. Show me against my calendar and what I already have scheduled — **not on calendar yet**. Cross-check where the same subs are already scheduled. Let me plan multiple jobs and balance against other jobs. **Finalize** by actually scheduling.”

This is **internal what-if planning** with a deliberate **commit boundary** before [schedule-confirmation-workflow.md](./schedule-confirmation-workflow.md) (propose → accept → poke → Google sync).

---

## Core principle: Plan vs Schedule

Two modes, one product — separated by a **finalize** action.

| | **Plan** | **Schedule** (after finalize) |
|--|----------|-------------------------------|
| **Purpose** | What-if, sold-job setup, capacity balancing | Live coordination with subs |
| **Visible to subs?** | ❌ No | ✅ Yes (proposals, portal) |
| **Google Calendar writes?** | ❌ No | ✅ On sub accept only |
| **SMS / email pokes?** | ❌ No | ✅ Yes (see confirmation workflow) |
| **Date changes** | Free drag / recalc | Propose → re-confirm per assignment |
| **Project status** | `planning` | `active` |
| **UI styling** | Draft / dashed / “PLANNING” | Committed / solid |

```text
PLAN (draft, internal, reversible)
  phases → durations → buffers → start date → preview → overlays → conflicts
        ↓
   [ Publish prelim to customer ]     ← separate action (2B gate optional, off by default)
        ↓
   [ Finalize & start sub cascade ]   ← separate action; project → active
        ↓
SCHEDULE (commit — see schedule-confirmation-workflow.md)
  propose → SMS/email → accept/decline → poke → Google Calendar
```

**Rule:** Nobody external is bothered until the GC **finalizes**.

---

## Per-job planning model

Each **sold job** (project/contract) gets a planning workspace.

### Work types (phases)

Ordered list of work needed on the job — trades or logical phases:

| Phase | Duration | Buffer after | Planned sub |
|-------|----------|--------------|-------------|
| Demo | 2 days | 1 day | Carlos |
| Rough plumbing | 3 days | 2 days | Jose |
| Rough electric | 2 days | 1 day | Mike |
| Drywall | 4 days | 3 days | (TBD) |
| Prime / paint | 3 days | 0 days | Mike |

**Naming:** Call these **work phases** in the data model (or `tasks` with `planning_only` until finalize — see Data model).

### Duration

- **Duration** = working days (or calendar days — **open question**) the phase occupies
- Default unit: **days** for residential GC MVP (not hours)
- GC can override per phase on each job

### Buffer between types

- **Buffer after** = gap **after** this phase completes before the next may start
- **Per-phase** — paint may need 3 days after drywall; demo may need 0
- Implemented as lag on the dependency edge: `phase N` → `buffer` → `phase N+1`

Example computed chain (start Sept 2):

```text
Sept 2–3    Demo           (2d)
Sept 4      buffer         (1d)
Sept 5–7    Rough plumbing (3d)
Sept 8–9    buffer         (2d)
Sept 10–11  Rough electric (2d)
Sept 12     buffer         (1d)
Sept 13–16  Drywall        (4d)
Sept 17–19  buffer         (3d)
Sept 20–22  Prime / paint  (3d)
```

### Dependencies

**Default:** strict sequence — phase N+1 starts after phase N duration + buffer.

**Later:**

- Parallel phases (e.g. rough electric + rough plumbing overlap if different subs)
- Optional cascade within planning when one phase moves (preview only)

### Planned sub (optional in planning)

- Assign a **preferred sub** per phase before finalize (from company rolodex or prior project participants)
- Used for **conflict detection**, not notification
- Sub may be TBD until finalize

### Project templates (v0.2+)

Reuse common residential remodel sequences:

- “Kitchen remodel” → pre-filled phases, default durations/buffers
- GC edits per job
- See Ideas backlog in [discovery-log.md](../discovery-log.md)

---

## Planning views

### 1. Single-job planning timeline

**In-app only** — not Google Calendar.

| View | Use |
|------|-----|
| **Gantt / bar** | Phase sequence, durations, buffers (desktop-first) |
| **Month / week calendar** | “What does Sept look like for this job?” |

Visual distinction for planning:

- Dashed bars, muted color, `PLANNING` label
- Tooltip: “Not scheduled — subs not notified”

**Interactions:**

- Change **planning start date** → entire chain recalculates
- Drag phase bar → shift this phase (+ optional “shift all following”)
- Edit duration / buffer inline

### 2. Overlay — GC’s existing commitments

Show planning draft **against** what’s already real:

| Layer | Source | Meaning |
|-------|--------|---------|
| **Committed jobs** | ContractorPro `active` projects with confirmed assignments | Solid bars |
| **Other planning jobs** | ContractorPro `planning` projects | Dashed bars (other jobs) |
| **This job’s plan** | Current editing context | Highlighted dashed |
| **GC personal calendar** | Google Calendar free/busy (optional v0.3) | Gray blocks — inspections, estimates, PTO |

**Conflict examples:**

```text
Sept 10  [PLAN] Maple St — rough electric     ← this job (draft)
Sept 10  [LIVE] Oak Ave — final walkthrough    ← committed
         ⚠️ YOU: double-booked this day

Sept 11  [PLAN] Maple St — rough electric
Sept 11  [LIVE] Oak Ave — punch list (Mike)    ← same sub
         ⚠️ MIKE: assigned two places
```

### 3. Sub conflict check (cross-job)

For each **planned or preferred sub**, aggregate assignments across **all company projects**:

```text
Mike (Electric)
  ✅ Oak Ave     Sept 3–5    committed
  ⚠️ Maple St    Sept 10–11  planning — overlaps Oak punch-list Sept 10
  ⚠️ Pine Rd     Sept 12     planning — 1-day turnaround after Maple St
```

**Conflict types:**

| Type | Severity | Description |
|------|----------|-------------|
| **Sub double-book** | High | Same sub, overlapping dates, two jobs |
| **Sub tight turnaround** | Medium | Same sub, < N days between jobs (configurable) |
| **GC double-book** | High | GC marked required on site, two places |
| **Phase buffer violation** | Low | Manually dragged phase ignores buffer (warning) |

Panel: **“Subs at risk”** — filter all planning + active jobs.

### 4. Portfolio planner (multi-job)

Desk view — **all sold / planning / active jobs** on one timeline:

```text
              Sept              Oct
Maple St      ████████████░░░░
Oak Ave           ████████
Pine Rd               ██████████
              ↑ drag job start to balance load
```

**Goals:**

- Stagger job starts so crews aren’t idle
- Avoid booking the same sub on two jobs the same week
- See company capacity at a glance

**Interactions:**

- Drag **job start** on portfolio → recalc that job’s phases
- Click conflict badge → jump to detail
- Filter by sub, trade, or job status

---

## Finalize → actually schedule

When the plan is ready, GC commits.

### Finalize preview

```text
Finalize schedule for Maple St Kitchen?

This will PROPOSE dates to 4 subs (they have not been notified during planning):
  Jose  — Rough plumbing   Sept 5–7
  Mike  — Rough electric   Sept 10–11
  …

No Google Calendar events until subs accept.
Daily reminders will start if subs don't respond.

[ Cancel ]  [ Finalize & send proposals ]
```

### On finalize

1. `project.status`: `planning` → `active`
2. `project.finalized_at` = now
3. Work phases → **tasks** (or flip `planning_only` → false)
4. Create `task_assignments` with `status = proposed` and `proposed_start/end`
5. Trigger [schedule-confirmation-workflow.md](./schedule-confirmation-workflow.md):
   - Initial SMS/email per sub
   - `reminder_schedules` for poke engine
6. **No Google Calendar write** until sub accepts
7. Audit log: `plan_finalized` with snapshot of planned dates

### After finalize

- Further date changes follow **propose → re-confirm** rules (not free planning drag)
- Optional: **“Return to planning”** for major replan (rare; cancels pending proposals — **open question**)

### Re-planning an active job

| Scenario | Lean behavior |
|----------|---------------|
| Minor tweak | Edit task dates → proposed_change workflow |
| Major replan | GC explicitly “Re-open planning mode” → pauses pokes, marks assignments draft — v0.3 |

---

## Relationship to other features

| Feature | Relationship |
|---------|--------------|
| **Schedule confirmation** | Starts at finalize; planning never sends proposals |
| **Poke engine** | Starts at finalize; no reminders during planning |
| **Google Calendar** | Writes on sub accept only; planning is ContractorPro-only |
| **Cascade** | Post-finalize when committed dates move; planning may have local “shift following phases” preview only |
| **Messaging** | Subs not invited to planning; invite at or before finalize |
| **Homeowner** | Optional high-level milestone preview post-finalize only |

---

## Product phases

| Version | Planning scope |
|---------|----------------|
| **v0.1** | Simple project + task list with dates (minimal); coordination wedge ships without full planner |
| **v0.2** | **Job planning workflow** — phases, duration, buffer, planning calendar, finalize → schedule flow |
| **v0.2** | Sub conflict check across committed + planning jobs (ContractorPro data) |
| **v0.2** | Portfolio multi-job timeline |
| **v0.3** | Google free/busy overlay for GC personal calendar |
| **v0.3** | Project templates; parallel phases; AI suggested start date |
| **Later** | Crew capacity (headcount), material lead times, weather buffers |

**Positioning:** Not a full ERP or estimating tool — **plan the job, commit the schedule, coordinate subs**. Lighter than Buildertrend Gantt; tied to Google + magic-link subs on commit.

---

## Data model sketch

```text
projects
  id, company_id, name, address, ...
  status                    -- planning | active | completed | archived
  planning_start_date       -- anchor for phase calculation
  finalized_at              -- null until finalize
  sold_at                   -- optional; contract sold date

work_phases
  id, project_id
  name                      -- "Rough plumbing"
  trade_type                -- optional enum: plumbing | electric | ...
  sort_order
  duration_days
  buffer_after_days         -- gap before next phase may start
  depends_on_phase_id       -- null = follows previous in sort_order
  preferred_sub_id          -- FK → project_participants or company_contacts
  planned_start, planned_end -- computed; recalc on planning_start_date change
  task_id                   -- set on finalize → links to live task

-- Alternative: single `tasks` table with planning_mode flag until finalize
tasks
  ...
  is_planning_only          -- true until finalize
  duration_days, buffer_after_days

phase_templates / template_phases   -- v0.3
  company_id, name ("Kitchen remodel")
  default phases, durations, buffers

planning_conflicts   -- computed cache, optional
  id, company_id, conflict_type, severity
  sub_participant_id?, project_id_a, phase_id_a, project_id_b, phase_id_b
  detected_at
```

### Timeline calculation (pseudocode)

```
function recalcProject(project):
  start = project.planning_start_date
  for phase in project.phases.ordered():
    phase.planned_start = start
    phase.planned_end = addWorkingDays(start, phase.duration_days - 1)
    start = addDays(phase.planned_end, phase.buffer_after_days + 1)
```

Working days vs calendar days — **open question** (business days + GC blackout dates = v0.3).

---

## UI surfaces (GC desktop-first)

| Screen | Purpose |
|--------|---------|
| **Job planner** | Single job: phase list + mini Gantt + planning calendar |
| **Portfolio timeline** | All jobs; drag starts; conflict badges |
| **Sub availability** | Per-sub cross-job calendar |
| **Finalize modal** | Preview proposals before send |
| **Planning vs active badge** | Global status on project cards |

Subs and homeowners **do not** see planning mode.

---

## Conflict detection (technical)

Run on:

- Planning start date change
- Phase duration/buffer edit
- Phase drag
- Another job finalized (new committed dates)
- Portfolio load

```
ConflictService.check(companyId):
  committed = assignments where status = confirmed
  planning  = work_phases where project.status = planning
  for each sub with assignments in committed ∪ planning:
    detect date overlaps
    detect turnaround < threshold
  for each gc_required_day:
    detect overlaps
  return conflicts[] with severity
```

Display inline on calendar + aggregate panel. **Warnings, not hard blocks** — GC can override with acknowledgment.

---

## MVP checklist (v0.2 planning module)

- [ ] `projects.status` = `planning` | `active`
- [ ] `work_phases` with duration, buffer_after, sort order
- [ ] `planning_start_date` → auto-compute `planned_start/end`
- [ ] Single-job planning Gantt + calendar (in-app, dashed style)
- [ ] Portfolio view — multiple `planning` + `active` jobs
- [ ] Sub cross-job conflict detection (ContractorPro data)
- [ ] GC conflict overlay (committed jobs)
- [ ] Finalize preview + handoff to `task_assignments` (proposed)
- [ ] Clear UX: “Not scheduled yet — subs not notified”

### Later

- [ ] Project templates
- [ ] Google free/busy overlay
- [ ] Parallel phases
- [ ] Business-day / holiday calendar
- [ ] “Re-open planning” on active job
- [ ] AI: suggest start date given sub conflicts

---

## Open questions

### Product

- [ ] Duration unit: **calendar days** vs **working days** for v0.2?
- [ ] Can phases overlap (parallel trades) in v0.2 or strict sequence only?
- [ ] Finalize: invite subs who aren’t on project yet, or require invites first?
- [ ] Partial finalize — commit first 3 phases, plan the rest?
- [ ] Homeowner sees anything at planning stage? (Lean: no)

### Technical

- [ ] `work_phases` vs unified `tasks` table with planning flag?
- [ ] Recompute conflicts synchronously vs background job?
- [ ] Snapshot plan at finalize for audit / dispute?

---

## Discovery questions

1. When you sell a job, do you already have a **standard phase list** (kitchen vs bath vs addition)?
2. Do you plan **multiple sold jobs** on one whiteboard/spreadsheet today?
3. How often do you **double-book a sub** across jobs — pain level 1–10?
4. Would you want planning in **v0.1** or is coordination-first enough to start?

See [customer-discovery.md](../customer-discovery.md).

---

## Relation to wedge

| Buildertrend | ContractorPro |
|--------------|---------------|
| Heavy in-app Gantt, everything live | **Light planning** → explicit **finalize** → lightweight sub coordination |
| Sub sees schedule early | Sub sees nothing until finalize + proposal |
| Calendar is in-app | Google Calendar on **accept** after commit |
| Complex setup | Sold job → phases → dates → balance → **one button to schedule** |

Planning is **how the GC thinks**. Scheduling is **how the crew commits**.

Log decisions in [discovery-log.md](../discovery-log.md).
