using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using zeebregtsCs.ExchangeService;
using zeebregtsCs.usercontrols;

namespace zeebregtsCs
{
    public partial class persoon_form : base_form
    {
        string init_name = "";
        persoonsformdatasetTableAdapters.persoonTableAdapter adapter = new persoonsformdatasetTableAdapters.persoonTableAdapter();
        persoonsformdatasetTableAdapters.bedrijflinkTableAdapter linkadapter= new persoonsformdatasetTableAdapters.bedrijflinkTableAdapter();
        persoonsformdatasetTableAdapters.bedrijfTableAdapter bdradapter = new persoonsformdatasetTableAdapters.bedrijfTableAdapter();
        persoonsformdataset.persoonDataTable persoontable;
        persoonsformdataset.bedrijfDataTable bedrijftable;
        persoonsformdataset.bedrijflinkDataTable linktable;
        persoonsformdataset.persoonRow Rowpersoon;
        persoonsformdataset.bedrijflinkRow Rowlink;
        testsetTableAdapters.persoonTableAdapter pta = new testsetTableAdapters.persoonTableAdapter();
        testset.persoonDataTable pdt = new testset.persoonDataTable();
        //new varsss
        ToolTip tip_copy;
        Form_helper PFH;
        bool[] ingevulde_nrs = {false,false,false};
        bool van_zoek = false;
        int enum_nr;
        string antw = "";
        string vrijgeefstring = "";
        string fullname;
        //
        bool editing = false;
        bool date = false;

        bool update = false;
        string id = "0";
        int indexId = 0;
        
        int bdrnr;


        public persoon_form(base_form start_scherm, base_form close_naar,int PID, string vnm, string own, int enumr)
        {
            adapter.Connection.ConnectionString = Global.ConnectionString_fileserver;
            linkadapter.Connection.ConnectionString = Global.ConnectionString_fileserver;
            bdradapter.Connection.ConnectionString = Global.ConnectionString_fileserver;
            pta.Connection.ConnectionString = Global.ConnectionString_fileserver;

            Cursor.Current = Cursors.WaitCursor;
            //Global.FW_add(this, close_naar);
            this.DoubleBuffered = true;
            FormManager.VoegToe(this, close_naar);
            InitializeComponent();
            start_parent = start_scherm;
            close_parent = close_naar;
            Parent_ID = PID;
            veldnaam = vnm;
            ownernaam = own;
            enum_nr = enumr;
            this.Text = Global.WTitle;
            initialiseer();
        }
       
