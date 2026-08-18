# Discovery Log

Chronological and thematic capture of ideas, questions, and decisions. **Add to this document** as exploration continues.

---

## ⏭️ Resume here

**Next step when you return:** **Database schema design for v0.1**

Entities to model: companies, users, projects (`planning` \| `active`), **work_phases** (duration, buffer — v0.2), tasks, **task_assignments** (propose/accept status), task dependencies (cascade), **project_participants** (subs/homeowners — name + phone join, `notify_via`), **participant_sessions** (magic links), message threads, attachments metadata, calendar sync mappings, **confirmation_tokens**, **reminder_schedules**, notification log, subscription tier.

See [invite-join-flow.md](./technical-exploration/invite-join-flow.md), [schedule-confirmation-workflow.md](./technical-exploration/schedule-confirmation-workflow.md), [job-planning-workflow.md](./technical-exploration/job-planning-workflow.md).

See [README.md](./README.md) for full session handoff (2026-08-14).

---

## Decisions (tentative)

| Date | Decision | Notes |
|------|----------|-------|
| 2026-08-13 | Use BMAD for long-term planning | Installed in repo; brainstorming skipped in favor of direct conversation |
| 2026-08-13 | Primary user = small residential GC | Expand to commercial + service later |
| 2026-08-13 | Subs + homeowners both first-class | Different visibility; private messaging channels |
| 2026-08-13 | Access = web link + SMS | No native app for invitees in v0.1 |
| 2026-08-13 | Google Calendar primary for scheduling integration | Most people don't use Microsoft Calendar except at work; Google is default personal/small-biz calendar |
| 2026-08-13 | Microsoft 365 Calendar | **Later**, work-context / enterprise GCs — not v0.1 priority |
| 2026-08-13 | Monetization = tiers + free tier | Chargebee/Stripe for our billing; flat low monthly |
| 2026-08-13 | Wedge = cheaper, simpler, integrate-don't-replace + AI-forward | Evolved 2026-08-14 → **schedule coordination layer**; see below |
| 2026-08-13 | Signature feature = optional schedule cascade | Bundled with calendar + comms; BT has cascade too — not standalone category |
| 2026-08-13 | US only, English, USD for foreseeable future | |
| 2026-08-13 | QBO integration = explore post-MVP | Quicken only if users ask; often confused with QBO |
| 2026-08-13 | Auth: BYO OAuth **or** native ContractorPro accounts | Same pattern as calendar; native unavoidable for some GCs |
| 2026-08-13 | MFA / passkeys | Prefer **free**: TOTP + passkeys; avoid SMS MFA; see auth-byoa-vs-native-mfa.md |
| 2026-08-13 | Invitee auth = magic link (not OAuth) | Subs/homeowners via email/SMS link; still tracked in our DB |
| 2026-08-13 | Image uploads in messaging | **All three roles** — GC, sub, and homeowner — key to communication & project tracking |
| 2026-08-13 | Primary database type | **Relational** (Postgres or SQL Server); NoSQL not primary — see nosql-vs-relational.md |
| 2026-08-13 | Auth vendor undecided | Entra External ID vs ASP.NET Identity vs Clerk — see auth-byoa-vs-native-mfa.md |
| 2026-08-13 | Cloud preference: **Azure over AWS** | Team skillset; not locked — see azure-alignment.md |
| 2026-08-13 | Architecture: **web frontend + API + DB** | Separation of concerns; see stack-web-api-db.md |
| 2026-08-13 | Backend lean: **.NET / ASP.NET Core** | Personal strength; vibe-coded OK; frontend TBD |
| 2026-08-13 | Hosting on GCP vs Azure | **Azure for app/DB**; **Google Cloud project** for Calendar/OAuth APIs only — see google-cloud-vs-azure.md |
| 2026-08-14 | **No native mobile apps** | **Responsive web only** — mobile-friendly for field/subs/homeowners; **full experience on laptop/desktop** for GC; PWA optional later, not v0.1 priority |
| 2026-08-14 | Positioning = schedule coordination layer | Not "cheaper BT"; Google Calendar two-way + magic links + price |
| 2026-08-14 | ICP core = 2–5 person GC crews | BT churners; solo trades = Jobber lane |
| 2026-08-14 | Reject external roadmap Phase 2–3 | No AI estimating, T&M, embedded financing, supplier clipping in MVP |
| 2026-08-14 | **Easy join for subs/homeowners** | GC invites → invitee confirms name + phone only; passwordless; phone = identity |
| 2026-08-14 | **Passwordless by default** | GC: OAuth/passkeys preferred; invitees: magic link/SMS OTP — no passwords for invitees |
| 2026-08-14 | **Schedule confirmation workflow** | GC proposes → sub accepts/declines via magic link (SMS and/or email) → calendar syncs on accept only; GC dashboard for pending/confirmed/declined — see [schedule-confirmation-workflow.md](./technical-exploration/schedule-confirmation-workflow.md) |
| 2026-08-14 | **Sub notify_via** | Per-participant `sms` \| `email` \| `both` for schedule proposals; link-based accept (no SMS reply parsing) |
| 2026-08-14 | **Automated poke / reminders** | Daily SMS/email reminders until sub accepts/declines — **not** Google Calendar’s job; match BT persistence; batch + quiet hours + GC escalation — see schedule-confirmation-workflow.md |
| 2026-08-18 | **Journey expansion + backlog** | Added C-19–C-27, M-1–M-5 (Maci same access as Ryan), S-16–S-22, H-21–H-24, UJ-9 cross-persona slip; [backlog.md](./prds/prd-ContractorPro-2026-08-15/user-journeys/backlog.md) for discovery items; [future-journeys-v02.md](./prds/prd-ContractorPro-2026-08-15/user-journeys/future-journeys-v02.md) for v0.2+ |
| 2026-08-15 | **Identity vs roles** | **Contractor** = only fixed SaaS subscription; **Subcontractor** / **Customer** = per-project roles; same Person may differ by project; Team member may also be Sub on another Contractor's project — PRD §3, FR-20 |
| 2026-08-15 | **v0.1 epics & stories** | 11 epics, 31 user stories, suggested solo build order — [epics-and-stories.md](./prds/prd-ContractorPro-2026-08-15/epics-and-stories.md) |
| 2026-08-15 | **Counter-propose + reassignment** | Pending party can Accept / Counter-propose / Decline; negotiation thread; on decline Dana reassigns task to different sub (UJ-2d, UJ-2e; FR-8, FR-9a) — [user-journeys.md](./prds/prd-ContractorPro-2026-08-15/user-journeys.md) |
| 2026-08-17 | **MMS group threads (v0.1)** | Primary comms = native group MMS: Dana + sub + **project handle #** per relationship; app ingests/logs; Dana acts on schedule in web app; system sends confirmation MMS/SMS — **not** app-orchestrated chat — [messaging-and-media.md](./technical-exploration/messaging-and-media.md), UJ-8 |
| 2026-08-17 | **AI out of MVP** | No SMS intent parsing, no AI drafts, no auto-schedule from chat in v0.1 — defer to v0.2+ |
| 2026-08-17 | **Two lanes: MMS vs app** | **Conversation** = group MMS (logged). **Scheduling** = web app (Dana); multi-job complexity stays in app; MMS carries confirm links after Dana commits |
| 2026-08-17 | **MMS routing: per-project handle #** | One phone number per **project** (Maple # in all Dana↔sub/customer groups on that job); inbound `To` → project, `From` → membership; store platform `conversation_sid` / internal thread id at provision — [messaging-and-media.md](./technical-exploration/messaging-and-media.md) |
| 2026-08-18 | **Handle # vendor & pooling** | CPaaS (Twilio default; Telnyx spike); per-company **number pool** with cooling on archive; Google Voice ❌; ACS group MMS ❌ — [project-handle-numbers.md](./technical-exploration/project-handle-numbers.md) |
| 2026-08-18 | **Operating budget doc** | Monthly run rate + COGS tracking — [finances/monthly-run-rate.md](../finances/monthly-run-rate.md); **~$10/mo** telco planning default per active project |

