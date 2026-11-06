# Changelog

## 1.0.1 (noch nicht veroeffentlicht)

- Sterne bei Ability und Potential: graue Sterne (unsichere Einschaetzung der Scouts) zaehlen jetzt
  mit, so wie das Spiel sie zeigt. Beispiel: 1 gelber + 1 grauer Stern = `2.0` statt `1.0`.
  Betrifft vor allem gescoutete Spieler in Spieler-Datenbank und Shortlist (Hinweis aus dem
  FM Player Analyzer).
- Schnellerer Export: Ob die Zeilen nach einem Scrollschritt fertig sind, prueft das Plugin nur
  noch ueber Position, Unique ID und letzte Zelle; alle Zellen werden danach genau einmal gelesen
  (vorher mindestens zweimal). Weniger teure Zugriffe aufs Spiel je Zelle, Scrollschritte von 95 %
  statt 80 % der sichtbaren Hoehe (mit Sicherheitsnetz: ohne Ueberlappung wird kleiner
  nachgescrollt) und `Export.FramesPerStep` jetzt standardmaessig `1` statt `3`. Bestehende
  Einstellungsdateien behalten ihren Wert; wer `3` eingetragen hat, kann im Setup auf `1` stellen.
- Neue Einstellung `Export.RowsPerStep` (Standard 200): Die Liste wird waehrend des Exports kurz
  vergroessert, damit das Spiel pro Scrollschritt viel mehr Zeilen zeichnet und weniger Schritte
  noetig sind. `0` schaltet das ab.
- Behoben: Jede Wartezeit dauerte einen Frame laenger als eingestellt.
- Waehrend des Exports wird die Bildraten-Begrenzung des Spiels kurz aufgehoben (bis 240 Bilder/s)
  und danach wiederhergestellt. Es wird nichts gespeichert.
- Das Log nennt am Ende die Zeitaufteilung (Scrollschritte, Frames, Lesezeit), damit sich weitere
  Beschleunigungen gezielt messen lassen.

- Deinstallieren: Fremde Patcher unter `BepInEx\patchers` zaehlen jetzt wie fremde Plugins.
  Ist einer vorhanden, bleibt BepInEx stehen; entfernt werden nur Dateien und Einstellungen des
  FM View Exporters (vorher konnte der ganze BepInEx-Ordner samt fremdem Patcher geloescht werden).
- Setup: Ein vorhandenes, vollstaendiges BepInEx (gleicher oder neuerer Build als #755, oder eine
  nicht bestimmbare Version etwa von einer anderen Mod) bleibt beim Installieren/Aktualisieren
  unangetastet, es wird nur das Plugin geschrieben. Fehlt BepInEx, ist es unvollstaendig oder aelter,
  installiert bzw. repariert das Setup es wie bisher. Eine vorhandene `doorstop_config.ini` wird nie
  ueberschrieben.
- Keine doppelten Zeilen mehr, wenn die Unique ID oder andere Werte erst nach dem ersten Lesen
  erscheinen: Zeilen werden zusaetzlich ueber ihre Position in der Liste zugeordnet (nur wenn sich die
  Werte nicht widersprechen; Namen allein fuehren nie Zeilen zusammen). Bleiben Zeilen ohne Unique ID
  uebrig, die doppelt sein koennten, steht eine Warnung im Log.
- HTML-Sortierung: Geldbetraege mit K/M/B (`€900K` < `€1M`), Tausendertrennzeichen (`950` < `1,200`),
  Einheiten (`175 cm`) und Spannen (`10-12`, `€1M - €2M`, nach unterem, dann oberem Wert) sortieren
  jetzt richtig. Die Sortierwerte werden beim Export berechnet; der sichtbare Text bleibt gleich.
  Leere Zellen stehen immer am Ende, Text wird natuerlich sortiert (`Spieler 2` vor `Spieler 10`).

## 1.0.0 (31.10.2026)

