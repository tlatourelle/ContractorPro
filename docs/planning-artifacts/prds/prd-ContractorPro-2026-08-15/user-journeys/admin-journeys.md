# App / Site Admin Journeys — Internal Ops Review

**Status:** Draft — validate with founder / future ops before build  
**Persona:** **Thomas** (founder / super-admin) or **Alex** (future support ops — lookup, trace, data fixes; no billing or kill switches)  
**Related:** [Contractor journeys](./contractor-journeys.md) · [Subcontractor journeys](./subcontractor-journeys.md) · [Customer journeys](./customer-journeys.md) · [Full detail](../user-journeys.md) · [Backlog](./backlog.md)

**Not the same as contractor-admin:** **Ryan** (GC owner) manages *his company* — subscription, settings, projects. **App/Site Admin** manages *the platform* — all tenants, telco, billing overrides, abuse, infra health.

**Design principles:**
- v0.1 reality: **Thomas is all roles** — design for one operator with an audit log, not a full support desk
- **Minimal admin UI for MVP** — A-1, A-2, A-3, A-4, A-11, A-14 ship first; rest can live in runbooks + Stripe/Twilio/Azure consoles until volume demands UI
- Every destructive or impersonation action **writes an audit entry** (who, when, tenant, reason)
- Subs and customers have **no passwords** — admin recovery is magic-link regeneration, not password reset
- **SMS is high-risk** — kill switches and opt-out handling are launch blockers, not nice-to-haves

Use this list when designing internal tools. Ask: *"What wakes me up at 2am? What can wait until we have 50 paying GCs?"*

---

## Persona split (future)

| Action | Super-admin (Thomas) | Support ops (Alex) |
|--------|-------------------|-------------------|
| Tenant lookup & read-only project view | ✅ | ✅ |
| Trace SMS/MMS, regenerate magic links | ✅ | ✅ |
| Fix phone/email on roster member | ✅ | ✅ |
| Cancel stuck poke / reset confirmation | ✅ | ✅ |
| STOP / opt-out dispute | ✅ | ✅ |
| Trial extension / plan change / refund | ✅ | ❌ |
| Suspend tenant messaging | ✅ | ❌ |
| Platform kill switch / maintenance mode | ✅ | ❌ |
| Feature flags | ✅ | ❌ |
| Hard delete tenant | ✅ | ❌ |

**v0.1:** Not implemented — Thomas only. See [backlog.md](./backlog.md) **BL-19**.

---

## v0.1 decisions (locked 2026-08-19)

| ID | Decision | v0.1 behavior |
|----|----------|---------------|
| **BL-20** | No admin impersonation | **A-1** tenant snapshot + read-only project drill-down; **A-9** deferred v0.1.1 |
| **BL-21** | Platform-global STOP | STOP on shared sender blocks **all** ContractorPro automated SMS/MMS to that phone; re-opt-in via magic link + consent screen, START reply, or admin restore (A-11) with audit |
| **BL-22** | Platform kill + tenant suspend | **A-10** platform-wide SMS halt (Thomas-only, nuclear); **A-6** per-tenant `messaging_suspended` for billing/abuse; per-project kill → v0.2 |

Log: [discovery-log.md](../../../discovery-log.md)

---

## Tenant lookup & support snapshot

### A-1: Look up tenant and open support snapshot
- **Trigger:** Ryan emails "sub didn't get the text" or Stripe webhook shows past_due
- Searches by company name, owner email, phone, Stripe customer ID, or project handle (e.g. `#maple-st`)
- Opens **tenant snapshot**: plan tier, trial end, staff users, active project count, last login, last outbound SMS
- Sees **health flags**: calendar token expired, 10DLC pending, messaging suspended, poke backlog
- Drills into one project read-only — schedule, roster, confirmation states, recent threads (**no impersonation in v0.1** — BL-20)
- **Success:** Support context in &lt;2 min without SQL or three dashboards
- **Build priority:** ✅ v0.1 MVP
- **Decision (2026-08-19):** Drill-down only; view-as-Ryan (**A-9**) deferred v0.1.1

