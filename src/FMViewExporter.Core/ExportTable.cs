using System;
using System.Collections.Generic;

namespace FMViewExporter.Core
{
    /// <summary>
    /// Eine exportierte Tabelle: Spaltenkoepfe wie im Spiel und die Zeilen als Text.
    /// </summary>
    public sealed class ExportTable
    {
        private readonly List<string[]> _rows = new();

        public ExportTable(string title, IReadOnlyList<string> headers, DateTime createdAt)
        {
            if (headers.Count == 0)
                throw new ArgumentException("Eine Tabelle braucht mindestens eine Spalte.", nameof(headers));

            Title = title;
            Headers = headers;
            CreatedAt = createdAt;
        }

        public string Title { get; }
        public IReadOnlyList<string> Headers { get; }
        public DateTime CreatedAt { get; }
        public IReadOnlyList<string[]> Rows => _rows;

        /// <summary>
        /// Fuegt eine Zeile hinzu. Fehlende Zellen werden leer aufgefuellt,
        /// ueberzaehlige abgeschnitten, damit jede Zeile so breit ist wie der Kopf.
        /// </summary>
        public void AddRow(IReadOnlyList<string?> cells)
        {
            var row = new string[Headers.Count];
            for (var i = 0; i < row.Length; i++)
                row[i] = i < cells.Count ? cells[i] ?? string.Empty : string.Empty;
            _rows.Add(row);
        }
    }
}
