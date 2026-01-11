# WORKING SOLUTION - Updated Instructions
Write-Host "WORKING SOLUTION FOR IoTRAG" -ForegroundColor Green
Write-Host "=============================" -ForegroundColor White

Write-Host "COSMOS DB SOLUTION:" -ForegroundColor Yellow
Write-Host "✅ Windows emulator is running on port 8081" -ForegroundColor Green
Write-Host "✅ Open: https://localhost:8081/_explorer/index.html" -ForegroundColor Cyan
Write-Host "✅ Create database 'iotdb' and containers manually" -ForegroundColor White
Write-Host "" -ForegroundColor White

Write-Host "EVENT HUBS SOLUTION:" -ForegroundColor Yellow
Write-Host "❌ Event Hubs emulator has issues (common with Docker)" -ForegroundColor Red
Write-Host "🔄 SKIP telemetry generator for now" -ForegroundColor Yellow
Write-Host "✅ Focus on testing ChatWithRag endpoint" -ForegroundColor Green
Write-Host "" -ForegroundColor White

Write-Host "WORKING TEST SEQUENCE:" -ForegroundColor Cyan
Write-Host "1. Create Cosmos DB containers manually (step above)" -ForegroundColor White
Write-Host "2. Restart Function App:" -ForegroundColor Gray
Write-Host "   cd FunctionApp && func start" -ForegroundColor Gray
Write-Host "3. Test Chat API:" -ForegroundColor Gray
Write-Host "   curl -X POST http://localhost:7071/api/chat" -ForegroundColor Gray
Write-Host "        -H 'Content-Type: application/json'" -ForegroundColor Gray
Write-Host "        -d '{\"deviceId\":\"pump-001\",\"question\":\"Test question\"}'" -ForegroundColor Gray
Write-Host "" -ForegroundColor White

Write-Host "Once ChatWithRag works, then we can fix Event Hubs separately." -ForegroundColor Yellow

# Open Cosmos DB Explorer
Start-Process "https://localhost:8081/_explorer/index.html"