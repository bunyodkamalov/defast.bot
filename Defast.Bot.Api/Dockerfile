FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 3002
EXPOSE 3002

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Defast.Bot.Api/Defast.Bot.Api.csproj", "Defast.Bot.Api/"]
COPY ["Defast.Bot.Infrastructure/Defast.Bot.Infrastructure.csproj", "Defast.Bot.Infrastructure/"]
COPY ["Defast.Bot.Application/Defast.Bot.Application.csproj", "Defast.Bot.Application/"]
COPY ["Defast.Bot.Domain/Defast.Bot.Domain.csproj", "Defast.Bot.Domain/"]
COPY ["Defast.Bot.Persistence/Defast.Bot.Persistence.csproj", "Defast.Bot.Persistence/"]
RUN dotnet restore "Defast.Bot.Api/Defast.Bot.Api.csproj"
COPY . .
WORKDIR "/src/Defast.Bot.Api"
RUN dotnet build "Defast.Bot.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Defast.Bot.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Defast.Bot.Api.dll"]
