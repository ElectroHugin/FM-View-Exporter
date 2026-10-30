<img src="docs/branding/banner.png" alt="FM View Exporter" width="100%">

Exportiert die gerade angezeigte Spielerliste in Football Manager (Kader, Spielersuche) als CSV und HTML.
Seit FM26 hat das Spiel keinen eingebauten Export mehr; dieses Plugin schliesst die Luecke.

*Exports the player list currently shown in Football Manager to CSV and HTML (BepInEx 6 plugin, starting with FM27).*

- Liest nur die Oberflaeche. Veraendert keinen Spielstand, baut keine Internetverbindung auf.
- Spaltenkoepfe wie im Spiel, inklusive UID (fuer den FM Player Analyzer).
- Frei und quelloffen unter der GNU GPL v3.
- Nicht offiziell mit Sports Interactive oder SEGA verbunden.

## Stand

Testversion (Erkundungs-Build): Das Plugin laeuft in FM27 und schreibt mit **F10** ein Abbild der
Oberflaeche. Der eigentliche Export mit **F9** folgt in der naechsten Version.

## Installation

1. Unter [Releases](https://github.com/ElectroHugin/FM-View-Exporter/releases) das neueste
   `...-komplettpaket.zip` herunterladen.
2. Steam: Rechtsklick auf Football Manager 27 > Verwalten > Lokale Dateien durchsuchen.
3. Den Inhalt der Zip direkt in diesen Ordner entpacken (neben `fm.exe`).
4. Spiel starten. Der erste Start dauert einige Minuten.

Entfernen: `winhttp.dll` im Spielordner loeschen.

## Einstellungen

`BepInEx\config\de.electrohugin.fmviewexporter.cfg` (entsteht nach dem ersten Start):

| Einstellung | Standard | Bedeutung |
| --- | --- | --- |
| `Hotkeys.Export` | `F9` | Export der angezeigten Liste |
| `Hotkeys.Diagnose` | `F10` | Abbild der Oberflaeche |
| `Export.CsvStyle` | `English` | `English` = Komma, `German` = Semikolon |
| `Export.OutputFolder` | leer | leer = `Dokumente\Sports Interactive\FM View Exporter` |

## Entwicklung

- `src/FMViewExporter.Core` – Tabellenmodell, CSV, HTML (ohne Unity, getestet)
- `src/FMViewExporter` – BepInEx-Plugin (.NET 6)
- `tests/` – Tests fuer den Kern
- `lib/ref/` – Referenz-Assemblies zum Kompilieren, siehe [lib/ref/README.md](lib/ref/README.md)
- `packaging/` – Komplettpaket mit BepInEx

```
dotnet test tests/FMViewExporter.Core.Tests -c Release
dotnet build src/FMViewExporter -c Release
```

GitHub Actions baut bei jedem Push. Ein Tag `v*` erzeugt ein Release, Tags mit Bindestrich als Pre-release.
