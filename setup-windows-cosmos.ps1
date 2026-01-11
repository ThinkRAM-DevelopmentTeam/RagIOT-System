# Windows Cosmos DB Emulator Setup
# More reliable than Docker Linux version

Write-Host "🔧 Setting up Windows Cosmos DB Emulator..." -ForegroundColor Green

# Check if Windows Cosmos DB Emulator is available
$cosmosPath = "${env:ProgramFiles}\Azure Cosmos DB Emulator\Microsoft.Azure.Cosmos.Emulator.exe"
if (Test-Path $cosmosPath) {
    Write-Host "✅ Found Windows Cosmos DB Emulator" -ForegroundColor Green
    
    # Stop Docker version first
    Write-Host "🛑 Stopping Docker Cosmos DB emulator..." -ForegroundColor Yellow
    docker-compose stop cosmos-emulator
    
    # Start Windows emulator
    Write-Host "🚀 Starting Windows Cosmos DB Emulator..." -ForegroundColor Yellow
    Start-Process $cosmosPath -ArgumentList "/NoExplorer", "/DisableRateLimiting", "/EnableCassandraServer", "/EnableGremlinEndpoint"
    
    # Wait for startup
    Write-Host "⏳ Waiting for Windows emulator to start..." -ForegroundColor Yellow
    Start-Sleep -Seconds 30
    
    # Test connectivity
    try {
        $response = Invoke-WebRequest -Uri "https://localhost:8081/_explorer/index.html" -UseBasicParsing -TimeoutSec 10
        if ($response.StatusCode -eq 200) {
            Write-Host "✅ Windows Cosmos DB Emulator is ready!" -ForegroundColor Green
            Write-Host "🌐 Open Explorer: https://localhost:8081/_explorer/index.html" -ForegroundColor Cyan
            
            # Open browser
            Start-Process "https://localhost:8081/_explorer/index.html"
            
            Write-Host "`n📋 Create these containers:" -ForegroundColor White
            Write-Host "   Database: iotdb" -ForegroundColor Gray
            Write-Host "   - documents (partition: /deviceType)" -ForegroundColor Gray
            Write-Host "   - telemetry (partition: /deviceId)" -ForegroundColor Gray
            Write-Host "   - devices (partition: /deviceId)" -ForegroundColor Gray
            Write-Host "   - alerts (partition: /deviceId)" -ForegroundColor Gray
        }
    }
    catch {
        Write-Host "❌ Windows emulator not ready yet: $($_.Exception.Message)" -ForegroundColor Red
    }
} else {
    Write-Host "❌ Windows Cosmos DB Emulator not found" -ForegroundColor Red
    Write-Host "📥 Download from: https://aka.ms/cosmosdb-emulator" -ForegroundColor Yellow
    Write-Host "🔄 Or continue with Docker version (may need more retries)" -ForegroundColor Yellow
}