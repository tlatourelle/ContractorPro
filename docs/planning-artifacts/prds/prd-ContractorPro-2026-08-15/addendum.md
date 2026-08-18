# PRD Addendum — Technical & Architecture References

This addendum holds material that **does not belong in the PRD** but supports downstream architecture, TRD, and implementation work.

The PRD (`prd.md`) states **capabilities and user outcomes**. This document points to **how** those capabilities may be implemented.

---

## Architecture / TRD (to be created)

A consolidated **Architecture Document** or **TRD v0.1** should be authored separately (BMAD: `bmad-create-architecture`) and cover:

- System context (web app, API, DB, background workers)
- Tenancy model (**Contractor** subscription-scoped data)
- Auth boundaries (Team member OAuth vs project membership magic links)
- Integration contracts (Google Calendar API, SMS provider, blob storage, billing)
- Cross-cutting: security, observability, idempotency for notifications
- Data model (entities referenced in discovery-log)

**Placeholder path:** `docs/planning-artifacts/architecture-v0.1.md` (not yet written)

---

## Technical exploration index

| Topic | Document |
|-------|----------|
| Stack (.NET API, React web, Postgres) | `../technical-exploration/stack-web-api-db.md` |
| Azure hosting alignment | `../technical-exploration/azure-alignment.md` |
| Google Cloud (Calendar OAuth only) | `../technical-exploration/google-cloud-vs-azure.md` |
| Database options | `../technical-exploration/database-options.md` |
| Auth BYOA vs native | `../technical-exploration/auth-byoa-vs-native-mfa.md` |
| Auth & data overview | `../technical-exploration/auth-and-data.md` |
| Google Calendar integration | `../technical-exploration/google-calendar-integration.md` |
| Schedule confirmation + poke engine | `../technical-exploration/schedule-confirmation-workflow.md` |
| Invite & join flow | `../technical-exploration/invite-join-flow.md` |
| Messaging & media | `../technical-exploration/messaging-and-media.md` |
| Job planning (v0.2) | `../technical-exploration/job-planning-workflow.md` |
| MVP roadmap review | `../technical-exploration/external-mvp-roadmap-review.md` |

---

## Mechanism decisions (product-adjacent, technically owned)

PRD identity model: [prd.md §3](./prd.md) — Contractor subscription vs per-project Subcontractor/Customer roles.

These were decided in exploration and should land in architecture/TRD, not PRD:

| Decision | Lean for v0.1 |
|----------|----------------|
| Calendar provider (invitee link) | **Google Calendar + Apple iCal/iCloud** at v0.1; **Google preferred** (default UI, primary adapter) |
| Calendar provider (GC company) | Google Calendar API first |
| Sub calendar access | Email ACL on shared project calendar; no sub OAuth required |
| Calendar write timing | On sub **accept** only |
| Accept mechanism | Magic link (SMS/email); no SMS reply parsing |
| Poke ownership | ContractorPro background worker; not Google |
| Participant identity | Phone-keyed **project membership** per project; role = `subcontractor` \| `customer`; no global user type |
| Team member vs membership | `users` belong to **Contractor** subscription; `project_memberships` are separate |
| Primary DB | Relational (Postgres lean) |
| Hosting | Azure app/DB; Google Cloud project for Calendar API |
| Images | Blob storage + SQL metadata |

---

## Deferred technical topics (v0.2+)

- Microsoft Graph calendar adapter
- GC-side Apple Calendar connect (if not v0.1)
- SMS relay / virtual group member per thread
- Job planning: `work_phases`, portfolio conflict engine
- Google free/busy overlay for GC personal calendar
- Event-driven notification bus (Service Bus vs in-process)
- PWA / offline read-only for participant pages

---

## Open technical forks (for architecture doc)

See `../discovery-log.md` — auth vendor, calendar BYO vs pro-provided, webhook hosting, Google OAuth verification timeline for sensitive scopes.
