# SQL Server vs PostgreSQL — for .NET + Azure

Status: **Exploratory** (2026-08-13)  
Related: [database-options.md](./database-options.md), [stack-web-api-db.md](./stack-web-api-db.md), [azure-alignment.md](./azure-alignment.md)

## Question

> If we are using .NET, did you consider **SQL Server** (Community / Express / free editions) or would **PostgreSQL** be better?

**Short answer:** .NET works **excellently with both**. SQL Server is the “native Microsoft” choice; **PostgreSQL is still a strong default** for a greenfield SaaS on Azure **without existing SQL Server licenses** — cheaper managed hosting, no Express caps, and portable dev tiers (Neon). SQL Server makes more sense if you want maximum Azure/Microsoft integration and are fine with **Express limits** (self-host) or **Azure SQL** pricing (managed).

**No decision recorded.**

---

## Naming: “Community Edition”?

Microsoft does **not** ship a production edition called “SQL Server Community Edition” (unlike MySQL Community).

| Edition | Free? | Production use? |
|---------|-------|-----------------|
| **[SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)** | ✅ Yes | ✅ **Yes** — small apps (see limits) |
| **Standard Developer / Enterprise Developer** | ✅ Yes | ❌ **Dev/test only** — not production |
| **Evaluation** | ✅ 180 days | ❌ Temp only |

If you meant “free SQL Server,” that’s **Express** (on-prem/VM) or **Azure SQL free/low tiers** (cloud), **not** Developer edition in prod.

### SQL Server 2025 Express limits (current)

- **50 GB** max database size (raised from 10 GB)
- **4 CPU cores** max
- **~1.4 GB** buffer pool memory
- Fine for **dev, hobby, tiny prod**; tight for multi-tenant SaaS with message history

Sources: [SQL Server downloads](https://www.microsoft.com/en-us/sql-server/sql-server-downloads), [What's new in SQL Server 2025](https://learn.microsoft.com/en-us/sql/sql-server/what-s-new-in-sql-server-2025)

---

## .NET + EF Core — both are first-class

| | SQL Server | PostgreSQL |
|--|------------|------------|
| **EF Core provider** | `Microsoft.EntityFrameworkCore.SqlServer` | `Npgsql.EntityFrameworkCore.PostgreSQL` |
| **Migrations, LINQ** | ✅ Excellent | ✅ Excellent |
| **Azure managed service** | Azure SQL Database | Azure Database for PostgreSQL Flexible Server |
| **Local dev free** | Express, LocalDB, Docker | Docker, Neon free, LocalDB N/A |

**Choosing .NET does not force SQL Server.** Most greenfield .NET SaaS apps today pick **Postgres or SQL Server** based on cost, ops, and portability — not framework lock-in.

---

## Comparison for ContractorPro

| Criterion | SQL Server (Express / Azure SQL) | PostgreSQL (Neon / Azure PG) |
|-----------|----------------------------------|------------------------------|
| **.NET fit** | ⭐⭐⭐ Native | ⭐⭐⭐ Excellent |
| **Azure fit** | ⭐⭐⭐ Azure SQL, Entra auth to SQL | ⭐⭐⭐ Azure Flexible Server |
| **Free local/dev** | Express, LocalDB | Docker, Neon branch |
| **Free production** | Express (capped) | Neon/Supabase free (capped) → Azure PG Burstable ~$12–35/mo |
| **Managed Azure cost (small)** | Azure SQL — often **higher** at same vCores | Azure PG — often **30–45% cheaper** without SQL license |
| **Existing SQL licenses** | **Azure Hybrid Benefit** can reduce Azure SQL cost | N/A |
| **SaaS portability** | Lower (T-SQL, Azure SQL specifics) | Higher (Postgres everywhere) |
| **JSON / flexible fields** | JSON types (improved in 2025) | `jsonb` — mature |
| **Multi-tenant SaaS norm** | Common in enterprise .NET | Very common in startups |
| **50 GB ceiling** | Express hard limit | Not an issue on managed PG |
| **Team skill** | Strong if you know T-SQL | Strong if documented as lean — learnable |

---

## When SQL Server is the better pick

- You already know **T-SQL** and want zero dialect friction
- Company has **SQL Server licenses** → **Azure Hybrid Benefit** on Azure SQL
- You want **Azure SQL Database** features (geo-replica, tight Entra integration to DB)
- Workload stays **small** and self-hosted **Express** is enough (unusual for multi-tenant SaaS)
- All-in Microsoft stack preference outweighs cost

## When PostgreSQL is the better pick

- **Greenfield** SaaS — no SQL Server licenses
- Want **cheaper** Azure managed DB at small/medium scale
- Want **free dev** with branching (Neon) before Azure prod
- Care about **vendor portability** (same DB on Neon dev → Azure PG prod)
- Planning docs already assume Postgres; team hasn’t cited deep T-SQL need
- Message/audit tables may grow — **avoid 50 GB Express wall**

---

## Azure hosting comparison (small app)

| Service | Typical entry | Notes |
|---------|---------------|-------|
| **Azure Database for PostgreSQL** — Burstable B1ms | ~**$12–35/mo** | Good prod starter |
| **Azure SQL Database** — serverless / small GP | ~**$15–50+/mo** | Can pause (serverless); licensing component |
| **SQL Server Express on VM** | VM cost only | You manage patches, backups, HA |
| **Neon / Docker (dev)** | $0 | Postgres only |

At **4+ vCores**, managed PostgreSQL on Azure is often **meaningfully cheaper** than Azure SQL without Hybrid Benefit ([comparison refs](https://azurelessons.com/azure-sql-vs-postgresql/)).

---

## Hybrid path (valid)

| Phase | SQL Server path | PostgreSQL path |
|-------|-----------------|-----------------|
| **Dev** | LocalDB / Express / Azure SQL dev | Docker / Neon free |
| **Prod** | Azure SQL | Azure PostgreSQL Flexible Server |
| **EF Core** | One provider switch is painful — **pick one early** | |

Do **not** plan to “start SQL Server and migrate to Postgres” casually — budget a migration project.

---

## Draft lean for ContractorPro (not final)

| Factor | Lean |
|--------|------|
| **.NET API** | Either DB works |
| **Azure + cost-conscious SaaS** | **PostgreSQL** slightly ahead |
| **No SQL Server licenses mentioned** | Postgres |
| **If you love T-SQL / already on SQL Server** | SQL Server Express (dev) → Azure SQL (prod) |

**Revisit SQL Server** if customer discovery or your own preference strongly favors T-SQL, or you obtain Hybrid Benefit licenses.

---

## Open questions

- [ ] Team comfort: **T-SQL vs PostgreSQL** dialect?
- [ ] Any **existing SQL Server** licenses or HA infrastructure?
- [ ] Will **50 GB Express** ever bind before you can afford Azure SQL/PG?
- [ ] **Entra ID auth to database** — requirement for either platform (both support on Azure)
- [ ] EF Core spike: same entity model on both — feel test?

Log decisions in [discovery-log.md](../discovery-log.md).