---

## Communications ops (Twilio / MMS)

### A-2: Trace failed SMS/MMS delivery
- **Trigger:** Jesse says he never got the confirm text; Twilio shows `undelivered` or Ryan forwards carrier error
- Enters message ID, recipient phone, or tenant + time range
- Sees full trace: queued → sent → delivered/failed, carrier error code, segment count, cost
- Checks 10DLC campaign status for Riverside's brand
- Checks recipient opt-out status (STOP on file?)
- **Success:** Know whether to fix data (wrong number), resend (A-3), escalate carrier (A-11), or tell Ryan "Jesse opted out"
- **Build priority:** ✅ v0.1 MVP
- **SME check:** How far back must message history be searchable?

### A-3: Regenerate magic link for sub or customer
- **Trigger:** Link expired, Lauren lost email, Jesse forwarded to wrong phone
- Finds membership on project (sub Jesse on Maple St, customer Lauren)
- Invalidates outstanding tokens for that membership
- Generates new magic link; optionally triggers resend via SMS and/or email per channel rules
- Audit: "Magic link regenerated — reason: customer lost email"
- **Success:** Person can complete join/confirm without Ryan re-inviting from scratch
- **Build priority:** ✅ v0.1 MVP
- **Cross-ref:** C-24 (Ryan edits customer contact), H-1 (dual-channel confirm)

### A-11: Handle STOP / opt-out dispute
- **Trigger:** Jesse replies STOP then calls Ryan angry ("I still need job texts"); or wrongful STOP on shared family phone
- **Scope (BL-21):** STOP applies **platform-wide** on shared sender — all ContractorPro automated texts to that phone stop, not just Riverside
- Confirms opt-out recorded for phone (synced with Twilio opt-out list)
- Reviews recent messages to that number — which tenant sent what
- Auto-reply on STOP: *"You've been unsubscribed from ContractorPro messages. Reply START to resubscribe or use your project link."*
- **Re-opt-in:** magic link + explicit consent screen, START reply, or admin restore (**only** with verified consent + audit note)
- If legitimate STOP: Ryan sees "Jesse opted out of ContractorPro texts" — must re-invite / sub must re-opt-in
- **Success:** TCPA-safe; Ryan understands why texts stopped
- **Build priority:** ✅ v0.1 MVP (compliance)
- **Note:** Per-tenant opt-out deferred unless per-company 10DLC senders ship

### A-10: Platform maintenance mode + SMS kill switch
- **Trigger:** Twilio outage, deploy gone wrong, abuse wave, runaway poke loop across tenants
- **Platform kill (BL-22):** all outbound SMS/MMS halt or queue — **Thomas-only**, audited, rare/nuclear
- **Tenant suspend (A-6):** routine — one GC billing delinquent or abusive without affecting others
- **Maintenance mode:** portals read-only; optional queue outbound SMS during deploy
- Per-project kill → **v0.2** (not v0.1)
- Clears queue or drains after recovery; audit every toggle
- **Success:** Stop the bleeding before telco bill or reputation damage
- **Build priority:** ✅ v0.1 MVP (platform kill + tenant suspend)

---

## Data correction & stuck workflows

### A-4: Fix wrong phone or email on roster member
- **Trigger:** Ryan typo'd Jesse's phone; texts going to stranger; or Lauren's email bounce
- Finds membership on project; edits phone and/or email
- System invalidates magic links tied to old contact
- Resets channel confirm flags if customer (`email_confirmed` / `phone_confirmed`)
- Resends dual-channel confirm per customer rules (**H-1**) or invite SMS per sub rules (**S-1**)
- Audit: "Contact corrected; confirms reset"
- **Success:** Deliverability restored without duplicate memberships
- **Build priority:** ✅ v0.1 MVP
- **Cross-ref:** C-24 (Ryan self-service path — admin is escalation)

