# Empfohlene Ansicht für den FM Player Analyzer (FM27)

Diese Ansicht enthält genau die Spalten, die der
[FM Player Analyzer](https://github.com/ElectroHugin/FM-Player-Analyzer) braucht, plus alle
Attribute. Mit rund 75 Spalten bleibt sie weit unter der Grenze von etwa 255 Spalten, ab der
FM27 keine Werte mehr anzeigt.

## Ansicht im Spiel anlegen

1. Kader oder Spielersuche öffnen.
2. Über das Ansichten-Menü der Tabelle eine neue, leere Ansicht anlegen und z. B.
   `Analyzer` nennen.
3. Die Spalten unten hinzufügen. Die Reihenfolge ist egal, der Analyzer sucht die Spalten
   über ihren Namen.
4. Ansicht speichern. Danach in Kader und Spielersuche dieselbe Ansicht wählen und **F9**
   drücken.

## Pflichtspalten

Ohne diese beiden Spalten lehnt der Analyzer den Import ab.

| Spalte im Spiel | Feld im Analyzer |
| --- | --- |
| `Unique ID` | UID |
| `Player` | Name |

## Stammdaten

| Spalte im Spiel | Feld im Analyzer |
| --- | --- |
| `Position` | Position |
| `Age` | Age |
| `Club` | Club |
| `Nation` | Nationality |
| `2nd Nat` | Second Nationality |
| `Height` | Height |
| `Preferred Foot` | Preferred Foot |
| `Left Foot` | Left Foot |
| `Right Foot` | Right Foot |
| `Personality` | Personality |
| `Media Handling` | Media Handling |
| `Playing Time` | Agreed Playing Time |
| `Wage` | Wage |
| `Transfer Value` | Transfer Value |
| `Rating` | Average Rating |
| `Inf` | Information |

## Attribute

Alle Attribute der Gruppen *Torwart*, *Mental*, *Physisch* und *Technisch*, also:

- Torwart: `Aerial Reach`, `Command Of Area`, `Communication`, `Eccentricity`, `Handling`,
  `Kicking`, `One On Ones`, `Punching`, `Reflexes`, `Rushing Out (Tendency)`, `Throwing`
- Mental: `Aggression`, `Anticipation`, `Bravery`, `Composure`, `Concentration`,
  `Decisions`, `Determination`, `Flair`, `Leadership`, `Off The Ball`, `Positioning`,
  `Team Work`, `Vision`, `Work Rate`
- Physisch: `Acceleration`, `Agility`, `Balance`, `Jumping Reach`, `Natural Fitness`,
  `Pace`, `Stamina`, `Strength`
- Technisch: `Corners`, `Crossing`, `Dribbling`, `Finishing`, `First Touch`,
  `Free Kick Taking`, `Heading`, `Long Shots`, `Long Throws`, `Marking`, `Passing`,
  `Penalty Taking`, `Tackling`, `Technique`

Bedeutung aller Spalten: [SPALTEN-FM27.md](SPALTEN-FM27.md).

## Hinweise

- FM24 hatte eine Spalte `Reg` (Registrierung). In FM27 gibt es sie nicht; der Analyzer schaltet
  die Registrierung für FM27 deshalb ab.
- Der Analyzer liest FM27-Dateien über sein Profil `fm27`
  ([FM-Player-Analyzer#4](https://github.com/ElectroHugin/FM-Player-Analyzer/pull/4),
  Doku dort in `docs/IMPORT-FM27.md`). Von den beiden `Nation`-Spalten zählt die erste (Kürzel);
  alle anderen Spalten aus dem Spaltenverzeichnis werden still ignoriert.
- Ändern sich Spaltennamen in `docs/columns/fm27.csv`, muss die Zuordnung im Analyzer
  (`ImportSchema.cpp`) mitgezogen werden.
