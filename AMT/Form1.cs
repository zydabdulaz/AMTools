using ArdysaModsTools.Core;
using ArdysaModsTools.Services;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ArdysaModsTools
{
    public partial class Form1 : Form
    {
        private readonly ModToolManager _modManager;
        private readonly LoggerService _logger;

        public Form1()
        {
            InitializeComponent();

            // --- Service setup ---
            var fileService = new FileService();
            var updateService = new UpdateService();
            var detectionService = new DetectionService();
            _logger = new LoggerService(consoleLog);

            // Pass UI update callback into manager
            _modManager = new ModToolManager(fileService, updateService, detectionService, _logger, UpdateStatusUI, progressBar);

            _logger.Info("Ensure the 'game' folder is in the same directory as ArdysaModsTools.exe for installation to work!");

            // Initial UI state
            statusModsDotLabel.BackColor = Color.Gray;
            statusModsTextLabel.Text = "Not Checked";
            statusModsTextLabel.ForeColor = Color.Gray;

            EnableDetectionButtonsOnly();

            // Auto check for updates
            _ = _modManager.CheckForUpdatesOnStartupAsync();
        }

        // -------------------
        // UI Event Handlers
        // -------------------

        private async void AutoDetectButton_Click(object sender, EventArgs e)
        {
            DisableAllButtons();
            var result = await _modManager.AutoDetectAsync();
            MessageBox.Show(result.Message, result.Success ? "Success" : "Error");
            EnableAllButtons();
        }

        private async void ManualDetectButton_Click(object sender, EventArgs e)
        {
            DisableAllButtons();
            var result = await _modManager.ManualDetectAsync();
            MessageBox.Show(result.Message, result.Success ? "Success" : "Error");
            EnableAllButtons();
        }

        private async void InstallButton_Click(object sender, EventArgs e)
        {
            DisableAllButtons();
            var result = await _modManager.InstallModsAsync();
            MessageBox.Show(result.Message, result.Success ? "Installed" : "Error");
            EnableAllButtons();
        }

        private async void DisableButton_Click(object sender, EventArgs e)
        {
            DisableAllButtons();
            var result = await _modManager.DisableModsAsync();
            MessageBox.Show(result.Message, result.Success ? "Disabled" : "Error");
            EnableAllButtons();
        }

        private async void UpdatePatcherButton_Click(object sender, EventArgs e)
        {
            DisableAllButtons();
            var result = await _modManager.UpdatePatcherAsync();
            MessageBox.Show(result.Message, result.Success ? "Updated" : "Error");
            EnableAllButtons();
        }

        private void MiscellaneousButton_Click(object sender, EventArgs e)
        {
            using var miscForm = new MiscellaneousForm(_modManager.TargetPath, _logger.Log, consoleLog, DisableAllButtons, EnableAllButtons);
            miscForm.ShowDialog(this);
        }

        private void DiscordPictureBox_Click(object sender, EventArgs e)
        {
            _modManager.OpenUrl("https://discord.gg/ffXw265Z7e", "Discord");
        }

        private void YoutubePictureBox_Click(object sender, EventArgs e)
        {
            _modManager.OpenUrl("https://youtube.com/@Ardysa", "YouTube");
        }

        private void PaypalPictureBox_Click(object sender, EventArgs e)
        {
            _modManager.OpenUrl("https://paypal.me/Ardysa", "PayPal");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _modManager.LoadSocialIcons(discordPictureBox, youtubePictureBox, paypalPictureBox);
        }

        // -------------------
        // UI Helpers
        // -------------------

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

        // Update mods status label from manager
        private void UpdateStatusUI(string status, Color color)
        {
            BeginInvoke((Action)(() =>
            {
                statusModsTextLabel.Text = status;
                statusModsTextLabel.ForeColor = color;
                statusModsDotLabel.BackColor = color;
            }));
        }

        // UI hover effects
        private void Button_MouseEnter(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.Enabled)
            {
                btn.BackColor = Color.FromArgb(0, 123, 255);
                btn.ForeColor = Color.White;
            }
        }

        private void Button_MouseLeave(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.Enabled)
            {
                btn.BackColor = Color.FromArgb(58, 58, 58);
                btn.ForeColor = Color.FromArgb(200, 200, 200);
            }
        }

        private void label1_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void button1_Click(object sender, EventArgs e) { }
        private void progressBar_Click(object sender, EventArgs e) { }
    }
}
