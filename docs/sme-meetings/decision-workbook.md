# SME Decision Workbook

> Working decisions from SME Meeting 01 forward. **Nothing here is merged into PRD/architecture until marked `DECIDED` and propagated via correct-course.**

**Started:** 2026-08-20  
**Facilitator:** John (PM)  
**Source:** [sme-meeting-01 cross-reference](./sme-meeting-01-2026-08-20-software-design-lunch/cross-reference.md)

---

## How to use this doc

| Status | Meaning |
|--------|---------|
| `NOT STARTED` | Not yet discussed |
| `IN PROGRESS` | Options on table; needs input |
| `DECIDED` | Choice locked — ready to propagate |
| `DEFERRED` | Explicitly out of scope for now |

Each item has: **context → options → recommendation → decision → notes**

---

## Work-through order

| # | Topic | Status | Session |
|---|-------|--------|---------|
| **0** | [Scope ceiling: comm logging vs Ryan's four things](#0-scope-ceiling-comm-logging) | DECIDED | 1 |
| **1** | [Phone number model](#1-phone-number-model) | DECIDED | 1 |
| **1B** | [Company # SMS relay — SME validation](./sme-follow-ups/company-number-sms-relay.md) | OPEN → SME | — |
| **2** | [Approval process (sequential cascade + customer role)](#2-approval-process) | DECIDED* | 2 |
| **2B** | [Customer role in chain — SME follow-up](./sme-follow-ups/customer-approval-chain.md) | DEFERRED → SME | — |
| **1b** | [Company # provisioning timing](./sme-follow-ups/company-number-provisioning.md) | DEFERRED → SME | — |
| **3** | [Plan-first / job planning MVP cut](#3-plan-first--job-planning) | DECIDED | 3 |
| **4** | [File store / share backend](#4-file-store--share) | DECIDED | 4 |
| **5** | [Sub check-in / check-out](#5-sub-check-incheck-out) | DECIDED | 5 |
| **6** | [Auth / login per visit (OTP)](#6-auth--login-per-visit) | DECIDED | 6 |
| **7** | [Customer milestone comms](#7-customer-milestone-comms) | DECIDED | 7 |
| **8** | [Notification channel preferences](#8-notification-channels) | DECIDED | 7 |
| **9** | [Twilio portability on churn](#9-twilio-portability) | DEFERRED | 7 |

**Dependency rule:** Finish **#0** and **#1** before locking **#2**. Finish **#4** before **#5**.

---

## 0. Scope ceiling: comm logging

**Context:** Current plan treats **full MMS ingest + in-app message mirror** as core MVP (FR-14). Ryan said his four things don't require archived texts; Tom argued logging is the differentiator.

**Ryan's four things (v1 must-haves):**
1. Sub calendar integration (Google)
2. Customer read-only calendar view
3. Easy calendar/plan building for GC
4. Sub confirmation tracking (propose → accept → poke)

### Options

| Option | v1 behavior | Pros | Cons |
|--------|-------------|------|------|
| **A. Full comm logging** | Group MMS per relationship ingested; project-scoped threads in app | Audit trail; "who said what"; supports dispute resolution; matches current architecture | Per-project numbers feel clunky to SMEs; higher telco + build cost |
| **B. Approval-only comms** | System sends schedule texts + magic links; **no** ongoing dialogue ingest | Lean build; aligns with Ryan; works with central company number | Lose differentiator; GC still searches personal texts for ad-hoc talk |
| **C. Hybrid** | Log **system-generated** messages + link clicks; ad-hoc sub texts outside app unless in central inbox | Middle ground if central number used | Partial value; "hybrid" complexity |

### Recommendation (PM)

**Lean toward C for v1 if phone model moves to central number; keep A only if per-project handles survive.**

Ask Ryan explicitly: *"If calendar sync + confirmations work, do you ever open the app to read old texts — or only to see confirm status?"*

### Decision

| Field | Value |
|-------|-------|
| **Choice** | **C — Hybrid logging** (refined) |
| **Status** | DECIDED |
| **Decided by** | Thomas, 2026-08-20 |
| **Rationale** | Log all traffic on the company number. **Outbound from app** is reliably project-tagged. **Inbound** job attribution is best-effort without AI — accept <100% for v1. Do not require per-project group MMS ingest. |
| **Impacts** | FR-14 rewrite; E6 messaging epic; de-emphasize blob/MMS group mirror as MVP gate |

**v1 logging rules (locked intent):**

| Direction | Log? | Project tag |
|-----------|------|-------------|
| App → sub/customer (approvals, pokes, notices) | ✅ Yes | ✅ Explicit (system knows project + assignment) |
| Inbound to company # | ✅ Yes | ⚠️ Best-effort (manual assign, context, or future AI) |
| GC personal cell (outside company #) | ❌ No | N/A — out of scope |

### Session 1 notes

Thomas: outgoing from app = yes, tagged. Inbound tagging hard without AI — not pursuing 100% accuracy in v1.

---

## 1. Phone number model

**Context:** Plan locks **one handle # per project** ([project-handle-numbers.md](../planning-artifacts/technical-exploration/project-handle-numbers.md)). SME meeting pushed **one central company number** routing to Ryan + Maci (ReviewWave mental model).

**Coupled to #0:** Full logging needs identifiable project context on every message.

### Options

| Option | Model | Telco cost (rough) | Comm logging | SME fit |
|--------|-------|-------------------|--------------|---------|
| **A. Per-project handle** (current) | 1 Twilio # per active project; GC phone + handle + sub in group MMS | ~$10/project/mo + SMS/MMS | Full ingest ✅ | Ryan ⚠️ "subs won't comply" |
| **B. Per-company central** | 1 # per GC company; all subs/customers text one number | ~$1–2/mo + volume SMS | Needs project disambiguation in body or app routing | Ryan ✅ Maci ✅ (shared inbox) |
| **C. Hybrid central + tags** | Central # outbound; message prefix "Re: Maple St"; optional per-project groups for willing subs | Medium | Partial — central thread + tagged | Compromise |
| **D. Central for approvals only** | Personal/group texts for talk; system sends 1:1 approval SMS from central # | Low | Approval events only (#0 option B/C) | Ryan ✅; loses group MMS mirror |

### Multi-PM routing (Ryan + Maci) — required for B/C/D

| Approach | Behavior | Risk |
|----------|----------|------|
| **Shared inbox in app** | Both log into web; reply from central # in app (not personal SMS) | Requires GCs to use app for replies |
| **SMS forward to both** | Inbound central → fan-out to Ryan + Maci phones | Double-reply if both respond in SMS |
| **Claim / assign** | First responder claims thread in app | Extra UX; Ryan may ignore |

### Recommendation (PM)

**Do not flip to B without solving shared inbox.** Ryan's real ask is operational visibility (Maci sees what Ryan told Nick), not necessarily a new number per job.

**Spike before decide:** Twilio Conversations or Messaging — can one number serve multi-project threads with metadata?

### Decision

| Field | Value |
|-------|-------|
| **Choice** | **Per-company central number + SMS relay + app inbox** (Pattern A) |
| **Status** | DECIDED |
| **Decided by** | Thomas + Winston, 2026-08-20 |
| **Rationale** | One Twilio # per **Contractor subscription** (not per project). Subs/customers text Acme # only. Staff receive forwards from Acme # and reply **to** Acme #; platform routes to sub and copies other staff. App inbox for history, project assign, system messages. Matches Ryan/Maci ReviewWave ask; drops ~$10/project number cost. |
| **Impacts** | Supersedes `project-handle-numbers.md` → [company-number-messaging.md](../planning-artifacts/technical-exploration/company-number-messaging.md); architecture §1.7; E6/E8; cost model |

**Locked product rules:**

1. **One company number per subscriber** — **when provisioned: TBD** → [company-number-provisioning.md](./sme-follow-ups/company-number-provisioning.md) (Winston interim assumption: first paid / comms enabled).
2. **All system messages** (approvals, pokes, invites, milestone texts) send **from** company # with project prefix.
3. **Staff SMS relay** — inbound external → fan-out to team from company #; staff reply to company # (not sub's cell); **lenient ref token** — route by token, else single-open thread, else ask (no guess).
4. **App inbox** — shared visibility, orphan assign, reply without token; same threads as SMS relay.
5. **Inbound** to company # is ingested and logged; project association explicit on outbound; inbound best-effort + manual assign in v1.

**Thomas answers (2026-08-20):** inbound alert → **SMS to personal phone**; accept/decline → **magic links only**; provision timing → **SME TBD**; personal cell → **out of scope**.

**Architecture:** [company-number-messaging.md](../planning-artifacts/technical-exploration/company-number-messaging.md)  
**SME validation (flows + examples):** [company-number-sms-relay.md](./sme-follow-ups/company-number-sms-relay.md)  
**Architect handoff (complete):** [winston-company-phone-number.md](./handoffs/winston-company-phone-number.md)

**Explicit non-goals v1:**

- Per-project phone numbers (retire current locked model)
- 100% automatic inbound → project tagging without human or AI assist
- Group MMS with GC personal phone + handle # as virtual participant

### Session 1 notes

Thomas recommends hybrid: 1 # per contractor; app monitors and routes; flesh out routing with Winston. Outbound from app always logged + tagged.

---

## 2. Approval process

**Status:** IN PROGRESS  
**Depends on:** #0 ✅ #1 ✅ (outbound approvals now send from company # with explicit project tag)

**Sub-items:**

### 2A. Sequential / parallel invite cascade

**SME ask:** Don't blast all subs at once (Buildertrend pain). Sub₁ confirms → system invites Sub₂ — but not always; sometimes parallel is correct.

**Thomas direction (2026-08-20):**

1. **Templatable / reusable** — save cascade patterns (e.g. "kitchen remodel standard") as templates GC can apply to new jobs
2. **Configurable during use** — adjust chain at runtime per project; not locked at template apply
3. **Flexible dependencies** — support parallel contacts (multiple subs at once) and sequential gates (next wave waits for prior yes) on the same plan

| Mechanism | v1 intent |
|-----------|-----------|
| **Templates** | Company-level or project-type presets: ordered phases, which trades are sequential vs parallel, default deps |
| **Runtime override** | GC can change who goes next, skip a gate, or open parallel invites without rebuilding whole plan |
| **Dependency model** | Task/trade graph — some edges are hard gates (must confirm before downstream notify), others are soft/parallel |

**Not v1:** rigid single global chain for every job.

**Decision:** DECIDED (intent) — detailed UX/rules TBD in plan-first (#3) + epic work

**Open design questions (later):**
- Template lives at company level vs per project-type?
- Does "customer acknowledge" gate subs if 2B stays open?
- GC override: force-send next wave even without prior yes?

---

### 2B. Customer role in approval chain

**SME tension:** Maci wants customer preview/approve before subs; Ryan says constraints captured at contract signing.

**Status:** **DEFERRED → SME discussion** — do not spec until Ryan/Macie align.

**Follow-up doc:** [sme-follow-ups/customer-approval-chain.md](./sme-follow-ups/customer-approval-chain.md)

**Interim build rule:** Do not hard-code customer-as-blocker in cascade engine until 2B is decided. Support optional gate slot in template model (disabled by default).

**Decision:** DEFERRED

---

### 2C. Schedule-shift cascade

**Current plan:** FR-13 — dependent tasks shift; re-confirm required.

**Decision:** DECIDED — **unchanged** (no SME objection)

---

## 3. Plan-first / job planning

**Status:** DECIDED — **planning is IN for MVP** (promoted from v0.2)

**Prior plan:** Simple project + task dates in v0.1; full job planning module in v0.2 ([job-planning-workflow.md](../planning-artifacts/technical-exploration/job-planning-workflow.md)).

**Thomas direction (2026-08-20):** Planning is core — not a later add-on. Calendar is an **output** of the plan, not the starting point.

### End-to-end workflow (locked intent)

```text
┌─────────────────────────────────────────────────────────────────┐
│  PLAN (internal — project status: planning)                     │
│  • Apply / create template (trades, durations, buffers, deps)   │
│  • Set up contract needs (blackouts, access, customer constraints)│
│  • Assign subs to phases (rolodex; not notified yet)            │
│  • Pick start date → temporary layout (computed timeline)       │
│  • Overlay vs existing schedule / other active jobs             │
│  • Adjust until plan holds                                      │
└────────────────────────────┬────────────────────────────────────┘
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│  CUSTOMER PRELIM REVIEW                                         │
│  • Publish preliminary schedule view to customer                │
│  • Customer action per 2B (TBD — preview / ack / approve)     │
│  • Optional gate slot in template — off until SMEs decide       │
└────────────────────────────┬────────────────────────────────────┘
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│  FINALIZE → SUB CASCADE (project status: active)                │
│  • GC commits plan — still no Google writes until sub accepts   │
│  • Kick off configurable approval cascade (2A)                  │
│  • Parallel or sequential waves per template + runtime edits    │
│  • Sub confirm → poke → calendar sync                           │
└────────────────────────────┬────────────────────────────────────┘
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│  LIVE SCHEDULE + RESCHEDULE                                     │
│  • Changes re-enter plan preview where deps affected (FR-13)    │
│  • Shift cascade → re-confirm affected subs                     │
└─────────────────────────────────────────────────────────────────┘
```

### MVP scope (Thomas)

| In MVP | Notes |
|--------|-------|
| **Templates** | Reusable job patterns; feeds 2A cascade config |
| **Contract / customer constraints** | Blackout dates, access notes at project setup |
| **Phase list + durations + buffers** | Temporary layout; dashed/planning UI |
| **Dependency graph** | Parallel + sequential; runtime adjustable |
| **Sub assignment (planning only)** | Who *will* be invited — no SMS yet |
| **Overlay existing schedule** | Adjust plan against GC calendar + other jobs |
| **Customer prelim review step** | Publish read-only prelim; gate behavior = 2B |
| **Finalize → sub cascade** | Bridges to approval engine |

| Likely v0.1.1 / stretch | Notes |
|-------------------------|-------|
| Multi-job portfolio balance panel | Mentioned in job-planning doc; Thomas emphasized single-job overlay — validate with Ryan |
| Reverse-schedule from anchor trade (Nick/cabinets) | Ryan workflow; may be template feature |
| AI-assisted replan | Deferred per SME meeting |

### Coupling

| Item | Relationship |
|------|--------------|
| **2A** | Templates define default cascade waves + deps; runtime overrides during active project |
| **2B** | Customer step between plan and sub cascade — **gate rules TBD** |
| **2C** | After live: reschedule flows back through plan preview for affected deps |
| **#1** | Sub invites send from company # only after finalize + cascade wave fires |

### Decision

| Field | Value |
|-------|-------|
| **Choice** | **Plan-first in MVP** — full workflow above |
| **Status** | DECIDED (intent) |
| **Decided by** | Thomas, 2026-08-20 |
| **Rationale** | SMEs plan before calendar; Ryan's Buildertrend workaround (manual add-next-sub) is the problem we're solving. Templates + prelim customer review + cascade are one continuous flow. |
| **Impacts** | Promote job planning from v0.2 → MVP; PRD §scope; epics (new E? or expand E2/E7); UX mockups; `job-planning-workflow.md` status |

### Open design questions

- Minimum template fields for Ryan's first real job (kitchen standard?)
- "Contract needs" — structured fields vs freeform notes?
- Does prelim review require explicit GC "publish" action separate from finalize?
- Reverse-schedule from anchor date — MVP or template-only v0.1.1?

**Resolved (2026-08-20):** **Yes — two separate actions:**
1. **Publish prelim to customer** — customer sees skeletal schedule; 2B gate may apply
2. **Finalize & start sub cascade** — only after prelim step complete (per 2B when decided)

---

## Propagation checklist (when ready)

After Winston session closes #1 open items:

- [x] Run `bmad-correct-course` on phone + messaging + **planning promotion** — [sprint-change-proposal-2026-08-20.md](../planning-artifacts/sprint-change-proposal-2026-08-20.md) **approved 2026-08-20**
- [x] Update `project-handle-numbers.md` → company-number model (superseded banner)
- [x] Update PRD FR-14, messaging epic, architecture workers
- [x] Revise telco cost model (~$1–2/mo base + SMS/MMS volume, not ~$10/project)
- [x] Promote job planning v0.2 → **MVP** in PRD, product-vision, epics
- [x] Update `job-planning-workflow.md` + customer prelim step before sub cascade
- [x] UX: planning mode UI, prelim publish, finalize, cascade controls (mockup pack 2026-08-20)
- [x] Thin spots → backlog RC-1–RC-7 for later discussion

---

## 4. File store / share

**Status:** IN PROGRESS  
**Depends on:** #3 (project exists with phases); couples to **#5** check-in/out  
**SME source:** Ryan QR + Google Drive bucket; rejects Company Cam / fixed cameras

### What SMEs actually want

| Need | Ryan's take | Tom's take |
|------|-------------|------------|
| Job-site **resource bucket** | Blueprints, permits, layout, scope — scan QR at threshold | Software could attach docs to project + print QR |
| **Progress photos** | Sub scans QR, uploads to folder at end of day | Could be same bucket or app upload |
| **Complexity** | "Too easy to structure per job" in Drive — may not need software | Value in generating/managing bucket + QR from project setup |
| **Security** | Indoor QR OK; **no door codes** in QR; customer seeing permit/photos OK | Phone-restricted scan adds complexity Ryan may reject |

### What we're NOT building (SME aligned)

- Company Cam–style fixed job-site cameras
- Heavy access-control on every scan (v1)
- Full document management / versioning ERP

### Architecture options

| Option | How it works | Pros | Cons |
|--------|--------------|------|------|
| **A. Ryan's manual Drive** | GC creates Drive folder; laminates QR themselves; **out of app scope** | Zero build; Ryan happy today | No product value; check-in (#5) orphaned |
| **B. App-managed native storage** | Azure Blob per project; portal upload + QR → app URL | Full control; ties to permissions model; photos in threads | Build + storage cost; Ryan already uses Drive |
| **C. App-provisioned Google Drive** | OAuth connect GC Google; app creates folder, uploads docs, generates QR to Drive link | Matches Ryan mental model; low GC setup | Google API integration; sharing/ACL quirks; two backends if we also store MMS |
| **D. Hybrid — metadata in app, files in Drive** | Project doc list in DB; files live in linked Drive folder; QR = deep link | Ryan keeps Drive; app owns "what's on the job" + QR print sheet | Integration work; orphan if GC disconnects Google |
| **E. Hybrid — native for photos, Drive for docs** | Plans/permits via Drive link; progress photos via app blob (or MMS ingest) | Split by use case | Two systems; more confusion |

### PM recommendation (for discussion)

**Lean D for MVP** if file store is in scope at all:

- GC connects Google (already OAuth for calendar)
- On project create/finalize: app creates **project Drive folder** (or links existing)
- GC uploads / attaches doc list in app UI (sync to folder)
- App generates **printable QR sheet** (laminated at job site — Ryan's workflow)
- QR opens mobile-friendly **resource page**: Drive links + scope text + "upload photos here" (Drive folder or app upload — **#5 decision**)

**Defer native blob** for job-site bucket unless Ryan rejects Drive integration — we already need blob for MMS/media in messaging doc.

**MVP cut if schedule is tight:** QR sheet + doc **links** (GC pastes Drive URLs) before full Drive API automation.

### Decision

| Field | Value |
|-------|-------|
| **Choice** | **D — Hybrid (app metadata + Google Drive storage)** |
| **Status** | DECIDED |
| **Decided by** | Thomas, 2026-08-20 |
| **Rationale** | MVP includes file share. Drive is GC-familiar and cheap; app is the only sub/customer surface — no direct Drive interaction. Docs and progress photos land in project Drive folder; portal lists/views/uploads via app backend. |
| **Impacts** | Google Drive API integration (Winston); project setup flow; QR → app resource page; #5 check-in/out |

**Locked product rules:**

| Rule | Detail |
|------|--------|
| **In MVP** | Yes — Option D (not deferred) |
| **Storage backend** | Google Drive folder per project (auto-create or link on setup) |
| **Sub/customer UX** | **App portal only** — view files, upload photos; never open drive.google.com |
| **QR code** | Points to **app landing page** for project resources (not raw Drive URL) |
| **GC UX** | Manage doc list in app; files sync to linked Drive folder |
| **Photos** | Upload via app portal → server writes to project Drive folder; viewable in portal |

**Google Drive viability (PM + technical note):**

| ✅ Good fit | ⚠️ Watch outs |
|------------|---------------|
| Ryan already lives in Drive; aligns with Google OAuth for calendar | Need **`drive.file`** or scoped folder access — avoid over-broad Drive scope |
| GC pays for storage (Workspace); not our blob bill for job docs | OAuth token refresh + "GC disconnected Google" degradation path |
| Server-side upload on behalf of GC keeps subs off Google accounts | Drive API rate limits; large blueprint PDFs — test preview strategy |
| App proxy model matches "subs stay in app" rule | GC on non-Google shop is out of ICP for MVP anyway |
| Listing + download via API enables in-portal viewer | Winston spike: preview (iframe export vs proxied download vs thumbnail) |

**Not in MVP:** subs editing Drive directly; public Drive links; door codes in QR; phone-verified scan gates.

**Winston spike items:**
- OAuth scopes (calendar + drive minimum)
- Folder create/link on project finalize
- Server-side upload from portal (multipart → Drive API)
- In-portal file list + preview component
- Behavior when GC revokes Google access mid-project

---

## 5. Sub check-in / check-out

**Status:** DECIDED (intent)  
**Depends on:** #4 ✅ (QR → app resource page; Drive uploads via portal)

**Thomas direction (2026-08-20):**

```text
CHECK-IN                          CHECK-OUT
Scan QR ──► app                   Scan QR ──► app
              │                                 │
              ▼                                 ▼
     First visit: associate            Attach photos (+ notes?)
     phone → sub on project            to project (→ Drive folder)
              │                                 │
              ▼                                 ▼
     Return visits: recognized         EOD progress upload
     sub can view resources
```

### Locked product rules

| Rule | Detail |
|------|--------|
| **QR role** | Entry point to app only — same project resource URL (one QR per job site, not Drive) |
| **Check-in** | Scan QR → app; **first time** verify phone and link to sub assignment on this project |
| **Check-out** | Scan QR again → upload photos (+ optional notes) through app → Drive folder |
| **Identity** | Phone # associated to sub on first check-in; subsequent scans recognize them |
| **Enforcement** | Soft — no whitelist gate, no door codes (per SME + #4) |
| **Training** | Ryan's laminated sign at threshold; scope/docs on resource page |

### First-time phone → sub association

| Step | Behavior |
|------|----------|
| 1 | Sub scans QR → lands on app resource page |
| 2 | If phone unknown on project → prompt: confirm name + phone (match invite list or GC-added subs) |
| 3 | Link session to `project_participant` (sub role) |
| 4 | Show resources; allow photo/note upload on check-out flow |

**Open UX (later):** Sub on roster but different crew member's phone — GC manual link vs pick-from-list vs invite flow.

**Coupling:** Extends invite/join flow — check-in may be **first touch** for subs who ignored SMS invite but show up on site.

### One QR vs two (SME had "arrive" + "checkout" QRs)

**Decision:** **Single QR** → app page with **Check in** / **Upload today's work** actions (Thomas intent). Avoids two laminated sheets unless Ryan insists in SME #2.

### Decision

| Field | Value |
|-------|-------|
| **Choice** | QR-mediated check-in + check-out; phone→sub bind on first scan; photos/notes via app |
| **Status** | DECIDED (intent) |
| **Decided by** | Thomas, 2026-08-20 |
| **Impacts** | Resource page UX; participant verify; Drive upload API; optional check-in timestamps in DB |

**Winston spike:** Phone verify at QR landing (OTP vs match invited phone vs trust first association).

---

## 6. Auth / login per visit

**Status:** IN PROGRESS  

**SME signal:** Login-free for customers/subs; magic link + OTP when needed (Ryan cited Buildertrend login pain, student-loan-style codes).

**Current plan** ([invite-join-flow.md](../planning-artifacts/technical-exploration/invite-join-flow.md)): magic link in SMS; OTP for bookmarked return; optional trusted device cookie (30d).

**New coupling (#5):** QR check-in may be **first auth moment** for subs — phone verify + bind to participant.

### Options

| Audience | Option | Behavior |
|----------|--------|----------|
| **GC staff** | OAuth (unchanged) | Google MVP; session cookie |
| **Sub/customer — notification links** | Magic link in SMS | Each schedule/approval text includes signed link → session |
| **Sub/customer — bookmark/QR return** | OTP to verified phone | Every visit or trusted device TTL? |
| **Sub — QR check-in (#5)** | Phone match + optional OTP | First scan: associate phone; return: recognize device or re-verify |

### PM questions for Thomas

1. **OTP every visit** for customers — yes, or trusted device OK for 30 days?
2. Same rule for subs on portal (non-QR)?
3. QR return visit: skip OTP if same phone+device recently checked in?

### Decision

| Field | Value |
|-------|-------|
| **Choice** | **Trusted device (30d) + OTP fallback** — same rules for customers and subs |
| **Status** | DECIDED |
| **Decided by** | Thomas, 2026-08-20 |
| **Rationale** | Low friction when same device; OTP when new device or trust expired. QR path: phone+sub recognition sufficient on return — no extra OTP if already bound. |

**Locked auth rules:**

| Scenario | Behavior |
|----------|----------|
| **SMS magic link** | Signed link in notification → session (unchanged) |
| **Return visit, trusted device** | Same phone + browser/device within **30-day TTL** → no OTP |
| **Return visit, untrusted** | New device OR trust expired → **SMS OTP** to verified phone |
| **Customers & subs (portal)** | **Same policy** |
| **QR check-in return (#5)** | **Phone + sub association** on project is enough — no OTP if recognized |
| **QR first visit** | Phone bind to sub (verify against roster); establishes trust |
| **GC staff** | Google OAuth — unchanged |

**Implementation note:** `participant_sessions` + device fingerprint + `trusted_until`; OTP issues fresh trust on success.

---

## 7. Customer milestone comms

**Status:** DECIDED  

**SME ask:** Automated texts like *"Tomorrow is demo day"* with prep instructions (cover adjacent rooms, etc.) — not just date-change alerts.

**Coupling:** Plan-first (#3) has phases; milestone comms trigger off phase dates or customer-visible tasks.

### Options

| Option | Behavior |
|--------|----------|
| **A. Product-scheduled** | GC attaches prep templates to phases; system sends SMS/email on schedule from company # |
| **B. Manual only** | Ryan/Macie send from existing email templates; app not involved |
| **C. Hybrid** | Templates in app; GC previews/sends or auto-send per phase toggle |

**PM lean:** **C** — templates reusable like plan templates; default **manual send** for MVP, auto-send toggle per phase optional.

### Decision

| Field | Value |
|-------|-------|
| **Choice** | **A — Product-scheduled automated milestone comms** |
| **Status** | DECIDED |
| **Decided by** | Thomas, 2026-08-20 |
| **Rationale** | System sends customer milestone SMS/email from company # based on plan phase dates. GC configures **days before** milestone at **Contractor settings** (company-level default; optional per-phase override TBD in UX). |
| **Impacts** | Phase/milestone model; scheduled job worker; prep message templates; FR expansion |

**Locked rules:**
- Automated sends tied to **customer-visible phases/milestones** in plan
- **Lead time** (e.g. 1 day before, 7 days before) = **Contractor setting** (company default)
- Content: milestone name + prep instructions (template per milestone type)
- Sends from **company #** / email per participant `notify_via` (#8)
- Distinct from date-**change** notifications (those stay event-driven)

---

## 8. Notification channels

**Status:** DECIDED  

**SME ask:** Email + text; some subs email, some text; customer preference matters.

**Current plan:** `notify_via` per participant (sms | email | both).

### Decision

| Field | Value |
|-------|-------|
| **Choice** | **Per-participant preference: SMS, email, or both** |
| **Status** | DECIDED |
| **Decided by** | Thomas, 2026-08-20 |
| **Rationale** | Customers and subs choose how they receive messages. Defaults at join; editable in portal anytime. |
| **Impacts** | Join flow, participant settings, all outbound notification workers |

**Locked rules:**

| | Default at join | Editable |
|---|-----------------|----------|
| **Subcontractor** | SMS | ✅ Yes — SMS / email / both |
| **Customer** | Both | ✅ Yes — SMS / email / both |
| **GC staff** | Email + in-app | Separate from participant model |

All schedule, approval, poke, and milestone sends respect `notify_via`.

---

## 9. Twilio portability

**Status:** DEFERRED — **research only** (no product decision until findings)

**Question:** Can GC take company # with them on churn?

**Owner:** Tom / Winston spike  
**Trigger:** Before billing launch or if Ryan raises again in SME #2

**Research checklist:**
- [ ] Twilio number port-out policy when subscription ends
- [ ] Can number transfer to GC's own Twilio/carrier account?
- [ ] Alternative: export comm history but release number (current plan default)
- [ ] Document answer for sales/onboarding FAQ

**Interim product message:** Number stays while subscribed; released on cancel — portability TBD pending research.

---

## Session log

| Session | Date | Items worked | Outcomes |
|---------|------|--------------|----------|
| 1 | 2026-08-20 | #0, #1 | #0 Hybrid logging DECIDED; #1 Per-company # + SMS relay DECIDED (SME validation → 1B) |
| 2 | 2026-08-20 | #2 | 2A intent DECIDED; 2B DEFERRED; 2C unchanged |
| 3 | 2026-08-20 | #3 | Plan-first IN MVP; two-button prelim vs finalize |
| 4 | 2026-08-20 | #4 | D in MVP; Drive backend; app-only portal; QR → app resource page |
| 5 | 2026-08-20 | #5 | QR check-in; QR + photos/notes check-out; phone→sub on first scan |
| 6 | 2026-08-20 | #6 | 30d device trust + OTP fallback; same for sub/customer; QR = phone+sub recognize |
| 7 | 2026-08-20 | #7–#9 | Milestone auto-send + contractor lead-time setting; participant notify prefs; #9 research only |

---

## Work-through complete ✅

All numbered items decided or explicitly deferred. **Remaining before propagation:**

| Item | Status |
|------|--------|
| **2B Customer approval chain** | SME follow-up — [customer-approval-chain.md](./sme-follow-ups/customer-approval-chain.md) |
| **1B Company # SMS relay** | SME follow-up — [company-number-sms-relay.md](./sme-follow-ups/company-number-sms-relay.md) |
| **1b Company # provisioning timing** | SME follow-up — [company-number-provisioning.md](./sme-follow-ups/company-number-provisioning.md) |
| **#4/#5 Winston** | Google Drive API + QR resource page spike |
| **#9 Twilio portability** | Research only |
| **Propagation** | Run `bmad-correct-course` → update PRD, architecture, epics |

---
