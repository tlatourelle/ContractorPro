# ContractorPro — Planning Hub

Living documentation for discovery, exploration, and product planning. **No application code yet** — we're in the thinking phase.

---

## ⏭️ When you return — do this first

> **Next step: Database schema design for v0.1**
>
> Design the relational schema for projects, tasks, **task assignments (propose/accept)**, cascade dependencies, magic-link invitees, messaging threads, and calendar sync metadata. This is the critical path before sprint planning or PRD finalization.
>
> **Prompt to use:** *"John, let's design the v0.1 database schema — projects, tasks, cascade, magic links, messaging."*
>
> **Read first:** `product-vision.md` (MVP boundary), `discovery-log.md` (decisions), `stack-web-api-db.md` (architecture lean), `messaging-and-media.md` (attachments), `google-calendar-integration.md` (calendar entities), `schedule-confirmation-workflow.md` (propose → accept → sync), `job-planning-workflow.md` (plan → finalize).

---

## How to use this folder

| Document | Purpose |
|----------|---------|
| [product-vision.md](./product-vision.md) | North star: who we serve, wedge, client UI strategy, MVP boundary |
| [discovery-log.md](./discovery-log.md) | **Main working doc** — ideas, open questions, decisions, session notes |
| [competitor-research.md](./competitor-research.md) | BuilderTrend deep dive, small-operator segment, CF, BuildPass |
| [customer-discovery.md](./customer-discovery.md) | Interview questions for GCs, subs, homeowners |
| [technical-exploration/stack-web-api-db.md](./technical-exploration/stack-web-api-db.md) | .NET API + responsive web frontend architecture |
| [technical-exploration/external-mvp-roadmap-review.md](./technical-exploration/external-mvp-roadmap-review.md) | Gemini roadmap review — what to adopt vs reject |
| [technical-exploration/google-calendar-integration.md](./technical-exploration/google-calendar-integration.md) | Dual-view calendar, BYO vs Pro-provided |
| [technical-exploration/schedule-confirmation-workflow.md](./technical-exploration/schedule-confirmation-workflow.md) | Propose → accept → sync; SMS/email notify; GC pending dashboard; poke engine |
| [technical-exploration/job-planning-workflow.md](./technical-exploration/job-planning-workflow.md) | Plan phases/buffers → portfolio balance → finalize → schedule |
| [prds/prd-ContractorPro-2026-08-15/prd.md](./prds/prd-ContractorPro-2026-08-15/prd.md) | **v0.1 MVP PRD** (draft) — product requirements; tech in addendum |
| [technical-exploration/messaging-and-media.md](./technical-exploration/messaging-and-media.md) | Image messaging, Azure Blob, SMS relay |
| [technical-exploration/invite-join-flow.md](./technical-exploration/invite-join-flow.md) | Easy join for subs/homeowners — name + phone, passwordless |
| [technical-exploration/auth-and-data.md](./technical-exploration/auth-and-data.md) | OAuth BYOA, magic links, Postgres hosting |
| [technical-exploration/azure-alignment.md](./technical-exploration/azure-alignment.md) | Azure-first hosting path |
| [technical-exploration/database-options.md](./technical-exploration/database-options.md) | Postgres hosts: Neon, Supabase, Azure |
| [technical-exploration/nosql-vs-relational.md](./technical-exploration/nosql-vs-relational.md) | Why relational primary DB |
| [technical-exploration/sql-server-vs-postgres.md](./technical-exploration/sql-server-vs-postgres.md) | Postgres vs SQL Server for .NET |
| [technical-exploration/google-cloud-vs-azure.md](./technical-exploration/google-cloud-vs-azure.md) | Azure hosting + Google Cloud for APIs only |
| [technical-exploration/auth-comparison-managed-vs-authjs.md](./technical-exploration/auth-comparison-managed-vs-authjs.md) | Clerk/Supabase vs Auth.js (deprioritized for .NET) |

When you return, start here. Ask the agent to read this README and `discovery-log.md` for full context.

---

## Current phase

**PRD drafted (v0.1 fast path)** — review and lock scope; then **database schema** and **architecture/TRD**; then epics & stories.

PRD: [prds/prd-ContractorPro-2026-08-15/prd.md](./prds/prd-ContractorPro-2026-08-15/prd.md)

---

## Session handoff (last updated: 2026-08-14)

### Positioning (refined)

**We are NOT a cheaper Buildertrend.** We are a **schedule coordination layer**:

