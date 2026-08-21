# SME Mockup Pack — Ryan & Maci Review

**Status:** Updated for correct-course (2026-08-20)  
**Created:** 2026-08-18 · **Updated:** 2026-08-20  
**Audience:** Ryan (contractor/owner) + Maci (office manager)  
**Goal:** Validate *"is this how you work?"* — plan-first, company #, shared inbox, QR resources

> **Browse in browser:** Open <a href="index.html" target="_blank">index.html</a>  
> **System overview:** <a href="mockups/system-overview.html" target="_blank">system-overview.html</a>  
> From terminal: `start index.html`

---

## How to use this pack

0. **Context:** <a href="mockups/system-overview.html" target="_blank">system-overview.html</a> — company # + plan + QR (updated 2026-08-20)
1. **Onboarding:** <a href="mockups/contractor-project-create-desktop.html" target="_blank">project create</a> → <a href="mockups/contractor-planning-workspace-desktop.html" target="_blank">planning workspace</a>
2. **Company #:** <a href="mockups/contractor-company-number-desktop.html" target="_blank">company number setup</a> (replaces group MMS setup)
3. **Comms:** <a href="mockups/contractor-inbox-desktop.html" target="_blank">shared inbox</a> + <a href="mockups/workflow-connected-overview.html" target="_blank">full workflow</a>
4. **Job site:** <a href="mockups/sub-resource-portal-mobile.html" target="_blank">QR resource portal</a>
5. **Log decisions** in [discovery-log.md](../../discovery-log.md)

---

## Company number — planning note (for facilitators)

**One Twilio 10DLC number per company** (not per project). Subs/customers text Acme Co; staff use SMS relay + app inbox.

| What | Rough cost (Twilio, US) |
|------|-------------------------|
| Company number | ~**$1.15/mo** |
| Messaging volume | ~**$4–15/mo** typical (depends on traffic) |
| 5 active jobs | **Not** 5× number rent — one line serves all |

Architecture: [company-number-messaging.md](../../technical-exploration/company-number-messaging.md)

**SME prompts (SP-4):** Will you reply to Acme # instead of Marcus's cell? Can Maci follow relay forwards?

---

## Mockup inventory

### Connected workflow (start here)

| File | Surface | Journey |
|------|---------|---------|
| <a href="mockups/system-overview.html" target="_blank">system-overview.html</a> | Architecture | Plan · Talk · Commit |
| <a href="mockups/workflow-connected-overview.html" target="_blank">workflow-connected-overview.html</a> | Overview | Full walkthrough |

### Contractor — Ryan / Maci

| File | Form factor | Journey |
|------|-------------|---------|
| <a href="mockups/contractor-project-create-desktop.html" target="_blank">contractor-project-create-desktop.html</a> | Desktop | C-1 (planning mode) |
| <a href="mockups/contractor-planning-workspace-desktop.html" target="_blank">contractor-planning-workspace-desktop.html</a> | Desktop | E13 · phase list |
| <a href="mockups/contractor-planning-calendar-desktop.html" target="_blank">contractor-planning-calendar-desktop.html</a> | Desktop | E13 · overlay calendar |
| <a href="mockups/contractor-cascade-template-editor-desktop.html" target="_blank">contractor-cascade-template-editor-desktop.html</a> | Desktop | E13-S1 · company templates |
| <a href="mockups/contractor-finalize-cascade-desktop.html" target="_blank">contractor-finalize-cascade-desktop.html</a> | Desktop | E13-S6 · per-job override |
| <a href="mockups/contractor-google-calendar-connect-desktop.html" target="_blank">contractor-google-calendar-connect-desktop.html</a> | Desktop | E3-S1 · Calendar + Drive OAuth |
| <a href="mockups/contractor-portfolio-calendar-desktop.html" target="_blank">contractor-portfolio-calendar-desktop.html</a> | Desktop | E3-S3 · all jobs |
| <a href="mockups/contractor-project-files-desktop.html" target="_blank">contractor-project-files-desktop.html</a> | Desktop | E14 · GC files + upload |
| <a href="mockups/printable-job-site-qr-sheet.html" target="_blank">printable-job-site-qr-sheet.html</a> | Print / PDF | E14-S3 · laminate sheet |
| <a href="mockups/contractor-company-number-desktop.html" target="_blank">contractor-company-number-desktop.html</a> | Desktop modal | E8-S1 |
| <a href="mockups/contractor-inbox-desktop.html" target="_blank">contractor-inbox-desktop.html</a> | Desktop | E8 · inbox |
| <a href="mockups/contractor-cascade-preview-desktop.html" target="_blank">contractor-cascade-preview-desktop.html</a> | Desktop | C-20 live reschedule |
| <a href="mockups/contractor-dashboard-desktop.html" target="_blank">contractor-dashboard-desktop.html</a> | Desktop | C-6, C-19 |

### Sub — Jesse / Marcus

| File | Form factor | Journey |
|------|-------------|---------|
| <a href="mockups/sub-accept-decline-mobile.html" target="_blank">sub-accept-decline-mobile.html</a> | Mobile portal | S-1 |
| <a href="mockups/sub-resource-portal-mobile.html" target="_blank">sub-resource-portal-mobile.html</a> | Mobile · QR | E14 · check-in/out |

### Customer — Lauren

| File | Form factor | Journey |
|------|-------------|---------|
| <a href="mockups/customer-prelim-mobile.html" target="_blank">customer-prelim-mobile.html</a> | Mobile portal | E13-S4 prelim |
| <a href="mockups/customer-connect-mobile.html" target="_blank">customer-connect-mobile.html</a> | Mobile portal | H-1 |
| <a href="mockups/customer-timeline-mobile.html" target="_blank">customer-timeline-mobile.html</a> | Mobile portal | H-21 |

### Retired / redirect

| File | Notes |
|------|-------|
| contractor-mms-group-setup-desktop.html | → company-number-desktop |
| contractor-mms-mirror-desktop.html | → inbox-desktop |
| mms-*.png | Legacy SMS appearance refs only |

---

## SME discussion prompts

### Planning (new)
- Is template → overlay → prelim → finalize the right sequence?
- Two buttons (publish prelim vs finalize) — clear enough?
- How long can a job sit in planning?
- **Template editor:** Kitchen/bath/whole-home enough? Default invite waves make sense?

### Google connect
- Calendar + Drive in one OAuth — OK?
- Planning dates stay out of Google until subs accept — clear?

### Company # + inbox
- Will you text Acme # instead of subs' personal cells?
- Is shared inbox enough for Maci without asking Ryan?
- Orphan assign when Jose texts about wrong job — workable?

### QR resources
- Single QR with Check in + Upload — OK? (RC-1)
- Laminated at threshold — matches your workflow?

### Cascade + portals
- Magic links only (no reply YES) — fine for subs?
- Customer prelim — enough detail or too skeletal?

---

## What's NOT in this pack (deferred)

- Sub roster management (C-4, C-5)
- Full Drive admin / doc upload UX for Ryan
- Interactive prototypes
- Customer approval gate before subs (2B SME pending)

Thin spots: backlog RC-1–RC-7

---

## Design spines

- [DESIGN.md](./DESIGN.md)
- [EXPERIENCE.md](./EXPERIENCE.md)

---

## Change log

| Date | Change |
|------|--------|
| 2026-08-18 | Initial SME pack |
| 2026-08-20 | Correct-course: company #, planning, inbox, QR; retired group MMS mockups |
| 2026-08-20 | Added: GC files, planning calendar, portfolio calendar, printable QR sheet, finalize cascade, Google connect, cascade template editor |
