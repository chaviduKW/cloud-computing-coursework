# ==============================================================================
# TEST ALL APIS THROUGH GATEWAY (PORT 5000)
# ==============================================================================

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Testing TechSalary APIs via Gateway" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$gatewayUrl = "http://localhost:5000"

# Function to test endpoint
function Test-Endpoint {
    param(
        [string]$Name,
        [string]$Url,
        [string]$Method = "GET"
    )
    
    Write-Host "Testing: $Name" -ForegroundColor Yellow
    Write-Host "  URL: $Url" -ForegroundColor Gray
    Write-Host "  Method: $Method" -ForegroundColor Gray
    
    try {
        $response = Invoke-WebRequest -Uri $Url -Method $Method -UseBasicParsing -ErrorAction Stop
        Write-Host "  ✅ Status: $($response.StatusCode)" -ForegroundColor Green
    }
    catch {
        Write-Host "  ❌ Error: $($_.Exception.Message)" -ForegroundColor Red
    }
    Write-Host ""
}

Write-Host "1️⃣  Testing Identity API (http://localhost:5000/api/auth/*)" -ForegroundColor Cyan
Write-Host "Note: Most auth endpoints require POST with data" -ForegroundColor Gray
Write-Host ""

Write-Host "2️⃣  Testing Salary Submission API (http://localhost:5000/api/salaries/*)" -ForegroundColor Cyan
Test-Endpoint -Name "Get Pending Salaries" -Url "$gatewayUrl/api/salaries/pending"

Write-Host "3️⃣  Testing Vote API (http://localhost:5000/api/vote/*)" -ForegroundColor Cyan
Write-Host "Note: Vote endpoints require submission ID" -ForegroundColor Gray
Write-Host ""

Write-Host "4️⃣  Testing Stats API (http://localhost:5000/api/stats)" -ForegroundColor Cyan
Test-Endpoint -Name "Get Stats (All)" -Url "$gatewayUrl/api/stats?role=Engineer"

Write-Host "5️⃣  Testing Search API (http://localhost:5000/api/search/*)" -ForegroundColor Cyan
Test-Endpoint -Name "Search Salaries" -Url "$gatewayUrl/api/search?pageSize=5"
Test-Endpoint -Name "Get Companies" -Url "$gatewayUrl/api/search/companies"
Test-Endpoint -Name "Get Designations" -Url "$gatewayUrl/api/search/designations"

Write-Host "========================================" -ForegroundColor Green
Write-Host "  Testing Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Access Swagger UI for interactive testing:" -ForegroundColor Yellow
Write-Host "  http://localhost:5000/swagger" -ForegroundColor Green
Write-Host ""
