using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using zeebregtsCs.ExchangeService;


namespace zeebregtsCs
{

    public partial class newrecord : base_form
    {
        System.Data.SqlClient.SqlConnection con;
        System.Data.SqlClient.SqlDataAdapter da;
        DataSet ds1;
        //new vars
        Form_helper newrecord_form_helper;
        dubbele_invoerTableAdapters.bedrijfTableAdapter bdr_adapt = new dubbele_invoerTableAdapters.bedrijfTableAdapter();
        dubbele_invoerTableAdapters.projectTableAdapter proj_adapt = new dubbele_invoerTableAdapters.projectTableAdapter();
        dubbele_invoerTableAdapters.persoonTableAdapter pers_adapt = new dubbele_invoerTableAdapters.persoonTableAdapter();
        zeebregtsdbDataSetTableAdapters.adressenTableAdapter adres_adapt = new zeebregtsdbDataSetTableAdapters.adressenTableAdapter();
        private string[] blackList = { "--", ";--", ";", "/*", "*/", "@@", "@" };
        string lbltopstr = "";
        ToolTip tt_memo = new ToolTip();
        //
        bool status = false;
        int type = 0;
        int id;
        bool return_proj;
        String[] bdr_foute_velden = new String[10];
        String[] pers_foute_velden = new String[6];
        List<string> ignorelist = new List<string> { "woningen", "huur", "appartementen", "project", "kantoor", "koop", "woon","woning", "app." };
        bool nuniet = false;
        bool NewIsUniek = false;
        bool zoekopkvk = false;
       
        

        public newrecord(int tp, base_form start_scherm, base_form close_naar, int P_ID, string vnm, string own)
        {
            bdr_adapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
            proj_adapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
            pers_adapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
            adres_adapt.Connection.ConnectionString = Global.ConnectionString_fileserver;



            Cursor.Current = Cursors.WaitCursor;
           // Global.FW_add(this, close_naar);
            FormManager.VoegToe(this, close_naar);
            InitializeComponent();
            type = tp;
            Parent_ID = P_ID;
            start_parent = start_scherm;
            close_parent = close_naar;
            veldnaam = vnm;
            ownernaam = own;
            this.Text = Global.WTitle;
            initialiseer();
        }

        private void GetLatesID(SqlConnection con)
        {
            string sql = "";
            string tabel = "";
            string nummer = "";

            switch (type)
            {
                case 1:
                    tabel = "project";
                    init_proj();
                    nummer = "project_nr";
                    break;
                case 2:
                    tabel = "bedrijf";
                    init_bedr();
                    nummer = "bedrijf_nr";
                    break;
                case 3:
                    tabel = "persoon";
                    init_pers();
                    nummer = "persoon_nr";
                    break;
            }
            sql = "SELECT * FROM " + tabel + "";
            da = new System.Data.SqlClient.SqlDataAdapter(sql, con);
            da.Fill(ds1, tabel);
            SqlCommand command = new SqlCommand("SELECT MAX (" + nummer + ") FROM " + tabel + "", con);
            object result = command.ExecuteScalar();
            if (result != null)
            {
                id = Convert.ToInt32(result) + 1;
                switch (type)
                {
                    case 1:
                        Global.max_id_proj = Convert.ToInt32(result);
                        break;
                    case 2:
                        Global.max_id_bedr = Convert.ToInt32(result);
                        break;
                    case 3:
                        Global.max_id_pers = Convert.ToInt32(result);
                        break;
                }
            }
            con.Close();
        }
        private void initialiseer()
        {
            lbl_bot.Text = "";
            this.bedrijf2TableAdapter.Connection.ConnectionString = Global.ConnectionString_fileserver;
            this.bedrijf1TableAdapter.Connection.ConnectionString = Global.ConnectionString_fileserver;
            this.statusTableAdapter.Connection.ConnectionString = Global.ConnectionString_fileserver;
            this.bedrijf2TableAdapter.Fill(this.zeebregtsdbDataSet.bedrijf2);
            this.bedrijf1TableAdapter.Fill(this.zeebregtsdbDataSet.bedrijf1);
            this.statusTableAdapter.Fill(this.testset.status);
            con = new System.Data.SqlClient.SqlConnection();
            con.ConnectionString = Global.ConnectionString_fileserver;
            con.Open();
            ds1 = new DataSet();

            GetLatesID(con);

            Point pt1 = new Point(panel1.Width / 2 - lbl_top.Width / 2, 5);
            lbl_top.Location = pt1;
            Point pt2 = new Point(panel1.Width / 2 - lbl_mid.Width / 2, 28);
            lbl_mid.Location = pt2;
            Point pt3 = new Point(panel1.Width / 2 - lbl_bot.Width / 2, panel1.Height - 18);
            lbl_bot.Location = pt3;
            
        }
        private void report_Load(object sender, EventArgs e)
        {
            this.MinimumSize = new Size(450, 310);
            newrecord_form_helper = new Form_helper();
        //    this.Size = Global.size;
      //      this.Location = Global.position;
      //      this.WindowState = Global.windowstate;
            lbltopstr = lbl_top.Text;
            Cursor.Current = Cursors.Default;
        }

