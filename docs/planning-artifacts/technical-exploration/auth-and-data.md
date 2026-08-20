# Technical Exploration — Auth & Database

> **Superseded for implementation:** [architecture-v0.1.md](../architecture-v0.1.md) §4 (2026-08-20). MVP = Entra External ID (Google only) + BFF cookie + magic links.

Status: **Exploratory** (2026-08-13)  
Related: [product-vision.md](../product-vision.md), [discovery-log.md](../discovery-log.md)

## Requirements (from product direction)

### Authentication

| Requirement | Notes |
|-------------|-------|
| **Bring your own account (BYOA)** | Google, Apple, Microsoft work accounts, etc. via OAuth/OIDC |
| **We track identity on our side** | User records, sessions, roles, company membership — not “auth-only, no database” |
| **Multiple auth paths** | GC staff = full OAuth; subs/homeowners = magic link (email/SMS) — still tracked in our DB |
| **Account linking** | Same person might use Google today, Microsoft tomorrow — optional but valuable |
| **Small now, enterprise later** | Work SSO (Entra ID / Okta) may matter for larger GCs eventually |

### Database

| Requirement | Notes |
|-------------|-------|
| **Low cost at start** | Free or near-free for solo dev / MVP |
| **Grows with product** | HA, backups, read replicas, connection pooling without rewrite |
| **Relational fit** | Projects, tasks, dependencies, messages, permissions — natural fit for Postgres |
| **US data** | US-only product for now; region pinning matters later |

---

## Recommended mental model

```
┌─────────────────────────────────────────────────────────────┐
│  Identity providers (they authenticate)                      │
│  Google · Apple · Microsoft · (later) SAML/OIDC enterprise   │
└──────────────────────────┬──────────────────────────────────┘
                           │ OAuth 2.0 / OIDC
┌──────────────────────────▼──────────────────────────────────┐
│  Auth layer (you choose one — see options below)             │
│  Issues session / JWT · validates tokens · optional MFA        │
└──────────────────────────┬──────────────────────────────────┘
                           │
┌──────────────────────────▼──────────────────────────────────┐
│  ContractorPro database (you own this)                       │
│  users · auth_identities · companies · memberships ·         │
│  sessions · projects · tasks · messages · invites              │
└─────────────────────────────────────────────────────────────┘
```

**Key idea:** OAuth proves *who they are*; your database records *who they are to ContractorPro* (role, company, projects). Never treat the IdP as your only user store.

---

## Two auth populations (don’t conflate)

| Population | Auth method | Why |
|------------|-------------|-----|
| **GC staff** (paying users) | OAuth BYOA (Google, Apple, Microsoft, …) | Repeat login, calendar integration, trust |
| **Subs & homeowners** (invitees) | **Magic link** via email/SMS | No account setup friction; may never create Google/Apple login |

Both end up as rows in **`users`** (or `people`) with different `auth_type` / `role`. GC might link OAuth; homeowner might only ever have `magic_link` + phone/email.

---

## OAuth / identity provider options

### Option A — Managed auth platform (recommended for small team)

