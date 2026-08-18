# Messaging & Media (Images) — Exploration

Status: **Exploratory** (2026-08-13)  
Related: [product-vision.md](../product-vision.md), [nosql-vs-relational.md](./nosql-vs-relational.md), [stack-web-api-db.md](./stack-web-api-db.md)

## Product intent (from stakeholder)

> A lot of messaging will include **many images/pictures** for communication between the three main user types: **contractor (GC)**, **subcontractor**, and **customer (homeowner)**.

This is expected field behavior — jobsite photos, progress pics, “what broke,” selections, punch-list evidence — not an edge case.

---

## Implications at a glance

| Area | Impact |
|------|--------|
| **Primary database** | Still **relational** — store message **metadata**, not image bytes |
| **Blob storage** | **Required** — Azure Blob Storage (fits Azure lean) |
| **NoSQL** | Still not primary DB; blobs are object storage, not Mongo/Cosmos documents |
| **SMS** | Text + link only; images viewed in **web portal** (bandwidth/cost) |
| **Permissions** | Same thread rules — homeowner never sees sub-only images |
| **Cost / tiers** | Storage + egress may affect **subscription limits** |
| **MVP scope** | Images in GC↔sub and GC↔homeowner threads; define limits early |

---

## Architecture pattern

```
┌─────────────┐     upload      ┌──────────────────┐
│ Web client  │ ──────────────▶ │ .NET API         │
│ (GC/sub/    │                 │ validate, virus  │
│  homeowner) │ ◀────────────── │ scan?, resize    │
└─────────────┘     URL         └────────┬─────────┘
                                         │
                    ┌────────────────────┼────────────────────┐
                    ▼                    ▼                    ▼
            ┌──────────────┐    ┌──────────────┐    ┌──────────────┐
            │ PostgreSQL / │    │ Azure Blob   │    │ (optional)   │
            │ SQL Server   │    │ Storage      │    │ CDN / thumbs │
            │ messages,    │    │ originals +  │    │              │
            │ attachments  │    │ thumbnails   │    │              │
            └──────────────┘    └──────────────┘    └──────────────┘
```

**Rule:** Database rows are small and queryable; **bytes live in blob storage**.

---

## Data model (sketch)

```
messages
  id, thread_id, project_id, sender_user_id
  body_text (nullable)
  audience (homeowner | sub:{id})  -- same visibility as text threads
  created_at

message_attachments
  id, message_id
  blob_container, blob_path
  content_type, byte_size
  width, height (optional)
  thumbnail_blob_path (optional)
  original_filename
```

**Threads** remain split:

| Thread | Participants | Images |
|--------|--------------|--------|
| GC ↔ homeowner | GC + homeowner | Progress, delays, selections, **homeowner questions/issues** |
| GC ↔ sub | GC + that sub | Site conditions, scope, punch items |

Subs and homeowners **do not** share a thread; image ACL follows message ACL.

---

## User experience notes

### GC (contractor)

- Camera upload from phone browser or desktop
- Multiple images per message (common on site)
- Attach to project + thread automatically

### Subcontractor

- Magic-link portal — **must work well on mobile** for photo upload
- See images on **their** GC threads only

### Homeowner

- **Upload and view images** — first-class, not read-only (questions, concerns, progress confirmation)
- Magic-link portal — **must work well on mobile** for camera upload
- Simpler UI than GC — milestone photos, issues, approvals
- Sees only **GC ↔ homeowner** thread images — never sub-only content

### SMS

- **v0.1:** Group **MMS** is primary channel for GC↔sub and GC↔customer (see § MMS group threads)
- System-generated messages (propose, poke, confirm links) sent via MMS/SMS into the thread
- Web portal: read mirror + Dana schedule actions; not required for subs to chat

---

## MMS group threads + project handle (v0.1 decision — 2026-08-17)

**Decision:** Field communication runs through **native group MMS** on the phone, not through the web app as primary UI. The web app **records** conversations and is where Dana **commits** schedule changes.

### Two lanes (2026-08-17)

