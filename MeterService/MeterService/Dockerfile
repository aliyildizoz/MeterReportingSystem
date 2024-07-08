#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 3515
EXPOSE 3514

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MeterService/MeterService/MeterService.csproj", "MeterService/MeterService/"]
COPY ["MeterService/MeterServiceTests/MeterServiceTests.csproj", "MeterService/MeterServiceTests/"]
RUN dotnet restore "./MeterService/MeterService/MeterService.csproj"
RUN dotnet restore "./MeterService/MeterServiceTests/MeterServiceTests.csproj"

COPY . .
WORKDIR "/src/MeterService/MeterService"
RUN dotnet build "./MeterService.csproj" -c $BUILD_CONFIGURATION -o /app/build

WORKDIR "/src/MeterService/MeterServiceTests"
RUN dotnet build "./MeterServiceTests.csproj" -c $BUILD_CONFIGURATION
RUN dotnet test "./MeterServiceTests.csproj" -c $BUILD_CONFIGURATION --no-build --logger:"trx;LogFileName=TestResults.trx"

FROM build AS publish
WORKDIR "/src/MeterService/MeterService"
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MeterService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MeterService.dll"]