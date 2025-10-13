using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Readers;
using ArdysaModsTools.Core.Models;

namespace ArdysaModsTools.Core.Services
{
    /// <summary>
    /// Handles mod generation: extracting VPK, patching assets, and repacking.
    /// </summary>
    /// <remarks>
    /// Pure backend logic. The UI communicates through `Action<string> log`.
    /// </remarks>
    public class MiscGenerationService
    {
        // --- URL dictionaries (moved from form) ---
        // Only keys are shown here: selection keys like "Weather", "Map", "Music", etc.
        // Values are the URL resources used by the generation logic.
        // (I copied your dictionaries; feel free to edit/trim/extend later.)
        private readonly Dictionary<string, string> weatherValues = new()
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

        private readonly Dictionary<string, string> mapValues = new()
        {
            { "Default Terrain", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Terrain/Default.txt" },
            { "Seasonal - Autumn", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Terrain/Seasonal%20Terrain%20-%20Autumn.txt" },
            { "Seasonal - Spring", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Terrain/Seasonal%20Terrain%20-%20Spring.txt" },
            { "Seasonal - Summer", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Terrain/Seasonal%20Terrain%20-%20Summer.txt" },
            { "Seasonal - Winter", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Terrain/Seasonal%20Terrain%20-%20Winter.txt" },
            { "Desert Terrain", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Terrain/Desert.txt" },
            { "Immortal Gardens", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Terrain/Immortal.txt" },
            { "Reef's Edge", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Terrain/Reef.txt" },
            { "Emerald Abyss", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Terrain/Emerald.txt" },
            { "Overgrown Empire", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Terrain/Overgrown.txt" },
            { "Sanctum of the Divine", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Terrain/Sanctum.txt" },
            { "The King's New Journey", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Terrain/King.txt" }
        };

        private readonly Dictionary<string, string> musicValues = new()
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

        private readonly Dictionary<string, string> emblemsValues = new()
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

        private readonly Dictionary<string, string> shadersValues = new()
        {
            { "Disable Shader", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Shaders/Diretide/materials/dev/root.md" },
            { "Aghanim Labyrinth", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Shader/Aghanim/materials/dev/deferred_post_process.vmat_c" },
            { "Diretide", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Shader/Diretide/materials/dev/deferred_post_process.vmat_c" }
        };

        private readonly Dictionary<string, string> atkModifierValues = new()
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

        private readonly Dictionary<string, string> radiantCreepValues = new()
        {
            { "Default Radiant Creep", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Creep/Radiant/Default.txt" },
            { "Cavernite", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Creep/Radiant/Cavernite%20Radiant%20Creeps.txt" },
            { "Crownfall", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Creep/Radiant/Crownfall.txt" },
            { "Nemestice", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Creep/Radiant/Radiant%20Nemestice%20Creeps.txt" },
            { "Reptilian Refuge", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Creep/Radiant/Reptilian%20Refuge%20Radiant%20Creeps.txt" },
            { "Woodland Warbands", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Creep/Radiant/Woodland%20Warbands.txt" }
        };

        private readonly Dictionary<string, string> direCreepValues = new()
        {
            { "Default Dire Creep", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Creep/Dire/Default.txt" },
            { "Cavernite", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Creep/Dire/Cavernite%20Dire%20Creeps.txt" },
            { "Crownfall", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Creep/Dire/Crownfall.txt" },
            { "Nemestice", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Creep/Dire/Dire%20Nemestice%20Creeps.txt" },
            { "Reptilian Refuge", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Creep/Dire/Reptilian%20Refuge%20Dire%20Creeps.txt" },
            { "Woodland Warbands", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Creep/Dire/Woodland%20Warbands.txt" }
        };

        private readonly Dictionary<string, string> direSiegeValues = new()
        {
            { "Default Dire Siege", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Siege/Dire/Default.txt" },
            { "Crownfall", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Siege/Dire/Crownfall.txt" },
            { "Woodland Warbands", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Siege/Dire/Woodland%20Warbands.txt" }
        };

        private readonly Dictionary<string, string> radiantSiegeValues = new()
        {
            { "Default Radiant Siege", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Siege/Radiant/Default.txt" },
            { "Crownfall", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Siege/Radiant/Crownfall.txt" },
            { "Woodland Warbands", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Siege/Radiant/Woodland%20Warbands.txt" }
        };

        private readonly Dictionary<string, string> hudValues = new()
        {
            { "Default HUD", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/HUD/Default.txt" },
            { "Direstone", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/HUD/Direstone.txt" },
            { "Portal", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/HUD/Portal.txt" },
            { "Radiant Ore", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/HUD/Portal.txt" },
            { "Triumph", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/HUD/Radiant%20Ore.txt" },
            { "Valor", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/HUD/Triumph.txt" }
        };

        private readonly Dictionary<string, string> versusValues = new()
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

        private readonly Dictionary<string, string> riverValues = new()
        {
            { "Default Vial", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/River/root.md" },
            { "Blood Vial", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/River/Blood/Blood.rar" },
            { "Chrome Vial", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/River/Chrome/Chrome.rar" },
            { "Dry Vial", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/River/Dry/Dry.rar" },
            { "Electrifield Vial", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/River/Electrifield/Electrifield.rar" },
            { "Oil Vial", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/River/Oil/Oil.rar" }
        };

        private readonly Dictionary<string, string> radiantTowerValues = new()
        {
            { "Default Radiant Tower", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Tower/Radiant/Default.txt" },
            { "Declaration of the Divine Light", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Tower/Radiant/Declaration%20of%20the%20Divine%20Light%20Radiant%20Towers.txt" },
            { "Grasp of the Elder Gods", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Tower/Radiant/Grasp%20of%20the%20Elder%20Gods%20-%20Radiant%20Towers.txt" },
            { "Guardians of the Lost Path", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Tower/Radiant/Guardians%20of%20the%20Lost%20Path%20Radiant%20Towers.txt" },
            { "Stoneclaw Scavengers", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Tower/Radiant/Stoneclaw%20Scavengers%20Radiant%20Towers.txt" },
            { "The Eyes of Avilliva", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Tower/Radiant/The%20Eyes%20of%20Avilliva%20-%20Radiant%20Towers.txt" }
        };

        private readonly Dictionary<string, string> direTowerValues = new()
        {
            { "Default Dire Tower", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Tower/Dire/Default.txt" },
            { "Declaration of the Divine Shadow", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Tower/Dire/Declaration%20of%20the%20Divine%20Shadow%20Dire%20Towers.txt" },
            { "Grasp of the Elder Gods", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Tower/Dire/Grasp%20of%20the%20Elder%20Gods%20-%20Dire%20Towers.txt" },
            { "Guardians of the Lost Path", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Tower/Dire/Guardians%20of%20the%20Lost%20Path%20Dire%20Towers.txt" },
            { "Stoneclaw Scavengers", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Tower/Dire/Stoneclaw%20Scavengers%20Dire%20Towers.txt" },
            { "The Gaze of Scree'Auk", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Tower/Dire/The%20Gaze%20of%20Scree'Auk%20-%20Dire%20Towers.txt" }
        };

        // Constructor - currently stateless but kept for future DI
        public MiscGenerationService() { }

        // -------------------------------------------------------
        // Public API
        // -------------------------------------------------------
        /// <summary>
        /// Perform generation using the provided selections. UI should pass user-selected values in `selections`.
        /// Keys used by the service: "Weather","Map","Music","Emblem","Shader","AtkModifier",
        /// "RadiantCreep","DireCreep","RadiantSiege","DireSiege","HUD","Versus","River","RadiantTower","DireTower"
        /// </summary>
        public async Task<OperationResult> PerformGenerationAsync(
            string targetPath,
            Dictionary<string, string> selections,
            Action<string> log,
            CancellationToken ct = default)
        {
            try
            {
                ct.ThrowIfCancellationRequested();

                if (string.IsNullOrEmpty(targetPath))
                {
                    log("No target path set.");
                    return new OperationResult { Success = false, Message = "No target path set." };
                }

                string vpkPath = Path.Combine(targetPath, "game", "_ArdysaMods", "pak01_dir.vpk");
                if (!File.Exists(vpkPath))
                {
                    log($"VPK file not found at: {vpkPath}");
                    return new OperationResult { Success = false, Message = "VPK file not found." };
                }

                string extractDir = Path.Combine(targetPath, "game", "_ArdysaMods", "extracted");
                string hlExtractPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HLExtract.exe");
                string vpkToolPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "vpk.exe");

                log("Checking Server tools...");
                if (!File.Exists(hlExtractPath) || !File.Exists(vpkToolPath))
                {
                    log("Server tools (HLExtract.exe or vpk.exe) not found.");
                    return new OperationResult { Success = false, Message = "Server tools not found." };
                }

                // Ensure a fresh extract directory
                if (Directory.Exists(extractDir)) Directory.Delete(extractDir, true);
                Directory.CreateDirectory(extractDir);

                // 1) Extract using HLExtract.exe (run process and wait)
                log("Extracting VPK...");
                var extractPsi = new ProcessStartInfo
                {
                    FileName = hlExtractPath,
                    Arguments = $"-p \"{vpkPath}\" -d \"{extractDir}\" -e \"root\"",
                    WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                await RunProcessAsync(extractPsi, log, ct);

                // Move contents from root folder if present
                string? rootDir = Directory.GetDirectories(extractDir)
                .FirstOrDefault(d => Path.GetFileName(d).Equals("root", StringComparison.OrdinalIgnoreCase));

                if (rootDir != null)
                {
                    log("Organizing extracted files...");
                    foreach (string file in Directory.GetFiles(rootDir, "*", SearchOption.AllDirectories))
                    {
                        ct.ThrowIfCancellationRequested();
                        string relativePath = Path.GetRelativePath(rootDir, file);
                        string destFile = Path.Combine(extractDir, relativePath);
                        Directory.CreateDirectory(Path.GetDirectoryName(destFile) ?? string.Empty);
                        File.Move(file, destFile, true);
                    }
                    Directory.Delete(rootDir, true);
                }
                else
                {
                    log("No 'root' folder found after extract — maybe flat extraction layout.");
                }

                // Basic guard: ensure items_game exists
                string itemsGamePath = Path.Combine(extractDir, "scripts", "items", "items_game.txt");
                if (!File.Exists(itemsGamePath))
                {
                    log("core/dota items_game.txt not found.");
                    return new OperationResult { Success = false, Message = "items_game.txt not found." };
                }

                // Read content for modification
                string content = await File.ReadAllTextAsync(itemsGamePath, ct);

                // Helper local function to get selection safely
                string? GetSelection(string key)
                {
                    if (selections != null && selections.TryGetValue(key, out var val))
                        return val;
                    return null;
                }

                // Replacement routine examples: Weather, Map, Music, Creeps, Siege, Tower, HUD, Versus
                // Each block: fetch remote content (if selection exists and map contains URL),
                // then apply regex replacement if pattern exists.
                ct.ThrowIfCancellationRequested();

                // WEATHER
                var selectedWeather = GetSelection("Weather");
                if (selectedWeather != null && weatherValues.TryGetValue(selectedWeather, out var weatherUrl))
                {
                    log("Fetching Weather...");
                    var weatherContent = await MiscUtilityService.GetStringWithRetryAsync(weatherUrl);
                    if (!string.IsNullOrEmpty(weatherContent))
                    {
                        string weatherPattern = @"(?s)""555""\s*\{[^}]*""prefab""\s*""weather""[^}]*\}.*?(?=""556""|$)";
                        if (Regex.IsMatch(content, weatherPattern))
                        {
                            content = Regex.Replace(content, weatherPattern, weatherContent);
                            log("Weather applied.");
                        }
                    }
                    await Task.Delay(1000, ct);
                }

                ct.ThrowIfCancellationRequested();

                // MAP / TERRAIN
                var selectedMap = GetSelection("Map");
                if (selectedMap != null && mapValues.TryGetValue(selectedMap, out var mapUrl))
                {
                    log("Fetching Terrain...");
                    var mapContent = await MiscUtilityService.GetStringWithRetryAsync(mapUrl);
                    if (!string.IsNullOrEmpty(mapContent))
                    {
                        string mapPattern = @"(?s)""590""\s*\{[^}]*""prefab""\s*""terrain""[^}]*\}.*?(?=""591""|$)";
                        if (Regex.IsMatch(content, mapPattern))
                        {
                            content = Regex.Replace(content, mapPattern, mapContent);
                            log("Terrain applied.");
                        }
                    }
                    await Task.Delay(1000, ct);
                }

                ct.ThrowIfCancellationRequested();

                // MUSIC
                var selectedMusic = GetSelection("Music");
                if (selectedMusic != null && musicValues.TryGetValue(selectedMusic, out var musicUrl))
                {
                    log("Fetching Music...");
                    var musicContent = await MiscUtilityService.GetStringWithRetryAsync(musicUrl);
                    if (!string.IsNullOrEmpty(musicContent))
                    {
                        string musicPattern = @"(?s)""588""\s*\{[^}]*""prefab""\s*""music""[^}]*\}.*?(?=""589""|$)";
                        if (Regex.IsMatch(content, musicPattern))
                        {
                            content = Regex.Replace(content, musicPattern, musicContent);
                            log("Music applied.");
                        }
                    }
                    await Task.Delay(1000, ct);
                }

                ct.ThrowIfCancellationRequested();

                // RADIANT / DIRE CREEPS
                var selectedRadCreep = GetSelection("RadiantCreep");
                if (selectedRadCreep != null && radiantCreepValues.TryGetValue(selectedRadCreep, out var radCreepUrl))
                {
                    log("Fetching Radiant Creeps...");
                    var radContent = await MiscUtilityService.GetStringWithRetryAsync(radCreepUrl);
                    if (!string.IsNullOrEmpty(radContent))
                    {
                        string radPattern = @"(?s)""660""\s*\{[^}]*""prefab""\s*""radiantcreeps""[^}]*\}.*?(?=""661""|$)";
                        if (Regex.IsMatch(content, radPattern))
                        {
                            content = Regex.Replace(content, radPattern, radContent);
                            log("Radiant creeps applied.");
                        }
                    }
                    await Task.Delay(1000, ct);
                }

                ct.ThrowIfCancellationRequested();

                var selectedDireCreep = GetSelection("DireCreep");
                if (selectedDireCreep != null && direCreepValues.TryGetValue(selectedDireCreep, out var direCreepUrl))
                {
                    log("Fetching Dire Creeps...");
                    var direContent = await MiscUtilityService.GetStringWithRetryAsync(direCreepUrl);
                    if (!string.IsNullOrEmpty(direContent))
                    {
                        string direPattern = @"(?s)""661""\s*\{[^}]*""prefab""\s*""direcreeps""[^}]*\}.*?(?=""662""|$)";
                        if (Regex.IsMatch(content, direPattern))
                        {
                            content = Regex.Replace(content, direPattern, direContent);
                            log("Dire creeps applied.");
                        }
                    }
                    await Task.Delay(1000, ct);
                }

                ct.ThrowIfCancellationRequested();

                // SIEGE
                var selRadSiege = GetSelection("RadiantSiege");
                if (selRadSiege != null && radiantSiegeValues.TryGetValue(selRadSiege, out var radSiegeUrl))
                {
                    log("Fetching Radiant Siege...");
                    var radSiegeContent = await MiscUtilityService.GetStringWithRetryAsync(radSiegeUrl);
                    if (!string.IsNullOrEmpty(radSiegeContent))
                    {
                        string radSiegePattern = @"(?s)""34462""\s*\{[^}]*""prefab""\s*""radiantsiegecreeps""[^}]*\}.*?(?=""34463""|$)";
                        if (Regex.IsMatch(content, radSiegePattern))
                        {
                            content = Regex.Replace(content, radSiegePattern, radSiegeContent);
                            log("Radiant siege applied.");
                        }
                    }
                    await Task.Delay(1000, ct);
                }

                ct.ThrowIfCancellationRequested();

                var selDireSiege = GetSelection("DireSiege");
                if (selDireSiege != null && direSiegeValues.TryGetValue(selDireSiege, out var direSiegeUrl))
                {
                    log("Fetching Dire Siege...");
                    var direSiegeContent = await MiscUtilityService.GetStringWithRetryAsync(direSiegeUrl);
                    if (!string.IsNullOrEmpty(direSiegeContent))
                    {
                        string direSiegePattern = @"(?s)""34463""\s*\{[^}]*""prefab""\s*""diresiegecreeps""[^}]*\}.*?(?=""34925""|$)";
                        if (Regex.IsMatch(content, direSiegePattern))
                        {
                            content = Regex.Replace(content, direSiegePattern, direSiegeContent);
                            log("Dire siege applied.");
                        }
                    }
                    await Task.Delay(1000, ct);
                }

                ct.ThrowIfCancellationRequested();

                // TOWERS
                var selRadTower = GetSelection("RadiantTower");
                if (selRadTower != null && radiantTowerValues.TryGetValue(selRadTower, out var radTowerUrl))
                {
                    log("Fetching Radiant Tower...");
                    var radTowerContent = await MiscUtilityService.GetStringWithRetryAsync(radTowerUrl);
                    if (!string.IsNullOrEmpty(radTowerContent))
                    {
                        string radTowerPattern = @"(?s)""677""\s*\{[^}]*""prefab""\s*""radianttowers""[^}]*\}.*?(?=""678""|$)";
                        if (Regex.IsMatch(content, radTowerPattern))
                        {
                            content = Regex.Replace(content, radTowerPattern, radTowerContent);
                            log("Radiant tower applied.");
                        }
                    }
                    await Task.Delay(1000, ct);
                }

                ct.ThrowIfCancellationRequested();

                var selDireTower = GetSelection("DireTower");
                if (selDireTower != null && direTowerValues.TryGetValue(selDireTower, out var direTowerUrl))
                {
                    log("Fetching Dire Tower...");
                    var direTowerContent = await MiscUtilityService.GetStringWithRetryAsync(direTowerUrl);
                    if (!string.IsNullOrEmpty(direTowerContent))
                    {
                        string direTowerPattern = @"(?s)""678""\s*\{[^}]*""prefab""\s*""diretowers""[^}]*\}.*?(?=""679""|$)";
                        if (Regex.IsMatch(content, direTowerPattern))
                        {
                            content = Regex.Replace(content, direTowerPattern, direTowerContent);
                            log("Dire tower applied.");
                        }
                    }
                    await Task.Delay(1000, ct);
                }

                ct.ThrowIfCancellationRequested();

                // HUD
                var selHUD = GetSelection("HUD");
                if (selHUD != null && hudValues.TryGetValue(selHUD, out var hudUrl))
                {
                    log("Fetching HUD...");
                    var hudContent = await MiscUtilityService.GetStringWithRetryAsync(hudUrl);
                    if (!string.IsNullOrEmpty(hudContent))
                    {
                        string hudPattern = @"(?s)""587""\s*\{[^}]*""prefab""\s*""hud_skin""[^}]*\}.*?(?=""588""|$)";
                        if (Regex.IsMatch(content, hudPattern))
                        {
                            content = Regex.Replace(content, hudPattern, hudContent);
                            log("HUD applied.");
                        }
                    }
                    await Task.Delay(1000, ct);
                }

                ct.ThrowIfCancellationRequested();

                // VERSUS
                var selVersus = GetSelection("Versus");
                if (selVersus != null && versusValues.TryGetValue(selVersus, out var versusUrl))
                {
                    log("Fetching Versus Screen...");
                    var versusContent = await MiscUtilityService.GetStringWithRetryAsync(versusUrl);
                    if (!string.IsNullOrEmpty(versusContent))
                    {
                        string versusPattern = @"(?s)""12970""\s*\{[^}]*""prefab""\s*""versus_screen""[^}]*\}.*?(?=""12971""|$)";
                        if (Regex.IsMatch(content, versusPattern))
                        {
                            content = Regex.Replace(content, versusPattern, versusContent);
                            log("Versus applied.");
                        }
                    }
                    await Task.Delay(1000, ct);
                }

                ct.ThrowIfCancellationRequested();

                // RIVER / VIAL handling (complex: delete or download + extract)
                var selRiver = GetSelection("River");
                if (selRiver != null && riverValues.TryGetValue(selRiver, out var riverUrl))
                {
                    var vpkDir = Path.GetDirectoryName(vpkPath);
                    if (vpkDir == null)
                    {
                        log("Failed to determine VPK directory.");
                        return new OperationResult { Success = false, Message = "VPK directory unknown." };
                    }
                    string tempDir = Path.Combine(vpkDir, "_temp");
                    Directory.CreateDirectory(tempDir);

                    if (selRiver == "Default Vial")
                    {
                        // attempt to read extraction.log and delete extracted files
                        log("Disabling Vial...");
                        string logExtraction = Path.Combine(tempDir, "extraction.log");
                        if (File.Exists(logExtraction))
                        {
                            var lines = await File.ReadAllLinesAsync(logExtraction, ct);
                            var filesToDelete = new HashSet<string>(lines.Select(l => l.Replace("Extracted: ", "")));
                            if (Directory.Exists(extractDir))
                            {
                                foreach (var f in Directory.GetFiles(extractDir, "*", SearchOption.AllDirectories))
                                {
                                    ct.ThrowIfCancellationRequested();
                                    var rel = Path.GetRelativePath(extractDir, f);
                                    if (filesToDelete.Contains(rel))
                                        File.Delete(f);
                                }
                                // cleanup empty directories
                                foreach (var d in Directory.GetDirectories(extractDir, "*", SearchOption.AllDirectories).Reverse())
                                {
                                    if (!Directory.GetFiles(d, "*", SearchOption.AllDirectories).Any())
                                        Directory.Delete(d, true);
                                }
                            }
                        }
                    }
                    else
                    {
                        log("Fetching River Vial...");
                        string rarFilePath = Path.Combine(tempDir, "riverVial.rar");
                        using (var response = await MiscUtilityService.GetWithRetryAsync(riverUrl))
                        {
                            if (response == null)
                            {
                                log("River resource not found (404).");
                                // continue (not fatal)
                            }
                            else
                            {
                                using var fs = new FileStream(rarFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
                                await response.Content.CopyToAsync(fs, ct);
                            }
                        }

                        await Task.Delay(1000, ct);

                        // Extract with SharpCompress using password
                        using (var archive = RarArchive.Open(rarFilePath, new ReaderOptions { Password = "muvestein@98" }))
                        {
                            var entries = archive.Entries.Where(e => !e.IsDirectory).ToList();
                            if (!entries.Any())
                            {
                                File.Delete(rarFilePath);
                                log("No valid files found in River Vial archive.");
                                return new OperationResult { Success = false, Message = "Empty River archive." };
                            }

                            foreach (var entry in entries)
                            {
                                ct.ThrowIfCancellationRequested();
                                if (string.IsNullOrEmpty(entry.Key)) continue;

                                string destPath = Path.Combine(tempDir, entry.Key);
                                Directory.CreateDirectory(Path.GetDirectoryName(destPath) ?? string.Empty);
                                entry.WriteToFile(destPath);
                                string logPath = Path.Combine(tempDir, "extraction.log");
                                await File.AppendAllTextAsync(logPath, $"Extracted: {entry.Key}\n", ct);
                            }
                        }

                        File.Delete(rarFilePath);

                        // Copy extracted files to extractDir
                        foreach (var file in Directory.GetFiles(tempDir, "*", SearchOption.AllDirectories))
                        {
                            ct.ThrowIfCancellationRequested();
                            var relativePath = Path.GetRelativePath(tempDir, file);
                            if (Path.GetFileName(file) == "extraction.log") continue;
                            var dest = Path.Combine(extractDir, relativePath);
                            Directory.CreateDirectory(Path.GetDirectoryName(dest) ?? string.Empty);
                            File.Copy(file, dest, true);
                        }

                        // Create missing directories
                        foreach (var dir in Directory.GetDirectories(tempDir, "*", SearchOption.AllDirectories))
                        {
                            var rel = Path.GetRelativePath(tempDir, dir);
                            var destDir = Path.Combine(extractDir, rel);
                            if (!Directory.Exists(destDir))
                                Directory.CreateDirectory(destDir);
                        }

                        // Validate extraction minimally
                        string extractedParticlesDir = Path.Combine(extractDir, "particles");
                        if (!Directory.Exists(extractedParticlesDir))
                        {
                            log("Extraction failed or no valid River Vial folders found.");
                            return new OperationResult { Success = false, Message = "River extraction failed." };
                        }
                        log("River Vial applied.");
                    }
                }

                ct.ThrowIfCancellationRequested();

                // EMBLEMS (copy or delete)
                var selEmblem = GetSelection("Emblem");
                if (selEmblem != null && emblemsValues.TryGetValue(selEmblem, out var emblemUrl))
                {
                    string emblemDir = Path.Combine(extractDir, "particles", "ui_mouseactions");
                    if (selEmblem == "Disable Emblem")
                    {
                        log("Disabling Emblems...");
                        if (Directory.Exists(emblemDir))
                            Directory.Delete(emblemDir, true);
                    }
                    else
                    {
                        Directory.CreateDirectory(emblemDir);
                        log("Fetching Emblems...");
                        var emblemFileName = Path.GetFileName(emblemUrl);
                        var emblemDestPath = Path.Combine(emblemDir, emblemFileName);
                        var emblemContent = await MiscUtilityService.GetByteArrayWithRetryAsync(emblemUrl);
                        if (emblemContent != null)
                        {
                            await File.WriteAllBytesAsync(emblemDestPath, emblemContent, ct);
                            log($"Emblem applied.");
                        }
                        await Task.Delay(1000, ct);
                    }
                }

                ct.ThrowIfCancellationRequested();

                // SHADERS
                var selShader = GetSelection("Shader");
                if (selShader != null && shadersValues.TryGetValue(selShader, out var shaderUrl))
                {
                    string shaderDir = Path.Combine(extractDir, "materials", "dev");
                    if (selShader == "Disable Shader")
                    {
                        log("Disabling Shaders...");
                        if (Directory.Exists(shaderDir))
                            Directory.Delete(shaderDir, true);
                    }
                    else
                    {
                        Directory.CreateDirectory(shaderDir);
                        log("Fetching Shader...");
                        var shaderFileName = Path.GetFileName(shaderUrl);
                        var shaderDestPath = Path.Combine(shaderDir, shaderFileName);
                        var shaderContent = await MiscUtilityService.GetByteArrayWithRetryAsync(shaderUrl);
                        if (shaderContent != null)
                        {
                            await File.WriteAllBytesAsync(shaderDestPath, shaderContent, ct);
                            log($"Shader applied.");
                        }
                        await Task.Delay(1000, ct);
                    }
                }

                ct.ThrowIfCancellationRequested();

                // Write modified items game file back
                await File.WriteAllTextAsync(itemsGamePath, content, ct);

                ct.ThrowIfCancellationRequested();

                // ATK MODIFIER (download/extract or disable)
                var selAtk = GetSelection("AtkModifier");
                if (selAtk != null && atkModifierValues.TryGetValue(selAtk, out var atkUrl))
                {
                    var vpkDir = Path.GetDirectoryName(vpkPath) ?? string.Empty;
                    string tempDir = Path.Combine(vpkDir, "_temp");
                    Directory.CreateDirectory(tempDir);

                    if (selAtk == "Disable Attack Modifier")
                    {
                        log("Disabling Attack Modifier...");
                        string logPath = Path.Combine(tempDir, "extraction.log");
                        if (File.Exists(logPath))
                        {
                            var lines = await File.ReadAllLinesAsync(logPath, ct);
                            var filesToDelete = new HashSet<string>(lines.Select(l => l.Replace("Extracted: ", "")));
                            if (Directory.Exists(extractDir))
                            {
                                foreach (var f in Directory.GetFiles(extractDir, "*", SearchOption.AllDirectories))
                                {
                                    ct.ThrowIfCancellationRequested();
                                    var rel = Path.GetRelativePath(extractDir, f);
                                    if (filesToDelete.Contains(rel))
                                        File.Delete(f);
                                }

                                // Clean up empty directories
                                foreach (var d in Directory.GetDirectories(extractDir, "*", SearchOption.AllDirectories).Reverse())
                                {
                                    var rel = Path.GetRelativePath(extractDir, d);
                                    if (filesToDelete.Contains(rel) && !Directory.GetFiles(d, "*", SearchOption.AllDirectories).Any())
                                        Directory.Delete(d, true);
                                }
                            }
                        }
                    }
                    else
                    {
                        log("Fetching Attack Modifier...");
                        string rarFilePath = Path.Combine(tempDir, "atkModifier.rar");
                        using (var response = await MiscUtilityService.GetWithRetryAsync(atkUrl))
                        {
                            if (response == null)
                            {
                                log("Attack modifier resource not found.");
                            }
                            else
                            {
                                using var fs = new FileStream(rarFilePath, FileMode.Create);
                                await response.Content.CopyToAsync(fs, ct);
                            }
                        }

                        await Task.Delay(1000, ct);

                        using (var archive = RarArchive.Open(rarFilePath, new ReaderOptions { Password = "muvestein@98" }))
                        {
                            var entries = archive.Entries.Where(e => !e.IsDirectory).ToList();
                            if (!entries.Any())
                            {
                                File.Delete(rarFilePath);
                                log("No valid files found in atkModifier archive.");
                            }
                            else
                            {
                                foreach (var entry in entries)
                                {
                                    ct.ThrowIfCancellationRequested();
                                    if (string.IsNullOrEmpty(entry.Key)) continue;
                                    string destPath = Path.Combine(tempDir, entry.Key);
                                    Directory.CreateDirectory(Path.GetDirectoryName(destPath) ?? string.Empty);
                                    entry.WriteToFile(destPath);
                                    string logPath = Path.Combine(tempDir, "extraction.log");
                                    await File.AppendAllTextAsync(logPath, $"Extracted: {entry.Key}\n", ct);
                                }
                            }
                        }

                        File.Delete(rarFilePath);

                        // Copy _temp -> extractDir
                        foreach (var file in Directory.GetFiles(tempDir, "*", SearchOption.AllDirectories))
                        {
                            ct.ThrowIfCancellationRequested();
                            var rel = Path.GetRelativePath(tempDir, file);
                            if (Path.GetFileName(file) == "extraction.log") continue;
                            var dest = Path.Combine(extractDir, rel);
                            Directory.CreateDirectory(Path.GetDirectoryName(dest) ?? string.Empty);
                            File.Copy(file, dest, true);
                        }

                        foreach (var dir in Directory.GetDirectories(tempDir, "*", SearchOption.AllDirectories))
                        {
                            var rel = Path.GetRelativePath(tempDir, dir);
                            var destDir = Path.Combine(extractDir, rel);
                            if (!Directory.Exists(destDir))
                                Directory.CreateDirectory(destDir);
                        }

                        // Validate extraction
                        string kisilevDir = Path.Combine(extractDir, "kisilev_ind", "particles", "modifier_attack");
                        string particlesDir = Path.Combine(extractDir, "particles");
                        if (!Directory.Exists(kisilevDir) && !Directory.Exists(particlesDir))
                        {
                            log("Extraction failed or no valid folders found for attack modifier.");
                            return new OperationResult { Success = false, Message = "Attack modifier extraction failed." };
                        }
                        log("Attack Modifier applied.");
                    }
                }

                ct.ThrowIfCancellationRequested();

                // Inform user and pack with vpk.exe
                log("Please wait... Do not close this app while processing.");
                var packPsi = new ProcessStartInfo
                {
                    FileName = vpkToolPath,
                    Arguments = $"\"{extractDir}\"",
                    WorkingDirectory = Path.GetDirectoryName(vpkPath) ?? AppDomain.CurrentDomain.BaseDirectory,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                await RunProcessAsync(packPsi, log, ct);

                // After pack, replace original vpk if temp found
                var vpkDirectory = Path.GetDirectoryName(vpkPath) ?? string.Empty;
                var tempVpk = Path.Combine(vpkDirectory, "extracted.vpk");
                if (File.Exists(tempVpk))
                {
                    if (File.Exists(vpkPath)) File.Delete(vpkPath);
                    File.Move(tempVpk, vpkPath);
                    Directory.Delete(extractDir, true);

                    // Clean up _temp except extraction.log
                    string tempDir = Path.Combine(vpkDirectory, "_temp");
                    if (Directory.Exists(tempDir))
                    {
                        foreach (var file in Directory.GetFiles(tempDir, "*", SearchOption.AllDirectories))
                        {
                            if (Path.GetFileName(file) != "extraction.log")
                                File.Delete(file);
                        }
                        foreach (var dir in Directory.GetDirectories(tempDir, "*", SearchOption.AllDirectories).Reverse())
                        {
                            if (!Directory.GetFiles(dir, "*", SearchOption.AllDirectories).Any())
                                Directory.Delete(dir, true);
                        }
                        File.SetAttributes(tempDir, FileAttributes.Normal);
                    }

                    // Successful install — no need to double log here
                    return new OperationResult { Success = true, Message = "Done! All mods applied successfully." };
                }
                else
                {
                    log($"Recompilation failed: {tempVpk} not found.");
                    return new OperationResult { Success = false, Message = "Recompile failed: temp vpk missing." };
                }
            }
            catch (OperationCanceledException)
            {
                log("Operation canceled.");
                return new OperationResult { Success = false, Message = "Cancelled by user." };
            }
            catch (Exception ex)
            {
                log($"Error: {ex.Message}");
                return new OperationResult { Success = false, Message = ex.Message, Exception = ex };
            }
        }

        /// <summary>
        /// Runs an external process and logs its output.
        /// </summary>
        /// <param name="psi">ProcessStartInfo configuration.</param>
        /// <param name="log">Logger delegate from UI.</param>
        /// <param name="ct">Cancellation token for stopping the process.</param>
        private static async Task RunProcessAsync(ProcessStartInfo psi, Action<string> log, CancellationToken ct)
        {
            using var proc = new Process { StartInfo = psi };
            proc.Start();
            // Read both streams asynchronously to avoid deadlocks
            var outputTask = proc.StandardOutput.ReadToEndAsync();
            var errorTask = proc.StandardError.ReadToEndAsync();

            await Task.WhenAll(outputTask, errorTask);

            string outText = outputTask.Result;
            string errText = errorTask.Result;
            if (!string.IsNullOrEmpty(outText)) log("Process output...");
            if (!string.IsNullOrEmpty(errText)) log("Process error...");

            proc.WaitForExit();
        }
    }
}
