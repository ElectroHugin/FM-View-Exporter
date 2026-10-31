using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace FMViewExporter.Core
{
    public static class ExportFileNames
    {
        /// <summary>
        /// Baut einen Dateinamen ohne Endung, z. B. "squad_20261029_201500".
        /// </summary>
        public static string Build(string viewName, DateTime timestamp)
        {
            return Build("", viewName, timestamp);
        }

        /// <summary>
        /// Baut einen Dateinamen ohne Endung aus Praefix, Ansicht und optionalem Zeitstempel,
        /// z. B. "fc-bayern-squad" (ohne Zeitstempel) oder "squad_20261029_201500".
        /// </summary>
        public static string Build(string prefix, string viewName, DateTime? timestamp)
        {
            var name = Sanitize(viewName);
            var cleanPrefix = string.IsNullOrWhiteSpace(prefix) ? "" : Sanitize(prefix);
            if (cleanPrefix.Length > 0)
                name = cleanPrefix + "-" + name;
            if (timestamp.HasValue)
                name += "_" + timestamp.Value.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
            return name;
        }

        public static string Sanitize(string name)
        {
            var invalid = Path.GetInvalidFileNameChars();
            var builder = new StringBuilder(name.Length);
            foreach (var c in name.Trim().ToLowerInvariant())
            {
                if (char.IsWhiteSpace(c))
                    builder.Append('-');
                else if (Array.IndexOf(invalid, c) < 0 && c != '\\' && c != '/' && c != ':' && c != '*' && c != '?' && c != '"' && c != '<' && c != '>' && c != '|')
                    builder.Append(c);
            }
            return builder.Length == 0 ? "export" : builder.ToString();
        }
    }
}
