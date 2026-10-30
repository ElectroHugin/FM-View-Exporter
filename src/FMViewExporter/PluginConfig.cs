using BepInEx.Configuration;
using FMViewExporter.Core;

namespace FMViewExporter
{
    /// <summary>
    /// Einstellungen in BepInEx\config\de.electrohugin.fmviewexporter.cfg.
    /// </summary>
    internal sealed class PluginConfig
    {
        public PluginConfig(ConfigFile config)
        {
            ExportHotkey = config.Bind("Hotkeys", "Export", "F9",
                "Taste zum Exportieren der angezeigten Spielerliste. Beispiele: F9, Ctrl+Shift+E.");
            DiagnosticsHotkey = config.Bind("Hotkeys", "Diagnose", "F10",
                "Taste fuer ein Abbild der Oberflaeche (hilft bei der Anpassung an neue FM-Versionen).");
            CsvStyle = config.Bind("Export", "CsvStyle", Core.CsvStyle.English,
                "English = Komma als Trennzeichen (englisches Excel), German = Semikolon (deutsches Excel).");
            OutputFolder = config.Bind("Export", "OutputFolder", "",
                "Zielordner. Leer = Dokumente\\Sports Interactive\\FM View Exporter.");
            ScanColumns = config.Bind("Export", "ScanColumns", false,
                "true = beim Export auch seitlich durch alle Spalten scrollen. Langsamer; noetig, falls das Spiel Spalten ausserhalb des Bildes leer laesst.");
            FramesPerStep = config.Bind("Export", "FramesPerStep", 3,
                "Frames, die das Spiel nach jedem Scrollschritt zum Befuellen der Zeilen bekommt. Bei fehlenden Werten erhoehen.");
        }

        public ConfigEntry<string> ExportHotkey { get; }
        public ConfigEntry<string> DiagnosticsHotkey { get; }
        public ConfigEntry<CsvStyle> CsvStyle { get; }
        public ConfigEntry<string> OutputFolder { get; }
        public ConfigEntry<bool> ScanColumns { get; }
        public ConfigEntry<int> FramesPerStep { get; }
    }
}
