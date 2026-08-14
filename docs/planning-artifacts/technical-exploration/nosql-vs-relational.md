# NoSQL vs Relational — Does It Make Sense?

Status: **Exploratory** (2026-08-13)  
Related: [database-options.md](./database-options.md), [sql-server-vs-postgres.md](./sql-server-vs-postgres.md)

## Question

> We haven’t talked about NoSQL. Does it make sense for ContractorPro?

**Short answer:** **Not as the primary database.** Your core domain — companies, projects, task dependencies, cascade rules, role-based messaging, and audit history — is **relational**. A SQL database (PostgreSQL or SQL Server) is the natural fit. NoSQL can appear **later** as a **supplement** (cache, search, blobs) if specific needs emerge.

**No decision to adopt NoSQL** — this doc explains why SQL stayed the default.

---

## What ContractorPro stores (from planning)

| Data | Shape | Relationships |
|------|-------|---------------|
| Companies (GC tenants) | Structured | → users, projects, billing |
| Users, auth identities | Structured | → company, role |
| Projects | Structured | → tasks, messages, calendars |
| **Tasks + dependencies** | Graph-like | Parent/child, cascade chain |
| **Messages** (GC↔sub, GC↔homeowner) | Threaded, ACL’d | → project, participants |
| Schedule cascade audit | Append-only log | → task, user, timestamp |
| Google Calendar refs | IDs + sync state | → project, external API |
| (Later) files/photos | Binary | → project, message |

Most of this is **many-to-many, foreign keys, transactions, and permissions** — classic relational problems.

---

## NoSQL families — fit check

| Type | Examples | Fit for ContractorPro primary DB? |
|------|----------|-----------------------------------|
| **Document** | MongoDB, Cosmos DB (NoSQL API) | ⭐ Weak — joins and tenant isolation across projects/tasks/messages get awkward |
| **Key-value** | Redis, DynamoDB | ⭐ Not primary — great as **cache** or session store later |
| **Wide-column** | Cassandra | ⭐ Overkill — built for massive write scale, not small GC SaaS |
| **Graph** | Neo4j, Cosmos Gremlin | ⭐⭐ Task dependencies are graph-shaped, but **volume is tiny**; SQL handles trees fine |
| **Search** | Elasticsearch, Azure AI Search | ⭐⭐ Later — “search all messages/projects,” not core store |
| **Blob** | Azure Blob, S3 | ⭐⭐ Later — daily log photos, attachments |

---

## Why SQL fits better (primary database)

### 1. Cascade scheduling is relational + transactional

Cascade = update task A → shift dependents B, C, D → write audit → queue notifications → update Google events.

You want **one transaction** (or tight saga) so the schedule never half-updates.

- **SQL:** `BEGIN` … update tasks … insert audit … `COMMIT` — natural in Postgres/SQL Server.
- **Document DB:** Embedding whole project schedule in one doc causes **large-document rewrites** and concurrency pain; splitting across docs needs **application-level transactions** or eventual consistency (bad for “what’s the date?”).

### 2. Messaging with strict visibility

- GC↔homeowner thread **must not** leak to subs.
- Sub A **must not** see Sub B’s private GC thread.

That’s **row-level security by project + thread + role** — SQL models this cleanly (`messages`, `thread_participants`, `company_id`).

Document stores can do it with careful schema design, but you reinvent relational patterns with weaker ad-hoc query tools.

### 3. Multi-tenant SaaS

Pattern: every row has `company_id`, queries always scoped.

- SQL: indexes on `(company_id, project_id)`, EF Core global query filters.
- NoSQL: same discipline required, but fewer guardrails; cross-tenant bugs are costly.

### 4. Scale expectations

Target: small residential GCs — **hundreds to low thousands** of companies, not billions of events/day.

PostgreSQL/SQL Server handles **millions of rows** easily. NoSQL’s horizontal scale is **unused complexity** at this size.

### 5. Team and tooling

