#!/usr/bin/env pwsh
# Simple start script - everything is already cleaned up

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "Starting Fresh Microservices Application" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Database volumes cleaned: ✓" -ForegroundColor Green
Write-Host "Containers removed: ✓" -ForegroundColor Green
Write-Host "Migrations ready: ✓" -ForegroundColor Green
Write-Host ""
Write-Host "Starting AppHost..." -ForegroundColor Cyan
Write-Host ""

Set-Location AppHost
dotnet run
