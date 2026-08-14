# Auth Approach Comparison — Managed vs Auth.js + Postgres

Status: **Exploratory** (2026-08-13)  
Context: [auth-and-data.md](./auth-and-data.md)

## What ContractorPro needs from auth

| Need | GC staff | Subs / homeowners |
|------|----------|-------------------|
| Login method | OAuth BYOA (Google, Apple, Microsoft) | Magic link (email/SMS) |
| Our DB owns | Users, companies, roles, projects | Invite tokens, access, messages |
| Integrations | Google Calendar OAuth (related) | Twilio SMS |
| Scale path | 10 → 10k users | Many invitees, light sessions |
| Team size now | Small / solo build | — |

---

## Option 1 — Managed auth (Clerk **or** Supabase Auth)

Third party handles sign-in UI, OAuth flows, sessions, MFA, and much of security hygiene. Your app stores business data in Postgres and links records via `user_id` / webhooks.

### Clerk

| Pros | Cons |
|------|------|
| Excellent DX — prebuilt `<SignIn />`, user profile, orgs (could map to GC companies) | **Cost at scale** — Pro ~$25/mo+; enterprise SSO extra |
| Google, Apple, Microsoft, social out of the box | Another vendor; auth logic lives outside your repo |
| **Organizations** feature fits multi-user GC companies well | Magic link / passwordless possible but **subs/homeowners flow may still be custom** |
| Webhooks sync user lifecycle to your DB | Migration off Clerk later = planned work (not impossible) |
| Session/JWT handling, CSRF, secure cookies handled | Pricing can jump if invitees count as MAU (check: magic link users) |
| Good docs, fast MVP for GC login | |
| No Postgres required for auth itself | |

**Clerk + Neon Postgres** is a common split: Clerk = identity, Neon = app data.

### Supabase Auth

| Pros | Cons |
|------|------|
| **Auth + Postgres in one platform** — `auth.users` + your `public` schema | Tighter coupling — auth and DB vendor same basket |
| Generous free tier for hobby/MVP | **RLS complexity** — powerful but learning curve; easy to get wrong |
| Google, Apple, Azure, GitHub, magic link / OTP **built-in** | Supabase Auth less polished than Clerk for **multi-tenant B2B** (orgs) |
| Row Level Security can enforce tenant isolation in DB | Self-hosting Supabase possible but ops-heavy |
| Realtime, storage available if needed later | Enterprise SSO / SAML on higher tiers |
| Magic link email built-in (helpful for invitees) | Apple Sign-In still needs Apple Developer setup |
| Open source core — less pure lock-in than Clerk | JWT in localStorage vs cookie patterns need care on web |

**Supabase** = fastest “one dashboard” MVP if entire stack lives there.

### Managed — shared pros

- Security patches, OAuth provider churn (Google API changes) mostly **their problem**
- MFA, bot protection, session rotation without building it
- Apple / Microsoft OAuth wiring done for you
- **Ship GC login in days**, not weeks

### Managed — shared cons

- **Invitee auth ambiguity** — subs/homeowners via SMS magic link may not fit MAU pricing or product model; often **custom tokens anyway**
- **Account linking** (Google + Microsoft same person) — supported but vendor-specific rules
- **Google Calendar OAuth** is a **second** OAuth consent (Calendar scopes ≠ Sign-In) — true for all options
- Debugging auth issues across vendor + your app
- Compliance / data residency: user auth metadata on vendor infrastructure

---

## Option 2 — Auth.js + Postgres

