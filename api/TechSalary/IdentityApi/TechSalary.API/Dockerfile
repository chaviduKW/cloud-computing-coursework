FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TechSalary.API/IdentityApi.csproj", "TechSalary.API/"]
COPY ["TechSalary.Core/IdentityApi.Core.csproj", "TechSalary.Core/"]
COPY ["TechSalary.Infrastructure/IdentityApi.Infrastructure.csproj", "TechSalary.Infrastructure/"]
RUN dotnet restore "TechSalary.API/IdentityApi.csproj"
COPY . .
WORKDIR "/src/TechSalary.API"
RUN dotnet build "IdentityApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IdentityApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IdentityApi.dll"]