### A-8: Cancel stuck poke / reset confirmation state
- **Trigger:** Sub confirmed in portal but dashboard still ⏳; poke firing after Ryan archived project; cascade left tasks orphaned
- Views poke scheduler state for membership or project
- Cancels pending poke jobs
- Resets confirmation to last known good state **or** forces re-propose (Ryan notified)
- **Success:** Dashboard matches reality; no SMS after archive (**C-25**)
- **Build priority:** Should
- **Cross-ref:** C-7 (poke), C-25 (archive)

### A-6: Suspend tenant messaging (billing or abuse)
- **Trigger:** Card failed 14 days; Riverside sending spammy blast; 10DLC violation notice
- Sets tenant **messaging_suspended** — block outbound SMS/MMS; in-app still works
- Ryan sees banner: "Messaging paused — contact support" (not silent failure)
- Optionally suspend new project creation
- Re-enable after payment, abuse review, or 10DLC fix
- **Success:** Platform protected; tenant knows why texts stopped
- **Build priority:** ✅ v0.1 MVP
- **Cross-ref:** C-27 (**BL-16** billing limits)

---

## Auth & access recovery

### A-16: Recover contractor staff access (OAuth)
- **Trigger:** Ryan signed in with wrong Google account; Maci locked out after domain change
- Views staff users on tenant; sees linked OAuth provider + email
- Unlinks broken OAuth binding; sends fresh sign-in link to correct email
- Revokes active sessions if compromise suspected
- **Success:** Ryan/Maci back in without new company signup
- **Build priority:** Should
- **Note:** Subs/customers have no accounts — use A-3 only

### A-9: Impersonate contractor (read-only support view) — **deferred v0.1.1**
- **Trigger:** Ryan on phone can't describe what he sees; A-1 drill-down insufficient
- Starts **read-only** impersonation session as Ryan on Riverside tenant
- Banner on admin screen: "Viewing as Ryan — read only"; optional watermark in app
- Cannot send messages, change dates, or billing — view only
- Session logged; auto-expires in 30 min
- **Success:** Reproduce issue without sharing passwords (there are none) or screen-share gymnastics
- **Build priority:** v0.1.1 — **not v0.1** (BL-20 decided 2026-08-19: drill-down only at launch)

---

## Calendar integration ops

### A-7: Diagnose calendar sync failure
- **Trigger:** Ryan says "Sept 10 never showed in Google"; Jesse accepted but no event
- Views calendar connection per user: provider, token expiry, last sync, last error
- Sees failed sync jobs for assignment ID
- Actions: force token refresh, disconnect stale link, manual re-sync one assignment or whole project
- Optionally disable calendar **writes** for tenant while debugging (reads OK)
- **Success:** Root cause identified — expired token vs Google API outage vs bad date payload
- **Build priority:** Should
- **Cross-ref:** C-2, S-5a, H-6

---

## Billing & subscription ops

### A-5: Extend trial / adjust plan manually
- **Trigger:** Beta GC needs 30 more days; Ryan hit free tier cap (**C-27**, **BL-16**); goodwill credit after outage
- Opens Stripe customer link from tenant snapshot
- Extends trial end, changes plan tier, applies coupon, or comp months
- Updates internal tenant record to match Stripe (or webhook sync)
- Ryan sees updated limits in app on next load
- **Success:** Revenue ops without hacky DB edits
- **Build priority:** ✅ v0.1 MVP
- **Cross-ref:** C-27, **BL-16**

### A-17: Handle failed payment → grace → suspend
- **Trigger:** Stripe `invoice.payment_failed`; Riverside still sending poke SMS
- Day 0: email Ryan; in-app banner
- Day 7: grace — messaging continues with warning
- Day 14: **A-6** messaging suspend until paid
- Day 30: account read-only; Day 60: archive + offboarding conversation
- **Success:** Predictable dunning; no free telco burn
- **Build priority:** Should — tie to Stripe Billing launch
- **SME check:** Grace period length for small GCs?

---

## Abuse, trust & safety

