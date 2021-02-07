FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src

COPY "Services/Blogging/" "Services/Blogging/"
COPY "MercuryBus" "MercuryBus"

RUN dotnet publish Services/Blogging/Blogging.API/Blogging.API.csproj -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "Blogging.API.dll"]
