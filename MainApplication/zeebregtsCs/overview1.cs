using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace zeebregtsCs
{


    public partial class overview1 : base_form
    {
        
        System.Data.SqlClient.SqlConnection con;
        //new
        Form_helper List_form_helper;
       
        int enum_nr = -1;
        int type = 0;
      
        SqlCommand cmd = new SqlCommand();
        bool zoekend= false;

        string tabel = String.Empty;
        string lbldubstring = String.Empty;
        string lbldubtopstring = String.Empty;
        string[] cproj = new string[4] { "project_nr", "naam_project", "status", "plaats" };
        string[] cbedr = new string[4] { "bedrijf_nr", "naam", "zoeknaam", "plaats" };
        string[] cpers = new string[4] { "persoon_nr", "voornaam", "achternaam", "voorvoegsel" };
        bool advzoek = false;
        string[] zoek_col = new string[4];
        string nummer = String.Empty;
        string adv_zoekterm = String.Empty;
        multibox m_multibox;
        SqlCommand command1;
        bool retrn = false;
        bool Dubbel_bekijk = true;
        int colnum;
        bool wait4it = true;
        bool initialized = false;
        
        public overview1(int tp, base_form start_scherm, base_form close_naar,int P_ID, string vnm, string own, int enr)
        {
            Cursor.Current = Cursors.WaitCursor;
            InitializeComponent();
            FormManager.VoegToe(this, close_naar);
            type = tp;
            enum_nr = enr;
            Parent_ID = P_ID;
            start_parent = start_scherm;
            close_parent = close_naar;
            veldnaam = vnm;
            ownernaam = own;
            this.Text = Global.WTitle;
            initialiseer();
         }
        public overview1(multibox mb, int tp, base_form sp, base_form cp, string zoeknm)
        {
            Cursor.Current = Cursors.WaitCursor;
            InitializeComponent();
            //opened from multibox advfilter
            FormManager.VoegToe(this, cp);
            start_parent = sp;
            close_parent = cp;
            type = tp;
            m_multibox = mb;
            enum_nr = 5;
            adv_zoekterm = zoeknm;
            initialiseer();

        }
        public void initialiseer()
        {
            init_all();
            switch (type)
            {
                case 1://project
                        tabel = "project";
                        zoek_col = cproj;
                        lbl_mid.Text = "Alle projecten";
                        
                       // this.Text = "Alle projecten";
                        nummer = "project_NR";
                        
                        switch (enum_nr)
                        {
                            case 0://vanuit menu
                                Change_btnImage(1);
                                break;
                            case 4://dubbel inv
                                lbl_top.Text = "Dubbele invoer controle";
                                lbl_mid.Text = "Nieuw project";
                                lbldubstring= "Staat het nieuwe project in de onderstaande lijst?";
                                lbl_dub.Text = lbldubstring;
                                lbldubtopstring  = "Indien het nieuwe project al eerder is ingevoerd, dan staat het in de onderstaande lijst";
                                lbl_dub_top.Text = lbldubtopstring;
                                hide_adv_show_dubinvctrls();
                                Dubbel_inv_list();
                                
                                nieuw_btn.Hide();
                                Change_btnImage(2);
                               // btn_gereed.Show();
                                break;
                            case 5:
                                break;
                           
                        }
                    break;
                case 2://bedrijf
                        tabel = "bedrijf";
                        zoek_col = cbedr;
                        lbl_mid.Text = "Alle bedrijven";
                        nummer = "bedrijf_nr";
                       // this.Text = "Alle bedrijven";
                        lbl_mid.Text = "Alle bedrijven";

                        switch (enum_nr)
                        {
                            case 0://vanuit menu
                                lbl_top.Text = "";
                                Change_btnImage(1);
                                break;
                            case 1://vanuit nieuw(PROJECT/persoon) > opdachtgever/(werktbij)bedrijf nwrcrd>
                                if (veldnaam == "de opdrachtgever ")
                                {
                                    lbl_top.Text = "Kies " + veldnaam + "van het project " + ownernaam;
                                }
                                else
                                {
                                    lbl_top.Text = "Kies " + veldnaam + "van het contact " + ownernaam;
                                }
                                lbl_mid.Text = "Alle bedrijven";
                                Change_btnImage(2);
                                break;
                            case 2://vanuit project project>wijzig >> bdr
                                lbl_top.Text = "Kies "+veldnaam+ "van het project "+ ownernaam;
                                lbl_mid.Text = "Alle bedrijven";
                                Change_btnImage(2);
                                break;
                            case 3://vanuit persoon bedrijf>wijzig >>
                                lbl_top.Text = "Kies "+veldnaam+ "van het contact "+ ownernaam;
                                lbl_mid.Text = "Alle bedrijven";
                                Change_btnImage(2);
                                break;
                            case 4://dubbel invoer
                                lbl_top.Text = "Dubbele invoer controle";
                                lbl_mid.Text = "Overeenkomstige bedrijven";
                                lbldubstring = "Staat het nieuwe bedrijf in de onderstaande lijst?";
                                lbl_dub.Text = lbldubstring;
                                lbldubtopstring  = "Indien het nieuwe bedrijf al eerder is ingevoerd, dan staat het in de onderstaande lijst";
                                lbl_dub_top.Text = lbldubtopstring;
                                hide_adv_show_dubinvctrls();
                                Dubbel_inv_list();
                                nieuw_btn.Hide();
                                Change_btnImage(2);
                             //   btn_gereed.Show();
                                break;
                            case 5:
                                lbl_top.Text = "Kies een "+adv_zoekterm+ " voor een uitgebreide zoekregel. ";
                                lbl_mid.Text = "Alle bedrijven met functie: "+adv_zoekterm;
                                Change_btnImage(2);
                                nieuw_btn.Hide();
                                break;
                            case 6:
                                lbl_top.Text = "Kies " + veldnaam + "van het bedrijf " + ownernaam;
                                Change_btnImage(2);
                                break;
                        }
                    break;
                case 3://persoon
                        tabel = "persoon";
                        zoek_col = cpers;
                        lbl_mid.Text = "Alle contacten";
                        nummer = "persoon_nr";
                        lbl_mid.Text = "Alle contacten";

                        switch (enum_nr)
                        {
                            case 0://vanuit menu
                                Change_btnImage(1);
                                lbl_top.Text = "";
                                break;
                            case 1://vanuit ...nvt
                                break;
                            case 2://vanuit project
                                lbl_top.Text = "Kies " + veldnaam + "van het project " + ownernaam;
                                Change_btnImage(2);
                                break;
                            case 4:
                                 lbl_top.Text = "Dubbele invoer controle";
                                lbl_mid.Text = "Overeenkomstige contacten";
                                lbldubstring = "Staat het nieuwe contact in de onderstaande lijst?";
                                lbldubtopstring = "Indien het nieuwe contact al eerder is ingevoerd, dan staat het in de onderstaande lijst";
                                lbl_dub_top.Text = lbldubtopstring;
                                lbl_dub.Text = lbldubstring;
                                hide_adv_show_dubinvctrls();
                                Dubbel_inv_list();
                                nieuw_btn.Hide();
                                Change_btnImage(2);
                                
                                break;
                            case 5:
                                lbl_top.Text = "Kies een "+adv_zoekterm+" voor een uitgebreide zoekregel. ";
                                lbl_mid.Text = "Alle contacten met functie: "+adv_zoekterm;
                                Change_btnImage(2);
                                nieuw_btn.Hide();
                                break;

                        }
                    break;
            }
            
            if (!Global.NoConnection && enum_nr != 4)
            {
                dbcon();
            }
            initialized = true;
        }
        private void prefilter(DataTable dt)
        {
            int cntr = 0;
            string stringlist = String.Empty;
            if (type == 2)
            {
                stringlist = "bedrijf_nr IN (";
            }
            else if (type == 3)
            {
                stringlist = "persoon_nr IN (";
            }
            if (m_multibox.valid_numbers.Count > 0)
            {
                foreach (int i in m_multibox.valid_numbers)
                {
                    if (cntr == 0)
                    {
                        stringlist += i;
                    }
                    else
                    {
                        stringlist += "," + i;
                    }
                    cntr++;
                }
                stringlist += ")";
                DataView dv = dt.DefaultView;
                dv.RowFilter = stringlist;
                dataGridView1.DataSource = dv;
            }
            ///////////////
           /*foreach (DataGridViewRow dr in dataGridView1.Rows)
            {
                if (m_multibox.valid_numbers.Contains(int.Parse(dr.Cells[0].Value.ToString())))
                {

                }
                else
                {
                    DataTable dt = (DataTable)dataGridView1.DataSource;
                    dt.Rows.RemoveAt(dr.Index);
                 //   dataGridView1.Rows.RemoveAt(dr.Index);
                    dataGridView1.DataSource = dt;

                }
            }*/
        }
        public void textBox_TextChanged()
        {
            dataGridView1.ForeColor = Color.Black;
            dataGridView1.Enabled = true;
            if (enum_nr != 5)
            {
                nieuw_btn.Show();
            }
        }
        public void full_list(DataTable dt)
        {
            if (initialized)
            {
                ChangeDatasource(dt);
            }
        }
        private void dbcon()
        {
            SuspendLayout();
            dataGridView1.Columns.Clear();
            dataGridView1.Refresh();
            con = new System.Data.SqlClient.SqlConnection();
            con.ConnectionString = Global.ConnectionString_fileserver;
            try
            {
                con.Open();
           
            if (type == 1)
            {
                 command1 = new SqlCommand("project overview", con);
                 colnum = 5;
            }
            else if (type == 2)
            {
                 command1 = new SqlCommand("bedrijf_overview", con);
                 colnum = 3;
            }
            else if(type == 3)
            {
                 command1 = new SqlCommand("persoon_overview", con);
                 colnum = 4;
            }
            command1.CommandType = CommandType.StoredProcedure;
           
            SqlDataAdapter adapt = new SqlDataAdapter();
            DataTable dt = new DataTable();
            adapt.SelectCommand = command1;
            adapt.Fill(dt);
            dataGridView1.BindingContext = new BindingContext();
            dataGridView1.DataSource = dt;

            if (m_multibox != null)
            {
                prefilter(dt);
            }

                DataGridViewColumn bcol = new DataGridViewColumn();
                bcol.Name = "Details";
                bcol.HeaderText = "";
                bcol.CellTemplate = new DataGridViewTextBoxCell();
               
                dataGridView1.Columns.Add(bcol);
                
                dataGridView1.AdvancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
                
                dataGridView1.Columns[0].HeaderText = "ID";

                dataGridView1.DefaultCellStyle.Padding = new Padding(0, 0, 30, 0);

                if (type == 1)
                {
                    //dataGridView1.Columns[1].HeaderText = "project naam";
                    //dataGridView1.Columns[0].Width = 40;
                    //dataGridView1.Columns[1].Width = 300;
                    //dataGridView1.Columns[2].Width = 125;
                    //dataGridView1.Columns[3].Width = 124;
                    // dataGridView1.Columns[4].Width = 200;
                    dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    dataGridView1.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }
                else if (type == 2)
                {
                    // dataGridView1.Columns[0].Width = 40;
                    //  dataGridView1.Columns[1].Width = 250;
                    //dataGridView1.Columns[2].Width = 250;

                    dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                }
                else if (type == 3)
                {
                    // dataGridView1.Columns[0].Width = 40;
                    //dataGridView1.Columns[1].Width = 200;
                    // dataGridView1.Columns[2].Width = 200;
                    //dataGridView1.Columns[3].Width = 150;
                    dataGridView1.Columns[2].HeaderText = "werkgever";
                    //dataGridView1.Columns[4].Width = 250;
                    dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    //dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    //dataGridView1.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                }

               
                bcol.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                colnum = dataGridView1.Columns.Count;
           
            
            con.Close();

            
           
                con.Open();
                SqlCommand command = new SqlCommand("SELECT MAX (" + nummer + ") FROM " + tabel + "", con);
                object result = command.ExecuteScalar();
                if (result.ToString() != "")
                {
                    //MessageBox.Show(result.ToString()+ " is max");
                    if (type == 1)
                    {
                        Global.max_id_proj = Convert.ToInt32(result.ToString());
                    }
                    else if (type == 2)
                    {
                        Global.max_id_bedr = Convert.ToInt32(result);
                    }
                    else if (type == 3)
                    {
                        Global.max_id_pers = Convert.ToInt32(result);
                    }
                }
                con.Close();
            }
            catch (SqlException e)
            {
                String tmp = e.ToString();
                MessageBox.Show("login failed");
                this.program_closes = true;
                this.Close();
                FormManager.GetMenu().herlaad();

            }
           
            ResumeLayout();
        }
        private void init_all()
        {
            btn_gereed.Hide();
            advancedFilter1.set_startNcloseParent(this, this);
            advancedFilter1.link_grid(this,dataGridView1,type);
            List_form_helper = new Form_helper();
            Global.van_zoek_bdr = false;
            Global.van_zoek_pers = false;
            Global.van_zoek_proj = false;
           
           /* lbl_top.Text = "";
            lbl_mid.Text = "";
             Point pt = new Point(panel2.Width / 2 - lbl_mid.Width / 2, (panel2.Height / 2 - lbl_top.Height / 2)-10);
            lbl_mid.Location = pt;
            Point pt2 = new Point(panel2.Width / 2 - lbl_top.Width / 2, 5);
            lbl_top.Location = pt2;*/
            if (Global.give_return)
            {
                retrn = true;
                Global.give_return = false;
            }
            else
            {
                retrn = false;
            }
            btnadvzoek.Hide();
          
        }
        private void overview1_Load(object sender, EventArgs e)
        {
            
            
            
            /////
            this.MinimumSize = new Size(450, 310);
          //  this.Size = Global.size;
          //  this.Location = Global.position;
         //   this.WindowState = Global.windowstate;
            Point pt = new Point(panel2.Width / 2 - lbl_top.Width / 2, 5);
            lbl_top.Location = pt;
            Point pt2 = new Point(panel2.Width / 2 - lbl_mid.Width / 2, 28);
            lbl_mid.Location = pt2;
            this.Show();
            dataGridView1.Focus();
            wait4it = false;
            Cursor.Current = Cursors.Default;
			if (enum_nr == 4 || true)
			{
				overview1_Resize(this, new EventArgs());
				this.Refresh();
			}
        }
        private void vul_box()
        {
           System.Data.SqlClient.SqlConnection con_cb = new System.Data.SqlClient.SqlConnection();
           con_cb.ConnectionString = Global.ConnectionString_fileserver;
            System.Data.SqlClient.SqlDataAdapter adapter_veld;
            DataSet set_velden = new DataSet();
            con_cb.Open();
            string sql = "";
            if (type == 1)
            {
                sql = "SELECT * FROM advzoek_velden WHERE lijstnr = 1";
            }
            else if (type == 2)
            {
                sql = "SELECT * FROM advzoek_velden WHERE lijstnr = 2";
            }
            else if (type == 3)
            {
                sql = "SELECT * FROM advzoek_velden WHERE lijstnr = 3";
            }
            adapter_veld = new System.Data.SqlClient.SqlDataAdapter(sql, con_cb);
            adapter_veld.Fill(set_velden,"advzoek_velden");
            con_cb.Close();
           
        }
        
        private void init_beta() 
        {
           
        }
        int scrollpos;
        private void Change_btnImage(int nr)
        {
            switch (nr)
            {
                case 1://terug naar menu
                    terug_btn.Image = null;
                    terug_btn.Image = Properties.Resources.Menu2;
                    break;
                case 2://annuleer zoeken/annuleer selecternanieuw
                    terug_btn.Image = null;
                    terug_btn.Image = Properties.Resources.Annuleer2;
                break;

            }
        }
        public void refresh_zoek()
        {
            int prevrow = 0;
            old_row = 0;
            if (dataGridView1.SelectedRows.Count > 0)
            {
                 prevrow = dataGridView1.SelectedRows[0].Index;
                 old_row = prevrow;
                
            }
            scrollpos = this.dataGridView1.FirstDisplayedScrollingRowIndex;
            try
            {
                if (enum_nr == 4)
                {
                    Dubbel_inv_list();
                }
                else
                {
                    advancedFilter1.zoeken();
                    /* switch (type)
                     {
                         case 1:
                             if (textBox2.Text != "project zoeken...")
                             { snel_zoek2(); }
                             else { dbcon(); }
                             break;
                         case 2:
                             if (textBox2.Text != "bedrijf zoeken...")
                             { snel_zoek2(); }
                             else { dbcon(); }
                             break;
                         case 3:
                             if (textBox2.Text != "contacten zoeken...")
                             { snel_zoek2(); }
                             else { dbcon(); }
                             break;

                     }*/
                    if (dataGridView1.Rows.Count > 0 && dataGridView1.Rows.Count > (prevrow - 1))
                    {
                        dataGridView1.Rows[0].Selected = false;
                        dataGridView1.Rows[prevrow].Selected = true;
                        this.dataGridView1.FirstDisplayedScrollingRowIndex = scrollpos;
                    }
                    else
                    {
                        dbcon();
                    }
                }
            }
            catch (Exception e)
            {
                String log_line = "list refresh failed " + e;
                System.IO.FileInfo info = new System.IO.FileInfo(Global.log_file_path);
                System.IO.StreamWriter file = new System.IO.StreamWriter(Global.log_file_path, true);
                file.WriteLine(log_line);
                file.Close();
                close_parent.herlaad();
                this.Close();
            }
            
            
        }
        private void terug_btn_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            if (zoekend)
            {
                advancedFilter1.back2default();

               if(enum_nr !=5)
               { 
                   nieuw_btn.Show();
               }
                if (type == 1)
                    {                
                        lbl_mid.Text = "Alle projecten";
                    }
                    else if (type == 2)
                        {                
                            lbl_mid.Text = "Alle bedrijven";
                        }
                        else if (type == 3)
                            {                
                                lbl_mid.Text = "Alle contacten";
                            }
                if (enum_nr > 0)
                {

                    Change_btnImage(2);
                }
                else
                {
                    Change_btnImage(1);
                }
                dataGridView1.ForeColor = Color.Black;
                dataGridView1.Enabled = true;
                Global.van_zoek_proj = false;
                Global.van_zoek_pers = false;
                Global.van_zoek_bdr = false;
                zoekend = false;
                dbcon();
                dataGridView1.Focus();
                
            }
            else if (NewIsUniek ||!Dubbel_bekijk)
            {
                btn_gereed.Hide();
                dubbel_inv_nee_btn.Show();
                dubbel_inv_ja_btn.Show();
                lbl_dub.Text = lbldubstring;
                lbl_dub_top.Text = lbldubtopstring;
                Point pt3 = new Point(dubbel_q_panel.Width / 2 - lbl_dub.Width / 2, 60);
                lbl_dub.Location = pt3;
                Point pt5 = new Point(dubbel_q_panel.Width / 2 - lbl_dub_top.Width / 2, 5);
                lbl_dub_top.Location = pt5;
                NewIsUniek = false;
                Dubbel_bekijk = true;
                Global.Dubbel_is_bevestigd = false;
                loc_dubbel_is_bevestigd = false;
                
            }
            else if(enum_nr == 4)
            {
                if (this.WindowState == FormWindowState.Normal)
                {
                    Global.size = this.Size;
                    Global.position = this.Location;
                }
                Global.windowstate = this.WindowState;
                close_parent.herlaad();
                
                this.sluit();
                Close();
            }
            else
            {
                if (this.WindowState == FormWindowState.Normal)
                {
                    Global.size = this.Size;
                    Global.position = this.Location;
                }
                Global.windowstate = this.WindowState;
                start_parent.herlaad();
                this.sluit();
                Close();
            }
            Global.give_return = false;
            Cursor.Current = Cursors.Default;
        }

        private void nieuw_btn_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            if (this.WindowState == FormWindowState.Normal)
            {
                Global.size = this.Size;
                Global.position = this.Location;
            }
            Global.windowstate = this.WindowState;
            //return??
            if (zoekend)
            {
                Global.Dubb_van_zoek = true;
            }
                switch (type)
                {
                    case 1:
                        List_form_helper.Start_route(1, 1, this, close_parent, 0, "", "");
                        Cursor.Current = Cursors.Default;
                        this.Hide();
                        break;
                    case 2:

                        List_form_helper.Start_route(2, 1, this, close_parent, Parent_ID, veldnaam, ownernaam);
                        Cursor.Current = Cursors.Default;
                        this.Hide();

                        break;
                    case 3:

                        List_form_helper.Start_route(3, 1, this, close_parent, Parent_ID, veldnaam, ownernaam);
                        Cursor.Current = Cursors.Default;
                        this.Hide();

                        break;
                }
          
            
        }
        private void btnadvzoek_Click(object sender, EventArgs e)
        {
            if (advzoek == false)
            {
                advzoek = true;
                
               // btnadvzoek.Text = "snel zoeken";
            }
            else
            {
                //btnadvzoek.Text = "uitgebreid zoeken";
                advzoek = false;
                
            }
        }
        private void Dubbel_inv_list()
        {
            
            btn_gereed.Hide();
            dataGridView1.Columns.Clear();
            dataGridView1.Refresh();
            dubblinv_linq.Get_doublicates_dgv(type, dataGridView1);

            ////////////////////
            /*con = new System.Data.SqlClient.SqlConnection();
            con.ConnectionString = Global.ConnectionString_fileserver;
            
            con.Open();
            if (type == 1)
            {
                command1 = new SqlCommand("Dubbel_inv_list_proj", con);
                SqlParameter naam_project = command1.Parameters.Add("@naam_project", SqlDbType.NVarChar);
                naam_project.Direction = ParameterDirection.Input;
                naam_project.Value = Global.dub_proj_naamproj;
                SqlParameter plaats = command1.Parameters.Add("@plaats", SqlDbType.NVarChar);
                plaats.Direction = ParameterDirection.Input;
                plaats.Value = Global.dub_proj_plaats;
                SqlParameter opdrachtgever_nr = command1.Parameters.Add("@opdrachtgever_nr", SqlDbType.Int);
                opdrachtgever_nr.Direction = ParameterDirection.Input;
                opdrachtgever_nr.Value = Global.dub_proj_opdrachtgever;
                colnum = 5;
            }
            else if (type == 2)
            {
                command1 = new SqlCommand("Dubbel_inv_list_bdr", con);
                SqlParameter naam = command1.Parameters.Add("@naam", SqlDbType.VarChar);
                naam.Direction = ParameterDirection.Input;
                naam.Value = Global.dub_bdr_naam;
                SqlParameter zoeknaam = command1.Parameters.Add("@zoeknaam", SqlDbType.VarChar);
                zoeknaam.Direction = ParameterDirection.Input;
                zoeknaam.Value = Global.dub_bdr_zoeknaam;
                SqlParameter postcode = command1.Parameters.Add("@postcode", SqlDbType.VarChar);
                postcode.Direction = ParameterDirection.Input;
                postcode.Value = Global.dub_bdr_postcode;
                
                colnum = 3;
            }
            else if (type == 3)
            {
                command1 = new SqlCommand("Dubbel_inv_list_pers", con);
                SqlParameter voornaam = command1.Parameters.Add("@voornaam", SqlDbType.VarChar);
                voornaam.Direction = ParameterDirection.Input;
                voornaam.Value =  Global.dub_pers_voornaam;
                SqlParameter voorletters = command1.Parameters.Add("@voorletters", SqlDbType.VarChar);
                voorletters.Direction = ParameterDirection.Input;
                voorletters.Value =  Global.dub_pers_voorletters;
                SqlParameter achternaam = command1.Parameters.Add("@achternaam", SqlDbType.VarChar);
                achternaam.Direction = ParameterDirection.Input;
                achternaam.Value =  Global.dub_pers_achternaam;
                SqlParameter bedrijf_nr = command1.Parameters.Add("@bedrijf_nr", SqlDbType.Int);
                bedrijf_nr.Direction = ParameterDirection.Input;
                bedrijf_nr.Value =  Global.dub_pers_bedrijf_nr;
                SqlParameter man = command1.Parameters.Add("@man", SqlDbType.Bit);
                man.Direction = ParameterDirection.Input;
                man.Value = Global.dub_pers_man;
                colnum = 4;
            }

            command1.CommandType = CommandType.StoredProcedure;

            SqlDataAdapter adapt = new SqlDataAdapter();
            DataTable dt = new DataTable();
            adapt.SelectCommand = command1;
            adapt.Fill(dt);
            dataGridView1.BindingContext = new BindingContext();
            dataGridView1.DataSource = dt;
            */

            DataGridViewColumn bcol = new DataGridViewColumn();
            bcol.Name = "Details";
            bcol.HeaderText = "";
            bcol.CellTemplate = new DataGridViewTextBoxCell();

            dataGridView1.Columns.Add(bcol);

            dataGridView1.AdvancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;

            dataGridView1.Columns[0].HeaderText = "ID";

            dataGridView1.DefaultCellStyle.Padding = new Padding(0, 0, 30, 0);

            if (type == 1)
            {
                //dataGridView1.Columns[1].HeaderText = "project naam";
                //dataGridView1.Columns[0].Width = 40;
                //dataGridView1.Columns[1].Width = 300;
                //dataGridView1.Columns[2].Width = 125;
                //dataGridView1.Columns[3].Width = 124;
                // dataGridView1.Columns[4].Width = 200;
                dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGridView1.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            else if (type == 2)
            {
                // dataGridView1.Columns[0].Width = 40;
                //  dataGridView1.Columns[1].Width = 250;
                //dataGridView1.Columns[2].Width = 250;

                dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            }
            else if (type == 3)
            {
                // dataGridView1.Columns[0].Width = 40;
                //dataGridView1.Columns[1].Width = 200;
                // dataGridView1.Columns[2].Width = 200;
                //dataGridView1.Columns[3].Width = 150;
                dataGridView1.Columns[2].HeaderText = "werkgever";
                //dataGridView1.Columns[4].Width = 250;
                dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                //dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                //dataGridView1.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }


            bcol.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colnum = dataGridView1.Columns.Count;


            //con.Close();

        }
       private void overview1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            Global.Dubbel_is_bevestigd = false;
            Global.van_zoek_bdr = false;
            Global.van_zoek_pers = false;
            Global.van_zoek_proj = false;
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
            else
            {
            }
            Cursor.Current = Cursors.Default;
        }
        
        public void zoekbox_MouseClick()
        {
            if (!wait4it)
            {

                zoekend = true;


                //dataGridView1.Enabled = false;
                //dataGridView1.CurrentRow.Selected = false;
                //dataGridView1.ForeColor = Color.Gray;
                terug_btn.Image = null;
                terug_btn.Image = Properties.Resources.Annuleer2;

                switch (type)
                {
                    case 1: Global.van_zoek_proj = true;
                        lbl_mid.Text = "Zoek project";
                        break;
                    case 2: Global.van_zoek_bdr = true;
                        lbl_mid.Text = "Zoek bedrijf";
                        break;
                    case 3: Global.van_zoek_pers = true;
                        lbl_mid.Text = "Zoek contact";
                        break;
                }
            }
        }

        private void panel3_MouseClick(object sender, MouseEventArgs e)
        {
            zoekend = true;
            dataGridView1.Enabled = false;
            dataGridView1.CurrentRow.Selected = false;
            dataGridView1.ForeColor = Color.Gray;
            terug_btn.Image = null;
            terug_btn.Image = Properties.Resources.Annuleer2;
            nieuw_btn.Hide();
            switch (type)
            {
                case 1: Global.van_zoek_proj = true;
                    lbl_mid.Text = "Zoek project";
                    break;
                case 2: Global.van_zoek_bdr = true;
                    lbl_mid.Text = "Zoek bedrijf";
                    break;
                case 3: Global.van_zoek_pers = true;
                    lbl_mid.Text = "Zoek contact";
                    break;
            }
        }
        
        private void overview1_Resize(object sender, EventArgs e)
        {
            Point pt = new Point(panel2.Width / 2 - lbl_top.Width / 2,5);
            lbl_top.Location = pt;
            Point pt2 = new Point(panel2.Width / 2 - lbl_mid.Width / 2, 28);
            lbl_mid.Location = pt2;
            Point pt3 = new Point(dubbel_q_panel.Width / 2 - lbl_dub.Width / 2, 60);
            lbl_dub.Location = pt3;
            Point pt4 = new Point(dubbel_q_panel.Width / 2 - dubbel_inv_ja_btn.Width, 80);
            dubbel_inv_ja_btn.Location = pt4;
            pt4.X = (dubbel_q_panel.Width / 2);
            dubbel_inv_nee_btn.Location = pt4;
            Point pt5 = new Point(dubbel_q_panel.Width / 2 - lbl_dub_top.Width / 2, 5);
            lbl_dub_top.Location = pt5;
            keep_selected_row_visible();
            Point pt6 = new Point(panel1.Width / 2 - dubbel_q_panel.Width / 2, dubbel_q_panel.Location.Y);
            dubbel_q_panel.Location = pt6;
           
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                dataGridView1.Focus();
                return;
            }
             
        }

        private void textBox2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Down)
            {
                dataGridView1.Focus();
                return;
            }
        }

        private void dataGridView1_Enter(object sender, EventArgs e)
        {
          dataGridView1.DefaultCellStyle.SelectionBackColor = Color.DarkBlue;
        }

        private void dataGridView1_Leave(object sender, EventArgs e)
        {
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.LightBlue;
        }
        int old_row = 0;
        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            int index = 0;
            old_row = dataGridView1.SelectedRows[0].Index;
            if (dataGridView1.RowCount > 0)
            {
                int.TryParse(dataGridView1.CurrentRow.Cells[0].Value.ToString(), out index);
                if (Global.Dubbel_is_bevestigd)
                {
                    handelingen_logger.log_handeling(index, type, 5);
                }

                if (this.WindowState == FormWindowState.Normal)
                {
                    Global.size = this.Size;
                    Global.position = this.Location;
                }
                Global.windowstate = this.WindowState;
                Global.overzicht_select = index.ToString();
                if (loc_dubbel_is_bevestigd == false)
                {
                    Global.Dubbel_is_bevestigd = false;
                }
                if (enum_nr == 4 && Dubbel_bekijk)
                {
                    switch (type)
                    {
                        case 1:
                            List_form_helper.Start_route(1, 6,this,close_parent, 0,veldnaam, ownernaam);
                            break;
                        case 2:
                            List_form_helper.Start_route(2, 6, this, close_parent, 0, veldnaam, ownernaam);
                            break;
                        case 3:
                            List_form_helper.Start_route(3, 6, this, close_parent, 0, veldnaam, ownernaam);
                            break;
                    }
                    Cursor.Current = Cursors.Default;
                    this.Hide();
                }
                else if (enum_nr == 5)
                {
                    m_multibox.fill_id(index);
                    close_parent.herlaad();
                    Cursor.Current = Cursors.Default;
                    this.sluit();
                    Close();
                }
                else if (retrn == false)
                {
                    
                    switch (type)
                    {

                        case 1:
                            handelingen_logger.log_handeling(index, type, 1);
                            List_form_helper.Start_route(1, 0, this, FormManager.GetMenu(), 0, "", "");
                            if (!this.IsDisposed)
                            {
                                Cursor.Current = Cursors.Default;
                                this.Hide();
                            }
                            break;
                        case 2:
                            handelingen_logger.log_handeling(index, type, 1);
                            List_form_helper.Start_route(2, 0, this, FormManager.GetMenu(), 0, "", "");
                            if (!this.IsDisposed)
                            {
                                Cursor.Current = Cursors.Default;
                                this.Hide();
                            }
                            break;
                        case 3:
                            handelingen_logger.log_handeling(index, type, 1);
                            List_form_helper.Start_route(3, 0, this, FormManager.GetMenu(), 0, "", "");
                            if (!this.IsDisposed)
                            {
                                Cursor.Current = Cursors.Default;
                                this.Hide();
                            }
                            break;
                    }
                }
                else
                {
                    Global.return_id = index.ToString();
                    int log_nr = 0;
                    if(type == 2)
                    {
                        log_nr = 8;
                    }
                    else if(type == 3)
                    {
                        log_nr = 7;
                    }
                    if (close_parent is project_form)
                    {
                        handelingen_logger.log_handeling(index, 1, log_nr);
                        (close_parent as project_form).fill_id(true);
                    }
                    else if (close_parent is persoon_form)
                    {
                        handelingen_logger.log_handeling(index, 3, log_nr);
                        (close_parent as persoon_form).fill_id(true);
                    }
                    else if (close_parent is newrecord)
                    {
                        (close_parent as newrecord).fillid();
                    }
                    else if (close_parent is bedrijf_form)
                    {
                        (close_parent as bedrijf_form).fill_id(true);
                    }
                    Global.give_return = false;
                    Cursor.Current = Cursors.Default;
                    this.sluit();
                    Close();
                }
            }
            
        }
        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return && dataGridView1.RowCount > 0)
            {
                int i = int.Parse(dataGridView1.CurrentRow.Index.ToString());
                DataGridViewCellEventArgs dc = new DataGridViewCellEventArgs(1, i - 1);
                dataGridView1_CellContentDoubleClick(this, dc);
                e.Handled = true;
            }
        }
        private void textBox2_Leave(object sender, EventArgs e)
        {
            //selecta = false;
        }
        bool NewIsUniek = false;
        bool loc_dubbel_is_bevestigd = false;
        private void dubbel_inv_nee_btn_Click(object sender, EventArgs e)
        {
            Global.Dubbel_is_bevestigd = false;
            loc_dubbel_is_bevestigd = false;
            (start_parent as newrecord).extrn_opslaan();
            int l_id = 0;
            int.TryParse(Global.overzicht_select.ToString(), out l_id);
            handelingen_logger.log_handeling(l_id, type, 6);
            this.Hide();
        }
        private void dubbel_inv_ja_btn_Click(object sender, EventArgs e)
        {
            dubbel_inv_ja_btn.Hide();
            dubbel_inv_nee_btn.Hide();
            string typenaam = String.Empty;
            switch (type)
            { case 1: typenaam = "project"; break; case 2: typenaam = "bedrijf"; break; case 3: typenaam = "contact"; break; }
            lbl_dub_top.Text = "Je hebt aangegeven dat het nieuwe "+typenaam+" al eerder is ingevoerd";
            lbl_dub.Text = "Bevestig het "+typenaam+" in de onderstaande lijst.";
            Dubbel_bekijk = false;
            Global.Dubbel_is_bevestigd = true;
            loc_dubbel_is_bevestigd = true;
            NewIsUniek = false;
            this.overview1_Resize(this, e);
        }
        private void log_dubb_gebruik(int id)
        {
            string naam_gebruiker = Global.username;
            //log naam, record_id, type,tijd
            System.Data.SqlClient.SqlConnection con;
            con = new System.Data.SqlClient.SqlConnection();
            con.ConnectionString = Global.ConnectionString_fileserver;
            con.Open();
            string stopro = "log_dubbel_inv";

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            SqlParameter gebruiker = command.Parameters.Add("@gebruiker", SqlDbType.NVarChar);
            SqlParameter record_ID = command.Parameters.Add("@record_id", SqlDbType.Int);
            SqlParameter record_type = command.Parameters.Add("@type", SqlDbType.Int);
            SqlParameter timestamp = command.Parameters.Add("@timestamp", SqlDbType.DateTime);
            SqlParameter user_level = command.Parameters.Add("@user_level", SqlDbType.Int);
            gebruiker.Value = naam_gebruiker;
            user_level.Value = Global.UserLevel;
            user_level.Direction = ParameterDirection.Input;
            switch (type)
            {
                case 1:
                    record_type.Value = 1;
                    break;
                case 2:
                    record_type.Value = 2;
                    break;
                case 3:
                    record_type.Value = 3;
                    break;
            }
            timestamp.Value = DateTime.Now;
            gebruiker.Direction = ParameterDirection.Input;
            record_type.Direction = ParameterDirection.Input;
            timestamp.Direction = ParameterDirection.Input;
            record_ID.Value = id; record_ID.Direction = ParameterDirection.Input;

            command.CommandText = stopro;
            command.CommandType = CommandType.StoredProcedure;
            command.Connection = con;
            command.ExecuteNonQuery();
            con.Close();


        }
        private void button3_Click(object sender, EventArgs e)
        {

        }
        public void ChangeDatasource(DataTable dt)
        {
            dataGridView1.SuspendLayout();
            try
            { //dataGridView1.DataSource = typeof(DataTable);
                //dataGridView1.DataSource = dt;
                if (dataGridView1 != null)
                {
                    if (dt == null)
                    {
                        dataGridView1.DataSource = typeof(DataTable);
                        dataGridView1.Refresh();
                        dataGridView1.Columns.Clear();
                    }
                    else
                    {
                        dataGridView1.AllowUserToAddRows = false;
                        if (dataGridView1.Columns != null)
                        {
                            dataGridView1.DataSource = typeof(DataTable);
                            dataGridView1.Refresh();
                            dataGridView1.Columns.Clear();
                        }
                        dataGridView1.Refresh();
                        dataGridView1.BindingContext = new BindingContext();
                        dataGridView1.DataSource = dt;
                        DataGridViewColumn bcol = new DataGridViewColumn();
                        bcol.Name = "Details";
                        bcol.HeaderText = "";
                        bcol.CellTemplate = new DataGridViewTextBoxCell();
                        if (m_multibox != null)
                        {
                            prefilter(dt);
                        }
                        dataGridView1.Columns.Add(bcol);

                        dataGridView1.AdvancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;

                        dataGridView1.Columns[0].HeaderText = "ID";

                        dataGridView1.DefaultCellStyle.Padding = new Padding(0, 0, 30, 0);
                        if (type == 1)
                        {
                            //dataGridView1.Columns[1].HeaderText = "project naam";
                            //dataGridView1.Columns[0].Width = 40;
                            //dataGridView1.Columns[1].Width = 300;
                            //dataGridView1.Columns[2].Width = 125;
                            //dataGridView1.Columns[3].Width = 124;
                            // dataGridView1.Columns[4].Width = 200;
                            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                            dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                            dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                            dataGridView1.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        }
                        else if (type == 2)
                        {
                            // dataGridView1.Columns[0].Width = 40;
                            //  dataGridView1.Columns[1].Width = 250;
                            //dataGridView1.Columns[2].Width = 250;

                            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                            dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                        }
                        else if (type == 3)
                        {
                            // dataGridView1.Columns[0].Width = 40;
                            //dataGridView1.Columns[1].Width = 200;
                            // dataGridView1.Columns[2].Width = 200;
                            //dataGridView1.Columns[3].Width = 150;
                            dataGridView1.Columns[2].HeaderText = "werkgever";
                            //dataGridView1.Columns[4].Width = 250;
                            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                            dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                            //dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                            //dataGridView1.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                        }


                        bcol.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        colnum = dataGridView1.Columns.Count;
                    }
                }
                if (dataGridView1.Rows.Count > 0 && old_row != 0)
                {
                    if (dataGridView1.Rows.Count > old_row)
                    {
                        dataGridView1.Rows[0].Selected = false;
                        dataGridView1.Rows[old_row].Selected = true;
                        this.dataGridView1.FirstDisplayedScrollingRowIndex = scrollpos;
                        old_row = 0;
                    }

                }
            }
            catch (Exception e)
            {
                log_exception(e);
            }
            dataGridView1.ResumeLayout();
        }
        bool tmp = false;
        private void hide_adv_show_dubinvctrls()
        {
            if (!tmp)
            {
                tmp = true;
                advancedFilter1.Visibility = System.Windows.Visibility.Hidden;
                dubbel_q_panel.Visible = true;
                dubbel_q_panel.Show();
                dubbel_q_panel.BringToFront();
                lbl_dub.Show();
                dubbel_inv_nee_btn.Show();
                dubbel_inv_ja_btn.Show();
                dubbel_inv_nee_btn.Visible = true;
                dubbel_inv_ja_btn.Visible = true;
                lbl_dub.Visible = true;
                lbl_dub.BringToFront();
                lbl_dub_top.Visible = true;
                lbl_dub_top.BringToFront();
                dubbel_inv_nee_btn.BringToFront();
                dubbel_inv_ja_btn.BringToFront();
            }
            else
            {
                tmp = false;
                advancedFilter1.Visibility = System.Windows.Visibility.Visible;
                dubbel_q_panel.Visible = false;
                dubbel_q_panel.Hide();
                dubbel_q_panel.SendToBack();
                lbl_dub.Hide();
                dubbel_inv_nee_btn.Hide();
                dubbel_inv_ja_btn.Hide();
                lbl_dub.Visible = false;
                dubbel_inv_nee_btn.Visible = false;
                dubbel_inv_ja_btn.Visible = false;
                lbl_dub.SendToBack();
                lbl_dub_top.Visible = false;
                lbl_dub_top.SendToBack();
                dubbel_inv_nee_btn.SendToBack();
                dubbel_inv_ja_btn.SendToBack();
            }
        }
        private void keep_selected_row_visible()
        {
            if (dataGridView1 != null)
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    int index = dataGridView1.SelectedRows[0].Index;
                    if (dataGridView1.FirstDisplayedScrollingRowIndex > index)
                    {
                        dataGridView1.Rows[index + 1].Selected = true;
                        dataGridView1.Rows[index].Selected = false;
                        keep_selected_row_visible();
                    }
                    if (dataGridView1.FirstDisplayedScrollingRowIndex + dataGridView1.DisplayedRowCount(false) - 1 < index)
                    {
                        dataGridView1.Rows[index - 1].Selected = true;
                        dataGridView1.Rows[index].Selected = false;
                        keep_selected_row_visible();
                    }
                }
            }
        }
        private void dataGridView1_Scroll(object sender, ScrollEventArgs e)
        {
            keep_selected_row_visible();
        }
    }
}
