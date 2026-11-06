using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FMViewExporter.Core
{
    /// <summary>Wie Zahlen in einer Spalte geschrieben sind.</summary>
    public enum NumberFormat
    {
        /// <summary>Punkt als Dezimal-, Komma als Tausendertrennzeichen (1,200.5). Standard von FM.</summary>
        English,
        /// <summary>Komma als Dezimal-, Punkt als Tausendertrennzeichen (1.200,5).</summary>
        German
    }

    /// <summary>
    /// Numerischer Sortierwert einer Zelle fuer die HTML-Sortierung. Erkannt werden nur Zellen, die
    /// vollstaendig wie eine Zahl aussehen: optional Waehrung, Zahl, optional Groessenordnung (K, M, B
    /// bzw. k, bn) direkt an der Zahl, optional Einheit (cm, kg, p/w ...). Spannen wie "10-12" oder
    /// "€1M - €2M" sortieren nach dem unteren, dann nach dem oberen Wert. Alles andere (Namen,
    /// "Unknown", "1. FC Heidenheim") bleibt Text. Der sichtbare Zelltext wird nicht veraendert.
    /// </summary>
    public static class SortValue
    {
        private const string Currency = @"(?:[€$£¥₹]|(?:EUR|USD|GBP|CHF|Fr\.?)\s?)";
        private const string Amount = Currency + @"?(?<num>\d[\d.,]*)(?<mag>[KMB]|bn|k)?" + Currency + "?";
        private const string Unit = @"(?:\s*(?:cm|kg|lbs?|ft|in|m|km|%|p/w|p/m|p/a|pw|pm|pa|mins?|yrs?))?";

        private static readonly Regex Single = new(
            @"^\s*(?<sign>[-+−])?" + Amount + Unit + @"\s*$", RegexOptions.CultureInvariant);

        private static readonly Regex Range = new(
            @"^\s*" + Amount.Replace("num", "a").Replace("mag", "ma") + @"\s*[-–]\s*" + Amount.Replace("num", "b").Replace("mag", "mb") + Unit + @"\s*$",
            RegexOptions.CultureInvariant);

        // Eindeutige Hinweise: "1,5" / "1,25" kann nicht englisch sein, "1.5" / "1.25" nicht deutsch.
        private static readonly Regex GermanHint = new(@"\d,\d{1,2}(?!\d)|\d\.\d{3},\d", RegexOptions.CultureInvariant);
        private static readonly Regex EnglishHint = new(@"\d\.\d{1,2}(?!\d)|\d,\d{3}\.\d", RegexOptions.CultureInvariant);

        /// <summary>
        /// Zahlenformat einer Spalte. Deutsch nur, wenn es eindeutige deutsche und keine eindeutigen
        /// englischen Werte gibt; sonst Englisch (so zeigt FM die Werte an).
        /// </summary>
        public static NumberFormat DetectFormat(IEnumerable<string> cells)
        {
            var german = false;
            foreach (var cell in cells)
            {
                if (string.IsNullOrEmpty(cell))
                    continue;
                if (EnglishHint.IsMatch(cell))
                    return NumberFormat.English;
                if (GermanHint.IsMatch(cell))
                    german = true;
            }
            return german ? NumberFormat.German : NumberFormat.English;
        }

        /// <summary>
        /// Sortierwert als Text fuer das HTML (invariant, Punkt als Dezimalzeichen): "Wert" oder bei
        /// Spannen "unten|oben". null = kein Zahlenwert (Text oder leer).
        /// </summary>
        public static string? Format(string? text, NumberFormat format)
        {
            if (!TryParse(text, format, out var low, out var high))
                return null;
            var a = low.ToString("R", CultureInfo.InvariantCulture);
            return low == high ? a : a + "|" + high.ToString("R", CultureInfo.InvariantCulture);
        }

        public static bool TryParse(string? text, NumberFormat format, out double low, out double high)
        {
            low = high = 0;
            if (string.IsNullOrWhiteSpace(text))
                return false;

            var single = Single.Match(text);
            if (single.Success)
            {
                if (!Number(single.Groups["num"].Value, single.Groups["mag"].Value, format, out low))
                    return false;
                if (single.Groups["sign"].Success && single.Groups["sign"].Value != "+")
                    low = -low;
                high = low;
                return true;
            }

            var range = Range.Match(text);
            if (range.Success
                && Number(range.Groups["a"].Value, range.Groups["ma"].Value, format, out low)
                && Number(range.Groups["b"].Value, range.Groups["mb"].Value, format, out high))
                return true;

            low = high = 0;
            return false;
        }

        private static bool Number(string digits, string magnitude, NumberFormat format, out double value)
        {
            value = 0;
            string normalized;
            if (format == NumberFormat.German)
            {
                if (!Regex.IsMatch(digits, @"^\d{1,3}(\.\d{3})*(,\d+)?$|^\d+(,\d+)?$"))
                    return false;
                normalized = digits.Replace(".", "").Replace(',', '.');
            }
            else
            {
                if (!Regex.IsMatch(digits, @"^\d{1,3}(,\d{3})*(\.\d+)?$|^\d+(\.\d+)?$"))
                    return false;
                normalized = digits.Replace(",", "");
            }
            if (!double.TryParse(normalized, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out value))
                return false;
            switch (magnitude)
            {
                case "K":
                case "k":
                    value *= 1e3;
                    break;
                case "M":
                    value *= 1e6;
                    break;
                case "B":
                case "bn":
                    value *= 1e9;
                    break;
            }
            return true;
        }
    }
}
