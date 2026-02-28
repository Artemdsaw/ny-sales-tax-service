
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Этот этап используется для сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SurveySystem.Api/SurveySystem.csproj", "SurveySystem.Api/"]
COPY ["SurveySystem.Application/SurveySystem.Application.csproj", "SurveySystem.Application/"]
COPY ["SurveySystem.Domain/SurveySystem.Domain.csproj", "SurveySystem.Domain/"]
COPY ["SurveySystem.Infrastructure/SurveySystem.Infrastructure.csproj", "SurveySystem.Infrastructure/"]
RUN dotnet restore "./SurveySystem.Api/SurveySystem.csproj"
COPY . .
WORKDIR "/src/SurveySystem.Api"
RUN dotnet build "./SurveySystem.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SurveySystem.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SurveySystem.dll"]