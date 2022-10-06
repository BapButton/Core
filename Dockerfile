ARG VERSION=6.0-bullseye-slim-arm32v7

#FROM mcr.microsoft.com/dotnet/aspnet:$VERSION-arm32v7 AS base
FROM mcr.microsoft.com/dotnet/aspnet:6.0.0-preview.7-bullseye-slim-arm32v7 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0.100-preview.7-bullseye-slim AS build
WORKDIR /src
COPY ["BapWeb/BapWeb.csproj", "BapWeb/"]
COPY ["TinkerButton/BapButton.csproj", "TinkerButton/"]
COPY ["BapShared/BapShared.csproj", "BapShared/"]
COPY ["MockButtonCore/MockButtonCore.csproj", "MockButtonCore/"]
COPY ["BapDb/BapDb.csproj", "BapDb/"]
RUN dotnet restore "BapWeb/BapWeb.csproj"
COPY . .
WORKDIR "/src/BapWeb"
RUN dotnet build "BapWeb.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BapWeb.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .
ENV DOTNET_RUNNING_IN_CONTAINER=true \
  ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "BapWeb.dll"]