---

## Ideas backlog

- **Cascade preview** — before applying, show GC which tasks/subs/homeowner notifications will fire
- **“What changed” feed** — homeowner-friendly timeline of schedule shifts (not full Gantt)
- **SMS relay / virtual group member** — **→ Decision 2026-08-17:** MMS group per Dana↔sub (+ handle); see messaging-and-media.md
- **Per-sub task slice** — sub portal shows only their trades/phases
- **Project templates** — common residential remodel phases pre-wired with dependencies — part of [job-planning-workflow.md](./technical-exploration/job-planning-workflow.md) (v0.3)
- **AI weekly digest** for GC — “here’s what moved, who hasn’t read messages” (later)
- **Confirm Date** toggle on sub magic-link portal — sub acks schedule without account → **expanded:** full propose/accept/decline workflow; see [schedule-confirmation-workflow.md](./technical-exploration/schedule-confirmation-workflow.md)
- **Event-driven notification bus** — decouple cascade, SMS, calendar sync, AI draft
- **Async cascade via background job** — preview in UI, execute in worker after confirm
- **Quiet hours** for SMS — respect GC and homeowner preferences
- **Project photo timeline** — unified project tracking view aggregating images from GC, subs (GC-visible), and homeowner

---

## Open questions — product

### Calendar (invitees + GC)

