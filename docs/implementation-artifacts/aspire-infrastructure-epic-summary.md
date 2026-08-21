# ContractorPro Aspire Infrastructure Stories — Integration Guide

**Created:** 2026-08-21  
**For:** Amelia (Developer Agent) · ContractorPro Sprint Planning  
**Status:** Ready for sprint-status.yaml integration and implementation

---

## Executive Summary

Five actionable stories implement .NET Aspire AppHost orchestration to replace docker-compose local dev workflow. Enables:
- **F5 single-click local launch** (AppHost + Aspire Dashboard)
- **Real integration tests** (Aspire test harness with database isolation)
- **Production deployment path** (Aspire → Azure Container Apps manifest generation)

**Total effort estimate:** 40–60 story points (T-shirt: XL)  
**Parallelization:** Stories 0 & 1 sequential; 2 & 3 can run in parallel after 1 complete  
**Duration:** 2–3 weeks for one developer  
**Blocker for E1 stories?** No — can be run in parallel track; integration tests available for E1.1+ by story 3  
**Windows-only:** ✓ (dev machines constraint met; CI/CD keeps docker-compose fallback)

---

## Story Dependency Graph

```
Aspire-Infra.0: Create AppHost project
    ↓
Aspire-Infra.1: Wire services with service discovery
    ├─→ Aspire-Infra.2: Dashboard & health checks (can start after 1 done)
    │       ↓
    │   Aspire-Infra.3: Integration test harness (can start after 1 OR 2 done)
    │       ↓
    └─→ Aspire-Infra.4: Deployment manifest & docs (can start after 2 done)

Legend:
  →   : Hard dependency (blocker)
  ┌─→ : Can start in parallel after this story completes
```

---

## Sprint Status YAML Addition

Add this epic and stories to `docs/implementation-artifacts/sprint-status.yaml` under the existing `development_status` section:

```yaml
development_status:
  epic-1: in-progress
  1-0-solution-scaffold: done
  1-1-google-oauth-bff-session-and-contractor-auto-provision: in-review
  1-2-company-profile-and-session-context: backlog
  1-3-guided-onboarding-checklist-shell: backlog
  epic-1-retrospective: optional

  # ← NEW: Aspire Infrastructure Epic
  epic-aspire-infra: backlog
  aspire-infra.0-create-apphost-project: ready-for-dev
  aspire-infra.1-wire-services-with-service-discovery: backlog
  aspire-infra.2-dashboard-health-checks-and-launch: backlog
  aspire-infra.3-integration-test-harness: backlog
  aspire-infra.4-deployment-manifest-and-documentation: backlog
  epic-aspire-infra-retrospective: optional

  epic-2: backlog
  2-1-create-project-placeholder: backlog
  epic-2-retrospective: optional
```

**Note:** Epic numbering (`epic-aspire-infra` vs `epic-2` vs `epic-infrastructure`) is flexible. Adjust based on your naming convention.

---

## Story Sequencing Options

### **Option A: Sequential (Conservative, 2–3 weeks)**

Recommended if Amelia is solo or limited context-switching capacity.

```
Week 1:
  Mon–Tue: Aspire-Infra.0 (2 days)
  Wed–Thu: Aspire-Infra.1 (2 days)
  Fri: Buffer / code review

Week 2:
  Mon–Tue: Aspire-Infra.2 (2 days) + Aspire-Infra.3 starts (parallel)
  Wed–Fri: Aspire-Infra.3 (3 days)

Week 3:
  Mon–Tue: Aspire-Infra.4 (2 days, mostly documentation)
  Wed–Fri: Code review + dry-run validation
```

**Advantage:** Linear knowledge buildup; each story reinforces the previous.  
**Disadvantage:** Longer total calendar time; Aspire-Infra.3 (tests) available late.

### **Option B: Parallel (Aggressive, 10–14 days)**

If you have two developers or Amelia is high-context.

```
Sprint window 1 (5–7 days):
  Developer 1: Aspire-Infra.0, 1, 2 (sequential)
  Developer 2: Start E1.2 or other feature work (Aspire not yet needed)

Sprint window 2 (5–7 days):
  Developer 1: Aspire-Infra.3, 4
  Developer 2: Can now write integration tests using Aspire fixture
```

