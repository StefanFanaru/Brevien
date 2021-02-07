FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src

COPY "Services/IdentityServer/" ./
RUN dotnet publish IdentityServer.API/IdentityServer.API.csproj -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim
WORKDIR /app
EXPOSE 443
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "IdentityServer.API.dll"]
