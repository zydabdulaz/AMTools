using System.Diagnostics;
using System.Text.RegularExpressions;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Readers;

namespace ArdysaModsTools.Services
{
    public class MiscService
    {
        private readonly LoggerService _logger;
        private static readonly HttpClient httpClient;

        static MiscService()
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "ArdysaModsTools");
            httpClient.Timeout = TimeSpan.FromSeconds(300);
            // ⚠️ Replace with secure storage or config
            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Token", "YOUR_GITHUB_TOKEN");
        }

        public MiscService(LoggerService logger) => _logger = logger;

        #region Dictionaries (fill in like before)
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
        #endregion

        #region Helpers
        private async Task<string?> GetStringWithRetryAsync(string url)
        {
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    var res = await httpClient.GetAsync(url);
                    if (res.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    {
                        await Task.Delay(5000);
                        continue;
                    }
                    if (res.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
                    res.EnsureSuccessStatusCode();
                    return await res.Content.ReadAsStringAsync();
                }
                catch { await Task.Delay(2000); }
            }
            return null;
        }
        #endregion

        #region Main Generation
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
                    return OperationResult.Fail("Target path is empty.");

                string vpkPath = Path.Combine(targetPath, "game", "_ArdysaMods", "pak01_dir.vpk");
                if (!File.Exists(vpkPath))
                    return OperationResult.Fail($"VPK not found: {vpkPath}");

                string extractDir = Path.Combine(targetPath, "game", "_ArdysaMods", "extracted");
                string itemsGamePath = Path.Combine(extractDir, "scripts", "items", "items_game.txt");

                // === Extraction (HLExtract)
                if (Directory.Exists(extractDir)) Directory.Delete(extractDir, true);
                Directory.CreateDirectory(extractDir);

                string hlExtractPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HLExtract.exe");
                var psi = new ProcessStartInfo
                {
                    FileName = hlExtractPath,
                    Arguments = $"-p \"{vpkPath}\" -d \"{extractDir}\" -e \"root\"",
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };
                using (var proc = Process.Start(psi)!)
                {
                    proc.WaitForExit();
                    if (proc.ExitCode != 0) return OperationResult.Fail("Extraction failed.");
                }

                if (!File.Exists(itemsGamePath))
                    return OperationResult.Fail("items_game.txt not found after extraction.");

                string content = File.ReadAllText(itemsGamePath);

                // === WEATHER
                if (weather != null && WeatherValues.TryGetValue(weather, out var weatherUrl))
                {
                    var weatherContent = await GetStringWithRetryAsync(weatherUrl);
                    if (weatherContent != null)
                        content = Regex.Replace(content, @"(?s)""555"".*?(?=""556""|$)", weatherContent);
                }

                // === TERRAIN
                if (map != null && MapValues.TryGetValue(map, out var mapUrl))
                {
                    var mapContent = await GetStringWithRetryAsync(mapUrl);
                    if (mapContent != null)
                        content = Regex.Replace(content, @"(?s)""2123"".*?(?=""2124""|$)", mapContent);
                }

                // === MUSIC
                if (music != null && MusicValues.TryGetValue(music, out var musicUrl))
                {
                    var musicContent = await GetStringWithRetryAsync(musicUrl);
                    if (musicContent != null)
                        content = Regex.Replace(content, @"(?s)""13100"".*?(?=""13101""|$)", musicContent);
                }

                // === EMBLEM
                if (emblem != null && EmblemsValues.TryGetValue(emblem, out var emblemUrl))
                {
                    var emblemContent = await GetStringWithRetryAsync(emblemUrl);
                    if (emblemContent != null)
                        content = Regex.Replace(content, @"(?s)""4200"".*?(?=""4201""|$)", emblemContent);
                }

                // === SHADER
                if (shader != null && ShadersValues.TryGetValue(shader, out var shaderUrl))
                {
                    var shaderContent = await GetStringWithRetryAsync(shaderUrl);
                    if (shaderContent != null)
                        content = Regex.Replace(content, @"(?s)""990"".*?(?=""991""|$)", shaderContent);
                }

                // === ATTACK MODIFIER
                if (atkModifier != null && AtkModifierValues.TryGetValue(atkModifier, out var atkUrl))
                {
                    var atkContent = await GetStringWithRetryAsync(atkUrl);
                    if (atkContent != null)
                        content = Regex.Replace(content, @"(?s)""7000"".*?(?=""7001""|$)", atkContent);
                }

                // === CREEPS (Radiant/Dire)
                if (radiantCreep != null && RadiantCreepValues.TryGetValue(radiantCreep, out var rcUrl))
                {
                    var rcContent = await GetStringWithRetryAsync(rcUrl);
                    if (rcContent != null)
                        content = Regex.Replace(content, @"(?s)""2001"".*?(?=""2002""|$)", rcContent);
                }
                if (direCreep != null && DireCreepValues.TryGetValue(direCreep, out var dcUrl))
                {
                    var dcContent = await GetStringWithRetryAsync(dcUrl);
                    if (dcContent != null)
                        content = Regex.Replace(content, @"(?s)""2002"".*?(?=""2003""|$)", dcContent);
                }

                // === SIEGE
                if (radiantSiege != null && RadiantSiegeValues.TryGetValue(radiantSiege, out var rsUrl))
                {
                    var rsContent = await GetStringWithRetryAsync(rsUrl);
                    if (rsContent != null)
                        content = Regex.Replace(content, @"(?s)""3001"".*?(?=""3002""|$)", rsContent);
                }
                if (direSiege != null && DireSiegeValues.TryGetValue(direSiege, out var dsUrl))
                {
                    var dsContent = await GetStringWithRetryAsync(dsUrl);
                    if (dsContent != null)
                        content = Regex.Replace(content, @"(?s)""3002"".*?(?=""3003""|$)", dsContent);
                }

                // === HUD
                if (hud != null && HudValues.TryGetValue(hud, out var hudUrl))
                {
                    var hudContent = await GetStringWithRetryAsync(hudUrl);
                    if (hudContent != null)
                        content = Regex.Replace(content, @"(?s)""4000"".*?(?=""4001""|$)", hudContent);
                }

                // === VERSUS
                if (versus != null && VersusValues.TryGetValue(versus, out var versusUrl))
                {
                    var versusContent = await GetStringWithRetryAsync(versusUrl);
                    if (versusContent != null)
                        content = Regex.Replace(content, @"(?s)""6000"".*?(?=""6001""|$)", versusContent);
                }

                // === RIVER
                if (river != null && RiverValues.TryGetValue(river, out var riverUrl))
                {
                    var riverContent = await GetStringWithRetryAsync(riverUrl);
                    if (riverContent != null)
                        content = Regex.Replace(content, @"(?s)""8000"".*?(?=""8001""|$)", riverContent);
                }

                // === TOWERS
                if (radiantTower != null && RadiantTowerValues.TryGetValue(radiantTower, out var rtUrl))
                {
                    var rtContent = await GetStringWithRetryAsync(rtUrl);
                    if (rtContent != null)
                        content = Regex.Replace(content, @"(?s)""9000"".*?(?=""9001""|$)", rtContent);
                }
                if (direTower != null && DireTowerValues.TryGetValue(direTower, out var dtUrl))
                {
                    var dtContent = await GetStringWithRetryAsync(dtUrl);
                    if (dtContent != null)
                        content = Regex.Replace(content, @"(?s)""9001"".*?(?=""9002""|$)", dtContent);
                }

                File.WriteAllText(itemsGamePath, content);

                // === REPACK (vpk.exe)
                string vpkToolPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "vpk.exe");
                var packPsi = new ProcessStartInfo
                {
                    FileName = vpkToolPath,
                    Arguments = $"\"{extractDir}\"",
                    WorkingDirectory = Path.GetDirectoryName(vpkPath)!,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (var proc = Process.Start(packPsi)!)
                {
                    proc.WaitForExit();
                    if (proc.ExitCode != 0) return OperationResult.Fail("Repack failed.");
                }

                _logger.Log("✅ Mods installed successfully.");
                return OperationResult.Ok("Mods installed successfully.");
            }
            catch (Exception ex)
            {
                _logger.Log($"❌ Error: {ex.Message}");
                return OperationResult.Fail(ex.Message);
            }
        }
        #endregion
    }
}