        private void init_proj()
        {
            adres_user_control3.initialiseer_newproj();
            lbl_mid.Text = "Nieuw project ";
            lbl_bot.Text = "";
            lbl_top.Text = "";
            panel_bedrijf.Hide();
            panel_persoon.Hide();
            panel_project.Show();
            cb_proj_opdrachtgever.BindingContext = new System.Windows.Forms.BindingContext();
            cb_proj_opdrachtgever.DataSource = this.zeebregtsdbDataSet.bedrijf2;
            cb_proj_opdrachtgever.DisplayMember = "zoeknaam";
            cb_proj_opdrachtgever.ValueMember = "bedrijf_nr";
            cb_proj_opdrachtgever.SelectedIndex = -1;
            cb_proj_status.BindingContext = new System.Windows.Forms.BindingContext();
            int[] a = {1,3,4,7,8,9,11};
            List<int> nrs = a.ToList<int>();
            var ds_linq = (from S in testset.status
                     where nrs.Contains(S.omschrijving_nr)
                     select S).ToList();
            cb_proj_status.DataSource = ds_linq;
            cb_proj_status.DisplayMember = "omschrijving";
            cb_proj_status.ValueMember = "omschrijving_nr";
            cb_proj_status.SelectedIndex = -1;
            //proj_lbl_specinfo.Text = @"Gebruik de pijl bij project specificatie om informatie"+Environment.NewLine+"over de opbouw van het project in te voeren.";
                 
        }
        private void init_bedr()
        {
            lbl_mid.Text = "Nieuw bedrijf ";
            //splitContainer8.Hide();
            adres_user_control1.initialiseer(true, false);
            adres_user_control2.initialiseer(true, false);
            tb_bdr_kvk.Hide();
            tb_bdr_naam_bedrijf.ReadOnly = true;
            tb_bdr_zoeknaam.ReadOnly = true;
            telefoonnummer_user_control1.Wijzigstand(true);
            telefoonnummer_user_control1.IsEnabled = false;
            telefoonnummer_user_control1.set_naam(0, false);
            telefoonnummer_user_control1.Minder_opties();
            if (veldnaam != "" && ownernaam != "")
            {
                if (veldnaam == "het bedrijf ")
                {
                    lbl_top.Text = "Kies " + veldnaam + "van het contact " + ownernaam;
                }
                else
                {
                    lbl_top.Text = "Kies " + veldnaam + "van het project " + ownernaam;
                }
            }
            else
            {
                
                lbl_top.Text = "";
            }
            panel_bedrijf.Show();
            panel_project.Hide();
            panel_persoon.Hide();
            p_bezoekadres.Hide();
            p_postadres.Hide();
            p_bdr_telnrs.Hide();
            splitContainer5.Hide();
            splitContainer6.Hide();
            Organize_Panels();
        }
        private void init_pers()
        {
            if (veldnaam != "" && ownernaam != "")
            {
                lbl_top.Text = "Kies " + veldnaam + "van het project " + ownernaam;
                lbl_mid.Text = "Nieuw contact ";
            }
            else
            {
                lbl_top.Text = "";
                lbl_mid.Text = "Nieuw contact ";
            }
            panel_persoon.Show();
            panel_project.Hide();
            panel_bedrijf.Hide();
            cb_pers_bedrijfnr.BindingContext = new BindingContext();
            cb_pers_bedrijfnr.DataSource = this.zeebregtsdbDataSet.bedrijf2;
            cb_pers_bedrijfnr.DisplayMember = "zoeknaam";
            cb_pers_bedrijfnr.ValueMember = "bedrijf_nr";
            cb_pers_bedrijfnr.SelectedIndex = -1;
            tb_pers_tussenvoegsel.Hide();

        }