- [x] **v0.1 providers:** Google Calendar + Apple Calendar (iCal/iCloud) — **Google preferred** internally
- [ ] Apple iCal: CalDAV/iCloud OAuth vs webcal subscribe — two-way write path for TRD
- [ ] MVP: BYO only, Pro-provided only, or both at launch?
- [ ] Pro-provided: calendar under GC's Google vs ContractorPro service account?
- [ ] Hybrid mode (company BYO + per-project Pro-provided) — when?
- [ ] Sub calendar access: OAuth connect vs email ACL invite only? → **Lean:** email ACL for MVP; confirm via magic link
- [ ] Two-way sync: what happens if sub drags event in Google? → Defer; app is source of truth until accept
- [ ] Homeowner: Google invite vs app-only?
- [ ] Google OAuth verification timeline for sensitive calendar scopes?

### Cascade scheduling

- [ ] Fixed duration vs fixed end date when cascading?
- [ ] Partial cascade — move only some dependents?
- [ ] What if a sub is double-booked across two GC projects? → **Job planning v0.2:** sub conflict panel; see [job-planning-workflow.md](./technical-exploration/job-planning-workflow.md)
- [ ] Manual override always wins — how is that communicated to subs?
- [ ] Business days only vs calendar days?
- [ ] Holidays / GC blackout dates?

### Messaging

- [ ] File/photo attachments in v0.1 — **yes, images expected heavily** (blob + SQL metadata)
- [ ] Can GC @mention or tag a specific sub on a task thread?
- [ ] Message retention / export for disputes?
- [ ] Can homeowner reply via SMS or only via web? → **Resolved for v0.1:** same MMS group model as subs (Dana + customer + handle)
- [ ] SMS relay: MVP default or opt-in per thread? → **Resolved:** default per relationship when Dana invites; not opt-in
- [ ] MMS group: one handle # per project vs per thread? → **Resolved (2026-08-17):** **per-project handle #** + thread record (`conversation_sid` / `mms_thread_id`) per relationship
- [ ] Moderation — can GC delete/edit messages?

### Roles & permissions

- [ ] Multiple GC users per company (office manager + field super)?
- [ ] Can one sub user belong to multiple GCs’ projects?
- [ ] Homeowner couple — two contacts on one job?

### Portals

- [ ] Branding — GC logo/colors on homeowner link?
- [ ] Expiring magic links vs long-lived?
- [ ] What happens when project completes — archive access?

### AI

- [ ] **Deferred past v0.1** — no AI in MVP (2026-08-17 decision)
- [ ] Which model/provider? Cost per GC on free tier?
- [ ] GC must approve every AI draft before send? (likely yes)
- [ ] SMS intent → schedule action suggestions (v0.2+)

### Monetization

