using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArdysaModsTools.Helpers;
using ArdysaModsTools.Core.Models;

namespace ArdysaModsTools.Core.Services
{
    public class UpdaterService
    {
        public Action<int>? OnProgressChanged { get; set; }

        private readonly HttpClient _httpClient;
        private readonly Logger _logger;

        private const string GitHubApiUrl = "https://api.github.com/repos/Anneardysa/ArdysaModsTools/releases/latest";
        public event Action<string>? OnVersionChanged;
        public string CurrentVersion { get; private set; } = "1.9.7.4";


        public UpdaterService(Logger logger)
        {
            _logger = logger;
            _httpClient = new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            })
            {
                Timeout = TimeSpan.FromMinutes(10)
            };
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
        }

        public async Task CheckForUpdatesAsync()
        {
            try
            {
                _logger.Log("Checking for updates...");

                using var response = await _httpClient.GetAsync(GitHubApiUrl);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.Log($"Failed to check for updates: {response.StatusCode}");
                    return;
                }

                var release = await NetworkHelper.GetLatestReleaseAsync(_httpClient, GitHubApiUrl);
                if (release == null)
                {
                    _logger.Log("Failed to parse latest release info.");
                    return;
                }
                string latestVersion = release.TagName;
                string downloadUrl = release.DownloadUrl;

                if (IsNewerVersion(latestVersion, CurrentVersion))
                {
                    _logger.Log($"New version {latestVersion} available (current: {CurrentVersion}).");

                    // 🔸 Update internal version and notify listeners
                    CurrentVersion = latestVersion;
                    OnVersionChanged?.Invoke(CurrentVersion);

                    var result = MessageBox.Show(
                        $"A new version ({latestVersion}) is available. Would you like to update now?",
                        "Update Available",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information);

                    if (result == DialogResult.Yes)
                        await DownloadAndApplyUpdateAsync(downloadUrl);
                    else
                        _logger.Log("Update skipped by user.");
                }
                else
                {
                    _logger.Log($"You are running the latest version ({CurrentVersion}).");

                    // 🔸 Ensure label updates even if already latest
                    OnVersionChanged?.Invoke(CurrentVersion);
                }
            }
            catch (Exception ex)
            {
                _logger.Log($"Update check failed: {ex.Message}");
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

        private async Task DownloadAndApplyUpdateAsync(string downloadUrl)
        {
            string? tempArchive = null;
            string? extractDir = null;
            string? scriptPath = null;

            try
            {
                _logger.Log("Downloading latest version...");

                string tempRoot = Path.Combine(Path.GetTempPath(), $"ArdysaModsTools_Update_{Guid.NewGuid()}");
                Directory.CreateDirectory(tempRoot);
                tempArchive = Path.Combine(tempRoot, "update.zip");
                extractDir = Path.Combine(tempRoot, "extracted");

                using (var response = await _httpClient.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();

                    await using var netStream = await response.Content.ReadAsStreamAsync();
                    await using var fileStream = new FileStream(tempArchive, FileMode.Create, FileAccess.Write, FileShare.None, 81920, true);

                    long totalRead = 0;
                    long? totalBytes = response.Content.Headers.ContentLength;
                    byte[] buffer = new byte[81920];
                    int bytesRead;
                    int lastProgress = 0;
                    var sw = Stopwatch.StartNew();

                    while ((bytesRead = await netStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await fileStream.WriteAsync(buffer, 0, bytesRead);
                        totalRead += bytesRead;

                        if (totalBytes.HasValue)
                        {
                            int progress = (int)(totalRead * 100L / totalBytes.Value);
                            if (progress > lastProgress)
                            {
                                lastProgress = progress;
                                OnProgressChanged?.Invoke(progress);

                                if (progress % 10 == 0)
                                    _logger.Log($"Downloading update... {progress}%");
                            }
                        }
                    }

                    sw.Stop();
                    double mbps = totalRead / 1024d / 1024d / sw.Elapsed.TotalSeconds;
                }

                ZipFile.ExtractToDirectory(tempArchive, extractDir);

                var exeFiles = Directory.GetFiles(extractDir, "*.exe", SearchOption.AllDirectories);
                if (exeFiles.Length == 0)
                    throw new FileNotFoundException("No executable found in the downloaded package.");

                string newExePath = exeFiles[0];
                string newExeName = Path.GetFileName(newExePath);

                string currentExe = Process.GetCurrentProcess().MainModule!.FileName;
                string currentExeDir = Path.GetDirectoryName(currentExe)!;

                // Create an update workspace in the Windows TEMP folder
                string targetTempDir = Path.Combine(Path.GetTempPath(), $"ArdysaModsTools_Update_{Guid.NewGuid()}");
                Directory.CreateDirectory(targetTempDir);

                // Copy new exe to temp workspace
                string tempExeCopy = Path.Combine(targetTempDir, newExeName);
                File.Copy(newExePath, tempExeCopy, true);

                // Hide the folder for cleanliness
                try { new DirectoryInfo(targetTempDir).Attributes |= FileAttributes.Hidden; } catch { }

                // Build batch script inside the temp directory
                scriptPath = Path.Combine(targetTempDir, "apply_update.bat");

                string batchContent = $@"
@echo off
title ArdysaModsTools Updater
setlocal

set NEW=""{tempExeCopy}""
set TARGET=""{currentExe}""
set TEMPDIR=""{targetTempDir}""

echo Updating application...
:retry
move /Y %NEW% %TARGET% >nul 2>&1
if %ERRORLEVEL%==0 goto done
timeout /t 1 /nobreak >nul
goto retry

:done
echo Launching new version...
start """" ""%TARGET%""

REM Give the new app a moment to start
timeout /t 2 /nobreak >nul

REM Cleanup temporary update folder
rmdir /s /q %TEMPDIR% >nul 2>&1

exit
";

                await File.WriteAllTextAsync(scriptPath, batchContent);

                OnProgressChanged?.Invoke(100);

                // Start the batch script from temp and exit app
                Process.Start(new ProcessStartInfo
                {
                    FileName = scriptPath,
                    UseShellExecute = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                });

                Environment.Exit(0);

            }
            catch (Exception ex)
            {
                _logger.Log($"Update failed: {ex.Message}");
            }
            finally
            {
                try
                {
                    if (!string.IsNullOrEmpty(tempArchive) && File.Exists(tempArchive)) File.Delete(tempArchive);
                    if (!string.IsNullOrEmpty(extractDir) && Directory.Exists(extractDir)) Directory.Delete(extractDir, true);
                }
                catch { }
            }
        }
    }
}
