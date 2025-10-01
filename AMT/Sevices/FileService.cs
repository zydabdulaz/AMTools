using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArdysaModsTools.Services
{
    public class FileService
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public async Task<bool> ExtractAndValidateVPKAsync(string targetPath, LoggerService logger)
        {
            string vpkPath = Path.Combine(targetPath, "game", "_ArdysaMods", "pak01_dir.vpk");
            string extractDir = Path.Combine(targetPath, "game", "_ArdysaMods", "pak01_dir");
            string hlExtractPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HLExtract.exe");
            string validationFile = Path.Combine(extractDir, "root", "version", "_ArdysaMods");

            if (!File.Exists(vpkPath))
            {
                logger.Error($"pak01_dir.vpk not found at {vpkPath}");
                return false;
            }

            if (!File.Exists(hlExtractPath))
            {
                logger.Error("HLExtract.exe missing!");
                return false;
            }

            try
            {
                if (Directory.Exists(extractDir)) Directory.Delete(extractDir, true);
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

                using var process = new Process { StartInfo = psi };
                process.Start();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    logger.Error("HLExtract failed: " + error);
                    return false;
                }

                if (!File.Exists(validationFile))
                {
                    logger.Error("Validation failed: required file missing.");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                logger.Error($"Extraction failed: {ex.Message}");
                return false;
            }
        }

        public async Task CopyModsAsync(string targetPath)
        {
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            string sourceGamePath = Path.Combine(appPath, "game");
            string destGamePath = Path.Combine(targetPath, "game");

            DirectoryCopy(sourceGamePath, destGamePath, true);
            await Task.CompletedTask;
        }

        public async Task PatchSignaturesAsync(string targetPath, string gameInfoUrl)
        {
            string signaturesPath = Path.Combine(targetPath, "game", "bin", "win64", "dota.signatures");
            string gameInfoPath = Path.Combine(targetPath, "game", "dota", "gameinfo_branchspecific.gi");

            if (!File.Exists(signaturesPath)) return;

            string[] lines = await File.ReadAllLinesAsync(signaturesPath);
            int digestIndex = Array.FindIndex(lines, line => line.StartsWith("DIGEST:"));

            if (digestIndex == -1) return;

            List<string> modifiedLines = new List<string>(lines[..(digestIndex + 1)]);
            modifiedLines.Add(@"...\..\..\dota\gameinfo_branchspecific.gi~SHA1:1A9B91FB43FE89AD104B8001282D292EED94584D;CRC:043F604A");

            await File.WriteAllLinesAsync(signaturesPath, modifiedLines);

            Directory.CreateDirectory(Path.GetDirectoryName(gameInfoPath)!);
            byte[] gameInfoBytes = await httpClient.GetByteArrayAsync(gameInfoUrl);
            await File.WriteAllBytesAsync(gameInfoPath, gameInfoBytes);
        }

        public async Task RevertSignaturesAsync(string targetPath)
        {
            string signaturesPath = Path.Combine(targetPath, "game", "bin", "win64", "dota.signatures");
            if (!File.Exists(signaturesPath)) return;

            string[] lines = await File.ReadAllLinesAsync(signaturesPath);
            int digestIndex = Array.FindIndex(lines, line => line.StartsWith("DIGEST:"));

            if (digestIndex == -1) return;

            List<string> reverted = new List<string>(lines[..(digestIndex + 1)]);
            await File.WriteAllLinesAsync(signaturesPath, reverted);
        }

        public async Task<bool> CheckModsDigestAsync(string targetPath)
        {
            string signaturesPath = Path.Combine(targetPath, "game", "bin", "win64", "dota.signatures");
            string coreJsonPath = Path.Combine(targetPath, "game", "_ArdysaMods", "_temp", "core.json");

            if (!File.Exists(signaturesPath)) return false;

            string content = await File.ReadAllTextAsync(signaturesPath);
            Match digestMatch = Regex.Match(content, @"DIGEST:([A-F0-9]+)");
            if (!digestMatch.Success) return false;

            string newDigest = digestMatch.Groups[1].Value;
            var json = File.Exists(coreJsonPath)
                ? JsonSerializer.Deserialize<Dictionary<string, string>>(await File.ReadAllTextAsync(coreJsonPath))
                : new Dictionary<string, string>();

            string? oldDigest = json.ContainsKey("digest") ? json["digest"] : null;
            json["digest"] = newDigest;

            string updatedJson = JsonSerializer.Serialize(json, new JsonSerializerOptions { WriteIndented = true });
            Directory.CreateDirectory(Path.GetDirectoryName(coreJsonPath)!);
            await File.WriteAllTextAsync(coreJsonPath, updatedJson);

            return oldDigest == newDigest;
        }

        public void LoadResourceIcon(string resourceName, PictureBox pic, LoggerService logger)
        {
            try
            {
                using var stream = typeof(FileService).Assembly.GetManifestResourceStream(resourceName);
                if (stream != null) pic.Image = Image.FromStream(stream);
                else logger.Warn($"Resource {resourceName} not found.");
            }
            catch (Exception ex)
            {
                logger.Error($"Failed to load icon {resourceName}: {ex.Message}");
            }
        }

        private void DirectoryCopy(string sourceDir, string destDir, bool copySubDirs)
        {
            DirectoryInfo dir = new(sourceDir);
            if (!dir.Exists) throw new DirectoryNotFoundException($"Source dir not found: {sourceDir}");

            Directory.CreateDirectory(destDir);

            foreach (FileInfo file in dir.GetFiles())
                file.CopyTo(Path.Combine(destDir, file.Name), true);

            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dir.GetDirectories())
                    DirectoryCopy(subdir.FullName, Path.Combine(destDir, subdir.Name), copySubDirs);
            }
        }
    }
}
