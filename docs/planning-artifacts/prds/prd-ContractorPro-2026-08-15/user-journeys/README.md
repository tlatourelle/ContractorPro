# User Journeys — SME Review Pack

**Status:** Draft  
**Purpose:** Persona-split journey lists for workshop review with SMEs. Bulleted and scannable — not implementation specs.

| File | Persona | Journeys |
|------|---------|----------|
| [contractor-journeys.md](./contractor-journeys.md) | Ryan, Maci (same access v0.1) | C-1 … C-27, M-1 … M-5 |
| [subcontractor-journeys.md](./subcontractor-journeys.md) | Jesse, Marcus | S-1 … S-22 |
| [customer-journeys.md](./customer-journeys.md) | Lauren, Erin | H-1 … H-24 |
| [backlog.md](./backlog.md) | — | Discovery + BL-* items |
| [future-journeys-v02.md](./future-journeys-v02.md) | — | FJ-1 … FJ-6 (v0.2+) |

### Example cast

| Name | Role |
|------|------|
| **Ryan** | Contractor / owner — Riverside Remodeling |
| **Maci** | Office manager (same company, **same product access as Ryan in v0.1**) |
| **Jesse** | Sub — painter |
| **Marcus** | Sub — flooring |
| **Nate** | Sub — replacement painter |
| **Lauren** | Primary customer — Maple St Kitchen |
| **Erin** | Additional customer — Lauren's spouse |

**Full detail** (step tables, system behavior, cross-references to epics): [../user-journeys.md](../user-journeys.md)

**Cross-persona walkthrough:** [UJ-9](../user-journeys.md#uj-9-end-to-end-schedule-slip-cross-persona) — framing slip end-to-end.

**Shared rule (subs + customers):** Accept/Reject always recorded in ContractorPro; linked personal calendar updated when calendar is linked.

**Calendar providers (v0.1):** Google Calendar + Apple Calendar (iCal/iCloud). **Google preferred** internally.

**Customer onboarding:** Primary customer at **project creation** — **email + MMS** sent; **both channels must confirm**; poke until both ✅. Family invite is secondary (H-7).

**Maci vs Ryan:** Identical permissions in v0.1. Maci journeys (M-1–M-5) = typical office focus. Post-POC role split → [backlog.md](./backlog.md).

## Recommended SME sessions

| Session | Audience | Files | Suggested order |
|---------|----------|-------|-----------------|
| 1 | Subcontractors | `subcontractor-journeys.md` | S-1 → S-3 → S-5 → S-18 |
| 2 | GC / office manager | `contractor-journeys.md` | C-3 → C-6 → C-19 → C-20 → **UJ-9** |
| 3 | Homeowners | `customer-journeys.md` | H-1 → H-4 → H-21 → H-14 |
| 4 | Maci workflow | `contractor-journeys.md` § Maci | M-1 → M-2 → M-5 |

**Critical path:** Start with **sub invite + confirm** (S-1, C-3) — nothing else works if subs won't engage.

**Needs discovery before build:** [backlog.md](./backlog.md)

Log decisions in [discovery-log.md](../../../discovery-log.md).
