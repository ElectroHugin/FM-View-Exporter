using System;
using FMViewExporter.Diagnostics;
using FMViewExporter.Input;
using UnityEngine;

namespace FMViewExporter
{
    /// <summary>
    /// Laeuft jeden Frame im Spiel und reagiert auf die Tasten des Plugins.
    /// </summary>
    public class ExporterBehaviour : MonoBehaviour
    {
        private HotkeyWatcher? _export;
        private HotkeyWatcher? _diagnostics;

        public ExporterBehaviour(IntPtr pointer) : base(pointer)
        {
        }

        public void Start()
        {
            _export = HotkeyWatcher.FromConfig(Plugin.Settings.ExportHotkey.Value, "Export", "F9");
            _diagnostics = HotkeyWatcher.FromConfig(Plugin.Settings.DiagnosticsHotkey.Value, "Diagnose", "F10");
        }

        public void Update()
        {
            try
            {
                if (_diagnostics != null && _diagnostics.WasPressed())
                    UiDumper.DumpToFile();

                if (_export != null && _export.WasPressed())
                    Plugin.Logger.LogMessage("Export ist in diesem Erkundungs-Build noch nicht enthalten. Bitte F10 fuer ein UI-Abbild verwenden.");
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"Fehler bei der Tastenabfrage: {ex}");
            }
        }
    }
}
