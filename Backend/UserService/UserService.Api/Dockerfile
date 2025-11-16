FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["UserService/UserService.Api/UserService.Api.csproj", "UserService/UserService.Api/"]
COPY ["Aspire/ArmoryVestments.Aspire.ServiceDefaults/ArmoryVestments.Aspire.ServiceDefaults.csproj", "Aspire/ArmoryVestments.Aspire.ServiceDefaults/"]
COPY ["Shared/Shared.csproj", "Shared/"]
RUN dotnet restore "UserService/UserService.Api/UserService.Api.csproj"
COPY . .
WORKDIR "/src/UserService/UserService.Api"
RUN dotnet build "./UserService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./UserService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserService.Api.dll"]
