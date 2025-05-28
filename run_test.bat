@echo off
cd "C:\Insight Software\AutoBot-Enterprise"
"C:\Program Files\Microsoft Visual Studio\2022\Enterprise\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" "AutoBotUtilities.Tests\bin\x64\Debug\net48\AutoBotUtilities.Tests.dll" --Tests:CanImportAmazoncomOrder11291264431163432 --Platform:x64
pause
