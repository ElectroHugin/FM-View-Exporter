FM View Exporter for Linux (Proton)
===================================

Exports the player list you are looking at in Football Manager 27 (squad,
player database, shortlist) as CSV and HTML.
Free software under the GNU GPL v3: https://github.com/ElectroHugin/FM-View-Exporter
Not affiliated with Sports Interactive or SEGA.

The plugin only reads the user interface. It never changes a save game and
never connects to the internet.

Installation
------------
1. Extract this archive.
2. Make the program executable and run it:

     chmod +x fmviewexporter-setup
     ./fmviewexporter-setup install

   It finds Football Manager 27 in your Steam libraries on its own. If it does
   not, point it at the folder with fm.exe:

     ./fmviewexporter-setup install --game "/path/to/steamapps/common/Football Manager 27"

3. In Steam, right-click Football Manager 27 > Properties > Launch Options and
   enter:

     WINEDLLOVERRIDES="winhttp.dll=n,b" %command%

   This is needed so that Proton loads the winhttp.dll from BepInEx instead of
   Wine's own one. Without it the plugin never starts.

4. Start the game. The first start takes a few minutes because BepInEx prepares
   itself once. For this BepInEx downloads Unity base files once from
   unity.bepinex.dev (that is BepInEx, not this plugin).

5. Press F9 in the squad or the player database to export.

By hand (full package)
----------------------
If you prefer to unpack the files yourself, extract
FMViewExporter-<version>-komplettpaket.zip into the folder that contains fm.exe
(Steam: right-click the game > Manage > Browse local files). winhttp.dll and
the BepInEx folder must sit right next to fm.exe. You still need the launch
option from step 3.

Commands
--------
  fmviewexporter-setup locate       Show the FM27 folders found in the Steam libraries
  fmviewexporter-setup install      Install BepInEx and the plugin
  fmviewexporter-setup update       Update the plugin (BepInEx stays if it is usable)
  fmviewexporter-setup uninstall    Remove the plugin, its settings and BepInEx
  fmviewexporter-setup status       Show what is installed
  fmviewexporter-setup settings get <Section.Key>
  fmviewexporter-setup settings set <Section.Key> <value>

Options: --game DIR (folder with fm.exe), --package ZIP (a Komplettpaket to use
instead of the built-in one), --yes (do not ask).

Keys
----
F9   exports the visible player list (squad, player database or shortlist)
     as CSV and HTML. The plugin scrolls through the list by itself; please
     don't click anything until "Export fertig" appears in the BepInEx log.
F10  writes a dump of the user interface to a Diagnose folder.

Settings: run "fmviewexporter-setup settings set Export.FilePrefix fc-bayern"
or edit BepInEx/config/de.electrohugin.fmviewexporter.cfg in the game folder
(created on the first start).

Where the files land
--------------------
The plugin and its settings sit in the game folder, next to fm.exe. The
exports are written to the Documents folder inside the Proton prefix, for
example:

  ~/.local/share/Steam/steamapps/compatdata/<appid>/pfx/drive_c/users/steamuser/Documents/Sports Interactive/FM View Exporter

"fmviewexporter-setup status" prints the exact path for your installation. If
you prefer the exports in your own Documents folder, set Export.OutputFolder
to a Windows path, for example:

  Z:\home\yourname\Documents\FM View Exporter

The plugin runs as a Windows program inside Wine, so a Linux path like
/home/yourname/Documents would end up inside the prefix as
C:\home\yourname\Documents. Proton maps Z: to the Linux root.

Uninstall
---------
Run "fmviewexporter-setup uninstall". It removes the plugin, its settings and
BepInEx, but leaves other mods and your saves alone. By hand: delete
winhttp.dll in the game folder and the game starts without mods. Completely:
also delete doorstop_config.ini, .doorstop_version, changelog.txt, the two
"FM View Exporter - ..." text files and the folders BepInEx and dotnet.
