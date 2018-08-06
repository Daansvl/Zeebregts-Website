using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms.Integration;
using System.Collections.ObjectModel;
using WPFcontrolLib;



namespace zeebregtsCs
{
    public partial class Menu : base_form
    {
        public String user_name;
        public Button[] btns = new Button[7];
        public Label[] lbls = new Label[7];
        Form_helper Menu_form_helper;
        bool loading = true;
        bool closing = false;
        public Menu()
        {
            Cursor.Current = Cursors.WaitCursor;
            InitializeComponent();
            //Global.FW_add(this, this);
           FormManager.VoegToe(this, this);
           
        }


       
        private void Check_rechten()
        {
            Regex usernm = new Regex("administrator",RegexOptions.IgnoreCase);
            Regex usernm2 = new Regex("daan", RegexOptions.IgnoreCase);
            Regex userm4 = new Regex("daan van leth", RegexOptions.IgnoreCase);
            Regex usernm3 = new Regex("joost", RegexOptions.IgnoreCase);
            Match m = usernm.Match(user_name);
            Match m2 = usernm2.Match(user_name);
            Match m3 = usernm3.Match(user_name);
            Match m4 = userm4.Match(user_name);
            if (m3.Success)
            {
                Global.UserLevel = 2;
                //settings_btn.Hide();
                planning_btn.Hide();
                labels0.Hide();
                //labels3.Hide();
                planning_btn.Dispose();
            }
            else if (m.Success || m2.Success || m4.Success)
            {
                Global.UserLevel = 1;
                settings_btn.Show();
                planning_btn.Show();
                monitor_btn.Show();
                labels7.Show();
                labels0.Show();
                labels3.Show();
            }
            else
            {
                Global.UserLevel = 3;
                settings_btn.Dispose();
                planning_btn.Dispose();
                monitor_btn.Dispose();
                labels0.Hide();
                labels3.Hide();
                labels7.Hide();
            }

          
			

        }
        private void reset_inuse()
        {
            try { 
            //release uses!!!
            int ack = 0;

            System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection();
            con.ConnectionString = Global.ConnectionString_fileserver;
            string stopro = "reset_ingebruik";
            System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.CommandText = stopro;
            command.Connection = con;
            string a;
            a = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
            string[] naam_array = a.Split('\\');
            String naam = naam_array[naam_array.Length - 1];
            System.Data.SqlClient.SqlParameter user_name_R = command.Parameters.Add("@user_name", System.Data.SqlDbType.NVarChar, 250);
            user_name_R.Value = naam;
            user_name_R.Direction = System.Data.ParameterDirection.Input;
            con.Open();
            ack = command.ExecuteNonQuery();
            con.Close();
            }
            catch(SqlException sqle)
            {
                ///
            }
            /////////
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.MinimumSize = new Size(450, 310);
            load_settings();
            Menu_form_helper = new Form_helper();

            Point pt = new Point(panel1.Width / 2 - label1.Width / 2, 28);
            label1.Location = pt;
            string a;
            a = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
           string[] naam_array =  a.Split('\\');
           
            this.Text = "Welkom " + naam_array[naam_array.Length-1];
            user_name = naam_array[naam_array.Length - 1];
            Global.username = user_name;
            String log_line = "login by user: " + user_name;

            FileInfo info = new FileInfo(Global.log_file_path);
            long size = info.Length;
            if (size > 5000000)
            {
                File.Delete(Global.log_file_path);
            }
            System.IO.StreamWriter file = new System.IO.StreamWriter(Global.log_file_path, true);
            
            
            file.WriteLine(log_line);
            file.Close();

            this.btns[0] = this.planning_btn;
            this.btns[1] = this.bedrijf_btn;
            this.btns[2] = project_btn;
            this.btns[3] = this.settings_btn;
            this.btns[4] = this.persoon_btn;
            this.btns[5] = this.monitor_btn;
            lbls[0] = labels0;
            lbls[1] = labels1;
            lbls[2] = labels2;
            lbls[3] = labels3;
            lbls[4] = labels5;
            lbls[5] = labels7;

            lbls[0].Text = "Planning";
            lbls[1].Text = "Bedrijven";
            lbls[2].Text = "Projecten";
            lbls[3].Text = "Settings";
            lbls[4].Text = "Contacten";
            lbls[5].Text = "Monitor";

            for (int i = 0; i < 6; i++)
            {
                Point pnt = new Point(btns[i].Location.X + (Grid.Width - lbls[i].Width)/2 -15 , btns[i].Location.Y + Grid.Height -30);
                lbls[i].Location = pnt;
                
                lbls[i].Show();
            }
            Check_rechten();
            loading = false;
            Cursor.Current = Cursors.Default;
            reset_inuse(); 
           
        }
        private void project_btn_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            Global.WTitle = "Project";
            reset_inuse();
            if (this.WindowState == FormWindowState.Normal)
            {
                Global.size = this.Size;
                Global.position = this.Location;
            }
                Global.windowstate = this.WindowState;
                Menu_form_helper.Start_route(1, 3, this, this, 0, "", "");
                Cursor.Current = Cursors.Default;
                this.Hide();
        }

