@echo off
echo ====================================
echo File Converter Extension - Install
echo ====================================
echo.
echo This will install the context menu entry.
echo Please make sure you run this as Administrator!
echo.
pause

powershell.exe -ExecutionPolicy Bypass -File "%~dp0Install.ps1"

if errorlevel 1 (
    echo.
    echo Installation failed!
    pause
    exit /b 1
)

echo.
pause
