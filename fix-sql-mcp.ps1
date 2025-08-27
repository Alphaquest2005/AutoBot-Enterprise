#!/usr/bin/env powershell
# Simple SQL MCP Fix Script - Just Works

Write-Host "🚀 Fixing SQL Server for MCP..." -ForegroundColor Green

# Enable SQL Browser and restart services
Write-Host "1. Configuring services..." -ForegroundColor Yellow
Set-Service "SQLBrowser" -StartupType Automatic
Start-Service "SQLBrowser"
Restart-Service "MSSQL`$SQLDEVELOPER2022"

# Add firewall rules (if not already exists)
Write-Host "2. Configuring firewall..." -ForegroundColor Yellow
try {
    New-NetFirewallRule -DisplayName "SQL Server MCP" -Direction Inbound -Protocol TCP -LocalPort 1433 -Action Allow -ErrorAction SilentlyContinue
    New-NetFirewallRule -DisplayName "SQL Browser MCP" -Direction Inbound -Protocol UDP -LocalPort 1434 -Action Allow -ErrorAction SilentlyContinue
} catch { }

# Test local connection
Write-Host "3. Testing local connection..." -ForegroundColor Yellow
$result = sqlcmd -S "MINIJOE\SQLDEVELOPER2022" -U sa -P "pa`$`$word" -Q "SELECT @@SERVERNAME" -h -1 2>$null
if ($result) {
    Write-Host "✅ Local connection works!" -ForegroundColor Green
} else {
    Write-Host "❌ Local connection failed" -ForegroundColor Red
}

Write-Host "4. Starting MCP server test..." -ForegroundColor Yellow
Write-Host "Run this in WSL2 to test:" -ForegroundColor Cyan
Write-Host "sqlcmd -S '10.255.255.254\SQLDEVELOPER2022' -U sa -P 'pa`$`$word' -Q 'SELECT @@SERVERNAME' -C" -ForegroundColor White

Write-Host "5. If WSL2 test works, start MCP with:" -ForegroundColor Cyan  
Write-Host "cd '/mnt/c/Insight Software/AutoBot-Enterprise/mcp-servers/mssql-mcp-server' && npm start" -ForegroundColor White

Write-Host "✅ Done! Test the WSL2 connection now." -ForegroundColor Green