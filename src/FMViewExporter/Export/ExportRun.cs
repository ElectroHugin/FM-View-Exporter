using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using FMViewExporter.Core;
using UnityEngine;

namespace FMViewExporter.Export
{
    /// <summary>
    /// Ein Exportvorgang. Das Spiel zeichnet nur die sichtbaren Zeilen (und eventuell nur die
    /// sichtbaren Spalten), deshalb wird die Liste Schritt fuer Schritt durchgescrollt. Zwischen
    /// den Schritten bekommt das Spiel Frames, um die Zeilen neu zu befuellen.
    /// Am Ende werden die urspruengliche Scrollposition und die Bildrate wiederhergestellt.
    /// </summary>
    internal sealed class ExportRun
    {
        private const int MaxSteps = 5000;
        private const int MaxSettleFrames = 30;
        private const string KeyHeader = "Unique ID";

        // Anteil der sichtbaren Hoehe je Scrollschritt. Die Ueberlappung wird ueber "Unique ID"
        // zusammengefuehrt; fehlt sie, scrollt der Export kleiner nach.
        private const float PageFraction = 0.95f;

        private readonly IEnumerator<int> _steps;
        private int _waitFrames;
        private Action? _cleanup;
        private int _frames;
        private double _frameSeconds;
        private double _slowestFrame;

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
            // Wie lange das Spiel fuer jeden Frame braucht (fuer die Zeitaufteilung im Log).
            var delta = Time.unscaledDeltaTime;
            _frames++;
            _frameSeconds += delta;
            _slowestFrame = Math.Max(_slowestFrame, delta);
            if (_waitFrames > 0)
            {
                _waitFrames--;
                return;
            }
            try
            {
                // "yield return n" heisst: in n Frames weitermachen. Dieser Frame zaehlt als erster.
                if (_steps.MoveNext())
                    _waitFrames = Math.Max(0, _steps.Current - 1);
                else
                    Finished = true;
            }
            catch (Exception ex)
            {
                Finished = true;
                Plugin.Logger.LogError($"Export abgebrochen: {ex}");
            }
            if (Finished)
                RunCleanup();
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
            var gapReported = false;

            // Das Plugin wartet in Frames, also laeuft der Export so schnell wie die Bildrate.
            var frameRate = FrameRateUnlock.Apply();

            // Liste hochziehen: Das Spiel braucht pro Frame etwa gleich lang, egal wie viele Zeilen
            // es zeichnet, also bringen mehr Zeilen je Schritt weniger Schritte insgesamt.
            TallList? tall = null;
            var rowHeight = 30f;
            var rowsPerStep = Plugin.Settings.RowsPerStep.Value;
            try
            {
                var firstRows = table.FindVisibleRows();
                if (firstRows.Count > 1 && Math.Abs(firstRows[1].Top - firstRows[0].Top) > 1f)
                    rowHeight = Math.Abs(firstRows[1].Top - firstRows[0].Top);
                tall = TallList.Apply(scroll, rowsPerStep, rowHeight);
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogWarning($"Liste konnte nicht vergroessert werden: {ex.Message}");
            }
            _cleanup = () =>
            {
                tall?.Restore();
                scroll.scrollOffset = original;
                frameRate.Restore();
            };
            if (tall != null)
            {
                yield return 3;
                Plugin.Logger.LogInfo($"Liste vergroessert: {table.FindVisibleRows().Count} Zeilen je Schritt (Einstellung RowsPerStep = {rowsPerStep}).");
            }

