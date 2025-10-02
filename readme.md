# GameOps - ASP.NET Core API
[![CI](https://github.com/mikosz08/fantastic-portfolio/actions/workflows/game_ops_ci.yml/badge.svg)](https://github.com/mikosz08/fantastic-portfolio/actions/workflows/game_ops_ci.yml)
[![Docker](https://github.com/mikosz08/fantastic-portfolio/actions/workflows/docker_build.yml/badge.svg)](https://github.com/mikosz08/fantastic-portfolio/actions/workflows/docker_build.yml)

> Minimal API scaffold for **Milestone M0**.

## Quickstart

### Prerequisites

* .NET SDK **9.x** (pinned via `global.json`)
* Docker (optional, for container run)

### Run locally

```bash
dotnet --info
dotnet restore
dotnet build -c Release
dotnet run --project src/App
```

### Run in Docker

```bash
# build & run with compose
docker compose -f ops/docker-compose.yml up --build

# stop & clean
docker compose -f ops/docker-compose.yml down
```

---

## Project layout

```
.github/workflows # CI pipelines
GameOps/
  src/App/          # ASP.NET Core API project
  tests/            # test projects (to be added)
  ops/              # Dockerfile, scripts 
  infra/            # IaC (later milestones)
  deploy/           # deployment manifests/pipelines (later milestones)
  docs/             # design notes
```

---

## Development

* Branching: trunk-based (`main` always deployable). Short-lived branches like `chore/m0-1-init-repo`.

---

## Database & Migrations
```bash
docker compose up -d db
dotnet ef migrations add Init --project src/App --startup-project src/App
dotnet ef database update --project src/App --startup-project src/App
```

---

## Smoke Tests 

### POST /events

```bash
curl --location 'http://localhost:5245/events' \
--header 'Content-Type: application/json' \
--header 'Idempotency-Key: ik-327' \
--data '{
    "type": "item_collected",
    "occurredAt": "2025-09-15T12:34:56Z",
    "playerId": "p-001",
    "sessionId": "s-abc",
    "payload": { "foo": "bar" }
  }'
```

---

## Evidence Index

Audit trail of artifacts and decisions. Add links as work progresses.

| Date (UTC) | Evidence                         | Path / Link                                                                 |
| ---------- | -------------------------------- | --------------------------------------------------------------------------- |
| 2025-09-17 | M0‑1            | **[ Scaffold & MVP Endpoints ](https://github.com/mikosz08/fantastic-portfolio/pull/20)** |
| 2025-09-18 | M0‑2            | **[ Containerize the App ](https://github.com/mikosz08/fantastic-portfolio/pull/21)** |
| 2025-09-19 | M0‑3            | **[ Events Model, EF Core, Postgres ](https://github.com/mikosz08/fantastic-portfolio/pull/24)** |
| 2025-09-19 | M0‑4-1            | **[ POST /events - DTOs & validation ](https://github.com/mikosz08/fantastic-portfolio/pull/30)** |
| ---------- | -------------------------------- | --------------------------------------------------------------------------- |

---

## Project Stack

| Date       | Area        | Tech / Badge                                                                                                  |
|------------|-------------|----------------------------------------------------------------------------------------------------------------|
| 2025-09-19 | Runtime     | ![.NET 9.0](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)                                          |
| 2025-09-19 | Web API     | ASP.NET Core Minimal API                                                                                        |
| 2025-09-19 | Data Access | EF Core + Npgsql                                                                                                |
| 2025-09-19 | Database    | ![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-336791?logo=postgresql&logoColor=white)               |
| 2025-09-19 | Container   | ![Docker](https://img.shields.io/badge/Docker-Buildx-2496ED?logo=docker&logoColor=white) • docker compose      |
| 2025-09-19 | CI/CD       | ![GitHub Actions](https://img.shields.io/badge/GitHub%20Actions-CI-2088FF?logo=githubactions&logoColor=white) |
| 2025-09-19 | Security    | ![CodeQL](https://img.shields.io/badge/CodeQL-SEC-181717?logo=github) • Dependabot                            |
| 2025-09-19 | Docs        | Swagger / OpenAPI                                                                                               |
| 2025-09-19 | Testing     | xUnit (unit/integration)                                                                                        |
| 2025-09-19 | Packaging   | Docker image (GHCR-ready)                                                                                       |
| 2025-09-19 | Hosting     | _(TBD: e.g., Fly.io / Azure App Service / K8s)_                                                                |

---

## Release & Versioning (todo, maybe in 0-4-2)

* Version: SemVer via `<Version>` in `src/App/App.csproj`.
* Tag releases on `main`: `v0.1.0` (CI will publish artifacts in a later milestone).


