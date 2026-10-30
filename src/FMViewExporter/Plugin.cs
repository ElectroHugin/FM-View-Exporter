using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using UnityEngine;

namespace FMViewExporter
{
    /// <summary>
    /// Einstieg fuer BepInEx. Das Plugin liest nur die Oberflaeche des Spiels,
    /// veraendert keinen Spielstand und baut keine Netzwerkverbindung auf.
    /// </summary>
    [BepInPlugin(PluginInfo.Guid, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BasePlugin
    {
        internal static ManualLogSource Logger { get; private set; } = null!;
        internal static PluginConfig Settings { get; private set; } = null!;
        internal static BepInEx.Configuration.ConfigFile ConfigFile { get; private set; } = null!;

        public override void Load()
        {
            Logger = Log;
            ConfigFile = Config;
            Settings = new PluginConfig(Config);

            Logger.LogInfo($"{PluginInfo.Name} {PluginInfo.Version} geladen. Spiel: {Application.productName} {Application.version}, Unity {Application.unityVersion}");
            Logger.LogInfo($"Ausgabeordner: {OutputPaths.BaseFolder}");

            AddComponent<ExporterBehaviour>();
        }
    }
}
