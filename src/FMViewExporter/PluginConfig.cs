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
            FilePrefix = config.Bind("Export", "FilePrefix", "",
                "Wird dem Dateinamen vorangestellt, z. B. fc-bayern -> fc-bayern-squad.csv. Leer = kein Praefix.");
            Timestamp = config.Bind("Export", "Timestamp", true,
                "true = Datum und Uhrzeit im Dateinamen (jeder Export eine neue Datei). false = fester Name, der letzte Export wird ueberschrieben.");
            Formats = config.Bind("Export", "Formats", ExportFormats.Both,
                "Both = CSV und HTML, CsvOnly = nur CSV, HtmlOnly = nur HTML.");
            ScanColumns = config.Bind("Export", "ScanColumns", false,
                "true = beim Export auch seitlich durch alle Spalten scrollen. Langsamer; noetig, falls das Spiel Spalten ausserhalb des Bildes leer laesst.");
            FramesPerStep = config.Bind("Export", "FramesPerStep", 3,
                "Frames, die das Spiel nach jedem Scrollschritt zum Befuellen der Zeilen bekommt. Bei fehlenden Werten erhoehen.");
        }

        public ConfigEntry<string> ExportHotkey { get; }
        public ConfigEntry<string> DiagnosticsHotkey { get; }
        public ConfigEntry<CsvStyle> CsvStyle { get; }
        public ConfigEntry<string> OutputFolder { get; }
        public ConfigEntry<string> FilePrefix { get; }
        public ConfigEntry<bool> Timestamp { get; }
        public ConfigEntry<ExportFormats> Formats { get; }
        public ConfigEntry<bool> ScanColumns { get; }
        public ConfigEntry<int> FramesPerStep { get; }
    }
}
