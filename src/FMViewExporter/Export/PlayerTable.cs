using System;
using System.Collections.Generic;
using System.Globalization;
using FMViewExporter.Core;
using FMViewExporter.Ui;
using Il2CppInterop.Runtime;
using UnityEngine.UIElements;

namespace FMViewExporter.Export
{
    /// <summary>
    /// Die Spielertabelle des Spiels (SI.Bindable.StreamedTable #playertable), wie sie im
    /// UI-Abbild von FM27 zu sehen ist: Spaltenkoepfe in #column-headers (ein Pane je Spalte),
    /// Zeilen als .streamed-table__row in einer virtualisierten Liste, die Zellen als direkte
    /// Kinder von #DragObject in derselben Reihenfolge wie die Koepfe.
    /// </summary>
    internal sealed class PlayerTable
    {
        private const string TableName = "playertable";
        private const string HeaderContainerName = "column-headers";
        private const string HeaderTextClass = "table-header-xs";
        private const string RowClass = "streamed-table__row";
        private const string CellClass = "streamed-table__cell";
        private const string StarClass = "fm-star-rating-star";

        private PlayerTable(VisualElement table, VisualElement headerContainer, ScrollView rows, string viewName)
        {
            Table = table;
            HeaderContainer = headerContainer;
            Rows = rows;
            ViewName = viewName;
        }

        public VisualElement Table { get; }
        public VisualElement HeaderContainer { get; }
        public ScrollView Rows { get; }

        /// <summary>Name der Ansicht fuer den Dateinamen, z. B. "squad" aus #SquadScreen.</summary>
        public string ViewName { get; }

        /// <summary>
        /// Sucht die sichtbare Spielertabelle mit der groessten Flaeche. Zuerst ueber den Namen
        /// #playertable (Kader, Spielersuche) oder #playertable... (z. B. #playertableshortlist
        /// in der Shortlist); gibt es keine, ueber den Aufbau der Tabelle
        /// (#column-headers plus Zeilen-Liste), weil andere Ansichten wie die Shortlist die
        /// gleiche Tabelle unter anderem Namen verwenden koennen.
        /// </summary>
        public static PlayerTable? Find()
        {
            var panels = RuntimePanels.Find();
            return FindByName(panels) ?? FindByStructure(panels);
        }

        private static PlayerTable? FindByName(IEnumerable<RuntimePanels.PanelInfo> panels)
        {
            PlayerTable? best = null;
            var bestArea = 0f;
            foreach (var panel in panels)
            {
                foreach (var candidate in Descendants(panel.Root, e => IsTableName(e.name) && !IsHidden(e)))
                {
                    var area = Area(candidate);
                    if (area <= bestArea)
                        continue;

                    var headers = FirstDescendant(candidate, e => e.name == HeaderContainerName);
                    var rows = FindRowScrollView(candidate);
                    if (headers == null || rows == null)
                        continue;

                    best = new PlayerTable(candidate, headers, rows, GuessViewName(candidate));
                    bestArea = area;
                }
            }
            return best;
        }

        private const int MaxStructureLevels = 8;

        private static PlayerTable? FindByStructure(IEnumerable<RuntimePanels.PanelInfo> panels)
        {
            PlayerTable? best = null;
            var bestArea = 0f;
            foreach (var panel in panels)
            {
                foreach (var headers in Descendants(panel.Root, e => e.name == HeaderContainerName && !IsHidden(e)))
                {
                    // Vom Kopfbereich nach oben gehen, bis ein Vorfahr auch die Zeilen enthaelt.
                    var level = 0;
                    for (var candidate = headers.parent; candidate != null && level < MaxStructureLevels; candidate = candidate.parent, level++)
                    {
                        if (IsHidden(candidate))
                            break;
                        var rows = FindRowScrollView(candidate);
                        if (rows == null)
                            continue;

                        var area = Area(candidate);
                        if (area > bestArea)
                        {
                            best = new PlayerTable(candidate, headers, rows, GuessViewName(candidate));
                            bestArea = area;
                        }
                        break;
                    }
                }
            }
            return best;
        }

