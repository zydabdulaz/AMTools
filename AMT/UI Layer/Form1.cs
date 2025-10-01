using AMTools.Core;
using AMTools.Services;
using System;
using System.Windows.Forms;

namespace AMTools.UI
{
    public partial class MainForm : Form
    {
        private readonly ModToolManager _modManager;

        public MainForm()
        {
            InitializeComponent();
            var fileService = new FileService();
            var logger = new LoggerService();
            _modManager = new ModToolManager(fileService, logger);
        }

        private async void btnInstall_Click(object sender, EventArgs e)
        {
            string modPath = txtModPath.Text;
            string targetDir = txtTargetDir.Text;

            btnInstall.Enabled = false;
            var result = await _modManager.InstallModAsync(modPath, targetDir);
            btnInstall.Enabled = true;

            MessageBox.Show(result.Message, 
                result.Success ? "Success" : "Error",
                MessageBoxButtons.OK, 
                result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Error);
        }
    }
}