[Auth.js](https://authjs.dev) (formerly NextAuth) is a library — **you** host the auth routes, session store, and user tables. Postgres holds everything via an adapter (Prisma, Drizzle, etc.).

| Pros | Cons |
|------|------|
| **Identity data in your Postgres from day one** — `users`, `accounts`, `sessions` tables you own | **You build more** — sign-in pages, error states, email templates |
| No per-MAU auth vendor fee | **You own security** — session fixation, CSRF, cookie flags, rate limits |
| Easy to add **custom magic link** flow for subs/homeowners in same codebase | Apple Sign-In + Microsoft require manual provider config |
| **Portable** — swap hosting; auth code in repo | MFA / passkeys — more work or add plugin |
| Natural fit if stack is **Next.js** (first-class support) | .NET has no Auth.js — would use **OpenIddict / Identity** instead (different doc) |
| Webhooks not required to sync users — already in DB | Enterprise SAML later = you integrate or add Keycloak/etc. |
| Chargebee/Stripe user id = your `users.id` — simple | Session store at scale needs Redis or DB session table tuning |
| Full control over **account merge** logic | Provider outages — you handle error UX |

| Pros (continued) | Cons (continued) |
|------------------|------------------|
| One less SaaS bill | Magic link email delivery — you wire Resend/SendGrid |
| Auditable schema for “track auth on our side” requirement | Testing burden — OAuth flows in CI are annoying |
| Works with **any** Postgres host (Neon, Azure, RDS) | |

---

## Side-by-side matrix (ContractorPro lens)

| Criterion | Clerk | Supabase Auth | Auth.js + Postgres |
|-----------|-------|---------------|-------------------|
| **Time to GC OAuth MVP** | ⭐⭐⭐ Fastest | ⭐⭐⭐ Fastest | ⭐⭐ Slower |
| **Magic link for invitees** | ⭐⭐ Custom likely | ⭐⭐⭐ Built-in email OTP | ⭐⭐⭐ You design it |
| **“We own user records”** | ⭐⭐ Via sync/webhook | ⭐⭐⭐ Same DB | ⭐⭐⭐ Native |
| **GC company / multi-user** | ⭐⭐⭐ Organizations | ⭐⭐ DIY in schema | ⭐⭐⭐ DIY in schema |
| **Cost at low scale** | ⭐⭐⭐ Free tier | ⭐⭐⭐ Free tier | ⭐⭐⭐ Only infra |
| **Cost at 10k+ MAU** | ⭐⭐ Paid | ⭐⭐ Paid / limits | ⭐⭐⭐ Infra only |
| **Vendor lock-in** | ⭐⭐ Medium | ⭐⭐ Medium-high | ⭐⭐⭐ Low |
| **Postgres host flexibility** | ⭐⭐⭐ Any | ⭐ Tied to Supabase | ⭐⭐⭐ Any |
| **Azure / Entra path later** | ⭐⭐ SAML on paid | ⭐⭐ Enterprise features | ⭐⭐⭐ OpenIddict / Entra |
| **Security burden on you** | ⭐ Low | ⭐ Low–medium | ⭐⭐⭐ High |
| **Next.js fit** | ⭐⭐⭐ | ⭐⭐⭐ | ⭐⭐⭐ Best |
| **Non-Next stack** | ⭐⭐⭐ | ⭐⭐⭐ | ⭐ Varies |

---

## Clerk vs Supabase (if managed wins)

| Pick **Clerk** if… | Pick **Supabase** if… |
|--------------------|------------------------|
| You want best-in-class sign-in UI and **Organizations** for GC teams | You want **one platform** (auth + DB + maybe storage) |
| App DB is Neon/Azure and you want auth separate | You’re happy on Supabase Postgres for MVP |
| You’re okay paying for auth as you grow | Magic link email for invitees without extra service |
| B2B SaaS patterns (roles, invites) matter | You’ll use RLS for tenant isolation |

---

## The magic-link wrinkle (important)

**Subs and homeowners** probably won’t use Clerk/Supabase sign-in widgets. You’ll likely build:

- `project_invites` table + hashed token
- SMS/email with link
- Short-lived session cookie after click

That’s **custom in all three approaches**. Managed auth mainly helps **GC staff** who log in repeatedly. Don’t choose Supabase *only* for invitee magic links — Auth.js can do that equally well.

---

## Free tier — Clerk & Supabase Auth

Both are **third-party vendors** with **$0 plans**. Verify on [clerk.com/pricing](https://clerk.com/pricing) and [supabase.com/pricing](https://supabase.com/pricing) before deciding.

| | Clerk Hobby | Supabase Free |
|--|-------------|---------------|
| **Price** | $0 | $0 |
| **Auth quota** | 50,000 **MRU**/app (returns 24h+ after signup) | 50,000 **MAU** |
| **Card required** | No | No |
| **Commercial use** | Yes | Yes |
| **Database included** | No — bring Neon/Azure/etc. | Yes — 500 MB Postgres, 2 projects |
| **Prod on free** | Viable for GC auth | ⚠️ DB pauses after ~7d inactivity |
| **Paid step-up** | Pro ~$25/mo | Pro ~$25/mo |

**Auth.js + Neon:** $0 library + $0 Neon free tier — no auth SaaS bill; you own more security/ops.

**No decision** on auth approach — this is reference only.

---

## Cost sketch (rough, 2026 — verify before deciding)

| Scenario | Clerk | Supabase | Auth.js + Neon |
|----------|-------|----------|----------------|
| Dev / solo | $0 | $0 | $0 (Neon free) |
| 500 GC users + 2k invitees/mo | Check MAU definition for invitees | Pro ~$25/mo | Neon free/low + email/SMS |
| 5k MAU | Clerk Pro $ | Supabase Pro + overages | Neon + app hosting |

**Action:** Before choosing Clerk, confirm whether **one-time magic-link visitors** count as MAU.

---

## Decision tree

```
Are you building with Next.js (App Router)?
├─ No  → Clerk or Supabase (or .NET Identity) — Auth.js less relevant
└─ Yes
    ├─ Want fastest GC login, okay with vendor? 
    │   ├─ Need orgs/teams UI → Clerk + Neon
    │   └─ Want all-in-one → Supabase
    └─ Want max ownership, comfortable with auth security?
        → Auth.js + Drizzle/Prisma + Neon
```

---

## Tentative recommendation for ContractorPro

| Phase | Suggestion | Why |
|-------|------------|-----|
| **Still planning / no stack** | **Don’t decide yet** — pick stack first | Auth.js is Next-centric |
| **Next.js app** | **Auth.js + Neon** if you’re experienced; **Clerk + Neon** if you want speed | Ownership vs velocity |
| **Supabase** | Good if you accept platform coupling for MVP | Magic links + Postgres together |
| **.NET / Blazor** | Skip Auth.js → **OpenIddict** or **Entra External ID** + Postgres | Different ecosystem |

**For your stated goal** (“track authentication on our side” + grow over time):

1. **Auth.js + Postgres** aligns best philosophically — nothing to sync, full schema control.
2. **Clerk + Postgres** is the best managed compromise if auth security isn’t your favorite problem.
3. **Supabase** wins only if you want the **integrated** MVP and will migrate carefully if you outgrow it.

---

## Open decisions after this comparison

- [ ] Confirm **web framework** (blocks Auth.js)
- [ ] Clerk MAU policy for magic-link invitees
- [ ] Email provider for Auth.js magic links (Resend, SendGrid)
- [ ] Whether **Clerk Organizations** maps 1:1 to GC `companies` table
- [ ] Spike: 1-day prototype with top candidate

---

## Next step

Run a **1-day throwaway spike**: Google sign-in + create `users` row + one magic-link invite flow — twice if needed (Clerk vs Auth.js). The right choice is often obvious after touching both.