| Service | Free tier (approx.) | Social + enterprise | You own user DB |
|---------|---------------------|---------------------|-----------------|
| **[Clerk](https://clerk.com)** | **Hobby $0** — 50k MRU/app ([pricing](https://clerk.com/pricing)) | Google, Apple, Microsoft, SAML on paid | Yes — sync via webhooks + your tables |
| **[Auth0](https://auth0.com)** | ~7k MAU free | Broad IdP list, enterprise SSO | Yes — same pattern |
| **[Supabase Auth](https://supabase.com)** | **Free** — 50k MAU ([pricing](https://supabase.com/pricing)) | Google, Apple, Azure, etc. | Built-in `auth.users` + your public schema |
| **Firebase Auth** | Free tier | Google, Apple, Microsoft | Firebase UID → map in your DB |

**Pros:** Apple Sign-In, Microsoft, MFA, session security handled; fast MVP.  
**Cons:** Vendor lock-in for auth flows; cost at scale; enterprise SSO often paid tier.

### Option B — Auth library + your API (more control)

| Library | Stack | Notes |
|---------|-------|-------|
| **[Auth.js (NextAuth)](https://authjs.dev)** | Next.js, others | OAuth providers built-in; sessions in **your** DB (adapter) |
| **ASP.NET Core Identity + OpenIddict** | .NET | Full control if stack is C# |

**Pros:** Identity data lives in your Postgres from day one; portable.  
**Cons:** You implement account linking, email verification, magic links, rate limits.

### Option C — Azure / Microsoft path (growth to work accounts)

| Service | When |
|---------|------|
| **[Microsoft Entra External ID](https://learn.microsoft.com/en-us/entra/external-id/)** (formerly Azure AD B2C) | GCs on Microsoft 365; enterprise SSO later |
| **Entra ID** social + local accounts | Heavier setup; strong if you’re all-in Azure |

**Pros:** Natural for “work Microsoft account” GCs; enterprise path.  
**Cons:** More config than Clerk/Supabase; overkill for earliest MVP unless team is Azure-native.

### Free tiers — Clerk & Supabase (third-party vendors)

Both have **$0 plans** suitable for development and early MVP. **No auth vendor decision yet.**

| | [Clerk Hobby](https://clerk.com/pricing) | [Supabase Free](https://supabase.com/pricing) |
|--|------------------------------------------|-----------------------------------------------|
| **Cost** | $0, no card required | $0 |
| **Auth quota** | 50,000 **MRU**/app (user returns 24h+ after signup) | 50,000 **MAU** |
| **Commercial use** | Allowed on Hobby | Allowed |
| **Includes** | OAuth UI, sessions, basic orgs | Auth + **500 MB Postgres** + 2 projects |
| **Prod caveats** | Pro ~$25/mo for advanced features | Free DB **pauses after ~7d idle**; no PITR |
| **ContractorPro note** | Likely **GC staff only**; invitee magic links may stay custom | Auth + DB bundled — couples vendors |

See full comparison: [auth-comparison-managed-vs-authjs.md](./auth-comparison-managed-vs-authjs.md).

---

## What to store on your side (minimum schema sketch)

Not implementation — planning reference.

```
users
  id, email, phone, display_name, created_at, status

auth_identities          -- links external IdP to user (GC staff)
  user_id, provider (google|apple|microsoft|magic_link)
  provider_subject_id, email_at_provider, last_login_at

companies                -- GC tenant
  id, name, subscription_tier, ...

company_memberships
  company_id, user_id, role (owner|admin|member)

sessions                 -- if not fully delegated to auth vendor
  id, user_id, expires_at, ...

project_invites          -- subs/homeowners
  project_id, user_id OR email/phone, role (sub|homeowner)
  token_hash, expires_at, last_accessed_at
```

**Rules:**
- One `user` can have multiple `auth_identities` (linked accounts).
- Subscription/billing ties to `company`, not IdP.
- Magic links: store **hashed** token, single-use or short TTL.

---

## Database options

**No host decision yet.** Working assumption: **PostgreSQL** engine. Full exploration:

→ **[database-options.md](./database-options.md)** — Neon, Supabase, Railway, Render, Azure PG, RDS, free tiers, scale path.

**Avoid for core data (general guidance):** SQLite-only in prod, Firebase as primary relational store, switching SQL engines mid-flight.

---

## Pairing auth + database (common patterns)

| Pattern | Auth | DB | Fit for ContractorPro |
|---------|------|-----|------------------------|
| **Supabase stack** | Supabase Auth | Supabase Postgres | Fastest MVP; magic links + RLS possible |
| **Clerk + Neon** | Clerk | Neon Postgres | Clean split; Clerk webhooks sync user id |
| **Auth.js + Neon** | Auth.js | Neon Postgres | Max ownership; more build |
| **Azure app + Entra + Azure PG** | Entra External ID | Azure PostgreSQL | Long-term Microsoft shop |

---

## Open questions (auth & data)

- [ ] **Stack preference?** **.NET API** (lean); React vs Blazor for frontend — see stack-web-api-db.md
- [ ] **Clerk vs Entra External ID vs OpenIddict** — Auth.js poor fit for .NET API
- [ ] **Apple Sign-In** — requires Apple Developer account ($99/yr); required for iOS credibility?
- [ ] **Same email, two providers** — auto-merge or ask user to link?
- [ ] **GC invites office staff** — email invite + OAuth on first login?
- [ ] **Magic link only for subs/homeowners** — confirm no OAuth for invitees in v0.1
- [ ] **Session length** — GC stays logged in 30 days; homeowner link 7 days?
- [ ] **Data residency** — US East/West only when on Azure?
- [ ] **ORM / migrations** — Prisma, Drizzle, EF Core — decide with stack

---

## Risks

| Risk | Mitigation |
|------|------------|
| Auth vendor price jump at scale | Abstract `user_id`; don’t store business logic in vendor metadata only |
| Free DB tier sleeps or limits connections | Pooler + paid tier before launch to paying customers |
| Over-building auth before product validation | Ship Google + magic links first; add Apple/Microsoft in v0.2 |
| Sub/homeowner magic link abuse | Rate limit, short TTL, hashed tokens, audit log |

---

## Next exploration steps

1. Pick **app stack** (drives auth library choice) — separate decision
2. **Spike:** Google OAuth + one `users` / `auth_identities` table + magic link flow (throwaway)
3. Compare **Clerk vs Supabase** pricing at 100 / 1k / 10k MAU
4. Document **Google Calendar OAuth scopes** — may share Google Cloud project with Sign-In (plan consent screen)

---

## Status — no decisions

| Area | Direction | Decision |
|------|-----------|----------|
| Protocol | OAuth 2.0 / OIDC for GC BYOA | Exploring |
| GC login | Clerk vs Supabase Auth vs Auth.js | **Undecided** |
| Invitee login | Magic link (email/SMS) | Likely; not final |
| Database engine | PostgreSQL | Likely; not final |
| Database host | Neon vs Supabase vs Azure PG, etc. | **Undecided** |
| Identity storage | Our tables + optional auth vendor id | Requirement |

Log decisions in [discovery-log.md](../discovery-log.md) when made.