        /// <summary>
        /// Kurze Beschreibung fuers Log, wenn keine Tabelle gefunden wurde: wo Kopfzeilen und
        /// Tabellenzeilen liegen (Namenskette der Vorfahren), damit sich die Ansicht nachziehen laesst.
        /// </summary>
        public static List<string> DescribeCandidates()
        {
            var result = new List<string>();
            foreach (var panel in RuntimePanels.Find())
            {
                var found = Descendants(panel.Root, e => !IsHidden(e) &&
                    (e.name == HeaderContainerName || IsTableName(e.name) || e.ClassListContains(RowClass)));
                var seen = new HashSet<string>();
                foreach (var element in found)
                {
                    var kind = element.ClassListContains(RowClass) ? "Zeile" : "#" + element.name;
                    var line = kind + " in " + AncestorPath(element);
                    if (seen.Add(line) && result.Count < 10)
                        result.Add(line);
                }
            }
            return result;
        }

        private static string AncestorPath(VisualElement element)
        {
            var names = new List<string>();
            for (var e = element.parent; e != null && names.Count < 12; e = e.parent)
            {
                if (!string.IsNullOrEmpty(e.name))
                    names.Add("#" + e.name);
            }
            names.Reverse();
            return names.Count == 0 ? "(ohne Namen)" : string.Join(" > ", names);
        }

        private static bool IsTableName(string? name) =>
            name != null && name.StartsWith(TableName, StringComparison.OrdinalIgnoreCase);

        private static float Area(VisualElement element)
        {
            var bounds = element.worldBound;
            return bounds.width * bounds.height;
        }

        /// <summary>Spaltenkoepfe in Anzeige-Reihenfolge; die Auswahlspalte hat einen leeren Kopf.</summary>
        public List<string> ReadHeaders()
        {
            var result = new List<string>();
            for (var i = 0; i < HeaderContainer.childCount; i++)
            {
                var pane = HeaderContainer[i];
                var text = FirstDescendant(pane, e => e.ClassListContains(HeaderTextClass))?.TryCast<TextElement>()?.text;
                result.Add(CellText.Clean(text));
            }
            return result;
        }

        /// <summary>Eine gerade gezeichnete Zeile mit ihren Zellen (noch nicht ausgelesen).</summary>
        internal sealed class VisibleRow
        {
            public VisibleRow(float top, float contentY, List<VisualElement> cells)
            {
                Top = top;
                ContentY = contentY;
                Cells = cells;
            }

            /// <summary>Bildschirmposition der Zeile (fuer die Reihenfolge).</summary>
            public float Top { get; }

            /// <summary>Position der Zeile im Inhalt der ScrollView (unabhaengig vom Scrollstand).</summary>
            public float ContentY { get; }

            public List<VisualElement> Cells { get; }
        }

        /// <summary>
        /// Findet alle gerade gezeichneten Zeilen, ohne die Zellen auszulesen. Jede Zeile hat so
        /// viele Zellen wie die Tabelle Spalten.
        /// </summary>
        public List<VisibleRow> FindVisibleRows()
        {
            var result = new List<VisibleRow>();
            var contentTop = Rows.contentContainer.worldBound.y;
            foreach (var row in Descendants(Rows, e => e.ClassListContains(RowClass)))
            {
                if (IsHidden(row))
                    continue;
                var cells = Descendants(row, e => e.ClassListContains(CellClass));
                if (cells.Count == 0)
                    continue;
                var top = row.worldBound.y;
                result.Add(new VisibleRow(top, top - contentTop, cells));
            }
            return result;
        }

        /// <summary>Text einer einzelnen Zelle; leer, wenn es die Spalte in der Zeile nicht gibt.</summary>
        public static string ReadCell(VisibleRow row, int column) =>
            column >= 0 && column < row.Cells.Count ? ReadCell(row.Cells[column]) : string.Empty;

        /// <summary>Alle Zellen einer Zeile als Text.</summary>
        public static List<string> ReadCells(VisibleRow row)
        {
            var cells = new List<string>(row.Cells.Count);
            foreach (var cell in row.Cells)
                cells.Add(ReadCell(cell));
            return cells;
        }

