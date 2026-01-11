# Fixed PowerShell Test for ChatWithRag
Write-Host "Testing ChatWithRag with correct headers..." -ForegroundColor Green

try {
    $body = @{
        deviceId = "pump-001"
        question = "Why is pump showing high temperature?"
    } | ConvertTo-Json
    
    $response = Invoke-WebRequest -Uri "http://localhost:7071/api/chat" `
        -Method POST `
        -ContentType "application/json" `
        -Body $body `
        -UseBasicParsing `
        -TimeoutSec 30

    Write-Host "✅ SUCCESS!" -ForegroundColor Green
    Write-Host "Status Code: $($response.StatusCode)" -ForegroundColor Cyan
    Write-Host "Response:" -ForegroundColor White
    $response.Content | ConvertFrom-Json | ConvertTo-Json -Depth 10
    
} catch {
    Write-Host "❌ ERROR: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        Write-Host "Status Code: $($_.Exception.Response.StatusCode)" -ForegroundColor Yellow
    }
}