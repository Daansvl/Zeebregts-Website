using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlTypes;
using System.Threading;
using zeebregtsCs.usercontrols;
using System.Diagnostics;

namespace zeebregtsCs
{
    public partial class bedrijf_form : base_form
    {
        //initializer
        bedrijfformdatasetTableAdapters.bedrijfTableAdapter adapter = new bedrijfformdatasetTableAdapters.bedrijfTableAdapter();
        bedrijfformdataset.bedrijfDataTable bedrijftable;
        bedrijfformdataset.bedrijfRow Rowbedrijf;
        AdresDataSetTableAdapters.adressenTableAdapter aTA = new AdresDataSetTableAdapters.adressenTableAdapter();
        AdresDataSet.adressenDataTable aDT = new AdresDataSet.adressenDataTable();
        AdresDataSet.adressenRow aRow;
        DataSet ds1;
        System.Data.SqlClient.SqlConnection con;
        System.Data.SqlClient.SqlDataAdapter da;
        //new vars
        //joost test regel
        ToolTip tip1;
        int enum_nr = -1;
        string lbltopstr = "";
        bool[] ingevulde_nrs = { false, false, false };
        bool van_zoek = false;
        string antw = "";
        //
        string vrijgeefstring = "";
        bool editing = false;
        bedrijfformdatasetTableAdapters.bedrijfTableAdapter btcacbb = new bedrijfformdatasetTableAdapters.bedrijfTableAdapter();
        bedrijfformdataset.bedrijfDataTable bdtcbb = new bedrijfformdataset.bedrijfDataTable();
        string id = "0";
        int MaxRows = 0;
        
        
       
        
        private bool[] dates = new bool[2];

        public bedrijf_form(base_form start_scherm, base_form close_naar, int P_Id, string vnm, string own, int enumr)
        {
            adapter.Connection.ConnectionString = Global.ConnectionString_fileserver;
            aTA.Connection.ConnectionString = Global.ConnectionString_fileserver;
            btcacbb.Connection.ConnectionString = Global.ConnectionString_fileserver;

            Cursor.Current = Cursors.WaitCursor;
            //Global.FW_add(this, close_naar);
            this.DoubleBuffered = true;
            FormManager.VoegToe(this, close_naar);
            InitializeComponent();
            start_parent = start_scherm;
            close_parent = close_naar;
            Parent_ID = P_Id;
            veldnaam = vnm;
            ownernaam = own;
            enum_nr = enumr;
            this.Text = Global.WTitle;
            inititaliseer();
        }

       
        private void inititaliseer()
        {
            van_zoek = Global.van_zoek_bdr;
            switch (enum_nr)
            {
                case 0://normaal
                        if (van_zoek || Global.Dubb_van_zoek)
                        {
                            ChangeBtnImage(1); //terug naar zoek
                            Global.Dubb_van_zoek = false;
                        }
                        else
                        { ChangeBtnImage(2);}//alle bedrijven
                        lbl_top.Text = "";
                        lbl_mid.Text = "Info bedrijf";
                    break;
                case 1://nieuw clean
                        btnterug.Hide();
                        if (van_zoek)
                        { ChangeBtnImage(1); }//terug naar zoek
                        else
                        {ChangeBtnImage(2);}//alle bedrijven
                        lbl_mid.Text = "Nieuw bedrijf";
                        lbl_top.Text = "";
                    break;
                case 2://nieuw met owner
                        btnterug.Hide();
                        lbl_mid.Text = "Nieuw bedrijf";
                        if (close_parent is persoon_form)
                        {
                            lbl_top.Text = "Kies " + veldnaam + "van het contact " + ownernaam;
                        }
                        else
                        {
                            lbl_top.Text = "Kies " + veldnaam + "van het project " + ownernaam;
                        }
                    break;
                case 3://normaal subroute
                        if (close_parent is persoon_form)
                        { 
                            ChangeBtnImage(3);
                            lbl_top.Text = "Info van " + veldnaam + "van het contact " + ownernaam;
                        }//terug naar info contact
                        else
                        {
                            ChangeBtnImage(4);
                            lbl_top.Text = "Info van " + veldnaam + "van het project " + ownernaam;
                        }//terug naar info project
                        lbl_mid.Text = "Info bedrijf";
                        break;
                case 4:
                        lbl_top.Text = "Bekijk de gegevens t.b.v. de dubbele invoer controle";
                        lbl_mid.Text = "Overeenkomstig bedrijf";
                        ChangeBtnImage(6);
                        break;
            }
            init_all();
        }
        private void init_all()
        {
            
            button1.PerformClick();
            p_verw_verv.Hide();
            adres_user_control1.initialiseer(false,false);
            adres_user_control2.initialiseer(false,false);
            telefoonnummer_user_control1.set_naam(0,false);
            telefoonnummer_user_control2.set_naam(1,false);
            telefoonnummer_user_control3.set_naam(2,false);
            telefoonnummer_user_control1.Minder_opties();
            p_verw_verv.Hide();
            if (Global.UserLevel < 3)
            {
                button3.Show();
            }
            else
            {
                button3.Hide();
            }
            

            id = Global.overzicht_select;
            btn_verwijder.Hide();
             this.btnwijzig.Hide();
            this.btnopslaan.Show();
            btn_verwijder_annulleer.Hide();
            btn_verwijder_bevestig.Hide();
            Global.overzicht_select = "";
            dateTimePicker2.Format = DateTimePickerFormat.Custom;
            dateTimePicker2.CustomFormat = "dddd dd - MMMM MM - yyyy";
            label2.Text = "-";
            label23.Text = "-";
            label25.Text = "-";
            linkLabel2.Text = "-";
            label37.Text = "-";
            label38.Text = "-";
            label39.Text = "-";
            label41.Text = "-";
            label42.Text = "-";
            label43.Text = "-";
            label44.Text = "-";
            label46.Text = "-";
            label49.Text = "-";
            label51.Text = "-";
            label53.Text = "-";
            label55.Text = "-";
            label57.Text = "-";
            //blijf
            if (!Global.NoConnection)
            {
                //make and open db connection
                con = new System.Data.SqlClient.SqlConnection();
                con.ConnectionString = Global.ConnectionString_fileserver;
                con.Open();
                 //make database communication objects
                ds1 = new DataSet();
                string sql;
                sql = "SELECT * From bedrijf where bedrijf_nr = " + id + "";
                da = new System.Data.SqlClient.SqlDataAdapter(sql, con);
                da.Fill(ds1, "bedrijf");
                con.Close();
                //NavigateRecord();
                MaxRows = ds1.Tables["bedrijf"].Rows.Count;
                //StedenTableAdapters.StedenTableAdapter stedentabladapt = new StedenTableAdapters.StedenTableAdapter();
                //Steden.StedenDataTable stedendt = new Steden.StedenDataTable();
               // stedendt = stedentabladapt.GetData();

               // multiColumnComboBox1.BindingContext = new BindingContext();
               // multiColumnComboBox1.DataSource = stedendt;
               // multiColumnComboBox1.DisplayMember = "Woonplaats";
               // multiColumnComboBox1.ValueMember = "Woonplaats";
               // multiColumnComboBox1.SelectedIndex = -1;
            }
            Point pt1 = new Point(panel2.Width / 2 - lbl_top.Width / 2, 5);
            lbl_top.Location = pt1;
            Point pt2 = new Point(panel2.Width / 2 - lbl_mid.Width / 2, 28);
            lbl_mid.Location = pt2;
            Point pt3 = new Point(panel2.Width / 2 - lbl_bot.Width / 2, panel2.Height - 18);
            lbl_bot.Location = pt3;
            lbl_bot.Location = pt3;
            
            
            lbltopstr = lbl_top.Text;
            if (enum_nr == 4)
            {
                btnwijzig.Hide();
                btnopslaan.Hide();
            }

            bdtcbb = btcacbb.GetData();//.GetDataBy11(int.Parse(id));
            multiColumnComboBox1.BindingContext = new BindingContext();
            multiColumnComboBox1.DisplayMember = "zoeknaam";
            multiColumnComboBox1.ValueMember = "bedrijf_nr";
            multiColumnComboBox1.DataSource = bdtcbb;
            multiColumnComboBox1.SelectedIndex = -1;

            tip1 = new ToolTip();
            ToolTip_settings(tip1);
            no_edit();
        }
        private void ToolTip_settings(ToolTip tt)
        {

            tt.AutoPopDelay = 10000;
            tt.InitialDelay = 200;
            tt.IsBalloon = true;
            tt.ReshowDelay = 300;
            tt.ShowAlways = true;
        }
        private void bedrijf_form_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'steden._Steden' table. You can move, or remove it, as needed.
           // this.stedenTableAdapter.Fill(this.steden._Steden);
            this.MinimumSize = new Size(450, 310);
            if (enum_nr == 1 || enum_nr == 2)
            {
                btnwijzig.PerformClick();
            }
         //   this.Size = Global.size;
            disable_splitter(this);
          //  this.Location = Global.position;
           // this.WindowState = Global.windowstate;
            bedrijf_form_Resize(this, e);

