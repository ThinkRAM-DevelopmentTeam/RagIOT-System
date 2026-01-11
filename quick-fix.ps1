# Quick Fix Scripts for Both Issues
Write-Host "FIXING BOTH ISSUES" -ForegroundColor Green
Write-Host "==================" -ForegroundColor White

Write-Host "1. Fixing Cosmos DB Authorization..." -ForegroundColor Yellow
# Test if we can access Cosmos DB
try {
    $response = Invoke-WebRequest -Uri "https://localhost:8081/_explorer/index.html" -UseBasicParsing -TimeoutSec 5
    if ($response.StatusCode -eq 200) {
        Write-Host "Cosmos DB is accessible. Connection string may need adjustment." -ForegroundColor Yellow
    }
} catch {
    Write-Host "Cosmos DB not accessible. Check Windows emulator." -ForegroundColor Red
}

Write-Host "2. Starting Simple Event Hubs..." -ForegroundColor Yellow
# Start Event Hubs without dependencies
docker run -d --name eventhubs-simple -p 5671:5671 --rm mcr.microsoft.com/azure-messaging/eventhubs-emulator:latest

Write-Host "3. Testing connections..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

# Test Event Hubs
try {
    $eventhubs = Test-NetConnection -ComputerName localhost -Port 5671
    if ($eventhubs.TcpTestSucceeded) {
        Write-Host "Event Hubs: WORKING" -ForegroundColor Green
    } else {
        Write-Host "Event Hubs: STILL FAILED" -ForegroundColor Red
    }
} catch {
    Write-Host "Event Hubs: FAILED" -ForegroundColor Red
}

Write-Host "4. Instructions:" -ForegroundColor Cyan
Write-Host "   Retry Function App with updated connection string" -ForegroundColor Gray
Write-Host "   Retry Telemetry Generator" -ForegroundColor Gray
Write-Host "   If still failing, use Mock Event Hubs setup" -ForegroundColor Gray