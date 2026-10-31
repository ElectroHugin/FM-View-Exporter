using System.Collections.Generic;

namespace FMViewExporter.Setup
{
    public enum SettingKind
    {
        Text,
        Folder,
        Boolean,
        Choice,
        Number
    }

    /// <summary>Eine Einstellung des Plugins, wie sie in der cfg-Datei steht.</summary>
    public sealed class SettingDefinition
    {
        public SettingDefinition(string section, string key, SettingKind kind, string defaultValue, string labelDe, string labelEn, string helpDe, string helpEn, params string[] choices)
        {
            Section = section;
            Key = key;
            Kind = kind;
            DefaultValue = defaultValue;
            LabelDe = labelDe;
            LabelEn = labelEn;
            HelpDe = helpDe;
            HelpEn = helpEn;
            Choices = choices;
        }

        public string Section { get; }
        public string Key { get; }
        public SettingKind Kind { get; }
        public string DefaultValue { get; }
        public string LabelDe { get; }
        public string LabelEn { get; }
        public string HelpDe { get; }
        public string HelpEn { get; }
        public string[] Choices { get; }
    }

    /// <summary>
    /// Die Einstellungen aus src/FMViewExporter/PluginConfig.cs. Neue Einstellungen dort auch hier eintragen.
    /// </summary>
    public static class ExporterSettings
    {
        public const string ConfigFileName = "de.electrohugin.fmviewexporter.cfg";

        public static readonly IReadOnlyList<SettingDefinition> All = new[]
        {
            new SettingDefinition("Export", "FilePrefix", SettingKind.Text, "",
                "Dateiname-Präfix", "File name prefix",
                "z. B. fc-bayern ergibt fc-bayern-squad.csv; leer = kein Präfix",
                "e.g. fc-bayern gives fc-bayern-squad.csv; empty = no prefix"),
            new SettingDefinition("Export", "Timestamp", SettingKind.Boolean, "true",
                "Datum und Uhrzeit im Dateinamen", "Date and time in the file name",
                "aus = fester Dateiname, jeder Export überschreibt den letzten",
                "off = fixed file name, each export overwrites the last one"),
            new SettingDefinition("Export", "Formats", SettingKind.Choice, "Both",
                "Dateiformate", "File formats",
                "Both = CSV und HTML", "Both = CSV and HTML",
                "Both", "CsvOnly", "HtmlOnly"),
            new SettingDefinition("Export", "CsvStyle", SettingKind.Choice, "English",
                "CSV-Stil", "CSV style",
                "English = Komma; German = Semikolon und Dezimalkomma (deutsches Excel)",
                "English = comma; German = semicolon and decimal comma (German Excel)",
                "English", "German"),
            new SettingDefinition("Export", "OutputFolder", SettingKind.Folder, "",
                "Zielordner", "Output folder",
                "leer = Dokumente\\Sports Interactive\\FM View Exporter",
                "empty = Documents\\Sports Interactive\\FM View Exporter"),
            new SettingDefinition("Hotkeys", "Export", SettingKind.Text, "F9",
                "Taste: Export", "Key: export",
                "z. B. F9 oder Ctrl+Shift+E; gilt nach einem Neustart des Spiels",
                "e.g. F9 or Ctrl+Shift+E; applies after restarting the game"),
            new SettingDefinition("Hotkeys", "Diagnose", SettingKind.Text, "F10",
                "Taste: Diagnose", "Key: diagnostics",
                "Abbild der Oberfläche für die Fehlersuche", "UI dump for troubleshooting"),
            new SettingDefinition("Export", "ScanColumns", SettingKind.Boolean, "false",
                "Auch seitlich scrollen", "Also scroll sideways",
                "langsamer, in FM27 nicht nötig", "slower, not needed in FM27"),
            new SettingDefinition("Export", "FramesPerStep", SettingKind.Number, "3",
                "Wartezeit je Scrollschritt", "Wait per scroll step",
                "bei fehlenden Werten erhöhen", "increase if values are missing"),
        };
    }
}
