FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TruthNews.Api/TruthNews.Api.csproj", "TruthNews.Api/"]
COPY ["TruthNews.Infrastructure/TruthNews.Infrastructure.csproj", "TruthNews.Infrastructure/"]
RUN dotnet restore "TruthNews.Api/TruthNews.Api.csproj"
COPY . .
WORKDIR "/src/TruthNews.Api"
RUN dotnet build "./TruthNews.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TruthNews.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TruthNews.Api.dll"]
