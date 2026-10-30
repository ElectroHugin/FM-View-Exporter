"""
Spaltenmenue von FM27 ("Insert Column" in Spielertabellen), abgeschrieben aus Screenshots.
Erzeugt docs/columns/fm27-menu.csv: Menuepfad, Menueeintrag, Spaltenkopf in der Tabelle.
Der Menuename weicht oft vom Spaltenkopf ab (z. B. "Player Of The Match" -> "PoM").
Aufruf: python3 tools/menu_fm27.py
"""
import csv, os

# Menuepfad -> Eintraege, wie im Spiel geschrieben (auch Schreibfehler des Spiels).
MENU = {
 "Attributes > Goalkeeping": ["Aerial Reach", "Command Of Area", "Communication", "Eccentricity", "Handling", "Kicking", "One on Ones", "Punching (Tendency)", "Reflexes", "Rushing out (Tendency)", "Throwing"],
 "Attributes > Mental": ["Aggression", "Anticipation", "Bravery", "Composure", "Concentration", "Decisions", "Determination", "Flair", "Leadership", "Off The Ball", "Positioning", "Teamwork", "Vision", "Work Rate"],
 "Attributes > Physical": ["Acceleration", "Agility", "Balance", "Jumping Reach", "Natural Fitness", "Pace", "Stamina", "Strength"],
 "Attributes > Technical": ["Corners", "Crossing", "Dribbling", "Finishing", "First Touch", "Free Kicks", "Heading", "Long Shots", "Long Throws", "Marking", "Passing", "Penalty Taking", "Tackling", "Technique"],
 "Ability and Roles": ["Assistant manager's opinion of this player's current ability", "Assistant manager's opinion of this player's current ability in the selected role - IP", "Assistant manager's opinion of this player's current ability in the selected role - OOP", "Assistant manager's opinion of this player's potential ability", "Assistant manager's opinion of this player's best position", "Assistant manager's opinion of this player's best tactical role", "Player Style", "Strongest Area of Game", "Weakest Area of Game"],
 "Club": ["Based", "Club", "Division"],
 "Contract > Bonuses": ["Appearance Fee", "Assist Bonus", "Clean Sheet Bonus", "Goal Bonus", "International Cap Bonus", "Seasonal Landmark Assist Bonus", "Season Landmark Assist Bonus - Number Of Assists", "Seasonal Landmark Combined Goal and Assist Bonus", "Seasonal Landmark Combined Goal and Assist Bonus - Number of combined Goals and Assists", "Seasonal Landmark Goal Bonus", "Seasonal Landmark Goal Bonus - Number of Goals", "Unused Substitute Fee"],
 "Contract > Clauses (Release)": ["Minimum Fee Release Clause (Domestic Club Release Fee Date Change)", "Active Non Promotion Release Clause", "Active Relegation Release Clause", "Injury Release Clause", "Minimum Fee Release Clause", "Minimum Fee Release Clause - Expiry Date", "Minimum Fee Release Clause (Clubs in a Continental Competition)", "Minimum Fee Release Clause (Clubs in a Continental Competition) - Expiry Date", "Minimum Fee Release Clause (Clubs in a Major Continental Competition)", "Minimum Fee Release Clause (Clubs in a Major Continental Competition) - Expiry Date", "Minimum Fee Release Clause (Domestic Clubs in Higher Division)", "Minimum Fee Release Clause (Domestic Clubs in Higher Division) - Expiry Date", "Minimum Fee Release Clause (Domestic Clubs)", "Minimum Fee Release Clause (Domestic Clubs) - Expiry Date", "Minimum Fee Release Clause (Foreign Clubs)", "Minimum Fee Release Clause (Foreign Clubs) - Expiry Date", "Non Promotion Release Clause", "Non-Playing Job Offer Release Clause", "Relegation Release Clause"],
 "Contract > General": ["Wage Of Offered Contract", "Contract Expiry Date", "Contract Start Date", "Contract Type", "Expiry Date Of Offered Contract", "FFP Contribution", "Full/Part Time", "Loan Contract Expiry Date", "Loan Duration", "Set For Release", "Status Of Offered Contract", "Transfer Options", "Wage", "Wage After Tax", "Wage Contribution"],
 "Dynamics and Happiness": ["General Happiness", "Hierarchy", "International Happiness", "Long-Term Plans", "Morale", "Overall Happiness Level", "Progress", "Promises", "Short-term Plans", "Social Groups"],
 "Fitness and Injuries": ["Fatigue", "Injured On", "Injury", "Injury preventing training", "Injury Risk", "Injury Susceptibility", "Match Load", "Match Sharpness", "Overall Physical Condition", "Overall Risk", "Recurring Injury", "Time Out", "Training Load"],
 "Stats (General) > General": ["Assists", "Average Rating Club", "Average Rating International", "Average Minutes Per Goal", "Average Rating", "Clean Sheets", "Clean Sheets per 90 minutes", "Club Form", "Conceded per 90 minutes", "Expected Assists", "Expected Assists per 90 minutes", "Expected Goals Overperformance", "Expected Goals pers 90 minutes", "Fouls Against", "Fouls Made", "Game Win Ratio", "Games Drawn", "Games Lost", "Games Missed In A Row", "Games Won", "Goals", "Goals Conceded", "Goals per 90 minutes", "Form International", "Last 5 Games", "Form with Value", "Last 5 Games Club", "Last 5 Games International", "Last Match Rating", "Minutes", "Minutes Per Game", "Minutes Since Last Conceded", "Minutes Since Last Goal", "Non Penalty Expected Goals", "Non Penalty Expected Goals per 90", "Penalties Taken", "Penalties Faced", "Penalties Saved", "Penalties Saved Ratio", "Penalties Scored", "Penalties Scored Ratio", "Player Of The Match", "Points Won Per Game", "Red Cards", "Starting Appearances", "Team Goals", "Team Goals Conceded", "Team Goals Conceded per 90 minutes", "Team Goals per 90 Minutes", "Total Saves per 90 Minutes", "xG", "Yellow Cards"],
 "Stats (General) > Career": ["All Time Career Appearances", "All Time Career Goals", "All Time League Appearances", "All Time League Goals"],
}

