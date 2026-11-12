using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using FMViewExporter.Setup;

namespace FMViewExporter.Setup.Cli
{
    /// <summary>
    /// Kommandozeile fuer Linux (Proton). Nutzt dieselbe Installationslogik wie das Windows-Setup:
    /// BepInEx und das Plugin landen neben fm.exe, Einstellungen in BepInEx\config.
    /// </summary>
    internal static class Program
    {
        /// <summary>Startoption fuer Steam: Wine soll winhttp.dll zuerst nativ laden.</summary>
        private const string LaunchOption = "WINEDLLOVERRIDES=\"winhttp.dll=n,b\" %command%";

        private static int Main(string[] args)
        {
            try
            {
                var options = Options.Parse(args);
                switch (options.Command)
                {
                    case "locate": return Locate(options);
                    case "install": return Install(options, false);
                    case "update": return Install(options, true);
                    case "uninstall": return Uninstall(options);
                    case "status": return Status(options);
                    case "settings": return Settings(options);
                    case "help": case "--help": case "-h": Usage(); return 0;
                    default:
                        Console.Error.WriteLine("Unknown command: " + options.Command);
                        Usage();
                        return 2;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error: " + ex.Message);
                return 1;
            }
        }

        private static void Usage()
        {
            Console.WriteLine("FM View Exporter setup for Linux (Proton)");
            Console.WriteLine();
            Console.WriteLine("  fmviewexporter-setup locate                     Show the FM27 folders found in the Steam libraries");
            Console.WriteLine("  fmviewexporter-setup install [--game DIR]       Install BepInEx and the plugin");
            Console.WriteLine("  fmviewexporter-setup update  [--game DIR]       Update the plugin (BepInEx stays if usable)");
            Console.WriteLine("  fmviewexporter-setup uninstall [--game DIR] [--yes]");
            Console.WriteLine("  fmviewexporter-setup status  [--game DIR]       Show what is installed");
            Console.WriteLine("  fmviewexporter-setup settings get <Section.Key>");
            Console.WriteLine("  fmviewexporter-setup settings set <Section.Key> <value>");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  --game DIR     Game folder with fm.exe (default: search the Steam libraries)");
            Console.WriteLine("  --package ZIP  Komplettpaket (default: a *komplettpaket*.zip next to this program)");
            Console.WriteLine("  --yes          Do not ask for confirmation");
        }

        // ---------------------------------------------------------------- locate

        private static int Locate(Options options)
        {
            var roots = SteamLibraries.LinuxSteamRoots(Home());
            Console.WriteLine("Steam installations searched:");
            foreach (var root in roots)
                Console.WriteLine("  " + root + (Directory.Exists(root) ? "" : "  (not there)"));

            var libraries = Libraries();
            Console.WriteLine();
            Console.WriteLine("Libraries:");
            foreach (var library in UniqueByRealPath(libraries))
                Console.WriteLine("  " + library + (Directory.Exists(library) ? "" : "  (not there)"));

            var folders = FindGameFolders(libraries);
            Console.WriteLine();
            if (folders.Count == 0)
            {
                Console.WriteLine("Football Manager 27 was not found in the Steam libraries.");
                Console.WriteLine("Use --game DIR to point at the folder that contains fm.exe.");
                return 1;
            }
            foreach (var folder in folders)
            {
                Console.WriteLine("Game folder: " + folder);
                PrintProtonInfo(folder);
            }
            return 0;
        }

        // ---------------------------------------------------------------- install

        private static int Install(Options options, bool updateOnly)
        {
            var folder = ResolveGameFolder(options);
            var game = new GameInstallation(folder);
            if (updateOnly && !game.HasPlugin)
            {
                Console.Error.WriteLine("FM View Exporter is not installed in " + folder + ". Use 'install' first.");
                return 1;
            }
            if (GameIsRunning(folder))
            {
                Console.Error.WriteLine("Football Manager is still running. Quit the game first.");
                return 1;
            }
            // Ob davor ein Update statt einer Neuinstallation war, entscheidet der alte Zustand.
            var update = game.HasPlugin;

            InstallResult result;
            using (var stream = File.OpenRead(ResolvePackage(options)))
                result = game.Install(stream);

            Console.WriteLine((update ? "FM View Exporter updated" : "FM View Exporter installed") + " in " + folder + ".");
            Console.WriteLine("Files written: " + result.FilesWritten + ", BepInEx: " + result.Runtime + ", mode: " + result.Mode + ".");
            if (result.Mode == InstallMode.Full && (result.Runtime == RuntimeState.Incomplete || result.Runtime == RuntimeState.Outdated))
                Console.WriteLine("BepInEx was incomplete or outdated and was renewed. Your doorstop_config.ini was kept.");
            if (update)
                Console.WriteLine("Your settings in " + game.ConfigPath + " were kept.");
            PrintProtonInfo(folder);
            Console.WriteLine("The first game start takes a few minutes while BepInEx prepares itself.");
            Console.WriteLine("Then press F9 in the squad or the player database.");
            return 0;
        }

        // ---------------------------------------------------------------- uninstall

        private static int Uninstall(Options options)
        {
            var folder = ResolveGameFolder(options);
            var game = new GameInstallation(folder);
            if (!game.HasPlugin && !game.HasBepInEx)
            {
                Console.WriteLine("Nothing to remove in " + folder + ".");
                return 0;
            }
            if (GameIsRunning(folder))
            {
                Console.Error.WriteLine("Football Manager is still running. Quit the game first.");
                return 1;
            }

            var others = game.OtherPlugins();
            if (others.Count > 0)
            {
                Console.WriteLine("BepInEx stays because other mods are installed: " + string.Join(", ", others));
                Console.WriteLine("Only FM View Exporter and its settings are removed.");
            }
            else
            {
                Console.WriteLine("This removes FM View Exporter, its settings and BepInEx from the game folder.");
                Console.WriteLine("Your saves are not affected.");
            }
            if (!options.Yes && !Confirm("Remove? [y/N] "))
            {
                Console.WriteLine("Nothing was removed.");
                return 0;
            }

            var removed = game.Uninstall();
            Console.WriteLine("Removed:");
            foreach (var entry in removed)
                Console.WriteLine("  " + entry);
            return 0;
        }

        // ---------------------------------------------------------------- status

        private static int Status(Options options)
        {
            var folder = ResolveGameFolder(options);
            var game = new GameInstallation(folder);
            Console.WriteLine("Game folder:      " + folder);
            Console.WriteLine("Plugin:           " + (game.HasPlugin ? "installed" : "not installed"));
            if (game.HasPlugin)
            {
                var version = InstalledVersion(game);
                Console.WriteLine("Version:          " + (version.Length > 0 ? version : "unknown"));
            }
            Console.WriteLine("BepInEx:          " + (game.HasBepInEx ? "installed" : "missing"));
            Console.WriteLine("Settings file:    " + game.ConfigPath + (File.Exists(game.ConfigPath) ? "" : " (created on the first game start)"));
            var others = game.OtherPlugins();
            if (others.Count > 0)
                Console.WriteLine("Other mods:       " + string.Join(", ", others));
            Console.WriteLine();
            PrintProtonInfo(folder);
            return 0;
        }

        // ---------------------------------------------------------------- settings

        private static int Settings(Options options)
        {
            var action = options.Argument(0);
            var key = options.Argument(1);
            if (action != "get" && action != "set")
                throw new ArgumentException("Use 'settings get <Section.Key>' or 'settings set <Section.Key> <value>'.");
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Missing setting, for example Export.FilePrefix.");

            var split = key.IndexOf('.');
            if (split <= 0 || split == key.Length - 1)
                throw new ArgumentException("The setting needs a section and a key, for example Export.FilePrefix.");
            var section = key.Substring(0, split);
            var name = key.Substring(split + 1);

            var definition = ExporterSettings.All.FirstOrDefault(s =>
                string.Equals(s.Section, section, StringComparison.OrdinalIgnoreCase)
                && string.Equals(s.Key, name, StringComparison.OrdinalIgnoreCase));
            if (definition == null)
                throw new ArgumentException("Unknown setting: " + key + ". Known settings: " + string.Join(", ", ExporterSettings.All.Select(s => s.Section + "." + s.Key)) + ".");

            var folder = ResolveGameFolder(options);
            var game = new GameInstallation(folder);
            var config = ReadConfig(game.ConfigPath);

            if (action == "get")
            {
                var value = config.Get(definition.Section, definition.Key);
                Console.WriteLine((value ?? definition.DefaultValue) + (value == null ? "  (default)" : ""));
                return 0;
            }

            var raw = options.Argument(2);
            if (raw == null)
                throw new ArgumentException("Missing value for " + key + ".");
            var valueToWrite = Validate(definition, raw);
            config.Set(definition.Section, definition.Key, valueToWrite);
            Directory.CreateDirectory(Path.GetDirectoryName(game.ConfigPath));
            File.WriteAllText(game.ConfigPath, config.ToString(), new UTF8Encoding(false));
            Console.WriteLine(definition.Section + "." + definition.Key + " = " + valueToWrite);
            Console.WriteLine("Saved to " + game.ConfigPath);
            return 0;
        }

        /// <summary>Prueft den Wert gegen die Art der Einstellung und gibt die gespeicherte Form zurueck.</summary>
        private static string Validate(SettingDefinition setting, string value)
        {
            switch (setting.Kind)
            {
                case SettingKind.Boolean:
                    if (string.Equals(value, "true", StringComparison.OrdinalIgnoreCase))
                        return "true";
                    if (string.Equals(value, "false", StringComparison.OrdinalIgnoreCase))
                        return "false";
                    throw new ArgumentException(setting.Section + "." + setting.Key + " is true or false.");
                case SettingKind.Choice:
                    var choice = setting.Choices.FirstOrDefault(c => string.Equals(c.Value, value, StringComparison.OrdinalIgnoreCase));
                    if (choice == null)
                        throw new ArgumentException(setting.Section + "." + setting.Key + " is one of: " + string.Join(", ", setting.Choices.Select(c => c.Value)) + ".");
                    return choice.Value;
                case SettingKind.Number:
                    int number;
                    if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out number))
                        throw new ArgumentException(setting.Section + "." + setting.Key + " is a whole number.");
                    return number.ToString(CultureInfo.InvariantCulture);
                default:
                    return value.Trim();
            }
        }

