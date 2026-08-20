# ContractorPro — Planning Hub

Living documentation for discovery, exploration, and product planning. **No application code yet** — planning complete; **M1 build is next**.

---

## ⏭️ When you return — do this first

> **Next step: Start M1 build — Google OAuth signup (E1-S1)**
>
> Implement Entra External ID Google sign-in, BFF session cookie, and auto-provision Contractor company on first login.
>
> **Prompt to use:** *"Implement M1 — auth + auto-provision Contractor on first Google OAuth."*
>
> **Read first:** [architecture-v0.1.md](./architecture-v0.1.md) (TRD) · [epics-and-stories.md](./prds/prd-ContractorPro-2026-08-15/epics-and-stories.md) (M1–M21) · [discovery-log.md](./discovery-log.md) · [planning-decision-checklist.md](./planning-decision-checklist.md) (Sections A–E ✅ 2026-08-20)

---

## How to use this folder

| Document | Purpose |
|----------|---------|
| [product-vision.md](./product-vision.md) | North star: who we serve, wedge, client UI strategy, MVP boundary |
| [discovery-log.md](./discovery-log.md) | **Main working doc** — ideas, open questions, decisions, session notes |
| [architecture-v0.1.md](./architecture-v0.1.md) | **TRD / architecture v0.1** — stack, schema, integrations, billing hooks |
| [planning-decision-checklist.md](./planning-decision-checklist.md) | Formal decision walkthrough (Sections A–E) — complete 2026-08-20 |
| [competitor-research.md](./competitor-research.md) | BuilderTrend deep dive, small-operator segment, CF, BuildPass |
| [customer-discovery.md](./customer-discovery.md) | Interview questions for GCs, subs, homeowners |
| [prds/prd-ContractorPro-2026-08-15/prd.md](./prds/prd-ContractorPro-2026-08-15/prd.md) | **v0.1 MVP PRD** (draft) — product requirements |
| [prds/prd-ContractorPro-2026-08-15/epics-and-stories.md](./prds/prd-ContractorPro-2026-08-15/epics-and-stories.md) | v0.1 dev stories (**44**) — implementation breakdown |
| [prds/prd-ContractorPro-2026-08-15/user-journeys/](./prds/prd-ContractorPro-2026-08-15/user-journeys/) | **v0.1 user journeys (SME review)** — contractor, sub, customer, admin |
| [prds/prd-ContractorPro-2026-08-15/user-journeys.md](./prds/prd-ContractorPro-2026-08-15/user-journeys.md) | **v0.1 user journeys (full detail)** — step tables, system behavior |
| [technical-exploration/](./technical-exploration/) | Pre-architecture exploration (some superseded by architecture-v0.1) |
| [../finances/monthly-run-rate.md](../finances/monthly-run-rate.md) | **Operating budget** — Azure, domain, telco COGS, design, ads |

When you return, start here. Ask the agent to read this README and `discovery-log.md` for full context.

---

## Current phase

**Planning complete — ready for M1 build (2026-08-20)**

- ✅ PRD drafted and synced with architecture
- ✅ Architecture / TRD: [architecture-v0.1.md](./architecture-v0.1.md)
- ✅ Epics & stories: 12 epics, 44 stories (M1–M21 checklist)
- ✅ Sections A–E of [planning-decision-checklist.md](./planning-decision-checklist.md) locked
- ⏭️ **Next:** implementation in `docs/implementation-artifacts/` (empty) + application code

PRD: [prds/prd-ContractorPro-2026-08-15/prd.md](./prds/prd-ContractorPro-2026-08-15/prd.md)

---

## Session handoff (last updated: 2026-08-20)

### Positioning

**Schedule coordination layer** — not a cheaper Buildertrend:

- When the schedule moves, everyone who needs to know finds out — in **Google Calendar** and via **text/link**
- **Integrate, don't replace** — Google Calendar two-way for GC; subs/customers via **event attendee invites**
- **Magic-link subs/homeowners** — no app install; GC stays hub
- **Cascade in MVP** — optional per-project; preview + re-confirm
- **Event-triggered AI** — deferred v0.2+

