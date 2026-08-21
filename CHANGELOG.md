# CRM SaaS System — Project Change Log

> **Purpose:** This is the single source of truth for all modifications, decisions, and planned work in this repository.
> Update this file **every time** a change is made — before committing.
>
> **Format:**
> - One entry per session/PR under the correct phase.
> - Status badges: `✅ Done` · `🔄 In Progress` · `⏳ Planned` · `❌ Blocked` · `⚠️ Needs Decision`
> - Newest entries go at the **top** of each phase.

---

## Table of Contents

1. [Project Overview](#project-overview)
2. [Architecture Summary](#architecture-summary)
3. [Migration Phases & Roadmap](#migration-phases--roadmap)
4. [Change Log](#change-log)
   - [Phase 0 — Repository Restructure](#phase-0--repository-restructure--solution-scaffolding)
   - [Phase 1 — Architecture Cleanup](#phase-1--architecture-cleanup-planned)
   - [Phase 2 — Database Separation](#phase-2--database-separation-planned)
   - [Phase 3 — CRM Core Extraction](#phase-3--crm-core-extraction-planned)
   - [Phase 4 — Async Messaging](#phase-4--async-messaging-planned)
   - [Phase 5 — Gateway & Containerization](#phase-5--gateway--containerization-planned)
5. [Open Architect Decisions](#open-architect-decisions)
6. [Migration Priority Table](#migration-priority-table)
7. [Service Ownership Map](#service-ownership-map)

---

## Project Overview

| Field | Value |
|:---|:---|
| **Project** | CRM SaaS System |
| **Architecture Target** | Distributed Microservices (Clean Architecture per service) |
| **Current State** | Monolith (`backend/`) — fully functional, Clean Architecture |
| **Active Branch** | `feature/microservices-migration` |
| **Framework** | .NET 8 |
| **Database** | SQL Server via Entity Framework Core 8 |
| **Auth** | JWT Bearer + Google OAuth 2.0 |
| **Patterns** | CQRS (MediatR), Repository, Unit of Work, Audit Interceptor |
| **Validation** | FluentValidation |
| **Logging** | Serilog (Console + File sinks) + Correlation IDs |

---

## Architecture Summary

### Current Monolith Structure (`backend/`)

```
backend/
├── CRM.Domain/          → Entities, Enums, Specs — ZERO external refs
├── CRM.Shared/          → Constants, Exceptions, Audit events — ZERO refs
├── CRM.Application/     → CQRS handlers, DTOs, Interfaces → Domain, Shared
├── CRM.Infrastructure/  → EF Core, Repos, JWT, Email, OAuth → Application, Domain, Shared
└── CRM.API/             → REST controllers, DI, Middleware → all
```

### Target Microservices Structure

```
services/
├── identity/            → Users, Tenants, Roles, JWT issuance
└── core/                → Customers, Leads, Deals, Activities, LeadConversionHistories
```

### Target Project Reference Rules (Clean Architecture — per service)

```
API → Application + Infrastructure
Infrastructure → Application + Domain
Application → Domain
Domain → (nothing)
```

---

## Migration Phases & Roadmap

| Phase | Name | Status | Blocking Issue |
|:---:|:---|:---:|:---|
| **0** | Repository Restructure & Solution Scaffolding | ✅ Done | — |
| **1** | Architecture Cleanup (Decouple JWT, Read Models) | ⏳ Planned | Decision on session revocation strategy |
| **2** | Database Separation (Split DbContext, Migrations) | ⏳ Planned | Requires Phase 1 complete |
| **3** | CRM Core Extraction (Migrate business logic) | ⏳ Planned | Requires Phase 2 complete |
| **4** | Async Messaging (RabbitMQ / MassTransit) | ⏳ Planned | Requires Phase 3 complete |
| **5** | API Gateway & Containerization (YARP, Docker, K8s) | ⏳ Planned | Requires Phase 4 complete |

---

## Change Log

---

### Phase 0 — Repository Restructure & Solution Scaffolding

---

#### [2026-08-21] BUGFIX — CRM Core Projects Appearing at Solution Root ✅ Done

**Problem:**
The four `CRM.Core.*` projects appeared at the Visual Studio Solution Explorer root instead of nested under `services → core`.

**Root Cause:**
Project GUIDs contained non-hexadecimal characters (`R`, `M` in `{C0RE0001-D0MA-...}`). Visual Studio silently rejects malformed GUIDs in the `NestedProjects` section, causing nesting to be ignored and projects to float to the solution root.

**Fix:**
- Replaced all four Core project GUIDs in [`CRM.sln`](./CRM.sln) and [`services/core/CRM.Core.sln`](./services/core/CRM.Core.sln) with valid RFC-compliant hex-only GUIDs.

| Project | Old GUID (invalid) | New GUID (valid) |
|:---|:---|:---|
| CRM.Core.Domain | `{C0RE0001-D0MA-4111-A000-000000000001}` | `{CC000001-0000-4000-A000-000000000001}` |
| CRM.Core.Application | `{C0RE0002-APPL-4222-B000-000000000002}` | `{CC000002-0000-4000-A000-000000000002}` |
| CRM.Core.Infrastructure | `{C0RE0003-INFR-4333-C000-000000000003}` | `{CC000003-0000-4000-A000-000000000003}` |
| CRM.Core.API | `{C0RE0004-APIA-4444-D000-000000000004}` | `{CC000004-0000-4000-A000-000000000004}` |

**Validation:**
```
dotnet build services/core/CRM.Core.sln  → Build succeeded. 0 Warning(s), 0 Error(s)
dotnet build CRM.sln                     → Build succeeded. 0 Warning(s), 0 Error(s)
```

**Files Modified:**
- [`CRM.sln`](./CRM.sln)
- [`services/core/CRM.Core.sln`](./services/core/CRM.Core.sln)

---

#### [2026-08-21] CRM Core Microservice Scaffolding ✅ Done

**Objective:**
Create Clean Architecture project scaffolding for the future CRM Core service under `services/core/`.

**What was created:**

| File | Description |
|:---|:---|
| [`services/core/CRM.Core.sln`](./services/core/CRM.Core.sln) | Service-level solution (replaced empty Phase 0 placeholder) |
| [`services/core/CRM.Core.Domain/CRM.Core.Domain.csproj`](./services/core/CRM.Core.Domain/CRM.Core.Domain.csproj) | Class Library — net8.0 — ZERO project references |
| [`services/core/CRM.Core.Application/CRM.Core.Application.csproj`](./services/core/CRM.Core.Application/CRM.Core.Application.csproj) | Class Library — net8.0 → Domain |
| [`services/core/CRM.Core.Infrastructure/CRM.Core.Infrastructure.csproj`](./services/core/CRM.Core.Infrastructure/CRM.Core.Infrastructure.csproj) | Class Library — net8.0 → Application + Domain |
| [`services/core/CRM.Core.API/CRM.Core.API.csproj`](./services/core/CRM.Core.API/CRM.Core.API.csproj) | Web API — net8.0 → Application + Infrastructure |
| [`services/core/CRM.Core.API/Program.cs`](./services/core/CRM.Core.API/Program.cs) | Minimal ASP.NET Core bootstrap (controllers + Swagger) |
| [`services/core/CRM.Core.API/appsettings.json`](./services/core/CRM.Core.API/appsettings.json) | Standard logging config |
| [`services/core/CRM.Core.API/Properties/launchSettings.json`](./services/core/CRM.Core.API/Properties/launchSettings.json) | HTTP port 5004 / HTTPS 7192 |

**Project reference graph verified:**
```
CRM.Core.API
 ├── CRM.Core.Application
 └── CRM.Core.Infrastructure
        ├── CRM.Core.Application
        └── CRM.Core.Domain

CRM.Core.Domain → (nothing)
```

**Root solution updated:**
All four Core projects added to [`CRM.sln`](./CRM.sln) under `services → core` solution folder.

**What was NOT done (by design):**
- No business logic migrated (entities, repos, handlers, controllers)
- No Identity service references added
- No infrastructure (RabbitMQ, Redis, gRPC, EF migrations)
- Monolith (`backend/`) was not touched

**Validation:**
```
dotnet build services/core/CRM.Core.sln  → Build succeeded. 0 Warning(s), 0 Error(s)
dotnet build CRM.sln                     → Build succeeded. 0 Warning(s), 0 Error(s)
```

---

#### [2026-08-21] Identity Service Scaffolding ✅ Done

**Objective:**
Create Clean Architecture project scaffolding for the Identity service under `services/identity/`.

**What was created:**

| File | Description |
|:---|:---|
| [`services/identity/CRM.Identity.sln`](./services/identity/CRM.Identity.sln) | Identity service solution |
| [`services/identity/CRM.Identity.Domain/`](./services/identity/CRM.Identity.Domain/) | Class Library — net8.0 — ZERO project references |
| [`services/identity/CRM.Identity.Application/`](./services/identity/CRM.Identity.Application/) | Class Library — net8.0 → Identity.Domain |
| [`services/identity/CRM.Identity.Infrastructure/`](./services/identity/CRM.Identity.Infrastructure/) | Class Library — net8.0 → Identity.Application + Identity.Domain |
| [`services/identity/CRM.Identity.API/`](./services/identity/CRM.Identity.API/) | Web API — net8.0 → Identity.Application + Identity.Infrastructure, port 5003/7191 |

**Root solution updated:**
All four Identity projects added to [`CRM.sln`](./CRM.sln) under `services → identity` solution folder.

---

#### [2026-08-21] Repository-Level Restructuring ✅ Done

**Objective:**
Establish the top-level folder and solution structure for the microservices migration without touching the existing monolith.

**What was created:**

```
CRM/
├── CRM.sln                         → Root solution (all services visible in one IDE view)
├── backend/                        → Existing monolith (untouched)
├── services/
│   ├── identity/                   → Identity service home
│   └── core/                       → CRM Core service home
├── shared/
│   ├── CRM.Contracts/              → Future shared event contracts
│   └── CRM.SharedKernel/           → Future shared domain primitives
├── gateway/
│   └── CRM.Gateway/                → Future YARP API Gateway project placeholder
├── infrastructure/
│   ├── kubernetes/                 → K8s manifests (core/, identity/, gateway/)
│   └── monitoring/                 → Prometheus + Grafana config
├── docs/
│   └── architecture/
│       ├── adr/                    → Architecture Decision Records
│       └── diagrams/               → Architecture diagrams
└── tests/                          → Future cross-service integration tests
```

**Solution folder hierarchy in `CRM.sln`:**
```
Solution 'CRM'
├── backend/     → CRM.API, CRM.Application, CRM.Domain, CRM.Infrastructure, CRM.Shared
├── services/
│   ├── identity/ → CRM.Identity.API, .Application, .Domain, .Infrastructure
│   └── core/     → CRM.Core.API, .Application, .Domain, .Infrastructure
├── shared/
├── gateway/
└── tests/
```

**Monolith status:** Fully functional. No files modified. All backend builds pass.

---

### Phase 1 — Architecture Cleanup ⏳ Planned

**Goal:** Decouple the two critical runtime dependencies that block physical service extraction.

---

#### [PLANNED] Decouple JWT Validation from Database Reads

**Priority:** 🔴 Critical (Priority 1 — blocks all extraction)

**Problem:**
Every authenticated API request in the monolith triggers two synchronous DB reads:
1. Read token version to detect forced logouts (`AccessTokenStateValidator`)
2. Read user lockout status (`IUserRepository`)

When CRM Core is deployed as a separate service with its own database, it will have **no access** to the Identity database — making these reads impossible.

**Proposed Solution:**
- Embed `tokenVersion` and `lockout` state as **JWT claims** at issuance time.
- The Identity service signs the token with these claims baked in.
- CRM Core validates the signature cryptographically — no DB read needed.
- Short token lifetime (5–15 min) limits the blast radius of stale claims.

**Files to modify (Identity service — future):**
- `IAccessTokenStateValidator` — replace DB-based implementation with claim-based validation
- JWT issuance logic — add `tokenVersion` and `lockoutEnabled` claims
- `AuthenticationExtensions` — remove DB validator middleware

**Decision required:** ⚠️ See [Open Architect Decisions](#open-architect-decisions) — Q1: Session Revocation Strategy.

**Status:** ⏳ Planned — awaiting architect decision on session revocation

---

#### [PLANNED] Implement Local User / Tenant Read Models in CRM Core

**Priority:** 🟠 High (Priority 2)

**Problem:**
The `AssignLead` command directly calls `IUserRepository` (Identity's data) to validate that an assignee exists and is active. This is a synchronous cross-service DB query that breaks service isolation.

**Proposed Solution:**
- Introduce a lightweight `UserReadModel` (id, name, email, isActive, tenantId) in CRM Core's local database.
- Populated initially via a data seed / sync script.
- Kept eventually consistent via integration events (Phase 4).
- The `AssignLead` handler queries the local read model — no cross-service call.

**Decision required:** ⚠️ See [Open Architect Decisions](#open-architect-decisions) — Q2: User Cache Ownership.

**Status:** ⏳ Planned — depends on Decision Q2

---

### Phase 2 — Database Separation ⏳ Planned

**Goal:** Give each service its own isolated database and EF DbContext.

---

#### [PLANNED] Split AppDbContext into Service-Scoped DbContexts

**Priority:** 🟠 High (Priority 3)

**Problem:**
All entities currently share a single `AppDbContext` in `CRM.Infrastructure`. This means:
- Shared EF migrations (one migration history table for all tables)
- Shared transaction boundaries (a bug in CRM Core can roll back Identity writes)
- Shared `AuditInterceptor` scoped to a single context

**Plan:**
- Create `IdentityDbContext` in `CRM.Identity.Infrastructure` → owns: `Users`, `Tenants`, `Roles`, `UserRoles`, `AuditLogs` (identity scope)
- Create `CoreDbContext` in `CRM.Core.Infrastructure` → owns: `Customers`, `Leads`, `Deals`, `Activities`, `LeadConversionHistories`, `AuditLogs` (core scope)
- Separate EF migration histories per DbContext
- Separate connection strings in each service's `appsettings.json`

**Status:** ⏳ Planned — requires Phase 1 complete

---

### Phase 3 — CRM Core Extraction ⏳ Planned

**Goal:** Physically migrate business logic from `backend/` into `services/core/`.

**Entities to migrate (identified in Microservices Readiness Assessment):**

| Entity | Current Location | Target Layer |
|:---|:---|:---|
| `Customer` | `backend/CRM.Domain/Entities/Customer.cs` | `CRM.Core.Domain` |
| `Lead` | `backend/CRM.Domain/Entities/Lead.cs` | `CRM.Core.Domain` |
| `Deal` | `backend/CRM.Domain/Entities/Deal.cs` | `CRM.Core.Domain` |
| `Activity` | `backend/CRM.Domain/Entities/Activity.cs` | `CRM.Core.Domain` |
| `LeadConversionHistory` | `backend/CRM.Domain/Entities/LeadConversionHistory.cs` | `CRM.Core.Domain` |
| Repositories (Customers, Leads, Deals, Activities) | `backend/CRM.Infrastructure/Repositories/CRMCore/` | `CRM.Core.Infrastructure` |
| CQRS Handlers (Customers, Leads, Deals, Activities) | `backend/CRM.Application/CRM/` | `CRM.Core.Application` |
| Controllers (Customers, Leads, Deals, Activities) | `backend/CRM.API/Controllers/` | `CRM.Core.API` |
| EF Configurations | `backend/CRM.Infrastructure/Configurations/CRMCore/` | `CRM.Core.Infrastructure` |
| Validators, DTOs | `backend/CRM.Application/CRM/*/` | `CRM.Core.Application` |

> **Rule:** Do NOT remove from `backend/` until the Core service is fully tested and deployed.

**Status:** ⏳ Planned — requires Phase 2 complete

---

### Phase 4 — Async Messaging ⏳ Planned

**Goal:** Replace synchronous cross-service dependencies with event-driven eventual consistency.

**Planned events:**

| Event | Publisher | Consumer | Purpose |
|:---|:---|:---|:---|
| `UserCreated` | Identity | CRM Core | Populate local UserReadModel |
| `UserUpdated` | Identity | CRM Core | Sync user name/status changes |
| `UserDeactivated` | Identity | CRM Core | Mark local UserReadModel inactive |
| `TenantCreated` | Identity | CRM Core | Populate local TenantReadModel |

**Tech stack decision (not yet made):**
- Option A: RabbitMQ + MassTransit
- Option B: Azure Service Bus
- Option C: Kafka (if high-throughput required)

**Status:** ⏳ Planned — requires Phase 3 complete

---

### Phase 5 — Gateway & Containerization ⏳ Planned

**Goal:** Unified entry point and production-ready deployment.

**Planned work:**

| Item | Description |
|:---|:---|
| YARP API Gateway | `gateway/CRM.Gateway/` — route `/api/identity/*` and `/api/core/*` |
| Docker | `Dockerfile` per service, update `docker-compose.dev.yml` |
| Kubernetes | Manifests in `infrastructure/kubernetes/` (already scaffolded) |
| Monitoring | Prometheus scrape config + Grafana dashboards |
| Health checks | `/healthz` and `/readyz` endpoints per service |

**Status:** ⏳ Planned — requires Phase 4 complete

---

## Open Architect Decisions

| # | Question | Impact | Status |
|:---|:---|:---|:---|
| **Q1** | **Session Revocation Strategy** — Short JWT lifespans (5–15 min, accept stale window) vs. Redis distributed revocation cache (real-time, added infra)? | Affects Phase 1 decoupling approach | ⚠️ Needs Decision |
| **Q2** | **User Cache Ownership** — Local read-only `UserReadModel` table in CRM Core DB vs. gRPC call to Identity with caching layer? | Affects Phase 1 + Phase 3 implementation | ⚠️ Needs Decision |
| **Q3** | **Audit Log Strategy** — Local audit tables per service (simple, per-service isolation) vs. Central Audit microservice via async events (unified, more infra)? | Affects Phase 2 DbContext split | ⚠️ Needs Decision |
| **Q4** | **Async Messaging Technology** — RabbitMQ+MassTransit vs. Azure Service Bus vs. Kafka? | Affects Phase 4 | ⚠️ Needs Decision |

---

## Migration Priority Table

*(From `MICROSERVICES_READINESS_ASSESSMENT.md` — Table C)*

| Priority | Change | Reason | Risk if Delayed |
|:---|:---|:---|:---|
| **1 — Critical** | Decouple JWT validation database reads | Token validation blocks physical microservice isolation | Immediate extraction failure |
| **2 — High** | Implement local User/Tenant Read Models in CRM | Solves synchronous cross-service DB queries | Service boundary breakdown |
| **3 — High** | Split DbContext and migrations | Segregates persistence layers | Shared data transaction failures |
| **4 — Medium** | RabbitMQ integration events | Synchronizes local data cache eventually | Data state inconsistency |
| **5 — Low** | API Gateway and containerization | Simplifies consumer integrations and deployment | Deployment complexity |

---

## Service Ownership Map

*(From `MICROSERVICES_READINESS_ASSESSMENT.md` — Table A)*

| Entity | Current Location | Target Service | Migration Difficulty |
|:---|:---|:---|:---|
| `Tenant` | `CRM.Domain/Entities/Tenant.cs` | **Identity** | Low |
| `User` | `CRM.Domain/Entities/User.cs` | **Identity** | Low |
| `Role`, `UserRole` | `CRM.Domain/Entities/Role.cs` | **Identity** | Low |
| `Customer` | `CRM.Domain/Entities/Customer.cs` | **CRM Core** | Low |
| `Lead` | `CRM.Domain/Entities/Lead.cs` | **CRM Core** | Low |
| `Deal` | `CRM.Domain/Entities/Deal.cs` | **CRM Core** | Low |
| `Activity` | `CRM.Domain/Entities/Activity.cs` | **CRM Core** | Low |
| `LeadConversionHistory` | `CRM.Domain/Entities/LeadConversionHistory.cs` | **CRM Core** | Low |
| `AuditLog` | `CRM.Domain/Entities/AuditLog.cs` | **Shared / Local** | Medium |

---

## How to Update This File

When you make any change to this repository:

1. **Find the correct phase section** above.
2. **Add a new `####` entry** at the **top** of that phase section.
3. **Use this template:**

```markdown
#### [YYYY-MM-DD] Brief Title of Change ✅ Done / 🔄 In Progress / ⏳ Planned

**Objective / Problem:**
One sentence.

**What changed:**
- File or folder created/modified/deleted
- ...

**Validation:**
\```
command run → result
\```

**Files Modified:**
- [path/to/file](./path/to/file)
```

4. Update the **Phase status badge** in the [Migration Phases](#migration-phases--roadmap) table if the phase status changed.
5. If an **architect decision** was made, move it from `⚠️ Needs Decision` to `✅ Decided: <answer>` in the [Open Architect Decisions](#open-architect-decisions) table.

---

*Last updated: 2026-08-21 — CRM Core scaffolding complete, GUID bug fixed.*
