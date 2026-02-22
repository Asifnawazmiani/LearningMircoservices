# Development Workflow Helper Script
# This script provides common commands for working with the microservices solution

param(
    [Parameter(Mandatory=$false)]
    [ValidateSet('build', 'run', 'test', 'clean', 'migrations', 'help')]
    [string]$Command = 'help'
)

function Show-Help {
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "Microservices Development Helper" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Usage: .\dev.ps1 <command>" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Available Commands:" -ForegroundColor Green
    Write-Host "  build       - Build the entire solution" -ForegroundColor White
    Write-Host "  run         - Start the AppHost (all services)" -ForegroundColor White
    Write-Host "  test        - Run the API test script" -ForegroundColor White
    Write-Host "  clean       - Clean build artifacts" -ForegroundColor White
    Write-Host "  migrations  - Create database migrations" -ForegroundColor White
    Write-Host "  help        - Show this help message" -ForegroundColor White
    Write-Host ""
    Write-Host "Examples:" -ForegroundColor Green
    Write-Host "  .\dev.ps1 build" -ForegroundColor Gray
    Write-Host "  .\dev.ps1 run" -ForegroundColor Gray
    Write-Host "  .\dev.ps1 test" -ForegroundColor Gray
    Write-Host ""
}

function Build-Solution {
    Write-Host "Building solution..." -ForegroundColor Yellow
    dotnet build
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Build successful" -ForegroundColor Green
    } else {
        Write-Host "✗ Build failed" -ForegroundColor Red
        exit $LASTEXITCODE
    }
}

function Run-Application {
    Write-Host "Starting AppHost..." -ForegroundColor Yellow
    Write-Host "This will start all services, databases, and the API Gateway" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Press Ctrl+C to stop all services" -ForegroundColor Gray
    Write-Host ""
    
    Push-Location AppHost
    try {
        dotnet run
    }
    finally {
        Pop-Location
    }
}

function Test-Services {
    Write-Host "Running service tests..." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Make sure the AppHost is running before running tests!" -ForegroundColor Red
    Write-Host "You can run it in another terminal with: .\dev.ps1 run" -ForegroundColor Gray
    Write-Host ""
    
    $response = Read-Host "Is the AppHost running? (y/n)"
    if ($response -ne 'y') {
        Write-Host "Please start the AppHost first, then run this command again." -ForegroundColor Yellow
        return
    }
    
    Write-Host ""
    $gatewayPort = Read-Host "Enter the API Gateway port (check Aspire Dashboard)"
    
    if ([string]::IsNullOrWhiteSpace($gatewayPort)) {
        Write-Host "Port is required. Check the Aspire Dashboard for the gateway port." -ForegroundColor Red
        return
    }
    
    # Update the test script with the correct port
    $testScript = Get-Content -Path "test-services.ps1" -Raw
    $testScript = $testScript -replace '\$gatewayUrl = "https://localhost:\d+"', "`$gatewayUrl = `"https://localhost:$gatewayPort`""
    $testScript | Set-Content -Path "test-services.ps1"
    
    Write-Host "Updated test script with port $gatewayPort" -ForegroundColor Green
    Write-Host ""
    
    .\test-services.ps1
}

function Clean-Solution {
    Write-Host "Cleaning solution..." -ForegroundColor Yellow
    dotnet clean
    
    Write-Host "Removing bin and obj folders..." -ForegroundColor Yellow
    Get-ChildItem -Include bin,obj -Recurse -Force | Remove-Item -Force -Recurse
    
    Write-Host "✓ Clean complete" -ForegroundColor Green
}

function Create-Migrations {
    Write-Host "Creating database migrations..." -ForegroundColor Yellow
    Write-Host ""
    
    # OrderService migrations
    Write-Host "1. Creating OrderService migrations..." -ForegroundColor Cyan
    Push-Location OrderService.Infrastructure
    try {
        dotnet ef migrations add InitialCreate --startup-project ../OrderService
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✓ OrderService migration created" -ForegroundColor Green
        }
    }
    catch {
        Write-Host "✗ Failed to create OrderService migration: $_" -ForegroundColor Red
    }
    finally {
        Pop-Location
    }
    
    Write-Host ""
    
    # InventoryService migrations
    Write-Host "2. Creating InventoryService migrations..." -ForegroundColor Cyan
    Push-Location InventoryService.Infrastructure
    try {
        dotnet ef migrations add InitialCreate --startup-project ../InventoryService
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✓ InventoryService migration created" -ForegroundColor Green
        }
    }
    catch {
        Write-Host "✗ Failed to create InventoryService migration: $_" -ForegroundColor Red
    }
    finally {
        Pop-Location
    }
    
    Write-Host ""
    Write-Host "Migrations created successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "To apply migrations, either:" -ForegroundColor Yellow
    Write-Host "  1. Start the AppHost (migrations will auto-apply if configured)" -ForegroundColor Gray
    Write-Host "  2. Manually run: dotnet ef database update --startup-project ../[Service]" -ForegroundColor Gray
}

# Execute the requested command
switch ($Command.ToLower()) {
    'build' {
        Build-Solution
    }
    'run' {
        Run-Application
    }
    'test' {
        Test-Services
    }
    'clean' {
        Clean-Solution
    }
    'migrations' {
        Create-Migrations
    }
    'help' {
        Show-Help
    }
    default {
        Show-Help
    }
}
