# Create Cosmos DB database and containers manually
$cosmosEndpoint = "https://localhost:8082"
$cosmosKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqwm+DJgSepwBZSQdmkcJXcaXjRnxzrqlP7A=="
$databaseId = "iot-rag-db"
$containers = @(
    @{ id = "telemetry"; partitionKey = "/deviceId" },
    @{ id = "documents"; partitionKey = "/id" },
    @{ id = "devices"; partitionKey = "/deviceId" },
    @{ id = "alerts"; partitionKey = "/deviceId" }
)

# Function to create auth header
function New-CosmosAuthHeader {
    param([string]$Method, [string]$ResourceType, [string]$ResourceId)
    
    $keyBytes = [System.Convert]::FromBase64String($cosmosKey)
    $dateString = [DateTime]::UtcNow.ToString("r")
    
    # Create the payload to sign
    $payload = "$($Method.ToLower())`n$($ResourceType.ToLower())`n$ResourceId`n$($dateString.ToLower())`n`n"
    $payloadBytes = [System.Text.Encoding]::UTF8.GetBytes($payload)
    
    # Create signature
    $hmac = New-Object System.Security.Cryptography.HMACSHA256($keyBytes)
    $hash = $hmac.ComputeHash($payloadBytes)
    $signature = [System.Convert]::ToBase64String($hash)
    
    # Return auth header
    $auth = "type=master&ver=1.0&sig=$([System.Web.HttpUtility]::UrlEncode($signature))"
    return @{
        "Authorization" = $auth
        "x-ms-version" = "2018-12-31"
        "x-ms-date" = $dateString
        "Content-Type" = "application/json"
    }
}

Write-Host "Creating Cosmos DB database and containers..." -ForegroundColor Cyan

# Create database
Write-Host "Creating database '$databaseId'..."
$headers = New-CosmosAuthHeader -Method "POST" -ResourceType "dbs" -ResourceId ""
$dbPayload = @{ id = $databaseId } | ConvertTo-Json

try {
    $response = Invoke-WebRequest -Uri "$cosmosEndpoint/dbs" -Method POST -Headers $headers -Body $dbPayload -SkipCertificateCheck -ErrorAction Stop
    Write-Host "✓ Database created successfully" -ForegroundColor Green
}
catch {
    if ($_.Exception.Response.StatusCode -eq 409) {
        Write-Host "✓ Database already exists" -ForegroundColor Yellow
    }
    else {
        Write-Host "✗ Error creating database: $($_.Exception.Message)" -ForegroundColor Red
        exit 1
    }
}

# Create containers
foreach ($container in $containers) {
    Write-Host "Creating container '$($container.id)'..."
    $headers = New-CosmosAuthHeader -Method "POST" -ResourceType "colls" -ResourceId "dbs/$databaseId"
    
    $containerPayload = @{
        id = $container.id
        partitionKey = @{ paths = @($container.partitionKey) }
        indexingPolicy = @{
            indexingMode = "consistent"
            automatic = $true
            includedPaths = @(@{ path = "/*" })
            excludedPaths = @(@{ path = '/_etag/?' })
        }
    } | ConvertTo-Json -Depth 10
    
    try {
        $response = Invoke-WebRequest -Uri "$cosmosEndpoint/dbs/$databaseId/colls" -Method POST -Headers $headers -Body $containerPayload -SkipCertificateCheck -ErrorAction Stop
        Write-Host "✓ Container '$($container.id)' created successfully" -ForegroundColor Green
    }
    catch {
        if ($_.Exception.Response.StatusCode -eq 409) {
            Write-Host "✓ Container '$($container.id)' already exists" -ForegroundColor Yellow
        }
        else {
            Write-Host "✗ Error creating container '$($container.id)': $($_.Exception.Message)" -ForegroundColor Red
        }
    }
}

Write-Host "`nDatabase and containers setup complete!" -ForegroundColor Cyan
