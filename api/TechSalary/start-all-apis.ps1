# ==============================================================================
# START ALL APIS FOR TECH SALARY PLATFORM
# ==============================================================================
# This script starts all 6 API services in separate terminal windows
# ==============================================================================

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Starting TechSalary Platform APIs" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Function to start a service in a new PowerShell window
function Start-Service {
    param(
        [string]$ServiceName,
        [string]$ProjectPath,
        [int]$Port
    )
    
    Write-Host "Starting $ServiceName on port $Port..." -ForegroundColor Yellow
    
    $command = "cd '$PSScriptRoot\$ProjectPath'; dotnet run; Read-Host 'Press Enter to close'"
    
    Start-Process powershell -ArgumentList "-NoExit", "-Command", $command
    
    Start-Sleep -Seconds 2
}

# Stop any existing dotnet processes (optional - uncomment if needed)
# Write-Host "Stopping existing dotnet processes..." -ForegroundColor Red
# Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Stop-Process -Force
# Start-Sleep -Seconds 3

Write-Host ""
Write-Host "Starting services in order..." -ForegroundColor Green
Write-Host ""

# Start all services
Start-Service -ServiceName "Identity API" -ProjectPath "IdentityApi\TechSalary.API" -Port 5178
Start-Service -ServiceName "Salary Submission API" -ProjectPath "SalarySubmissionApi" -Port 5002
Start-Service -ServiceName "Vote API" -ProjectPath "VoteApi" -Port 5215
Start-Service -ServiceName "Stats API" -ProjectPath "StatsApi" -Port 5003
Start-Service -ServiceName "Search API" -ProjectPath "SearchApi" -Port 5254
Start-Service -ServiceName "API Gateway" -ProjectPath "ApiGateway" -Port 5000

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  All Services Started Successfully!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Service Ports:" -ForegroundColor Cyan
Write-Host "  - Identity API:          http://localhost:5178" -ForegroundColor White
Write-Host "  - Salary Submission API: http://localhost:5002" -ForegroundColor White
Write-Host "  - Vote API:              http://localhost:5215" -ForegroundColor White
Write-Host "  - Stats API:             http://localhost:5003" -ForegroundColor White
Write-Host "  - Search API:            http://localhost:5254" -ForegroundColor White
Write-Host "  - API Gateway:           http://localhost:5000" -ForegroundColor White
Write-Host ""
Write-Host "Access Gateway Swagger UI:" -ForegroundColor Yellow
Write-Host "  http://localhost:5000/swagger" -ForegroundColor Green
Write-Host ""
Write-Host "All APIs are accessible through Gateway (port 5000):" -ForegroundColor Cyan
Write-Host "  - http://localhost:5000/api/auth/*" -ForegroundColor White
Write-Host "  - http://localhost:5000/api/salaries/*" -ForegroundColor White
Write-Host "  - http://localhost:5000/api/vote/*" -ForegroundColor White
Write-Host "  - http://localhost:5000/api/stats" -ForegroundColor White
Write-Host "  - http://localhost:5000/api/search/*" -ForegroundColor White
Write-Host ""
Write-Host "Press any key to continue..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
