using Microsoft.Win32;

namespace FMViewExporter.Setup
{
    /// <summary>Einstellungen des Setup-Programms selbst (nicht des Plugins), unter HKCU gespeichert.</summary>
    internal static class Preferences
    {
        private const string KeyPath = @"Software\ElectroHugin\FM View Exporter Setup";

        /// <summary>"en" (Standard) oder "de".</summary>
        public static string Language
        {
            get
            {
                try
                {
                    using (var key = Registry.CurrentUser.OpenSubKey(KeyPath))
                        return key?.GetValue("Language") as string == "de" ? "de" : "en";
                }
                catch
                {
                    return "en";
                }
            }
            set
            {
                try
                {
                    using (var key = Registry.CurrentUser.CreateSubKey(KeyPath))
                        key?.SetValue("Language", value);
                }
                catch
                {
                }
            }
        }
    }
}
