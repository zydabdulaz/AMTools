using ArdysaModsTools.Services;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArdysaModsTools.Core
{
    public class OperationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";

        public static OperationResult Ok(string msg) => new() { Success = true, Message = msg };
        public static OperationResult Fail(string msg) => new() { Success = false, Message = msg };
    }

    public class ModToolManager
    {
        private readonly FileService _fileService;
        private readonly UpdateService _updateService;
        private readonly DetectionService _detectionService;
        private readonly LoggerService _logger;
        private readonly Action<string, Color> _updateStatusUI;
        private readonly ProgressBar _progressBar;

        private string? _targetPath;
        public string? TargetPath => _targetPath;

        private const string RequiredModFilePath = "game/_ArdysaMods/pak01_dir.vpk";
        private const string CurrentVersion = "1.9.7.2";
        private const string GameInfoUrl = "https://drive.google.com/uc?export=download&id=1Avc6S5oiGzPNCtK0psKdftVHZb2cznZN";

        public ModToolManager(
            FileService fileService,
            UpdateService updateService,
            DetectionService detectionService,
            LoggerService logger,
            Action<string, Color> updateStatusUI,
            ProgressBar progressBar)
        {
            _fileService = fileService;
            _updateService = updateService;
            _detectionService = detectionService;
            _logger = logger;
            _updateStatusUI = updateStatusUI;
            _progressBar = progressBar;
        }

        // ----------------
        // Update Handling
        // ----------------
        public async Task CheckForUpdatesOnStartupAsync()
        {
            var updateResult = await _updateService.CheckForUpdatesAsync(CurrentVersion);
            if (!updateResult.Success)
                _logger.Warn(updateResult.Message);
            else
                _logger.Info(updateResult.Message);
        }

        // ----------------
        // Detection
        // ----------------
        public async Task<OperationResult> AutoDetectAsync()
        {
            _logger.Info("Auto-detecting Dota 2 folder...");
            var result = _detectionService.TryAutoDetect();
            if (!result.Success) return result;

            _targetPath = result.Message;
            _logger.Info($"Dota 2 folder detected: {_targetPath}");
            await CheckModsStatusAsync();
            return OperationResult.Ok($"Detected at {_targetPath}");
        }

        public async Task<OperationResult> ManualDetectAsync()
        {
            using FolderBrowserDialog folderBrowser = new()
            {
                Description = "Select Dota 2 Folder (e.g., C:\\Program Files (x86)\\Steam\\steamapps\\common\\dota 2 beta)"
            };
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                string selectedPath = folderBrowser.SelectedPath;
                if (Path.GetFileName(selectedPath).Equals("dota 2 beta", StringComparison.OrdinalIgnoreCase))
                {
                    _targetPath = selectedPath;
                    _logger.Info($"Dota 2 folder manually selected: {_targetPath}");
                    await CheckModsStatusAsync();
                    return OperationResult.Ok($"Detected at {_targetPath}");
                }
                return OperationResult.Fail("Invalid folder (must be 'dota 2 beta').");
            }
            return OperationResult.Fail("Manual detection cancelled.");
        }

        // ----------------
        // Install Mods
        // ----------------
        public async Task<OperationResult> InstallModsAsync()
        {
            if (string.IsNullOrEmpty(_targetPath))
                return OperationResult.Fail("Target path not set. Please detect first.");

            try
            {
                _logger.Info("Applying mods...");
                _progressBar.Value = 0;

                bool ok = await _fileService.ExtractAndValidateVPKAsync(_targetPath, _logger);
                if (!ok) return OperationResult.Fail("Validation failed.");

                await _fileService.CopyModsAsync(_targetPath);

                await _fileService.PatchSignaturesAsync(_targetPath, GameInfoUrl);

                _logger.Info("Mod installation completed.");
                _progressBar.Value = 100;

                return OperationResult.Ok("Installation successful.");
            }
            catch (Exception ex)
            {
                _logger.Error($"Installation failed: {ex.Message}");
                return OperationResult.Fail(ex.Message);
            }
        }

        // ----------------
        // Disable Mods
        // ----------------
        public async Task<OperationResult> DisableModsAsync()
        {
            if (string.IsNullOrEmpty(_targetPath))
                return OperationResult.Fail("Target path not set.");

            try
            {
                await _fileService.RevertSignaturesAsync(_targetPath);
                _logger.Info("Mods disabled.");
                return OperationResult.Ok("Disabled successfully.");
            }
            catch (Exception ex)
            {
                _logger.Error($"Disable failed: {ex.Message}");
                return OperationResult.Fail(ex.Message);
            }
        }

        // ----------------
        // Update Patcher
        // ----------------
        public async Task<OperationResult> UpdatePatcherAsync()
        {
            if (string.IsNullOrEmpty(_targetPath))
                return OperationResult.Fail("Target path not set.");

            try
            {
                await _fileService.PatchSignaturesAsync(_targetPath, GameInfoUrl);
                _logger.Info("Patcher updated.");
                return OperationResult.Ok("Patcher updated.");
            }
            catch (Exception ex)
            {
                _logger.Error($"Update patcher failed: {ex.Message}");
                return OperationResult.Fail(ex.Message);
            }
        }

        // ----------------
        // Mods Status
        // ----------------
        private async Task CheckModsStatusAsync()
        {
            try
            {
                _logger.Info("Checking mods status...");
                bool ready = await _fileService.CheckModsDigestAsync(_targetPath);

                if (ready)
                    _updateStatusUI("Ready", Color.Green);
                else
                    _updateStatusUI("Need Update", Color.Orange);
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to check mods: {ex.Message}");
                _updateStatusUI("Error", Color.Red);
            }
        }

        // ----------------
        // Misc
        // ----------------
        public void LoadSocialIcons(PictureBox discord, PictureBox youtube, PictureBox paypal)
        {
            _fileService.LoadResourceIcon("ArdysaModsTools.dc.png", discord, _logger);
            _fileService.LoadResourceIcon("ArdysaModsTools.yt.png", youtube, _logger);
            _fileService.LoadResourceIcon("ArdysaModsTools.paypal.png", paypal, _logger);
        }

        public void OpenUrl(string url, string name)
        {
            try
            {
                Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to open {name} link: {ex.Message}");
                MessageBox.Show($"Failed to open {name}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
