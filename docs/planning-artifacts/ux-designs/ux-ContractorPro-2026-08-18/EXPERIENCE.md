---
name: ContractorPro Experience
status: draft
updated: 2026-08-18
sources:
  - docs/planning-artifacts/prds/prd-ContractorPro-2026-08-15/user-journeys/
  - docs/planning-artifacts/technical-exploration/messaging-and-media.md
---

## Foundation

Multi-surface product: **desktop + mobile web** for contractors (Ryan/Macie); **mobile web magic links** for subs and customers; **native MMS** as primary field comm channel. Visual identity: [DESIGN.md](./DESIGN.md).

## Information Architecture

| Persona | Primary surfaces | Key entry |
|---------|------------------|-----------|
| Contractor | Dashboard, project detail, cascade editor, MMS mirror | Web app login |
| Sub | Accept/decline, optional calendar link | SMS magic link |
| Customer | Channel confirm, schedule timeline, milestone accept | Email + MMS magic links |

## Voice and Tone

- SMS/MMS: short, contractor-branded prefix `[Riverside Remodeling]`, project name, one action
- App: direct operational language — "3 pending confirmations", not "You have outstanding items"
- Customer portal: plain language, no construction jargon

## Key Flows

### WF-0: Project creation + MMS group setup (C-1, C-13)

**Protagonist:** Ryan on laptop, Maci may assist

1. Create Maple St Kitchen — customer, tasks, project handle # auto-assigned
2. Customer gets email + MMS on save (no separate invite step)
3. Assign first sub → modal prompts MMS group creation with handle #

Mockups: [contractor-project-create-desktop.html](./mockups/contractor-project-create-desktop.html), [contractor-project-create-mobile.html](./mockups/contractor-project-create-mobile.html), [contractor-mms-group-setup-desktop.html](./mockups/contractor-mms-group-setup-desktop.html), [mms-group-created.png](./mockups/mms-group-created.png)

### WF-1: Sub delay → cascade → confirm (UJ-9)

**Protagonist:** Maci (office manager) with Ryan approving

1. Marcus texts delay in group MMS → ingested to app thread mirror
2. Maci opens Maple St, sees alert + MMS context
3. Maci drafts cascade (+3 days), previews affected subs + customer milestone
4. Ryan approves on phone; Maci publishes
5. Jesse + Marcus get SMS re-confirm; Lauren gets milestone MMS
6. Dashboard clears as each confirms

Mockups: [workflow-connected-overview.html](./mockups/workflow-connected-overview.html), [contractor-mms-mirror-desktop.html](./mockups/contractor-mms-mirror-desktop.html), [contractor-cascade-preview-desktop.html](./mockups/contractor-cascade-preview-desktop.html)

### WF-2: Sub first invite + confirm (S-1)

Jesse gets one SMS → taps link → join + accept date on one mobile screen.

Mockup: [sub-accept-decline-mobile.html](./mockups/sub-accept-decline-mobile.html)

### WF-3: Customer dual-channel connect (H-1)

Lauren confirms email and MMS independently; Ryan sees per-channel status.

Mockup: [customer-connect-mobile.html](./mockups/customer-connect-mobile.html)

### WF-4: Customer schedule visibility (H-21)

Rolling timeline — what changed, accept/reject pending milestones.

Mockup: [customer-timeline-mobile.html](./mockups/customer-timeline-mobile.html)

## Component Patterns

| Pattern | Behavior |
|---------|----------|
| Pending badge | ⏳ with day count; tap → send reminder or snooze |
| Cascade preview | Lists tasks moving, who must re-confirm, customer milestones |
| MMS mirror | Read-only thread in app; reply happens in native Messages |
| Magic-link screen | One primary action; no nav chrome; project context in header |

## State Patterns

- **Pending confirm:** amber badge, poke available
- **Confirmed:** green check, calendar sync note if linked
- **Channel gate:** customer notifications blocked until channel confirmed

## Accessibility Floor

Touch targets ≥44px on mobile magic links. Status never color-only — icon + text label.
