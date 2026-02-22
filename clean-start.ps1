#!/usr/bin/env pwsh
# Clean database and fresh start script

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Clean Database & Fresh Start" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if Docker is running
Write-Host "Checking Docker status..." -ForegroundColor Yellow
$dockerRunning = docker ps 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: Docker is not running!" -ForegroundColor Red
    Write-Host "Please start Docker Desktop and try again." -ForegroundColor Yellow
    exit 1
}
Write-Host "✓ Docker is running" -ForegroundColor Green
Write-Host ""

# Stop existing containers
Write-Host "Stopping existing containers..." -ForegroundColor Yellow
docker stop $(docker ps -a -q --filter "name=user-postgres") 2>$null
docker stop $(docker ps -a -q --filter "name=order-postgres") 2>$null
docker stop $(docker ps -a -q --filter "name=inventory-postgres") 2>$null
docker stop $(docker ps -a -q --filter "name=rabbitmq") 2>$null
docker stop $(docker ps -a -q --filter "name=redis") 2>$null
Write-Host "✓ Containers stopped" -ForegroundColor Green
Write-Host ""

# Remove containers
Write-Host "Removing containers..." -ForegroundColor Yellow
docker rm $(docker ps -a -q --filter "name=user-postgres") 2>$null
docker rm $(docker ps -a -q --filter "name=order-postgres") 2>$null
docker rm $(docker ps -a -q --filter "name=inventory-postgres") 2>$null
docker rm $(docker ps -a -q --filter "name=rabbitmq") 2>$null
docker rm $(docker ps -a -q --filter "name=redis") 2>$null
Write-Host "✓ Containers removed" -ForegroundColor Green
Write-Host ""

# Remove volumes
Write-Host "Removing database volumes..." -ForegroundColor Yellow
docker volume rm user-postgres-data 2>$null
docker volume rm order-postgres-data 2>$null
docker volume rm inventory-postgres-data 2>$null
docker volume rm user-redis-data 2>$null
docker volume rm order-redis-data 2>$null
docker volume rm inventory-redis-data 2>$null
docker volume rm rabbitmq-data 2>$null
Write-Host "✓ Volumes removed" -ForegroundColor Green
Write-Host ""

Write-Host "========================================" -ForegroundColor Green
Write-Host "Cleanup Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Starting fresh AppHost..." -ForegroundColor Yellow
Write-Host ""
Write-Host "This will:" -ForegroundColor Cyan
Write-Host "  1. Create fresh Docker containers" -ForegroundColor White
Write-Host "  2. Create empty databases" -ForegroundColor White
Write-Host "  3. Apply migrations (create tables)" -ForegroundColor White
Write-Host "  4. Seed sample data" -ForegroundColor White
Write-Host "  5. Start all services" -ForegroundColor White
Write-Host ""

Set-Location "D:\Asif Workspace\Learnings\Learning Microservices\LearningMircoservices\src\AppHost"
dotnet run
