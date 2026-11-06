using System;
using System.IO;
using FMViewExporter.Export;
using UnityEngine.UIElements;
using Il2CppObject = Il2CppSystem.Object;

namespace FMViewExporter.Diagnostics
{
    /// <summary>
    /// Teil des F10-Abbilds: beschreibt die Objekte hinter der Spielertabelle (Tabelle, Vorfahren,
    /// erste Zeile, erste Zelle) mit Feldern und Eigenschaften. Damit laesst sich herausfinden, ob
    /// die Spielerliste direkt aus dem Datenmodell gelesen werden kann, statt durch die Oberflaeche
    /// zu scrollen. Nur lesend.
    /// </summary>
    internal static class TableModel
    {
        private const int Budget = 2500;
        private const int AncestorLevels = 4;

        public static void Write(StreamWriter writer)
        {
            writer.WriteLine();
            writer.WriteLine("== Datenmodell der Spielertabelle");
            try
            {
                var table = PlayerTable.Find();
                if (table == null)
                {
                    writer.WriteLine("Keine Spielertabelle sichtbar.");
                    return;
                }

                writer.WriteLine($"Ansicht: {table.ViewName}");
                Section(writer, "Tabelle #" + table.Table.name, table.Table, 2);

                var level = 0;
                for (var e = table.Table.parent; e != null && level < AncestorLevels; e = e.parent, level++)
                    Section(writer, $"Vorfahr {level + 1} #{e.name}", e, 1);

                Section(writer, "Zeilen-ScrollView", table.Rows, 1);

                var rows = table.FindVisibleRows();
                writer.WriteLine($"Sichtbare Zeilen: {rows.Count}");
                if (rows.Count == 0)
                    return;
                var first = rows[0];
                var rowElement = first.Cells[0].parent;
                while (rowElement != null && !rowElement.ClassListContains("streamed-table__row"))
                    rowElement = rowElement.parent;
                if (rowElement != null)
                {
                    Section(writer, "Erste Zeile", rowElement, 2);
                    Data(writer, "Erste Zeile", rowElement);
                }
                Section(writer, "Erste Zelle", first.Cells[0], 1);
                Data(writer, "Erste Zelle", first.Cells[0]);
                if (first.Cells.Count > 2)
                {
                    Section(writer, "Dritte Zelle", first.Cells[2], 1);
                    Data(writer, "Dritte Zelle", first.Cells[2]);
                }
            }
            catch (Exception ex)
            {
                writer.WriteLine("Fehler: " + ex.GetType().Name + ": " + ex.Message);
            }
        }

        private static void Data(StreamWriter writer, string label, VisualElement element)
        {
            if (element.dataSource != null)
                Section(writer, label + " dataSource", element.dataSource, 2);
            if (element.userData != null)
                Section(writer, label + " userData", element.userData, 2);
        }

        private static void Section(StreamWriter writer, string title, Il2CppObject obj, int depth)
        {
            writer.WriteLine();
            writer.WriteLine($"-- {title}: {Il2CppInspector.TypeName(obj)}");
            var budget = Budget;
            try
            {
                Il2CppInspector.Describe(obj, "  ", depth, writer.WriteLine, ref budget, skipUnity: obj.TryCast<VisualElement>() != null);
            }
            catch (Exception ex)
            {
                writer.WriteLine("  Fehler: " + ex.GetType().Name + ": " + ex.Message);
            }
        }
    }
}