### A-13: Review abuse report and suspend tenant
- **Trigger:** Carrier complaint; homeowner reports phishing SMS; photo upload flagged
- Reviews reported content + send history for tenant
- Suspends messaging (**A-6**) or full account
- Documents reason; notifies Ryan with appeal path
- **Success:** Bad actor off platform; legitimate GCs protected from shared 10DLC reputation damage
- **Build priority:** v0.1.1 (low volume at launch — manual process OK)
- **SME check:** Who receives abuse reports before in-app reporting exists?

### A-18: Block phone platform-wide
- **Trigger:** Repeat wrong-number spam; harassment via MMS ingest
- Adds phone to platform block list — no outbound from any tenant; inbound dropped or quarantined
- **Success:** One bad number can't burn multiple tenants
- **Build priority:** v0.1.1

---

## Observability & cost control

### A-14: Monitor cost and usage dashboards
- **Trigger:** Weekly check; Azure cost alert; "why is Twilio $400 this month?"
- Views: active tenants, messages/day, MMS segments, photo storage, failed job queue depth
- Drills into top tenants by telco spend
- Compares to [monthly-run-rate.md](../../../../finances/monthly-run-rate.md) planning defaults
- **Success:** Catch runaway poke loop or one GC with 20 projects before invoice shock
- **Build priority:** ✅ v0.1 MVP (ugly is fine)
- **Cross-ref:** [monthly-run-rate.md](../../../../finances/monthly-run-rate.md)

### A-19: Respond to platform health alert
- **Trigger:** Pager on 5xx spike, Twilio webhook failures, calendar sync queue backlog
- Opens dependency status: App Service, Postgres, Twilio, Google Calendar API, Stripe webhooks
- Identifies failing job type; retries or drains queue
- Enables **A-10** maintenance if deploy rollback needed
- **Success:** MTTR measured in minutes for solo founder
- **Build priority:** Should — even if alerts go to email initially

---

## Onboarding & growth ops

### A-20: Manually provision beta company
- **Trigger:** Design partner not ready for self-serve; pre-seed 10DLC
- Creates tenant + owner invite; sets plan/trial; notes in CRM
- Tracks activation checklist: calendar connected → first project → first sub confirmed
- **Success:** Founder-led onboarding without Ryan hitting broken signup
- **Build priority:** v0.1 — may be manual SQL + Stripe at first

---

## Data lifecycle (post-launch)

### A-12: Export or delete tenant data
- **Trigger:** Ryan churns and wants export; GDPR-style deletion request
- Export: projects, messages, photos, audit log — zip to secure link
- Delete: soft-delete → retention window → hard purge blob + DB + anonymize Stripe
- **Success:** Clean offboarding; no orphan PII
- **Build priority:** v0.2
- **SME check:** Retention period for MMS photos and message bodies?

---

## Platform configuration

### A-15: Manage feature flags
- **Trigger:** Ship plan mode (**FJ-1**) to beta only; disable Apple calendar during outage
- Toggles flags globally or per tenant: plan mode, AI drafts, Apple calendar, cascade batch SMS
- **Success:** Gradual rollout without separate deploys
- **Build priority:** v0.2
- **Cross-ref:** [future-journeys-v02.md](./future-journeys-v02.md)

---

## Workshop prompts (platform admin)

1. Which journeys happen **before first paying customer** vs. only at scale?
2. What can stay in **Stripe + Twilio + Azure consoles** for six months?
3. Where is **audit log** non-negotiable on day one? (contact edits, kill switch, opt-in restore — not impersonation in v0.1)
4. ~~**Impersonation:**~~ **Decided:** A-1 drill-down only; A-9 v0.1.1
5. ~~**Kill switch:**~~ **Decided:** platform kill (A-10) + tenant suspend (A-6); per-project v0.2
6. Anything here that's **too much for v0.1**? → [backlog.md](./backlog.md)

**Suggested build order:** A-1 → A-2 → A-3 → A-4 → A-11 → A-14 → A-6 → A-10 → A-5 → A-7 → A-8

**Critical path for launch:** **A-11** (STOP compliance) and **A-2** (delivery trace) — you cannot operate an SMS product without them.

Log decisions in [discovery-log.md](../../../discovery-log.md).
