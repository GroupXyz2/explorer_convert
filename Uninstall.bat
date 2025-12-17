@echo off
echo ====================================
echo File Converter Extension - Uninstall
echo ====================================
echo.
echo This will remove the context menu entry.
echo Please make sure you run this as Administrator!
echo.
pause

powershell.exe -ExecutionPolicy Bypass -File "%~dp0Uninstall.ps1"

if errorlevel 1 (
    echo.
    echo Uninstallation failed!
    pause
    exit /b 1
)

echo.
pause
