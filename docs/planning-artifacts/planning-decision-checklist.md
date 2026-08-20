# Planning Decision Checklist — Walkthrough

**Purpose:** Close gaps before M1 build. Work top-to-bottom; mark each row when decided.  
**Session goal today:** (1) decide → (2) sync docs → (3) add Epic E12 + missing stories  
**Owners:** Thomas decides · John (product) · Winston (architecture)

**Status key:** `[ ]` open · `[x]` decided · `[~]` default OK unless Thomas overrides

---

## How to use this doc

1. Walk **Section A** first — blockers and scope locks.
2. **Section B** — backlog BL-* tags (batch by bucket).
3. **Section C** — minor defaults (fast “yes/no” or “accept default”).
4. **Section D** — doc sync checklist (run after A–C).
5. **Section E** — epic/story additions (Epic E12 + gaps).

Record decisions in [discovery-log.md](./discovery-log.md) and strike/update open questions there.

---

## A. Major decisions (decide first)

| ID | Question | Options | Recommendation | Your call | Updates |
|----|----------|---------|----------------|-----------|---------|
| **A-1** | **Cascade (E7) in MVP Phase 1?** | Yes full · Preview only · Phase 1.1 | **Yes** — wedge demo needs slip + re-confirm | `[x]` **Yes — MVP** · 2026-08-20 | PRD §7.1, epics E7, arch §9 |
| **A-2** | **Sub/customer Apple Calendar in MVP?** | Google attendee invites only · Apple CalDAV v0.1 · Portal-only no calendar | **Google attendee invites only**; defer Apple → v0.1.1 | `[x]` **Google invites only** · 2026-08-20 | PRD FR-3, journeys S-5a/H-*, addendum, arch §1.6 |
| **A-3** | **GC team member auth providers in MVP?** | Google only · Google + Apple · Google + Apple + Microsoft | **Google only** for M1 | `[x]` **Google only** · 2026-08-20 | E1-S1, PRD FR-1, arch §4 |
| **A-4** | **Admin console in MVP product UI?** | None (ops via Twilio/DB) · Thin `/admin` read-only · Full A-1 slice | **None for M1 UI**; STOP logic in API before prod SMS | `[x]` **No admin UI M1** · 2026-08-20 | E12 scope, admin-journeys build priority |
| **A-5** | **STOP / opt-out (BL-21) before first prod SMS?** | API + Twilio sync required · Manual only at launch | **Required in API** before PL-1–3 green | `[x]` **API required pre-prod** · 2026-08-20 | New epic story, arch §4.3 persons opt-out |
| **A-6** | **Cooling default after project archive?** | 90d platform default · 180d · Fixed 90 non-configurable | **90d default**, per-contractor override (Winston model) | `[x]` **90d + override** · 2026-08-20 | Confirm in discovery (supersedes Aug 19 180d discussion) |
| **A-7** | **Number JIT in MVP?** | Always buy fresh · Pull from pool if available | **Always buy fresh** until E8-S5 (locked in arch) | `[x]` **Always fresh** · 2026-08-20 | E8-S4 — confirm only |
| **A-8** | **Phase 2 billing: 6th active project on Pro 5?** | Plan-only 6th · Hard block · Auto-upgrade prompt | **Plan-only 6th** | `[x]` **Plan-only 6th** · 2026-08-20 | FR-18, E1-S5 |
| **A-9** | **Native auth (E1-S2) in MVP?** | Defer · Ship passkey fallback | **Defer** — OAuth Google only M1 | `[x]` **Defer v0.1.1** · 2026-08-20 | Epics open Q #3 |

---

## B. Backlog BL-* — assign bucket

For each: **MVP** (build now) · **v0.1.1** · **v0.2+** · **Won’t do**

