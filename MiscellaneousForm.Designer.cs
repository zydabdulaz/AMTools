namespace ArdysaModsTools
{
    partial class MiscellaneousForm
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
            generateButton = new Button();
            WeatherBox = new ComboBox();
            MapBox = new ComboBox();
            MusicBox = new ComboBox();
            EmblemsBox = new ComboBox();
            ShaderBox = new ComboBox();
            weatherLabel = new Label();
            terrainLabel = new Label();
            musicLabel = new Label();
            emblemsLabel = new Label();
            shaderLabel = new Label();
            radiantcreeplabel = new Label();
            RadiantCreepBox = new ComboBox();
            direcreeplabel = new Label();
            DireCreepBox = new ComboBox();
            diresiegelabel = new Label();
            DireSiegeBox = new ComboBox();
            radiantsiegelabel = new Label();
            RadiantSiegeBox = new ComboBox();
            dividerLabel = new Label();
            atkModifier = new Label();
            atkModifierBox = new ComboBox();
            ConsoleLogBox = new RichTextBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            RiverBox = new ComboBox();
            VersusBox = new ComboBox();
            HUDBox = new ComboBox();
            label5 = new Label();
            DireTowerBox = new ComboBox();
            label6 = new Label();
            RadiantTowerBox = new ComboBox();
            LoadPreset = new Button();
            SavePreset = new Button();
            SuspendLayout();
            // 
            // generateButton
            // 
            generateButton.BackColor = Color.Cyan;
            generateButton.FlatAppearance.BorderSize = 0;
            generateButton.FlatStyle = FlatStyle.Flat;
            generateButton.Font = new Font("Segoe UI", 9F);
            generateButton.ForeColor = Color.Black;
            generateButton.Location = new Point(453, 359);
            generateButton.Name = "generateButton";
            generateButton.Size = new Size(179, 49);
            generateButton.TabIndex = 0;
            generateButton.Text = "Generate";
            generateButton.UseVisualStyleBackColor = false;
            generateButton.Click += GenerateButton_Click;
            // 
            // WeatherBox
            // 
            WeatherBox.DropDownStyle = ComboBoxStyle.DropDownList;
            WeatherBox.FormattingEnabled = true;
            WeatherBox.Location = new Point(29, 46);
            WeatherBox.Name = "WeatherBox";
            WeatherBox.Size = new Size(194, 23);
            WeatherBox.TabIndex = 1;
            // 
            // MapBox
            // 
            MapBox.DropDownStyle = ComboBoxStyle.DropDownList;
            MapBox.FormattingEnabled = true;
            MapBox.Location = new Point(29, 104);
            MapBox.Name = "MapBox";
            MapBox.Size = new Size(194, 23);
            MapBox.TabIndex = 2;
            // 
            // MusicBox
            // 
            MusicBox.DropDownStyle = ComboBoxStyle.DropDownList;
            MusicBox.FormattingEnabled = true;
            MusicBox.Location = new Point(29, 162);
            MusicBox.Name = "MusicBox";
            MusicBox.Size = new Size(194, 23);
            MusicBox.TabIndex = 3;
            // 
            // EmblemsBox
            // 
            EmblemsBox.DropDownStyle = ComboBoxStyle.DropDownList;
            EmblemsBox.FormattingEnabled = true;
            EmblemsBox.Location = new Point(438, 46);
            EmblemsBox.Name = "EmblemsBox";
            EmblemsBox.Size = new Size(194, 23);
            EmblemsBox.TabIndex = 4;
            // 
            // ShaderBox
            // 
            ShaderBox.DropDownStyle = ComboBoxStyle.DropDownList;
            ShaderBox.FormattingEnabled = true;
            ShaderBox.Location = new Point(438, 104);
            ShaderBox.Name = "ShaderBox";
            ShaderBox.Size = new Size(194, 23);
            ShaderBox.TabIndex = 5;
            // 
            // weatherLabel
            // 
            weatherLabel.AutoSize = true;
            weatherLabel.Font = new Font("Segoe UI", 9F);
            weatherLabel.ForeColor = Color.FromArgb(200, 200, 200);
            weatherLabel.Location = new Point(29, 23);
            weatherLabel.Name = "weatherLabel";
            weatherLabel.Size = new Size(51, 15);
            weatherLabel.TabIndex = 0;
            weatherLabel.Text = "Weather";
            // 
            // terrainLabel
            // 
            terrainLabel.AutoSize = true;
            terrainLabel.Font = new Font("Segoe UI", 9F);
            terrainLabel.ForeColor = Color.FromArgb(200, 200, 200);
            terrainLabel.Location = new Point(29, 81);
            terrainLabel.Name = "terrainLabel";
            terrainLabel.Size = new Size(43, 15);
            terrainLabel.TabIndex = 1;
            terrainLabel.Text = "Terrain";
            // 
            // musicLabel
            // 
            musicLabel.AutoSize = true;
            musicLabel.Font = new Font("Segoe UI", 9F);
            musicLabel.ForeColor = Color.FromArgb(200, 200, 200);
            musicLabel.Location = new Point(29, 139);
            musicLabel.Name = "musicLabel";
            musicLabel.Size = new Size(39, 15);
            musicLabel.TabIndex = 2;
            musicLabel.Text = "Music";
            // 
            // emblemsLabel
            // 
            emblemsLabel.AutoSize = true;
            emblemsLabel.Font = new Font("Segoe UI", 9F);
            emblemsLabel.ForeColor = Color.FromArgb(200, 200, 200);
            emblemsLabel.Location = new Point(439, 23);
            emblemsLabel.Name = "emblemsLabel";
            emblemsLabel.Size = new Size(56, 15);
            emblemsLabel.TabIndex = 3;
            emblemsLabel.Text = "Emblems";
            // 
            // shaderLabel
            // 
            shaderLabel.AutoSize = true;
            shaderLabel.Font = new Font("Segoe UI", 9F);
            shaderLabel.ForeColor = Color.FromArgb(200, 200, 200);
            shaderLabel.Location = new Point(438, 81);
            shaderLabel.Name = "shaderLabel";
            shaderLabel.Size = new Size(48, 15);
            shaderLabel.TabIndex = 4;
            shaderLabel.Text = "Shaders";
            // 
            // radiantcreeplabel
            // 
            radiantcreeplabel.AutoSize = true;
            radiantcreeplabel.Font = new Font("Segoe UI", 9F);
            radiantcreeplabel.ForeColor = Color.FromArgb(200, 200, 200);
            radiantcreeplabel.Location = new Point(29, 236);
            radiantcreeplabel.Name = "radiantcreeplabel";
            radiantcreeplabel.Size = new Size(81, 15);
            radiantcreeplabel.TabIndex = 6;
            radiantcreeplabel.Text = "Radiant Creep";
            // 
            // RadiantCreepBox
            // 
            RadiantCreepBox.DropDownStyle = ComboBoxStyle.DropDownList;
            RadiantCreepBox.FormattingEnabled = true;
            RadiantCreepBox.Location = new Point(29, 259);
            RadiantCreepBox.Name = "RadiantCreepBox";
            RadiantCreepBox.Size = new Size(194, 23);
            RadiantCreepBox.TabIndex = 7;
            // 
            // direcreeplabel
            // 
            direcreeplabel.AutoSize = true;
            direcreeplabel.Font = new Font("Segoe UI", 9F);
            direcreeplabel.ForeColor = Color.FromArgb(200, 200, 200);
            direcreeplabel.Location = new Point(234, 236);
            direcreeplabel.Name = "direcreeplabel";
            direcreeplabel.Size = new Size(62, 15);
            direcreeplabel.TabIndex = 8;
            direcreeplabel.Text = "Dire Creep";
            // 
            // DireCreepBox
            // 
            DireCreepBox.DropDownStyle = ComboBoxStyle.DropDownList;
            DireCreepBox.FormattingEnabled = true;
            DireCreepBox.Location = new Point(234, 259);
            DireCreepBox.Name = "DireCreepBox";
            DireCreepBox.Size = new Size(194, 23);
            DireCreepBox.TabIndex = 9;
            // 
            // diresiegelabel
            // 
            diresiegelabel.AutoSize = true;
            diresiegelabel.Font = new Font("Segoe UI", 9F);
            diresiegelabel.ForeColor = Color.FromArgb(200, 200, 200);
            diresiegelabel.Location = new Point(234, 298);
            diresiegelabel.Name = "diresiegelabel";
            diresiegelabel.Size = new Size(59, 15);
            diresiegelabel.TabIndex = 12;
            diresiegelabel.Text = "Dire Siege";
            // 
            // DireSiegeBox
            // 
            DireSiegeBox.DropDownStyle = ComboBoxStyle.DropDownList;
            DireSiegeBox.FormattingEnabled = true;
            DireSiegeBox.Location = new Point(234, 321);
            DireSiegeBox.Name = "DireSiegeBox";
            DireSiegeBox.Size = new Size(194, 23);
            DireSiegeBox.TabIndex = 13;
            // 
            // radiantsiegelabel
            // 
            radiantsiegelabel.AutoSize = true;
            radiantsiegelabel.Font = new Font("Segoe UI", 9F);
            radiantsiegelabel.ForeColor = Color.FromArgb(200, 200, 200);
            radiantsiegelabel.Location = new Point(29, 298);
            radiantsiegelabel.Name = "radiantsiegelabel";
            radiantsiegelabel.Size = new Size(78, 15);
            radiantsiegelabel.TabIndex = 10;
            radiantsiegelabel.Text = "Radiant Siege";
            // 
            // RadiantSiegeBox
            // 
            RadiantSiegeBox.DropDownStyle = ComboBoxStyle.DropDownList;
            RadiantSiegeBox.FormattingEnabled = true;
            RadiantSiegeBox.Location = new Point(29, 321);
            RadiantSiegeBox.Name = "RadiantSiegeBox";
            RadiantSiegeBox.Size = new Size(194, 23);
            RadiantSiegeBox.TabIndex = 11;
            // 
            // dividerLabel
            // 
            dividerLabel.BackColor = Color.FromArgb(100, 100, 100);
            dividerLabel.Location = new Point(28, 209);
            dividerLabel.Name = "dividerLabel";
            dividerLabel.Size = new Size(604, 10);
            dividerLabel.TabIndex = 14;
            // 
            // atkModifier
            // 
            atkModifier.AutoSize = true;
            atkModifier.Font = new Font("Segoe UI", 9F);
            atkModifier.ForeColor = Color.FromArgb(200, 200, 200);
            atkModifier.Location = new Point(438, 139);
            atkModifier.Name = "atkModifier";
            atkModifier.Size = new Size(89, 15);
            atkModifier.TabIndex = 16;
            atkModifier.Text = "Attack Modifier";
            // 
            // atkModifierBox
            // 
            atkModifierBox.DropDownStyle = ComboBoxStyle.DropDownList;
            atkModifierBox.FormattingEnabled = true;
            atkModifierBox.Location = new Point(438, 162);
            atkModifierBox.Name = "atkModifierBox";
            atkModifierBox.Size = new Size(194, 23);
            atkModifierBox.TabIndex = 17;
            // 
            // ConsoleLogBox
            // 
            ConsoleLogBox.BackColor = Color.FromArgb(40, 40, 40);
            ConsoleLogBox.BorderStyle = BorderStyle.FixedSingle;
            ConsoleLogBox.DetectUrls = false;
            ConsoleLogBox.Font = new Font("Cascadia Code", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ConsoleLogBox.ForeColor = Color.FromArgb(200, 200, 200);
            ConsoleLogBox.Location = new Point(663, 46);
            ConsoleLogBox.Margin = new Padding(4, 3, 4, 3);
            ConsoleLogBox.Name = "ConsoleLogBox";
            ConsoleLogBox.ReadOnly = true;
            ConsoleLogBox.ScrollBars = RichTextBoxScrollBars.Vertical;
            ConsoleLogBox.Size = new Size(326, 362);
            ConsoleLogBox.TabIndex = 18;
            ConsoleLogBox.Text = "";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.ForeColor = Color.MintCream;
            label1.Location = new Point(785, 23);
            label1.Name = "label1";
            label1.Size = new Size(79, 15);
            label1.TabIndex = 19;
            label1.Text = "Console Log :";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F);
            label2.ForeColor = Color.FromArgb(200, 200, 200);
            label2.Location = new Point(234, 23);
            label2.Name = "label2";
            label2.Size = new Size(32, 15);
            label2.TabIndex = 20;
            label2.Text = "HUD";
            label2.Click += label2_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9F);
            label3.ForeColor = Color.FromArgb(200, 200, 200);
            label3.Location = new Point(233, 81);
            label3.Name = "label3";
            label3.Size = new Size(78, 15);
            label3.TabIndex = 21;
            label3.Text = "Versus Screen";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 9F);
            label4.ForeColor = Color.FromArgb(200, 200, 200);
            label4.Location = new Point(233, 139);
            label4.Name = "label4";
            label4.Size = new Size(55, 15);
            label4.TabIndex = 23;
            label4.Text = "River Vial";
            // 
            // RiverBox
            // 
            RiverBox.DropDownStyle = ComboBoxStyle.DropDownList;
            RiverBox.FormattingEnabled = true;
            RiverBox.Location = new Point(233, 162);
            RiverBox.Name = "RiverBox";
            RiverBox.Size = new Size(194, 23);
            RiverBox.TabIndex = 25;
            // 
            // VersusBox
            // 
            VersusBox.DropDownStyle = ComboBoxStyle.DropDownList;
            VersusBox.FormattingEnabled = true;
            VersusBox.Location = new Point(233, 104);
            VersusBox.Name = "VersusBox";
            VersusBox.Size = new Size(194, 23);
            VersusBox.TabIndex = 24;
            // 
            // HUDBox
            // 
            HUDBox.DropDownStyle = ComboBoxStyle.DropDownList;
            HUDBox.FormattingEnabled = true;
            HUDBox.Location = new Point(233, 46);
            HUDBox.Name = "HUDBox";
            HUDBox.Size = new Size(194, 23);
            HUDBox.TabIndex = 22;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 9F);
            label5.ForeColor = Color.FromArgb(200, 200, 200);
            label5.Location = new Point(233, 362);
            label5.Name = "label5";
            label5.Size = new Size(63, 15);
            label5.TabIndex = 28;
            label5.Text = "Dire Tower";
            // 
            // DireTowerBox
            // 
            DireTowerBox.DropDownStyle = ComboBoxStyle.DropDownList;
            DireTowerBox.FormattingEnabled = true;
            DireTowerBox.Location = new Point(233, 385);
            DireTowerBox.Name = "DireTowerBox";
            DireTowerBox.Size = new Size(194, 23);
            DireTowerBox.TabIndex = 29;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 9F);
            label6.ForeColor = Color.FromArgb(200, 200, 200);
            label6.Location = new Point(28, 362);
            label6.Name = "label6";
            label6.Size = new Size(82, 15);
            label6.TabIndex = 26;
            label6.Text = "Radiant Tower";
            label6.Click += label6_Click;
            // 
            // RadiantTowerBox
            // 
            RadiantTowerBox.DropDownStyle = ComboBoxStyle.DropDownList;
            RadiantTowerBox.FormattingEnabled = true;
            RadiantTowerBox.Location = new Point(28, 385);
            RadiantTowerBox.Name = "RadiantTowerBox";
            RadiantTowerBox.Size = new Size(194, 23);
            RadiantTowerBox.TabIndex = 27;
            // 
            // LoadPreset
            // 
            LoadPreset.BackColor = Color.FromArgb(58, 58, 58);
            LoadPreset.FlatAppearance.BorderSize = 0;
            LoadPreset.FlatStyle = FlatStyle.Flat;
            LoadPreset.Font = new Font("Segoe UI", 9F);
            LoadPreset.ForeColor = Color.FromArgb(200, 200, 200);
            LoadPreset.Location = new Point(483, 309);
            LoadPreset.Name = "LoadPreset";
            LoadPreset.Size = new Size(123, 31);
            LoadPreset.TabIndex = 30;
            LoadPreset.Text = "Load Preset";
            LoadPreset.UseVisualStyleBackColor = false;
            // 
            // SavePreset
            // 
            SavePreset.BackColor = Color.FromArgb(58, 58, 58);
            SavePreset.FlatAppearance.BorderSize = 0;
            SavePreset.FlatStyle = FlatStyle.Flat;
            SavePreset.Font = new Font("Segoe UI", 9F);
            SavePreset.ForeColor = Color.FromArgb(200, 200, 200);
            SavePreset.Location = new Point(483, 258);
            SavePreset.Name = "SavePreset";
            SavePreset.Size = new Size(123, 31);
            SavePreset.TabIndex = 31;
            SavePreset.Text = "Save Preset";
            SavePreset.UseVisualStyleBackColor = false;
            // 
            // MiscellaneousForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(45, 45, 45);
            ClientSize = new Size(1018, 439);
            Controls.Add(SavePreset);
            Controls.Add(LoadPreset);
            Controls.Add(label5);
            Controls.Add(DireTowerBox);
            Controls.Add(label6);
            Controls.Add(RadiantTowerBox);
            Controls.Add(label2);
            Controls.Add(label3);
            Controls.Add(label4);
            Controls.Add(RiverBox);
            Controls.Add(VersusBox);
            Controls.Add(HUDBox);
            Controls.Add(label1);
            Controls.Add(ConsoleLogBox);
            Controls.Add(atkModifier);
            Controls.Add(atkModifierBox);
            Controls.Add(dividerLabel);
            Controls.Add(diresiegelabel);
            Controls.Add(DireSiegeBox);
            Controls.Add(radiantsiegelabel);
            Controls.Add(RadiantSiegeBox);
            Controls.Add(direcreeplabel);
            Controls.Add(DireCreepBox);
            Controls.Add(radiantcreeplabel);
            Controls.Add(RadiantCreepBox);
            Controls.Add(shaderLabel);
            Controls.Add(emblemsLabel);
            Controls.Add(weatherLabel);
            Controls.Add(terrainLabel);
            Controls.Add(musicLabel);
            Controls.Add(ShaderBox);
            Controls.Add(EmblemsBox);
            Controls.Add(MusicBox);
            Controls.Add(MapBox);
            Controls.Add(WeatherBox);
            Controls.Add(generateButton);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "MiscellaneousForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = " ";
            Load += MiscellaneousForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.Button generateButton;
        private ComboBox WeatherBox;
        private ComboBox MapBox;
        private ComboBox MusicBox;
        private ComboBox EmblemsBox;
        private ComboBox ShaderBox;
        private Label weatherLabel;
        private Label terrainLabel;
        private Label musicLabel;
        private Label emblemsLabel;
        private Label shaderLabel;
        private Label radiantcreeplabel;
        private ComboBox RadiantCreepBox;
        private Label direcreeplabel;
        private ComboBox DireCreepBox;
        private Label diresiegelabel;
        private ComboBox DireSiegeBox;
        private Label radiantsiegelabel;
        private ComboBox RadiantSiegeBox;
        private Label dividerLabel;
        private Label atkModifier;
        private ComboBox atkModifierBox;
        private RichTextBox ConsoleLogBox; // Renamed from consoleLog to ConsoleLogBox
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private ComboBox RiverBox;
        private ComboBox VersusBox;
        private ComboBox HUDBox;
        private Label label5;
        private ComboBox DireTowerBox;
        private Label label6;
        private ComboBox RadiantTowerBox;
        private Button LoadPreset;
        private Button SavePreset;
    }
}