# Install and run Windows Cosmos DB Emulator
# Download from: https://aka.ms/cosmosdb-emulator

Write-Host "Installing Windows Cosmos DB Emulator..." -ForegroundColor Green

# Check if already installed
$cosmosPath = "${env:ProgramFiles}\Azure Cosmos DB Emulator\Microsoft.Azure.Cosmos.Emulator.exe"
if (Test-Path $cosmosPath) {
    Write-Host "✅ Cosmos DB Emulator already installed" -ForegroundColor Green
    Write-Host "Starting emulator..." -ForegroundColor Yellow
    Start-Process $cosmosPath -ArgumentList "/Start", "/NoExplorer", "/EnableCassandraServer", "/EnableGremlinEndpoint"
    Write-Host "✅ Cosmos DB Emulator started on https://localhost:8081" -ForegroundColor Green
    Write-Host "Open Explorer: https://localhost:8081/_explorer/index.html" -ForegroundColor Cyan
} else {
    Write-Host "❌ Please download and install Cosmos DB Emulator from:" -ForegroundColor Red
    Write-Host "https://aka.ms/cosmosdb-emulator" -ForegroundColor Yellow
    Write-Host "Then run this script again." -ForegroundColor Yellow
}