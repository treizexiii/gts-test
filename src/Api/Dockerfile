FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/Api", "Api"]
COPY ["src/Core", "Core"]
COPY ["src/Commands", "Commands"]
COPY ["src/Query", "Query"]

WORKDIR /src/Api
RUN dotnet restore
RUN dotnet build -c Release -o /app/build
FROM build AS publish
ARG configuration=Release
RUN dotnet publish -c $configuration -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN useradd -r nonroot
RUN chown -R nonroot /app
USER nonroot

ENTRYPOINT ["dotnet", "Api.dll"]