# Changelog

## Unveroeffentlicht

- Neues Setup-Programm `FMViewExporter-<version>-setup.exe` (Deutsch/Englisch): findet FM27 in den
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
