namespace FMViewExporter.Core
{
    /// <summary>
    /// Bestimmt Trennzeichen und Dezimalzeichen der CSV-Datei, passend zur Excel-Sprache.
    /// </summary>
    public enum CsvStyle
    {
        /// <summary>Komma als Trennzeichen, Punkt als Dezimalzeichen (englisches Excel).</summary>
        English,

        /// <summary>Semikolon als Trennzeichen, Komma als Dezimalzeichen (deutsches Excel).</summary>
        German,
    }

    public static class CsvStyleExtensions
    {
        public static char Delimiter(this CsvStyle style) => style == CsvStyle.German ? ';' : ',';

        public static char DecimalSeparator(this CsvStyle style) => style == CsvStyle.German ? ',' : '.';
    }
}
