using System;
using UnityEngine.UIElements;

namespace FMViewExporter.Export
{
    /// <summary>
    /// Zieht die Zeilenliste waehrend des Exports in die Hoehe, damit das Spiel pro Scrollschritt
    /// viel mehr Zeilen zeichnet. Der sichtbare Teil wird vom Fenster abgeschnitten, die Zeilen
    /// werden trotzdem befuellt. Danach werden die urspruenglichen Stilwerte zurueckgesetzt.
    /// Es aendert nur die Darstellung, nichts am Spielstand.
    /// </summary>
    internal sealed class TallList
    {
        private const float MaxHeight = 30000f;

        private readonly Saved[] _saved;

        private TallList(Saved[] saved)
        {
            _saved = saved;
        }

        public static TallList? Apply(ScrollView rows, int rowCount, float rowHeight)
        {
            if (rowCount <= 0)
                return null;
            var height = Math.Min(MaxHeight, rowCount * Math.Max(10f, rowHeight));
            if (height <= rows.contentViewport.layout.height)
                return null;

            // Die ScrollView selbst und ihre Liste (SI.UI.VirtualisedList) drumherum.
            var targets = rows.parent != null ? new VisualElement[] { rows.parent, rows } : new VisualElement[] { rows };
            var saved = new Saved[targets.Length];
            for (var i = 0; i < targets.Length; i++)
            {
                var style = targets[i].style;
                saved[i] = new Saved(targets[i], style.height, style.minHeight, style.maxHeight, style.flexShrink);
                style.height = new StyleLength(height);
                style.minHeight = new StyleLength(height);
                style.maxHeight = new StyleLength(height);
                style.flexShrink = new StyleFloat(0f);
            }
            return new TallList(saved);
        }

        public void Restore()
        {
            foreach (var s in _saved)
            {
                try
                {
                    var style = s.Element.style;
                    style.height = s.Height;
                    style.minHeight = s.MinHeight;
                    style.maxHeight = s.MaxHeight;
                    style.flexShrink = s.FlexShrink;
                }
                catch (Exception ex)
                {
                    Plugin.Logger.LogWarning($"Listenhoehe konnte nicht zurueckgesetzt werden: {ex.Message}");
                }
            }
        }

        private sealed class Saved
        {
            public Saved(VisualElement element, StyleLength height, StyleLength minHeight, StyleLength maxHeight, StyleFloat flexShrink)
            {
                Element = element;
                Height = height;
                MinHeight = minHeight;
                MaxHeight = maxHeight;
                FlexShrink = flexShrink;
            }

            public VisualElement Element { get; }
            public StyleLength Height { get; }
            public StyleLength MinHeight { get; }
            public StyleLength MaxHeight { get; }
            public StyleFloat FlexShrink { get; }
        }
    }
}
