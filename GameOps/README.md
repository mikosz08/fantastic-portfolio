# GameOps — ASP.NET Core API (.NET 9)

> Minimal API scaffold for **Milestone M0**.

[![CI](https://img.shields.io/github/actions/workflow/status/mikosz08/fantastic-portfolio/game_ops_ci.yml?branch=main)](../../actions)

---

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
GameOps/
  src/App/          # ASP.NET Core API project
  tests/            # test projects (to be added)
  .github/workflows # CI pipelines
  ops/              # Dockerfile, scripts 
  infra/            # IaC (later milestones)
  deploy/           # deployment manifests/pipelines (later milestones)
  docs/             # design notes
```

## Development

* Branching: trunk-based (`main` always deployable). Short-lived branches like `chore/m0-1-init-repo`.

## Database & Migrations
```bash
docker compose up -d db
dotnet ef migrations add Init --project src/App --startup-project src/App
dotnet ef database update --project src/App --startup-project src/App
```

## Evidence Index

Audit trail of artifacts and decisions. Add links as work progresses.

| Date (UTC) | Evidence                         | Path / Link                                                                 |
| ---------- | -------------------------------- | --------------------------------------------------------------------------- |
| 2025-09-16 | Solution & API project           | `GameOps.sln`, `src/App/App.csproj`                                         |
| 2025-09-16 | CI workflow (build .NET 9)       | `.github/workflows/game_ops_ci.yml`                                         |
| 2025-09-16 | First green CI on PR             | **[ M0-1 — Init repository (.NET 9) + CI stub #20 ](https://github.com/mikosz08/fantastic-portfolio/pull/20)** |
| 2025-09-18 | Dockerfile + docker-compose base | `ops/Dockerfile`, `ops/docker-compose.yml`, `.dockerignore`                 |
| 2025-09-18 | CI building Docker image on PR   | `.github/workflows/docker-build.yml`                                        |


## Release & Versioning (short)

* Version: SemVer via `<Version>` in `src/App/App.csproj`.
* Tag releases on `main`: `v0.1.0` (CI will publish artifacts in a later milestone).

