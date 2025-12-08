FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/UsersService/UsersService.csproj", "src/Services/UsersService/"]
COPY ["src/Shared/Auth/Auth.csproj", "src/Shared/Auth/"]
COPY ["src/Shared/Messaging/Messaging.csproj", "src/Shared/Messaging/"]
COPY ["src/Shared/ServiceDefaults/ServiceDefaults.csproj", "src/Shared/ServiceDefaults/"]
COPY ["src/Shared/Utilities/Utilities.csproj", "src/Shared/Utilities/"]
RUN dotnet restore "src/Services/UsersService/UsersService.csproj"
COPY . .
WORKDIR "/src/src/Services/UsersService"
RUN dotnet build "./UsersService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./UsersService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UsersService.dll"]
