using SharpCompress.Archives.Rar;
using SharpCompress.Readers;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;

namespace ArdysaModsTools.Services
{
    public class MiscService
    {
        private readonly Action<string> _log;
        private static readonly HttpClient httpClient;

        static MiscService()
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
            httpClient.Timeout = TimeSpan.FromSeconds(300);
            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Token", "YOUR_GITHUB_TOKEN");
        }

        public MiscService(Action<string> safeLog)
        {
            _log = safeLog ?? throw new ArgumentNullException(nameof(safeLog));
        }

        // --- Dictionaries (copied exactly from your provided values) ---
        private static readonly Dictionary<string, string> weatherValues = new Dictionary<string, string>
        {
            { "Default Weather", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Weather/Default.txt" },
            { "Weather Ash", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Weather/Ash.txt" },
            { "Weather Aurora", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Weather/Aurora.txt" },
            { "Weather Harvest", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Weather/Harvest.txt" },
            { "Weather Moonbeam", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Weather/Moonbeam.txt" },
            { "Weather Pestilence", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Weather/Pestilence.txt" },
            { "Weather Rain", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Weather/Rain.txt" },
            { "Weather Sirocco", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Weather/Sirocco.txt" },
            { "Weather Snow", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Weather/Snow.txt" },
            { "Weather Spring", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Weather/Spring.txt" }
        };

        private static readonly Dictionary<string, string> mapValues = new Dictionary<string, string>
        {
            { "Default Terrain", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Terrain/Default.txt" },
            { "Seasonal - Autumn", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Terrain/Seasonal%20Terrain%20-%20Autumn.txt" },
            { "Seasonal - Spring", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Terrain/Seasonal%20Terrain%20-%20Spring.txt" },
            { "Seasonal - Summer", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Terrain/Seasonal%20Terrain%20-%20Summer.txt" },
            { "Seasonal - Winter", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Terrain/Seasonal%20Terrain%20-%20Winter.txt" },
            { "Desert Terrain", "https://raw.githubusercontent.com/Anneardysa/Anneardysa/ArdysaModsTools/main/models/root/Terrain/Desert.txt" },
            { "Immortal Gardens", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Terrain/Immortal.txt" },
            { "Reef's Edge", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Terrain/Reef.txt" },
            { "Emerald Abyss", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Terrain/Emerald.txt" },
            { "Overgrown Empire", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Terrain/Overgrown.txt" },
            { "Sanctum of the Divine", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Terrain/Sanctum.txt" },
            { "The King's New Journey", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Terrain/King.txt" }
        };

        private static readonly Dictionary<string, string> musicValues = new Dictionary<string, string>
        {
            { "Default Music", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Music/Default.txt" },
            { "Deadmau5 Music", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Music/Deadmau5.txt" },
            { "Elemental Fury Music", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Music/Elemental.txt" },
            { "Desert Music", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Music/Desert.txt" },
            { "Magic Sticks of Dynamite", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Music/Dynamite.txt" },
            { "The FatRat Warrior Songs", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Music/FatRat.txt" },
            { "Heroes Within Music", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Music/HeroesWithin.txt" },
            { "Humanitys Last Breath Void", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Music/Humanitys.txt" },
            { "Northern Winds Music", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Music/Northern.txt" },
            { "JJ Lins Timekeeper Music", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Music/Timekeeper.txt" }
        };

        private static readonly Dictionary<string, string> emblemsValues = new Dictionary<string, string>
        {
            { "Disable Emblem", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Shaders/Diretide/materials/dev/root.md" },
            { "Aghanim 2021 Emblem", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Emblem/Emblems/Aghanim%202021/particles/ui_mouseactions/selected_ring.vpcf_c" },
            { "BattlePass 2022 Emblem", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Emblem/Emblems/Battlepass%202022/particles/ui_mouseactions/selected_ring.vpcf_c" },
            { "Crystal Echeron Emblem", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Emblem/Emblems/Crystal%20Echeron/particles/ui_mouseactions/selected_ring.vpcf_c" },
            { "Divinity Emblem", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Emblem/Emblems/Divinity/particles/ui_mouseactions/selected_ring.vpcf_c" },
            { "Diretide Green", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Emblem/Emblems/Diretide%20-%20Green/particles/ui_mouseactions/selected_ring.vpcf_c" },
            { "Diretide Blue", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Emblem/Emblems/Diretide%20-%20Blue/particles/ui_mouseactions/selected_ring.vpcf_c" },
            { "Diretide Red", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Emblem/Emblems/Diretide%20-%20Red/particles/ui_mouseactions/selected_ring.vpcf_c" },
            { "Diretide Yellow", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Emblem/Emblems/Diretide%20-%20Yellow/particles/ui_mouseactions/selected_ring.vpcf_c" },
            { "Nemestice Emblem", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Emblem/Emblems/Nemestice/particles/ui_mouseactions/selected_ring.vpcf_c" },
            { "Overgrown Emblem", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Emblem/Emblems/Overgrown/particles/ui_mouseactions/selected_ring.vpcf_c" },
            { "Sunken Emblem", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Emblem/Emblems/Sunken/particles/ui_mouseactions/selected_ring.vpcf_c" }
        };

        private static readonly Dictionary<string, string> shadersValues = new Dictionary<string, string>
        {
            { "Disable Shader", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Shaders/Diretide/materials/dev/root.md" },
            { "Aghanim Labyrinth", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Shader/Aghanim/materials/dev/deferred_post_process.vmat_c" },
            { "Diretide", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Shader/Diretide/materials/dev/deferred_post_process.vmat_c" }
        };

        private static readonly Dictionary<string, string> atkModifierValues = new Dictionary<string, string>
        {
            { "Disable Attack Modifier", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Shaders/Diretide/materials/dev/root.md" },
            { "Aghanim's Labyrinth", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Attack%20Modifier/Aghanim/Aghanim.rar" },
            { "Diretide - Blue", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Attack%20Modifier/Diretide%20-%20Blue/Diretide%20-%20Blue.rar" },
            { "Diretide - Green", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Attack%20Modifier/Diretide%20-%20Green/Diretide%20-%20Green.rar" },
            { "Diretide - Red", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Attack%20Modifier/Diretide%20-%20Red/Diretide%20-%20Red.rar" },
            { "Diretide - Yellow", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Attack%20Modifier/Diretide%20-%20Yellow/Diretide%20-%20Yellow.rar" },
            { "Nemestic", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Attack%20Modifier/Nemestic/Nemestic.rar" },
            { "The International 12", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Attack%20Modifier/T12/T12.rar" },
            { "The International 10", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Attack%20Modifier/TI10/TI10.rar" },
            { "The International 9", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Attack%20Modifier/TI9/TI9.rar" }
        };

        private static readonly Dictionary<string, string> radiantCreepValues = new Dictionary<string, string>
        {
            { "Default Radiant Creep", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Creep/Radiant/Default.txt" },
            { "Cavernite", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Creep/Radiant/Cavernite%20Radiant%20Creeps.txt" },
            { "Crownfall", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Creep/Radiant/Crownfall.txt" },
            { "Nemestice", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Creep/Radiant/Radiant%20Nemestice%20Creeps.txt" },
            { "Reptilian Refuge", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Creep/Radiant/Reptilian%20Refuge%20Radiant%20Creeps.txt" },
            { "Woodland Warbands", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Creep/Radiant/Woodland%20Warbands.txt" }
        };

        private static readonly Dictionary<string, string> direCreepValues = new Dictionary<string, string>
        {
            { "Default Dire Creep", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Creep/Dire/Default.txt" },
            { "Cavernite", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Creep/Dire/Cavernite%20Dire%20Creeps.txt" },
            { "Crownfall", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Creep/Dire/Crownfall.txt" },
            { "Nemestice", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Creep/Dire/Dire%20Nemestice%20Creeps.txt" },
            { "Reptilian Refuge", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Creep/Dire/Reptilian%20Refuge%20Dire%20Creeps.txt" },
            { "Woodland Warbands", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Creep/Dire/Woodland%20Warbands.txt" }
        };

        private static readonly Dictionary<string, string> direSiegeValues = new Dictionary<string, string>
        {
            { "Default Dire Siege", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Siege/Dire/Default.txt" },
            { "Crownfall", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Siege/Dire/Crownfall.txt" },
            { "Woodland Warbands", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Siege/Dire/Woodland%20Warbands.txt" }
        };

        private static readonly Dictionary<string, string> radiantSiegeValues = new Dictionary<string, string>
        {
            { "Default Radiant Siege", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Siege/Radiant/Default.txt" },
            { "Crownfall", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Siege/Radiant/Crownfall.txt" },
            { "Woodland Warbands", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Siege/Radiant/Woodland%20Warbands.txt" }
        };

        private static readonly Dictionary<string, string> hudValues = new Dictionary<string, string>
        {
            { "Default HUD", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/HUD/Default.txt" },
            { "Direstone", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/HUD/Direstone.txt" },
            { "Portal", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/HUD/Portal.txt" },
            { "Radiant Ore", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/HUD/Portal.txt" },
            { "Triumph", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/HUD/Radiant%20Ore.txt" },
            { "Valor", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/HUD/Triumph.txt" }
        };

        private static readonly Dictionary<string, string> versusValues = new Dictionary<string, string>
        {
            { "Default Versus Screen", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Versus/Default.txt" },
            { "The International 2019 - 1", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Versus/The%20International%202019%20Versus%20Screen%201.txt" },
            { "The International 2019 - 2", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Versus/The%20International%202019%20Versus%20Screen%202.txt" },
            { "The International 2020", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Versus/The%20International%202020.txt" },
            { "Battlepass 2022 - Diretide", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Versus/The%20International%202022%20Diretide.txt" },
            { "Battlepass 2022 - Nemestice", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Versus/The%20International%202022%20Nemestice.txt" },
            { "The International 2022 - 1", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Versus/The%20International%202022%20Versus%20Screen%201.txt" },
            { "The International 2022 - 2", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Versus/The%20International%202022%20Versus%20Screen%202.txt" },
            { "The International 2024", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Versus/The%20International%202024%20Versus%20Screen.txt" },
            { "Winter Versus Screen", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Versus/Winter%20Versus%20Screen.txt" }
        };

        private static readonly Dictionary<string, string> riverValues = new Dictionary<string, string>
        {
            { "Default Vial", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/River/root.md" },
            { "Blood Vial", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/River/Blood/Blood.rar" },
            { "Chrome Vial", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/River/Chrome/Chrome.rar" },
            { "Dry Vial", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/River/Dry/Dry.rar" },
            { "Electrifield Vial", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/River/Electrifield/Electrifield.rar" },
            { "Oil Vial", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/River/Oil/Oil.rar" }
        };

        private static readonly Dictionary<string, string> radiantTowerValues = new Dictionary<string, string>
        {
            { "Default Radiant Tower", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Tower/Radiant/Default.txt" },
            { "Declaration of the Divine Light", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Tower/Radiant/Declaration%20of%20the%20Divine%20Light%20Radiant%20Towers.txt" },
            { "Grasp of the Elder Gods", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Tower/Radiant/Grasp%20of%20the%20Elder%20Gods%20-%20Radiant%20Towers.txt" },
            { "Guardians of the Lost Path", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Tower/Radiant/Guardians%20of%20the%20Lost%20Path%20Radiant%20Towers.txt" },
            { "Stoneclaw Scavengers", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Tower/Radiant/Stoneclaw%20Scavengers%20Radiant%20Towers.txt" },
            { "The Eyes of Avilliva", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Tower/Radiant/The%20Eyes%20of%20Avilliva%20-%20Radiant%20Towers.txt" }
        };

        private static readonly Dictionary<string, string> direTowerValues = new Dictionary<string, string>
        {
            { "Default Dire Tower", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Tower/Dire/Default.txt" },
            { "Declaration of the Divine Shadow", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Tower/Dire/Declaration%20of%20the%20Divine%20Shadow%20Dire%20Towers.txt" },
            { "Grasp of the Elder Gods", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Tower/Dire/Grasp%20of%20the%20Elder%20Gods%20-%20Dire%20Towers.txt" },
            { "Guardians of the Lost Path", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Tower/Dire/Guardians%20of%20the%20Lost%20Path%20Dire%20Towers.txt" },
            { "Stoneclaw Scavengers", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Tower/Dire/Stoneclaw%20Scavengers%20Dire%20Towers.txt" },
            { "The Gaze of Scree'Auk", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Tower/Dire/The%20Gaze%20of%20Scree'Auk%20-%20Dire%20Towers.txt" }
        };

        // --- Public GenerateAsync (migrated PerformGeneration) ---
        public async Task GenerateAsync(
            string? targetPath,
            string? selectedWeather,
            string? selectedMap,
            string? selectedMusic,
            string? selectedEmblem,
            string? selectedShader,
            string? selectedAtkModifier,
            string? selectedRadiantCreep,
            string? selectedDireCreep,
            string? selectedRadiantSiege,
            string? selectedDireSiege,
            string? selectedHUD,
            string? selectedVersus,
            string? selectedRiver,
            string? selectedRadiantTower,
            string? selectedDireTower)
        {
            try
            {
                if (string.IsNullOrEmpty(targetPath))
                {
                    _log("No target path set.");
                    return;
                }

                string vpkPath = Path.Combine(targetPath, "game", "_ArdysaMods", "pak01_dir.vpk");
                if (!File.Exists(vpkPath))
                {
                    _log($"VPK file not found at: {vpkPath}");
                    return;
                }

                string extractDir = Path.Combine(targetPath, "game", "_ArdysaMods", "extracted");
                string hlExtractPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HLExtract.exe");
                string vpkToolPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "vpk.exe");

                _log("Checking Server...");
                if (!File.Exists(hlExtractPath) || !File.Exists(vpkToolPath))
                {
                    _log("Server is offline!.");
                    return;
                }

                _log($"Weather: {selectedWeather}");
                _log($"Terrain: {selectedMap}");
                _log($"Music: {selectedMusic}");
                _log($"Emblems: {selectedEmblem}");
                _log($"Shaders: {selectedShader}");
                _log($"Radiant Creeps: {selectedRadiantCreep}");
                _log($"Dire Creeps: {selectedDireCreep}");
                _log($"Dire Siege: {selectedDireSiege}");
                _log($"Radiant Siege: {selectedRadiantSiege}");
                _log($"Attack Modifier: {selectedAtkModifier}");
                _log($"HUD: {selectedHUD}");
                _log($"Versus: {selectedVersus}");
                _log($"River: {selectedRiver}");
                _log($"Radiant Tower: {selectedRadiantTower}");
                _log($"Dire Tower: {selectedDireTower}");
                _log("Please wait...");

                // prepare extraction
                if (Directory.Exists(extractDir)) Directory.Delete(extractDir, true);
                Directory.CreateDirectory(extractDir);

                if (!await RunProcessAndCapture(hlExtractPath, $"-p \"{vpkPath}\" -d \"{extractDir}\" -e \"root\"", AppDomain.CurrentDomain.BaseDirectory))
                {
                    _log("Extraction failed.");
                    return;
                }

                // move root contents
                string rootDir = Path.Combine(extractDir, "root");
                if (Directory.Exists(rootDir))
                {
                    _log("Moving contents...");
                    foreach (string file in Directory.GetFiles(rootDir, "*", SearchOption.AllDirectories))
                    {
                        string relativePath = Path.GetRelativePath(rootDir, file);
                        string destFile = Path.Combine(extractDir, relativePath);
                        Directory.CreateDirectory(Path.GetDirectoryName(destFile)!);
                        File.Move(file, destFile, true);
                    }
                    Directory.Delete(rootDir, true);
                }
                else
                {
                    _log("No 'root' folder found.");
                    return;
                }

                string itemsGamePath = Path.Combine(extractDir, "scripts", "items", "items_game.txt");
                if (!File.Exists(itemsGamePath))
                {
                    _log("core/dota not found.");
                    return;
                }

                _log("Modifying core/dota...");
                string content = await File.ReadAllTextAsync(itemsGamePath).ConfigureAwait(false);

                // WEATHER
                if (!string.IsNullOrEmpty(selectedWeather) && weatherValues.TryGetValue(selectedWeather, out string weatherValue))
                {
                    _log("Fetching Weather...");
                    string? weatherContent = await GetStringWithRetryAsync(weatherValue);
                    if (weatherContent != null)
                    {
                        string weatherPattern = @"(?s)""555""\s*\{[^}]*""prefab""\s*""weather""[^}]*\}.*?(?=""556""|$)";
                        if (Regex.IsMatch(content, weatherPattern))
                        {
                            content = Regex.Replace(content, weatherPattern, weatherContent);
                        }
                    }
                    await Task.Delay(1000).ConfigureAwait(false);
                }

                // MAP
                if (!string.IsNullOrEmpty(selectedMap) && mapValues.TryGetValue(selectedMap, out string mapValue))
                {
                    _log("Fetching Terrain...");
                    string? mapContent = await GetStringWithRetryAsync(mapValue);
                    if (mapContent != null)
                    {
                        string mapPattern = @"(?s)""590""\s*\{[^}]*""prefab""\s*""terrain""[^}]*\}.*?(?=""591""|$)";
                        if (Regex.IsMatch(content, mapPattern))
                        {
                            content = Regex.Replace(content, mapPattern, mapContent);
                        }
                    }
                    await Task.Delay(1000).ConfigureAwait(false);
                }

                // MUSIC
                if (!string.IsNullOrEmpty(selectedMusic) && musicValues.TryGetValue(selectedMusic, out string musicValue))
                {
                    _log("Fetching Music...");
                    string? musicContent = await GetStringWithRetryAsync(musicValue);
                    if (musicContent != null)
                    {
                        string musicPattern = @"(?s)""588""\s*\{[^}]*""prefab""\s*""music""[^}]*\}.*?(?=""589""|$)";
                        if (Regex.IsMatch(content, musicPattern))
                        {
                            content = Regex.Replace(content, musicPattern, musicContent);
                        }
                    }
                    await Task.Delay(1000).ConfigureAwait(false);
                }

                // RADIANT CREEPS
                if (!string.IsNullOrEmpty(selectedRadiantCreep) && radiantCreepValues.TryGetValue(selectedRadiantCreep, out string radiantCreepValue))
                {
                    _log("Fetching Radiant Creeps...");
                    string? radiantCreepContent = await GetStringWithRetryAsync(radiantCreepValue);
                    if (radiantCreepContent != null)
                    {
                        string radiantCreepPattern = @"(?s)""660""\s*\{[^}]*""prefab""\s*""radiantcreeps""[^}]*\}.*?(?=""661""|$)";
                        if (Regex.IsMatch(content, radiantCreepPattern))
                        {
                            content = Regex.Replace(content, radiantCreepPattern, radiantCreepContent);
                        }
                    }
                    await Task.Delay(1000).ConfigureAwait(false);
                }

                // DIRE CREEPS
                if (!string.IsNullOrEmpty(selectedDireCreep) && direCreepValues.TryGetValue(selectedDireCreep, out string direCreepValue))
                {
                    _log("Fetching Dire Creeps...");
                    string? direCreepContent = await GetStringWithRetryAsync(direCreepValue);
                    if (direCreepContent != null)
                    {
                        string direCreepPattern = @"(?s)""661""\s*\{[^}]*""prefab""\s*""direcreeps""[^}]*\}.*?(?=""662""|$)";
                        if (Regex.IsMatch(content, direCreepPattern))
                        {
                            content = Regex.Replace(content, direCreepPattern, direCreepContent);
                        }
                    }
                    await Task.Delay(1000).ConfigureAwait(false);
                }

                // RADIANT SIEGE
                if (!string.IsNullOrEmpty(selectedRadiantSiege) && radiantSiegeValues.TryGetValue(selectedRadiantSiege, out string radiantSiegeValue))
                {
                    _log("Fetching Radiant Siege...");
                    string? radiantSiegeContent = await GetStringWithRetryAsync(radiantSiegeValue);
                    if (radiantSiegeContent != null)
                    {
                        string radiantSiegePattern = @"(?s)""34462""\s*\{[^}]*""prefab""\s*""radiantsiegecreeps""[^}]*\}.*?(?=""34463""|$)";
                        if (Regex.IsMatch(content, radiantSiegePattern))
                        {
                            content = Regex.Replace(content, radiantSiegePattern, radiantSiegeContent);
                        }
                    }
                    await Task.Delay(1000).ConfigureAwait(false);
                }

                // DIRE SIEGE
                if (!string.IsNullOrEmpty(selectedDireSiege) && direSiegeValues.TryGetValue(selectedDireSiege, out string direSiegeValue))
                {
                    _log("Fetching Dire Siege...");
                    string? direSiegeContent = await GetStringWithRetryAsync(direSiegeValue);
                    if (direSiegeContent != null)
                    {
                        string direSiegePattern = @"(?s)""34463""\s*\{[^}]*""prefab""\s*""diresiegecreeps""[^}]*\}.*?(?=""34925""|$)";
                        if (Regex.IsMatch(content, direSiegePattern))
                        {
                            content = Regex.Replace(content, direSiegePattern, direSiegeContent);
                        }
                    }
                    await Task.Delay(1000).ConfigureAwait(false);
                }

                // RADIANT TOWER
                if (!string.IsNullOrEmpty(selectedRadiantTower) && radiantTowerValues.TryGetValue(selectedRadiantTower, out string radiantTowerValue))
                {
                    _log("Fetching Radiant Tower...");
                    string? radiantTowerContent = await GetStringWithRetryAsync(radiantTowerValue);
                    if (radiantTowerContent != null)
                    {
                        string radiantTowerPattern = @"(?s)""677""\s*\{[^}]*""prefab""\s*""radianttowers""[^}]*\}.*?(?=""678""|$)";
                        if (Regex.IsMatch(content, radiantTowerPattern))
                        {
                            content = Regex.Replace(content, radiantTowerPattern, radiantTowerContent);
                        }
                    }
                    await Task.Delay(1000).ConfigureAwait(false);
                }

                // DIRE TOWER
                if (!string.IsNullOrEmpty(selectedDireTower) && direTowerValues.TryGetValue(selectedDireTower, out string direTowerValue))
                {
                    _log("Fetching Dire Tower...");
                    string? direTowerContent = await GetStringWithRetryAsync(direTowerValue);
                    if (direTowerContent != null)
                    {
                        string direTowerPattern = @"(?s)""678""\s*\{[^}]*""prefab""\s*""diretowers""[^}]*\}.*?(?=""679""|$)";
                        if (Regex.IsMatch(content, direTowerPattern))
                        {
                            content = Regex.Replace(content, direTowerPattern, direTowerContent);
                        }
                    }
                    await Task.Delay(1000).ConfigureAwait(false);
                }

                // HUD
                if (!string.IsNullOrEmpty(selectedHUD) && hudValues.TryGetValue(selectedHUD, out string hudValue))
                {
                    _log("Fetching HUD...");
                    string? hudContent = await GetStringWithRetryAsync(hudValue);
                    if (hudContent != null)
                    {
                        string hudPattern = @"(?s)""587""\s*\{[^}]*""prefab""\s*""hud_skin""[^}]*\}.*?(?=""588""|$)";
                        if (Regex.IsMatch(content, hudPattern))
                        {
                            content = Regex.Replace(content, hudPattern, hudContent);
                        }
                    }
                    await Task.Delay(1000).ConfigureAwait(false);
                }

                // VERSUS
                if (!string.IsNullOrEmpty(selectedVersus) && versusValues.TryGetValue(selectedVersus, out string versusValue))
                {
                    _log("Fetching Versus Screen...");
                    string? versusContent = await GetStringWithRetryAsync(versusValue);
                    if (versusContent != null)
                    {
                        string versusPattern = @"(?s)""12970""\s*\{[^}]*""prefab""\s*""versus_screen""[^}]*\}.*?(?=""12971""|$)";
                        if (Regex.IsMatch(content, versusPattern))
                        {
                            content = Regex.Replace(content, versusPattern, versusContent);
                        }
                    }
                    await Task.Delay(1000).ConfigureAwait(false);
                }

                // RIVER (vial) - special handling (rar extraction or disabling)
                if (!string.IsNullOrEmpty(selectedRiver) && riverValues.TryGetValue(selectedRiver, out string riverValue))
                {
                    string? vpkDir = Path.GetDirectoryName(vpkPath);
                    if (vpkDir == null)
                    {
                        _log("Failed to determine VPK directory.");
                        return;
                    }

                    string tempDir = Path.Combine(vpkDir, "_temp"); // Temporary folder

                    if (selectedRiver == "Default Vial")
                    {
                        _log("Disabling Vial...");
                        string logPath = Path.Combine(tempDir, "extraction.log");
                        if (File.Exists(logPath))
                        {
                            string[] logLines = await File.ReadAllLinesAsync(logPath).ConfigureAwait(false);
                            HashSet<string> filesToDelete = new HashSet<string>(logLines.Select(line => line.Replace("Extracted: ", "")));

                            if (Directory.Exists(extractDir))
                            {
                                // delete extracted files listed in log
                                foreach (string file in Directory.GetFiles(extractDir, "*", SearchOption.AllDirectories))
                                {
                                    string relativePath = Path.GetRelativePath(extractDir, file);
                                    if (filesToDelete.Contains(relativePath))
                                    {
                                        File.Delete(file);
                                    }
                                }

                                // Remove empty directories
                                foreach (string dir in Directory.GetDirectories(extractDir, "*", SearchOption.AllDirectories).Reverse())
                                {
                                    if (!Directory.GetFiles(dir, "*", SearchOption.AllDirectories).Any())
                                    {
                                        Directory.Delete(dir, true);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // Download rar
                        _log("Fetching River Vial...");
                        string rarFilePath = Path.Combine(tempDir, "riverVial.rar");
                        Directory.CreateDirectory(tempDir);
                        using (var response = await GetWithRetryAsync(riverValue).ConfigureAwait(false))
                        {
                            if (response == null) return; // skip if not found
                            using (var fs = new FileStream(rarFilePath, FileMode.Create))
                            {
                                await response.Content.CopyToAsync(fs).ConfigureAwait(false);
                            }
                        }
                        await Task.Delay(1000).ConfigureAwait(false);

                        // Extract with SharpCompress
                        using (var archive = RarArchive.Open(rarFilePath, new ReaderOptions { Password = "muvestein@98" }))
                        {
                            var entries = archive.Entries.Where(e => !e.IsDirectory).ToList();
                            if (!entries.Any())
                            {
                                File.Delete(rarFilePath);
                                _log("No valid files found in River Vial archive.");
                                return;
                            }

                            foreach (var entry in entries)
                            {
                                if (string.IsNullOrEmpty(entry.Key)) continue;
                                string destPath = Path.Combine(tempDir, entry.Key);
                                string? dir = Path.GetDirectoryName(destPath);
                                if (dir != null) Directory.CreateDirectory(dir);

                                entry.WriteToFile(destPath);

                                // Log extraction
                                string logPath = Path.Combine(tempDir, "extraction.log");
                                await File.AppendAllTextAsync(logPath, $"Extracted: {entry.Key}\n").ConfigureAwait(false);
                            }
                        }
                        File.Delete(rarFilePath);

                        // Copy extracted to extractDir
                        foreach (string file in Directory.GetFiles(tempDir, "*", SearchOption.AllDirectories))
                        {
                            string relativePath = Path.GetRelativePath(tempDir, file);
                            if (Path.GetFileName(file) == "extraction.log") continue;

                            string destPath = Path.Combine(extractDir, relativePath);
                            string? dir = Path.GetDirectoryName(destPath);
                            if (dir != null) Directory.CreateDirectory(dir);

                            File.Copy(file, destPath, true);
                        }

                        foreach (string dir in Directory.GetDirectories(tempDir, "*", SearchOption.AllDirectories))
                        {
                            string relativePath = Path.GetRelativePath(tempDir, dir);
                            string destDir = Path.Combine(extractDir, relativePath);
                            if (!Directory.Exists(destDir)) Directory.CreateDirectory(destDir);
                        }

                        // verify
                        string extractedParticlesDir = Path.Combine(extractDir, "particles");
                        if (!Directory.Exists(extractedParticlesDir))
                        {
                            _log("Extraction failed or no valid River Vial folders found.");
                            return;
                        }
                    }
                }

                // EMBLEMS
                if (!string.IsNullOrEmpty(selectedEmblem) && emblemsValues.TryGetValue(selectedEmblem, out string emblemValue))
                {
                    string emblemDir = Path.Combine(extractDir, "particles", "ui_mouseactions");
                    if (selectedEmblem == "Disable Emblem")
                    {
                        _log("Disabling Emblems...");
                        if (Directory.Exists(emblemDir)) Directory.Delete(emblemDir, true);
                    }
                    else
                    {
                        Directory.CreateDirectory(emblemDir);
                        _log("Fetching Emblems...");
                        string emblemFileName = Path.GetFileName(emblemValue);
                        string emblemDestPath = Path.Combine(emblemDir, emblemFileName);
                        byte[]? emblemContent = await GetByteArrayWithRetryAsync(emblemValue).ConfigureAwait(false);
                        if (emblemContent != null) File.WriteAllBytes(emblemDestPath, emblemContent);
                        await Task.Delay(1000).ConfigureAwait(false);
                    }
                }

                // SHADERS
                if (!string.IsNullOrEmpty(selectedShader) && shadersValues.TryGetValue(selectedShader, out string shaderValue))
                {
                    string shaderDir = Path.Combine(extractDir, "materials", "dev");
                    if (selectedShader == "Disable Shader")
                    {
                        _log("Disabling Shaders...");
                        if (Directory.Exists(shaderDir)) Directory.Delete(shaderDir, true);
                    }
                    else
                    {
                        Directory.CreateDirectory(shaderDir);
                        _log("Fetching Shader...");
                        string shaderFileName = Path.GetFileName(shaderValue);
                        string shaderDestPath = Path.Combine(shaderDir, shaderFileName);
                        byte[]? shaderContent = await GetByteArrayWithRetryAsync(shaderValue).ConfigureAwait(false);
                        if (shaderContent != null) File.WriteAllBytes(shaderDestPath, shaderContent);
                        await Task.Delay(1000).ConfigureAwait(false);
                    }
                }

                // Write modified items_game
                await File.WriteAllTextAsync(itemsGamePath, content).ConfigureAwait(false);

                // ATTACK MODIFIER (RAR) handling
                if (!string.IsNullOrEmpty(selectedAtkModifier) && atkModifierValues.TryGetValue(selectedAtkModifier, out string atkModifierValue))
                {
                    string? vpkDir = Path.GetDirectoryName(vpkPath);
                    if (vpkDir == null)
                    {
                        _log("Failed to determine VPK directory.");
                        return;
                    }
                    string tempDir = Path.Combine(vpkDir, "_temp");

                    if (selectedAtkModifier == "Disable Attack Modifier")
                    {
                        _log("Disabling Attack Modifier...");
                        string logPath = Path.Combine(tempDir, "extraction.log");
                        if (File.Exists(logPath))
                        {
                            string[] logLines = await File.ReadAllLinesAsync(logPath).ConfigureAwait(false);
                            HashSet<string> filesToDelete = new HashSet<string>(logLines.Select(line => line.Replace("Extracted: ", "")));

                            if (Directory.Exists(extractDir))
                            {
                                foreach (string file in Directory.GetFiles(extractDir, "*", SearchOption.AllDirectories))
                                {
                                    string relativePath = Path.GetRelativePath(extractDir, file);
                                    if (filesToDelete.Contains(relativePath))
                                    {
                                        File.Delete(file);
                                    }
                                }

                                foreach (string dir in Directory.GetDirectories(extractDir, "*", SearchOption.AllDirectories).Reverse())
                                {
                                    string relativePath = Path.GetRelativePath(extractDir, dir);
                                    if (filesToDelete.Contains(relativePath) && !Directory.GetFiles(dir, "*", SearchOption.AllDirectories).Any())
                                    {
                                        Directory.Delete(dir, true);
                                    }
                                }
                            }
                        }

                        // nothing more to do for disabling
                    }
                    else
                    {
                        // download
                        _log("Fetching Attack Modifier...");
                        string rarFilePath = Path.Combine(tempDir, "atkModifier.rar");
                        Directory.CreateDirectory(tempDir);
                        using (var response = await GetWithRetryAsync(atkModifierValue).ConfigureAwait(false))
                        {
                            if (response == null) return;
                            using (var fs = new FileStream(rarFilePath, FileMode.Create))
                            {
                                await response.Content.CopyToAsync(fs).ConfigureAwait(false);
                            }
                        }
                        await Task.Delay(1000).ConfigureAwait(false);

                        // extract
                        using (var archive = RarArchive.Open(rarFilePath, new ReaderOptions { Password = "muvestein@98" }))
                        {
                            var entries = archive.Entries.Where(e => !e.IsDirectory).ToList();
                            if (!entries.Any())
                            {
                                File.Delete(rarFilePath);
                                return;
                            }

                            foreach (var entry in entries)
                            {
                                if (string.IsNullOrEmpty(entry.Key)) continue;
                                string destPath = Path.Combine(tempDir, entry.Key);
                                Directory.CreateDirectory(Path.GetDirectoryName(destPath)!);
                                entry.WriteToFile(destPath);

                                string logPath = Path.Combine(tempDir, "extraction.log");
                                await File.AppendAllTextAsync(logPath, $"Extracted: {entry.Key}\n").ConfigureAwait(false);
                            }
                        }
                        File.Delete(rarFilePath);

                        // copy to extractDir
                        foreach (string file in Directory.GetFiles(tempDir, "*", SearchOption.AllDirectories))
                        {
                            string relativePath = Path.GetRelativePath(tempDir, file);
                            if (Path.GetFileName(file) == "extraction.log") continue;
                            string destPath = Path.Combine(extractDir, relativePath);
                            Directory.CreateDirectory(Path.GetDirectoryName(destPath)!);
                            File.Copy(file, destPath, true);
                        }
                        foreach (string dir in Directory.GetDirectories(tempDir, "*", SearchOption.AllDirectories))
                        {
                            string relativePath = Path.GetRelativePath(tempDir, dir);
                            string destDir = Path.Combine(extractDir, relativePath);
                            if (!Directory.Exists(destDir)) Directory.CreateDirectory(destDir);
                        }

                        // verification
                        string extractedKisilevIndDir = Path.Combine(extractDir, "kisilev_ind", "particles", "modifier_attack");
                        string extractedParticlesDir = Path.Combine(extractDir, "particles");
                        if (!(Directory.Exists(extractedKisilevIndDir) || Directory.Exists(extractedParticlesDir)))
                        {
                            _log("Extraction failed or no valid folders found.");
                            return;
                        }
                    }
                }

                _log("Please wait... Do not close this App while processing");
                // pack
                if (!await RunProcessAndCapture(vpkToolPath, $"\"{extractDir}\"", Path.GetDirectoryName(vpkToolPath)))
                {
                    _log("Recompilation failed.");
                    return;
                }

                // confirm and move
                string? vpkDirFinal = Path.GetDirectoryName(vpkPath);
                if (vpkDirFinal == null)
                {
                    _log("Failed to determine VPK directory for final packaging.");
                    return;
                }
                string tempVpk = Path.Combine(vpkDirFinal, "extracted.vpk");
                if (File.Exists(tempVpk))
                {
                    if (File.Exists(vpkPath)) File.Delete(vpkPath);
                    File.Move(tempVpk, vpkPath);
                    Directory.Delete(extractDir, true);
                    _log("Mods installed successfully.");

                    // cleanup _temp except extraction.log
                    string tempDir = Path.Combine(vpkDirFinal, "_temp");
                    if (Directory.Exists(tempDir))
                    {
                        foreach (string file in Directory.GetFiles(tempDir, "*", SearchOption.AllDirectories))
                        {
                            if (Path.GetFileName(file) != "extraction.log")
                            {
                                File.Delete(file);
                            }
                        }
                        foreach (string dir in Directory.GetDirectories(tempDir, "*", SearchOption.AllDirectories).Reverse())
                        {
                            if (!Directory.GetFiles(dir, "*", SearchOption.AllDirectories).Any())
                            {
                                Directory.Delete(dir, true);
                            }
                        }
                        File.SetAttributes(tempDir, FileAttributes.Normal);
                    }
                }
                else
                {
                    _log($"Recompilation failed: {tempVpk} not found.");
                    return;
                }
            }
            catch (Exception ex)
            {
                _log($"Error: {ex.Message}");
            }
        }

        // --- Helpers ---
        private static async Task<HttpResponseMessage?> GetWithRetryAsync(string url, int maxRetries = 3)
        {
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    HttpResponseMessage response = await httpClient.GetAsync(url).ConfigureAwait(false);
                    if (response.StatusCode == HttpStatusCode.TooManyRequests)
                    {
                        string retryAfter = response.Headers.Contains("Retry-After") ? response.Headers.GetValues("Retry-After").FirstOrDefault() ?? "60" : "60";
                        if (int.TryParse(retryAfter, out int secs)) await Task.Delay(secs * 1000).ConfigureAwait(false);
                        else await Task.Delay(60 * 1000).ConfigureAwait(false);
                        continue;
                    }
                    if (response.StatusCode == HttpStatusCode.NotFound) return null;
                    response.EnsureSuccessStatusCode();
                    return response;
                }
                catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    await Task.Delay(5000).ConfigureAwait(false);
                    if (attempt == maxRetries) throw;
                }
                catch (TaskCanceledException)
                {
                    if (attempt == maxRetries) throw;
                    await Task.Delay(5000).ConfigureAwait(false);
                }
                catch
                {
                    if (attempt == maxRetries) throw;
                    await Task.Delay(2000).ConfigureAwait(false);
                }
            }
            return null;
        }

        private static async Task<string?> GetStringWithRetryAsync(string url) 
        {
            using var resp = await GetWithRetryAsync(url).ConfigureAwait(false);
            if (resp == null) return null;
            return await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        private static async Task<byte[]?> GetByteArrayWithRetryAsync(string url)
        {
            using var resp = await GetWithRetryAsync(url).ConfigureAwait(false);
            if (resp == null) return null;
            return await resp.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
        }

        private static async Task<bool> RunProcessAndCapture(string fileName, string args, string? workingDir)
        {
            var psi = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = args,
                WorkingDirectory = workingDir,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (var proc = new Process { StartInfo = psi })
            {
                proc.Start();
                string output = await proc.StandardOutput.ReadToEndAsync().ConfigureAwait(false);
                string error = await proc.StandardError.ReadToEndAsync().ConfigureAwait(false);
                proc.WaitForExit();

                // small logging (original used SafeLog("Extract output...") etc.)
                if (!string.IsNullOrEmpty(output)) { /* no-op (UI shows "Extract output...") */ }
                if (!string.IsNullOrEmpty(error)) { /* no-op */ }

                return proc.ExitCode == 0;
            }
        }
    }
}
