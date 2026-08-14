# ContractorPro — Planning Hub

Living documentation for discovery, exploration, and product planning. **No code yet** — we're in the thinking phase.

## How to use this folder

| Document | Purpose |
|----------|---------|
| [product-vision.md](./product-vision.md) | North star: who we serve, wedge, principles, MVP boundary (updated when direction solidifies) |
| [discovery-log.md](./discovery-log.md) | **Main working doc** — ideas, open questions, decisions, research backlog |
| [competitor-research.md](./competitor-research.md) | BuilderTrend, Contractor Foreman, BuildPass analysis |
| [customer-discovery.md](./customer-discovery.md) | Questions to ask real GCs, subs, and homeowners |
| [technical-exploration/auth-and-data.md](./technical-exploration/auth-and-data.md) | OAuth BYOA, magic links, Postgres hosting path |
| [technical-exploration/auth-comparison-managed-vs-authjs.md](./technical-exploration/auth-comparison-managed-vs-authjs.md) | Clerk/Supabase vs Auth.js pros/cons + free tiers |
| [technical-exploration/database-options.md](./technical-exploration/database-options.md) | Postgres hosts: Neon, Supabase, Azure, free tiers, scale path |
| [technical-exploration/azure-alignment.md](./technical-exploration/azure-alignment.md) | Azure-first hosting, DB, auth path (team skillset) |
| [technical-exploration/messaging-and-media.md](./technical-exploration/messaging-and-media.md) | Image-heavy messaging, Azure Blob, SMS relay exploration |
| [technical-exploration/google-calendar-integration.md](./technical-exploration/google-calendar-integration.md) | Dual-view calendar, BYO vs Pro-provided |
| [technical-exploration/google-cloud-vs-azure.md](./technical-exploration/google-cloud-vs-azure.md) | Azure hosting + Google Cloud for APIs only |
| [technical-exploration/nosql-vs-relational.md](./technical-exploration/nosql-vs-relational.md) | Why relational primary DB |
| [technical-exploration/sql-server-vs-postgres.md](./technical-exploration/sql-server-vs-postgres.md) | Postgres vs SQL Server for .NET |
| [technical-exploration/auth-byoa-vs-native-mfa.md](./technical-exploration/auth-byoa-vs-native-mfa.md) | BYO OAuth vs native accounts, TOTP/passkeys |
| [technical-exploration/stack-web-api-db.md](./technical-exploration/stack-web-api-db.md) | .NET API + web frontend architecture |

When you return to this project, start here. Ask the agent to read this README and `discovery-log.md` for context and recommended next steps.

## Current phase

**Discovery & exploration** — not ready for PRD, architecture, or implementation.

## Session handoff (last updated: 2026-08-13)

### Where we left off

- Planning hub complete — product vision, discovery log, competitors, customer questions, 11 technical exploration docs
- **Wedge:** Cheaper/simpler than BuilderTrend/CF/BuildPass; integrate don't replace; **optional schedule cascade** is signature feature
- **Users:** Small residential GC (pays); subs + homeowners (free magic-link + SMS)
- **Stack lean:** ASP.NET Core API + web frontend (React vs Blazor TBD) + relational DB (Postgres vs SQL Server TBD); **Azure** for app/DB; **Google Cloud project** for Calendar/OAuth only
- **Messaging:** Heavy images from **all three roles** (GC, sub, homeowner); Azure Blob + SQL metadata; SMS = notify + link for images
- **SMS relay explored:** Virtual 3rd-party number per thread (not one megagroup) — opt-in post-MVP default; preserves GC-as-hub privacy
- **Auth:** GC = OAuth BYOA or native accounts (TOTP/passkeys); invitees = magic links
- **Calendar:** Google primary; dual-view (native Google + ContractorPro); BYO or Pro-provided per company
- BMAD Method v6.11 installed (`_bmad/`, `.agents/skills/`) for future structured planning

### Decisions locked (see discovery-log.md)

| Area | Decision |
|------|----------|
| Primary user | Small residential GC |
| Invitee access | Magic web link + SMS |
| Cascade | Optional per-project; signature differentiator |
| Images | All three roles upload in v0.1 |
| SMS images | Web portal only; SMS nudges with link |
| Hosting | Azure over AWS |
| DB type | Relational primary (engine TBD) |
| Monetization | Flat tiers + free tier |

### Recommended next steps (pick one when you return)

1. **Cascade scheduling deep-dive** — edge cases, Google Calendar sync conflicts, business-day rules
2. **Customer discovery** — 1–2 GC conversations using `customer-discovery.md` (validate cascade, SMS relay appetite)
3. **Stack spikes** — React vs Blazor UI; Entra External ID vs ASP.NET Core Identity
4. **Product brief** — when discovery feels sufficient (`/bmad-product-brief` or draft from docs)

### Open forks (not decided)

- Postgres vs SQL Server
- React vs Blazor
- Entra External ID vs ASP.NET Core Identity vs Clerk
- Calendar MVP: BYO only, Pro-provided only, or both
- SMS relay: MVP default vs opt-in per thread
- Project photo timeline vs thread-only views

### Not yet

- PRD, architecture doc, UI mockups, application code, infra provisioning, Chargebee vs Stripe spike
