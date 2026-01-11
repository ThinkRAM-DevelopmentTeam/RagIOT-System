# FINAL TEST - All Issues Fixed
Write-Host "FINAL IoTRag TEST - All Issues Should Be Fixed" -ForegroundColor Green
Write-Host "==============================================" -ForegroundColor White

Write-Host "FIXES APPLIED:" -ForegroundColor Yellow
Write-Host "✅ Removed duplicate Content-Type header from ChatWithRag.cs" -ForegroundColor Green
Write-Host "✅ Fixed Cosmos DB connection to port 8081 (Windows emulator)" -ForegroundColor Green
Write-Host "✅ Changed Cosmos DB ConnectionMode to Direct" -ForegroundColor Green
Write-Host "" -ForegroundColor White

Write-Host "Testing ChatWithRag endpoint..." -ForegroundColor Cyan
try {
    $body = @{
        deviceId = "pump-001"
        question = "Why is pump showing high temperature?"
    } | ConvertTo-Json -Compress
    
    Write-Host "Sending request with body:" -ForegroundColor Gray
    Write-Host $body -ForegroundColor White
    
    $response = Invoke-WebRequest -Uri "http://localhost:7071/api/chat" `
        -Method POST `
        -ContentType "application/json" `
        -Body $body `
        -UseBasicParsing `
        -TimeoutSec 30

    Write-Host "" -ForegroundColor White
    Write-Host "✅ SUCCESS! Status: $($response.StatusCode)" -ForegroundColor Green
    
    $jsonResponse = $response.Content | ConvertFrom-Json
    if ($jsonResponse.error) {
        Write-Host "❌ Still has error: $($jsonResponse.error)" -ForegroundColor Red
    } else {
        Write-Host "✅ Working! Response:" -ForegroundColor Green
        $jsonResponse | ConvertTo-Json -Depth 5
    }
    
} catch {
    Write-Host "❌ ERROR: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        Write-Host "Status Code: $($_.Exception.Response.StatusCode)" -ForegroundColor Yellow
        $errorContent = $_.Exception.Response.GetResponseStream()
        $reader = New-Object System.IO.StreamReader($errorContent)
        $errorText = $reader.ReadToEnd()
        Write-Host "Error Details: $errorText" -ForegroundColor Gray
    }
}

Write-Host "" -ForegroundColor White
Write-Host "Next step: Create Cosmos DB containers if not working" -ForegroundColor Cyan