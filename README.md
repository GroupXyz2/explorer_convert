# File Converter Extension für Windows Explorer

Ein Windows Explorer Kontext-Menü-Addon, das es ermöglicht, Dateien direkt mit FFmpeg zu konvertieren.

## Features

- 🎬 **Video-Konvertierung**: MP4, MKV, AVI, MOV, WebM, FLV, WMV und mehr
- 🎵 **Audio-Konvertierung**: MP3, AAC, FLAC, WAV, OGG, Opus und mehr
- 🖼️ **Bild-Konvertierung**: JPG, PNG, WebP, BMP, GIF, TIFF
- 🔄 **Automatischer Ersatz**: Option zum Ersetzen der Originaldatei
- ⚡ **Hintergrund-Konvertierung**: FFmpeg läuft im Hintergrund
- 🎯 **Intelligente Format-Auswahl**: Passende Formate je nach Dateityp

## Voraussetzungen

### 1. .NET 8.0 SDK
Download: [https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download)

### 2. FFmpeg
FFmpeg muss installiert und im System-PATH verfügbar sein.

**Installation auf Windows:**

#### Option A: Mit Chocolatey (empfohlen)
```powershell
choco install ffmpeg
```

#### Option B: Mit Scoop
```powershell
scoop install ffmpeg
```

#### Option C: Manuelle Installation
1. Download von [https://ffmpeg.org/download.html](https://ffmpeg.org/download.html)
2. Entpacken Sie die ZIP-Datei
3. Fügen Sie den `bin` Ordner zum System-PATH hinzu:
   - Rechtsklick auf "Dieser PC" → Eigenschaften
   - Erweiterte Systemeinstellungen → Umgebungsvariablen
   - Unter "System-Variablen" → PATH → Bearbeiten
   - Neuer Eintrag: Pfad zum `bin` Ordner (z.B. `C:\ffmpeg\bin`)

**FFmpeg testen:**
```cmd
ffmpeg -version
```

## Installation

### 1. Projekt bauen

Führen Sie `Build.bat` aus oder nutzen Sie die Kommandozeile:

```cmd
dotnet build FileConverterExtension.sln -c Release
```

Die fertige Executable befindet sich dann in:
```
FileConverterExtension\bin\Release\net8.0-windows\FileConverterExtension.exe
```

### 2. Kontext-Menü installieren

**Wichtig: Als Administrator ausführen!**

```cmd
# Rechtsklick auf Install.bat → "Run as Administrator"
```

Oder doppelklicken Sie auf `Install.bat` und bestätigen Sie die Administrator-Berechtigung.

Das Installations-Skript:
- Registriert das Kontext-Menü in der Windows Registry
- Prüft, ob FFmpeg verfügbar ist
- Fügt das Icon zum Kontext-Menü hinzu

## Verwendung

1. **Datei im Windows Explorer** auswählen
2. **Rechtsklick** auf die Datei
3. **"Convert with FFmpeg..."** auswählen
4. **Zielformat** aus der Dropdown-Liste wählen
5. Optional: **"Replace original file"** aktivieren/deaktivieren
6. **"Convert"** klicken

### Optionen

- **Replace original file**: Wenn aktiviert, wird die Originaldatei nach erfolgreicher Konvertierung gelöscht
- Wenn deaktiviert, wird die neue Datei mit dem gleichen Namen aber neuer Endung gespeichert

## Unterstützte Formate

### Video
- **Input/Output**: MP4, MKV, AVI, MOV, WebM, FLV, WMV, M4V, MPG, MPEG

### Audio
- **Input/Output**: MP3, AAC, FLAC, WAV, OGG, M4A, WMA, Opus, ALAC

### Bilder
- **Input/Output**: JPG, PNG, WebP, BMP, GIF, TIFF

## Deinstallation

**Als Administrator ausführen:**

```cmd
# Rechtsklick auf Uninstall.bat → "Run as Administrator"
```

Oder doppelklicken Sie auf `Uninstall.bat` und bestätigen Sie die Administrator-Berechtigung.

Dies entfernt alle Registry-Einträge für das Kontext-Menü.

## Qualitätseinstellungen

Die Konvertierung verwendet optimierte Einstellungen für jedes Format:

### Video
- **MP4**: H.264, CRF 23, AAC Audio 192k
- **MKV**: H.265, CRF 28, AAC Audio 192k
- **WebM**: VP9, AAC Opus Audio

### Audio
- **MP3**: 320 kbps
- **AAC**: 256 kbps
- **FLAC**: Verlustfrei
- **WAV**: PCM 16-bit

### Bilder
- **JPG**: Hohe Qualität (q:v 2)
- **PNG**: Maximale Kompression
- **WebP**: 90% Qualität

## Fehlerbehandlung

### "FFmpeg not found"
- Stellen Sie sicher, dass FFmpeg im PATH ist
- Testen Sie mit `ffmpeg -version` in der Kommandozeile

### "Conversion failed"
- Überprüfen Sie, ob die Quelldatei nicht beschädigt ist
- Versuchen Sie ein anderes Zielformat
- Prüfen Sie, ob genügend Speicherplatz vorhanden ist

### Kontext-Menü erscheint nicht
- Führen Sie `Install.bat` erneut als Administrator aus
- Starten Sie den Windows Explorer neu (Task-Manager → Windows Explorer → Neu starten)

## Projektstruktur

```
FileConverterExtension/
│
├── FileConverterExtension.sln          # Visual Studio Solution
├── Build.bat                           # Build-Skript
├── Install.bat                         # Installations-Skript (Wrapper)
├── Install.ps1                         # PowerShell Installations-Skript
├── Uninstall.bat                       # Deinstallations-Skript (Wrapper)
├── Uninstall.ps1                       # PowerShell Deinstallations-Skript
├── README.md                           # Diese Datei
│
└── FileConverterExtension/
    ├── FileConverterExtension.csproj   # Projekt-Konfiguration
    ├── Program.cs                      # Haupteinstiegspunkt
    ├── ConversionForm.cs               # UI für Format-Auswahl
    ├── FFmpegConverter.cs              # FFmpeg-Konvertierungs-Logik
    └── app.manifest                    # Administrator-Rechte für Installation
```

## Technische Details

- **Framework**: .NET 8.0 Windows Forms
- **Sprache**: C#
- **Registry**: HKEY_CLASSES_ROOT\*\shell\ConvertWithFFmpeg
- **FFmpeg**: Externe Prozessausführung mit optimierten Parametern

## Lizenz

Dieses Projekt kann frei verwendet und modifiziert werden.

## Hinweise

- Die Konvertierung kann je nach Dateigröße einige Minuten dauern
- Größere Dateien benötigen entsprechend mehr Zeit
- Die Originalqualität wird bestmöglich erhalten
- Bei Problemen prüfen Sie die FFmpeg-Verfügbarkeit

## Support

Bei Problemen:
1. Überprüfen Sie die FFmpeg-Installation
2. Stellen Sie sicher, dass Sie Administrator-Rechte haben
3. Prüfen Sie die Systemvoraussetzungen
4. Testen Sie die Konvertierung manuell mit FFmpeg auf der Kommandozeile
