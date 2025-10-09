using System;
using System.Drawing;
using System.Windows.Forms;

namespace ArdysaModsTools
{
    public partial class UpdateProgressForm : Form
    {
        private System.ComponentModel.IContainer components = null;
        public ProgressBar progressBar;
        public Label statusLabel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            progressBar = new ProgressBar();
            statusLabel = new Label();
            SuspendLayout();
            // 
            // progressBar
            // 
            progressBar.Location = new Point(23, 58);
            progressBar.Margin = new Padding(4, 3, 4, 3);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(292, 27);
            progressBar.TabIndex = 0;
            // 
            // statusLabel
            // 
            statusLabel.AutoSize = true;
            statusLabel.ForeColor = Color.White;
            statusLabel.Location = new Point(23, 23);
            statusLabel.Margin = new Padding(4, 0, 4, 0);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(127, 15);
            statusLabel.TabIndex = 1;
            statusLabel.Text = "Starting process...";
            // 
            // UpdateProgressForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(50, 50, 50);
            ClientSize = new Size(338, 115);
            ControlBox = false;
            Controls.Add(statusLabel);
            Controls.Add(progressBar);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(4, 3, 4, 3);
            Name = "UpdateProgressForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Processing Mods";
            TopMost = true;
            ResumeLayout(false);
            PerformLayout();
        }

        public void UpdateProgress(string message, int progressPercentage)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string, int>(UpdateProgress), message, progressPercentage);
                return;
            }
            statusLabel.Text = message;
            progressBar.Value = Math.Min(progressPercentage, 100); // Cap at 100%
        }
    }
}