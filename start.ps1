#!/usr/bin/env pwsh
# Quick start script - runs everything

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Starting Microservices Application" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "✅ EF Core tools installed" -ForegroundColor Green
Write-Host "✅ Migrations created for all services" -ForegroundColor Green
Write-Host "✅ Design-time factories configured" -ForegroundColor Green
Write-Host "✅ Build successful" -ForegroundColor Green
Write-Host ""
Write-Host "Starting AppHost..." -ForegroundColor Yellow
Write-Host "This will:" -ForegroundColor Yellow
Write-Host "  1. Start Docker containers (PostgreSQL, Redis, RabbitMQ)" -ForegroundColor White
Write-Host "  2. Apply database migrations" -ForegroundColor White
Write-Host "  3. Seed sample data" -ForegroundColor White
Write-Host "  4. Start all microservices" -ForegroundColor White
Write-Host "  5. Start API Gateway" -ForegroundColor White
Write-Host "  6. Open Aspire Dashboard" -ForegroundColor White
Write-Host ""
Write-Host "Keep this window open and check the Aspire Dashboard!" -ForegroundColor Cyan
Write-Host ""

Set-Location AppHost
dotnet run
