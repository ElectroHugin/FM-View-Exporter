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
        /// <summary>Englisch ist Standard; Deutsch laesst sich oben rechts umschalten und wird gemerkt.</summary>
        private static bool German;

        /// <summary>True, wenn das Fenster wegen eines Sprachwechsels neu geoeffnet werden soll.</summary>
        public bool RestartRequested { get; private set; }
        private static readonly Color Brand = Color.FromArgb(0x1B, 0x6A, 0xF5);
        private const string ProjectUrl = "https://github.com/ElectroHugin/FM-View-Exporter";
        private const string IssuesUrl = ProjectUrl + "/issues";

        private readonly TextBox _folder = new TextBox { Dock = DockStyle.Fill };
        private readonly Label _status = new Label { AutoSize = true, Dock = DockStyle.Fill, Padding = new Padding(0, 6, 0, 6) };
        private readonly Button _install = new Button { AutoSize = true };
        private readonly Button _uninstall = new Button { AutoSize = true };
        private readonly Button _openExports = new Button { AutoSize = true };
        private readonly Button _save = new Button { AutoSize = true };
        private readonly Label _saved = new Label { AutoSize = true, ForeColor = Color.SeaGreen, Padding = new Padding(8, 8, 0, 0) };
        private readonly Panel _settingsPanel = new Panel();
        private readonly Dictionary<SettingDefinition, Control> _editors = new Dictionary<SettingDefinition, Control>();

        public SetupForm()
        {
            German = Preferences.Language == "de";
            Text = "FM View Exporter Setup";
            Font = new Font("Segoe UI", 9F);
            // Groessen werden selbst mit der Bildschirmskalierung multipliziert (S), damit bei 125 % oder 150 % nichts abgeschnitten wird.
            AutoScaleMode = AutoScaleMode.None;
            using (var graphics = CreateGraphics())
                _scale = Math.Max(1f, graphics.DpiX / 96f);
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(S(720), S(600));
            MinimumSize = new Size(S(600), S(480));
            try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); } catch { }

            _tabs.Padding = new Point(S(14), S(5));
            var tabs = _tabs;
            tabs.TabPages.Add(Page(T("Installation", "Installation"), BuildGamePage()));
            tabs.TabPages.Add(Page(T("Einstellungen", "Settings"), BuildSettingsPage()));
            tabs.TabPages.Add(Page(T("Über", "About"), BuildAboutPage()));

            // Docking folgt der umgekehrten Reihenfolge: zuerst das Fuellelement, dann der Kopf oben.
            Controls.Add(tabs);
            _header = BuildHeader();
            Controls.Add(_header);

            _folder.TextChanged += (s, e) => RefreshState();
            Load += (s, e) =>
            {
                FitToContent();
                FitToScreen();
                _folder.Text = GameLocator.Find() ?? "";
                RefreshState();
            };
        }

        private readonly float _scale = 1f;
        private readonly TabControl _tabs = new TabControl { Dock = DockStyle.Fill };
        private Control _header;
        private Control _gameGrid;
        private Control _settingsGrid;
        private Control _settingsBar;
        private Control _aboutColumn;

        private int S(int pixels) => (int)Math.Round(pixels * _scale);

        private static string T(string de, string en) => German ? de : en;

        private static TabPage Page(string title, Control content)
        {
            var page = new TabPage(title) { Padding = new Padding(10), UseVisualStyleBackColor = true };
            page.Controls.Add(content);
            return page;
        }

        /// <summary>Fenster so hoch machen, dass der laengste Reiter ohne Scrollen passt.</summary>
        private void FitToContent()
        {
            try
            {
                var content = Math.Max(_gameGrid.PreferredSize.Height,
                              Math.Max(_settingsGrid.PreferredSize.Height + _settingsBar.PreferredSize.Height,
                                       _aboutColumn.PreferredSize.Height));
                var tabChrome = _tabs.Height - _tabs.DisplayRectangle.Height;
                var pagePadding = 2 * S(10);
                var height = _header.Height + tabChrome + pagePadding + content + S(12);
                var width = Math.Max(ClientSize.Width, _settingsGrid.PreferredSize.Width + 2 * S(10) + SystemInformation.VerticalScrollBarWidth + S(24));
                ClientSize = new Size(width, Math.Max(ClientSize.Height, height));
            }
            catch
            {
            }
        }

        /// <summary>Fenster nie groesser als der freie Bildschirm (kleine Laptops, hohe Skalierung).</summary>
        private void FitToScreen()
        {
            var area = Screen.FromControl(this).WorkingArea;
            Width = Math.Min(Width, area.Width);
            Height = Math.Min(Height, area.Height);
            MinimumSize = new Size(Math.Min(MinimumSize.Width, area.Width), Math.Min(MinimumSize.Height, area.Height));
            Location = new Point(area.Left + (area.Width - Width) / 2, area.Top + (area.Height - Height) / 2);
        }

        private static string Version
        {
            get
            {
                var info = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "";
                var plus = info.IndexOf('+');
                return (plus >= 0 ? info.Substring(0, plus) : info).TrimStart('v');
            }
        }

        private Control BuildHeader()
        {
            var panel = new Panel { Dock = DockStyle.Top, Height = S(68), BackColor = Brand };
            var title = new Label
            {
                Text = "FM View Exporter",
                ForeColor = Color.White,
                Font = new Font("Segoe UI Semibold", 17F),
                AutoSize = true,
                Location = new Point(S(16), S(8))
            };
            var subtitle = new Label
            {
                Text = T("Setup und Einstellungen", "Setup and settings") + (Version.Length > 0 ? "  ·  " + Version : ""),
                ForeColor = Color.FromArgb(220, 232, 255),
                AutoSize = true,
                Location = new Point(S(19), S(42))
            };
            var language = new LinkLabel
            {
                Text = German ? "English" : "Deutsch",
                AutoSize = true,
                LinkColor = Color.White,
                ActiveLinkColor = Color.White,
                VisitedLinkColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            language.LinkClicked += (s, e) =>
            {
                Preferences.Language = German ? "en" : "de";
                RestartRequested = true;
                Close();
            };
            panel.Controls.Add(title);
            panel.Controls.Add(subtitle);
            panel.Controls.Add(language);
            panel.Layout += (s, e) => language.Location = new Point(panel.ClientSize.Width - language.Width - S(16), S(12));
            return panel;
        }

        private Control BuildGamePage()
        {
            var grid = new TableLayoutPanel { Dock = DockStyle.Top, ColumnCount = 3, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink };
            _gameGrid = grid;
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            var intro = new Label
            {
                Text = T("Das Setup findet Football Manager 27 über Steam. Liegt das Spiel woanders, wähle den Ordner mit fm.exe.",
                         "The setup finds Football Manager 27 through Steam. If the game is somewhere else, choose the folder that contains fm.exe."),
                AutoSize = true,
                MaximumSize = new Size(S(640), 0),
                Margin = new Padding(3, 0, 3, 12)
            };
            grid.Controls.Add(intro, 0, 0);
            grid.SetColumnSpan(intro, 3);

            var browse = new Button { Text = T("Durchsuchen…", "Browse…"), AutoSize = true };
            browse.Click += (s, e) => BrowseGameFolder();
            grid.Controls.Add(new Label { Text = T("Spielordner", "Game folder"), AutoSize = true, Anchor = AnchorStyles.Left, Padding = new Padding(0, 6, 8, 0) }, 0, 1);
            grid.Controls.Add(_folder, 1, 1);
            grid.Controls.Add(browse, 2, 1);
            _status.MaximumSize = new Size(S(640), 0);
            grid.Controls.Add(_status, 0, 2);
            grid.SetColumnSpan(_status, 3);

            var buttons = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Fill, WrapContents = true, Margin = new Padding(0, 6, 0, 0) };
            _install.Click += (s, e) => Install();
            _uninstall.Text = T("Deinstallieren", "Uninstall");
            _uninstall.Click += (s, e) => Uninstall();
            _openExports.Text = T("Exportordner öffnen", "Open export folder");
            _openExports.Click += (s, e) => OpenExportFolder();
            buttons.Controls.Add(_install);
            buttons.Controls.Add(_uninstall);
            buttons.Controls.Add(_openExports);
            grid.Controls.Add(buttons, 0, 3);
            grid.SetColumnSpan(buttons, 3);

            var hint = new Label
            {
                Text = T("Beim Aktualisieren bleiben deine Einstellungen erhalten. Beim Deinstallieren bleiben Spielstände unberührt.",
                         "Updating keeps your settings. Uninstalling never touches your saves."),
                AutoSize = true,
                MaximumSize = new Size(S(640), 0),
                ForeColor = SystemColors.GrayText,
                Margin = new Padding(3, 12, 3, 3)
            };
            grid.Controls.Add(hint, 0, 4);
            grid.SetColumnSpan(hint, 3);

            var scroll = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
            scroll.Controls.Add(grid);
            return scroll;
        }

        private Control BuildSettingsPage()
        {
            var grid = new TableLayoutPanel { Dock = DockStyle.Top, ColumnCount = 2, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink };
            _settingsGrid = grid;
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            var row = 0;
            foreach (var setting in ExporterSettings.All)
            {
                var label = new Label
                {
                    Text = German ? setting.LabelDe : setting.LabelEn,
                    AutoSize = true,
                    MaximumSize = new Size(S(200), 0),
                    Anchor = AnchorStyles.Left | AnchorStyles.Top,
                    Padding = new Padding(0, 6, 12, 0)
                };
                var editor = CreateEditor(setting);
                var help = new Label
                {
                    Text = German ? setting.HelpDe : setting.HelpEn,
                    AutoSize = true,
                    MaximumSize = new Size(S(400), 0),
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

            var scroll = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
            scroll.Controls.Add(grid);

            // Speichern steht fest unten und ist damit immer sichtbar, auch wenn die Liste scrollt.
            var bar = new TableLayoutPanel { Dock = DockStyle.Bottom, ColumnCount = 2, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Padding = new Padding(0, 8, 0, 0) };
            _settingsBar = bar;
            bar.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            bar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            _save.Text = T("Einstellungen speichern", "Save settings");
            _save.Click += (s, e) => SaveSettings();
            bar.Controls.Add(_save, 0, 0);
            bar.Controls.Add(_saved, 1, 0);
            var note = new Label
            {
                Text = T("Änderungen gelten ab dem nächsten Export, geänderte Tasten nach einem Neustart des Spiels.",
                         "Changes apply from the next export, changed keys after restarting the game."),
                AutoSize = true,
                MaximumSize = new Size(S(640), 0),
                ForeColor = SystemColors.GrayText,
                Margin = new Padding(3, 6, 3, 0)
            };
            bar.Controls.Add(note, 0, 1);
            bar.SetColumnSpan(note, 2);

            _settingsPanel.Dock = DockStyle.Fill;
            _settingsPanel.Controls.Add(scroll);
            _settingsPanel.Controls.Add(bar);
            return _settingsPanel;
        }

        private Control CreateEditor(SettingDefinition setting)
        {
            switch (setting.Kind)
            {
                case SettingKind.Boolean:
                    return new CheckBox { AutoSize = true, Text = T("an", "on"), Margin = new Padding(3, 5, 3, 0) };
                case SettingKind.Choice:
                    var combo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = S(260) };
                    foreach (var choice in setting.Choices)
                        combo.Items.Add(German ? choice.LabelDe : choice.LabelEn);
                    return combo;
                case SettingKind.Number:
                    return new NumericUpDown { Minimum = 1, Maximum = 60, Width = S(80) };
                case SettingKind.Folder:
                    var panel = new FlowLayoutPanel { AutoSize = true, WrapContents = false, Margin = Padding.Empty };
                    var text = new TextBox { Width = S(320) };
                    var pick = new Button { Text = "…", Width = S(32) };
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
                    return new TextBox { Width = S(220) };
            }
        }

        private Control BuildAboutPage()
        {
            var width = S(440);
            var column = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Anchor = AnchorStyles.Top
            };

            Label Line(string text, Font font = null, Color? color = null, bool center = true, int top = 0)
            {
                return new Label
                {
                    Text = text,
                    AutoSize = false,
                    Width = width,
                    Height = TextRenderer.MeasureText(text, font ?? Font, new Size(width, int.MaxValue), TextFormatFlags.WordBreak).Height + 2,
                    Font = font ?? Font,
                    ForeColor = color ?? SystemColors.ControlText,
                    TextAlign = center ? ContentAlignment.TopCenter : ContentAlignment.TopLeft,
                    Margin = new Padding(0, top, 0, 2)
                };
            }

            Control Separator() => new Label { AutoSize = false, Width = width, Height = S(1), BackColor = SystemColors.ControlLight, Margin = new Padding(0, 12, 0, 10) };

            var logo = new PictureBox
            {
                Image = LoadImage("electrohugin.png"),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
                Size = new Size(S(88), S(88)),
                Margin = new Padding((width - S(88)) / 2, 4, 0, 6)
            };
            column.Controls.Add(logo);
            column.Controls.Add(Line("FM View Exporter", new Font("Segoe UI Semibold", 17F)));
            column.Controls.Add(Line(T("Version ", "Version ") + (Version.Length > 0 ? Version : "?"), color: SystemColors.GrayText));
            column.Controls.Add(Line(T("Exportiert Kader, Spieler-Datenbank und Shortlist aus Football Manager 27 als CSV und HTML, passend für den FM Player Analyzer.",
                                       "Exports squad, player database and shortlist from Football Manager 27 as CSV and HTML, ready for the FM Player Analyzer."), top: 8));
            var author = Line(T("Entwickelt von ElectroHugin", "Made by ElectroHugin"), new Font(Font, FontStyle.Bold), top: 4);
            column.Controls.Add(author);
            column.Controls.Add(Separator());

            column.Controls.Add(Line("Support", new Font("Segoe UI Semibold", 10.5F), center: false));
            column.Controls.Add(Line(T("Fehler gefunden oder eine Idee? Bitte öffne ein Issue auf GitHub. So bleibt alles an einem Ort, und andere finden die Antwort später auch.",
                                       "Found a bug or have an idea? Please open an issue on GitHub. That keeps everything in one place, and others can find the answer later."), center: false));
            var issues = new LinkLabel { Text = IssuesUrl.Replace("https://", ""), AutoSize = true, Margin = new Padding(0, 4, 0, 4) };
            issues.LinkClicked += (s, e) => Open(IssuesUrl);
            column.Controls.Add(issues);
            column.Controls.Add(Line(T("Bitte mitschicken: Systeminfo (Knopf unten), welcher Bildschirm (Kader, Spieler-Datenbank, Shortlist) und die Datei BepInEx\\LogOutput.log.",
                                       "Please include: system info (button below), which screen (squad, player database, shortlist) and the file BepInEx\\LogOutput.log."),
                                     color: SystemColors.GrayText, center: false, top: 2));

            column.Controls.Add(Line(T("Für alles andere per E-Mail:", "For everything else, by email:"), center: false, top: 10));
            var mail = new PictureBox
            {
                Image = TextImage(SupportAddress(), new Font("Segoe UI", 10F), SystemColors.ControlText),
                SizeMode = PictureBoxSizeMode.AutoSize,
                BackColor = Color.Transparent,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(8, 4, 8, 4),
                Margin = new Padding(0, 4, 0, 2)
            };
            column.Controls.Add(mail);
            column.Controls.Add(Line(T("Die Adresse ist absichtlich ein Bild, damit Spam-Bots sie nicht auslesen können. Bitte von Hand abtippen.",
                                       "The address is an image on purpose so spam bots cannot read it. Please type it by hand."),
                                     new Font("Segoe UI", 8F), SystemColors.GrayText, center: false));

            var copy = new Button { Text = T("Systeminfo kopieren", "Copy system info"), AutoSize = true, Margin = new Padding(0, 10, 0, 0) };
            copy.Click += (s, e) =>
            {
                try { Clipboard.SetText(SystemInfo()); } catch { }
                copy.Text = T("Kopiert ✓", "Copied ✓");
                var timer = new Timer { Interval = 1800 };
                timer.Tick += (s2, e2) => { timer.Dispose(); copy.Text = T("Systeminfo kopieren", "Copy system info"); };
                timer.Start();
            };
            column.Controls.Add(copy);
            column.Controls.Add(Separator());

            column.Controls.Add(Line(T("© 2026 ElectroHugin · Freie Software unter der GNU GPL v3\nNicht offiziell mit Sports Interactive oder SEGA verbunden.",
                                       "© 2026 ElectroHugin · Free software under the GNU GPL v3\nNot affiliated with Sports Interactive or SEGA."),
                                     new Font("Segoe UI", 8F), SystemColors.GrayText));
            var project = new LinkLabel { Text = ProjectUrl.Replace("https://", ""), AutoSize = true, Margin = new Padding((width - TextRenderer.MeasureText(ProjectUrl.Replace("https://", ""), Font).Width) / 2, 4, 0, 4) };
            project.LinkClicked += (s, e) => Open(ProjectUrl);
            column.Controls.Add(project);

            // Spalte waagerecht mittig halten, senkrecht scrollen, wenn das Fenster klein ist.
            var scroll = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
            scroll.Controls.Add(column);
            _aboutColumn = column;
            scroll.Resize += (s, e) => column.Left = Math.Max(0, (scroll.ClientSize.Width - column.Width) / 2) + scroll.AutoScrollPosition.X;
            return scroll;
        }

        private static Image LoadImage(string resource)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
                return stream == null ? null : new Bitmap(Image.FromStream(stream));
        }

        // Die Support-Adresse steht nicht als Klartext in der .exe (wie im FM Player Analyzer) und wird nur als Bild gezeichnet.
        private static string SupportAddress()
        {
            byte[] key = { 0x5A, 0x13, 0xC7, 0x2E, 0x91 };
            byte[] scrambled =
            {
                0x1F, 0x7F, 0xA2, 0x4D, 0xE5, 0x28, 0x7C, 0xAF, 0x5B, 0xF6, 0x33,
                0x7D, 0x87, 0x49, 0xFC, 0x3B, 0x7A, 0xAB, 0x00, 0xF2, 0x35, 0x7E
            };
            var plain = new StringBuilder(scrambled.Length);
            for (var i = 0; i < scrambled.Length; i++)
                plain.Append((char)(scrambled[i] ^ key[i % key.Length]));
            return plain.ToString();
        }

        private static Image TextImage(string text, Font font, Color color)
        {
            var size = TextRenderer.MeasureText(text, font);
            var bitmap = new Bitmap(size.Width + 4, size.Height + 2);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.Transparent);
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                using (var brush = new SolidBrush(color))
                    graphics.DrawString(text, font, brush, 2, 1);
            }
            return bitmap;
        }

        private string SystemInfo()
        {
            var game = Game;
            return "FM View Exporter Setup " + Version + Environment.NewLine
                   + "Plugin: " + (game.HasPlugin ? InstalledVersion(game) : "-") + Environment.NewLine
                   + "BepInEx: " + (game.HasBepInEx ? "ok" : "-") + Environment.NewLine
                   + Environment.OSVersion.VersionString + (Environment.Is64BitOperatingSystem ? " (64-bit)" : "") + Environment.NewLine
                   + ".NET " + Environment.Version;
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
            _settingsPanel.Enabled = game.IsGameFolder;
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
                    var index = Array.FindIndex(setting.Choices, c => string.Equals(c.Value, value, StringComparison.OrdinalIgnoreCase));
                    combo.SelectedIndex = index >= 0 ? index : Array.FindIndex(setting.Choices, c => c.Value == setting.DefaultValue);
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
                    var selected = ((ComboBox)editor).SelectedIndex;
                    return selected >= 0 ? setting.Choices[selected].Value : setting.DefaultValue;
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
