# Cosmos DB Setup script (creates database and containers)
$cosmosEndpoint = "https://localhost:8082"
$cosmosKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqwm+DJgSepwBZSQdmkcJXcaXjRnxzrqlP7A=="
$databaseId = "iotdb"

Write-Host "Setting up Cosmos DB for IoTRag system..." -ForegroundColor Green

# Wait for Cosmos DB to be ready
Write-Host "Waiting for Cosmos DB emulator to be ready..."
$maxAttempts = 30
$attempt = 0

do {
    $attempt++
    try {
        $response = Invoke-WebRequest -Uri "$cosmosEndpoint/_explorer/index.html" -UseBasicParsing -TimeoutSec 5
        if ($response.StatusCode -eq 200) {
            Write-Host "✅ Cosmos DB is ready!" -ForegroundColor Green
            break
        }
    }
    catch {
        Write-Host "Attempt ${attempt}/${maxAttempts}: Cosmos DB not ready yet..."
        Start-Sleep -Seconds 2
    }
} while ($attempt -lt $maxAttempts)

if ($attempt -eq $maxAttempts) {
    Write-Host "❌ Cosmos DB emulator failed to start. Please check Docker containers." -ForegroundColor Red
    Write-Host "Try opening the Explorer manually: $cosmosEndpoint/_explorer/index.html" -ForegroundColor Yellow
    exit 1
}

Write-Host "`n🎯 MANUAL SETUP INSTRUCTIONS:" -ForegroundColor Cyan
Write-Host "1. Open browser and go to: $cosmosEndpoint/_explorer/index.html" -ForegroundColor White
Write-Host "2. Click 'New Container' to create database 'iotdb'" -ForegroundColor White
Write-Host "3. Create these containers:" -ForegroundColor White
Write-Host "   - documents (partition key: /deviceType)" -ForegroundColor Gray
Write-Host "   - telemetry (partition key: /deviceId)" -ForegroundColor Gray
Write-Host "   - devices (partition key: /deviceId)" -ForegroundColor Gray
Write-Host "   - alerts (partition key: /deviceId)" -ForegroundColor Gray
Write-Host "`n💡 The web interface is more reliable than automated scripts for the emulator." -ForegroundColor Yellow

# Open the browser automatically
Start-Process "$cosmosEndpoint/_explorer/index.html"
