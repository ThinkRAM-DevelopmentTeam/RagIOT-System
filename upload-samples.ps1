#!/usr/bin/env pwsh
# Script to upload sample SOP files to Azurite Blob Storage (local emulator)
# Compiles and runs a C# helper application

$scriptRoot = $PSScriptRoot
$uploadHelperPath = Join-Path -Path $scriptRoot -ChildPath "UploadHelper"
$sampleDataPath = Join-Path -Path $scriptRoot -ChildPath "SampleData"

# Check if SampleData folder exists
if (-Not (Test-Path -Path $sampleDataPath)) {
    Write-Host "ERROR: SampleData folder not found at $sampleDataPath" -ForegroundColor Red
    exit 1
}

# Build the helper app if needed
if (-Not (Test-Path -Path "$uploadHelperPath/bin/Release/net7.0/UploadHelper.dll")) {
    Write-Host "Building UploadHelper..." -ForegroundColor Green
    Push-Location $uploadHelperPath
    dotnet build -c Release 2>&1 | Where-Object { $_ -match "error|Error" } | ForEach-Object { Write-Host $_ -ForegroundColor Red }
    if ($LASTEXITCODE -ne 0) {
        Write-Host "ERROR: Failed to build UploadHelper" -ForegroundColor Red
        Pop-Location
        exit 1
    }
    Pop-Location
}

# Run the helper
Write-Host "Uploading sample files..." -ForegroundColor Green
& dotnet run --project "$uploadHelperPath/UploadHelper.csproj" --configuration Release -- "$sampleDataPath"

exit $LASTEXITCODE
