using System.Collections.Generic;
using System.IO;
using Microsoft.Win32;

namespace FMViewExporter.Setup
{
    /// <summary>Sucht FM27 ueber die Steam-Installation aus der Registry.</summary>
    internal static class GameLocator
    {
        public static string Find()
        {
            var roots = new List<string>
            {
                Read(Registry.CurrentUser, @"Software\Valve\Steam", "SteamPath"),
                Read(Registry.LocalMachine, @"SOFTWARE\WOW6432Node\Valve\Steam", "InstallPath"),
                Read(Registry.LocalMachine, @"SOFTWARE\Valve\Steam", "InstallPath"),
            };
            return SteamLibraries.FindGameFolder(roots,
                path => File.Exists(path) ? File.ReadAllText(path) : null,
                File.Exists);
        }

        private static string Read(RegistryKey hive, string key, string value)
        {
            try
            {
                using (var k = hive.OpenSubKey(key))
                    return (k?.GetValue(value) as string)?.Replace('/', '\\');
            }
            catch
            {
                return null;
            }
        }
    }
}
