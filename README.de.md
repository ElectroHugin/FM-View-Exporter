<div align="center">

<img src="docs/branding/banner.png" alt="FM View Exporter" width="100%">

### Deine FM27-Spielerlisten als Tabelle. Ein Tastendruck.

Ein kostenloses Plugin für **Football Manager 27**, das den Export zurückbringt,
den Sports Interactive gestrichen hat: Im Kader oder in der Spielersuche **F9**
drücken und alle Spalten als **CSV** und **HTML** bekommen, mit den
Spaltenköpfen genau wie im Spiel.

[![Download](https://img.shields.io/badge/%E2%AC%87%20Download-kostenlos-1B6AF5?style=for-the-badge)](https://github.com/ElectroHugin/FM-View-Exporter/releases)

[![Neueste Version](https://img.shields.io/github/v/release/ElectroHugin/FM-View-Exporter?include_prereleases&label=neueste%20Version)](https://github.com/ElectroHugin/FM-View-Exporter/releases)
[![Lizenz: GPL v3](https://img.shields.io/badge/Lizenz-GPLv3-blue)](LICENSE)
![Plattform](https://img.shields.io/badge/Plattform-Windows%2010%20%2F%2011-0078d4)
![Football Manager 27](https://img.shields.io/badge/Football%20Manager-27-1B6AF5)
![Mod-Lader](https://img.shields.io/badge/BepInEx-6%20IL2CPP-555)

[🇬🇧 English](README.md) · **🇩🇪 Deutsch**

[Warum du es willst](#-warum-du-es-willst) ·
[Loslegen](#-in-5-minuten-startklar) ·
[Mit dem FM Player Analyzer](#-gemacht-für-den-fm-player-analyzer) ·
[Einstellungen](#-einstellungen) ·
[FAQ](#-faq)

</div>

---

## 🔥 Warum du es willst

Seit FM26 gibt es kein *Drucken → Exportieren* mehr. Scoutingliste, Kader und
all die mühsam gebauten Ansichten bleiben im Spiel eingesperrt. FM View
Exporter liest die Tabelle, die du gerade siehst, und schreibt sie in eine
Datei, so wie früher der eingebaute Export.

| Du willst | Du bekommst |
|---|---|
| *„Meinen ganzen Kader in Excel."* | Kader öffnen, **F9** drücken, fertig. Eine CSV- und eine HTML-Datei landen in deinem Dokumente-Ordner. |
| *„Alle Spalten, nicht nur den ersten Bildschirm."* | Das Plugin scrollt die Liste selbst durch und sammelt jede Zeile und jede Spalte der Ansicht. |
| *„Spaltenköpfe, die ich kenne."* | Spaltennamen genau wie im Spiel, von `Unique ID` bis `xG-OP`. |
| *„Spieler über mehrere Exporte verfolgen."* | `Unique ID` wird mitexportiert, so erkennen Programme denselben Spieler über den ganzen Spielstand. |
| *„Es soll auch im deutschen Excel aufgehen."* | Standardmäßig Komma, mit einer Einstellung Semikolon und Dezimalkomma. |
| *„Ich will wissen, was `Svh` heißt."* | Ein [Spaltenverzeichnis](docs/SPALTEN-FM27.md) erklärt alle 375 Spalten auf Deutsch und Englisch. |

**Und es stört nicht:**

- 🆓 **Kostenlos und quelloffen.** GPLv3, keine Werbung, kein Konto.
- 🔌 **Offline.** Das Plugin baut nie eine Internetverbindung auf.
- 🛡️ **Dein Spielstand bleibt unberührt.** Es liest nur, was auf dem Bildschirm steht.
- 🎯 **Kein Schummeln.** Exportiert wird genau das, was du siehst. Nicht gescoutete Werte bleiben verborgen.
- 🧹 **Saubere Daten.** Farben und Links werden entfernt, Sterne werden zu Zahlen (`0.0` bis `5.0`).

---

## 🚀 In 5 Minuten startklar

**1 · Herunterladen**

Unter [Releases](https://github.com/ElectroHugin/FM-View-Exporter/releases) die
Datei **`FMViewExporter-…-komplettpaket.zip`** laden. Sie bringt den Mod-Lader
[BepInEx](https://github.com/BepInEx/BepInEx) gleich mit, sonst brauchst du nichts.

**2 · Spielordner öffnen**

In Steam Rechtsklick auf *Football Manager 27* → *Verwalten* → *Lokale Dateien
durchsuchen*. Das ist der Ordner, in dem `fm.exe` liegt.

**3 · Entpacken**

Den **Inhalt** der Zip-Datei direkt in diesen Ordner entpacken, nicht in einen
Unterordner. Danach liegen `winhttp.dll`, `doorstop_config.ini` und der Ordner
`BepInEx` neben `fm.exe`.

**4 · Spiel starten**

Der **erste Start dauert einige Minuten**, weil BepInEx sich einrichtet. Dafür
lädt BepInEx einmalig Unity-Grunddateien von `unity.bepinex.dev` herunter (das
ist BepInEx, nicht dieses Plugin). Ab dem zweiten Start geht es normal schnell.
Ob alles geklappt hat, zeigt die Zeile `FM View Exporter … geladen` in
`BepInEx\LogOutput.log`.

**5 · Exportieren**

Kader oder Spielersuche öffnen, Ansicht wählen und **F9** drücken. Das Plugin
scrollt die Liste selbst durch, bitte so lange nichts anklicken. Die Dateien
liegen in `Dokumente\Sports Interactive\FM View Exporter`, zum Beispiel
`squad_20261030_192739.csv` und `.html`.

> **Tipps**
> - Update? Nur `FMViewExporter-…-plugin.zip` laden, `BepInEx\plugins\FMViewExporter.dll` ersetzen und das Spiel neu starten.
> - Deutsches Excel? `Export.CsvStyle = German` setzen (siehe [Einstellungen](#-einstellungen)). Gilt ab dem nächsten F9, ohne Neustart.
> - Exportansichten schlank halten. FM27 befüllt nur ungefähr die ersten 255 Spalten einer Ansicht, auch im Spiel selbst.

---

## 🤝 Gemacht für den FM Player Analyzer

Die Exporte passen direkt zum
[**FM Player Analyzer**](https://github.com/ElectroHugin/FM-Player-Analyzer),
der daraus Rollenbewertungen, eine optimale Startelf und Transfertipps macht.

1. Im Spiel einmal die Analyzer-Ansicht anlegen, wie in
   [docs/VIEW-ANALYZER-FM27.md](docs/VIEW-ANALYZER-FM27.md) beschrieben.
2. Kader und Spielersuche mit dieser Ansicht per **F9** exportieren.
3. Die CSV-Dateien im Analyzer importieren.

Beide Projekte teilen sich eine Spaltenvereinbarung: Die Spaltennamen in
[`docs/columns/fm27.csv`](docs/columns/fm27.csv) sind das, was der Analyzer einliest.

<details>
<summary><b>📋 Spaltenverzeichnis und Menükatalog</b></summary>

<br>

| Datei | Inhalt |
|---|---|
| [docs/SPALTEN-FM27.md](docs/SPALTEN-FM27.md) | Alle 375 Tabellenspalten mit Gruppe, Bedeutung (Deutsch und Englisch) und Format |
| [docs/columns/fm27.csv](docs/columns/fm27.csv) | Dasselbe als CSV, für Programme |
| [docs/columns/fm27-menu.csv](docs/columns/fm27-menu.csv) | Alle 377 Einträge des FM27-Menüs *Insert Column* und der Spaltenkopf, den jeder erzeugt |

Die Menünamen weichen oft von den Spaltenköpfen ab (aus *Mistakes Leading To
Goal* wird `MLG`), deshalb gibt es beide Listen.

</details>

---

## 🔧 Einstellungen

Die Datei `BepInEx\config\de.electrohugin.fmviewexporter.cfg` im Spielordner
entsteht beim ersten Start. Änderungen im Abschnitt `Export` gelten ab dem
nächsten F9, Tasten erst nach einem Neustart.

| Einstellung | Standard | Bedeutung |
| --- | --- | --- |
| `Hotkeys.Export` | `F9` | Export der angezeigten Liste |
| `Hotkeys.Diagnose` | `F10` | Abbild der Oberfläche (für Fehlersuche) |
| `Export.CsvStyle` | `English` | `English` = Komma, `German` = Semikolon und Dezimalkomma |
| `Export.OutputFolder` | leer | leer = `Dokumente\Sports Interactive\FM View Exporter` |
| `Export.ScanColumns` | `false` | `true` = auch seitlich durch alle Spalten scrollen (langsamer, in FM27 nicht nötig) |
| `Export.FramesPerStep` | `3` | Wartezeit je Scrollschritt; bei fehlenden Werten erhöhen |

---

## ❓ FAQ

**Ist das wirklich kostenlos?**
Ja. Quelloffen unter GPLv3, keine Werbung, keine Bezahlversion.

**Kann es meinen Spielstand beschädigen?**
Nein. Das Plugin liest nur die Tabelle auf dem Bildschirm und schreibt Dateien
in deinen Dokumente-Ordner. Einen Spielstand öffnet oder ändert es nie.

**Manche Spalten sind leer. Warum?**
FM27 befüllt nur ungefähr die ersten 255 Spalten einer Ansicht, das ist auch
im Spiel so. Spalten, die nur ein Symbol zeigen (zum Beispiel Moral), sind
ebenfalls leer. Wichtige Spalten nach vorne stellen oder auf zwei Ansichten
verteilen.

**Bei F9 passiert nichts.**
In `BepInEx\LogOutput.log` nachsehen. Steht dort „Keine Spielerliste gefunden",
ist gerade keine Spielertabelle offen.

**Das Spiel startet nicht mehr oder verhält sich seltsam.**
`winhttp.dll` im Spielordner umbenennen oder löschen. Das Spiel startet dann
ohne Mods.

**Welche Versionen werden unterstützt?**
Football Manager 27 über Steam unter Windows. Linux (Proton) folgt später.

**Wie deinstalliere ich es?**
Im Spielordner `winhttp.dll`, `doorstop_config.ini`, `.doorstop_version`,
`changelog.txt` sowie die Ordner `BepInEx` und `dotnet` löschen. Spielstände
bleiben unberührt.

**Ich habe einen Fehler gefunden.**
Ein [Issue](https://github.com/ElectroHugin/FM-View-Exporter/issues) anlegen
und `LogOutput.log` anhängen. Hilfreich ist auch ein Abbild mit **F10** aus
`Dokumente\Sports Interactive\FM View Exporter\Diagnose`.

---

<details>
<summary><b>🛠️ Für Entwickler: Aufbau und selbst bauen</b></summary>

<br>

| Ordner | Inhalt |
|---|---|
| [`src/FMViewExporter.Core`](src/FMViewExporter.Core) | Tabellenmodell, CSV- und HTML-Schreiber (ohne Unity, getestet) |
| [`src/FMViewExporter`](src/FMViewExporter) | Das BepInEx-Plugin (.NET 6) |
| [`tests/`](tests) | Tests für den Kern |
| [`lib/ref/`](lib/ref) | Referenz-Assemblies zum Kompilieren, siehe [lib/ref/README.md](lib/ref/README.md) |
| [`packaging/`](packaging) | Komplettpaket mit BepInEx |
| [`tools/`](tools) | Quellen des Spaltenverzeichnisses und des Menükatalogs |

```
dotnet test tests/FMViewExporter.Core.Tests -c Release
dotnet build src/FMViewExporter -c Release
```

GitHub Actions baut bei jedem Push. Ein Release entsteht über einen Tag `v*`
oder einen manuellen Start des Workflows mit `release_tag`; Tags mit Bindestrich
werden Pre-releases.

</details>

---

## Lizenz und Hinweis

Lizenziert unter **GPLv3**, siehe [LICENSE](LICENSE).

Dies ist ein inoffizielles Fan-Projekt und steht in keiner Verbindung zu Sports
Interactive oder SEGA. *Football Manager* und Sports Interactive sind Marken
der Sports Interactive Limited; SEGA ist eine Marke der SEGA Corporation. Alle
Spieldaten gehören Sports Interactive und/oder SEGA. Die Software wird ohne
jede Gewährleistung bereitgestellt.
