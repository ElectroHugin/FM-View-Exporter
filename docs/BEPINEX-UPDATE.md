# BepInEx aktualisieren

Das Setup und das Komplettpaket bringen einen fest gewählten BepInEx-Build mit. Welcher das ist,
steht in `packaging/bepinex.env`. So kommt ein neuer Build in ein Release.

## 1. Neuesten Build finden

Alle Builds stehen auf <https://builds.bepinex.dev/projects/bepinex_be>, der neueste ganz oben.
Unter jedem Build steht, was sich geändert hat. Gebraucht wird die Datei

```
BepInEx-Unity.IL2CPP-win-x64-6.0.0-be.<Nummer>+<Kennung>.zip
```

(IL2CPP, win-x64). Den Link der Datei kopieren (Rechtsklick, Link-Adresse kopieren).

## 2. Mit dem Spiel testen

1. Den Spielordner sichern, mindestens die Ordner `BepInEx` und die Datei `winhttp.dll`.
2. FM schließen. Die Zip in den Spielordner entpacken (neben `fm.exe`) und vorhandene Dateien
   ersetzen. Die Ordner `BepInEx\interop` und `BepInEx\unity-libs` löschen, damit BepInEx sie
   für den neuen Build neu erzeugt. `BepInEx\config` und `BepInEx\plugins` bleiben.
3. FM starten. Der erste Start dauert einige Minuten.
4. In `BepInEx\LogOutput.log` prüfen: oben steht die neue Build-Nummer, weiter unten
   `Loading [FM View Exporter ...]` und keine roten Fehler.
5. Kader, Spieler-Datenbank und Shortlist mit F9 exportieren und die Dateien ansehen.

Klappt etwas nicht: FM schließen und die Sicherung zurückspielen.

## 3. Prüfsummen ermitteln

In PowerShell im Spielordner:

```powershell
Get-FileHash .\BepInEx\core\BepInEx.Core.dll, .\winhttp.dll -Algorithm SHA256
```

Die Prüfsumme der Zip selbst rechnet die CI aus und schreibt sie ins Build-Log
(`BepInEx #<Nummer> zip sha256: ...`).

## 4. Ins Projekt übernehmen

In `packaging/bepinex.env` eintragen:

- `BEPINEX_BUILD` = Nummer
- `BEPINEX_URL` = Link aus Schritt 1 (das `+` in der Adresse als `%2B`)
- `BEPINEX_CORE_SHA256` und `WINHTTP_SHA256` = Werte aus Schritt 3
- `BEPINEX_SHA256` = Wert aus dem CI-Log, sobald der erste Build gelaufen ist

Die Referenz-Assemblies in `lib/ref/bepinex-be755` müssen nur erneuert werden, wenn das Plugin
neue Funktionen von BepInEx braucht oder der Build nicht mehr kompiliert
(`./tools/make-refs.sh "<Spielordner>" fm27 be<Nummer>`, siehe `lib/ref/README.md`).

Danach baut die CI wie gewohnt; das nächste Release enthält den neuen Build. Im CHANGELOG
vermerken, mit welchem Build getestet wurde.

## Was Nutzer davon merken

Ein Update über das Setup ersetzt nur das Plugin, wenn BepInEx schon installiert ist. Wer
schon installiert hat, behält also seinen BepInEx-Build; den neuen bekommen Neuinstallationen,
das Komplettpaket und wer über das Setup deinstalliert und neu installiert.
