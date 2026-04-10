# TechSalary - Start All Services Script
# This script starts all services in the correct order

$ErrorActionPreference = "Stop"

Write-Host @"
╔════════════════════════════════════════════════════════╗
║                                                        ║
║     TechSalary API Platform - Service Launcher        ║
║                                                        ║
╚════════════════════════════════════════════════════════╝
"@ -ForegroundColor Cyan

$workspace = "C:\Users\MahelaSulakk_j86u0bi\Desktop\cloud-computing-coursework\api\TechSalary"

# Function to check if port is in use
function Test-Port {
    param($Port)
    $connection = Get-NetTCPConnection -LocalPort $Port -ErrorAction SilentlyContinue
    return $null -ne $connection
}

# Function to wait for service to start
function Wait-ForService {
    param($Port, $ServiceName, $MaxWaitSeconds = 30)
    
    Write-Host "⏳ Waiting for $ServiceName to start on port $Port..." -ForegroundColor Yellow
    $waited = 0
    
    while (-not (Test-Port $Port) -and $waited -lt $MaxWaitSeconds) {
        Start-Sleep -Seconds 1
        $waited++
        Write-Host "." -NoNewline -ForegroundColor Gray
    }
    
    Write-Host ""
    
    if (Test-Port $Port) {
        Write-Host "✅ $ServiceName is running on port $Port" -ForegroundColor Green
        return $true
    } else {
        Write-Host "❌ $ServiceName failed to start on port $Port" -ForegroundColor Red
        return $false
    }
}

# Check if services are already running
Write-Host "`n[STEP 1] Checking for running services..." -ForegroundColor Cyan

if (Test-Port 5178) {
    Write-Host "⚠️  Port 5178 (Identity API) is already in use" -ForegroundColor Yellow
    $kill = Read-Host "Kill existing process? (Y/N)"
    if ($kill -eq "Y" -or $kill -eq "y") {
        $proc = Get-NetTCPConnection -LocalPort 5178 | Select-Object -ExpandProperty OwningProcess
        Stop-Process -Id $proc -Force
        Write-Host "✅ Killed process on port 5178" -ForegroundColor Green
        Start-Sleep -Seconds 2
    }
}

if (Test-Port 5002) {
    Write-Host "⚠️  Port 5002 (Salary API) is already in use" -ForegroundColor Yellow
    $kill = Read-Host "Kill existing process? (Y/N)"
    if ($kill -eq "Y" -or $kill -eq "y") {
        $proc = Get-NetTCPConnection -LocalPort 5002 | Select-Object -ExpandProperty OwningProcess
        Stop-Process -Id $proc -Force
        Write-Host "✅ Killed process on port 5002" -ForegroundColor Green
        Start-Sleep -Seconds 2
    }
}

if (Test-Port 5000) {
    Write-Host "⚠️  Port 5000 (API Gateway) is already in use" -ForegroundColor Yellow
    $kill = Read-Host "Kill existing process? (Y/N)"
    if ($kill -eq "Y" -or $kill -eq "y") {
        $proc = Get-NetTCPConnection -LocalPort 5000 | Select-Object -ExpandProperty OwningProcess
        Stop-Process -Id $proc -Force
        Write-Host "✅ Killed process on port 5000" -ForegroundColor Green
        Start-Sleep -Seconds 2
    }
}

# Start Identity API
Write-Host "`n[STEP 2] Starting Identity API (Port 5178)..." -ForegroundColor Cyan
$identityPath = Join-Path $workspace "IdentityApi\TechSalary.API"

