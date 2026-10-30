<img src="docs/branding/banner.png" alt="FM View Exporter" width="100%">

[🇩🇪 Deutsch](README.md) · **🇬🇧 English**

Exports the player list currently shown in Football Manager 27 (squad, player search) to
**CSV** and **HTML** with a single key press. FM26 dropped the built-in export; this plugin
fills the gap. The files work directly with the
[FM Player Analyzer](https://github.com/ElectroHugin/FM-Player-Analyzer).

- Exports every column of the view, with headers exactly as in the game, including `Unique ID`.
- Only reads the user interface. Never changes your save and never connects to the internet.
- Free and open source under the GNU GPL v3. Not affiliated with Sports Interactive or SEGA.

> **Status:** test version for FM27 (Windows, Steam). Tested with the squad view.

## Requirements

- Windows 10 or 11, Football Manager 27 on Steam
- Nothing else: the full package ships the mod loader [BepInEx](https://github.com/BepInEx/BepInEx).

## Installation

1. Download **`FMViewExporter-…-komplettpaket.zip`** (full package) from the latest
   [release](https://github.com/ElectroHugin/FM-View-Exporter/releases).
2. Open the game folder: in Steam right-click *Football Manager 27* > *Manage* >
   *Browse local files*. This is the folder that contains `fm.exe`.
3. Extract the **contents** of the zip straight into that folder, not into a subfolder.
   `winhttp.dll`, `doorstop_config.ini` and the `BepInEx` folder now sit next to `fm.exe`.
4. Start the game. **The first start takes a few minutes** while BepInEx generates files;
   for this BepInEx downloads Unity base libraries once from `unity.bepinex.dev` (that is
   BepInEx, not this plugin). Later starts are as fast as usual.
5. Check: `BepInEx\LogOutput.log` contains a line `FM View Exporter … geladen`.

**Updating:** download only `FMViewExporter-…-plugin.zip`, replace
`BepInEx\plugins\FMViewExporter.dll` in the game folder and restart the game.

## Usage

1. Open the squad or player search in the game and pick the view you want.
2. Press **F9**. The plugin scrolls through the list on its own; please don't click anything
   until it is done. Large lists can take a while.
3. The files are written to `Documents\Sports Interactive\FM View Exporter`, e.g.
   `squad_20261030_192739.csv` and `.html`.

The CSV is UTF-8 with comma delimiters (English Excel). For German Excel set
`Export.CsvStyle = German` (semicolon, decimal comma).

## With the FM Player Analyzer

1. Create the view described in [docs/ANSICHT-ANALYZER-FM27.md](docs/ANSICHT-ANALYZER-FM27.md)
   once in the game. It holds exactly the columns the Analyzer needs.
2. Export squad and player search with that view using F9.
3. Import the CSV files in the Analyzer.

The meaning of every column is listed in the [column reference](docs/SPALTEN-FM27.md)
(German and English, also as [CSV](docs/columns/fm27.csv)).

## Good to know

- FM27 only fills roughly the first 255 columns of a view, in the game too. Keep export
  views lean.
- Columns that only show an icon in the game (e.g. morale) are empty in the export.
  Star ratings (Ability, Potential) are exported as numbers from 0.0 to 5.0.

## Settings

File `BepInEx\config\de.electrohugin.fmviewexporter.cfg` in the game folder (created on first
start). Changes in the `Export` section apply from the next F9, hotkeys after a restart.

| Setting | Default | Meaning |
| --- | --- | --- |
| `Hotkeys.Export` | `F9` | Export the visible list |
| `Hotkeys.Diagnose` | `F10` | Dump of the user interface (for troubleshooting) |
| `Export.CsvStyle` | `English` | `English` = comma, `German` = semicolon and decimal comma |
| `Export.OutputFolder` | empty | empty = `Documents\Sports Interactive\FM View Exporter` |
| `Export.ScanColumns` | `false` | `true` = also scroll sideways through all columns (slower, not needed in FM27) |
| `Export.FramesPerStep` | `3` | Wait per scroll step; increase if values are missing |

## Problems?

- **Nothing happens on F9:** check `BepInEx\LogOutput.log`. "Keine Spielerliste gefunden"
  means no player table is open.
- **The game no longer starts or behaves oddly:** rename or delete `winhttp.dll` in the game
  folder. The game then starts without mods.
- **Report a bug:** open an [issue](https://github.com/ElectroHugin/FM-View-Exporter/issues)
  and attach `LogOutput.log`. A dump made with **F10** (in
  `Documents\Sports Interactive\FM View Exporter\Diagnose`) helps too.

## Uninstall

Delete `winhttp.dll`, `doorstop_config.ini`, `.doorstop_version`, `changelog.txt` and the
folders `BepInEx` and `dotnet` from the game folder. Your saves are not affected.

## Development

See the [German README](README.md#entwicklung).
