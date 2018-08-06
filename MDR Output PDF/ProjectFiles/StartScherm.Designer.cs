namespace MDR2PDF
{
    partial class StartScherm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StartScherm));
            this.btnTerug = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btVerder = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.SuspendLayout();
            // 
            // btnTerug
            // 
            this.btnTerug.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnTerug.Location = new System.Drawing.Point(57, -1);
            this.btnTerug.Margin = new System.Windows.Forms.Padding(4);
            this.btnTerug.Name = "btnTerug";
            this.btnTerug.Size = new System.Drawing.Size(100, 28);
            this.btnTerug.TabIndex = 1;
            this.btnTerug.Text = "Terug";
            this.btnTerug.UseVisualStyleBackColor = false;
            this.btnTerug.Click += new System.EventHandler(this.btnTerug_Click);
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1345, 753);
            this.panel1.TabIndex = 2;
            // 
            // btVerder
            // 
            this.btVerder.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btVerder.Location = new System.Drawing.Point(1188, 0);
            this.btVerder.Margin = new System.Windows.Forms.Padding(4);
            this.btVerder.Name = "btVerder";
            this.btVerder.Size = new System.Drawing.Size(100, 28);
            this.btVerder.TabIndex = 1;
            this.btVerder.Text = "Bevestig";
            this.btVerder.UseVisualStyleBackColor = false;
            this.btVerder.Click += new System.EventHandler(this.btVerder_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            this.contextMenuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStrip1_ItemClicked);
            // 
            // StartScherm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(0, 750);
            this.ClientSize = new System.Drawing.Size(1345, 753);
            this.Controls.Add(this.btVerder);
            this.Controls.Add(this.btnTerug);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "StartScherm";
            this.Text = "PDF Output";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnTerug;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btVerder;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;











        //public USMario usMario1;

        public USMario usMario2 { get; set; }
    }
}