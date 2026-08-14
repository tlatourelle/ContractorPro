# ContractorPro — Product Vision (Draft)

> Status: **Exploratory** — reflects conversation through 2026-08-13. Expect changes.

## One-liner

**ContractorPro helps small residential GCs keep subs and homeowners aligned when the schedule moves — without replacing Google Calendar or paying enterprise prices.**

## Problem

- All-in-one construction platforms ([BuilderTrend](https://buildertrend.com/), [Contractor Foreman](https://get.contractorforeman.com/), [BuildPass](https://www.buildpass.ai/)) are **prohibitively expensive** for smaller contractors
- They **force their own tools** (calendars, workflows) instead of integrating with what people already use
- Homeowners are often an afterthought; subs get uneven portal experiences
- When a schedule slips, **phone tag** follows — subs, homeowners, and the GC are out of sync

## Wedge

**Cheaper, friendlier, simpler** than the big three — **and AI-forward** where it reduces admin (draft updates, summarize threads, explain cascade impact), not where it adds complexity.

| Principle | Meaning |
|-----------|---------|
| **Integrate, don't replace** | Google Calendar is a **first-class UX** — BYO existing calendars **or** Pro-provided per-entity calendars; dual-view in Google + app |
| **Contractor is the hub** | GC orchestrates; subs and homeowners don't talk past the GC |
| **Lightweight portals** | Web link + SMS for subs/homeowners — no app install |
| **Start small** | Ship cascade + messaging + calendar sync before estimating, invoicing, safety modules |
| **Grow by tier** | Free tier + low flat monthly; GC company pays; invitees free |

## Target users

### Primary (pays)

**Small general contractor** — roughly 2–15 employees, handful of active **residential** jobs.

### Secondary (invited, free)

| Role | Access | Visibility |
|------|--------|------------|
| **Subcontractor** | Magic web link + SMS | Assigned tasks, sub-relevant info, **private** GC↔sub messaging |
| **Homeowner** | Magic web link + SMS | Schedule, what-changed feed, **upload/view photos** in GC↔homeowner thread |

Homeowners never see sub-only threads, sub pricing, or internal GC notes.

## Signature feature (primary differentiator)

**Optional schedule cascade** — when a task moves, dependent downstream tasks shift by the same delta (per-project toggle). Notify affected subs and homeowner. AI can draft homeowner-friendly “what changed” copy.

## MVP boundary (v0.1 — draft)

### In scope

- Residential projects with linked task timeline
- Optional cascade on schedule changes
- Messaging with **image uploads from GC, subs, and homeowners** — Azure Blob + SQL; central to project tracking
- Messaging: GC↔sub (private), GC↔homeowner (private)
- Magic-link invites for subs and homeowners
- SMS notifications (link back to web; tier limits TBD)
- Subscription billing for GC (Chargebee or Stripe Billing)
- AI-assisted comms (draft updates, thread summary — stretch in v0.1)

### Out of scope (explicitly later)

- Commercial workflows, service tickets
- Native mobile apps (responsive web first)
- Estimating, selections, time cards, safety/compliance modules
- Microsoft Calendar, multi-currency, multi-language
- Deep QBO sync (explore after MVP)
- Homeowner/sub payments

## Market scope

| Dimension | Now | Later |
|-----------|-----|-------|
| Vertical | Residential | Commercial, service work |
| Geography | US only, English, USD | TBD |
| Accounting | None in MVP; explore QBO | Quicken only if users demand it |

## Monetization (draft)

- Flat monthly tiers + **free tier** (limited projects/features)
- **Chargebee or Stripe Billing** for ContractorPro subscriptions
- SMS volume caps per tier
- GC company billed; subs and homeowners never pay

## AI direction

Use AI for **communication and coordination**, not feature bloat:

- Draft homeowner update when schedule cascades
- Summarize message threads for busy GCs
- Preview cascade impact (“4 tasks, 2 subs affected”)
- Smart routing of notifications (SMS vs in-app)

Not MVP: AI estimating, takeoff, document extraction.

## Integrations roadmap (explore, not build yet)

| System | Purpose | Priority |
|--------|---------|----------|
| Google Calendar | Dual-view + app-managed shared calendars; cascade → event updates | v0.1 — see [google-calendar-integration.md](./technical-exploration/google-calendar-integration.md) |
| Twilio (or similar) | SMS to subs/homeowners | v0.1 |
| Chargebee / Stripe | SaaS subscription for GCs | v0.1 |
| QuickBooks Online | Customer/invoice handoff, light sync | Post-MVP explore |
| Quicken | Only if discovery shows demand | Low / validate first |
| Microsoft 365 Calendar | Work-context only; defer — most subs/homeowners on Google | Post-MVP if needed |

## Success metrics (to refine)

- GC publishes cascade update in under 2 minutes
- Homeowner or sub opens notification link within 24 hours
- Reduction in “when are you coming?” messages (qualitative in discovery)
- GC completes onboarding (connect calendar, create project, invite 1 sub + 1 homeowner) in one session
