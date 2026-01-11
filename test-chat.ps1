# Test ChatWithRag Endpoint
Write-Host "Testing ChatWithRag endpoint..." -ForegroundColor Green

try {
    $response = Invoke-WebRequest -Uri "http://localhost:7071/api/chat" `
        -Method POST `
        -ContentType "application/json" `
        -Body '{"deviceId":"pump-001","question":"Why is pump showing high temperature?"}' `
        -TimeoutSec 30

    Write-Host "Response Status: $($response.StatusCode)" -ForegroundColor Cyan
    Write-Host "Response Content:" -ForegroundColor White
    Write-Host $response.Content -ForegroundColor Gray
    
} catch {
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        Write-Host "Status Code: $($_.Exception.Response.StatusCode)" -ForegroundColor Yellow
    }
}