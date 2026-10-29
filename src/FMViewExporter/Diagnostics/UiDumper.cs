using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using FMViewExporter.Ui;
using UnityEngine;
using UnityEngine.UIElements;

namespace FMViewExporter.Diagnostics
{
    /// <summary>
    /// Schreibt ein Abbild der sichtbaren Oberflaeche in eine Textdatei: Elementbaum mit Typ,
    /// Name, Klassen, Position und Text, dazu Details zu Listen und ihren Datenobjekten.
    /// Daraus wird abgeleitet, wie Kader und Spielersuche ausgelesen werden.
    /// </summary>
    internal static class UiDumper
    {
        private const int MaxElements = 60000;
        private const int MaxDepth = 80;
        private const int ItemsToShow = 3;

        public static void DumpToFile()
        {
            var started = DateTime.Now;
            var folder = OutputPaths.EnsureFolder(OutputPaths.DiagnosticsFolder);
            var path = Path.Combine(folder, "ui-abbild_" + started.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture) + ".txt");

            using var writer = new StreamWriter(path, false, new UTF8Encoding(false));
            var dumper = new Dumper(writer);

            writer.WriteLine($"# FM View Exporter {PluginInfo.Version} - UI-Abbild");
            writer.WriteLine($"# Zeit: {started:yyyy-MM-dd HH:mm:ss}");
            writer.WriteLine($"# Spiel: {Application.productName} {Application.version}, Unity {Application.unityVersion}");
            writer.WriteLine("# Format: Typ #name .klassen [x,y breite x hoehe] \"text\" ds=Datenquelle ud=userData");
            writer.WriteLine();

            var panels = RuntimePanels.Find();
            writer.WriteLine($"# {panels.Count} Panel(s) gefunden");
            for (var i = 0; i < panels.Count; i++)
            {
                var panel = panels[i];
                writer.WriteLine();
                writer.WriteLine($"== Panel {i}: {panel.Description}");
                dumper.Walk(panel.Root, 0);
            }

            writer.WriteLine();
            writer.WriteLine($"# {dumper.ElementCount} Elemente, {dumper.ListCount} Listen, {(DateTime.Now - started).TotalMilliseconds:0} ms");
            writer.Flush();

            Plugin.Logger.LogMessage($"UI-Abbild geschrieben: {path} ({dumper.ElementCount} Elemente, {dumper.ListCount} Listen)");
        }

        private sealed class Dumper
        {
            private readonly StreamWriter _writer;

            public Dumper(StreamWriter writer)
            {
                _writer = writer;
            }

            public int ElementCount { get; private set; }
            public int ListCount { get; private set; }

            public void Walk(VisualElement element, int depth)
            {
                if (ElementCount >= MaxElements)
                    return;
                ElementCount++;

                var indent = new string(' ', depth * 2);
                var hidden = IsHidden(element);
                _writer.WriteLine(indent + Describe(element, hidden));

                if (LooksLikeList(element))
                    DescribeList(element, indent + "  | ");

                var childCount = element.childCount;
                if (hidden && childCount > 0)
                {
                    _writer.WriteLine($"{indent}  ({childCount} Kinder ausgeblendet, uebersprungen)");
                    return;
                }
                if (depth >= MaxDepth)
                {
                    _writer.WriteLine($"{indent}  (maximale Tiefe erreicht)");
                    return;
                }

                for (var i = 0; i < childCount; i++)
                    Walk(element[i], depth + 1);
            }

            private static bool IsHidden(VisualElement element)
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

            private static string Describe(VisualElement element, bool hidden)
            {
                var line = new StringBuilder();
                line.Append(Il2CppInspector.TypeName(element));

                var name = element.name;
                if (!string.IsNullOrEmpty(name))
                    line.Append(" #").Append(name);

                var classes = element.GetClassesForIteration();
                if (classes != null)
                {
                    for (var i = 0; i < classes.Count; i++)
                        line.Append(" .").Append(classes[i]);
                }

                var bounds = element.worldBound;
                line.Append(CultureInfo.InvariantCulture, $" [{bounds.x:0},{bounds.y:0} {bounds.width:0}x{bounds.height:0}]");

                var text = element.TryCast<TextElement>()?.text;
                if (!string.IsNullOrEmpty(text))
                    line.Append(" \"").Append(Clean(text!)).Append('"');

                AppendObject(line, " ds=", element.dataSource);
                AppendObject(line, " ud=", element.userData);

                if (hidden)
                    line.Append(" HIDDEN");
                return line.ToString();
            }

            private static void AppendObject(StringBuilder line, string label, Il2CppSystem.Object? value)
            {
                if (value != null)
                    line.Append(label).Append(Il2CppInspector.TypeName(value));
            }

            private static string Clean(string text)
            {
                text = text.Replace("\r", "\\r").Replace("\n", "\\n").Replace("\"", "'");
                return text.Length > 200 ? text.Substring(0, 200) + "..." : text;
            }

            private static bool LooksLikeList(VisualElement element)
            {
                foreach (var typeName in Il2CppInspector.TypeChain(element))
                {
                    if (typeName.EndsWith("VirtualisedList", StringComparison.Ordinal)
                        || typeName.EndsWith("BaseVerticalCollectionView", StringComparison.Ordinal)
                        || typeName == "UnityEngine.UIElements.ScrollView")
                        return true;
                }
                return false;
            }

            private void DescribeList(VisualElement element, string prefix)
            {
                ListCount++;
                try
                {
                    var scrollView = element.TryCast<ScrollView>();
                    if (scrollView != null)
                        _writer.WriteLine(prefix + $"ScrollView scrollOffset={scrollView.scrollOffset}");

                    foreach (var member in new[] { "ActiveView", "VisibleView", "ScrollOffset", "ContentSize", "ViewportSize", "FixedElementSize", "EstimatedElementSize" })
                    {
                        var value = Il2CppInspector.GetMember(element, member);
                        if (value != null)
                            _writer.WriteLine(prefix + member + " = " + Il2CppInspector.Short(value));
                    }

                    var items = Il2CppInspector.GetMember(element, "Items") ?? Il2CppInspector.GetMember(element, "itemsSource");
                    if (items == null)
                        return;

                    var count = Il2CppInspector.GetInt(items, "Count") ?? Il2CppInspector.GetInt(items, "Length");
                    _writer.WriteLine(prefix + $"Items: {Il2CppInspector.TypeName(items)}, Anzahl={count?.ToString(CultureInfo.InvariantCulture) ?? "?"}");
                    if (count is not > 0)
                        return;

                    for (var i = 0; i < Math.Min(ItemsToShow, count.Value); i++)
                    {
                        var item = Il2CppInspector.GetListItem(items, i);
                        _writer.WriteLine(prefix + $"Item[{i}]: {Il2CppInspector.TypeName(item)} = {Il2CppInspector.Short(item)}");
                        if (i == 0 && item != null)
                        {
                            var budget = 400;
                            Il2CppInspector.Describe(item, prefix + "  ", 2, _writer.WriteLine, ref budget);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _writer.WriteLine(prefix + "Fehler beim Lesen der Liste: " + ex.GetType().Name + ": " + ex.Message);
                }
            }
        }
    }
}
