using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ArdysaModsTools.Services
{
    public class MiscService
    {
        private readonly LoggerService _logger;

        public MiscService(LoggerService logger)
        {
            _logger = logger;
        }

        // 👉 expose your static dictionaries (previously inside MiscellaneousForm)
        public static readonly Dictionary<string, string> WeatherValues = new() { /* ... */ };
        public static readonly Dictionary<string, string> MapValues = new() { /* ... */ };
        public static readonly Dictionary<string, string> MusicValues = new() { /* ... */ };
        public static readonly Dictionary<string, string> EmblemsValues = new() { /* ... */ };
        public static readonly Dictionary<string, string> ShadersValues = new() { /* ... */ };
        public static readonly Dictionary<string, string> AtkModifierValues = new() { /* ... */ };
        public static readonly Dictionary<string, string> RadiantCreepValues = new() { /* ... */ };
        public static readonly Dictionary<string, string> DireCreepValues = new() { /* ... */ };
        public static readonly Dictionary<string, string> RadiantSiegeValues = new() { /* ... */ };
        public static readonly Dictionary<string, string> DireSiegeValues = new() { /* ... */ };
        public static readonly Dictionary<string, string> HudValues = new() { /* ... */ };
        public static readonly Dictionary<string, string> VersusValues = new() { /* ... */ };
        public static readonly Dictionary<string, string> RiverValues = new() { /* ... */ };
        public static readonly Dictionary<string, string> RadiantTowerValues = new() { /* ... */ };
        public static readonly Dictionary<string, string> DireTowerValues = new() { /* ... */ };

        // --------------------------
        // PRESET HANDLING
        // --------------------------
        public void LoadPreset(params ComboBox[] boxes) { /* your old LoadPreset_Click logic here */ }
        public void SavePreset(params ComboBox[] boxes) { /* your old SavePreset_Click logic here */ }

        // --------------------------
        // GENERATION
        // --------------------------
        public async Task<OperationResult> GenerateAsync(
            string? targetPath,
            string? weather, string? map, string? music, string? emblem, string? shader,
            string? atkModifier, string? radiantCreep, string? direCreep,
            string? radiantSiege, string? direSiege,
            string? hud, string? versus, string? river,
            string? radiantTower, string? direTower)
        {
            try
            {
                if (string.IsNullOrEmpty(targetPath))
                    return OperationResult.Fail("Target path not set.");

                // 🔥 move all your old PerformGeneration() logic here
                // - Unpack pak01_dir.vpk
                // - Apply regex replacements
                // - Fetch remote resources
                // - Write to items_game.txt
                // - Repack with vpk.exe
                // - Cleanup

                _logger.Log("Mods installed successfully.");
                return OperationResult.Ok("Mods installed successfully.");
            }
            catch (Exception ex)
            {
                _logger.Log($"Error: {ex.Message}");
                return OperationResult.Fail(ex.Message);
            }
        }
    }
}
