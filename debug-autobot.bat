@echo off
setlocal enabledelayedexpansion

set MSBUILD_PATH="C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
set PROJECT_PATH=autobot1\autobot.csproj
set LOG_FILE=build_log.txt

echo =========================================
echo Debugging AutoBot project with MSBuild
echo =========================================
echo MSBuild Path: %MSBUILD_PATH%
echo Project: %PROJECT_PATH%
echo Log File: %LOG_FILE%
echo.

echo Attempting to build with x64 configuration...
%MSBUILD_PATH% %PROJECT_PATH% /p:Platform=x64 /p:Configuration=Debug /v:detailed /fileLogger /fileLoggerParameters:LogFile=%LOG_FILE%;Verbosity=detailed

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo x64 build failed with error code %ERRORLEVEL%
    echo Detailed error information has been saved to %LOG_FILE%
    echo Displaying last 20 lines of errors:
    echo.
    findstr /C:"error" %LOG_FILE% | tail -20
    echo.
    echo Attempting to build with x86 configuration instead...
    echo.
    %MSBUILD_PATH% %PROJECT_PATH% /p:Platform=x86 /p:Configuration=Debug /v:detailed /fileLogger /fileLoggerParameters:LogFile=%LOG_FILE%.x86;Verbosity=detailed
    
    if %ERRORLEVEL% NEQ 0 (
        echo.
        echo x86 build also failed with error code %ERRORLEVEL%
        echo Detailed error information has been saved to %LOG_FILE%.x86
        echo Displaying last 20 lines of errors:
        echo.
        findstr /C:"error" %LOG_FILE%.x86 | tail -20
        echo.
        echo Debug attempt unsuccessful.
    ) else (
        echo.
        echo Successfully built with x86 configuration.
    )
) else (
    echo.
    echo Successfully built with x64 configuration.
)

echo.
echo Debug build completed.
echo =========================================
echo Press any key to exit...
pause > nul

endlocal
