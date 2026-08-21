---
name: ContractorPro Experience
status: draft
updated: 2026-08-20
sources:
  - docs/planning-artifacts/prds/prd-ContractorPro-2026-08-15/user-journeys/
  - docs/planning-artifacts/technical-exploration/company-number-messaging.md
  - docs/planning-artifacts/sprint-change-proposal-2026-08-20.md
---

## Foundation

Multi-surface product: **desktop + mobile web** for contractors (Ryan/Macie); **mobile web magic links** for subs and customers; **1:1 SMS to company #** + **SMS relay** + **app inbox** for field comms (replaces per-project group MMS). Visual identity: [DESIGN.md](./DESIGN.md).

## Information Architecture

| Persona | Primary surfaces | Key entry |
|---------|------------------|-----------|
| Contractor | Planning workspace, shared inbox, cascade, dashboard | Web app login |
| Sub | Accept/decline, QR resource portal, optional calendar link | SMS magic link · QR scan |
| Customer | Prelim schedule, channel confirm, timeline, milestone accept | Email + SMS magic links |

## Voice and Tone

- SMS: short, project prefix (`Maple St · Riverside`), one action — magic link for confirms (not "reply YES")
- App: direct operational language — "3 pending confirmations", "1 unassigned thread"
- Customer portal: plain language; prelim banner warns dates are draft

## Key Flows

### WF-0: Project create → plan → prelim → finalize (C-1, E13)

**Protagonist:** Ryan on laptop, Maci assists

1. Create Maple St in **planning** status — customer + tasks; no sub SMS yet
2. Apply template; **planning calendar** with overlay + sub conflicts
3. **Publish prelim** → Lauren sees read-only draft schedule
4. **Finalize & cascade config** → first wave from Acme Co #
5. **Project files** — upload docs, print QR sheet for job site

Mockups: [contractor-project-create-desktop.html](./mockups/contractor-project-create-desktop.html), [contractor-planning-workspace-desktop.html](./mockups/contractor-planning-workspace-desktop.html), [contractor-planning-calendar-desktop.html](./mockups/contractor-planning-calendar-desktop.html), [contractor-finalize-cascade-desktop.html](./mockups/contractor-finalize-cascade-desktop.html), [customer-prelim-mobile.html](./mockups/customer-prelim-mobile.html), [contractor-project-files-desktop.html](./mockups/contractor-project-files-desktop.html), [printable-job-site-qr-sheet.html](./mockups/printable-job-site-qr-sheet.html), [contractor-company-number-desktop.html](./mockups/contractor-company-number-desktop.html)

### WF-0b: Google connect (E3-S1)

Onboarding: OAuth for Calendar + Drive; explains what syncs only on sub accept.

Mockup: [contractor-google-calendar-connect-desktop.html](./mockups/contractor-google-calendar-connect-desktop.html)

### WF-0c: Template editor (E13-S1, FR-22)

Company-level kitchen/bath templates: phases, durations, default invite waves. Per-job override at finalize.

Mockups: [contractor-cascade-template-editor-desktop.html](./mockups/contractor-cascade-template-editor-desktop.html) → [contractor-finalize-cascade-desktop.html](./mockups/contractor-finalize-cascade-desktop.html)

### WF-0d: Portfolio calendar (E3-S3)

Live multi-job view after projects are active — pending confirms highlighted.

Mockup: [contractor-portfolio-calendar-desktop.html](./mockups/contractor-portfolio-calendar-desktop.html)

### WF-1: Sub delay → inbox → cascade → confirm (UJ-9)

**Protagonist:** Maci with Ryan on SMS relay

1. Marcus texts **Acme Co #** → shared inbox + SMS relay to Ryan/Macie
2. Maci reads thread in app (or relay on phone); replies to Acme # or in app
3. Maci drafts cascade; previews affected subs + customer milestone
4. Publish → magic links from company #; Lauren prep comm (FR-24)
5. Dashboard clears as each confirms

Mockups: [workflow-connected-overview.html](./mockups/workflow-connected-overview.html), [contractor-inbox-desktop.html](./mockups/contractor-inbox-desktop.html), [contractor-cascade-preview-desktop.html](./mockups/contractor-cascade-preview-desktop.html)

### WF-2: Sub first invite + confirm (S-1)

Jesse gets SMS from Acme Co # → taps link → join + accept date.

Mockup: [sub-accept-decline-mobile.html](./mockups/sub-accept-decline-mobile.html)

### WF-3: QR job site (E14)

Marcus scans laminated QR → resource page → check in → EOD photo upload to Drive via app.

Mockup: [sub-resource-portal-mobile.html](./mockups/sub-resource-portal-mobile.html)

### WF-4: Customer dual-channel connect (H-1)

Lauren confirms email and SMS independently.

Mockup: [customer-connect-mobile.html](./mockups/customer-connect-mobile.html)

### WF-5: Customer schedule visibility (H-21)

Rolling timeline + milestone accept.

Mockup: [customer-timeline-mobile.html](./mockups/customer-timeline-mobile.html)

## Component Patterns

| Pattern | Behavior |
|---------|----------|
| Planning badge | Dashed timeline; Publish prelim + Finalize as separate primary actions |
| Shared inbox | Thread list + filters; orphan assign queue; reply in app or SMS relay |
| Cascade preview | Tasks moving, who must re-confirm, customer milestones |
| QR resource page | Check in + upload; doc list; no drive.google.com |
| Printable QR sheet | Letter-size laminate; check-in + EOD upload steps; Acme # for questions |
| Project files (GC) | Upload, visibility badges, Drive sync status, print QR |
| Planning calendar | Dashed plan bars + live job overlay + conflict panel |
| Magic-link screen | One primary action; no nav chrome; project context in header |

## Coverage matrix (2026-08-20)

| Area | Mockup(s) | Still thin (RC / backlog) |
|------|-----------|---------------------------|
| GC add/view files | project-files-desktop | Drive preview UX (SP-3) |
| Planning list + calendar | planning-workspace, planning-calendar | Portfolio balance panel (RC-6) |
| Finalize cascade | finalize-cascade-desktop | Per-job wave override |
| Template editor | cascade-template-editor-desktop | Drag-drop wave builder |
| Google connect | google-calendar-connect-desktop | OAuth error / reconnect |
| Live portfolio calendar | portfolio-calendar-desktop | Google Calendar embed |
| QR laminate sheet | printable-job-site-qr-sheet | Two-QR vs one-QR (RC-1) |
| Sub portal after scan | sub-resource-portal-mobile | OTP on first bind |

## State Patterns

- **Planning:** no outbound comms; subs assigned but not invited
- **Pending confirm:** amber badge, poke available
- **Orphan thread:** warn badge until assigned to project
- **Prelim published:** customer sees draft banner

## Accessibility Floor

Touch targets ≥44px on mobile magic links. Status never color-only — icon + text label.

## Archived mockups (2026-08-20)

Group MMS setup/mirror replaced by company # onboarding + shared inbox. Old files redirect:
- `contractor-mms-group-setup-desktop.html` → `contractor-company-number-desktop.html`
- `contractor-mms-mirror-desktop.html` → `contractor-inbox-desktop.html`

MMS PNG assets retained for SMS appearance reference only — flows updated in workflow overview.
