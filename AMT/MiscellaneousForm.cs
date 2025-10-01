using ArdysaModsTools.Services;

namespace ArdysaModsTools
{
    public partial class MiscellaneousForm : Form
    {
        private readonly string? _targetPath;
        private readonly LoggerService _logger;
        private readonly MiscService _miscService;
        private readonly Action _disableButtons;
        private readonly Action _enableButtons;

        public MiscellaneousForm(
            string? targetPath,
            Action<string> log,
            RichTextBox consoleLogBox,
            Action disableButtons,
            Action enableButtons)
        {
            _targetPath = targetPath;
            _logger = new LoggerService(consoleLogBox);
            _miscService = new MiscService(_logger);
            _disableButtons = disableButtons;
            _enableButtons = enableButtons;

            InitializeComponent();
            HookUiEvents();
            PopulateComboBoxes();
        }

        private void HookUiEvents()
        {
            LoadPreset.Click += (s, e) => _miscService.LoadPreset(WeatherBox, MapBox, MusicBox, EmblemsBox,
                                                                   ShaderBox, atkModifierBox, RadiantCreepBox,
                                                                   DireCreepBox, RadiantSiegeBox, DireSiegeBox,
                                                                   HUDBox, VersusBox, RiverBox,
                                                                   RadiantTowerBox, DireTowerBox);

            SavePreset.Click += (s, e) => _miscService.SavePreset(WeatherBox, MapBox, MusicBox, EmblemsBox,
                                                                  ShaderBox, atkModifierBox, RadiantCreepBox,
                                                                  DireCreepBox, RadiantSiegeBox, DireSiegeBox,
                                                                  HUDBox, VersusBox, RiverBox,
                                                                  RadiantTowerBox, DireTowerBox);

            generateButton.Click += GenerateButton_Click;
        }

        private void PopulateComboBoxes()
        {
            WeatherBox.Items.AddRange(MiscService.WeatherValues.Keys.ToArray());
            MapBox.Items.AddRange(MiscService.MapValues.Keys.ToArray());
            MusicBox.Items.AddRange(MiscService.MusicValues.Keys.ToArray());
            EmblemsBox.Items.AddRange(MiscService.EmblemsValues.Keys.ToArray());
            ShaderBox.Items.AddRange(MiscService.ShadersValues.Keys.ToArray());
            atkModifierBox.Items.AddRange(MiscService.AtkModifierValues.Keys.ToArray());
            RadiantCreepBox.Items.AddRange(MiscService.RadiantCreepValues.Keys.ToArray());
            DireCreepBox.Items.AddRange(MiscService.DireCreepValues.Keys.ToArray());
            RadiantSiegeBox.Items.AddRange(MiscService.RadiantSiegeValues.Keys.ToArray());
            DireSiegeBox.Items.AddRange(MiscService.DireSiegeValues.Keys.ToArray());
            HUDBox.Items.AddRange(MiscService.HudValues.Keys.ToArray());
            VersusBox.Items.AddRange(MiscService.VersusValues.Keys.ToArray());
            RiverBox.Items.AddRange(MiscService.RiverValues.Keys.ToArray());
            RadiantTowerBox.Items.AddRange(MiscService.RadiantTowerValues.Keys.ToArray());
            DireTowerBox.Items.AddRange(MiscService.DireTowerValues.Keys.ToArray());

            // defaults
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

        private async void GenerateButton_Click(object sender, EventArgs e)
        {
            _disableButtons();
            DisableFormControls();

            try
            {
                var result = await _miscService.GenerateAsync(
                    _targetPath,
                    WeatherBox.SelectedItem?.ToString(),
                    MapBox.SelectedItem?.ToString(),
                    MusicBox.SelectedItem?.ToString(),
                    EmblemsBox.SelectedItem?.ToString(),
                    ShaderBox.SelectedItem?.ToString(),
                    atkModifierBox.SelectedItem?.ToString(),
                    RadiantCreepBox.SelectedItem?.ToString(),
                    DireCreepBox.SelectedItem?.ToString(),
                    RadiantSiegeBox.SelectedItem?.ToString(),
                    DireSiegeBox.SelectedItem?.ToString(),
                    HUDBox.SelectedItem?.ToString(),
                    VersusBox.SelectedItem?.ToString(),
                    RiverBox.SelectedItem?.ToString(),
                    RadiantTowerBox.SelectedItem?.ToString(),
                    DireTowerBox.SelectedItem?.ToString()
                );

                MessageBox.Show(result.Message, result.Success ? "Success" : "Error");
            }
            finally
            {
                _enableButtons();
                EnableFormControls();
            }
        }
    }
}
