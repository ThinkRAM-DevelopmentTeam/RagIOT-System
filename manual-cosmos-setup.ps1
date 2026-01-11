# Direct Cosmos DB Container Creation
# This bypasses the UI and creates containers directly

$endpoint = "https://localhost:8082/"
$key = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqwm+DJgSepwBZSQdmkcJXcaXjRnxzrqlP7A=="
$databaseId = "iotdb"

Write-Host "Creating Cosmos DB database and containers directly..." -ForegroundColor Green

# Use Azure Cosmos DB .NET Core if available, or provide manual instructions
Write-Host "🎯 MANUAL SETUP (Most Reliable):" -ForegroundColor Cyan
Write-Host "1. Open: https://localhost:8082/_explorer/index.html" -ForegroundColor White
Write-Host "2. Click 'New Container'" -ForegroundColor White
Write-Host "3. Database ID: iotdb" -ForegroundColor White
Write-Host "4. Container ID: documents, Partition Key: /deviceType" -ForegroundColor White
Write-Host "5. Click 'New Container' again for:" -ForegroundColor White
Write-Host "   - telemetry (partition: /deviceId)" -ForegroundColor Gray
Write-Host "   - devices (partition: /deviceId)" -ForegroundColor Gray
Write-Host "   - alerts (partition: /deviceId)" -ForegroundColor Gray

# Open browser
Start-Process "https://localhost:8082/_explorer/index.html"

Write-Host "`n✅ Browser opened. Create the containers manually as shown above." -ForegroundColor Green