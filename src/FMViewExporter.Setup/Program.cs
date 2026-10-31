using System;
using System.Windows.Forms;

namespace FMViewExporter.Setup
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            bool restart;
            do
            {
                using (var form = new SetupForm())
                {
                    Application.Run(form);
                    restart = form.RestartRequested;
                }
            }
            while (restart);
        }
    }
}
