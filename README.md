<div align="center">

<img src="docs/branding/banner.png" alt="FM View Exporter" width="100%">

### Your FM27 player lists as a spreadsheet. One key press.

A free plugin for **Football Manager 27** that brings back the export Sports
Interactive dropped: press **F9** on your squad or player search and get every
column as **CSV** and **HTML**, with headers exactly as in the game.

[![Download](https://img.shields.io/badge/%E2%AC%87%20Download-free-1B6AF5?style=for-the-badge)](https://github.com/ElectroHugin/FM-View-Exporter/releases)

[![Latest release](https://img.shields.io/github/v/release/ElectroHugin/FM-View-Exporter?include_prereleases&label=latest%20version)](https://github.com/ElectroHugin/FM-View-Exporter/releases)
[![License: GPL v3](https://img.shields.io/badge/license-GPLv3-blue)](LICENSE)
![Platform](https://img.shields.io/badge/platform-Windows%2010%20%2F%2011-0078d4)
![Football Manager 27](https://img.shields.io/badge/Football%20Manager-27-1B6AF5)
![Mod loader](https://img.shields.io/badge/BepInEx-6%20IL2CPP-555)

**🇬🇧 English** · [🇩🇪 Deutsch](README.de.md)

[Why you'll want it](#-why-youll-want-it) ·
[Get started](#-get-started-in-5-minutes) ·
[With the FM Player Analyzer](#-made-for-the-fm-player-analyzer) ·
[Settings](#-settings) ·
[FAQ](#-faq)

</div>

---

## 🔥 Why you'll want it

Since FM26 there is no *Print → Export* any more. Your scouting list, your
squad, all those carefully built views stay locked inside the game. FM View
Exporter reads the table you are looking at and writes it to a file, the way
the old export used to.

| You want | You get |
|---|---|
| *"My whole squad in Excel."* | Open the squad, press **F9**, done. A CSV and an HTML file land in your Documents folder. |
| *"Every column, not just the first screen."* | The plugin scrolls through the list by itself and collects every row and every column of the view. |
| *"Headers I recognise."* | Column names exactly as FM shows them, from `Unique ID` to `xG-OP`. |
| *"Players I can track across exports."* | `Unique ID` is exported, so tools can match the same player over a whole save. |
| *"It should open in German Excel too."* | Comma by default, or semicolon and decimal comma with one setting. |
| *"I want to know what `Svh` means."* | A [column reference](docs/SPALTEN-FM27.md) explains all 375 columns in English and German. |

**And it keeps out of your way:**

- 🆓 **Free and open source.** GPLv3, no ads, no account.
- 🔌 **Offline.** The plugin never connects to the internet.
- 🛡️ **Your save is never touched.** It only reads what is on screen.
- 🎯 **No cheating.** It exports exactly what you see. Unscouted values stay hidden.
- 🧹 **Clean data.** Colours and links are stripped, star ratings become numbers (`0.0` to `5.0`).

---

## 🚀 Get started in 5 minutes

**1 · Download**

Grab **`FMViewExporter-…-setup.exe`** from the
[Releases page](https://github.com/ElectroHugin/FM-View-Exporter/releases).
It contains everything, including the mod loader [BepInEx](https://github.com/BepInEx/BepInEx).

**2 · Run the setup**

Start the setup and confirm the Windows prompt (the game folder usually needs
administrator rights). It finds Football Manager 27 in your Steam libraries on
its own; if not, pick the folder that contains `fm.exe`. Click **Install**.

**3 · Pick your settings (optional)**

In the same window you can set a file prefix such as `fc-bayern`, turn the
timestamp off, choose CSV, HTML or both, and more. Click **Save settings**.

<details>
<summary>Prefer to install by hand?</summary>

<br>

Download **`FMViewExporter-…-komplettpaket.zip`** instead. In Steam, right-click
*Football Manager 27* → *Manage* → *Browse local files* and extract the
**contents** of the zip straight into that folder, so that `winhttp.dll`,
`doorstop_config.ini` and the `BepInEx` folder sit next to `fm.exe`.

</details>

**4 · Start the game**

The **first start takes a few minutes** while BepInEx prepares itself. For this
BepInEx downloads Unity base libraries once from `unity.bepinex.dev` (that is
BepInEx, not this plugin). Later starts are as fast as usual. To check it
worked, look for `FM View Exporter … geladen` in `BepInEx\LogOutput.log`.

**5 · Export**

Open your squad or the player search, pick a view and press **F9**. The plugin
scrolls through the list on its own, so please don't click anything until it is
done. Your files are in `Documents\Sports Interactive\FM View Exporter`, for
example `squad_20261030_192739.csv` and `.html`.

> **Tips**
> - Updating? Run the new setup and click **Update**. Your settings are kept.
> - German Excel? Set the CSV style to `German` in the setup (see [Settings](#-settings)). It applies from the next F9, no restart needed.
> - Keep export views lean. FM27 only fills roughly the first 255 columns of a view, in the game too.

---

## 🤝 Made for the FM Player Analyzer

The exports feed straight into the
[**FM Player Analyzer**](https://github.com/ElectroHugin/FM-Player-Analyzer),
which turns them into role ratings, an optimal Best XI and signing advice.

1. Import the ready-made Analyzer views from [`views/fm27/`](views/fm27) in the game
   (squad, player database, shortlist). How, and how to build them yourself:
   [docs/VIEW-ANALYZER-FM27.md](docs/VIEW-ANALYZER-FM27.md).
2. Export your squad and the player search with that view using **F9**.
3. Import the CSV files in the Analyzer.

The two projects share one column contract: the column names in
[`docs/columns/fm27.csv`](docs/columns/fm27.csv) are what the Analyzer reads.

<details>
<summary><b>📋 Column reference and menu catalogue</b></summary>

<br>

| File | What's inside |
|---|---|
| [docs/SPALTEN-FM27.md](docs/SPALTEN-FM27.md) | All 375 table columns with group, meaning (English and German) and format |
| [docs/columns/fm27.csv](docs/columns/fm27.csv) | The same as CSV, for tools |
| [docs/columns/fm27-menu.csv](docs/columns/fm27-menu.csv) | All 377 entries of FM27's *Insert Column* menu and the header each one produces |

The menu names often differ from the table headers (*Mistakes Leading To Goal*
becomes `MLG`), which is why both lists exist.

</details>

---

## 🔧 Settings

The easiest way is the setup program: run it again at any time and change the
settings there. They are stored in
`BepInEx\config\de.electrohugin.fmviewexporter.cfg` in the game folder, which you
can also edit by hand. Changes in the `Export` section apply from the
next F9, hotkeys after a restart.

| Setting | Default | Meaning |
| --- | --- | --- |
| `Hotkeys.Export` | `F9` | Export the visible list |
| `Hotkeys.Diagnose` | `F10` | Dump of the user interface (for troubleshooting) |
| `Export.CsvStyle` | `English` | `English` = comma, `German` = semicolon and decimal comma |
| `Export.OutputFolder` | empty | empty = `Documents\Sports Interactive\FM View Exporter` |
| `Export.FilePrefix` | empty | Put in front of the file name, e.g. `fc-bayern` → `fc-bayern-squad.csv` |
| `Export.Timestamp` | `true` | `false` = fixed file name, each export overwrites the last one |
| `Export.Formats` | `Both` | `Both`, `CsvOnly` or `HtmlOnly` |
| `Export.ScanColumns` | `false` | `true` = also scroll sideways through all columns (slower, not needed in FM27) |
| `Export.FramesPerStep` | `3` | Wait per scroll step; increase if values are missing |

---

## ❓ FAQ

**Is it really free?**
Yes. Open source under GPLv3, no ads, no paid tier.

**Can it damage my save game?**
No. The plugin only reads the table on screen and writes files to your
Documents folder. It never opens or changes a save.

**Some columns are empty. Why?**
FM27 only fills roughly the first 255 columns of a view, and that happens in
the game too. Columns that only show an icon (morale, for example) are empty
as well. Put the columns you need first, or split them over two views.

**Nothing happens when I press F9.**
Check `BepInEx\LogOutput.log`. "Keine Spielerliste gefunden" means no player
table is open right now.

**The game no longer starts or behaves oddly.**
Rename or delete `winhttp.dll` in the game folder. The game then starts without
mods.

**Which versions are supported?**
Football Manager 27 on Steam for Windows. Linux (Proton) will follow.

**How do I uninstall it?**
Run the setup and click **Uninstall**. By hand: delete `winhttp.dll`, `doorstop_config.ini`, `.doorstop_version`,
`changelog.txt` and the folders `BepInEx` and `dotnet` from the game folder.
Your saves are not affected.

**I found a bug.**
[Open an issue](https://github.com/ElectroHugin/FM-View-Exporter/issues) and
attach `LogOutput.log`. A dump made with **F10** (in
`Documents\Sports Interactive\FM View Exporter\Diagnose`) helps too.

---

<details>
<summary><b>🛠️ For developers: repository layout and building from source</b></summary>

<br>

| Directory | Contents |
|---|---|
| [`src/FMViewExporter.Core`](src/FMViewExporter.Core) | Table model, CSV and HTML writers (no Unity, unit tested) |
| [`src/FMViewExporter`](src/FMViewExporter) | The BepInEx plugin (.NET 6) |
| [`src/FMViewExporter.Setup`](src/FMViewExporter.Setup) | Setup program for Windows (.NET Framework 4.8): install, update, uninstall, settings |
| [`src/FMViewExporter.Setup.Logic`](src/FMViewExporter.Setup.Logic) | Its logic without UI (unit tested) |
| [`tests/`](tests) | Tests for the core |
| [`lib/ref/`](lib/ref) | Reference assemblies for compiling, see [lib/ref/README.md](lib/ref/README.md) |
| [`packaging/`](packaging) | Full package with BepInEx |
| [`tools/`](tools) | Sources of the column reference and the menu catalogue |

```
dotnet test tests/FMViewExporter.Core.Tests -c Release
dotnet build src/FMViewExporter -c Release
```

GitHub Actions builds every push. A release is created from a `v*` tag or by
starting the workflow manually with `release_tag`; tags with a hyphen become
pre-releases.

</details>

---

## License & disclaimer

Licensed under **GPLv3**, see [LICENSE](LICENSE).

This is an unofficial, fan-made tool and is not endorsed by or affiliated with
Sports Interactive or SEGA. *Football Manager* and Sports Interactive are
trademarks of Sports Interactive Limited; SEGA is a trademark of SEGA
Corporation. All in-game data is the property of Sports Interactive and/or
SEGA. The software is provided "as is", without warranty of any kind.
