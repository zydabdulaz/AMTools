using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArdysaModsTools.Helpers;
using System.IO.Compression;
using System.Text.Json;
using System.Threading;

namespace ArdysaModsTools.Core.Services
{
    public class ModInstallerService
    {
        private readonly ILogger _logger;
        private static readonly HttpClient _httpClient = new HttpClient(new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        })
        {
            Timeout = TimeSpan.FromMinutes(10)
        };

        private const string GameInfoUrl = "https://drive.google.com/uc?export=download&id=1Avc6S5oiGzPNCtK0psKdftVHZb2cznZN";
        private const string DisableGameInfoUrl = "https://drive.google.com/uc?export=download&id=10SkUNsjVRkqGJBuRgx8I4Ufa44NlpNxD";
        private const string RequiredModFilePath = "game/_ArdysaMods/pak01_dir.vpk";

        public ModInstallerService(ILogger logger)
        {
            _logger = logger;
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
        }

        // ------------------------------------------------------------
        //  VALIDATION
        // ------------------------------------------------------------
        public bool IsRequiredModFilePresent(string targetPath)
        {
            if (string.IsNullOrEmpty(targetPath))
                return false;

            string requiredFilePath = Path.Combine(targetPath, RequiredModFilePath);
            bool exists = File.Exists(requiredFilePath);

            if (!exists)
                _logger.Log("Required mod file missing — please install mods first.");

            return exists;
        }

        private async Task<string?> DownloadRemoteHashAsync()
        {
            try
            {
                string api = "https://raw.githubusercontent.com/Anneardysa/ModsPack/main/ModsPack.hash";
                return await _httpClient.GetStringAsync(api);
            }
            catch
            {
                _logger.Log("Failed to fetch remote hash.");
                return null;
            }
        }

        private static string ComputeSHA256(string filePath)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            using var stream = File.OpenRead(filePath);
            var hash = sha.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
        

