# Alternative Cosmos DB Solutions
Write-Host "COSMOS DB TROUBLESHOOTING" -ForegroundColor Cyan
Write-Host "" -ForegroundColor White

Write-Host "SOLUTION 1: Windows Emulator (Recommended)" -ForegroundColor Green
Write-Host "  Windows emulator should be starting on port 8081" -ForegroundColor Gray
Write-Host "  Try: https://localhost:8081/_explorer/index.html" -ForegroundColor Yellow
Write-Host "" -ForegroundColor White

Write-Host "SOLUTION 2: Docker with Different Port" -ForegroundColor Green
Write-Host "  Stop current containers and restart on different ports:" -ForegroundColor Gray
docker-compose down
Write-Host "  Try accessing on port 8083 instead:" -ForegroundColor Yellow
docker run -d --name cosmos-temp -p 8083:8081 mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator
Write-Host "  Then: https://localhost:8083/_explorer/index.html" -ForegroundColor Yellow
Write-Host "" -ForegroundColor White

Write-Host "SOLUTION 3: Clear Docker State" -ForegroundColor Green
Write-Host "  Remove all containers and volumes to clear corruption:" -ForegroundColor Gray
Write-Host "  docker-compose down -v" -ForegroundColor White
Write-Host "  docker volume prune -f" -ForegroundColor White
Write-Host "  docker-compose up -d" -ForegroundColor White
Write-Host "" -ForegroundColor White

Write-Host "SOLUTION 4: Wait and Retry" -ForegroundColor Green
Write-Host "  High demand errors are temporary. Wait 2-3 minutes and retry." -ForegroundColor Gray
Write-Host "" -ForegroundColor White

Write-Host "INSTRUCTIONS FOR ANY WORKING SOLUTION:" -ForegroundColor Cyan
Write-Host "1. Open the working Explorer URL" -ForegroundColor White
Write-Host "2. Click 'New Container'" -ForegroundColor White
Write-Host "3. Database ID: iotdb" -ForegroundColor White
Write-Host "4. Create containers with these partition keys:" -ForegroundColor White
Write-Host "   - documents: /deviceType" -ForegroundColor Gray
Write-Host "   - telemetry: /deviceId" -ForegroundColor Gray
Write-Host "   - devices: /deviceId" -ForegroundColor Gray
Write-Host "   - alerts: /deviceId" -ForegroundColor Gray