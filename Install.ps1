param(
    [switch]$Uninstall
)

$ErrorActionPreference = "Stop"


$currentPrincipal = New-Object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent())
$isAdmin = $currentPrincipal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

if (-not $isAdmin) {
    Write-Host "This script must be run as Administrator!" -ForegroundColor Red
    Write-Host "Please right-click and select 'Run as Administrator'" -ForegroundColor Yellow
    pause
    exit 1
}

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

$possiblePaths = @(
    (Join-Path $scriptDir "FileConverterExtension.exe"),
    (Join-Path $scriptDir "FileConverterExtension\bin\Release\net8.0-windows\FileConverterExtension.exe"),
    (Join-Path $scriptDir "bin\Release\net8.0-windows\FileConverterExtension.exe")
)

$exePath = $null
foreach ($path in $possiblePaths) {
    if (Test-Path $path) {
        $exePath = $path
        break
    }
}

if ($null -eq $exePath) {
    Write-Host "Error: FileConverterExtension.exe not found in any expected location!" -ForegroundColor Red
    Write-Host "Searched locations:" -ForegroundColor Yellow
    foreach ($path in $possiblePaths) {
        Write-Host "  - $path" -ForegroundColor Gray
    }
    pause
    exit 1
}

$registryPath = "Registry::HKEY_CLASSES_ROOT\*\shell\ConvertWithFFmpeg"
$commandPath = "$registryPath\command"

function Install-ContextMenu {
    Write-Host "Installing File Converter Extension..." -ForegroundColor Green
    
    if (-not (Test-Path $exePath)) {
        Write-Host "Error: FileConverterExtension.exe not found!" -ForegroundColor Red
        Write-Host "Expected path: $exePath" -ForegroundColor Yellow
        Write-Host "Please build the project first." -ForegroundColor Yellow
        pause
        exit 1
    }
    
    try {
        $ffmpegVersion = & ffmpeg -version 2>&1
        Write-Host "FFmpeg found: OK" -ForegroundColor Green
    }
    catch {
        Write-Host "FFmpeg not found. Installing automatically..." -ForegroundColor Yellow
        
        $ffmpegInstalled = $false
        
        try {
            $null = & winget --version 2>&1
            Write-Host "Installing FFmpeg via winget..." -ForegroundColor Cyan
            $wingetResult = & winget install --id Gyan.FFmpeg --silent --accept-package-agreements --accept-source-agreements 2>&1
            
            $env:Path = [System.Environment]::GetEnvironmentVariable("Path","Machine") + ";" + [System.Environment]::GetEnvironmentVariable("Path","User")
            
            Start-Sleep -Seconds 2
            try {
                $null = & ffmpeg -version 2>&1
                Write-Host "FFmpeg installed successfully!" -ForegroundColor Green
                $ffmpegInstalled = $true
            }
            catch { }
        }
        catch { }
        
        if (-not $ffmpegInstalled) {
            try {
                $null = & choco --version 2>&1
                Write-Host "Trying chocolatey installation..." -ForegroundColor Yellow
                & choco install ffmpeg -y | Out-Null
                
                $env:Path = [System.Environment]::GetEnvironmentVariable("Path","Machine") + ";" + [System.Environment]::GetEnvironmentVariable("Path","User")
                Start-Sleep -Seconds 2
                
                try {
                    $null = & ffmpeg -version 2>&1
                    Write-Host "FFmpeg installed via chocolatey!" -ForegroundColor Green
                    $ffmpegInstalled = $true
                }
                catch { }
            }
            catch { }
        }
        
        if (-not $ffmpegInstalled) {
            Write-Host "Automatic installation unsuccessful. Install FFmpeg manually:" -ForegroundColor Yellow
            Write-Host "Run: winget install Gyan.FFmpeg" -ForegroundColor Cyan
            Write-Host "Or download: https://www.gyan.dev/ffmpeg/builds/" -ForegroundColor Cyan
            Write-Host "Note: Context menu is installed, but conversions need FFmpeg." -ForegroundColor Yellow
        }
    }
    
    try {
        Write-Host "Creating registry entries..." -ForegroundColor Cyan
        
        $result1 = & reg add "HKCR\*\shell\ConvertWithFFmpeg" /ve /d "Convert with FFmpeg..." /f 2>&1
        Write-Host "Menu entry created" -ForegroundColor Gray
        
        $result2 = & reg add "HKCR\*\shell\ConvertWithFFmpeg" /v "Icon" /d "$exePath,0" /f 2>&1
        Write-Host "Icon set" -ForegroundColor Gray
        
        $result3 = & reg add "HKCR\*\shell\ConvertWithFFmpeg\command" /ve /d "`"$exePath`" `"%1`"" /f 2>&1
        Write-Host "Command set" -ForegroundColor Gray
        
        Write-Host ""
        Write-Host "Installation completed successfully!" -ForegroundColor Green
        Write-Host "Right-click any file in Windows Explorer and select 'Convert with FFmpeg...'" -ForegroundColor Cyan
    }
    catch {
        Write-Host "Error during installation: $_" -ForegroundColor Red
        Write-Host "Details: $result1 $result2 $result3" -ForegroundColor Yellow
        pause
        exit 1
    }
}

function Uninstall-ContextMenu {
    Write-Host "Uninstalling File Converter Extension..." -ForegroundColor Yellow
    
    try {
        if (Test-Path $registryPath) {
            Remove-Item -Path $registryPath -Recurse -Force
            Write-Host "Uninstallation completed successfully!" -ForegroundColor Green
        }
        else {
            Write-Host "Context menu entry not found. Already uninstalled?" -ForegroundColor Yellow
        }
    }
    catch {
        Write-Host "Error during uninstallation: $_" -ForegroundColor Red
        pause
        exit 1
    }
}

Write-Host "=================================" -ForegroundColor Cyan
Write-Host "File Converter Extension Setup" -ForegroundColor Cyan
Write-Host "=================================" -ForegroundColor Cyan
Write-Host ""

if ($Uninstall) {
    Uninstall-ContextMenu
}
else {
    Install-ContextMenu
}

Write-Host ""
Write-Host "Press any key to exit..."
pause | Out-Null
