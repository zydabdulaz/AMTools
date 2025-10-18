using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArdysaModsTools.Core.Controllers;
using ArdysaModsTools.Core.Services;
using ArdysaModsTools.UI.Controls;

namespace ArdysaModsTools
{
    public partial class MiscForm : Form
    {
        private readonly string? _targetPath;
        private readonly Action<string> _log;
        private readonly Action _disableButtons;
        private readonly Action _enableButtons;

        // Services
        private readonly MiscGenerationService _generator;
        private readonly MiscUtilityService _utils;

        // Logging
        private readonly Logger _miscLogger;

        public MiscForm(string? targetPath, Action<string> log, Action disableButtons, Action enableButtons)
        {
            _targetPath = targetPath;
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _disableButtons = disableButtons ?? throw new ArgumentNullException(nameof(disableButtons));
            _enableButtons = enableButtons ?? throw new ArgumentNullException(nameof(enableButtons));

            InitializeComponent();

            // Initialize services
            _generator = new MiscGenerationService();
            _utils = new MiscUtilityService();

            // Create dedicated logger for MiscForm
            _miscLogger = new Logger(ConsoleLogBox);

            // Wire up button events
            LoadPreset.Click += (s, e) => LoadPreset_Click();
            SavePreset.Click += (s, e) => SavePreset_Click();
            generateButton.Click += async (s, e) => await GenerateButton_Click();
        }

        // -----------------------------------------------------------
        // EVENT HANDLERS
        // -----------------------------------------------------------

        private void LoadPreset_Click()
        {
            using var open = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                Title = "Load Preset",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (open.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                var json = File.ReadAllText(open.FileName);
                var preset = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                if (preset == null) return;

                foreach (var kv in preset)
                {
                    var box = Controls.OfType<ComboBox>().FirstOrDefault(c => c.Name.StartsWith(kv.Key, StringComparison.OrdinalIgnoreCase));
                    if (box != null && box.Items.Contains(kv.Value))
                        box.SelectedItem = kv.Value;
                }

                _miscLogger.Log($"Preset loaded from {open.FileName}");
            }
            catch (Exception ex)
            {
                _miscLogger.Log($"Error loading preset: {ex.Message}");
            }
        }

        private void SavePreset_Click()
        {
            using var save = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                Title = "Save Preset",
                DefaultExt = "json",
                FileName = "Custom.json",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (save.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                var preset = Controls.OfType<ComboBox>()
                    .ToDictionary(c => c.Name.Replace("Box", ""), c => c.SelectedItem?.ToString() ?? "");

                var json = System.Text.Json.JsonSerializer.Serialize(preset, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(save.FileName, json);
                _miscLogger.Log($"Preset saved to {save.FileName}");
            }
            catch (Exception ex)
            {
                _miscLogger.Log($"Error saving preset: {ex.Message}");
            }
        }
        private async Task GenerateButton_Click()
        {
            _disableButtons();
            DisableFormControls();

            try
            {
                if (string.IsNullOrEmpty(_targetPath))
                {
                    _miscLogger.Log("No target path set.");
                    return;
                }

                // Collect user selections automatically
                var selections = Controls.OfType<ComboBox>()
                    .Where(c => c.Name.EndsWith("Box", StringComparison.OrdinalIgnoreCase))
                    .ToDictionary(
                        c => c.Name.Replace("Box", "", StringComparison.OrdinalIgnoreCase),
                        c => c.SelectedItem?.ToString() ?? "",
                        StringComparer.OrdinalIgnoreCase);

                // Save user selections
                await UserSettingsService.SaveSelectionsAsync(this);
                _miscLogger.Log("Selections saved successfully.");

                // Run mod generation
                var controller = new MiscController();
                var result = await controller.GenerateModsAsync(_targetPath!, selections, _miscLogger.Log, CancellationToken.None);

                if (result.Success)
                {
                    // Show only the final clean message
                }
                else
                {
                    _miscLogger.Log("Generation failed: " + (result.Message ?? "Unknown error."));
                }
            }
            catch (Exception ex)
            {
                _miscLogger.Log($"Error during generation: {ex.Message}");
            }
            finally
            {
                BeginInvoke((Action)(() =>
                {
                    _enableButtons();
                    EnableFormControls();
                }));
            }
        }


        // -----------------------------------------------------------
        // UI HELPERS
        // -----------------------------------------------------------

        private void DisableFormControls()
        {
            foreach (Control control in Controls)
            {
                if (control is ComboBox or Button)
                    control.Enabled = false;
            }
        }

        private void EnableFormControls()
        {
            foreach (Control control in Controls)
            {
                if (control is ComboBox or Button)
                    control.Enabled = true;
            }
        }

        // -----------------------------------------------------------
        // FORM EVENTS
        // -----------------------------------------------------------

        private async void MiscForm_Load(object sender, EventArgs e)
        {
            // Populate all ComboBoxes first
            try
            {
                WeatherBox.Items.AddRange(ComboBoxDataService.GetWeatherOptions().ToArray());
                MapBox.Items.AddRange(ComboBoxDataService.GetMapOptions().ToArray());
                MusicBox.Items.AddRange(ComboBoxDataService.GetMusicOptions().ToArray());
                EmblemsBox.Items.AddRange(ComboBoxDataService.GetEmblemOptions().ToArray());
                ShaderBox.Items.AddRange(ComboBoxDataService.GetShaderOptions().ToArray());
                atkModifierBox.Items.AddRange(ComboBoxDataService.GetAtkModifierOptions().ToArray());
                RadiantCreepBox.Items.AddRange(ComboBoxDataService.GetRadiantCreepOptions().ToArray());
                DireCreepBox.Items.AddRange(ComboBoxDataService.GetDireCreepOptions().ToArray());
                RadiantSiegeBox.Items.AddRange(ComboBoxDataService.GetRadiantSiegeOptions().ToArray());
                DireSiegeBox.Items.AddRange(ComboBoxDataService.GetDireSiegeOptions().ToArray());
                HUDBox.Items.AddRange(ComboBoxDataService.GetHudOptions().ToArray());
                VersusBox.Items.AddRange(ComboBoxDataService.GetVersusOptions().ToArray());
                RiverBox.Items.AddRange(ComboBoxDataService.GetRiverOptions().ToArray());
                RadiantTowerBox.Items.AddRange(ComboBoxDataService.GetRadiantTowerOptions().ToArray());
                DireTowerBox.Items.AddRange(ComboBoxDataService.GetDireTowerOptions().ToArray());

                // Default selections
                foreach (var box in Controls.OfType<ComboBox>())
                {
                    if (box.Items.Count > 0)
                        box.SelectedIndex = 0;
                }

                // Step 2 – Restore user selections
                await UserSettingsService.RestoreSelectionsAsync(this, msg => { _miscLogger.Log(msg); });
            }
            catch (Exception ex)
            {
                _miscLogger.Log($"Error during form load: {ex.Message}");
            }
        }
    }
}
