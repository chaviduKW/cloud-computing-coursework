FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# -------- Build image --------
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

# Copy only the project file to cache layers
COPY ["StatsApi.csproj", "./"]
RUN dotnet restore "StatsApi.csproj"

# Copy everything else and publish
COPY . .

RUN dotnet build "StatsApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StatsApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "StatsApi.dll"]