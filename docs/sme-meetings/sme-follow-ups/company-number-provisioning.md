# SME Follow-Up: When to Provision Company Phone Number

| Field | Value |
|-------|-------|
| **Status** | Open — needs Ryan + Maci (+ Thomas/Winston cost input) |
| **Blocks** | #1 architecture finalization; sandbox vs paid telco gates |
| **Workbook** | [decision-workbook.md](../decision-workbook.md) §1 |
| **Winston handoff** | [winston-company-phone-number.md](../handoffs/winston-company-phone-number.md) |

---

## The question

**When does a Contractor get their company Twilio number?**

| Option | Behavior | Cost impact |
|--------|----------|-------------|
| **A. At signup** | Number provisioned on account create | Telco cost on every signup, including sandbox/trial |
| **B. First project create** | Number when GC creates first job | Cost tied to intent; sandbox may create projects without comms |
| **C. First paid tier / comms enabled** | Number when subscription activates outbound | Aligns with current sandbox blocks on SMS send |
| **D. First outbound send** | JIT on first sub invite / approval / milestone | Latest possible; may delay first-value moment |

**Thomas:** Undecided — needs SME + unit economics discussion.

---

## What we already know

| Decision | Detail |
|----------|--------|
| **One # per company** | Not per project |
| **All system outbound** | From company # (approvals, pokes, milestones) |
| **GC inbound alert** | SMS to personal phone when someone texts company # |
| **Accept/decline** | Magic links only — not SMS YES/NO |
| **Personal cell** | Out of scope for logging |

---

## Questions for Ryan + Maci

1. When you **first try the app**, do you expect a **working company text number** immediately, or only after you pay / go live on a real job?
2. Would you **create test projects** before sending any sub texts? (If yes, option C or D avoids number cost on experiments.)
3. Is the company # something you'd **give to customers day one** on a job, or only after schedule is finalized?
4. ReviewWave-style: did you have a number **before** your first real customer message?

---

## Questions for Winston / Thomas (cost)

1. Monthly **fixed cost** of one idle Twilio number (~$1–2?) vs **10DLC** registration timing
2. Can **sandbox** tenants share a platform demo number, or must every company get a unique E.164?
3. Align with **Phase 2 billing** — number provisioned at Stripe subscription `active`?

---

## Options to mark in room

```text
Provision company # at:
  ☐ Signup
  ☐ First project create
  ☐ First paid / comms enabled
  ☐ First outbound send
```

---

## Interim rule (until decided)

Winston architecture spike may **assume C (first paid / comms enabled)** for build planning — **not a product lock**. Thomas marked **undecided** pending SME + cost discussion.

---

## Decision (fill after session)

| Field | Value |
|-------|-------|
| **Choice** | _TBD_ |
| **Decided by** | |
| **Date** | |
| **Rationale** | |

---

## Todo

- [ ] Discuss with Ryan/Macie — expectations for "when do we get our number?"
- [ ] Winston: cost model per option
- [ ] Lock decision → update handoff + workbook §1
- [ ] Propagate to epics (E1 sandbox gates, telephony provision trigger)