- [ ] Free tier limits: 1 project? No cascade? No SMS?
- [ ] Price points — validate against CF ($49) and “would pay $X for cascade + messaging”
- [ ] Annual discount?

---

## Open questions — technical (explore later)

See also: [technical-exploration/auth-and-data.md](./technical-exploration/auth-and-data.md)

- [ ] **App stack** (.NET vs Next.js vs other) — blocks auth library choice
- [ ] **Clerk vs Supabase Auth vs Auth.js** — managed vs roll-your-own
- [ ] Google Calendar: one-way export vs two-way sync — conflict resolution?
- [ ] Magic link token security, rotation, revocation (subs/homeowners)
- [ ] Account linking: same email from Google + Microsoft — auto-merge?
- [ ] Apple Sign-In: required for v0.1? ($99/yr Apple Developer)
- [ ] Enterprise SSO (SAML) — which tier / how late?
- [x] SMS/MMS provider — **Twilio default** (Telnyx spike); number pool model — see [project-handle-numbers.md](./technical-exploration/project-handle-numbers.md)
- [ ] Chargebee vs Stripe Billing — feature/cost comparison
- [ ] QBO OAuth + minimal sync scope (customer create only?)
- [ ] Hosting: Azure vs other — pairs with Entra + Azure PG path
- [ ] Multi-tenant data model — company → projects → tasks → messages

---

## Research backlog

| Topic | Why | Status |
|-------|-----|--------|
| BuilderTrend, Contractor Foreman, BuildPass | Baseline competitors | ✅ Done → [competitor-research.md](./competitor-research.md) |
| Small-operator anti-BT segment (pricing, onboarding, magic links, micro-SaaS playbook) | Market validation + positioning | ✅ Done 2026-08-14 → competitor-research.md § Small-operator segment |
| JobTread | "Affordable full suite" adjacent to BT | Not started — next priority |
| Jobber, Houzz Pro, CoConstruct | Adjacent / residential | CoConstruct → see BT section (sunset); Jobber/Houzz not started |
| Google Cloud vs Azure for hosting | Calendar API does not require GCP | ✅ google-cloud-vs-azure.md |
| Twilio SMS/MMS pricing at scale | Tier economics | ✅ Draft → [project-handle-numbers.md](./technical-exploration/project-handle-numbers.md) |
| Chargebee vs Stripe Billing | Our subscription stack | Not started |
| QBO integration patterns for construction SaaS | Post-MVP path | Not started |
| Magic link auth best practices | Sub/homeowner access | In progress → auth-and-data.md |
| Auth BYO vs native + MFA/passkeys free options | GC login model | ✅ auth-byoa-vs-native-mfa.md |
| Messaging + image attachments (blob storage) | Core comms pattern | ✅ messaging-and-media.md |
| Stack: web + API + DB | .NET API lean; frontend open | ✅ stack-web-api-db.md |
| Cascade scheduling UX in other industries | Inspiration (MS Project, Asana deps) | Not started |
| Gemini MVP roadmap (technical phases) | External architecture patterns | ✅ Done 2026-08-14 → technical-exploration/external-mvp-roadmap-review.md |

---

## Risks & assumptions

| Assumption | Risk if wrong |
|------------|----------------|
| Small GCs will pay for coordination, not full ERP | They expect invoicing in v0.1 |
| Google Calendar is universal for target GCs | Many use paper/whiteboard only |
| Homeowners will use a link | Low engagement; SMS-only insufficient |
| Cascade is the killer feature | BT already has Gantt cascade — must bundle with calendar + lightweight comms |
| "Cheaper BT" positioning | JobTread + Contractor Foreman also undercut; we're narrower + calendar-native |
| AI comms race | BT shipped weekly AI Client Updates (Jun 2025) — we need event-triggered, not digest parity |
| Free tier converts to paid | Abuse, no conversion |
| AI drafts save time | GCs don’t trust or edit anyway |

---

## Parking lot (maybe never)

