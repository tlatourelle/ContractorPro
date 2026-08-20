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
| **Transactional email** | **$0** (Resend free tier) | — | **Resend** | Magic links, invites, pokes; ~3k/mo free — [architecture-v0.1.md](../planning-artifacts/architecture-v0.1.md) §1.5 |
| **Auth — Entra External ID (CIAM)** | **$0** | — | **Locked** | GC team-member Google sign-in via External tenant — see [Entra External ID (CIAM)](#entra-external-id-ciam--team-member-auth) below |
| **Billing platform** | $0 + % | — | **Stripe Billing** | Checkout + Customer Portal; ~0.7% billing + 2.9% + $0.30 card |
| **Source control / CI** | $0–4 | — | TBD | GitHub free or Team |
| **Error tracking** (optional) | $0 | — | TBD | Sentry free tier |
| **Apple Developer** (if Sign in with Apple) | ~$8 | $99/yr | Open | Required only if shipping Apple Sign-In |
| **10DLC campaign** (Twilio) | ~$2–10 | — | Platform | One brand/campaign for all tenants — amortize across customers — see [architecture-v0.1.md](../planning-artifacts/architecture-v0.1.md) §1.8 |

### Fixed subtotal (planning)

| Scenario | / month |
|----------|---------|
| **Lean pre-launch** | **~$35–50** |
| **Prod-ready MVP** | **~$50–80** |
| **With paid email tier** | **~$80–120** |

---

## Entra External ID (CIAM) — team member auth

**Decision (2026-08-20):** GC team members sign in with **Google OAuth via Microsoft Entra External ID** (CIAM External tenant). Subs/customers use **magic links** in our app — they do **not** count toward Entra MAU. Platform admin (Thomas) uses **workforce Entra** in a separate tenant/plane — not CIAM pricing below.

Reference: [architecture-v0.1.md](../planning-artifacts/architecture-v0.1.md) §4 · [Microsoft Entra External ID pricing](https://www.microsoft.com/en-us/security/pricing/microsoft-entra-external-id)

### Trial vs post-trial

| Phase | Azure subscription required? | Entra cost | Notes |
|-------|------------------------------|------------|-------|
| **30-day External ID free trial** | **No** | **$0** | Create via [Entra admin center](https://entra.microsoft.com) → Manage tenants → Create → **External** → free trial. Eval/dev only; one trial per user account. |
| **After trial (production path)** | **Yes** — link any Azure subscription | **$0** at MVP scale | Upgrade trial tenant or create External tenant with subscription. Subscription alone costs **$0/mo** if no Azure resources deployed. |
| **Local dev** | Same as above for real OAuth QA | **$0** | `localhost` redirect URIs work — **no Azure hosting required** to develop auth. Optional **DevAuth** bypass in Development (story 1.1). |

Post-trial is **not** a step-change in auth pricing — it is **attaching a subscription** to keep the CIAM tenant. Real spend after launch is **Azure hosting + Twilio**, not Entra MAU, at our scale.

Docs: [Create external tenant](https://learn.microsoft.com/en-us/entra/external-id/customers/how-to-create-external-tenant-portal) · [Free trial setup](https://learn.microsoft.com/en-us/entra/external-id/customers/quickstart-trial-setup)

### MAU billing model

**MAU** = unique users who **authenticate to the External CIAM tenant** in a calendar month (each user counted once per month).

| User type | Counts toward Entra MAU? |
|-----------|--------------------------|
| GC **team members** (Ryan, Maci) — Google via Entra | **Yes** |
| Subs / customers — magic links only | **No** |
| Platform admin — workforce Entra | **No** (separate tenant) |

| Tier | Price | Notes |
|------|-------|-------|
| **Entra External ID Basic — first 50,000 MAU / month** | **$0** | Core CIAM (social sign-in, user flows). Sufficient for years at our scale. |
| **Above 50,000 MAU** | **~$0.03 / MAU** | Confirm in [Azure pricing calculator](https://azure.microsoft.com/pricing/calculator/) for your offer. Example: 60,000 MAU → ~$300/mo Entra only. |

**Add-ons we are not using in MVP:** SMS phone auth via Entra (metered per country); Entra ID Governance for External Identities (~$0.75/MAU). Google OAuth federation itself is **$0** at MVP scale.

### Planning scenarios (Entra line item only)

| Scenario | Team-member MAU / mo | Entra / mo |
|----------|----------------------|------------|
| Solo build (Thomas) | 1–2 | **$0** |
| 3 design-partner GCs × 2 people | ~6 | **$0** |
| 10 GCs × 2 team members | ~20 | **$0** |
| 100 GCs × 2 | ~200 | **$0** |
| 1,000 GCs × 2 | ~2,000 | **$0** |
| 25,000 GCs × 2 | ~50,000 | **$0** (free tier ceiling) |

**Takeaway:** Budget **$0/mo for Entra** until tens of thousands of active GC logins per month. Pre-launch checklist **PL-7**: Entra External ID production tenant + prod redirect URIs.

### Post-trial monthly picture (full stack)

| Phase | Azure sub | Entra MAU | Azure hosting | Telco | Typical total |
|-------|-----------|-----------|---------------|-------|---------------|
| **Build locally** (Docker Postgres, no App Service) | $0 (optional free account) | $0 | $0 | $5–20 test | **~$7–22** + domain |
| **Prod-ready MVP, 0 customers** | $0 min charge | $0 | ~$50–80 | ~$0 | **~$50–80** |
| **Design partners** (3 GCs, 6 projects) | — | $0 | ~$60–80 | ~$75 | **~$135–155** |

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
| Entra External ID (CIAM) | $0 |
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
- [x] Auth: **Entra External ID (CIAM)** — Google via External tenant; $0 under 50k MAU (2026-08-20)
- [ ] Entra: upgrade trial to subscription-backed tenant before day 30 (or when moving past eval)
- [x] Billing: **Stripe Billing** (Phase 2) — Chargebee deferred (2026-08-19)
- [x] Email: **Resend** (2026-08-20)
- [ ] Twilio vs Telnyx (spike) — affects per-project COGS
- [ ] Free tier: max active projects + MMS cap
- [ ] Paid tier price point ($29–79 range to validate)
- [ ] Logo / legal: DIY vs vendor

---

## Changelog

| Date | Change |
|------|--------|
| 2026-08-18 | Initial budget; **$10/mo per active project** telco planning default |
| 2026-08-20 | **Entra External ID (CIAM)** section — trial vs post-trial, MAU model, 50k free tier, local dev without Azure hosting; locked auth/billing/email open decisions |
