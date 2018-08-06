using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace zeebregtsCs
{
    public partial class TextBrowser : zeebregtsCs.base_form
    {
        String werktext = "";
        LinkLabel koppelLabel;
        TextBox koppelbox;
        base_form _ParentForm;
        bool _Editing;
        public TextBrowser(String txt, LinkLabel LL, bool edit, string veldnaam, string ownernaam, int id, base_form _parent)
        {
            InitializeComponent();
            btnopslaan.Hide();
            werktext = txt;
            koppelLabel = LL;
            _Editing = edit;
            textBox1.ReadOnly = true;
            this.Text = Global.WTitle;
            label1.Text = "Info " + veldnaam + "van project " + ownernaam; ;
            label2.Text = "Memo";
            label3.Text = "ID " + id + " - " + ownernaam;
            //handelingen_logger.log_handeling(id, 1, 4);
            _ParentForm = _parent;
        }
        public TextBrowser(String txt, TextBox tb, bool edit, string veldnaam, string ownernaam, int id,base_form _parent)
        {
            InitializeComponent();
            btnopslaan.Hide();
            werktext = txt;
            koppelbox = tb;
            _Editing = edit;
            textBox1.ReadOnly = true;
            this.Text = Global.WTitle;
            label1.Text = "Info " + veldnaam + "van project " + ownernaam; ;
            label2.Text = "Memo";
            label3.Text = "ID " + id + " - " + ownernaam;
            //handelingen_logger.log_handeling(id, 1, 4);
            _ParentForm = _parent;
            
        }
        private void TextBrowser_Load(object sender, EventArgs e)
        {
            textBox1.Text = werktext;
            this.WindowState = Global.windowstate;
            if (this.WindowState != FormWindowState.Maximized)
            {
                this.Size = Global.size;
                this.Location = Global.position;
            }
           
            label4.Hide();
            if (_Editing)
            {
                
                btnwijzig.PerformClick();
                btnwijzig.Hide();
                btnopslaan.Hide();
            }
            else
            {
                label4.Text = wrap_text(textBox1.Text);
                //textBox1.Hide();
                btnwijzig.Hide();
                btnopslaan.Hide();
            }

        }
        private void btnterug_Click(object sender, EventArgs e)
        {
            if (!_Editing)
            {
                Close();
            }
            else
            {
                if (koppelLabel != null)
                {
                    koppelLabel.Text = textBox1.Text.Trim();
                }
                else if (koppelbox != null)
                {
                    koppelbox.Text = textBox1.Text.Trim();
                }
                Close();
               
            }
        }
        private void btnwijzig_Click(object sender, EventArgs e)
        {
            textBox1.ReadOnly = false;
            btnwijzig.Hide();
            btnopslaan.Show();
            wijzigstand = true;
        }
        private void btnopslaan_Click(object sender, EventArgs e)
        {
            wijzigstand = false;
            textBox1.ReadOnly = true;
            btnopslaan.Hide();
            btnwijzig.Show();
            werktext = textBox1.Text;
        }
        private void TextBrowser_Resize(object sender, EventArgs e)
        {
            Point pt = new Point(panel2.Width / 2 - p_memo.Width / 2, p_memo.Location.Y);
            p_memo.Location = pt;
            Point pt1 = new Point(panel1.Width / 2 - label1.Width / 2, 5);
            Point pt2 = new Point(panel1.Width / 2 - label2.Width / 2, 28);
            Point pt3 = new Point(panel1.Width / 2 - label3.Width / 2, panel1.Height - 18);
            label1.Location = pt1;
            label2.Location = pt2;
            label3.Location = pt3;

        }
        private void TextBrowser_FormClosing(object sender, FormClosingEventArgs e)
        {
            Global.windowstate = this.WindowState;
            if (this.WindowState != FormWindowState.Maximized)
            {
                Global.size = this.Size;
                Global.position = this.Location;
            }
           
            _ParentForm.herlaad();
            
        }

        private string wrap_text(string tekst)
        {
            StringBuilder SB = new StringBuilder();
            int previndxof = 0;
            for (int i = 0; i < tekst.Length; i++)
            {
                SB.Append(tekst[i]);
                if ((i % 70) == 0 && i > 1)
                {
                    if (tekst.IndexOf(Environment.NewLine,previndxof) > i || !tekst.Contains(Environment.NewLine))
                    {
                        int space = SB.ToString().LastIndexOf(' ', previndxof);
                        SB.Insert(space + 1, Environment.NewLine);
                    }
                   
                    previndxof = i+Environment.NewLine.Length;
                }
            }

            return tekst = SB.ToString();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = textBox1;
            Bitmap b = new Bitmap(tb.Width - 10, tb.Height - 10);
            Graphics g = tb.CreateGraphics();
            Size s = g.MeasureString(tb.Text, tb.Font, b.Width).ToSize();
            if (s.Height > tb.Height - 8)
            {
                tb.ScrollBars = ScrollBars.Vertical;
                tb.ScrollToCaret();
            }
            else
            {
                tb.ScrollBars = ScrollBars.None; // tb.ReadOnly = false;
            }
            
        }
    }
}