| Lane | Where | Owner |
|------|-------|-------|
| **General conversation** | Group MMS → ingested to app | Everyone texts naturally |
| **Scheduling** | Web app (portfolio, tasks, cascade) | Dana — contractor controls the schedule |

Scheduling does not happen in MMS threads — too messy across multiple jobs/teams. MMS may *trigger* a schedule action (sub says they're delayed); Dana executes in the app; system sends confirm links via MMS.

### Pattern

Each **project × relationship** gets a group MMS thread:

```text
[Dana / Riverside]  +  [Mike the painter]  +  [ContractorPro project handle #]
```

Same for each sub and each customer — **separate groups**, not one project megachat. Dana coordinates between threads.

| Role | What they do |
|------|----------------|
| **Dana (contractor)** | Creates group text with sub + project handle #; decision maker; reads MMS; **reschedules in web app** when needed |
| **Sub / customer** | Texts in the group Dana started (or was added to); e.g. "can't start on time" |
| **ContractorPro handle #** | Virtual participant — ingests MMS (text + images); mirrors to project in web app |
| **System** | After Dana acts in app (propose, reschedule, cascade), sends **confirmation MMS/SMS** into the thread or with magic link |

### Example flow (flooring slip)

1. Dana previously started group: Dana + flooring guy + Maple St handle #
2. Flooring guy texts group: *"Can't start Thursday — supplier delay"*
3. Message logged in app under Maple St / flooring thread
4. Dana opens **web app**, moves task, triggers reschedule (UJ-2a)
5. System sends MMS to group: *`[Maple St] Flooring moved to next Tuesday. Confirm: [link]`*
6. Sub confirms via link; calendars sync

**Negotiation is often in MMS; commitment is in the app + confirm link.**

### What this is NOT

| Not this | Why |
|----------|-----|
| App-orchestrated chat (subs use portal first) | Subs live in Messages |
| One group per whole project | Privacy — subs don't see each other |
| AI parsing "yes" from MMS in v0.1 | Deferred — Dana reads and acts manually |
| Inject into Dana's **existing** iMessage group | Must be a **new** group with handle # |

### Project handle # (routing — locked 2026-08-17)

Each **project** gets its own ContractorPro phone number — the **project handle**. All group MMS on that job use the same handle; each **relationship** still has its own group (Dana + Mike + Maple#, Dana + Jose + Maple#).

| Inbound webhook | Maps to |
|-----------------|---------|
| `To` = Maple handle # | `project_id` (Maple St) |
| `From` = Mike's phone | `membership_id` (Mike, Sub on Maple) |

**Why per-project number (not one company number):** Same sub on Maple + Oak needs different handles so messages don't collapse into one thread. iPhone does not expose its thread id to our API — project is not embedded in the MMS payload.

**Thread id:** When a relationship group is provisioned, store platform `conversation_sid` (Twilio Conversations) or internal `mms_thread_id` → `(project_id, membership_id)`. Used for dedupe, audit, and outbound routing — not a substitute for per-project `To` on inbound.

**Branding:** Outbound system messages also prefix `[Maple St · Riverside]` for humans; routing uses the number.

**On project create:** Provision Maple handle # and show Dana in onboarding:

```text
Maple St group text — add with each sub/customer:
  [Sub phone]
  Maple St: (555) 100-0001
```

### Data model (sketch)

```text
projects
  id, handle_phone_e164, ...     -- one number per project

mms_threads
  id, project_id, membership_id
  conversation_sid?              -- Twilio Conversations id when provisioned
  handle_phone_e164              -- denormalized from project
  created_at

messages
  id, mms_thread_id, project_id, membership_id
  direction (inbound|outbound), body, provider_message_sid, ...
```

Dana is always the contractor on record and owner of the project.

### Technical implementation (lean)

| Piece | Approach |
|-------|----------|
| Group MMS | Twilio Group MMS or Conversations API — handle # is a participant — see [project-handle-numbers.md](./project-handle-numbers.md) for vendor, pooling, costs |
| Inbound | Webhook → store message + attachments → blob storage |
| Outbound | System messages + poke reminders sent into thread or 1:1 from handle |
| Images | **MMS photos ingested** into thread in app (v0.1); also viewable in web |
| Web mirror | Dana can read full thread in dashboard; optional reply from web → MMS to group |
| Compliance | A2P 10DLC; opt-in when added to group |

### Known constraints (accept, don't fight in v0.1)

- iPhone users see **green bubble** (not iMessage blue) for handle #
- Group MMS: carrier limits (~10 participants), higher cost than SMS
- Dana must **create** the group — can't silently monitor old threads
- Dual-send: text in MMS + post in web → dedupe or show both `[OPEN]`

---

## SMS relay: “virtual group member” (exploration — superseded by MMS decision above)

> **Note:** Pre-2026-08-17 exploration below. v0.1 follows **MMS group threads** per relationship + project handle #. Relay-per-thread concepts still apply; delivery is **group MMS**, not notify-only.

**Idea:** ContractorPro acts as an extra participant in SMS conversations. GC, subs, and homeowners keep using their normal Messages app; ContractorPro logs everything and mirrors it in the web UI. Users can reply via SMS **or** the portal — same thread.

### What’s actually possible

You **cannot** silently inject into an **existing** iMessage/SMS group on someone’s phone. SMS has no bot API like Slack. What you **can** do:

| Pattern | How it works | Feels like |
|---------|--------------|------------|
| **Relay number per thread** | GC creates contact “123 Main – Tile” = Twilio number. Participants text it; API routes by sender phone → correct thread. Outbound from web → SMS to participants. | 3-way chat with an invisible “app” member |
| **Hosted group (Twilio Conversations / Group MMS)** | Platform hosts the group; everyone texts one number. | Real group chat, but **new** thread — not their old one |
| **Notify-only (current plan)** | SMS = alert + link; conversation lives in web | Lowest SMS cost; more portal friction |

**Practical MVP shape:** **relay number per conversation** (not one giant project group), aligned with GC-as-hub:

```
GC ↔ Sub relay:     [GC phone] [Sub phone] [ContractorPro #]
GC ↔ Homeowner:     [GC phone] [HO phone]  [ContractorPro #]
```

ContractorPro is always the third party. Messages in = logged + visible in web. Messages from web = SMS to the humans. Same audit trail either way.

### Why this is attractive

- **Meet users where they are** — subs and GCs already live in text; no behavior change required
- **Full capture** — schedule context, disputes, “he said / she said” — searchable project record
- **Dual channel** — SMS for speed in the field; web for photos, history, and homeowner-friendly UI
- **Cascade tie-in** — auto-SMS when schedule moves, replies stay in the same logged thread

### Hard problems

| Issue | Detail |
|-------|--------|
| **Privacy vs one big group** | A single project group chat exposes sub ↔ homeowner chatter — **conflicts with hub model**. Relay-per-thread preserves ACL. |
| **Images** | Group MMS for photos is **expensive, unreliable, and carrier-limited**. Keep images on web; SMS = “new photo — [link]” even with relay. |
| **iMessage quirks** | Twilio numbers are always “green bubble”; iPhone users may see a **separate** thread from their old group. |
| **Identity** | Map phone numbers → users; handle shared phones, wrong numbers, GC staff with multiple phones. |
| **Dual-send** | User texts SMS *and* posts in web → dedupe or show both? |
| **Cost** | Every SMS segment in **and** out is billed (~$0.01+). Chatty subs × many projects × tiers. Group MMS costs more. |
| **Compliance** | US **A2P 10DLC** registration (Twilio/ACS); opt-in consent for automated texts; TCPA. |
| **“Use our group or yours”** | GC may already have a messy 5-person group — ask in customer discovery: adopt ContractorPro thread or keep parallel? |

### Suggested hybrid (if pursued)

| Layer | MVP behavior |
|-------|----------------|
| **Default** | Web-first threads + SMS **notifications** with deep link (cheap, simple) |
| **Opt-in relay** | Per thread: “Enable SMS texting for this conversation” → provision relay #, GC invites sub/HO to text it |
| **Images** | Always web upload; SMS only nudges |
| **GC dashboard** | Unified inbox — whether message arrived via SMS or web |

**Verdict (2026-08-17):** v0.1 = **MMS group per relationship** + project handle #; hub model preserved; AI deferred.

---

## Storage & processing (Azure-aligned)

| Component | Suggested approach |
|-----------|-------------------|
| **Storage** | Azure Blob Storage — hot tier for recent; cool/archive later |
| **Upload** | API direct upload or **SAS URL** (client → blob, API records metadata) |
| **Thumbnails** | Generate on upload (ImageSharp / Azure Function) — faster mobile feed |
| **Max file size** | e.g. 10–25 MB per image (configurable per tier) |
| **Formats** | JPEG, PNG, HEIC (convert HEIC → JPEG for web) |
| **Virus scan** | Optional MVP+ (Defender for Storage, ClamAV) |
| **CDN** | Azure CDN in front of blobs when traffic grows |

### Container layout (example)

```
/{company_id}/{project_id}/messages/{message_id}/{attachment_id}.jpg
/{company_id}/{project_id}/messages/{message_id}/{attachment_id}_thumb.jpg
```

---

## Cost & subscription tiers

Images drive **storage** and **egress** — may need tier limits:

| Tier | Draft limit idea |
|------|------------------|
| **Free** | e.g. 1 GB storage / project or company |
| **Starter** | e.g. 25–50 GB |
| **Pro** | Higher cap or “fair use” |

Track per `company_id` for billing alignment with Chargebee/Stripe.

**Not in relational DB** — avoids 50 GB SQL Express problem and keeps backups small.

---

## Why not store images in SQL or NoSQL

| Approach | Problem |
|----------|---------|
| **BYTEA / varbinary in Postgres** | DB bloat, slow backups, expensive queries |
| **MongoDB documents with embedded images** | Same bloat; wrong tool |
| **NoSQL as primary for “flexible messages”** | Still need relational threads, ACLs, projects |

**Blob + SQL metadata** is the standard pattern (CompanyCam, Slack, iMessage backends, etc.).

---

## Relation to other features

| Feature | Interaction |
|---------|-------------|
| **Cascade / schedule** | Text + optional image in “schedule moved” message |
| **Google Calendar** | Calendar events ≠ message images; separate systems |
| **AI** | Later: caption/summarize images (“water damage in basement photo”) — needs vision API ($) |
| **Daily logs** | May overlap with messaging — decide: message thread vs structured daily log (MVP: messages first) |

---

## MVP vs later

### MVP (include)

- [ ] Image upload on **all three roles**: GC, sub, homeowner (GC↔sub and GC↔homeowner threads)
- [ ] **Homeowner upload** — required v0.1, not read-only portal
- [ ] Images as part of **project tracking** timeline (not only chat scrollback)
- [ ] Azure Blob + `message_attachments` table
- [ ] Thumbnails for list view
- [ ] Mobile-friendly upload on magic-link portals
- [ ] Per-company storage quota (basic)

### Later

- [ ] Image markup (arrows, circles on photo)
- [ ] Before/after galleries per project
- [ ] Video short clips
- [ ] AI description / defect detection
- [ ] Export zip for dispute/warranty
- [ ] Integration with CompanyCam-style timeline (if ever)

---

## Open questions

- [ ] Max images per message? Per day per project?
- [ ] **Project tracking feed** — images from all parties on a single homeowner-visible timeline vs threads only? (Homeowner uploads confirmed for v0.1)
- [ ] Retention: delete images when project archived?
- [ ] HEIC from iPhones — server-side convert required?
- [ ] Offline upload queue for poor signal? (later)
- [ ] Watermark with project name/date automatically?
- [ ] Does image count toward **SMS** tier or only storage tier?

---

## Updated stack picture

```
React/Blazor web  →  ASP.NET Core API  →  SQL (messages, metadata)
                              ↓
                      Azure Blob Storage (images)
                              ↓
                      (optional) CDN, thumbnail worker
```

Relational DB choice (Postgres vs SQL Server) **unchanged** — both pair well with Azure Blob.

Log updates in [discovery-log.md](../discovery-log.md).