Erste vollstaendige Version fuer Football Manager 27: F9 exportiert Kader, Spieler-Datenbank und
Shortlist als CSV und HTML, das Setup installiert und konfiguriert alles. Getestet am 31.10.2026
mit FM27 und BepInEx BE #755: Deinstallieren, Neuinstallation ueber das Setup, Exporte aller drei
Ansichten mit Praefix und Zeitstempel in beiden Formaten. Das Setup ist noch nicht signiert.

- Release-Notes auf Englisch mit Kurzbeschreibung; englische Anleitung `FM View Exporter - README.txt`
  in den Paketen (neben der deutschen LIESMICH). Das Setup zeigt die Version ohne fuehrendes `v`.

- Neues Setup-Programm `FMViewExporter-<version>-setup.exe` (Englisch, oben rechts auf Deutsch umschaltbar): findet FM27 in den
  Steam-Bibliotheken, installiert BepInEx und das Plugin, aktualisiert ohne die Einstellungen zu
  verlieren, deinstalliert (BepInEx bleibt, wenn weitere Mods da sind) und bearbeitet alle
  Einstellungen in einem Formular. Drei Reiter (Installation, Einstellungen, Über) mit
  „Speichern" immer sichtbar; passt sich der Windows-Skalierung an (125 %, 150 %).
- Neue Einstellungen `Export.FilePrefix` (z. B. `fc-bayern` -> `fc-bayern-squad.csv`),
  `Export.Timestamp` (false = fester Dateiname, wird ueberschrieben) und `Export.Formats`
  (Both, CsvOnly, HtmlOnly).
- Exporte aus der Spieler-Datenbank heissen jetzt `player-database_...` statt `players_...`.
- Fertige Analyzer-Ansichten (.fmf) fuer Kader, Spieler-Datenbank und Shortlist unter `views/fm27/`.
- F9 exportiert jetzt auch die Shortlist (Tabelle #playertableshortlist, Dateiname shortlist_...).
  Zusaetzlich Rueckfall ueber den Aufbau der Tabelle (#column-headers plus Zeilenliste),
  falls eine Ansicht ihre Tabelle anders benennt.
- Shortlist-Exporte hiessen faelschlich `player-database`: FM27 legt einen Rahmen #PlayerTable um
  #playertableshortlist. Der Ansichtsname kommt jetzt auch aus inneren Tabellennamen und aus den
  Bereichen #RecruitmentShortlistTool (shortlist) und #PlayerSearchReport (player-database).
  Der HTML-Titel nennt dieselbe Ansicht wie der Dateiname.
- Code signing policy im README; die CI signiert das Setup bei Releases ueber SignPath, sobald das
  Projekt dort aufgenommen ist (Anleitung `docs/CODE-SIGNING.md`).
- Wird keine Tabelle gefunden, schreibt das Log Diagnose-Hinweise, wo Kopf- und Tabellenzeilen liegen.

## 0.0.2-test.1 (Testversion)

- F9 exportiert die Spielertabelle (#playertable) als CSV und HTML: alle Spalten mit den
  Koepfen wie im Spiel, inklusive "Unique ID". Rich-Text (Farben, Links) wird entfernt,
  Sterne (Ability/Potential) werden als Zahl geschrieben.
- Das Plugin scrollt die Liste selbst durch und fuehrt doppelte Zeilen ueber "Unique ID" zusammen.
- Neue Einstellungen `Export.ScanColumns` und `Export.FramesPerStep`.
- Einstellungen werden bei jedem F9 neu eingelesen (kein Neustart noetig).
- CSV-Stil German schreibt Dezimalzahlen mit Komma.
- F10: Zahlenwerte (z. B. Anzahl der Listeneintraege) werden jetzt richtig angezeigt.

## 0.0.1-erkundung.1 (Testversion)

- Grundgeruest: BepInEx-6-Plugin fuer FM27 (Unity 6000.3, IL2CPP).
- F10 schreibt ein Abbild der Oberflaeche nach `Dokumente\Sports Interactive\FM View Exporter\Diagnose`.
- Einstellungen: Tasten, CSV-Stil (Standard Englisch: Komma), Ausgabeordner.
- CSV- und HTML-Schreiber mit Tests (noch nicht an eine Taste gebunden).
- Komplettpaket mit BepInEx #755.
