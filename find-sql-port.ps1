# Find SQL Server actual port
Write-Host "🔍 Finding SQL Server port..." -ForegroundColor Green

$result = sqlcmd -S "MINIJOE\SQLDEVELOPER2022" -U sa -P "pa`$`$word" -Q "SELECT local_net_address, local_tcp_port FROM sys.dm_exec_connections WHERE session_id = @@SPID" -h -1

Write-Host "SQL Server connection details:" -ForegroundColor Yellow
$result

# Extract port from result
$port = ($result | Select-String "\d+$").Matches.Value
if ($port) {
    Write-Host "✅ Found port: $port" -ForegroundColor Green
    Write-Host "Test WSL2 connection with:" -ForegroundColor Cyan
    Write-Host "sqlcmd -S '10.255.255.254,$port' -U sa -P 'pa`$`$word' -Q 'SELECT @@SERVERNAME' -C" -ForegroundColor White
} else {
    Write-Host "❌ Could not extract port. Check the output above." -ForegroundColor Red
}