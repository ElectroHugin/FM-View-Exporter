FM View Exporter
================

Exports the player list you are looking at in Football Manager 27 (squad,
player database, shortlist) as CSV and HTML.
Free software under the GNU GPL v3: https://github.com/ElectroHugin/FM-View-Exporter
Not affiliated with Sports Interactive or SEGA.

The plugin only reads the user interface. It never changes a save game and
never connects to the internet.

Installation
------------
Easiest with FMViewExporter-<version>-setup.exe from the releases page:
start it, click Install, done. The setup also edits the settings.

By hand (full package):
1. Steam: right-click Football Manager 27 > Manage > Browse local files.
2. Extract the whole content of this zip into that folder.
   winhttp.dll and the BepInEx folder must sit right next to fm.exe.
3. Start the game. The first start takes a few minutes because BepInEx
   prepares itself once. For this BepInEx downloads Unity base files once
   from unity.bepinex.dev (that is BepInEx, not this plugin).

Keys
----
F9   exports the visible player list (squad, player database or shortlist)
     as CSV and HTML to Documents\Sports Interactive\FM View Exporter.
     The plugin scrolls through the list by itself; please don't click
     anything until "Export fertig" appears in BepInEx\LogOutput.log.
F10  writes a dump of the user interface to
     Documents\Sports Interactive\FM View Exporter\Diagnose

Settings: run the setup again, or edit
BepInEx\config\de.electrohugin.fmviewexporter.cfg (created on the first start).

Uninstall
---------
Run the setup and click Uninstall. By hand: delete winhttp.dll in the game
folder and the game starts without mods. Completely: also delete
doorstop_config.ini, .doorstop_version, changelog.txt, the two
"FM View Exporter - ..." text files and the folders BepInEx and dotnet.
Your saves are not affected.
