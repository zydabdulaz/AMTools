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
using ArdysaModsTools.Core.Helpers;

namespace ArdysaModsTools.Core.Services
{
    public class MiscGenerationService
    {
        // --- Dictionaries (trimmed for brevity in this example) ---
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

        

        // Single valid constructor
        public MiscGenerationService() { }

        // ============================================================
        // MAIN ENTRY POINT
        // ============================================================
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
                    return Fail("No target path set.", log);

                targetPath = PathUtility.NormalizeTargetPath(targetPath);
                string vpkPath = PathUtility.GetVpkPath(targetPath);
                if (!File.Exists(vpkPath))
                    return Fail($"VPK file not found at: {vpkPath}", log);

                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string hlExtractPath = Path.Combine(baseDir, "HLExtract.exe");
                string vpkToolPath = Path.Combine(baseDir, "vpk.exe");
                if (!File.Exists(hlExtractPath) || !File.Exists(vpkToolPath))
                    return Fail("Missing required tools (HLExtract.exe / vpk.exe).", log);

                string tempRoot = Path.Combine(Path.GetTempPath(), $"ArdysaMods_{Guid.NewGuid():N}");
                string extractDir = Path.Combine(tempRoot, "extract");
                string buildDir = Path.Combine(tempRoot, "build");
                Directory.CreateDirectory(extractDir);
                Directory.CreateDirectory(buildDir);

                if (!await Step_ExtractVpkAsync(hlExtractPath, vpkPath, extractDir, log, ct))
                    return Fail("Extraction failed.", log);

                if (!await Step_ModifyAsync(vpkPath, extractDir, selections, log, ct))
                    return Fail("Modification failed.", log);

                string? newVpk = await Step_RecompileAsync(vpkToolPath, extractDir, buildDir, tempRoot, log, ct);
                if (newVpk == null)
                    return Fail("Recompile failed: no output.", log);
                await Task.Delay(2000, ct);
                if (!await Step_ReplaceAsync(targetPath, newVpk, log, ct))
                    return Fail("Checking failed.", log);

                await Step_CleanupAsync(tempRoot, log, ct);

