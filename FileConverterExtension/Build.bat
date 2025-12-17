@echo off
echo ====================================
echo File Converter Extension - Builder
echo ====================================
echo.

echo Checking for .NET SDK...
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo ERROR: .NET SDK not found!
    echo Please install .NET 8.0 SDK from https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

echo Building project...
dotnet build FileConverterExtension.sln -c Release

if errorlevel 1 (
    echo.
    echo ERROR: Build failed!
    pause
    exit /b 1
)

echo.
echo ====================================
echo Build completed successfully!
echo ====================================
echo.
echo Output location: FileConverterExtension\bin\Release\net8.0-windows\
echo.
echo Next steps:
echo 1. Run Install.ps1 as Administrator to install the context menu
echo 2. Right-click any file in Windows Explorer
echo 3. Select "Convert with FFmpeg..."
echo.
pause
