using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using System.Text.RegularExpressions;

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

        /// <summary>
        /// Fremde Erweiterungen neben diesem Plugin (BepInEx\plugins und BepInEx\patchers); dann bleibt
        /// BepInEx beim Deinstallieren stehen. Patcher werden als "patchers/Name" gemeldet.
        /// </summary>
        public List<string> OtherPlugins()
        {
            var result = new List<string>();
            var plugins = Path.Combine(GameFolder, "BepInEx", "plugins");
            if (Directory.Exists(plugins))
            {
                foreach (var entry in Directory.GetFileSystemEntries(plugins))
                {
                    var name = Path.GetFileName(entry);
                    if (!string.Equals(name, "FMViewExporter", StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(name, "FMViewExporter.dll", StringComparison.OrdinalIgnoreCase))
                        result.Add(name);
                }
            }
            var patchers = Path.Combine(GameFolder, "BepInEx", "patchers");
            if (Directory.Exists(patchers))
            {
                foreach (var entry in Directory.GetFileSystemEntries(patchers))
                    result.Add("patchers/" + Path.GetFileName(entry));
            }
            return result;
        }

        /// <summary>
        /// Dateien, ohne die BepInEx nicht startet. Geprueft werden nur die, die auch im Paket liegen.
        /// </summary>
        private static readonly string[] RuntimeFiles =
        {
            "winhttp.dll", "doorstop_config.ini",
            Path.Combine("BepInEx", "core", "BepInEx.Core.dll"),
            Path.Combine("BepInEx", "core", "BepInEx.Unity.IL2CPP.dll"),
            Path.Combine("dotnet", "coreclr.dll")
        };

        private static readonly string CoreDll = Path.Combine("BepInEx", "core", "BepInEx.Core.dll");

        /// <summary>Loader-Einstellungen, die bei vorhandener Datei nie ueberschrieben werden.</summary>
        private static readonly string[] KeptLoaderSettings = { "doorstop_config.ini" };

        /// <summary>
        /// Liest die Produktversion einer DLL (z. B. "6.0.0-be.755+3fab71a"). Austauschbar fuer Tests.
        /// </summary>
        public Func<string, string> ReadProductVersion { get; set; } = DefaultProductVersion;

        /// <summary>
        /// Prueft die vorhandene BepInEx-Laufzeit gegen das Paket.
        /// </summary>
        public RuntimeState CheckRuntime(Stream package)
        {
            using var zip = new ZipArchive(package, ZipArchiveMode.Read, leaveOpen: true);
            return CheckRuntime(zip);
        }

        private RuntimeState CheckRuntime(ZipArchive zip)
        {
            var entries = new Dictionary<string, ZipArchiveEntry>(StringComparer.OrdinalIgnoreCase);
            foreach (var entry in zip.Entries)
                entries[Normalize(entry.FullName)] = entry;

            var present = 0;
            var required = 0;
            foreach (var file in RuntimeFiles)
            {
                if (!entries.ContainsKey(file))
                    continue;
                required++;
                if (File.Exists(Path.Combine(GameFolder, file)))
                    present++;
            }
            if (present == 0 && !Directory.Exists(Path.Combine(GameFolder, "BepInEx", "core")))
                return RuntimeState.Missing;
            if (present < required)
                return RuntimeState.Incomplete;

            if (!entries.TryGetValue(CoreDll, out var packagedCore))
                return RuntimeState.Compatible;
            var installedCore = Path.Combine(GameFolder, CoreDll);
            if (SameContent(packagedCore, installedCore))
                return RuntimeState.Compatible;

            var installedBuild = BuildNumber(SafeRead(installedCore));
            var packagedBuild = BuildNumber(ReadPackagedVersion(packagedCore));
            if (installedBuild == null || packagedBuild == null)
                return RuntimeState.Unknown;
            return installedBuild >= packagedBuild ? RuntimeState.Compatible : RuntimeState.Outdated;
        }

        /// <summary>
        /// Waehlt, was installiert wird: bei vollstaendiger, passender (oder nicht bestimmbarer, also
        /// fremder) Laufzeit nur das Plugin, sonst das Komplettpaket (Neuinstallation oder Reparatur).
        /// </summary>
        public static InstallMode ChooseMode(RuntimeState state) =>
            state == RuntimeState.Compatible || state == RuntimeState.Unknown ? InstallMode.PluginOnly : InstallMode.Full;

        /// <summary>
        /// Installiert oder aktualisiert und waehlt den Umfang selbst (siehe <see cref="ChooseMode"/>).
        /// </summary>
        public InstallResult Install(Stream package)
        {
            using var zip = new ZipArchive(package, ZipArchiveMode.Read, leaveOpen: true);
            var state = CheckRuntime(zip);
            var mode = ChooseMode(state);
            return new InstallResult(state, mode, Install(zip, mode == InstallMode.PluginOnly));
        }

        /// <summary>
        /// Entpackt das Komplettpaket in den Spielordner. Vorhandene Einstellungen (BepInEx\config und
        /// doorstop_config.ini) bleiben unangetastet. <paramref name="pluginOnly"/> = nur die Plugin-Dateien
        /// (Update bei vorhandenem BepInEx). Liefert die Anzahl geschriebener Dateien.
        /// </summary>
        public int Install(Stream package, bool pluginOnly)
        {
            using var zip = new ZipArchive(package, ZipArchiveMode.Read, leaveOpen: true);
            return Install(zip, pluginOnly);
        }

        private int Install(ZipArchive zip, bool pluginOnly)
        {
            var written = 0;
            var root = Path.GetFullPath(GameFolder);
            foreach (var entry in zip.Entries)
            {
                if (string.IsNullOrEmpty(entry.Name))
                    continue; // Ordner
                var relative = Normalize(entry.FullName);
                if (pluginOnly && !relative.StartsWith(PluginFolder + Sep, StringComparison.OrdinalIgnoreCase))
                    continue;
                var target = Path.GetFullPath(Path.Combine(root, relative));
                if (!target.StartsWith(root.TrimEnd(Sep) + Sep, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidDataException("Ungueltiger Pfad im Paket: " + entry.FullName);
                if (File.Exists(target) && IsKeptSetting(relative))
                    continue;

                Directory.CreateDirectory(Path.GetDirectoryName(target));
                using (var source = entry.Open())
                using (var destination = File.Create(target))
                    source.CopyTo(destination);
                written++;
            }
            return written;
        }

        private static bool IsKeptSetting(string relative)
        {
            if (relative.StartsWith(ConfigFolder + Sep, StringComparison.OrdinalIgnoreCase))
                return true;
            foreach (var file in KeptLoaderSettings)
            {
                if (string.Equals(relative, file, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        private static string Normalize(string zipPath) => zipPath.Replace('/', Sep).Replace('\\', Sep);

        private static bool SameContent(ZipArchiveEntry entry, string path)
        {
            try
            {
                var info = new FileInfo(path);
                if (!info.Exists || info.Length != entry.Length)
                    return false;
                using var a = entry.Open();
                using var b = File.OpenRead(path);
                var bufA = new byte[81920];
                var bufB = new byte[81920];
                while (true)
                {
                    var n = ReadFull(a, bufA);
                    var m = ReadFull(b, bufB);
                    if (n != m)
                        return false;
                    if (n == 0)
                        return true;
                    for (var i = 0; i < n; i++)
                    {
                        if (bufA[i] != bufB[i])
                            return false;
                    }
                }
            }
            catch (IOException)
            {
                return false;
            }
        }

        private static int ReadFull(Stream stream, byte[] buffer)
        {
            var total = 0;
            while (total < buffer.Length)
            {
                var n = stream.Read(buffer, total, buffer.Length - total);
                if (n == 0)
                    break;
                total += n;
            }
            return total;
        }

        private string ReadPackagedVersion(ZipArchiveEntry entry)
        {
            var temp = Path.Combine(Path.GetTempPath(), "fmve-" + Guid.NewGuid().ToString("N") + ".dll");
            try
            {
                using (var source = entry.Open())
                using (var destination = File.Create(temp))
                    source.CopyTo(destination);
                return SafeRead(temp);
            }
            finally
            {
                try { File.Delete(temp); } catch (IOException) { }
            }
        }

        private string SafeRead(string path)
        {
            try
            {
                return File.Exists(path) ? ReadProductVersion(path) : null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string DefaultProductVersion(string path) => FileVersionInfo.GetVersionInfo(path).ProductVersion;

        /// <summary>Build-Nummer aus einer BepInEx-Version wie "6.0.0-be.755+3fab71a", sonst null.</summary>
        public static int? BuildNumber(string version)
        {
            if (string.IsNullOrEmpty(version))
                return null;
            var match = Regex.Match(version, @"be\.(\d+)", RegexOptions.IgnoreCase);
            return match.Success && int.TryParse(match.Groups[1].Value, out var build) ? build : (int?)null;
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

    /// <summary>Zustand der BepInEx-Laufzeit im Spielordner.</summary>
    public enum RuntimeState
    {
        /// <summary>Kein BepInEx vorhanden.</summary>
        Missing,
        /// <summary>BepInEx teilweise vorhanden, Pflichtdateien fehlen.</summary>
        Incomplete,
        /// <summary>Vollstaendig, aber aelter als der mitgelieferte Build.</summary>
        Outdated,
        /// <summary>Vollstaendig, Version nicht bestimmbar (z. B. von einer anderen Mod).</summary>
        Unknown,
        /// <summary>Vollstaendig und mindestens so neu wie der mitgelieferte Build.</summary>
        Compatible
    }

    public enum InstallMode
    {
        /// <summary>BepInEx und Plugin.</summary>
        Full,
        /// <summary>Nur die Plugin-Dateien.</summary>
        PluginOnly
    }

    public sealed class InstallResult
    {
        public InstallResult(RuntimeState runtime, InstallMode mode, int filesWritten)
        {
            Runtime = runtime;
            Mode = mode;
            FilesWritten = filesWritten;
        }

        public RuntimeState Runtime { get; }
        public InstallMode Mode { get; }
        public int FilesWritten { get; }
    }
}
