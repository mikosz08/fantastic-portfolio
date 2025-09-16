# GameOps — ASP.NET Core API (.NET 9)

> Short description: **PLACEHOLDER** — minimal API scaffold for M0.

[![CI](https://img.shields.io/github/actions/workflow/status/<org>/<repo>/game_ops_ci.yml?branch=main)](../../actions)

## Quickstart

### Prerequisites
- .NET SDK **9.x** (pinned via `global.json`)
- Docker (optional, for container run)

### Run locally
```bash
dotnet --info
dotnet restore
dotnet build -c Release
dotnet run --project src/App
```

# build
docker build -t gameops/app:dev -f ops/Dockerfile .
# run (maps port 8080)
docker run --rm -p 8080:8080 -e ASPNETCORE_URLS=http://+:8080 gameops/app:dev

```bash
src/App/          # ASP.NET Core API project
tests/            # test projects (to be added)
.github/workflows # CI pipelines
ops/              # Dockerfile, scripts (upcoming)
infra/ deploy/    # IaC / deployment (later milestones)
docs/             # design notes
```