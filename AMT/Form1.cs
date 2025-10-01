using AMTools.Core;
using AMTools.Services;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ArdysaModsTools
{
    public partial class Form1 : Form
    {
        private readonly ModToolManager _modManager;

        public Form1()
        {
            InitializeComponent();

            // Setup dependencies
            var fileService = new FileService();
            var updateService = new UpdateService();
            var detectionService = new DetectionService();
            var logger = new LoggerService(consoleLog); // now logs to UI textbox

            _modManager = new ModToolManager(fileService, updateService, detectionService, logger, UpdateStatusUI);

            logger.Info("Ensure the 'game' folder is in the same directory as ArdysaModsTools.exe for installation to work!");

            // Initial status
            statusModsDotLabel.BackColor = Color.Gray;
            statusModsTextLabel.Text = "Not Checked";
            statusModsTextLabel.ForeColor = Color.Gray;

            EnableDetectionButtonsOnly();
            _ = _modManager.CheckForUpdatesOnStartup();
        }

        // --- UI Handlers ---
        private async void autoDetectButton_Click(object sender, EventArgs e)
        {
            DisableAllButtons();
            var result = await _modManager.AutoDetectAsync();
            MessageBox.Show(result.Message, result.Success ? "Success" : "Error");
            EnableAllButtons();
        }

        private async void manualDetectButton_Click(object sender, EventArgs e)
        {
            DisableAllButtons();
            var result = await _modManager.ManualDetectAsync();
            MessageBox.Show(result.Message, result.Success ? "Success" : "Error");
            EnableAllButtons();
        }

        private async void installButton_Click(object sender, EventArgs e)
        {
            DisableAllButtons();
            var result = await _modManager.InstallModsAsync();
            MessageBox.Show(result.Message, result.Success ? "Installed" : "Error");
            EnableAllButtons();
        }

        private async void disableButton_Click(object sender, EventArgs e)
        {
            DisableAllButtons();
            var result = await _modManager.DisableModsAsync();
            MessageBox.Show(result.Message, result.Success ? "Disabled" : "Error");
            EnableAllButtons();
        }

        private async void updatePatcherButton_Click(object sender, EventArgs e)
        {
            DisableAllButtons();
            var result = await _modManager.UpdatePatcherAsync();
            MessageBox.Show(result.Message, result.Success ? "Updated" : "Error");
            EnableAllButtons();
        }

        private void miscellaneousButton_Click(object sender, EventArgs e)
        {
            using var miscForm = new MiscellaneousForm(_modManager.TargetPath, _modManager.Log);
            miscForm.ShowDialog(this);
        }

        // --- UI Helpers ---
        private void DisableAllButtons()
        {
            autoDetectButton.Enabled = manualDetectButton.Enabled =
            installButton.Enabled = disableButton.Enabled =
            updatePatcherButton.Enabled = miscellaneousButton.Enabled = false;
        }

        private void EnableDetectionButtonsOnly()
        {
            autoDetectButton.Enabled = manualDetectButton.Enabled = true;
            installButton.Enabled = disableButton.Enabled =
            updatePatcherButton.Enabled = miscellaneousButton.Enabled = false;
        }

        private void EnableAllButtons()
        {
            autoDetectButton.Enabled = manualDetectButton.Enabled = true;
            installButton.Enabled = disableButton.Enabled =
            updatePatcherButton.Enabled = miscellaneousButton.Enabled = true;
        }

        // Updates status labels (called by ModToolManager via delegate)
        private void UpdateStatusUI(string status, Color color)
        {
            BeginInvoke((Action)(() =>
            {
                statusModsTextLabel.Text = status;
                statusModsTextLabel.ForeColor = color;
                statusModsDotLabel.BackColor = color;
            }));
        }
    }
}
