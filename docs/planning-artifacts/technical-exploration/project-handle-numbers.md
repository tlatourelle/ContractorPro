# Project Handle Numbers — Vendor, Pooling & Cost Model

Status: **Exploratory → tentative decision** (2026-08-18)  
Related: [messaging-and-media.md](./messaging-and-media.md), [stack-web-api-db.md](./stack-web-api-db.md), [discovery-log.md](../discovery-log.md)

## Summary

ContractorPro v0.1 uses **one phone number per active project** — the **project handle** — as a virtual participant in native group MMS threads (Dana + sub/customer + handle #). Numbers are **not** owned by the GC's personal phone; they are provisioned from a **per-company pool** via a CPaaS vendor and recycled when projects complete.

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

GCs run multiple projects over time but only need numbers for **concurrent active projects**, not every job ever completed.

### Lifecycle

```
available → assigned (project active) → cooling (project archived) → available
```

| State | Meaning |
|-------|---------|
| `available` | In pool; ready for next project |
| `assigned` | Bound to `projects.handle_phone_e164` |
| `cooling` | Released after archive; not reassigned yet (default 30–90 days) |
| `retired` | Permanently removed (compliance, abuse, carrier issue) |

### Why cooling matters

If Maple St's `(555) 100-0001` is immediately reassigned to a new job while Marcus still has the old group on his phone, his texts land on the wrong project. Cooling reduces collision risk.

### Pool sizing

```
pool_size ≈ peak_concurrent_active_projects × 1.2
```

Example: 5 active jobs, ~15 completions/year → **5–7 numbers** in rotation, not 15.

### Data sketch

```text
phone_number_pool
  id, company_id, e164, status, assigned_project_id?,
  released_at, cooling_until, provider_sid, created_at

projects
  id, company_id, handle_phone_e164, ...
```

### Open product questions

- [ ] Default cooling period: 30, 60, or 90 days?
- [ ] Allow GC to **retain** a number on archived projects (read-only) vs always recycle?
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
| 10DLC campaign (shared per company) | ~$1.50–10/mo |
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
- **Opt-in** when participant added to group (copy in onboarding + C-13 modal)
- **TCPA** — business messaging consent; document in terms
- Handle # appears as **green bubble** on iPhone (not iMessage blue) — set expectations in SME review

---

## MVP engineering checklist

- [ ] 10DLC brand + campaign registration (per company or platform — TBD)
- [ ] Number search/buy/release API integration
- [ ] Pool service: assign on project create, release on archive
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
