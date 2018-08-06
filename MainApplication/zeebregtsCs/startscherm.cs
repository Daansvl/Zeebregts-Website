using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace zeebregtsCs
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
            
        }

        
        

        private void Form1_Load(object sender, EventArgs e)
        {
            
            
            

            
        }

       
        
        private void button1_Click(object sender, EventArgs e)//toon
        {

            if (this.comboBox1.SelectedIndex == 0)//bedrijf
            {
                //creating bedrijf_form
                bedrijf_form bedrijf_form = new bedrijf_form();
                bedrijf_form.MdiParent = this;
                bedrijf_form.Show();
            }
            else if (this.comboBox1.SelectedIndex == 1)//persoon
            {
                //creating persoon_form          
                persoon_form persoon_form = new persoon_form();
                persoon_form.MdiParent = this;
                persoon_form.Show();
            }
            else if (this.comboBox1.SelectedIndex == 2)//project
            {
                //creating project_form
                project_form project_form = new project_form();
                project_form.MdiParent = this;
                project_form.Show();
            }
        }

        private void project_btn_Click(object sender, EventArgs e)
        {
            //creating project_form
            project_form project_form2 = new project_form();
            project_form2.ShowDialog(this);
        }

        private void bedrijf_btn_Click(object sender, EventArgs e)
        {

        }

        private void persoon_btn_Click(object sender, EventArgs e)
        {

        }

        

        
    }
}
