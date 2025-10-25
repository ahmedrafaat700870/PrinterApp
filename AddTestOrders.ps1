# PowerShell Script to Add Test Orders to Database
# Run this script from PowerShell: .\AddTestOrders.ps1

$ServerInstance = "localhost"
$Database = "PrinterAppDb"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Adding Test Orders to Database" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Server: $ServerInstance" -ForegroundColor Yellow
Write-Host "Database: $Database" -ForegroundColor Yellow
Write-Host ""

# Check if SqlServer module is available
if (-not (Get-Module -ListAvailable -Name SqlServer)) {
    Write-Host "SqlServer module not found. Installing..." -ForegroundColor Yellow
    try {
        Install-Module -Name SqlServer -Force -AllowClobber -Scope CurrentUser
        Write-Host "SqlServer module installed successfully." -ForegroundColor Green
    }
    catch {
        Write-Host "Failed to install SqlServer module. Trying alternate method..." -ForegroundColor Yellow
    }
}

# SQL Script path
$SqlScriptPath = Join-Path $PSScriptRoot "AddTestOrders.sql"

if (-not (Test-Path $SqlScriptPath)) {
    Write-Host "ERROR: SQL script not found at: $SqlScriptPath" -ForegroundColor Red
    Write-Host "Please ensure AddTestOrders.sql is in the same directory." -ForegroundColor Red
    exit 1
}

Write-Host "Executing SQL script..." -ForegroundColor Yellow
Write-Host ""

try {
    # Try using Invoke-Sqlcmd if available
    if (Get-Command Invoke-Sqlcmd -ErrorAction SilentlyContinue) {
        Invoke-Sqlcmd -ServerInstance $ServerInstance -Database $Database -InputFile $SqlScriptPath -Verbose
        Write-Host ""
        Write-Host "========================================" -ForegroundColor Green
        Write-Host "SUCCESS! Test orders created." -ForegroundColor Green
        Write-Host "========================================" -ForegroundColor Green
    }
    else {
        # Fallback to sqlcmd.exe
        Write-Host "Using sqlcmd.exe..." -ForegroundColor Yellow
        $result = & sqlcmd -S $ServerInstance -d $Database -i $SqlScriptPath -b
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host $result
            Write-Host ""
            Write-Host "========================================" -ForegroundColor Green
            Write-Host "SUCCESS! Test orders created." -ForegroundColor Green
            Write-Host "========================================" -ForegroundColor Green
        }
        else {
            Write-Host "Error executing script. Exit code: $LASTEXITCODE" -ForegroundColor Red
            Write-Host $result -ForegroundColor Red
            exit $LASTEXITCODE
        }
    }
}
catch {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Red
    Write-Host "ERROR: Failed to execute SQL script" -ForegroundColor Red
    Write-Host "========================================" -ForegroundColor Red
    Write-Host "Error Details: $_" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please try one of these alternatives:" -ForegroundColor Yellow
    Write-Host "1. Open SQL Server Management Studio" -ForegroundColor White
    Write-Host "2. Connect to: $ServerInstance" -ForegroundColor White
    Write-Host "3. Open file: $SqlScriptPath" -ForegroundColor White
    Write-Host "4. Execute the script" -ForegroundColor White
    exit 1
}

Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Cyan
Write-Host "1. Run the application: dotnet run --project PrinterApp.web" -ForegroundColor White
Write-Host "2. Navigate to Print Orders page" -ForegroundColor White
Write-Host "3. Test pagination (50 orders created)" -ForegroundColor White
Write-Host "4. Test print queue with priority management" -ForegroundColor White
Write-Host ""
