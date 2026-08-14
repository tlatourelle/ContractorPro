# External MVP Roadmap Review (Gemini)

Status: **Research only** (2026-08-14)  
Source: Gemini-generated technical feature roadmap for small-operator anti-Buildertrend MVP  
Related: [stack-web-api-db.md](./stack-web-api-db.md), [product-vision.md](../product-vision.md), [competitor-research.md](../competitor-research.md)

## Summary

Gemini proposes a **3-phase, 9-month** mobile-first MVP: Phase 1 (scheduling + magic links), Phase 2 (AI estimating + T&M + Stripe payments), Phase 3 (QBO/Xero + embedded financing). Architecture: serverless, Flutter/React Native, offline-first, event-driven.

**ContractorPro takeaway:** Phase 1 patterns largely **validate** our direction. Phases 2–3 are **scope creep** we already parked. Several **specific UX and infra patterns** are worth adopting; stack choices **diverge** from our .NET + React lean.

---

## Phase-by-phase assessment

### Phase 1 (Months 1–3) — Foundations & core workflow

**Stated objective:** Solo contractor onboards, sketches timeline, sends active comm link to client/sub in **10 minutes**.

| Gemini proposal | ContractorPro fit | Action |
|-----------------|-------------------|--------|
| **10-minute TTV** | ✅ Core product goal | Adopt as success metric |
| **Lightweight event/Gantt engine** | ✅ MVP scope | Cascade via API background job |
| **Async cascade on date change** | ✅ Aligns with stack doc | Queue: Azure Functions or `IHostedService` + Service Bus later |
| **Zero-login SMS portal** | ✅ Core MVP | Magic link + Twilio (or ACS) |
| **Signed short-lived magic URLs** | ✅ Aligns with invitee auth | Implement in .NET API; define TTL + rotation |
| **Sub "Confirm Date" toggle** | ✅ New UX detail | Spec'd in [schedule-confirmation-workflow.md](./schedule-confirmation-workflow.md) |
| **Sub photo upload via mobile browser** | ✅ In messaging scope | No app required; blob upload on magic-link session |
| **Passkeys (WebAuthn)** | ✅ Already explored | GC auth; TOTP + passkeys free per auth doc |
| **PWA / Flutter / React Native** | ⚠️ Partial | **Defer native.** Responsive web + optional PWA manifest for GC; not Flutter/RN in v0.1 |
| **Offline-first (WatermelonDB/SQLite)** | ⚠️ Later | Field connectivity is real; **v0.2+** unless discovery proves blocker. GC can queue actions with optimistic UI later |
| **Serverless backend** | ⚠️ Partial | Jobs/serverless OK (Azure Functions); **core API stays ASP.NET Core on App Service** per stack lean |

### Phase 2 (Months 4–6) — AI estimating & financial triage

