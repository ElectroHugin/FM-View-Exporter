using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace FMViewExporter.Export
{
    /// <summary>
    /// Hebt waehrend des Exports die Bildraten-Begrenzung des Spiels auf (VSync, Ziel-Bildrate,
    /// gedrosseltes Zeichnen). Das Plugin wartet in Frames, deshalb ist der Export bei hoher
    /// Bildrate schneller. Danach werden die Werte des Spiels genau so wiederhergestellt.
    /// Es wird nichts gespeichert, die Einstellungen des Spiels bleiben unveraendert.
    /// </summary>
    internal sealed class FrameRateUnlock
    {
        // Obergrenze statt "unbegrenzt", damit die Grafikkarte nicht unnoetig hochdreht.
        private const int ExportFrameRate = 240;

        private readonly int _vSync;
        private readonly int _targetFrameRate;
        private readonly int _renderInterval;
        private bool _restored;

        private FrameRateUnlock(int vSync, int targetFrameRate, int renderInterval)
        {
            _vSync = vSync;
            _targetFrameRate = targetFrameRate;
            _renderInterval = renderInterval;
        }

        public static FrameRateUnlock Apply()
        {
            var unlock = new FrameRateUnlock(QualitySettings.vSyncCount, Application.targetFrameRate,
                OnDemandRendering.renderFrameInterval);
            try
            {
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = ExportFrameRate;
                OnDemandRendering.renderFrameInterval = 1;
                Plugin.Logger.LogInfo($"Bildrate fuer den Export: bis {ExportFrameRate} (vorher VSync {unlock._vSync}, " +
                    $"Ziel {unlock._targetFrameRate}, Zeichenintervall {unlock._renderInterval}).");
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogWarning($"Bildrate konnte nicht angehoben werden: {ex.Message}");
            }
            return unlock;
        }

        public void Restore()
        {
            if (_restored)
                return;
            _restored = true;
            QualitySettings.vSyncCount = _vSync;
            Application.targetFrameRate = _targetFrameRate;
            OnDemandRendering.renderFrameInterval = _renderInterval;
        }
    }
}
