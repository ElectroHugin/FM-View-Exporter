using System;
using System.IO;

namespace FMViewExporter
{
    internal static class OutputPaths
    {
        public static string BaseFolder
        {
            get
            {
                var configured = Plugin.Settings.OutputFolder.Value;
                if (!string.IsNullOrWhiteSpace(configured))
                    return configured;

                var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                return Path.Combine(documents, "Sports Interactive", "FM View Exporter");
            }
        }

        public static string DiagnosticsFolder => Path.Combine(BaseFolder, "Diagnose");

        public static string EnsureFolder(string folder)
        {
            Directory.CreateDirectory(folder);
            return folder;
        }
    }
}
