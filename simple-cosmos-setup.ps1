# Simple Cosmos DB Setup
Write-Host "Setting up Cosmos DB..." -ForegroundColor Green

# Option 1: Try Windows emulator
$cosmosPath = "${env:ProgramFiles}\Azure Cosmos DB Emulator\Microsoft.Azure.Cosmos.Emulator.exe"
if (Test-Path $cosmosPath) {
    Write-Host "Found Windows Cosmos DB Emulator - starting it..." -ForegroundColor Green
    Start-Process $cosmosPath -ArgumentList "/NoExplorer"
    Start-Sleep -Seconds 20
    Write-Host "Open: https://localhost:8081/_explorer/index.html" -ForegroundColor Cyan
    Start-Process "https://localhost:8081/_explorer/index.html"
} else {
    Write-Host "Windows emulator not found, using Docker version..." -ForegroundColor Yellow
    Write-Host "Open: https://localhost:8082/_explorer/index.html" -ForegroundColor Cyan
    Start-Process "https://localhost:8082/_explorer/index.html"
}

Write-Host "Create database: iotdb" -ForegroundColor White
Write-Host "Create containers:" -ForegroundColor Gray
Write-Host "  - documents (partition: /deviceType)" -ForegroundColor Gray
Write-Host "  - telemetry (partition: /deviceId)" -ForegroundColor Gray
Write-Host "  - devices (partition: /deviceId)" -ForegroundColor Gray
Write-Host "  - alerts (partition: /deviceId)" -ForegroundColor Gray