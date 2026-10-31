# The "Analyzer" view for FM27

Ready-made FM27 views that contain exactly the columns the
[FM Player Analyzer](https://github.com/ElectroHugin/FM-Player-Analyzer) reads:
identity, contract basics, the assistant's opinion and all 47 attributes.
Each has 69 columns, far below the roughly 255 columns FM27 fills with values.

## Use the ready-made views

FM27 keeps separate views per screen: a view saved in the squad screen cannot be used in the
player database or the shortlists, and vice versa. That is why there is one file per screen, with the same columns.

| Screen | File | Name in the game |
| --- | --- | --- |
| Squad | [`views/fm27/FM27-PlayerAnalyzer-Squad-Standard.fmf`](../views/fm27/FM27-PlayerAnalyzer-Squad-Standard.fmf) | *Player Analyzer – Squad Standard* |
| Recruitment > Player Database | [`views/fm27/FM27-PlayerAnalyzer-PlayerDatabase-Standard.fmf`](../views/fm27/FM27-PlayerAnalyzer-PlayerDatabase-Standard.fmf) | *Player Analyzer – Player Database Standard* |
| Recruitment > Shortlists | [`views/fm27/FM27-PlayerAnalyzer-Shortlist-Standard.fmf`](../views/fm27/FM27-PlayerAnalyzer-Shortlist-Standard.fmf) | *Player Analyzer – Shortlist Standard* |

1. Download the file for the screen you want.
2. In FM27 open that screen, open the view menu of the table and choose *Import View…*.
3. Select the view and press **F9**.

## Build it yourself

Start from any view, remove the columns you don't need and insert the ones below with
*Insert Column*. `Player` is always part of the table and needs no menu entry. The menu only
lists columns that are not in the view yet, so a column that seems to be missing is usually
already there.

The order inside the view does not matter to the Analyzer; it finds every column by its
header. Keep the list below anyway, so every export looks the same.

### General, club and contract

| Menu path | Menu entry | Header in the table |
| --- | --- | --- |
| General | Unique ID | `Unique ID` |
| General | Position | `Position` |
| Ability and Roles | Assistant manager's opinion of this player's best position | `Best Pos` |
| General | Age | `Age` |
| Club | Club | `Club` |
| General | Nationality | `Nation` |
| General | Second Nationality | `2nd Nat` |
| General | Height | `Height` |
| General | Preferred Foot | `Preferred Foot` |
| General | Left Foot | `Left Foot` |
| General | Right Foot | `Right Foot` |
| General | Personality | `Personality` |
| General | Media Handling Style | `Media Handling` |
| Transfer | Agreed Playing Time | `Playing Time` |
| Contract > General | Wage | `Wage` |
| Transfer | Transfer Value | `Transfer Value` |
| Stats (General) > General | Average Rating | `Rating` |
| General | Player Status | `Inf` |
| Ability and Roles | Assistant manager's opinion of this player's best tactical role | `Best Role` |
| Ability and Roles | Assistant manager's opinion of this player's current ability | `Ability` |
| Ability and Roles | Assistant manager's opinion of this player's potential ability | `Potential` |

### Player Database and Shortlist differences

The column menu of the Player Database differs slightly from the squad screen: *General* is
called *General Info* there, *Media Handling Style* is just *Media Handling*, and
*Stats (General) > General* is *Stats > General* (e.g. *Average Rating*). Most headers in the table are the same; the player column is
called `Name` instead of `Player`.

**`Personality` is not offered in the Player Database or the Shortlists.** Those views therefore have no
Personality column; the Analyzer treats it as unknown for those players.

### Player Database and Shortlist only

| Column in the game | Header in the export | Note |
| --- | --- | --- |
| Rec. (Player Database) | `Rec.` | Your scouts' grade from `A+` to `F`, empty when unknown. Kept for scouting; the Analyzer ignores it for now. |
| Recommendation (Shortlist) | `Recommendation` | The same grade with its label, e.g. `B+ Superb` or `D Last Resort`. Kept for scouting; the Analyzer ignores it for now. |
| Player's Interest (Shortlist) | `Player's Interest` | How interested the player is in joining; often empty. Kept for transfers; the Analyzer ignores it for now. |

### Attributes

Every entry of these four menus, 47 in total. Menu entry and header are the same except where
noted.

| Menu path | Entries |
| --- | --- |
| Attributes > Goalkeeping | Aerial Reach, Command Of Area, Communication, Eccentricity, Handling, Kicking, One on Ones (`One On Ones`), Punching (Tendency) (`Punching`), Reflexes, Rushing out (Tendency) (`Rushing Out (Tendency)`), Throwing |
| Attributes > Mental | Aggression, Anticipation, Bravery, Composure, Concentration, Decisions, Determination, Flair, Leadership, Off The Ball, Positioning, Teamwork (`Team Work`), Vision, Work Rate |
| Attributes > Physical | Acceleration, Agility, Balance, Jumping Reach, Natural Fitness, Pace, Stamina, Strength |
| Attributes > Technical | Corners, Crossing, Dribbling, Finishing, First Touch, Free Kicks (`Free Kick Taking`), Heading, Long Shots, Long Throws, Marking, Passing, Penalty Taking, Tackling, Technique |

### Save and share it

Build the view once in each screen. Name it after the screen, then open the view menu, choose
*Export Current View…* and save it under the matching file name from the table above.

Naming scheme for all views shipped with the exporter:
`FM27-PlayerAnalyzer-<Screen>-<Preset>.fmf`. `<Screen>` is `Squad`, `PlayerDatabase` or `Shortlist`, `<Preset>` is
`Standard`, `Full` or `Stats` for the presets "Analyzer", "Analyzer-Voll" and "Statistik".

## Notes

- The column list is defined by the preset "Analyzer" in the shared interface note
  (`docs/fm27-schnittstelle.md` in the Analyzer repository). If the preset changes, this page and
  the `.fmf` file change with it.
- The Analyzer reads FM27 files through its `fm27` import profile (see `docs/IMPORT-FM27.md` in
  the Analyzer repository). Columns it does not know are ignored.
- FM24 had a `Reg` (registration) column. FM27 has none, so the Analyzer turns registration off
  for FM27.
- The meaning of every column is listed in the [column reference](SPALTEN-FM27.md), every menu
  entry in [fm27-menu.csv](columns/fm27-menu.csv).
