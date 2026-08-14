# Database Options — Exploration

Status: **Exploratory** — no vendor or host decision  
Related: [auth-and-data.md](./auth-and-data.md), [auth-comparison-managed-vs-authjs.md](./auth-comparison-managed-vs-authjs.md)

## What ContractorPro needs from a database

| Need | Why |
|------|-----|
| **Relational model** | Projects → tasks → dependencies (cascade); messages; company tenancy |
| **Low cost now** | Solo/small team, long planning phase, few users initially |
| **Scale path** | Backups, HA, pooling, more storage without rewriting the app |
| **US region** | US-only product for now |
| **Portable** | Same Postgres dialect whether host is Neon, Supabase, Azure, or RDS |
| **Works with any auth choice** | Clerk, Supabase Auth, or Auth.js all pair with Postgres |

**Working assumption (not final):** Relational DB required. **PostgreSQL** and **SQL Server** both fit; see [sql-server-vs-postgres.md](./sql-server-vs-postgres.md). Prior docs leaned Postgres for cost/portability; SQL Server valid for .NET+Azure if you prefer T-SQL or have licenses.

---

## Engine: why Postgres is the default candidate

| Engine | Fit | Notes |
|--------|-----|-------|
| **PostgreSQL** | ⭐⭐⭐ | JSON columns, mature tooling, every host below supports it |
| **MySQL** | ⭐⭐ | Possible; less common in greenfield SaaS |
| **SQLite** | ⭐ dev only | Fine locally; not for multi-tenant prod |
| **NoSQL (Mongo, etc.)** | ⭐ | Awkward for task dependencies and message threading |

---

## Host options — Tier 1: Free / cheap (MVP & dev)

All are **third-party vendors** (managed Postgres in their cloud) unless self-hosted.

### Neon

