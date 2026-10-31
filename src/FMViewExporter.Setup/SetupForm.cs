using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace FMViewExporter.Setup
{
    /// <summary>
    /// Hauptfenster: Spielordner waehlen, installieren/aktualisieren/deinstallieren, Einstellungen bearbeiten.
    /// </summary>
    internal sealed class SetupForm : Form
    {
        private static readonly bool German = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "de";
        private static readonly Color Brand = Color.FromArgb(0x1B, 0x6A, 0xF5);
        private const string ProjectUrl = "https://github.com/ElectroHugin/FM-View-Exporter";

        private readonly TextBox _folder = new TextBox { Dock = DockStyle.Fill };
        private readonly Label _status = new Label { AutoSize = true, Dock = DockStyle.Fill, Padding = new Padding(0, 6, 0, 6) };
        private readonly Button _install = new Button { AutoSize = true };
        private readonly Button _uninstall = new Button { AutoSize = true };
        private readonly Button _openExports = new Button { AutoSize = true };
        private readonly Button _save = new Button { AutoSize = true };
        private readonly Label _saved = new Label { AutoSize = true, ForeColor = Color.SeaGreen, Padding = new Padding(8, 8, 0, 0) };
        private readonly GroupBox _settingsBox = new GroupBox { Dock = DockStyle.Fill, AutoSize = true, Padding = new Padding(10) };
        private readonly Dictionary<SettingDefinition, Control> _editors = new Dictionary<SettingDefinition, Control>();

        public SetupForm()
        {
            Text = "FM View Exporter Setup";
            Font = new Font("Segoe UI", 9F);
            AutoScaleMode = AutoScaleMode.Dpi;
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(640, 560);
            Size = new Size(720, 760);
            try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); } catch { }

            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, AutoScroll = true, Padding = new Padding(14) };
            layout.Controls.Add(BuildHeader());
            layout.Controls.Add(BuildGameBox());
            layout.Controls.Add(BuildSettingsBox());
            layout.Controls.Add(BuildFooter());
            Controls.Add(layout);

            _folder.TextChanged += (s, e) => RefreshState();
            Load += (s, e) =>
            {
                _folder.Text = GameLocator.Find() ?? "";
                RefreshState();
            };
        }

        private static string T(string de, string en) => German ? de : en;

        private static string Version
        {
            get
            {
                var info = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "";
                var plus = info.IndexOf('+');
                return plus >= 0 ? info.Substring(0, plus) : info;
            }
        }

        private Control BuildHeader()
        {
            var panel = new Panel { Dock = DockStyle.Top, Height = 72, BackColor = Brand, Margin = new Padding(0, 0, 0, 12) };
            var title = new Label
            {
                Text = "FM View Exporter",
                ForeColor = Color.White,
                Font = new Font("Segoe UI Semibold", 18F),
                AutoSize = true,
                Location = new Point(16, 10)
            };
            var subtitle = new Label
            {
                Text = T("Setup und Einstellungen", "Setup and settings") + (Version.Length > 0 ? "  ·  " + Version : ""),
                ForeColor = Color.FromArgb(220, 232, 255),
                AutoSize = true,
                Location = new Point(19, 46)
            };
            panel.Controls.Add(title);
            panel.Controls.Add(subtitle);
            return panel;
        }

        private Control BuildGameBox()
        {
            var box = new GroupBox { Text = T("Football Manager 27", "Football Manager 27"), Dock = DockStyle.Fill, AutoSize = true, Padding = new Padding(10) };
            var grid = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, AutoSize = true };
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            var browse = new Button { Text = T("Durchsuchen…", "Browse…"), AutoSize = true };
            browse.Click += (s, e) => BrowseGameFolder();
            grid.Controls.Add(new Label { Text = T("Spielordner", "Game folder"), AutoSize = true, Anchor = AnchorStyles.Left, Padding = new Padding(0, 6, 8, 0) }, 0, 0);
            grid.Controls.Add(_folder, 1, 0);
            grid.Controls.Add(browse, 2, 0);
            grid.Controls.Add(_status, 0, 1);
            grid.SetColumnSpan(_status, 3);

            var buttons = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Fill, WrapContents = true };
            _install.Click += (s, e) => Install();
            _uninstall.Text = T("Deinstallieren", "Uninstall");
            _uninstall.Click += (s, e) => Uninstall();
            _openExports.Text = T("Exportordner öffnen", "Open export folder");
            _openExports.Click += (s, e) => OpenExportFolder();
            buttons.Controls.Add(_install);
            buttons.Controls.Add(_uninstall);
            buttons.Controls.Add(_openExports);
            grid.Controls.Add(buttons, 0, 2);
            grid.SetColumnSpan(buttons, 3);

            box.Controls.Add(grid);
            return box;
        }

        private Control BuildSettingsBox()
        {
            _settingsBox.Text = T("Einstellungen", "Settings");
            var grid = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, AutoSize = true };
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            var row = 0;
            foreach (var setting in ExporterSettings.All)
            {
                var label = new Label
                {
                    Text = German ? setting.LabelDe : setting.LabelEn,
                    AutoSize = true,
                    Anchor = AnchorStyles.Left | AnchorStyles.Top,
                    Padding = new Padding(0, 6, 12, 0)
                };
                var editor = CreateEditor(setting);
                var help = new Label
                {
                    Text = German ? setting.HelpDe : setting.HelpEn,
                    AutoSize = true,
                    ForeColor = SystemColors.GrayText,
                    Margin = new Padding(3, 0, 3, 8)
                };
                var cell = new FlowLayoutPanel { FlowDirection = FlowDirection.TopDown, AutoSize = true, Dock = DockStyle.Fill, WrapContents = false };
                cell.Controls.Add(editor);
                cell.Controls.Add(help);
                grid.Controls.Add(label, 0, row);
                grid.Controls.Add(cell, 1, row);
                _editors[setting] = editor;
                row++;
            }

            var actions = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Fill };
            _save.Text = T("Einstellungen speichern", "Save settings");
            _save.Click += (s, e) => SaveSettings();
            actions.Controls.Add(_save);
            actions.Controls.Add(_saved);
            grid.Controls.Add(actions, 0, row);
            grid.SetColumnSpan(actions, 2);

            var note = new Label
            {
                Text = T("Änderungen gelten ab dem nächsten Export (F9), Tasten nach einem Neustart des Spiels.",
                         "Changes apply from the next export (F9), keys after restarting the game."),
                AutoSize = true,
                ForeColor = SystemColors.GrayText
            };
            grid.Controls.Add(note, 0, row + 1);
            grid.SetColumnSpan(note, 2);

            _settingsBox.Controls.Add(grid);
            return _settingsBox;
        }

        private Control CreateEditor(SettingDefinition setting)
        {
            switch (setting.Kind)
            {
                case SettingKind.Boolean:
                    return new CheckBox { AutoSize = true, Text = T("an", "on"), Margin = new Padding(3, 5, 3, 0) };
                case SettingKind.Choice:
                    var combo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 160 };
                    combo.Items.AddRange(setting.Choices);
                    return combo;
                case SettingKind.Number:
                    return new NumericUpDown { Minimum = 1, Maximum = 60, Width = 80 };
                case SettingKind.Folder:
                    var panel = new FlowLayoutPanel { AutoSize = true, WrapContents = false, Margin = Padding.Empty };
                    var text = new TextBox { Width = 320 };
                    var pick = new Button { Text = "…", Width = 32 };
                    pick.Click += (s, e) =>
                    {
                        using (var dialog = new FolderBrowserDialog { SelectedPath = text.Text })
                        {
                            if (dialog.ShowDialog(this) == DialogResult.OK)
                                text.Text = dialog.SelectedPath;
                        }
                    };
                    panel.Controls.Add(text);
                    panel.Controls.Add(pick);
                    panel.Tag = text;
                    return panel;
                default:
                    return new TextBox { Width = 220 };
            }
        }

        private Control BuildFooter()
        {
            var link = new LinkLabel { Text = ProjectUrl, AutoSize = true, Margin = new Padding(3, 10, 3, 3) };
            link.LinkClicked += (s, e) => Open(ProjectUrl);
            return link;
        }

        private GameInstallation Game => new GameInstallation(_folder.Text.Trim());

        private static Stream OpenPayload() => Assembly.GetExecutingAssembly().GetManifestResourceStream("payload.zip");

        private static readonly bool HasPayload = Array.IndexOf(Assembly.GetExecutingAssembly().GetManifestResourceNames(), "payload.zip") >= 0;

        private void RefreshState()
        {
            var game = Game;
            var lines = new List<string>();
            if (!game.IsGameFolder)
            {
                lines.Add(_folder.Text.Trim().Length == 0
                    ? T("FM27 wurde nicht automatisch gefunden. Bitte den Ordner mit fm.exe auswählen.",
                        "FM27 was not found automatically. Please choose the folder that contains fm.exe.")
                    : T("In diesem Ordner liegt keine fm.exe.", "There is no fm.exe in this folder."));
            }
            else if (!game.HasPlugin)
            {
                lines.Add(T("✗ FM View Exporter ist noch nicht installiert.", "✗ FM View Exporter is not installed yet."));
            }
            else
            {
                var version = InstalledVersion(game);
                lines.Add(T("✓ FM View Exporter ist installiert", "✓ FM View Exporter is installed") + (version.Length > 0 ? " (" + version + ")." : "."));
                lines.Add(game.HasBepInEx
                    ? T("✓ BepInEx ist installiert.", "✓ BepInEx is installed.")
                    : T("✗ BepInEx fehlt. Bitte neu installieren.", "✗ BepInEx is missing. Please install again."));
                if (!File.Exists(game.ConfigPath))
                    lines.Add(T("Die Einstellungsdatei entsteht beim ersten Spielstart; hier gespeicherte Werte werden übernommen.",
                                "The settings file is created on the first game start; values saved here are kept."));
            }
            if (!HasPayload)
                lines.Add(T("Hinweis: Dieses Setup enthält kein Paket (Entwickler-Build), Installieren ist deshalb aus.",
                            "Note: this setup contains no package (developer build), so installing is disabled."));
            _status.Text = string.Join(Environment.NewLine, lines);

            _install.Text = game.HasPlugin ? T("Aktualisieren", "Update") : T("Installieren", "Install");
            _install.Enabled = game.IsGameFolder && HasPayload;
            _uninstall.Enabled = game.IsGameFolder && (game.HasPlugin || game.HasBepInEx);
            _settingsBox.Enabled = game.IsGameFolder;
            _saved.Text = "";
            LoadSettings();
        }

        private static string InstalledVersion(GameInstallation game)
        {
            try
            {
                var info = FileVersionInfo.GetVersionInfo(Path.Combine(game.GameFolder, GameInstallation.PluginDll));
                return info.ProductVersion ?? info.FileVersion ?? "";
            }
            catch
            {
                return "";
            }
        }

        private void BrowseGameFolder()
        {
            using (var dialog = new FolderBrowserDialog
            {
                Description = T("Ordner von Football Manager 27 (mit fm.exe)", "Football Manager 27 folder (with fm.exe)"),
                SelectedPath = _folder.Text
            })
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                    _folder.Text = dialog.SelectedPath;
            }
        }

        private bool GameIsRunning()
        {
            if (Process.GetProcessesByName("fm").Length == 0)
                return false;
            MessageBox.Show(this, T("Football Manager läuft noch. Bitte das Spiel zuerst beenden.", "Football Manager is still running. Please quit the game first."),
                Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return true;
        }

        private void Install()
        {
            if (GameIsRunning())
                return;
            var game = Game;
            var update = game.HasPlugin;
            try
            {
                Cursor = Cursors.WaitCursor;
                using (var payload = OpenPayload())
                    game.Install(payload, pluginOnly: false);
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return;
            }
            finally
            {
                Cursor = Cursors.Default;
            }

            MessageBox.Show(this, update
                    ? T("FM View Exporter wurde aktualisiert. Deine Einstellungen bleiben erhalten.", "FM View Exporter was updated. Your settings were kept.")
                    : T("FM View Exporter ist installiert.\n\nDer erste Spielstart dauert einige Minuten, weil BepInEx einmalig Dateien erzeugt. Danach im Kader oder in der Spieler-Datenbank F9 drücken.",
                        "FM View Exporter is installed.\n\nThe first game start takes a few minutes while BepInEx prepares itself. Then press F9 in the squad or player database."),
                Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            RefreshState();
        }

        private void Uninstall()
        {
            if (GameIsRunning())
                return;
            var game = Game;
            var others = game.OtherPlugins();
            var question = others.Count > 0
                ? T("FM View Exporter und seine Einstellungen entfernen? BepInEx bleibt, weil weitere Mods installiert sind: ",
                    "Remove FM View Exporter and its settings? BepInEx stays because other mods are installed: ") + string.Join(", ", others)
                : T("FM View Exporter, seine Einstellungen und BepInEx aus dem Spielordner entfernen? Spielstände bleiben unberührt.",
                    "Remove FM View Exporter, its settings and BepInEx from the game folder? Your saves are not affected.");
            if (MessageBox.Show(this, question, Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;
            try
            {
                game.Uninstall();
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return;
            }
            MessageBox.Show(this, T("Entfernt.", "Removed."), Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            RefreshState();
        }

        private BepInExConfigFile ReadConfig()
        {
            var path = Game.ConfigPath;
            return new BepInExConfigFile(File.Exists(path) ? File.ReadAllText(path) : "");
        }

        private void LoadSettings()
        {
            if (!Game.IsGameFolder)
                return;
            BepInExConfigFile config;
            try
            {
                config = ReadConfig();
            }
            catch
            {
                config = new BepInExConfigFile("");
            }
            foreach (var pair in _editors)
                SetEditorValue(pair.Key, pair.Value, config.Get(pair.Key.Section, pair.Key.Key) ?? pair.Key.DefaultValue);
        }

        private void SaveSettings()
        {
            var game = Game;
            try
            {
                var config = ReadConfig();
                foreach (var pair in _editors)
                    config.Set(pair.Key.Section, pair.Key.Key, GetEditorValue(pair.Key, pair.Value));
                Directory.CreateDirectory(Path.GetDirectoryName(game.ConfigPath));
                File.WriteAllText(game.ConfigPath, config.ToString(), new UTF8Encoding(false));
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return;
            }
            _saved.Text = T("✓ Gespeichert", "✓ Saved");
        }

        private static void SetEditorValue(SettingDefinition setting, Control editor, string value)
        {
            switch (setting.Kind)
            {
                case SettingKind.Boolean:
                    ((CheckBox)editor).Checked = string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
                    break;
                case SettingKind.Choice:
                    var combo = (ComboBox)editor;
                    var index = Array.FindIndex(setting.Choices, c => string.Equals(c, value, StringComparison.OrdinalIgnoreCase));
                    combo.SelectedIndex = index >= 0 ? index : Array.IndexOf(setting.Choices, setting.DefaultValue);
                    break;
                case SettingKind.Number:
                    var number = (NumericUpDown)editor;
                    number.Value = int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var n)
                        ? Math.Max(number.Minimum, Math.Min(number.Maximum, n))
                        : decimal.Parse(setting.DefaultValue, CultureInfo.InvariantCulture);
                    break;
                case SettingKind.Folder:
                    ((TextBox)editor.Tag).Text = value;
                    break;
                default:
                    editor.Text = value;
                    break;
            }
        }

        private static string GetEditorValue(SettingDefinition setting, Control editor)
        {
            switch (setting.Kind)
            {
                case SettingKind.Boolean:
                    return ((CheckBox)editor).Checked ? "true" : "false";
                case SettingKind.Choice:
                    return ((ComboBox)editor).SelectedItem as string ?? setting.DefaultValue;
                case SettingKind.Number:
                    return ((int)((NumericUpDown)editor).Value).ToString(CultureInfo.InvariantCulture);
                case SettingKind.Folder:
                    return ((TextBox)editor.Tag).Text.Trim();
                default:
                    return editor.Text.Trim();
            }
        }

        private void OpenExportFolder()
        {
            var configured = "";
            try
            {
                configured = ReadConfig().Get("Export", "OutputFolder") ?? "";
            }
            catch
            {
            }
            var folder = configured.Length > 0
                ? configured
                : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Sports Interactive", "FM View Exporter");
            Directory.CreateDirectory(folder);
            Open(folder);
        }

        private static void Open(string target)
        {
            try
            {
                Process.Start(new ProcessStartInfo(target) { UseShellExecute = true });
            }
            catch
            {
            }
        }

        private void ShowError(Exception ex)
        {
            MessageBox.Show(this, T("Das hat nicht geklappt:\n\n", "That did not work:\n\n") + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