- Native iOS/Android apps — **decided out** (responsive web only, 2026-08-14)
- In-app video calls
- Lien waiver / compliance document workflows
- Material ordering integrations
- CompanyCam-style photo timeline
- AI photo estimating / blueprint reading (KonstructIQ territory)
- Supplier "clipping" from Home Depot/Lowe's (Materio-style)
- T&M time-and-materials logging + receipt-to-invoice
- Embedded consumer financing in proposals (GreenSky/Nelnet-style partnerships)
- WhatsApp integration (defer until beyond US SMS-first)

---

## Session notes

### 2026-08-13

- Competitor research: three platforms analyzed; CF cheapest published, BT premium residential/selections, BuildPass AI-native expanding Win/Pay
- User vision: integrate don't replace, lightweight, subs + homeowners, cascade scheduling, messaging, tiers + free
- Chargebee clarified for **our** SaaS billing; QBO for **GC accounting** is separate future integration
- User wants extended planning phase — documentation started, no PRD yet

### 2026-08-13 (auth & data)

- GC auth: OAuth BYOA (Google, Apple, Microsoft work accounts); ContractorPro maintains user/identity records
- Subs/homeowners: magic links, not full OAuth — still tracked in DB
- DB: Postgres; free tier (Neon/Supabase) now, HA cloud Postgres (e.g. Azure Flexible Server) later
- Documented in `technical-exploration/auth-and-data.md`

### 2026-08-13 (Azure)

- Team more likely **Azure than AWS** given skillset
- Long-term lean: App Service, Azure PostgreSQL, Entra, Key Vault, Application Insights
- Dev can still use Neon/local Postgres; migrate to Azure PG for prod
- See `technical-exploration/azure-alignment.md`

### 2026-08-13 (stack)

- Architecture: **web frontend + API backend + database**
- Personal strength: **.NET**; vibe-coded / AI-assisted build OK
- API lean: **ASP.NET Core**; frontend **React vs Blazor** still open
- Auth.js deprioritized for .NET API; Entra/OpenIddict/Clerk remain options
- See `technical-exploration/stack-web-api-db.md`

### 2026-08-13 (calendar provisioning)

- Two modes per GC company: **BYO** (integrate existing Google calendars) **or Pro-provided** (app creates/holds dedicated calendar per entity)
- Aligns with auth philosophy: bring your own or we provide
- Hybrid possible (e.g. company BYO + per-project Pro-provided) — TBD

- Google Calendar does **not** require hosting on GCP
- Lean: **Azure** for .NET API + Postgres; **Google Cloud project** only for Calendar API + OAuth credentials
- Documented in `technical-exploration/google-cloud-vs-azure.md`

- **Google Calendar** = default for subs, homeowners, and most GCs personally/small biz
- **Microsoft Calendar** = mostly **work accounts**; minority use for field/subs; phase 2+ if demand
- Auth may still offer **Microsoft login** (work email) without prioritizing M365 calendar sync in MVP

- Integration goal: **dual-view** — native Google Calendar + ContractorPro project view of same data
- App manages **many shared calendars** through project workflow (ACL, not manual Google sharing)
- Cascade should update Google **events**, not just in-app tasks
- Documented in `technical-exploration/google-calendar-integration.md`

### 2026-08-13 (messaging & media)

- Expect **heavy image use** in GC / sub / homeowner messaging
- **All three roles upload images** in v0.1 — homeowners included; key to communication + project tracking
- **Azure Blob Storage** for images; SQL for message + attachment metadata only
- SMS = text + link; images in web portal
- Storage quotas may tie to subscription tiers
- See `technical-exploration/messaging-and-media.md`

### 2026-08-14 (easy join + passwordless)

- **Idea:** GC sends invite → sub/homeowner joins with **name + phone only** — no password
- Evolves magic links into lightweight **project_participant** identity (phone = key)
- Return visits: magic link in SMS or OTP to verified phone — still passwordless
- GC staff: OAuth/passkeys preferred over passwords
- Documented in [invite-join-flow.md](./technical-exploration/invite-join-flow.md)
- DB schema should include `project_participants` + `participant_sessions`

### 2026-08-14 (session wrap — documentation complete)

