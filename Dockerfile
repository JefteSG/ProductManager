# ── Stage 1: build ────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Restaurar dependências primeiro (aproveita cache de camadas do Docker)
COPY ProductManager.csproj .
RUN dotnet restore

# Copiar o código-fonte e publicar
COPY . .
RUN dotnet publish -c Release -o /app/publish --no-restore

# ── Stage 2: runtime ──────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# curl é necessário para o healthcheck do docker-compose (não está na imagem base)
RUN apt-get update && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

# O arquivo SQLite será armazenado em um volume nomeado montado em /app/data
RUN mkdir -p /app/data

ENV ASPNETCORE_URLS=http://+:8080
ENV ConnectionStrings__DefaultConnection="Data Source=/app/data/products.db"

EXPOSE 8080

ENTRYPOINT ["dotnet", "ProductManager.dll"]