        // ---------------------------------------------------------------- helpers

        private static string ResolveGameFolder(Options options)
        {
            if (!string.IsNullOrWhiteSpace(options.Game))
            {
                var folder = Path.GetFullPath(options.Game.Trim());
                if (!File.Exists(Path.Combine(folder, SteamLibraries.GameExe)))
                    throw new ArgumentException("There is no " + SteamLibraries.GameExe + " in " + folder + ".");
                return folder;
            }

            var folders = FindGameFolders(Libraries());
            if (folders.Count == 0)
                throw new ArgumentException("Football Manager 27 was not found in the Steam libraries. Use --game DIR to point at the folder that contains fm.exe.");
            if (folders.Count > 1)
                throw new ArgumentException("Football Manager 27 was found more than once. Use --game DIR: " + string.Join(", ", folders));
            return folders[0];
        }

        /// <summary>
        /// Alle Bibliotheken, in denen nach dem Spiel gesucht wird: die Steam-Installationen
        /// selbst plus die Eintraege aus ihren libraryfolders.vdf.
        /// </summary>
        private static List<string> Libraries()
        {
            var roots = SteamLibraries.LinuxSteamRoots(Home());
            return SteamLibraries.Libraries(SteamLibraries.Libraries(roots, ReadFile), ReadFile);
        }

