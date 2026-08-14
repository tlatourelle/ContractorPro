# Google Calendar Integration — Exploration

Status: **Exploratory** (2026-08-13)  
Related: [product-vision.md](../product-vision.md), [stack-web-api-db.md](./stack-web-api-db.md), [schedule-confirmation-workflow.md](./schedule-confirmation-workflow.md), [job-planning-workflow.md](./job-planning-workflow.md)

## Product intent (from stakeholder)

Integration with Google Calendar is not only “export dates.” It is a **dual-view model**:

1. **Native Google Calendar** — each person uses their own Google Calendar app/web as they already do.
2. **ContractorPro web app** — project-centric schedule view of the **same underlying calendars/events**.
3. **Shared calendars** — many calendars created and managed through the **app/project process** (not ad-hoc sharing in Google UI).

> “Every person's ability to look at and use their own Google Calendar directly **as well as** use the web-app's view of the same calendars. Lots of shared calendars being managed via the app/project process.”

This elevates calendar integration from a feature to a **platform principle**: **integrate, don’t replace** — Google remains a first-class UX for time.

**Planning vs scheduling:** Dates are modeled in **plan mode** first (in-app only, no Google writes) — see [job-planning-workflow.md](./job-planning-workflow.md). Google Calendar receives events **after finalize** and **sub accept** — see [schedule-confirmation-workflow.md](./schedule-confirmation-workflow.md).

---

## Calendar provisioning: BYO or Pro-provided

Each **GC company** chooses how calendars connect to ContractorPro. Both paths support the **dual-view** model (Google app + ContractorPro project view).

### Mode A — **Bring your own (BYO)**

The contractor **connects their existing Google account** and integrates calendars they already use.

| Aspect | Behavior |
|--------|----------|
| **Who owns the Google calendar** | The GC (their Google account) |
| **Setup** | OAuth connect → GC picks which calendar(s) to link (company calendar, “Jobs”, etc.) |
| **ContractorPro role** | Sync project tasks/events **into** linked calendars; read changes back (per sync rules) |
| **Best for** | GCs already living in Google Calendar; want zero migration |
| **Dual-view** | ✅ Events appear in their existing Google calendars + app project view |

> “Contractor has their own Google Calendar and integrates it with ContractorPro.”

### Mode B — **Pro-provided (app-created)**

ContractorPro **creates and holds** dedicated Google calendar(s) for each **entity** — managed through the app/project process.

| Aspect | Behavior |
|--------|----------|
| **Who owns the Google calendar** | Created via API (under GC’s connected Google **or** ContractorPro service identity — TBD technically) |
| **Setup** | GC enables “ContractorPro calendars” → app creates e.g. one calendar per project |
| **ContractorPro role** | **System of record** for that calendar’s project schedule; creates ACLs, events, cascade updates |
| **Best for** | GCs starting fresh; want clean separation (one calendar per job); subs invited automatically |
| **Dual-view** | ✅ Calendars still live in Google — subs/GC open Google app; app manages sharing |

> “ContractorPro can create and hold a dedicated calendar (or more than one) for each entity.”

### Entities (what gets a calendar)

Draft — not final:

| Entity | Pro-provided calendar? | BYO equivalent |
|--------|------------------------|----------------|
| **GC company** | Optional company-wide calendar | Link existing “Company” calendar |
| **Project** | **One dedicated calendar per project** (likely default in Mode B) | Map project → chosen existing calendar |
| **Phase / trade** | Optional sub-calendar or tagged events | Filters on linked calendar |
| **Sub** | Usually **ACL on project calendar**, not separate calendar | N/A |

**Each contractor company** configures preference at onboarding (or per project):

```
Company settings → Calendar mode: [ BYO | Pro-provided | Hybrid ]
```

**Hybrid example:** Company calendar BYO + each new project gets a Pro-provided project calendar.

---

## Mode comparison

| | BYO | Pro-provided |
|--|-----|--------------|
| **GC effort at setup** | Pick existing calendars | One click; app creates structure |
| **Calendar sprawl** | Uses what they have | App creates many calendars (named, archived) |
| **Sub sharing** | GC may need to share manually if BYO misconfigured | App auto-ACL on project calendar |
| **Leaving ContractorPro** | Their calendars remain theirs | Calendars remain in Google; ownership transfer TBD |
| **Cascade target** | Linked calendar(s) | Pro-provided `google_calendar_id` per entity |
| **MVP complexity** | Medium (which calendar? conflicts?) | Medium (create + ACL automation) |

**Product principle:** Same as auth — **bring your own OR we provide one**. No forced migration away from existing Google setup.

---

## What users should experience

| Person | Google Calendar | ContractorPro |
|--------|-----------------|-----------------|
| **GC staff** | See project events on phone/desktop Google app; personal + work calendars together | Project timeline, cascade, manage shared project calendars |
| **Sub** | Project tasks appear on **their** Google Calendar (if connected) | Sub portal: their slice + messages |
| **Homeowner** | Optional: key milestones on **their** Google Calendar | Homeowner portal: simple schedule + changes |

