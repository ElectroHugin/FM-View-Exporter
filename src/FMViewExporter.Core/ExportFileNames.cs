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
            return Sanitize(viewName) + "_" + timestamp.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
        }

        public static string Sanitize(string name)
        {
            var invalid = Path.GetInvalidFileNameChars();
            var builder = new StringBuilder(name.Length);
            foreach (var c in name.Trim().ToLowerInvariant())
            {
                if (char.IsWhiteSpace(c))
                    builder.Append('_');
                else if (Array.IndexOf(invalid, c) < 0 && c != '\\' && c != '/' && c != ':' && c != '*' && c != '?' && c != '"' && c != '<' && c != '>' && c != '|')
                    builder.Append(c);
            }
            return builder.Length == 0 ? "export" : builder.ToString();
        }
    }
}
