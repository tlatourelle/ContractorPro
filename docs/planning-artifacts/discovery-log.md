# Discovery Log

Chronological and thematic capture of ideas, questions, and decisions. **Add to this document** as exploration continues.

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
| 2026-08-13 | Wedge = cheaper, simpler, integrate-don't-replace + AI-forward | Not a BT clone or full CF feature parity |
| 2026-08-13 | Signature feature = optional schedule cascade | Trickles changes to dependent tasks |
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

---

## Ideas backlog

- **Cascade preview** — before applying, show GC which tasks/subs/homeowner notifications will fire
- **“What changed” feed** — homeowner-friendly timeline of schedule shifts (not full Gantt)
- **SMS relay / virtual group member** — ContractorPro number joins SMS threads; users text natively OR use web; app logs everything (see messaging-and-media.md § SMS relay)
- **Per-sub task slice** — sub portal shows only their trades/phases
- **Project templates** — common residential remodel phases pre-wired with dependencies (later)
- **AI weekly digest** for GC — “here’s what moved, who hasn’t read messages” (later)
- **Read receipts / opened link tracking** — did homeowner see the delay notice?
- **Quiet hours** for SMS — respect GC and homeowner preferences
- **Project photo timeline** — unified project tracking view aggregating images from GC, subs (GC-visible), and homeowner

---

## Open questions — product

### Google Calendar

- [ ] MVP: BYO only, Pro-provided only, or both at launch?
- [ ] Pro-provided: calendar under GC's Google vs ContractorPro service account?
- [ ] Hybrid mode (company BYO + per-project Pro-provided) — when?
- [ ] Sub calendar access: OAuth connect vs email ACL invite only?
- [ ] Two-way sync: what happens if sub drags event in Google?
- [ ] Homeowner: Google invite vs app-only?
- [ ] Google OAuth verification timeline for sensitive calendar scopes?

### Cascade scheduling

- [ ] Fixed duration vs fixed end date when cascading?
- [ ] Partial cascade — move only some dependents?
- [ ] What if a sub is double-booked across two GC projects? (out of scope v0.1?)
- [ ] Manual override always wins — how is that communicated to subs?
- [ ] Business days only vs calendar days?
- [ ] Holidays / GC blackout dates?

### Messaging

- [ ] File/photo attachments in v0.1 — **yes, images expected heavily** (blob + SQL metadata)
- [ ] Can GC @mention or tag a specific sub on a task thread?
- [ ] Message retention / export for disputes?
- [ ] Can homeowner reply via SMS or only via web? → **Explore SMS relay** (virtual 3rd party per thread)
- [ ] SMS relay: MVP default or opt-in per thread? Cost model per tier?
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

- [ ] Which model/provider? Cost per GC on free tier?
- [ ] GC must approve every AI draft before send? (likely yes)
- [ ] What data can AI see — full project or redacted sub info in homeowner drafts?

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
- [ ] SMS provider: Twilio, AWS SNS, other — cost model per tier
- [ ] Chargebee vs Stripe Billing — feature/cost comparison
- [ ] QBO OAuth + minimal sync scope (customer create only?)
- [ ] Hosting: Azure vs other — pairs with Entra + Azure PG path
- [ ] Multi-tenant data model — company → projects → tasks → messages

---

## Research backlog

| Topic | Why | Status |
|-------|-----|--------|
| BuilderTrend, Contractor Foreman, BuildPass | Baseline competitors | ✅ Done → [competitor-research.md](./competitor-research.md) |
| Jobber, Houzz Pro, CoConstruct | Adjacent / residential | Not started |
| Google Cloud vs Azure for hosting | Calendar API does not require GCP | ✅ google-cloud-vs-azure.md |
| Twilio SMS pricing at scale | Tier economics | Not started |
| Chargebee vs Stripe Billing | Our subscription stack | Not started |
| QBO integration patterns for construction SaaS | Post-MVP path | Not started |
| Magic link auth best practices | Sub/homeowner access | In progress → auth-and-data.md |
| Auth BYO vs native + MFA/passkeys free options | GC login model | ✅ auth-byoa-vs-native-mfa.md |
| Messaging + image attachments (blob storage) | Core comms pattern | ✅ messaging-and-media.md |
| Stack: web + API + DB | .NET API lean; frontend open | ✅ stack-web-api-db.md |
| Cascade scheduling UX in other industries | Inspiration (MS Project, Asana deps) | Not started |

---

## Risks & assumptions

| Assumption | Risk if wrong |
|------------|----------------|
| Small GCs will pay for coordination, not full ERP | They expect invoicing in v0.1 |
| Google Calendar is universal for target GCs | Many use paper/whiteboard only |
| Homeowners will use a link | Low engagement; SMS-only insufficient |
| Cascade is the killer feature | Nice-to-have; messaging alone might be wedge |
| Free tier converts to paid | Abuse, no conversion |
| AI drafts save time | GCs don’t trust or edit anyway |

---

## Parking lot (maybe never)

- Native iOS/Android apps
- In-app video calls
- Lien waiver / compliance document workflows
- Material ordering integrations
- CompanyCam-style photo timeline
- Xactimate / insurance restoration

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

### 2026-08-13 (SMS relay exploration)

- Idea: ContractorPro as virtual group-chat member — users text natively OR use web; app logs everything
- **Cannot** inject into existing iMessage groups; practical pattern = **relay number per thread** (Twilio/ACS)
- **Not** one project megagroup — preserves GC-as-hub (separate GC↔sub and GC↔homeowner relays)
- Images still via web portal; SMS = nudge + link (group MMS too costly/unreliable)
- Suggested hybrid: web-first + SMS notify default; **opt-in relay** per conversation later
- Customer discovery Q: would GCs move existing text threads to a ContractorPro number?
- See `technical-exploration/messaging-and-media.md` § SMS relay