| ID | Topic | Suggested bucket | Your call |
|----|-------|------------------|-----------|
| **BL-1** | Customer channel gating (H-23) | MVP — simple rules | `[x]` **MVP** · 2026-08-20 |
| **BL-2** | Customer milestone filter (H-22) | v0.1.1 | `[x]` **v0.1.1** · 2026-08-20 |
| **BL-3** | Family dual-channel confirm (Erin) | MVP — MMS-only for family invite | `[x]` **MVP** · 2026-08-20 |
| **BL-4** | Returning customer fast path | v0.1.1 | `[x]` **v0.1.1** · 2026-08-20 |
| **BL-5** | Batch cascade confirm (subs) | MVP — one SMS batch | `[x]` **MVP** · 2026-08-20 |
| **BL-6** | Batch cascade confirm (customer) | MVP — one digest | `[x]` **MVP** · 2026-08-20 |
| **BL-7** | Quiet hours | MVP — company default 8pm–8am | `[x]` **MVP** · 2026-08-20 |
| **BL-8** | `notify_via` per sub | MVP — set at invite, GC editable | `[x]` **MVP** · 2026-08-20 |
| **BL-9** | Poke Ryan on sub-request 48h | v0.1.1 | `[x]` **v0.1.1** · 2026-08-20 |
| **BL-10** | Draft schedule mode (C-21) | v0.2 — FJ-1 | `[x]` **v0.2+** · 2026-08-20 |
| **BL-11** | Partial cascade | v0.1.1 — full cascade MVP only | `[x]` **v0.1.1** · 2026-08-20 |
| **BL-12** | Business days / holidays | v0.1.1 — calendar days MVP | `[x]` **v0.1.1** · 2026-08-20 |
| **BL-13** | MMS before handle ready | MVP — queue + warn Ryan | `[x]` **MVP** · 2026-08-20 |
| **BL-14** | Unified inbox vs mirror | MVP — mirror-only | `[x]` **MVP** · 2026-08-20 |
| **BL-15** | Project photo timeline | v0.1.1 | `[x]` **v0.1.1** · 2026-08-20 |
| **BL-17** | Sub “my jobs” portal | v0.2 — FJ-4 | `[x]` **v0.2+** · 2026-08-20 |
| **BL-18** | Courtesy SMS on reassignment | MVP — yes | `[x]` **MVP** · 2026-08-20 |
| **BL-19** | Admin role split (Alex) | When hired | `[x]` **When hired** · 2026-08-20 |

*Already decided (no action): BL-16 billing, BL-20–22 admin, BL-23 number reuse → v0.1.1*

---

## C. Minor decisions (defaults — override if needed)

| ID | Question | Default | Your call |
|----|----------|---------|-----------|
| **C-1** | On decline (E5-S3): hard `declined` vs revert to last confirmed? | **Hard declined** + reassign (E5-S3b) | `[x]` **Hard decline** · 2026-08-20 | E5-S3, E5-S3b |
| **C-2** | Magic link TTL (portal)? | **7 days** active; regenerate on resend | `[x]` **7 days** · 2026-08-20 | invite-join-flow, arch §4 |
| **C-3** | Handle # contact label in SMS/group setup? | **`{Project} · {Company}`** | `[x]` · 2026-08-20 | E8-S1, messaging-and-media |
| **C-4** | Portfolio calendar in MVP UI? | **Yes** — unified view per arch §1.6 | `[x]` **Yes** · 2026-08-20 | E3-S3, arch §1.6 |
| **C-5** | Background jobs MVP? | **In-process `IHostedService`**; Azure Queue v0.1.1 | `[x]` **IHostedService** · 2026-08-20 | arch §1.1 |
| **C-6** | OpenAPI client for React? | **Hand-typed fetch v1**; codegen v0.1.1 | `[x]` **Hand-typed** · 2026-08-20 | arch §1.4 |
| **C-7** | Twilio vs Telnyx for MVP? | **Twilio**; SP-2 spike before prod scale | `[x]` **Twilio** · 2026-08-20 | E8, SP-2 |
| **C-8** | Apple Sign-In ($99/yr)? | **Defer** v0.1.1 | `[x]` **Defer** · 2026-08-20 | E1-S1, A-3 |
| **C-9** | Annual pricing at Phase 2? | **~2 months free** on annual — tune later | `[x]` **~2 mo free** · 2026-08-20 | FR-18, monthly-run-rate |

