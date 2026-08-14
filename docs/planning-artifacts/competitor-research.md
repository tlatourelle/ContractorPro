# Competitor Research

Research date: **2026-08-13**  
Platforms: [BuilderTrend](https://buildertrend.com/), [Contractor Foreman](https://get.contractorforeman.com/), [BuildPass](https://www.buildpass.ai/)

## Summary for ContractorPro

| | BuilderTrend | Contractor Foreman | BuildPass |
|--|-------------|-------------------|-----------|
| **Position** | Premium residential / custom homes | Budget all-in-one | AI-native, site + finance expansion |
| **Price signal** | Demo-gated; ~$339–$1,099/mo reported | $49 basics / $221 full platform | Modular; demo-gated; ~$129–$1,099 reported |
| **Users billed** | Unlimited users | Per company | Admin users; subs/workers free |
| **Moat** | Selections + client portal | Price + breadth | Agent + safety/compliance |
| **Calendar** | Built-in | Built-in | Built-in |
| **Our opportunity** | Too expensive, heavy | Still a big suite | Complex, newer finance modules |

**ContractorPro wedge:** **Schedule coordination layer** — Google Calendar as first-class (two-way), magic-link sub + homeowner comms, optional cascade, event-triggered AI drafts. Not a cheaper BT clone; not full ERP.

---

## BuilderTrend

Research updated: **2026-08-14** (review platforms, product docs, third-party analysis)

### Positioning

- **Target:** 5–50 person residential GCs, custom home builders, remodelers running **8–15+ concurrent jobs**
- **Claimed scale:** 20,000+ builders; 1M+ users ([buildertrend.com](https://buildertrend.com/))
- **Moat:** Selections (Good/Better/Best, allowances) + Client Portal + financial depth — **not** scheduling alone
- **Out of scope for BT sweet spot:** Solo operators, 2–15 employee GCs with handful of jobs, service trades

### Pricing (2026)

- **Published tiers removed** — volume-based custom quotes via 5-step form ([pricing page](https://buildertrend.com/pricing/))
- Volume brackets: $0–499K through $31M+ annual construction volume
- **Historical tiers** (annual billing, third-party reports): Essential ~$339/mo → Advanced ~$499 → Complete ~$829; M2M up to ~$1,099/mo
- Onboarding: **$400–$1,500** implementation; 12-week guided onboarding (weeks 1–2 setup, 3–12 live job)
- Renewal hikes of **50–75%** reported at contract renewal
- Unlimited users on all plans; 14-day trial typically available

### User sentiment (aggregated reviews)

Sources: [G2](https://www.g2.com/products/buildertrend/reviews?qs=pros-and-cons), [Capterra](https://www.capterra.com/p/70092/Buildertrend/reviews/), [Software Advice](https://www.softwareadvice.com/construction/buildertrend-profile/reviews/), [Trustpilot](https://www.trustpilot.com/review/buildertrend.com)

**Pros (what users love):**

- All-in-one centralization — scheduling, docs, comms, budgeting in one place
- Responsive, knowledgeable support during onboarding
- Gantt scheduling with automated sub notifications on schedule shifts
- Client portal — daily logs, progress updates, selections with pricing
- Financial features — change orders, POs, billing; time savings vs spreadsheets

**Cons (what users hate):**

- Steep learning curve; overwhelming feature volume; significant setup time
- High, inflexible cost — cost-prohibitive for smaller ops and new businesses
- **Subcontractor resistance** — subs must engage with platform (profile/notifications)
- Clunky UI; too many clicks for simple tasks (desktop + mobile)
- Native estimating rigid vs dedicated tools; QBO/Xero integrations basic for complex tax workflows
- **Data offboarding painful** — bulk export of historical docs/photos tedious
- Mobile app reliability complaints (crashes, partial offline, clunky job-switching)

### Scheduling & cascade (critical — parity with ContractorPro)

Buildertrend **already has dependency cascade**:

- Gantt predecessors/successors — moving a task **auto-adjusts linked successors** ([Schedule Overview](https://helpcenter.buildertrend.net/s/article/Schedule-Overview))
- **Online/Offline mode** — edit privately (mute notifications), then go live
- On save in Online mode: prompt to **notify assigned subs** (text, email, push) + log shift reason/notes
- Change orders can ripple into schedule; trade-conflict warnings; baseline vs actual variance
- Default settings: auto-notify linked subs/vendors, schedule reminders, confirmation requests

**Implication for ContractorPro:** Cascade is **table stakes**, not a unique category. Differentiate on Google Calendar sync, sub friction, price, event-triggered comms, and **automated daily poke until subs confirm** (calendar invites do not do this).

### Google Calendar integration (ContractorPro opportunity)

- **One-way iCal feed only** — Personal Settings → Create Feed → paste URL into Google Calendar ([Project Management Settings](https://helpcenter.buildertrend.net/s/article/Project-Management-Settings))
- Read-only; per-user setup on desktop; window: **30 days past / 60 days forward**
- Also supports Outlook and Apple Calendar via same feed model
- **Not** two-way; subs/homeowners don't get native Google experience

### AI features (2025–2026)

| Feature | Launched | What it does |
|---------|----------|--------------|
| **AI Client Updates** | Jun 2025 | Weekly homeowner summaries from Daily Logs, Schedule, COs, Invoices; GC reviews/edits before send; ~6.5 min vs 30–60 min manual ([press release](https://buildertrend.com/press-releases/buildertrend-launches-ai-tool-for-97-faster-client-updates/)) |
| **AI Bill Pay** | Feb 2026 (IBS) | Invoice capture, digitization, approval routing in BT Payments |
| **Client Portal refresh** | 2025 | Cleaner mobile UX; feeds AI updates |

**What BT does NOT have:** AI scheduling assist, AI copilot, AI estimating, two-way calendar sync, lightweight sub onboarding.

### Integrations

- **Native:** QBO, Xero, Sage, Gusto, HubSpot/Salesforce/Pipedrive, Home Depot/Lowe's/Ferguson, STACK Takeoff
- **Calendar:** One-way iCal feed to Google/Outlook/Apple (see above)
- **Gaps:** No native CompanyCam, EagleView, Xactimate, DocuSign; thin Marketplace; Zapier not officially listed
- Best as self-contained hub with QBO as accounting backbone

### CoConstruct acquisition

- Acquired Jul 2021; CoConstruct ~$399/mo still exists but **active development stopped**
- Existing customers migrating to BT Complete tier (~$829/mo); multi-week migration
- New buyers should evaluate BT directly — CoConstruct is slow sunset

### Adjacent competitor

- **[JobTread](https://buildertrend.com/buildertrend-vs-jobtread/)** — transparent pricing (~$199/mo base + per-user), strong estimating/job costing; BT positions itself above JobTread for "growing businesses" with 10+ concurrent projects
- JobTread = closer on price but still full suite, not calendar-first

### ContractorPro vs BuilderTrend (our segment)

| Dimension | BuilderTrend | ContractorPro (target) |
|-----------|--------------|------------------------|
| Price (small GC) | $400–1,000+ opaque | Free tier + ~$29–79 published |
| Time to value | Weeks (guided onboarding) | One session |
| Google Calendar | One-way iCal feed | First-class, two-way |
| Sub adoption | Profile/app required | Magic link + SMS |
| Cascade | Full Gantt deps | Simpler, optional, preview + notify |
| Homeowner portal | Best-in-class | Lightweight "what changed" feed |
| Selections / estimating | Category leader | Explicitly deferred |
| AI comms | Weekly digest (needs daily log discipline) | Event-triggered on schedule change |

### Who leaves / never buys BT (ContractorPro ICP)

- 2–15 employees, 1–5 active residential jobs
- Already on Google Calendar + group text
- Demo'd BT, killed by price or setup time or sub adoption
- Don't need Selections depth (remodel vs custom spec)
- Want coordination, not ERP

### Features (high level)

PM, Gantt + deps, daily logs, RFIs, change orders, estimating (Advanced+), takeoff, client portal, selections (Complete), warranties, time clock, QBO/Xero, lien waivers, punch lists, bid requests, POs, online payments, financing integrations

---

## Contractor Foreman

- **Target:** Subs, GCs, all trades — 1 to 300+ employees
- **Strengths:** Low published price, 35+ features, unlimited projects at fixed company price, permit manager, service tickets, GPS timecards
- **Pricing:** [get.contractorforeman.com](https://get.contractorforeman.com/) — basics from **$49/mo**, full platform from **$221/mo** per company
- **Weaknesses:** Jack-of-all-trades UX, weak homeowner experience vs BT, limited AI story
- **Integrations:** QuickBooks, Zapier, 50+ systems, WePay for online payments

### Feature groups (from marketing)

**Project:** Projects, opportunities, daily logs, scheduling, work orders, punchlist, permits, service tickets, client portal, to-dos

**Financial:** Estimates, bids, orders, invoices, POs, sub-contracts, expenses, online payments, cost items, job costing dashboard

**People:** Directory, team chat, leads, GPS time cards, calendar, incidents, safety meetings (800 topics)

---

## BuildPass

- **Target:** Head contractors; strong in AU, expanding US
- **Structure:** Win (precon) / Pay (finance) / Build (site) / Connect (AI + integrations)
- **Strengths:** BuildPass Agent on live data, MCP, safety (sign-ons, SWMS, inductions), flat monthly not project-value-based, unlimited projects/storage/subs on plans
- **Pricing:** Demo-gated ([us-pricing.buildpass.ai](https://us-pricing.buildpass.ai/)); self-serve checkout shows Lite/Standard/Pro tiers; third-party cites $219–$1,099/mo
- **Weaknesses:** Win/Pay newer; selections/homeowner depth not BT-class

### Build module features

Scheduling, drawings, site diaries, inductions, sign-ons, SWMS, defect management, checklists, RFIs

### AI

Agent searches records, builds reports, creates/updates with approval; does not train on customer data; MCP for external AI tools

---

## Small-operator anti-Buildertrend segment

Research date: **2026-08-14**  
Sources: [Projul pricing analysis](https://projul.com/blog/buildertrend-pricing-analysis-2026/), [Foreman alternatives](https://foreman.co/blog/buildertrend-alternatives-small-contractors), [FieldFuze](https://toricentlabs.com/blog/buildertrend-alternative.html), [SubcontractorHub](https://www.subcontractorhub.com/buildertrend-alternatives), [MyQuoteIQ](https://myquoteiq.com/best-buildertrend-alternative-for-remodeling-companies/), ENR/small-business trends

### Market signal

- Solo and micro-operations are the **fastest-growing small-business segment** (~35% faster than broader market) — [ENR/Autodesk small-business coverage](https://www.enr.com/articles/63065-autodesk-launches-new-division-for-small-business-users)
- ~**80% of small operators** struggle to balance admin vs field work
- BT's volume-based quoting ($299–$900+/mo typical) + $400–$1,500 onboarding creates acute barrier for 1–5 person crews

### Strategic playbook (category consensus)

Emerging micro-SaaS competitors converge on:

| Dimension | Buildertrend paradigm | Small-operator playbook |
|-----------|----------------------|-------------------------|
| **Audience** | Mid-size builders, 50+ projects/yr | Solo–5 person residential crews, boutique remodelers |
| **Pricing** | Volume quotes, sales call required | Flat transparent ($39–$49/mo), free tier, zero onboarding fee |
| **Onboarding** | Weeks, dedicated implementation | Self-serve, first value in **~10 minutes** |
| **Sub integration** | Profile + portal + app ecosystem | **No-login magic links** via SMS/WhatsApp |
| **Estimating** | Heavy backend cost tracking | AI field quoting, photo estimate, supplier "clipping" |
| **Financials** | Fixed-price custom home builds | T&M logging, receipt capture, **embedded consumer financing** |
| **UX** | Full ERP surface area | Ruthlessly limited daily-use features; glove-friendly mobile |

### Named alternatives in this segment

| Product | Angle | Relevance to ContractorPro |
|---------|-------|---------------------------|
| [Foreman](https://foreman.co/blog/buildertrend-alternatives-small-contractors) | Transparent low pricing, small contractor focus | Pricing/onboarding benchmark |
| [FieldFuze](https://toricentlabs.com/blog/buildertrend-alternative.html) | Micro-SaaS anti-bloat | UX simplicity benchmark |
| [Jobtable](https://jobtable.com/quotations/) | Fast quotations | Estimating competitor (not our lane) |
| [KonstructIQ](https://konstructiq.com/ai) | AI estimating | Estimating competitor (not our lane) |
| [SubcontractorHub](https://www.subcontractorhub.com/buildertrend-alternatives) | Embedded financing in proposals | Post-MVP financing reference |
| Contractor Foreman | $49 basics tier | Full-suite price floor |

### ContractorPro alignment matrix

| Blueprint pillar | Adopt for ContractorPro? | Notes |
|------------------|-------------------------|-------|
| Transparent pricing + free tier | ✅ **Yes** | Already in vision; validate $29–79 range in discovery |
| 10-minute self-serve onboarding | ✅ **Yes** | Core differentiator vs BT's 12-week onboarding |
| Magic-link sub coordination (no app) | ✅ **Yes** | **Core MVP** — strongest overlap with blueprint |
| Anti-bloat UX | ✅ **Yes** | Schedule + comms only in v0.1 |
| AI field estimating / photo quotes | ❌ **Defer** | Different product; KonstructIQ/Jobtable territory; high build cost |
| Supplier "clipping" (Materio-style) | ❌ **Defer** | Materials procurement = new category |
| T&M logging + receipt invoicing | ❌ **Defer** | Explicitly out of MVP; explore post-MVP if discovery demands |
| Embedded consumer financing | ❌ **Defer** | Partnership/regulatory complexity; BT already has GreenSky/Nelnet |
| WhatsApp integration | ⚠️ **Later** | SMS first (US); WhatsApp if international expansion |

### What the blueprint misses (ContractorPro unique wedge)

- **Google Calendar two-way sync** — not mentioned; BT only has one-way iCal feed
- **GC-as-hub messaging model** — separate GC↔sub and GC↔homeowner threads
- **Event-triggered AI** on schedule change vs weekly digest requiring daily log discipline
- **Coordination layer positioning** — not a "direct BT competitor" but a wedge that sits *beside* calendar + text

### Strategic warning

Following the full blueprint turns ContractorPro into **Contractor Foreman + KonstructIQ** — a broad all-in-one that competes on feature breadth. Our validated wedge is narrower: **win schedule coordination first**, expand only when users pull us.

---

## Category table stakes (what “construction software” implies)

- Estimating, scheduling, daily logs, change orders, job costing, invoicing, client portal, document storage, time tracking, sub coordination

**ContractorPro explicitly defers** most of these in v0.1 — only schedule coordination, messaging, and calendar sync.

---

## Further competitor research (suggested)

- [ ] Jobber — service + small contractor
- [ ] Houzz Pro — residential remodel
- [ ] CoConstruct — residential, selections (BT acquired)
- [ ] Procore — enterprise ceiling reference only

Log findings in [discovery-log.md](./discovery-log.md).