        private void button1_Click(object sender, EventArgs e)// annuleer
        {
            Cursor.Current = Cursors.WaitCursor;
            Global.windowstate = this.WindowState;
            if (this.WindowState == FormWindowState.Normal)
            {
                Global.size = this.Size;
                Global.position = this.Location;
            }
            
            nuniet = true;
            if (start_parent != null && !start_parent.IsDisposed)
            {
                if (start_parent is overview1)
                {
                    (start_parent as overview1).refresh_zoek();
                }
                start_parent.herlaad();
                
                
            }
            else
            {
                FormManager.GetMenu().herlaad();
            }

            Cursor.Current = Cursors.Default;
            this.sluit();
            this.Close();
            
        }
        private bool invoer_check()
        {
            bool ok = false;
             
            switch (type)
            {
                case 1:
                    bool error = false;
                   
                    tb_proj_projectnaam.BackColor = Color.White;
                    tb_proj_projectnaam.ForeColor = Color.Black;
                    cb_proj_opdrachtgever.BackColor = Color.White;
                    cb_proj_opdrachtgever.ForeColor = Color.Black;
                    cb_proj_status.BackColor = Color.White;
                    cb_proj_status.ForeColor = Color.Black;
                    if (!status)
                    {
                      error = true;
                      cb_proj_status.BackColor = Color.Crimson;
                      cb_proj_status.ForeColor = Color.White;
                    }
                    if (cb_proj_opdrachtgever.SelectedIndex < 0)
                    {
                        error = true;
                        cb_proj_opdrachtgever.BackColor = Color.Crimson;
                        cb_proj_opdrachtgever.ForeColor = Color.White;
                    }
                    if (tb_proj_projectnaam.TextLength < 2)
                    {
                        error = true;
                        tb_proj_projectnaam.BackColor = Color.Crimson;
                        tb_proj_projectnaam.ForeColor = Color.White;
                    }
                    if (!adres_user_control3.invul_check())
                    {
                        error = true;
                    }
                    if (!error)
                    {
                        ok = true;
                    }
                    
                    break;
                case 2:
                   
                    //tb_bdr_telefoonnr.BackColor = Color.White;
                    tb_bdr_zoeknaam.BackColor = Color.White;
                    tb_bdr_zoeknaam.ForeColor = Color.Black;
                    tb_bdr_naam_bedrijf.BackColor = Color.White;
                    tb_bdr_naam_bedrijf.ForeColor = Color.Black;
                    error = false;
                    if (!adres_user_control1.invul_check())
                    {
                        error = true;
                    }
                    if (!adres_user_control2.invul_check())
                    {
                        error = true;
                    }
                    if (tb_bdr_naam_bedrijf.TextLength < 1)
                    {
                        error = true;
                        tb_bdr_naam_bedrijf.BackColor = Color.Crimson;
                        tb_bdr_naam_bedrijf.ForeColor = Color.White;
                        
                    }
                    if (!telefoonnummer_user_control1.check_verplicht())
                    {
                        error = true;
                        //tb_bdr_telefoonnr.BackColor = Color.Crimson;
                    }
                    if (tb_bdr_zoeknaam.TextLength < 1)
                    {
                        error = true;
                        tb_bdr_zoeknaam.BackColor = Color.Crimson;
                        tb_bdr_zoeknaam.ForeColor = Color.White;
                    }
                    if (!error)
                    {
                        ok = true;
                    }
                    break;
                case 3:
                     error = false;
                    cb_pers_bedrijfnr.BackColor = Color.White;
                    cb_pers_bedrijfnr.ForeColor = Color.Black;
                    tb_pers_voornaam.BackColor = Color.White;
                    tb_pers_voornaam.ForeColor = Color.Black;
                    tb_pers_achternaam.BackColor = Color.White;
                    tb_pers_achternaam.ForeColor = Color.Black;
                    rb_pers_man.BackColor = Color.White; 
                    rb_pers_vrouw.BackColor = Color.White;
                    tb_pers_tussenvoegsel.BackColor = Color.White;
                    tb_pers_tussenvoegsel.ForeColor = Color.Black;
                    tb_pers_voorletters.BackColor = Color.White;
                    tb_pers_voorletters.ForeColor = Color.Black;
                    pers_rb_tussvg1.BackColor = Color.White; pers_rb_tussvg2.BackColor = Color.White;
                    if (cb_pers_bedrijfnr.SelectedIndex < 0)
                    {
                        error = true;
                        cb_pers_bedrijfnr.BackColor = Color.Crimson;
                        cb_pers_bedrijfnr.ForeColor = Color.White;
                    }
                     if (tb_pers_achternaam.TextLength < 1)
                     {
                         error = true;
                         tb_pers_achternaam.BackColor = Color.Crimson;
                         tb_pers_achternaam.ForeColor = Color.White;
                     }
                     if (tb_pers_tussenvoegsel.TextLength < 1 && pers_rb_tussvg1.Checked)
                     {
                         error = true;
                         tb_pers_tussenvoegsel.BackColor = Color.Crimson;
                         tb_pers_tussenvoegsel.ForeColor = Color.White;
                     }

                     if (!pers_rb_tussvg1.Checked && !pers_rb_tussvg2.Checked)
                     {
                         error = true;
                         pers_rb_tussvg1.BackColor = Color.Crimson; pers_rb_tussvg2.BackColor = Color.Crimson;
                     }
                     if (tb_pers_voorletters.TextLength < 1)
                     {
                         error = true;
                         tb_pers_voorletters.BackColor = Color.Crimson;
                         tb_pers_voorletters.ForeColor = Color.White;
                     }
                     if (tb_pers_voornaam.TextLength < 1)
                     {
                         error = true;
                         tb_pers_voornaam.BackColor = Color.Crimson;
                         tb_pers_voornaam.ForeColor = Color.White;
                     }
                     if (!rb_pers_man.Checked && !rb_pers_vrouw.Checked)
                     {
                         error = true;
                         rb_pers_man.BackColor = Color.Crimson; rb_pers_vrouw.BackColor = Color.Crimson;
                     }
                     if (!error)
                     {
                         ok = true;
                     }
                    break;
            }
            return ok;
        }
        private bool dubbel_check()
        {
            bool ok = false;
            if (Global.Dubbel_inv_check && !NewIsUniek )
            {
                int result = 0;
                switch (type)
                {
                    case 1:
                         result =dubblinv_linq.Get_doublicates_hxzr_proj(tb_proj_projectnaam.Text, adres_user_control3.Plaats, int.Parse(cb_proj_opdrachtgever.SelectedValue.ToString())).Count;
                         if (result > 0)
                         {
                             Global.dub_proj_naamproj = tb_proj_projectnaam.Text; Global.dub_proj_plaats =adres_user_control3.Plaats; Global.dub_proj_opdrachtgever = int.Parse(cb_proj_opdrachtgever.SelectedValue.ToString());
                              newrecord_form_helper.Start_route(1, 5, this, start_parent, id, veldnaam, ownernaam);
                             this.Hide();
                         }
                         else
                         {
							 handelingen_logger.log_handeling(id, type, 30);
                             ok = true;
                         }
                         break;
                    case 2:
                         result =  dubblinv_linq.Get_doublicates_hxzr_bdr(tb_bdr_naam_bedrijf.Text, tb_bdr_zoeknaam.Text, adres_user_control1.Postcode_cijfers.ToString()+adres_user_control1.Postcode_letters).Count;
                        if (result > 0)
                        {
                            Global.dub_bdr_naam = tb_bdr_naam_bedrijf.Text; Global.dub_bdr_zoeknaam = tb_bdr_zoeknaam.Text; Global.dub_bdr_postcode = adres_user_control1.Postcode_cijfers.ToString() + adres_user_control1.Postcode_letters;
                            newrecord_form_helper.Start_route(2, 5, this, start_parent, id, veldnaam, ownernaam);
                            this.Hide();
                        }
                        else
                        {
							handelingen_logger.log_handeling(id, type, 30);
                            ok = true;
                        }
                        break;
                    case 3:
                        bool mv = false;
                        if (rb_pers_man.Checked)
                        { mv = false; }
                        else
                        { mv = true; }
                        result = (int)pers_adapt.DubbelInv_pers(tb_pers_voornaam.Text, tb_pers_voorletters.Text, tb_pers_achternaam.Text, int.Parse(cb_pers_bedrijfnr.SelectedValue.ToString()),mv);
                        if (result > 0)
                        {
                            Global.dub_pers_voornaam = tb_pers_voornaam.Text; Global.dub_pers_voorletters = tb_pers_voorletters.Text; Global.dub_pers_achternaam = tb_pers_achternaam.Text;
                            Global.dub_pers_bedrijf_nr = int.Parse(cb_pers_bedrijfnr.SelectedValue.ToString()); Global.dub_pers_man = mv;
                            newrecord_form_helper.Start_route(3, 5, this, start_parent, id, veldnaam, ownernaam);
                            this.Hide();
                        }
                        else
                        {
							handelingen_logger.log_handeling(id, type, 30);
                            ok = true;
                        }
                        break;
                }
            }
            else
            {
                ok = true;
            }
            return ok;
        }
        public void extrn_opslaan()
        {
            NewIsUniek = true;
            EventArgs e = new EventArgs();
            button2_Click(this, e);
        }
        private void button2_Click(object sender, EventArgs e)// aanmaken
        {
            Cursor.Current = Cursors.WaitCursor;
            if (invoer_check())
            {
                if (dubbel_check() || NewIsUniek)
                {
                    NewIsUniek = false;
                    System.Data.SqlClient.SqlConnection con;
                    con = new System.Data.SqlClient.SqlConnection();
                    con.ConnectionString = Global.ConnectionString_fileserver;
                    con.Open();
                    string stopro = "";

                    SqlCommand command = new SqlCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    SqlParameter result = command.Parameters.Add("@result", SqlDbType.Int);
                    result.Direction = ParameterDirection.Output;
                    try
                    {
                        switch (type)
                        {
                            case 1:
                                stopro = "new_project";
                                SqlParameter proj_naam = command.Parameters.Add("@proj_naam", SqlDbType.NVarChar);
                                SqlParameter plaats = command.Parameters.Add("@plaats", SqlDbType.NVarChar);
                                SqlParameter opdrachtgever = command.Parameters.Add("@opd_geef", SqlDbType.Int);
                                SqlParameter status = command.Parameters.Add("@status", SqlDbType.Int);
                                SqlParameter proj_id = command.Parameters.Add("@proj_id", SqlDbType.Int);
                                SqlParameter adres_id_bouw = command.Parameters.Add("@adres_id_bouw", SqlDbType.Int);
                                SqlParameter spec = command.Parameters.Add("@spec", SqlDbType.Text);
                                
                                proj_naam.Value = tb_proj_projectnaam.Text;
                                proj_naam.Direction = ParameterDirection.Input;
                                plaats.Value = adres_user_control3.Plaats;
                                plaats.Direction = ParameterDirection.Input;
                                opdrachtgever.Value = cb_proj_opdrachtgever.SelectedValue;
                                opdrachtgever.Direction = ParameterDirection.Input;
                                status.Value = cb_proj_status.SelectedValue;
                                status.Direction = ParameterDirection.Input;
                                proj_id.Value = id;
                                proj_id.Direction = ParameterDirection.Input;
                                int? max_id3 = (int)adres_adapt.adres_max_id() + 1;
                                adres_id_bouw.Value = max_id3; adres_id_bouw.Direction = ParameterDirection.Input;
                                spec.Value = tb_proj_specificatie.Text;
                                spec.Direction = ParameterDirection.Input;
                                
                                adres_adapt.adres_new(max_id3, 
                                                      adres_user_control3.Land,
                                                      adres_user_control3.Plaats,
                                                      adres_user_control3.Straat,
                                                      adres_user_control3.Straat2,
                                                      adres_user_control3.Postcode_cijfers,
                                                      adres_user_control3.Postcode_letters,
                                                      adres_user_control3.Huisnummer,
                                                      adres_user_control3.Huisnummer_toevoeging,
                                                      false,
                                                      adres_user_control3.ViaPostcode,
                                                      adres_user_control3._City_key);
                                testsetTableAdapters.memosTableAdapter mta = new testsetTableAdapters.memosTableAdapter();
                                mta.Connection.ConnectionString = Global.ConnectionString_fileserver;
                                mta.new_memo(id, 1, 1, tb_proj_specificatie.Text);
                                break;
                            case 2:
                                stopro = "new_bedrijf";
                                SqlParameter naam = command.Parameters.Add("@naam", SqlDbType.NVarChar);
                                SqlParameter zoeknaam = command.Parameters.Add("@zoek_naam", SqlDbType.NVarChar);
                                SqlParameter telefoon = command.Parameters.Add("@telefoon", SqlDbType.NVarChar);
                                SqlParameter b_straat = command.Parameters.Add("@b_straat", SqlDbType.NVarChar);
                                SqlParameter b_post = command.Parameters.Add("@b_post", SqlDbType.NVarChar);
                                SqlParameter b_plaats = command.Parameters.Add("@b_plaats", SqlDbType.NVarChar);
                                SqlParameter p_straat = command.Parameters.Add("@p_straat", SqlDbType.NVarChar);
                                SqlParameter p_post = command.Parameters.Add("@p_post", SqlDbType.NVarChar);
                                SqlParameter p_plaats = command.Parameters.Add("@p_plaats", SqlDbType.NVarChar);
                                SqlParameter bdr_id = command.Parameters.Add("@bdr_id", SqlDbType.Int);
                                SqlParameter adres_id_bezoek = command.Parameters.Add("@adres_id_bezoek", SqlDbType.Int);
                                SqlParameter adres_id_post = command.Parameters.Add("@adres_id_post", SqlDbType.Int);
                                SqlParameter kvk_nr = command.Parameters.Add("@kvk_nr", SqlDbType.NVarChar);
                                naam.Value = tb_bdr_naam_bedrijf.Text; naam.Direction = ParameterDirection.Input;
                                zoeknaam.Value = tb_bdr_zoeknaam.Text; zoeknaam.Direction = ParameterDirection.Input;
                                KeyValuePair<string, bool> nummer_antw = telefoonnummer_user_control1.Get_Nummer();
                                telefoon.Value = nummer_antw.Key;
                                telefoon.Direction = ParameterDirection.Input;
                                //
                                //
                                //string[] bezoek = adres_user_control1.save_data();
                                //b_straat.Value = bezoek[1]; b_straat.Direction = ParameterDirection.Input;
                                //b_post.Value = bezoek[2]; b_post.Direction = ParameterDirection.Input;
                                //b_plaats.Value = bezoek[0]; b_plaats.Direction = ParameterDirection.Input;
                                ////
                                //string[] post = adres_user_control2.save_data();
                                //p_straat.Value = post[1]; p_straat.Direction = ParameterDirection.Input;
                                //p_post.Value = post[2]; p_post.Direction = ParameterDirection.Input;
                                //p_plaats.Value = post[0]; p_plaats.Direction = ParameterDirection.Input;
                                //bdr_id.Value = id; bdr_id.Direction = ParameterDirection.Input;
                                ///// save to adres tabel!!!!!
                                int? max_id = (int)adres_adapt.adres_max_id() + 1;
                                adres_adapt.adres_new(max_id,
                                                      adres_user_control1.Land,
                                                      adres_user_control1.Plaats,
                                                      adres_user_control1.Straat,
                                                      adres_user_control1.Straat2,
                                                      adres_user_control1.Postcode_cijfers,
                                                      adres_user_control1.Postcode_letters,
                                                      adres_user_control1.Huisnummer,
                                                      adres_user_control1.Huisnummer_toevoeging,
                                                      true,
                                                      adres_user_control1.ViaPostcode,
                                                      adres_user_control1._City_key);
                                int? max_id2 = (int)adres_adapt.adres_max_id() + 1;
                                adres_adapt.adres_new(max_id2,
                                                      adres_user_control2.Land,
                                                      adres_user_control2.Plaats,
                                                      adres_user_control2.Straat,
                                                      adres_user_control2.Straat2,
                                                      adres_user_control2.Postcode_cijfers,
                                                      adres_user_control2.Postcode_letters,
                                                      adres_user_control2.Huisnummer,
                                                      adres_user_control2.Huisnummer_toevoeging,
                                                      true,
                                                      adres_user_control2.ViaPostcode,
                                                      adres_user_control2._City_key);
                                adres_id_bezoek.Value = max_id; adres_id_bezoek.Direction = ParameterDirection.Input;
                                adres_id_post.Value = max_id2; adres_id_post.Direction = ParameterDirection.Input;
                                string kvknr = String.Empty;
                                if (zoekopkvk)
                                {
                                    kvknr = tb_bdr_kvk.Text;
                                }
                                else
                                {
                                    kvknr = null;
                                }
                                kvk_nr.Value = kvknr; kvk_nr.Direction = ParameterDirection.Input;
                                break;
                            case 3:
                                stopro = "new_persoon";
                                SqlParameter bedrijf_nr = command.Parameters.Add("@bedrijf_nr", SqlDbType.Int);
                                SqlParameter voornaam = command.Parameters.Add("@voornaam", SqlDbType.NVarChar);
                                SqlParameter voorletters = command.Parameters.Add("@voorletters", SqlDbType.NVarChar);
                                SqlParameter tussenvoegsel = command.Parameters.Add("@tussenvoegsel", SqlDbType.NVarChar);
                                SqlParameter achternaam = command.Parameters.Add("@achternaam", SqlDbType.NVarChar);
                                SqlParameter man_vrouw = command.Parameters.Add("@man_vrouw", SqlDbType.Bit);
                                SqlParameter pers_id = command.Parameters.Add("@pers_id", SqlDbType.Int);
                                bedrijf_nr.Value = cb_pers_bedrijfnr.SelectedValue; bedrijf_nr.Direction = ParameterDirection.Input;
                                voornaam.Value = tb_pers_voornaam.Text; voornaam.Direction = ParameterDirection.Input;
                                voorletters.Value = tb_pers_voorletters.Text; voorletters.Direction = ParameterDirection.Input;
                                tussenvoegsel.Value = tb_pers_tussenvoegsel.Text; tussenvoegsel.Direction = ParameterDirection.Input;
                                achternaam.Value = tb_pers_achternaam.Text; achternaam.Direction = ParameterDirection.Input;
                                if (rb_pers_man.Checked)
                                { man_vrouw.Value = false; }
                                else
                                { man_vrouw.Value = true; }
                                man_vrouw.Direction = ParameterDirection.Input;
                                pers_id.Value = id; pers_id.Direction = ParameterDirection.Input;

                                //insert contact in exchange lists
                                
                                
                                
                                var contactitem = new ExchangeComs.ExchangeContactItem();
                                if (tb_pers_tussenvoegsel.Text.Length > 0)
                                {
                                    contactitem.Tussenvoegsel = tb_pers_tussenvoegsel.Text.Trim();
                                }
                                if (tb_pers_achternaam.Text.Length > 0)
                                {
                                    contactitem.Achternaam = tb_pers_achternaam.Text.Trim();
                                }
                                contactitem.Voornaam = tb_pers_voornaam.Text.Trim();
                                var service = ExchangeComs.EWSFunctions.GetNewServiceHook();
                                ExchangeComs.EWSFunctions.MakeNewContact(service, contactitem);
                                break;
                        }
                        command.CommandText = stopro;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Connection = con;
                        int nwok = command.ExecuteNonQuery();
                       // GetLatesID(con);
                        id = (int)result.Value;
                        con.Close();
                        if (nwok > 0)
                        {
                            log();
                            MessageBox.Show("toevoegen gelukt");
                            open_record();
                        }
                        else
                        {
                            MessageBox.Show("toevoegen mislukt");
                        }
                    }
                    catch (Exception exep)
                    {
                        log_exception(exep);
                        //debug only!!
                        MessageBox.Show("Exception trown!! " + exep.Message);
                        MessageBox.Show("toevoegen mislukt");
                    }
                }
                else
                {
                    //dubbel inv check geeft results..
                }
            }
            else
            {
                MessageBox.Show("Controleer de gemarkeerde velden op correcte invoer");
            }
            Cursor.Current = Cursors.Default;
        }
        private void log()
        {

            System.Data.SqlClient.SqlConnection con;
            con = new System.Data.SqlClient.SqlConnection();
            con.ConnectionString = Global.ConnectionString_fileserver;
            con.Open();
            string stopro = "nieuw_record_log";

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            SqlParameter naam = command.Parameters.Add("@naam", SqlDbType.NVarChar);
            SqlParameter record_type = command.Parameters.Add("@type", SqlDbType.NVarChar);
            SqlParameter datum = command.Parameters.Add("@datum", SqlDbType.DateTime);
            SqlParameter record_ID = command.Parameters.Add("@id", SqlDbType.Int);
            string a;
            a = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
           string[] naam_array =  a.Split('\\');
           naam.Value = naam_array[naam_array.Length - 1];
           switch (type)
           {
               case 1:
                   record_type.Value = "project";
                   break;
               case 2:
                   record_type.Value = "bedrijf";
                   break;
               case 3:
                   record_type.Value = "persoon";
                   break;
           }
           datum.Value = DateTime.Now;
           naam.Direction = ParameterDirection.Input;
           record_type.Direction = ParameterDirection.Input;
           datum.Direction = ParameterDirection.Input;
           record_ID.Value = id; record_ID.Direction = ParameterDirection.Input;

           command.CommandText = stopro;
           command.CommandType = CommandType.StoredProcedure;
           command.Connection = con;
           command.ExecuteNonQuery();
           con.Close();
           
        }
        private void open_record()
        {
            Cursor.Current = Cursors.WaitCursor;
             nuniet = true;
             if (this.WindowState == FormWindowState.Normal)
             {
                 Global.size = this.Size;
                 Global.position = this.Location;
             }
            Global.windowstate = this.WindowState;
            Global.overzicht_select = id.ToString();
            
            switch (type)
            {
                case 1:
                    newrecord_form_helper.Start_route(1, 4, start_parent, close_parent, Parent_ID, veldnaam, ownernaam);
                    break;
                case 2:
                    newrecord_form_helper.Start_route(2, 4, start_parent, close_parent, Parent_ID, veldnaam, ownernaam);
                    break;
                case 3:
                    newrecord_form_helper.Start_route(3, 4, start_parent, close_parent, Parent_ID, veldnaam, ownernaam);
                    break;
            }

            if (close_parent is Menu)//normaal new
            {
                handelingen_logger.log_handeling(id, type, 2);
            }
            else if (close_parent is project_form)//rood proj
            {
                handelingen_logger.log_handeling(id, type, 3);
            }
            else if (close_parent is persoon_form)//rood contact
            {
                handelingen_logger.log_handeling(id, type, 4);
            }
           else if (close_parent is newrecord)//new>new
            {
                if((close_parent as newrecord).type == 1)//new-project>new>opdrachtgever
                {
                    handelingen_logger.log_handeling(id, type, 28);
                }
                else // new>persoon>new>werkgever
                {
                    handelingen_logger.log_handeling(id, type, 29);
                }
            }
            Cursor.Current = Cursors.Default;
            this.Close();
        }
        private void newrecord_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (skip_close || nuniet)
            {
                skip_close = false;
                nuniet = false;
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
                //laat sluiten
               
            }
        }
        
