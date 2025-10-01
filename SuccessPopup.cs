namespace ArdysaModsTools
{
    public partial class SuccessPopup : Form
    {
        public SuccessPopup()
        {
            InitializeComponent();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.Close(); // Simply closes the pop-up
        }

        private void launchDotaButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Launch Dota 2 via Steam
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "steam://rungameid/570",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to launch Dota 2: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Close(); // Close the pop-up after launching
        }
    }
}