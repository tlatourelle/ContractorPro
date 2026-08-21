# SME Meeting 01 — Cross-Reference vs Current Plan

> **Read-only analysis.** Maps lunch discussion to existing artifacts. Flags alignment, gaps, and **potential direction changes** — nothing here is adopted until we decide.

**Primary plan references:** [product-vision.md](../../planning-artifacts/product-vision.md) · [prd.md](../../planning-artifacts/prds/prd-ContractorPro-2026-08-15/prd.md) · [project-handle-numbers.md](../../planning-artifacts/technical-exploration/project-handle-numbers.md) · [job-planning-workflow.md](../../planning-artifacts/technical-exploration/job-planning-workflow.md) · [epics-and-stories.md](../../planning-artifacts/prds/prd-ContractorPro-2026-08-15/epics-and-stories.md)

---

## Executive summary

The meeting **validates the wedge** (cheap, lean, calendar-integrated scheduling — not ERP) and **pricing tiers** already in epics. It also surfaces **one major architectural tension** (per-project phone numbers vs. one company number) and **two workflow gaps** (sequential sub-invite cascade, plan-first as day-one UX). QR job-site tooling and customer milestone comms are **largely new** to the written plan.

**Do not merge yet.** The phone-number model alone ripples through messaging architecture, cost model, and MVP scope.

---

## Topic matrix

