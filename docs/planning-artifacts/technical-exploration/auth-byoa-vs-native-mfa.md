# Authentication — BYO Account vs Native + MFA / Passkeys

Status: **Exploratory** (2026-08-13)  
Related: [auth-and-data.md](./auth-and-data.md), [auth-comparison-managed-vs-authjs.md](./auth-comparison-managed-vs-authjs.md), [azure-alignment.md](./azure-alignment.md)

## Product intent (from stakeholder)

Same pattern as **Google Calendar BYO vs Pro-provided**:

| Mode | Description |
|------|-------------|
| **BYO account** | User signs in with **Google, Apple, Microsoft**, etc. — we use their existing identity |
| **Native account** | User **creates and manages an account in ContractorPro** (email + password, etc.) — unavoidable for some users |

> “Bring your own account and we use that, or we allow them to create and manage an account in our system. Not great, but cannot be avoided.”

Additionally: **MFA** and/or **passkey** options — what can we do **for free**?

---

## Who gets which auth path

| User | BYO OAuth | Native account | Magic link | MFA / passkey |
|------|-----------|----------------|------------|---------------|
| **GC staff** (paying) | ✅ Primary | ✅ Fallback | — | ✅ Expected for native; optional for BYO |
| **GC staff** | Google / Apple / Microsoft | Email + password in our system | — | TOTP, passkey, (not SMS if avoiding cost) |
| **Sub / homeowner** | — | ❌ Never | ✅ Join + SMS magic link return | N/A |

**Native accounts** are mainly for GC users who won’t or can’t use social/work OAuth — not for every invitee.

---

## BYO (federated) sign-in

| Provider | Typical user | Notes |
|----------|--------------|-------|
| **Google** | Most subs, many GCs | Pairs with Google Calendar BYO |
| **Apple** | iOS users | Apple Developer $99/yr for Sign in with Apple |
| **Microsoft** | Office / work email GCs | Entra / personal Microsoft account |

**Our DB still stores:** `users`, `auth_identities` (provider + subject id), `company_memberships`, roles.

**Account linking:** Same person signs in with Google once, Microsoft later — merge or prompt to link (open question).

---

## Native accounts (ContractorPro-managed)

Minimum viable native auth:

- Email + password registration
- **Email verification** (required)
- Forgot / reset password
- Lockout after failed attempts
- Store password hash only (ASP.NET Identity PasswordHasher — free)
- Optional: **passkey** registration (passwordless or second factor)
- Optional: **TOTP MFA** (Google Authenticator, Authy, Microsoft Authenticator)

**Not great** because: we own security, support (“I forgot my password”), breach surface, compliance narrative. **Unavoidable** because not everyone uses Google/Apple/Microsoft.

---

## MFA & passkeys — what’s “free”?

### Free (no per-user SaaS fee)

| Method | Cost | Notes |
|--------|------|-------|
| **TOTP MFA** (authenticator app) | **$0** | Built into ASP.NET Core Identity; also Entra External ID |
| **Passkeys (WebAuthn/FIDO2)** | **$0** software | Browser + device (Face ID, Windows Hello, security key); no per-auth fee |
| **Email OTP** (sign-in or MFA) | **~$0** at low volume | Resend/SendGrid free tiers; you send codes |
| **Email magic link** (invitees) | **~$0** at low volume | Already planned for subs/homeowners |

### Not free (avoid as default MFA)

| Method | Cost | Notes |
|--------|------|-------|
| **SMS MFA / SMS OTP** | **Paid** | Twilio ~$0.01+/SMS; Entra External ID SMS is **paid add-on** per message |
| **Clerk MFA + passkeys** | **Pro $25/mo** | **Not on Hobby free tier** |
| **Auth0 MFA factors** | Often paid tier | Check current plans |

**Recommendation for cost-conscious MVP:** Offer **TOTP** + **passkeys**; skip **SMS MFA** for GC login (SMS reserved for schedule/message **notifications** to subs/homeowners, not MFA).

---

## Option comparison (.NET + Azure lean)

### A — ASP.NET Core Identity (self-hosted, roll your own)

| Feature | Free? |
|---------|-------|
| Email + password native accounts | ✅ |
| Google / Apple / Microsoft OAuth | ✅ (add handlers or Identity external login) |
| TOTP MFA | ✅ Built-in |
| Passkeys | ✅ **.NET 10+** built into Identity ([docs](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/passkeys/)) |
| Magic links for invitees | ✅ Custom (you build) |
| Auth SaaS bill | **$0** |
| You own | Security patches, UX, rate limits, account recovery |

