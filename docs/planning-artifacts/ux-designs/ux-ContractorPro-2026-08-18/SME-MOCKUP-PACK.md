# SME Mockup Pack — Ryan & Maci Review

**Status:** Draft for planning validation  
**Created:** 2026-08-18  
**Audience:** Ryan (contractor/owner) + Maci (office manager) — same v0.1 access  
**Goal:** Validate *"is this how you work?"* — not pixel-perfect brand

---

## How to use this pack

1. **Onboarding first:** Open [contractor-project-create-desktop.html](./mockups/contractor-project-create-desktop.html) — walk C-1 project setup with Ryan/Macie.
2. **Then MMS setup:** [contractor-mms-group-setup-desktop.html](./mockups/contractor-mms-group-setup-desktop.html) + [mms-group-created.png](./mockups/mms-group-created.png) — C-13 group text creation.
3. **Then the slip workflow:** [workflow-connected-overview.html](./mockups/workflow-connected-overview.html) — UJ-9 connecting MMS + app + portals.
4. **Log decisions** in [discovery-log.md](../../discovery-log.md).

---

## Mockup inventory

### Connected workflow (start here)

| File | Surface | Journey |
|------|---------|---------|
| [workflow-connected-overview.html](./mockups/workflow-connected-overview.html) | Overview | UJ-9 full walkthrough |

### Contractor — Ryan / Maci

| File | Form factor | Journey |
|------|-------------|---------|
| [contractor-project-create-desktop.html](./mockups/contractor-project-create-desktop.html) | Desktop | C-1, C-17 |
| [contractor-project-create-mobile.html](./mockups/contractor-project-create-mobile.html) | Mobile | C-1 |
| [contractor-mms-group-setup-desktop.html](./mockups/contractor-mms-group-setup-desktop.html) | Desktop modal | C-13 |
| [contractor-dashboard-desktop.html](./mockups/contractor-dashboard-desktop.html) | Desktop | C-6, C-7, C-19 |
| [contractor-cascade-preview-desktop.html](./mockups/contractor-cascade-preview-desktop.html) | Desktop | C-12, C-20 |
| [contractor-mms-mirror-desktop.html](./mockups/contractor-mms-mirror-desktop.html) | Desktop | C-14, C-15 |
| [contractor-mobile-queue.html](./mockups/contractor-mobile-queue.html) | Mobile | C-19 |

### Sub — Jesse (role-play with Ryan/Macie)

| File | Form factor | Journey |
|------|-------------|---------|
| [sub-accept-decline-mobile.html](./mockups/sub-accept-decline-mobile.html) | Mobile portal | S-1, S-5 |

### Customer — Lauren (role-play with Ryan/Macie)

| File | Form factor | Journey |
|------|-------------|---------|
| [customer-connect-mobile.html](./mockups/customer-connect-mobile.html) | Mobile portal | H-1, H-4 |
| [customer-timeline-mobile.html](./mockups/customer-timeline-mobile.html) | Mobile portal | H-21 |

### MMS — native phone (images)

| File | Shows | Journey |
|------|-------|---------|
| [mms-group-created.png](./mockups/mms-group-created.png) | New group with Marcus + handle # | C-13 |
| [mms-sub-delay-thread.png](./mockups/mms-sub-delay-thread.png) | Marcus texts delay in group | UJ-9 step 1, C-14 |
| [mms-system-confirm.png](./mockups/mms-system-confirm.png) | Jesse gets re-confirm SMS | UJ-9 step 4, S-5 |
| [mms-customer-milestone.png](./mockups/mms-customer-milestone.png) | Lauren gets milestone MMS | UJ-9 step 4, H-21 |

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