        private void newrecord_Resize(object sender, EventArgs e)
        {
           
            Point pt1 = new Point(panel1.Width / 2 - lbl_top.Width / 2, 5);
            lbl_top.Location = pt1;
            Point pt2 = new Point(panel1.Width / 2 - lbl_mid.Width / 2, 28);
            lbl_mid.Location = pt2;
            Point pt3 = new Point(panel1.Width / 2 - lbl_bot.Width / 2, panel1.Height -18);
            lbl_bot.Location = pt3;

            p_bezoekadres.Location = new Point(panel1.Width / 2 - p_bezoekadres.Width / 2, p_bezoekadres.Location.Y);
            p_contact_gegevens.Location = new Point(panel1.Width / 2 - p_contact_gegevens.Width / 2, p_contact_gegevens.Location.Y);
            p_pers_contgegef.Location = new Point(panel1.Width / 2 - p_pers_contgegef.Width / 2, p_pers_contgegef.Location.Y);
            p_postadres.Location = new Point(panel1.Width / 2 - p_postadres.Width / 2, p_postadres.Location.Y);
            p_proj_planning.Location = new Point(panel1.Width / 2 - p_proj_planning.Width / 2, p_proj_planning.Location.Y);
            p_proj_info.Location = new Point(panel1.Width / 2 - p_proj_info.Width / 2, p_proj_info.Location.Y);
            p_bdr_telnrs.Location = new Point(panel1.Width / 2 - p_bdr_telnrs.Width / 2, p_bdr_telnrs.Location.Y);
        }

