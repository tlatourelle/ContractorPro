# SME Meeting 01 — Overview

> Structured summary from Pocket transcript + overview. Raw source: [raw-transcript.txt](./raw-transcript.txt).

## Purpose

Refine design and technical requirements for ContractorPro with the paying-customer SMEs (Ryan & Maci). Focus: automated schedule tracking vs. subcontractor tech limitations.

## Workflow (as discussed)

```text
Plan Project → Set Date → Send Sub Text
                              ├─ Approve  → Cascading Invites → QR Site Check-in → Google Drive Photos
                              └─ Reject   → Planner Reschedule
```

## Topics covered

### Software architecture & communication

- **Tom's proposal:** Each project gets a unique temporary Twilio number so the app can monitor and log all GC ↔ sub ↔ customer messaging.
- **Ryan's pushback:** Per-project threads are clunky; subs will ignore them and call directly. Prefers **one central company number** that routes to Ryan and Maci.
- **Compromise explored:** One number per contractor (not per project); approval links still work; lose full conversation logging in the app.
- **Approval cascades:** When a sub approves a date, automatically invite/notify the **next sub in sequence** (plumber → electrician). Buildertrend pain: "send all or nothing" forces manual re-entry.
- **Twilio cost:** ~$10/month per active project (number + texting). Subscription idea: **$100/mo for 5 active jobs**, **$200/mo for 10**.
- **Number ownership:** Open question — what happens to the number if they cancel the app?

### Job site operations & QR codes

- QR code at job site links to a **Google Drive bucket** (blueprints, permits, layout, scope of work).
- Subs scan to see resources and upload **progress photos** at end of day — replaces Company Cam / fixed cameras.
- Ryan: keep it simple; may not need software to host files if Drive works. Tom: software could generate QR + manage project docs.
- **Security:** Door codes in QR rejected; indoor placement at threshold acceptable.

### Customer integration

- Maci **beta-testing** a customer-facing calendar on one active job (separate Google account pattern today).
- Customers get a **read-only schedule window** — no editing rights.
- **Login-free access:** Magic link + one-time code (no passwords).
- **Milestone reminders:** Automated texts (e.g. "Tomorrow is demo day") with prep instructions.
- **Customer in approval sequence:** Maci wants customer to approve skeletal schedule first, then subs cascade one-by-one. Ryan: customer constraints captured at contract signing; ongoing per-task customer approval is tricky.

### Product scope & positioning

- **Lean scheduling only** — no estimates, financials, or full CRM (Buildertrend trap).
- **Ryan's "four things":** (1) sub calendar integration, (2) customer calendar view, (3) easy calendar building for GC, (4) sub confirmation tracking.
- **Plan-first workflow:** Start with job plan (templates, durations, buffers, what-if dates) — **not** calendar as backbone. Reschedule returns to planner, not manual calendar fiddling.
- **Google Calendar MVP only** — Apple/Outlook later.
- **AI rescheduling:** Interesting but deferred until product is profitable.

### Action items

| Owner | Item |
|-------|------|
| Ryan | Send work order screenshot samples to Tom |
| Tom | Verify Twilio number ownership/portability on churn |
| Tom | Analyze central-number routing to multiple team members (Ryan + Maci) without double-reply chaos |