- Full session documented in planning hub README.md (handoff 2026-08-14)
- All research logged: BT deep dive, small-operator blueprint, Gemini technical roadmap, responsive web decision
- **Next step when returning:** database schema for v0.1 (projects, tasks, cascade deps, invitees, magic links, messaging, calendar sync)
- Prompt: *"John, let's design the v0.1 database schema"*

### 2026-08-14 (client UI strategy)

- **Decision:** No native mobile apps for MVP or foreseeable roadmap
- **Approach:** Responsive web — mobile-friendly on all devices; **GC experience desktop-first**; **sub/homeowner magic-link pages mobile-first**
- PWA optional later; offline-first deferred
- Reinforces stack lean (React + .NET API) vs Gemini Flutter/RN proposal
- Logged in product-vision.md § Client strategy, stack-web-api-db.md § Responsive web strategy

### 2026-08-14 (Gemini technical MVP roadmap)

- External 3-phase roadmap: PWA/native, serverless, AI estimating, T&M, embedded financing
- **Phase 1 validates us:** 10-min TTV, async cascade, magic-link SMS, passkeys, sub photo upload
- **Phase 2–3 rejected for MVP:** OCR estimating, web clipping, receipt pipeline, Stripe Connect client payments, embedded loans
- **Useful gleanings:** Confirm Date UX, signed short-lived magic URLs, event-driven notification bus, staged offline (v0.2+)
- **Stack conflicts:** Flutter/RN and full serverless vs .NET + React + App Service lean
- **Gemini gap:** no Google Calendar two-way — our wedge they missed
- Logged in [external-mvp-roadmap-review.md](./technical-exploration/external-mvp-roadmap-review.md)

### 2026-08-14 (small-operator anti-BT blueprint)

- External strategic analysis: micro-ops fastest-growing segment; 80% admin burden; anti-bloat playbook
- **Strong alignment:** transparent pricing, 10-min onboarding, magic-link subs, anti-bloat UX
- **Reject for MVP:** AI estimating, supplier clipping, T&M invoicing, embedded financing — scope creep toward CF/KonstructIQ
- **Our unique wedge not in blueprint:** Google Calendar two-way, GC-as-hub messaging, event-triggered AI
- **ICP refinement:** core sweet spot 2–5 person GC crews; solo specialty trades = Jobber lane
- **Strategic warning:** "direct BT competitor" framing is trap — stay coordination-layer narrow
- Logged in competitor-research.md § Small-operator anti-Buildertrend segment

### 2026-08-14 (Buildertrend deep dive)

- Aggregated user pros/cons from G2, Capterra, Software Advice, Trustpilot
- **2026 pricing shift:** published tiers removed; volume-based custom quotes ($0–499K → $31M+ brackets)
- **Cascade parity:** BT has Gantt predecessor/successor auto-shift + sub notifications — not unique to us
- **Google Calendar:** BT offers one-way iCal feed only (read-only, 30d past / 60d future) — major ContractorPro opportunity
- **Sub resistance** confirmed as structural BT weakness — profile/app required
- **AI:** Client Updates (Jun 2025) = weekly digest from platform data; Bill Pay (Feb 2026); no AI scheduling
- **Positioning refinement:** "Schedule coordination layer" not "cascade tool"; cascade is feature within bundle
- **ICP signal:** GCs who demo'd/churned BT due to price, setup, or sub adoption
- **Next competitor research:** JobTread (transparent pricing, estimating strength)
- Logged in [competitor-research.md](./competitor-research.md)

### 2026-08-13 (SMS relay exploration)

- Idea: ContractorPro as virtual group-chat member — users text natively OR use web; app logs everything
- **Cannot** inject into existing iMessage groups; practical pattern = **relay number per thread** (Twilio/ACS)
- **Not** one project megagroup — preserves GC-as-hub (separate GC↔sub and GC↔homeowner relays)
- Images still via web portal; SMS = nudge + link (group MMS too costly/unreliable)
- Suggested hybrid: web-first + SMS notify default; **opt-in relay** per conversation later
- Customer discovery Q: would GCs move existing text threads to a ContractorPro number?
- See `technical-exploration/messaging-and-media.md` § SMS relay
