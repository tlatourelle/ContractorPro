# User Journeys — Backlog & Discovery

Journeys drafted but **not ready to build** without more discovery, SME validation, or explicit v0.2 scope lock.

Related: [README.md](./README.md) · [discovery-log.md](../../../discovery-log.md)

---

## Team roles (post-POC — not v0.1)

| Future role | Intent | v0.1 today |
|-------------|--------|------------|
| **Contractor-admin** (Ryan) | Subscription, billing, company settings, destructive actions | Ryan and Maci share all capabilities |
| **Contractor-office** (Maci) | Projects, customers, subs, scheduling, messaging | Same login powers as Ryan |

**v0.1 rule:** Maci journeys ([contractor-journeys.md](./contractor-journeys.md) § Maci) describe **typical task focus**, not permission differences.

---

## Needs discovery / SME before v0.1 build

| ID | Topic | Journey ref | Open question |
|----|-------|-------------|---------------|
| **BL-1** | Customer channel gating | H-23, UJ-3b | Block schedule MMS until `phone_confirmed`? Block email digests until `email_confirmed`? |
| **BL-2** | Customer milestone filter | H-22 | Which tasks are customer-visible vs internal-only? Who marks them? |
| **BL-3** | Family dual-channel confirm | H-7, H-8 | Same email+MMS+poke as primary, or MMS-only for Erin? |
| **BL-4** | Returning customer fast path | H-24, H-5 | Skip dual channel confirm if both channels verified on prior project? |
| **BL-5** | Batch cascade confirm (subs) | S-18, C-12 | One SMS with all task moves vs one per assignment? |
| **BL-6** | Batch cascade confirm (customer) | H-15 | One digest vs per-milestone MMS? |
| **BL-7** | Quiet hours | S-19 | Company default vs per-participant? Timezone source? |
| **BL-8** | `notify_via` per sub | S-16 | Set at invite, editable by sub, or GC-only? |
| **BL-9** | Poke Ryan on sub-request | C-23 | Auto-poke GC if sub reschedule request pending 48h? |
| **BL-10** | Draft schedule mode | C-21 | Per-project toggle or per-edit? How long can draft sit? |
| **BL-11** | Partial cascade | C-20 | Move only selected dependents? Fixed duration vs end date? |
| **BL-12** | Business days / holidays | C-20 | Calendar days vs business days for cascade delta? GC blackout dates? |
| **BL-13** | MMS ingest before handle ready | C-13 | Queue messages, warn Ryan, or drop? |
| **BL-14** | Unified inbox (MMS + app) | C-14, H-17 | Single thread view or mirror-only? |
| **BL-15** | Project photo timeline | C-26 | Chronological all-project photos across threads — MVP or v0.1.1? |
| **BL-16** | Billing / trial limits | C-27 | Free tier caps; upgrade flow when limit hit |
| **BL-17** | Sub "my jobs" landing | S-17 | v0.1 per-link only; when does unified portal ship? |
| **BL-18** | Courtesy SMS on reassignment | S-20, UJ-2e | Auto-send when Nate replaces Jesse? |

---

## v0.2+ future journeys (drafted — not in v0.1 scope)

Documented in [future-journeys-v02.md](./future-journeys-v02.md):

| ID | Journey | Persona |
|----|---------|---------|
| **FJ-1** | Plan mode → Finalize schedule | Ryan / Maci |
| **FJ-2** | Portfolio balance / sub conflict across jobs | Ryan |
| **FJ-3** | AI draft "what changed" on schedule shift | Ryan |
| **FJ-4** | Unified person portal (all projects, one phone) | Jesse / Lauren |
| **FJ-5** | Project template (kitchen remodel phases) | Ryan / Maci |
| **FJ-6** | Role-based permissions (admin vs office) | Ryan / Maci |

---

## Competitive / wedge journeys (documented in v0.1 files)

These are **intentionally in scope** for differentiation — see contractor C-19–C-25, customer H-21–H-24, UJ-9:

- Portfolio triage home screen (vs BT dashboard sprawl)
- Event-triggered customer updates (vs BT weekly AI digest)
- Persistence / poke layer (vs passive iCal)
- Google + Apple calendar on accept (vs BT one-way iCal)

Log decisions in [discovery-log.md](../../../discovery-log.md).
