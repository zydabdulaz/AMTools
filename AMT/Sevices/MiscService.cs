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
            _miscService = new MiscService(_logger, "your-github-token-here");
            _disableButtons = disableButtons;
            _enableButtons = enableButtons;

            InitializeComponent();
            HookUiEvents();
            PopulateDictionaries();
        }

        private void HookUiEvents()
        {
            LoadPreset.Click += LoadPreset_Click;
            SavePreset.Click += SavePreset_Click;
            generateButton.Click += GenerateButton_Click;
        }

        private void PopulateDictionaries()
        {
            // Instead of duplicating dictionaries everywhere, 
            // call static dictionary provider or load from JSON later
            WeatherBox.Items.AddRange(MiscService.WeatherValues.Keys.ToArray());
            MapBox.Items.AddRange(MiscService.MapValues.Keys.ToArray());
            HUDBox.Items.AddRange(MiscService.HudValues.Keys.ToArray());
            // ... repeat for MusicBox, EmblemsBox, etc.
        }

        private async void GenerateButton_Click(object sender, EventArgs e)
        {
            _disableButtons();
            DisableFormControls();

            try
            {
                var result = await _miscService.GenerateAsync(_targetPath,
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
                    DireTowerBox.SelectedItem?.ToString());

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
