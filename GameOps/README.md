# GameOps — ASP.NET Core API (.NET 9)

> Minimal API scaffold for **Milestone M0**.

[![CI](https://img.shields.io/github/actions/workflow/status/<org>/<repo>/game_ops_ci.yml?branch=main)](../../actions)

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

### Run in Docker (placeholder)

> Dockerfile will be added in a later issue (M0-2). Commands shown for reference:

```bash
# build
docker build -t gameops/app:dev -f ops/Dockerfile .

# run (maps port 8080)
docker run --rm -p 8080:8080 -e ASPNETCORE_URLS=http://+:8080 gameops/app:dev
```

---

## Project layout

```
src/App/          # ASP.NET Core API project
tests/            # test projects (to be added)
.github/workflows # CI pipelines
ops/              # Dockerfile, scripts (upcoming)
infra/            # IaC (later milestones)
deploy/           # deployment manifests/pipelines (later milestones)
docs/             # design notes
```

## Development

* Branching: trunk-based (`main` always deployable). Short-lived branches like `chore/m0-1-init-repo`.

## Evidence Index

Audit trail of artifacts and decisions. Add links as work progresses.

| Date (UTC) | Evidence                   | Path / Link                         |
| ---------- | -------------------------- | ----------------------------------- |
| 2025-09-16 | Solution & API project     | `GameOps.sln`, `src/App/App.csproj` |
| 2025-09-16 | CI workflow (build .NET 9) | `.github/workflows/game_ops_ci.yml` |
| 2025-09-16 | First green CI on PR       | **[ M0‑1 — Init repository (.NET 9) + CI stub #20 ](https://github.com/mikosz08/fantastic-portfolio/pull/20)**             |

## Release & Versioning (short)

* Version: SemVer via `<Version>` in `src/App/App.csproj`.
* Tag releases on `main`: `v0.1.0` (CI will publish artifacts in a later milestone).

