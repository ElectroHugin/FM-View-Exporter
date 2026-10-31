using System;
using System.Collections.Generic;

namespace FMViewExporter.Core
{
    /// <summary>
    /// Leitet aus den Elementnamen rund um die Spielertabelle den Namen der Ansicht ab,
    /// der im Dateinamen steht: squad, player-database, shortlist.
    /// </summary>
    public static class ViewNames
    {
        public const string PlayerDatabase = "player-database";
        public const string Shortlist = "shortlist";

        private const string TablePrefix = "playertable";

        /// <summary>Bekannte Bereiche in FM27, die nicht auf "Screen" enden.</summary>
        private static readonly Dictionary<string, string> KnownPanels = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["PlayerSearchReport"] = PlayerDatabase,
            ["RecruitmentShortlistTool"] = Shortlist,
        };

        /// <param name="tableNames">Namen der Tabelle und ihrer Nachfahren, die mit "playertable" beginnen
        /// (in FM27 z. B. ein Rahmen #PlayerTable um #playertableshortlist).</param>
        /// <param name="ancestorNames">Namen der Vorfahren, vom naechsten zum entferntesten.</param>
        public static string Guess(IEnumerable<string> tableNames, IEnumerable<string> ancestorNames)
        {
            // #playertableshortlist -> "shortlist"
            foreach (var name in tableNames)
            {
                if (name != null && name.Length > TablePrefix.Length && name.StartsWith(TablePrefix, StringComparison.OrdinalIgnoreCase))
                    return name.Substring(TablePrefix.Length).ToLowerInvariant();
            }

            foreach (var name in ancestorNames)
            {
                if (string.IsNullOrEmpty(name))
                    continue;
                if (KnownPanels.TryGetValue(name, out var known))
                    return known;
                // #SquadScreen -> "squad"
                if (name.EndsWith("Screen", StringComparison.Ordinal) && name.Length > "Screen".Length)
                    return name.Substring(0, name.Length - "Screen".Length).ToLowerInvariant();
            }

            // Bisher ohne eigenen Namen: die Spieler-Datenbank.
            return PlayerDatabase;
        }
    }
}
