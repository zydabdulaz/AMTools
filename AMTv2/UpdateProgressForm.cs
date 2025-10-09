using System;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ArdysaModsTools
{
    public partial class UpdateProgressForm : Form
    {
        public UpdateProgressForm()
        {
            InitializeComponent();
        }

        public void UpdateProgress(int percentage)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<int>(UpdateProgress), percentage);
            }
            else
            {
                progressBar.Value = Math.Min(percentage, 100);
            }
        }

        public void SetStatus(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(SetStatus), message);
            }
            else
            {
                statusLabel.Text = message;
            }
        }
    }
}