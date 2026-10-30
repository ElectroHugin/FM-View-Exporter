using System;
using FMViewExporter.Diagnostics;
using FMViewExporter.Export;
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
        private ExportRun? _run;

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
            if (_run != null)
            {
                _run.Tick();
                if (_run.Finished)
                    _run = null;
            }

            try
            {
                if (_diagnostics != null && _diagnostics.WasPressed())
                    UiDumper.DumpToFile();

                if (_export != null && _export.WasPressed())
                {
                    if (_run != null)
                        Plugin.Logger.LogMessage("Export laeuft bereits, bitte warten.");
                    else
                        _run = new ExportRun();
                }
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"Fehler bei der Tastenabfrage: {ex}");
            }
        }
    }
}
