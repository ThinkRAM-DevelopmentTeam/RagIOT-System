# Simple test without complex body
Write-Host "Testing with simple body..." -ForegroundColor Green

try {
    $simpleBody = '{"deviceId":"pump-001","question":"Simple test"}'
    
    Write-Host "Sending simple request..." -ForegroundColor Gray
    $response = Invoke-WebRequest -Uri "http://localhost:7071/api/chat" `
        -Method POST `
        -ContentType "application/json" `
        -Body $simpleBody `
        -UseBasicParsing `
        -TimeoutSec 30

    Write-Host "Status: $($response.StatusCode)" -ForegroundColor Cyan
    
    if ($response.Content -like '*Content-Type*') {
        Write-Host "❌ Still has Content-Type header error" -ForegroundColor Red
        Write-Host "Response: $($response.Content)" -ForegroundColor Gray
    } else {
        Write-Host "✅ SUCCESS! No header error!" -ForegroundColor Green
        $jsonResponse = $response.Content | ConvertFrom-Json
        Write-Host "Response:" -ForegroundColor White
        $jsonResponse | ConvertTo-Json -Depth 5
    }
    
} catch {
    Write-Host "❌ ERROR: $($_.Exception.Message)" -ForegroundColor Red
}