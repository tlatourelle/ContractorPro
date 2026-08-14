# Invite & Join Flow — Easy Onboarding for Subs and Homeowners

Status: **Exploratory** (2026-08-14)  
Related: [product-vision.md](../product-vision.md), [auth-byoa-vs-native-mfa.md](./auth-byoa-vs-native-mfa.md), [auth-and-data.md](./auth-and-data.md), [schedule-confirmation-workflow.md](./schedule-confirmation-workflow.md)

## Product intent (from stakeholder)

> Team member sends an invite to a subcontractor or customer. Their join process is basic info — phone #, name, and that's it. Work toward **passwordless login** whenever possible for system ease of use.

This is the **anti-Buildertrend sub onboarding** play: no profile setup, no app download, no password to remember.

---

## Design principle: passwordless by default

| User type | Auth model | Password? |
|-----------|------------|-----------|
| **GC staff** | OAuth BYOA (Google/Apple/Microsoft) or passkey | ❌ Preferred — passkeys over passwords where native account needed |
| **GC staff fallback** | Email + password + TOTP/passkey | ⚠️ Only when OAuth unavailable |
| **Sub / homeowner** | **Phone + name join** → SMS magic link for every return visit | ❌ Never |

**Rule:** If a user would need to remember a password, we failed the UX — except GC native-account fallback.

---

## Join flow (v0.1 target)

### 1. GC sends invite

GC team member (or owner) from project screen:

- Enters **name** + **phone** (required) and optionally **email**
- Selects role: **Subcontractor** or **Homeowner**
- For subs: optionally assign trade / link to tasks now or later
- For subs: **notify via** — `SMS` | `Email` | `Both` (default from company settings; see [schedule-confirmation-workflow.md](./schedule-confirmation-workflow.md))
- Taps **Send invite**

System sends **SMS** (primary) with short link; email optional if provided.

```
[GC Company] invited you to the Maple St remodel.
Tap to join: https://app.contractorpro.com/join/abc123
```

### 2. Invitee lands on join page (mobile-first)

Single screen — **no account creation wizard**:

| Field | Required | Notes |
|-------|----------|-------|
| **Name** | ✅ | Pre-filled if GC entered it; invitee confirms/edits |
| **Phone** | ✅ | Pre-filled from invite; verify via SMS code **or** trust invite token |
| **Email** | Optional | For email notifications if they prefer |
| **Notify via** | Subs only | `sms` \| `email` \| `both` — schedule proposals and confirmations (see [schedule-confirmation-workflow.md](./schedule-confirmation-workflow.md)) |

Tap **Join project** → done. No password field. Ever.

### 3. Session established

- Create `project_participant` record (name, phone, role, project scope)
- Issue **session cookie** + store device trust token (optional, scoped to project)
- Redirect to participant home: my tasks, messages, schedule slice

### 4. Return visits (passwordless re-auth)

When session expires or they open a new notification link:

| Method | When | Cost |
|--------|------|------|
| **Signed magic link in SMS** | Every notification (schedule change, new message) | ~$0.01/SMS — already budgeted |
| **SMS OTP** ("enter 6-digit code") | User navigates to bookmarked URL without fresh link | ~$0.01/SMS |
| **Trusted device cookie** | Same phone/browser within TTL (e.g. 30 days) | Free |

**Recommendation:** Magic link in every outbound SMS doubles as re-auth — minimize separate OTP flows. Bookmarked return without SMS = send OTP to verified phone.

---

## What we store (identity model)

Invitees are **not** full `users` with passwords. They are **project participants**:

```
project_participants
  id
  project_id
  role                    -- subcontractor | homeowner
  display_name
  phone_e164              -- verified
  email                   -- optional
  notify_via              -- sms | email | both (subs; schedule notifications)
  invited_by_user_id
  joined_at
  phone_verified_at
  status                  -- invited | joined | revoked

participant_sessions
  id
  participant_id
  token_hash              -- magic link or session
  expires_at
  device_fingerprint      -- optional
  created_at
```

Same person on **two GC projects** = two participant records (or link by phone later — v0.2).

Same person as **sub on Project A** and **homeowner on Project B** (rare) = separate records; phone can match.

---

## GC team invites (internal)

GC staff are **not** part of this flow — they use OAuth/passkey (see auth-byoa-vs-native-mfa.md).

Future: GC office manager invites another GC user via email → OAuth sign-in. Still passwordless if they use Google/Microsoft.

---

## Security notes

| Concern | Mitigation |
|---------|------------|
| Invite link forwarded to wrong person | Join requires **phone verification** (OTP or match invited phone) |
| Stolen magic link | Short TTL on sensitive actions; phone verify for first join |
| Session hijack | HttpOnly secure cookies; scoped to participant + project |
| Spam invites | GC tier limits; rate limit invites per project |
| Revoked sub still has link | Invalidate sessions on revoke; links return "access removed" |

---

## UX details

- **Pre-fill from GC entry** — GC types "Mike's Electric" + phone; Mike only confirms
- **One screen** — not a 5-step wizard; target **under 60 seconds** on phone
- **Large touch targets** — glove-friendly; camera for photos on next screen
- **No "create account" language** — say **"Join [Project Name]"** not "Sign up"
- **Confirm Date** — available immediately after join on sub portal; full propose → accept → sync flow in [schedule-confirmation-workflow.md](./schedule-confirmation-workflow.md)

---

## Comparison to prior "magic link only" plan

| Before | Now (refined) |
|--------|---------------|
| Anonymous magic-link session | **Named participant** with phone identity |
| No explicit join step | Explicit **join** screen (name + phone confirm) |
| Return via any magic link | Return via magic link **or** OTP to verified phone |
| "No accounts for invitees" | **Lightweight accounts** — no password, phone = identity |

We still avoid Buildertrend-style "create a profile with username and password."

---

## Open questions

- [ ] Phone verify on join: **trust invite token** vs **always SMS OTP**?
- [ ] Session TTL: 7 days vs 30 days vs until project complete?
- [ ] Can invitee edit name after join?
- [ ] GC pre-fills name — can invitee change it, or locked?
- [ ] Email-only invite path (no phone) — defer or support?
- [ ] Re-invite same phone to same project — merge or error?
- [ ] Participant sees **all their projects** from one phone login (v0.2)?

---

## Discovery questions

1. When a GC texts you a project link, would you enter **just name + phone** to join — or is that still too much?
2. Would you rather get a **new text link** each time or **one bookmark** you open all week?
3. Is verifying your phone with a **6-digit code** on first join acceptable?

See [customer-discovery.md](../customer-discovery.md).

Log decisions in [discovery-log.md](../discovery-log.md).