- When the schedule moves, everyone who needs to know finds out — in **Google Calendar** and via **text/link**
- **Integrate, don't replace** — Google Calendar is first-class (two-way); BT only has one-way iCal feed
- **Magic-link subs/homeowners** — no app install; GC stays hub
- **Optional cascade** — table stakes (BT has it too); our bundle is calendar + frictionless comms + price
- **Event-triggered AI** — draft "what changed" on schedule shift (not BT's weekly digest requiring daily logs)

**One-liner:** *ContractorPro helps small residential GCs keep subs and homeowners aligned when the schedule moves — without replacing Google Calendar or paying enterprise prices.*

### Target users (refined)

| Segment | Status |
|---------|--------|
| **Core ICP** | 2–5 person residential GCs / boutique remodelers who churned BT over price, setup, or sub adoption |
| **Expansion** | Up to 8–15 employees |
| **Not primary** | Solo specialty trades (Jobber territory) |

### Client UI (decided 2026-08-14)

**Responsive web only — no native iOS/Android apps.**

| Surface | Device | Experience |
|---------|--------|------------|
| GC dashboard | Laptop/desktop first | Full scheduling, cascade, messaging, settings |
| GC field | Mobile browser | Core actions; not full desktop parity |
| Sub/homeowner portals | Mobile browser first | Magic links — confirm dates, photos, messages |

Optional PWA later. Online-first v0.1 (no offline SQLite sync).

### Competitor intelligence (2026-08-14)

**BuilderTrend:**

- Volume-based custom quotes in 2026 ($299–$900+/mo typical); 12-week onboarding
- Has Gantt cascade + sub notifications — **not unique to us**
- One-way iCal to Google Calendar only
- AI Client Updates (Jun 2025) = weekly digest; AI Bill Pay (Feb 2026)
- Sub resistance is structural weakness
- Moat = Selections + Client Portal (we defer)

**Small-operator anti-BT segment:**

- Transparent pricing ($39–49/mo), 10-min onboarding, magic-link subs validated by market
- **Reject for MVP:** AI estimating, supplier clipping, T&M invoicing, embedded financing

**Gemini technical roadmap:**

- **Adopt:** async cascade, event notification bus, signed magic URLs, Confirm Date UX, passkeys
- **Reject:** Flutter/RN, full serverless, offline-first DB, Phase 2–3 financial/estimating scope

Full detail: [competitor-research.md](./competitor-research.md), [external-mvp-roadmap-review.md](./technical-exploration/external-mvp-roadmap-review.md)

### Decisions locked (see discovery-log.md)

| Area | Decision |
|------|----------|
| Primary user | Small residential GC (2–5 core, up to 15) |
| Positioning | Schedule coordination layer, not BT clone |
| Invitee access | Magic web link + SMS; no accounts for subs/homeowners |
| Cascade | Optional per-project; bundled with calendar + comms |
| Images | All three roles upload in v0.1 |
| Client UI | **Responsive web only** — no native apps |
| GC UI | Desktop-first |
| Invitee UI | Mobile-first magic-link pages |
| Hosting | Azure over AWS |
| DB type | Relational primary (Postgres lean) |
| API | ASP.NET Core (.NET) |
| Frontend | React + TypeScript (lean; Blazor swap possible) |
| Monetization | Flat tiers + free tier |
| Calendar | Google primary; sync on sub accept; BYO or Pro-provided — see [schedule-confirmation-workflow.md](./technical-exploration/schedule-confirmation-workflow.md) |
| Auth (GC) | OAuth BYOA or native; TOTP/passkeys |
| Auth (invitees) | Magic links |
| Job planning | Phases, buffers, portfolio balance, finalize → schedule (v0.2) — see [job-planning-workflow.md](./technical-exploration/job-planning-workflow.md) |

### MVP v0.1 scope (in)

- Projects + task timeline + optional cascade
- Google Calendar two-way sync
- Magic-link sub/homeowner portals + SMS notifications
- Messaging with image uploads (GC, sub, homeowner)
- GC auth + SaaS subscription billing
- AI comms drafts (stretch)

### MVP v0.1 scope (out)

- Native mobile apps, Flutter, React Native
- Estimating, selections, T&M, client payments
- Embedded financing, QBO deep sync
- Offline-first local DB
- Microsoft Calendar
- Full job planning module (v0.2)

### MVP v0.2 scope (in)

- Job planning: phases, durations, buffers, in-app preview, portfolio balance, sub conflicts
- Finalize → schedule confirmation handoff
- See [job-planning-workflow.md](./technical-exploration/job-planning-workflow.md)

### Recommended next steps (ordered)

1. **Database schema for v0.1** ← **START HERE**
2. v0.1 sprint plan / epic breakdown
3. Customer discovery (3–5 GCs who evaluated/churned BT)
4. PRD draft (`bmad-prd`)
5. Stack spikes (Entra vs Identity, React UI spike)

### Open forks (not decided)

- Postgres vs SQL Server (Postgres lean)
- React vs Blazor
- Entra External ID vs ASP.NET Core Identity vs Clerk
- Calendar MVP: BYO only, Pro-provided only, or both
- Magic link TTL and Confirm Date required vs optional
- SMS relay: MVP default vs opt-in per thread
- Free tier limits and price points ($29–79 range to validate)

### Parking lot (deferred / maybe never)

- AI photo estimating, supplier web-clipping
- T&M receipt OCR, Stripe Connect client payments
- Embedded consumer financing (Hearth/Wisetack)
- Native iOS/Android apps
- Offline-first sync
- WhatsApp integration

### Not yet

- PRD, architecture doc, UI mockups, application code, infra provisioning

---

## BMAD

BMAD Method v6.11 installed (`_bmad/`, `.agents/skills/`). Invoke `bmad-agent-pm` (John) for PRD work after schema + discovery.
