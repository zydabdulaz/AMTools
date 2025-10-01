namespace ArdysaModsTools
{
    partial class Form1
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

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            autoDetectButton = new Button();
            manualDetectButton = new Button();
            installButton = new Button();
            disableButton = new Button();
            dividerLabel = new Label();
            updatePatcherButton = new Button();
            statusModsDotLabel = new Label();
            statusModsTextLabel = new Label();
            progressBar = new ProgressBar();
            consoleLog = new RichTextBox();
            label1 = new Label();
            label2 = new Label();
            discordPictureBox = new PictureBox();
            youtubePictureBox = new PictureBox();
            paypalPictureBox = new PictureBox();
            label3 = new Label();
            miscellaneousButton = new Button();
            label4 = new Label();
            ((System.ComponentModel.ISupportInitialize)discordPictureBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)youtubePictureBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)paypalPictureBox).BeginInit();
            SuspendLayout();
            // 
            // autoDetectButton
            // 
            autoDetectButton.BackColor = Color.FromArgb(58, 58, 58);
            autoDetectButton.FlatAppearance.BorderSize = 0;
            autoDetectButton.FlatStyle = FlatStyle.Flat;
            autoDetectButton.Font = new Font("Segoe UI", 9F);
            autoDetectButton.ForeColor = Color.FromArgb(200, 200, 200);
            autoDetectButton.Location = new Point(22, 21);
            autoDetectButton.Margin = new Padding(4, 3, 4, 3);
            autoDetectButton.Name = "autoDetectButton";
            autoDetectButton.Size = new Size(150, 50);
            autoDetectButton.TabIndex = 3;
            autoDetectButton.Text = "Auto Detect";
            autoDetectButton.UseVisualStyleBackColor = false;
            autoDetectButton.Click += AutoDetectButton_Click;
            autoDetectButton.MouseEnter += Button_MouseEnter;
            autoDetectButton.MouseLeave += Button_MouseLeave;
            // 
            // manualDetectButton
            // 
            manualDetectButton.BackColor = Color.FromArgb(58, 58, 58);
            manualDetectButton.FlatAppearance.BorderSize = 0;
            manualDetectButton.FlatStyle = FlatStyle.Flat;
            manualDetectButton.Font = new Font("Segoe UI", 9F);
            manualDetectButton.ForeColor = Color.FromArgb(200, 200, 200);
            manualDetectButton.Location = new Point(180, 21);
            manualDetectButton.Margin = new Padding(4, 3, 4, 3);
            manualDetectButton.Name = "manualDetectButton";
            manualDetectButton.Size = new Size(150, 50);
            manualDetectButton.TabIndex = 4;
            manualDetectButton.Text = "Manual Detect";
            manualDetectButton.UseVisualStyleBackColor = false;
            manualDetectButton.Click += ManualDetectButton_Click;
            manualDetectButton.MouseEnter += Button_MouseEnter;
            manualDetectButton.MouseLeave += Button_MouseLeave;
            // 
            // installButton
            // 
            installButton.BackColor = Color.FromArgb(58, 58, 58);
            installButton.FlatAppearance.BorderSize = 0;
            installButton.FlatStyle = FlatStyle.Flat;
            installButton.Font = new Font("Segoe UI", 9F);
            installButton.ForeColor = Color.FromArgb(200, 200, 200);
            installButton.Location = new Point(22, 80);
            installButton.Margin = new Padding(4, 3, 4, 3);
            installButton.Name = "installButton";
            installButton.Size = new Size(150, 50);
            installButton.TabIndex = 5;
            installButton.Text = "Install";
            installButton.UseVisualStyleBackColor = false;
            installButton.Click += InstallButton_Click;
            installButton.MouseEnter += Button_MouseEnter;
            installButton.MouseLeave += Button_MouseLeave;
            // 
            // disableButton
            // 
            disableButton.BackColor = Color.FromArgb(58, 58, 58);
            disableButton.FlatAppearance.BorderSize = 0;
            disableButton.FlatStyle = FlatStyle.Flat;
            disableButton.Font = new Font("Segoe UI", 9F);
            disableButton.ForeColor = Color.FromArgb(200, 200, 200);
            disableButton.Location = new Point(180, 80);
            disableButton.Margin = new Padding(4, 3, 4, 3);
            disableButton.Name = "disableButton";
            disableButton.Size = new Size(150, 50);
            disableButton.TabIndex = 6;
            disableButton.Text = "Disable";
            disableButton.UseVisualStyleBackColor = false;
            disableButton.Click += DisableButton_Click;
            disableButton.MouseEnter += Button_MouseEnter;
            disableButton.MouseLeave += Button_MouseLeave;
            // 
            // dividerLabel
            // 
            dividerLabel.BackColor = Color.FromArgb(100, 100, 100);
            dividerLabel.Location = new Point(22, 145);
            dividerLabel.Name = "dividerLabel";
            dividerLabel.Size = new Size(308, 2);
            dividerLabel.TabIndex = 7;
            // 
            // updatePatcherButton
            // 
            updatePatcherButton.BackColor = Color.FromArgb(58, 58, 58);
            updatePatcherButton.FlatAppearance.BorderSize = 0;
            updatePatcherButton.FlatStyle = FlatStyle.Flat;
            updatePatcherButton.Font = new Font("Segoe UI", 9F);
            updatePatcherButton.ForeColor = Color.FromArgb(200, 200, 200);
            updatePatcherButton.Location = new Point(23, 226);
            updatePatcherButton.Margin = new Padding(4, 3, 4, 3);
            updatePatcherButton.Name = "updatePatcherButton";
            updatePatcherButton.Size = new Size(150, 34);
            updatePatcherButton.TabIndex = 9;
            updatePatcherButton.Text = "Patch Update";
            updatePatcherButton.UseVisualStyleBackColor = false;
            updatePatcherButton.Click += UpdatePatcherButton_Click;
            updatePatcherButton.MouseEnter += Button_MouseEnter;
            updatePatcherButton.MouseLeave += Button_MouseLeave;
            // 
            // statusModsDotLabel
            // 
            statusModsDotLabel.BackColor = Color.FromArgb(150, 150, 150);
            statusModsDotLabel.Location = new Point(206, 236);
            statusModsDotLabel.Name = "statusModsDotLabel";
            statusModsDotLabel.Size = new Size(15, 15);
            statusModsDotLabel.TabIndex = 10;
            // 
            // statusModsTextLabel
            // 
            statusModsTextLabel.AutoSize = true;
            statusModsTextLabel.Font = new Font("Segoe UI", 9F);
            statusModsTextLabel.ForeColor = Color.FromArgb(150, 150, 150);
            statusModsTextLabel.Location = new Point(229, 236);
            statusModsTextLabel.Margin = new Padding(4, 0, 4, 0);
            statusModsTextLabel.Name = "statusModsTextLabel";
            statusModsTextLabel.Size = new Size(76, 15);
            statusModsTextLabel.TabIndex = 11;
            statusModsTextLabel.Text = "Not Checked";
            // 
            // progressBar
            // 
            progressBar.BackColor = Color.FromArgb(40, 40, 40);
            progressBar.ForeColor = Color.FromArgb(0, 123, 255);
            progressBar.Location = new Point(12, 299);
            progressBar.Margin = new Padding(4, 3, 4, 3);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(327, 14);
            progressBar.TabIndex = 12;
            progressBar.Click += progressBar_Click;
            // 
            // consoleLog
            // 
            consoleLog.BackColor = Color.FromArgb(40, 40, 40);
            consoleLog.BorderStyle = BorderStyle.FixedSingle;
            consoleLog.DetectUrls = false;
            consoleLog.Font = new Font("Cascadia Code", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            consoleLog.ForeColor = Color.FromArgb(200, 200, 200);
            consoleLog.Location = new Point(13, 343);
            consoleLog.Margin = new Padding(4, 3, 4, 3);
            consoleLog.Name = "consoleLog";
            consoleLog.ReadOnly = true;
            consoleLog.ScrollBars = RichTextBoxScrollBars.Vertical;
            consoleLog.Size = new Size(326, 124);
            consoleLog.TabIndex = 13;
            consoleLog.Text = "";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.ForeColor = Color.MintCream;
            label1.Location = new Point(137, 321);
            label1.Name = "label1";
            label1.Size = new Size(79, 15);
            label1.TabIndex = 14;
            label1.Text = "Console Log :";
            label1.Click += label1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.ForeColor = Color.MintCream;
            label2.Location = new Point(260, 481);
            label2.Name = "label2";
            label2.Size = new Size(84, 15);
            label2.TabIndex = 17;
            label2.Text = "Version: 1.9.7.2";
            label2.Click += label2_Click;
            // 
            // discordPictureBox
            // 
            discordPictureBox.Location = new Point(94, 481);
            discordPictureBox.Name = "discordPictureBox";
            discordPictureBox.Size = new Size(70, 15);
            discordPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            discordPictureBox.TabIndex = 18;
            discordPictureBox.TabStop = false;
            discordPictureBox.Click += DiscordPictureBox_Click;
            // 
            // youtubePictureBox
            // 
            youtubePictureBox.Location = new Point(172, 481);
            youtubePictureBox.Name = "youtubePictureBox";
            youtubePictureBox.Size = new Size(70, 15);
            youtubePictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            youtubePictureBox.TabIndex = 19;
            youtubePictureBox.TabStop = false;
            youtubePictureBox.Click += YoutubePictureBox_Click;
            // 
            // paypalPictureBox
            // 
            paypalPictureBox.Location = new Point(16, 481);
            paypalPictureBox.Name = "paypalPictureBox";
            paypalPictureBox.Size = new Size(70, 15);
            paypalPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            paypalPictureBox.TabIndex = 20;
            paypalPictureBox.TabStop = false;
            paypalPictureBox.Click += PaypalPictureBox_Click;
            // 
            // label3
            // 
            label3.BackColor = Color.FromArgb(100, 100, 100);
            label3.Location = new Point(23, 207);
            label3.Name = "label3";
            label3.Size = new Size(308, 2);
            label3.TabIndex = 22;
            // 
            // miscellaneousButton
            // 
            miscellaneousButton.BackColor = Color.FromArgb(128, 128, 255);
            miscellaneousButton.FlatAppearance.BorderSize = 0;
            miscellaneousButton.FlatStyle = FlatStyle.Flat;
            miscellaneousButton.Font = new Font("Segoe UI", 9F);
            miscellaneousButton.ForeColor = Color.Transparent;
            miscellaneousButton.Location = new Point(94, 160);
            miscellaneousButton.Name = "miscellaneousButton";
            miscellaneousButton.Size = new Size(150, 30);
            miscellaneousButton.TabIndex = 21;
            miscellaneousButton.Text = "Miscellaneous Options";
            miscellaneousButton.UseVisualStyleBackColor = false;
            miscellaneousButton.Click += MiscellaneousButton_Click;
            miscellaneousButton.MouseEnter += Button_MouseEnter;
            miscellaneousButton.MouseLeave += Button_MouseLeave;
            // 
            // label4
            // 
            label4.BackColor = Color.FromArgb(100, 100, 100);
            label4.Location = new Point(23, 279);
            label4.Name = "label4";
            label4.Size = new Size(308, 2);
            label4.TabIndex = 23;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(45, 45, 45);
            ClientSize = new Size(355, 515);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(miscellaneousButton);
            Controls.Add(paypalPictureBox);
            Controls.Add(youtubePictureBox);
            Controls.Add(discordPictureBox);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(consoleLog);
            Controls.Add(progressBar);
            Controls.Add(statusModsTextLabel);
            Controls.Add(statusModsDotLabel);
            Controls.Add(updatePatcherButton);
            Controls.Add(dividerLabel);
            Controls.Add(disableButton);
            Controls.Add(installButton);
            Controls.Add(manualDetectButton);
            Controls.Add(autoDetectButton);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Form1";
            RightToLeft = RightToLeft.No;
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "ArdysaModsTools";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)discordPictureBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)youtubePictureBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)paypalPictureBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        // Add field declaration
        private System.Windows.Forms.Button miscellaneousButton;

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        private System.Windows.Forms.Button autoDetectButton;
        private System.Windows.Forms.Button manualDetectButton;
        private System.Windows.Forms.Button installButton;
        private System.Windows.Forms.Button disableButton;
        private System.Windows.Forms.Label dividerLabel;
        private System.Windows.Forms.Button updatePatcherButton;
        private System.Windows.Forms.Label statusModsDotLabel;
        private System.Windows.Forms.Label statusModsTextLabel;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.RichTextBox consoleLog;
        private Label label1;
        private Label label2;
        private PictureBox discordPictureBox;
        private PictureBox youtubePictureBox;
        private PictureBox paypalPictureBox; // Added field declaration
        private Label label3;
        private Label label4;
    }
}