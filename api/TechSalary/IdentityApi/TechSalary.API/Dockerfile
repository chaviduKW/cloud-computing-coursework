FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TechSalary.API/TechSalaryIdentity.API.csproj", "TechSalary.API/"]
COPY ["TechSalary.Core/TechSalaryIdentity.Core.csproj", "TechSalary.Core/"]
COPY ["TechSalary.Infrastructure/TechSalaryIdentity.Infrastructure.csproj", "TechSalary.Infrastructure/"]
RUN dotnet restore "TechSalary.API/TechSalaryIdentity.API.csproj"
COPY . .
WORKDIR "/src/TechSalary.API"
RUN dotnet build "TechSalaryIdentity.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TechSalaryIdentity.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TechSalaryIdentity.API.dll"]
