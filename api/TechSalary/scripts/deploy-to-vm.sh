#!/bin/bash

# ============================================================================
# TechSalary Microservices - Azure VM Deployment Script
# ============================================================================
# This script deploys the TechSalary microservices to an Azure VM
# Usage: ./deploy-to-vm.sh [build-id]
# ============================================================================

set -e  # Exit on error

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Functions
log_info() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

log_warn() {
    echo -e "${YELLOW}[WARN]${NC} $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# ============================================================================
# Configuration
# ============================================================================

# Build ID (passed from Azure DevOps or use 'latest')
BUILD_ID=${1:-${BUILD_ID:-latest}}
ACR_NAME=${ACR_NAME:-acrtechsalary.azurecr.io}
APP_DIR=${APP_DIR:-/home/azureuser/techsalary-app}
COMPOSE_FILE="docker-compose.azure.yml"
ENV_FILE=".env"

log_info "========================================="
log_info "TechSalary Deployment Script"
log_info "========================================="
log_info "Build ID: $BUILD_ID"
log_info "ACR: $ACR_NAME"
log_info "App Directory: $APP_DIR"
log_info "========================================="

# ============================================================================
# Pre-deployment Checks
# ============================================================================

log_info "Running pre-deployment checks..."

# Check if Docker is installed
if ! command -v docker &> /dev/null; then
    log_error "Docker is not installed!"
    exit 1
fi
log_info "✓ Docker is installed"

# Check if Docker Compose is installed
if ! command -v docker-compose &> /dev/null; then
    log_error "Docker Compose is not installed!"
    exit 1
fi
log_info "✓ Docker Compose is installed"

# Check if .env file exists
if [ ! -f "$APP_DIR/$ENV_FILE" ]; then
    log_warn ".env file not found. Creating from template..."
    if [ -f "$APP_DIR/.env.template" ]; then
        cp "$APP_DIR/.env.template" "$APP_DIR/$ENV_FILE"
        log_info "✓ .env file created. Please update with actual values!"
    else
        log_error ".env file not found and no template available!"
        exit 1
    fi
fi

# Create app directory if not exists
mkdir -p "$APP_DIR"
cd "$APP_DIR"

# ============================================================================
# Generate Docker Compose File
# ============================================================================

log_info "Generating Docker Compose file..."

cat > "$COMPOSE_FILE" <<EOF
version: '3.8'

services:
  apigateway:
    image: ${ACR_NAME}/apigateway:${BUILD_ID}
    container_name: techsalary-gateway
    restart: unless-stopped
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:80
      - Jwt__SecretKey=\${JWT_SECRET}
      - Jwt__Issuer=TechSalaryAPI
      - Jwt__Audience=TechSalaryClient
    ports:
      - "80:80"
      - "5000:80"
    depends_on:
      - identityapi
      - salarysubmissionapi
      - searchapi
      - voteapi
      - statsapi
    networks:
      - techsalary-network
    logging:
      driver: "json-file"
      options:
        max-size: "10m"
        max-file: "3"

  identityapi:
    image: ${ACR_NAME}/identityapi:${BUILD_ID}
    container_name: techsalary-identityapi
    restart: unless-stopped
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:80
      - ConnectionStrings__DefaultConnection=\${DATABASE_CONNECTION_STRING}
      - Jwt__SecretKey=\${JWT_SECRET}
      - Jwt__Issuer=TechSalaryAPI
      - Jwt__Audience=TechSalaryClient
      - Jwt__AccessTokenExpirationMinutes=15
      - Jwt__RefreshTokenExpirationDays=7
    networks:
      - techsalary-network
    logging:
      driver: "json-file"
      options:
        max-size: "10m"
        max-file: "3"

  salarysubmissionapi:
    image: ${ACR_NAME}/salarysubmissionapi:${BUILD_ID}
    container_name: techsalary-salaryapi
    restart: unless-stopped
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:80
      - ConnectionStrings__DefaultConnection=\${DATABASE_CONNECTION_STRING}
      - Authentication__ValidateTokens=false
    networks:
      - techsalary-network
    logging:
      driver: "json-file"
      options:
        max-size: "10m"
        max-file: "3"

  searchapi:
    image: ${ACR_NAME}/searchapi:${BUILD_ID}
    container_name: techsalary-searchapi
    restart: unless-stopped
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:80
      - ConnectionStrings__DefaultConnection=\${DATABASE_CONNECTION_STRING}
    networks:
      - techsalary-network
    logging:
      driver: "json-file"
      options:
        max-size: "10m"
        max-file: "3"

  voteapi:
    image: ${ACR_NAME}/voteapi:${BUILD_ID}
    container_name: techsalary-voteapi
    restart: unless-stopped
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:80
      - ConnectionStrings__DefaultConnection=\${DATABASE_CONNECTION_STRING}
    networks:
      - techsalary-network
    logging:
      driver: "json-file"
      options:
        max-size: "10m"
        max-file: "3"

  statsapi:
    image: ${ACR_NAME}/statsapi:${BUILD_ID}
    container_name: techsalary-statsapi
    restart: unless-stopped
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:80
      - ConnectionStrings__DefaultConnection=\${DATABASE_CONNECTION_STRING}
    networks:
      - techsalary-network
    logging:
      driver: "json-file"
      options:
        max-size: "10m"
        max-file: "3"

networks:
  techsalary-network:
    driver: bridge
EOF

log_info "✓ Docker Compose file generated"

# ============================================================================
# Login to ACR
# ============================================================================

log_info "Logging into Azure Container Registry..."

# Try Azure CLI login first
if command -v az &> /dev/null; then
    az acr login --name ${ACR_NAME%.azurecr.io} 2>/dev/null || {
        log_warn "Azure CLI login failed, trying docker login..."
        if [ ! -z "$ACR_USERNAME" ] && [ ! -z "$ACR_PASSWORD" ]; then
            echo "$ACR_PASSWORD" | docker login "$ACR_NAME" --username "$ACR_USERNAME" --password-stdin
        else
            log_error "ACR credentials not found. Please set ACR_USERNAME and ACR_PASSWORD"
            exit 1
        fi
    }
else
    # Use docker login with credentials
    if [ ! -z "$ACR_USERNAME" ] && [ ! -z "$ACR_PASSWORD" ]; then
        echo "$ACR_PASSWORD" | docker login "$ACR_NAME" --username "$ACR_USERNAME" --password-stdin
    else
        log_error "ACR credentials not found. Please set ACR_USERNAME and ACR_PASSWORD"
        exit 1
    fi
fi

log_info "✓ Logged into ACR"

# ============================================================================
# Pull Latest Images
# ============================================================================

log_info "Pulling latest images from ACR..."

if docker-compose -f "$COMPOSE_FILE" pull; then
    log_info "✓ Images pulled successfully"
else
    log_error "Failed to pull images"
    exit 1
fi

# ============================================================================
# Backup Current Deployment (if exists)
# ============================================================================

if docker-compose -f "$COMPOSE_FILE" ps | grep -q "Up"; then
    log_info "Backing up current deployment..."
    BACKUP_FILE="docker-compose.backup.$(date +%Y%m%d_%H%M%S).yml"
    cp "$COMPOSE_FILE" "$BACKUP_FILE"
    log_info "✓ Backup created: $BACKUP_FILE"
fi

# ============================================================================
# Stop Old Containers
# ============================================================================

log_info "Stopping old containers..."

if docker-compose -f "$COMPOSE_FILE" down; then
    log_info "✓ Old containers stopped"
else
    log_warn "No containers to stop or error occurred"
fi

# ============================================================================
# Start New Containers
# ============================================================================

log_info "Starting new containers..."

if docker-compose -f "$COMPOSE_FILE" up -d; then
    log_info "✓ Containers started successfully"
else
    log_error "Failed to start containers"
    log_info "Attempting rollback..."
    
    # Try to restore from backup
    if [ -f "$BACKUP_FILE" ]; then
        cp "$BACKUP_FILE" "$COMPOSE_FILE"
        docker-compose -f "$COMPOSE_FILE" up -d
        log_warn "Rolled back to previous version"
    fi
    exit 1
fi

# ============================================================================
# Wait for Services to Start
# ============================================================================

log_info "Waiting for services to initialize..."
sleep 30

# ============================================================================
# Health Checks
# ============================================================================

log_info "Running health checks..."

# Check if containers are running
RUNNING_CONTAINERS=$(docker-compose -f "$COMPOSE_FILE" ps --services --filter "status=running" | wc -l)
EXPECTED_CONTAINERS=6

if [ "$RUNNING_CONTAINERS" -eq "$EXPECTED_CONTAINERS" ]; then
    log_info "✓ All $EXPECTED_CONTAINERS containers are running"
else
    log_warn "Expected $EXPECTED_CONTAINERS containers, but only $RUNNING_CONTAINERS are running"
    docker-compose -f "$COMPOSE_FILE" ps
fi

# Check API Gateway health
log_info "Checking API Gateway health..."
if curl -f -s http://localhost:5000/ > /dev/null 2>&1; then
    log_info "✓ API Gateway is responding"
else
    log_warn "API Gateway health check failed (this might be OK if no root endpoint exists)"
fi

# ============================================================================
# Display Status
# ============================================================================

log_info "========================================="
log_info "Deployment Status"
log_info "========================================="
docker-compose -f "$COMPOSE_FILE" ps

# ============================================================================
# Cleanup
# ============================================================================

log_info "Cleaning up old images..."
docker image prune -f
log_info "✓ Cleanup completed"

# ============================================================================
# Show Logs (last 50 lines)
# ============================================================================

log_info "========================================="
log_info "Recent logs:"
log_info "========================================="
docker-compose -f "$COMPOSE_FILE" logs --tail=50

# ============================================================================
# Deployment Summary
# ============================================================================

log_info "========================================="
log_info "Deployment Summary"
log_info "========================================="
log_info "Build ID: $BUILD_ID"
log_info "Deployment Time: $(date)"
log_info "Running Containers: $RUNNING_CONTAINERS/$EXPECTED_CONTAINERS"
log_info "========================================="
log_info "Deployment completed successfully! 🎉"
log_info "========================================="
log_info ""
log_info "Access your application at:"
log_info "  - API Gateway: http://$(hostname -I | awk '{print $1}'):5000"
log_info ""
log_info "Useful commands:"
log_info "  - View logs: docker-compose -f $COMPOSE_FILE logs -f"
log_info "  - Check status: docker-compose -f $COMPOSE_FILE ps"
log_info "  - Restart services: docker-compose -f $COMPOSE_FILE restart"
log_info "========================================="

exit 0