            var timing = new ExportTiming();
            var step = 0;
            var y = 0f;
            HashSet<string>? previousKeys = null;
            string? previousSignature = null;
            var previousPosition = new Vector2(float.NaN, float.NaN);
            var lastRowCount = 0;
            while (true)
            {
                var x = 0f;
                var stepKeys = new HashSet<string>();
                while (true)
                {
                    var position = new Vector2(x, y);
                    scroll.scrollOffset = position;
                    timing.Frames += settle;
                    yield return settle;

                    // Warten, bis die Zeilen zur neuen Position passen und zwei Lesungen hintereinander
                    // gleich sind. Dafuer reichen Position, Schluessel und letzte Zelle jeder Zeile;
                    // alle Zellen werden danach genau einmal gelesen.
                    var moved = position != previousPosition;
                    var visible = Timed(timing, table.FindVisibleRows);
                    var signature = Timed(timing, () => Signature(visible, keyColumn));
                    for (var i = 0; i < MaxSettleFrames; i++)
                    {
                        timing.Frames++;
                        yield return 1;
                        var again = Timed(timing, table.FindVisibleRows);
                        var againSignature = Timed(timing, () => Signature(again, keyColumn));
                        var same = againSignature == signature;
                        visible = again;
                        signature = againSignature;
                        // Sieht alles aus wie vor dem Scrollen, hat das Spiel die Zeilen noch nicht
                        // neu befuellt: weiter warten.
                        if (same && (!moved || signature != previousSignature))
                            break;
                    }
                    previousSignature = signature;
                    previousPosition = position;

                    lastRowCount = visible.Count;
                    var read = Timed(timing, () => visible.Select(r => (r.Top, r.ContentY, Cells: PlayerTable.ReadCells(r))).ToList());
                    foreach (var (top, contentY, cells) in read)
                    {
                        if (cells.Count != headers.Count && !mismatchReported)
                        {
                            mismatchReported = true;
                            Plugin.Logger.LogWarning($"Zeile hat {cells.Count} Zellen, Kopf hat {headers.Count} Spalten. Zuordnung nach Reihenfolge.");
                        }
                        if (keyColumn >= 0 && keyColumn < cells.Count && cells[keyColumn].Length > 0)
                            stepKeys.Add(cells[keyColumn]);
                        // Position im Listeninhalt = stabile Zeilenidentitaet (gleich nach Scrollen und
                        // seitlichem Scannen); so findet eine erst spaeter gefuellte Unique ID ihre Zeile.
                        rows.Add(cells, step * 100000.0 + top, RowId(contentY));
                    }

                    step++;
                    var maxX = scroll.horizontalScroller.highValue;
                    if (!sweepColumns || x >= maxX || step >= MaxSteps)
                        break;
                    x = Math.Min(maxX, x + Math.Max(50f, scroll.contentViewport.layout.width * 0.9f));
                }

                var page = Math.Max(20f, scroll.contentViewport.layout.height * PageFraction);
                // Nie weiter scrollen, als das Spiel tatsaechlich Zeilen gezeichnet hat.
                if (lastRowCount > 1)
                    page = Math.Min(page, Math.Max(20f, (lastRowCount - 1) * rowHeight));

                // Sicherheitsnetz fuer die grossen Schritte: teilt dieser Schritt keinen Spieler mit
                // dem vorherigen, koennten Zeilen dazwischen fehlen. Dann einen halben Schritt zurueck.
                if (previousKeys != null && previousKeys.Count > 0 && stepKeys.Count > 0 &&
                    !stepKeys.Overlaps(previousKeys) && y > 0 && step < MaxSteps)
                {
                    if (!gapReported)
                    {
                        gapReported = true;
                        Plugin.Logger.LogInfo("Scrollschritt ohne Ueberlappung, scrolle kleiner nach.");
                    }
                    previousKeys = null;
                    y = Math.Max(0f, y - page / 2);
                    continue;
                }
                previousKeys = stepKeys;

                var maxY = scroll.verticalScroller.highValue;
                if (y >= maxY || step >= MaxSteps)
                    break;
                y = Math.Min(maxY, y + page);
            }