        private void bedrijf_btn_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            Global.WTitle = "Bedrijf";
            reset_inuse();
            if (this.WindowState == FormWindowState.Normal)
            {
                Global.size = this.Size;
                Global.position = this.Location;
            }
                Global.windowstate = this.WindowState;
                Menu_form_helper.Start_route(2, 3, this, this, 0, "", "");
                Cursor.Current = Cursors.Default;
                this.Hide();
        }

        private void persoon_btn_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            Global.WTitle = "Contact";
            reset_inuse();
            if (this.WindowState == FormWindowState.Normal)
            {
                Global.size = this.Size;
                Global.position = this.Location;
            }
                Global.windowstate = this.WindowState;
                Menu_form_helper.Start_route(3, 3, this, this, 0, "", "");
                Cursor.Current = Cursors.Default;
                this.Hide();
        }
        public void save_settings()
        {
            if (!loading && !closing)
            {
                Properties.Settings.Default.MyState = this.WindowState;
                if (this.WindowState == FormWindowState.Normal)
                {
                    Properties.Settings.Default.MySize = this.Size;
                    Properties.Settings.Default.MyLoc = this.Location;
                }
                else
                {
                    Properties.Settings.Default.MySize = this.RestoreBounds.Size;
                    Properties.Settings.Default.MyLoc = this.RestoreBounds.Location;
                }
                Properties.Settings.Default.Save();
            }
        }
        
        public void load_settings()
        {
            this.Size = Properties.Settings.Default.MySize;
            this.Location = Properties.Settings.Default.MyLoc;
            this.WindowState = Properties.Settings.Default.MyState;
        }
        
        private Point _pntOffset;
        private bool _controlMoving;
       
        private void control_MouseDown(object sender  , MouseEventArgs e)
        {
            Cursor.Clip = panel2.RectangleToScreen(panel2.ClientRectangle);
            if (e.Button == MouseButtons.Right)
            {
                _controlMoving = true;
                _pntOffset = new Point(e.X, e.Y);
            }
            
        }

        private void control_MouseUp(object sender, MouseEventArgs e)
        {
            Button b = (Button)sender;
            

                if (e.Button == MouseButtons.Right && _controlMoving)
                {
                    b.Location = GridPoint(new Point(b.Location.X + e.X - _pntOffset.X, b.Location.Y + e.Y - _pntOffset.Y));
                }
                _controlMoving = false;

            

                Cursor.Clip = Rectangle.Empty;
        }
        
        private void control_MouseMove(object sender, MouseEventArgs e)
        {
            Button b = (Button)sender;
            if (e.Button == MouseButtons.Right && _controlMoving)
            {

                b.Location = new Point(b.Location.X + e.X - _pntOffset.X, b.Location.Y + e.Y - _pntOffset.Y);
                
            }
           
        
        }

        private void planning_btn_Click(object sender, EventArgs e)
        {
           // Global.WTitle = "Planning";
        }

        private void Menu_Resize(object sender, EventArgs e)
        {
            Point pt = new Point(panel1.Width / 2 - label1.Width / 2, 28);
            label1.Location = pt;
            save_settings();
        }

        private void Menu_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (program_closes)
            { }
            else
            {
                save_settings();
                closing = true;
                FormManager.Sluit_form(this);
                
            }
        }

        private void settings_btn_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            Global.WTitle = "Settings";
            if (this.WindowState == FormWindowState.Normal)
            {
                Global.size = this.Size;
                Global.position = this.Location;
            }
            Global.windowstate = this.WindowState;

            if (Global.UserLevel == 1)
            {
                instellingen_zb settings = new instellingen_zb(this);
                settings.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
                settings.WindowState = this.WindowState;
                if (this.WindowState != FormWindowState.Maximized)
                {
                    settings.Size = this.Size;
                    settings.Location = this.Location;
                }
                settings.Show();
                Cursor.Current = Cursors.Default;
                this.Hide();
            }
            else if (Global.UserLevel == 2)
            {
                Adminscreen adminscrn = new Adminscreen(this);
                adminscrn.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
                adminscrn.WindowState = this.WindowState;
                if (this.WindowState != FormWindowState.Maximized)
                {
                    adminscrn.Size = this.Size;
                    adminscrn.Location = this.Location;
                }
                adminscrn.Show();
                Cursor.Current = Cursors.Default;
                this.Hide();
            }
        }
       
        public static readonly Size Grid = new Size(100, 100);
        public static readonly Size HalfGrid = new Size(Grid.Width / 2, Grid.Height / 2);
        private Point overlapcheck(Point p)
        {
            bool empty = false;
            Point tmp = new Point(0,0);
            bool exit = false;
            for (int i = 0; i < 6; i++)
            {
                
                if (btns[i].Location == p)
                {
                    empty = false;
                    tmp = p;
                    exit = true;
                }
                else if (!exit)
                {
                    empty = true;
                }
            }
            if (empty)
            {
                return p;
            }
            else
            {
                bool hit = true;
                while (hit)
                {
                    int cnt = 0;
                    tmp.X += Grid.Width;
                    if (tmp.X > (panel2.Width - 2*Grid.Width))
                    {
                        tmp.X = 25; tmp.Y = 25;
                        //tmp.X = 0 +25; 
                        //tmp.Y += Grid.Height + 25;
                        if (tmp.Y > (panel2.Height - 2*Grid.Height))
                        { 
                            //tmp.Y -= Grid.Height + 25;
                            tmp.X = 25; tmp.Y = 25;
                        }
                    }
                    for (int i = 0; i < 6; i++)
                    {
                        if (btns[i].Location == tmp)
                        {
                            hit = true;

                        }
                        else
                        {
                            cnt++;
                        }
                    }
                    if (cnt == 6)
                    {
                        hit = false;
                    }
                }
                p = tmp;
                return p;
            }
        }
        System.Drawing.Point GridPoint(System.Drawing.Point pt)
        {
            if (pt.X > panel2.Width - Grid.Width || pt.Y > panel2.Height - Grid.Height)
            {
                pt.X = 25; pt.Y = 25;
            }
            
                int snapX = (((pt.X + HalfGrid.Width) / Grid.Width) * Grid.Width) + 25;
                int snapY = (((pt.Y + HalfGrid.Height) / Grid.Height) * Grid.Height) + 25;
                Point tmp = new Point(snapX, snapY);
                tmp = overlapcheck(tmp);
            
            return tmp;
        }

        private void btn_LocationChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < 6; i++)
            {
                Point pnt = new Point(btns[i].Location.X + (Grid.Width - lbls[i].Width) / 2 - 15, btns[i].Location.Y + Grid.Height - 30);
                lbls[i].Location = pnt;
                //lbls[i].Show();

            }

        }

        private void monitor_btn_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
           var wpfwindow = new GebruikersMonitor.Module1Monitor(Global.ConnectionString_fileserver,this);
           ElementHost.EnableModelessKeyboardInterop(wpfwindow);
           wpfwindow.Show();
           this.Hide();
           Cursor.Current = Cursors.Default;
        }

       

        private void btnSql2Excel_Click(object sender, EventArgs e)
        {
            //Excel2SqlContainer.Form1 sql2excelscreen = new Excel2SqlContainer.Form1();
            //sql2excelscreen.Show();
        }

        private void btnMandagenRegistratie_Click(object sender, EventArgs e)
        {
            //MandagenRegistratie.controls.PageContainer p = new MandagenRegistratie.controls.PageContainer();
            //p.LoadControl();
            //p.Show();
        }

        private void btnSql2_Click(object sender, EventArgs e)
        {
            //Excel2SqlContainer.Form1 sql2excelscreen = new Excel2SqlContainer.Form1();
            //sql2excelscreen.Show();
        }

        private void button1_Click(object sender, EventArgs e)//project folder explorer
        {
            var  con = new System.Data.SqlClient.SqlConnection();
            con.ConnectionString = Global.ConnectionString_fileserver;

            var projColl = new ObservableCollection<ProjectFolderItem>();

            try
            {
                con.Open();

                var command1 = new SqlCommand("project overview", con);
                
                command1.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter adapt = new SqlDataAdapter();
                DataTable dt = new DataTable();
                adapt.SelectCommand = command1;
                adapt.Fill(dt);
                
                foreach(DataRow row in dt.Rows)
                {
                    
                    var obj = new ProjectFolderItem
                    {
                        ProjNum = (int)row["project_NR"],
                        ProjName = (string)row["naam_project"],
                        ProjPlaats = (string)row["plaats"],
                        ProjOmsch = (string)row[3],
                        ProjOpdr = (string)row[4]
                    };
                    projColl.Add(obj);
                }
                con.Close();

            }
            catch (SqlException exSql)
            {
                String tmp = exSql.ToString();
                MessageBox.Show("login failed");
                this.program_closes = true;
                this.Close();
                FormManager.GetMenu().herlaad();

            }

            var wind = new ProjectFolderExplorer(projColl);
            ElementHost.EnableModelessKeyboardInterop(wind);
            wind.Width = 1100;
            wind.Show();

        }

        
     

        

        
    }
    
}
