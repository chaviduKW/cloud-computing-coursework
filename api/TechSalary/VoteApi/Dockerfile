# -------- Base runtime image --------
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# -------- Build image --------
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["VoteApi.csproj", "./"]
RUN dotnet restore "VoteApi.csproj"

# Copy everything else and build
COPY . .
RUN dotnet build "VoteApi.csproj" -c Release -o /app/build

# -------- Publish --------
FROM build AS publish
RUN dotnet publish "VoteApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# -------- Final image --------
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VoteApi.dll"]