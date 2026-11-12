using System;
using System.IO;
using FMViewExporter.Setup;
using Xunit;

namespace FMViewExporter.Core.Tests
{
    public class LinuxSteamTests
    {
        private const string Manifest = "\"AppState\"\n{\n\t\"appid\"\t\t\"1234567\"\n\t\"name\"\t\t\"Football Manager 27\"\n\t\"installdir\"\t\t\"Football Manager 27\"\n}\n";

        [Fact]
        public void Reads_app_id_and_folder_from_manifest()
        {
            var app = SteamLibraries.ParseManifest(Manifest);
            Assert.Equal("1234567", app.AppId);
            Assert.Equal("Football Manager 27", app.Name);
            Assert.Equal("Football Manager 27", app.InstallDir);
        }

        [Fact]
        public void Returns_null_without_app_id()
        {
            Assert.Null(SteamLibraries.ParseManifest("\"AppState\"\n{\n\t\"name\"\t\t\"Football Manager 27\"\n}\n"));
        }

        [Fact]
        public void Reads_linux_library_paths()
        {
            const string vdf = "\"libraryfolders\"\n{\n\t\"0\"\n\t{\n\t\t\"path\"\t\t\"/home/u/.local/share/Steam\"\n\t}\n\t\"1\"\n\t{\n\t\t\"path\"\t\t\"/mnt/Games SSD\"\n\t}\n}\n";
            Assert.Equal(new[] { "/home/u/.local/share/Steam", "/mnt/Games SSD" }, SteamLibraries.ParseLibraryFolders(vdf));
        }

        [Fact]
        public void Finds_game_in_second_linux_library()
        {
            var steam = "/home/u/.local/share/Steam";
            var expected = "/mnt/Games SSD/steamapps/common/Football Manager 27";
            var found = SteamLibraries.FindGameFolder(new[] { steam },
                path => path == "/home/u/.local/share/Steam/steamapps/libraryfolders.vdf"
                    ? "\"libraryfolders\"\n{\n\t\"0\"\n\t{\n\t\t\"path\"\t\t\"/home/u/.local/share/Steam\"\n\t}\n\t\"1\"\n\t{\n\t\t\"path\"\t\t\"/mnt/Games SSD\"\n\t}\n}\n"
                    : null,
                path => path == expected + "/fm.exe");
            Assert.Equal(expected, found);
        }

        [Fact]
        public void Lists_every_steam_root_on_linux()
        {
            var roots = SteamLibraries.LinuxSteamRoots("/home/u");
            Assert.Contains("/home/u/.local/share/Steam", roots);
            Assert.Contains("/home/u/.steam/steam", roots);
            Assert.Contains("/home/u/.var/app/com.valvesoftware.Steam/data/Steam", roots);
            Assert.Contains("/home/u/snap/steam/common/.local/share/Steam", roots);
        }

        [Fact]
        public void Finds_the_app_of_the_game_folder()
        {
            var root = Path.Combine(Path.GetTempPath(), "fmve-steam-" + Guid.NewGuid().ToString("N"));
            var steamapps = Path.Combine(root, "steamapps");
            var game = Path.Combine(steamapps, "common", "Football Manager 27");
            Directory.CreateDirectory(game);
            try
            {
                File.WriteAllText(Path.Combine(game, "fm.exe"), "");
                File.WriteAllText(Path.Combine(steamapps, "appmanifest_111111.acf"),
                    "\"AppState\"\n{\n\t\"appid\"\t\t\"111111\"\n\t\"name\"\t\t\"Football Manager 27\"\n\t\"installdir\"\t\t\"Football Manager 27\"\n}\n");
                // Eine andere App mit gleichem Namen darf den Ordner nicht beanspruchen.
                File.WriteAllText(Path.Combine(steamapps, "appmanifest_222222.acf"),
                    "\"AppState\"\n{\n\t\"appid\"\t\t\"222222\"\n\t\"name\"\t\t\"Football Manager 27\"\n\t\"installdir\"\t\t\"Football Manager 27 Demo\"\n}\n");

                var app = SteamLibraries.FindApp(game);
                Assert.Equal("111111", app.AppId);
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }
    }
}