Source: [neon.tech](https://neon.tech)

| | |
|--|--|
| **Model** | Serverless Postgres |
| **Free tier** | Yes — limited storage/compute (check current limits on site) |
| **Standout** | **Database branching** (dev branch from prod snapshot) |
| **Good for** | Auth.js + Postgres, Clerk + Postgres — **auth and DB separate** |
| **Watch outs** | Cold starts; connection limits on free; scale tier for always-on prod |

**Pros:** Clean split from auth vendor; portable; branching helps migrations.  
**Cons:** Another vendor; free tier not meant for serious prod SLA.

---

### Supabase (Postgres)

Source: [supabase.com/pricing](https://supabase.com/pricing)

| | |
|--|--|
| **Model** | Managed Postgres + optional auth, storage, realtime |
| **Free tier** | **$0** — 2 active projects, **500 MB DB** per project, 50k auth MAU if using their auth |
| **Paid** | Pro ~**$25/mo** per org — 8 GB disk, 100k MAU, daily backups |
| **Good for** | All-in-one MVP if you also use Supabase Auth |
| **Watch outs** | Free projects **pause after ~7 days inactivity**; no PITR on free; 500 MB fills up with messages/logs |

**Pros:** One dashboard for DB + auth + storage; RLS for tenant isolation.  
**Cons:** Platform coupling; egress/storage limits; free tier not 24/7 prod.

**Note:** You can use **Supabase only as Postgres** and auth elsewhere — but then you’re paying for bundled features you don’t use.

---

### Railway

| | |
|--|--|
| **Free tier** | Limited monthly credit (changes over time) |
| **Good for** | Quick throwaway Postgres + app on same platform |
| **Watch outs** | Credit exhaustion; less predictable long-term cost |

---

### Render

| | |
|--|--|
| **Free tier** | Postgres existed with expiration policies — verify current offering |
| **Good for** | Short spikes |
| **Watch outs** | Historically **not** reliable for free long-term prod DB |

---

### Azure Database for PostgreSQL — Flexible Server (Burstable)

| | |
|--|--|
| **Free tier** | **No** perpetual free tier for production PG (Azure free account credits only) |
| **Entry cost** | ~**$15–35/mo** Burstable B1ms class (verify region/pricing) |
| **Good for** | **Preferred long-term host** given team Azure skillset; HA, backup, US regions |
| **Watch outs** | More ops than Neon/Supabase; overkill for day-1 local dev |

**Team lean:** Start dev on Neon/local if cheaper; **plan migration to Azure PG** for first production with paying customers. See [azure-alignment.md](./azure-alignment.md).

---

## Host options — Tier 2: Production growth

| Provider | When to consider |
|----------|------------------|
| **Neon Scale** | Outgrow free; want serverless + branching at scale |
| **Supabase Pro** | Already on Supabase stack; need backups, no pause |
| **Azure PostgreSQL Flexible Server** | HA zone-redundant, geo-backup, Entra integration, US regions |
| **AWS RDS PostgreSQL** | If app lands on AWS (deprioritized — team Azure skillset) |
| **Google Cloud SQL (Postgres)** | If app on GCP |
| **Crunchy Bridge / Aiven** | Managed PG specialists, multi-cloud |

---

## Host options — Tier 3: Scale & redundancy

Capabilities to add as traffic and paying customers grow:

| Capability | Why |
|------------|-----|
| **Point-in-time recovery (PITR)** | Undo bad migration or data bug |
| **Connection pooling** (PgBouncer, Neon pooler, Supabase pooler) | App servers × connections will exhaust Postgres |
| **Read replica** | Reporting, heavy reads without hammering primary |
| **Multi-AZ / HA** | Failover when AZ goes down |
| **Monitoring & alerts** | Disk, connections, slow queries |
| **US region pinning** | Latency + compliance posture |

---

## Free tier comparison (verify before deciding)

| Provider | Free? | DB size (approx.) | Prod 24/7 on free? | Auth bundled? |
|----------|-------|-------------------|--------------------|---------------|
| **Neon** | Yes | Limited (check site) | Marginal — check SLA | No |
| **Supabase** | Yes | 500 MB / project | ⚠️ Pauses if idle 7d | Optional |
| **Railway** | Credit-based | Varies | No | No |
| **Render** | Limited | Varies | No | No |
| **Azure PG** | Credits only | N/A | Paid from start | No |
| **Auth.js + local Postgres** | $0 | Your machine | Dev only | N/A |

**Auth vendor free tiers (separate from DB):** see [auth-comparison-managed-vs-authjs.md](./auth-comparison-managed-vs-authjs.md#free-tier-clerk--supabase-auth).

---

## Pairing with auth (no decision — reference only)

| Auth approach | Natural DB host |
|---------------|-----------------|
| **Supabase Auth** | Supabase Postgres (same platform) |
| **Clerk** | Neon, Azure PG, RDS — any Postgres |
| **Auth.js** | Neon, Azure PG, RDS — any Postgres |
| **Entra + Azure** | Azure PostgreSQL |

Auth and DB vendors are **independent** unless you choose Supabase for both.

---

## Data volume rough guess (planning)

Order-of-magnitude for small GC SaaS — not capacity planning:

| Entity | Early | 100 GCs |
|--------|-------|---------|
| Companies | 1–10 | 100 |
| Projects | 5–50 | 1,000+ |
| Tasks / dependencies | 50–500 | 10k+ |
| Messages | 100–1k | 100k+ |
| Audit / cascade events | grows fast | index carefully |

**500 MB** (Supabase free) is fine for dev and demos; real prod with message history likely needs **paid tier or larger Neon** within first paying customers.

---

## Options explicitly not recommended (for now)

| Option | Why defer |
|--------|-----------|
| **SQLite in production** | Multi-tenant, concurrent writes, hosting |
| **Firebase/Firestore as primary** | Poor fit for dependency graph + SQL reporting |
| **Switch engines later** (PG → MySQL) | Migration pain — pick Postgres once if at all |
| **Self-hosted Postgres on a VPS** | Ops burden unless you enjoy DB admin |

---

## Open questions — database

- [ ] **ORM** — EF Core (lean with .NET API); see stack-web-api-db.md
- [ ] **Neon vs Supabase Postgres-only** vs **Azure from day one**
- [ ] **Multi-tenant model** — `company_id` on every row vs schema-per-tenant vs RLS
- [ ] **Message retention policy** — affects storage growth
- [ ] **Soft delete vs hard delete** for projects/messages
- [ ] **Read replica** — needed at what scale?
- [ ] **Backup RPO/RTO** targets before first paying customer
- [ ] **Local dev** — Docker Postgres vs Neon branch vs Supabase local

---

## Migration path (if host changes)

Postgres-to-Postgres moves are **feasible** (`pg_dump` / logical replication):

```
Dev:     Neon free branch OR local Docker
MVP:     Neon paid OR Supabase Pro OR Azure Burstable
Scale:   Azure/AWS HA + pooler + replica
```

Avoid features **locked to one vendor** (Supabase RLS policies, Neon-specific extensions) until host is stable — or accept migration cost.

---

## Next exploration steps

1. **Estimate row counts** after cascade + messaging MVP schema sketch
2. **Compare Neon vs Supabase Pro** at $25/mo for storage, connections, backups
3. **Azure PG pricing calculator** for US East — future path if team is Azure-heavy
4. **Spike:** Prisma or Drizzle migrations against Neon free — no product features

**No decision recorded** — log choices in [discovery-log.md](../discovery-log.md) when made.
