#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

WORKDIR /app
EXPOSE 3516

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ReportService/ReportService/ReportService.csproj", "ReportService/ReportService/"]
COPY ["ReportService/ReportServiceTests/ReportServiceTests.csproj", "ReportService/ReportServiceTests/"]
RUN dotnet restore "./ReportService/ReportService/ReportService.csproj"
RUN dotnet restore "./ReportService/ReportServiceTests/ReportServiceTests.csproj"

COPY . .
WORKDIR "/src/ReportService/ReportService"
RUN dotnet build "./ReportService.csproj" -c $BUILD_CONFIGURATION -o /app/build

WORKDIR "/src/ReportService/ReportServiceTests"
RUN dotnet build "./ReportServiceTests.csproj" -c $BUILD_CONFIGURATION
RUN dotnet test "./ReportServiceTests.csproj" -c $BUILD_CONFIGURATION --no-build --logger:"trx;LogFileName=TestResults.trx"

FROM build AS publish
WORKDIR "/src/ReportService/ReportService"
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ReportService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN mkdir -p /app/wwwroot && chown -R app:app /app/wwwroot && chmod -R 755 /app/wwwroot
USER app
ENTRYPOINT ["dotnet", "ReportService.dll"]