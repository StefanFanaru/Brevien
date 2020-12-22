#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src

COPY "Brevien.sln" "Brevien.sln"

COPY "Services/Identity/Identity.API/Identity.API.csproj" "Services/Identity/Identity.API/Identity.API.csproj"
COPY "Services/Identity/Identity.Data/Identity.Data.csproj" "Services/Identity/Identity.Data/Identity.Data.csproj"
COPY "Services/Identity/IdentityControl.API/IdentityControl.API.csproj" "Services/Identity/IdentityControl.API/IdentityControl.API.csproj"

COPY "docker-compose.dcproj" "docker-compose.dcproj"

RUN dotnet restore "Brevien.sln"

COPY . .
WORKDIR "/src/Services/Identity/Identity.API"
RUN dotnet publish --no-restore -c Release -o /app

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Identity.API.dll"]
