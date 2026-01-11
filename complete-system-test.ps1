# Complete IoTRag System Test
Write-Host "IoTRAG COMPLETE SYSTEM TEST" -ForegroundColor Green
Write-Host "=================================" -ForegroundColor White

# Test 1: Azure Emulators
Write-Host "TEST 1: Checking Azure Emulators..." -ForegroundColor Yellow

# Azurite (Blob Storage)
try {
    $azurite = Test-NetConnection -ComputerName localhost -Port 10010
    if ($azurite.TcpTestSucceeded) {
        Write-Host "✅ Azurite (Blob Storage): RUNNING" -ForegroundColor Green
    } else {
        Write-Host "❌ Azurite (Blob Storage): FAILED" -ForegroundColor Red
    }
} catch {
    Write-Host "❌ Azurite (Blob Storage): FAILED" -ForegroundColor Red
}

# Cosmos DB
try {
    $cosmos = Invoke-WebRequest -Uri "https://localhost:8081/_explorer/index.html" -UseBasicParsing -TimeoutSec 5
    if ($cosmos.StatusCode -eq 200) {
        Write-Host "✅ Cosmos DB (Windows): RUNNING on port 8081" -ForegroundColor Green
    } else {
        Write-Host "❌ Cosmos DB: FAILED (Status $($cosmos.StatusCode))" -ForegroundColor Red
    }
} catch {
    Write-Host "❌ Cosmos DB: FAILED - $($_.Exception.Message)" -ForegroundColor Red
}

# Event Hubs
try {
    $eventhubs = Test-NetConnection -ComputerName localhost -Port 5671
    if ($eventhubs.TcpTestSucceeded) {
        Write-Host "✅ Event Hubs Emulator: RUNNING" -ForegroundColor Green
    } else {
        Write-Host "❌ Event Hubs Emulator: FAILED" -ForegroundColor Red
    }
} catch {
    Write-Host "❌ Event Hubs Emulator: FAILED" -ForegroundColor Red
}

# Service Bus
try {
    $servicebus = Test-NetConnection -ComputerName localhost -Port 5673
    if ($servicebus.TcpTestSucceeded) {
        Write-Host "✅ Service Bus Emulator: RUNNING" -ForegroundColor Green
    } else {
        Write-Host "❌ Service Bus Emulator: FAILED" -ForegroundColor Red
    }
} catch {
    Write-Host "❌ Service Bus Emulator: FAILED" -ForegroundColor Red
}

Write-Host "" -ForegroundColor White

# Test 2: Unit Tests
Write-Host "TEST 2: Running Unit Tests..." -ForegroundColor Yellow
Set-Location "FunctionApp.Tests"
$testResult = dotnet test --nologo
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Unit Tests: PASSED" -ForegroundColor Green
} else {
    Write-Host "❌ Unit Tests: FAILED" -ForegroundColor Red
}
Set-Location ".."

Write-Host "" -ForegroundColor White

# Test 3: Blob Storage Upload
Write-Host "TEST 3: Testing Blob Storage Upload..." -ForegroundColor Yellow
Set-Location "UploadHelper"
$uploadResult = dotnet run "../../../SampleData"
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Blob Storage Upload: WORKING" -ForegroundColor Green
} else {
    Write-Host "❌ Blob Storage Upload: FAILED" -ForegroundColor Red
}
Set-Location ".."

Write-Host "" -ForegroundColor White

# Final Instructions
Write-Host "FINAL SETUP INSTRUCTIONS:" -ForegroundColor Cyan
Write-Host "1. Open Cosmos DB Explorer: https://localhost:8081/_explorer/index.html" -ForegroundColor White
Write-Host "2. Create database 'iotdb'" -ForegroundColor White
Write-Host "3. Create containers:" -ForegroundColor White
Write-Host "   - documents (partition key: /deviceType)" -ForegroundColor Gray
Write-Host "   - telemetry (partition key: /deviceId)" -ForegroundColor Gray
Write-Host "   - devices (partition key: /deviceId)" -ForegroundColor Gray
Write-Host "   - alerts (partition key: /deviceId)" -ForegroundColor Gray
Write-Host "" -ForegroundColor White
Write-Host "4. Then test complete system:" -ForegroundColor White
Write-Host "   Terminal 1: cd FunctionApp && func start" -ForegroundColor Gray
Write-Host "   Terminal 2: cd TelemetryGenerator && dotnet run" -ForegroundColor Gray
Write-Host "   Terminal 3: curl -X POST http://localhost:7071/api/chat -H 'Content-Type: application/json' -d '{\"deviceId\":\"pump-001\",\"question\":\"Why is pump showing high temperature?\"}'" -ForegroundColor Gray
Write-Host "" -ForegroundColor White
Write-Host "🎉 IoTRag system ready for full testing!" -ForegroundColor Green