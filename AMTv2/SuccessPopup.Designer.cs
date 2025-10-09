namespace ArdysaModsTools
{
    partial class SuccessPopup
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            messageLabel = new Label();
            okButton = new Button();
            launchDotaButton = new Button();
            SuspendLayout();
            // 
            // messageLabel
            // 
            messageLabel.Dock = DockStyle.Top;
            messageLabel.Font = new Font("Arial", 9F, FontStyle.Bold);
            messageLabel.ForeColor = Color.White;
            messageLabel.Location = new Point(0, 0);
            messageLabel.Name = "messageLabel";
            messageLabel.Size = new Size(284, 50);
            messageLabel.TabIndex = 0;
            messageLabel.Text = "Mods Installed!\nRun Dota 2 now.";
            messageLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // okButton
            // 
            okButton.BackColor = Color.FromArgb(0, 188, 212);
            okButton.FlatStyle = FlatStyle.Flat;
            okButton.ForeColor = Color.White;
            okButton.Location = new Point(40, 60);
            okButton.Name = "okButton";
            okButton.Size = new Size(75, 23);
            okButton.TabIndex = 1;
            okButton.Text = "OK";
            okButton.UseVisualStyleBackColor = false;
            okButton.Click += okButton_Click;
            // 
            // launchDotaButton
            // 
            launchDotaButton.BackColor = Color.FromArgb(0, 188, 212);
            launchDotaButton.FlatStyle = FlatStyle.Flat;
            launchDotaButton.ForeColor = Color.White;
            launchDotaButton.Location = new Point(169, 60);
            launchDotaButton.Name = "launchDotaButton";
            launchDotaButton.Size = new Size(75, 23);
            launchDotaButton.TabIndex = 2;
            launchDotaButton.Text = "Launch Dota";
            launchDotaButton.UseVisualStyleBackColor = false;
            launchDotaButton.Click += launchDotaButton_Click;
            // 
            // SuccessPopup
            // 
            AcceptButton = okButton;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(50, 50, 50);
            ClientSize = new Size(284, 95);
            Controls.Add(launchDotaButton);
            Controls.Add(okButton);
            Controls.Add(messageLabel);
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SuccessPopup";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Installation Complete";
            TopMost = true;
            ResumeLayout(false);
        }
        #endregion

        private System.Windows.Forms.Label messageLabel;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button launchDotaButton;
    }
}