        // ------------------------------------------------------------
        //  INSTALL MODS
        // ------------------------------------------------------------
        public async Task<bool> InstallModsAsync(string targetPath, string appPath, IProgress<int>? progress = null)
        {
            _logger.Log("Installing mods...");

            string tempRoot = Path.Combine(Path.GetTempPath(), $"ArdysaMods_Installer_{Guid.NewGuid()}");
            Directory.CreateDirectory(tempRoot);

            string downloadPath = Path.Combine(tempRoot, "ModsPack.zip");
            string extractPath = Path.Combine(tempRoot, "Extracted");
            Directory.CreateDirectory(extractPath);

            try
            {
                // 0. HASH VALIDATION
                _logger.Log("Checking ModsPack version...");

                string modsDir = Path.Combine(targetPath, "game", "_ArdysaMods");
                Directory.CreateDirectory(modsDir);

                string localHashFile = Path.Combine(modsDir, "ModsPack.hash");

                string? remoteHash = await DownloadRemoteHashAsync();
                if (!string.IsNullOrWhiteSpace(remoteHash) && File.Exists(localHashFile))
                {
                    string localHash = await File.ReadAllTextAsync(localHashFile);

                    if (remoteHash.Trim() == localHash.Trim())
                    {
                        // already installed and same version
                        var dialog = MessageBox.Show(
                            "ModsPack already installed and up to date.\nReinstall anyway?",
                            "Mods Installation",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        if (dialog == DialogResult.No)
                            return true; // skip installation
                    }
                }

                // STEP 1 — Resolve ModsPack URL (or local path)

                var (success, url) = await TryGetModsPackAssetUrlAsync();
                if (!success || string.IsNullOrWhiteSpace(url))
                {
                    _logger.Log("Failed to locate ModsPack from server.");
                    return false;
                }

                // STEP 2 — Download OR copy the file with progress reporting
                _logger.Log("Downloading ModsPack...");

                // If url is a rooted local path and exists, treat as local copy
                if (Path.IsPathRooted(url) && File.Exists(url))
                {
                    // Copy file with progress
                    const int bufferSize = 81920;
                    using (var source = new FileStream(url, FileMode.Open, FileAccess.Read, FileShare.Read))
                    using (var dest = new FileStream(downloadPath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        long total = source.Length;
                        long read = 0;
                        var buffer = new byte[bufferSize];
                        int bytesRead;
                        int lastReported = -1;

                        while ((bytesRead = await source.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await dest.WriteAsync(buffer, 0, bytesRead);
                            read += bytesRead;

                            if (total > 0 && progress != null)
                            {
                                int pct = (int)(read * 100L / total);
                                if (pct != lastReported)
                                {
                                    lastReported = pct;
                                    progress.Report(pct);
                                }
                            }
                        }
                    }
                }
                else
                {
                    // HTTP download with progress
                    using (var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            _logger.Log($"Download failed: {response.StatusCode}");
                            return false;
                        }

                        long? total = response.Content.Headers.ContentLength;
                        using (var netStream = await response.Content.ReadAsStreamAsync())
                        using (var fileStream = new FileStream(downloadPath, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            const int bufferSize = 81920;
                            var buffer = new byte[bufferSize];
                            long totalRead = 0;
                            int bytesRead;
                            int lastReported = -1;

                            while ((bytesRead = await netStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                            {
                                await fileStream.WriteAsync(buffer, 0, bytesRead);
                                totalRead += bytesRead;

                                if (total.HasValue && progress != null)
                                {
                                    int pct = (int)(totalRead * 100L / total.Value);
                                    if (pct != lastReported)
                                    {
                                        lastReported = pct;
                                        progress.Report(pct);
                                    }
                                }
                            }
                        }
                    }
                }

                // Ensure 100% reported after download
                progress?.Report(100);

                // STEP 3 — Extract ModsPack into temporary directory
                try
                {
                    // If the ZIP is bad, ZipFile.ExtractToDirectory throws
                    ZipFile.ExtractToDirectory(downloadPath, extractPath);
                }
                catch (Exception)
                {
                    _logger.Log($"ERROR 08"); // Failed to extract ModsPack: {ex.Message}
                    return false;
                }

                if (!string.IsNullOrWhiteSpace(remoteHash))
                {
                    await File.WriteAllTextAsync(localHashFile, remoteHash.Trim());
                }

                // STEP 4 — Copy extracted contents into game/_ArdysaMods
                Directory.CreateDirectory(modsDir);

                foreach (string dir in Directory.GetDirectories(extractPath, "*", SearchOption.AllDirectories))
                {
                    string rel = Path.GetRelativePath(extractPath, dir);
                    string dest = Path.Combine(modsDir, rel);
                    Directory.CreateDirectory(dest);
                }

                foreach (string file in Directory.GetFiles(extractPath, "*", SearchOption.AllDirectories))
                {
                    string rel = Path.GetRelativePath(extractPath, file);
                    string dest = Path.Combine(modsDir, rel);
                    Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
                    File.Copy(file, dest, true);
                }

                // STEP 5 — Patch dota.signatures and gameinfo_branchspecific.gi (unchanged)

                string signaturesPath = Path.Combine(targetPath, "game", "bin", "win64", "dota.signatures");
                string gameInfoPath = Path.Combine(targetPath, "game", "dota", "gameinfo_branchspecific.gi");

                if (!File.Exists(signaturesPath))
                {
                    _logger.Log("Missing dota.signatures");
                    return false;
                }

                Directory.CreateDirectory(Path.GetDirectoryName(gameInfoPath)!);

                // Download clean gameinfo
                byte[] giData = await _httpClient.GetByteArrayAsync(GameInfoUrl);
                await File.WriteAllBytesAsync(gameInfoPath, giData);

                var lines = await File.ReadAllLinesAsync(signaturesPath);
                int digestIndex = Array.FindIndex(lines, l => l.StartsWith("DIGEST:", StringComparison.Ordinal));
                if (digestIndex < 0)
                {
                    _logger.Log("DIGEST not found inside signatures.");
                    return false;
                }

                var modified = new List<string>(lines[..(digestIndex + 1)])
                    {
                        @"...\..\..\dota\gameinfo_branchspecific.gi~SHA1:1A9B91FB43FE89AD104B8001282D292EED94584D;CRC:043F604A"
                    };

                await File.WriteAllLinesAsync(signaturesPath, modified);

                _logger.Log("Mod installation completed successfully.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.Log($"Installation failed: {ex.Message}");
                return false;
            }
            finally
            {
                try { if (Directory.Exists(tempRoot)) Directory.Delete(tempRoot, true); }
                catch { }
            }
        }

        // ------------------------------------------------------------
        //  DISABLE MODS
        // ------------------------------------------------------------
        public async Task<bool> DisableModsAsync(string targetPath)
        {
            _logger.Log("Disabling mods...");
            string signaturesPath = Path.Combine(targetPath, "game", "bin", "win64", "dota.signatures");
            string gameInfoPath = Path.Combine(targetPath, "game", "dota", "gameinfo_branchspecific.gi");

            if (!File.Exists(signaturesPath))
            {
                return true;
            }

            string[] lines = await File.ReadAllLinesAsync(signaturesPath);
            int digestIndex = Array.FindIndex(lines, l => l.StartsWith("DIGEST:"));
            if (digestIndex >= 0)
            {
                var trimmed = new List<string>(lines[..(digestIndex + 1)]);
                await File.WriteAllLinesAsync(signaturesPath, trimmed);
            }

            // Download clean gameinfo (restore)
            byte[] fileBytes = await _httpClient.GetByteArrayAsync(DisableGameInfoUrl);
            await File.WriteAllBytesAsync(gameInfoPath, fileBytes);

            _logger.Log("Mods disabled successfully.");
            return true;
        }

        // ------------------------------------------------------------
        //  UPDATE PATCHER
        // ------------------------------------------------------------
        public async Task<bool> UpdatePatcherAsync(string targetPath)
        {
            _logger.Log("Updating patcher...");
            string signaturesPath = Path.Combine(targetPath, "game", "bin", "win64", "dota.signatures");
            string gameInfoPath = Path.Combine(targetPath, "game", "dota", "gameinfo_branchspecific.gi");

            if (!File.Exists(signaturesPath))
            {
                _logger.Log("Cannot update patcher: Missing signatures.");
                return false;
            }

            string[] lines = await File.ReadAllLinesAsync(signaturesPath);
            int digestIndex = Array.FindIndex(lines, l => l.StartsWith("DIGEST:"));
            if (digestIndex < 0)
            {
                _logger.Log("DIGEST not found in signatures.");
                return false;
            }

            var modified = new List<string>(lines[..(digestIndex + 1)])
            {
                @"...\..\..\dota\gameinfo_branchspecific.gi~SHA1:1A9B91FB43FE89AD104B8001282D292EED94584D;CRC:043F604A"
            };
            await File.WriteAllLinesAsync(signaturesPath, modified);

            Directory.CreateDirectory(Path.GetDirectoryName(gameInfoPath)!);
            byte[] fileBytes = await NetworkHelper.GetByteArrayAsync(_httpClient, GameInfoUrl);
            await File.WriteAllBytesAsync(gameInfoPath, fileBytes);

            _logger.Log("Update Patch completed.");
            return true;
        }

        // ------------------------------------------------------------
        //  PRIVATE HELPERS
        // ------------------------------------------------------------

        private async Task<(bool Success, string Url)> TryGetModsPackAssetUrlAsync()
        {
            try
            {
                string api = "https://api.github.com/repos/Anneardysa/ModsPack/releases/tags/mods-v1.0";
                var response = await _httpClient.GetAsync(api);
                response.EnsureSuccessStatusCode();

                using var stream = await response.Content.ReadAsStreamAsync();
                using var json = await JsonDocument.ParseAsync(stream);

                foreach (var asset in json.RootElement.GetProperty("assets").EnumerateArray())
                {
                    var name = asset.GetProperty("name").GetString() ?? "";
                    if (name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                    {
                        string url = asset.GetProperty("browser_download_url").GetString() ?? "";
                        if (!string.IsNullOrEmpty(url))
                            return (true, url);
                    }
                }

                return (false, "");
            }
            catch
            {
                return (false, "");
            }
        }

    }
}
