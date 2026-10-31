using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using FMViewExporter.Core;
using UnityEngine;

namespace FMViewExporter.Export
{
    /// <summary>
    /// Ein Exportvorgang. Das Spiel zeichnet nur die sichtbaren Zeilen (und eventuell nur die
    /// sichtbaren Spalten), deshalb wird die Liste Schritt fuer Schritt durchgescrollt. Zwischen
    /// den Schritten bekommt das Spiel einige Frames, um die Zeilen neu zu befuellen.
    /// Am Ende wird die urspruengliche Scrollposition wiederhergestellt.
    /// </summary>
    internal sealed class ExportRun
    {
        private const int MaxSteps = 5000;
        private const int MaxSettleFrames = 30;
        private const string KeyHeader = "Unique ID";

        private readonly IEnumerator<int> _steps;
        private int _waitFrames;

        public ExportRun()
        {
            _steps = Run().GetEnumerator();
        }

        public bool Finished { get; private set; }

        /// <summary>Einmal pro Frame aufrufen.</summary>
        public void Tick()
        {
            if (Finished)
                return;
            if (_waitFrames > 0)
            {
                _waitFrames--;
                return;
            }
            try
            {
                if (_steps.MoveNext())
                    _waitFrames = _steps.Current;
                else
                    Finished = true;
            }
            catch (Exception ex)
            {
                Finished = true;
                Plugin.Logger.LogError($"Export abgebrochen: {ex}");
            }
        }

        private IEnumerable<int> Run()
        {
            var watch = Stopwatch.StartNew();

            // Einstellungen bei jedem Export neu einlesen, damit Aenderungen ohne Neustart greifen.
            try
            {
                Plugin.ConfigFile.Reload();
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogWarning($"Einstellungen konnten nicht neu gelesen werden: {ex.Message}");
            }
            var table = PlayerTable.Find();
            if (table == null)
            {
                Plugin.Logger.LogWarning("Keine Spielerliste gefunden. Bitte Kader, Spielersuche oder Shortlist oeffnen und F9 erneut druecken.");
                try
                {
                    var hints = PlayerTable.DescribeCandidates();
                    if (hints.Count == 0)
                        Plugin.Logger.LogInfo("Diagnose: keine Tabellenteile sichtbar. Ein Abbild mit F10 hilft bei der Fehlersuche.");
                    foreach (var hint in hints)
                        Plugin.Logger.LogInfo("Diagnose: " + hint);
                }
                catch (Exception ex)
                {
                    Plugin.Logger.LogInfo($"Diagnose nicht moeglich: {ex.Message}");
                }
                yield break;
            }

            var headers = table.ReadHeaders();
            var keyColumn = headers.IndexOf(KeyHeader);
            Plugin.Logger.LogMessage($"Export gestartet: Ansicht \"{table.ViewName}\", {headers.Count} Spalten" +
                (keyColumn < 0 ? " (ohne \"Unique ID\"-Spalte, doppelte Zeilen werden ueber den ganzen Inhalt erkannt)." : "."));

            var rows = new RowCollector(headers.Count, keyColumn);
            var scroll = table.Rows;
            var original = scroll.scrollOffset;
            var sweepColumns = Plugin.Settings.ScanColumns.Value;
            var settle = Math.Max(1, Plugin.Settings.FramesPerStep.Value);
            var mismatchReported = false;

            var step = 0;
            var y = 0f;
            while (true)
            {
                var x = 0f;
                while (true)
                {
                    scroll.scrollOffset = new Vector2(x, y);
                    yield return settle;

                    // Warten, bis zwei Lesungen hintereinander gleich sind (Zeilen fertig befuellt).
                    var read = table.ReadVisibleRows();
                    for (var i = 0; i < MaxSettleFrames; i++)
                    {
                        yield return 1;
                        var again = table.ReadVisibleRows();
                        if (Same(read, again))
                            break;
                        read = again;
                    }

                    foreach (var (top, cells) in read)
                    {
                        if (cells.Count != headers.Count && !mismatchReported)
                        {
                            mismatchReported = true;
                            Plugin.Logger.LogWarning($"Zeile hat {cells.Count} Zellen, Kopf hat {headers.Count} Spalten. Zuordnung nach Reihenfolge.");
                        }
                        rows.Add(cells, step * 100000.0 + top);
                    }

                    step++;
                    var maxX = scroll.horizontalScroller.highValue;
                    if (!sweepColumns || x >= maxX || step >= MaxSteps)
                        break;
                    x = Math.Min(maxX, x + Math.Max(50f, scroll.contentViewport.layout.width * 0.9f));
                }

                var maxY = scroll.verticalScroller.highValue;
                if (y >= maxY || step >= MaxSteps)
                    break;
                y = Math.Min(maxY, y + Math.Max(20f, scroll.contentViewport.layout.height * 0.8f));
            }

            scroll.scrollOffset = original;
            Write(table, headers, rows, watch);
        }

        private static bool Same(List<(float Top, List<string> Cells)> a, List<(float Top, List<string> Cells)> b)
        {
            if (a.Count != b.Count)
                return false;
            for (var i = 0; i < a.Count; i++)
            {
                if (a[i].Top != b[i].Top || !a[i].Cells.SequenceEqual(b[i].Cells))
                    return false;
            }
            return true;
        }

        private static void Write(PlayerTable table, List<string> headers, RowCollector rows, Stopwatch watch)
        {
            // Spalten ohne Kopf (Auswahlkaestchen) fallen weg.
            var keep = Enumerable.Range(0, headers.Count).Where(i => headers[i].Length > 0).ToList();
            var now = DateTime.Now;
            var title = $"{Application.productName} - {table.ViewName}";
            var export = new ExportTable(title, keep.Select(i => headers[i]).ToList(), now);
            foreach (var row in rows.RowsInOrder())
                export.AddRow(keep.Select(i => row[i]).ToList());

            var folder = OutputPaths.EnsureFolder(OutputPaths.BaseFolder);
            var baseName = ExportFileNames.Build(table.ViewName, now);
            var csv = Path.Combine(folder, baseName + ".csv");
            var html = Path.Combine(folder, baseName + ".html");
            CsvWriter.WriteFile(export, csv, Plugin.Settings.CsvStyle.Value);
            HtmlWriter.WriteFile(export, html);

            Plugin.Logger.LogMessage($"Export fertig: {export.Rows.Count} Zeilen, {export.Headers.Count} Spalten in {watch.Elapsed.TotalSeconds:0.0} s -> {csv}");

            var empty = rows.EmptyColumns().Where(i => headers[i].Length > 0).Select(i => headers[i]).ToList();
            if (empty.Count > 0)
                Plugin.Logger.LogInfo($"{empty.Count} Spalten ohne Werte: {string.Join(", ", empty)}");
        }
    }
}