**Advantage:** Shorter calendar time; integration tests available earlier for E1.1+ testing.  
**Disadvantage:** Requires clear handoff; Aspire-Infra.1 critical for Aspire-Infra.2 & 3 success.

---

## Story Effort Estimates (T-shirt)

| Story | Effort | Notes |
|-------|--------|-------|
| Aspire-Infra.0 | **M** (1–2 days) | Project scaffolding, straightforward |
| Aspire-Infra.1 | **M-L** (2–3 days) | Requires adjusting API/Web configs for service discovery |
| Aspire-Infra.2 | **M** (1.5–2 days) | Health check wiring, straightforward |
| Aspire-Infra.3 | **L** (3–4 days) | Test harness has dependencies; xUnit fixture learning curve if new to Aspire |
| Aspire-Infra.4 | **S** (1–2 days) | Documentation + dry-run; mostly manual steps |
| **Total** | **XL** (40–60 pts) | Full epic ~2–3 weeks for one developer |

---

## Ready-to-Build Checklist

Before Amelia starts:

- [ ] **Aspire-Infra.0 story file** reviewed and approved (format matches project standard)
- [ ] **Local environment ready:**
  - [ ] .NET 10 SDK installed
  - [ ] Docker Desktop running (for PostgreSQL during dev)
  - [ ] Visual Studio 2025 or VS Code with C# extension
  - [ ] Aspire tooling installed (`dotnet add package Aspire.Hosting --prerelease` or similar)
- [ ] **Architecture locked** — Decisions in [architecture-v0.1.md](../planning-artifacts/architecture-v0.1.md) respected (no changes mid-sprint)
- [ ] **Story files in place:**
  - [ ] All 5 stories created in `docs/implementation-artifacts/` ✓
  - [ ] Linked in sprint-status.yaml ✓
- [ ] **Parallel work alignment:**
  - [ ] E1.1 code review not blocking Aspire-Infra.0–1 start ✓
  - [ ] E1.2 can proceed independently (no Aspire dependency until Aspire-Infra.3)
- [ ] **Communication cadence:**
  - [ ] Daily standup covers Aspire-specific blockers (Docker issues, NuGet outdated versions, etc.)
  - [ ] Code review scheduled after Aspire-Infra.2 (before Aspire-Infra.3 for integration test feedback)

---

## Success Criteria for Full Epic

When Aspire-Infra.0–4 are **done**:

1. ✓ Developer can F5 AppHost → full stack (API + Web + Database) launches in <30s
2. ✓ Aspire Dashboard shows health status of all services
3. ✓ Integration tests run via `dotnet test` and spin up isolated Postgres + API per test class
4. ✓ Manifest generation (`dotnet aspire build`) produces valid ACA-deployable YAML
5. ✓ E1.1 (OAuth story) and beyond can use Aspire integration tests (not mocked HTTP)
6. ✓ Deployment flow documented and dry-run validated (optional in v0.1, but recommended)
7. ✓ docker-compose.yml remains as CI/CD fallback (no removal)

---

## Known Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|-----------|--------|-----------|
| **Aspire package version conflicts** | Medium | 1–2 days blocked | Keep packages in sync (9.0.0 for base packages); pin versions in .csproj |
| **Docker Desktop not running** | Low | 30 min delay | Document prerequisite; add startup check to AppHost Program.cs (fail fast) |
| **PostgreSQL migrations not auto-run** | High | 1 day learning | Aspire-Infra.2 documents manual migration workaround; Aspire-Infra.3 test fixture handles it |
| **React + Vite service discovery wiring** | Medium | 1 day troubleshooting | Story 1 has sample `.env.local` + client.ts; test before moving forward |
| **Port conflicts (5000, 5173, 15000 in use)** | Low | 30 min | Aspire assigns dynamically; check Dashboard if manual port change needed |
| **Azure CLI / ACR setup for dry-run (Aspire-Infra.4)** | Medium | 2–3 days if new to Azure | Optional in v0.1; document prerequisites; test on laptop before sprint |

**Mitigation strategy:** Start with Aspire-Infra.0 on a feature branch; if version conflicts arise, fix + document in a follow-up patch story.

---

## Integration Points with E1 Stories

### **Aspire-Infra.1–3 available** → E1.1 can use integration tests

