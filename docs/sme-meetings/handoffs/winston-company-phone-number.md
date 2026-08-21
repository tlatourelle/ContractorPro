# Architect Handoff: Company Phone Number & Messaging Inbox

| Field | Value |
|-------|-------|
| **Audience** | Winston (architect) |
| **From** | Thomas + John (PM) |
| **Date** | 2026-08-20 |
| **Status** | Product intent DECIDED — **routing DECIDED** (2026-08-20 Winston session) |
| **Workbook** | [decision-workbook.md](../decision-workbook.md) §0, §1 |
| **Supersedes** | [project-handle-numbers.md](../../planning-artifacts/technical-exploration/project-handle-numbers.md) (per-project handle model) |
| **SME context** | [sme-meeting-01 cross-reference](../sme-meeting-01-2026-08-20-software-design-lunch/cross-reference.md) |

---

## Purpose of this session

Design the **technical architecture** for ContractorPro's messaging layer after a **direction change**:

- **Was:** one Twilio number per **project** + group MMS (GC personal phone + handle # + sub)
- **Now:** one Twilio number per **Contractor subscription** + **app-monitored shared inbox** (Ryan + Maci model)

Thomas is bringing this doc to Winston. **Deliverable:** recommended architecture + delta vs current `architecture-v0.1.md` / `project-handle-numbers.md`.

---

## What changed and why (SME)

Ryan & Maci (paying-customer SMEs) rejected per-project group threads:

- Subs won't stay in separate project threads; they'll call Ryan's cell anyway
- Ryan wants **one central company number** (ReviewWave mental model)
- Maci wants **shared visibility** — see what Ryan told a sub without asking

Thomas accepted: **one # per contractor**, app routes traffic, log everything on that #.

---

## Locked product decisions (do not re-litigate)

### #0 — Hybrid comm logging

| Direction | Log? | Project tag |
|-----------|------|-------------|
| **App → sub/customer** (approvals, pokes, milestones, invites) | ✅ Yes | ✅ Explicit |
| **Inbound to company #** | ✅ Yes | ⚠️ Best-effort; manual assign OK in v1 |
| **GC personal cell** (outside company #) | ❌ No | Out of scope |

**Non-goal v1:** 100% automatic inbound → project tagging (no AI requirement for MVP).

### #1 — Company phone number

| Rule | Detail |
|------|--------|
| **Cardinality** | **One E.164 per Contractor company** (subscription), not per project |
| **Outbound** | All system SMS/MMS from company # (approvals, pokes, invites, milestone comms) |
| **Inbound** | Webhook ingest → company inbox; associate to project when possible |
| **GC coordination** | Ryan + Maci use **app inbox**; replies go **out via company #** |
| **Provisioning** | TBD by Winston — signup vs first paid tier vs first outbound |
| **Retired** | Per-project handles, group MMS with personal phone + virtual handle |

### Related locked decisions (coupling)

| # | Impact on messaging |
|---|---------------------|
| **2A** | Approval cascade sends from company #; outbound always tagged to project |
| **7** | Automated customer milestone SMS from company # on schedule |
| **8** | Respect participant `notify_via` (sms / email / both) |
| **6** | Magic links in SMS; separate from inbox auth |
| **4/5** | QR check-in/upload is **web flow**, not SMS thread — same participant phone model |

**Out of scope this session:** customer approval gate (2B SME pending), Google Drive API (#4), Twilio port-out research (#9).

---

## Current architecture to replace

From [project-handle-numbers.md](../../planning-artifacts/technical-exploration/project-handle-numbers.md) and [architecture-v0.1.md](../../planning-artifacts/architecture-v0.1.md):

```text
OLD MODEL
─────────
JIT buy number per project → phone_number_pool per company
Group MMS: GC phone + sub + project handle #
Inbound: To = handle # → project_id (deterministic)
Workers: MmsMediaIngestWorker, mms_threads, conversation_sid
Cost: ~$10/active project/month for numbers
```

```text
NEW MODEL (intent)
──────────────────
One number per contractor company
1:1 SMS (and maybe MMS) from company # to sub/customer
Inbound to company # → shared inbox → thread TBD
Outbound from app → always project_id + assignment_id in DB
Cost: ~$1–2/mo number + SMS/MMS volume
```

---

## Questions for Winston — need recommendations

### 1. Thread / conversation model

One company # serves **all projects**. How do we slice conversations?

| Model | Description | Tradeoff |
|-------|-------------|----------|
| **A. By external phone only** | One thread per `(company, participant_phone)` | Simple; multi-project sub mixes jobs |
| **B. By phone + project** | One thread per `(company, participant_phone, project_id)` | Cleaner ACL; need project on inbound |
| **C. Unified inbox + tags** | Single feed; filter by project/participant | Flexible UX; heavier UI |

**Need:** Recommended model + entity sketch (`threads`, `messages`, nullable `project_id`).

**Product fallback v1:** Orphan inbound → **manual "assign to project"** queue when sender matches 0 or many projects.

---

### 2. Twilio product stack

| Question | Options |
|----------|---------|
| API | Programmable Messaging vs Conversations vs both |
| Number type | 10DLC local vs toll-free — deliverability, cost, trust |
| 10DLC | Platform brand (current arch decision) vs per-contractor brand |
| Reply correlation | Status callbacks, `Body` parsing YES/NO, custom params on outbound |
| MMS inbound | Ingest to blob + thread, or SMS-only + photos via QR portal (#4/#5)? |

**Need:** Spike conclusion with doc links; webhook handler shape.

---

### 3. Outbound send path

All system messages from company #:

| Message type | Recipient | Must carry in DB |
|--------------|-----------|------------------|
| Sub approval / poke | Sub phone | `project_id`, `assignment_id`, magic link |
| Customer milestone (#7) | Customer phone | `project_id`, phase/milestone id |
| Invite / join | Sub or customer | `project_id`, invite token |
| GC reply from inbox | Sub or customer | `project_id`, `thread_id`, sender user id |

**Questions:**

- Require **project prefix in SMS body** for human readability? (PM lean: yes — e.g. `Maple St — confirm electrical Aug 25`)
- Is **SMS reply "YES"/"NO"** valid accept/decline, or magic-link only? *(Thomas — decide with Winston)*
- MMS from GC in app — in MVP or link-only?

---

### 4. Inbound ingest

```text
Twilio webhook POST /api/v1/webhooks/twilio
  → validate signature
  → lookup From phone → project_participants (0 / 1 / many)
  → 0: orphan queue
  → 1: attach to thread
  → many: prompt GC to pick project (app) or best-guess if body contains project hint
  → persist message; notify GC team
```

**Need:** Idempotency key, worker vs sync, notification fan-out.

---

### 5. Ryan + Maci shared inbox (critical)

SME requirement: both see traffic; avoid double-reply chaos.

| Topic | Options |
|-------|---------|
| **GC reply surface** | App-only (product intent) vs SMS relay to personal phones |
| **Inbound alert** | In-app only / email / SMS ping to personal phones |
| **Collision** | Show "Ryan replied 2m ago" / claim thread / assign owner |
| **Team scope** | All team members see all threads in MVP? |

**Need:** Real-time strategy (SignalR?), notification table, reply API that sends via Twilio from company #.

**Risk:** If Ryan never opens app and expects SMS relay, app-only model fails — flag for Thomas.

---

### 6. Number lifecycle

| Event | Expected behavior (confirm/refine) |
|-------|-----------------------------------|
| Company signup (sandbox) | No number until paid? Or number but no outbound? |
| First paid / comms enabled | Provision company # |
| Active subscription | Number retained |
| Unsubscribe / churn | Release to Twilio immediately; DB history retained |
| Return customer | New number (no reattach) — unless #9 research changes this |

**Retire:** per-project JIT pool, cooling per project number, `projects.handle_phone_e164` as primary routing key.

**Need:** Revised `phone_number_pool` schema — one row per company vs pool of one.

---

### 7. ACL & thread separation

Product rule unchanged:

- **Customer threads** and **sub threads** remain separate
- Customer never sees sub-only content

**Need:** How inbox enforces audience when one number serves both — separate thread types? `audience` column?

---

### 8. Architecture delta checklist

Winston to mark each:

| Artifact | Action |
|----------|--------|
| `architecture-v0.1.md` §1.7, telephony module, ERD | Rewrite handle model |
| `project-handle-numbers.md` | Replace or archive → new `company-number-messaging.md` |
| `messaging-and-media.md` | Remove group MMS as MVP; inbox model |
| `mms_threads` / `phone_number_pool` tables | Redesign |
| `MmsMediaIngestWorker` | Rename/repurpose for inbound SMS/MMS |
| Epics E6 messaging | Scope change |
| Cost model / finances doc | ~$10/project → volume-based |

---

## Open questions for Thomas (in room with Winston)

| # | Question | Thomas answer |
|---|----------|---------------|
| 1 | **GC inbound alert:** app-only, email, or SMS to personal phone when someone texts company #? | **SMS to personal phone** — staff notified on personal cell when inbound hits company # (see also SMS relay pattern in [company-number-messaging.md](../../planning-artifacts/technical-exploration/company-number-messaging.md)) |
| 2 | **SMS "YES" reply:** valid accept/decline in v1, or magic-link only? | **Magic links only** |
| 3 | **When to buy number:** signup, first project, first paid tier, first outbound? | **Undecided** — SME follow-up → [company-number-provisioning.md](../sme-follow-ups/company-number-provisioning.md) |
| 4 | **GC override:** can team reply from personal phone and expect it logged? (Product says no — confirm) | **Yes — out of scope.** Personal cell not logged; job comms through company # only. |

---

## Explicit non-goals (v1)

- Per-project phone numbers
- Group MMS with GC personal phone + handle #
- AI inbound → project classification
- Twilio number port-out on churn (#9 research only)
- Perfect inbound project tagging without human assist

---

## Suggested session agenda (~90 min)

| Time | Topic |
|------|-------|
| 15 min | Thread model recommendation |
| 20 min | Twilio stack + 10DLC + webhooks |
| 15 min | Outbound templates + reply YES vs link |
| 20 min | Inbound + orphan queue + multi-project same sub |
| 15 min | GC shared inbox + notifications + reply path |
| 15 min | Schema delta + deliverables agree |

---

## Deliverables expected from Winston

- [x] **Recommendation memo** → [company-number-messaging.md](../../planning-artifacts/technical-exploration/company-number-messaging.md)
- [x] **Updated entity sketch**: `contractor_phone_numbers`, `comm_threads`, `staff_sms_sessions`, orphan queue
- [x] **Webhook flow diagram**: in company-number-messaging.md
- [x] **Delta list** against `architecture-v0.1.md` §1.7, §5.5 (applied)
- [x] **Open risks** flagged — see company-number-messaging.md + SME follow-up
- [x] Mark #1 **DECIDED** in [decision-workbook.md](../decision-workbook.md)
- [ ] **SME validation** — [company-number-sms-relay.md](../sme-follow-ups/company-number-sms-relay.md) (flows + examples with Ryan/Macie)

---

## Reference links

| Doc | Path |
|-----|------|
| Decision workbook §0–§1 | [decision-workbook.md](../decision-workbook.md) |
| SME cross-reference | [cross-reference.md](../sme-meeting-01-2026-08-20-software-design-lunch/cross-reference.md) |
| Old handle model | [project-handle-numbers.md](../../planning-artifacts/technical-exploration/project-handle-numbers.md) |
| Architecture v0.1 | [architecture-v0.1.md](../../planning-artifacts/architecture-v0.1.md) |
| Messaging exploration | [messaging-and-media.md](../../planning-artifacts/technical-exploration/messaging-and-media.md) |
| Schedule confirmations | [schedule-confirmation-workflow.md](../../planning-artifacts/technical-exploration/schedule-confirmation-workflow.md) |
| Twilio portability (research) | Workbook §9 — not blocking MVP design |