        /// <summary>Alle Spielordner, ohne dieselben Ordner doppelt zu zaehlen (Steam verlinkt oft).</summary>
        private static List<string> FindGameFolders(IEnumerable<string> libraries)
        {
            return UniqueByRealPath(SteamLibraries.FindGameFolders(libraries, ReadFile, File.Exists));
        }

        /// <summary>Ordner ohne Doppelungen, gleiche Ordner auch ueber Symlinks nur einmal.</summary>
        private static List<string> UniqueByRealPath(IEnumerable<string> paths)
        {
            var result = new List<string>();
            var seen = new List<string>();
            foreach (var path in paths)
            {
                var real = RealPath(path);
                if (seen.Any(s => s == real))
                    continue;
                seen.Add(real);
                result.Add(path);
            }
            return result;
        }

        /// <summary>Loest Symlinks auf, damit derselbe Ordner nur einmal gefunden wird.</summary>
        private static string RealPath(string path)
        {
            var full = Path.GetFullPath(path);
            var root = Path.GetPathRoot(full);
            var current = root;
            foreach (var part in full.Substring(root.Length).Split('/', '\\'))
            {
                if (part.Length == 0)
                    continue;
                var next = Path.Combine(current, part);
                var info = new DirectoryInfo(next);
                if (info.Exists)
                {
                    var target = info.ResolveLinkTarget(true);
                    if (target != null)
                        next = target.FullName;
                }
                current = next;
            }
            return current.Length > 1 ? current.TrimEnd('/') : current;
        }

