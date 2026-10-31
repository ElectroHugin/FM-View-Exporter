# The "Analyzer" view for FM27

A ready-made FM27 view that contains exactly the columns the
[FM Player Analyzer](https://github.com/ElectroHugin/FM-Player-Analyzer) reads:
identity, contract basics, the assistant's opinion and all 47 attributes.
It has 69 columns, far below the roughly 255 columns FM27 fills with values.

## Use the ready-made view

1. Download [`views/fm27/FM27-PlayerAnalyzer-Standard.fmf`](../views/fm27/FM27-PlayerAnalyzer-Standard.fmf).
2. In FM27 open your squad or the player search, open the view menu of the table and choose
   *Import View…*. Pick `FM27-PlayerAnalyzer-Standard.fmf`. In the game the view is called *Player Analyzer – Standard*.
3. Select the view and press **F9**. Use the same view for the squad and the player search.

> The `.fmf` file is not in the repository yet. Until it is, build the view yourself as
> described below.

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

Name the view *Player Analyzer – Standard*. Then open the view menu, choose *Export Current View…* and save it as
`FM27-PlayerAnalyzer-Standard.fmf`.

Naming scheme for all views shipped with the exporter: `FM27-PlayerAnalyzer-<Preset>.fmf`, for example
`-Standard`, `-Full` and `-Stats` for the presets "Analyzer", "Analyzer-Voll" and "Statistik".

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
