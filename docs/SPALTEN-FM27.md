# Spaltenverzeichnis FM27

Alle Spalten, die Football Manager 27 in Spielertabellen anbietet, mit Bedeutung und Format.
Die Spaltenköpfe stehen genau so im Export wie im Spiel (englische Spieloberfläche).
Maschinenlesbar: [columns/fm27.csv](columns/fm27.csv). Erzeugt mit `tools/columns_fm27.py`.

Hinweise:

- Das Spiel befüllt nur etwa die ersten 255 Spalten einer Ansicht; alles dahinter bleibt auch im Spiel leer.
  Ansichten für den Export deshalb schlank halten.
- Einige Köpfe kommen doppelt vor (`Nation`, `Expires`, `Defending - Technical`, `Attacking - Tactical`).
  Die Spalten bleiben im Export in der Reihenfolge des Spiels.
- Sterne (`Ability`, `Potential`, `Role Ability`) werden als Zahl von 0.0 bis 5.0 exportiert.
- Spalten, die im Spiel nur ein Symbol zeigen (z. B. `Morale`, `Happiness`), sind im Export leer.
- `(?)` = Bedeutung noch nicht im Spiel bestätigt.
- Im Spiel heißen die Spalten im Menü *Insert Column* oft anders als im Tabellenkopf
  (z. B. *Player Of The Match* → `PoM`). Zuordnung: [columns/fm27-menu.csv](columns/fm27-menu.csv).
