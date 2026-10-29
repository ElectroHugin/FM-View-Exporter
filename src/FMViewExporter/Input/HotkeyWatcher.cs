using System;
using System.Runtime.InteropServices;
using FMViewExporter.Core;

namespace FMViewExporter.Input
{
    /// <summary>
    /// Fragt eine Tastenkombination direkt bei Windows ab. Das funktioniert unabhaengig vom
    /// Eingabesystem des Spiels und reagiert nur, wenn das Spielfenster im Vordergrund ist.
    /// </summary>
    internal sealed class HotkeyWatcher
    {
        private readonly Hotkey _hotkey;
        private bool _wasDown;

        private HotkeyWatcher(Hotkey hotkey)
        {
            _hotkey = hotkey;
        }

        public static HotkeyWatcher? FromConfig(string text, string purpose, string fallback)
        {
            if (Hotkey.TryParse(text, out var hotkey))
                return new HotkeyWatcher(hotkey);

            Plugin.Logger.LogWarning($"Taste \"{text}\" fuer {purpose} nicht erkannt, nehme {fallback}.");
            return Hotkey.TryParse(fallback, out hotkey) ? new HotkeyWatcher(hotkey) : null;
        }

        public bool WasPressed()
        {
            var down = IsDown(_hotkey.VirtualKey)
                && IsDown(Hotkey.VkControl) == _hotkey.Ctrl
                && IsDown(Hotkey.VkShift) == _hotkey.Shift
                && IsDown(Hotkey.VkAlt) == _hotkey.Alt;

            var pressed = down && !_wasDown && GameWindowHasFocus();
            _wasDown = down;
            return pressed;
        }

        private static bool IsDown(int virtualKey) => (GetAsyncKeyState(virtualKey) & 0x8000) != 0;

        private static bool GameWindowHasFocus()
        {
            var window = GetForegroundWindow();
            if (window == IntPtr.Zero)
                return false;
            GetWindowThreadProcessId(window, out var processId);
            return processId == (uint)Environment.ProcessId;
        }

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);
    }
}
