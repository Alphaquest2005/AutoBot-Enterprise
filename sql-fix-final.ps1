# Final SQL Fix - Enable Remote Access
Write-Host "🔧 Enabling remote access in SQL Server..." -ForegroundColor Green

sqlcmd -S "MINIJOE\SQLDEVELOPER2022" -U sa -P "pa`$`$word" -Q "EXEC sys.sp_configure 'remote access', 1; RECONFIGURE WITH OVERRIDE;"

Write-Host "✅ Remote access enabled. Test WSL2 connection now." -ForegroundColor Green