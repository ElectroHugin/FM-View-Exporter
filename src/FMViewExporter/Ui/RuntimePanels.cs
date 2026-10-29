using System;
using System.Collections.Generic;
using FMViewExporter.Diagnostics;
using UnityEngine.UIElements;

namespace FMViewExporter.Ui
{
    /// <summary>
    /// Findet die UI-Toolkit-Panels, die das Spiel gerade zeichnet.
    /// </summary>
    internal static class RuntimePanels
    {
        internal sealed class PanelInfo
        {
            public PanelInfo(VisualElement root, string description)
            {
                Root = root;
                Description = description;
            }

            public VisualElement Root { get; }
            public string Description { get; }
        }

        public static List<PanelInfo> Find()
        {
            var result = new List<PanelInfo>();
            var seen = new HashSet<IntPtr>();

            try
            {
                var panels = UIElementsRuntimeUtility.GetSortedPlayerPanels();
                for (var i = 0; i < panels.Count; i++)
                {
                    var panel = panels[i];
                    var root = panel?.visualTree;
                    if (root != null && seen.Add(root.Pointer))
                        result.Add(new PanelInfo(root, $"{Il2CppInspector.TypeName(panel)} \"{panel!.name}\""));
                }
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogWarning($"Panels ueber UIElementsRuntimeUtility nicht lesbar: {ex.Message}");
            }

            if (result.Count == 0)
            {
                // Rueckfall: ueber die UIDocument-Komponenten in der Szene.
                var documents = UnityEngine.Object.FindObjectsOfType(Il2CppInterop.Runtime.Il2CppType.Of<UIDocument>());
                foreach (var obj in documents)
                {
                    var document = obj.TryCast<UIDocument>();
                    var root = document?.rootVisualElement;
                    if (root != null && seen.Add(root.Pointer))
                        result.Add(new PanelInfo(root, $"UIDocument \"{document!.name}\""));
                }
            }

            return result;
        }
    }
}