**Same data, two lenses:**
- Google = *“What’s on my day?”*
- ContractorPro = *“What’s happening on this job, and who needs to move when it slips?”*

---

## Shared calendars — conceptual model

Works with **either BYO or Pro-provided** mode above.

### Likely calendar types (draft)

| Calendar | Owner | Who sees it | Managed by |
|----------|-------|-------------|------------|
| **Company master** | GC org | GC staff | App (on connect) |
| **Per-project** | GC or service account | GC + invited subs + optional homeowner | App when project created |
| **Per-sub / trade slice** | Optional subcalendar or filtered view | That sub + GC | App from task assignments |
| **Personal** | Each user’s Google account | That user only | Google (not created by app) |

**Open product question:** One shared calendar **per project**, or one calendar per project **phase**, or events on company calendar with project tags?

### App-managed sharing

When GC adds a sub to a project, the app should:

1. Grant calendar access (ACL) or invite to shared project calendar
2. Create/update events for assigned tasks
3. On **cascade**, shift Google events and notify via app + optional SMS

User should **not** have to manually share calendars in Google UI for routine workflow.

---

## Technical building blocks (Google Calendar API)

| Capability | API concept | ContractorPro use |
|------------|-------------|-------------------|
| **OAuth per user** | User grants `calendar` scope | GC/subs connect Google; app acts on their behalf for their visibility |
| **Create calendars** | `calendars.insert` | New project → new shared calendar |
| **Share calendars** | `acl.insert` | Sub/homeowner email → reader or writer role |
| **Events** | `events.insert/update` | Tasks map to events; cascade = batch update |
| **Sync** | `events.list` + sync tokens / webhooks | Detect changes made **in Google** and reflect in app |
| **Push notifications** | Google Calendar push channels | Webhook when event changed externally |

