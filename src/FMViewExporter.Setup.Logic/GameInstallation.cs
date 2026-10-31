using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace FMViewExporter.Setup
{
    /// <summary>
    /// Installiert, aktualisiert und entfernt BepInEx und das Plugin im Spielordner.
    /// </summary>
    public sealed class GameInstallation
    {
        private static readonly char Sep = Path.DirectorySeparatorChar;
        public static readonly string PluginFolder = Path.Combine("BepInEx", "plugins", "FMViewExporter");
        public static readonly string PluginDll = Path.Combine(PluginFolder, "FMViewExporter.dll");
        public static readonly string ConfigFolder = Path.Combine("BepInEx", "config");

        /// <summary>Was das Komplettpaket neben fm.exe ablegt (ausser dem Ordner BepInEx).</summary>
        private static readonly string[] LoaderFiles =
        {
            "winhttp.dll", "doorstop_config.ini", ".doorstop_version", "changelog.txt", "FM View Exporter - LIESMICH.txt", "FM View Exporter - README.txt"
        };

        private static readonly string[] LoaderFolders = { "BepInEx", "dotnet" };

        public GameInstallation(string gameFolder)
        {
            GameFolder = gameFolder ?? "";
        }

        public string GameFolder { get; }

        public bool IsGameFolder => File.Exists(Path.Combine(GameFolder, SteamLibraries.GameExe));
        public bool HasBepInEx => File.Exists(Path.Combine(GameFolder, "winhttp.dll")) && Directory.Exists(Path.Combine(GameFolder, "BepInEx"));
        public bool HasPlugin => File.Exists(Path.Combine(GameFolder, PluginDll));
        public string ConfigPath => Path.Combine(GameFolder, ConfigFolder, ExporterSettings.ConfigFileName);

        /// <summary>Andere Plugins neben diesem; dann bleibt BepInEx beim Deinstallieren stehen.</summary>
        public List<string> OtherPlugins()
        {
            var result = new List<string>();
            var plugins = Path.Combine(GameFolder, "BepInEx", "plugins");
            if (!Directory.Exists(plugins))
                return result;
            foreach (var entry in Directory.GetFileSystemEntries(plugins))
            {
                var name = Path.GetFileName(entry);
                if (!string.Equals(name, "FMViewExporter", StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(name, "FMViewExporter.dll", StringComparison.OrdinalIgnoreCase))
                    result.Add(name);
            }
            return result;
        }

        /// <summary>
        /// Entpackt das Komplettpaket in den Spielordner. Vorhandene Einstellungen (BepInEx\config) bleiben
        /// unangetastet. <paramref name="pluginOnly"/> = nur die Plugin-Dateien (Update bei vorhandenem BepInEx).
        /// Liefert die Anzahl geschriebener Dateien.
        /// </summary>
        public int Install(Stream package, bool pluginOnly)
        {
            var written = 0;
            var root = Path.GetFullPath(GameFolder);
            using (var zip = new ZipArchive(package, ZipArchiveMode.Read))
            {
                foreach (var entry in zip.Entries)
                {
                    if (string.IsNullOrEmpty(entry.Name))
                        continue; // Ordner
                    var relative = entry.FullName.Replace('/', Sep).Replace('\\', Sep);
                    if (pluginOnly && !relative.StartsWith(PluginFolder + Sep, StringComparison.OrdinalIgnoreCase))
                        continue;
                    var target = Path.GetFullPath(Path.Combine(root, relative));
                    if (!target.StartsWith(root.TrimEnd(Sep) + Sep, StringComparison.OrdinalIgnoreCase))
                        throw new InvalidDataException("Ungueltiger Pfad im Paket: " + entry.FullName);
                    if (relative.StartsWith(ConfigFolder + Sep, StringComparison.OrdinalIgnoreCase) && File.Exists(target))
                        continue;

                    Directory.CreateDirectory(Path.GetDirectoryName(target));
                    using (var source = entry.Open())
                    using (var destination = File.Create(target))
                        source.CopyTo(destination);
                    written++;
                }
            }
            return written;
        }

        /// <summary>
        /// Entfernt das Plugin und seine Einstellungen. Ohne andere Plugins auch BepInEx selbst.
        /// Liefert die entfernten Eintraege.
        /// </summary>
        public List<string> Uninstall()
        {
            var removed = new List<string>();
            var keepLoader = OtherPlugins().Count > 0;

            DeleteDirectory(PluginFolder, removed);
            DeleteFile(Path.Combine("BepInEx", "plugins", "FMViewExporter.dll"), removed);
            DeleteFile(Path.Combine(ConfigFolder, ExporterSettings.ConfigFileName), removed);
            if (keepLoader)
                return removed;

            foreach (var file in LoaderFiles)
                DeleteFile(file, removed);
            foreach (var folder in LoaderFolders)
                DeleteDirectory(folder, removed);
            return removed;
        }

        private void DeleteFile(string relative, List<string> removed)
        {
            var path = Path.Combine(GameFolder, relative);
            if (!File.Exists(path))
                return;
            File.Delete(path);
            removed.Add(relative);
        }

        private void DeleteDirectory(string relative, List<string> removed)
        {
            var path = Path.Combine(GameFolder, relative);
            if (!Directory.Exists(path))
                return;
            Directory.Delete(path, true);
            removed.Add(relative + Sep);
        }
    }
}
