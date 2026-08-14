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

- **Do not** send images via MMS by default (cost, complexity, carrier limits)
- SMS: *“New photo on 123 Main St — [link]”*
- Full image in web thread

---

## SMS relay: “virtual group member” (exploration)

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

**Verdict:** Strong product instinct (capture + low friction), but **literal group-chat takeover** should become **hub-aligned SMS relay**, not one project megagroup. Validate with GCs: *“Would you move your sub texts to a number we give you if everything got logged to the project?”*

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
