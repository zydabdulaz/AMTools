using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArdysaModsTools.Core.Services
{
    /// <summary>
    /// Handles saving and restoring ComboBox selections for persistence across sessions.
    /// Stores values in %TEMP%/vuex.log by default.
    /// </summary>
    public static class UserSettingsService
    {
        private static readonly string LogPath = Path.Combine(Path.GetTempPath(), "vuex.log");

        /// <summary>
        /// Saves the selected ComboBox values to vuex.log.
        /// </summary>
        public static async Task SaveSelectionsAsync(Form form)
        {
            try
            {
                var selections = form.Controls.OfType<ComboBox>()
                    .Where(c => c.Name.EndsWith("Box", StringComparison.OrdinalIgnoreCase))
                    .ToDictionary(
                        c => c.Name.Replace("Box", "", StringComparison.OrdinalIgnoreCase),
                        c => c.SelectedItem?.ToString() ?? "",
                        StringComparer.OrdinalIgnoreCase);

                var lines = selections.Select(kv => $"{kv.Key}={kv.Value}");
                await File.WriteAllLinesAsync(LogPath, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARN] Failed to save vuex.log: {ex.Message}");
            }
        }

        /// <summary>
        /// Restores saved ComboBox selections from vuex.log (if it exists).
        /// Should be called after ComboBoxes are populated.
        /// </summary>
        public static async Task RestoreSelectionsAsync(Form form, Action<string>? logAction = null)
        {
            try
            {
                if (!File.Exists(LogPath))
                {
                    logAction?.Invoke("No previous settings found.");
                    return;
                }

                var lines = await File.ReadAllLinesAsync(LogPath);
                var saved = lines
                    .Select(l => l.Split('='))
                    .Where(p => p.Length == 2)
                    .ToDictionary(p => p[0].Trim(), p => p[1].Trim(), StringComparer.OrdinalIgnoreCase);

                foreach (var kv in saved)
                {
                    var box = form.Controls.OfType<ComboBox>()
                        .FirstOrDefault(c => c.Name.StartsWith(kv.Key, StringComparison.OrdinalIgnoreCase));

                    if (box != null && box.Items.Contains(kv.Value))
                        box.SelectedItem = kv.Value;
                }
                logAction?.Invoke("Restored previous settings successfully.");
            }
            catch (Exception ex)
            {
                logAction?.Invoke($"Failed to restore vuex.log: {ex.Message}");
            }
        }
    }
}