- .NET + **EF Core** → relational is the path of least resistance.
- Vibe coding + AI assistants → **far more** examples for SQL + EF than Cosmos custom patterns.
- Reporting, QBO integration later → SQL exports and joins win.

---

## Where NoSQL *could* appear (hybrid, later)

Not now — but legitimate **add-ons**:

| Need | NoSQL-ish tool | When |
|------|----------------|------|
| **Session / rate limit cache** | Redis | Traffic grows |
| **Real-time presence** (“GC is typing”) | Redis pub/sub or SignalR backplane | Nice-to-have |
| **Full-text search** across messages/projects | Azure AI Search, Elasticsearch | Many projects, search UX |
| **Photo / file attachments** | Azure Blob Storage | Daily logs with images |
| **Analytics / event stream** | Event hub + warehouse | Business metrics at scale |
| **Graph DB for dependencies** | Neo4j | Only if dependency logic becomes extremely complex **and** SQL proves slow — unlikely |

**Architecture pattern:**

```
Primary: PostgreSQL or SQL Server  (source of truth)
Auxiliary: Redis, Blob, Search     (optional, problem-specific)
```

---

## “But tasks are a graph — shouldn’t we use a graph DB?”

Task dependencies **are** a directed graph. Options:

| Approach | Pros | Cons |
|----------|------|------|
| **Adjacency list** (`task.depends_on_task_id`) in SQL | Simple, EF-friendly, fine for &lt;10k tasks/company | Deep chain queries need recursion (CTE) |
| **Closure table** in SQL | Fast “all descendants” for cascade preview | More tables |
| **Graph DB** | Native traversals | Second database, sync, ops, overkill for remodel schedules |

For residential projects (dozens of tasks, not thousands), **SQL + adjacency list or closure table** is standard and sufficient.

---

## Cosmos DB specifically (Azure)

Azure Cosmos DB is sometimes pitched for .NET/Azure shops.

| Pros | Cons for ContractorPro |
|------|------------------------|
| Global distribution, SLA | **Expensive** at small scale |
| Multiple APIs (SQL, Mongo, Gremlin) | Wrong default for greenfield relational SaaS |
| Serverless option | RU pricing unpredictable for chatty OLTP |

**Verdict:** Cosmos is for **massive scale or global low-latency** — not a small GC scheduling app unless you have specific Cosmos expertise and requirements.

---

## Comparison summary

| Criterion | SQL (Postgres / SQL Server) | NoSQL primary |
|-----------|----------------------------|---------------|
| Tasks + dependencies | ⭐⭐⭐ | ⭐⭐ |
| Messaging + ACLs | ⭐⭐⭐ | ⭐⭐ |
| Multi-tenant | ⭐⭐⭐ | ⭐⭐ |
| Transactions (cascade) | ⭐⭐⭐ | ⭐ |
| .NET + EF Core | ⭐⭐⭐ | ⭐⭐ (Cosmos/Mongo providers exist) |
| Small team / vibe code | ⭐⭐⭐ | ⭐⭐ |
| Horizontal mega-scale | ⭐⭐ | ⭐⭐⭐ |
| Schema flexibility | ⭐⭐ | ⭐⭐⭐ |

ContractorPro is **correctness and relationships at modest scale**, not **unstructured data at planetary scale**.

---

## Draft lean (not final)

| Layer | Choice |
|-------|--------|
| **Primary database** | **Relational** — PostgreSQL or SQL Server (see sql-server-vs-postgres.md) |
| **NoSQL primary** | **No** — not justified for current requirements |
| **NoSQL auxiliary** | Revisit when you need cache, blobs, or search — not MVP |

---

## Open questions

- [ ] Will daily logs with **heavy photo volume** push blob storage early? (Blob yes; NoSQL DB no)
- [ ] Real-time messaging — **SignalR + SQL** enough, or need Redis?
- [ ] Any requirement for **offline-first mobile** with sync? (Could change storage story — not in v0.1)

Log decisions in [discovery-log.md](../discovery-log.md).
