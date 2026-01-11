# FINAL WORKING SOLUTION
Write-Host "COSMOS DB SETUP - WORKING SOLUTION" -ForegroundColor Green
Write-Host "" -ForegroundColor White

# Check Windows emulator
try {
    $response = Invoke-WebRequest -Uri "https://localhost:8081/_explorer/index.html" -UseBasicParsing -TimeoutSec 5
    if ($response.StatusCode -eq 200) {
        Write-Host "SUCCESS: Windows Cosmos DB Emulator is working!" -ForegroundColor Green
        Write-Host "URL: https://localhost:8081/_explorer/index.html" -ForegroundColor Cyan
        
        # Open browser automatically
        Start-Process "https://localhost:8081/_explorer/index.html"
        
        Write-Host "" -ForegroundColor White
        Write-Host "MANUAL SETUP STEPS:" -ForegroundColor Yellow
        Write-Host "1. Browser opened to Cosmos DB Explorer" -ForegroundColor White
        Write-Host "2. Click 'New Container'" -ForegroundColor White
        Write-Host "3. Database ID: iotdb" -ForegroundColor White
        Write-Host "4. Click 'OK'" -ForegroundColor White
        Write-Host "5. Create 4 more containers in database 'iotdb':" -ForegroundColor White
        Write-Host "   - documents (partition key: /deviceType)" -ForegroundColor Gray
        Write-Host "   - telemetry (partition key: /deviceId)" -ForegroundColor Gray
        Write-Host "   - devices (partition key: /deviceId)" -ForegroundColor Gray
        Write-Host "   - alerts (partition key: /deviceId)" -ForegroundColor Gray
        Write-Host "" -ForegroundColor White
        Write-Host "Once containers are created, IoTRag system is ready!" -ForegroundColor Green
        
    } else {
        Write-Host "Windows emulator not ready yet. Wait and retry." -ForegroundColor Yellow
    }
}
catch {
    Write-Host "Windows emulator not responding. Try starting it manually:" -ForegroundColor Red
    Write-Host "1. Open File Explorer" -ForegroundColor White
    Write-Host "2. Go to C:\Program Files\Azure Cosmos DB Emulator\" -ForegroundColor White
    Write-Host "3. Run Microsoft.Azure.Cosmos.Emulator.exe" -ForegroundColor White
    Write-Host "4. Then open: https://localhost:8081/_explorer/index.html" -ForegroundColor Cyan
}