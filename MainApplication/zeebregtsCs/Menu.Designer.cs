namespace zeebregtsCs
{
    partial class Menu
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Menu));
            this.panel2 = new System.Windows.Forms.Panel();
            this.labels7 = new System.Windows.Forms.Label();
            this.monitor_btn = new System.Windows.Forms.Button();
            this.labels5 = new System.Windows.Forms.Label();
            this.labels3 = new System.Windows.Forms.Label();
            this.labels2 = new System.Windows.Forms.Label();
            this.labels1 = new System.Windows.Forms.Label();
            this.labels0 = new System.Windows.Forms.Label();
            this.planning_btn = new System.Windows.Forms.Button();
            this.bedrijf_btn = new System.Windows.Forms.Button();
            this.project_btn = new System.Windows.Forms.Button();
            this.settings_btn = new System.Windows.Forms.Button();
            this.persoon_btn = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.AutoSize = true;
            this.panel2.BackColor = System.Drawing.SystemColors.Control;
            this.panel2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.button1);
            this.panel2.Controls.Add(this.labels7);
            this.panel2.Controls.Add(this.monitor_btn);
            this.panel2.Controls.Add(this.labels5);
            this.panel2.Controls.Add(this.labels3);
            this.panel2.Controls.Add(this.labels2);
            this.panel2.Controls.Add(this.labels1);
            this.panel2.Controls.Add(this.labels0);
            this.panel2.Controls.Add(this.planning_btn);
            this.panel2.Controls.Add(this.bedrijf_btn);
            this.panel2.Controls.Add(this.project_btn);
            this.panel2.Controls.Add(this.settings_btn);
            this.panel2.Controls.Add(this.persoon_btn);
            this.panel2.Location = new System.Drawing.Point(0, 75);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1024, 667);
            this.panel2.TabIndex = 7;
            // 
            // labels7
            // 
            this.labels7.AutoSize = true;
            this.labels7.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labels7.Location = new System.Drawing.Point(439, 261);
            this.labels7.Name = "labels7";
            this.labels7.Size = new System.Drawing.Size(46, 17);
            this.labels7.TabIndex = 15;
            this.labels7.Text = "label2";
            this.labels7.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // monitor_btn
            // 
            this.monitor_btn.BackgroundImage = global::zeebregtsCs.Properties.Resources.Iftab;
            this.monitor_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.monitor_btn.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::zeebregtsCs.Properties.Settings.Default, "mo_btn_loc", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.monitor_btn.Location = global::zeebregtsCs.Properties.Settings.Default.mo_btn_loc;
            this.monitor_btn.Name = "monitor_btn";
            this.monitor_btn.Padding = new System.Windows.Forms.Padding(25);
            this.monitor_btn.Size = new System.Drawing.Size(70, 70);
            this.monitor_btn.TabIndex = 14;
            this.monitor_btn.UseVisualStyleBackColor = true;
            this.monitor_btn.LocationChanged += new System.EventHandler(this.btn_LocationChanged);
            this.monitor_btn.Click += new System.EventHandler(this.monitor_btn_Click);
            this.monitor_btn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.control_MouseDown);
            this.monitor_btn.MouseMove += new System.Windows.Forms.MouseEventHandler(this.control_MouseMove);
            this.monitor_btn.MouseUp += new System.Windows.Forms.MouseEventHandler(this.control_MouseUp);
            // 
            // labels5
            // 
            this.labels5.AutoSize = true;
            this.labels5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labels5.Location = new System.Drawing.Point(285, 274);
            this.labels5.Name = "labels5";
            this.labels5.Size = new System.Drawing.Size(46, 17);
            this.labels5.TabIndex = 13;
            this.labels5.Text = "label2";
            this.labels5.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // labels3
            // 
            this.labels3.AutoSize = true;
            this.labels3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labels3.Location = new System.Drawing.Point(285, 248);
            this.labels3.Name = "labels3";
            this.labels3.Size = new System.Drawing.Size(46, 17);
            this.labels3.TabIndex = 11;
            this.labels3.Text = "label2";
            this.labels3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // labels2
            // 
            this.labels2.AutoSize = true;
            this.labels2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labels2.Location = new System.Drawing.Point(285, 235);
            this.labels2.Name = "labels2";
            this.labels2.Size = new System.Drawing.Size(46, 17);
            this.labels2.TabIndex = 10;
            this.labels2.Text = "label2";
            this.labels2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // labels1
            // 
            this.labels1.AutoSize = true;
            this.labels1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labels1.Location = new System.Drawing.Point(285, 223);
            this.labels1.Name = "labels1";
            this.labels1.Size = new System.Drawing.Size(46, 17);
            this.labels1.TabIndex = 9;
            this.labels1.Text = "label2";
            this.labels1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // labels0
            // 
            this.labels0.AutoSize = true;
            this.labels0.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labels0.Location = new System.Drawing.Point(285, 210);
            this.labels0.Name = "labels0";
            this.labels0.Size = new System.Drawing.Size(46, 17);
            this.labels0.TabIndex = 8;
            this.labels0.Text = "label2";
            this.labels0.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // planning_btn
            // 
            this.planning_btn.BackgroundImage = global::zeebregtsCs.Properties.Resources.Pl;
            this.planning_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.planning_btn.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::zeebregtsCs.Properties.Settings.Default, "pl_btn_loc", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.planning_btn.Location = global::zeebregtsCs.Properties.Settings.Default.pl_btn_loc;
            this.planning_btn.Name = "planning_btn";
            this.planning_btn.Padding = new System.Windows.Forms.Padding(25);
            this.planning_btn.Size = new System.Drawing.Size(70, 70);
            this.planning_btn.TabIndex = 7;
            this.planning_btn.UseVisualStyleBackColor = true;
            this.planning_btn.Visible = false;
            this.planning_btn.LocationChanged += new System.EventHandler(this.btn_LocationChanged);
            this.planning_btn.Click += new System.EventHandler(this.planning_btn_Click);
            this.planning_btn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.control_MouseDown);
            this.planning_btn.MouseMove += new System.Windows.Forms.MouseEventHandler(this.control_MouseMove);
            this.planning_btn.MouseUp += new System.Windows.Forms.MouseEventHandler(this.control_MouseUp);
            // 
            // bedrijf_btn
            // 
            this.bedrijf_btn.BackColor = System.Drawing.Color.Gainsboro;
            this.bedrijf_btn.BackgroundImage = global::zeebregtsCs.Properties.Resources.Bekleur;
            this.bedrijf_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.bedrijf_btn.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::zeebregtsCs.Properties.Settings.Default, "be_btn_loc", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.bedrijf_btn.FlatAppearance.BorderSize = 0;
            this.bedrijf_btn.Location = global::zeebregtsCs.Properties.Settings.Default.be_btn_loc;
            this.bedrijf_btn.Name = "bedrijf_btn";
            this.bedrijf_btn.Padding = new System.Windows.Forms.Padding(25);
            this.bedrijf_btn.Size = new System.Drawing.Size(70, 70);
            this.bedrijf_btn.TabIndex = 2;
            this.bedrijf_btn.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.bedrijf_btn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.bedrijf_btn.UseVisualStyleBackColor = false;
            this.bedrijf_btn.LocationChanged += new System.EventHandler(this.btn_LocationChanged);
            this.bedrijf_btn.Click += new System.EventHandler(this.bedrijf_btn_Click);
            this.bedrijf_btn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.control_MouseDown);
            this.bedrijf_btn.MouseMove += new System.Windows.Forms.MouseEventHandler(this.control_MouseMove);
            this.bedrijf_btn.MouseUp += new System.Windows.Forms.MouseEventHandler(this.control_MouseUp);
            // 
            // project_btn
            // 
            this.project_btn.BackColor = System.Drawing.Color.Gainsboro;
            this.project_btn.BackgroundImage = global::zeebregtsCs.Properties.Resources.Prkleur;
            this.project_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.project_btn.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::zeebregtsCs.Properties.Settings.Default, "pr_btn_loc", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.project_btn.Location = global::zeebregtsCs.Properties.Settings.Default.pr_btn_loc;
            this.project_btn.Name = "project_btn";
            this.project_btn.Padding = new System.Windows.Forms.Padding(25);
            this.project_btn.Size = new System.Drawing.Size(70, 70);
            this.project_btn.TabIndex = 1;
            this.project_btn.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.project_btn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.project_btn.UseVisualStyleBackColor = false;
            this.project_btn.LocationChanged += new System.EventHandler(this.btn_LocationChanged);
            this.project_btn.Click += new System.EventHandler(this.project_btn_Click);
            this.project_btn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.control_MouseDown);
            this.project_btn.MouseMove += new System.Windows.Forms.MouseEventHandler(this.control_MouseMove);
            this.project_btn.MouseUp += new System.Windows.Forms.MouseEventHandler(this.control_MouseUp);
            // 
            // settings_btn
            // 
            this.settings_btn.BackColor = System.Drawing.Color.Gainsboro;
            this.settings_btn.BackgroundImage = global::zeebregtsCs.Properties.Resources.Inkleur;
            this.settings_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.settings_btn.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::zeebregtsCs.Properties.Settings.Default, "se_btn_loc", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.settings_btn.Location = global::zeebregtsCs.Properties.Settings.Default.se_btn_loc;
            this.settings_btn.Name = "settings_btn";
            this.settings_btn.Padding = new System.Windows.Forms.Padding(25);
            this.settings_btn.Size = new System.Drawing.Size(70, 70);
            this.settings_btn.TabIndex = 6;
            this.settings_btn.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.settings_btn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.settings_btn.UseVisualStyleBackColor = false;
            this.settings_btn.LocationChanged += new System.EventHandler(this.btn_LocationChanged);
            this.settings_btn.Click += new System.EventHandler(this.settings_btn_Click);
            this.settings_btn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.control_MouseDown);
            this.settings_btn.MouseMove += new System.Windows.Forms.MouseEventHandler(this.control_MouseMove);
            this.settings_btn.MouseUp += new System.Windows.Forms.MouseEventHandler(this.control_MouseUp);
            // 
            // persoon_btn
            // 
            this.persoon_btn.BackColor = System.Drawing.Color.Gainsboro;
            this.persoon_btn.BackgroundImage = global::zeebregtsCs.Properties.Resources.Cokleur;
            this.persoon_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.persoon_btn.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::zeebregtsCs.Properties.Settings.Default, "pe_btn_loc", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.persoon_btn.Location = global::zeebregtsCs.Properties.Settings.Default.pe_btn_loc;
            this.persoon_btn.Name = "persoon_btn";
            this.persoon_btn.Padding = new System.Windows.Forms.Padding(25);
            this.persoon_btn.Size = new System.Drawing.Size(70, 70);
            this.persoon_btn.TabIndex = 3;
            this.persoon_btn.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.persoon_btn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.persoon_btn.UseVisualStyleBackColor = false;
            this.persoon_btn.LocationChanged += new System.EventHandler(this.btn_LocationChanged);
            this.persoon_btn.Click += new System.EventHandler(this.persoon_btn_Click);
            this.persoon_btn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.control_MouseDown);
            this.persoon_btn.MouseMove += new System.Windows.Forms.MouseEventHandler(this.control_MouseMove);
            this.persoon_btn.MouseUp += new System.Windows.Forms.MouseEventHandler(this.control_MouseUp);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1024, 75);
            this.panel1.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(472, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "Menu";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(476, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(70, 70);
            this.button1.TabIndex = 16;
            this.button1.Text = "Project Folders";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Menu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gainsboro;
            this.ClientSize = new System.Drawing.Size(1024, 742);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Menu";
            this.Text = "Menu";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Menu_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.Menu_Resize);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button project_btn;
        private System.Windows.Forms.Button persoon_btn;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button settings_btn;
        private System.Windows.Forms.Button planning_btn;
        private System.Windows.Forms.Button bedrijf_btn;
        private System.Windows.Forms.Label labels5;
        private System.Windows.Forms.Label labels3;
        private System.Windows.Forms.Label labels2;
        private System.Windows.Forms.Label labels1;
        private System.Windows.Forms.Label labels0;
        private System.Windows.Forms.Label labels7;
        private System.Windows.Forms.Button monitor_btn;
        private System.Windows.Forms.Button button1;
    }
}