if (Test-Path $identityPath) {
    Start-Process powershell -ArgumentList @(
        "-NoExit",
        "-Command",
        "Set-Location '$identityPath'; Write-Host 'Identity API Terminal' -ForegroundColor Cyan; dotnet run"
    )
    
    if (Wait-ForService -Port 5178 -ServiceName "Identity API" -MaxWaitSeconds 30) {
        # Test Swagger JSON
        Start-Sleep -Seconds 2
        try {
            $response = Invoke-WebRequest -Uri "http://localhost:5178/swagger/v1/swagger.json" -UseBasicParsing -TimeoutSec 5
            if ($response.StatusCode -eq 200) {
                Write-Host "✅ Identity API Swagger JSON is accessible" -ForegroundColor Green
            }
        } catch {
            Write-Host "⚠️  Identity API Swagger JSON not yet ready (this is OK)" -ForegroundColor Yellow
        }
    } else {
        Write-Host "❌ Failed to start Identity API. Check the terminal window for errors." -ForegroundColor Red
        Read-Host "Press Enter to exit"
        exit 1
    }
} else {
    Write-Host "❌ Identity API path not found: $identityPath" -ForegroundColor Red
    Read-Host "Press Enter to exit"
    exit 1
}

# Start Salary Submission API
Write-Host "`n[STEP 3] Starting Salary Submission API (Port 5002)..." -ForegroundColor Cyan
$salaryPath = Join-Path $workspace "SalarySubmissionApi"

if (Test-Path $salaryPath) {
    Start-Process powershell -ArgumentList @(
        "-NoExit",
        "-Command",
        "Set-Location '$salaryPath'; Write-Host 'Salary Submission API Terminal' -ForegroundColor Cyan; dotnet run"
    )
    
    if (Wait-ForService -Port 5002 -ServiceName "Salary Submission API" -MaxWaitSeconds 30) {
        # Test Swagger JSON
        Start-Sleep -Seconds 2
        try {
            $response = Invoke-WebRequest -Uri "http://localhost:5002/swagger/v1/swagger.json" -UseBasicParsing -TimeoutSec 5
            if ($response.StatusCode -eq 200) {
                Write-Host "✅ Salary API Swagger JSON is accessible" -ForegroundColor Green
            }
        } catch {
            Write-Host "⚠️  Salary API Swagger JSON not yet ready (this is OK)" -ForegroundColor Yellow
        }
    } else {
        Write-Host "❌ Failed to start Salary Submission API. Check the terminal window for errors." -ForegroundColor Red
        Read-Host "Press Enter to exit"
        exit 1
    }
} else {
    Write-Host "❌ Salary Submission API path not found: $salaryPath" -ForegroundColor Red
    Read-Host "Press Enter to exit"
    exit 1
}

# Wait a bit before starting Gateway
Write-Host "`n[STEP 4] Waiting 5 seconds before starting API Gateway..." -ForegroundColor Cyan
Start-Sleep -Seconds 5

# Start API Gateway
Write-Host "`n[STEP 5] Starting API Gateway (Port 5000)..." -ForegroundColor Cyan
$gatewayPath = Join-Path $workspace "ApiGateway"

if (Test-Path $gatewayPath) {
    Start-Process powershell -ArgumentList @(
        "-NoExit",
        "-Command",
        "Set-Location '$gatewayPath'; Write-Host 'API Gateway Terminal' -ForegroundColor Cyan; dotnet run"
    )
    
    if (Wait-ForService -Port 5000 -ServiceName "API Gateway" -MaxWaitSeconds 30) {
        Write-Host "✅ API Gateway is running" -ForegroundColor Green
    } else {
        Write-Host "❌ Failed to start API Gateway. Check the terminal window for errors." -ForegroundColor Red
        Read-Host "Press Enter to exit"
        exit 1
    }
} else {
    Write-Host "❌ API Gateway path not found: $gatewayPath" -ForegroundColor Red
    Read-Host "Press Enter to exit"
    exit 1
}

# Wait for Gateway to fully initialize
Write-Host "`n[STEP 6] Waiting for API Gateway to initialize..." -ForegroundColor Cyan
Start-Sleep -Seconds 5

# Test Gateway Swagger endpoints
Write-Host "`n[STEP 7] Testing Gateway Swagger aggregation..." -ForegroundColor Cyan

