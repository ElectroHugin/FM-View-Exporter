using System;
using System.Collections.Generic;
using System.Linq;

namespace FMViewExporter.Core
{
    /// <summary>
    /// Sammelt Tabellenzeilen, waehrend die Liste durchgescrollt wird. Das Spiel zeichnet nur
    /// die sichtbaren Zeilen, deshalb tauchen dieselben Spieler mehrfach auf.
    /// <para>
    /// Zuordnung einer gelesenen Zeile zu einer schon gesammelten:
    /// 1. ueber die Schluesselspalte (z. B. "Unique ID"), wenn sie gefuellt ist;
    /// 2. sonst ueber die Zeilenidentitaet aus der Oberflaeche (<c>rowId</c>, z. B. die Position der
    ///    Zeile in der Liste), aber nur, wenn sich die gefuellten Zellen nicht widersprechen;
    /// 3. ohne beides nur bei exakt gleichem Inhalt.
    /// Eine Zeile, deren ID erst spaeter erscheint, wird ueber die Zeilenidentitaet mit ihrem
    /// frueheren Eintrag zusammengefuehrt. Leere Zellen werden durch spaeter gelesene Werte ergaenzt,
    /// gefuellte nie ueberschrieben. Namen allein fuehren nie Zeilen zusammen.
    /// </para>
    /// </summary>
    public sealed class RowCollector
    {
        private readonly int _columnCount;
        private readonly int _keyColumn;
        private readonly List<Entry> _entries = new();
        private readonly Dictionary<string, Entry> _byKey = new();
        private readonly Dictionary<string, Entry> _byRowId = new();
        private readonly Dictionary<string, Entry> _byContent = new();

        /// <param name="columnCount">Anzahl der Spalten.</param>
        /// <param name="keyColumn">Index der Schluesselspalte, -1 = keine.</param>
        public RowCollector(int columnCount, int keyColumn)
        {
            if (columnCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(columnCount));
            _columnCount = columnCount;
            _keyColumn = keyColumn < columnCount ? keyColumn : -1;
        }

        public int Count => _entries.Count;

        /// <summary>
        /// Zeilen ohne Schluessel, die zu einer anderen gesammelten Zeile passen (keine widerspruechlichen
        /// Zellen). Sie koennten doppelt sein; zusammengefuehrt werden sie bewusst nicht, um keine Spieler
        /// zu vermischen. Fuers Log, damit so ein Fall nicht still bleibt.
        /// </summary>
        public int PossibleDuplicates
        {
            get
            {
                var count = 0;
                foreach (var entry in _entries)
                {
                    if (entry.Key != null)
                        continue;
                    if (_entries.Any(other => !ReferenceEquals(other, entry) && Compatible(other.Cells, entry.Cells)))
                        count++;
                }
                return count;
            }
        }

        /// <summary>
        /// Nimmt eine gelesene Zeile auf. <paramref name="position"/> bestimmt die Reihenfolge
        /// im Export; massgeblich ist die Position beim ersten Auftauchen.
        /// <paramref name="rowId"/> ist eine stabile Identitaet der Zeile aus der Oberflaeche
        /// (gleicher Wert = gleiche Zeile der Liste, auch nach dem Scrollen), oder null.
        /// </summary>
        /// <returns>true, wenn die Zeile neu war.</returns>
        public bool Add(IReadOnlyList<string> cells, double position, string? rowId = null)
        {
            if (cells.All(string.IsNullOrEmpty))
                return false;

            var copy = new string[_columnCount];
            for (var i = 0; i < copy.Length; i++)
                copy[i] = i < cells.Count ? cells[i] ?? string.Empty : string.Empty;

            var key = _keyColumn >= 0 && copy[_keyColumn].Length > 0 ? copy[_keyColumn] : null;
            if (string.IsNullOrEmpty(rowId))
                rowId = null;

            Entry? match = null;
            if (key != null && _byKey.TryGetValue(key, out var byKey))
                match = byKey;
            else if (rowId != null && _byRowId.TryGetValue(rowId, out var byRow)
                     && (key == null || byRow.Key == null) && Compatible(byRow.Cells, copy))
                match = byRow;
            else if (key == null && rowId == null && _byContent.TryGetValue(ContentKey(copy), out var byContent))
                match = byContent;

            if (match != null)
            {
                Merge(match, copy);
                if (key != null && match.Key == null)
                {
                    match.Key = key;
                    _byKey[key] = match;
                }
                if (rowId != null && match.RowId == null && !_byRowId.ContainsKey(rowId))
                {
                    match.RowId = rowId;
                    _byRowId[rowId] = match;
                }
                return false;
            }

            var entry = new Entry(copy, position, _entries.Count) { Key = key };
            _entries.Add(entry);
            if (key != null)
                _byKey[key] = entry;
            if (rowId != null)
            {
                // Eine neue Zeile mit derselben Identitaet (Liste hat sich geaendert) uebernimmt sie.
                entry.RowId = rowId;
                if (_byRowId.TryGetValue(rowId, out var previous))
                    previous.RowId = null;
                _byRowId[rowId] = entry;
            }
            if (key == null && rowId == null)
                _byContent[ContentKey(copy)] = entry;
            return true;
        }

        public IEnumerable<string[]> RowsInOrder()
        {
            return _entries.OrderBy(e => e.Position).ThenBy(e => e.Sequence).Select(e => e.Cells);
        }

        /// <summary>Spalten, die in keiner Zeile einen Wert haben.</summary>
        public IEnumerable<int> EmptyColumns()
        {
            for (var i = 0; i < _columnCount; i++)
            {
                if (_entries.All(e => e.Cells[i].Length == 0))
                    yield return i;
            }
        }

        private static void Merge(Entry entry, string[] cells)
        {
            for (var i = 0; i < entry.Cells.Length; i++)
            {
                if (entry.Cells[i].Length == 0 && cells[i].Length > 0)
                    entry.Cells[i] = cells[i];
            }
        }

        /// <summary>Kein Widerspruch: wo beide Zeilen einen Wert haben, ist er gleich.</summary>
        private static bool Compatible(string[] a, string[] b)
        {
            for (var i = 0; i < a.Length && i < b.Length; i++)
            {
                if (a[i].Length > 0 && b[i].Length > 0 && !string.Equals(a[i], b[i], StringComparison.Ordinal))
                    return false;
            }
            return true;
        }

        private static string ContentKey(string[] cells) => string.Join("\u001f", cells);

        private sealed class Entry
        {
            public Entry(string[] cells, double position, int sequence)
            {
                Cells = cells;
                Position = position;
                Sequence = sequence;
            }

            public string[] Cells { get; }
            public double Position { get; }
            public int Sequence { get; }
            public string? Key { get; set; }
            public string? RowId { get; set; }
        }
    }
}
