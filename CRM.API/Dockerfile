FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj files first for better layer caching.
COPY ["CRM.API/CRM.API.csproj", "CRM.API/"]
COPY ["CRM.Application/CRM.Application.csproj", "CRM.Application/"]
COPY ["CRM.Domain/CRM.Domain.csproj", "CRM.Domain/"]
COPY ["CRM.Infrastructure/CRM.Infrastructure.csproj", "CRM.Infrastructure/"]
COPY ["CRM.Shared/CRM.Shared.csproj", "CRM.Shared/"]

RUN dotnet restore "CRM.API/CRM.API.csproj"

# Copy the full source only after restore.
COPY . .
WORKDIR "/src/CRM.API"
RUN dotnet publish "CRM.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim AS final
WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

# Run as a non-root user (built-in UID from .NET runtime image).
USER $APP_UID

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CRM.API.dll"]
