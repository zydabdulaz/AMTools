using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Readers;

namespace ArdysaModsTools
{
    public partial class MiscellaneousForm : Form
    {
        private readonly string? _targetPath;
        private readonly Action<string> _log;
        private readonly Action _disableButtons; // Callback to disable Form1 buttons
        private readonly Action _enableButtons; // Callback to enable Form1 buttons
        private static readonly HttpClient httpClient;

        // Static constructor to initialize HttpClient once
        static MiscellaneousForm()
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
            httpClient.Timeout = TimeSpan.FromSeconds(300);
            // Replace with your actual GitHub Personal Access Token
            string githubToken = "TOKEN";
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", githubToken);
        }

        // Dictionaries unchanged
        private readonly Dictionary<string, string> weatherValues = new Dictionary<string, string>
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

        private readonly Dictionary<string, string> mapValues = new Dictionary<string, string>
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

        private readonly Dictionary<string, string> musicValues = new Dictionary<string, string>
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

        private readonly Dictionary<string, string> emblemsValues = new Dictionary<string, string>
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

        private readonly Dictionary<string, string> shadersValues = new Dictionary<string, string>
        {
            { "Disable Shader", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Shaders/Diretide/materials/dev/root.md" },
            { "Aghanim Labyrinth", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Shader/Aghanim/materials/dev/deferred_post_process.vmat_c" },
            { "Diretide", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Shader/Diretide/materials/dev/deferred_post_process.vmat_c" }
        };

        private readonly Dictionary<string, string> atkModifierValues = new Dictionary<string, string>
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

        private readonly Dictionary<string, string> radiantCreepValues = new Dictionary<string, string>
        {
            { "Default Radiant Creep", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Creep/Radiant/Default.txt" },
            { "Cavernite", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Creep/Radiant/Cavernite%20Radiant%20Creeps.txt" },
            { "Crownfall", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Creep/Radiant/Crownfall.txt" },
            { "Nemestice", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Creep/Radiant/Radiant%20Nemestice%20Creeps.txt" },
            { "Reptilian Refuge", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Creep/Radiant/Reptilian%20Refuge%20Radiant%20Creeps.txt" },
            { "Woodland Warbands", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Creep/Radiant/Woodland%20Warbands.txt" }
        };

        private readonly Dictionary<string, string> direCreepValues = new Dictionary<string, string>
        {
            { "Default Dire Creep", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Creep/Dire/Default.txt" },
            { "Cavernite", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Creep/Dire/Cavernite%20Dire%20Creeps.txt" },
            { "Crownfall", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Creep/Dire/Crownfall.txt" },
            { "Nemestice", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Creep/Dire/Dire%20Nemestice%20Creeps.txt" },
            { "Reptilian Refuge", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Creep/Dire/Reptilian%20Refuge%20Dire%20Creeps.txt" },
            { "Woodland Warbands", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Creep/Dire/Woodland%20Warbands.txt" }
        };

        private readonly Dictionary<string, string> direSiegeValues = new Dictionary<string, string>
        {
            { "Default Dire Siege", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Siege/Dire/Default.txt" },
            { "Crownfall", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Siege/Dire/Crownfall.txt" },
            { "Woodland Warbands", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Siege/Dire/Woodland%20Warbands.txt" }
        };

        private readonly Dictionary<string, string> radiantSiegeValues = new Dictionary<string, string>
        {
            { "Default Radiant Siege", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Siege/Radiant/Default.txt" },
            { "Crownfall", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Siege/Radiant/Crownfall.txt" },
            { "Woodland Warbands", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/main/models/root/Siege/Radiant/Woodland%20Warbands.txt" }
        };

        // New dictionaries for HUD, Versus Screen, Vial, Radiant Tower, Dire Tower
        private readonly Dictionary<string, string> hudValues = new Dictionary<string, string>
        {
            { "Default HUD", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/HUD/Default.txt" },
            { "Direstone", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/HUD/Direstone.txt" }, // Replace with actual URL
            { "Portal", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/HUD/Portal.txt" },  // Replace with actual URL
            { "Radiant Ore", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/HUD/Portal.txt" },
            { "Triumph", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/HUD/Radiant%20Ore.txt" },
            { "Valor", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/HUD/Triumph.txt" }
        };

        private readonly Dictionary<string, string> versusValues = new Dictionary<string, string>
        {
            { "Default Versus Screen", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Versus/Default.txt" },
            { "The International 2019 - 1", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Versus/The%20International%202019%20Versus%20Screen%201.txt" }, // Replace with actual URL
            { "The International 2019 - 2", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Versus/The%20International%202019%20Versus%20Screen%202.txt" }, // Replace with actual URL
            { "The International 2020", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Versus/The%20International%202020.txt" }, // Replace with actual URL
            { "Battlepass 2022 - Diretide", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Versus/The%20International%202022%20Diretide.txt" }, // Replace with actual URL
            { "Battlepass 2022 - Nemestice", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Versus/The%20International%202022%20Nemestice.txt" }, // Replace with actual URL
            { "The International 2022 - 1", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Versus/The%20International%202022%20Versus%20Screen%201.txt" }, // Replace with actual URL
            { "The International 2022 - 2", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Versus/The%20International%202022%20Versus%20Screen%202.txt" }, // Replace with actual URL
            { "The International 2024", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Versus/The%20International%202024%20Versus%20Screen.txt" }, // Replace with actual URL
            { "Winter Versus Screen", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Versus/Winter%20Versus%20Screen.txt" }  // Replace with actual URL
        };

        private readonly Dictionary<string, string> riverValues = new Dictionary<string, string>
        {
            { "Default Vial", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/River/root.md" },
            { "Blood Vial", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/River/Blood/Blood.rar" }, // Replace with actual URL
            { "Chrome Vial", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/River/Chrome/Chrome.rar" }, // Replace with actual URL
            { "Dry Vial", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/River/Dry/Dry.rar" }, // Replace with actual URL
            { "Electrifield Vial", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/River/Electrifield/Electrifield.rar" }, // Replace with actual URL
            { "Oil Vial", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/River/Oil/Oil.rar" }  // Replace with actual URL
        };

        private readonly Dictionary<string, string> radiantTowerValues = new Dictionary<string, string>
        {
            { "Default Radiant Tower", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Tower/Radiant/Default.txt" },
            { "Declaration of the Divine Light", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Tower/Radiant/Declaration%20of%20the%20Divine%20Light%20Radiant%20Towers.txt" }, // Replace with actual URL
            { "Grasp of the Elder Gods", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Tower/Radiant/Grasp%20of%20the%20Elder%20Gods%20-%20Radiant%20Towers.txt" }, // Replace with actual URL
            { "Guardians of the Lost Path", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Tower/Radiant/Guardians%20of%20the%20Lost%20Path%20Radiant%20Towers.txt" }, // Replace with actual URL
            { "Stoneclaw Scavengers", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Tower/Radiant/Stoneclaw%20Scavengers%20Radiant%20Towers.txt" }, // Replace with actual URL
            { "The Eyes of Avilliva", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Tower/Radiant/The%20Eyes%20of%20Avilliva%20-%20Radiant%20Towers.txt" }  // Replace with actual URL
        };

        private readonly Dictionary<string, string> direTowerValues = new Dictionary<string, string>
        {
            { "Default Dire Tower", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Tower/Dire/Default.txt" },
            { "Declaration of the Divine Shadow", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Tower/Dire/Declaration%20of%20the%20Divine%20Shadow%20Dire%20Towers.txt" }, // Replace with actual URL
            { "Grasp of the Elder Gods", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Tower/Dire/Grasp%20of%20the%20Elder%20Gods%20-%20Dire%20Towers.txt" }, // Replace with actual URL
            { "Guardians of the Lost Path", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Tower/Dire/Guardians%20of%20the%20Lost%20Path%20Dire%20Towers.txt" }, // Replace with actual URL
            { "Stoneclaw Scavengers", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Tower/Dire/Stoneclaw%20Scavengers%20Dire%20Towers.txt" }, // Replace with actual URL
            { "The Gaze of Scree'Auk", "https://raw.githubusercontent.com/Anneardysa/ArdysaModsTools/refs/heads/main/models/root/Tower/Dire/The%20Gaze%20of%20Scree'Auk%20-%20Dire%20Towers.txt" }  // Replace with actual URL
        };

        public MiscellaneousForm(string? targetPath, Action<string> log, RichTextBox consoleLogBox, Action disableButtons, Action enableButtons)
        {
            _targetPath = targetPath;
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _disableButtons = disableButtons ?? throw new ArgumentNullException(nameof(disableButtons));
            _enableButtons = enableButtons ?? throw new ArgumentNullException(nameof(enableButtons));
            InitializeComponent();

            // Wire up button click events
            LoadPreset.Click += (sender, e) => LoadPreset_Click(sender!, e);
            SavePreset.Click += (sender, e) => SavePreset_Click(sender!, e);

            WeatherBox.Items.AddRange(weatherValues.Keys.ToArray());
            MapBox.Items.AddRange(mapValues.Keys.ToArray());
            MusicBox.Items.AddRange(musicValues.Keys.ToArray());
            EmblemsBox.Items.AddRange(emblemsValues.Keys.ToArray());
            ShaderBox.Items.AddRange(shadersValues.Keys.ToArray());
            atkModifierBox.Items.AddRange(atkModifierValues.Keys.ToArray());
            RadiantCreepBox.Items.AddRange(radiantCreepValues.Keys.ToArray());
            DireCreepBox.Items.AddRange(direCreepValues.Keys.ToArray());
            RadiantSiegeBox.Items.AddRange(radiantSiegeValues.Keys.ToArray());
            DireSiegeBox.Items.AddRange(direSiegeValues.Keys.ToArray());
            HUDBox.Items.AddRange(hudValues.Keys.ToArray());
            VersusBox.Items.AddRange(versusValues.Keys.ToArray());
            RiverBox.Items.AddRange(riverValues.Keys.ToArray());
            RadiantTowerBox.Items.AddRange(radiantTowerValues.Keys.ToArray());
            DireTowerBox.Items.AddRange(direTowerValues.Keys.ToArray());

            // Set default values initially
            WeatherBox.SelectedIndex = 0;
            MapBox.SelectedIndex = 0;
            MusicBox.SelectedIndex = 0;
            EmblemsBox.SelectedIndex = 0;
            ShaderBox.SelectedIndex = 0;
            atkModifierBox.SelectedIndex = 0;
            RadiantCreepBox.SelectedIndex = 0;
            DireCreepBox.SelectedIndex = 0;
            RadiantSiegeBox.SelectedIndex = 0;
            DireSiegeBox.SelectedIndex = 0;
            HUDBox.SelectedIndex = 0;
            VersusBox.SelectedIndex = 0;
            RiverBox.SelectedIndex = 0;
            RadiantTowerBox.SelectedIndex = 0;
            DireTowerBox.SelectedIndex = 0;
        }

        private void LoadPreset_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
                openFileDialog.Title = "Load Preset";
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); // Default to Documents folder

                if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        string jsonContent = File.ReadAllText(openFileDialog.FileName);
                        var preset = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent);

                        if (preset != null)
                        {
                            WeatherBox.SelectedItem = preset.ContainsKey("Weather") ? preset["Weather"] : WeatherBox.Items[0]?.ToString();
                            MapBox.SelectedItem = preset.ContainsKey("Map") ? preset["Map"] : MapBox.Items[0]?.ToString();
                            MusicBox.SelectedItem = preset.ContainsKey("Music") ? preset["Music"] : MusicBox.Items[0]?.ToString();
                            EmblemsBox.SelectedItem = preset.ContainsKey("Emblem") ? preset["Emblem"] : EmblemsBox.Items[0]?.ToString();
                            ShaderBox.SelectedItem = preset.ContainsKey("Shader") ? preset["Shader"] : ShaderBox.Items[0]?.ToString();
                            atkModifierBox.SelectedItem = preset.ContainsKey("AtkModifier") ? preset["AtkModifier"] : atkModifierBox.Items[0]?.ToString();
                            RadiantCreepBox.SelectedItem = preset.ContainsKey("RadiantCreep") ? preset["RadiantCreep"] : RadiantCreepBox.Items[0]?.ToString();
                            DireCreepBox.SelectedItem = preset.ContainsKey("DireCreep") ? preset["DireCreep"] : DireCreepBox.Items[0]?.ToString();
                            RadiantSiegeBox.SelectedItem = preset.ContainsKey("RadiantSiege") ? preset["RadiantSiege"] : RadiantSiegeBox.Items[0]?.ToString();
                            DireSiegeBox.SelectedItem = preset.ContainsKey("DireSiege") ? preset["DireSiege"] : DireSiegeBox.Items[0]?.ToString();
                            HUDBox.SelectedItem = preset.ContainsKey("HUD") ? preset["HUD"] : HUDBox.Items[0]?.ToString();
                            VersusBox.SelectedItem = preset.ContainsKey("Versus") ? preset["Versus"] : VersusBox.Items[0]?.ToString();
                            RiverBox.SelectedItem = preset.ContainsKey("River") ? preset["River"] : RiverBox.Items[0]?.ToString();
                            RadiantTowerBox.SelectedItem = preset.ContainsKey("RadiantTower") ? preset["RadiantTower"] : RadiantTowerBox.Items[0]?.ToString();
                            DireTowerBox.SelectedItem = preset.ContainsKey("DireTower") ? preset["DireTower"] : DireTowerBox.Items[0]?.ToString();

                            SafeLog($"Preset loaded from {openFileDialog.FileName}");
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                else
                {
                }
            }
        }

        private void SavePreset_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
                saveFileDialog.Title = "Save Preset";
                saveFileDialog.DefaultExt = "json";
                saveFileDialog.FileName = "Custom.json";
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); // Default to Documents folder

                if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        var preset = new Dictionary<string, string>
                {
                    { "Weather", WeatherBox.SelectedItem?.ToString() ?? "" },
                    { "Map", MapBox.SelectedItem?.ToString() ?? "" },
                    { "Music", MusicBox.SelectedItem?.ToString() ?? "" },
                    { "Emblem", EmblemsBox.SelectedItem?.ToString() ?? "" },
                    { "Shader", ShaderBox.SelectedItem?.ToString() ?? "" },
                    { "AtkModifier", atkModifierBox.SelectedItem?.ToString() ?? "" },
                    { "RadiantCreep", RadiantCreepBox.SelectedItem?.ToString() ?? "" },
                    { "DireCreep", DireCreepBox.SelectedItem?.ToString() ?? "" },
                    { "RadiantSiege", RadiantSiegeBox.SelectedItem?.ToString() ?? "" },
                    { "DireSiege", DireSiegeBox.SelectedItem?.ToString() ?? "" },
                    { "HUD", HUDBox.SelectedItem?.ToString() ?? "" },
                    { "Versus", VersusBox.SelectedItem?.ToString() ?? "" },
                    { "River", RiverBox.SelectedItem?.ToString() ?? "" },
                    { "RadiantTower", RadiantTowerBox.SelectedItem?.ToString() ?? "" },
                    { "DireTower", DireTowerBox.SelectedItem?.ToString() ?? "" }
                };

                        string jsonContent = System.Text.Json.JsonSerializer.Serialize(preset, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                        File.WriteAllText(saveFileDialog.FileName, jsonContent);

                        SafeLog($"Preset saved to {saveFileDialog.FileName}");
                    }
                    catch (Exception)
                    {
                    }
                }
                else
                {
                }
            }
        }

        private void DisableFormControls()
        {
            // Disable all ComboBoxes
            WeatherBox.Enabled = false;
            MapBox.Enabled = false;
            MusicBox.Enabled = false;
            EmblemsBox.Enabled = false;
            ShaderBox.Enabled = false;
            atkModifierBox.Enabled = false;
            RadiantCreepBox.Enabled = false;
            DireCreepBox.Enabled = false;
            RadiantSiegeBox.Enabled = false;
            DireSiegeBox.Enabled = false;
            HUDBox.Enabled = false;
            VersusBox.Enabled = false;
            RiverBox.Enabled = false;
            RadiantTowerBox.Enabled = false;
            DireTowerBox.Enabled = false;

            // Disable all buttons on the form (including GenerateButton)
            foreach (Control control in this.Controls)
            {
                if (control is Button button)
                {
                    button.Enabled = false;
                }
            }
        }

        private void EnableFormControls()
        {
            // Enable all ComboBoxes
            WeatherBox.Enabled = true;
            MapBox.Enabled = true;
            MusicBox.Enabled = true;
            EmblemsBox.Enabled = true;
            ShaderBox.Enabled = true;
            atkModifierBox.Enabled = true;
            RadiantCreepBox.Enabled = true;
            DireCreepBox.Enabled = true;
            RadiantSiegeBox.Enabled = true;
            DireSiegeBox.Enabled = true;
            HUDBox.Enabled = true;
            VersusBox.Enabled = true;
            RiverBox.Enabled = true;
            RadiantTowerBox.Enabled = true;
            DireTowerBox.Enabled = true;

            // Enable all buttons on the form
            foreach (Control control in this.Controls)
            {
                if (control is Button button)
                {
                    button.Enabled = true;
                }
            }
        }

        private async void MiscellaneousForm_Load(object sender, EventArgs e)
        {
            // Restore saved values from vuex.log in the system temp directory if it exists
            string logPath = Path.Combine(Path.GetTempPath(), "vuex.log");
            if (File.Exists(logPath))
            {
                try
                {
                    string[] lines = await File.ReadAllLinesAsync(logPath);
                    Dictionary<string, string> savedValues = lines
                        .Select(line => line.Split('='))
                        .Where(parts => parts.Length == 2)
                        .ToDictionary(parts => parts[0], parts => parts[1]);

                    // Update ComboBox selections on the UI thread
                    WeatherBox.SelectedItem = savedValues.ContainsKey("Weather") ? savedValues["Weather"] : WeatherBox.Items[0]?.ToString();
                    MapBox.SelectedItem = savedValues.ContainsKey("Map") ? savedValues["Map"] : MapBox.Items[0]?.ToString();
                    MusicBox.SelectedItem = savedValues.ContainsKey("Music") ? savedValues["Music"] : MusicBox.Items[0]?.ToString();
                    EmblemsBox.SelectedItem = savedValues.ContainsKey("Emblem") ? savedValues["Emblem"] : EmblemsBox.Items[0]?.ToString();
                    ShaderBox.SelectedItem = savedValues.ContainsKey("Shader") ? savedValues["Shader"] : ShaderBox.Items[0]?.ToString();
                    atkModifierBox.SelectedItem = savedValues.ContainsKey("AtkModifier") ? savedValues["AtkModifier"] : atkModifierBox.Items[0]?.ToString();
                    RadiantCreepBox.SelectedItem = savedValues.ContainsKey("RadiantCreep") ? savedValues["RadiantCreep"] : RadiantCreepBox.Items[0]?.ToString();
                    DireCreepBox.SelectedItem = savedValues.ContainsKey("DireCreep") ? savedValues["DireCreep"] : DireCreepBox.Items[0]?.ToString();
                    RadiantSiegeBox.SelectedItem = savedValues.ContainsKey("RadiantSiege") ? savedValues["RadiantSiege"] : RadiantSiegeBox.Items[0]?.ToString();
                    DireSiegeBox.SelectedItem = savedValues.ContainsKey("DireSiege") ? savedValues["DireSiege"] : DireSiegeBox.Items[0]?.ToString();
                    HUDBox.SelectedItem = savedValues.ContainsKey("HUD") ? savedValues["HUD"] : HUDBox.Items[0]?.ToString();
                    VersusBox.SelectedItem = savedValues.ContainsKey("Versus") ? savedValues["Versus"] : VersusBox.Items[0]?.ToString();
                    RiverBox.SelectedItem = savedValues.ContainsKey("River") ? savedValues["River"] : RiverBox.Items[0]?.ToString();
                    RadiantTowerBox.SelectedItem = savedValues.ContainsKey("RadiantTower") ? savedValues["RadiantTower"] : RadiantTowerBox.Items[0]?.ToString();
                    DireTowerBox.SelectedItem = savedValues.ContainsKey("DireTower") ? savedValues["DireTower"] : DireTowerBox.Items[0]?.ToString();
                }
                catch (Exception)
                {
                    // Defaults are already set in the constructor
                }
            }
            // If vuex.log doesn't exist, defaults are already set in the constructor
        }

        private void SafeLog(string message)
        {
            try
            {
                // Update ConsoleLogBox on the UI thread
                if (ConsoleLogBox.InvokeRequired)
                {
                    ConsoleLogBox.Invoke(new Action<string>(msg =>
                    {
                        ConsoleLogBox.AppendText($"{DateTime.Now:HH:mm:ss} - {msg}\n");
                        ConsoleLogBox.ScrollToCaret();
                    }), message);
                }
                else
                {
                    ConsoleLogBox.AppendText($"{DateTime.Now:HH:mm:ss} - {message}\n");
                    ConsoleLogBox.ScrollToCaret();
                }
            }
            catch (Exception)
            {
                // Fallback to file logging if UI update fails
                File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mod_log.txt"),
                    $"{DateTime.Now:HH:mm:ss} - {message}\n");
            }
        }

        // Helper method to make HTTP GET requests with retry logic for 429 errors, timeouts, and 404 handling
        private async Task<HttpResponseMessage?> GetWithRetryAsync(string url, int maxRetries = 3)
        {
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    HttpResponseMessage response = await httpClient.GetAsync(url);
                    if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests) // 429
                    {
                        string retryAfterHeader = response.Headers.GetValues("Retry-After")?.FirstOrDefault() ?? "60";
                        int retryAfterSeconds = int.TryParse(retryAfterHeader, out int seconds) ? seconds : 60;
                        await Task.Delay(retryAfterSeconds * 1000);
                        continue;
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.NotFound) // 404
                    {
                        return null; // Return null to indicate the resource was not found
                    }
                    response.EnsureSuccessStatusCode();
                    return response;
                }
                catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    string retryAfterHeader = ex.Message.Contains("Retry-After") ? "60" : "60"; // Fallback if header parsing fails
                    int retryAfterSeconds = int.TryParse(retryAfterHeader, out int seconds) ? seconds : 60;
                    await Task.Delay(retryAfterSeconds * 1000);
                    if (attempt == maxRetries)
                    {
                        throw; // Rethrow if max retries exceeded
                    }
                }
                catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null; // Return null to indicate the resource was not found
                }
                catch (TaskCanceledException ex) when (ex.Message.Contains("Timeout"))
                {
                    if (attempt == maxRetries)
                    {
                        throw; // Rethrow if max retries exceeded
                    }
                    await Task.Delay(5000); // Wait 5 seconds before retrying
                }
                catch (Exception)
                {
                    throw; // Rethrow unexpected errors for debugging
                }
            }
            throw new HttpRequestException("Failed to complete request after maximum retries.");
        }

        // Helper method to get string content with retry and delay
        private async Task<string?> GetStringWithRetryAsync(string url)
        {
            using (var response = await GetWithRetryAsync(url))
            {
                if (response == null) return null; // Resource not found
                return await response.Content.ReadAsStringAsync();
            }
        }

        // Helper method to get byte array content with retry and delay
        private async Task<byte[]?> GetByteArrayWithRetryAsync(string url)
        {
            using (var response = await GetWithRetryAsync(url))
            {
                if (response == null) return null; // Resource not found
                return await response.Content.ReadAsByteArrayAsync();
            }
        }

        private async void GenerateButton_Click(object sender, EventArgs e)
        {
            _disableButtons(); // Disable Form1 buttons during the process
            DisableFormControls(); // Disable all buttons and ComboBoxes on MiscellaneousForm

            try
            {
                // Run the generation process in the background
                await Task.Run(() => PerformGeneration());

                // Save selected values to vuex.log in the system temp directory
                string? selectedWeather = WeatherBox.SelectedItem?.ToString();
                string? selectedMap = MapBox.SelectedItem?.ToString();
                string? selectedMusic = MusicBox.SelectedItem?.ToString();
                string? selectedEmblem = EmblemsBox.SelectedItem?.ToString();
                string? selectedShader = ShaderBox.SelectedItem?.ToString();
                string? selectedAtkModifier = atkModifierBox.SelectedItem?.ToString();
                string? selectedRadiantCreep = RadiantCreepBox.SelectedItem?.ToString();
                string? selectedDireCreep = DireCreepBox.SelectedItem?.ToString();
                string? selectedRadiantSiege = RadiantSiegeBox.SelectedItem?.ToString();
                string? selectedDireSiege = DireSiegeBox.SelectedItem?.ToString();
                string? selectedHUD = HUDBox.SelectedItem?.ToString();
                string? selectedVersus = VersusBox.SelectedItem?.ToString();
                string? selectedRiver = RiverBox.SelectedItem?.ToString();
                string? selectedRadiantTower = RadiantTowerBox.SelectedItem?.ToString();
                string? selectedDireTower = DireTowerBox.SelectedItem?.ToString();

                string logPath = Path.Combine(Path.GetTempPath(), "vuex.log");
                using (var writer = new StreamWriter(logPath))
                {
                    if (selectedWeather != null) await writer.WriteLineAsync($"Weather={selectedWeather}");
                    if (selectedMap != null) await writer.WriteLineAsync($"Map={selectedMap}");
                    if (selectedMusic != null) await writer.WriteLineAsync($"Music={selectedMusic}");
                    if (selectedEmblem != null) await writer.WriteLineAsync($"Emblem={selectedEmblem}");
                    if (selectedShader != null) await writer.WriteLineAsync($"Shader={selectedShader}");
                    if (selectedAtkModifier != null) await writer.WriteLineAsync($"AtkModifier={selectedAtkModifier}");
                    if (selectedRadiantCreep != null) await writer.WriteLineAsync($"RadiantCreep={selectedRadiantCreep}");
                    if (selectedDireCreep != null) await writer.WriteLineAsync($"DireCreep={selectedDireCreep}");
                    if (selectedRadiantSiege != null) await writer.WriteLineAsync($"RadiantSiege={selectedRadiantSiege}");
                    if (selectedDireSiege != null) await writer.WriteLineAsync($"DireSiege={selectedDireSiege}");
                    if (selectedHUD != null) await writer.WriteLineAsync($"HUD={selectedHUD}");
                    if (selectedVersus != null) await writer.WriteLineAsync($"Versus={selectedVersus}");
                    if (selectedRiver != null) await writer.WriteLineAsync($"River={selectedRiver}");
                    if (selectedRadiantTower != null) await writer.WriteLineAsync($"RadiantTower={selectedRadiantTower}");
                    if (selectedDireTower != null) await writer.WriteLineAsync($"DireTower={selectedDireTower}");
                }
            }
            catch (Exception ex)
            {
                SafeLog($"Error saving vuex.log: {ex.Message}");
            }
            finally
            {
                // Ensure UI thread execution for enabling buttons and closing the form
                this.BeginInvoke((Action)(() =>
                {
                    _enableButtons(); // Re-enable Form1 buttons
                    EnableFormControls(); // Re-enable all buttons and ComboBoxes on MiscellaneousForm
                }));
            }
        }

        private void CopyDirectory(string sourceDir, string destDir)
        {
            if (!Directory.Exists(destDir)) Directory.CreateDirectory(destDir);
            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string destFile = Path.Combine(destDir, Path.GetFileName(file));
                File.Copy(file, destFile, true);
            }
            foreach (string dir in Directory.GetDirectories(sourceDir))
            {
                string destSubDir = Path.Combine(destDir, Path.GetFileName(dir));
                CopyDirectory(dir, destSubDir);
            }
        }

        private async Task PerformGeneration()
        {
            try
            {
                if (string.IsNullOrEmpty(_targetPath))
                {
                    SafeLog("No target path set.");
                    return;
                }

                string vpkPath = Path.Combine(_targetPath, "game", "_ArdysaMods", "pak01_dir.vpk");
                if (!File.Exists(vpkPath))
                {
                    SafeLog($"VPK file not found at: {vpkPath}");
                    return;
                }

                string extractDir = Path.Combine(_targetPath, "game", "_ArdysaMods", "extracted");
                string hlExtractPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HLExtract.exe");
                string vpkToolPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "vpk.exe");

                SafeLog("Checking tools...");
                if (!File.Exists(hlExtractPath))
                {
                    SafeLog("HLExtract.exe is missing or not found. Re-download the tool.");
                    return;
                }
                if (!File.Exists(vpkToolPath))
                {
                    SafeLog("vpk.exe is missing or not found. Re-download the tool.");
                    return;
                }

                string? selectedWeather = null;
                string? selectedMap = null;
                string? selectedMusic = null;
                string? selectedEmblem = null;
                string? selectedShader = null;
                string? selectedAtkModifier = null;
                string? selectedRadiantCreep = null;
                string? selectedDireCreep = null;
                string? selectedDireSiege = null;
                string? selectedRadiantSiege = null;
                string? selectedHUD = null;
                string? selectedVersus = null;
                string? selectedRiver = null;
                string? selectedRadiantTower = null;
                string? selectedDireTower = null;

                // Safely access UI elements on the UI thread
                if (InvokeRequired)
                {
                    Invoke(new Action(() =>
                    {
                        selectedWeather = WeatherBox.SelectedItem?.ToString();
                        selectedMap = MapBox.SelectedItem?.ToString();
                        selectedMusic = MusicBox.SelectedItem?.ToString();
                        selectedEmblem = EmblemsBox.SelectedItem?.ToString();
                        selectedShader = ShaderBox.SelectedItem?.ToString();
                        selectedRadiantCreep = RadiantCreepBox.SelectedItem?.ToString();
                        selectedDireCreep = DireCreepBox.SelectedItem?.ToString();
                        selectedDireSiege = DireSiegeBox.SelectedItem?.ToString();
                        selectedRadiantSiege = RadiantSiegeBox.SelectedItem?.ToString();
                        selectedAtkModifier = atkModifierBox.SelectedItem?.ToString();
                        selectedHUD = HUDBox.SelectedItem?.ToString();
                        selectedVersus = VersusBox.SelectedItem?.ToString();
                        selectedRiver = RiverBox.SelectedItem?.ToString();
                        selectedRadiantTower = RadiantTowerBox.SelectedItem?.ToString();
                        selectedDireTower = DireTowerBox.SelectedItem?.ToString();
                    }));
                }
                else
                {
                    selectedWeather = WeatherBox.SelectedItem?.ToString();
                    selectedMap = MapBox.SelectedItem?.ToString();
                    selectedMusic = MusicBox.SelectedItem?.ToString();
                    selectedEmblem = EmblemsBox.SelectedItem?.ToString();
                    selectedShader = ShaderBox.SelectedItem?.ToString();
                    selectedRadiantCreep = RadiantCreepBox.SelectedItem?.ToString();
                    selectedDireCreep = DireCreepBox.SelectedItem?.ToString();
                    selectedDireSiege = DireSiegeBox.SelectedItem?.ToString();
                    selectedRadiantSiege = RadiantSiegeBox.SelectedItem?.ToString();
                    selectedAtkModifier = atkModifierBox.SelectedItem?.ToString();
                    selectedHUD = HUDBox.SelectedItem?.ToString();
                    selectedVersus = VersusBox.SelectedItem?.ToString();
                    selectedRiver = RiverBox.SelectedItem?.ToString();
                    selectedRadiantTower = RadiantTowerBox.SelectedItem?.ToString();
                    selectedDireTower = DireTowerBox.SelectedItem?.ToString();
                }

                SafeLog($"Weather: {selectedWeather}");
                SafeLog($"Terrain: {selectedMap}");
                SafeLog($"Music: {selectedMusic}");
                SafeLog($"Emblems: {selectedEmblem}");
                SafeLog($"Shaders: {selectedShader}");
                SafeLog($"Radiant Creeps: {selectedRadiantCreep}");
                SafeLog($"Dire Creeps: {selectedDireCreep}");
                SafeLog($"Dire Siege: {selectedDireSiege}");
                SafeLog($"Radiant Siege: {selectedRadiantSiege}");
                SafeLog($"Attack Modifier: {selectedAtkModifier}");
                SafeLog($"HUD: {selectedHUD}");
                SafeLog($"Versus: {selectedVersus}");
                SafeLog($"River: {selectedRiver}");
                SafeLog($"Radiant Tower: {selectedRadiantTower}");
                SafeLog($"Dire Tower: {selectedDireTower}");
                SafeLog("Please wait...");

                if (Directory.Exists(extractDir))
                {
                    Directory.Delete(extractDir, true);
                }
                Directory.CreateDirectory(extractDir);

                ProcessStartInfo extractPsi = new ProcessStartInfo
                {
                    FileName = hlExtractPath,
                    Arguments = $"-p \"{vpkPath}\" -d \"{extractDir}\" -e \"root\"",
                    WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (Process extractProcess = new Process { StartInfo = extractPsi })
                {
                    extractProcess.Start();
                    string output = extractProcess.StandardOutput.ReadToEnd();
                    string error = extractProcess.StandardError.ReadToEnd();
                    extractProcess.WaitForExit();

                    if (!string.IsNullOrEmpty(output)) SafeLog($"Extract output...");
                    if (!string.IsNullOrEmpty(error)) SafeLog($"Extract error...");

                    if (extractProcess.ExitCode != 0)
                    {
                        SafeLog($"Extraction failed with code {extractProcess.ExitCode}.");
                        return;
                    }
                }

                string rootDir = Path.Combine(extractDir, "root");
                if (Directory.Exists(rootDir))
                {
                    SafeLog("Moving contents...");
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
                    SafeLog("No 'root' folder found.");
                    return;
                }

                string itemsGamePath = Path.Combine(extractDir, "scripts", "items", "items_game.txt");
                if (!File.Exists(itemsGamePath))
                {
                    SafeLog("core/dota not found.");
                    return;
                }

                SafeLog("Modifying core/dota...");
                string content = File.ReadAllText(itemsGamePath);

                if (selectedWeather != null && weatherValues.TryGetValue(selectedWeather, out string? weatherValue))
                {
                    SafeLog("Fetching Weather...");
                    string? weatherContent = await GetStringWithRetryAsync(weatherValue);
                    if (weatherContent != null)
                    {
                        string weatherPattern = @"(?s)""555""\s*\{[^}]*""prefab""\s*""weather""[^}]*\}.*?(?=""556""|$)";
                        if (Regex.IsMatch(content, weatherPattern))
                        {
                            content = Regex.Replace(content, weatherPattern, weatherContent);
                        }
                    }
                    await Task.Delay(1000); // Delay to avoid rate limiting
                }

                if (selectedMap != null && mapValues.TryGetValue(selectedMap, out string? mapValue))
                {
                    SafeLog("Fetching Terrain...");
                    string? mapContent = await GetStringWithRetryAsync(mapValue);
                    if (mapContent != null)
                    {
                        string mapPattern = @"(?s)""590""\s*\{[^}]*""prefab""\s*""terrain""[^}]*\}.*?(?=""591""|$)";
                        if (Regex.IsMatch(content, mapPattern))
                        {
                            content = Regex.Replace(content, mapPattern, mapContent);
                        }
                    }
                    await Task.Delay(1000); // Delay to avoid rate limiting
                }

                if (selectedMusic != null && musicValues.TryGetValue(selectedMusic, out string? musicValue))
                {
                    SafeLog("Fetching Music...");
                    string? musicContent = await GetStringWithRetryAsync(musicValue);
                    if (musicContent != null)
                    {
                        string musicPattern = @"(?s)""588""\s*\{[^}]*""prefab""\s*""music""[^}]*\}.*?(?=""589""|$)";
                        if (Regex.IsMatch(content, musicPattern))
                        {
                            content = Regex.Replace(content, musicPattern, musicContent);
                        }
                    }
                    await Task.Delay(1000); // Delay to avoid rate limiting
                }

                if (selectedRadiantCreep != null && radiantCreepValues.TryGetValue(selectedRadiantCreep, out string? radiantCreepValue))
                {
                    SafeLog("Fetching Radiant Creeps...");
                    string? radiantCreepContent = await GetStringWithRetryAsync(radiantCreepValue);
                    if (radiantCreepContent != null)
                    {
                        string radiantCreepPattern = @"(?s)""660""\s*\{[^}]*""prefab""\s*""radiantcreeps""[^}]*\}.*?(?=""661""|$)";
                        if (Regex.IsMatch(content, radiantCreepPattern))
                        {
                            content = Regex.Replace(content, radiantCreepPattern, radiantCreepContent);
                        }
                    }
                    await Task.Delay(1000); // Delay to avoid rate limiting
                }

                if (selectedDireCreep != null && direCreepValues.TryGetValue(selectedDireCreep, out string? direCreepValue))
                {
                    SafeLog("Fetching Dire Creeps...");
                    string? direCreepContent = await GetStringWithRetryAsync(direCreepValue);
                    if (direCreepContent != null)
                    {
                        string direCreepPattern = @"(?s)""661""\s*\{[^}]*""prefab""\s*""direcreeps""[^}]*\}.*?(?=""662""|$)";
                        if (Regex.IsMatch(content, direCreepPattern))
                        {
                            content = Regex.Replace(content, direCreepPattern, direCreepContent);
                        }
                    }
                    await Task.Delay(1000); // Delay to avoid rate limiting
                }

                if (selectedRadiantSiege != null && radiantSiegeValues.TryGetValue(selectedRadiantSiege, out string? radiantSiegeValue))
                {
                    SafeLog("Fetching Radiant Siege...");
                    string? radiantSiegeContent = await GetStringWithRetryAsync(radiantSiegeValue);
                    if (radiantSiegeContent != null)
                    {
                        string radiantSiegePattern = @"(?s)""34462""\s*\{[^}]*""prefab""\s*""radiantsiegecreeps""[^}]*\}.*?(?=""34463""|$)";
                        if (Regex.IsMatch(content, radiantSiegePattern))
                        {
                            content = Regex.Replace(content, radiantSiegePattern, radiantSiegeContent);
                        }
                    }
                    await Task.Delay(1000); // Delay to avoid rate limiting
                }

                if (selectedDireSiege != null && direSiegeValues.TryGetValue(selectedDireSiege, out string? direSiegeValue))
                {
                    SafeLog("Fetching Dire Siege...");
                    string? direSiegeContent = await GetStringWithRetryAsync(direSiegeValue);
                    if (direSiegeContent != null)
                    {
                        string direSiegePattern = @"(?s)""34463""\s*\{[^}]*""prefab""\s*""diresiegecreeps""[^}]*\}.*?(?=""34925""|$)";
                        if (Regex.IsMatch(content, direSiegePattern))
                        {
                            content = Regex.Replace(content, direSiegePattern, direSiegeContent);
                        }
                    }
                    await Task.Delay(1000); // Delay to avoid rate limiting
                }

                if (selectedRadiantTower != null && radiantTowerValues.TryGetValue(selectedRadiantTower, out string? radiantTowerValue))
                {
                    SafeLog("Fetching Radiant Tower...");
                    string? radiantTowerContent = await GetStringWithRetryAsync(radiantTowerValue);
                    if (radiantTowerContent != null)
                    {
                        string radiantTowerPattern = @"(?s)""677""\s*\{[^}]*""prefab""\s*""radianttowers""[^}]*\}.*?(?=""678""|$)";
                        if (Regex.IsMatch(content, radiantTowerPattern))
                        {
                            content = Regex.Replace(content, radiantTowerPattern, radiantTowerContent);
                        }
                    }
                    await Task.Delay(1000); // Delay to avoid rate limiting
                }

                if (selectedDireTower != null && direTowerValues.TryGetValue(selectedDireTower, out string? direTowerValue))
                {
                    SafeLog("Fetching Dire Tower...");
                    string? direTowerContent = await GetStringWithRetryAsync(direTowerValue);
                    if (direTowerContent != null)
                    {
                        string direTowerPattern = @"(?s)""678""\s*\{[^}]*""prefab""\s*""diretowers""[^}]*\}.*?(?=""679""|$)";
                        if (Regex.IsMatch(content, direTowerPattern))
                        {
                            content = Regex.Replace(content, direTowerPattern, direTowerContent);
                        }
                    }
                    await Task.Delay(1000); // Delay to avoid rate limiting
                }

                if (selectedHUD != null && hudValues.TryGetValue(selectedHUD, out string? hudValue))
                {
                    SafeLog("Fetching HUD...");
                    string? hudContent = await GetStringWithRetryAsync(hudValue);
                    if (hudContent != null)
                    {
                        string hudPattern = @"(?s)""587""\s*\{[^}]*""prefab""\s*""hud_skin""[^}]*\}.*?(?=""588""|$)";
                        if (Regex.IsMatch(content, hudPattern))
                        {
                            content = Regex.Replace(content, hudPattern, hudContent);
                        }
                    }
                    await Task.Delay(1000); // Delay to avoid rate limiting
                }

                if (selectedVersus != null && versusValues.TryGetValue(selectedVersus, out string? versusValue))
                {
                    SafeLog("Fetching Versus Screen...");
                    string? versusContent = await GetStringWithRetryAsync(versusValue);
                    if (versusContent != null)
                    {
                        string versusPattern = @"(?s)""12970""\s*\{[^}]*""prefab""\s*""versus_screen""[^}]*\}.*?(?=""12971""|$)";
                        if (Regex.IsMatch(content, versusPattern))
                        {
                            content = Regex.Replace(content, versusPattern, versusContent);
                        }
                    }
                    await Task.Delay(1000); // Delay to avoid rate limiting
                }

                if (selectedRiver != null && riverValues.TryGetValue(selectedRiver, out string? riverValue))
                {
                    string? vpkDir = Path.GetDirectoryName(vpkPath);
                    if (vpkDir == null)
                    {
                        SafeLog("Failed to determine VPK directory.");
                        return;
                    }
                    string tempDir = Path.Combine(vpkDir, "_temp"); // Temporary folder

                    if (selectedRiver == "Default Vial")
                    {
                        SafeLog("Disabling Vial...");
                        string logPath = Path.Combine(tempDir, "extraction.log");
                        if (File.Exists(logPath))
                        {
                            string[] logLines = await File.ReadAllLinesAsync(logPath);
                            HashSet<string> filesToDelete = new HashSet<string>(
                                logLines.Select(line => line.Replace("Extracted: ", "")));

                            if (Directory.Exists(extractDir))
                            {
                                // Hapus file hasil extract yang ada di log
                                foreach (string file in Directory.GetFiles(extractDir, "*", SearchOption.AllDirectories))
                                {
                                    string relativePath = Path.GetRelativePath(extractDir, file);
                                    if (filesToDelete.Contains(relativePath))
                                    {
                                        File.Delete(file);
                                    }
                                }

                                // Hapus folder kosong
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
                        // Download the file
                        SafeLog("Fetching River Vial...");
                        string rarFilePath = Path.Combine(tempDir, "riverVial.rar");
                        Directory.CreateDirectory(tempDir);
                        using (var response = await GetWithRetryAsync(riverValue))
                        {
                            if (response == null) return; // Skip jika resource tidak ditemukan
                            using (var fs = new FileStream(rarFilePath, FileMode.Create))
                            {
                                await response.Content.CopyToAsync(fs);
                            }
                        }
                        await Task.Delay(1000); // Delay untuk menghindari rate limit

                        // Extract the file with password using SharpCompress
                        using (var archive = RarArchive.Open(rarFilePath, new ReaderOptions { Password = "muvestein@98" }))
                        {
                            var entries = archive.Entries.Where(e => !e.IsDirectory).ToList();
                            if (!entries.Any())
                            {
                                File.Delete(rarFilePath);
                                SafeLog("No valid files found in River Vial archive.");
                                return;
                            }

                            foreach (var entry in entries)
                            {
                                if (string.IsNullOrEmpty(entry.Key)) continue; // Skip jika key kosong

                                string destPath = Path.Combine(tempDir, entry.Key);
                                string? dir = Path.GetDirectoryName(destPath);
                                if (dir != null) Directory.CreateDirectory(dir);

                                entry.WriteToFile(destPath);

                                // Log the extracted file ke extraction.log
                                string logPath = Path.Combine(tempDir, "extraction.log");
                                await File.AppendAllTextAsync(logPath, $"Extracted: {entry.Key}\n");
                            }
                        }
                        File.Delete(rarFilePath); // Bersihkan RAR file

                        // Copy extracted contents dari _temp ke extractDir
                        foreach (string file in Directory.GetFiles(tempDir, "*", SearchOption.AllDirectories))
                        {
                            string relativePath = Path.GetRelativePath(tempDir, file);
                            if (Path.GetFileName(file) == "extraction.log") continue; // Skip extraction.log

                            string destPath = Path.Combine(extractDir, relativePath);
                            string? dir = Path.GetDirectoryName(destPath);
                            if (dir != null) Directory.CreateDirectory(dir);

                            File.Copy(file, destPath, true);
                        }

                        foreach (string dir in Directory.GetDirectories(tempDir, "*", SearchOption.AllDirectories))
                        {
                            string relativePath = Path.GetRelativePath(tempDir, dir);
                            string destDir = Path.Combine(extractDir, relativePath);
                            if (!Directory.Exists(destDir))
                            {
                                Directory.CreateDirectory(destDir);
                            }
                        }

                        // Verify extraction (minimal folder target ada)
                        string extractedParticlesDir = Path.Combine(extractDir, "particles");
                        if (!Directory.Exists(extractedParticlesDir))
                        {
                            SafeLog("Extraction failed or no valid River Vial folders found.");
                            return;
                        }
                    }
                }

                if (selectedEmblem != null && emblemsValues.TryGetValue(selectedEmblem, out string? emblemValue))
                {
                    string emblemDir = Path.Combine(extractDir, "particles", "ui_mouseactions");
                    if (selectedEmblem == "Disable Emblem")
                    {
                        SafeLog("Disabling Emblems...");
                        if (Directory.Exists(emblemDir))
                        {
                            Directory.Delete(emblemDir, true);
                        }
                    }
                    else
                    {
                        Directory.CreateDirectory(emblemDir);
                        SafeLog("Fetching Emblems...");
                        string emblemFileName = Path.GetFileName(emblemValue);
                        string emblemDestPath = Path.Combine(emblemDir, emblemFileName);
                        byte[]? emblemContent = await GetByteArrayWithRetryAsync(emblemValue);
                        if (emblemContent != null)
                        {
                            File.WriteAllBytes(emblemDestPath, emblemContent);
                        }
                        await Task.Delay(1000); // Delay to avoid rate limiting
                    }
                }

                if (selectedShader != null && shadersValues.TryGetValue(selectedShader, out string? shaderValue))
                {
                    string shaderDir = Path.Combine(extractDir, "materials", "dev");
                    if (selectedShader == "Disable Shader")
                    {
                        SafeLog("Disabling Shaders...");
                        if (Directory.Exists(shaderDir))
                        {
                            Directory.Delete(shaderDir, true);
                        }
                    }
                    else
                    {
                        Directory.CreateDirectory(shaderDir);
                        SafeLog("Fetching Shader...");
                        string shaderFileName = Path.GetFileName(shaderValue);
                        string shaderDestPath = Path.Combine(shaderDir, shaderFileName);
                        byte[]? shaderContent = await GetByteArrayWithRetryAsync(shaderValue);
                        if (shaderContent != null)
                        {
                            File.WriteAllBytes(shaderDestPath, shaderContent);
                        }
                        await Task.Delay(1000); // Delay to avoid rate limiting
                    }
                }

                File.WriteAllText(itemsGamePath, content);

                if (selectedAtkModifier != null && atkModifierValues.TryGetValue(selectedAtkModifier, out string? atkModifierValue))
                {
                    string? vpkDir = Path.GetDirectoryName(vpkPath);
                    if (vpkDir == null)
                    {
                        SafeLog("Failed to determine VPK directory.");
                        return;
                    }
                    string tempDir = Path.Combine(vpkDir, "_temp"); // Temporary folder

                    if (selectedAtkModifier == "Disable Attack Modifier")
                    {
                        SafeLog("Disabling Attack Modifier...");
                        string logPath = Path.Combine(tempDir, "extraction.log");
                        if (File.Exists(logPath))
                        {
                            string[] logLines = await File.ReadAllLinesAsync(logPath);
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

                                // Clean up empty directories
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

                        if (!Directory.GetFiles(extractDir, "*", SearchOption.AllDirectories).Any() && !Directory.GetDirectories(extractDir, "*", SearchOption.AllDirectories).Any())
                        {
                        }
                        else
                        {
                        }
                    }
                    else
                    {
                        // Download the file
                        SafeLog("Fetching Attack Modifier...");
                        string rarFilePath = Path.Combine(tempDir, "atkModifier.rar");
                        Directory.CreateDirectory(tempDir);
                        using (var response = await GetWithRetryAsync(atkModifierValue))
                        {
                            if (response == null) return; // Skip if resource not found
                            using (var fs = new FileStream(rarFilePath, FileMode.Create))
                            {
                                await response.Content.CopyToAsync(fs);
                            }
                        }
                        await Task.Delay(1000); // Delay to avoid rate limiting

                        // Extract the file with password using SharpCompress
                        using (var archive = SharpCompress.Archives.Rar.RarArchive.Open(rarFilePath, new SharpCompress.Readers.ReaderOptions { Password = "muvestein@98" }))
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

                                // Log the extracted file to extraction.log in _temp
                                string logPath = Path.Combine(tempDir, "extraction.log");
                                await File.AppendAllTextAsync(logPath, $"Extracted: {entry.Key}\n");
                            }
                        }
                        File.Delete(rarFilePath); // Clean up RAR file

                        // Copy (instead of move) extracted contents from _temp to extractDir
                        foreach (string file in Directory.GetFiles(tempDir, "*", SearchOption.AllDirectories))
                        {
                            string relativePath = Path.GetRelativePath(tempDir, file);
                            if (Path.GetFileName(file) == "extraction.log") continue; // Skip copying extraction.log
                            string destPath = Path.Combine(extractDir, relativePath);
                            Directory.CreateDirectory(Path.GetDirectoryName(destPath)!);
                            File.Copy(file, destPath, true); // Copy instead of move
                        }
                        foreach (string dir in Directory.GetDirectories(tempDir, "*", SearchOption.AllDirectories))
                        {
                            string relativePath = Path.GetRelativePath(tempDir, dir);
                            string destDir = Path.Combine(extractDir, relativePath);
                            if (!Directory.Exists(destDir))
                            {
                                Directory.CreateDirectory(destDir);
                            }
                        }

                        // Verify extraction
                        string extractedKisilevIndDir = Path.Combine(extractDir, "kisilev_ind", "particles", "modifier_attack");
                        string extractedParticlesDir = Path.Combine(extractDir, "particles");
                        if (Directory.Exists(extractedKisilevIndDir) || Directory.Exists(extractedParticlesDir))
                        {
                        }
                        else
                        {
                            SafeLog("Extraction failed or no valid folders found.");
                            return;
                        }
                    }
                }
                SafeLog("Please wait... Do not close this App while processing!");
                ProcessStartInfo packPsi = new ProcessStartInfo
                {
                    FileName = vpkToolPath,
                    Arguments = $"\"{extractDir}\"",
                    WorkingDirectory = Path.GetDirectoryName(vpkPath)!,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (Process packProcess = new Process { StartInfo = packPsi })
                {
                    try
                    {
                        packProcess.Start();
                        await Task.Delay(2000); // 2-second delay to ensure process completes
                        string packOutput = packProcess.StandardOutput.ReadToEnd();
                        string packError = packProcess.StandardError.ReadToEnd();
                        packProcess.WaitForExit();

                        if (packProcess.ExitCode == 0)
                        {
                            string? vpkDir = Path.GetDirectoryName(vpkPath);
                            if (vpkDir == null)
                            {
                                SafeLog("Failed to determine VPK directory for final packaging.");
                                return;
                            }
                            string tempVpk = Path.Combine(vpkDir, "extracted.vpk");
                            if (File.Exists(tempVpk))
                            {
                                if (File.Exists(vpkPath)) File.Delete(vpkPath);
                                await Task.Delay(5000);
                                File.Move(tempVpk, vpkPath);
                                await Task.Delay(5000);
                                Directory.Delete(extractDir, true);
                                await Task.Delay(5000);
                                SafeLog("Mods installed successfully.");

                                string tempDir = Path.Combine(vpkDir, "_temp");
                                if (Directory.Exists(tempDir))
                                {
                                    foreach (string file in Directory.GetFiles(tempDir, "*", SearchOption.AllDirectories))
                                    {
                                        if (Path.GetFileName(file) != "extraction.log")

                                            await Task.Delay(5000);
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
                                SafeLog($"Recompilation failed: {tempVpk} not found.");
                                return;
                            }
                        }
                        else
                        {
                            SafeLog($"Recompilation failed with code {packProcess.ExitCode}.");
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        SafeLog($"Error running: {ex.Message}");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                SafeLog($"Error: {ex.Message}");
            }
        }

        private void notelabel1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {
        }
    }
}
