# Sprint Status YAML — Aspire Infrastructure Stories

**Copy-and-paste these entries into `docs/implementation-artifacts/sprint-status.yaml`**

---

## Option 1: Add Aspire Epic as Separate Track (Recommended)

Insert after the Epic 1 entries, before Epic 2:

```yaml
  # ← END of Epic 1 section

  # ASPIRE INFRASTRUCTURE EPIC (parallel track)
  epic-aspire-infra: backlog
  aspire-infra.0-create-apphost-project: ready-for-dev
  aspire-infra.1-wire-services-with-service-discovery: backlog
  aspire-infra.2-dashboard-health-checks-and-launch: backlog
  aspire-infra.3-integration-test-harness: backlog
  aspire-infra.4-deployment-manifest-and-documentation: backlog
  aspire-infra-retrospective: optional

  # ← START of Epic 2 section
  epic-2: backlog
```

---

## Option 2: Rename Epic 2 to "Aspire Infrastructure", defer "Projects & Tasks" to Epic 3

If you want numeric continuity (1, 2, 3 instead of 1, aspire, 2):

```yaml
  # ← END of Epic 1 section

  epic-2: backlog
  2-0-create-apphost-project: ready-for-dev
  2-1-wire-services-with-service-discovery: backlog
  2-2-dashboard-health-checks-and-launch: backlog
  2-3-integration-test-harness: backlog
  2-4-deployment-manifest-and-documentation: backlog
  epic-2-retrospective: optional

  epic-3: backlog
  3-1-create-project-placeholder: backlog
  epic-3-retrospective: optional
```

**Note:** If using Option 2, update all story file references:
- `aspire-infra.0-*.md` → `2-0-*.md`
- `aspire-infra.1-*.md` → `2-1-*.md`
- (etc.)

---

## Example: Full Development Status Section (using Option 1)

```yaml
development_status:
  epic-1: in-progress
  1-0-solution-scaffold: done
  1-1-google-oauth-bff-session-and-contractor-auto-provision: in-review
  1-2-company-profile-and-session-context: backlog
  1-3-guided-onboarding-checklist-shell: backlog
  epic-1-retrospective: optional

  epic-aspire-infra: backlog
  aspire-infra.0-create-apphost-project: ready-for-dev
  aspire-infra.1-wire-services-with-service-discovery: backlog
  aspire-infra.2-dashboard-health-checks-and-launch: backlog
  aspire-infra.3-integration-test-harness: backlog
  aspire-infra.4-deployment-manifest-and-documentation: backlog
  aspire-infra-retrospective: optional

  epic-2: backlog
  2-1-create-project-placeholder: backlog
  epic-2-retrospective: optional
```

---

## Status Transitions (Amelia's workflow)

As Amelia works through stories, update sprint-status.yaml:

### **Day 1: Start Aspire-Infra.0**
```yaml
  epic-aspire-infra: in-progress  # ← Changed from backlog
  aspire-infra.0-create-apphost-project: in-progress  # ← Changed from ready-for-dev
```

### **Day 2–3: Aspire-Infra.0 done, start Aspire-Infra.1**
```yaml
  epic-aspire-infra: in-progress
  aspire-infra.0-create-apphost-project: done  # ← Changed from in-progress
  aspire-infra.1-wire-services-with-service-discovery: ready-for-dev  # ← Changed from backlog
  # If Aspire-Infra.0 needs code review:
  aspire-infra.0-create-apphost-project: review  # ← Instead of done
```

### **Day 5: Aspire-Infra.1 done, Aspire-Infra.2 & 3 start in parallel**
```yaml
  aspire-infra.1-wire-services-with-service-discovery: done
  aspire-infra.2-dashboard-health-checks-and-launch: ready-for-dev
  aspire-infra.3-integration-test-harness: ready-for-dev
```

### **Final state (all done)**
```yaml
  epic-aspire-infra: done  # ← All stories are done
  aspire-infra.0-create-apphost-project: done
  aspire-infra.1-wire-services-with-service-discovery: done
  aspire-infra.2-dashboard-health-checks-and-launch: done
  aspire-infra.3-integration-test-harness: done
  aspire-infra.4-deployment-manifest-and-documentation: done
  aspire-infra-retrospective: optional  # ← Decide if retro is needed
```

---

## Valid Status Values

Per [sprint-status.yaml](./sprint-status.yaml#L1-L20):

**Story status options:**
- `backlog` — Story only exists in epic file
- `ready-for-dev` — Story file created, Amelia can start
- `in-progress` — Developer actively working on implementation
- `review` — Implementation complete, ready for code review
- `done` — Story completed

**Epic status options:**
- `backlog` — Epic not yet started
- `in-progress` — Epic actively being worked on
- `done` — All stories in epic completed

---

## Update Metadata (top of sprint-status.yaml)

After adding Aspire stories, update the metadata section:

```yaml
generated: 08-20-2026 11:20
last_updated: 2026-08-21 16:30  # ← Update timestamp
project: ContractorPro
project_key: NOKEY
tracking_system: file-system
story_location: 
  c:\Users\Thomas.LaTourelle\ContractorPro\docs\implementation-artifacts
# No new correct_course needed (architecture locked 2026-08-20)
```

---

## Commit Message Suggestion

After updating sprint-status.yaml:

```
docs: Add Aspire Infrastructure epic (stories aspire-infra.0-4)

Five implementation stories for .NET Aspire AppHost orchestration:
- aspire-infra.0: Create AppHost project
- aspire-infra.1: Wire services with service discovery
- aspire-infra.2: Dashboard, health checks, launch experience
- aspire-infra.3: Integration test harness
- aspire-infra.4: Deployment manifest & documentation

Enables:
- F5 single-click local dev launch
- Real integration tests (Aspire test fixture)
- Production deployment path (manifest to ACA)

Parallelizable with E1 stories (no blocking dependencies).
Ready for sprint planning assignment to Amelia.

Refs: aspire-infrastructure-epic-summary.md
```

---