---

## D. Doc sync checklist (after A–C)

Run once decisions are recorded. Check when complete.

| # | Task | Files |
|---|------|-------|
| D-1 | Refresh planning hub — current phase, links, MVP scope, story count | [README.md](./README.md) | `[x]` 2026-08-20 |
| D-2 | Update “Resume here” + strike resolved open questions | [discovery-log.md](./discovery-log.md) | `[x]` 2026-08-20 |
| D-3 | Sync PRD §7, §9, FR-1, FR-3, integrations with architecture | [prd.md](./prds/prd-ContractorPro-2026-08-15/prd.md) | `[x]` 2026-08-20 |
| D-4 | Close epics open questions; tag E7, E3-S2; add E12 | [epics-and-stories.md](./prds/prd-ContractorPro-2026-08-15/epics-and-stories.md) | `[x]` 2026-08-20 |
| D-5 | Align journey calendar/auth wording with A-2, A-3 | contractor/sub/customer journeys, [user-journeys.md](./prds/prd-ContractorPro-2026-08-15/user-journeys.md) | `[x]` 2026-08-20 |
| D-6 | Re-tag BL-* table with MVP / v0.1.1 / v0.2 | [backlog.md](./prds/prd-ContractorPro-2026-08-15/user-journeys/backlog.md) | `[x]` 2026-08-20 |
| D-7 | Architecture §1.2 forks + §9 open Qs + changelog | [architecture-v0.1.md](./architecture-v0.1.md) | `[x]` 2026-08-20 |
| D-8 | Banner on superseded exploration docs (optional) | stack, auth-and-data, google-calendar-integration | `[x]` 2026-08-20 |

---

## E. Epic / story additions (after decisions)

| # | Story | Phase | Triggered by |
|---|-------|-------|--------------|
| E-1 | **E12 — Platform admin (minimal)** | MVP API only / Phase 2 UI | A-4, A-5 | `[x]` 2026-08-20 |
| E-2 | **E6-S5 — STOP / opt-out handling** | Pre-prod SMS | A-5 | `[x]` 2026-08-20 |
| E-3 | **E3-S3 — Portfolio calendar view** (if C-4 yes) | MVP | C-4 | `[x]` 2026-08-20 |
| E-4 | **E12-S4 — Pre-launch PL-1–8 tracking** | Pre-beta | arch §10 | `[x]` 2026-08-20 |
| E-5 | Update **E7** priority tag from A-1 | MVP or defer | A-1 | `[x]` 2026-08-20 |

---

## Session log (fill as you go)

| Time | ID | Decision | Notes |
|------|-----|----------|-------|
| 2026-08-20 | A-1–A-9 | Thomas accepted all Section A recommendations | See discovery-log checklist session |
| 2026-08-20 | B BL-1–19 | Thomas approved all Section B bucket tags | MVP×9 · v0.1.1×6 · v0.2+×2 · when hired×1 |
| 2026-08-20 | C-1–C-9 | Thomas accepted all Section C defaults | Hard decline · 7d TTL · handle label · portfolio cal · IHostedService · hand fetch · Twilio · defer Apple · ~2mo annual |
| 2026-08-20 | D-1–D-8 | Doc sync complete | README · discovery-log · PRD · epics · journeys · arch · exploration banners |
| 2026-08-20 | E-1–E-5 | Epic additions complete | E12 · E6-S5 · E3-S3 · E7 MVP tag |

---

## Quick stats

| Section | Items | Est. time |
|---------|-------|-----------|
| A Major | 9 | ~30 min |
| B BL buckets | 16 | ~20 min (batch) |
| C Minor | 9 | ~15 min |
| D Doc sync | 8 | ~45 min (agent-assisted) |
| E Epics | 5 | ~30 min |

**Total:** ~2–2.5 hours for full #4 today.
