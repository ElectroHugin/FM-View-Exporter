using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace FMViewExporter.Setup
{
    /// <summary>
    /// Findet den Spielordner von Football Manager in den Steam-Bibliotheken.
    /// </summary>
    public static class SteamLibraries
    {
        public const string GameFolderName = "Football Manager 27";
        public const string GameExe = "fm.exe";

        private static readonly Regex PathEntry = new Regex("\"path\"\\s+\"((?:[^\"\\\\]|\\\\.)*)\"", RegexOptions.IgnoreCase);

        /// <summary>Liest die Bibliotheksordner aus steamapps\libraryfolders.vdf.</summary>
        public static List<string> ParseLibraryFolders(string vdfText)
        {
            var result = new List<string>();
            foreach (Match match in PathEntry.Matches(vdfText ?? ""))
            {
                var path = Regex.Unescape(match.Groups[1].Value);
                if (path.Length > 0 && !result.Contains(path, StringComparer.OrdinalIgnoreCase))
                    result.Add(path);
            }
            return result;
        }

        /// <summary>
        /// Sucht den Spielordner in allen Bibliotheken der angegebenen Steam-Installationen.
        /// <paramref name="readFile"/> liefert den Dateiinhalt oder null, <paramref name="fileExists"/> prueft eine Datei.
        /// </summary>
        public static string FindGameFolder(IEnumerable<string> steamRoots, Func<string, string> readFile, Func<string, bool> fileExists)
        {
            var libraries = new List<string>();
            foreach (var root in steamRoots)
            {
                if (string.IsNullOrWhiteSpace(root))
                    continue;
                AddUnique(libraries, root);
                var vdf = readFile(Path.Combine(root, "steamapps", "libraryfolders.vdf"));
                if (vdf != null)
                {
                    foreach (var library in ParseLibraryFolders(vdf))
                        AddUnique(libraries, library);
                }
            }

            foreach (var library in libraries)
            {
                var folder = Path.Combine(library, "steamapps", "common", GameFolderName);
                if (fileExists(Path.Combine(folder, GameExe)))
                    return folder;
            }
            return null;
        }

        private static void AddUnique(List<string> list, string path)
        {
            var normalized = path.TrimEnd('\\', '/');
            if (!list.Contains(normalized, StringComparer.OrdinalIgnoreCase))
                list.Add(normalized);
        }

        private static bool Contains(this List<string> list, string value, StringComparer comparer)
        {
            foreach (var item in list)
            {
                if (comparer.Equals(item, value))
                    return true;
            }
            return false;
        }
    }
}
