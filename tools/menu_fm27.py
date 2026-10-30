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
 "Stats > Aerial Challenges": ["Aerial Challenges Attempted", "Aerial Challenges Attempted per 90 Minutes", "Headers Lost per 90 Minutes", "Headers Won", "Headers Won per 90 minutes", "Headers Won Ratio", "Key Headers per 90 minutes"],
 "Stats > Crosses": ["Cross Completion Ratio", "Crosses Attempted", "Crosses attempted per 90 minutes", "Crosses Completed", "Crosses completed per 90 minutes", "Open Play Cross Completion Ratio", "Open Play Crosses Attempted", "Open Play Crosses attempted per 90 minutes", "Open Play Crosses completed", "Open Play Crosses completed per 90 minutes"],
 "Stats > Mistakes": ["Mistakes Leading To Goal"],
 "Stats > Movement": ["Distance Covered", "Distance Covered per 90 Minutes", "Dribbles Made", "Dribbles Made per 90 minutes", "High Intensity Sprints per 90 minutes", "Offsides"],
 "Stats > Passes": ["Assists per 90 minutes", "Chances Created per 90 Minutes", "Clear Cut Chances Created", "Key Passes", "Key Passes per 90 minutes", "Open Play Key Passes per 90 Minutes", "Pass Completion Ratio", "Passes Attempted", "Passes Attempted per 90 Minutes", "Passes Completed", "Passes Completed per 90 Minutes", "Possession Lost per 90 minutes", "Progressive Passes", "Progressive Passes per 90 minutes"],
 "Stats > Saves": ["Expected Goals Prevented", "Expected Goals Prevented per 90 Minutes", "Expected Save Percentage", "Save Ratio", "Saves Held", "Saves Parried", "Saves Tipped"],
 "Stats > Shots": ["Conversion Rate (%)", "Expected Goals per Shot", "Free Kick Shots", "Goals From Outside The Box", "Shots", "Shots From Outside The Box Per 90 minutes", "Shots On Target", "Shots On Target per 90 minutes", "Shots On Target Ratio", "Shots per 90 minutes"],
 "Stats > Tackles and Interceptions": ["Blocks", "Blocks per 90 minutes", "Clearances", "Clearances per 90 minutes", "Interceptions", "Interceptions per 90 minutes", "Key Tackles", "Key Tackles per 90 minutes", "Possession Won per 90 minutes", "Pressures Attempted", "Pressures attempted per 90 minutes", "Pressures completed", "Pressures completed per 90 minutes", "Shots Blocked Defending", "Shots Blocked Defending per 90 minutes", "Tackle completion ratio", "Tackles attempted", "Tackles completed", "Tackles per 90 minutes"],
 "Stats (General) > Career": ["All Time Career Appearances", "All Time Career Goals", "All Time League Appearances", "All Time League Goals"],
}

# Noch nicht erfasst (Screenshots fehlen). Bei "Stats (General) > General" fehlt evtl. der Anfang der Liste.
# Hinweis: Das Menue blendet Spalten aus, die schon in der Ansicht stehen (z. B. Future Playing Time).
# Bei "Stats > Crosses" ist die Liste evtl. unten abgeschnitten.
MISSING = ["Contract > Clauses", "General", "International", "Squad", "Tactical Effectiveness", "Training", "Transfer"]

# Menueeintrag -> Spaltenkopf, wo beide abweichen. "(?)" = Zuordnung vermutet.
HEADER = {
 "Aerial Challenges Attempted": "Hdrs A",
 "Aerial Challenges Attempted per 90 Minutes": "Aer A/90",
 "Headers Lost per 90 Minutes": "Hdrs L/90",
 "Headers Won": "Hdrs",
 "Headers Won per 90 minutes": "Hdrs W/90",
 "Headers Won Ratio": "Hdr %",
 "Key Headers per 90 minutes": "K Hdrs/90",
 "Cross Completion Ratio": "Cr C/A",
 "Crosses Attempted": "Cr A",
 "Crosses attempted per 90 minutes": "Crs A/90",
 "Crosses Completed": "Cr C",
 "Crosses completed per 90 minutes": "Cr C/90",
 "Open Play Cross Completion Ratio": "OP-Cr %",
 "Open Play Crosses Attempted": "OP-Crs A",
 "Open Play Crosses attempted per 90 minutes": "OP-Crs A/90",
 "Open Play Crosses completed": "OP-Crs C",
 "Open Play Crosses completed per 90 minutes": "OP-Crs C/90",
 "Mistakes Leading To Goal": "MLG",
 "Distance Covered": "Distance",
 "Distance Covered per 90 Minutes": "Dist/90",
 "Dribbles Made": "Drb",
 "Dribbles Made per 90 minutes": "Drb/90",
 "High Intensity Sprints per 90 minutes": "Sprints/90",
 "Offsides": "Off",
 "Assists per 90 minutes": "Asts/90",
 "Chances Created per 90 Minutes": "Ch C/90",
 "Clear Cut Chances Created": "CCC",
 "Key Passes": "Key",
 "Key Passes per 90 minutes": "KP/90",
 "Open Play Key Passes per 90 Minutes": "OP-KP/90",
 "Pass Completion Ratio": "Pas %",
 "Passes Attempted": "Pas A",
 "Passes Attempted per 90 Minutes": "Ps A/90",
 "Passes Completed": "Ps C",
 "Passes Completed per 90 Minutes": "Ps C/90",
 "Possession Lost per 90 minutes": "Poss Lost/90",
 "Progressive Passes": "PsP",
 "Progressive Passes per 90 minutes": "Pr passes/90",
 "Expected Goals Prevented": "xGP",
 "Expected Goals Prevented per 90 Minutes": "xGP/90",
 "Expected Save Percentage": "xSv %",
 "Save Ratio": "Sv %",
 "Saves Held": "Svh",
 "Saves Parried": "Svp",
 "Saves Tipped": "Svt",
 "Conversion Rate (%)": "Conv %",
 "Expected Goals per Shot": "xG/shot",
 "Shots On Target": "ShT",
 "Shots On Target per 90 minutes": "ShT/90",
 "Shots On Target Ratio": "Shot %",
 "Shots per 90 minutes": "Shot/90",
 "Blocks": "Blk",
 "Blocks per 90 minutes": "Blk/90",
 "Clearances per 90 minutes": "Clr/90",
 "Interceptions": "Itc",
 "Interceptions per 90 minutes": "Int/90",
 "Key Tackles": "K Tck",
 "Key Tackles per 90 minutes": "K Tck/90",
 "Possession Won per 90 minutes": "Poss Won/90",
 "Pressures Attempted": "Pres A",
 "Pressures attempted per 90 minutes": "Pres A/90",
 "Pressures completed": "Pres C",
 "Pressures completed per 90 minutes": "Pres C/90",
 "Shots Blocked Defending": "Shts Blckd",
 "Shots Blocked Defending per 90 minutes": "Shts Blckd/90",
 "Tackle completion ratio": "Tck R",
 "Tackles attempted": "Tck A",
 "Tackles completed": "Tck C",
 "Tackles per 90 minutes": "Tck/90",
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