        private static string ReadCell(VisualElement cell)
        {
            var texts = new List<string>();
            var stars = 0;
            var total = 0.0;
            Visit(cell, e =>
            {
                if (IsHidden(e))
                    return false;
                // Jeder Zugriff auf ein Spielobjekt geht ueber IL2CPP und ist teuer, deshalb nur
                // die noetigen: Elemente mit Text sind nie Sterne.
                var text = AsTextElement(e)?.text;
                if (!string.IsNullOrEmpty(text))
                {
                    texts.Add(text!);
                    return true;
                }
                if (e.ClassListContains(StarClass) || e.ClassListContains(StarClass + "-no-margin"))
                {
                    stars++;
                    var names = new List<string>();
                    var classes = e.GetClassesForIteration();
                    for (var i = 0; classes != null && i < classes.Count; i++)
                        names.Add(classes[i]);
                    total += StarRating.StarValue(names);
                }
                return true;
            });

            var value = CellText.Join(texts);
            if (value.Length == 0 && stars > 0)
                value = StarRating.Format(total);
            return value;
        }

        // Ergebnis der Typpruefung je IL2CPP-Klasse, damit sie nicht fuer jedes Element neu laeuft.
        private static readonly Dictionary<IntPtr, bool> TextClasses = new();

        private static TextElement? AsTextElement(VisualElement element)
        {
            var klass = IL2CPP.il2cpp_object_get_class(element.Pointer);
            if (!TextClasses.TryGetValue(klass, out var isText))
            {
                isText = element.TryCast<TextElement>() != null;
                TextClasses[klass] = isText;
            }
            return isText ? new TextElement(element.Pointer) : null;
        }

        private static ScrollView? FindRowScrollView(VisualElement table)
        {
            // Die Zeilen liegen in der ScrollView der virtualisierten Liste, nicht in der Kopf-ScrollView.
            foreach (var element in Descendants(table, e => e.TryCast<ScrollView>() != null))
            {
                var scrollView = element.TryCast<ScrollView>()!;
                if (FirstDescendant(scrollView, e => e.ClassListContains(RowClass)) != null)
                    return scrollView;
            }
            return null;
        }

        private static string GuessViewName(VisualElement table)
        {
            var tableNames = new List<string>();
            Visit(table, e =>
            {
                if (IsTableName(e.name))
                    tableNames.Add(e.name);
                return true;
            });
            var ancestors = new List<string>();
            for (var e = table.parent; e != null; e = e.parent)
                ancestors.Add(e.name);
            return ViewNames.Guess(tableNames, ancestors);
        }

        internal static bool IsHidden(VisualElement element)
        {
            try
            {
                // visible zuerst: ein Aufruf weniger als resolvedStyle.display.
                return !element.visible || element.resolvedStyle.display == DisplayStyle.None;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>Tiefensuche; <paramref name="enter"/> entscheidet, ob Kinder besucht werden.</summary>
        private static void Visit(VisualElement element, Func<VisualElement, bool> enter)
        {
            if (!enter(element))
                return;
            for (var i = 0; i < element.childCount; i++)
                Visit(element[i], enter);
        }

        /// <summary>Alle Nachfahren, auf die <paramref name="match"/> passt, ohne in Treffer hineinzugehen.</summary>
        private static List<VisualElement> Descendants(VisualElement root, Func<VisualElement, bool> match)
        {
            var result = new List<VisualElement>();
            var stack = new Stack<VisualElement>();
            for (var i = root.childCount - 1; i >= 0; i--)
                stack.Push(root[i]);
            while (stack.Count > 0)
            {
                var element = stack.Pop();
                if (match(element))
                {
                    result.Add(element);
                    continue;
                }
                for (var i = element.childCount - 1; i >= 0; i--)
                    stack.Push(element[i]);
            }
            return result;
        }

        private static VisualElement? FirstDescendant(VisualElement root, Func<VisualElement, bool> match)
        {
            var stack = new Stack<VisualElement>();
            for (var i = root.childCount - 1; i >= 0; i--)
                stack.Push(root[i]);
            while (stack.Count > 0)
            {
                var element = stack.Pop();
                if (match(element))
                    return element;
                for (var i = element.childCount - 1; i >= 0; i--)
                    stack.Push(element[i]);
            }
            return null;
        }
    }
}