            RunCleanup();
            Write(table, headers, rows, watch);
            var other = watch.Elapsed - timing.Read;
            Plugin.Logger.LogInfo($"Zeitaufteilung: {step} Scrollschritte, {timing.Frames} Frames gewartet; " +
                $"Lesen {timing.Read.TotalSeconds:0.0} s, Warten auf das Spiel und Schreiben {Math.Max(0, other.TotalSeconds):0.0} s; " +
                $"Frame-Dauer im Schnitt {(_frames > 0 ? _frameSeconds / _frames * 1000 : 0):0} ms, laengster {_slowestFrame * 1000:0} ms.");
        }

        private sealed class ExportTiming
        {
            public int Frames;
            public TimeSpan Read;
        }

        private static T Timed<T>(ExportTiming timing, Func<T> action)
        {
            var start = Stopwatch.GetTimestamp();
            try
            {
                return action();
            }
            finally
            {
                timing.Read += TimeSpan.FromSeconds((Stopwatch.GetTimestamp() - start) / (double)Stopwatch.Frequency);
            }
        }

        /// <summary>
        /// Kurzer Fingerabdruck der sichtbaren Zeilen: Position, Schluessel und letzte Zelle.
        /// Ohne Schluesselspalte wird die ganze Zeile genommen.
        /// </summary>
        private static string Signature(List<PlayerTable.VisibleRow> visible, int keyColumn)
        {
            var sb = new StringBuilder();
            foreach (var row in visible)
            {
                sb.Append(row.Top.ToString("R", CultureInfo.InvariantCulture)).Append('\u001e');
                if (keyColumn < 0)
                {
                    sb.Append(string.Join("\u001f", PlayerTable.ReadCells(row)));
                }
                else
                {
                    sb.Append(PlayerTable.ReadCell(row, keyColumn)).Append('\u001f');
                    sb.Append(PlayerTable.ReadCell(row, row.Cells.Count - 1));
                }
                sb.Append('\u001d');
            }
            return sb.ToString();
        }

        private void RunCleanup()
        {
            var cleanup = _cleanup;
            _cleanup = null;
            try
            {
                cleanup?.Invoke();
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogWarning($"Ausgangszustand konnte nicht wiederhergestellt werden: {ex.Message}");
            }
        }

        /// <summary>Zeilenidentitaet aus der Position im Listeninhalt (auf ganze Pixel gerundet).</summary>
        private static string? RowId(float contentY) =>
            float.IsNaN(contentY) || float.IsInfinity(contentY) ? null : "y:" + Math.Round(contentY).ToString(System.Globalization.CultureInfo.InvariantCulture);

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
            var settings = Plugin.Settings;
            var baseName = ExportFileNames.Build(settings.FilePrefix.Value, table.ViewName,
                settings.Timestamp.Value ? now : (DateTime?)null);
            var formats = settings.Formats.Value;
            var written = new List<string>();
            if (formats != ExportFormats.HtmlOnly)
            {
                var csv = Path.Combine(folder, baseName + ".csv");
                CsvWriter.WriteFile(export, csv, settings.CsvStyle.Value);
                written.Add(csv);
            }
            if (formats != ExportFormats.CsvOnly)
            {
                var html = Path.Combine(folder, baseName + ".html");
                HtmlWriter.WriteFile(export, html);
                written.Add(html);
            }

            Plugin.Logger.LogMessage($"Export fertig: {export.Rows.Count} Zeilen, {export.Headers.Count} Spalten in {watch.Elapsed.TotalSeconds:0.0} s -> {string.Join(", ", written)}");

            if (rows.PossibleDuplicates > 0)
                Plugin.Logger.LogWarning($"{rows.PossibleDuplicates} Zeilen ohne Unique ID passen zu einer anderen Zeile und koennten doppelt sein. Sie wurden nicht zusammengefuehrt; bitte die Spalte \"Unique ID\" in die Ansicht aufnehmen.");

            var empty = rows.EmptyColumns().Where(i => headers[i].Length > 0).Select(i => headers[i]).ToList();
            if (empty.Count > 0)
                Plugin.Logger.LogInfo($"{empty.Count} Spalten ohne Werte: {string.Join(", ", empty)}");
        }
    }
}
