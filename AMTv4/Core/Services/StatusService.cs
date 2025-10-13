using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArdysaModsTools.Core.Services
{
    public class StatusService
    {
        private readonly ILogger _logger;

        public StatusService(ILogger logger)
        {
            _logger = logger;
        }

        public async Task CheckModsStatusAsync(
            string? targetPath,
            Label statusModsTextLabel,
            Label statusModsDotLabel)
        {
            if (string.IsNullOrEmpty(targetPath))
            {
                statusModsDotLabel.BackColor = Color.FromArgb(150, 150, 150);
                statusModsTextLabel.Text = "Not Checked";
                statusModsTextLabel.ForeColor = Color.FromArgb(150, 150, 150);
                return;
            }

            try
            {
                _logger.Log("Checking mods status...");
                string ardysaModsPath = Path.Combine(targetPath, "game", "_ArdysaMods", "_temp");
                string signaturesPath = Path.Combine(targetPath, "game", "bin", "win64", "dota.signatures");
                string coreJsonPath = Path.Combine(ardysaModsPath, "core.json");

                if (!File.Exists(signaturesPath))
                {
                    _logger.Log("Error: Missing dota.signatures file.");
                    SetStatus(statusModsDotLabel, statusModsTextLabel, "Error", Color.FromArgb(255, 50, 50));
                    return;
                }

                string signaturesContent = await File.ReadAllTextAsync(signaturesPath);
                Match digestMatch = Regex.Match(signaturesContent, @"DIGEST:([A-F0-9]+)");

                if (!digestMatch.Success)
                {
                    _logger.Log("Error: DIGEST not found (Code 2001).");
                    SetStatus(statusModsDotLabel, statusModsTextLabel, "Error", Color.FromArgb(255, 50, 50));
                    return;
                }

                string newDigest = digestMatch.Groups[1].Value;

                string existingJson = File.Exists(coreJsonPath)
                    ? await File.ReadAllTextAsync(coreJsonPath)
                    : "{}";

                var jsonObject = JsonSerializer.Deserialize<Dictionary<string, string>>(existingJson)
                                 ?? new Dictionary<string, string>();

                string? existingDigest = jsonObject.ContainsKey("digest") ? jsonObject["digest"] : null;

                jsonObject["digest"] = newDigest;
                string jsonContent = JsonSerializer.Serialize(jsonObject, new JsonSerializerOptions { WriteIndented = true });
                Directory.CreateDirectory(ardysaModsPath);
                await File.WriteAllTextAsync(coreJsonPath, jsonContent);

                if (existingDigest == newDigest)
                {
                    _logger.Log("Mods status: Ready.");
                    SetStatus(statusModsDotLabel, statusModsTextLabel, "Ready", Color.FromArgb(0, 200, 0));
                }
                else
                {
                    _logger.Log("Mods status: Need Update.");
                    SetStatus(statusModsDotLabel, statusModsTextLabel, "Need Update", Color.FromArgb(255, 165, 0));
                }
            }
            catch (Exception ex)
            {
                _logger.Log($"Error: Failed to check mods status — {ex.Message}");
                SetStatus(statusModsDotLabel, statusModsTextLabel, "Error", Color.FromArgb(255, 50, 50));
            }
        }

        private void SetStatus(Label dotLabel, Label textLabel, string text, Color color)
        {
            if (dotLabel.InvokeRequired)
            {
                dotLabel.BeginInvoke(new Action(() => SetStatus(dotLabel, textLabel, text, color)));
                return;
            }

            dotLabel.BackColor = color;
            textLabel.Text = text;
            textLabel.ForeColor = color;
        }
    }
}
