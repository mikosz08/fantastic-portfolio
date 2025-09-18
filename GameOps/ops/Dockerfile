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
ENTRYPOINT ["dotnet", "App.dll"]
