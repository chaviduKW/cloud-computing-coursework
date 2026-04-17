# TechSalary Identity API - Docker Setup

## Overview
This project is configured to run using Docker and Docker Compose. The setup includes the Identity API and a PostgreSQL database.

## Prerequisites
- Docker Desktop installed and running
- .NET 8 SDK (for local development)

## Project Structure
- `TechSalary.API/` - Identity API source code
- `TechSalary.API/Dockerfile` - Docker image definition for the API
- `TechSalary.API/.dockerignore` - Files to exclude from Docker build
- `docker-compose.yml` - Orchestration file for all services (located in parent directory)

## Database Configuration
The PostgreSQL database is configured with:
- **Database Name**: TechSalaryDB
- **Username**: keycloak
- **Password**: keycloak
- **Port**: 25432 (on host machine)
- **Internal Port**: 5432 (inside Docker network)

## Running with Docker Compose

### 1. Start all services
From the parent directory (where docker-compose.yml is located):
```bash
docker-compose up -d
```

This will:
- Build the Identity API Docker image
- Start the PostgreSQL database container
- Start the Identity API container
- Run database migrations automatically

### 2. View logs
```bash
# View all logs
docker-compose logs -f

# View specific service logs
docker-compose logs -f identityapi
docker-compose logs -f postgres
```

### 3. Stop services
```bash
docker-compose down
```

### 4. Stop and remove volumes (clean database)
```bash
docker-compose down -v
```

## Running Only the Identity API

If you already have a PostgreSQL database running (on port 25432), you can run just the API:

```bash
# Build the Docker image
cd IdentityApi
docker build -t techsalary-identityapi -f TechSalary.API/Dockerfile .

# Run the container
docker run -d \
  -p 5000:8080 \
  --name techsalary-identityapi \
  -e "ConnectionStrings__DefaultConnection=Host=host.docker.internal;Port=25432;Database=TechSalaryDB;Username=keycloak;Password=keycloak" \
  --add-host=host.docker.internal:host-gateway \
  techsalary-identityapi
```

## Accessing the API

Once running:
- **API**: http://localhost:5000
- **Swagger UI**: http://localhost:5000/swagger
- **Database**: localhost:25432

## Troubleshooting

### Database Connection Issues
If the API can't connect to the database:
1. Ensure PostgreSQL is running: `docker-compose ps`
2. Check database logs: `docker-compose logs postgres`
3. Verify connection string in environment variables

### Port Conflicts
If port 25432 or 5000 is already in use:
- Edit `docker-compose.yml` and change the port mappings
- For example: `"25433:5432"` or `"5001:80"`

### Rebuild After Code Changes
```bash
# Rebuild and restart the API service
docker-compose up -d --build identityapi
```

### View Container Status
```bash
docker-compose ps
```

### Access Database Directly
```bash
docker exec -it techsalary-postgres psql -U keycloak -d TechSalaryDB
```

## Development Workflow

1. **Make code changes** in your IDE
2. **Rebuild the container**:
   ```bash
   docker-compose up -d --build identityapi
   ```
3. **Test your changes** at http://localhost:5000

## Production Considerations

Before deploying to production:
1. Update `ASPNETCORE_ENVIRONMENT` to `Production` in docker-compose.yml
2. Use environment variables for sensitive data (JWT secrets, passwords)
3. Create a `.env` file for environment-specific configuration
4. Enable HTTPS with proper certificates
5. Configure proper logging and monitoring

## Environment Variables

The docker-compose.yml sets these environment variables:
- `ASPNETCORE_ENVIRONMENT`: Development/Production
- `ASPNETCORE_URLS`: URL bindings
- `ConnectionStrings__DefaultConnection`: Database connection
- `Jwt__SecretKey`: JWT signing key
- `Jwt__Issuer`: JWT issuer
- `Jwt__Audience`: JWT audience
- `Jwt__AccessTokenExpirationMinutes`: Access token lifetime
- `Jwt__RefreshTokenExpirationDays`: Refresh token lifetime
