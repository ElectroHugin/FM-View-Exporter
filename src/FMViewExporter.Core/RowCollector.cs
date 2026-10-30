using System;
using System.Collections.Generic;
using System.Linq;

namespace FMViewExporter.Core
{
    /// <summary>
    /// Sammelt Tabellenzeilen, waehrend die Liste durchgescrollt wird. Das Spiel zeichnet nur
    /// die sichtbaren Zeilen, deshalb tauchen dieselben Spieler mehrfach auf. Zeilen werden ueber
    /// die Schluesselspalte (z. B. "Unique ID") zusammengefuehrt; leere Zellen werden spaeter
    /// gelesene Werte ergaenzt.
    /// </summary>
    public sealed class RowCollector
    {
        private readonly int _columnCount;
        private readonly int _keyColumn;
        private readonly Dictionary<string, Entry> _rows = new();

        /// <param name="columnCount">Anzahl der Spalten.</param>
        /// <param name="keyColumn">Index der Schluesselspalte, -1 = ganze Zeile als Schluessel.</param>
        public RowCollector(int columnCount, int keyColumn)
        {
            if (columnCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(columnCount));
            _columnCount = columnCount;
            _keyColumn = keyColumn < columnCount ? keyColumn : -1;
        }

        public int Count => _rows.Count;

        /// <summary>
        /// Nimmt eine gelesene Zeile auf. <paramref name="position"/> bestimmt die Reihenfolge
        /// im Export; massgeblich ist die Position beim ersten Auftauchen.
        /// </summary>
        /// <returns>true, wenn die Zeile neu war.</returns>
        public bool Add(IReadOnlyList<string> cells, double position)
        {
            if (cells.All(string.IsNullOrEmpty))
                return false;

            var key = _keyColumn >= 0 && _keyColumn < cells.Count && !string.IsNullOrEmpty(cells[_keyColumn])
                ? "k:" + cells[_keyColumn]
                : "r:" + string.Join("\u001f", cells);

            if (_rows.TryGetValue(key, out var existing))
            {
                for (var i = 0; i < existing.Cells.Length && i < cells.Count; i++)
                {
                    if (existing.Cells[i].Length == 0 && !string.IsNullOrEmpty(cells[i]))
                        existing.Cells[i] = cells[i];
                }
                return false;
            }

            var copy = new string[_columnCount];
            for (var i = 0; i < copy.Length; i++)
                copy[i] = i < cells.Count ? cells[i] ?? string.Empty : string.Empty;
            _rows.Add(key, new Entry(copy, position, _rows.Count));
            return true;
        }

        public IEnumerable<string[]> RowsInOrder()
        {
            return _rows.Values.OrderBy(e => e.Position).ThenBy(e => e.Sequence).Select(e => e.Cells);
        }

        /// <summary>Spalten, die in keiner Zeile einen Wert haben.</summary>
        public IEnumerable<int> EmptyColumns()
        {
            for (var i = 0; i < _columnCount; i++)
            {
                if (_rows.Values.All(e => e.Cells[i].Length == 0))
                    yield return i;
            }
        }

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
        }
    }
}
