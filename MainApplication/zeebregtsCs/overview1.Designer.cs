namespace zeebregtsCs
{
    partial class overview1
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(overview1));
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.listView2 = new System.Windows.Forms.ListView();
			this.bedrijfformdataset1 = new zeebregtsCs.bedrijfformdataset();
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.panel2 = new System.Windows.Forms.Panel();
			this.lbl_mid = new System.Windows.Forms.Label();
			this.lbl_top = new System.Windows.Forms.Label();
			this.nieuw_btn = new System.Windows.Forms.Button();
			this.terug_btn = new System.Windows.Forms.Button();
			this.btnadvzoek = new System.Windows.Forms.Button();
			this.btn_gereed = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
			this.advancedFilter1 = new zeebregtsCs.AdvancedFilter();
			this.dubbel_q_panel = new System.Windows.Forms.Panel();
			this.panel4 = new System.Windows.Forms.Panel();
			this.lbl_dub_top = new System.Windows.Forms.Label();
			this.dubbel_inv_ja_btn = new System.Windows.Forms.Button();
			this.lbl_dub = new System.Windows.Forms.Label();
			this.dubbel_inv_nee_btn = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.bedrijfformdataset1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			this.panel2.SuspendLayout();
			this.panel1.SuspendLayout();
			this.dubbel_q_panel.SuspendLayout();
			this.panel4.SuspendLayout();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(-2, 0);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 0;
			this.button1.Text = "terug";
			this.button1.UseVisualStyleBackColor = true;
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(220, 0);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 23);
			this.button2.TabIndex = 1;
			this.button2.Text = "nieuw";
			this.button2.UseVisualStyleBackColor = true;
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(-2, 29);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(211, 20);
			this.textBox1.TabIndex = 2;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(215, 32);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(50, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "zoekveld";
			// 
			// listView2
			// 
			this.listView2.Location = new System.Drawing.Point(12, 55);
			this.listView2.Name = "listView2";
			this.listView2.Size = new System.Drawing.Size(253, 374);
			this.listView2.TabIndex = 4;
			this.listView2.UseCompatibleStateImageBehavior = false;
			// 
			// bedrijfformdataset1
			// 
			this.bedrijfformdataset1.DataSetName = "bedrijfformdataset";
			this.bedrijfformdataset1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
			// 
			// dataGridView1
			// 
			this.dataGridView1.AllowUserToAddRows = false;
			this.dataGridView1.AllowUserToDeleteRows = false;
			this.dataGridView1.AllowUserToResizeRows = false;
			dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
			this.dataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
			this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
			this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.dataGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.LightBlue;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
			this.dataGridView1.Location = new System.Drawing.Point(0, 210);
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.ReadOnly = true;
			this.dataGridView1.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
			dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
			dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
			this.dataGridView1.RowHeadersVisible = false;
			this.dataGridView1.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dataGridView1.Size = new System.Drawing.Size(1022, 531);
			this.dataGridView1.StandardTab = true;
			this.dataGridView1.TabIndex = 19;
			this.dataGridView1.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentDoubleClick);
			this.dataGridView1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.dataGridView1_Scroll);
			this.dataGridView1.Enter += new System.EventHandler(this.dataGridView1_Enter);
			this.dataGridView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridView1_KeyDown);
			this.dataGridView1.Leave += new System.EventHandler(this.dataGridView1_Leave);
			// 
			// panel2
			// 
			this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel2.Controls.Add(this.lbl_mid);
			this.panel2.Controls.Add(this.lbl_top);
			this.panel2.Controls.Add(this.nieuw_btn);
			this.panel2.Controls.Add(this.terug_btn);
			this.panel2.Controls.Add(this.btnadvzoek);
			this.panel2.Controls.Add(this.btn_gereed);
			this.panel2.Location = new System.Drawing.Point(0, 0);
			this.panel2.MaximumSize = new System.Drawing.Size(2000, 75);
			this.panel2.MinimumSize = new System.Drawing.Size(0, 75);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(1022, 75);
			this.panel2.TabIndex = 19;
			// 
			// lbl_mid
			// 
			this.lbl_mid.AutoEllipsis = true;
			this.lbl_mid.AutoSize = true;
			this.lbl_mid.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbl_mid.Location = new System.Drawing.Point(454, 30);
			this.lbl_mid.Name = "lbl_mid";
			this.lbl_mid.Size = new System.Drawing.Size(66, 24);
			this.lbl_mid.TabIndex = 7;
			this.lbl_mid.Text = "label3";
			// 
			// lbl_top
			// 
			this.lbl_top.AutoEllipsis = true;
			this.lbl_top.AutoSize = true;
			this.lbl_top.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbl_top.Location = new System.Drawing.Point(453, -1);
			this.lbl_top.Name = "lbl_top";
			this.lbl_top.Size = new System.Drawing.Size(0, 13);
			this.lbl_top.TabIndex = 6;
			// 
			// nieuw_btn
			// 
			this.nieuw_btn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.nieuw_btn.BackColor = System.Drawing.Color.Transparent;
			this.nieuw_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.nieuw_btn.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
			this.nieuw_btn.FlatAppearance.BorderSize = 0;
			this.nieuw_btn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.nieuw_btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.nieuw_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.nieuw_btn.Image = global::zeebregtsCs.Properties.Resources.Nieuw2;
			this.nieuw_btn.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.nieuw_btn.Location = new System.Drawing.Point(944, 18);
			this.nieuw_btn.Name = "nieuw_btn";
			this.nieuw_btn.Size = new System.Drawing.Size(71, 35);
			this.nieuw_btn.TabIndex = 1;
			this.nieuw_btn.TabStop = false;
			this.nieuw_btn.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			this.nieuw_btn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.nieuw_btn.UseVisualStyleBackColor = false;
			this.nieuw_btn.Click += new System.EventHandler(this.nieuw_btn_Click);
			// 
			// terug_btn
			// 
			this.terug_btn.BackColor = System.Drawing.SystemColors.Control;
			this.terug_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.terug_btn.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
			this.terug_btn.FlatAppearance.BorderSize = 0;
			this.terug_btn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.terug_btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.terug_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.terug_btn.Image = global::zeebregtsCs.Properties.Resources.Menu2;
			this.terug_btn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.terug_btn.Location = new System.Drawing.Point(3, 18);
			this.terug_btn.Name = "terug_btn";
			this.terug_btn.Size = new System.Drawing.Size(101, 38);
			this.terug_btn.TabIndex = 0;
			this.terug_btn.TabStop = false;
			this.terug_btn.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			this.terug_btn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.terug_btn.UseVisualStyleBackColor = false;
			this.terug_btn.Click += new System.EventHandler(this.terug_btn_Click);
			// 
			// btnadvzoek
			// 
			this.btnadvzoek.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnadvzoek.BackColor = System.Drawing.Color.Transparent;
			this.btnadvzoek.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.btnadvzoek.FlatAppearance.BorderSize = 0;
			this.btnadvzoek.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnadvzoek.Location = new System.Drawing.Point(867, 11);
			this.btnadvzoek.Name = "btnadvzoek";
			this.btnadvzoek.Size = new System.Drawing.Size(138, 55);
			this.btnadvzoek.TabIndex = 5;
			this.btnadvzoek.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			this.btnadvzoek.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.btnadvzoek.UseVisualStyleBackColor = false;
			this.btnadvzoek.Visible = false;
			this.btnadvzoek.Click += new System.EventHandler(this.btnadvzoek_Click);
			// 
			// btn_gereed
			// 
			this.btn_gereed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btn_gereed.BackColor = System.Drawing.Color.Transparent;
			this.btn_gereed.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.btn_gereed.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
			this.btn_gereed.FlatAppearance.BorderSize = 0;
			this.btn_gereed.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.btn_gereed.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.btn_gereed.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn_gereed.Image = global::zeebregtsCs.Properties.Resources.Gereed21;
			this.btn_gereed.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.btn_gereed.Location = new System.Drawing.Point(933, 18);
			this.btn_gereed.Name = "btn_gereed";
			this.btn_gereed.Size = new System.Drawing.Size(82, 38);
			this.btn_gereed.TabIndex = 8;
			this.btn_gereed.TabStop = false;
			this.btn_gereed.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			this.btn_gereed.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.btn_gereed.UseVisualStyleBackColor = false;
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel1.Controls.Add(this.elementHost1);
			this.panel1.Controls.Add(this.dubbel_q_panel);
			this.panel1.Location = new System.Drawing.Point(0, 74);
			this.panel1.MinimumSize = new System.Drawing.Size(0, 130);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(1022, 130);
			this.panel1.TabIndex = 9;
			// 
			// elementHost1
			// 
			this.elementHost1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.elementHost1.Location = new System.Drawing.Point(0, 0);
			this.elementHost1.Name = "elementHost1";
			this.elementHost1.Size = new System.Drawing.Size(1018, 126);
			this.elementHost1.TabIndex = 3;
			this.elementHost1.Text = "elementHost1";
			this.elementHost1.Child = this.advancedFilter1;
			// 
			// dubbel_q_panel
			// 
			this.dubbel_q_panel.BackColor = System.Drawing.SystemColors.Control;
			this.dubbel_q_panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.dubbel_q_panel.Controls.Add(this.panel4);
			this.dubbel_q_panel.Controls.Add(this.dubbel_inv_ja_btn);
			this.dubbel_q_panel.Controls.Add(this.lbl_dub);
			this.dubbel_q_panel.Controls.Add(this.dubbel_inv_nee_btn);
			this.dubbel_q_panel.Location = new System.Drawing.Point(171, 6);
			this.dubbel_q_panel.Name = "dubbel_q_panel";
			this.dubbel_q_panel.Size = new System.Drawing.Size(668, 117);
			this.dubbel_q_panel.TabIndex = 6;
			// 
			// panel4
			// 
			this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(161)))), ((int)(((byte)(161)))), ((int)(((byte)(161)))));
			this.panel4.Controls.Add(this.lbl_dub_top);
			this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel4.Location = new System.Drawing.Point(0, 0);
			this.panel4.Name = "panel4";
			this.panel4.Size = new System.Drawing.Size(666, 26);
			this.panel4.TabIndex = 5;
			// 
			// lbl_dub_top
			// 
			this.lbl_dub_top.AutoSize = true;
			this.lbl_dub_top.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbl_dub_top.Location = new System.Drawing.Point(301, 5);
			this.lbl_dub_top.Name = "lbl_dub_top";
			this.lbl_dub_top.Size = new System.Drawing.Size(39, 13);
			this.lbl_dub_top.TabIndex = 5;
			this.lbl_dub_top.Text = "dubbel";
			this.lbl_dub_top.Visible = false;
			// 
			// dubbel_inv_ja_btn
			// 
			this.dubbel_inv_ja_btn.Location = new System.Drawing.Point(222, 83);
			this.dubbel_inv_ja_btn.Name = "dubbel_inv_ja_btn";
			this.dubbel_inv_ja_btn.Size = new System.Drawing.Size(75, 23);
			this.dubbel_inv_ja_btn.TabIndex = 4;
			this.dubbel_inv_ja_btn.Text = "JA";
			this.dubbel_inv_ja_btn.UseVisualStyleBackColor = true;
			this.dubbel_inv_ja_btn.Visible = false;
			this.dubbel_inv_ja_btn.Click += new System.EventHandler(this.dubbel_inv_ja_btn_Click);
			// 
			// lbl_dub
			// 
			this.lbl_dub.AutoSize = true;
			this.lbl_dub.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbl_dub.Location = new System.Drawing.Point(300, 54);
			this.lbl_dub.Name = "lbl_dub";
			this.lbl_dub.Size = new System.Drawing.Size(57, 20);
			this.lbl_dub.TabIndex = 0;
			this.lbl_dub.Text = "dubbel";
			this.lbl_dub.Visible = false;
			// 
			// dubbel_inv_nee_btn
			// 
			this.dubbel_inv_nee_btn.Location = new System.Drawing.Point(303, 83);
			this.dubbel_inv_nee_btn.Name = "dubbel_inv_nee_btn";
			this.dubbel_inv_nee_btn.Size = new System.Drawing.Size(75, 23);
			this.dubbel_inv_nee_btn.TabIndex = 1;
			this.dubbel_inv_nee_btn.Text = "NEE";
			this.dubbel_inv_nee_btn.UseVisualStyleBackColor = true;
			this.dubbel_inv_nee_btn.Visible = false;
			this.dubbel_inv_nee_btn.Click += new System.EventHandler(this.dubbel_inv_nee_btn_Click);
			// 
			// overview1
			// 
			this.AutoScrollMargin = new System.Drawing.Size(0, 10);
			this.ClientSize = new System.Drawing.Size(1022, 741);
			this.Controls.Add(this.dataGridView1);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.Name = "overview1";
			this.Text = "overzicht";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.overview1_FormClosing);
			this.Load += new System.EventHandler(this.overview1_Load);
			this.Resize += new System.EventHandler(this.overview1_Resize);
			((System.ComponentModel.ISupportInitialize)(this.bedrijfformdataset1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.dubbel_q_panel.ResumeLayout(false);
			this.dubbel_q_panel.PerformLayout();
			this.panel4.ResumeLayout(false);
			this.panel4.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView listView2;
        private bedrijfformdataset bedrijfformdataset1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lbl_mid;
        private System.Windows.Forms.Label lbl_top;
        private System.Windows.Forms.Button nieuw_btn;
        private System.Windows.Forms.Button terug_btn;
        private System.Windows.Forms.Button btnadvzoek;
        private System.Windows.Forms.Button btn_gereed;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lbl_dub;
        private System.Windows.Forms.Button dubbel_inv_nee_btn;
        private System.Windows.Forms.Integration.ElementHost elementHost1;
        private AdvancedFilter advancedFilter1;
        private System.Windows.Forms.Button dubbel_inv_ja_btn;
        private System.Windows.Forms.Label lbl_dub_top;
        private System.Windows.Forms.Panel dubbel_q_panel;
        private System.Windows.Forms.Panel panel4;
    }
}