                log("All mods successfully applied.");
                return new OperationResult { Success = true, Message = "All mods successfully applied." };
            }
            catch (OperationCanceledException)
            {
                log("Operation canceled.");
                return new OperationResult { Success = false, Message = "Canceled by user." };
            }
            catch (Exception ex)
            {
                log($"Error: {ex.Message}");
                return new OperationResult { Success = false, Message = ex.Message, Exception = ex };
            }
        }

        // ============================================================
        // STEP 1 — Extract
        // ============================================================
        private async Task<bool> Step_ExtractVpkAsync(
            string hlExtractPath, string vpkPath, string extractDir,
            Action<string> log, CancellationToken ct)
        {
            log("Extracting..."); // Step 1

            var psi = new ProcessStartInfo
            {
                FileName = hlExtractPath,
                Arguments = $"-p \"{vpkPath}\" -d \"{extractDir}\" -e \"root\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            await RunProcessAsync(psi, log, ct);

            string rootDir = Path.Combine(extractDir, "root");
            if (Directory.Exists(rootDir))
            {
                foreach (var file in Directory.GetFiles(rootDir, "*", SearchOption.AllDirectories))
                {
                    string relative = Path.GetRelativePath(rootDir, file);
                    string dest = Path.Combine(extractDir, relative);
                    Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
                    File.Move(file, dest, true);
                }
                Directory.Delete(rootDir, true);
            }

            string itemsGamePath = Path.Combine(extractDir, "scripts", "items", "items_game.txt");
            if (!File.Exists(itemsGamePath))
            {
                log("items_game.txt missing after extraction.");
                return false;
            }

            log("Extraction completed.");
            return true;
        }

        // ============================================================
        // STEP 2 — Modify
        // ============================================================
        private async Task<bool> Step_ModifyAsync(
            string vpkPath, string extractDir, Dictionary<string, string> selections,
            Action<string> log, CancellationToken ct)
        {
            log("Applying modifications..."); //Step 2

            string itemsGamePath = Path.Combine(extractDir, "scripts", "items", "items_game.txt");
            if (!File.Exists(itemsGamePath))
            {
                log("core not found.");
                return false;
            }

            string content = await File.ReadAllTextAsync(itemsGamePath, ct);
            string? GetSelection(string key) => selections != null && selections.TryGetValue(key, out var val) ? val : null;

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

            // TERRAIN
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
                    return false;
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
                            log("River resource not found.");
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
                            log("No valid files found.");
                            return false;
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
                        log("Extraction failed.");
                        return false;
                    }
                    log("River Vial applied.");
                }
            }

            ct.ThrowIfCancellationRequested();

            // EMBLEMS (copy or delete)
            var selEmblem = GetSelection("Emblems");
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
                            log("Resource not found.");
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
                            log("No valid files found.");
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
                        log("Extraction failed.");
                        return false;
                    }
                    log("Attack Modifier applied.");
                }
            }

            await File.WriteAllTextAsync(itemsGamePath, content, ct);
            log("Modification completed.");
            return true;
        }

        // ============================================================
        // STEP 3 — Recompile (clean logging version)
        // ============================================================
        private async Task<string?> Step_RecompileAsync(
            string vpkToolPath, string extractDir, string buildDir, string tempRoot,
            Action<string> log, CancellationToken ct)
        {
            log("Please wait... Do not close this App while processing");

            if (string.IsNullOrWhiteSpace(vpkToolPath) || !File.Exists(vpkToolPath))
            {
                log("[Recompile] vpk.exe not found.");
                return null;
            }

            var psi = new ProcessStartInfo
            {
                FileName = vpkToolPath,
                Arguments = $"\"{extractDir}\"",
                WorkingDirectory = buildDir,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            var processOutput = new List<string>();
            var startTime = DateTime.UtcNow;

            try
            {
                log("Processing...");
                await Task.Delay(3500, ct);
                using (var proc = new Process { StartInfo = psi, EnableRaisingEvents = true })
                {
                    proc.OutputDataReceived += (s, e) => { if (e.Data != null) processOutput.Add(e.Data); };
                    proc.ErrorDataReceived += (s, e) => { if (e.Data != null) processOutput.Add($"[ERR] {e.Data}"); };

                    if (!proc.Start())
                    {
                        log("[Recompile] Failed to start vpk.exe process.");
                        return null;
                    }

                    proc.BeginOutputReadLine();
                    proc.BeginErrorReadLine();

                    using (ct.Register(() =>
                    {
                        try { if (!proc.HasExited) proc.Kill(); } catch { }
                    }))
                    {
                        await proc.WaitForExitAsync(ct);
                    }
                }
            }
            catch (Exception ex)
            {
                log($"[Recompile] Error running vpk.exe: {ex.Message}");
                return null;
            }

            if (processOutput.Count > 0)
            {
                foreach (var line in processOutput) ;
            }
            // Allow some filesystem delay for ultra-fast drives
            await Task.Delay(3500, ct);

            // Search for output file (Source 2: folder.vpk)
            string? newVpk = null;
            string parentDir = Path.GetDirectoryName(extractDir) ?? tempRoot;
            string[] searchDirs = { buildDir, extractDir, parentDir, tempRoot };

            for (int i = 0; i < 20 && newVpk == null; i++)
            {
                foreach (var dir in searchDirs.Where(Directory.Exists))
                {
                    var found = Directory.GetFiles(dir, "*.vpk", SearchOption.TopDirectoryOnly)
                        .Where(f => File.GetCreationTimeUtc(f) >= startTime.AddSeconds(-5))
                        .OrderByDescending(File.GetCreationTimeUtc)
                        .FirstOrDefault();
                    if (found != null)
                    {
                        newVpk = found;
                        break;
                    }
                }

                if (newVpk == null)
                    await Task.Delay(3500, ct);
            }

            if (newVpk == null)
            {
                log("[Recompile] Failed — no output VPK found.");
                return null;
            }

            await WaitForFileReady(newVpk, log, ct);
            return newVpk;
        }

        // ============================================================
        // STEP 4 — Replace (move compiled file to _ArdysaMods)
        // ============================================================
        private async Task<bool> Step_ReplaceAsync(
            string targetPath,
            string newVpkDirOrFile,
            Action<string> log,
            CancellationToken ct)
        {

            string modsDir = Path.Combine(targetPath, "game", "_ArdysaMods");
            Directory.CreateDirectory(modsDir);

            bool isFolder = Directory.Exists(newVpkDirOrFile);
            string buildSource = isFolder ? newVpkDirOrFile : Path.GetDirectoryName(newVpkDirOrFile)!;
            string tempRoot = Directory.GetParent(buildSource)?.FullName ?? buildSource;

            string currentVpk = Path.Combine(modsDir, "pak01_dir.vpk");
            string backupVpk = Path.Combine(modsDir, "old_pak01_dir.vpk");

            // Backup old file
            if (File.Exists(currentVpk))
            {
                File.Move(currentVpk, backupVpk, true);
                await Task.Delay(3500, ct);
            }

            // Move compiled files
            var vpkFiles = Directory.GetFiles(buildSource, "*.vpk", SearchOption.TopDirectoryOnly);
            if (vpkFiles.Length == 0)
            {
                return false;
            }

            foreach (var file in vpkFiles)
            {
                string dest = Path.Combine(modsDir, Path.GetFileName(file));
                File.Copy(file, dest, true);
            }

            string newPakPath = Path.Combine(modsDir, "pak01_dir.vpk");
            string altPath = Path.Combine(modsDir, "extract.vpk");
            if (!File.Exists(newPakPath) && File.Exists(altPath))
            {
                File.Move(altPath, newPakPath, true);
            }

            await WaitForFileReady(newPakPath, log, ct);
            await Task.Delay(3500, ct);

            if (File.Exists(newPakPath))
            {
                if (File.Exists(backupVpk)) File.Delete(backupVpk);
            }
            else
            {
                return false;
            }

            try
            {
                if (Directory.Exists(tempRoot))
                {

                    // PROTECT runtime folder
                    if (IsProtectedRuntimeFolder(tempRoot))
                    {
                    }
                    else
                    {
                        // extra safety: ensure folder name starts with ArdysaMods_ OR is inside system temp
                        var folderName = Path.GetFileName(tempRoot) ?? string.Empty;
                        if (folderName.StartsWith("ArdysaMods_", StringComparison.OrdinalIgnoreCase) ||
                            tempRoot.StartsWith(Path.GetTempPath(), StringComparison.OrdinalIgnoreCase))
                        {
                            Directory.Delete(tempRoot, true);
                        }
                        else
                        {
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return true;
        }


        // ============================================================
        // STEP 5 — Cleanup
        // ============================================================
        private async Task Step_CleanupAsync(string tempRoot, Action<string> log, CancellationToken ct)
        {
            try
            {
                await Task.Delay(300, ct);

                if (string.IsNullOrWhiteSpace(tempRoot))
                    return;

                string full = Path.GetFullPath(tempRoot).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);


                if (IsProtectedRuntimeFolder(full))
                {
                    return;
                }

                string folderName = Path.GetFileName(full);

                if (!folderName.StartsWith("ArdysaMods_", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                if (Directory.Exists(full))
                {
                    Directory.Delete(full, true);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception)
            {
            }
        }

        // ============================================================
        // HELPERS
        // ============================================================
        private static async Task RunProcessAsync(ProcessStartInfo psi, Action<string> log, CancellationToken ct)
        {
            using var proc = new Process { StartInfo = psi };
            proc.Start();
            await Task.WhenAll(proc.StandardOutput.ReadToEndAsync(), proc.StandardError.ReadToEndAsync());
            await proc.WaitForExitAsync(ct);
        }
        private bool IsProtectedRuntimeFolder(string path)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path))
                    return false;

                // Normalize path
                string full = Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

                // Lowercase for simple comparisons
                string lower = full.ToLowerInvariant();

                // QUICK CHECK: look for the common combined substrings in various forms
                if (lower.Contains(@"\.net\ardysamodstools\") || lower.Contains(@"/.net/ardysamodstools/"))
                    return true;
                if (lower.EndsWith(@"\.net\ardysamodstools", StringComparison.OrdinalIgnoreCase) ||
                    lower.EndsWith(@"/.net/ardysamodstools", StringComparison.OrdinalIgnoreCase))
                    return true;

                // --- Robust segment check: find ".net" followed by "ArdysaModsTools" in path segments ---
                // Split on both separators
                var segments = full.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries)
                                   .Select(s => s.Trim()).ToArray();

                for (int i = 0; i + 1 < segments.Length; i++)
                {
                    if (string.Equals(segments[i], ".net", StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(segments[i + 1], "ArdysaModsTools", StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }

                // Also protect the base ArdysaModsTools folder itself if the path contains it anywhere
                if (segments.Any(s => string.Equals(s, "ArdysaModsTools", StringComparison.OrdinalIgnoreCase)))
                    return true;

                return false;
            }
            catch
            {
                // If anything strange happens, err on the safe side and protect
                return true;
            }
        }

        private static async Task WaitForFileReady(string path, Action<string> log, CancellationToken ct)
        {
            for (int i = 0; i < 30; i++)
            {
                try
                {
                    if (File.Exists(path))
                    {
                        using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                        return;
                    }
                }
                catch (IOException) { }
                await Task.Delay(300, ct);
            }
        }

        private static OperationResult Fail(string msg, Action<string> log)
        {
            log(msg);
            return new OperationResult { Success = false, Message = msg };
        }
    }
}