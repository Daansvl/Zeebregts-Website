using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace zeebregtsCs
{
    public partial class update : base_form
    {
        private base_form _parent;
        private List<String> file_list = new List<String>();
       
        public update(base_form parent)
        {
           // Global.FW_add(this, parent);
            FormManager.VoegToe(this, parent);
            _parent = parent;
            this.Text = Global.WTitle;
            InitializeComponent();
        }

        private void update_Load(object sender, EventArgs e)
        {
            this.MinimumSize = new Size(450, 310);
            label1.Text = "Update";
            Point pt = new Point(panel1.Width / 2 - label1.Width / 2, 28);
            label1.Location = pt;
            button2.Hide();
          //  this.Size = Global.size;
           // this.Location = Global.position;
            //this.WindowState = Global.windowstate;
            update_Resize(this, e);
        }

        private void update_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (skip_close)
            {
                skip_close = false;
                FormManager.Sluit_form(this);
                e.Cancel = true;
            }
             if (e.CloseReason == CloseReason.UserClosing && !program_closes)
            {
                //start close-route
                FormManager.Sluit_forms(this);
                e.Cancel = true;
            }
            
        }

       

        private void button2_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            System.Data.SqlClient.SqlConnection con;
            con = new System.Data.SqlClient.SqlConnection();
            con.ConnectionString = Global.ConnectionString_fileserver;
            con.Open();
            string stopro = "";
            
            foreach (string file in file_list)
            {
                string temp = file;
                temp = file.Remove(1,1);
                stopro = "update_"+temp ;
                SqlCommand command = new SqlCommand(stopro, con);
                command.CommandType = CommandType.StoredProcedure;

                try
                {
                    int numrow = command.ExecuteNonQuery();
                }
                catch (Exception exc)
                {
                    System.IO.StreamWriter file2 = new System.IO.StreamWriter(Global.log_file_path, true);


                    file2.WriteLine(exc.ToString());
                    file2.Close();
                }
                 
             }
            con.Close();
            Cursor.Current = Cursors.Default;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            if (this.WindowState == FormWindowState.Normal)
            {
                Global.size = this.Size;
                Global.position = this.Location;
            }
            Global.windowstate = this.WindowState;
            Cursor.Current = Cursors.Default;
            this.sluit();
            Close();
            _parent.herlaad();
        }

       

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            
            CheckBox temp = (CheckBox)sender;

            if (temp.Checked)
            {
                file_list.Add(temp.Text);
            }
            else
            {
                file_list.Remove(temp.Text);
            }
            if (file_list.Count == 0)
            {
                button2.Hide();
            }
            else
            {
                button2.Show();
            }

        }

        private void update_Resize(object sender, EventArgs e)
        {
            Point pt = new Point(panel1.Width / 2 - label1.Width / 2,28);
            label1.Location = pt;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                checkBox1_1.Checked = true;
                checkBox1_2.Checked = true;
                checkBox2_1.Checked = true;
                checkBox2_2.Checked = true;
                checkBox2_3.Checked = true;
                checkBox2_4.Checked = true;
                checkBox3_1.Checked = true;
                checkBox3_2.Checked = true;
                checkBox3_3.Checked = true;
                checkBox4_1.Checked = true;
                checkBox4_2.Checked = true;
                checkBox4_3.Checked = true;
                checkBox5_1.Checked = true;

            }
            else
            {
                checkBox1_1.Checked = false;
                checkBox1_2.Checked = false;
                checkBox2_1.Checked = false;
                checkBox2_2.Checked = false;
                checkBox2_3.Checked = false;
                checkBox2_4.Checked = false;
                checkBox3_1.Checked = false;
                checkBox3_2.Checked = false;
                checkBox3_3.Checked = false;
                checkBox4_1.Checked = false;
                checkBox4_2.Checked = false;
                checkBox4_3.Checked = false;
                checkBox5_1.Checked = false;
            }
        }

        

        
    }
}