        private void cb_proj_opdrachtgever_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (cb_proj_opdrachtgever.SelectedIndex > -1)
            //{ opdgeef = true; }
            //else
            //{
            //    opdgeef = false;
            //}
        }

        private void cb_proj_status_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cb_proj_status.SelectedIndex > -1)
            { status = true; }
            else
            {
                status = false;
            }
        }

        private void cb_pers_bedrijfnr_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (cb_pers_bedrijfnr.SelectedIndex != -1)
            //{ bedrijfnr = true; }
            //else
            //{
            //    bedrijfnr = false;
            //}
        }

        private void check_bezoek_post_CheckedChanged(object sender, EventArgs e)
        {
            if (check_bezoek_post.Checked)
            {
                adres_user_control2.Wijzigstand = false;

                adres_user_control2.ViaPostcode = adres_user_control1.ViaPostcode;
                adres_user_control2.Land = adres_user_control1.Land;
                adres_user_control2.Plaats = adres_user_control1.Plaats;
                adres_user_control2.Straat = adres_user_control1.Straat;
                adres_user_control2.Postcode_cijfers = adres_user_control1.Postcode_cijfers;
                adres_user_control2.Postcode_letters = adres_user_control1.Postcode_letters;
                adres_user_control2.Huisnummer = adres_user_control1.Huisnummer;
                adres_user_control2.Huisnummer_toevoeging = adres_user_control1.Huisnummer_toevoeging;
                adres_user_control2.Straat2 = adres_user_control1.Straat2;
            }
            else
            {
                adres_user_control2.Wijzigstand = true;
                adres_user_control2.ViaPostcode = false;
                adres_user_control2.Land = String.Empty;
                adres_user_control2.Plaats = String.Empty;
                adres_user_control2.Straat = String.Empty;
                adres_user_control2.Postcode_cijfers = 0;
                adres_user_control2.Postcode_letters = String.Empty;
                adres_user_control2.Huisnummer = String.Empty;
                adres_user_control2.Huisnummer_toevoeging = String.Empty;
                adres_user_control2.Straat2 = String.Empty;
            }
            Organize_Panels();
        }
        private void Organize_Panels()
        {
            Cursor.Current = Cursors.WaitCursor;
            using (Panel P = new Panel())
            {
                P.Location = panel_bedrijf.Location;
                P.Size = panel_bedrijf.Size;
                P.BackColor = System.Drawing.SystemColors.ControlLightLight;
                P.BorderStyle = BorderStyle.Fixed3D;
                this.Controls.Add(P);
                P.Show();
                P.BringToFront();

                int p_x = panel_bedrijf.Width / 2 - 216;
                Point scrollpos;
                scrollpos = panel_bedrijf.AutoScrollPosition;
                panel_bedrijf.AutoScrollPosition = new Point(0, 0);

                p_contact_gegevens.Location = new Point(p_x, 10);
                p_contact_gegevens.Size = bereken_panel_size(p_contact_gegevens);
                p_bezoekadres.Location = new Point(p_x, p_contact_gegevens.Height + p_contact_gegevens.Location.Y + 8);
                p_bezoekadres.Size = bereken_panel_size(p_bezoekadres);
                p_postadres.Location = new Point(p_x, p_bezoekadres.Height + p_bezoekadres.Location.Y + 8);
                p_postadres.Size = bereken_panel_size(p_postadres);
                p_bdr_telnrs.Location = new Point(p_x, p_postadres.Height + p_postadres.Location.Y + 8);
                scrollpos.X *= -1;
                scrollpos.Y *= -1;
                panel_bedrijf.AutoScrollPosition = scrollpos;
            }
            Cursor.Current = Cursors.Default;
        }
        private Size bereken_panel_size(Panel P)
        {
            Size Sz = new Size();
            int tmp_H = 42;
            int n_showed = 0;
            foreach (Control C in P.Controls)
            {
                if (C is SplitContainer)
                {
                    if (C.Visible)
                    {
                        C.Location = new Point(10, 25 * (n_showed + 1) + 5);
                        if (C.Height > 25)
                        {
                            n_showed += C.Height / 25;
                        }
                        else
                        {
                            n_showed++;
                        }
                        tmp_H += C.Height;
                        }
                }
                else if (C is System.Windows.Forms.Integration.ElementHost)
                {
                    if ((C as System.Windows.Forms.Integration.ElementHost).Visible)
                    {
                        (C as System.Windows.Forms.Integration.ElementHost).Child.Visibility = System.Windows.Visibility.Visible;
                        C.Location = new Point(10, 25 * (n_showed + 1) + 5);
                        if (C.Height > 25)
                        {
                            n_showed += C.Height / 25;
                        }
                        else
                        {
                            n_showed++;
                        }
                        tmp_H += C.Height;
                    }
                }
                else if (C is Button && P == p_bdr_telnrs)
                {
                    if (C.Visible && n_showed < 3)
                    {
                        n_showed++;
                        tmp_H += C.Height;
                        C.Location = new Point(347, 25 * n_showed + 5);
                    }
                }
            }
            if (tmp_H < 67)
            {
                tmp_H = 67;
            }
            Sz.Height = tmp_H;
            Sz.Width = 432;
            return Sz;

        }
        private void proj_opd_ov_Click(object sender, EventArgs e)
        {
            Global.overzicht_type = 2;
            
            if (this.WindowState == FormWindowState.Normal)
            {
                Global.size = this.Size;
                Global.position = this.Location;
            }
            Global.windowstate = this.WindowState;
            string tmpveldnaam;
            string tmpownernaam;
            if(type == 1)
            {
                if (tb_proj_projectnaam.Text.ToString() != "")
                {
                   tmpveldnaam= "de opdrachtgever ";
                    tmpownernaam = tb_proj_projectnaam.Text.ToString();
                    
                }
                else
                {
                   tmpveldnaam = "de opdrachtgever ";
                    tmpownernaam = "nieuw project ";
                   
                }
                Global.give_return = true;
                newrecord_form_helper.Start_route(2, 3, this, this, id, tmpveldnaam, tmpownernaam);
                Global.give_return = false;
            }
           
            this.Hide();
            return_proj = true;
        }

        private void pers_bdr_ov_Click(object sender, EventArgs e)
        {
            Global.overzicht_type = 2;
            string tmpveldnaam;
            string tmpownernaam;
            if (this.WindowState == FormWindowState.Normal)
            {
                Global.size = this.Size;
                Global.position = this.Location;
            }
            Global.windowstate = this.WindowState;
            if (tb_pers_voornaam.Text != "")
            {
                if (tb_pers_achternaam.Text != "")
                {
                    if (pers_rb_tussvg1.Checked)
                    {
                        tmpownernaam =  tb_pers_voornaam.Text + " " + tb_pers_tussenvoegsel.Text + " " + tb_pers_achternaam.Text;
                    }
                    else
                    {
                        tmpownernaam =   tb_pers_voornaam.Text + " " + tb_pers_achternaam.Text;
                    }
                 }
                else
                {
                    tmpownernaam =  tb_pers_voornaam.Text;
                    
                }
            }
            else if (tb_pers_achternaam.Text != "")
            {
                if (pers_rb_tussvg1.Checked)
                {
                    tmpownernaam = tb_pers_tussenvoegsel.Text + " " + tb_pers_achternaam.Text;
                }
                else
                {
                    tmpownernaam = tb_pers_achternaam.Text;
                }
              
            }
            else
            {
                tmpownernaam = "nieuw contact ";
                
            }
            tmpveldnaam = "bedrijf "; 
            Global.give_return = true;
            newrecord_form_helper.Start_route(2, 3, this, this, id, tmpveldnaam, tmpownernaam);
            this.Hide();
            Global.give_return = false;
            return_proj = false;
        }
        public void fillid()
        {
            this.bedrijf1TableAdapter.Connection.ConnectionString = Global.ConnectionString_fileserver;
            this.bedrijf2TableAdapter.Connection.ConnectionString = Global.ConnectionString_fileserver;
            this.bedrijf2TableAdapter.Fill(this.zeebregtsdbDataSet.bedrijf2);
            this.bedrijf1TableAdapter.Fill(this.zeebregtsdbDataSet.bedrijf1);
            //this.statusTableAdapter.Fill(this.testset.status);
            this.Show();
            if (return_proj)
            {
                cb_proj_opdrachtgever.SelectedValue = Global.return_id;
            }
            else
            {
                cb_pers_bedrijfnr.SelectedValue = Global.return_id;
            }
            lbl_mid.Focus();
        }

        private void pers_rb_tussvg1_CheckedChanged(object sender, EventArgs e)
        {
            if (pers_rb_tussvg1.Checked)
            {
                tb_pers_tussenvoegsel.Show(); tb_pers_tussenvoegsel.Clear();
            }
            else
            {
                tb_pers_tussenvoegsel.Hide();
            }
        }

        private void textBox_Enter(object sender, EventArgs e)
        {
            //TextBox tb = (TextBox)sender;
            //tb.BeginInvoke(new MethodInvoker(tb.SelectAll));
        }

        private void textBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //TextBox tb = (TextBox)sender;
            //if (37 == e.KeyValue)
            //{
            //    if (tb.TextLength == tb.SelectionLength)
            //    {
            //        tb.SelectionStart = 0;
            //        tb.SelectionLength = 0;
            //    }
            //}
        }
        private void textBoxNumOnly_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), "\\d+"))
            {
                return;

            }
            else if (e.KeyChar == '\b')
            {
                return;
            }
            e.Handled = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
            TextBox tb = sender as TextBox;
            if (tb.Text.Length > 7 && tb.Text.Length < 9)
            {
                btn_bdr_nee_kvk.Enabled = true;

            }
            else
            {
                btn_bdr_nee_kvk.Enabled = false;
            }
        }
        private void zoek_op_kvk(int kvknr)
        {
            tb_bdr_naam_bedrijf.ReadOnly = false;
            tb_bdr_zoeknaam.ReadOnly = false;
            telefoonnummer_user_control1.IsEnabled = true;
            p_bezoekadres.Show();
            p_postadres.Show();
            p_bdr_telnrs.Show();
            splitContainer5.Show();
            splitContainer6.Show();
            string[] bdrgegevens;
            string sURL;
            sURL = @"https://officieel.openkvk.nl/csv/" + kvknr;

            try
            {
                WebRequest wrGETURL;
                wrGETURL = WebRequest.Create(sURL);
                Stream objStream;
                objStream = wrGETURL.GetResponse().GetResponseStream();
                StreamReader objReader = new StreamReader(objStream);

                string sLine = "";
                int i = 0;
                var done = false;
                while (sLine != null)
                {
                    i++;
                    sLine = objReader.ReadLine();
                    if (sLine != null && !done && i >= 2)
                    {
                        sLine = sLine.Replace("\"", "");
                        bdrgegevens = sLine.Split(',');
                        if (bdrgegevens.Count() > 6)
                        {
                            tb_bdr_naam_bedrijf.Text = bdrgegevens[3];
                            tb_bdr_zoeknaam.Text = bdrgegevens[3];
                            adres_user_control1.load_data(bdrgegevens[6], bdrgegevens[4], bdrgegevens[5]);
                            done = true;
                        }
                        //tb_bdr_bezoek_plaats.Text = bdrgegevens[6];
                        // tb_bdr_bezoek_postcode.Text = bdrgegevens[5];
                        // tb_bdr_bezoek_straat.Text = bdrgegevens[4];

                    }
                }

            }
            catch (WebException we)
            {
                //log error
            }
        }
        private string Injectioncheck(string inputSQL)
        {
            string InputSQL = inputSQL;
            if (InputSQL.Length > 0)
            {
                for (int i = 0; i < blackList.Length; i++)
                {
                    if ((InputSQL.IndexOf(blackList[i], StringComparison.OrdinalIgnoreCase) >= 0))
                    {
                        //
                        //Handle the discovery of suspicious Sql characters here
                        //
                        MessageBox.Show("Ongeweste tekst of tekens ingevoerd");
                        InputSQL = "";

                    }
                }
                return InputSQL.Replace("'", "''");
            }
            else
            {
                return InputSQL;
            }
        }
        private void btn_bdr_ja_kvk_Click(object sender, EventArgs e)
        {
            if (zoekopkvk)
            {
                int nr; 
                int.TryParse(Injectioncheck(tb_bdr_kvk.Text), out nr);
                zoek_op_kvk(nr);
            }
            else
            {

                btn_bdr_ja_kvk.Hide();
                Button btn = sender as Button;
                if (btn.Name == "btn_bdr_ja_kvk")
                {
                    tb_bdr_kvk.Show();
                    lbl_bdr_zoekopkvk.Hide();
                    zoekopkvk = true;
                    btn_bdr_nee_kvk.Text = "vind";
                    btn_bdr_nee_kvk.Enabled = false;
                }
                else
                {
                    btn_bdr_nee_kvk.Hide();
                    zoekopkvk = false;
                    splitContainer8.Hide();
                    tb_bdr_naam_bedrijf.ReadOnly = false;
                    tb_bdr_zoeknaam.ReadOnly = false;
                    telefoonnummer_user_control1.IsEnabled = true;
                    p_bezoekadres.Show();
                    p_postadres.Show();
                    p_bdr_telnrs.Show();
                    splitContainer5.Show();
                    splitContainer6.Show();
                }
            }
            Organize_Panels();
            
            
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {
            telefoonnummer_user_control1.Api_get_areacode(adres_user_control1.Postcode_cijfers + adres_user_control1.Postcode_letters);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            String tmpveldnaam = "project specificatie ";
            int tmpProjectID = id;
            String tmpownrnm = "nieuw project";
            Global.windowstate = this.WindowState;
            if (this.WindowState != FormWindowState.Maximized)
            {
                Global.position = this.Location;
                Global.size = this.Size;
            }
            
            TextBrowser tb = new TextBrowser(tb_proj_specificatie.Text, tb_proj_specificatie, true, tmpveldnaam, tmpownrnm, tmpProjectID,this);
            tb.Show();
            this.Hide();
            panel_project.Focus();
            
        }

        private void proj_llabel_naamspec_Click(object sender, EventArgs e)
        {
            String tmpveldnaam = "project specificatie ";
            int tmpProjectID = id;
            String tmpownrnm = "nieuw project";
            TextBrowser tb = new TextBrowser(proj_llabel_naamspec.Text, proj_llabel_naamspec, false, tmpveldnaam, tmpownrnm, tmpProjectID,this);
            tb.ShowDialog();
        }

        private void tb_proj_projectnaam_TextChanged(object sender, EventArgs e)
        {
            if(tb_proj_projectnaam.Text.EndsWith(" "))
            {
                foreach(string str in ignorelist)
                {
                    if (tb_proj_projectnaam.Text.Contains(str))
                    {
                        tb_proj_projectnaam.Text.TrimEnd(' ');
                        MessageBox.Show("Informatie m.b.t. \""+str+"\" hoort thuis in het veld \"project specificatie\".");
                        break;
                    }
                }
            }
        }
        private void show_mem_btn(TextBox tb, Button B)
        {
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
            if (tb.Height / 4 * 3 < s.Height)
            {
                B.Show();
            }
            else
            {
                B.Hide();
            }
        }
        private void tb_proj_specificatie_KeyPress(object sender, KeyPressEventArgs e)
        {
            show_mem_btn(tb_proj_specificatie, button3);
            proj_llabel_naamspec.Text = wrap_text(tb_proj_specificatie.Text);
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
                    if (tekst.IndexOf(Environment.NewLine, previndxof) > i || !tekst.Contains(Environment.NewLine))
                    {
                        int space = SB.ToString().LastIndexOf(' ', previndxof);
                        SB.Insert(space + 1, Environment.NewLine);
                    }

                    previndxof = i + Environment.NewLine.Length;
                }
            }

            return tekst = SB.ToString();
        }

        private void proj_naamspec_tooltip_MouseHover(object sender, EventArgs e)
        {
            if (button3.Visible)
            {
                tt_memo.AutoPopDelay = 10000;
                tt_memo.InitialDelay = 200;
                tt_memo.IsBalloon = true;
                tt_memo.ReshowDelay = 300;
                tt_memo.ShowAlways = true;
                string tt_text = "Gebruik de pijl om over te schakelen naar een uitgebreider invoerscherm";
                Control c = (Control)sender;
                if (c is SplitterPanel)
                {
                    tt_memo.Show(tt_text, (c.Parent as SplitContainer).Panel1, 0, -40);
                }
                else if (c is Label || c is Button)
                {
                    tt_memo.Show(tt_text, (c.Parent.Parent as SplitContainer).Panel1, 0, -40);
                }
            }
        }

        private void proj_naamspec_tooltip_MouseLeave(object sender, EventArgs e)
        {
            Control c = sender as Control;
            if (c is SplitterPanel)
            {
                try { tt_memo.Hide((c.Parent as SplitContainer).Panel1); }
                catch (Exception ex)
                {
                    log_exception(ex);
                }
            }
            else if (c is Label || c is Button)
            {
                try { tt_memo.Hide((c.Parent.Parent as SplitContainer).Panel1); }
                catch (Exception ex)
                {
                    log_exception(ex);
                }
            }
            if (c is Button)
            {
                Button btn = sender as Button;
                btn.FlatStyle = FlatStyle.Flat;
            }
        }

        private void pers_bdr_ov_MouseEnter(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            btn.FlatStyle = FlatStyle.Standard;
        }

        private void pers_bdr_ov_MouseLeave(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            btn.FlatStyle = FlatStyle.Flat;
        }

        private void p_bdr_telnrs_Enter(object sender, EventArgs e)
        {
            telefoonnummer_user_control1.Api_get_areacode(adres_user_control1.Postcode_cijfers + adres_user_control1.Postcode_letters);
        }

    }
}
