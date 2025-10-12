using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArdysaModsTools.Helpers;

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

        // ------------------------------------------------------------
        //  INSTALL MODS
        // ------------------------------------------------------------
        public async Task<bool> InstallModsAsync(string targetPath, string appPath)
        {
            _logger.Log("Installing mods...");
            string sourceGamePath = Path.Combine(appPath, "game");
            string copyDestination = Path.Combine(targetPath, "game");

            if (!Directory.Exists(sourceGamePath))
            {
                _logger.Log($"Error: 'game' folder not found at '{sourceGamePath}'.");
                MessageBox.Show($"The 'game' folder is missing at '{sourceGamePath}'. Please include it next to ArdysaModsTools.exe.",
                    "Installation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Copy everything
            FileHelper.DirectoryCopy(sourceGamePath, copyDestination, true);

            // Prepare patch files
            string signaturesPath = Path.Combine(copyDestination, "bin", "win64", "dota.signatures");
            string gameInfoPath = Path.Combine(copyDestination, "dota", "gameinfo_branchspecific.gi");

            Directory.CreateDirectory(Path.GetDirectoryName(signaturesPath)!);
            Directory.CreateDirectory(Path.GetDirectoryName(gameInfoPath)!);

            // Download latest gameinfo
            byte[] giData = await _httpClient.GetByteArrayAsync(GameInfoUrl);
            await File.WriteAllBytesAsync(gameInfoPath, giData);

            if (!File.Exists(signaturesPath))
            {
                _logger.Log("Missing 'dota.signatures' — cannot patch.");
                return false;
            }

            // Append patch line
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

            // Clean extracted temp directory
            string extractDir = Path.Combine(sourceGamePath, "_ArdysaMods", "pak01_dir");
            try
            {
                if (Directory.Exists(extractDir))
                    Directory.Delete(extractDir, true);
            }
            catch { /* ignore cleanup failure */ }

            _logger.Log("Mod installation completed successfully.");
            return true;
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
                _logger.Log("No signatures file found — mods may already be disabled.");
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
        private bool ExtractAndValidateVPK(string sourceGamePath)
        {
            string vpkPath = Path.Combine(sourceGamePath, "_ArdysaMods", "pak01_dir.vpk");
            string extractDir = Path.Combine(sourceGamePath, "_ArdysaMods", "pak01_dir");
            string hlExtractPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HLExtract.exe");
            string validationFile = Path.Combine(extractDir, "root", "version", "_ArdysaMods");

            if (!File.Exists(vpkPath))
            {
                _logger.Log($"Missing 'pak01_dir.vpk' at '{vpkPath}'.");
                return false;
            }

            if (!File.Exists(hlExtractPath))
            {
                _logger.Log("Missing 'HLExtract.exe'.");
                return false;
            }

            try
            {
                if (Directory.Exists(extractDir))
                    Directory.Delete(extractDir, true);
                Directory.CreateDirectory(extractDir);

                var psi = new ProcessStartInfo
                {
                    FileName = hlExtractPath,
                    Arguments = $"-p \"{vpkPath}\" -d \"{extractDir}\" -e \"root\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using var proc = new Process { StartInfo = psi };
                proc.Start();
                string output = proc.StandardOutput.ReadToEnd();
                string error = proc.StandardError.ReadToEnd();
                proc.WaitForExit();

                if (proc.ExitCode != 0)
                {
                    _logger.Log($"HLExtract failed: {error}");
                    return false;
                }

                if (!File.Exists(validationFile))
                {
                    _logger.Log("Missing validation file inside extracted VPK.");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.Log($"VPK extraction failed: {ex.Message}");
                return false;
            }
        }

    }
}
