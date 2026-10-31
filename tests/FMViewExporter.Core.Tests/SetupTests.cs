using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using FMViewExporter.Setup;
using Xunit;

namespace FMViewExporter.Core.Tests
{
    public class SteamLibrariesTests
    {
        private const string Vdf = "\"libraryfolders\"\n{\n\t\"0\"\n\t{\n\t\t\"path\"\t\t\"C:\\\\Program Files (x86)\\\\Steam\"\n\t\t\"apps\" { }\n\t}\n\t\"1\"\n\t{\n\t\t\"path\"\t\t\"T:\\\\SteamUbisoft\"\n\t}\n}\n";

        [Fact]
        public void Reads_library_paths_from_vdf()
        {
            Assert.Equal(new[] { @"C:\Program Files (x86)\Steam", @"T:\SteamUbisoft" }, SteamLibraries.ParseLibraryFolders(Vdf));
        }

        [Fact]
        public void Finds_game_in_second_library()
        {
            var steam = @"C:\Program Files (x86)\Steam";
            var expected = Path.Combine(@"T:\SteamUbisoft", "steamapps", "common", "Football Manager 27");
            var found = SteamLibraries.FindGameFolder(new[] { steam },
                path => path == Path.Combine(steam, "steamapps", "libraryfolders.vdf") ? Vdf : null,
                path => path == Path.Combine(expected, "fm.exe"));
            Assert.Equal(expected, found);
        }

        [Fact]
        public void Returns_null_without_game()
        {
            Assert.Null(SteamLibraries.FindGameFolder(new[] { "", @"C:\Steam" }, _ => null, _ => false));
        }
    }

    public class BepInExConfigFileTests
    {
        private const string Cfg = "[Export]\n\n## Zielordner.\n# Setting type: String\n# Default value: \nOutputFolder = \n\n## Stil\nCsvStyle = English\n\n[Hotkeys]\n\nExport = F9\n";

        [Fact]
        public void Reads_values()
        {
            var cfg = new BepInExConfigFile(Cfg);
            Assert.Equal("English", cfg.Get("Export", "CsvStyle"));
            Assert.Equal("", cfg.Get("Export", "OutputFolder"));
            Assert.Equal("F9", cfg.Get("Hotkeys", "Export"));
            Assert.Null(cfg.Get("Export", "FilePrefix"));
        }

        [Fact]
        public void Changes_value_and_keeps_comments()
        {
            var cfg = new BepInExConfigFile(Cfg);
            cfg.Set("Export", "CsvStyle", "German");
            var text = cfg.ToString();
            Assert.Contains("CsvStyle = German", text);
            Assert.Contains("## Zielordner.", text);
            Assert.Equal("German", new BepInExConfigFile(text).Get("Export", "CsvStyle"));
        }

        [Fact]
        public void Adds_missing_key_to_its_section()
        {
            var cfg = new BepInExConfigFile(Cfg);
            cfg.Set("Export", "FilePrefix", "fc-bayern");
            var text = cfg.ToString();
            Assert.True(text.IndexOf("FilePrefix = fc-bayern", StringComparison.Ordinal) < text.IndexOf("[Hotkeys]", StringComparison.Ordinal));
            Assert.Equal("fc-bayern", new BepInExConfigFile(text).Get("Export", "FilePrefix"));
        }

        [Fact]
        public void Creates_file_from_scratch()
        {
            var cfg = new BepInExConfigFile("");
            cfg.Set("Export", "Timestamp", "false");
            cfg.Set("Hotkeys", "Export", "F8");
            var reread = new BepInExConfigFile(cfg.ToString());
            Assert.Equal("false", reread.Get("Export", "Timestamp"));
            Assert.Equal("F8", reread.Get("Hotkeys", "Export"));
        }
    }

    public class GameInstallationTests : IDisposable
    {
        private readonly string _game = Path.Combine(Path.GetTempPath(), "fmve-setup-" + Guid.NewGuid().ToString("N"));

        public GameInstallationTests()
        {
            Directory.CreateDirectory(_game);
            File.WriteAllText(Path.Combine(_game, "fm.exe"), "");
        }

        public void Dispose() => Directory.Delete(_game, true);

        private static MemoryStream Package()
        {
            var stream = new MemoryStream();
            using (var zip = new ZipArchive(stream, ZipArchiveMode.Create, true))
            {
                foreach (var name in new[] { "winhttp.dll", "doorstop_config.ini", "BepInEx/core/BepInEx.Core.dll",
                             "BepInEx/plugins/FMViewExporter/FMViewExporter.dll", "BepInEx/config/de.electrohugin.fmviewexporter.cfg" })
                {
                    using var writer = new StreamWriter(zip.CreateEntry(name).Open());
                    writer.Write("new " + name);
                }
            }
            stream.Position = 0;
            return stream;
        }

        private string Read(string relative) => File.ReadAllText(Path.Combine(new[] { _game }.Concat(relative.Split('/')).ToArray()));

        [Fact]
        public void Installs_package_and_keeps_existing_settings()
        {
            var config = Path.Combine(_game, "BepInEx", "config");
            Directory.CreateDirectory(config);
            File.WriteAllText(Path.Combine(config, "de.electrohugin.fmviewexporter.cfg"), "mine");

            var install = new GameInstallation(_game);
            Assert.True(install.IsGameFolder);
            install.Install(Package(), pluginOnly: false);

            Assert.True(install.HasBepInEx);
            Assert.True(install.HasPlugin);
            Assert.Equal("mine", Read("BepInEx/config/de.electrohugin.fmviewexporter.cfg"));
        }

        [Fact]
        public void Update_replaces_only_the_plugin()
        {
            File.WriteAllText(Path.Combine(_game, "winhttp.dll"), "old");
            new GameInstallation(_game).Install(Package(), pluginOnly: true);
            Assert.Equal("old", Read("winhttp.dll"));
            Assert.Equal("new BepInEx/plugins/FMViewExporter/FMViewExporter.dll", Read("BepInEx/plugins/FMViewExporter/FMViewExporter.dll"));
        }

        [Fact]
        public void Uninstall_removes_everything_without_other_plugins()
        {
            var install = new GameInstallation(_game);
            install.Install(Package(), pluginOnly: false);
            install.Uninstall();
            Assert.False(File.Exists(Path.Combine(_game, "winhttp.dll")));
            Assert.False(Directory.Exists(Path.Combine(_game, "BepInEx")));
            Assert.True(File.Exists(Path.Combine(_game, "fm.exe")));
        }

        [Fact]
        public void Uninstall_keeps_bepinex_for_other_plugins()
        {
            var install = new GameInstallation(_game);
            install.Install(Package(), pluginOnly: false);
            Directory.CreateDirectory(Path.Combine(_game, "BepInEx", "plugins", "OtherMod"));
            install.Uninstall();
            Assert.False(install.HasPlugin);
            Assert.True(install.HasBepInEx);
        }
    }
}