**Passkeys in .NET 10:** `SignInManager` / `UserManager` — no third-party fee. Limitations: scoped to Identity auth scenarios; advanced FIDO2 → optional `fido2-net-lib` (also free, OSS).

---

### B — Microsoft Entra External ID (CIAM)

| Feature | Free? |
|---------|-------|
| Native email + password | ✅ |
| Social IdPs (Google, Apple, Microsoft) | ✅ |
| **First 50,000 MAU** | ✅ [Free tier](https://learn.microsoft.com/en-us/entra/external-id/external-identities-pricing) |
| **Passkeys** | ✅ **Included, no extra cost** ([docs](https://learn.microsoft.com/en-us/entra/external-id/customers/how-to-sign-in-with-passkey)) |
| TOTP / authenticator MFA | ✅ |
| **SMS MFA** | ❌ **Paid add-on** (per SMS by country) |
| Azure alignment | ⭐⭐⭐ |
| .NET integration | `Microsoft.Identity.Web` |

**Note:** Entra passkeys for **local email+password users** today; federated users on roadmap per Microsoft docs.

---

### C — Clerk

| Feature | Hobby (free) | Pro ($25/mo) |
|---------|--------------|--------------|
| OAuth social | ✅ (up to 3) | ✅ |
| Email + password | ✅ | ✅ |
| **MFA** | ❌ | ✅ |
| **Passkeys** | ❌ | ✅ |
| .NET API JWT validation | ✅ | ✅ |

**Free tier does NOT include MFA or passkeys** — production security pushes you to **$25/mo** minimum.

---

### D — Supabase Auth

| Feature | Free tier |
|---------|-----------|
| OAuth, email/password | ✅ |
| MFA (TOTP) | ✅ on free (verify current docs) |
| Passkeys | Check current — less mature than Entra |
| .NET fit | Weak (Node-centric admin) |

---

## Draft matrix for ContractorPro

| Requirement | Identity (self) | Entra External ID | Clerk |
|-------------|-----------------|-------------------|-------|
| BYO Google/Apple/Microsoft | ✅ | ✅ | ✅ |
| Native email accounts | ✅ | ✅ | ✅ |
| **Free MFA (TOTP)** | ✅ | ✅ | ❌ Hobby |
| **Free passkeys** | ✅ .NET 10 | ✅ | ❌ Hobby |
| Free at 50k users | ✅ | ✅ MAU | ✅ MRU (no MFA) |
| Azure + .NET skillset | ✅ | ✅✅ | ⭐ |
| Magic link invitees | Custom | Custom / email OTP | Custom |

**Draft lean (not final):** **Entra External ID** or **ASP.NET Core Identity** maximize “free” MFA/passkeys on .NET/Azure. **Clerk** free tier is insufficient if MFA/passkeys are required at launch.

---

## Suggested user flows

### GC onboarding

```
Sign up / Sign in
├── Continue with Google
├── Continue with Apple
├── Continue with Microsoft
└── Create account with email
         ├── verify email
         ├── set password
         └── optional: add passkey or TOTP (encouraged for native accounts)
```

### Native account security (recommended defaults)

| Control | Native account | BYO OAuth |
|---------|----------------|-----------|
| Email verification | Required | Trust IdP |
| MFA / passkey | **Encouraged**; required on Pro tier? | Optional (IdP may already have MFA) |
| SMS MFA | Skip (cost) | — |

### Invitee (sub/homeowner)

- Magic link — **no** full native account, **no** MFA burden
- Short-lived session; project-scoped access

---

## Data model addition

```
users
  id, email, email_verified, display_name, ...

auth_identities
  user_id, provider (google|apple|microsoft|local|magic_link)
  provider_subject_id, password_hash? (only if local)

user_mfa
  user_id, type (totp|passkey), credential_data, created_at

passkeys / webauthn_credentials  -- if using Identity .NET 10 schema
```

---

## Open questions

- [ ] **Require MFA** for native accounts at launch, or “strongly encouraged”?
- [ ] **Require passkey** for Pro tier GCs?
- [ ] Entra External ID vs self-hosted Identity — spike on Azure trial?
- [ ] Apple Sign-In — $99/yr worth it for v0.1?
- [ ] Can BYO user add **our** passkey on top of Google login (step-up)?
- [ ] Account recovery if passkey-only user loses device?
- [ ] Sub/homeowner: ever offer optional Google connect for calendar only?

---

## Relation to calendar BYO

| | Calendar | Auth |
|--|----------|------|
| **BYO** | Link existing Google calendars | Sign in with Google/Apple/Microsoft |
| **We provide** | App-created Google calendars | Native email account in our system |
| **Both valid** | Per company setting | Per user choice at login |

Log decisions in [discovery-log.md](../discovery-log.md).