| # | SME idea | Current plan | Verdict | Notes |
|---|----------|--------------|---------|-------|
| 1 | **Per-project Twilio number** for comm logging | **Locked:** one handle # per project ([project-handle-numbers.md](../../planning-artifacts/technical-exploration/project-handle-numbers.md), PRD FR-14) | ⚠️ **Conflict** | Tom: app must monitor threads. Ryan: subs won't stay in per-project groups; will call personal numbers. Meeting ended exploring **one number per contractor**. |
| 2 | **Central company number** → Ryan + Maci | Not designed; plan assumes Dana's personal phone + project handle in group MMS | ⚠️ **New / conflict** | Ryan wants ReviewWave-style shared inbox. Open: routing without duplicate replies ([Pocket todo #3](./README.md)). |
| 3 | **Lose comm tracking** if central number wins | Messaging ingest is core MVP (group MMS mirror, FR-14) | ⚠️ **Scope tradeoff** | Ryan: "four things" enough without archived texts. Tom: logging is differentiator. Needs explicit product decision. |
| 4 | **Approval cascade** (sub₁ yes → invite sub₂) | Cascade exists for **schedule shifts** (FR-13), not sequential **initial invites** | 🆕 **Gap** | Ryan/Macie Buildertrend pain — "send all or nothing." Not in epics today. |
| 5 | **Plan-first** (templates, durations, what-if, then calendar) | **v0.2** job planning module; v0.1 = project + tasks + dates | ⚠️ **Priority shift?** | SMEs describe plan-first as how Ryan works **today** (reverse-schedule from Nick's date). Aligns with [job-planning-workflow.md](../../planning-artifacts/technical-exploration/job-planning-workflow.md) but that doc is v0.2. |
| 6 | **Reschedule via planner**, not calendar drag | v0.1: propose/re-confirm on committed assignments; cascade preview | ✅ **Mostly aligned** | SME language matches cascade + replan intent. v0.1 may lack full planner UI. |
| 7 | **Sub calendar sync** (Google, no app) | Core differentiator — PRD UJ-1, UJ-2, product vision | ✅ **Aligned** | Ryan: "secret to subs" — same as plan. |
| 8 | **Daily poke until sub responds** | FR-11 poke reminders | ✅ **Aligned** | Explicitly endorsed in transcript. |
| 9 | **Customer read-only calendar** | PRD UJ-4, customer portal | ✅ **Aligned** | Maci already beta-testing manually via extra Google account. |
| 10 | **Customer milestone SMS** ("demo day tomorrow" + prep) | Partial — change notifications exist; **milestone prep bundles** not specified | 🆕 **Expand** | Ryan wants scheduled customer comms tied to phases, not just date-change alerts. |
| 11 | **Customer approves schedule before subs** | PRD: customer **acknowledge**, not hard approve on task dates; constraints at signing | ⚠️ **Tension** | Maci: customer-first cascade. Ryan: upfront contract conversation, not per-trade approval. PRD assumption may need SME pick. |
| 12 | **Login-free magic link + OTP** | Core identity model (PRD §3, invite-join-flow) | ✅ **Aligned** | Ryan cited Buildertrend login friction. |
| 13 | **QR → Google Drive** (resources + photo check-out) | **Not in v0.1 PRD** | 🆕 **New** | Ryan prefers Drive bucket over app-hosted files. Tom open to software generating QR + doc list. Company Cam explicitly rejected. |
| 14 | **QR check-in / check-out** | Not in plan | 🆕 **New** | Ryan: company policy + laminated QR, not software-enforced attendance. Lower priority than scheduling core? |
| 15 | **Scope: scheduling only** (no estimates/financials) | Explicit out-of-scope in PRD §6 | ✅ **Aligned** | Strong SME reinforcement. |
| 16 | **Pricing $100 / 5 jobs, $200 / 10** | E1-S3 epics (Stripe Phase 2) | ✅ **Aligned** | Ryan OK with tiering if closed jobs don't count toward cap. |
| 17 | **~$10/mo telco cost per active project** | project-handle-numbers cost model | ✅ **Aligned** | Validates unit economics; central-number model changes this. |
| 18 | **Google-only MVP** | Locked in planning checklist / PRD | ✅ **Aligned** | Apple called out as v0.1.1. |
| 19 | **Email + text notifications (opt-in)** | PRD supports SMS/email channels | ✅ **Mostly aligned** | SMEs want per-person preference. |
| 20 | **AI auto-reschedule** | Deferred / v0.2+ in vision | ✅ **Aligned defer** | Tom: only after revenue. |
| 21 | **Twilio number portability on churn** | Churn → release numbers ([project-handle-numbers.md](../../planning-artifacts/technical-exploration/project-handle-numbers.md)) | ❓ **Open** | Pocket todo — Ryan worried about losing numbers. Plan says release on unsubscribe; portability TBD. |
| 22 | **Work order screenshots for Tom** | Mockups exist; real Ryan artifacts needed | 📋 **Action** | Needed for realistic task/scope UI. |

**Legend:** ✅ aligned · ⚠️ tension or priority shift · 🆕 not in current plan · ❓ needs research · 📋 action item

---

## The big direction change (parked)

### Phone number model

```text
CURRENT PLAN                          SME MEETING DIRECTION
────────────────                      ─────────────────────
1 number per PROJECT                  1 number per CONTRACTOR (company)
+ GC personal phone in group MMS      Central number = customer/sub-facing
Full MMS ingest & mirror              Approval links only; dialogue may stay outside app
~$10/project/month telco              Lower per-project overhead; different routing problem
```

**Why it matters:** FR-14, architecture messaging workers, epics E6/E7, and [messaging-and-media.md](../../planning-artifacts/technical-exploration/messaging-and-media.md) all assume per-project handles. Changing this is not a tweak — it's a **correct-course** event.

**Middle ground from transcript:** Central number for outbound approvals + optional logging; subs who text Ryan directly get manually associated (Ryan: "in regards to…" prefix). Tom to analyze multi-recipient routing.

---

## Ryan's "four things" vs MVP scope

| Ryan's four | Maps to |
|-------------|---------|
| Sub calendar integration | E4/E5 calendar sync — **in MVP** |
| Customer calendar view | Customer portal + calendar feed — **in MVP** |
| Easy calendar building | Task/dependency UI — **partial MVP**; full planner **v0.2** |
| Sub confirmation tracking | Propose/accept/poke — **in MVP** |

**Implication:** Ryan doesn't require comm archiving for v1. That's a deliberate scope cut if we follow his prioritization.

---

## Ideas worth a dedicated follow-up session

1. **Phone number architecture** — per-project vs per-company vs hybrid (central + project tag in message body)
2. **Sequential invite cascade** — customer-first? sub-chain? both configurable?
3. **Plan-first MVP cut** — what's the minimum planner Ryan needs before first sub text goes out?
4. **QR / job-site bucket** — in scope v0.1, v0.2, or Ryan's manual Drive forever?
5. **Customer comms** — milestone reminders + prep docs: product feature or Ryan's existing email templates?

---

## Suggested next steps (PM — not executed)

1. Schedule SME meeting #2 focused **only** on phone number model — bring 2–3 options with pros/cons/cost
2. Ryan sends work order screenshots (Pocket todo #1)
3. Tom researches Twilio portability (Pocket todo #2)
4. After decisions: run `bmad-correct-course` if phone model changes, then PRD addendum — **not before**

---

*Generated from SME meeting 01 analysis pass. Last updated: 2026-08-20.*
