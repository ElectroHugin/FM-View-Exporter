using System;
using System.Collections.Generic;
using System.Globalization;
using FMViewExporter.Core;
using FMViewExporter.Ui;
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

        /// <summary>
        /// Liest alle gerade gezeichneten Zeilen. Jede Zeile hat so viele Zellen wie die Tabelle Spalten;
        /// <c>Top</c> ist die Bildschirmposition der Zeile (fuer die Reihenfolge).
        /// </summary>
        public List<(float Top, List<string> Cells)> ReadVisibleRows()
        {
            var result = new List<(float, List<string>)>();
            foreach (var row in Descendants(Rows, e => e.ClassListContains(RowClass)))
            {
                if (IsHidden(row))
                    continue;
                var cells = new List<string>();
                foreach (var cell in Descendants(row, e => e.ClassListContains(CellClass)))
                    cells.Add(ReadCell(cell));
                if (cells.Count > 0)
                    result.Add((row.worldBound.y, cells));
            }
            return result;
        }

        private static string ReadCell(VisualElement cell)
        {
            var texts = new List<string>();
            var full = 0;
            var half = 0;
            var stars = 0;
            Visit(cell, e =>
            {
                if (IsHidden(e))
                    return false;
                var text = e.TryCast<TextElement>()?.text;
                if (!string.IsNullOrEmpty(text))
                    texts.Add(text!);
                if (e.ClassListContains(StarClass) || e.ClassListContains(StarClass + "-no-margin"))
                {
                    stars++;
                    var classes = e.GetClassesForIteration();
                    for (var i = 0; classes != null && i < classes.Count; i++)
                    {
                        var name = classes[i];
                        if (name.Contains("-max-"))
                            full++;
                        else if (name.Contains("-half-"))
                            half++;
                    }
                }
                return true;
            });

            var value = CellText.Join(texts);
            if (value.Length == 0 && stars > 0)
                value = (full + half * 0.5).ToString("0.0", CultureInfo.InvariantCulture);
            return value;
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
            // #playertableshortlist -> "shortlist"
            var tableName = table.name;
            if (tableName != null && tableName.Length > TableName.Length && IsTableName(tableName))
                return tableName.Substring(TableName.Length);
            for (var e = table.parent; e != null; e = e.parent)
            {
                var name = e.name;
                if (!string.IsNullOrEmpty(name) && name.EndsWith("Screen", StringComparison.Ordinal) && name.Length > "Screen".Length)
                    return name.Substring(0, name.Length - "Screen".Length);
            }
            return "players";
        }

        internal static bool IsHidden(VisualElement element)
        {
            try
            {
                return element.resolvedStyle.display == DisplayStyle.None || !element.visible;
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