        private static string ResolvePackage(Options options)
        {
            if (!string.IsNullOrWhiteSpace(options.Package))
            {
                var path = Path.GetFullPath(options.Package.Trim());
                if (!File.Exists(path))
                    throw new FileNotFoundException("Package not found: " + path);
                return path;
            }
            var directory = AppContext.BaseDirectory;
            var candidates = Directory.Exists(directory)
                ? Directory.GetFiles(directory, "*komplettpaket*.zip").OrderBy(f => f).ToList()
                : new List<string>();
            if (candidates.Count == 0)
                throw new FileNotFoundException("No Komplettpaket found next to this program (" + directory + "). Use --package ZIP.");
            if (candidates.Count > 1)
                throw new FileNotFoundException("More than one Komplettpaket next to this program. Use --package ZIP: " + string.Join(", ", candidates));
            return candidates[0];
        }

        /// <summary>
        /// Prueft, ob das Spiel aus genau diesem Ordner laeuft. Unter Proton heisst der Prozess
        /// fm.exe; entscheidend ist sein Arbeitsverzeichnis, damit ein FM27 aus einer anderen
        /// Installation nicht blockiert.
        /// </summary>
        private static bool GameIsRunning(string gameFolder)
        {
            var wanted = RealPath(gameFolder);
            foreach (var id in Directory.GetDirectories("/proc"))
            {
                var name = Path.GetFileName(id);
                if (name.Length == 0 || !char.IsDigit(name[0]))
                    continue;
                if (!IsGameProcess(id))
                    continue;
                var cwd = ReadLink(Path.Combine(id, "cwd"));
                if (cwd == null)
                    return true; // Arbeitsverzeichnis unbekannt: vorsichtshalber blockieren.
                var real = RealPath(cwd);
                if (real == wanted || real.StartsWith(wanted + "/", StringComparison.Ordinal))
                    return true;
            }
            return false;
        }