| Gemini proposal | ContractorPro fit | Action |
|-----------------|-------------------|--------|
| **OCR / blueprint → estimate grid** | ❌ Out of MVP | Parked — KonstructIQ territory |
| **LLM vision (GPT-4o / Claude) for takeoffs** | ❌ Out of MVP | Parked |
| **Web-clipping extension (Home Depot/Lowe's)** | ❌ Out of MVP | Parked — scraping fragility + new category |
| **Receipt OCR → T&M sheet** | ❌ Out of MVP | Parked |
| **Stripe Connect progress invoicing** | ❌ Out of MVP | We use Stripe/Chargebee for **our** SaaS billing only in v0.1 |
| **Client Apple Pay / ACH on change orders** | ❌ Out of MVP | Parked — homeowner payments deferred |

### Phase 3 (Months 7–9) — Platform & ecosystem

| Gemini proposal | ContractorPro fit | Action |
|-----------------|-------------------|--------|
| **QBO / Xero two-way webhooks** | ⏳ Post-MVP | Explore after coordination wedge proven |
| **Hide ledger complexity from field UI** | ✅ Principle | Adopt when we integrate accounting — not v0.1 |
| **Embedded financing (Hearth / Wisetack)** | ❌ Parked | BT already has financing partners; regulatory lift |

---

## Architecture comparison

### Gemini conceptual architecture

```
Mobile App/PWA → API Gateway (Passkeys)
    → Core Workflow Engine (cascade, SMS bus)
    → Field AI Engine (blueprint parser, receipt OCR)
    → Financial Gateway (Stripe, POS loans)
    → Offline-first sync datastore
    → QBO/Xero webhooks
```

### ContractorPro v0.1 architecture (recommended)

```
Responsive web (GC) + thin magic-link pages (sub/homeowner)
    → ASP.NET Core API (App Service)
    → PostgreSQL + EF Core
    → Azure Blob (images)
    → Background jobs: cascade notify, SMS dispatch
    → Google Calendar API (two-way — Gemini omits entirely)
    → Twilio SMS
    → Stripe/Chargebee (SaaS subscription only)

External (post-MVP): QBO explore
```

**Key gap in Gemini roadmap:** No **Google Calendar two-way sync** — our primary wedge vs Buildertrend's one-way iCal feed.

---

## Useful patterns to adopt (Phase 1 gleanings)

### 1. Event-driven notification bus

When schedule changes:

```
ScheduleChangeEvent → CascadeService → NotificationService → SMS + in-app
                     → CalendarSyncService → Google Calendar API
                     → (optional) AiDraftService → homeowner message draft
```

Decouple cascade logic from SMS/calendar/AI so each can fail/retry independently.

### 2. Async cascade processing

- User confirms schedule change in UI → API persists + enqueues `CascadeJob`
- Worker recalculates dependents, writes audit log, fires notifications
- GC sees preview **before** confirm; worker executes **after** confirm
- Matches Gemini's "background job queue" without full serverless rewrite

### 3. Magic link security model (detail)

| Property | Recommendation |
|----------|----------------|
| **Signing** | HMAC or JWT signed by API; include `projectId`, `inviteeId`, `purpose` |
| **TTL** | Short-lived for sensitive actions (24–72h); refresh on SMS resend |
| **Scope** | Sub link ≠ homeowner link; thread-scoped permissions |
| **Actions** | View schedule, confirm date, upload photo, reply in thread |
| **No account** | Session cookie bound to magic token; no password |

### 4. Sub portal minimal UX (from Gemini)

- **Confirm Date** — Accept/Decline via magic link (SMS and/or email); GC sees confirmation status — see [schedule-confirmation-workflow.md](./schedule-confirmation-workflow.md)
- **Upload photo** — camera capture from mobile browser
- **View my tasks only** — per-sub slice (already in vision)
- Large touch targets for field use (glove-friendly) — apply to invitee pages

### 5. Passkeys for GC field login

Already in [auth-byoa-vs-native-mfa.md](./auth-byoa-vs-native-mfa.md). Gemini reinforces: passwordless on-site matters for GCs, not subs.

### 6. Offline-first — staged approach

| Version | Offline capability |
|---------|-------------------|
| **v0.1** | Online-first; graceful error states; SMS still delivers when GC offline |
| **v0.2** | Service worker cache for invitee pages; read-only schedule offline |
| **v0.3+** | GC optimistic edits + sync queue if field pain validated |

Don't block MVP on WatermelonDB/SQLite — adds significant complexity.

---

## Stack conflicts (do not adopt from Gemini)

| Gemini choice | Our lean | Why |
|---------------|----------|-----|
| Flutter / React Native | React responsive web (+ optional PWA) | Team .NET strength; no native app in v0.1; two deployables enough |
| Full serverless | App Service API + Functions for jobs | .NET API is decided lean; serverless for everything fights EF Core patterns |
| Offline-first local DB | PostgreSQL as source of truth | Simpler multi-tenant model; offline is enhancement |
| Scraping supplier DOMs | — | Fragile, legal gray area, not our wedge |
| Stripe Connect for clients | Stripe for SaaS billing only | Different product surface |

---

## Revised ContractorPro phase map (research)

Distills Gemini's 9-month plan into our narrower scope:

| Phase | Timeline | Scope | Gemini equivalent |
|-------|----------|-------|-------------------|
| **v0.1** | Months 1–3 | Projects, tasks, cascade, magic links, SMS, messaging + images, Google Calendar sync, GC auth, SaaS billing | Phase 1 only (minus offline-first, minus native) |
| **v0.2** | Months 4–6 | **Job planning** (phases, buffers, portfolio, finalize), AI draft on cascade, read receipts, PWA polish, SMS relay opt-in — see [job-planning-workflow.md](./job-planning-workflow.md) | Small slice of Gemini Phase 1 + our AI comms |
| **v0.3+** | Months 7+ | QBO explore, multi-GC-user, Microsoft calendar if demanded | Partial Gemini Phase 3 |
| **Never / parking lot** | — | AI estimating, web clipping, T&M OCR, embedded financing, client payments | Gemini Phase 2–3 bulk |

---

## Open technical questions (from this review)

- [x] **Confirm Date** — **required** ack for assigned subs; calendar syncs on accept only — see [schedule-confirmation-workflow.md](./schedule-confirmation-workflow.md)
- [ ] **Magic link TTL** — 24h vs 7d vs until project complete?
- [ ] **Cascade job idempotency** — what if GC drags same task twice quickly?
- [ ] **Notification bus** — in-process events vs Azure Service Bus at launch?
- [ ] **PWA** — worth service worker in v0.1 for sub pages or defer?
- [ ] **Glove-friendly UI** — design token minimum touch target (48px?) for invitee views

---

## References

- Internal: [stack-web-api-db.md](./stack-web-api-db.md), [messaging-and-media.md](./messaging-and-media.md), [google-calendar-integration.md](./google-calendar-integration.md), [schedule-confirmation-workflow.md](./schedule-confirmation-workflow.md), [job-planning-workflow.md](./job-planning-workflow.md)
- External segment: [competitor-research.md](../competitor-research.md) § Small-operator anti-Buildertrend segment

Log updates in [discovery-log.md](../discovery-log.md).
