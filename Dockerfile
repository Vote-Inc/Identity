FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Identity.Domain/Identity.Domain.csproj Identity.Domain/
COPY Identity.Application/Identity.Application.csproj Identity.Application/
COPY Identity.Infrastructure/Identity.Infrastructure.csproj Identity.Infrastructure/
COPY Identity.API/Identity.API.csproj Identity.API/

RUN dotnet restore Identity.API/Identity.API.csproj

COPY . .

RUN dotnet publish Identity.API/Identity.API.csproj \
-c Release \
-o /app/publish \
--no-restore
    
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN groupadd --system --gid 1001 appgroup && \
    useradd --system --uid 1001 --gid appgroup appuser

RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

RUN chown -R appuser:appgroup /app

USER appuser

EXPOSE 8080

HEALTHCHECK --interval=10s --timeout=5s --start-period=30s --retries=3 \
  CMD curl -sf http://localhost:8080/health || exit 1
  
ENTRYPOINT ["dotnet", "Identity.API.dll"]