        private void initialiseer()
        {
            van_zoek = Global.van_zoek_pers;
            
            switch (enum_nr)
            {
                case 0://normaal
                    if (van_zoek || Global.Dubb_van_zoek)
                    {
                        Global.Dubb_van_zoek = false;
                        btnterug.Image = null;
                        btnterug.Image = Properties.Resources.ZoekContact2;
                    }
                    else
                    {
                        btnterug.Image = null;
                        btnterug.Image = Properties.Resources.AlleContacten;
                    }
                    lbl_top.Text = "";
                    lbl_mid.Text = "Info contact";
                    break;
                case 1://nieuw-clean
                    btnterug.Hide();
                    lbl_top.Text = "";
                    lbl_mid.Text = "Nieuw contact";
                    break;
                case 2://nieuw-owner
                    btnterug.Hide();
                    btn_verwijder.Enabled = false;
                    lbl_mid.Text = "Nieuw contact";
                    lbl_top.Text = "Kies " + veldnaam + "van het project " + ownernaam;
                    break;
                case 3://normaal-sub
                    btnterug.Image = null;
                    btnterug.Image = Properties.Resources.InfoProject2;
                    btn_verwijder.Enabled = false;
                    lbl_mid.Text = "Info contact";
                    lbl_top.Text = "Info van " + veldnaam + "van het project " + ownernaam;
                    break;
                case 4:
                    btnterug.Image = Properties.Resources.Terug;
                    lbl_top.Text = "Bekijk de gegevens t.b.v. de dubbele invoer controle";
                    lbl_mid.Text = "Overeenkomstig contact";
                    break;
            }
            if (!Global.NoConnection)
            {
                this.bedrijfTableAdapter1.Connection.ConnectionString = Global.ConnectionString_fileserver;
                this.bedrijfTableAdapter.Connection.ConnectionString = Global.ConnectionString_fileserver;
                this.bedrijfTableAdapter1.Fill(this.persoonsformdataset1.bedrijf);
                this.bedrijfTableAdapter.Fill(this.testset.bedrijf);
            }
            init_all();
        }
        private void init_all()
        {
            
            button2.PerformClick();
            btn_verwijder.Hide();
            p_verw_verv.Hide();
            btn_verwijder_annulleer.Hide();
            btn_verwijder_bevestig.Hide();
            telefoonnummer_user_control1.set_naam(1,false);
            telefoonnummer_user_control2.set_naam(0,false);
            telefoonnummer_user_control3.set_naam(2,false);
            label13.Text = "-";
            label14.Text = "-";
            label15.Text = "-";
            label16.Text = "-";
            label17.Text = "-";
            label22.Text = "-";
            label24.Text = "-";
            label26.Text = "-";
            label28.Text = "-";
            label30.Text = "-";
             dbcon();
            id = Global.overzicht_select;
            
           
            Global.overzicht_select = "";
            this.btnwijzig.Hide();
            this.btnopslaan.Show();
            no_edit();
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "dddd dd - MMMM MM - yyyy";
            Point pt1 = new Point(panel2.Width / 2 - lbl_top.Width / 2, 5);
            lbl_top.Location = pt1;
            Point pt2 = new Point(panel2.Width / 2 - lbl_mid.Width / 2, 28);
            lbl_mid.Location = pt2;
            Point pt3 = new Point(panel2.Width / 2 - lbl_bot.Width / 2, panel2.Height - 18);
            lbl_bot.Location = pt3;
            panel3.Focus();
            if (enum_nr == 4)
            {
                btnwijzig.Hide();
                btnopslaan.Hide();
            }
            if (Global.UserLevel < 3)
            {
                button4.Show();
            }
            else
            {
                button4.Hide();
            }
            tip_copy = new ToolTip();
            ToolTip_settings(tip_copy);
            
        }
        private void ToolTip_settings(ToolTip tt)
        {

            tt.AutoPopDelay = 10000;
            tt.InitialDelay = 200;
            tt.IsBalloon = true;
            tt.ReshowDelay = 300;
            tt.ShowAlways = true;
        }
        private void dbcon()
        {
            if (!Global.NoConnection)
            {
                
                bedrijftable = bdradapter.GetData();
                mcb1.BindingContext = new BindingContext();
                mcb1.DisplayMember = "zoeknaam";
                mcb1.ValueMember = "bedrijf_nr";
                mcb1.DataSource = bedrijftable;
                mcb1.SelectedIndex = -1;


                pdt = pta.GetDataBy(int.Parse(id));
                multiColumnComboBox1.BindingContext = new BindingContext();
                multiColumnComboBox1.DisplayMember = "SearchName";
                multiColumnComboBox1.ValueMember = "persoon_nr";
                multiColumnComboBox1.DataSource = pdt;
                multiColumnComboBox1.SelectedIndex = -1;
                
                
            }
           
        }
        private void persoon_form_Load(object sender, EventArgs e)
        {
            this.MinimumSize = new Size(450, 310);
            PFH = new Form_helper();
            if (enum_nr == 1 || enum_nr == 2)
            {
                btnwijzig.PerformClick();

            }
       //     this.Size = Global.size;
        //    this.Location = Global.position;
         //   this.WindowState = Global.windowstate;
            disable_splitter(this);
            label13.Focus();
            if (Global.Dubbel_is_bevestigd)
            {
               
                if (start_parent.s_parent().c_parent() is project_form)
                {
                    Global.return_id =id;
                    close_parent = start_parent.c_parent();
                    (start_parent.s_parent().c_parent() as project_form).fill_id(false);
                    start_parent.s_parent().c_parent().herlaad();
                    Cursor.Current = Cursors.Default;
                    FormManager.Sluit_forms(start_parent.s_parent());
                    
                    this.Close();
                }
                else if (start_parent is overview1)
                {
                    // FormManager.Sluit_form(start_parent.s_parent());
                    // FormManager.Sluit_form(start_parent);
                    start_parent = start_parent.s_parent().s_parent();
                    close_parent = start_parent.c_parent();
                    enum_nr = 0;

                    //initialiseer();
                }
                
            }

            Cursor.Current = Cursors.Default;
            button3.PerformClick();
            //Organize_panels();
        }
        private void set_telnr_type(string settingss)
        {
            string[] types = settingss.Split(',');
            telefoonnummer_user_control1.set_naam(int.Parse(types[0]), false);
            telefoonnummer_user_control2.set_naam(int.Parse(types[1]), false);
            telefoonnummer_user_control3.set_naam(int.Parse(types[2]), false);

        }
        private string Get_werkgevers_log()
        {
            string log = String.Empty;
            persoonsformdatasetTableAdapters.werkgevers_logTableAdapter wta = new persoonsformdatasetTableAdapters.werkgevers_logTableAdapter();
            wta.Connection.ConnectionString = Global.ConnectionString_fileserver;
            persoonsformdataset.werkgevers_logDataTable wdt = wta.GetData(int.Parse(id));
            foreach (persoonsformdataset.werkgevers_logRow wglR in wdt.Rows)
            {
                if (wglR.naam.Length > 2)
                {
                    log += wglR.naam + " was de werkgever van dit contact tot " + wglR.timestamp+"\n";
                }
            }
            return log;
        }
        private void NavigateRecord()
        {
            label37.Text = "";
            init_name = "";
            try
            {
                dbcon();
                
                persoontable = adapter.GetDataBy(int.Parse(id));
                Rowpersoon = (persoonsformdataset.persoonRow)persoontable.Rows[0];
                linktable = linkadapter.GetData(int.Parse(id));
                Rowlink = (persoonsformdataset.bedrijflinkRow)linktable.Rows[0];
                fullname = "";
                indexId = Rowpersoon.persoon_ID;
                verwijder_panel1.start_init(3, int.Parse(id));
                string[] functies = verwijder_panel1.Get_function();
                if (functies[0] != null)
                {
                    if (functies[0].Length > 2)
                    {
                        label39.Text = functies[0];
                        textBox15.Text = functies[0];
                    }
                    else
                    {
                        label39.Parent.Parent.Hide();
                    }
                }
                else
                {
                    label39.Parent.Parent.Hide();
                }
                if (functies[1] != null)
                {
                    if (functies[1].Length > 2)
                    {
                        label41.Text = functies[1];
                    }
                    else
                    {
                        label41.Text = String.Empty;
                        label41.Parent.Parent.Hide();
                    }
                }
                else
                {
                    label41.Text = String.Empty;
                    label41.Parent.Parent.Hide();
                }
                ////////
                vWerkg_lbl.Text = Get_werkgevers_log();
                ///////
                if (update == false)
                {
                    if (!Rowpersoon.Isbedrijf_nrNull())
                    {
                        label21.Text = Rowlink.zoeknaam;

                        bdrnr = Rowpersoon.bedrijf_nr;
                        //Get_Set_plaats();
                    }
                    else
                    {
                        label21.Parent.Parent.Hide();
                        label21.Text = "-";
                    }
                }
                else
                {
                    label21.Text = bdrnr.ToString();
                    update = false;
                }
                ////////newlog
                persoonsformdatasetTableAdapters.new_del_record_logTableAdapter NdrL_adapt = new persoonsformdatasetTableAdapters.new_del_record_logTableAdapter();
                NdrL_adapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
                persoonsformdataset.new_del_record_logDataTable NdrL_dt = NdrL_adapt.GetData(int.Parse(id), "persoon");
                if (NdrL_dt.Rows.Count > 0)
                {
                    persoonsformdataset.new_del_record_logRow NdrL_row = (persoonsformdataset.new_del_record_logRow)NdrL_dt.Rows[0];
                    if (!NdrL_row.IsnaamNull() && !NdrL_row.Isdatum_tijdNull())
                    {
                        label43.Text = "Dit contact is aangemaakt door: " + NdrL_row.naam + " op: " + NdrL_row.datum_tijd.ToString();
                    }
                    else
                    {
                        label43.Text = "Het is niet bekend wanneer en door wie dit contact is ingevoerd";
                    }
                }
                else
                {
                    label43.Text = "Het is niet bekend wanneer en door wie dit contact is ingevoerd";
                }
                ///////
                // bedrijf
                if (!Rowpersoon.Isbedrijf_nrNull())
                {
                    mcb1.SelectedValue = Rowlink.bedrijf_nr;
                    DataRowView drv = (DataRowView)mcb1.SelectedItem;
                    label21.Text = drv.Row.ItemArray[4].ToString();

                }
                else
                {
                    label21.Parent.Parent.Hide();
                    label21.Text = "-";
                }
                //man/vrouw
                if (!Rowpersoon.IsmanNull())
                {
                    if (!Rowpersoon.man)
                    {
                        radioButton1.Checked = true;
                        radioButton2.Checked = false;
                        label13.Text = "man";
                        label37.Text += "Dhr. ";
                    }
                    else
                    {
                        radioButton2.Checked = true;
                        radioButton1.Checked = false;
                        label13.Text = "vrouw";
                        label37.Text += "Mvr. ";
                    }
                }
                else
                {
                    label13.Parent.Parent.Hide();
                    label13.Text = "nb";
                }
                //voorletters
                if (!Rowpersoon.IsvoorlettersNull())
                {
                    textBox3.Text = Rowpersoon.voorletters;
                    label14.Text = Rowpersoon.voorletters;
                    label37.Text += "(" + Rowpersoon.voorletters + ") ";
                }
                else
                {
                    label14.Parent.Parent.Hide();
                    label14.Text = "-";
                }
                //voornaam
                if (!Rowpersoon.IsvoornaamNull())
                {
                    fullname += Rowpersoon.voornaam + " ";
                    textBox4.Text = Rowpersoon.voornaam;
                    label15.Text = Rowpersoon.voornaam;
                    init_name += Rowpersoon.voornaam;
                }
                else
                {
                    label15.Parent.Parent.Hide();
                    label15.Text = "-";
                }
                //tussenvoegsel
                if (!Rowpersoon.IstussenvoegselNull() && Rowpersoon.tussenvoegsel.Length > 0)
                {
                    fullname += Rowpersoon.tussenvoegsel + " ";
                    textBox5.Text = Rowpersoon.tussenvoegsel;
                    label16.Text = Rowpersoon.tussenvoegsel;
                    radioButton5.Checked = false; radioButton6.Checked = true;
                    label35.Text = "ja";
                    init_name += " " + Rowpersoon.tussenvoegsel;

                }
                else
                {
                    radioButton5.Checked = true; radioButton6.Checked = false;
                    textBox5.Hide();
                    label35.Text = "nee";
                }
                //achternaam
                if (!Rowpersoon.IsachternaamNull())
                {
                    fullname += Rowpersoon.achternaam;
                    textBox6.Text = Rowpersoon.achternaam;
                    label17.Text = Rowpersoon.achternaam;
                    init_name += " " + Rowpersoon.achternaam;
                }
                else
                {
                    label17.Parent.Parent.Hide();
                    label17.Text = "-";
                }
                label37.Text += fullname;
                vrijgeefstring =fullname;
                if (!Rowpersoon.Istelefoon_nr_settingsNull())
                {
                    set_telnr_type(Rowpersoon.telefoon_nr_settings);
                }
                string[] telsetts;
                if (!Rowpersoon.Istelefoon_nr_settingsNull())
                {
                    telsetts = Rowpersoon.telefoon_nr_settings.Split(',');
                }
                else
                {
                    telsetts = "1,0,2".Split(',');
                }
               if (!Rowpersoon.Istelefoon_nr_1Null() && Rowpersoon.telefoon_nr_1 != String.Empty)
               {
                   elementHost1.Show();
                   telefoonnummer_user_control1.set_Nummer(Rowpersoon.telefoon_nr_1, this);
               }
               else
               {
                   elementHost1.Hide();
                   telefoonnummer_user_control1.set_Nummer("", this);
               }
               if (!Rowpersoon.Istelefoon_nr_2Null() && Rowpersoon.telefoon_nr_2 != String.Empty)
               {
                   elementHost2.Show();
                   telefoonnummer_user_control2.set_Nummer(Rowpersoon.telefoon_nr_2, this);
               }
               else
               {
                   elementHost2.Hide();
                   telefoonnummer_user_control2.set_Nummer("", this);
               }
               if (!Rowpersoon.Istelefoon_nr_3Null() && Rowpersoon.telefoon_nr_3 != String.Empty)
               {
                   elementHost3.Show();
                   telefoonnummer_user_control3.set_Nummer(Rowpersoon.telefoon_nr_3, this);
               }
               else
               {
                   elementHost3.Hide();
                   telefoonnummer_user_control3.set_Nummer("", this);
               }
              
                //email
               if (!Rowpersoon.IszaemailNull())
               {
                   textBox10.Text = Rowpersoon.zaemail;
                   linkLabel2.Text = Rowpersoon.zaemail;
               }
               else
               {
                   linkLabel2.Parent.Parent.Hide();
                   linkLabel2.Text = "-";
               }
                //geboortedatum

                if (!Rowpersoon.Isgeboortedatum1Null())
                {
                    dateTimePicker1.Value = Rowpersoon.geboortedatum1;
                    textBox14.Text = dateTimePicker1.Value.Day.ToString();
                    textBox2.Text = dateTimePicker1.Value.Month.ToString();
                    textBox1.Text = dateTimePicker1.Value.Year.ToString();
                    label22.Text = dateTimePicker1.Value.ToLongDateString();
                }
                else
                {
                    label22.Parent.Parent.Hide();
                    label22.Text = "-";
                }



                //vaste vrijedag
                if (!Rowpersoon.Isvastevrijedag1Null())
                {
                    splitContainer12.Show();
                    textBox11.Text = Rowpersoon.vastevrijedag1;
                    label24.Text = Rowpersoon.vastevrijedag1;
                }
                else
                {   
                    splitContainer12.Hide();
                }
                //vastevrijedag 2
                if (!Rowpersoon.Isvastevrijedag2Null())
                {
                    splitContainer11.Show();
                    textBox12.Text = Rowpersoon.vastevrijedag2;
                    label26.Text = Rowpersoon.vastevrijedag2;
                }
                else
                {
                    splitContainer11.Hide();
                }
                //vastevrijedag 3
                if (!Rowpersoon.Isvastevrijedag3Null())
                {
                    splitContainer16.Show();
                    textBox13.Text = Rowpersoon.vastevrijedag3;
                    label28.Text = Rowpersoon.vastevrijedag3;
                }
                else
                {
                    splitContainer16.Hide();
                }

                //actief/niet actief
                if (!Rowpersoon.IsNIETactiefNull())
                {
                    if (!Rowpersoon.NIETactief)
                    {
                        radioButton3.Checked = true;
                        radioButton4.Checked = false;
                        label30.Text = "ja";
                    }
                    else
                    {
                        radioButton3.Checked = false;
                        radioButton4.Checked = true;
                        label30.Text = "nee";
                    }
                } lbl_bot.Text = "ID " + id + " - " + fullname;
                
                
            }
            catch (Exception e)
            {
                String log_line = "crash program @ " + DateTime.Now.ToString() + "error: " + e;
                System.IO.StreamWriter file = new System.IO.StreamWriter(Global.log_file_path, true);
                file.WriteLine(log_line);
                file.Close();
                MessageBox.Show("fout bij inladen, wijzigen niet mogelijk");
                btnopslaan.Enabled = false;
                btnwijzig.Enabled = false;
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
           if (label41.Text == String.Empty)
           {
               label41.Parent.Parent.Hide();
           }
        }
        
        private void no_edit()
        {
            using (Panel tmp_p = new Panel())
            {
                tmp_p.Location = panel3.Location;
                tmp_p.Size = panel3.Size;
                tmp_p.BackColor = System.Drawing.SystemColors.ControlLightLight;
                tmp_p.BorderStyle = BorderStyle.Fixed3D;
                this.Controls.Add(tmp_p);
                tmp_p.Show();
                tmp_p.BringToFront();
                splitContainer20.Show();
                button6.Enabled = true;
                button7.Enabled = true;
                button8.Enabled = true;
                button9.Hide();
                textBox1.Hide();
                textBox2.Hide();
                textBox14.Hide();
                label12.Hide();
                label31.Hide();
                label32.Hide();
                label33.Hide();
                textBox15.Hide();
                label39.Show();
                label41.Show();
                Clear_text(this);

                splitContainer17.Show();
                // splitContainer17.Location = new Point(10, 130);
                splitContainer17.Visible = true;
                splitContainer17.BringToFront();
                splitContainer2.Hide();
                splitContainer3.Hide();
                splitContainer4.Hide();
                splitContainer14.Hide();
                splitContainer6.Hide();
                splitContainer5.Hide();
                //      splitContainer7.Location = new Point(splitContainer7.Location.X, splitContainer7.Location.Y - (5*25));
                //      splitContainer13.Location = new Point(splitContainer13.Location.X, splitContainer13.Location.Y - (5*25));
                //       splitContainer15.Location = new Point(splitContainer15.Location.X, splitContainer15.Location.Y - (5*25));
                //elementHost1.Location = new Point(elementHost1.Location.X, elementHost1.Location.Y - (5 * 33));
                //elementHost2.Location = new Point(elementHost2.Location.X, elementHost2.Location.Y - (5 * 33));
                //elementHost3.Location = new Point(elementHost3.Location.X, elementHost3.Location.Y - (5 * 33));
                // p_contact_gegevens.Height = p_contact_gegevens.Height - 125;
                // p_telnrs.Location = new Point(p_telnrs.Location.X, p_telnrs.Location.Y - 125);
                mcb1.Hide();
                radioButton1.Hide();
                radioButton2.Hide();
              //  button1.Hide();
                dateTimePicker1.Hide();
                this.textBox3.Hide();
                this.textBox4.Hide();
                this.textBox5.Hide();
                this.textBox6.Hide();
                this.textBox10.Hide();
                this.textBox11.Hide();
                this.textBox12.Hide();
                this.textBox13.Hide();
                radioButton3.Hide();
                radioButton4.Hide();
                radioButton5.Hide();
                radioButton6.Hide();
                label35.Show();
                this.btnopslaan.Hide();
                this.btnwijzig.Show();

                label21.Show();
                label13.Show();
                label14.Show();
                label15.Show();
                label16.Show();
                label17.Show();
                linkLabel2.Show();
                label22.Show();
                label24.Show();
                label26.Show();
                label28.Show();
                label30.Show();
                NavigateRecord();
                Organize_panels();
            }
        }
        private void do_edit()
        {
            using (Panel tmp_p = new Panel())
            {
                tmp_p.Location = panel3.Location;
                tmp_p.Size = panel3.Size;
                tmp_p.BackColor = System.Drawing.SystemColors.ControlLightLight;
                tmp_p.BorderStyle = BorderStyle.Fixed3D;
                this.Controls.Add(tmp_p);
                tmp_p.Show();
                tmp_p.BringToFront();
                show_all_containers(this);
                splitContainer20.Hide();
                if (!elementHost1.Visible || !elementHost2.Visible || !elementHost3.Visible)
                {
                    button9.Show();
                }
                /// moet in bgw thread///
                telefoonnummer_user_control1.Api_get_areacode(Rowlink.postcode);
                telefoonnummer_user_control2.Api_get_areacode(Rowlink.postcode);
                telefoonnummer_user_control3.Api_get_areacode(Rowlink.postcode);
                ////////////////////////
                radioButton5.Show();
                radioButton6.Show();
                label35.Hide();
                mcb1.Show();
                textBox1.Show();
                textBox2.Show();
                textBox14.Show();
                textBox15.Show();
                label39.Hide();
                label12.Show();
                label31.Show();
                label32.Show();
                label33.Show();
                radioButton1.Show();
                radioButton2.Show();
                radioButton3.Show();
                radioButton4.Show();
                button1.Show();

                splitContainer17.Hide();
                //splitContainer2.Show();
                //splitContainer3.Show();
                //splitContainer4.Show();
                //splitContainer14.Show();
                //splitContainer6.Show();
                //splitContainer5.Show();
                // splitContainer7.Location = new Point(splitContainer7.Location.X, splitContainer7.Location.Y + (5 * 25));
                // splitContainer13.Location = new Point(splitContainer13.Location.X, splitContainer13.Location.Y + (5 * 25));
                splitContainer12.Location = new Point(splitContainer12.Location.X, splitContainer12.Location.Y + (5 * 25));
                splitContainer11.Location = new Point(splitContainer11.Location.X, splitContainer11.Location.Y + (5 * 25));
                splitContainer16.Location = new Point(splitContainer16.Location.X, splitContainer16.Location.Y + (5 * 25));
                splitContainer15.Location = new Point(splitContainer15.Location.X, splitContainer15.Location.Y + (5 * 25));
                //elementHost1.Location = new Point(elementHost1.Location.X, elementHost1.Location.Y + (5 * 33));
                //elementHost2.Location = new Point(elementHost2.Location.X, elementHost2.Location.Y + (5 * 33));
                //elementHost3.Location = new Point(elementHost3.Location.X, elementHost3.Location.Y + (5 * 33));
                //  p_contact_gegevens.Height = p_contact_gegevens.Height + 125;
                //  p_telnrs.Location = new Point(p_telnrs.Location.X, p_telnrs.Location.Y + 125);
                this.textBox3.Show();
                this.textBox4.Show();
                if (radioButton6.Checked)
                {
                    this.textBox5.Show();
                }
                this.textBox6.Show();
                // this.textBox7.Show();
                //this.textBox8.Show();
                //this.textBox9.Show();
                this.textBox10.Show();
                this.textBox11.Show();
                this.textBox12.Show();
                this.textBox13.Show();
                dateTimePicker1.Show();
                this.btnwijzig.Hide();
                this.btnopslaan.Show();
                label21.Hide();
                label13.Hide();
                label14.Hide();
                label15.Hide();
                label16.Hide();
                label17.Hide();
                //label18.Hide();
                //label19.Hide();
                //label20.Hide();
                linkLabel2.Hide();
                label22.Hide();
                label24.Hide();
                label26.Hide();
                label28.Hide();
                label30.Hide();

                //// hide copy2clip
                // splitContainer20.Hide();
                button6.Enabled = false;
                button7.Enabled = false;
                button8.Enabled = false;
                Organize_panels();
            }
        }
        private void btnterug_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            antw = Is_ingebruik(int.Parse(id), 3);
            if (antw.Length > 1 && antw == Global.username)
            {
                if (!Verwijder_ingebruik(int.Parse(id), 3))
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
                editing = false; this.wijzigstand = false;
               
                no_edit();
                btn_verwijder.Hide();
                //NavigateRecord();
                telefoonnummer_user_control1.Annuleer();
                telefoonnummer_user_control2.Annuleer();
                telefoonnummer_user_control3.Annuleer();
                
                telefoonnummer_user_control1.Wijzigstand(false);
                telefoonnummer_user_control2.Wijzigstand(false);
                telefoonnummer_user_control3.Wijzigstand(false);

                switch (enum_nr)
                {
                    case 0://normaal
                        if (!van_zoek)
                        {
                            btnterug.Image = null;
                            btnterug.Image = Properties.Resources.AlleContacten;
                        }
                        else
                        {
                            btnterug.Image = null;
                            btnterug.Image = Properties.Resources.ZoekContact2;
                        }
                        break;
                    case 1://nieuw clean
                        
                        break;
                    case 2://nieuw own
                        
                        break;
                    case 3://norm-sub
                        btnterug.Image = null;
                        btnterug.Image = Properties.Resources.InfoProject2;
                        break;
                }
                Cursor.Current = Cursors.Default;
                panel3.Focus();
            }
            else
            {
                if (start_parent is overview1)
                {
                   (start_parent as overview1).refresh_zoek();
                }
                    start_parent.herlaad();

                    Cursor.Current = Cursors.Default;
                    this.sluit();
                Close();
            }
        }
        private void btnwijzig_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            antw = Is_ingebruik(int.Parse(id), 3);
            bezetnaam = antw;
            if (antw.Length > 1)
            {
                MessageBox.Show("Het wijzigen van het contact "+ vrijgeefstring+ " is momenteel niet mogelijk.\n Dit formulier is reeds in gebruik door "+ antw + ".\n U ontvangt een melding zodra dit formulier wordt vrijgegeven.");
                btnwijzig.Enabled = false;
                INUSE_BackgroundWorker.RunWorkerAsync();
            }
            else
            {
                if (Neem_ingebruik(int.Parse(id), 3, Global.username))
                {
                    editing = true; this.wijzigstand = true;
                    NavigateRecord();
                    do_edit();

                    if (enum_nr != 2 && enum_nr != 3)
                    {
                        btn_verwijder.Show();
                    }
                    
                    telefoonnummer_user_control1.Wijzigstand(true);
                    telefoonnummer_user_control2.Wijzigstand(true);
                    telefoonnummer_user_control3.Wijzigstand(true);
                    btnterug.Image = null;
                    btnterug.Image = Properties.Resources.Annuleer2;

                }
                else
                {

                    MessageBox.Show("reservering failed");
                }
            }
            Cursor.Current = Cursors.Default;
            panel3.Focus();
        }
        private bool VerplichtCheck()
        {
            bool compleet = true;
            if (mcb1.SelectedIndex < 0)
            {
                compleet = false;
                mcb1.BackColor = Color.Crimson;
                mcb1.ForeColor = Color.White;
            }
            else
            {
                mcb1.BackColor = Color.White;
                mcb1.ForeColor = Color.Black;
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
            if (textBox4.TextLength < 1)
            {
                compleet = false;
                textBox4.BackColor = Color.Crimson;
                textBox4.ForeColor = Color.White;
            }
            else
            {
                textBox4.BackColor = Color.White;
                textBox4.ForeColor = Color.Black;
            }

            if (textBox6.TextLength < 1)
            {
                compleet = false;
                textBox6.BackColor = Color.Crimson;
                textBox6.ForeColor = Color.White;
            }
            else
            {
                textBox6.BackColor = Color.White;
                textBox6.ForeColor = Color.Black;
            }


            return compleet;
        }
        private void Organize_panels()
        {
            Point scrollpos = panel3.AutoScrollPosition;
            panel3.AutoScrollPosition = new Point(0, 0);
        //    panel3.AutoScroll = false;
            /*
             * #1 p_contact_gegef
             * #2 p_telnrs
             * #3 p_vrijedagen
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

                    int p_x = panel3.Width / 2 - 216;
                    p_contact_gegevens.Location = new Point(p_x, 10);
                    p_contact_gegevens.Size = bereken_afmeting(p_contact_gegevens);
                    p_telnrs.Location = new Point(p_x, p_contact_gegevens.Size.Height + p_contact_gegevens.Location.Y + 8);
                    p_telnrs.Size = bereken_afmeting(p_telnrs);
                    p_vrijedagen.Location = new Point(p_x, p_telnrs.Size.Height + p_telnrs.Location.Y + 8);
                    p_vrijedagen.Size = bereken_afmeting(p_vrijedagen);
                }
               
            }
            scrollpos.X *= -1;
            scrollpos.Y *= -1;
            panel3.AutoScrollPosition = scrollpos;
        }
        private Size bereken_afmeting(Panel P)
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
                    if (SC is System.Windows.Forms.Integration.ElementHost)
                    {
                        SC.Width += 4;
                        SC.Width -= 4;
                    }
                }
                else if (SC is Button && P == p_telnrs)
                {
                    if (SC.Visible && n_showed < 3)
                    {
                        n_showed++;
                        tmp_H += SC.Height;
                        SC.Location = new Point(347,25*n_showed+5);
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
        
        private void btnopslaan_Click(object sender, EventArgs e)
        {
            bool fout_in_nummer = false;
            Cursor.Current = Cursors.WaitCursor;
            string old_name ="";
            if (!VerplichtCheck())
            {
                MessageBox.Show("Niet alle verplichte velden zijn ingevuld.");
                Cursor.Current = Cursors.Default;
            }
            else
            {
                //bedrijf_nr
                bool close_this = false;
                try
                { // bedrijf
                    if (mcb1.SelectedIndex != -1)
                    {
                        int bdrnr_new = int.Parse(mcb1.SelectedValue.ToString());
                        if (bdrnr != bdrnr_new)
                        {
                            //persoonsformdatasetTableAdapters.QueriesTableAdapter qtadapt = new persoonsformdatasetTableAdapters.QueriesTableAdapter();
                            adapter.add_werkgevers_wijziging(int.Parse(id), bdrnr, bdrnr_new, DateTime.Now);
                            
                            //qtadapt.add_werkgevers_wijziging(int.Parse(id), bdrnr, bdrnr_new, DateTime.Now);
                        }
                        Rowpersoon.bedrijf_nr = bdrnr_new;

                    }
                    //man/vrouw

                    if (radioButton1.Checked)
                    {
                        Rowpersoon.man = false;
                    }
                    else
                    {
                        Rowpersoon.man = true;
                    }
                    //Store old Name
                    try
                    {
                        old_name = "";
                          
                            if (!Rowpersoon.IsachternaamNull())
                            {
                                old_name = old_name.TrimStart(' ');
                                old_name += Rowpersoon.achternaam;
                            }
                            if (!Rowpersoon.IsvoornaamNull())
                            {
                                old_name = old_name.TrimStart(' ');
                                old_name += ","+Rowpersoon.voornaam;
                            }
                            if (!Rowpersoon.IstussenvoegselNull())
                            {
                                old_name = old_name.TrimStart(' ');
                                old_name += ","+Rowpersoon.tussenvoegsel;
                            }
                        
                    }
                    catch (Exception ename)
                    {
                        MessageBox.Show("Get old name failed: "+ename.Message);
                    }
                    //voorletters
                    if (textBox3.Text != "")
                    {
                        Rowpersoon.voorletters = textBox3.Text.Trim();
                    }
                    else { Rowpersoon.voorletters = null; }
                    //voornaam
                    if (textBox4.Text != "")
                    {
                        Rowpersoon.voornaam = textBox4.Text.Trim();
                    }
                    else { Rowpersoon.voornaam = null; }
                    //tussenvoegsel
                    if (radioButton6.Checked && textBox5.Text != "")
                    {
                        Rowpersoon.tussenvoegsel = textBox5.Text.Trim();
                    }
                    else { Rowpersoon.tussenvoegsel = null; }
                    //achternaam
                    if (textBox6.Text != "")
                    {
                        Rowpersoon.achternaam = textBox6.Text.Trim();
                    }
                    else { Rowpersoon.achternaam = null; }
                    KeyValuePair<string, bool> nummer_antw;
                    KeyValuePair<string, bool> nummer_antw2;
                    KeyValuePair<string, bool> nummer_antw3;
                    nummer_antw = telefoonnummer_user_control1.Get_Nummer();
                    if (nummer_antw.Value == true)
                    {
                        Rowpersoon.telefoon_nr_1 = nummer_antw.Key;
                    }
                    else
                    {
                        fout_in_nummer = true;
                        goto NUMBERFAULT;
                    }
                    nummer_antw2 = telefoonnummer_user_control2.Get_Nummer();
                    if (nummer_antw2.Value == true)
                    {
                        Rowpersoon.telefoon_nr_2 = nummer_antw2.Key;
                    }
                    else
                    {
                        fout_in_nummer = true;
                        goto NUMBERFAULT;
                    }
                    nummer_antw3 = telefoonnummer_user_control3.Get_Nummer();
                    if (nummer_antw3.Value == true)
                    {
                        Rowpersoon.telefoon_nr_3 = nummer_antw3.Key;
                    }
                    else
                    {
                        fout_in_nummer = true;
                        goto NUMBERFAULT;
                    }
                    Rowpersoon.telefoon_nr_settings = telefoonnummer_user_control1.type.ToString() + "," + telefoonnummer_user_control2.type.ToString() + "," + telefoonnummer_user_control3.type.ToString();
                    ///////////////////////////////
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
                                    Rowpersoon.zatelefoonvast = nummer_antw.Key;
                                    if (nummer_antw.Key != null)
                                    {
                                        tel_vast_done = true;
                                    }
                                }
                                break;
                            case 1://mobiel
                                if (!tel_mob_done)
                                {
                                    Rowpersoon.zamobiel = nummer_antw.Key;
                                    if (nummer_antw.Key != null)
                                    {
                                        tel_mob_done = true;
                                    }
                                }
                                break;
                            case 2://fax
                                if (!tel_fax_done)
                                {
                                    Rowpersoon.zafax = nummer_antw.Key;
                                    if (nummer_antw.Key != null)
                                    {
                                        tel_fax_done = true;
                                    }
                                }
                                break;
                            case 3://voip
                                if (!tel_vast_done)
                                {
                                    Rowpersoon.zatelefoonvast = nummer_antw.Key;
                                    if (nummer_antw.Key != null)
                                    {
                                        tel_vast_done = true;
                                    }
                                }
                                break;
                            case 4://skype
                                if (!tel_vast_done)
                                {
                                    Rowpersoon.zatelefoonvast = nummer_antw.Key;
                                    if (nummer_antw.Key != null)
                                    {
                                        tel_vast_done = true;
                                    }
                                }
                                break;
                            case 5://bedrijfsnummer
                                if (!tel_vast_done)
                                {
                                    Rowpersoon.zatelefoonvast = nummer_antw.Key;
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
                                    Rowpersoon.zatelefoonvast = nummer_antw2.Key;
                                    if (nummer_antw2.Key != null)
                                    {
                                        tel_vast_done = true;
                                    }
                                }
                                break;
                            case 1://mobiel
                                if (!tel_mob_done)
                                {
                                    Rowpersoon.zamobiel = nummer_antw2.Key;
                                    if (nummer_antw2.Key != null)
                                    {
                                        tel_mob_done = true;
                                    }
                                }
                                break;
                            case 2://fax
                                if (!tel_fax_done)
                                {
                                    Rowpersoon.zafax = nummer_antw2.Key;
                                    if (nummer_antw2.Key != null)
                                    {
                                        tel_fax_done = true;
                                    }
                                }
                                break;
                            case 3://voip
                                if (!tel_vast_done)
                                {
                                    Rowpersoon.zatelefoonvast = nummer_antw2.Key;
                                    if (nummer_antw2.Key != null)
                                    {
                                        tel_vast_done = true;
                                    }
                                }
                                break;
                            case 4://skype
                                if (!tel_vast_done)
                                {
                                    Rowpersoon.zatelefoonvast = nummer_antw2.Key;
                                    if (nummer_antw2.Key != null)
                                    {
                                        tel_vast_done = true;
                                    }
                                }
                                break;
                            case 5://bedrijfsnummer
                                if (!tel_vast_done)
                                {
                                    Rowpersoon.zatelefoonvast = nummer_antw2.Key;
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
                                    Rowpersoon.zatelefoonvast = nummer_antw3.Key;
                                    if (nummer_antw3.Key != null)
                                    {
                                        tel_vast_done = true;
                                    }
                                }
                                break;
                            case 1://mobiel
                                if (!tel_mob_done)
                                {
                                    Rowpersoon.zamobiel = nummer_antw3.Key;
                                    if (nummer_antw3.Key != null)
                                    {
                                        tel_mob_done = true;
                                    }
                                }
                                break;
                            case 2://fax
                                if (!tel_fax_done)
                                {
                                    Rowpersoon.zafax = nummer_antw3.Key;
                                    if (nummer_antw3.Key != null)
                                    {
                                        tel_fax_done = true;
                                    }
                                }
                                break;
                            case 3://voip
                                if (!tel_vast_done)
                                {
                                    Rowpersoon.zatelefoonvast = nummer_antw3.Key;
                                    if (nummer_antw3.Key != null)
                                    {
                                        tel_vast_done = true;
                                    }
                                }
                                break;
                            case 4://skype
                                if (!tel_vast_done)
                                {
                                    Rowpersoon.zatelefoonvast = nummer_antw3.Key;
                                    if (nummer_antw3.Key != null)
                                    {
                                        tel_vast_done = true;
                                    }
                                }
                                break;
                            case 5://bedrijfsnummer
                                if (!tel_vast_done)
                                {
                                    Rowpersoon.zatelefoonvast = nummer_antw3.Key;
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
                        Rowpersoon.SetzatelefoonvastNull();
                    }
                    if (!tel_mob_done)
                    {
                        Rowpersoon.SetzamobielNull();
                    }
                    if (!tel_fax_done)
                    {
                        Rowpersoon.SetzafaxNull();
                    }
                    //////////////////////////////
                    //email
                    if (textBox10.Text != "")
                    {
                        Rowpersoon.zaemail = textBox10.Text;
                    }
                    else { Rowpersoon.zaemail = null; }
                    //geboortedatum

                    if (date && dateTimePicker1.Value != DateTimePicker.MinimumDateTime)
                    {

                        Rowpersoon.geboortedatum1 = dateTimePicker1.Value;
                        date = false;
                    }
                    else if (dateTimePicker1.Value == DateTimePicker.MinimumDateTime)
                    {
                        Rowpersoon.Setgeboortedatum1Null();
                    }

                    //vaste vrijedag
                    if (textBox11.Text != "")
                    {
                        Rowpersoon.vastevrijedag1 = textBox11.Text;
                    }
                    else { Rowpersoon.vastevrijedag1 = null; }
                    //vastevrijedag 2
                    if (textBox12.Text != "")
                    {
                        Rowpersoon.vastevrijedag2 = textBox12.Text;
                    }
                    else { Rowpersoon.vastevrijedag2 = null; }
                    //vastevrijedag 3
                    if (textBox13.Text != "")
                    {
                        Rowpersoon.vastevrijedag3 = textBox13.Text;
                    }
                    else { Rowpersoon.vastevrijedag3 = null; }

                    //actief/niet actief

                    if (radioButton3.Checked)
                    {
                        Rowpersoon.NIETactief = false;
                    }
                    else
                    {
                        Rowpersoon.NIETactief = true;
                    }
                    verwijder_panel1.Save_function(textBox15.Text);
                }
                catch (Exception e1)
                {
                    MessageBox.Show("opslaan mislukt, controleer alle waardes.");
                    String log_line = "crash program @ " + DateTime.Now.ToString() + "error: " + e1;
                    System.IO.StreamWriter file = new System.IO.StreamWriter(Global.log_file_path, true);
                    file.WriteLine(log_line);
                    file.Close();
                    Cursor.Current = Cursors.Default;
                    return;
                }

                try
                {
                    adapter.Update(Rowpersoon);
                    //call service to set new data in exchange
                    string new_name = Rowpersoon.voornaam;
                    string email = "";
                    if(!Rowpersoon.IstussenvoegselNull())
                    {
                        new_name += " " + Rowpersoon.tussenvoegsel;
                    }
                    if(!Rowpersoon.IsachternaamNull())
                    {
                        new_name += " " + Rowpersoon.achternaam;
                    }
                    if (!Rowpersoon.IszaemailNull())
                    {
                        email = Rowpersoon.zaemail;
                    }
                    try
                    {

                        var service = ExchangeComs.EWSFunctions.GetNewServiceHook();
                        var namesplit = old_name.Split(',');
                        var old_achternaam = "";
                        var old_voornaam = "";
                        var old_tussenvoegsel = "";
                        if(namesplit != null)
                        {
                            if(namesplit.Length > 0 && namesplit[0] != null)
                            {
                                old_achternaam = namesplit[0];
                            }
                            if (namesplit.Length > 1 && namesplit[1] != null)
                            {
                                old_voornaam = namesplit[1];
                            }
                            if (namesplit.Length > 2 && namesplit[2] != null)
                            {
                                old_tussenvoegsel = namesplit[2];
                            }
                        }
                        
                        var contactId = ExchangeComs.EWSFunctions.FindContact(service,old_achternaam,old_voornaam,old_tussenvoegsel );//!!! check making of oldname!!!
                        
                        var contactinfo = new ExchangeComs.ExchangeContactItem();
                        contactinfo.Voornaam = Rowpersoon.IsvoornaamNull() == false ?  Rowpersoon.voornaam.Trim() : "";
                        contactinfo.Tussenvoegsel = Rowpersoon.IstussenvoegselNull() == false ? Rowpersoon.tussenvoegsel.Trim() : "";
                        contactinfo.Achternaam = Rowpersoon.IsachternaamNull() == false ? Rowpersoon.achternaam.Trim() : "";
                        contactinfo.BedrijfNaam = Rowpersoon.Isbedrijf_nrNull() == false ?  Rowlink.zoeknaam.Trim() : "";
                        contactinfo.Email1 = Rowpersoon.IszaemailNull() == false ? Rowpersoon.zaemail.Trim() : "" ; 
                        contactinfo.TelNr1 = Rowpersoon.Istelefoon_nr_1Null() == false ? Rowpersoon.telefoon_nr_1.Trim() : "";
                        contactinfo.TelNr2 = Rowpersoon.Istelefoon_nr_2Null() == false ? Rowpersoon.telefoon_nr_2.Trim() : "";
                        contactinfo.TelNr3 = Rowpersoon.Istelefoon_nr_3Null() == false ? Rowpersoon.telefoon_nr_3.Trim() : "";
                        contactinfo.TelNrTypes = Rowpersoon.Istelefoon_nr_settingsNull() == false ? Rowpersoon.telefoon_nr_settings : "";
                        if (contactId.ToString() != "0")
                        {
                            ExchangeComs.EWSFunctions.UpdateContact(service,contactId, contactinfo);
                        }
                        else 
                        {
                            ExchangeComs.EWSFunctions.MakeNewContact(service, contactinfo);
                        }
                        //var ci = new ContactInfo();
                        //ci.NewName = new_name;
                        //ci.OldName = old_name;
                        //ci.Email = email;
                       
                       // Global.ExService.ModifyContact(ci);
                    }
                    catch (Exception srvex)
                    {
                        MessageBox.Show("Service returnd exeption: " + srvex.Message);
                    }
                }
                catch (Exception e4)
                {
                    MessageBox.Show("opslaan mislukt, controleer netwerkverbinding");
                    String log_line = "crash program @ " + DateTime.Now.ToString() + "error in persoonsform: " + e4;
                    System.IO.StreamWriter file = new System.IO.StreamWriter(Global.log_file_path, true);
                    file.WriteLine(log_line);
                    file.Close();
                    Cursor.Current = Cursors.Default;
                    return;
                }

                Cursor.Current = Cursors.Default;
                if (editing == true)
                {

                    editing = false; this.wijzigstand = false;
                    telefoonnummer_user_control1.Wijzigstand(false);
                    telefoonnummer_user_control2.Wijzigstand(false);
                    telefoonnummer_user_control3.Wijzigstand(false);
                    no_edit();
                    switch (enum_nr)
                    {
                        case 0://normaal
                            handelingen_logger.log_handeling(int.Parse(id), 3, 26);
                            if (!van_zoek)
                            {
                                btnterug.Image = null;
                                btnterug.Image = Properties.Resources.AlleContacten;
                            }
                            else
                            {
                                btnterug.Image = null;
                                btnterug.Image = Properties.Resources.ZoekContact2;
                            }
                            break;
                        case 1://nieuw clean
                            enum_nr = 0;
                            if (van_zoek)
                            {
                                btnterug.Image = null;
                                btnterug.Image = Properties.Resources.ZoekContact2;
                            }
                            else
                            {
                                btnterug.Image = null;
                                btnterug.Image = Properties.Resources.AlleContacten;
                            }
                            lbl_top.Text = "";
                            lbl_mid.Text = "Info contact";
                            break;
                        case 2://nieuw own
                            close_this = true;
                            Global.return_id = id;
                            (close_parent as project_form).fill_id(close_this);
                            break;
                        case 3://norm-sub
                            btnterug.Image = null;
                            btnterug.Image = Properties.Resources.InfoProject2;
                            Global.return_id = id;
                            (close_parent as project_form).fill_id(close_this);
                            handelingen_logger.log_handeling(int.Parse(id), 1, 11);
                            break;
                        
                    }
                    btnterug.Show();
                    btn_verwijder.Hide();
                    antw = Is_ingebruik(int.Parse(id), 3);
                    if (antw.Length > 1 && antw == Global.username)
                    {
                        if (Verwijder_ingebruik(int.Parse(id), 3))
                        {
                        }
                        else
                        {
                            MessageBox.Show("fout bij vrijgeven opslaan " + antw);

                        }
                    }
                }
                if (close_this)
                {
                    this.Close();
                }
                this.persoon_form_Resize(this, e);
                //NavigateRecord();
               
            }
        NUMBERFAULT: if (fout_in_nummer)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show("Een telefoonnummer is fout ingevoerd. \nGooi leeg met het kruisje of pas het nummer aan.");
            }
        Cursor.Current = Cursors.Default;
            panel3.Focus();
        }
        public void fill_id(bool n)
        {
            string a = Global.return_id;
            Global.return_id = "";
            bdrnr = int.Parse(a);
                //NavigateRecord();
                bdradapter.Fill(bedrijftable);
            
                mcb1.SelectedValue = int.Parse(a);


                label21.Text = mcb1.Text;
                if (n)
                {
                    this.Show();
                }
           
            lbl_mid.Focus();
        }
        public override void save()
        {
            btnopslaan.PerformClick();
        }
        private void button1_Click(object sender, EventArgs e)//drop bedrijf
        {
            Cursor.Current = Cursors.WaitCursor;
            string tmpnaam = textBox4.Text + " " + textBox5.Text + " " + textBox6.Text;
            if (this.WindowState == FormWindowState.Normal)
            {
                Global.size = this.Size;
                Global.position = this.Location;
           }
           Global.windowstate = this.WindowState;
            if (wijzigstand)
            {
                
                Global.overzicht_type = 2;
                Global.give_return = true;
                PFH.Start_route(2, 3, this, this, int.Parse(id), "de werkgever ", tmpnaam);
                this.Hide();
                Global.overzicht_type = 0;
            }
            else
            {
                if (mcb1.SelectedIndex != -1)
                {

                    string temp = bdrnr.ToString();
                    Global.overzicht_select = temp;
                    PFH.Start_route(2, 2, this, this, int.Parse(id), "de werkgever ", tmpnaam);
                    this.Hide();
                }
            }
            Cursor.Current = Cursors.Default;
        }
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {



            if (!from_tb)
            {
                if (dateTimePicker1.Value != DateTimePicker.MinimumDateTime)
                {
                    date = true; changed = true;
                    label22.Text = dateTimePicker1.Value.ToLongDateString();
                    textBox14.Text = dateTimePicker1.Value.Day.ToString();
                    textBox2.Text = dateTimePicker1.Value.Month.ToString();
                    textBox1.Text = dateTimePicker1.Value.Year.ToString();
                }
            }
            else
            {
                from_tb = false;
            }
        }
        private void persoon_form_FormClosing(object sender, FormClosingEventArgs e)
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
        private void persoon_form_Resize(object sender, EventArgs e)
        {   Point pt1 = new Point(panel2.Width / 2 - lbl_top.Width / 2, 5);
            lbl_top.Location = pt1;
            Point pt2 = new Point(panel2.Width / 2 - lbl_mid.Width / 2, 28);
            lbl_mid.Location = pt2;
            Point pt3 = new Point(panel2.Width / 2 - lbl_bot.Width / 2, panel2.Height -18);
            lbl_bot.Location = pt3;

            Point PMid = new Point(panel2.Width / 2 - (p_contact_gegevens.Width / 2), p_contact_gegevens.Location.Y);
            p_contact_gegevens.Location = PMid;
            PMid.Y = p_telnrs.Location.Y;
            p_telnrs.Location = PMid;
            PMid.Y = p_xfunctie.Location.Y;
            p_xfunctie.Location = PMid;
            PMid.Y = p_verw_verv.Location.Y;
            p_verw_verv.Location = PMid;
            elementHost5.Location = new Point(panel4.Width / 2 - elementHost5.Width / 2, elementHost5.Location.Y);
            p_vorrige_werkgevers.Location = new Point(panel4.Width / 2 - p_vorrige_werkgevers.Width / 2, p_vorrige_werkgevers.Location.Y);
            p_vrijedagen.Location = new Point(panel2.Width / 2 - p_vrijedagen.Width / 2, p_vrijedagen.Location.Y);
            button2.Location = new Point(panel_verwijder.Width / 2 - button2.Width/2, button2.Location.Y);
            button3.Location = new Point((panel_verwijder.Width / 2 -button2.Width/2) - button3.Width, button3.Location.Y);
            button4.Location = new Point((panel_verwijder.Width / 2 +button4.Width/2), button4.Location.Y);

            label43.Location = new Point(panel4.Width / 2 - label43.Width / 2, label43.Location.Y);

        }
        private void Clear_lbl()
        {
            label21.Text = "-";
            label37.Text = "-";
            label14.Text = "-";
            label15.Text = "-";
            label35.Text = "-";
            label16.Text = "-";
            label17.Text = "-";
            label22.Text = "-";
            label24.Text = "-";
            label26.Text = "-";
            label28.Text = "-";
            label30.Text = "-";

        }
        private void Clear_text(Control parent)
        {
            Clear_lbl();
            foreach (Control c in parent.Controls)
            {
                if (c is TextBox)
                {
                    (c as TextBox).Clear();
                }
                if (c.Controls.Count > 0) Clear_text(c);
            }
        }
        private void splitContainer13_Panel2_Leave(object sender, EventArgs e)
        {
            if (textBox1.Text == "" && textBox2.Text == "" && textBox14.Text == "")
            {
                date = false;
                dateTimePicker1.Value = DateTimePicker.MinimumDateTime;
            }
            else
            {
                if (textBox14.Text == "")
                {
                    MessageBox.Show("Geef ook een waarde voor dag op.");
                    textBox14.Focus();
                    return;
                }
                else if (textBox2.Text == "")
                {
                    MessageBox.Show("Geef ook een waarde voor maand op.");
                    textBox2.Focus();
                    return;
                }
                else if (textBox1.Text == "")
                {
                    MessageBox.Show("Geef ook een waarde voor jaar op.");
                    textBox1.Focus();
                    return;
                }

                string date = "";
                if (textBox1.TextLength == 4)
                { date = textBox1.Text + "/" + textBox2.Text + "/" + textBox14.Text; }
                else if (textBox1.TextLength == 3)
                { date = "1" + textBox1.Text + "/" + textBox2.Text + "/" + textBox14.Text; }
                else if (textBox1.TextLength == 2)
                { date = "19" + textBox1.Text + "/" + textBox2.Text + "/" + textBox14.Text; }
                else if (textBox1.TextLength == 1)
                { date = "190" + textBox1.Text + "/" + textBox2.Text + "/" + textBox14.Text; }
                DateTime testdt;
                if (DateTime.TryParse(date, out testdt))
                {
                    dateTimePicker1.Value = testdt;
                    textBox14.Text = dateTimePicker1.Value.Day.ToString();
                    textBox2.Text = dateTimePicker1.Value.Month.ToString();
                    textBox1.Text = dateTimePicker1.Value.Year.ToString();

                }
                else
                {
                    MessageBox.Show("ongeldige datum");
                    textBox14.Focus();
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
        private void label33_Click(object sender, EventArgs e)
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
        private void dateTimePicker1_MouseEnter(object sender, EventArgs e)
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
        bool from_tb = false;
        private void dateTimePicker1_CloseUp(object sender, EventArgs e)
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
        private void splitContainer1_Panel2_Leave(object sender, EventArgs e)
        {
            Panel tmpp = (Panel)sender;
            foreach (Control c in tmpp.Controls)
            {
                if (c is MultiColumnComboBoxDemo.MultiColumnComboBox)
                {
                    if ((c as MultiColumnComboBoxDemo.MultiColumnComboBox).SelectedIndex < 0)
                    {
                        MessageBox.Show("Vul waarde in, dit is een verplicht veld");
                        c.Focus();
                    }
                }
                else if (c is TextBox)
                {
                    if ((c as TextBox) != textBox5)
                    {
                        if ((c as TextBox).TextLength < 1)
                        {
                            MessageBox.Show("Vul waarde in, dit is een verplicht veld");

                            c.Focus();
                        }
                    }
                    else if (radioButton5.Checked)
                    {
                    }
                    else if ((c as TextBox).TextLength < 1)
                    {
                        DialogResult dr = MessageBox.Show("Vul waarde in, dit is een verplicht veld. Wilt u geen tussenvoegsel instellen?", "Tussenvoegsel invullen?", MessageBoxButtons.YesNo);
                        if (dr == DialogResult.Yes)
                        {
                            radioButton5.Checked = true; radioButton6.Checked = false;
                        }
                        else
                        {
                            c.Focus();
                        }
                    }
                }
                else if (c is RadioButton)
                {
                    if (!radioButton1.Checked && !radioButton2.Checked)
                    {
                        MessageBox.Show("Vul waarde in, dit is een verplicht veld");
                        c.Focus();
                    }
                    if (!radioButton5.Checked && !radioButton6.Checked)
                    {
                        MessageBox.Show("Vul waarde in, dit is een verplicht veld");
                        c.Focus();
                    }
                    if (!radioButton3.Checked && !radioButton4.Checked)
                    {
                        MessageBox.Show("Vul waarde in, dit is een verplicht veld");
                        c.Focus();
                    }
                }
            }
        }
        private void radioButton5n6_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton5.Checked)
            {
                textBox5.Hide(); textBox5.Clear();
            }
            else if (radioButton6.Checked)
            {
                textBox5.Show();
            }
        }
        string bezetnaam = "";
        private void INUSE_BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
             string result = Is_ingebruik(int.Parse(id),3);
            while (result.Length > 1)
            {
                result = Is_ingebruik(int.Parse(id), 3);
                Thread.Sleep(5000);

            }
            if (result.Length < 1)
            {
                INUSE_BackgroundWorker.ReportProgress(1);
            }
        }
        private void INUSE_BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            MessageBox.Show("Het contact " + vrijgeefstring + " is zojuist vrijgegeven door " + bezetnaam + ".\n U kunt dit formulier nu wijzigen.");
        }
        private void INUSE_BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
           
            btnwijzig.Enabled = true;
        }
        private void textBoxNumOnlyTextChanged(object sender, EventArgs e)
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
        private void Lock_screen()
        {
            btnopslaan.Enabled = false;
            btnwijzig.Enabled = false;
            btnterug.Enabled = false;
            button2.Hide();
            button3.Hide();
            panel1.BringToFront();
            if (Global.UserLevel < 3)
            {
              //  groupBox4.Show(); 
                multiColumnComboBox1.Hide();
            }
            else
            {
                p_verw_verv.Hide();
            }
            panel1.Focus();

        }
        private void Free_screen()
        {
            btnopslaan.Enabled = true;
            btnwijzig.Enabled = true;
            btnterug.Enabled = true;
            btn_verwijder_bevestig.Enabled = true;
            button2.Show();
            button3.Show();
            panel3.BringToFront();
            p_verw_verv.Hide();
            vervang_lbl.Text = "Dit contact verwijderen en vervangen?";
            button5.Text = "Ja";
         //5   button6.Text = "Nee";
            multiColumnComboBox1.Enabled = true;
            panel3.Focus();

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
            vervang_lbl.Text = "Dit contact verwijderen en vervangen?";
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
            int pid = int.Parse(id);
            int result = 0;
            string column = "";
            string table = "";
            //persoonsformdatasetTableAdapters.QueriesTableAdapter qtadapt = new persoonsformdatasetTableAdapters.QueriesTableAdapter();
            
            persoonsformdatasetTableAdapters.persoon_nr_locatiesTableAdapter locaties_adapt = new persoonsformdatasetTableAdapters.persoon_nr_locatiesTableAdapter();
            locaties_adapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
            persoonsformdataset.persoon_nr_locatiesDataTable locaties_table = locaties_adapt.GetData();
            foreach (persoonsformdataset.persoon_nr_locatiesRow row in locaties_table.Rows)
            {
                column = row.column.ToString();
                table = row.tabel.ToString();
                result += (int)adapter.Check_del_pers(pid, column, table);
            }
            
            //check in mdr
            var mdrCount = CheckMdrGebruik();
            if (mdrCount > 0)
            {
                MessageBox.Show("Dit contact komt " + mdrCount + " keer voor in de Mandagen Registratie.\n Hierdoor kan dit contact niet worden verwijderd.");
                btn_verwijder_annulleer.PerformClick();
                btnterug.PerformClick();
                //qtadapt.Dispose();
                locaties_adapt.Dispose();
                locaties_table.Dispose();
                return;
            }
            else if (mdrCount == -1)//check failed block delete
            {
                MessageBox.Show("Het gebruik van dit contact in de Mandagen Registratie kan niet worden geverifieerd. \nHierdoor is het verwijderen van contacten niet mogelijk");
                btn_verwijder_annulleer.PerformClick();
                btnterug.PerformClick();
                //qtadapt.Dispose();
                locaties_adapt.Dispose();
                locaties_table.Dispose();
                return;
            }
            //
            if (result > 0)
            {
                if (Global.UserLevel < 3)
                {
                    var DR = MessageBox.Show("Dit contact komt " + result + " keer voor in andere gegevens.\nHierdoor kan dit contact niet worden verwijderd.\nKlik op \"JA\" om dit contact te vervangen door een ander contact.\nKlik op \"NEE\" om terug te gaan naar de lees-stand.", "Verwijderen en vervangen?",MessageBoxButtons.YesNo);
                   if( DR == DialogResult.Yes)
                    {
                        p_verw_verv.Show();
                        btn_verwijder_bevestig.Enabled = false;
                        multiColumnComboBox1.SelectedIndex = -1;
                        multiColumnComboBox1.Show();
                        multiColumnComboBox1.Enabled = true;
                        vervang_lbl.Text = "Kies door welk contact het huidige contact moet worden vervangen.";
                        vervang_lbl.Location = new Point(p_verw_verv.Width / 2 - vervang_lbl.Width / 2, vervang_lbl.Location.Y);
                        vervangt = true;
                        button5.Enabled = false;
                        button5.Text = "Doorgaan";
                    }
                    else
                    {
                        btn_verwijder_annulleer.PerformClick();
                        btnterug.PerformClick();
                    }
                }
                else
                {
                    handelingen_logger.log_handeling(int.Parse(id), 3, 24);
                    MessageBox.Show("Dit contact komt " + result + " keer voor in andere gegevens.\nHierdoor kan dit contact niet worden verwijderd.\nKlik op \"ok\" om terug te gaan naar de lees-stand.");
                    btn_verwijder_annulleer.PerformClick();
                    btnterug.PerformClick();
                }
            }
            else
            {
               // MessageBox.Show("Dit contact komt " + result + " keer voor in andere gegevens.\n\r U kunt dit contact verwijderen.");
                // delete persoon (mb?) 
                //get name to delete for exchange contacts
                 string name2del = "";
                try
                {
                   var conn = new System.Data.SqlClient.SqlConnection();
                    conn.ConnectionString = Global.ConnectionString_fileserver;
                    conn.Open();
                    System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("Select voornaam, tussenvoegsel, achternaam FROM persoon WHERE persoon_nr = '" + pid + "'",conn);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        
                       
                        if (reader[2] != DBNull.Value && reader[2].ToString().Length > 0)
                        {
                            name2del = reader[2].ToString();
                        }
                        if (reader[0] != DBNull.Value && reader[0].ToString().Length > 0)
                        {
                            name2del += ","+ reader[0].ToString();
                        }
                        if (reader[1] != DBNull.Value && reader[1].ToString().Length > 0)
                        {
                            name2del += "," + reader[1].ToString();
                        }
                    }
                }
                catch (Exception exserv)
                {
                    MessageBox.Show("Database encounterd error: " + exserv.Message);
                }

                System.Data.SqlClient.SqlConnection con;
                con = new System.Data.SqlClient.SqlConnection();
                con.ConnectionString = Global.ConnectionString_fileserver;
                con.Open();
                string stopro = "delete_persoon";
                System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(stopro, con);
               System.Data.SqlClient.SqlParameter persoon_nr = command.Parameters.Add("@persoon_nr", SqlDbType.Int);
               persoon_nr.Value = pid;
               persoon_nr.Direction = ParameterDirection.Input;
                command.CommandType = CommandType.StoredProcedure;
                int ok = command.ExecuteNonQuery();
                if (ok > 0)
                {
                    //call exchangeservice to delete contact
                    try
                    {
                        var service = ExchangeComs.EWSFunctions.GetNewServiceHook();
                        var del_achternaam = "";
                        var del_voornaam = "";
                        var del_tussenvoegsel = "";
                        var namesplit = name2del.Split(',');
                        if (namesplit != null)
                        {
                            if (namesplit[0] != null)
                            {
                                del_achternaam = namesplit[0];
                            }
                            if (namesplit[1] != null)
                            {
                                del_voornaam = namesplit[1];
                            }
                            if (namesplit[2] != null)
                            {
                                del_tussenvoegsel = namesplit[2];
                            }
                        }
                        var contactId = ExchangeComs.EWSFunctions.FindContact(service,del_achternaam,del_voornaam,del_tussenvoegsel);
                        ExchangeComs.EWSFunctions.DeleteContact(service, contactId);
                    }
                    catch (Exception exeption)
                    {
                        MessageBox.Show("Service returned error: " + exeption.Message);
                    }
                    if (!vervangt)
                    {
                        MessageBox.Show("succesvol verwijderd");
                        handelingen_logger.log_handeling(int.Parse(id), 3, 21);
                    }
                    Verwijder_ingebruik(pid, 3);
                    this.wijzigstand = false;
                    telefoonnummer_user_control1.Wijzigstand(false);
                    telefoonnummer_user_control2.Wijzigstand(false);
                    telefoonnummer_user_control3.Wijzigstand(false);
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
        private void button2_Click(object sender, EventArgs e)
        {
            button3.Image = zeebregtsCs.Properties.Resources.information_symbol_gray;
            button2.Image = zeebregtsCs.Properties.Resources.sym_function_x_blue;
            button4.Image = zeebregtsCs.Properties.Resources.haxorgray;
            panel1.BringToFront();
            panel1.Focus();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            button3.Image = zeebregtsCs.Properties.Resources.information_symbol;
            button2.Image = zeebregtsCs.Properties.Resources.sym_function_x_blue_gray;
            button4.Image = zeebregtsCs.Properties.Resources.haxorgray;
            panel3.BringToFront();
            panel3.Focus();
        }
        private int CheckMdrGebruik()
        {
            var result = -1;
            var mdrcon = new System.Data.SqlClient.SqlConnection();
            mdrcon.ConnectionString = Global.ConnectionString_Mdr;
            string stopro = "p_CountDependableObjectsVoorContactId";
            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = stopro;
            command.Connection = mdrcon;
            SqlParameter ContactId = command.Parameters.Add("@ContactId", SqlDbType.Int);
            SqlParameter Count = command.Parameters.Add("@Count", SqlDbType.Int);
            ContactId.Value = indexId;
            ContactId.Direction = ParameterDirection.Input;
            Count.Direction = ParameterDirection.ReturnValue;
            try
            {
                mdrcon.Open();
                command.ExecuteNonQuery();
                mdrcon.Close();
                result = (int)Count.Value;
            }
            catch (Exception ex)
            {
                String log_line = "crash program @ " + DateTime.Now.ToString() + "error: " + ex.Message;
                System.IO.StreamWriter file = new System.IO.StreamWriter(Global.log_file_path, true);
                file.WriteLine(log_line);
                file.Close();
            }
            return result;
        }
        
        private void vervang_persoon()
        {
            int id_oud = int.Parse(id);
            int id_new =int.Parse( multiColumnComboBox1.SelectedValue.ToString());
            bool result = true;
            string column = "";
            string table = "";
           

            /////////
            //persoonsformdatasetTableAdapters.QueriesTableAdapter qtadapt = new persoonsformdatasetTableAdapters.QueriesTableAdapter();
            persoonsformdatasetTableAdapters.persoon_nr_locatiesTableAdapter locaties_adapt = new persoonsformdatasetTableAdapters.persoon_nr_locatiesTableAdapter();
            locaties_adapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
            persoonsformdataset.persoon_nr_locatiesDataTable locaties_table = locaties_adapt.GetData();
            persoonsformdataset.persoonDataTable PDT = new persoonsformdataset.persoonDataTable();
            adapter.Fill(PDT);
            var res_oud = from p in PDT where p.persoon_nr == id_oud select p.bedrijf_nr;
            var res_new = from p2 in PDT where p2.persoon_nr == id_new select p2.bedrijf_nr;
            int bdr_oud = 0;
            int bdr_new= 0;
            foreach (int bdr_nr in res_oud)
            {
                bdr_oud = bdr_nr;
            }
            foreach (int bdr_nr in res_new)
            {
                bdr_new = bdr_nr;
            }
            if (bdr_new != bdr_oud)
            {
                adapter.add_werkgevers_wijziging(id_new, bdr_oud, bdr_new, DateTime.Now);
                adapter.Update_vervangen_werkgevers_log(id_new, id_oud);
            }
            foreach (persoonsformdataset.persoon_nr_locatiesRow row in locaties_table.Rows)
            {
                column = row.column.ToString();
                table = row.tabel.ToString();
                try
                {
                    adapter.vervang_del_pers(id_oud, column, table, id_new);
                    verwijder_panel1.start_init(3, id_oud);
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
                handelingen_logger.log_handeling(int.Parse(id), 3, 25);
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
        public void Hide_tel_ctrl(telefoonnummer_user_control tuc)
        {
            if (tuc == telefoonnummer_user_control1)
            {
                elementHost1.Hide();
            }
            else if (tuc == telefoonnummer_user_control2)
            {
                elementHost2.Hide();
            }
            else if (tuc == telefoonnummer_user_control3)
            {
                elementHost3.Hide();
            }
            button9.Show();
            Organize_panels();
        }
        private void button5_Click(object sender, EventArgs e)//JA
        {
            if (uitvoeren && vervangt)
            {

                vervang_persoon();
            }
            else if (vervangt)
            {
                multiColumnComboBox1.SelectionLength = 0;
                multiColumnComboBox1.Enabled = false;
                vervang_lbl.Text = "Vervang het contact \"ID " + id + " - " + textBox3.Text + " " + textBox4.Text + " " + textBox5.Text + " " + textBox6.Text + "\"\ndoor het contact \"ID " + multiColumnComboBox1.SelectedValue.ToString() + " - " + multiColumnComboBox1.Text.TrimEnd(' ') + "\"";
                vervang_lbl.Location = new Point(p_verw_verv.Width / 2 - vervang_lbl.Width / 2, vervang_lbl.Location.Y);
                uitvoeren = true;
                button5.Text = "Bevestigen";
            }
            else if(!vervangt && !uitvoeren)
            {
                
                
            }
        }

        private void button6_Click(object sender, EventArgs e)//NEE
        {
            if (uitvoeren)
            {
                uitvoeren = false;
                vervangt = false;
                vervang_lbl.Text = "Dit contact verwijderen en vervangen?";
                multiColumnComboBox1.Enabled = true;
                multiColumnComboBox1.Hide();
                multiColumnComboBox1.SelectedIndex = -1;
            }
            else if (vervangt)
            {
                vervangt = false;
                vervang_lbl.Text = "Dit contact verwijderen en vervangen?";
                multiColumnComboBox1.Hide();
                multiColumnComboBox1.SelectedIndex = -1;
                multiColumnComboBox1.Enabled = true;
                button5.Enabled = true;
            }
            else if(!uitvoeren && !vervangt)
            {
                p_verw_verv.Hide();
                Free_screen();
                btn_verwijder.Show();
                btn_verwijder_annulleer.Hide();
                btn_verwijder_bevestig.Hide();
            }
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

       private void button4_Click(object sender, EventArgs e)
       {
           button3.Image = zeebregtsCs.Properties.Resources.information_symbol_gray;
           button2.Image = zeebregtsCs.Properties.Resources.sym_function_x_blue_gray;
           button4.Image = zeebregtsCs.Properties.Resources.haxor;
           string voornaam = String.Empty;
           string voorletters = String.Empty;
           string achternaam = String.Empty;
           int bdrnr = 0;
           bool man = true;
           if (!Rowpersoon.IsvoornaamNull())
           {
               voornaam = Rowpersoon.voornaam;
           }
           if (!Rowpersoon.IsvoorlettersNull())
           {
               voorletters = Rowpersoon.voorletters;
           }
           if(!Rowpersoon.IsachternaamNull())
           {
               achternaam = Rowpersoon.achternaam;
           }
           if (!Rowpersoon.Isbedrijf_nrNull())
           {
               bdrnr = Rowpersoon.bedrijf_nr;
           }
           if (!Rowpersoon.IsmanNull())
           {
               man = Rowpersoon.man;
           }
           haxor_panel1.start_loading_pers(voornaam, voorletters, achternaam, bdrnr, man);
           panel4.BringToFront();
           panel4.Focus();
       }

       private void button6_Click_1(object sender, EventArgs e)//toclipboard naw
       {
           string summary = String.Empty;
           string copy_tel_1 = String.Empty;
           string copy_tel_2 = String.Empty;
           string copy_tel_3 = String.Empty;
           string copy_mail = String.Empty;
           string[] tel_sets = Rowpersoon.telefoon_nr_settings.Split(',');
           if (!Rowpersoon.Istelefoon_nr_1Null() && Rowpersoon.telefoon_nr_1.Length > 2)
           {
               int sett = int.Parse(tel_sets[0]);
               if (sett == 1)
               {
                   copy_tel_1 = "\nM " + Rowpersoon.telefoon_nr_1;
               }
               else if (sett == 2)
               {
                   copy_tel_1 = "\nF " + Rowpersoon.telefoon_nr_1;
               }
               else
               {
                   copy_tel_1 = "\nT " + Rowpersoon.telefoon_nr_1;
               }
           } 
           if (!Rowpersoon.Istelefoon_nr_2Null() && Rowpersoon.telefoon_nr_2.Length > 2)
           {
               int sett = int.Parse(tel_sets[1]);
               if (sett == 1)
               {
                   copy_tel_2 = "\nM " + Rowpersoon.telefoon_nr_2;
               }
               else if (sett == 2)
               {
                   copy_tel_2 = "\nF " + Rowpersoon.telefoon_nr_2;
               }
               else
               {
                   copy_tel_2 = "\nT " + Rowpersoon.telefoon_nr_2;
               }
           }
           if (!Rowpersoon.Istelefoon_nr_3Null() && Rowpersoon.telefoon_nr_3.Length > 2)
           {
               int sett = int.Parse(tel_sets[2]);
               if (sett == 1)
               {
                   copy_tel_3 = "\nM " + Rowpersoon.telefoon_nr_3;
               }
               else if (sett == 2)
               {
                   copy_tel_3 = "\nF " + Rowpersoon.telefoon_nr_3;
               }
               else
               {
                   copy_tel_3 = "\nT " + Rowpersoon.telefoon_nr_3;
               }
           }
           if (!Rowpersoon.IszaemailNull())
           {
               copy_mail = "\nE "+ Rowpersoon.zaemail;
           }
           summary = label37.Text + "\nOrganisatie: " + label21.Text + copy_mail + copy_tel_1 + copy_tel_2 + copy_tel_3;
           System.Windows.Forms.Clipboard.SetDataObject(summary, true);
           handelingen_logger.log_handeling(int.Parse(id), 3, 18);
       }

       private void button7_Click(object sender, EventArgs e)//toclipboard post
       {
           linktable = linkadapter.GetData(int.Parse(id));
           Rowlink = (persoonsformdataset.bedrijflinkRow)linktable.Rows[0];
           string summary = String.Empty;
           string copy_post_adres = String.Empty;
           if (!Rowlink.IspostSTRAATNull())
           {
               copy_post_adres += "\n" + Rowlink.postSTRAAT;
           }
           if (!Rowlink.IspostPOSTCODENull())
           {
               copy_post_adres += "\n" + Rowlink.postPOSTCODE;
           }
           if (!Rowlink.IspostPLAATSNull())
           {
               copy_post_adres += " " + Rowlink.postPLAATS;
           }
           string naam = String.Empty;
           if (Rowpersoon.man)
           {
               naam += "Mvr. ";
           }
           else
           {
               naam += "Dhr. ";
           }
           if (!Rowpersoon.IsvoorlettersNull())
           {
               naam += Rowpersoon.voorletters + " ";
           }
           if (!Rowpersoon.IstussenvoegselNull())
           {
               naam += Rowpersoon.tussenvoegsel + " ";
           }
           if (!Rowpersoon.IsachternaamNull())
           {
               naam += Rowpersoon.achternaam;
           }
           summary = label21.Text + "\nt.a.v. " + naam + copy_post_adres;
           System.Windows.Forms.Clipboard.SetDataObject(summary, true);
           handelingen_logger.log_handeling(int.Parse(id), 3, 22);
       }
       private void button8_Click(object sender, EventArgs e)//toclipboard bezoek
       {
           linktable = linkadapter.GetData(int.Parse(id));
           Rowlink = (persoonsformdataset.bedrijflinkRow)linktable.Rows[0];
           string summary = String.Empty;
           string copy_post_adres = String.Empty;
               if (!Rowlink.IsstraatNull())
               {
                   copy_post_adres += "\n" + Rowlink.straat;
               }
               if (!Rowlink.IspostcodeNull())
               {
                   copy_post_adres += "\n" + Rowlink.postcode;
               }
               if (!Rowlink.IsplaatsNull())
               {
                   copy_post_adres += " " + Rowlink.plaats;
               }
               string naam = String.Empty;
               if (Rowpersoon.man)
               {
                   naam += "Mvr. ";
               }
               else
               {
                   naam += "Dhr. ";
               }
               if (!Rowpersoon.IsvoorlettersNull())
               {
                   naam += Rowpersoon.voorletters + " ";
               }
               if (!Rowpersoon.IstussenvoegselNull())
               {
                   naam += Rowpersoon.tussenvoegsel + " ";
               }
               if (!Rowpersoon.IsachternaamNull())
               {
                   naam += Rowpersoon.achternaam;
               }
               summary = label21.Text + "\nt.a.v. " + naam + copy_post_adres;
           System.Windows.Forms.Clipboard.SetDataObject(summary, true);
           handelingen_logger.log_handeling(int.Parse(id), 3, 23);
       }
       private void splitContainer_Panel_MouseHover(object sender, EventArgs e)
       {
           Control c = (Control)sender;
           if (c is SplitterPanel)
           {
              tip_copy.Show("Door te klikken wordt er een beknopte kopie gemaakt van dit formulier, zodat vervolgens de tekst in een document geplakt kan worden.", (c.Parent as SplitContainer).Panel1, 0, -40);
           }
           else if (c is Label)
           {
               tip_copy.Show("Door te klikken wordt er een beknopte kopie gemaakt van dit formulier, zodat vervolgens de tekst in een document geplakt kan worden.", (c.Parent.Parent as SplitContainer).Panel1, 0, -40);
           }
           else if (c is Button)
           {
               tip_copy.Show("Door te klikken wordt er een beknopte kopie gemaakt van dit formulier, zodat vervolgens de tekst in een document geplakt kan worden.", (c.Parent.Parent as SplitContainer).Panel1, 0, -40);
           }
       }

       private void splitContainer_Panel_MouseLeave(object sender, EventArgs e)
       {
          
           Control c = (Control)sender;
           if (c is SplitterPanel)
           {
               
               try { tip_copy.Hide((c.Parent as SplitContainer).Panel1); }
               catch (Exception ex)
               {
                   log_exception(ex);
               }
           }
           else if (c is Label || c is Button)
           {
               try { tip_copy.Hide((c.Parent.Parent as SplitContainer).Panel1); }
               catch (Exception ex)
               {
                   log_exception(ex);
               }
           }
       }

       private void panel3_Click(object sender, EventArgs e)
       {
           Panel p = sender as Panel;
           p.Focus();
       }

       private void persoon_form_Shown(object sender, EventArgs e)
       {
           Organize_panels();
       }

       private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
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
                   handelingen_logger.log_handeling(int.Parse(id), 3, 19);
               }
           }



       }

       private void button1_MouseEnter(object sender, EventArgs e)
       {
           Button btn = sender as Button;
           btn.FlatStyle = FlatStyle.Standard;
       }

       private void button1_MouseLeave(object sender, EventArgs e)
       {
           Button btn = sender as Button;
           btn.FlatStyle = FlatStyle.Flat;
       }

       private void button9_Click(object sender, EventArgs e)
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
           p_telnrs.Controls.Remove(button9);
           p_telnrs.Controls.Add(x);
           p_telnrs.Controls.Add(button9);
          Organize_panels();
       }

}
}