Docs: [Google Calendar API](https://developers.google.com/calendar/api/guides/overview)

---

## Sync direction (critical design choice)

| Mode | Behavior | Fit |
|------|----------|-----|
| **One-way: App → Google** | ContractorPro is source of truth; writes events | Simplest MVP; cascade easy |
| **One-way: Google → App** | Import external changes | Weak for cascade ownership |
| **Two-way** | Edits in either place propagate | **Matches dual-view intent** — harder |
| **Google-primary for personal blocks** | User moves personal stuff in Google only; project events locked or flagged | Reduces conflict |

**Draft lean for MVP:** App is **source of truth for project tasks**; writes to shared project calendar. **Optional** import of free/busy or read-only personal calendar later.

**Long-term:** Two-way for **project calendar events** with clear rules (e.g. only GC can drag dates that trigger cascade).

### Schedule confirmation gates calendar writes

Calendar sync is tied to **sub acceptance**, not GC propose. See [schedule-confirmation-workflow.md](./schedule-confirmation-workflow.md).

| Assignment status | Google Calendar action |
|-------------------|------------------------|
| `proposed` / `proposed_change` | **No** confirmed event write (GC sees pending in app) |
| `confirmed` (sub accepted) | `events.insert` or `events.patch` on project calendar |
| `declined` | No update to proposed date; GC notified |

On reschedule, sub calendar keeps **last confirmed date** until they accept the new proposal.

---

## Platform reality (planning assumption)

**Google Calendar** is the default personal and small-business calendar for subs, homeowners, and most GCs in the field.

**Microsoft 365 / Outlook Calendar** is primarily a **work** calendar — office staff on company email, less common for subs and homeowners. Integrate M365 **later** for enterprise-style GCs, not MVP.

| | Google Calendar | Microsoft Calendar |
|--|-----------------|-------------------|
| **Subs / trades** | Common (personal Gmail + phone) | Uncommon except corporate subs |
| **Homeowners** | Common | Rare for scheduling |
| **GC owner / field** | Very common | Mixed — may use both |
| **GC office manager** | Often yes | Often yes (work M365) |
| **MVP integration** | ✅ Yes | ❌ Defer |
| **Auth (sign-in)** | ✅ | ✅ Optional (work Microsoft account) |

**Sign-in ≠ calendar sync:** A GC can **log in with Microsoft** (Entra) while project calendars still target **Google** for shared project visibility. Dual Microsoft (auth + calendar) only needed when customer research demands it.

---

| User type | Google account? | Integration |
|-----------|-----------------|-------------|
| GC staff | Usually yes | Full OAuth connect; manage shared calendars |
| Sub | Often yes | Connect OR receive calendar invite to email (reader without OAuth) |
| Homeowner | Sometimes | Invite to calendar or app-only view |

**Not everyone has Google** — magic-link portal remains required. Calendar integration is **enhancement**, not sole access path.

### OAuth scopes (planning)

- `openid email profile` — sign-in (may share Google Cloud project with auth)
- `https://www.googleapis.com/auth/calendar` — read/write calendars (sensitive scope → Google verification for production)
- Consider narrower scope if possible for read-only invitees

**Google verification:** Sensitive scopes require app review before public launch — budget time/cost.

---

## Data model sketch (app side)

```
companies
  id, name, calendar_mode (byo | pro_provided | hybrid), ...

google_connections
  user_id, company_id?, google_sub, refresh_token_encrypted, scopes, connected_at

calendars
  id, company_id, project_id?, entity_type (company|project|phase)
  provisioning (byo | pro_provided)
  google_calendar_id, linked_by_user_id?, summary, managed_by_app
  external_calendar_url?   -- BYO: which existing calendar was linked

calendar_acl
  calendar_id, email, role (reader|writer), google_acl_id

tasks
  id, project_id, ..., google_event_id, calendar_id

sync_state
  calendar_id, sync_token, last_webhook_at
```

**BYO:** `provisioning=byo`, `linked_by_user_id` set, events sync to `google_calendar_id` they chose.  
**Pro-provided:** `provisioning=pro_provided`, app created calendar on project/entity create.

**Cascade engine:** Update `tasks` → resolve `calendar_id` (BYO or pro-provided) → batch `events.patch` on Google → audit → notify.

---

## Conflict scenarios (need rules)

| Scenario | Question |
|----------|----------|
| Sub drags event in Google | Does it trigger cascade? Or snap back? Or ask GC to approve? |
| GC edits in app vs Google | Last-write-wins or app wins? |
| Event deleted in Google | Delete task or orphan? |
| Sub removed from project | Revoke ACL + remove future events? |
| Project archived | Calendar read-only or deleted? |
| User disconnects Google | Keep app schedule; stop sync? |

Document answers in cascade deep-dive — calendar and cascade are **one system**.

---

## “Lots of shared calendars” — scale notes

| Concern | Mitigation |
|---------|------------|
| Calendar sprawl (1 per project × 100 projects) | Naming convention; archive; folder in Google via naming |
| API quotas | Batch requests; exponential backoff |
| Token refresh | Store encrypted refresh tokens; Key Vault on Azure |
| ACL limits | Google calendar ACL limits per calendar — verify current quotas |
| Sub with 10 GCs | Multiple shared calendars on their Google — acceptable? |

---

## MVP vs later

### MVP (minimum credible Google integration)

- [ ] GC connects Google (OAuth) — required for both modes
- [ ] Company chooses **BYO or Pro-provided** (start with one mode if needed)
- [ ] **Pro-provided:** create **one shared calendar per project** on project create
- [ ] **BYO:** GC selects existing calendar to link per company or per project
- [ ] Tasks → events (app → Google) **on sub accept** — see [schedule-confirmation-workflow.md](./schedule-confirmation-workflow.md)
- [ ] Cascade proposes new dates; calendar patches **per sub accept**, not on GC save alone
- [ ] Subs invited by **email ACL** (read) even without OAuth
- [ ] App shows same schedule (from DB; Google as sync target)

### Later

- [ ] Sub OAuth — events on their personal calendar overlay
- [ ] Two-way sync + webhooks
- [ ] Homeowner calendar invite for milestones
- [ ] Microsoft 365 calendar (Graph API) — **deferred**; work-context GCs only; validate in customer discovery
- [ ] Free/busy overlay for scheduling conflicts

---

## Open questions — product

- [ ] **MVP: ship BYO, Pro-provided, or both?** (Pro-provided may be simpler for sub ACL automation)
- [ ] Default recommendation at onboarding — which mode do we nudge?
- [ ] **Hybrid** per company — supported day one or phase 2?
- [ ] One calendar per project, or company calendar with project tags?
- [ ] **Leaving ContractorPro** — who keeps Pro-provided Google calendars?
- [x] Subs **confirm dates** via magic link (Accept/Decline); calendar ACL read-only — see [schedule-confirmation-workflow.md](./schedule-confirmation-workflow.md)
- [ ] Should homeowners get Google calendar invites or app-only?
- [ ] What shows in ContractorPro that **doesn’t** go to Google (internal notes)?
- [ ] Branding: `ContractorPro: 123 Main St` vs GC company name on Pro-provided calendars?

## Open questions — technical

- [ ] **Pro-provided calendars:** created under **GC’s OAuth** vs **ContractorPro service account**?
- [ ] BYO: map one calendar per project or one company calendar with many projects?
- [ ] Google Workspace vs consumer `@gmail.com` — both supported?
- [ ] Webhook endpoint hosting on Azure for push notifications
- [ ] Offline / sync lag acceptable (seconds vs minutes)?

---

## Relation to wedge

This reinforces **integrate, don’t replace**:

- Competitors bury scheduling inside their app.
- ContractorPro **manages** shared calendars but users **live in Google** day-to-day.
- Cascade + notifications still driven by ContractorPro; Google is the **shared visibility layer**.

Log decisions in [discovery-log.md](../discovery-log.md).
