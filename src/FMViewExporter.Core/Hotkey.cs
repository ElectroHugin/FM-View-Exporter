using System;
using System.Collections.Generic;

namespace FMViewExporter.Core
{
    /// <summary>
    /// Eine Tastenkombination wie "F9" oder "Ctrl+Shift+E", als Windows-Tastencode.
    /// Das Plugin fragt die Tasten ueber Windows ab, damit es unabhaengig davon funktioniert,
    /// welches Eingabesystem das Spiel verwendet.
    /// </summary>
    public readonly struct Hotkey
    {
        public const int VkShift = 0x10;
        public const int VkControl = 0x11;
        public const int VkAlt = 0x12;

        private static readonly Dictionary<string, int> NamedKeys = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Insert"] = 0x2D, ["Delete"] = 0x2E, ["Home"] = 0x24, ["End"] = 0x23,
            ["PageUp"] = 0x21, ["PageDown"] = 0x22, ["Pause"] = 0x13, ["ScrollLock"] = 0x91,
        };

        public Hotkey(int virtualKey, bool ctrl, bool shift, bool alt)
        {
            VirtualKey = virtualKey;
            Ctrl = ctrl;
            Shift = shift;
            Alt = alt;
        }

        public int VirtualKey { get; }
        public bool Ctrl { get; }
        public bool Shift { get; }
        public bool Alt { get; }

        public static bool TryParse(string? text, out Hotkey hotkey)
        {
            hotkey = default;
            if (string.IsNullOrWhiteSpace(text))
                return false;

            bool ctrl = false, shift = false, alt = false;
            int? key = null;
            foreach (var rawPart in text!.Split('+'))
            {
                var part = rawPart.Trim();
                if (part.Equals("Ctrl", StringComparison.OrdinalIgnoreCase) || part.Equals("Strg", StringComparison.OrdinalIgnoreCase))
                    ctrl = true;
                else if (part.Equals("Shift", StringComparison.OrdinalIgnoreCase))
                    shift = true;
                else if (part.Equals("Alt", StringComparison.OrdinalIgnoreCase))
                    alt = true;
                else if (key == null && TryParseKey(part, out var vk))
                    key = vk;
                else
                    return false;
            }

            if (key == null)
                return false;

            hotkey = new Hotkey(key.Value, ctrl, shift, alt);
            return true;
        }

        private static bool TryParseKey(string name, out int vk)
        {
            vk = 0;
            if (name.Length >= 2 && (name[0] == 'F' || name[0] == 'f')
                && int.TryParse(name.Substring(1), out var n) && n >= 1 && n <= 24)
            {
                vk = 0x70 + n - 1;
                return true;
            }

            if (name.Length == 1)
            {
                var c = char.ToUpperInvariant(name[0]);
                if ((c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9'))
                {
                    vk = c;
                    return true;
                }
            }

            return NamedKeys.TryGetValue(name, out vk);
        }
    }
}
