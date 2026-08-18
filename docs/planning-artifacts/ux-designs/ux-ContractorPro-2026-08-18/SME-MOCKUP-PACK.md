# SME Mockup Pack — Ryan & Maci Review

**Status:** Draft for planning validation  
**Created:** 2026-08-18  
**Audience:** Ryan (contractor/owner) + Maci (office manager) — same v0.1 access  
**Goal:** Validate *"is this how you work?"* — not pixel-perfect brand

> **Browse in browser:** Open <a href="index.html" target="_blank">index.html</a> (all mockup links open in a new browser tab).  
> From terminal in this folder: `start index.html`

---

## How to use this pack

1. **Onboarding first:** <a href="mockups/contractor-project-create-desktop.html" target="_blank">contractor-project-create-desktop.html</a> — walk C-1 project setup with Ryan/Macie.
2. **Then MMS setup:** <a href="mockups/contractor-mms-group-setup-desktop.html" target="_blank">contractor-mms-group-setup-desktop.html</a> + <a href="mockups/mms-group-created.png" target="_blank">mms-group-created.png</a> — C-13 group text creation.
3. **Then the slip workflow:** <a href="mockups/workflow-connected-overview.html" target="_blank">workflow-connected-overview.html</a> — UJ-9 connecting MMS + app + portals.
4. **Log decisions** in [discovery-log.md](../../discovery-log.md).

---

## Mockup inventory

### Connected workflow (start here)

| File | Surface | Journey |
|------|---------|---------|
| <a href="mockups/workflow-connected-overview.html" target="_blank">workflow-connected-overview.html</a> | Overview | UJ-9 full walkthrough |

### Contractor — Ryan / Maci

| File | Form factor | Journey |
|------|-------------|---------|
| <a href="mockups/contractor-project-create-desktop.html" target="_blank">contractor-project-create-desktop.html</a> | Desktop | C-1, C-17 |
| <a href="mockups/contractor-project-create-mobile.html" target="_blank">contractor-project-create-mobile.html</a> | Mobile | C-1 |
| <a href="mockups/contractor-mms-group-setup-desktop.html" target="_blank">contractor-mms-group-setup-desktop.html</a> | Desktop modal | C-13 |
| <a href="mockups/contractor-dashboard-desktop.html" target="_blank">contractor-dashboard-desktop.html</a> | Desktop | C-6, C-7, C-19 |
| <a href="mockups/contractor-cascade-preview-desktop.html" target="_blank">contractor-cascade-preview-desktop.html</a> | Desktop | C-12, C-20 |
| <a href="mockups/contractor-mms-mirror-desktop.html" target="_blank">contractor-mms-mirror-desktop.html</a> | Desktop | C-14, C-15 |
| <a href="mockups/contractor-mobile-queue.html" target="_blank">contractor-mobile-queue.html</a> | Mobile | C-19 |

### Sub — Jesse (role-play with Ryan/Macie)

| File | Form factor | Journey |
|------|-------------|---------|
| <a href="mockups/sub-accept-decline-mobile.html" target="_blank">sub-accept-decline-mobile.html</a> | Mobile portal | S-1, S-5 |

### Customer — Lauren (role-play with Ryan/Macie)

| File | Form factor | Journey |
|------|-------------|---------|
| <a href="mockups/customer-connect-mobile.html" target="_blank">customer-connect-mobile.html</a> | Mobile portal | H-1, H-4 |
| <a href="mockups/customer-timeline-mobile.html" target="_blank">customer-timeline-mobile.html</a> | Mobile portal | H-21 |

### MMS — native phone (images)

| File | Shows | Journey |
|------|-------|---------|
| <a href="mockups/mms-group-created.png" target="_blank">mms-group-created.png</a> | New group with Marcus + handle # | C-13 |
| <a href="mockups/mms-sub-delay-thread.png" target="_blank">mms-sub-delay-thread.png</a> | Marcus texts delay in group | UJ-9 step 1, C-14 |
| <a href="mockups/mms-system-confirm.png" target="_blank">mms-system-confirm.png</a> | Jesse gets re-confirm SMS | UJ-9 step 4, S-5 |
| <a href="mockups/mms-customer-milestone.png" target="_blank">mms-customer-milestone.png</a> | Lauren gets milestone MMS | UJ-9 step 4, H-21 |

---

## SME discussion prompts

### Project creation (C-1)
- Is 20–30 min realistic for first project setup? What would you skip on day one?
- Should customer email + MMS go out automatically on save, or do you want a review step first?
- Are tasks + sub assignment on the same form right, or separate screens?
- Is the auto-assigned project text number clear enough?

### MMS group setup (C-13)
- Will you actually create a **new** group per sub, or try to add the handle to an existing thread?
- Is the copy-paste contact card helpful, or would you just do it from memory?
- Is "Maple St - Marcus" the right group naming convention for 5+ active jobs?

### Workflow (UJ-9)
- When Marcus texts a delay, do you see it in the app first or on your phone first?
- Is "talk in MMS, commit in app" how you actually handle slips today?
- Would Maci draft the cascade and Ryan approve on phone — or the reverse?

### Contractor dashboard
- Is the pending queue the right home screen priority?
- Is "Send reminder" / "Snooze 2 days" the right poke controls?
- Do you need to see customer channel status (email ✅ / MMS ⏳) on the dashboard?

### Cascade
- Is +3 days preview with "who gets notified" enough before publish?
- Should customer milestones be called out separately from sub re-confirms?

### MMS groups
- Will you create a **new** group per sub with the project handle #, or try to use existing threads?
- Is the group naming ("Maple St - Marcus") clear enough when you have 5 active jobs?

### Sub portal
- Would Jesse tap this link? What would make him ignore it?
- Is batch accept (3 tasks in one tap) right, or one task per link?

### Customer portal
- Is dual-channel confirm (email + MMS) worth the friction?
- Is the timeline view enough — or do customers need more detail?
- Accept/decline only (no counter-propose) — is that right for homeowners?

---

## What's NOT in this pack (deferred)

- Sub roster management (C-4, C-5)
- Calendar linking flows (S-5a, H-6)
- Photo upload in threads (C-16)
- Full brand / logo lockup
- Interactive prototypes

---

## Design spines

- Visual identity: [DESIGN.md](./DESIGN.md)
- Experience spec: [EXPERIENCE.md](./EXPERIENCE.md)

---

## Next steps after SME review

1. Log decisions in discovery-log.md
2. Update user journeys where SMEs push back
3. Add deferred screens if critical gaps found
4. Run `bmad-create-epics-and-stories` when journeys stabilize
