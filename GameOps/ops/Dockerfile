# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# copy solution + project for restore-layer caching
COPY GameOps.sln ./
COPY src/App/App.csproj src/App/

RUN dotnet restore src/App/App.csproj

# copy the rest and publish
COPY . .
RUN dotnet publish src/App/App.csproj -c Release -o /out -p:UseAppHost=false --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /out ./
ENV ASPNETCORE_URLS=http://+:8080 \
    DOTNET_EnableDiagnostics=0
ENTRYPOINT ["dotnet", "App.dll"]
