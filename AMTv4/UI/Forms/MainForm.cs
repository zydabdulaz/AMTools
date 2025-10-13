using System.Diagnostics;
using System.Net;
using System.Reflection;
using ArdysaModsTools.Core.Services;


namespace ArdysaModsTools
{
    public partial class MainForm : Form
    {
        private string? targetPath = null;
        private static readonly HttpClient httpClient = new HttpClient(new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        });
        private const string RequiredModFilePath = "game/_ArdysaMods/pak01_dir.vpk";

        private readonly Logger _logger;
        private readonly UpdaterService _updater = null!;
        private readonly ModInstallerService _modInstaller;
        private readonly DetectionService _detection;
        private readonly StatusService _status;
        private readonly MainConfigService _configService;


        public MainForm()
        {
            InitializeComponent();
            _logger = new Logger(mainConsoleBox);
            _logger.Log("Ensure the 'game' folder is in the same directory as ArdysaModsTools.exe for installation to work!");
            _updater = new UpdaterService(_logger);
            _updater.OnVersionChanged += version =>
            {
                if (InvokeRequired)
                    Invoke(new Action(() => versionLabel.Text = $"Version: {version}"));
                else
                    versionLabel.Text = $"Version: {version}";
            };

            _updater.OnProgressChanged = (progress) =>
            {
                // We know progressBar is created by InitializeComponent(), so it's safe
                if (progressBar!.InvokeRequired)
                    progressBar.Invoke(new Action(() => progressBar.Value = progress));
                else
                    progressBar.Value = progress;
            };

            _modInstaller = new ModInstallerService(_logger);

            _detection = new DetectionService(_logger);

            _status = new StatusService(_logger);

            _configService = new MainConfigService();

            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
            httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
            httpClient.Timeout = TimeSpan.FromMinutes(10);
            ServicePointManager.DefaultConnectionLimit = 10;

            try
            {
                using Stream? iconStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ArdysaModsTools.AppIcon.ico");
                if (iconStream == null)
                {
                    _logger.Log("Embedded icon 'AppIcon.ico' not found.");
                }
                else
                {
                    this.Icon = new Icon(iconStream);
                }
            }
            catch (Exception ex)
            {
                _logger.Log($"Error loading application icon: {ex.Message}");
            }

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length >= 2 && args[1] == "--update" && args.Length >= 5)
            {
                string currentExe = args[2];
                string backupExe = args[3];
                string tempArchive = args[4];
                string tempDir = args[5];

                Task.Run(async () =>
                {
                    try
                    {
                        await Task.Delay(2000);
                        if (File.Exists(backupExe)) File.Delete(backupExe);
                        string thisExe = Process.GetCurrentProcess().MainModule!.FileName;
                        File.Move(thisExe, currentExe, true);
                        if (File.Exists(tempArchive)) File.Delete(tempArchive);
                        if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
                    }
                    catch (Exception ex)
                    {
                        _logger.Log($"Update cleanup failed: {ex.Message}");
                        MessageBox.Show($"Update cleanup failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Application.Exit();
                    }
                });
            }

            // Set initial status to "Not Checked" on startup
            statusModsDotLabel.BackColor = Color.FromArgb(150, 150, 150);
            statusModsTextLabel.Text = "Not Checked";
            statusModsTextLabel.ForeColor = Color.FromArgb(150, 150, 150);

            EnableDetectionButtonsOnly();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            _logger.FlushBufferedLogs();
            try
            {
                var last = _configService.GetLastTargetPath();
                if (!string.IsNullOrEmpty(last))
                {
                    if (Directory.Exists(last))
                    {
                        targetPath = last;
                        EnableAllButtons();
                    }
                    else
                    {
                        _configService.SetLastTargetPath(null);
                    }
                }

                await _updater.CheckForUpdatesAsync();
                await UpdateVersionLabelAsync();

                // Load Discord icon (dc.png)
                using (Stream? discordStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ArdysaModsTools.dc.png"))
                {
                    if (discordStream == null)
                    {
                    }
                    else
                    {
                        discordPictureBox.Image = Image.FromStream(discordStream);
                    }
                }

                // Load YouTube icon (yt.png)
                using (Stream? youtubeStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ArdysaModsTools.yt.png"))
                {
                    if (youtubeStream == null)
                    {
                    }
                    else
                    {
                        youtubePictureBox.Image = Image.FromStream(youtubeStream);
                    }
                }

                // Load PayPal icon (paypal.png)
                using (Stream? paypalStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ArdysaModsTools.paypal.png"))
                {
                    if (paypalStream == null)
                    {
                    }
                    else
                    {
                        paypalPictureBox.Image = Image.FromStream(paypalStream);
                    }
                }

                // ✅ Ensure that only detection and icons are enabled at startup
                EnableDetectionButtonsOnly();
            }
            catch (Exception ex)
            {
                _logger.Log($"Error loading social media icons: {ex.Message}");
            }
        }


        private void DiscordPictureBox_Click(object? sender, EventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://discord.gg/ffXw265Z7e",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                _logger.Log($"Failed to open Discord link: {ex.Message}");
                MessageBox.Show($"Failed to open Discord link: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void YoutubePictureBox_Click(object? sender, EventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://youtube.com/@Ardysa",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                _logger.Log($"Failed to open YouTube link: {ex.Message}");
                MessageBox.Show($"Failed to open YouTube link: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PaypalPictureBox_Click(object? sender, EventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://paypal.me/Ardysa",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                _logger.Log($"Failed to open PayPal link: {ex.Message}");
                MessageBox.Show($"Failed to open PayPal link: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MiscellaneousButton_Click(object? sender, EventArgs e)
        {
            using (var miscForm = new MiscForm(targetPath, _logger.Log, DisableAllButtons, EnableAllButtons))
            {
                miscForm.ShowDialog(this);
            }
        }

        private void Button_MouseEnter(object? sender, EventArgs e)
        {
            if (sender is Button btn && btn.Enabled)
            {
                btn.BackColor = Color.FromArgb(0, 123, 255);
                btn.ForeColor = Color.White;
            }
        }

        private void Button_MouseLeave(object? sender, EventArgs e)
        {
            if (sender is Button btn && btn.Enabled)
            {
                btn.BackColor = Color.FromArgb(58, 58, 58);
                btn.ForeColor = Color.FromArgb(200, 200, 200);
            }
        }

        private void DisableAllButtons()
        {
            autoDetectButton.Enabled = false;
            manualDetectButton.Enabled = false;
            installButton.Enabled = false;
            disableButton.Enabled = false;
            updatePatcherButton.Enabled = false;
            miscellaneousButton.Enabled = false;
        }

        private void EnableDetectionButtonsOnly()
        {
            autoDetectButton.Enabled = true;
            manualDetectButton.Enabled = true;
            installButton.Enabled = false;
            disableButton.Enabled = false;
            updatePatcherButton.Enabled = false;
            miscellaneousButton.Enabled = false;
        }

        private void EnableAllButtons()
        {
            autoDetectButton.Enabled = true;
            manualDetectButton.Enabled = true;
            installButton.Enabled = targetPath != null;
            disableButton.Enabled = targetPath != null;
            updatePatcherButton.Enabled = targetPath != null && IsRequiredModFilePresent();
            miscellaneousButton.Enabled = targetPath != null;
        }

        private bool IsRequiredModFilePresent()
        {
            if (string.IsNullOrEmpty(targetPath))
            {
                return false;
            }

            string requiredFilePath = Path.Combine(targetPath, RequiredModFilePath);
            bool fileExists = File.Exists(requiredFilePath);

            if (!fileExists)
            {
                _logger.Log($"Required mod file '{RequiredModFilePath}' not found. Please install mods first.");
            }

            return fileExists;
        }

        private async Task CheckModsStatus()
        {
            await _status.CheckModsStatusAsync(targetPath, statusModsTextLabel, statusModsDotLabel);
        }

        private async void AutoDetectButton_Click(object? sender, EventArgs e)
        {
            DisableAllButtons();
            progressBar.Value = 0;

            string? detectedPath = await _detection.AutoDetectAsync();
            if (!string.IsNullOrEmpty(detectedPath))
            {
                targetPath = detectedPath;

                // SAVE the chosen path
                _configService.SetLastTargetPath(targetPath);

                await CheckModsStatus();
                EnableAllButtons();
                return;
            }
        }

        private async void ManualDetectButton_Click(object? sender, EventArgs e)
        {
            DisableAllButtons();

            string? selectedPath = _detection.ManualDetect();
            if (!string.IsNullOrEmpty(selectedPath))
            {
                targetPath = selectedPath;

                // SAVE the chosen path
                _configService.SetLastTargetPath(targetPath);

                _logger.Log($"Dota 2 folder manually selected: {targetPath}");
                await CheckModsStatus();
                EnableAllButtons();
            }
            else
            {
                _logger.Log("Manual detection canceled or invalid path.");
                EnableDetectionButtonsOnly();
            }
        }

        private async void InstallButton_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(targetPath))
            {
                return;
            }

            DisableAllButtons();
            progressBar.Value = 0;

            string appPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule!.FileName)!;

            bool success = await _modInstaller.InstallModsAsync(targetPath, appPath);

            EnableAllButtons();
        }

        private async void DisableButton_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(targetPath))
            {
                _logger.Log("No Dota 2 folder selected.");
                return;
            }

            DisableAllButtons();
            progressBar.Value = 0;

            bool success = await _modInstaller.DisableModsAsync(targetPath);
            await _status.CheckModsStatusAsync(targetPath, statusModsTextLabel, statusModsDotLabel);

            EnableAllButtons();
        }

        private async void UpdatePatcherButton_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(targetPath))
            {
                return;
            }

            if (!_modInstaller.IsRequiredModFilePresent(targetPath))
            {
                return;
            }

            DisableAllButtons();
            progressBar.Value = 0;

            bool success = await _modInstaller.UpdatePatcherAsync(targetPath);
            await _status.CheckModsStatusAsync(targetPath, statusModsTextLabel, statusModsDotLabel);

            EnableAllButtons();
        }
        private async Task UpdateVersionLabelAsync()
        {
            try
            {
                await Task.Delay(100); // Small async buffer

                string versionText = _updater?.CurrentVersion ?? "Unknown";

                if (versionLabel.InvokeRequired)
                {
                    versionLabel.Invoke(new Action(() =>
                        versionLabel.Text = $"Version: {versionText}"));
                }
                else
                {
                    versionLabel.Text = $"Version: {versionText}";
                }
            }
            catch (Exception ex)
            {
                _logger.Log($"Failed to update version label: {ex.Message}");
            }
        }

        private void label1_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void button1_Click(object sender, EventArgs e) { }
        private void progressBar_Click(object sender, EventArgs e) { }

        private void mainConsoleBox_TextChanged(object sender, EventArgs e)
        {

        }
    }

}