**One-liner:** *ContractorPro helps small residential GCs keep subs and homeowners aligned when the schedule moves — without replacing Google Calendar or paying enterprise prices.*

### Target users

| Segment | Status |
|---------|--------|
| **Core ICP** | 2–5 person residential GCs / boutique remodelers who churned BT over price, setup, or sub adoption |
| **Expansion** | Up to 8–15 employees |
| **Not primary** | Solo specialty trades (Jobber territory) |

### Client UI (decided 2026-08-14)

**Responsive web only — no native iOS/Android apps.**

| Surface | Device | Experience |
|---------|--------|------------|
| GC dashboard | Laptop/desktop first | Full scheduling, cascade, messaging, portfolio calendar |
| GC field | Mobile browser | Core actions; not full desktop parity |
| Sub/homeowner portals | Mobile browser first | Magic links — confirm dates, photos, messages |

Single React SPA: `/app/*` (team member) + `/p/*` (portal). No `/admin` UI in M1.

### Stack (locked 2026-08-20)

| Layer | Choice |
|-------|--------|
| API | ASP.NET Core .NET 9 (modular monolith) |
| DB | PostgreSQL 16 + EF Core |
| Frontend | React 19 + TypeScript + Vite + shadcn/ui |
| Team member auth | Entra External ID (CIAM) — **Google only M1** |
| Session | BFF HTTP-only cookie |
| SMS/MMS | Twilio (Telnyx spike SP-2 before prod scale) |
| Email | Resend |
| Billing (Phase 2) | Stripe Billing |
| Hosting | Azure |

### MVP v0.1 scope (in)

- Self-serve Google OAuth signup + guided onboarding
- Projects + tasks + dependencies + **cascade** (preview + apply)
- **Pro-provided Google calendar** per project + **portfolio calendar UI**
- Sub/customer calendar via **Google event attendee invites** (not Apple in MVP)
- Magic-link invite + propose → accept/decline → poke
- MMS group threads (project handle #) + photos
- Platform-global **STOP/opt-out** API (no admin UI — Twilio/DB ops)
- **No Stripe, no billing gates** — full coordination for beta

### MVP v0.1 scope (out)

- Native apps, Microsoft Calendar, Apple Calendar connect (v0.1.1)
- Stripe Billing, sandbox gates (Phase 2 immediately post-MVP)
- AI drafts, job planning module (v0.2)
- Admin `/admin` UI (Phase 2; M1 = API + manual ops)
- E1-S2 native auth (v0.1.1)

### Phase 2 (post-MVP)

- Stripe Checkout + Customer Portal + webhooks
- Free sandbox tier (plan-only); ~$100/mo per 5 concurrent active projects
- Dunning → messaging_suspended
- Admin UI for tenant ops (A-1, A-5, etc.)

### Recommended next steps (ordered)

1. **M1 — Auth + auto-provision Contractor** ← **START HERE**
2. M2–M7 — company profile, project create, onboarding, calendar connect, invites
3. M8–M11 — propose/accept/decline, poke, dashboard
4. M12–M15 — calendar write, MMS, customer feed
5. M16–M17 — cascade
6. M20–M21 — portfolio calendar, STOP/opt-out (before prod SMS)
7. Pre-launch checklist PL-1–PL-8 (10DLC, domain, OAuth verification)

### Still open (non-blocking for M1)

- Multi-team-member permissions (simplify v0.1)
- Customer discovery interviews (3–5 GCs)
- Google OAuth app verification timeline for public launch
- Cascade edge cases (business days, partial cascade) — v0.2

### Parking lot (deferred)

- AI photo estimating, supplier web-clipping, T&M, embedded financing
- Native iOS/Android, offline-first, WhatsApp
- Per-tenant 10DLC brands, same-company number pool reuse (v0.1.1)

---

## BMAD

BMAD Method v6.11 installed (`_bmad/`, `.agents/skills/`). Use `bmad-dev-story` or `bmad-build` for M1 implementation.
