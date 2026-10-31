using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FMViewExporter.Core
{
    /// <summary>
    /// Rechnet die Sterne einer Zelle (Ability, Potential) in eine Zahl um.
    /// FM27 gibt jedem Stern eine Klasse <c>ability-{a}-potential-level-{p}-youth-…</c>:
    /// <c>a</c> ist der sichere Teil (gelb: max = ganz, half = halb, minimum = nichts),
    /// <c>p</c> der Teil der Einschätzung (grau: fullfilled/full = ganz, half = halb, none = nichts).
    /// Ein Stern zählt mit dem größeren der beiden Teile, so wie er im Spiel gefüllt aussieht:
    /// 1 gelber + 1 grauer Stern = 2.0.
    /// </summary>
    public static class StarRating
    {
        private static readonly Regex Pattern = new Regex(
            @"^ability-(max|half|minimum)-potential-level-(fullfilled|full|half|none)\b",
            RegexOptions.CultureInvariant);

        /// <summary>Wert eines Sterns aus seinen CSS-Klassen, 0 bis 1.</summary>
        public static double StarValue(IEnumerable<string> classes)
        {
            var fallback = 0.0;
            foreach (var name in classes)
            {
                if (name == null)
                    continue;
                var match = Pattern.Match(name);
                if (match.Success)
                    return Math.Max(Part(match.Groups[1].Value), Part(match.Groups[2].Value));
                // Unbekanntes Schema: wie bisher nur ganze und halbe Sterne erkennen.
                if (name.Contains("-max-"))
                    fallback = Math.Max(fallback, 1.0);
                else if (name.Contains("-half-"))
                    fallback = Math.Max(fallback, 0.5);
            }
            return fallback;
        }

        public static string Format(double total) => total.ToString("0.0", CultureInfo.InvariantCulture);

        private static double Part(string level)
        {
            switch (level)
            {
                case "max":
                case "fullfilled":
                case "full":
                    return 1.0;
                case "half":
                    return 0.5;
                default:
                    return 0.0;
            }
        }
    }
}
