@echo off
REM Simple database setup script - just start the AppHost!
REM Migrations and seeding happen automatically

echo.
echo ========================================
echo Database Setup - Automatic Mode
echo ========================================
echo.
echo This will start the AppHost which automatically:
echo   1. Applies EF Core migrations
echo   2. Seeds sample data
echo   3. Starts all services
echo.
echo Press any key to continue or Ctrl+C to cancel...
pause >nul

echo.
echo Starting AppHost...
echo.

cd AppHost
dotnet run
