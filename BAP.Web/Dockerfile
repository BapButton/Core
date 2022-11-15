#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

####
#THIS WOULD ONLY BE USED FOR DEBUGGING.; THE BALENA ONE IS AT THE SOLUTION LEVEL SO IT CAN ACCESS ALL THE PROJECTS.
#i AM NOT SUE IT EVEN WORKS FOR THAT
###


FROM mcr.microsoft.com/dotnet/nightly/aspnet:6.0-bullseye-slim AS base
WORKDIR /app


FROM mcr.microsoft.com/dotnet/nightly/sdk:6.0-bullseye-slim AS build
WORKDIR /src
COPY ["../BapWeb/BapWeb.csproj", "../BapWeb/"]
COPY ["../TinkerButton/BapButton.csproj", "../TinkerButton/"]
COPY ["../BapShared/BapShared.csproj", "../BapShared/"]
COPY ["../MockButtonCore/MockButtonCore.csproj", "../MockButtonCore/"]
COPY ["../BapDb/BapDb.csproj", "../BapDb/"]
RUN dotnet restore "BapWeb/BapWeb.csproj"
COPY . .
WORKDIR "/src/BapWeb"
RUN dotnet build "BapWeb.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BapWeb.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .
# Expose the port in the container
EXPOSE 8080/tcp
ENV DOTNET_RUNNING_IN_CONTAINER=true \
  ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "BapWeb.dll"]
