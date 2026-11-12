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
        private static readonly Regex AcfAppId = new Regex("\"appid\"\\s+\"(\\d+)\"", RegexOptions.IgnoreCase);
        private static readonly Regex AcfName = new Regex("\"name\"\\s+\"([^\"]*)\"", RegexOptions.IgnoreCase);
        private static readonly Regex AcfInstallDir = new Regex("\"installdir\"\\s+\"([^\"]*)\"", RegexOptions.IgnoreCase);

        /// <summary>Angaben zu einer Steam-App aus einer appmanifest_*.acf.</summary>
        public sealed class SteamApp
        {
            public string AppId { get; set; }
            public string Name { get; set; }
            public string InstallDir { get; set; }
        }

        /// <summary>Liest App-ID, Name und Installationsordner aus einer appmanifest_*.acf.</summary>
        public static SteamApp ParseManifest(string acfText)
        {
            if (string.IsNullOrWhiteSpace(acfText))
                return null;
            var id = AcfAppId.Match(acfText);
            if (!id.Success)
                return null;
            var name = AcfName.Match(acfText);
            var dir = AcfInstallDir.Match(acfText);
            return new SteamApp
            {
                AppId = id.Groups[1].Value,
                Name = name.Success ? name.Groups[1].Value : "",
                InstallDir = dir.Success ? dir.Groups[1].Value : "",
            };
        }

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

        /// <summary>Steam-Installationen unter Linux: nativ, ~/.steam (oft Symlink), Flatpak und Snap.</summary>
        public static List<string> LinuxSteamRoots(string home) => new List<string>
        {
            Path.Combine(home, ".local", "share", "Steam"),
            Path.Combine(home, ".steam", "steam"),
            Path.Combine(home, ".var", "app", "com.valvesoftware.Steam", "data", "Steam"),
            Path.Combine(home, "snap", "steam", "common", ".local", "share", "Steam"),
        };

        /// <summary>
        /// Sucht die Steam-App zum Spielordner. Der Ordner liegt in steamapps\common, die
        /// appmanifest_*.acf daneben in steamapps. Gesucht wird zuerst nach dem Ordnernamen
        /// (installdir), weil der so zum Ordner auf der Platte passt; der Anzeigename ist nur
        /// der zweite Versuch.
        /// </summary>
        public static SteamApp FindApp(string gameFolder)
        {
            var folder = gameFolder ?? "";
            var library = Path.GetFullPath(Path.Combine(folder, "..", "..", ".."));
            var steamapps = Path.Combine(library, "steamapps");
            if (!Directory.Exists(steamapps))
                return null;
            var installDir = Path.GetFileName(folder.TrimEnd('/', '\\'));

            SteamApp byName = null;
            foreach (var file in Directory.GetFiles(steamapps, "appmanifest_*.acf"))
            {
                SteamApp app;
                try
                {
                    app = ParseManifest(File.ReadAllText(file));
                }
                catch (IOException)
                {
                    continue;
                }
                if (app == null)
                    continue;
                if (string.Equals(app.InstallDir, installDir, StringComparison.OrdinalIgnoreCase))
                    return app;
                if (byName == null && string.Equals(app.Name, GameFolderName, StringComparison.OrdinalIgnoreCase))
                    byName = app;
            }
            return byName;
        }

        /// <summary>
        /// Sucht den Spielordner in allen Bibliotheken der angegebenen Steam-Installationen.
        /// <paramref name="readFile"/> liefert den Dateiinhalt oder null, <paramref name="fileExists"/> prueft eine Datei.
        /// </summary>
        public static string FindGameFolder(IEnumerable<string> steamRoots, Func<string, string> readFile, Func<string, bool> fileExists)
        {
            var found = FindGameFolders(steamRoots, readFile, fileExists);
            return found.Count > 0 ? found[0] : null;
        }

        /// <summary>Wie <see cref="FindGameFolder"/>, liefert aber alle Treffer (mehrere Steam-Installationen).</summary>
        public static List<string> FindGameFolders(IEnumerable<string> steamRoots, Func<string, string> readFile, Func<string, bool> fileExists)
        {
            var result = new List<string>();
            foreach (var library in Libraries(steamRoots, readFile))
            {
                var folder = Path.Combine(library, "steamapps", "common", GameFolderName);
                if (fileExists(Path.Combine(folder, GameExe)))
                    result.Add(folder);
            }
            return result;
        }

        /// <summary>Alle Bibliotheksordner der angegebenen Steam-Installationen.</summary>
        public static List<string> Libraries(IEnumerable<string> steamRoots, Func<string, string> readFile)
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
            return libraries;
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