$testsPassed = 0
$testsTotal = 2

try {
    $response = Invoke-WebRequest -Uri "http://localhost:5000/swagger/docs/identity" -UseBasicParsing -TimeoutSec 5
    if ($response.StatusCode -eq 200) {
        Write-Host "✅ Gateway -> Identity API endpoint working" -ForegroundColor Green
        $testsPassed++
    }
} catch {
    Write-Host "❌ Gateway -> Identity API endpoint failed: $($_.Exception.Message)" -ForegroundColor Red
}

try {
    $response = Invoke-WebRequest -Uri "http://localhost:5000/swagger/docs/salary" -UseBasicParsing -TimeoutSec 5
    if ($response.StatusCode -eq 200) {
        Write-Host "✅ Gateway -> Salary API endpoint working" -ForegroundColor Green
        $testsPassed++
    }
} catch {
    Write-Host "❌ Gateway -> Salary API endpoint failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Final summary
Write-Host @"

╔════════════════════════════════════════════════════════╗
║                                                        ║
║                  🎉 STARTUP COMPLETE                   ║
║                                                        ║
╚════════════════════════════════════════════════════════╝
"@ -ForegroundColor Green

Write-Host "`n📊 SERVICE STATUS:" -ForegroundColor Cyan
Write-Host "  ✅ Identity API         http://localhost:5178" -ForegroundColor Green
Write-Host "  ✅ Salary Submission API http://localhost:5002" -ForegroundColor Green
Write-Host "  ✅ API Gateway           http://localhost:5000" -ForegroundColor Green

Write-Host "`n📝 SWAGGER AGGREGATION TEST:" -ForegroundColor Cyan
Write-Host "  Tests Passed: $testsPassed / $testsTotal" -ForegroundColor $(if ($testsPassed -eq $testsTotal) { "Green" } else { "Yellow" })

if ($testsPassed -eq $testsTotal) {
    Write-Host "`n✅ ALL SYSTEMS OPERATIONAL!" -ForegroundColor Green
    Write-Host "`n🌐 Opening Swagger UI in your browser..." -ForegroundColor Cyan
    Write-Host "   URL: http://localhost:5000/swagger" -ForegroundColor White
    Write-Host "`n💡 TIP: If you don't see both APIs, use Incognito mode (Ctrl+Shift+N)" -ForegroundColor Yellow
    
    Start-Sleep -Seconds 3
    Start-Process "http://localhost:5000/swagger"
    
    Write-Host "`n✅ You should see BOTH APIs in the dropdown:" -ForegroundColor Cyan
    Write-Host "   - Identity API v1" -ForegroundColor White
    Write-Host "   - Salary Submission API v1" -ForegroundColor White
} else {
    Write-Host "`n⚠️  Some tests failed. Check the terminal windows for errors." -ForegroundColor Yellow
    Write-Host "   You can still try opening: http://localhost:5000/swagger" -ForegroundColor White
}

Write-Host "`n📋 TERMINAL WINDOWS:" -ForegroundColor Cyan
Write-Host "   - 3 PowerShell windows opened (Identity API, Salary API, Gateway)" -ForegroundColor White
Write-Host "   - Keep them running to use the services" -ForegroundColor White
Write-Host "   - Press Ctrl+C in each window to stop a service" -ForegroundColor White

Write-Host "`n🔧 TROUBLESHOOTING:" -ForegroundColor Cyan
Write-Host "   If Swagger doesn't show both APIs:" -ForegroundColor Yellow
Write-Host "   1. Clear browser cache (Ctrl+Shift+Delete)" -ForegroundColor White
Write-Host "   2. Use Incognito mode (Ctrl+Shift+N)" -ForegroundColor White
Write-Host "   3. Run: .\diagnose-swagger-issue.ps1" -ForegroundColor White

Write-Host "`n✨ Happy Testing! ✨`n" -ForegroundColor Cyan

Read-Host "Press Enter to close this window (services will keep running)"
