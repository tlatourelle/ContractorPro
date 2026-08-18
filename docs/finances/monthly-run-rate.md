# Monthly Run Rate — Operating Budget

Status: **Living document** (started 2026-08-18)  
Owner: Thomas  
Currency: **USD**

Use this doc to answer: *"What does it cost to keep ContractorPro running each month?"* Separate **fixed run rate** (pay even with zero customers) from **variable COGS** (scales with usage).

---

## Quick totals (planning defaults)

| Phase | Fixed / mo | Variable (example) | Notes |
|-------|------------|-------------------|--------|
| **Pre-launch (build)** | ~$35–80 | ~$0–20 | Dev Azure, domain, tools; no customer traffic |
| **MVP live, 0 paying customers** | ~$50–100 | ~$0 | Prod stack idle; minimal telco |
| **Early traction (10 GCs × 3 projects)** | ~$60–120 | ~$300+ | Telco dominates variable |

**Planning default for telco per active project:** **~$10/mo** (see [Communications](#communications-twilio--mms)).

---

## Fixed monthly run rate

Costs you pay regardless of how many messages or customers (within a tier).

| Line item | Planning est. / mo | Annual (if easier) | Status | Notes |
|-----------|-------------------|--------------------|--------|-------|
| **Domain** (`contractorpro.com` or similar) | ~$1–2 | ~$12–20/yr | TBD | Registrar TBD (Cloudflare, Namecheap, etc.) |
| **DNS** | $0 | — | TBD | Often included with domain or Cloudflare free tier |
| **Azure — dev** | ~$0–25 | — | TBD | App Service free/B1, small PG or Neon free for dev |
| **Azure — production** | ~$30–60 | — | TBD | App Service B1/S1 + Azure PostgreSQL burstable — [google-cloud-vs-azure.md](../planning-artifacts/technical-exploration/google-cloud-vs-azure.md) |
| **Azure Blob Storage** | ~$1–5 | — | TBD | Hot tier; low until MMS photo volume grows |
| **Azure Key Vault** | ~$0–1 | — | TBD | Secrets (Google OAuth, Twilio) |
| **Application Insights / monitoring** | ~$0–10 | — | TBD | Free tier often enough pre-launch |
| **Google Cloud project** | $0 | — | Planned | Calendar API + OAuth only — no GCP hosting |
| **Transactional email** | $0–20 | — | TBD | Resend / SendGrid / Azure Comm — magic links, invites |
| **Auth** | $0 | — | Lean | ASP.NET Identity + passkeys = $0; Clerk Pro = $25/mo if chosen — [auth-byoa-vs-native-mfa.md](../planning-artifacts/technical-exploration/auth-byoa-vs-native-mfa.md) |
| **Billing platform** | $0 + % | — | TBD | Stripe Billing or Chargebee — platform fee + % of revenue |
| **Source control / CI** | $0–4 | — | TBD | GitHub free or Team |
| **Error tracking** (optional) | $0 | — | TBD | Sentry free tier |
| **Apple Developer** (if Sign in with Apple) | ~$8 | $99/yr | Open | Required only if shipping Apple Sign-In |
| **10DLC campaign** (Twilio) | ~$2–10 | — | TBD | Per **company** brand/campaign; amortize across customers — see communications |

### Fixed subtotal (planning)

| Scenario | / month |
|----------|---------|
| **Lean pre-launch** | **~$35–50** |
| **Prod-ready MVP** | **~$50–80** |
| **With Clerk + paid email** | **~$80–120** |

---

## Variable COGS (scales with customers & usage)

These are **cost of goods sold** — budget per customer or per active project, not flat monthly.

### Communications (Twilio / MMS)

| Metric | Planning default | Source |
|--------|------------------|--------|
| **Per active project / month** | **~$10** | Buffer over typical ~$4–7; heavy photo/chat jobs higher |
| **Per contractor company / month** | **~$5** overhead | 10DLC campaign, shared platform telco admin |
| **Formula** | `(active_projects × $10) + ($5 × paying_companies)` | |

Example: **1 contractor, 5 active projects** → `5 × $10 + $5` = **~$55/mo** telco COGS.

Detail: [project-handle-numbers.md](../planning-artifacts/technical-exploration/project-handle-numbers.md)

| Included in ~$10/project | Typical range |
|--------------------------|---------------|
| Number rental (10DLC) | ~$1.15 |
| Group MMS (subs + customer threads) | ~$2–6 |
| System SMS/MMS (confirm, poke, cascade) | ~$0.50–2 |
| Carrier passthrough headroom | buffer |

**Not included:** Azure Blob egress for MMS photos (usually small per project unless photo-heavy).

### Payment processing

| Item | Rate | Notes |
|------|------|-------|
| Stripe / Chargebee | ~2.9% + $0.30 per charge | On **revenue**, not run rate — model in unit economics when pricing tiers |
| Stripe Billing (if used) | +0.5–0.7% | Optional add-on |

### Other variable

| Item | When it hits | Planning est. |
|------|--------------|---------------|
| Blob egress / CDN | High MMS photo volume | ~$0–20/mo early |
| Google Calendar API | MVP scale | $0 |
| AI / vision APIs | v0.2+ | Deferred |

---

## One-time & periodic spend (not monthly run)

Track separately; amortize mentally or in a spreadsheet if useful.

### Brand, design, copy

| Item | Planning est. | Timing | Status | Notes |
|------|---------------|--------|--------|-------|
| **Logo** | $0–500 | Pre-launch | TBD | DIY / Fiverr / designer |
| **Brand kit** (colors, type — partial in [DESIGN.md](../planning-artifacts/ux-designs/ux-ContractorPro-2026-08-18/DESIGN.md)) | $0–1,500 | Pre-launch | In progress | UX mockups exist; production brand TBD |
| **Marketing site copy** | $0–1,000 | Pre-launch | TBD | Landing page, feature pages |
| **In-app microcopy** | $0 (internal) | MVP | TBD | SME-driven; UX spec exists |
| **Legal** (Privacy Policy, Terms, TCPA/SMS consent) | $0–1,500 | Before prod SMS | TBD | Template vs attorney |
| **Google OAuth verification** | Time, not $ | Before prod Calendar | TBD | Process cost |

### Advertising & growth (optional)

| Channel | Planning est. / mo | Status | Notes |
|---------|-------------------|--------|-------|
| **Paid search** (Google Ads) | $0–500+ | Deferred | Validate wedge with SMEs first |
| **Facebook / local trade groups** | $0–200 | Deferred | Organic + small tests |
| **Content / SEO** | $0 (time) | Deferred | Blog, comparison pages |
| **Trade show / local GC meetups** | Variable | Deferred | |

**Pre-revenue default:** budget **$0/mo** advertising until product + 3–5 design partners validate.

---

## Scenario worksheets

### A — Solo build (now → MVP)

| Category | / month |
|----------|---------|
| Azure dev + minimal prod | $35–60 |
| Domain + DNS | ~$2 |
| Email (free tier) | $0 |
| Auth (Identity) | $0 |
| Telco (testing only) | $5–20 |
| **Total** | **~$40–80** |

### B — MVP live, design partners (3 GCs, ~2 projects each)

| Category | / month |
|----------|---------|
| Fixed run (prod stack) | $60–80 |
| Telco: 6 projects × $10 + 3 × $5 | ~$75 |
| **Total** | **~$135–155** |

### C — Early paid (10 GCs, ~3 active projects each)

| Category | / month |
|----------|---------|
| Fixed run | $70–100 |
| Telco: 30 projects × $10 + 10 × $5 | ~$350 |
| **Total** | **~$420–450** |

At **$49/mo** subscription (CF-comparable), 10 customers = **$490/mo revenue** → telco alone is ~70% of revenue at this usage. **Tier limits on active projects and MMS** are required for margin. See PRD FR-18 / monetization open questions.

---

## Tracking actuals

| Month | Fixed actual | Variable actual | Notes |
|-------|--------------|-----------------|-------|
| 2026-08 | — | — | Budget doc created |
| | | | |

Replace planning estimates with invoice amounts as vendors are chosen.

---

## Open decisions

- [ ] Domain name + registrar
- [ ] Auth: Identity ($0) vs Clerk ($25/mo)
- [ ] Billing: Stripe Billing vs Chargebee
- [ ] Email provider
- [ ] Twilio vs Telnyx (spike) — affects per-project COGS
- [ ] Free tier: max active projects + MMS cap
- [ ] Paid tier price point ($29–79 range to validate)
- [ ] Logo / legal: DIY vs vendor

---

## Changelog

| Date | Change |
|------|--------|
| 2026-08-18 | Initial budget; **$10/mo per active project** telco planning default |
