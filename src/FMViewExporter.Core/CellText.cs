using System.Text;
using System.Text.RegularExpressions;

namespace FMViewExporter.Core
{
    /// <summary>
    /// Macht aus dem Text einer Tabellenzelle einen sauberen Exportwert:
    /// Rich-Text-Tags wie &lt;color=...&gt; oder &lt;link=...&gt; entfernen, Leerraum zusammenfassen.
    /// </summary>
    public static class CellText
    {
        private static readonly Regex Tag = new("<[^<>]*>", RegexOptions.Compiled);
        private static readonly Regex Space = new(@"\s+", RegexOptions.Compiled);
        private static readonly Regex Decimal = new(@"^[+-]?\d+\.\d+(%|km)?$", RegexOptions.Compiled);

        public static string Clean(string? text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;
            var plain = Tag.Replace(text, string.Empty);
            return Space.Replace(plain, " ").Trim();
        }

        /// <summary>Haengt mehrere Texte einer Zelle mit Leerzeichen aneinander.</summary>
        public static string Join(System.Collections.Generic.IEnumerable<string> parts)
        {
            var builder = new StringBuilder();
            foreach (var part in parts)
            {
                var clean = Clean(part);
                if (clean.Length == 0)
                    continue;
                if (builder.Length > 0)
                    builder.Append(' ');
                builder.Append(clean);
            }
            return builder.ToString();
        }

        /// <summary>
        /// Schreibt reine Dezimalzahlen wie "0.75" oder "12.5%" mit dem Dezimalzeichen des CSV-Stils.
        /// Andere Texte (Geldbetraege, Daten, Namen) bleiben unveraendert.
        /// </summary>
        public static string ForCsv(string value, CsvStyle style)
        {
            if (style == CsvStyle.English || !Decimal.IsMatch(value))
                return value;
            return value.Replace('.', style.DecimalSeparator());
        }
    }
}
