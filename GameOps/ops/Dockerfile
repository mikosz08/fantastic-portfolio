# syntax=docker/dockerfile:1

### Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY GameOps.sln ./
COPY src/App/App.csproj src/App/
RUN dotnet restore src/App/App.csproj

COPY . .
RUN dotnet publish src/App/App.csproj -c Release -o /out -p:UseAppHost=false --no-restore

### Run
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /out ./

HEALTHCHECK --interval=10s --timeout=3s --start-period=10s --retries=3 \
  CMD curl -fsS http://localhost:8080/readyz || exit 1

ENTRYPOINT ["dotnet", "App.dll"]
