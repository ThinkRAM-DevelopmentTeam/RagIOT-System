# Test Rebuilt Function App
Write-Host "Testing Rebuilt Function App..." -ForegroundColor Green

try {
    $simpleBody = '{"deviceId":"pump-001","question":"Test after rebuild"}'
    
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
        try {
            $jsonResponse = $response.Content | ConvertFrom-Json
            Write-Host "JSON Response:" -ForegroundColor White
            $jsonResponse | ConvertTo-Json -Depth 5
        } catch {
            Write-Host "JSON Parse Error: $($_.Exception.Message)" -ForegroundColor Yellow
            Write-Host "Raw Response: $($response.Content)" -ForegroundColor Gray
        }
    }
    
} catch {
    Write-Host "❌ ERROR: $($_.Exception.Message)" -ForegroundColor Red
}