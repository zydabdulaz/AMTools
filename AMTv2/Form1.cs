using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ArdysaModsTools
{
    public partial class Form1 : Form
    {
        private string? targetPath = null;
        private static readonly HttpClient httpClient = new HttpClient(new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        });
        private const string DotaSignaturesUrl = "https://drive.google.com/uc?export=download&id=1D5k_L8gpCxOY83H3z9GD2i3fndn95PnL";
        private const string GameInfoUrl = "https://drive.google.com/uc?export=download&id=1Avc6S5oiGzPNCtK0psKdftVHZb2cznZN";
        private const string RequiredModFilePath = "game/_ArdysaMods/pak01_dir.vpk";
        private const string CurrentVersion = "1.9.7.3";
        private const string GitHubApiUrl = "https://api.github.com/repos/Anneardysa/ArdysaModsTools/releases/latest";
        private readonly List<string> _logBuffer = new List<string>(); // Buffer for early logs

        public Form1()
        {
            InitializeComponent();
            Log("Ensure the 'game' folder is in the same directory as ArdysaModsTools.exe for installation to work!");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
            httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
            httpClient.Timeout = TimeSpan.FromMinutes(10);
            ServicePointManager.DefaultConnectionLimit = 10;

            try
            {
                using Stream? iconStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ArdysaModsTools.AppIcon.ico");
                if (iconStream == null)
                {
                    Log("Embedded icon 'AppIcon.ico' not found.");
                }
                else
                {
                    this.Icon = new Icon(iconStream);
                }
            }
            catch (Exception ex)
            {
                Log($"Error loading application icon: {ex.Message}");
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
                        Log($"Update cleanup failed: {ex.Message}");
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
            CheckForUpdatesOnStartup();
        }

        private void Form1_Load(object? sender, EventArgs e)
        {
            try
            {
                // Flush any buffered logs
                foreach (var message in _logBuffer)
                {
                    LogInternal(message);
                }
                _logBuffer.Clear();

                // Load Discord icon (dc.png)
                using (Stream? discordStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ArdysaModsTools.dc.png"))
                {
                    if (discordStream == null)
                    {
                        Log("Discord icon 'dc.png' not found in resources.");
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
                        Log("YouTube icon 'yt.png' not found in resources.");
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
                        Log("PayPal icon 'paypal.png' not found in resources.");
                    }
                    else
                    {
                        paypalPictureBox.Image = Image.FromStream(paypalStream);
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Error loading social media icons: {ex.Message}");
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
                Log($"Failed to open Discord link: {ex.Message}");
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
                Log($"Failed to open YouTube link: {ex.Message}");
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
                Log($"Failed to open PayPal link: {ex.Message}");
                MessageBox.Show($"Failed to open PayPal link: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MiscellaneousButton_Click(object? sender, EventArgs e)
        {
            using (var miscForm = new MiscellaneousForm(targetPath, Log, consoleLog, DisableAllButtons, EnableAllButtons))
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
                Log($"Required mod file '{RequiredModFilePath}' not found. Please install mods first.");
            }

            return fileExists;
        }

        private async void CheckForUpdatesOnStartup()
        {
            await CheckForUpdates();
        }

        private async Task CheckForUpdates()
        {
            const int maxRetries = 3;
            const int baseDelayMs = 1000;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    Log("Checking for updates...");
                    using HttpResponseMessage response = await httpClient.GetAsync(GitHubApiUrl);
                    if (!response.IsSuccessStatusCode)
                    {
                        if (response.StatusCode == HttpStatusCode.GatewayTimeout || response.StatusCode == HttpStatusCode.ServiceUnavailable)
                        {
                            if (attempt == maxRetries)
                            {
                                Log("Failed to check for updates after multiple attempts.");
                                return;
                            }
                            int delay = baseDelayMs * (int)Math.Pow(2, attempt - 1);
                            Log($"GitHub API returned {response.StatusCode}. Retrying in {delay / 1000} seconds...");
                            await Task.Delay(delay);
                            continue;
                        }
                        Log($"Failed to check for updates: {response.StatusCode}");
                        return;
                    }

                    string rawResponse = await response.Content.ReadAsStringAsync();
                    using var document = JsonDocument.Parse(rawResponse);
                    var root = document.RootElement;

                    string latestVersion = root.TryGetProperty("tag_name", out JsonElement tagNameElement) ? tagNameElement.GetString() ?? "0.0.0" : "0.0.0";
                    string downloadUrl = root.TryGetProperty("assets", out JsonElement assetsElement) && assetsElement[0].TryGetProperty("browser_download_url", out JsonElement urlElement) ? urlElement.GetString() ?? "" : "";

                    if (string.IsNullOrEmpty(downloadUrl))
                    {
                        Log("No valid download URL found in the latest release.");
                        return;
                    }

                    if (IsNewerVersion(latestVersion, CurrentVersion))
                    {
                        Log($"New version {latestVersion} available (current: {CurrentVersion}).");
                        if (MessageBox.Show($"A new version ({latestVersion}) is available. Would you like to update?", "Update Available", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                        {
                            await DownloadAndUpdate(downloadUrl);
                        }
                        else
                        {
                            Log("Update declined by user.");
                        }
                    }
                    else
                    {
                        Log($"You are running the latest version ({CurrentVersion}).");
                    }
                    return;
                }
                catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.GatewayTimeout || ex.StatusCode == HttpStatusCode.ServiceUnavailable)
                {
                    if (attempt == maxRetries)
                    {
                        Log("Failed to check for updates after multiple attempts.");
                        return;
                    }
                    int delay = baseDelayMs * (int)Math.Pow(2, attempt - 1);
                    Log($"Failed to check for updates: {ex.Message}. Retrying in {delay / 1000} seconds...");
                    await Task.Delay(delay);
                }
                catch (Exception ex)
                {
                    Log($"Failed to check for updates: {ex.Message}");
                    return;
                }
            }
        }

        private bool IsNewerVersion(string latest, string current)
        {
            var latestParts = latest.TrimStart('v').Split('.');
            var currentParts = current.Split('.');
            for (int i = 0; i < Math.Max(latestParts.Length, currentParts.Length); i++)
            {
                int latestNum = i < latestParts.Length ? int.Parse(latestParts[i]) : 0;
                int currentNum = i < currentParts.Length ? int.Parse(currentParts[i]) : 0;
                if (latestNum > currentNum) return true;
                if (latestNum < currentNum) return false;
            }
            return false;
        }

        private async Task DownloadAndUpdate(string downloadUrl)
        {
            string? tempArchive = null;
            string? tempDir = null;
            string? scriptPath = null;
            const int totalSteps = 6;

            try
            {
                DisableAllButtons();
                using var progressForm = new UpdateProgressForm();
                progressForm.Show(this);

                Log("Downloading update...");
                progressForm.SetStatus("Downloading update...");
                progressForm.UpdateProgress(0);
                var request = new HttpRequestMessage(HttpMethod.Get, downloadUrl);
                using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                long? contentLength = response.Content.Headers.ContentLength;
                using var stream = await response.Content.ReadAsStreamAsync();

                ProcessModule? mainModule = Process.GetCurrentProcess().MainModule;
                if (mainModule == null)
                {
                    throw new InvalidOperationException("Unable to determine the current executable path.");
                }
                string currentExe = mainModule.FileName;
                string? exeDir = Path.GetDirectoryName(currentExe);
                if (exeDir == null)
                {
                    throw new InvalidOperationException("Unable to determine the directory of the current executable.");
                }

                tempDir = Path.Combine(exeDir, "_temp");
                if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
                Directory.CreateDirectory(tempDir);

                DirectoryInfo dirInfo = new DirectoryInfo(tempDir);
                dirInfo.Attributes |= FileAttributes.Hidden;

                tempArchive = Path.Combine(tempDir, Path.GetFileName(downloadUrl));
                using (var fileStream = new FileStream(tempArchive, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    byte[] buffer = new byte[8192];
                    int bytesRead;
                    long totalBytesRead = 0;

                    while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await fileStream.WriteAsync(buffer, 0, bytesRead);
                        totalBytesRead += bytesRead;

                        if (contentLength.HasValue)
                        {
                            int progressPercentage = (int)((totalBytesRead * 100) / contentLength.Value) / totalSteps;
                            progressForm.UpdateProgress(progressPercentage);
                        }
                    }
                }

                Log("Download complete. Extracting update...");
                progressForm.SetStatus("Extracting update...");
                progressForm.UpdateProgress(33);
                ZipFile.ExtractToDirectory(tempArchive, tempDir);

                string[] exeFiles = Directory.GetFiles(tempDir, "*.exe", SearchOption.AllDirectories);
                if (exeFiles.Length == 0)
                {
                    throw new FileNotFoundException("No executable file found in the update archive.");
                }
                string extractedExe = exeFiles[0];

                string newExe = Path.Combine(exeDir, "ArdysaModsTools.exe");

                Log("Preparing update...");
                progressForm.SetStatus("Preparing update...");
                progressForm.UpdateProgress(50);
                if (Path.GetFileName(extractedExe) != "ArdysaModsTools.exe")
                {
                    File.Move(extractedExe, Path.Combine(tempDir, "ArdysaModsTools.exe"), true);
                    extractedExe = Path.Combine(tempDir, "ArdysaModsTools.exe");
                }

                scriptPath = Path.Combine(exeDir, "update.bat");
                using (StreamWriter sw = new StreamWriter(scriptPath))
                {
                    sw.WriteLine("@echo off");
                    sw.WriteLine("timeout /t 1 /nobreak >nul 2>&1");
                    sw.WriteLine("del \"" + newExe + "\" >nul 2>&1");
                    sw.WriteLine("move \"" + extractedExe + "\" \"" + newExe + "\" >nul 2>&1");
                    sw.WriteLine("start \"\" \"" + newExe + "\" >nul 2>&1");
                    sw.WriteLine("rmdir /s /q \"" + tempDir + "\" >nul 2>&1");
                    sw.WriteLine("del \"%~f0\" >nul 2>&1");
                }

                Log("Preparing to apply update...");
                progressForm.SetStatus("Preparing to apply update...");
                progressForm.UpdateProgress(66);
                await Task.Delay(500);

                Log("Download finished. Closing old version...");
                progressForm.SetStatus("Closing old version...");
                progressForm.UpdateProgress(83);

                Log("Executing update script...");
                progressForm.SetStatus("Applying update...");
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = scriptPath,
                    UseShellExecute = true,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };
                Process.Start(psi);

                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Log($"Update failed: {ex.Message}");
                MessageBox.Show($"Failed to update: {ex.Message}", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                EnableAllButtons();
            }
        }

        private async Task CheckModsStatus()
        {
            if (string.IsNullOrEmpty(targetPath))
            {
                BeginInvoke((Action)(() =>
                {
                    statusModsDotLabel.BackColor = Color.FromArgb(150, 150, 150);
                    statusModsTextLabel.Text = "Not Checked";
                    statusModsTextLabel.ForeColor = Color.FromArgb(150, 150, 150);
                }));
                return;
            }

            try
            {
                Log("Checking mods status...");
                string ardysaModsPath = Path.Combine(targetPath, "game", "_ArdysaMods", "_temp");
                string signaturesPath = Path.Combine(targetPath, "game", "bin", "win64", "dota.signatures");
                string coreJsonPath = Path.Combine(ardysaModsPath, "core.json");

                if (!File.Exists(signaturesPath))
                {
                    BeginInvoke((Action)(() =>
                    {
                        statusModsDotLabel.BackColor = Color.FromArgb(255, 50, 50);
                        statusModsTextLabel.Text = "Error";
                        statusModsTextLabel.ForeColor = Color.FromArgb(255, 50, 50);
                    }));
                    return;
                }

                string signaturesContent = await File.ReadAllTextAsync(signaturesPath);
                Match digestMatch = Regex.Match(signaturesContent, @"DIGEST:([A-F0-9]+)");
                if (!digestMatch.Success)
                {
                    Log("ERROR : 2001.");
                    BeginInvoke((Action)(() =>
                    {
                        statusModsDotLabel.BackColor = Color.FromArgb(255, 50, 50);
                        statusModsTextLabel.Text = "Error";
                        statusModsTextLabel.ForeColor = Color.FromArgb(255, 50, 50);
                    }));
                    return;
                }

                string newDigest = digestMatch.Groups[1].Value;

                string existingJson = File.Exists(coreJsonPath) ? await File.ReadAllTextAsync(coreJsonPath) : "{}";
                var jsonObject = JsonSerializer.Deserialize<Dictionary<string, string>>(existingJson) ?? new Dictionary<string, string>();
                string? existingDigest = jsonObject.ContainsKey("digest") ? jsonObject["digest"] : null;

                jsonObject["digest"] = newDigest;
                string jsonContent = JsonSerializer.Serialize(jsonObject, new JsonSerializerOptions { WriteIndented = true });
                Directory.CreateDirectory(ardysaModsPath);
                await File.WriteAllTextAsync(coreJsonPath, jsonContent);

                BeginInvoke((Action)(() =>
                {
                    if (existingDigest == newDigest)
                    {
                        Log("Mods status: Ready.");
                        statusModsDotLabel.BackColor = Color.FromArgb(0, 200, 0);
                        statusModsTextLabel.Text = "Ready";
                        statusModsTextLabel.ForeColor = Color.FromArgb(0, 200, 0);
                    }
                    else
                    {
                        Log("Mods status: Need Update.");
                        statusModsDotLabel.BackColor = Color.FromArgb(255, 165, 0);
                        statusModsTextLabel.Text = "Need Update";
                        statusModsTextLabel.ForeColor = Color.FromArgb(255, 165, 0);
                    }
                }));
            }
            catch (Exception ex)
            {
                Log($"Failed to check mods status: {ex.Message}");
                BeginInvoke((Action)(() =>
                {
                    statusModsDotLabel.BackColor = Color.FromArgb(255, 50, 50);
                    statusModsTextLabel.Text = "Error";
                    statusModsTextLabel.ForeColor = Color.FromArgb(255, 50, 50);
                }));
            }
        }

        private bool AreByteArraysEqual(byte[] array1, byte[] array2)
        {
            if (array1.Length != array2.Length)
                return false;

            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                    return false;
            }
            return true;
        }

        private async void AutoDetectButton_Click(object? sender, EventArgs e)
        {
            DisableAllButtons();
            Log("Auto-detecting Dota 2 folder...");
            progressBar.Value = 0;

            for (int i = 0; i <= 100; i += 25)
            {
                progressBar.Value = i;
                await Task.Delay(400);
                this.Refresh();
            }

            string? steamReg = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Valve\Steam", "SteamExe", null) as string;
            if (!string.IsNullOrEmpty(steamReg))
            {
                string steamBase = Path.GetDirectoryName(steamReg)!;
                string vdfPath = Path.Combine(steamBase, "steamapps", "LibraryFolders.vdf");
                if (File.Exists(vdfPath))
                {
                    string vdfContent = File.ReadAllText(vdfPath);
                    MatchCollection libraryPaths = Regex.Matches(vdfContent, @"""path""\s+""([^""]+)""");
                    foreach (Match match in libraryPaths.Cast<Match>())
                    {
                        string path = match.Groups[1].Value;
                        string dotaPath = Path.Combine(path, "steamapps", "common", "dota 2 beta", "game", "bin", "win64", "dota2.exe");
                        if (File.Exists(dotaPath))
                        {
                            targetPath = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(dotaPath))))!;
                            Log($"Dota 2 folder detected: {targetPath}");
                            await CheckModsStatus();
                            EnableAllButtons();
                            return;
                        }
                    }
                    Log("Dota 2 not found in any Steam library folders.");
                }
                else
                {
                    Log($"LibraryFolders.vdf not found at: {vdfPath}");
                }
            }
            else
            {
                Log("Steam registry key not found.");
            }

            try
            {
                string? regOutput = RunRegQuery(@"HKEY_CLASSES_ROOT\dota2\Shell\Open\Command", "/ve");
                if (!string.IsNullOrEmpty(regOutput))
                {
                    Match regMatch = Regex.Match(regOutput, @"REG_SZ\s+(.+)", RegexOptions.IgnoreCase);
                    if (regMatch.Success)
                    {
                        string regValue = regMatch.Groups[1].Value.Trim();
                        Match exeMatch = Regex.Match(regValue, @"^""([^""]+dota2\.exe)""");
                        if (exeMatch.Success)
                        {
                            string dotaExe = exeMatch.Groups[1].Value;
                            if (File.Exists(dotaExe))
                            {
                                targetPath = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(dotaExe))))!;
                                if (targetPath.EndsWith("game", StringComparison.OrdinalIgnoreCase))
                                    targetPath = Path.GetDirectoryName(targetPath)!;
                                else if (targetPath.EndsWith("bin", StringComparison.OrdinalIgnoreCase))
                                    targetPath = Path.GetDirectoryName(Path.GetDirectoryName(targetPath))!;
                                Log($"Dota 2 folder detected: {targetPath}, Method: HKEY_CLASSES_ROOT");
                                await CheckModsStatus();
                                EnableAllButtons();
                            }
                            else
                            {
                                Log($"dota2.exe not found in registry path: {dotaExe}");
                                EnableAllButtons();
                            }
                        }
                        else
                        {
                            Log("Could not parse registry value from HKEY_CLASSES_ROOT.");
                            EnableAllButtons();
                        }
                    }
                }
                else
                {
                    string? steamUninstallPath = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 570", "InstallLocation", null) as string;
                    if (string.IsNullOrEmpty(steamUninstallPath) || !File.Exists(Path.Combine(steamUninstallPath, "game", "bin", "win64", "dota2.exe")))
                    {
                        steamUninstallPath = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 570", "InstallLocation", null) as string;
                    }
                    if (!string.IsNullOrEmpty(steamUninstallPath) && File.Exists(Path.Combine(steamUninstallPath, "game", "bin", "win64", "dota2.exe")))
                    {
                        targetPath = steamUninstallPath;
                        Log($"Dota 2 folder detected: {targetPath}, Method: HKLM Uninstall");
                        await CheckModsStatus();
                        EnableAllButtons();
                    }
                    else if (!string.IsNullOrEmpty(steamReg))
                    {
                        string steamBase = Path.GetDirectoryName(steamReg)!;
                        string defaultDotaPath = Path.Combine(steamBase, "steamapps", "common", "dota 2 beta");
                        if (File.Exists(Path.Combine(defaultDotaPath, "game", "bin", "win64", "dota2.exe")))
                        {
                            targetPath = defaultDotaPath;
                            Log($"Dota 2 folder detected: {defaultDotaPath}, Method: SteamExe default path");
                            await CheckModsStatus();
                            EnableAllButtons();
                        }
                        else
                        {
                            Log("No Dota 2 folder found across all methods.");
                            EnableAllButtons();
                        }
                    }
                    else
                    {
                        Log("Steam registry not found and no library paths detected.");
                        EnableAllButtons();
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Auto-detection failed: {ex.Message}");
                EnableAllButtons();
            }
        }

        private string? RunRegQuery(string key, string args)
        {
            using Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "reg",
                    Arguments = $"query \"{key}\" {args}",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return process.ExitCode == 0 ? output : null;
        }

        private async void ManualDetectButton_Click(object? sender, EventArgs e)
        {
            DisableAllButtons();
            Log("Opening folder browser for manual detection...");
            while (true)
            {
                using FolderBrowserDialog folderBrowser = new()
                {
                    Description = "Select Dota 2 Folder (e.g., C:\\Program Files (x86)\\Steam\\steamapps\\common\\dota 2 beta)"
                };
                if (folderBrowser.ShowDialog() == DialogResult.OK)
                {
                    string selectedPath = folderBrowser.SelectedPath;
                    string folderName = Path.GetFileName(selectedPath);

                    if (string.Equals(folderName, "dota 2 beta", StringComparison.OrdinalIgnoreCase) && Directory.Exists(selectedPath))
                    {
                        targetPath = selectedPath;
                        Log($"Dota 2 folder manually selected: {targetPath}");
                        await CheckModsStatus();
                        EnableAllButtons();
                        break;
                    }
                    else
                    {
                        Log("Invalid folder selected: Not 'dota 2 beta'.");
                        MessageBox.Show("Please select 'dota 2 beta' Folder", "Invalid Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    Log("Manual detection canceled.");
                    EnableAllButtons();
                    break;
                }
            }
        }

        private bool ExtractAndValidateVPK(string sourceGamePath, string appPath)
        {
            string vpkPath = Path.Combine(sourceGamePath, "_ArdysaMods", "pak01_dir.vpk");
            string extractDir = Path.Combine(sourceGamePath, "_ArdysaMods", "pak01_dir");
            string hlExtractPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HLExtract.exe");
            string validationFile = Path.Combine(extractDir, "root", "version", "_ArdysaMods");
            

            if (!File.Exists(vpkPath))
            {
                Log($"Error: 'pak01_dir.vpk' not found at '{vpkPath}'.");
                MessageBox.Show($"The 'pak01_dir.vpk' file is missing at '{vpkPath}'. Please ensure its included in the '_ArdysaMods' folder.", "Installation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!File.Exists(hlExtractPath))
            {
                MessageBox.Show($"ERROR : 3001", "Installation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            try
            {
                if (Directory.Exists(extractDir))
                {
                    Directory.Delete(extractDir, true);
                }
                Directory.CreateDirectory(extractDir);

                ProcessStartInfo hlExtractPsi = new ProcessStartInfo
                {
                    FileName = hlExtractPath,
                    Arguments = $"-p \"{vpkPath}\" -d \"{extractDir}\" -e \"root\"",
                    WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };
                Log("Please wait...");

                using (Process hlExtractProcess = new Process { StartInfo = hlExtractPsi })
                {
                    hlExtractProcess.Start();
                    string output = hlExtractProcess.StandardOutput.ReadToEnd();
                    string error = hlExtractProcess.StandardError.ReadToEnd();
                    hlExtractProcess.WaitForExit();

                    if (!string.IsNullOrEmpty(error))
                    {
                    }
                    if (hlExtractProcess.ExitCode != 0)
                    {
                        MessageBox.Show($"Failed to extract core files", "Extraction Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }

                if (!File.Exists(validationFile))
                {
                    Log($"Critical Error: ArdysaMods Required! Please re-download Mods.");
                    MessageBox.Show($"The required file is missing.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                MessageBox.Show($"An error occurred during process", "Extraction Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private async void InstallButton_Click(object? sender, EventArgs e)
        {
            DisableAllButtons();
            Log("Applying mods...");
            progressBar.Value = 0;
            Log("Starting mod installation...");

            BeginInvoke((Action)(() =>
            {
                progressBar.Value = 33;
                this.Refresh();
            }));

            string? exePath = Process.GetCurrentProcess().MainModule?.FileName;
            if (string.IsNullOrEmpty(exePath))
            {
                Log("Error: Unable to determine the application path.");
                MessageBox.Show("Unable to determine the application path.", "Installation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                targetPath = null;
                EnableDetectionButtonsOnly();
                return;
            }

            string appPath = Path.GetDirectoryName(exePath)!;
            string sourceGamePath = Path.Combine(appPath, "game");
            string copyDestination = Path.Combine(targetPath!, "game");
            string validationFile = Path.Combine(sourceGamePath, "_ArdysaMods", "pak01_dir", "root", "version", "_ArdysaMods");
            string extractDir = Path.Combine(sourceGamePath, "_ArdysaMods", "pak01_dir");

            try
            {
                if (!Directory.Exists(sourceGamePath))
                {
                    Log($"Error: 'game' folder not found at '{sourceGamePath}'.");
                    MessageBox.Show($"The 'game' folder is missing at '{sourceGamePath}'. Please ensure its included with ArdysaModsTools.exe.", "Installation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    targetPath = null;
                    EnableDetectionButtonsOnly();
                    return;
                }

                bool extractionSuccess = ExtractAndValidateVPK(sourceGamePath, appPath);
                if (!extractionSuccess)
                {
                    // Clean up extracted folder if extraction fails
                    if (Directory.Exists(extractDir))
                    {
                        Directory.Delete(extractDir, true);
                    }

                    targetPath = null;
                    EnableDetectionButtonsOnly();
                    return;
                }

                // Double-check the validation file exists before proceeding to DirectoryCopy
                if (!File.Exists(validationFile))
                {
                    Log($"Critical Error: ArdysaMods Required! Please re-download Mods.");
                    MessageBox.Show($"The required file is missing. Installation cannot proceed.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    targetPath = null;
                    EnableDetectionButtonsOnly();
                    return;
                }

                // Delete the extracted pak01_dir folder after validation
                if (Directory.Exists(extractDir))
                {
                    Directory.Delete(extractDir, true);
                }

                DirectoryCopy(sourceGamePath, copyDestination, true);
            }
            catch (DirectoryNotFoundException ex)
            {
                Log($"Installation canceled: {ex.Message}");
                targetPath = null;
                EnableDetectionButtonsOnly();
                return;
            }
            catch (Exception ex)
            {
                Log($"Failed to copy some mod files: {ex.Message}");
                await CheckModsStatus();
                EnableAllButtons();
                return;
            }

            BeginInvoke((Action)(() =>
            {
                progressBar.Value = 66;
                this.Refresh();
            }));

            string dotaSignaturesPath = Path.Combine(targetPath!, "game", "bin", "win64", "dota.signatures");
            string gameInfoPath = Path.Combine(targetPath!, "game", "dota", "gameinfo_branchspecific.gi");

            Directory.CreateDirectory(Path.GetDirectoryName(dotaSignaturesPath)!);
            Directory.CreateDirectory(Path.GetDirectoryName(gameInfoPath)!);

            byte[] gameInfoBytes = await httpClient.GetByteArrayAsync(GameInfoUrl);
            await File.WriteAllBytesAsync(gameInfoPath, gameInfoBytes);

            try
            {
                if (!File.Exists(dotaSignaturesPath))
                {
                    EnableAllButtons();
                    return;
                }

                string[] lines = await File.ReadAllLinesAsync(dotaSignaturesPath);
                bool digestFound = false;
                int digestIndex = -1;

                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].StartsWith("DIGEST:"))
                    {
                        digestFound = true;
                        digestIndex = i;
                        break;
                    }
                }

                if (!digestFound)
                {
                    Log("ERROR : 1001.");
                    EnableAllButtons();
                    return;
                }

                List<string> modifiedLines = new List<string>();
                for (int i = 0; i <= digestIndex; i++)
                {
                    modifiedLines.Add(lines[i]);
                }
                modifiedLines.Add(@"...\..\..\dota\gameinfo_branchspecific.gi~SHA1:1A9B91FB43FE89AD104B8001282D292EED94584D;CRC:043F604A");

                await File.WriteAllLinesAsync(dotaSignaturesPath, modifiedLines);

                Log("Mod installation completed.");
                BeginInvoke((Action)(() =>
                {
                    progressBar.Value = 100;
                    this.Refresh();
                }));

                Task youtubeTask = Task.Run(async () =>
                {
                    try
                    {
                        string apiKey = "AIzaSyBNXV02QWOkeKgePbjjW5o0XyRifrjEDgg";
                        string channelId = "UCUk3IQpTv5nscVuxfKtGbwQ";

                        using (var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                        {
                            ApiKey = apiKey,
                            ApplicationName = "ArdysaModsTools"
                        }))
                        {
                            var playlistItemsRequest = youtubeService.PlaylistItems.List("snippet");
                            playlistItemsRequest.PlaylistId = await GetUploadsPlaylistIdAsync(channelId, youtubeService);
                            playlistItemsRequest.MaxResults = 1;

                            var playlistItemsResponse = await playlistItemsRequest.ExecuteAsync();
                            if (playlistItemsResponse.Items.Count > 0)
                            {
                                string latestVideoId = playlistItemsResponse.Items[0].Snippet.ResourceId.VideoId;
                                string latestVideoUrl = $"https://www.youtube.com/watch?v={latestVideoId}";
                                Process.Start(new ProcessStartInfo
                                {
                                    FileName = latestVideoUrl,
                                    UseShellExecute = true
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("Quota exceed"))
                        {
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = "https://www.youtube.com/@Ardysa",
                                UseShellExecute = true
                            });
                        }
                    }
                });

                BeginInvoke((Action)(() =>
                {
                    using (var popup = new SuccessPopup())
                    {
                        popup.ShowDialog(this);
                    }
                }));

                EnableAllButtons();
                await youtubeTask;
            }
            catch (HttpRequestException httpEx)
            {
                Log($"Download failed: {httpEx.Message}");
            }
            catch (Exception ex)
            {
                Log($"Installation failed: {ex.Message}");
            }
            finally
            {
                await CheckModsStatus();
                EnableAllButtons();
            }
        }

        private async Task<string> GetUploadsPlaylistIdAsync(string channelId, YouTubeService youtubeService)
        {
            var channelsRequest = youtubeService.Channels.List("contentDetails");
            channelsRequest.Id = channelId;
            var channelsResponse = await channelsRequest.ExecuteAsync();
            if (channelsResponse.Items.Count > 0)
            {
                return channelsResponse.Items[0].ContentDetails.RelatedPlaylists.Uploads;
            }
            throw new Exception("Channel not found or no uploads playlist available.");
        }

        private async void DisableButton_Click(object? sender, EventArgs e)
        {
            DisableAllButtons();
            Log("Disabling mods...");
            progressBar.Value = 0;
            Log("Starting mod disable process...");

            for (int i = 0; i <= 100; i += 25)
            {
                progressBar.Value = i;
                await Task.Delay(500);
                this.Refresh();
            }

            string signaturesPath = Path.Combine(targetPath!, "game", "bin", "win64", "dota.signatures");
            string gameInfoPath = Path.Combine(targetPath!, "game", "dota", "gameinfo_branchspecific.gi");

            Directory.CreateDirectory(Path.GetDirectoryName(signaturesPath)!);
            Directory.CreateDirectory(Path.GetDirectoryName(gameInfoPath)!);

            try
            {
                if (!File.Exists(signaturesPath))
                {
                    EnableAllButtons();
                    return;
                }

                string[] lines = await File.ReadAllLinesAsync(signaturesPath);
                int digestIndex = -1;

                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].StartsWith("DIGEST:"))
                    {
                        digestIndex = i;
                        break;
                    }
                }

                if (digestIndex >= 0)
                {
                    List<string> modifiedLines = new List<string>(lines.Take(digestIndex + 1));
                    await File.WriteAllLinesAsync(signaturesPath, modifiedLines);
                }
                else
                {
                    Log("ERROR : 1002");
                }

                byte[] file2 = await httpClient.GetByteArrayAsync("https://drive.google.com/uc?export=download&id=10SkUNsjVRkqGJBuRgx8I4Ufa44NlpNxD");
                await File.WriteAllBytesAsync(gameInfoPath, file2);

                Log("Mod disabling completed.");
            }
            catch (HttpRequestException httpEx)
            {
                Log($"Download failed: {httpEx.Message}");
            }
            catch (Exception ex)
            {
                Log($"Disabling failed: {ex.Message}");
            }
            finally
            {
                await CheckModsStatus();
                EnableAllButtons();
            }
        }

        private async void UpdatePatcherButton_Click(object? sender, EventArgs e)
        {
            if (!IsRequiredModFilePresent())
            {
                Log("Cannot update patcher: Required mod file 'game/_ArdysaMods/pak01_dir.vpk' is missing. Please install mods first.");
                EnableAllButtons();
                return;
            }

            DisableAllButtons();
            Log("Updating patcher...");
            progressBar.Value = 0;

            for (int i = 0; i <= 100; i += 25)
            {
                progressBar.Value = i;
                await Task.Delay(500);
                this.Refresh();
            }

            try
            {
                string dotaSignaturesPath = Path.Combine(targetPath!, "game", "bin", "win64", "dota.signatures");
                string gameInfoPath = Path.Combine(targetPath!, "game", "dota", "gameinfo_branchspecific.gi");

                if (!File.Exists(dotaSignaturesPath))
                {
                    EnableAllButtons();
                    return;
                }

                string[] lines = await File.ReadAllLinesAsync(dotaSignaturesPath);
                bool digestFound = false;
                int digestIndex = -1;

                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].StartsWith("DIGEST:"))
                    {
                        digestFound = true;
                        digestIndex = i;
                        break;
                    }
                }

                if (!digestFound)
                {
                    Log("ERROR : 1001.");
                    EnableAllButtons();
                    return;
                }

                List<string> modifiedLines = new List<string>(lines.Take(digestIndex + 1));
                modifiedLines.Add(@"...\..\..\dota\gameinfo_branchspecific.gi~SHA1:1A9B91FB43FE89AD104B8001282D292EED94584D;CRC:043F604A");
                await File.WriteAllLinesAsync(dotaSignaturesPath, modifiedLines);

                Directory.CreateDirectory(Path.GetDirectoryName(gameInfoPath)!);
                byte[] file2 = await httpClient.GetByteArrayAsync(GameInfoUrl);
                await File.WriteAllBytesAsync(gameInfoPath, file2);
            }
            catch (Exception ex)
            {
                Log($"Patcher update failed: {ex.Message}");
            }
            finally
            {
                await CheckModsStatus();
                EnableAllButtons();
            }
        }

        private void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new(sourceDirName);
            if (!dir.Exists)
            {
                Log($"Source directory '{sourceDirName}' not found.");
                MessageBox.Show("Error: '_ArdysaMods' not found, please check 'game' folder and try again.", "Missing Folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new DirectoryNotFoundException($"Source directory '{sourceDirName}' not found. Ensure '_ArdysaMods' with 'pak01_dir.vpk' is included.");
            }

            Directory.CreateDirectory(destDirName);

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                try
                {
                    file.CopyTo(tempPath, true);
                }
                catch (Exception)
                {
                    // Silently skip the file
                }
            }

            if (copySubDirs)
            {
                DirectoryInfo[] dirs = dir.GetDirectories();
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
                }
            }
        }

        private void label1_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }

        private void button1_Click(object sender, EventArgs e) { }

        private void progressBar_Click(object sender, EventArgs e) { }
    }

}
