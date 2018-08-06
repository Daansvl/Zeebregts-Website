namespace MDR2PDF
{
    partial class USMarioStart
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(USMarioStart));
            this.label1 = new System.Windows.Forms.Label();
            this.gbLijsten = new System.Windows.Forms.GroupBox();
            this.txbAdminInfo = new System.Windows.Forms.TextBox();
            this.lbLijsten = new System.Windows.Forms.ListBox();
            this.btnSettings = new System.Windows.Forms.Button();
            this.pnlSettings = new System.Windows.Forms.Panel();
            this.gbManager = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnLijsten = new System.Windows.Forms.Button();
            this.cbbGebruikers = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.txbOmschrijving = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.gbLijst = new System.Windows.Forms.Panel();
            this.tableLayoutPanelLijsten = new System.Windows.Forms.TableLayoutPanel();
            this.btSave = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.dataGridViewLijsten = new System.Windows.Forms.DataGridView();
            this.gbLijsten.SuspendLayout();
            this.pnlSettings.SuspendLayout();
            this.gbManager.SuspendLayout();
            this.gbLijst.SuspendLayout();
            this.tableLayoutPanelLijsten.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewLijsten)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(67, 58);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(167, 53);
            this.label1.TabIndex = 0;
            this.label1.Text = "DIT IS DE STARTPAGINA";
            this.label1.Visible = false;
            // 
            // gbLijsten
            // 
            this.gbLijsten.BackColor = System.Drawing.SystemColors.Control;
            this.gbLijsten.Controls.Add(this.txbAdminInfo);
            this.gbLijsten.Controls.Add(this.lbLijsten);
            this.gbLijsten.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbLijsten.Location = new System.Drawing.Point(68, 111);
            this.gbLijsten.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gbLijsten.Name = "gbLijsten";
            this.gbLijsten.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gbLijsten.Size = new System.Drawing.Size(620, 176);
            this.gbLijsten.TabIndex = 69;
            this.gbLijsten.TabStop = false;
            this.gbLijsten.Text = "Lijsten";
            // 
            // txbAdminInfo
            // 
            this.txbAdminInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txbAdminInfo.Enabled = false;
            this.txbAdminInfo.Location = new System.Drawing.Point(71, 143);
            this.txbAdminInfo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txbAdminInfo.Name = "txbAdminInfo";
            this.txbAdminInfo.Size = new System.Drawing.Size(479, 16);
            this.txbAdminInfo.TabIndex = 68;
            this.txbAdminInfo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txbAdminInfo.Visible = false;
            // 
            // lbLijsten
            // 
            this.lbLijsten.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbLijsten.FormattingEnabled = true;
            this.lbLijsten.ItemHeight = 17;
            this.lbLijsten.Items.AddRange(new object[] {
            "1a\tWeeklijst intern\tVakmannen per project per week\tVakmannen: per project, per we" +
                "ek, per dag",
            "*3\tVakmannen per project\t\tVakmannen: per project, per periode naar keuze",
            "*4\tVakmannen per periode\t\tVakmannen: per periode naar keuze (optioneel incl. NAW " +
                "gegevens)",
            "*5\tProjecten per uitvoerder\tBezetting per uitvoerder per project\tBezetting: per u" +
                "itvoerder, per project, per periode naar keuze",
            "*8\tBezetting per week \t\tBezetting per week (optioneel per uitvoerder, per week)"});
            this.lbLijsten.Location = new System.Drawing.Point(180, 23);
            this.lbLijsten.Margin = new System.Windows.Forms.Padding(4, 4, 4, 0);
            this.lbLijsten.Name = "lbLijsten";
            this.lbLijsten.Size = new System.Drawing.Size(307, 4);
            this.lbLijsten.TabIndex = 67;
            this.lbLijsten.SelectedIndexChanged += new System.EventHandler(this.lbLijsten_SelectedIndexChanged);
            this.lbLijsten.MouseMove += new System.Windows.Forms.MouseEventHandler(this.listBoxAlgemeeen_MouseHover);
            // 
            // btnSettings
            // 
            this.btnSettings.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSettings.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSettings.BackgroundImage")));
            this.btnSettings.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnSettings.Location = new System.Drawing.Point(627, 241);
            this.btnSettings.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(37, 34);
            this.btnSettings.TabIndex = 77;
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Visible = false;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // pnlSettings
            // 
            this.pnlSettings.BackColor = System.Drawing.SystemColors.Window;
            this.pnlSettings.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlSettings.Controls.Add(this.gbManager);
            this.pnlSettings.Location = new System.Drawing.Point(391, 239);
            this.pnlSettings.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlSettings.Name = "pnlSettings";
            this.pnlSettings.Size = new System.Drawing.Size(277, 237);
            this.pnlSettings.TabIndex = 74;
            this.pnlSettings.Visible = false;
            this.pnlSettings.MouseLeave += new System.EventHandler(this.pnlSettings_MouseLeave);
            this.pnlSettings.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlSettings_MouseMove);
            // 
            // gbManager
            // 
            this.gbManager.Controls.Add(this.label3);
            this.gbManager.Controls.Add(this.btnLijsten);
            this.gbManager.Controls.Add(this.label1);
            this.gbManager.Controls.Add(this.cbbGebruikers);
            this.gbManager.Controls.Add(this.label4);
            this.gbManager.Controls.Add(this.label2);
            this.gbManager.Controls.Add(this.comboBox1);
            this.gbManager.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.gbManager.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbManager.Location = new System.Drawing.Point(4, 4);
            this.gbManager.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gbManager.Name = "gbManager";
            this.gbManager.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gbManager.Size = new System.Drawing.Size(267, 228);
            this.gbManager.TabIndex = 75;
            this.gbManager.TabStop = false;
            this.gbManager.Text = "Settings";
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(8, 212);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(259, 16);
            this.label3.TabIndex = 80;
            this.label3.Text = "label3";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnLijsten
            // 
            this.btnLijsten.Location = new System.Drawing.Point(52, 170);
            this.btnLijsten.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnLijsten.Name = "btnLijsten";
            this.btnLijsten.Size = new System.Drawing.Size(115, 33);
            this.btnLijsten.TabIndex = 79;
            this.btnLijsten.Text = "Lijsten";
            this.btnLijsten.UseVisualStyleBackColor = true;
            this.btnLijsten.Click += new System.EventHandler(this.btnLijsten_Click);
            // 
            // cbbGebruikers
            // 
            this.cbbGebruikers.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbbGebruikers.FormattingEnabled = true;
            this.cbbGebruikers.Location = new System.Drawing.Point(145, 28);
            this.cbbGebruikers.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cbbGebruikers.Name = "cbbGebruikers";
            this.cbbGebruikers.Size = new System.Drawing.Size(20, 25);
            this.cbbGebruikers.TabIndex = 78;
            this.cbbGebruikers.DropDown += new System.EventHandler(this.cbbGebruikers_DropDown);
            this.cbbGebruikers.SelectedIndexChanged += new System.EventHandler(this.cbbGebruikers_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(55, 32);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 17);
            this.label4.TabIndex = 77;
            this.label4.Text = "Gebruiker";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(55, 111);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 17);
            this.label2.TabIndex = 77;
            this.label2.Text = "Omgeving";
            // 
            // comboBox1
            // 
            this.comboBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Beta",
            "DEV",
            "* LIVE *"});
            this.comboBox1.Location = new System.Drawing.Point(52, 130);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(113, 25);
            this.comboBox1.TabIndex = 76;
            this.comboBox1.Text = "- Select - ";
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged_1);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(171, 427);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(152, 28);
            this.button1.TabIndex = 78;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txbOmschrijving
            // 
            this.txbOmschrijving.Enabled = false;
            this.txbOmschrijving.Location = new System.Drawing.Point(68, 0);
            this.txbOmschrijving.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txbOmschrijving.Name = "txbOmschrijving";
            this.txbOmschrijving.Size = new System.Drawing.Size(608, 22);
            this.txbOmschrijving.TabIndex = 72;
            this.txbOmschrijving.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txbOmschrijving.Visible = false;
            // 
            // button2
            // 
            this.button2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button2.BackgroundImage")));
            this.button2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button2.Location = new System.Drawing.Point(669, 32);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(37, 34);
            this.button2.TabIndex = 77;
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // button3
            // 
            this.button3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button3.BackgroundImage")));
            this.button3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button3.Location = new System.Drawing.Point(33, 32);
            this.button3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(37, 34);
            this.button3.TabIndex = 77;
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Visible = false;
            this.button3.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            this.contextMenuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStrip1_ItemClicked);
            // 
            // gbLijst
            // 
            this.gbLijst.AutoSize = true;
            this.gbLijst.Controls.Add(this.tableLayoutPanelLijsten);
            this.gbLijst.Location = new System.Drawing.Point(4, 129);
            this.gbLijst.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gbLijst.Name = "gbLijst";
            this.gbLijst.Size = new System.Drawing.Size(673, 347);
            this.gbLijst.TabIndex = 69;
            this.gbLijst.Visible = false;
            // 
            // tableLayoutPanelLijsten
            // 
            this.tableLayoutPanelLijsten.AutoSize = true;
            this.tableLayoutPanelLijsten.ColumnCount = 2;
            this.tableLayoutPanelLijsten.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelLijsten.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 153F));
            this.tableLayoutPanelLijsten.Controls.Add(this.btSave, 1, 1);
            this.tableLayoutPanelLijsten.Controls.Add(this.btnLoad, 0, 1);
            this.tableLayoutPanelLijsten.Controls.Add(this.dataGridViewLijsten, 0, 0);
            this.tableLayoutPanelLijsten.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelLijsten.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelLijsten.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tableLayoutPanelLijsten.Name = "tableLayoutPanelLijsten";
            this.tableLayoutPanelLijsten.RowCount = 2;
            this.tableLayoutPanelLijsten.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLijsten.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelLijsten.Size = new System.Drawing.Size(673, 347);
            this.tableLayoutPanelLijsten.TabIndex = 79;
            // 
            // btSave
            // 
            this.btSave.Location = new System.Drawing.Point(524, 258);
            this.btSave.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btSave.Name = "btSave";
            this.btSave.Size = new System.Drawing.Size(145, 28);
            this.btSave.TabIndex = 1;
            this.btSave.Text = "Save";
            this.btSave.UseVisualStyleBackColor = true;
            this.btSave.Click += new System.EventHandler(this.btSave_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(4, 258);
            this.btnLoad.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(152, 28);
            this.btnLoad.TabIndex = 2;
            this.btnLoad.Text = "Cancel";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // dataGridViewLijsten
            // 
            this.dataGridViewLijsten.AllowUserToOrderColumns = true;
            this.dataGridViewLijsten.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewLijsten.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGridViewLijsten.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dataGridViewLijsten.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tableLayoutPanelLijsten.SetColumnSpan(this.dataGridViewLijsten, 2);
            this.dataGridViewLijsten.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewLijsten.Location = new System.Drawing.Point(4, 4);
            this.dataGridViewLijsten.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dataGridViewLijsten.MaximumSize = new System.Drawing.Size(0, 369);
            this.dataGridViewLijsten.MinimumSize = new System.Drawing.Size(0, 62);
            this.dataGridViewLijsten.MultiSelect = false;
            this.dataGridViewLijsten.Name = "dataGridViewLijsten";
            this.dataGridViewLijsten.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewLijsten.Size = new System.Drawing.Size(665, 246);
            this.dataGridViewLijsten.TabIndex = 0;
            // 
            // USMarioStart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnSettings);
            this.Controls.Add(this.pnlSettings);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.gbLijsten);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txbOmschrijving);
            this.Controls.Add(this.gbLijst);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MinimumSize = new System.Drawing.Size(709, 498);
            this.Name = "USMarioStart";
            this.Size = new System.Drawing.Size(711, 500);
            this.gbLijsten.ResumeLayout(false);
            this.gbLijsten.PerformLayout();
            this.pnlSettings.ResumeLayout(false);
            this.gbManager.ResumeLayout(false);
            this.gbManager.PerformLayout();
            this.gbLijst.ResumeLayout(false);
            this.gbLijst.PerformLayout();
            this.tableLayoutPanelLijsten.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewLijsten)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox gbLijsten;
        private System.Windows.Forms.ListBox lbLijsten;
        private System.Windows.Forms.Panel pnlSettings;
        private System.Windows.Forms.GroupBox gbManager;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.ComboBox cbbGebruikers;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txbOmschrijving;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TextBox txbAdminInfo;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Button btnLijsten;
        private System.Windows.Forms.Panel gbLijst;
        private System.Windows.Forms.DataGridView dataGridViewLijsten;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btSave;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelLijsten;
        private System.Windows.Forms.Label label3;

    }
}