        private static bool IsGameProcess(string processDir)
        {
            try
            {
                var comm = File.ReadAllText(Path.Combine(processDir, "comm")).Trim();
                return string.Equals(comm, "fm", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(comm, "fm.exe", StringComparison.OrdinalIgnoreCase);
            }
            catch (IOException)
            {
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        private static string ReadLink(string path)
        {
            try
            {
                return Directory.ResolveLinkTarget(path, true)?.FullName;
            }
            catch (IOException)
            {
                return null;
            }
            catch (UnauthorizedAccessException)
            {
                return null;
            }
        }

        private static string InstalledVersion(GameInstallation game)
        {
            try
            {
                var info = FileVersionInfo.GetVersionInfo(Path.Combine(game.GameFolder, GameInstallation.PluginDll));
                return info.ProductVersion ?? info.FileVersion ?? "";
            }
            catch
            {
                return "";
            }
        }

        private static BepInExConfigFile ReadConfig(string path)
        {
            try
            {
                return new BepInExConfigFile(File.Exists(path) ? File.ReadAllText(path) : "");
            }
            catch
            {
                return new BepInExConfigFile("");
            }
        }

        /// <summary>
        /// Zeigt die Proton-Angaben: Startoption, Prefix des Spiels und der Ordner, in dem die
        /// Exporte landen. Alles wird nur angezeigt, nichts am Prefix geaendert.
        /// </summary>
        private static void PrintProtonInfo(string gameFolder)
        {
            Console.WriteLine();
            Console.WriteLine("Proton:");
            Console.WriteLine("  Launch option in Steam (Properties > Launch Options):");
            Console.WriteLine("    " + LaunchOption);

            var app = SteamLibraries.FindApp(gameFolder);
            if (app == null)
            {
                Console.WriteLine("  App ID of FM27 not found, so the Proton prefix cannot be shown.");
                return;
            }
            var prefix = ProtonPrefix(gameFolder, app.AppId);
            Console.WriteLine("  Proton prefix: " + prefix);
            var documents = PrefixDocuments(prefix);
            if (documents != null)
                Console.WriteLine("  Exports land in: " + documents);
            else
                Console.WriteLine("  Exports land in the prefix's Documents folder (created on the first game start).");
        }

        /// <summary>Der Prefix liegt in der Bibliothek, in der auch das Spiel installiert ist.</summary>
        private static string ProtonPrefix(string gameFolder, string appId)
        {
            var library = Path.GetFullPath(Path.Combine(gameFolder, "..", "..", ".."));
            return Path.Combine(library, "steamapps", "compatdata", appId, "pfx");
        }

        /// <summary>Der Dokumentenordner im Prefix, falls der Prefix schon existiert.</summary>
        private static string PrefixDocuments(string prefix)
        {
            var users = Path.Combine(prefix, "drive_c", "users");
            if (!Directory.Exists(users))
                return null;
            // Proton benutzt "steamuser"; "Public" ist ein allgemeiner Ordner ohne Dokumente.
            var candidates = Directory.GetDirectories(users)
                .Where(u => !string.Equals(Path.GetFileName(u), "Public", StringComparison.OrdinalIgnoreCase))
                .OrderBy(u => u, StringComparer.OrdinalIgnoreCase)
                .ToList();
            foreach (var user in candidates)
            {
                var documents = Path.Combine(user, "Documents");
                if (Directory.Exists(documents))
                    return Path.Combine(documents, "Sports Interactive", "FM View Exporter");
            }
            return null;
        }

        private static string Home() => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        private static string ReadFile(string path) => File.Exists(path) ? File.ReadAllText(path) : null;

        private static bool Confirm(string question)
        {
            Console.Write(question);
            var answer = Console.ReadLine();
            return answer != null && (answer.Trim().Equals("y", StringComparison.OrdinalIgnoreCase) || answer.Trim().Equals("yes", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>Kommando, Optionen und freie Argumente.</summary>
        private sealed class Options
        {
            public string Command { get; private set; }
            public string Game { get; private set; }
            public string Package { get; private set; }
            public bool Yes { get; private set; }
            private readonly List<string> _arguments = new List<string>();

            public string Argument(int index) => index < _arguments.Count ? _arguments[index] : null;

            public static Options Parse(string[] args)
            {
                var options = new Options();
                if (args.Length == 0)
                {
                    options.Command = "help";
                    return options;
                }
                options.Command = args[0].ToLowerInvariant();
                for (var i = 1; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "--game":
                            options.Game = Next(args, ref i, "--game");
                            break;
                        case "--package":
                            options.Package = Next(args, ref i, "--package");
                            break;
                        case "--yes":
                        case "-y":
                            options.Yes = true;
                            break;
                        default:
                            if (args[i].StartsWith("--"))
                                throw new ArgumentException("Unknown option: " + args[i]);
                            options._arguments.Add(args[i]);
                            break;
                    }
                }
                return options;
            }

            private static string Next(string[] args, ref int index, string name)
            {
                if (index + 1 >= args.Length)
                    throw new ArgumentException(name + " needs a value.");
                return args[++index];
            }
        }
    }
}
