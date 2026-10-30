# FM View Exporter

**🇩🇪 Deutsch** · [🇬🇧 English](README.en.md)

Exportiert die gerade angezeigte Spielerliste in Football Manager 27 (Kader, Spielersuche) per
Tastendruck als **CSV** und **HTML**. Seit FM26 hat das Spiel keinen eingebauten Export mehr;
dieses Plugin schließt die Lücke. Die Dateien passen direkt zum
[FM Player Analyzer](https://github.com/ElectroHugin/FM-Player-Analyzer).

- Exportiert alle Spalten der Ansicht, mit den Spaltenköpfen genau wie im Spiel, inklusive `Unique ID`.
- Liest nur die Oberfläche. Verändert keinen Spielstand und baut keine Internetverbindung auf.
- Frei und quelloffen unter der GNU GPL v3. Nicht offiziell mit Sports Interactive oder SEGA verbunden.

> **Stand:** Testversion für FM27 (Windows, Steam). Getestet mit der Kaderansicht.

## Voraussetzungen

- Windows 10 oder 11, Football Manager 27 über Steam
- Keine weiteren Programme nötig: Das Komplettpaket bringt den Mod-Lader
  [BepInEx](https://github.com/BepInEx/BepInEx) gleich mit.

## Installation

1. Unter [Releases](https://github.com/ElectroHugin/FM-View-Exporter/releases) beim neuesten
   Release die Datei **`FMViewExporter-…-komplettpaket.zip`** herunterladen.
2. Den Spielordner öffnen: In Steam Rechtsklick auf *Football Manager 27* >
   *Verwalten* > *Lokale Dateien durchsuchen*. Das ist der Ordner, in dem `fm.exe` liegt.
3. Den **Inhalt** der Zip-Datei direkt in diesen Ordner entpacken, nicht in einen Unterordner.
   Danach liegen `winhttp.dll`, `doorstop_config.ini` und der Ordner `BepInEx` neben `fm.exe`.
4. Spiel starten. **Der erste Start dauert einige Minuten**, weil BepInEx einmalig Dateien
   erzeugt. Dafür lädt BepInEx einmalig Unity-Grunddateien von `unity.bepinex.dev` herunter
   (das ist BepInEx, nicht dieses Plugin). Ab dem zweiten Start geht es normal schnell.
5. Prüfen: In `BepInEx\LogOutput.log` steht eine Zeile `FM View Exporter … geladen`.

**Update:** Nur die Datei `FMViewExporter-…-plugin.zip` laden und die darin enthaltene
`BepInEx\plugins\FMViewExporter.dll` im Spielordner ersetzen. Dann das Spiel neu starten.

## Benutzung

1. Im Spiel den Kader oder die Spielersuche öffnen und die gewünschte Ansicht wählen.
2. **F9** drücken. Das Plugin scrollt die Liste einmal selbst durch. Bitte so lange nichts
   anklicken; bei großen Listen kann das etwas dauern.
3. Die Dateien liegen in `Dokumente\Sports Interactive\FM View Exporter`, z. B.
   `squad_20261030_192739.csv` und `.html`.

Die CSV ist UTF-8 und nutzt standardmäßig Komma als Trennzeichen (englisches Excel). Für
deutsches Excel `Export.CsvStyle = German` setzen (Semikolon, Dezimalkomma).

## Zusammen mit dem FM Player Analyzer

1. Im Spiel einmal die Ansicht aus [docs/ANSICHT-ANALYZER-FM27.md](docs/ANSICHT-ANALYZER-FM27.md)
   anlegen. Sie enthält genau die Spalten, die der Analyzer braucht.
2. Kader und Spielersuche mit dieser Ansicht per F9 exportieren.
3. Die CSV-Dateien im Analyzer importieren.

Was jede Spalte bedeutet, steht im [Spaltenverzeichnis](docs/SPALTEN-FM27.md)
(auch maschinenlesbar als [CSV](docs/columns/fm27.csv)).

## Gut zu wissen

- FM27 zeigt nur in etwa den ersten 255 Spalten einer Ansicht Werte an, auch im Spiel selbst.
  Für den Export lieber schlanke Ansichten nutzen.
- Spalten, die im Spiel nur ein Symbol zeigen (z. B. Moral), sind im Export leer.
  Sterne (Ability, Potential) werden als Zahl von 0.0 bis 5.0 exportiert.

## Einstellungen

Datei `BepInEx\config\de.electrohugin.fmviewexporter.cfg` im Spielordner (entsteht beim
ersten Start). Änderungen am Abschnitt `Export` gelten ab dem nächsten F9, Tasten erst nach
einem Neustart.

| Einstellung | Standard | Bedeutung |
| --- | --- | --- |
| `Hotkeys.Export` | `F9` | Export der angezeigten Liste |
| `Hotkeys.Diagnose` | `F10` | Abbild der Oberfläche (für Fehlersuche) |
| `Export.CsvStyle` | `English` | `English` = Komma, `German` = Semikolon und Dezimalkomma |
| `Export.OutputFolder` | leer | leer = `Dokumente\Sports Interactive\FM View Exporter` |
| `Export.ScanColumns` | `false` | `true` = auch seitlich durch alle Spalten scrollen (langsamer, in FM27 nicht nötig) |
| `Export.FramesPerStep` | `3` | Wartezeit je Scrollschritt; bei fehlenden Werten erhöhen |

## Probleme?

- **Nichts passiert bei F9:** In `BepInEx\LogOutput.log` nachsehen. Steht dort „Keine
  Spielerliste gefunden“, ist gerade keine Spielertabelle offen.
- **Spiel startet nicht mehr oder verhält sich seltsam:** `winhttp.dll` im Spielordner
  umbenennen oder löschen. Das Spiel startet dann ohne Mods.
- **Fehler melden:** Ein [Issue](https://github.com/ElectroHugin/FM-View-Exporter/issues)
  anlegen und `LogOutput.log` anhängen. Hilfreich ist auch ein Abbild mit **F10** aus
  `Dokumente\Sports Interactive\FM View Exporter\Diagnose`.

## Deinstallation

Im Spielordner `winhttp.dll`, `doorstop_config.ini`, `.doorstop_version`, `changelog.txt`
sowie die Ordner `BepInEx` und `dotnet` löschen. Spielstände bleiben unberührt.

## Entwicklung

- `src/FMViewExporter.Core` – Tabellenmodell, CSV, HTML (ohne Unity, getestet)
- `src/FMViewExporter` – BepInEx-Plugin (.NET 6)
- `tests/` – Tests fuer den Kern
- `lib/ref/` – Referenz-Assemblies zum Kompilieren, siehe [lib/ref/README.md](lib/ref/README.md)
- `packaging/` – Komplettpaket mit BepInEx
- `tools/columns_fm27.py` – Quelle des Spaltenverzeichnisses (`docs/`)

```
dotnet test tests/FMViewExporter.Core.Tests -c Release
dotnet build src/FMViewExporter -c Release
```

GitHub Actions baut bei jedem Push. Ein Release entsteht über einen Tag `v*` oder einen manuellen
Start des Workflows mit `release_tag`; Tags mit Bindestrich werden Pre-releases.