            if (Global.Dubbel_is_bevestigd)
            {
                Global.Dubbel_is_bevestigd = false;
                if (start_parent.s_parent().c_parent() is project_form)
                {
                    Global.return_id = id;
                    close_parent = start_parent.c_parent();
                    (start_parent.s_parent().c_parent() as project_form).fill_id(false);
                    start_parent.s_parent().c_parent().herlaad();
                    FormManager.Sluit_forms(start_parent.s_parent());
                    Cursor.Current = Cursors.Default;
                    this.Close();
                }
                else if (start_parent.s_parent().c_parent() is persoon_form)
                {
                    Global.return_id = id;
                    (start_parent.s_parent().c_parent() as persoon_form).fill_id(false);
                    Cursor.Current = Cursors.Default;
                    FormManager.Sluit_forms(start_parent.s_parent());
                    this.Close();
                }
                else
                {
                    start_parent = start_parent.s_parent().s_parent();
                    close_parent = start_parent.c_parent();
                    //  FormManager.Sluit_form(start_parent.s_parent());
                    // FormManager.Sluit_form(start_parent);
                    enum_nr = 0;
                    //do_edit();
                    // inititaliseer();
                }
            }
           panel1.Focus();
            button1.PerformClick();
            Cursor.Current = Cursors.Default;
            
        }
        public override void save()
        {
            btnopslaan.PerformClick();
        }
        public void fill_id(bool n)
        {
            string a = Global.return_id;
            Global.return_id = "";
            if (vervangt)
            {
                bdtcbb = btcacbb.GetData();
                multiColumnComboBox1.SelectedValue = int.Parse(a);
                if (n)
                {
                    this.Show();
                }
            }
        }
        private void show_all_containers(Control c)
        {
            foreach (Control sc in c.Controls)
            {
                if (sc is SplitContainer)
                {
                    sc.Show();
                }
                if (sc.Controls.Count > 0)
                {
                    show_all_containers(sc);
                }
            }
            if (label64.Text == String.Empty)
            {
                label64.Parent.Parent.Hide();
            }
        }
        private void no_edit()
        {
            using (Panel tmp_P = new Panel())
            {
                tmp_P.Location = panel1.Location;
                tmp_P.Size = panel1.Size;
                tmp_P.BackColor = System.Drawing.SystemColors.ControlLightLight;
                tmp_P.BorderStyle = BorderStyle.Fixed3D;
                this.Controls.Add(tmp_P);
                tmp_P.Show();
                tmp_P.BringToFront();
                button4.Enabled = true;
                button7.Enabled = true;
                button6.Hide();
                Clear_text(this);
                NavigateRecord();
                checkBox1.Hide();
                splitContainer28.Hide();
                label60.Show();
                textBox28.Hide();
                textBox22.Hide();
                textBox18.Hide();
                label58.Hide();
                label40.Hide();
                label22.Hide();
                label18.Hide();
                textBox29.Hide();
                label62.Show();
                //Clear_text();
                //inc = int.Parse(id);
                this.textBox1.Hide();
                this.textBox3.Hide();
                this.textBox13.Hide();
                this.textBox14.Hide();
                this.textBox15.Hide();
                this.textBox16.Hide();
                this.textBox17.Hide();
                this.textBox19.Hide();
                this.textBox20.Hide();
                this.textBox21.Hide();
                textBox23.Hide();
                textBox24.Hide();
                textBox25.Hide();
                textBox26.Hide();
                textBox27.Hide();
                label2.Show();
                label23.Show();

                label25.Show();
                //   label32.Show();
                //   label33.Show();
                //   label34.Show();
                linkLabel2.Show();
                linkLabel1.Show();
                label37.Show();
                label38.Show();
                label39.Show();
                //  label40.Show();
                label41.Show();
                label42.Show();
                label43.Show();
                label44.Show();
                label46.Show();
                label49.Show();
                label51.Show();
                label53.Show();
                label55.Show();
                label57.Show();
                radioButton1.Hide();
                radioButton2.Hide();
                this.btnopslaan.Hide();
                this.btnwijzig.Show();
                // NavigateRecord();
                dateTimePicker2.Hide();
                textBox2.Hide();
                Organize_panels();
            }
        }
        private void do_edit()
        {
            using (Panel tmp_P = new Panel())
            {
                tmp_P.Location = panel1.Location;
                tmp_P.Size = panel1.Size;
                tmp_P.BackColor = System.Drawing.SystemColors.ControlLightLight;
                tmp_P.BorderStyle = BorderStyle.Fixed3D;
                this.Controls.Add(tmp_P);
                tmp_P.Show();
                tmp_P.BringToFront();
                show_all_containers(this);
                label60.Hide();
                if (!elementHost3.Visible || !elementHost4.Visible || !elementHost5.Visible)
                {
                    button6.Show();
                }
                checkBox1.Show();
                textBox28.Show();
                textBox22.Show();
                textBox18.Show();
                label58.Show();
                label40.Show();
                label22.Show();
                label18.Show();
                label62.Hide();
                textBox29.Show();
                this.textBox1.Show();
                this.textBox3.Show();
                splitContainer28.Show();
                //p_bezoekadres.Height = 192;
                if (checkBox1.Checked == false)
                {
                    // p_postadres.Height = 192;
                }
                //Point pt = groupBox6.Location;
                //pt.Y += 100;
                //groupBox6.Location = pt;

                this.textBox13.Show();
                this.textBox14.Show();
                this.textBox15.Show();
                this.textBox16.Show();
                this.textBox17.Show();

                this.textBox19.Show();
                this.textBox20.Show();
                this.textBox21.Show();

                textBox23.Show();
                textBox24.Show();
                textBox25.Show();
                textBox26.Show();
                textBox27.Show();
                label2.Hide();
                label23.Hide();

                label25.Hide();
                // label26.Hide();
                //    label27.Hide();
                //   label28.Hide();
                //    label29.Hide();
                // label30.Hide();
                //label31.Hide();
                //label32.Hide();
                //label33.Hide();
                //label34.Hide();
                linkLabel2.Hide();
                linkLabel1.Hide();
                label37.Hide();
                label38.Hide();
                label39.Hide();

                label41.Hide();
                label42.Hide();
                label43.Hide();

                label44.Hide();
                label46.Hide();
                label49.Hide();
                label51.Hide();
                label53.Hide();
                label55.Hide();
                label57.Hide();
                dateTimePicker2.Show();
                textBox2.Show();
                radioButton1.Show();
                radioButton2.Show();
                this.btnwijzig.Hide();
                this.btnopslaan.Show();
                button4.Enabled = false;
                button7.Enabled = false;
                //   splitContainer31.Hide();
                /* foreach (SplitContainer sc in groupBox2.Controls)
                 {
                     if (sc != splitContainer31 && sc.Visible == true)
                     {
                         sc.Location = new Point(6, sc.Location.Y - 25);
                     }
                 }*/
                Organize_panels();
                 Get_Set_plaats();
            }
        }
        private void set_telnr_type(string settingss)
        {
            string[] types = settingss.Split(',');
            telefoonnummer_user_control1.set_naam(int.Parse(types[0]),false);
            telefoonnummer_user_control2.set_naam(int.Parse(types[1]), false);
            telefoonnummer_user_control3.set_naam(int.Parse(types[2]), false);
        }
        private void Navigate_adres()
        {
            if (!Rowbedrijf.Isadres_id_bezoekNull() && Rowbedrijf.adres_id_bezoek > 0)
            {
                aTA.FillBy(aDT, Rowbedrijf.adres_id_bezoek);

                aRow = aDT.Rows[0] as AdresDataSet.adressenRow;
                adres_user_control1.ViaPostcode = aRow.via_postcode;
                if (aRow != null)
                {
                    if (!aRow.Iscity_keyNull())
                    {
                        adres_user_control1.LoadedCityKey = aRow.city_key;
                    }
                    if (!aRow.Ishuis_postbus_nummerNull())
                    {
                        adres_user_control1.Huisnummer = aRow.huis_postbus_nummer;
                    }
                    if (!aRow.Ishuisnummer_toevoegingNull())
                    {
                        adres_user_control1.Huisnummer_toevoeging = aRow.huisnummer_toevoeging;
                    }
                    if (!aRow.IslandNull())
                    {
                        adres_user_control1.Land = aRow.land;
                    }
                    if (!aRow.IsplaatsNull())
                    {
                        adres_user_control1.Plaats = aRow.plaats;
                    }
                    if (!aRow.Isstraat_1Null())
                    {
                        adres_user_control1.Straat = aRow.straat_1;
                    }
                    if (!aRow.Ispostcode_cijfersNull())
                    {
                        adres_user_control1.Postcode_cijfers = aRow.postcode_cijfers;
                    }
                    if (!aRow.Ispostcode_lettersNull())
                    {
                        adres_user_control1.Postcode_letters = aRow.postcode_letters;
                    }
                    if (!aRow.Isstraat_2Null())
                    {
                        adres_user_control1.Straat2 = aRow.straat_2;
                    }
                    

                }
            }
            //////////////////////
            if (!Rowbedrijf.Isadres_id_postNull() && Rowbedrijf.adres_id_post > 0)
            {

                aDT.Clear();
                aTA.FillBy(aDT, Rowbedrijf.adres_id_post);
				if (aDT.Rows.Count > 0)
				{
					aRow = aDT.Rows[0] as AdresDataSet.adressenRow;
					adres_user_control2.ViaPostcode = aRow.via_postcode;
					if (aRow != null)
					{
                        if (!aRow.Iscity_keyNull())
                        {
                            adres_user_control2.LoadedCityKey = aRow.city_key;
                        }
						if (!aRow.Ishuis_postbus_nummerNull())
						{
							adres_user_control2.Huisnummer = aRow.huis_postbus_nummer;
						}
						if (!aRow.Ishuisnummer_toevoegingNull())
						{
							adres_user_control2.Huisnummer_toevoeging = aRow.huisnummer_toevoeging;
						}
						if (!aRow.IslandNull())
						{
							adres_user_control2.Land = aRow.land;
						}
						if (!aRow.IsplaatsNull())
						{
							adres_user_control2.Plaats = aRow.plaats;
						}
						if (!aRow.Isstraat_1Null())
						{
							adres_user_control2.Straat = aRow.straat_1;
						}
						if (!aRow.Ispostcode_cijfersNull())
						{
							adres_user_control2.Postcode_cijfers = aRow.postcode_cijfers;
						}
						if (!aRow.Ispostcode_lettersNull())
						{
							adres_user_control2.Postcode_letters = aRow.postcode_letters;
						}
						if (!aRow.Isstraat_2Null() || aRow.straat_2 != String.Empty)
						{
							adres_user_control2.Straat2 = aRow.straat_2;
						}

					}
				}
                if (adres_user_control1.Postcode_cijfers == adres_user_control2.Postcode_cijfers && adres_user_control1.Postcode_letters == adres_user_control2.Postcode_letters && adres_user_control1.Huisnummer == adres_user_control2.Huisnummer)
                {
                    checkBox1.Checked = true;
                }
            }
            
        }
        private void NavigateRecord()
        {
            try
            {
                
                adapter = new bedrijfformdatasetTableAdapters.bedrijfTableAdapter();
                adapter.Connection.ConnectionString = Global.ConnectionString_fileserver;
                bedrijftable = adapter.GetDataBy11(int.Parse(id));
                Rowbedrijf = (bedrijfformdataset.bedrijfRow)bedrijftable.Rows[0];
                String zoeknaam = "";
                //naam
                if (!Rowbedrijf.IsnaamNull())
                {
                    label23.Parent.Parent.Show();
                    zoeknaam += Rowbedrijf.naam;
                    textBox1.Text = Rowbedrijf.naam;
                    label23.Text = Rowbedrijf.naam;
                    vrijgeefstring = zoeknaam;
                }
                else
                {
                    label23.Parent.Parent.Hide();
                    label23.Text = "-";
                }
                verwijder_panel1.start_init(2, int.Parse(id));
				if(verwijder_panel1.Get_function()[0] == null)
				{
					verwijder_panel1.Save_function(String.Empty);
				}
                string[] functies = verwijder_panel1.Get_function();
               
				textBox29.Text = functies[0];
                if (functies[0].ToString().Length > 2)
                {
                    label62.Parent.Parent.Show();
                    label62.Text = functies[0];
                }
                else
                {
                    label64.Text = String.Empty;
                    label62.Parent.Parent.Hide();
                }
                if (functies[1].ToString().Length > 2)
                {
                    label64.Parent.Parent.Show();
                    label64.Text = functies[1];

                }
                else
                {
                    label64.Text = String.Empty;
                    label64.Parent.Parent.Hide();
                }
                /////////newlog
                bedrijfformdatasetTableAdapters.new_del_record_logTableAdapter NdrL_adapt = new bedrijfformdatasetTableAdapters.new_del_record_logTableAdapter();
                NdrL_adapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
                bedrijfformdataset.new_del_record_logDataTable NdrL_dt = NdrL_adapt.GetData(int.Parse(id), "bedrijf");
                if (NdrL_dt.Rows.Count > 0)
                {
                    bedrijfformdataset.new_del_record_logRow NdrL_row = (bedrijfformdataset.new_del_record_logRow)NdrL_dt.Rows[0];
                    if (!NdrL_row.IsnaamNull() && !NdrL_row.Isdatum_tijdNull())
                    {
                        label66.Text = "Dit bedrijf is aangemaakt door: " + NdrL_row.naam + " op: " + NdrL_row.datum_tijd.ToString();
                    }
                    else
                    {
                        label66.Text = "Het is niet bekend wanneer en door wie dit bedrijf is ingevoerd";
                    }
                }
                else
                {
                    label66.Text = "Het is niet bekend wanneer en door wie dit bedrijf is ingevoerd";
                }
                //zoeknaam
                if (!Rowbedrijf.IszoeknaamNull())
                {
                    label25.Parent.Parent.Show();
                    textBox3.Text = Rowbedrijf.zoeknaam;
                    label25.Text = Rowbedrijf.zoeknaam;
                }
                else
                {
                    label25.Parent.Parent.Hide();
                    label25.Text = "-";
                }
                
                //email
                if (!Rowbedrijf.IsemailNull())
                {
                    linkLabel2.Parent.Parent.Show();
                    textBox13.Text = Rowbedrijf.email;
                    linkLabel2.Text = Rowbedrijf.email;
                }
                else
                {
                    linkLabel2.Parent.Parent.Hide();
                    linkLabel2.Text = "-";
                }
                //website
                if (linkLabel1.Links.Count > 0)
                {
                    linkLabel1.Links.Remove(linkLabel1.Links[0]);
                }
                if (!Rowbedrijf.IswebsiteNull())
                {
                    linkLabel1.Parent.Parent.Show();
                    textBox14.Text = Rowbedrijf.website;
                    linkLabel1.Text = Rowbedrijf.website;
                    linkLabel1.Links.Add(0, Rowbedrijf.website.Length, Rowbedrijf.website);
                }
                else
                {
                    linkLabel1.Parent.Parent.Hide();
                    linkLabel1.Text = "-";
                }
                //actief ja/nee
                if (!Rowbedrijf.IsNIETactiefNull())
                {
                    if (Rowbedrijf.NIETactief)
                    {
                        radioButton1.Checked = false;
                        radioButton2.Checked = true;
                        label2.Text = "nee";
                    }
                    else
                    {
                        radioButton2.Checked = false;
                        radioButton1.Checked = true;
                        label2.Text = "ja";
                    }
                    lbl_bot.Text = "ID " + id + " - " + zoeknaam;
                }
                else
                {
                    radioButton2.Checked = false;
                    radioButton1.Checked = true;
                    label2.Text = "actief";
                }
                //bezoekadres
                Navigate_adres();
                if (!Rowbedrijf.Istelefoon_nr_settingsNull())
                {
                    set_telnr_type(Rowbedrijf.telefoon_nr_settings);
                }
                string[] tellsets;
                if (!Rowbedrijf.Istelefoon_nr_settingsNull())
                {
                    tellsets = Rowbedrijf.telefoon_nr_settings.Split(',');
                }
                else
                {
                    tellsets = "0,1,2".Split(',');
                }
                if (!Rowbedrijf.Istelefoon_nr_1Null() && Rowbedrijf.telefoon_nr_1.Length > 2)
                {
                    elementHost3.Show();
                    telefoonnummer_user_control1.set_Nummer(Rowbedrijf.telefoon_nr_1, this);
                   
                }
                else
                {
                    elementHost3.Hide();
                    telefoonnummer_user_control1.set_Nummer("", this);
                }
                if (!Rowbedrijf.Istelefoon_nr_2Null() && Rowbedrijf.telefoon_nr_2.Length > 2)
                {
                    elementHost4.Show();
                    telefoonnummer_user_control2.set_Nummer(Rowbedrijf.telefoon_nr_2, this);
                }
                else
                {
                    elementHost4.Hide();
                    telefoonnummer_user_control2.set_Nummer("", this);
                }
                if (!Rowbedrijf.Istelefoon_nr_3Null() && Rowbedrijf.telefoon_nr_3.Length > 2)
                {
                    elementHost5.Show();
                    telefoonnummer_user_control3.set_Nummer(Rowbedrijf.telefoon_nr_3, this);
                }
                else
                {
                    elementHost5.Hide();
                    telefoonnummer_user_control3.set_Nummer("", this);
                }
                
                //straat
                int eq = 0;
                if (!Rowbedrijf.IsstraatNull())
                {
                    if(Rowbedrijf.IspostSTRAATNull())
                    {}
                    else if (Rowbedrijf.straat == Rowbedrijf.postSTRAAT)
                    {
                        eq += 1;
                    }
                }
                //postcode
                if (!Rowbedrijf.IspostcodeNull())
                {
                    if (Rowbedrijf.IspostPOSTCODENull())
                    { }
                    else if (Rowbedrijf.postcode == Rowbedrijf.postPOSTCODE)
                    {
                        eq += 1;
                    }
                }
                //plaats
                if (!Rowbedrijf.IsplaatsNull())
                {
                    if (Rowbedrijf.IspostPLAATSNull())
                    { }
                    else if (Rowbedrijf.plaats == Rowbedrijf.postPLAATS)
                    {
                        eq += 1;
                    }
                }
                //postadres
                
                //
                bool same = false;
                if ((!Rowbedrijf.Isadres_id_bezoekNull() && !Rowbedrijf.Isadres_id_postNull()))
                {
                    if (Rowbedrijf.adres_id_bezoek == Rowbedrijf.adres_id_post)
                    {
                        same = true;
                    }
                }
                else if (eq == 3)
                {
                    same = true;
                }

                if (same)
                {
                    checkBox1.Checked = true;
                    label60.Text = "bezoekadres is ook postadres";
                    
                }
                else
                {
                    label60.Text = "postadres wijkt af van bezoekadres";
                }
                
                //administratie

                //debiteurnr
                if (!Rowbedrijf.IsdebiteurNRzeebregtsNull())
                {
                    label53.Parent.Parent.Show();
                    textBox25.Text = Rowbedrijf.debiteurNRzeebregts;
                    label53.Text = Rowbedrijf.debiteurNRzeebregts;
                }
                else
                {
                    label53.Parent.Parent.Hide();
                    label53.Text = "-";
                }
                //crediteurnr
                if (!Rowbedrijf.IscrediteurNRzeebregtsNull())
                {
                    label55.Parent.Parent.Show();
                    textBox26.Text = Rowbedrijf.crediteurNRzeebregts;
                    label55.Text = Rowbedrijf.crediteurNRzeebregts;
                }
                else
                {
                    label55.Parent.Parent.Hide();
                    label55.Text = "-";
                }
                //kostenplaats
                if (!Rowbedrijf.IskostenplaatsNull())
                {
                    label57.Parent.Parent.Show();
                    textBox27.Text = Rowbedrijf.kostenplaats;
                    label57.Text = Rowbedrijf.kostenplaats;
                }
                else
                {
                    label57.Parent.Parent.Hide();
                    label57.Text = "-";
                }
                //bank
                if (!Rowbedrijf.IsbankNull())
                {
                    label37.Parent.Parent.Show();
                    textBox15.Text = Rowbedrijf.bank;
                    label37.Text = Rowbedrijf.bank;
                }
                else
                {
                    label37.Parent.Parent.Hide();
                    label37.Text = "-";
                }
                //Grek
                if (!Rowbedrijf.IsGrekNull())
                {

                    label38.Parent.Parent.Show();
                    textBox16.Text = Rowbedrijf.Grek;
                    label38.Text = Rowbedrijf.Grek;
                }
                else
                {
                    label38.Parent.Parent.Hide();
                    label38.Text = "-";
                }
                //IBAN
                if (!Rowbedrijf.IsIBANNull())
                {

                    label49.Parent.Parent.Show();
                    textBox23.Text = Rowbedrijf.IBAN;
                    label49.Text = Rowbedrijf.IBAN;
                }
                else
                {
                    label49.Parent.Parent.Hide();
                    label49.Text = "-";
                }
                //SWIFT
                if (!Rowbedrijf.IsSWIFTNull())
                {
                    label51.Parent.Parent.Show();
                    textBox24.Text = Rowbedrijf.SWIFT;
                    label51.Text = Rowbedrijf.SWIFT;
                }
                else
                {
                    label51.Parent.Parent.Hide();
                    label51.Text = "-";
                }
                //omzetbelastingnr
                if (!Rowbedrijf.IsomzetbelastingnrNull())
                {
                    label41.Parent.Parent.Show();
                    textBox19.Text = Rowbedrijf.omzetbelastingnr;
                    label41.Text = Rowbedrijf.omzetbelastingnr;
                }
                else
                {
                    label41.Parent.Parent.Hide();
                    label41.Text = "-";
                }
                //loonbelastingnr
                if (!Rowbedrijf.IsloonbelastingnrNull())
                {
                    label42.Parent.Parent.Show();
                    textBox20.Text = Rowbedrijf.loonbelastingnr;
                    label42.Text = Rowbedrijf.loonbelastingnr;
                }
                else
                {
                    label42.Parent.Parent.Hide();
                    label42.Text = "-";
                }
                //aansluitnrbedrijver
                if (!Rowbedrijf.IsbedrijfverenigingnrNull())
                {
                    label43.Parent.Parent.Show();
                    textBox21.Text = Rowbedrijf.bedrijfverenigingnr;
                    label43.Text = Rowbedrijf.bedrijfverenigingnr;
                }
                else
                {
                    label43.Parent.Parent.Hide();
                }
                //kvk
                if (!Rowbedrijf.IskvkNull())
                {
                    label39.Parent.Parent.Show();
                    textBox17.Text = Rowbedrijf.kvk;
                    label39.Text = Rowbedrijf.kvk;
                }
                else
                {
                    label39.Parent.Parent.Hide();
                    label39.Text = "-";
                }
                //kvkuitgifte

                if (!Rowbedrijf.IskvkUITGIFTENull())
                {
                    label44.Parent.Parent.Show();
                    dateTimePicker2.Value = Rowbedrijf.kvkUITGIFTE;
                    label44.Text = dateTimePicker2.Value.ToLongDateString();


                    textBox18.Text = dateTimePicker2.Value.Day.ToString();
                    textBox22.Text = dateTimePicker2.Value.Month.ToString();
                    textBox28.Text = dateTimePicker2.Value.Year.ToString();

                }
                else
                {
                    label44.Parent.Parent.Hide();
                    label44.Text = "-";
                }
                //kvkscan
                if (!Rowbedrijf.IskvkSCANNull())
                {
                    label46.Parent.Parent.Show();
                    textBox2.Text = Rowbedrijf.kvkSCAN;
                    label46.Text = textBox2.Text;
                }
                else
                {
                    label46.Parent.Parent.Hide();
                    label46.Text = "-";
                }
            }
            catch (Exception e)
            {
                String log_line = "crash program @ " + DateTime.Now.ToString() + "error: " + e;
                System.IO.StreamWriter file = new System.IO.StreamWriter(Global.log_file_path, true);
                file.WriteLine(log_line);
                file.Close();
                MessageBox.Show("fout bij inladen, wijzigen niet mogelijk");
                btnwijzig.Enabled = false; btnopslaan.Enabled = false;
                return;
            }

        }
        private void btnterug_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            antw = Is_ingebruik(int.Parse(id), 2);
            if (antw.Length > 1 && antw == Global.username)
            {
                if (!Verwijder_ingebruik(int.Parse(id), 2))
                {
                    MessageBox.Show("fout bij vrijgeven annuleer " + antw);

                }

            }
            if (this.WindowState == FormWindowState.Normal)
            {
                Global.size = this.Size;
                Global.position = this.Location;
            }
            Global.windowstate = this.WindowState;
            if (editing == true)
            {

                using (Panel tmp_P = new Panel())
                {
                    tmp_P.Location = panel1.Location;
                    tmp_P.Size = panel1.Size;
                    tmp_P.BackColor = System.Drawing.SystemColors.ControlLightLight;
                    tmp_P.BorderStyle = BorderStyle.Fixed3D;
                    this.Controls.Add(tmp_P);
                    tmp_P.Show();
                    tmp_P.BringToFront();

                    adres_user_control1.Wijzigstand = false;
                    adres_user_control2.Wijzigstand = false;
                    telefoonnummer_user_control1.Annuleer();
                    telefoonnummer_user_control2.Annuleer();
                    telefoonnummer_user_control3.Annuleer();
                    telefoonnummer_user_control1.Wijzigstand(false);
                    telefoonnummer_user_control2.Wijzigstand(false);
                    telefoonnummer_user_control3.Wijzigstand(false);
                    Navigate_adres();
                    btn_verwijder.Hide();
                    // NavigateRecord();

                    editing = false; this.wijzigstand = false;
                    switch (enum_nr)
                    {
                        case 0:
                            if (van_zoek)
                            { ChangeBtnImage(1); }//terug naar zoek                    
                            else
                            { ChangeBtnImage(2); }//alle bedrijven
                            break;
                        case 1:
                            break;
                        case 2:
                            break;
                        case 3:
                            if (close_parent is persoon_form)
                            { ChangeBtnImage(3); }//terug naar info contact
                            else
                            { ChangeBtnImage(4); }//terug naar info project
                            break;
                    }
                }
                no_edit();
                
                panel1.Focus();
                
            }
            else
            {
                if (start_parent is overview1)//??? of close parent...
                {
                    (start_parent as overview1).refresh_zoek();
                    
                }
                start_parent.herlaad();
                this.sluit();
                Close();
            }
            Cursor.Current = Cursors.Default;
        }
        private bool VerplichtCheck()
        {
            
            bool compleet = true;
            if (textBox1.TextLength < 1)
            {
                compleet = false;
                textBox1.BackColor = Color.Crimson;
                textBox1.ForeColor = Color.White;
            }
            else
            {
                textBox1.BackColor = Color.White;
                textBox1.ForeColor = Color.Black;
            }
            if (textBox3.TextLength < 1)
            {
                compleet = false;
                textBox3.BackColor = Color.Crimson;
                textBox3.ForeColor = Color.White;
            }
            else
            {
                textBox3.BackColor = Color.White;
                textBox3.ForeColor = Color.Black;
            }
            if (!telefoonnummer_user_control1.check_verplicht())
            {
                compleet = false;
                MessageBox.Show("telefoonnummer is verplicht en niet correct ingevoerd");
                telefoonnummer_user_control1.Focus();
            }
            else
            {
                
            }
            /*  if (textBox11.TextLength < 1)
             {
                 compleet = false;
                 textBox11.BackColor = Color.Crimson;
             }
             else
             {
                 textBox11.BackColor = Color.White;
             }
            if (textBox7.TextLength < 1)
             {
                 compleet = false;
                 textBox7.BackColor = Color.Crimson;
             }
             else
             {
                 textBox7.BackColor = Color.White;
             }
             if (textBox5.TextLength < 1)
             {
                 compleet = false;
                 textBox5.BackColor = Color.Crimson;
             }
             else
             {
                 textBox5.BackColor = Color.White;
             }
             if (textBox6.TextLength < 1)
             {
                 compleet = false;
                 textBox6.BackColor = Color.Crimson;
             }
             else
             {
                 textBox6.BackColor = Color.White;
             }
             if (textBox4.TextLength < 1)
             {
                 compleet = false;
                 textBox4.BackColor = Color.Crimson;
             }
             else
             {
                 textBox4.BackColor = Color.White;
             }
             if (textBox8.TextLength < 1)
             {
                 compleet = false;
                 textBox8.BackColor = Color.Crimson;
             }
             else
             {
                 textBox8.BackColor = Color.White;
             }
             if (textBox9.TextLength < 1)
             {
                 compleet = false;
                 textBox9.BackColor = Color.Crimson;
             }
             else
             {
                 textBox9.BackColor = Color.White;
             }*/
            return compleet;
        }
        private void btnopslaan_Click(object sender, EventArgs e)
        {
            bool fout_in_nummer = false;
            Cursor.Current = Cursors.WaitCursor;
            if (!VerplichtCheck())
            {
                MessageBox.Show("Niet alle verplichte velden zijn ingevuld.");
            }
            else
            {
                try
                {

                    //naam
                    if (textBox1.Text != "")
                    {
                        Rowbedrijf.naam = textBox1.Text.Trim();

                    }
                    else { Rowbedrijf.naam = null; }
                    //zoeknaam
                    if (textBox3.Text != "")
                    {
                        Rowbedrijf.zoeknaam = textBox3.Text.Trim();
                    }
                    else { Rowbedrijf.zoeknaam = null; }
                    //tel
                    KeyValuePair<string, bool> nummer_antw;
                    KeyValuePair<string, bool> nummer_antw2;
                    KeyValuePair<string, bool> nummer_antw3;
                    nummer_antw = telefoonnummer_user_control1.Get_Nummer();
                    if (nummer_antw.Value == true)
                    {
                        Rowbedrijf.telefoon_nr_1 = nummer_antw.Key;
                    }
                    else
                    {
                        fout_in_nummer = true;
                        goto NUMBERFAULT;
                    }
                    nummer_antw2 = telefoonnummer_user_control2.Get_Nummer();
                    if (nummer_antw2.Value == true)
                    {
                        Rowbedrijf.telefoon_nr_2 = nummer_antw2.Key;
                    }
                    else
                    {
                        fout_in_nummer = true;
                        goto NUMBERFAULT;
                    }
                    nummer_antw3 = telefoonnummer_user_control3.Get_Nummer();
                    if (nummer_antw3.Value == true)
                    {
                        Rowbedrijf.telefoon_nr_3 = nummer_antw3.Key;
                    }
                    else
                    {
                        fout_in_nummer = true;
                        goto NUMBERFAULT;
                    }
                    //Rowbedrijf.tel = telefoonnummer_user_control1.Get_Nummer();
                    //Rowbedrijf.mobiel = telefoonnummer_user_control2.Get_Nummer();
                    //Rowbedrijf.fax = telefoonnummer_user_control3.Get_Nummer();
                    Rowbedrijf.telefoon_nr_settings = telefoonnummer_user_control1.type.ToString() + "," + telefoonnummer_user_control2.type.ToString() + "," + telefoonnummer_user_control3.type.ToString();
                    bool tel_mob_done = false;
                    bool tel_vast_done = false;
                    bool tel_fax_done = false;
                    if (telefoonnummer_user_control1.type > -1 && nummer_antw.Value == true)
                    {
                        switch (telefoonnummer_user_control1.type)
                        {
                            case 0://vast
                                if (!tel_vast_done)
                                {
                                    Rowbedrijf.tel = nummer_antw.Key;
                                    if (nummer_antw.Key != null)
                                    {
                                        tel_vast_done = true;
                                    }
                                }
                                break;
                            case 1://mobiel
                                if (!tel_mob_done)
                                {
                                    Rowbedrijf.mobiel = nummer_antw.Key;
                                    if (nummer_antw.Key != null)
                                    {
                                        tel_mob_done = true;
                                    }
                                }
                                break;
                            case 2://fax
                                if (!tel_fax_done)
                                {
                                    Rowbedrijf.fax = nummer_antw.Key;
                                    if (nummer_antw.Key != null)
                                    {
                                        tel_fax_done = true;
                                    }
                                }
                                break;
                            case 3://voip
                                if (!tel_vast_done)
                                {
                                    Rowbedrijf.tel = nummer_antw.Key;
                                    if (nummer_antw.Key != null)
                                    {
                                        tel_vast_done = true;
                                    }
                                }
                                break;
                            case 4://skype
                                if (!tel_vast_done)
                                {
                                    Rowbedrijf.tel = nummer_antw.Key;
                                    if (nummer_antw.Key != null)
                                    {
                                        tel_vast_done = true;
                                    }
                                }
                                break;
                            case 5://bedrijfsnummer
                                if (!tel_vast_done)
                                {
                                    Rowbedrijf.tel = nummer_antw.Key;
                                    if (nummer_antw.Key != null)
                                    {
                                        tel_vast_done = true;
                                    }
                                }
                                break;
                        }
                    }
                    if (telefoonnummer_user_control2.type > -1 && nummer_antw2.Value == true)
                    {
                        switch (telefoonnummer_user_control2.type)
                        {
                            case 0://vast
                                if (!tel_vast_done)
                                {
                                    Rowbedrijf.tel = nummer_antw2.Key;
                                    if (nummer_antw2.Key != null)
                                    {
                                        tel_vast_done = true;
                                    }
                                }
                                break;
                            case 1://mobiel
                                if (!tel_mob_done)
                                {
                                    Rowbedrijf.mobiel = nummer_antw2.Key;
                                    if (nummer_antw2.Key != null)
                                    {
                                        tel_mob_done = true;
                                    }
                                }
                                break;
                            case 2://fax
                                if (!tel_fax_done)
                                {
                                    Rowbedrijf.fax = nummer_antw2.Key;
                                    if (nummer_antw2.Key != null)
                                    {
                                        tel_fax_done = true;
                                    }
                                }
                                break;
                            case 3://voip
                                if (!tel_vast_done)
                                {
                                    Rowbedrijf.tel = nummer_antw2.Key;
                                    if (nummer_antw2.Key != null)
                                    {
                                        tel_vast_done = true;
                                    }
                                }
                                break;
                            case 4://skype
                                if (!tel_vast_done)
                                {
                                    Rowbedrijf.tel = nummer_antw2.Key;
                                    if (nummer_antw2.Key != null)
                                    {
                                        tel_vast_done = true;
                                    }
                                }
                                break;
                            case 5://bedrijfsnummer
                                if (!tel_vast_done)
                                {
                                    Rowbedrijf.tel = nummer_antw2.Key;
                                    if (nummer_antw2.Key != null)
                                    {
                                        tel_vast_done = true;
                                    }
                                }
                                break;
                        }
                    }
                    if (telefoonnummer_user_control3.type > -1)
                    {
                        switch (telefoonnummer_user_control3.type)
                        {
                            case 0://vast
                                if (!tel_vast_done)
                                {
                                    Rowbedrijf.tel = nummer_antw3.Key;
                                    if (nummer_antw3.Key != null)
                                    {
                                        tel_vast_done = true;
                                    }
                                }
                                break;
                            case 1://mobiel
                                if (!tel_mob_done)
                                {
                                    Rowbedrijf.mobiel = nummer_antw3.Key;
                                    if (nummer_antw3.Key != null)
                                    {
                                        tel_mob_done = true;
                                    }
                                }
                                break;
                            case 2://fax
                                if (!tel_fax_done)
                                {
                                    Rowbedrijf.fax = nummer_antw3.Key;
                                    if (nummer_antw3.Key != null)
                                    {
                                        tel_fax_done = true;
                                    }
                                }
                                break;
                            case 3://voip
                                if (!tel_vast_done)
                                {
                                    Rowbedrijf.tel = nummer_antw3.Key;
                                    if (nummer_antw3.Key != null)
                                    {
                                        tel_vast_done = true;
                                    }
                                }
                                break;
                            case 4://skype
                                if (!tel_vast_done)
                                {
                                    Rowbedrijf.tel = nummer_antw3.Key;
                                    if (nummer_antw3.Key != null)
                                    {
                                        tel_vast_done = true;
                                    }
                                }
                                break;
                            case 5://bedrijfsnummer
                                if (!tel_vast_done)
                                {
                                    Rowbedrijf.tel = nummer_antw3.Key;
                                    if (nummer_antw3.Key != null)
                                    {
                                        tel_vast_done = true;
                                    }
                                }
                                break;
                        }
                    }
                    if (!tel_vast_done)
                    {
                        Rowbedrijf.SettelNull();
                    }
                    if (!tel_mob_done)
                    {
                        Rowbedrijf.SetmobielNull();
                    }
                    if (!tel_fax_done)
                    {
                        Rowbedrijf.SetfaxNull();
                    }
                    
                    //////
                    if (textBox13.Text != "")
                    {
                        Rowbedrijf.email = textBox13.Text;
                    }
                    else { Rowbedrijf.email = null; }
                    //website
                    if (textBox14.Text != "")
                    {
                        Rowbedrijf.website = textBox14.Text;
                    }
                    else { Rowbedrijf.website = null; }
                    //actief ja/nee

                    if (!radioButton1.Checked)
                    {
                        Rowbedrijf.NIETactief = true;

                    }
                    else
                    {
                        Rowbedrijf.NIETactief = false;
                    }

                    //bezoekadres
                    if (!Rowbedrijf.Isadres_id_bezoekNull() && Rowbedrijf.adres_id_bezoek > 0)
                    {
                        
                        aTA.FillBy(aDT, Rowbedrijf.adres_id_bezoek);
                        aRow = aDT.Rows[0] as AdresDataSet.adressenRow;
                    }
                    else
                    {
                        aRow = aDT.NewadressenRow();
                    }
                    bool _valid = true;
                        if (adres_user_control1.Land != String.Empty)
                        {
                            aRow.land = adres_user_control1.Land;
                        }
                        else
                        {
                            aRow.land = String.Empty;
                            _valid = false;
                        }
                        if (adres_user_control1.Plaats != String.Empty)
                        {
                            aRow.plaats = adres_user_control1.Plaats;
                        }
                        else
                        {
                            aRow.plaats = String.Empty;
                            _valid = false;
                        }
                        if (adres_user_control1.Straat != String.Empty)
                        {
                            aRow.straat_1 = adres_user_control1.Straat;
                        }
                        else
                        {
                            aRow.straat_1 = String.Empty;
                            _valid = false;
                        }
                        if (adres_user_control1.Postcode_cijfers > 999)
                        {
                            aRow.postcode_cijfers = adres_user_control1.Postcode_cijfers;
                        }
                        else
                        {
                            //aRow.postcode_cijfers = 0;
                            _valid = false;
                        }
                        if (adres_user_control1.Postcode_letters != String.Empty)
                        {
                            aRow.postcode_letters = adres_user_control1.Postcode_letters;
                        }
                        else
                        {
                            aRow.postcode_letters = String.Empty;
                            _valid = false;
                        }
                        if (adres_user_control1.Huisnummer != null)
                        {
                            aRow.huis_postbus_nummer = adres_user_control1.Huisnummer;
                        }
                        else
                        {
                            aRow.huis_postbus_nummer = String.Empty;
                            _valid = false;
                        }
                        if (adres_user_control1.Huisnummer_toevoeging != String.Empty)
                        {
                            aRow.huisnummer_toevoeging = adres_user_control1.Huisnummer_toevoeging;
                        }
                        else
                        {
                            aRow.huisnummer_toevoeging = String.Empty;
                        }
                        if (adres_user_control1.Straat2 != String.Empty)
                        {
                            aRow.straat_2 = adres_user_control1.Straat2;
                        }
                        else
                        {
                            aRow.straat_2 = String.Empty;
                        }
                        if (!Rowbedrijf.Isadres_id_bezoekNull() && Rowbedrijf.adres_id_bezoek > 0)
                        {
                            aTA.adres_set(aRow.adres_id,
                                          aRow.land,
                                          aRow.plaats,
                                          aRow.straat_1,
                                          aRow.straat_2,
                                          aRow.postcode_cijfers,
                                          aRow.postcode_letters,
                                          aRow.huis_postbus_nummer,
                                          aRow.huisnummer_toevoeging,
                                          _valid,
                                          adres_user_control1.ViaPostcode,
                                          adres_user_control1._City_key);
                        }
                        else
                        {
                            int n_id = (int)aTA.adres_max_id() + 1;
                            aTA.adres_new(n_id,
                                          aRow.land,
                                          aRow.plaats,
                                          aRow.straat_1,
                                          aRow.straat_2,
                                          aRow.postcode_cijfers,
                                          aRow.postcode_letters,
                                          aRow.huis_postbus_nummer,
                                          aRow.huisnummer_toevoeging,
                                          _valid,
                                          adres_user_control2.ViaPostcode,
                                          adres_user_control2._City_key);
                            Rowbedrijf.adres_id_bezoek = n_id;
                        }
                   
                    //postadres
                    if (!Rowbedrijf.Isadres_id_postNull() && Rowbedrijf.adres_id_post > 0 )
                    {
                        aTA.FillBy(aDT, Rowbedrijf.adres_id_post);
						if (aDT.Rows.Count > 0)
						{
							aRow = aDT.Rows[0] as AdresDataSet.adressenRow;
						}
						else { aRow = aDT.NewadressenRow(); }
                    }
                    else
                    {
                        aRow = aDT.NewadressenRow();
                    }
                            _valid = true;
                        if (adres_user_control2.Land != String.Empty)
                        {
                            aRow.land = adres_user_control2.Land;
                        }
                        else
                        {
                            _valid = false;
                            aRow.land = String.Empty;
                        }
                        if (adres_user_control2.Plaats != String.Empty)
                        {
                            aRow.plaats = adres_user_control2.Plaats;
                        }
                        else
                        {
                            aRow.plaats = String.Empty;
                            _valid = false;
                        }
                        if (adres_user_control2.Straat != String.Empty)
                        {
                            aRow.straat_1 = adres_user_control2.Straat;
                        }
                        else
                        {
                            _valid = false;
                            aRow.straat_1 = String.Empty;
                        }
                        if (adres_user_control2.Postcode_cijfers > 999)
                        {
                            aRow.postcode_cijfers = adres_user_control2.Postcode_cijfers;
                        }
                        else
                        {
                            //aRow.postcode_cijfers = 0;
                            _valid = false;
                        }
                        if (adres_user_control2.Postcode_letters != String.Empty)
                        {
                            aRow.postcode_letters = adres_user_control2.Postcode_letters;
                        }
                        else
                        {
                            aRow.postcode_letters = String.Empty;
                            _valid = false;
                        }
                        if (adres_user_control2.Huisnummer != null)
                        {
                            aRow.huis_postbus_nummer = adres_user_control2.Huisnummer;
                        }
                        else
                        {
                            aRow.huis_postbus_nummer = String.Empty;
                            _valid = false;
                        }
                        if (adres_user_control2.Huisnummer_toevoeging != String.Empty)
                        {
                            aRow.huisnummer_toevoeging = adres_user_control2.Huisnummer_toevoeging;
                        }
                        else
                        {
                            aRow.huisnummer_toevoeging = String.Empty;

                        }
                        if (adres_user_control2.Straat2 != String.Empty)
                        {
                            aRow.straat_2 = adres_user_control2.Straat2;
                        }
                        else
                        {
                            aRow.straat_2 = String.Empty;
                        }
                        if (!Rowbedrijf.Isadres_id_postNull() && Rowbedrijf.adres_id_post > 0)
                        {
                            if (checkBox1.Checked == false)
                            {
                                if (Rowbedrijf.adres_id_bezoek == Rowbedrijf.adres_id_post)
                                {
                                    int m_id = (int)aTA.adres_max_id() + 1;
                                    aTA.adres_new(m_id,
                                                  aRow.land,
                                                  aRow.plaats,
                                                  aRow.straat_1,
                                                  aRow.straat_2,
                                                  aRow.postcode_cijfers,
                                                  aRow.postcode_letters,
                                                  aRow.huis_postbus_nummer,
                                                  aRow.huisnummer_toevoeging,
                                                  _valid,
                                                  adres_user_control2.ViaPostcode,
                                                  adres_user_control2._City_key);
                                    Rowbedrijf.adres_id_post = m_id;
                                }
                                else
                                {
                                    aTA.adres_set(Rowbedrijf.adres_id_post,
                                                  aRow.land,
                                                  aRow.plaats,
                                                  aRow.straat_1,
                                                  aRow.straat_2,
                                                  aRow.postcode_cijfers,
                                                  aRow.postcode_letters,
                                                  aRow.huis_postbus_nummer,
                                                  aRow.huisnummer_toevoeging,
                                                  _valid,
                                                  adres_user_control2.ViaPostcode,
                                                  adres_user_control2._City_key);
                                }
                            }
                            else
                            {
                                if (Rowbedrijf.adres_id_bezoek != Rowbedrijf.adres_id_post)
                                {
                                    aTA.adres_del(Rowbedrijf.adres_id_post);
                                }
                                Rowbedrijf.adres_id_post = Rowbedrijf.adres_id_bezoek;
                                // aTA.adres_set(aRow.adres_id, aRow.land, aRow.plaats, aRow.straat_1, aRow.straat_2, aRow.postcode_cijfers, aRow.postcode_letters, aRow.huis_postbus_nummer, aRow.huisnummer_toevoeging, _valid);
                            }
                        }
                        else if (checkBox1.Checked == false)
                        {
                            int n_id = (int)aTA.adres_max_id() + 1;
                            aTA.adres_new(n_id,
                                          aRow.land,
                                          aRow.plaats,
                                          aRow.straat_1,
                                          aRow.straat_2,
                                          aRow.postcode_cijfers,
                                          aRow.postcode_letters,
                                          aRow.huis_postbus_nummer,
                                          aRow.huisnummer_toevoeging,
                                          _valid,
                                          adres_user_control2.ViaPostcode,
                                          adres_user_control2._City_key);
                            Rowbedrijf.adres_id_post = n_id;
                        }
                        else
                        {
                            if (Rowbedrijf.adres_id_bezoek != Rowbedrijf.adres_id_post)
                            {
                                aTA.adres_del(Rowbedrijf.adres_id_post);
                            }
                            Rowbedrijf.adres_id_post = Rowbedrijf.adres_id_bezoek;
                        }
                   
                  //administratie

                    //debiteurnr
                    if (textBox25.Text != "")
                    {
                        Rowbedrijf.debiteurNRzeebregts = textBox25.Text;
                    }
                    else { Rowbedrijf.debiteurNRzeebregts = null; }
                    //crediteurnr
                    if (textBox26.Text != "")
                    {
                        Rowbedrijf.crediteurNRzeebregts = textBox26.Text;
                    }
                    else { Rowbedrijf.crediteurNRzeebregts = null; }
                    //kostenplaats
                    if (textBox27.Text != "")
                    {
                        Rowbedrijf.kostenplaats = textBox27.Text;
                    }
                    else { Rowbedrijf.kostenplaats = null; }
                    //bank
                    if (textBox15.Text != "")
                    {
                        Rowbedrijf.bank = textBox15.Text;
                    }
                    else { Rowbedrijf.bank = null; }
                    //Grek
                    if (textBox16.Text != "")
                    {
                        Rowbedrijf.Grek = textBox16.Text;
                    }
                    else { Rowbedrijf.Grek = null; }
                    //IBAN
                    if (textBox23.Text != "")
                    {
                        Rowbedrijf.IBAN = textBox23.Text;
                    }
                    else { Rowbedrijf.IBAN = null; }
                    //SWIFT
                    if (textBox24.Text != "")
                    {
                        Rowbedrijf.SWIFT = textBox24.Text;
                    }
                    else { Rowbedrijf.SWIFT = null; }
                    //omzetbelastingnr
                    if (textBox19.Text != "")
                    {
                        Rowbedrijf.omzetbelastingnr = textBox19.Text;
                    }
                    else { Rowbedrijf.omzetbelastingnr = null; }
                    //loonbelastingnr
                    if (textBox20.Text != "")
                    {
                        Rowbedrijf.loonbelastingnr = textBox20.Text;
                    }
                    else { Rowbedrijf.loonbelastingnr = null; }
                    //aansluitnrbedrijver
                    if (textBox21.Text != "")
                    {
                        Rowbedrijf.bedrijfverenigingnr = textBox21.Text;
                    }
                    else { Rowbedrijf.bedrijfverenigingnr = null; }
                    //kvk
                    if (textBox17.Text != "")
                    {
                        Rowbedrijf.kvk = textBox17.Text;
                    }
                    else { Rowbedrijf.kvk = null; }
                    //kvkuitgifte
                    if (dates[0] && dateTimePicker2.Value != DateTimePicker.MinimumDateTime)
                    {
                        Rowbedrijf.kvkUITGIFTE = dateTimePicker2.Value;
                        dates[0] = false;
                    }
                    else if (dateTimePicker2.Value == DateTimePicker.MinimumDateTime)
                    {
                        Rowbedrijf.SetkvkUITGIFTENull();
                    }
                    //kvkscan
                    if (textBox2.Text != "")
                    {
                        Rowbedrijf.kvkSCAN = textBox2.Text;

                    }
                    else { Rowbedrijf.SetkvkSCANNull(); }

                    verwijder_panel1.Save_function(textBox29.Text);
                    
                }
                catch (Exception e2)
                {
                    MessageBox.Show("opslaan mislukt, controleer alle waardes");
                    String log_line = "crash program @ " + DateTime.Now.ToString() + "error: " + e2;
                    System.IO.StreamWriter file = new System.IO.StreamWriter(Global.log_file_path, true);
                    file.WriteLine(log_line);
                    file.Close();
                    
                    return;
                }
                try
                {
                    adapter.Update(Rowbedrijf);
                    adres_user_control1.Wijzigstand = false;
                    adres_user_control2.Wijzigstand = false;
                    telefoonnummer_user_control1.Wijzigstand(false);
                    telefoonnummer_user_control2.Wijzigstand(false);
                    telefoonnummer_user_control3.Wijzigstand(false);
                    if (adres_user_control1.Straat != string.Empty && adres_user_control2.Straat != string.Empty)
                    {
                        string straat_bezoek_oud = adres_user_control1.Straat + " " + adres_user_control1.Huisnummer.ToString();
                        if (adres_user_control1.Huisnummer_toevoeging != string.Empty)
                        {
                            straat_bezoek_oud += adres_user_control1.Huisnummer_toevoeging;
                        }
                        string straat_bezoek_oud2 = adres_user_control2.Straat + " " + adres_user_control2.Huisnummer.ToString();
                        if (adres_user_control2.Huisnummer_toevoeging != string.Empty)
                        {
                            straat_bezoek_oud2 += adres_user_control2.Huisnummer_toevoeging;
                        }
                        adapter.Update_oud_adres(straat_bezoek_oud, adres_user_control1.Postcode_cijfers.ToString() + adres_user_control1.Postcode_letters, adres_user_control1.Plaats, straat_bezoek_oud2, adres_user_control2.Postcode_cijfers.ToString() + adres_user_control2.Postcode_letters, adres_user_control2.Plaats, Rowbedrijf.bedrijf_ID);
                    }
                }
                catch (Exception e5)
                {
                    MessageBox.Show("opslaan mislukt, controleer netwerkverbinding");
                    String log_line = "crash program @ " + DateTime.Now.ToString() + "error in bedrijfform: " + e5;
                    System.IO.StreamWriter file = new System.IO.StreamWriter(Global.log_file_path, true);
                    file.WriteLine(log_line);
                    file.Close();
                    
                    return;
                }

               
                if (editing == true)
                {
                    bool close_this = false;
                    editing = false; this.wijzigstand = false;
                    no_edit();
                    
                    switch (enum_nr)
                    {
                        case 0://normaal
                            handelingen_logger.log_handeling(int.Parse(id), 2, 26);
                            if (van_zoek)
                            { ChangeBtnImage(1);
                            }//terug naar zoek                    
                            else
                            { ChangeBtnImage(2); }//alle bedrijven
                            break;
                        case 1://nieuw clean
                            if (van_zoek)
                            { ChangeBtnImage(1); }//terug naar zoek
                            else
                            { ChangeBtnImage(2); }//alle bedrijven
                            enum_nr = 0;
                            break;
                        case 2://nieuw owner
                            close_this = true;
                            if (close_parent is newrecord)
                            {
                                Global.return_id = id;
                                (close_parent as newrecord).fillid();
                                this.Close();
                            }
                            else if (close_parent is persoon_form)
                            {
                                Global.return_id = id;
                                (close_parent as persoon_form).fill_id(close_this);
                            }
                            else if (close_parent is project_form)
                            {
                                Global.return_id = id;
                                (close_parent as project_form).fill_id(close_this);
                            }
                            break;
                        case 3://normaal subroute
                            if (close_parent is persoon_form)
                            {
                                handelingen_logger.log_handeling(int.Parse(id), 3, 12);
                                ChangeBtnImage(3);//terug naar info contact
                                Global.return_id = id;
                                (close_parent as persoon_form).fill_id(close_this);
                            }
                            else
                            {
                                handelingen_logger.log_handeling(int.Parse(id), 1, 12);
                                ChangeBtnImage(4);//terug naar info project
                                Global.return_id = id;
                                (close_parent as project_form).fill_id(close_this);
                            }
                            break;
                    }

                    antw = Is_ingebruik(int.Parse(id), 2);
                    if (antw.Length > 1 && antw == Global.username)
                    {
                        if (Verwijder_ingebruik(int.Parse(id), 2))
                        {
                        }
                        else
                        {
                            MessageBox.Show("fout bij vrijgeven opslaan " + antw);

                        }
                    }
                    if (close_this)
                    {
                        this.Close();
                    }

                    btnterug.Show();
                    btn_verwijder.Hide();
                }
                
                btnterug.Refresh(); this.bedrijf_form_Resize(this,e);
                panel1.Focus();
            }
        NUMBERFAULT: if (fout_in_nummer)
            {
                
                MessageBox.Show("Een telefoonnummer is fout ingevoerd. \nGooi leeg met het kruisje of pas het nummer aan.");
            }

        Cursor.Current = Cursors.Default;
        }
        private void btnwijzig_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            antw = Is_ingebruik(int.Parse(id), 2);
            bezetnaam = antw;
            if (antw.Length > 1)
            {
                MessageBox.Show("Het wijzigen van het bedrijf " + vrijgeefstring + " is momenteel niet mogelijk.\n Dit formulier is reeds in gebruik door " + antw + ".\n U ontvangt een melding zodra dit formulier wordt vrijgegeven.");
                btnwijzig.Enabled = false;
                INUSEBackgroundWorker.RunWorkerAsync();
            }
            else
            {
                if (Neem_ingebruik(int.Parse(id), 2, Global.username))
                {
                    this.wijzigstand = true;
                    using (Panel tmp_P = new Panel())
                    {
                        tmp_P.Location = panel1.Location;
                        tmp_P.Size = panel1.Size;
                        tmp_P.BackColor = System.Drawing.SystemColors.ControlLightLight;
                        tmp_P.BorderStyle = BorderStyle.Fixed3D;
                        this.Controls.Add(tmp_P);
                        tmp_P.Show();
                        tmp_P.BringToFront();
                        adres_user_control1.Wijzigstand = true;
                        if (checkBox1.Checked == false)
                        {
                            adres_user_control2.Wijzigstand = true;
                        }
                        telefoonnummer_user_control1.Wijzigstand(true);
                        telefoonnummer_user_control2.Wijzigstand(true);
                        telefoonnummer_user_control3.Wijzigstand(true);
                        NavigateRecord();
                       
                       
                        if (!checkBox1.Checked)
                        {
                            adres_user_control2.Wijzigstand = true;
                        }
                        if (enum_nr != 2 && enum_nr != 3)
                        {
                            btn_verwijder.Show();
                        }
                        editing = true;
                        ChangeBtnImage(5);
                    }
                    do_edit();
                }
                else
                {

                    MessageBox.Show("reservering failed");
                }
            }
            Cursor.Current = Cursors.Default;
            panel1.Focus();
        }
        private void bedrijf_form_FormClosing(object sender, FormClosingEventArgs e)
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
                program_closes = false;
                FormManager.Sluit_forms(this);
                e.Cancel = true;
            }
            else
            {
                //laat sluiten
                antw = Is_ingebruik(int.Parse(id), 2);
                if (antw.Length > 1 && antw == Global.username)
                {
                    if (!Verwijder_ingebruik(int.Parse(id), 2))
                    {
                        MessageBox.Show("fout bij vrijgeven closing " + antw);
                    }

                }
            }
        }
        private void ChangeBtnImage(int i)
        {
            switch (i)
            {
                case 1: //terug naar zoek
                        btnterug.Image = null;
                        btnterug.Image = ((System.Drawing.Image)(Properties.Resources.ZoekBedrijf2));
                    break;
                case 2://terug naar allebedrijven
                        btnterug.Image = null;
                        btnterug.Image = ((System.Drawing.Image)(Properties.Resources.AlleBedrijven2));
                    break;
                case 3://terug naar info contact
                        btnterug.Image = null;
                        btnterug.Image = ((System.Drawing.Image)(Properties.Resources.InfoContact2));
                    break;
                case 4://terug naar info project
                        btnterug.Image = null;
                        btnterug.Image = ((System.Drawing.Image)(Properties.Resources.InfoProject2));
                    break;
                case 5://annuleer wijzig
                        btnterug.Image = null;
                        btnterug.Image = ((System.Drawing.Image)(Properties.Resources.Annuleer2));
                    break;
                case 6://terug
                    btnterug.Image = null;
                    btnterug.Image = ((System.Drawing.Image)(Properties.Resources.Terug));
                    break;
            }
        }
     
        private void bedrijf_form_Resize(object sender, EventArgs e)
        {
           
            Point pt1 = new Point(panel2.Width / 2 - lbl_top.Width / 2, 5);
            lbl_top.Location = pt1;
            Point pt2 = new Point(panel2.Width / 2 - lbl_mid.Width / 2,28);
            lbl_mid.Location = pt2;
            Point pt3 = new Point(panel2.Width / 2 - lbl_bot.Width / 2, panel2.Height - 18);
            lbl_bot.Location = pt3;

            Point PMid = new Point(panel4.Width / 2 - (p_xfunctie.Width / 2), p_xfunctie.Location.Y);
            p_xfunctie.Location = PMid;
            PMid.Y = p_verw_verv.Location.Y;
            p_verw_verv.Location = PMid;

            elementHost7.Location = new Point(panel5.Width / 2 - elementHost7.Width / 2, elementHost7.Location.Y);
            
            button2.Location = new Point(panel3.Width / 2 - button2.Width/2, button2.Location.Y);
            button1.Location = new Point((panel3.Width / 2 - button2.Width/2)-button1.Width, button1.Location.Y);
            button3.Location = new Point((panel3.Width / 2 + button3.Width / 2), button3.Location.Y);


            Point PLinks = new Point((panel2.Width / 2 - p_contact_gegevens.Width) - 5, p_contact_gegevens.Location.Y);
            Point PRechts = new Point((panel2.Width / 2) + 5, p_administratie.Location.Y);
            p_contact_gegevens.Location = PLinks;
            p_administratie.Location = PRechts;
            PLinks.Y = p_bezoekadres.Location.Y;
            p_bezoekadres.Location = PLinks;
            PLinks.Y = p_postadres.Location.Y;
            p_postadres.Location = PLinks;
            PRechts.Y = p_telnrs.Location.Y;
            p_telnrs.Location = PRechts;

            label66.Location = new Point(panel5.Width / 2 - label66.Width / 2, label66.Location.Y);
        }
        bool from_tb = false;
        private void Clear_llb()
        {
            label2.Text = "-";
            label23.Text = "-";
            label25.Text = "-";
            linkLabel2.Text = "-";
            label37.Text = "-";
            label38.Text = "-";
            label39.Text = "-";
            label41.Text = "-";
            label42.Text = "-";
            label43.Text = "-";
            label44.Text = "-";
            label46.Text = "-";
            label49.Text = "-";
            label51.Text = "-";
            label53.Text = "-";
            label55.Text = "-";
            label57.Text = "-";
            label60.Text = "-";
            


        }
        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {

            if (!from_tb)
            {
                
                if (dateTimePicker2.Value != DateTimePicker.MinimumDateTime)
                {
                    dates[0] = true; changed = true;
                    label44.Text = dateTimePicker2.Value.ToLongDateString();
                    textBox18.Text = dateTimePicker2.Value.Day.ToString();
                    textBox22.Text = dateTimePicker2.Value.Month.ToString();
                    textBox28.Text = dateTimePicker2.Value.Year.ToString();
                }
            }
            else
            {
                from_tb = false;
            }
            
        }
        private void Clear_text(Control parent)
        {
            Clear_llb();
            
            foreach (Control c in parent.Controls)
            {
                if (c is TextBox)
                {
                    (c as TextBox).Clear();
                }
               
                if (c.Controls.Count > 0) Clear_text(c);
            }
            
        }
        private void Organize_panels()
        {
            /*
          * links
          * #1 p_contact_gegevens
          * #2 p_telnrs
          * #3 p_bezoekadres
          * rechts 
          * #1 p_administratie
          * #2p_postadres
          */
            if (this.Visible)
            {
                using (Panel tmp_P = new Panel())
                {
                    tmp_P.Location = panel1.Location;
                    tmp_P.Size = panel1.Size;
                    tmp_P.BackColor = System.Drawing.SystemColors.ControlLightLight;
                    tmp_P.BorderStyle = BorderStyle.Fixed3D;
                    this.Controls.Add(tmp_P);
                    tmp_P.Show();
                    tmp_P.BringToFront();

                    Point pref_scroll_poss = panel1.AutoScrollPosition;
                    panel1.AutoScrollPosition = new Point(0, 0);
                    int p_x_links = panel1.Width / 2 - 436;
                    int p_x_rechts = panel1.Width / 2 + 4;
                    //links
                    p_contact_gegevens.Location = new Point(p_x_links, 10);
                    p_contact_gegevens.Size = bereken_panel(p_contact_gegevens);
                    p_bezoekadres.Location = new Point(p_x_links, p_contact_gegevens.Size.Height + p_contact_gegevens.Location.Y + 8);
                    p_bezoekadres.Size = bereken_panel(p_bezoekadres);
                    p_postadres.Location = new Point(p_x_links, p_bezoekadres.Size.Height + p_bezoekadres.Location.Y + 8);
                    p_postadres.Size = bereken_panel(p_postadres);
                    //rechts
                    p_administratie.Location = new Point(p_x_rechts, 10);
                    p_administratie.Size = bereken_panel(p_administratie);
                    p_telnrs.Location = new Point(p_x_rechts, p_administratie.Size.Height + p_administratie.Location.Y + 8);
                    p_telnrs.Size = bereken_panel(p_telnrs);
                    pref_scroll_poss.X *= -1;
                    pref_scroll_poss.Y *= -1;
                    panel1.AutoScrollPosition = pref_scroll_poss;
                }
            }
        }
        private Size bereken_panel(Panel P)
        {
            Size Sz = new Size();
            int tmp_H = 42;
            int n_showed = 0;
            foreach (Control SC in P.Controls)
            {
                if (SC is SplitContainer || SC is System.Windows.Forms.Integration.ElementHost)
                {
                    if (SC.Visible)
                    {
                        n_showed++;
                        tmp_H += SC.Height;
                        SC.Location = new Point(10, 25 * n_showed + 5);
                    }
                }
                else if (SC is Button && P == p_telnrs)
                {
                    if (SC.Visible && n_showed < 3)
                    {
                        n_showed++;
                        tmp_H += SC.Height;
                        SC.Location = new Point(347, 25 * n_showed + 5);
                    }
                }
                
            }
           // tmp_H = 42 + n_showed * 25;
            if (tmp_H < 67)
            {
                tmp_H = 67;
            }
            Sz.Height = tmp_H;
            Sz.Width = 432;
            return Sz;
        }
        public void Hide_tel_ctrl(telefoonnummer_user_control tuc)
        {
            if (tuc == telefoonnummer_user_control1)
            {
                elementHost3.Hide();
            }
            else if (tuc == telefoonnummer_user_control2)
            {
                elementHost4.Hide();
            }
            else if (tuc == telefoonnummer_user_control3)
            {
                elementHost5.Hide();
            }
            button6.Show();
            Organize_panels();
        }
        private void splitContainer11_Panel2_Leave(object sender, EventArgs e)
        {
            if (textBox28.Text == "" && textBox22.Text == "" && textBox18.Text == "")
            {
                dates[0] = false;
                dateTimePicker2.Value = DateTimePicker.MinimumDateTime;
            }
            else
            {
                if (textBox18.Text == "")
                {
                    MessageBox.Show("Geef ook een waarde voor dag op.");
                    textBox18.Focus();
                    return;
                }
                else if (textBox22.Text == "")
                {
                    MessageBox.Show("Geef ook een waarde voor maand op.");
                    textBox22.Focus();
                    return;
                }
                else if (textBox28.Text == "")
                {
                    MessageBox.Show("Geef ook een waarde voor jaar op.");
                    textBox28.Focus();
                    return;
                }

                string date = "";
                if (textBox28.TextLength == 4)
                { date = textBox28.Text + "/" + textBox22.Text + "/" + textBox18.Text; }
                else if (textBox28.TextLength == 3)
                { date = "2" + textBox28.Text + "/" + textBox22.Text + "/" + textBox18.Text; }
                else if (textBox28.TextLength == 2)
                { date = "20" + textBox28.Text + "/" + textBox22.Text + "/" + textBox18.Text; }
                else if (textBox28.TextLength == 1)
                { date = "200" + textBox28.Text + "/" + textBox22.Text + "/" + textBox18.Text; }
                DateTime testdt;
                if (DateTime.TryParse(date, out testdt))
                {
                    dateTimePicker2.Value = testdt;
                    textBox18.Text = dateTimePicker2.Value.Day.ToString();
                    textBox22.Text = dateTimePicker2.Value.Month.ToString();
                    textBox28.Text = dateTimePicker2.Value.Year.ToString();

                }
                else
                {
                    MessageBox.Show("ongeldige datum");
                    textBox18.Focus();
                }
            }
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

        private void label18_Click(object sender, EventArgs e)
        {
            Label tmp = (Label)sender;
            foreach (Control c in tmp.Parent.Controls)
            {
                if (c is TextBox)
                {
                    TextBox tb = (TextBox)c;
                    tb.Clear();
                }
                else if (c is DateTimePicker)
                {
                    from_tb = true;
                    (c as DateTimePicker).Value = DateTimePicker.MinimumDateTime;
                }
            }
        }
        private void Get_Set_plaats()
        {
            telefoonnummer_user_control1.Api_get_areacode(adres_user_control1.Postcode_cijfers + adres_user_control1.Postcode_letters);
            telefoonnummer_user_control2.Api_get_areacode(adres_user_control1.Postcode_cijfers + adres_user_control1.Postcode_letters);
            telefoonnummer_user_control3.Api_get_areacode(adres_user_control1.Postcode_cijfers + adres_user_control1.Postcode_letters);
        }
        private void dateTimePicker2_MouseEnter(object sender, EventArgs e)
        {
            
            DateTimePicker dtmp = (DateTimePicker)sender;
            dtmp.Refresh();
            if (dtmp.Value == DateTimePicker.MinimumDateTime)
            {
                from_tb = true;
                dtmp.Value = DateTime.Now;
                drop = true;

            }
        }
        bool drop = false;
        bool changed = false;
        private void dateTimePicker2_CloseUp(object sender, EventArgs e)
        {
            DateTimePicker dtmp = (DateTimePicker)sender;
            if (drop && changed)
            {
                drop = false;
                changed = false;
                dtmp.Value = DateTimePicker.MinimumDateTime;
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

        private void bedrijf_form_Leave(object sender, EventArgs e)
        {
            Panel tmpp = (Panel)sender;
            foreach (Control c in tmpp.Controls)
            {
                if (c is TextBox)
                {
                    if ((c as TextBox).TextLength < 1)
                    {
                        MessageBox.Show("Vul waarde in, dit is een verplicht veld");

                        c.Focus();
                    }
                }
            }
            
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)//niet naar wel
            {
                p_postadres.Hide();
                label4.Text = "Bezoek- en postadres";
                    /*adres_user_control2.ViaPostcode = adres_user_control1.ViaPostcode;
                    adres_user_control2.Land = adres_user_control1.Land;
                    adres_user_control2.Plaats = adres_user_control1.Plaats;
                    adres_user_control2.Straat = adres_user_control1.Straat;
                    adres_user_control2.Postcode_cijfers = adres_user_control1.Postcode_cijfers;
                    adres_user_control2.Postcode_letters = adres_user_control1.Postcode_letters;
                    adres_user_control2.Huisnummer = adres_user_control1.Huisnummer;
                    adres_user_control2.Huisnummer_toevoeging = adres_user_control1.Huisnummer_toevoeging;
                    adres_user_control2.Straat2 = adres_user_control1.Straat2;
                    adres_user_control2.Wijzigstand = false;
                    *///p_postadres.Height = 117;
               
            }
            else //wel naar niet
            {
                p_postadres.Show();
                label4.Text = "Bezoekadres";
                if (wijzigstand)
                {
                   // p_postadres.Height = 192;
                    adres_user_control2.Wijzigstand = true;
                }
            }
            Organize_panels();
        }

      
        string bezetnaam = "";
        private void INUSEBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string result = Is_ingebruik(int.Parse(id), 2);
            while (result.Length > 1)
            {
                result = Is_ingebruik(int.Parse(id), 2);
                Thread.Sleep(5000);

            }
        }

        private void INUSEBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Het bedrijf " + vrijgeefstring + " is zojuist vrijgegeven door " + bezetnaam + ".\n U kunt dit formulier nu wijzigen.");
            btnwijzig.Enabled = true;
        }
        private void Lock_screen()
        {
            btnopslaan.Enabled = false;
            btnwijzig.Enabled = false;
            btnterug.Enabled = false;
            button1.Hide();
            button2.Hide();
            panel4.BringToFront();
            if (Global.UserLevel < 3)
            {
              
                multiColumnComboBox1.Hide();
                
            }
            else
            {
                p_verw_verv.Hide();
            }
            panel4.Focus();
        }
        private void Free_screen()
        {
            btnopslaan.Enabled = true;
            btnwijzig.Enabled = true;
            btnterug.Enabled = true;
            button1.Show();
            button2.Show();
            panel1.BringToFront();
            p_verw_verv.Hide();
            vervang_lbl.Text = "Dit bedrijf verwijderen en vervangen?";
            button5.Text = "Ja";
            //button6.Text = "Nee";
            panel1.Focus();
        }
        private void btn_verwijder_Click(object sender, EventArgs e)
        {
            Lock_screen();
            btn_verwijder.Hide();
            btn_verwijder_annulleer.Show();
            btn_verwijder_bevestig.Show();
        }

        private void btn_verwijder_annulleer_Click(object sender, EventArgs e)
        {
            uitvoeren = false;
            vervangt = false;
            vervang_lbl.Text = "Dit bedrijf verwijderen en vervangen?";
            multiColumnComboBox1.Enabled = true;
            multiColumnComboBox1.Hide();
            multiColumnComboBox1.SelectedIndex = -1;
            btn_verwijder_bevestig.Enabled = true;
            p_verw_verv.Hide();
            Free_screen();
            btn_verwijder.Show();
            btn_verwijder_annulleer.Hide();
            btn_verwijder_bevestig.Hide();
        }

        private void btn_verwijder_bevestig_Click(object sender, EventArgs e)
        {
            int bid = int.Parse(id);
            int result = 0;
            string column = "";
            string table = "";
           // bedrijfformdatasetTableAdapters.QueriesTableAdapter qtadapt = new bedrijfformdatasetTableAdapters.QueriesTableAdapter();
            bedrijfformdatasetTableAdapters.bedrijf_nr_locatiesTableAdapter locaties_adapt = new bedrijfformdatasetTableAdapters.bedrijf_nr_locatiesTableAdapter();
            locaties_adapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
            bedrijfformdataset.bedrijf_nr_locatiesDataTable locaties_table = locaties_adapt.GetData();
            foreach (bedrijfformdataset.bedrijf_nr_locatiesRow row in locaties_table.Rows)
            {
                column = row.column.ToString();
                table = row.tabel.ToString();
                result += (int)adapter.Check_del_bdr(bid, column, table);
            }

            if (result > 0)
            {
                if (Global.UserLevel < 3)
                {
                    var DR = MessageBox.Show("Dit bedrijf komt " + result + " keer voor in andere gegevens.\nHierdoor kan dit bedrijf niet worden verwijderd.\nKlik op \"JA\" om dit bedrijf te vervangen door een ander bedrijf.\nKlik op \"NEE\" om terug te gaan naar de lees-stand.", "Verwijderen en vervangen?", MessageBoxButtons.YesNo);
                    if (DR == DialogResult.Yes)
                    {
                        p_verw_verv.Show();
                        btn_verwijder_bevestig.Enabled = false;
                        multiColumnComboBox1.SelectedIndex = -1;
                        multiColumnComboBox1.Show();
                        multiColumnComboBox1.Enabled = true;
                        vervang_lbl.Text = "Kies door welk bedrijf het huidige bedrijf moet worden vervangen.";
                        vervang_lbl.Location = new Point(p_verw_verv.Width / 2 - vervang_lbl.Width / 2, vervang_lbl.Location.Y);
                        vervangt = true;
                        button5.Enabled = false;
                        button5.Text = "Doorgaan";
                    }
                    else
                    {
                        handelingen_logger.log_handeling(int.Parse(id), 2, 24);
                        btn_verwijder_annulleer.PerformClick();
                        btnterug.PerformClick();
                    }
                }
                else
                {
                    handelingen_logger.log_handeling(int.Parse(id), 2, 24);
                    MessageBox.Show("Dit bedrijf komt " + result + " keer voor in andere gegevens.\nHierdoor kan dit bedrijf niet worden verwijderd.\nKlik op \"ok\" om terug te gaan naar de lees-stand.");
                    btn_verwijder_annulleer.PerformClick();
                    btnterug.PerformClick();
                }
            }
            else
            {
                //MessageBox.Show("Dit bedrijf komt " + result + " keer voor in andere gegevens.\n\r U kunt dit bedrijf verwijderen.5");
                // delete bedrijf (mb?) 
                System.Data.SqlClient.SqlConnection con;
                con = new System.Data.SqlClient.SqlConnection();
                con.ConnectionString = Global.ConnectionString_fileserver;
                con.Open();
                string stopro = "delete_bedrijf";
                System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(stopro, con);
                System.Data.SqlClient.SqlParameter persoon_nr = command.Parameters.Add("@bedrijf_nr", SqlDbType.Int);
                persoon_nr.Value = bid;
                persoon_nr.Direction = ParameterDirection.Input;
                command.CommandType = CommandType.StoredProcedure;
                int ok = command.ExecuteNonQuery();
                if (ok > 0)
                {
                    MessageBox.Show("succesvol verwijderd");
                    handelingen_logger.log_handeling(int.Parse(id), 2, 21);
                    this.wijzigstand = false;
                    telefoonnummer_user_control1.Wijzigstand(false);
                    telefoonnummer_user_control2.Wijzigstand(false);
                    telefoonnummer_user_control3.Wijzigstand(false);
                    Verwijder_ingebruik(bid, 2);
                    if (start_parent is overview1)
                    {
                        (start_parent as overview1).refresh_zoek();
                    }
                    start_parent.herlaad();


                    this.sluit();
                    Close();
                }
            }
            
                //qtadapt.Dispose();
                locaties_adapt.Dispose();
                locaties_table.Dispose();
            
        }

        private void textBoxNumOnly_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            string Str = tb.Text.Trim();

            double Num;

            bool isNum = double.TryParse(Str, out Num);

            if (isNum)
            { tb.BackColor = Color.White; }
            else
            {
                tb.BackColor = Color.Crimson;
                // tb.Clear();
            }
        }

        private void groupBox1_Leave(object sender, EventArgs e)//nieuw bezoek
        {
            if (checkBox1.Checked)
            {
                adres_user_control2.Plaats = adres_user_control1.Plaats;
                adres_user_control2.Land = adres_user_control1.Land;
                adres_user_control2.Straat = adres_user_control1.Straat;
                adres_user_control2.Postcode_cijfers = adres_user_control1.Postcode_cijfers;
                adres_user_control2.Postcode_letters = adres_user_control1.Postcode_letters;
                adres_user_control2.Huisnummer = adres_user_control1.Huisnummer;
                adres_user_control2.Huisnummer_toevoeging = adres_user_control1.Huisnummer_toevoeging;
                adres_user_control2.Straat2 = adres_user_control1.Straat2;
            }
            else
            {
                if (adres_user_control1.Plaats == adres_user_control2.Plaats && adres_user_control1.Land == adres_user_control2.Land && adres_user_control1.Straat == adres_user_control2.Straat && adres_user_control1.Postcode_cijfers == adres_user_control2.Postcode_cijfers && adres_user_control1.Postcode_letters == adres_user_control2.Postcode_letters && adres_user_control1.Huisnummer == adres_user_control2.Huisnummer && adres_user_control1.Huisnummer_toevoeging == adres_user_control2.Huisnummer_toevoeging && adres_user_control1.Straat2 == adres_user_control2.Straat2)
                {
                    checkBox1.Checked = true;
                }
              
            }
            
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            panel1.BringToFront();
            button1.Image = zeebregtsCs.Properties.Resources.LAN_Party_pictogram_svg_med;
            button2.Image = zeebregtsCs.Properties.Resources.sym_function_x_blue_gray;
            button3.Image = zeebregtsCs.Properties.Resources.haxorgray;
            panel1.Focus();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button1.Image = zeebregtsCs.Properties.Resources.LAN_Party_pictogram_gray;
            button2.Image = zeebregtsCs.Properties.Resources.sym_function_x_blue;
            button3.Image = zeebregtsCs.Properties.Resources.haxorgray;
            panel4.BringToFront();
            panel4.Focus();
        }
        private void vervang_bedrijf()
        {
            int id_oud = int.Parse(id);
            int id_new = int.Parse(multiColumnComboBox1.SelectedValue.ToString());
            string column = "";
            string table = "";
            bool result = true;
           // bedrijfformdatasetTableAdapters.QueriesTableAdapter qtadapt = new bedrijfformdatasetTableAdapters.QueriesTableAdapter();
            bedrijfformdatasetTableAdapters.bedrijf_nr_locatiesTableAdapter locaties_adapt = new bedrijfformdatasetTableAdapters.bedrijf_nr_locatiesTableAdapter();
            locaties_adapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
            bedrijfformdataset.bedrijf_nr_locatiesDataTable locaties_table = locaties_adapt.GetData();
            foreach (bedrijfformdataset.bedrijf_nr_locatiesRow row in locaties_table.Rows)
            {
                column = row.column.ToString();
                table = row.tabel.ToString();
                try
                {
                    adapter.vervang_del_bdr(id_oud, column, table, id_new);
                    verwijder_panel1.start_init(2, id_oud);
                }
                catch (Exception ex)
                {
                    result = false;
                    String log_line = "SQL exception@ " + DateTime.Now.ToString() + "error: " + ex;
                    System.IO.StreamWriter file = new System.IO.StreamWriter(Global.log_file_path, true);
                    file.WriteLine(log_line);
                    file.Close();
                }
                
            }
            string msg = "";
            if (result)
            {
                handelingen_logger.log_handeling(int.Parse(id), 2, 25);
                btn_verwijder_bevestig.Enabled = true;
                btn_verwijder_bevestig.PerformClick();
                msg = "Succesvol vervangen en verwijderd";
            }
            else
            {
                msg = "Er is een fout opgetreden bij het vervangen.\nNiet verwijderd";
            }
            MessageBox.Show(msg);
        }
        bool vervangt = false;
        bool uitvoeren = false;
        private void button5_Click(object sender, EventArgs e)//JA
        {
            if (uitvoeren && vervangt)
            {
                vervang_bedrijf();
            }
            else if (vervangt)
            {
                multiColumnComboBox1.SelectionLength = 0;
                multiColumnComboBox1.Enabled = false;
                vervang_lbl.Text = "Vervang het bedrijf \"ID " + id + " - " + textBox3.Text + "\" \ndoor het bedrijf \"ID " + multiColumnComboBox1.SelectedValue.ToString() + " - " + multiColumnComboBox1.Text + "\"";
                vervang_lbl.Location = new Point(p_verw_verv.Width / 2 - vervang_lbl.Width / 2, vervang_lbl.Location.Y);
                uitvoeren = true;
                button5.Text = "Bevestigen";
            }
            else if (!vervangt && !uitvoeren)
            {
                
            }
        }

        
        private void button6_Click(object sender, EventArgs e)//NEE
        {
            if (uitvoeren)
            {
                uitvoeren = false;
                vervangt = false;
                vervang_lbl.Text = "Dit bedrijf verwijderen en vervangen?";
                multiColumnComboBox1.Enabled = true;
                multiColumnComboBox1.Hide();
                multiColumnComboBox1.SelectedIndex = -1;
            }
            else if (vervangt)
            {
                vervangt = false;
                vervang_lbl.Text = "Dit bedrijf verwijderen en vervangen?";
                multiColumnComboBox1.Hide();
                multiColumnComboBox1.SelectedIndex = -1;
                multiColumnComboBox1.Enabled = true;
                button5.Enabled = true;
            }
            else if (!uitvoeren && !vervangt)
            {
                p_verw_verv.Hide();
                Free_screen();
                btn_verwijder.Show();
                btn_verwijder_annulleer.Hide();
                btn_verwijder_bevestig.Hide();
            }
        }

        private void button4_Click(object sender, EventArgs e)//vind
        {
            string tmpnaam = textBox3.Text;
            if (this.WindowState == FormWindowState.Normal)
            {
                Global.size = this.Size;
                Global.position = this.Location;
            }
            Global.windowstate = this.WindowState;
            Global.overzicht_type = 2;
            Global.give_return = true;

            Form_helper BFH = new Form_helper();
            BFH.Start_route(2, 3, this, this, int.Parse(id), "de vervanging ", tmpnaam);
            this.Hide();
            Global.overzicht_type = 0;
        }

        private void multiColumnComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            MultiColumnComboBoxDemo.MultiColumnComboBox mcbb = sender as MultiColumnComboBoxDemo.MultiColumnComboBox;
            if (vervangt)
            {
                if (mcbb.SelectedIndex == -1)
                {
                    button5.Enabled = false;
                }
                else
                {
                    button5.Enabled = true;
                }
            }
            
        }

        private void button3_Click(object sender, EventArgs e)//h@x0r panel
        {
            string naam = String.Empty;
            string zoeknaam = String.Empty;
            string postcode = String.Empty;
            if(!Rowbedrijf.IsnaamNull())
            {
                naam = Rowbedrijf.naam;
            }
            if(!Rowbedrijf.IszoeknaamNull())
            {
                zoeknaam = Rowbedrijf.zoeknaam;
            }
            if(!Rowbedrijf.IspostPOSTCODENull())
            {
                postcode = Rowbedrijf.postPOSTCODE;
            }
            haxor_panel1.start_loading_bdr(naam, zoeknaam, postcode);
            button1.Image = zeebregtsCs.Properties.Resources.LAN_Party_pictogram_gray;
            button2.Image = zeebregtsCs.Properties.Resources.sym_function_x_blue_gray;
            button3.Image = zeebregtsCs.Properties.Resources.haxor;
            panel5.BringToFront();
            panel5.Focus();
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            string summary = String.Empty;
            string copy_bdrnaam = String.Empty;
            string copy_tel_1 = String.Empty;
            string copy_tel_2 = String.Empty;
            string copy_tel_3 = String.Empty;
            string copy_web = String.Empty;
            string copy_adres = String.Empty;
            string[] tel_sett = Rowbedrijf.telefoon_nr_settings.Split(',');
            copy_bdrnaam = Rowbedrijf.naam;
            if (!Rowbedrijf.Istelefoon_nr_1Null() && Rowbedrijf.telefoon_nr_1.Length > 2)
            {
                int sett = int.Parse(tel_sett[0]);
                if (sett == 1)
                {
                    copy_tel_1 = "\nM " + Rowbedrijf.telefoon_nr_1;
                }
                else if (sett == 2)
                {
                    copy_tel_1 = "\nF " + Rowbedrijf.telefoon_nr_1;
                }
                else
                {
                    copy_tel_1 = "\nT " + Rowbedrijf.telefoon_nr_1;
                }
                
            }
            if (!Rowbedrijf.Istelefoon_nr_2Null() && Rowbedrijf.telefoon_nr_2.Length > 2)
            {
                int sett = int.Parse(tel_sett[1]);
                if (sett == 1)
                {
                    copy_tel_2 = "\nM " + Rowbedrijf.telefoon_nr_2;
                }
                else if (sett == 2)
                {
                    copy_tel_2 = "\nF " + Rowbedrijf.telefoon_nr_2;
                }
                else
                {
                    copy_tel_2 = "\nT " + Rowbedrijf.telefoon_nr_2;
                }

            }
            if (!Rowbedrijf.Istelefoon_nr_3Null() && Rowbedrijf.telefoon_nr_3.Length > 2)
            {
                int sett = int.Parse(tel_sett[2]);
                if (sett == 1)
                {
                    copy_tel_3 = "\nM " + Rowbedrijf.telefoon_nr_3;
                }
                else if (sett == 2)
                {
                    copy_tel_3 = "\nF " + Rowbedrijf.telefoon_nr_3;
                }
                else
                {
                    copy_tel_3 = "\nT " + Rowbedrijf.telefoon_nr_3;
                }

            }
            copy_adres = "\n";
            if (Rowbedrijf.adres_id_bezoek != Rowbedrijf.adres_id_post && Rowbedrijf.adres_id_post > 0)
            {
                copy_adres += "Bezoekadres: ";
                copy_adres += adres_user_control1.Straat+ " "+adres_user_control1.Huisnummer+adres_user_control1.Huisnummer_toevoeging;
                copy_adres += ", " + adres_user_control1.Postcode_cijfers + adres_user_control1.Postcode_letters;
                copy_adres += " " + adres_user_control1.Plaats;
                copy_adres += "\nPostadres: ";
                copy_adres += adres_user_control2.Straat + " " + adres_user_control2.Huisnummer + adres_user_control2.Huisnummer_toevoeging;
                copy_adres += ", " + adres_user_control2.Postcode_cijfers + adres_user_control2.Postcode_letters;
                copy_adres += " " + adres_user_control2.Plaats;
            }
            else
            {
                copy_adres += "Bezoek- en postadres: ";
                copy_adres += adres_user_control1.Straat+" "+ adres_user_control1.Huisnummer+adres_user_control1.Huisnummer_toevoeging;
                copy_adres += ", " + adres_user_control1.Postcode_cijfers + adres_user_control1.Postcode_letters;
                copy_adres += " " + adres_user_control1.Plaats;
            }

            if (!Rowbedrijf.IswebsiteNull())
            {
                copy_web = "\n" + Rowbedrijf.website;
            }
           string copy_kvk = String.Empty;
            if (!Rowbedrijf.IskvkNull())
            {
                copy_kvk = "\nKvk: "+Rowbedrijf.kvk;
            }
            
            summary = copy_bdrnaam+copy_adres+copy_kvk+copy_web+copy_tel_1+copy_tel_2+copy_tel_3;
            System.Windows.Forms.Clipboard.SetDataObject(summary, true);
            handelingen_logger.log_handeling(int.Parse(id), 2, 18);
        }

        private void splitContainer31_Panel2_MouseHover(object sender, EventArgs e)
        {
            Control c = (Control)sender;
            if (c is SplitterPanel)
            {
                tip1.Show("Door te klikken wordt er een beknopte kopie gemaakt van dit formulier, zodat vervolgens de tekst in een document geplakt kan worden.", (c.Parent as SplitContainer).Panel1, 0, -40);
            }
            else if (c is Label || c is Button)
            {
                tip1.Show("Door te klikken wordt er een beknopte kopie gemaakt van dit formulier, zodat vervolgens de tekst in een document geplakt kan worden.", (c.Parent.Parent as SplitContainer).Panel1, 0, -40);
            }
        }

        private void splitContainer31_Panel2_MouseLeave(object sender, EventArgs e)
        {
            Control c = (Control)sender;
            if (c is SplitterPanel)
            {

                try { tip1.Hide((c.Parent as SplitContainer).Panel1); }
                catch (Exception ex)
                {
                    log_exception(ex);
                }
            }
            else if (c is Label || c is Button)
            {
                try { tip1.Hide((c.Parent.Parent as SplitContainer).Panel1); }
                catch (Exception ex)
                {
                    log_exception(ex);
                }
            }
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            Panel p = sender as Panel;
            p.Focus();
        }

        private void bedrijf_form_Shown(object sender, EventArgs e)
        {
            Organize_panels();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)//email
        {
           
            if (linkLabel2.Text != "-")
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Right)
                {
                    System.Windows.Forms.Clipboard.SetDataObject(linkLabel2.Text, true);
                }
                else
                { 
                    System.Diagnostics.Process.Start("mailto:" + linkLabel2.Text);
                    handelingen_logger.log_handeling(int.Parse(id), 2, 19);
                }
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)//website
        {
            ProcessStartInfo sInfo = new ProcessStartInfo(e.Link.LinkData.ToString());
            Process.Start(sInfo);
            handelingen_logger.log_handeling(int.Parse(id), 2, 20);
        }

        private void btn_add_telnr_Click(object sender, EventArgs e)//telnr add
        {
            Control x = new Control();
            foreach (Control c in p_telnrs.Controls)
            {
                if (c is System.Windows.Forms.Integration.ElementHost)
                {
                    if (!c.Visible)
                    {
                        x = c;
                        c.Visible = true;
                        break;
                    }
                }

            }
            p_telnrs.Controls.Remove(x);
            p_telnrs.Controls.Remove((sender as Button));
            p_telnrs.Controls.Add(x);
            p_telnrs.Controls.Add((sender as Button));
            Organize_panels();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string summary = String.Empty;
            string copy_post_adres = String.Empty;
            if (!Rowbedrijf.IspostSTRAATNull())
            {
                copy_post_adres += "\n" + Rowbedrijf.postSTRAAT;
            }
            if (!Rowbedrijf.IspostPOSTCODENull())
            {
                copy_post_adres += "\n" + Rowbedrijf.postPOSTCODE;
            }
            if (!Rowbedrijf.IspostPLAATSNull())
            {
                copy_post_adres += " " + Rowbedrijf.postPLAATS;
            }
            summary = label23.Text + copy_post_adres;
            System.Windows.Forms.Clipboard.SetDataObject(summary, true);
            handelingen_logger.log_handeling(int.Parse(id), 2, 22);
        }

      



       
    }
}
