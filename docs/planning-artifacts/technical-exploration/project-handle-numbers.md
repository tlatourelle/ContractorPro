# Project Handle Numbers — Vendor, Pooling & Cost Model

> **Superseded (2026-08-20)** by [company-number-messaging.md](./company-number-messaging.md) — one company # per contractor + SMS relay. Retained for historical context only.

Status: **Superseded** — was exploratory → tentative decision (2026-08-18)  
Related: [messaging-and-media.md](./messaging-and-media.md), [stack-web-api-db.md](./stack-web-api-db.md), [discovery-log.md](../discovery-log.md)

## Summary

ContractorPro v0.1 uses **one phone number per active project** — the **project handle** — as a virtual participant in native group MMS threads (Dana + sub/customer + handle #). Numbers are **not** owned by the GC's personal phone; they are **JIT-provisioned** from a **per-company pool** via a CPaaS vendor.

**Tenant isolation (locked 2026-08-19):** A number **never** moves from one Contractor company to another. Cross-tenant reuse is forbidden — old customers texting a handle must never land on another GC's project.

**Reuse (locked 2026-08-19, updated 2026-08-20):** After archive, hold the number in **`cooling`** still routing inbound to the closed project; default **90 days** (platform setting, per-contractor override). MVP: then **release** to Twilio (no reuse). v0.1.1: may return to same company's available pool.

**Churn (locked 2026-08-19):** On unsubscribe / leave → **release all numbers to CPaaS immediately**. DB retains threads and media; returning contractor gets **new** numbers.

**Google Voice is not viable** (no programmatic API, no webhook ingestion, no A2P 10DLC). **Azure Communication Services does not support group MMS** in v0.1 — keep ACS for voice/email later if needed.

**Leading vendor:** Twilio (Conversations API or Programmable Messaging + group MMS). **Alternative to spike:** Telnyx (native group MMS endpoint, often cheaper).

**Architecture:** CPaaS + ContractorPro .NET API + Azure Blob — not a texting SaaS product, not roll-your-own carrier.

---

## Product model (locked)

| Concept | Rule |
|---------|------|
| Handle assignment | One number per **project**, not per contractor or per thread |
| Threads | One group MMS per **relationship** (Dana + Marcus + Maple#, Dana + Lauren + Maple#) |
| Inbound routing | `To` = handle # → `project_id`; `From` = phone → `membership_id` |
| Outbound | System messages (confirm links, pokes) sent into thread or 1:1 from handle |
| Media | MMS photos ingested via webhook → Azure Blob; metadata in SQL |

See [messaging-and-media.md](./messaging-and-media.md) § MMS group threads for full UX and data model.

---

## Number pool & reuse

GCs run multiple projects over time but only need numbers for **concurrent active projects** plus numbers in **cooling** after close. While a Contractor **remains subscribed**, numbers stay in that company's pool — **never reassigned to another Contractor**.

**Churn exception (locked 2026-08-19):** On unsubscribe / account closure, **release all numbers to the CPaaS vendor immediately** (skip cooling). Message threads, photos, and project data **remain in our DB** (and blob storage per retention policy). If the Contractor returns, they get **new numbers** — no reattachment of old E.164s.

### Lifecycle

**Normal (subscribed contractor):**
```
JIT buy → assigned (project active) → cooling (archived, default 90d) → released (MVP) or available (v0.1.1+ reuse)
```

**Churn (unsubscribe / leave):**
```
any state → released (Twilio deprovision immediately) — DB history retained; company inactive
```

**Abuse / carrier:**
```
→ retired (never reuse internally; document reason)
```

| State | Meaning |
|-------|---------|
| `available` | In **this company's** pool; never used or cooling complete; ready for next project |
| `assigned` | Bound to `projects.handle_phone_e164`; active job |
| `cooling` | Project archived; number still provisioned; **inbound routes to archived project**; not assigned to new project; duration from `cooling_until` (default **90 days** — see below) |
| `released` | Deprovisioned at CPaaS; `e164` no longer ours; historical record only in `phone_number_assignments` |
| `retired` | Internal flag — do not re-buy this E.164 if ever seen again; compliance/abuse |

**While subscribed:** numbers never move from Company A to Company B.

**On churn:** numbers leave our platform entirely — not held in a global pool for reassignment to other Contractors.

### JIT provisioning

- **When:** On project create (MVP — E2-S1) — reserve/buy from CPaaS if company pool has no `available` number
- **Not:** Pre-buy numbers at company signup (avoids telco cost on sandbox/free tier companies)
- **Phase 2 option:** Defer buy until first outbound comms / sub invite — saves numbers on plan-only sandbox projects

### Cooling period (configurable)

| Level | Storage | Default |
|-------|---------|---------|
| **Platform** | `platform_settings.phone_cooling_days_default` | **90 days** |
| **Contractor** | `contractors.phone_cooling_days` | `NULL` → inherit platform default |

On archive: `cooling_until = archived_at + effective_days` (snapshot — later platform/contractor changes do not alter in-flight cooling).

Admin (Thomas) can raise default globally or per tenant for warranty-heavy GCs; lowering below 30d not recommended without product review.

### Why 90-day default (MVP)

Homeowners and subs may text after punch list ("warranty question"). **90 days** balances post-close support vs telco rent. Original exploration used 180d; **MVP default is 90d** — configurable per contractor and globally.

**Cost tradeoff:** Riverside with 5 active jobs and ~10 completions/year may hold **~5 active + ~5 cooling** ≈ 10 numbers (~$11.50/mo rent) for the cooling window; 90d vs 180d halves average cooling hold time.

### Why tenant-scoped pool only

If Maple St's `(555) 100-0001` is ever assigned to **another Contractor's** job, Lauren's old group text hits the wrong company — trust and TCPA nightmare. **Numbers never cross `company_id`.**

### Same-company reuse — collision to manage

Even within one GC, after cooling ends, reassigning `(555) 100-0001` from Maple St → Oak Ave means **Marcus's old group MMS** (Ryan + Marcus + handle) may send to a number now tied to Oak Ave.

**Mitigations (pick for TRD):**

1. **Assignment history routing (recommended):** Keep `phone_number_assignments` history. Inbound: route by `(to_e164, from_phone)` — if sender was a member of a **cooling or recent archived** project on this number, deliver to that project (even if number is now assigned elsewhere); else route to current assigned project.
2. **Reuse gate:** Only move `available` → new project if **zero inbound** during entire cooling window.
3. **Conservative v0.1:** No reuse in MVP — number stays on archived project indefinitely; company always buys fresh. Simplest; higher rent. Revisit when pool cost hurts.

### Pool sizing

```
pool_numbers ≈ peak_concurrent_active + ceil(completions_per_cooling_window)
```

Example: 5 active, 10 jobs closed per year, 90d cooling → ~5 + ~3 = **~8 numbers** in rotation at steady state (MVP: released after cooling, not reused).

### Data sketch

```text
phone_number_pool
  id, company_id, e164, status, current_project_id?,
  released_at, cooling_until, provider_sid, created_at
  -- company_id NEVER changes; number never moves to another company

phone_number_assignments   -- history for inbound routing
  id, phone_number_id, project_id, assigned_at, released_at, release_reason

projects
  id, company_id, handle_phone_e164, status, archived_at, ...
```

### Churn — immediate number release

**Trigger:** Subscription canceled / account closed (Stripe `customer.subscription.deleted`, admin offboard, or messaging_suspended → full closure).

| Action | Rule |
|--------|------|
| CPaaS numbers | **Release all** for `company_id` immediately — assigned, cooling, and available |
| Inbound after release | No longer ours; Lauren's text to old handle goes to void / future unrelated Twilio buyer — **expected** |
| App data | Projects, messages, attachments metadata **retained** per retention policy; blob media retained |
| Contractor return | Reactivate company or new signup → **JIT new numbers**; show historical threads read-only from DB |
| Old E.164 on projects | Keep on `projects.handle_phone_e164` as **historical display only** (`released_at` set); not routable |

**Why immediate release:** No ongoing ~$1.15/mo/number rent for churned tenants. Product value after leave is in **stored comms**, not live SMS routing.

**Customer expectation:** On voluntary cancel, optional email: *"Project texts to old project numbers will no longer reach [Company]. Contact them directly."* — Phase 2 copy.

**Contrast with archive cooling:** Closed **project** while still subscribed → cooling (default 90d, configurable), inbound works. Closed **account** → instant telco release.

### Inbound during cooling (archived project — still subscribed)

| Inbound | Behavior |
|---------|----------|
| MMS/SMS to handle | Ingest to **archived** project thread; notify Ryan in-app ("Message on closed Maple St") |
| System auto-reply (optional) | *"Maple St is complete. Riverside will see your message."* |
| New outbound system SMS | Blocked unless Ryan reopens project or forwards manually |

### Open product questions

- [x] Default cooling period → **90 days** (platform default); per-contractor override + global `platform_settings` — 2026-08-20 (was 180d exploration default)
- [x] Cross-contractor reuse → **Never while number is on platform**
- [x] Churn → **Immediate CPaaS release**; DB history kept; return = new numbers — 2026-08-19
- [x] Same-company reuse in MVP → **No reuse v0.1** — cooling then release; history routing v0.1.1 — 2026-08-20
- [ ] Contact card label: **"Maple St · ContractorPro"** vs **"Maple St project line"** — affects sub trust (green bubble)

---

## Why not Google Voice?

| Requirement | Google Voice | ContractorPro |
|-------------|--------------|---------------|
| MMS for humans | ✅ (manual, in app) | ✅ |
| Programmatic send/receive | ❌ No public API | ✅ Required |
| Inbound webhooks (ingest group MMS) | ❌ | ✅ Core v0.1 |
| A2P 10DLC compliance | ❌ | ✅ Required |
| Virtual group participant | ❌ | ✅ Product thesis |

Google Voice is a ~$10–20/user/month phone app, not telco infrastructure. Unofficial scrapers/browser automation are brittle and non-compliant — do not use.

---

## Vendor comparison

### Twilio — default recommendation

| | |
|---|---|
| **Group MMS** | ✅ Conversations API (classic group texting); up to 10 participants |
| **10DLC long code** | ~$1.15/mo per number |
| **MMS (US)** | ~$0.022 outbound, ~$0.0165 inbound per segment + carrier fees |
| **SMS (US)** | ~$0.0083 per segment each way + carrier fees |
| **Conversations API** | $0.05/MAU after first 200 free users/month |
| **10DLC registration** | Brand ~$4.50 one-time; campaign $1.50–10/mo |
| **.NET SDK** | `Twilio` NuGet package |
| **Pros** | Best docs, mature webhooks, already referenced in stack docs |
| **Cons** | Higher list price; Conversations adds MAU fee |

### Telnyx — cost challenger (spike before commit)

| | |
|---|---|
| **Group MMS** | ✅ `/v2/messages/group_mms` — single API call |
| **Max participants** | 8 + sender (we need 3: Dana + sub + handle) |
| **MMS** | ~$0.015/msg part + carrier passthrough |
| **Pros** | Simpler group MMS API; often 30–70% cheaper at volume; no Conversations MAU |
| **Cons** | Smaller ecosystem; validate webhook reliability in spike |

### Azure Communication Services — not for v0.1 group MMS

ACS supports 1:1 SMS/MMS but **not native group MMS**. Using ACS here would still require Twilio/Telnyx for the core field-comms workflow. Keep ACS aligned for future voice/email if useful.

### Others

Plivo, Bandwidth — viable CPaaS; no clear advantage over Twilio/Telnyx for group MMS in MVP.

**Decision process:** 2-day engineering spike — provision number → create group MMS → receive webhook → store media in blob. Compare Twilio vs Telnyx on reliability and integration effort. Pick one vendor for v0.1.

---

## Architecture

```
┌─────────────────────────────────────────────────────────┐
│  Dana's phone (native Messages)                          │
│  Group: Dana + Marcus + (555) 100-0001                  │
└────────────────────────┬────────────────────────────────┘
                         │ carrier MMS
┌────────────────────────▼────────────────────────────────┐
│  CPaaS (Twilio / Telnyx)                                 │
│  • phone numbers, group MMS delivery                     │
│  • inbound webhooks, outbound API                        │
└────────────────────────┬────────────────────────────────┘
                         │ HTTPS
┌────────────────────────▼────────────────────────────────┐
│  ContractorPro ASP.NET Core API                          │
│  • pool provision / assign / release / cooling           │
│  • route To/From → project + membership                  │
│  • store messages; MMS media → Azure Blob                │
│  • send system messages (confirm, poke)                  │
└─────────────────────────────────────────────────────────┘
```

### What we build vs what vendor provides

| We own | Vendor owns |
|--------|-------------|
| Number pool logic, project routing | Phone numbers, carrier connectivity |
| Thread/membership mapping | MMS protocol, delivery |
| Message storage, ACLs, blob media | 10DLC registration plumbing (we file; they route) |
| Business logic (pokes, cascade notify) | Webhook delivery |

### Libraries & integration

| Layer | Choice |
|-------|--------|
| API | ASP.NET Core webhook controller + signature validation |
| SDK | `Twilio` NuGet or Telnyx REST via `HttpClient` |
| Media | Download MMS URLs from webhook payload → Azure Blob |
| Compliance | 10DLC brand + campaign via vendor portal (one-time + monthly) |

No special "MMS library" — REST + webhooks + blob storage.

---

## Unit economics (Twilio, US, rough)

Numbers are cheap; **message volume drives cost**.

### Per active project (illustrative month)

| Line item | Estimate |
|-----------|----------|
| Number rental (1× 10DLC) | ~$1.15 |
| Group MMS traffic (~50 in, ~30 out) | ~$2–4 |
| System SMS/MMS (proposes, pokes, confirms) | ~$0.50–1 |
| **Subtotal per active project** | **~$4–7/mo** (typical) · **~$10/mo** (planning default) |

**Planning default for budgets:** use **~$10/mo per active project** — see [monthly-run-rate.md](../../finances/monthly-run-rate.md).

### Per contractor company (5 concurrent projects)

| Line item | Estimate |
|-----------|----------|
| Number pool (5 numbers) | ~$5.75/mo |
| Messaging (5 projects × above) | ~$15–30/mo |
| 10DLC campaign (platform, shared) | ~$1.50–10/mo |
| **Rough telco COGS** | **~$22–46/mo** |

Plus Azure Blob egress for MMS photos (separate; see messaging-and-media.md).

**Pricing implication:** Unlimited MMS on a low subscription tier is not sustainable. Tier limits should cap active projects and/or included message volume.

### Pool vs no-pool (numbers only)

| Model | 5 active + 15 completed/year | Monthly number rent |
|-------|-------------------------------|---------------------|
| Never release | 20 numbers | ~$23/mo |
| Pool with reuse | 5–7 numbers | ~$6–8/mo |

---

## Compliance notes

- **A2P 10DLC** required for application-generated US SMS/MMS on long codes
- **Model (locked 2026-08-20):** **Platform brand + campaign** — ContractorPro registers once in Twilio Trust Hub; all tenant handle #s use platform campaign (not per-GC brands in MVP)
- **Opt-in** when participant added to group (copy in onboarding + C-13 modal)
- **TCPA** — business messaging consent; document in terms
- Handle # appears as **green bubble** on iPhone (not iMessage blue) — set expectations in SME review

Pre-launch checklist: [architecture-v0.1.md](../architecture-v0.1.md) §10 (PL-1–PL-3).

---

## MVP engineering checklist

- [ ] **10DLC brand + campaign** — **platform-level** (ContractorPro) in Twilio Trust Hub — see [architecture-v0.1.md](../architecture-v0.1.md) §1.8, §10 PL-1–PL-3
- [ ] Number search/buy/release API integration
- [ ] Pool service: assign on project create, release on archive, **release all on churn** — **E8-S4**
- [ ] Inbound webhook: parse MMS, map To/From, dedupe
- [ ] Media pipeline: webhook → blob → `message_attachments`
- [ ] Outbound: system messages into group thread
- [ ] Spike: Twilio vs Telnyx group MMS end-to-end

---

## References

- [Twilio US SMS/MMS pricing](https://www.twilio.com/en-us/sms/pricing/us)
- [Twilio Conversations group texting](https://www.twilio.com/docs/conversations-classic/group-texting)
- [Telnyx group messaging](https://developers.telnyx.com/docs/messaging/messages/group-messaging)
- [Azure Communication Services SMS pricing](https://learn.microsoft.com/en-us/azure/communication-services/concepts/sms-pricing)

Log updates in [discovery-log.md](../discovery-log.md).