# Noch nicht erfasst (Screenshots fehlen). Bei "Stats (General) > General" fehlt evtl. der Anfang der Liste.
MISSING = ["Contract > Clauses", "General", "International", "Squad", "Tactical Effectiveness", "Training", "Transfer", "Stats (alle Untergruppen)"]

# Menueeintrag -> Spaltenkopf, wo beide abweichen. "(?)" = Zuordnung vermutet.
HEADER = {
 "Punching (Tendency)": "Punching",
 "Teamwork": "Team Work",
 "Free Kicks": "Free Kick Taking",
 "Assistant manager's opinion of this player's current ability": "Ability",
 "Assistant manager's opinion of this player's current ability in the selected role - IP": "Role Ability",
 "Assistant manager's opinion of this player's current ability in the selected role - OOP": "Role Ability Out Of Possession",
 "Assistant manager's opinion of this player's potential ability": "Potential",
 "Assistant manager's opinion of this player's best position": "Best Pos",
 "Assistant manager's opinion of this player's best tactical role": "Best Role",
 "Player Style": "Style",
 "Strongest Area of Game": "Pros",
 "Weakest Area of Game": "Cons",
 "Based": "Based In",
 "International Cap Bonus": "Int Cap Bonus",
 "Season Landmark Assist Bonus - Number Of Assists": "Seasonal Landmark Assist Bonus - Number Of Assists",
 "Contract Expiry Date": "Expires",
 "Loan Contract Expiry Date": "Expires (2. Spalte) (?)",
 "Full/Part Time": "Contract Status",
 "General Happiness": "Happiness (?)",
 "Overall Happiness Level": "Overall Happiness",
 "Match Sharpness": "SHP",
 "Overall Physical Condition": "CON",
 "Time Out": "Time Missed",
 "Average Rating": "Rating (?)",
 "Average Minutes Per Goal": "Mins/Gl",
 "Clean Sheets per 90 minutes": "Cln/90",
 "Conceded per 90 minutes": "Con/90",
 "Expected Assists": "xA",
 "Expected Assists per 90 minutes": "xA/90",
 "Expected Goals Overperformance": "xG-OP",
 "Expected Goals pers 90 minutes": "xG/90",
 "Fouls Made": "Fouls",
 "Last 5 Games": "Form (?)",
 "Minutes Per Game": "Mins/Gm",
 "Non Penalty Expected Goals": "NP-xG",
 "Non Penalty Expected Goals per 90": "NP-xG/90",
 "Penalties Taken": "Pens",
 "Penalties Faced": "Pens Faced",
 "Penalties Saved": "Pens Saved",
 "Penalties Saved Ratio": "Pens Saved Ratio",
 "Penalties Scored": "Pens S",
 "Penalties Scored Ratio": "Pen/R",
 "Player Of The Match": "PoM",
 "Points Won Per Game": "Pts/Gm",
 "Red Cards": "Red",
 "Starting Appearances": "Starts",
 "Team Goals Conceded": "Tcon",
 "Team Goals Conceded per 90 minutes": "Tcon/90",
 "Team Goals per 90 Minutes": "Tgls/90",
 "Total Saves per 90 Minutes": "Saves/90",
 "Yellow Cards": "Yel",
 "All Time Career Appearances": "AT Apps",
 "All Time Career Goals": "AT Gls",
 "All Time League Appearances": "AT League Apps",
 "All Time League Goals": "AT League Goals",
}

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))

def main():
    with open(os.path.join(ROOT, "docs", "columns", "fm27.csv"), encoding="utf-8") as f:
        headers = [r["header"] for r in csv.DictReader(f)]
    lower = {h.lower(): h for h in headers}
    rows, unmatched = [], []
    for path, entries in MENU.items():
        for e in entries:
            h = HEADER.get(e) or lower.get(e.lower(), "")
            if not h:
                unmatched.append(f"{path} > {e}")
            rows.append((path, e, h))
    with open(os.path.join(ROOT, "docs", "columns", "fm27-menu.csv"), "w", newline="", encoding="utf-8") as f:
        w = csv.writer(f, lineterminator="\n")
        w.writerow(["menu_path", "menu_entry", "header"])
        w.writerows(rows)
    print(len(rows), "Eintraege; ohne Spaltenkopf:", *unmatched, sep="\n  ")

if __name__ == "__main__":
    main()