- Spalte *Analyzer* = Feld im [FM Player Analyzer](https://github.com/ElectroHugin/FM-Player-Analyzer).

## Basis

| Spalte | Bedeutung | Meaning | Format | Analyzer |
| --- | --- | --- | --- | --- |
| `Unique ID` | Eindeutige Spieler-ID im Spielstand | Unique player id in the save | Zahl, 8718372 | Unique ID (Pflicht) |
| `Player` | Name | Name | Text, Manuel Neuer | Name (Pflicht) |
| `Position` | Positionen | Positions | Text, GK / D (RC), DM | Position |
| `Best Pos` | Beste Position | Best position | Text, GK |  |
| `Transfer Value` | Marktwert (Spanne, solange nicht voll gescoutet) | Transfer value (range unless fully scouted) | Text, €1.5M - €3.1M | Transfer Value |
| `Best Role` | Beste Rolle (Kürzel) | Best role (abbreviation) | Text, BGK |  |
| `Age` | Alter | Age | Zahl, 40 | Age |
| `Playing Time` | Vereinbarte Einsatzzeit | Agreed playing time | Text, First-Choice Goalkeeper | Agreed Playing Time |
| `Nation` | Nationalität (Kürzel). Kommt zweimal vor; die zweite Spalte ist der volle Name | Nationality (code). Appears twice; the second column holds the full name | Text, GER / Germany | Nationality |
| `Wage` | Gehalt | Wage | Text, €300K p/w | Wage |
| `Division` | Liga | Division | Text, Bundesliga |  |
| `Club` | Verein | Club | Text, FC Bayern | Club |
| `Based In` | Land des Vereins | Country the club is based in | Text, Germany |  |
| `Prom.` | Versprochene Rückennummer (Menü: Promised Squad Number) (?) | Promised squad number (?) | Zahl |  |
| `Pref.` | Wunsch-Rückennummer (Menü: Preferred Squad Number) (?) | Preferred squad number (?) | Zahl |  |
| `Inf` | Spielerstatus/Info-Symbole (Verletzung, Sperre, Transferliste ...) (Menü: Player Status (?)) | Info flags (injury, ban, transfer list ...) | Text/Kürzel | Information |
| `Sec. Position` | Nebenposition | Secondary position | Text |  |
| `No.` | Rückennummer | Squad number | Zahl |  |

## Attribute Torwart

| Spalte | Bedeutung | Meaning | Format | Analyzer |
| --- | --- | --- | --- | --- |
| `Aerial Reach` | Lufthoheit | Aerial reach | 1-20 (gescoutet: Spanne) | Aerial Reach |
| `Eccentricity` | Exzentrik | Eccentricity | 1-20 |  |
| `Communication` | Kommunikation | Communication | 1-20 |  |
| `Command Of Area` | Strafraumbeherrschung | Command of area | 1-20 | Command of Area |
| `Handling` | Fangsicherheit | Handling | 1-20 | Handling |
| `Kicking` | Abschlag | Kicking | 1-20 | Kicking |
| `One On Ones` | Eins gegen eins | One on ones | 1-20 | One vs One |
| `Reflexes` | Reflexe | Reflexes | 1-20 | Reflexes |
| `Punching` | Fausten (Neigung) | Punching (tendency) | 1-20 |  |
| `Rushing Out (Tendency)` | Herauslaufen (Neigung) | Rushing out (tendency) | 1-20 | Rushing Out (Tendency) |
| `Throwing` | Abwurf | Throwing | 1-20 | Throwing |

## Attribute mental

| Spalte | Bedeutung | Meaning | Format | Analyzer |
| --- | --- | --- | --- | --- |
| `Aggression` | Aggressivität | Aggression | 1-20 | Aggression |
| `Anticipation` | Antizipation | Anticipation | 1-20 | Anticipation |
| `Bravery` | Mut | Bravery | 1-20 | Bravery |
| `Composure` | Nervenstärke | Composure | 1-20 | Composure |
| `Concentration` | Konzentration | Concentration | 1-20 | Concentration |
| `Determination` | Entschlossenheit | Determination | 1-20 | Determination |
| `Decisions` | Entscheidungen | Decisions | 1-20 | Decisions |
| `Flair` | Kreativität/Flair | Flair | 1-20 | Flair |
| `Leadership` | Führungsqualität | Leadership | 1-20 | Leadership |
| `Off The Ball` | Ohne Ball | Off the ball | 1-20 | Off the Ball |
| `Positioning` | Stellungsspiel | Positioning | 1-20 | Positioning |
| `Team Work` | Teamwork | Teamwork | 1-20 | Teamwork |
| `Vision` | Übersicht | Vision | 1-20 | Vision |
| `Work Rate` | Laufbereitschaft | Work rate | 1-20 | Work Rate |

## Attribute physisch

| Spalte | Bedeutung | Meaning | Format | Analyzer |
| --- | --- | --- | --- | --- |
| `Acceleration` | Antritt | Acceleration | 1-20 | Acceleration |
| `Agility` | Beweglichkeit | Agility | 1-20 | Agility |
| `Balance` | Balance | Balance | 1-20 | Balance |
| `Jumping Reach` | Sprungkraft | Jumping reach | 1-20 | Jumping Reach |
| `Natural Fitness` | Grundfitness | Natural fitness | 1-20 |  |
| `Pace` | Schnelligkeit | Pace | 1-20 | Pace |
| `Stamina` | Ausdauer | Stamina | 1-20 | Stamina |
| `Strength` | Kraft | Strength | 1-20 | Strength |

## Attribute technisch

| Spalte | Bedeutung | Meaning | Format | Analyzer |
| --- | --- | --- | --- | --- |
| `Corners` | Ecken | Corners | 1-20 | Corners |
| `Crossing` | Flanken | Crossing | 1-20 | Crossing |
| `Dribbling` | Dribbling | Dribbling | 1-20 | Dribbling |
| `Finishing` | Abschluss | Finishing | 1-20 | Finishing |
| `First Touch` | Ballannahme | First touch | 1-20 | First Touch |
| `Free Kick Taking` | Freistöße | Free kick taking | 1-20 |  |
| `Heading` | Kopfball | Heading | 1-20 | Heading |
| `Long Shots` | Distanzschüsse | Long shots | 1-20 | Long Shots |
| `Long Throws` | Weite Einwürfe | Long throws | 1-20 |  |
| `Marking` | Manndeckung | Marking | 1-20 | Marking |
| `Passing` | Passen | Passing | 1-20 | Passing |
| `Penalty Taking` | Elfmeter | Penalty taking | 1-20 |  |
| `Tackling` | Zweikampf/Tackling | Tackling | 1-20 | Tackling |
| `Technique` | Technik | Technique | 1-20 | Technique |

## Person

| Spalte | Bedeutung | Meaning | Format | Analyzer |
| --- | --- | --- | --- | --- |
| `Height` | Größe | Height | Text, 193 cm | Height |
| `Home-Grown Status` | Eigengewächs-Status | Home-grown status | Text |  |
| `Media Description` | Mediale Beschreibung | Media description | Text, World class goalkeeper |  |
| `Media Handling` | Umgang mit Medien | Media handling | Text, Unflappable | Media Handling |
| `Nation` | Nationalität (voller Name, zweite Nation-Spalte) | Nationality (full name, second Nation column) | Text, Germany |  |
| `Personality` | Persönlichkeit | Personality | Text, Driven | Personality |
| `Recent Changes (Last 2 Weeks)` | Attributänderungen der letzten zwei Wochen | Attribute changes in the last two weeks | Text |  |
| `2nd Nat` | Zweite Nationalität | Second nationality | Text, Guinea-Bissau | Second Nationality |
| `Right Foot` | Stärke rechter Fuß | Right foot | Text, Very Strong | Right Foot |
| `Birth Region` | Geburtsregion | Birth region | Text |  |
| `Preferred Foot` | Starker Fuß | Preferred foot | Text, Right | Preferred Foot |
| `Nation of Birth` | Geburtsland | Nation of birth | Text |  |
| `Left Foot` | Stärke linker Fuß | Left foot | Text, Weak | Left Foot |
| `Favoured Personnel` | Bevorzugte Personen | Favoured personnel | Text |  |
| `Favoured Clubs` | Lieblingsvereine | Favoured clubs | Text |  |
| `EU National` | EU-Bürger | EU national | Yes/No |  |
| `Eligible` | Für den Verein spielberechtigt (Menü: Eligible to Sign for Club) | Eligible to sign for club | Text |  |
| `DoB` | Geburtsdatum | Date of birth | Datum |  |
| `World Reputation` | Weltweites Ansehen | World reputation | Text, Unknown |  |
| `Player Trait` | Spielereigenschaften (Traits) | Player traits | Text |  |

## Zufriedenheit

| Spalte | Bedeutung | Meaning | Format | Analyzer |
| --- | --- | --- | --- | --- |
| `Playing Time Happiness` | Zufriedenheit mit der Einsatzzeit | Happiness with playing time | Text, Satisfied |  |
| `Hierarchy` | Stellung in der Kaderhierarchie | Squad hierarchy status | Text |  |
| `Happiness` | Zufriedenheit | Happiness | Symbol (kein Text) |  |
| `Social Groups` | Soziale Gruppe im Kader | Social group | Text |  |
| `Short-term Plans` | Kurzfristige Pläne | Short-term plans | Text |  |
| `Promises` | Versprechen | Promises | Text |  |
| `Overall Happiness` | Gesamtzufriedenheit | Overall happiness | Symbol (kein Text) |  |
| `Morale` | Moral | Morale | Symbol (kein Text) |  |
| `Long-Term Plans` | Langfristige Pläne | Long-term plans | Text |  |
| `International Happiness` | Zufriedenheit in der Nationalmannschaft | International happiness | Text |  |
| `Manager Tendencies` | Trainerneigungen | Manager tendencies | Text |  |
| `Teammates and Staff Relationships` | Beziehungen zu Mitspielern und Stab | Teammates and staff relationships | Text |  |
| `Managerial Support` | Rückhalt für den Trainer | Managerial support | Text |  |
| `Actual Playing Time` | Tatsächliche Einsatzzeit | Actual playing time | Text |  |

## Statistik Angriff

| Spalte | Bedeutung | Meaning | Format | Analyzer |
| --- | --- | --- | --- | --- |
| `ShT` | Schüsse aufs Tor | Shots on target | Zahl |  |
| `Shot/90` | Schüsse pro 90 Min. | Shots per 90 | Dezimal |  |
| `Shot %` | Anteil Schüsse aufs Tor | Shots on target ratio | Prozent |  |
| `ShT/90` | Schüsse aufs Tor pro 90 Min. | Shots on target per 90 | Dezimal |  |
| `Shots From Outside The Box Per 90 minutes` | Schüsse von außerhalb des Strafraums pro 90 Min. | Shots from outside the box per 90 | Dezimal |  |
| `Shots` | Schüsse | Shots | Zahl |  |
| `Goals From Outside The Box` | Tore von außerhalb des Strafraums | Goals from outside the box | Zahl |  |
| `Free Kick Shots` | Freistoßschüsse | Free kick shots | Zahl |  |
| `xG/shot` | Erwartete Tore pro Schuss | Expected goals per shot | Dezimal |  |
| `Conv %` | Chancenverwertung (Tore pro Schuss) | Conversion rate | Prozent |  |
| `Asts/90` | Vorlagen pro 90 Min. | Assists per 90 | Dezimal |  |
| `Off` | Abseits | Offsides | Zahl |  |
| `Drb/90` | Erfolgreiche Dribblings pro 90 Min. | Dribbles made per 90 | Dezimal |  |
| `Drb` | Erfolgreiche Dribblings | Dribbles made | Zahl |  |
| `xG` | Erwartete Tore | Expected goals | Dezimal |  |
| `Pen/R` | Elfmeterquote | Penalty conversion ratio | Prozent |  |
| `Pens S` | Verwandelte Elfmeter | Penalties scored | Zahl |  |
| `Pens` | Geschossene Elfmeter | Penalties taken | Zahl |  |
| `NP-xG/90` | xG ohne Elfmeter pro 90 Min. | Non-penalty xG per 90 | Dezimal |  |
| `NP-xG` | xG ohne Elfmeter | Non-penalty xG | Dezimal |  |
| `Minutes Since Last Goal` | Minuten seit dem letzten Tor | Minutes since last goal | Zahl |  |
| `Goals per 90 minutes` | Tore pro 90 Min. | Goals per 90 | Dezimal |  |
| `Goals` | Tore | Goals | Zahl |  |
| `xG/90` | Erwartete Tore pro 90 Min. | Expected goals per 90 | Dezimal |  |
| `xG-OP` | Tore minus xG (Überperformance) | Expected goals overperformance | Dezimal |  |
| `Mins/Gl` | Minuten pro Tor | Minutes per goal | Zahl |  |
| `Assists` | Vorlagen | Assists | Zahl |  |

## Statistik Verteidigung

| Spalte | Bedeutung | Meaning | Format | Analyzer |
| --- | --- | --- | --- | --- |
| `Tck/90` | Tacklings pro 90 Min. | Tackles per 90 | Dezimal |  |
| `K Hdrs/90` | Wichtige gewonnene Kopfbälle pro 90 Min. | Key headers per 90 | Dezimal |  |
| `Hdr %` | Kopfballquote | Header win ratio | Prozent |  |
| `Hdrs W/90` | Gewonnene Kopfbälle pro 90 Min. | Headers won per 90 | Dezimal |  |
| `Hdrs` | Gewonnene Kopfbälle | Headers won | Zahl |  |
| `Hdrs L/90` | Verlorene Kopfbälle pro 90 Min. | Headers lost per 90 | Dezimal |  |
| `Aer A/90` | Kopfballduelle pro 90 Min. | Aerial challenges attempted per 90 | Dezimal |  |
| `Hdrs A` | Kopfballduelle | Headers attempted | Zahl |  |
| `Fouls` | Begangene Fouls | Fouls made | Zahl |  |
| `K Tck/90` | Wichtige Tacklings pro 90 Min. | Key tackles per 90 | Dezimal |  |
| `K Tck` | Wichtige Tacklings | Key tackles | Zahl |  |
| `Int/90` | Abgefangene Bälle pro 90 Min. | Interceptions per 90 | Dezimal |  |
| `Itc` | Abgefangene Bälle | Interceptions | Zahl |  |
| `Clr/90` | Klärende Aktionen pro 90 Min. | Clearances per 90 | Dezimal |  |
| `Clearances` | Klärende Aktionen | Clearances | Zahl |  |
| `Blk/90` | Geblockte Bälle pro 90 Min. | Blocks per 90 | Dezimal |  |
| `Poss Won/90` | Ballgewinne pro 90 Min. | Possession won per 90 | Dezimal |  |
| `Pres A` | Pressingversuche | Pressures attempted | Zahl |  |
| `Pres A/90` | Pressingversuche pro 90 Min. | Pressures attempted per 90 | Dezimal |  |
| `Pres C` | Erfolgreiche Pressingaktionen | Pressures completed | Zahl |  |
| `Pres C/90` | Erfolgreiche Pressingaktionen pro 90 Min. | Pressures completed per 90 | Dezimal |  |
| `Shts Blckd` | Geblockte Schüsse | Shots blocked | Zahl |  |
| `Shts Blckd/90` | Geblockte Schüsse pro 90 Min. | Shots blocked per 90 | Dezimal |  |
| `Tck R` | Tackling-Quote | Tackle success ratio | Prozent |  |
| `Tck A` | Tackling-Versuche | Tackles attempted | Zahl |  |
| `Tck C` | Erfolgreiche Tacklings | Tackles completed | Zahl |  |
| `Blk` | Geblockte Bälle | Blocks | Zahl |  |

## Statistik Torwart

| Spalte | Bedeutung | Meaning | Format | Analyzer |
| --- | --- | --- | --- | --- |
| `Svt` | Paraden, Ball über die Latte/ums Tor gelenkt (Menü: Saves Tipped) | Saves tipped | Zahl |  |
| `Svp` | Paraden, Ball abgewehrt | Saves parried | Zahl |  |
| `Svh` | Paraden, Ball festgehalten (Menü: Saves Held) | Saves held | Zahl |  |
| `Sv %` | Paradenquote | Save percentage | Prozent |  |
| `xSv %` | Erwartete Paradenquote | Expected save percentage | Prozent |  |
| `xGP/90` | Verhinderte Tore (xG) pro 90 Min. | Expected goals prevented per 90 | Dezimal |  |
| `xGP` | Verhinderte Tore (xG) | Expected goals prevented | Dezimal |  |
| `Saves/90` | Paraden pro 90 Min. | Saves per 90 | Dezimal |  |
| `Pens Saved Ratio` | Quote gehaltener Elfmeter | Penalties saved ratio | Prozent |  |
| `Pens Saved` | Gehaltene Elfmeter | Penalties saved | Zahl |  |
| `Pens Faced` | Elfmeter gegen sich | Penalties faced | Zahl |  |
| `Minutes Since Last Conceded` | Minuten seit dem letzten Gegentor | Minutes since last goal conceded | Zahl |  |
| `Goals Conceded` | Gegentore | Goals conceded | Zahl |  |
| `Con/90` | Gegentore pro 90 Min. | Goals conceded per 90 | Dezimal |  |
| `Cln/90` | Zu-null-Spiele pro 90 Min. | Clean sheets per 90 | Dezimal |  |
| `Clean Sheets` | Zu-null-Spiele | Clean sheets | Zahl |  |

## Statistik Passspiel

| Spalte | Bedeutung | Meaning | Format | Analyzer |
| --- | --- | --- | --- | --- |
| `Pr passes/90` | Raumgewinnende Pässe pro 90 Min. | Progressive passes per 90 | Dezimal |  |
| `PsP` | Raumgewinnende Pässe | Progressive passes | Zahl |  |
| `Poss Lost/90` | Ballverluste pro 90 Min. | Possession lost per 90 | Dezimal |  |
| `Ps C/90` | Angekommene Pässe pro 90 Min. | Passes completed per 90 | Dezimal |  |
| `Ps C` | Angekommene Pässe | Passes completed | Zahl |  |
| `Ps A/90` | Passversuche pro 90 Min. | Passes attempted per 90 | Dezimal |  |
| `Pas A` | Passversuche | Passes attempted | Zahl |  |
| `Pas %` | Passquote | Pass completion ratio | Prozent |  |
| `OP-KP/90` | Schlüsselpässe aus dem Spiel pro 90 Min. | Open-play key passes per 90 | Dezimal |  |
| `KP/90` | Schlüsselpässe pro 90 Min. | Key passes per 90 | Dezimal |  |
| `Key` | Schlüsselpässe | Key passes | Zahl |  |
| `CCC` | Großchancen kreiert | Clear-cut chances created | Zahl |  |
| `Ch C/90` | Kreierte Chancen pro 90 Min. | Chances created per 90 | Dezimal |  |
| `OP-Crs C/90` | Angekommene Flanken aus dem Spiel pro 90 Min. | Open-play crosses completed per 90 | Dezimal |  |
| `OP-Crs C` | Angekommene Flanken aus dem Spiel | Open-play crosses completed | Zahl |  |
| `OP-Crs A/90` | Flankenversuche aus dem Spiel pro 90 Min. | Open-play crosses attempted per 90 | Dezimal |  |
| `OP-Crs A` | Flankenversuche aus dem Spiel | Open-play crosses attempted | Zahl |  |
| `OP-Cr %` | Flankenquote aus dem Spiel | Open-play cross completion ratio | Prozent |  |
| `Cr C/90` | Angekommene Flanken pro 90 Min. | Crosses completed per 90 | Dezimal |  |
| `Cr C` | Angekommene Flanken | Crosses completed | Zahl |  |
| `Crs A/90` | Flankenversuche pro 90 Min. | Crosses attempted per 90 | Dezimal |  |
| `Cr A` | Flankenversuche | Crosses attempted | Zahl |  |
| `Cr C/A` | Flankenquote | Cross completion ratio | Prozent |  |
| `xA/90` | Erwartete Vorlagen pro 90 Min. | Expected assists per 90 | Dezimal |  |
| `xA` | Erwartete Vorlagen | Expected assists | Dezimal |  |

## Statistik allgemein

| Spalte | Bedeutung | Meaning | Format | Analyzer |
| --- | --- | --- | --- | --- |
| `Sprints/90` | Sprints pro 90 Min. | Sprints per 90 | Dezimal |  |
| `Dist/90` | Laufdistanz pro 90 Min. | Distance covered per 90 | Dezimal (km) |  |
| `Distance` | Laufdistanz | Distance covered | Text, 0.0km |  |
| `MLG` | Fehler, die zu Gegentoren führten | Mistakes leading to goal | Zahl |  |
| `Form` | Form (letzte Spiele) | Form | Text, - |  |
| `Form International` | Form in der Nationalmannschaft | International form | Text |  |
| `Games Won` | Gewonnene Spiele | Games won | Zahl |  |
| `Games Missed In A Row` | Verpasste Spiele in Folge | Games missed in a row | Zahl |  |
| `Games Lost` | Verlorene Spiele | Games lost | Zahl |  |
| `Games Drawn` | Unentschiedene Spiele | Games drawn | Zahl |  |
| `Game Win Ratio` | Siegquote | Game win ratio | Prozent |  |
| `Fouls Against` | Erlittene Fouls | Fouls against | Zahl |  |
| `Club Form` | Form im Verein | Club form | Text |  |
| `Average Rating International` | Durchschnittsnote Nationalmannschaft | Average rating (international) | Dezimal |  |
| `Last 5 Games Club` | Noten der letzten fünf Vereinsspiele | Last five club ratings | Text |  |
| `Last 5 Games International` | Noten der letzten fünf Länderspiele | Last five international ratings | Text |  |
| `Last Match Rating` | Note im letzten Spiel | Last match rating | Dezimal |  |
| `PoM` | Spieler des Spiels | Player of the match awards | Zahl |  |
| `Starts` | Einsätze von Beginn | Starts | Zahl |  |
| `Tcon/90` | Gegentore der Mannschaft pro 90 Min. (mit dem Spieler) | Team goals conceded per 90 | Dezimal |  |
| `Tgls/90` | Tore der Mannschaft pro 90 Min. (mit dem Spieler) | Team goals per 90 | Dezimal |  |
| `Yel` | Gelbe Karten | Yellow cards | Zahl |  |
| `Tcon` | Gegentore der Mannschaft (mit dem Spieler) | Team goals conceded | Zahl |  |
| `Team Goals` | Tore der Mannschaft (mit dem Spieler) | Team goals | Zahl |  |
| `Red` | Rote Karten | Red cards | Zahl |  |
| `Pts/Gm` | Punkte pro Spiel | Points per game | Dezimal |  |
| `Mins/Gm` | Minuten pro Spiel | Minutes per game | Zahl |  |
| `Minutes` | Gespielte Minuten | Minutes played | Zahl |  |
| `Rating` | Durchschnittsnote der Saison | Average rating (season) | Dezimal, - wenn keine Spiele | Average Rating |
| `Apps` | Einsätze | Appearances | Zahl (Einwechslungen in Klammern) |  |
| `Average Rating Club` | Durchschnittsnote im Verein | Average rating (club) | Dezimal |  |
| `Form with Value` | Form mit Notenwert | Form with value | Text |  |

## Karriere

| Spalte | Bedeutung | Meaning | Format | Analyzer |
| --- | --- | --- | --- | --- |
| `AT League Goals` | Ligatore gesamte Karriere | All-time league goals | Zahl |  |
| `AT League Apps` | Ligaeinsätze gesamte Karriere | All-time league appearances | Zahl |  |
| `AT Gls` | Tore gesamte Karriere | All-time goals | Zahl |  |
| `AT Apps` | Einsätze gesamte Karriere | All-time appearances | Zahl |  |
| `Yth Apps` | Einsätze in Jugend-Nationalmannschaften | Youth international appearances | Zahl |  |
| `National Team` | Status in der Nationalmannschaft | National team status | Text, Main |  |
| `Int Gls` | Länderspieltore | International goals | Zahl |  |
| `Int Apps` | Länderspiele | International appearances | Zahl |  |
| `International Assists` | Vorlagen in Länderspielen | International assists | Zahl |  |
| `International Goals Conceded` | Gegentore in Länderspielen | International goals conceded | Zahl |  |
| `International Appearances (Season)` | Länderspiele dieser Saison | International appearances (season) | Zahl |  |
| `Yth Gls` | Tore in Jugend-Nationalmannschaften | Youth international goals | Zahl |  |

## Vertrag

| Spalte | Bedeutung | Meaning | Format | Analyzer |
| --- | --- | --- | --- | --- |
| `Contract Start Date` | Vertragsbeginn | Contract start date | Datum T/M/JJJJ, 15/5/2026 |  |
| `Wage Of Offered Contract` | Gehalt im angebotenen Vertrag | Wage of offered contract | Text |  |
| `Expires` | Vertragsende (Menü: Contract Expiry Date). Kommt zweimal vor, die zweite Spalte ist vermutlich das Leihende | Contract expiry (menu: Contract Expiry Date). Appears twice; the second is probably the loan expiry | Datum, 30/6/2027 |  |
| `Wage Contribution` | Gehaltsanteil (?) | Wage contribution (?) | Geld |  |
| `Wage After Tax` | Gehalt nach Steuern | Wage after tax | Geld, €170K p/w |  |
| `Status Of Offered Contract` | Status des angebotenen Vertrags | Status of offered contract | Text |  |
| `Set For Release` | Zur Freigabe vorgesehen | Set for release | Yes/No |  |
| `Expires` | Ende des Leihvertrags (Menü: Loan Contract Expiry Date) (?) | Loan contract expiry (menu: Loan Contract Expiry Date) (?) | Datum oder N/A |  |
| `Contract Status` | Vertragsart | Contract status | Text, Full Time |  |
| `FFP Contribution` | Anteil an Financial Fair Play | FFP contribution | Geld, €375K p/w |  |
| `Expiry Date Of Offered Contract` | Ablaufdatum des angebotenen Vertrags | Expiry date of offered contract | Datum oder N/A |  |
| `Contract Type` | Vertragstyp | Contract type | Text |  |
| `Due Date` | Fälligkeitsdatum (?) | Due date (?) | Datum |  |

## Scouting

| Spalte | Bedeutung | Meaning | Format | Analyzer |
| --- | --- | --- | --- | --- |
| `Cons` | Schwächen laut Scoutbericht | Weaknesses (scout report) | Text, Heading |  |
| `Pros` | Stärken laut Scoutbericht | Strengths (scout report) | Text |  |
| `Style` | Spielstil | Playing style | Text, Distributor |  |
| `Potential` | Potenzial in Sternen (relativ zum eigenen Team) | Potential in stars (relative to your squad) | Dezimal 0.0-5.0 |  |
| `Role Ability Out Of Possession` | Rollenstärke gegen den Ball (Sterne) | Role ability out of possession (stars) | Dezimal 0.0-5.0 |  |
| `Role Ability` | Rollenstärke (Sterne) | Role ability (stars) | Dezimal 0.0-5.0 |  |
| `Ability` | Aktuelle Stärke in Sternen (relativ zum eigenen Team) | Current ability in stars (relative to your squad) | Dezimal 0.0-5.0 |  |

## Vertrag Klauseln/Boni

| Spalte | Bedeutung | Meaning | Format | Analyzer |
| --- | --- | --- | --- | --- |
| `Appearance Fee` | Auflaufprämie | Appearance fee | Geld, €125K |  |
| `Match Highest Earner Clause` | Klausel: Gehalt wie Topverdiener | Match highest earner clause | Yes/No |  |
| `Extension After Promotion` | Verlängerung nach Aufstieg | Extension after promotion | Text |  |
| `Minimum Fee Release Clause` | Ausstiegsklausel (Mindestablöse) | Minimum fee release clause | Geld, €65,000,000 |  |
| `Top Division Relegation Wage Drop` | Gehaltskürzung bei Abstieg aus der höchsten Liga | Top division relegation wage drop | Prozent |  |
| `Wage After Reaching Club League Games` | Gehaltserhöhung nach Ligaeinsätzen | Wage after reaching club league games | Geld |  |
| `Top Division Promotion Wage Rise` | Gehaltserhöhung bei Aufstieg in die höchste Liga | Top division promotion wage rise | Prozent |  |
| `Sell On Fee Percentage` | Weiterverkaufsbeteiligung | Sell-on fee percentage | Prozent |  |
| `Relegation Wage Drop` | Gehaltskürzung bei Abstieg | Relegation wage drop | Prozent |  |
| `Promotion Wage Rise` | Gehaltserhöhung bei Aufstieg | Promotion wage rise | Prozent |  |
| `Percentage of Wages paid by Sponsor` | Gehaltsanteil, den ein Sponsor zahlt | Percentage of wages paid by sponsor | Prozent |  |
| `Optional Contract Extension By Club` | Option auf Vertragsverlängerung durch den Verein | Optional contract extension by club | Text |  |
| `One-Year Extension After League Games (Promoted Final Season)` | Ein Jahr Verlängerung nach Ligaeinsätzen (Aufstieg im letzten Jahr) | One-year extension after league games (promoted, final season) | Zahl |  |
| `One-Year Extension After League Games (Final Season)` | Ein Jahr Verlängerung nach Ligaeinsätzen (letztes Vertragsjahr) | One-year extension after league games (final season) | Zahl |  |
| `One-Year Extension After League Games (Avoid Relegation Final Season)` | Ein Jahr Verlängerung nach Ligaeinsätzen (Klassenerhalt im letzten Jahr) | One-year extension after league games (avoid relegation, final season) | Zahl |  |
| `Unused Substitute Fee` | Prämie als unbenutzter Ersatzspieler | Unused substitute fee | Geld |  |
| `Seasonal Landmark Goal Bonus - Number Of Goals` | Saisonbonus Tore: Anzahl | Seasonal goal landmark: number of goals | Zahl |  |
| `Seasonal Landmark Goal Bonus` | Saisonbonus Tore: Betrag | Seasonal goal landmark bonus | Geld |  |
| `Seasonal Landmark Combined Goal and Assist Bonus - Number Of Combined Goals and Assists` | Saisonbonus Scorerpunkte: Anzahl | Seasonal goals+assists landmark: number | Zahl |  |
| `Seasonal Landmark Combined Goal and Assist Bonus` | Saisonbonus Scorerpunkte: Betrag | Seasonal goals+assists landmark bonus | Geld |  |
| `Seasonal Landmark Assist Bonus - Number Of Assists` | Saisonbonus Vorlagen: Anzahl | Seasonal assist landmark: number | Zahl |  |
| `Seasonal Landmark Assist Bonus` | Saisonbonus Vorlagen: Betrag | Seasonal assist landmark bonus | Geld |  |
| `Int Cap Bonus` | Prämie pro Länderspiel | International cap bonus | Geld |  |
| `Goal Bonus` | Torprämie | Goal bonus | Geld |  |
| `Clean Sheet Bonus` | Zu-null-Prämie | Clean sheet bonus | Geld |  |
| `Assist Bonus` | Vorlagenprämie | Assist bonus | Geld |  |
| `Active Non Promotion Release Clause` | Aktive Ausstiegsklausel bei Nichtaufstieg | Active non-promotion release clause | Geld |  |
| `Non-Playing Job Offer Release Clause` | Ausstiegsklausel bei Jobangebot außerhalb des Platzes | Non-playing job offer release clause | Yes/No |  |
| `Relegation Release Clause` | Ausstiegsklausel bei Abstieg | Relegation release clause | Geld |  |
| `Injury Release Clause` | Ausstiegsklausel bei Verletzung (?) | Injury release clause (?) | Geld |  |
| `Active Relegation Release Clause` | Aktive Ausstiegsklausel bei Abstieg | Active relegation release clause | Geld |  |
| `Yearly Wage Rise` | Jährliche Gehaltserhöhung | Yearly wage rise | Prozent, 10% |  |
| `Waive Club Compensation For Managerial Role` | Verzicht auf Ablöse bei Trainerangebot | Waive compensation for managerial role | Text |  |
| `Non Promotion Release Clause` | Ausstiegsklausel bei Nichtaufstieg | Non-promotion release clause | Geld |  |
| `Minimum Fee Release Clause (Foreign Clubs) - Expiry Date` | Ausstiegsklausel ausländische Vereine: Ablaufdatum | Release clause (foreign clubs): expiry | Datum |  |
| `Minimum Fee Release Clause (Foreign Clubs)` | Ausstiegsklausel ausländische Vereine | Release clause (foreign clubs) | Geld |  |
| `Minimum Fee Release Clause (Domestic Clubs) - Expiry Date` | Ausstiegsklausel inländische Vereine: Ablaufdatum | Release clause (domestic clubs): expiry | Datum |  |
| `Minimum Fee Release Clause (Domestic Clubs)` | Ausstiegsklausel inländische Vereine | Release clause (domestic clubs) | Geld |  |
| `Minimum Fee Release Clause (Domestic Clubs in Higher Division) - Expiry Date` | Ausstiegsklausel höherklassige inländische Vereine: Ablaufdatum | Release clause (higher-division domestic clubs): expiry | Datum |  |
| `Minimum Fee Release Clause (Domestic Clubs in Higher Division)` | Ausstiegsklausel höherklassige inländische Vereine | Release clause (higher-division domestic clubs) | Geld |  |
| `Minimum Fee Release Clause (Clubs in a Major Continental Competition) - Expiry Date` | Ausstiegsklausel Vereine im großen Europapokal: Ablaufdatum | Release clause (major continental competition): expiry | Datum |  |
| `Minimum Fee Release Clause (Clubs in a Major Continental Competition)` | Ausstiegsklausel Vereine im großen Europapokal | Release clause (major continental competition) | Geld |  |
| `Minimum Fee Release Clause (Clubs in a Continental Competition) - Expiry Date` | Ausstiegsklausel Vereine im Europapokal: Ablaufdatum | Release clause (continental competition): expiry | Datum |  |
| `Minimum Fee Release Clause (Clubs in a Continental Competition)` | Ausstiegsklausel Vereine im Europapokal | Release clause (continental competition) | Geld |  |
| `Minimum Fee Release Clause - Expiry Date` | Ausstiegsklausel: Ablaufdatum | Release clause: expiry | Datum |  |

## Transfer

| Spalte | Bedeutung | Meaning | Format | Analyzer |
| --- | --- | --- | --- | --- |
| `Transfer Options` | Transferoptionen | Transfer options | Text |  |
| `Loan Duration` | Leihdauer | Loan duration | Text |  |
| `WP Needed` | Chance auf Arbeitserlaubnis (Menü: Work Permit Chance) (?) | Work permit chance (?) | Text |  |
| `Last Transfer Fee` | Letzte Ablöse | Last transfer fee | Geld |  |
| `Loan Status` | Leihstatus | Loan status | Text |  |
| `On Loan From` | Ausgeliehen von | On loan from | Text |  |
| `Overall Fee` | Gesamtablöse | Overall fee | Geld |  |
| `Player's Interest` | Interesse des Spielers an einem Wechsel | Player's interest | Text |  |
| `Transfer Fees Received` | Erhaltene Ablösen | Transfer fees received | Geld |  |
| `Transfer Status` | Transferstatus | Transfer status | Text |  |
| `Type` | Art (Transfer/Leihe) (?) | Type (transfer/loan) (?) | Text |  |
| `Current Year +1 Future Playing Time` | Geplante Einsatzzeit nächstes Jahr | Planned playing time next year | Text |  |
| `Work Permit Required` | Arbeitserlaubnis erforderlich | Work permit required | Yes/No |  |
| `Fee` | Ablöse | Fee | Geld |  |
| `Current Year +4 Future Playing Time` | Geplante Einsatzzeit in vier Jahren | Planned playing time in four years | Text |  |
| `Current Year +3 Future Playing Time` | Geplante Einsatzzeit in drei Jahren | Planned playing time in three years | Text |  |
| `Current Year +2 Future Playing Time` | Geplante Einsatzzeit in zwei Jahren | Planned playing time in two years | Text |  |
| `Asking Price` | Geforderte Ablöse | Asking price | Geld |  |
| `Agent` | Berater | Agent | Text |  |
| `Future Playing Time` | Geplante Einsatzzeit | Planned playing time | Text |  |
| `Last Club` | Letzter Verein | Last club | Text |  |

## Training

| Spalte | Bedeutung | Meaning | Format | Analyzer |
| --- | --- | --- | --- | --- |
| `Progress` | Entwicklungsfortschritt | Progress | Symbol (kein Text) |  |
| `Training Happiness` | Zufriedenheit mit dem Training | Training happiness | Text |  |
| `Training Happiness Details` | Details zur Trainingszufriedenheit | Training happiness details | Text |  |
| `Training Rating` | Trainingsnote | Training rating | Dezimal |  |
| `Workload` | Trainingsumfang | Workload | Text |  |
| `Fitness` | Trainingsschwerpunkt Fitness | Training focus: fitness | Text |  |
| `Goalkeeping` | Trainingsschwerpunkt Torwart | Training focus: goalkeeping | Text |  |
| `Possession - Technical` | Training Ballbesitz, technisch | Training: possession, technical | Text |  |
| `Possession - Tactical` | Training Ballbesitz, taktisch | Training: possession, tactical | Text |  |
| `Defending - Technical` | Training Verteidigung, technisch. Kommt zweimal vor (?) | Training: defending, technical. Appears twice (?) | Text |  |
| `Defending - Technical` | Training Verteidigung, zweite Spalte (vermutlich taktisch) | Training: defending, second column (probably tactical) | Text |  |
| `Attacking - Tactical` | Training Angriff, taktisch. Kommt zweimal vor (?) | Training: attacking, tactical. Appears twice (?) | Text |  |
| `Attacking - Tactical` | Training Angriff, zweite Spalte (vermutlich technisch) | Training: attacking, second column (probably technical) | Text |  |
| `Additional Focus` | Zusätzlicher Trainingsschwerpunkt | Additional focus | Text |  |
| `Set Pieces` | Training Standards | Training: set pieces | Text |  |
| `Overall Training Improvement` | Gesamtfortschritt im Training | Overall training improvement | Text |  |

## Medizin/Belastung

| Spalte | Bedeutung | Meaning | Format | Analyzer |
| --- | --- | --- | --- | --- |
| `Training Load` | Trainingsbelastung | Training load | Text |  |
| `Time Missed` | Ausfallzeit | Time missed | Text |  |
| `Recurring Injury` | Wiederkehrende Verletzung | Recurring injury | Text |  |
| `Overall Risk` | Gesamtrisiko | Overall risk | Text |  |
| `CON` | Kondition | Condition | Prozent/Symbol |  |
| `SHP` | Spielfitness (Sharpness) | Match sharpness | Prozent/Symbol |  |
| `Match Load` | Spielbelastung | Match load | Text |  |
| `Injury Susceptibility` | Verletzungsanfälligkeit | Injury susceptibility | Text |  |
| `Injury Risk` | Verletzungsrisiko | Injury risk | Text |  |
| `Injury preventing training` | Verletzung verhindert Training | Injury preventing training | Text |  |
| `Injury` | Aktuelle Verletzung | Current injury | Text |  |
| `Injured On` | Verletzt seit | Injured on | Datum |  |
| `Fatigue` | Ermüdung | Fatigue | Text/Symbol |  |

## Taktik

| Spalte | Bedeutung | Meaning | Format | Analyzer |
| --- | --- | --- | --- | --- |
| `Effectiveness` | Taktische Wirksamkeit (Menü: Tactical Effectiveness) | Tactical effectiveness | Text |  |
| `Tactical Understanding` | Taktisches Verständnis | Tactical understanding | Text |  |
| `Next Match Info` | Info zum nächsten Spiel | Next match info | Text |  |
| `Player Buy-In` | Akzeptanz der Taktik | Player buy-in | Text |  |
| `Player Ability` | Spielerfähigkeit (Taktik-Ansicht) | Player ability (tactics view) | Text |  |
| `Quality of Next Opposition Report` | Qualität des Gegnerberichts | Quality of next opposition report | Text |  |
| `Opposition Player Knowledge` | Wissen über Gegenspieler | Opposition player knowledge | Text |  |
| `Passing Style` | Vertrautheit: Passstil | Familiarity: passing style | Text |  |
| `Build Up` | Vertrautheit: Spielaufbau | Familiarity: build-up | Text |  |
| `Chance Creation` | Vertrautheit: Chancenkreierung | Familiarity: chance creation | Text |  |
| `In Possession Shape` | Vertrautheit: Formation mit Ball | Familiarity: in-possession shape | Text |  |
| `Line of Engagement` | Vertrautheit: Pressinglinie | Familiarity: line of engagement | Text |  |
| `Pressing Intensity` | Vertrautheit: Pressingintensität | Familiarity: pressing intensity | Text |  |
| `Pressing Trap` | Vertrautheit: Pressingfalle | Familiarity: pressing trap | Text |  |
| `Out Of Possession Shape` | Vertrautheit: Formation gegen den Ball | Familiarity: out-of-possession shape | Text |  |
| `Player Tactical Ability` | Taktische Fähigkeit | Tactical ability | Text |  |
| `Tactical Complexity` | Taktische Komplexität | Tactical complexity | Text |  |
| `Position/Role` | Position/Rolle in der Taktik | Position/role in tactic | Text |  |
| `Player Role Suitability` | Eignung für die Rolle | Role suitability | Text |  |
| `Player Capability` | Leistungsvermögen (?) | Player capability (?) | Text |  |
| `Tactical Comprehension` | Taktisches Auffassungsvermögen | Tactical comprehension | Text |  |
| `Postion Suitability` | Eignung für die Position (Schreibfehler im Spiel) | Position suitability (typo in game) | Text |  |