**E1.1 (Google OAuth) benefit:**
- Currently mocks HTTP client and database
- After Aspire-Infra.3, can write real integration tests:
  ```csharp
  [Fact]
  public async Task GoogleOAuthCallback_CreatesContractorOnFirstLogin()
  {
      // Use AppHostFixture to call real API + check real database
      // No mocks; verifies session cookie, redirects, DB state
  }
  ```

**Recommendation:** Have E1.1 code review wait for Aspire-Infra.2 completion, so integration test patterns can be aligned.

---

## Deployment Gate (Aspire-Infra.4)

**Aspire-Infra.4 is a pre-MVP gate:** Before releasing v0.1 to Azure, dry-run the manifest-to-ACA flow.

- **If dry-run succeeds:** Release confidence ↑; proceed to E2 with deployment automation in CI/CD pipeline
- **If dry-run fails:** Debug during Aspire-Infra.4; document workaround or file Azure SDK issue

Aspire-Infra.4 documentation will be the SOC (Setup, Operate, Confirm) guide for all future releases.

---

## Files Delivered

### Story markdown files (ready to implement)

```
docs/implementation-artifacts/
  aspire-infra.0-create-apphost-project.md
  aspire-infra.1-wire-services-with-service-discovery.md
  aspire-infra.2-dashboard-health-checks-and-launch.md
  aspire-infra.3-integration-test-harness.md
  aspire-infra.4-deployment-manifest-and-documentation.md
```

### Supporting docs (to be created during stories)

```
docs/deployment/
  aspire-to-aca-deployment-guide.md                  [Aspire-Infra.4]
  aspire-manifest-checklist.md                       [Aspire-Infra.4]
  
tests/
  ContractorPro.Api.Integration.Tests/               [Aspire-Infra.3]
    Fixtures/AppHostFixture.cs
    ApiIntegrationTestCollection.cs
    HealthEndpointTests.cs
```

---

## Next Steps for Sprint

### **Immediate (next standup):**
1. Add Aspire-Infra epic to sprint-status.yaml (decide epic numbering)
2. Review Aspire-Infra.0 story with Amelia; address questions
3. Confirm startup environment ready (Docker, .NET 10, VS 2025)
4. **Move Aspire-Infra.0 to `ready-for-dev`** in sprint-status.yaml

### **Day 1 (Amelia starts):**
1. Clone branch, create feature branch: `feature/aspire-infra-0`
2. Follow Aspire-Infra.0 tasks
3. Verify AC-1–4 (project compiles, loads in VS, builds cleanly)
4. Commit, push, PR (no code review needed until Aspire-Infra.1 integration tests)

### **End of Aspire-Infra.0:**
1. Move status to `in-progress` in sprint-status.yaml
2. Start Aspire-Infra.1; can occur same day if on track

### **End of Aspire-Infra.1:**
1. Launch `dotnet run --project src/ContractorPro.AppHost` → verify full stack works
2. If services connected cleanly, proceed to Aspire-Infra.2 & 3 in parallel
3. If issues, debug + document in Aspire-Infra.1 retrospective

---

## Questions for Sprint Planning

Before committing to this epic, clarify with Thomas/team:

1. **Epic numbering:** Should this be `epic-aspire-infra`, `epic-infrastructure`, `epic-devops`, or renumber existing epics?
2. **Parallel track:** Can Aspire work run in parallel with E1.1 code review / E1.2 development, or is Amelia 100% allocated here?
3. **Azure dry-run (Aspire-Infra.4):** Should this be in-sprint or deferred to release phase? (Optional in v0.1; recommended in v0.1 for confidence.)
4. **CI/CD fallback:** Confirm docker-compose.yml remains untouched; Aspire is dev + deployment only?
5. **Windows-only acceptance:** Is "Windows dev machines only" acceptable, or should we prepare for future Mac/Linux support?

---

## References

- **Architecture decisions:** [architecture-v0.1.md](../planning-artifacts/architecture-v0.1.md) § 9
- **Story standard:** [story-standard.md](./story-standard.md) (all 5 stories follow this)
- **.NET Aspire docs:** [Aspire overview](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/aspire-overview)
- **Project dev workflow:** [dev-workflow.md](./dev-workflow.md)

---

**Status:** Ready for sprint-status.yaml integration and handoff to Amelia.

---
