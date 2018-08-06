using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Globalization;
using System.Threading;
using System.Text;
using System.Diagnostics;
using zeebregtsCs.usercontrols;
using System.IO;

namespace zeebregtsCs
{
    public partial class project_form : base_form
    {
        //initializer
        //new-way vals
        int enum_nr = -1;
        
        Form_helper Proj_form_helper;
        bool van_zoek = false;
        private Dictionary<MultiColumnComboBoxDemo.MultiColumnComboBox, int> cbb = new Dictionary<MultiColumnComboBoxDemo.MultiColumnComboBox, int>();
        ToolTip tip1;
        ToolTip tip2;
        ToolTip tip3;
        ToolTip tip4;
        ToolTip tip5;
        ToolTip tip6;
        ToolTip tip7;
        ToolTip tip8;
        ToolTip tip9;
        ToolTip tip10;
        //dit zijn nieuwe wijzigingen
        ToolTip tt_memo = new ToolTip();
        private Dictionary<ToolTip, string> tooltiptexts = new Dictionary<ToolTip, string>();
        private Dictionary<SplitContainer, ToolTip> ttd = new Dictionary<SplitContainer,ToolTip>();
        private Dictionary<Label, ToolTip> labeld = new Dictionary<Label, ToolTip>();
        string antw;
        string vrijgeefstring = "";
        //
        public bool wait = true;
        testset.projectRow testrow;
        testset.projectRow ValidateRow;
        testset.memosRow memoRow;
        testsetTableAdapters.projectTableAdapter adapter = new testsetTableAdapters.projectTableAdapter();
        public int combox = 0;
        int tab = 1;
        int startWeekNum;
        int startJaar;
        int eindWeekNum;
        int eindJaar;
        private bool[] dates = new bool[12];
        bool from_tb = false;
        bool upsub = false;
        MultiColumnComboBoxDemo.MultiColumnComboBox cbb_to_fill;
        string id = "0";
        int indexId = 0;
        testset.projectDataTable table;
        testsetTableAdapters.bedrijfTableAdapter bedrijfadapt = new testsetTableAdapters.bedrijfTableAdapter();
        testsetTableAdapters.persoonTableAdapter persoonadapt = new testsetTableAdapters.persoonTableAdapter();
        testsetTableAdapters.meerwerkverrekenmethodeTableAdapter methodeadapt = new testsetTableAdapters.meerwerkverrekenmethodeTableAdapter();
        testsetTableAdapters.statusTableAdapter statusadapt = new testsetTableAdapters.statusTableAdapter();
        testset.bedrijfDataTable bedrijftable;
        testset.persoonDataTable persoontable;
        testset.meerwerkverrekenmethodeDataTable methodetable;
        testset.statusDataTable statustable;
        testsetTableAdapters.memosTableAdapter memoadapt = new testsetTableAdapters.memosTableAdapter();
        testset.memosDataTable memotabl;
        AdresDataSetTableAdapters.adressenTableAdapter aTA = new AdresDataSetTableAdapters.adressenTableAdapter();
        AdresDataSet.adressenDataTable aDT = new AdresDataSet.adressenDataTable();
        AdresDataSet.adressenRow aRow;
        private overview1 _parent;
        public project_form(base_form start_scherm, base_form close_naar, int enumr)
        {
           

            adapter.Connection.ConnectionString = Global.ConnectionString_fileserver;
            bedrijfadapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
            persoonadapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
            methodeadapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
            statusadapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
            memoadapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
            aTA.Connection.ConnectionString = Global.ConnectionString_fileserver;
            //Global.FW_add(this, close_naar);
            FormManager.VoegToe(this, close_naar);
            InitializeComponent();
            this.DoubleBuffered = true;
            start_parent = start_scherm;
            close_parent = close_naar;
            enum_nr = enumr;
            this.Text = Global.WTitle;
            initialiseer();
        }
        
        public void SaveCBB_extern()
        {
            cbb.Clear();
            Save_cbb_settings(this);
        }
        private void ToolTip_settings(ToolTip tt)
        {
            
            tt.AutoPopDelay = 10000;
            tt.InitialDelay = 200;
            tt.IsBalloon = true;
            tt.ReshowDelay = 300;
            tt.ShowAlways = true;
        }
        private void initialiseer()
        {
            
            van_zoek = Global.van_zoek_proj;
            init_all();
            switch (enum_nr)
            {

                case 0: //normaal (menu>proj_overview>regel)
                    
                    _parent = (start_parent as overview1);
                    
                    lbl_top.Text = "";
                    lbl_mid.Text = "Info project";
                      
                      if (van_zoek || Global.Dubb_van_zoek)
                      {btnterug.Image = null;
                          btnterug.Image = Properties.Resources.ZoekProject2;
                          Global.Dubb_van_zoek = false;
                      }
                      else
                      {btnterug.Image = null;
                          btnterug.Image = Properties.Resources.Alleprojecten2;}

                    break;
                case 1: //rood (menu>proj_overview>nieuw>aanmaken)
                    btnterug.Hide();
                   // btn_verwijder.Hide();
                    
                    lbl_top.Text = "";
                    lbl_mid.Text = "Nieuw project";
                    //nwi = 1;
                    break;
                case 4:
                    lbl_top.Text = "Bekijk de gegevens t.b.v de dubbele invoer controle.";
                    lbl_mid.Text = "Overeenkomstig project";
                    btnterug.Image = Properties.Resources.Terug;
                    btnwijzig.Hide();
                    btnopslaan.Hide();
                    break;
            }
            EventArgs e = new EventArgs();
            
            this.project_form_Resize(this, e);
        }
        private void init_all()
        {
            if (Global.Dubbel_is_bevestigd)
            {
                Global.Dubbel_is_bevestigd = false;
                start_parent = start_parent.s_parent().s_parent();
                close_parent = start_parent.c_parent();
               // FormManager.Sluit_form(start_parent.s_parent());
               // FormManager.Sluit_form(start_parent);
               //Global.overzicht_select = id;
              // initialiseer();

            }
           // telefoonnummer_user_control1.set_naam(0, true);
            
//            telefoonnummer_user_control2.set_naam(2, true);
            Proj_form_helper = new Form_helper();
            if (Global.UserLevel < 3)
            {
                button18.Show();
            }
            else
            {
                button18.Hide();
            }
            adres_user_control1.initialiseer(false, true);
            adres_user_control2.initialiseer(false, false);
            id = Global.overzicht_select;
            Global.overzicht_select = "";
            
            init_comboNToolTip(true);
            this.btnwijzig.Hide();
            this.btnopslaan.Show();
           
            tip1 = new ToolTip();
            ttd.Clear();
            ttd.Add(splitContainer71, tip1);
            tooltiptexts.Add(tip1, "rendement meerwerk luxe tegels");
            tip2 = new ToolTip();
            ttd.Add(splitContainer70, tip2);
            tooltiptexts.Add(tip2,"rendement meerwerk arbeid en standaard materiaal" );
            tip3 = new ToolTip();
            ttd.Add(splitContainer68, tip3);
            tooltiptexts.Add(tip3,"niet aangenomen werk tonen in financieel overzicht");
            tip4 = new ToolTip();
            ttd.Add(splitContainer65, tip4);
            tooltiptexts.Add(tip4, "opslag meerwerk arbeid en standaard materiaal");
            tip5 = new ToolTip();
            ttd.Add(splitContainer64, tip5);
            tooltiptexts.Add(tip5,"opslag meters in verrekenstaten");
            tip6 = new ToolTip();
            ttd.Add(splitContainer75, tip6);
            tooltiptexts.Add(tip6,"STABU norm opnemen in kopersofferte");
            tip7 = new ToolTip();
            ttd.Add(splitContainer77, tip7);
            tooltiptexts.Add(tip7,"project opnemen in statusoverzicht");
            tip8 = new ToolTip();
            ttd.Add(splitContainer9, tip8);
            tooltiptexts.Add(tip8,"memo opnemen in statusoverzicht");
            tip9 = new ToolTip();
            ttd.Add(splitContainer61, tip9);
            tooltiptexts.Add(tip9,"verpakkingstoeslag meenemen bij het opstap pakket ");
            tip10 = new ToolTip();
            ttd.Add(splitContainer79, tip10);
            tooltiptexts.Add(tip10,"Door te klikken wordt er een beknopte kopie gemaakt van dit formulier, zodat vervolgens de tekst in een document geplakt kan worden.");
            ToolTip_settings(tip1);
            ToolTip_settings(tip2);
            ToolTip_settings(tip3);
            ToolTip_settings(tip4);
            ToolTip_settings(tip5);
            ToolTip_settings(tip6);
            ToolTip_settings(tip7);
            ToolTip_settings(tip8);
            ToolTip_settings(tip9);
            ToolTip_settings(tip10);
            Point pt1 = new Point(panel1.Width / 2 - lbl_top.Width / 2, 5);
            lbl_top.Location = pt1;
            Point pt2 = new Point(panel1.Width / 2 - lbl_mid.Width / 2, 28);
            lbl_mid.Location = pt2;
            Point pt3 = new Point(panel1.Width / 2 - lbl_bot.Width / 2, panel1.Height - 18);
            lbl_bot.Location = pt3;
            variabele_set1.set_type_n_projnum(3, int.Parse(id), this);
            variabele_set2.set_type_n_projnum(2, int.Parse(id), this);
            //lbltopstr = lbl_top.Text;
            convarPos = variabele_set1.Location;
            bdrvarPos = variabele_set2.Location;
            if (!Global.NoConnection)
            { no_edit(true); }
			
        }
		public void fill_id(bool n)
        {
            init_comboNToolTip(false);
            Load_cbb_settings();
            int a = int.Parse(Global.return_id);
            if (combox == -2 )
            {
                variabele_set2.fill_id(a);
            }
            else if (combox == -3)
            {
                variabele_set1.fill_id(a);
            }
            else
            {
                variabele_set1.UpdateCBBs();
                variabele_set2.UpdateCBBs();
                cbb_to_fill.SelectedValue = a;
            }
            if (n)
            {
                this.Show();
            }
            lbl_mid.Focus();
           
            
        }
        
        private void project_form_Load(object sender, EventArgs e)
        {
            this.MinimumSize = new Size(450, 310);
           button30.PerformClick();
           disable_splitter(this);
            if (enum_nr == 1)
            {
                btnwijzig.PerformClick();
            }
           // this.WindowState = Global.windowstate;
           // if (Global.windowstate != FormWindowState.Maximized)
          //  {
         //       this.Size = Global.size;
         //       this.Location = Global.position;
         //   }
           
            //lbl_mid.Focus();
            panel5.Focus();

          //  groupBox2.Hide();
           // groupBox3.Hide();
			
        }
		
		private void clear_text_lbl()
        {
            label42.Text = "";
            label96.Text = "";
            label97.Text = "";
            label98.Text = "";
            label99.Text = "";
            label103.Text = "";
            label90.Text = "";
            label92.Text = "";
            label93.Text = "";
            label94.Text = "";
            label95.Text = "";
            label168.Text = "";
            label124.Text = "";
            label125.Text = "";
            label126.Text = "";
            label127.Text = "";
            label128.Text = "";
            label129.Text = "";
            label130.Text = "";
            label131.Text = "";
            label132.Text = "";
            label133.Text = "";
            label134.Text = "";
            label135.Text = "";
            label136.Text = "";
            label137.Text = "";
            label138.Text = "";
            label139.Text = "";
            label140.Text = "";
            label141.Text = "";
            label142.Text = "";
            label143.Text = "";
            label144.Text = "";
            label145.Text = "";
            label146.Text = "";
            label147.Text = "";
            linkLabel25.Text = "...";
            label149.Text = "";
            label150.Text = "";
            label151.Text = "";
            label152.Text = "";
            label153.Text = "";
            label154.Text = "";
            label155.Text = "";
            linkLabel26.Text = "...";
            label170.Text = "";
            label69.Text = "-";
            label70.Text = "-";
            label100.Text = "-";
            label101.Text = "-";
            label102.Text = "-";
            label104.Text = "-";
            label105.Text = "-";
            label106.Text = "-";
            label107.Text = "-";
            label123.Text = "-";
            label201.Text = "-";
            label202.Text = "-";
            label203.Text = "-";
            label204.Text = "-";
            label205.Text = "";
            label206.Text = "";
            label207.Text = "";
            label208.Text = "";
            label209.Text = "";
            label210.Text = "";
            label211.Text = "";
            label212.Text = "";
            label213.Text = "";
            label214.Text = "";
        }
       
        private void init_comboNToolTip(bool n)
        {
            bool tmp = n;
           

           dateTimePicker1.Format = DateTimePickerFormat.Custom;
           dateTimePicker1.CustomFormat = "dddd dd - MMMM MM - yyyy";
           dateTimePicker2.Format = DateTimePickerFormat.Custom;
           dateTimePicker2.CustomFormat = "dddd dd - MMMM MM - yyyy";
           dateTimePicker3.Format = DateTimePickerFormat.Custom;
           dateTimePicker3.CustomFormat = "dddd dd - MMMM MM - yyyy";
           dateTimePicker4.Format = DateTimePickerFormat.Custom;
           dateTimePicker4.CustomFormat = "dddd dd - MMMM MM - yyyy";
           dateTimePicker5.Format = DateTimePickerFormat.Custom;
           dateTimePicker5.CustomFormat = "dddd dd - MMMM MM - yyyy";
           dateTimePicker6.Format = DateTimePickerFormat.Custom;
           dateTimePicker6.CustomFormat = "dddd dd - MMMM MM - yyyy";
           dateTimePicker7.Format = DateTimePickerFormat.Custom;
           dateTimePicker7.CustomFormat = "dddd dd - MMMM MM - yyyy";
           dateTimePicker8.Format = DateTimePickerFormat.Custom;
           dateTimePicker8.CustomFormat = "dddd dd - MMMM MM - yyyy";
           dateTimePicker9.Format = DateTimePickerFormat.Custom;
           dateTimePicker9.CustomFormat = "dddd dd - MMMM MM - yyyy";
           dateTimePicker10.Format = DateTimePickerFormat.Custom;
           dateTimePicker10.CustomFormat = "dddd dd - MMMM MM - yyyy";
           dateTimePicker11.Format = DateTimePickerFormat.Custom;
           dateTimePicker11.CustomFormat = "dddd dd - MMMM MM - yyyy";

           
           if (!Global.NoConnection)
           {
               bedrijftable = bedrijfadapt.GetData();
               persoontable = persoonadapt.GetData();
               methodetable = methodeadapt.GetData();
               statustable = statusadapt.GetData();
              
               foreach (DataRow dr in bedrijftable)
               {
                   foreach (DataRow dr2 in bedrijftable)
                   {
                       if ((int)dr[0] != (int)dr2[0] && (string)dr[1] == (string)dr2[1])
                       {
                           dr[1] = dr[1].ToString() +" "+ dr[0].ToString();
                           dr2[1] = dr2[1].ToString() + " " + dr2[0].ToString();
                       }
                   }
               }
               foreach (DataRow dr in persoontable)
               {
                   
                   foreach (DataRow dr2 in persoontable)
                   {
                       if ((int)dr[0] != (int)dr2[0] && (string)dr[1] == (string)dr2[1])
                       {
                           dr[1] = dr[1].ToString() + " " + dr[0].ToString();
                           dr2[1] = dr2[1].ToString() + " " + dr2[0].ToString();
                       }
                   }
               }


               btn_verwijder.Hide();
               btn_verwijder_annuleer.Hide();
               btn_verwijder_bevestig.Hide();

               //inkoper
               mcb_pers1.BindingContext = new BindingContext();
               
               mcb_pers1.DisplayMember = "SearchName";
               mcb_pers1.ValueMember = "persoon_nr";
               mcb_pers1.DataSource = persoontable;
               
               //mcb_pers1.Text = "";
               
               //werkvoorbereider
               mcb_pers2.BindingContext = new BindingContext();
               
               mcb_pers2.DisplayMember = "SearchName";
               mcb_pers2.ValueMember = "persoon_nr";
               mcb_pers2.DataSource = persoontable;
               
              // mcb_pers2.Text = "";
               
               //projectleider
               mcb_pers3.BindingContext = new BindingContext();
               
               mcb_pers3.DisplayMember = "SearchName";
               mcb_pers3.ValueMember = "persoon_nr";
               mcb_pers3.DataSource = persoontable;
               
             //  mcb_pers3.Text = "";
               
               //uitvoerder
               mcb_pers4.BindingContext = new BindingContext();
              
               mcb_pers4.DisplayMember = "SearchName";
               mcb_pers4.ValueMember = "persoon_nr";
               mcb_pers4.DataSource = persoontable;
               
            //   mcb_pers4.Text = "";
               
               //opzichter
               mcb_pers5.BindingContext = new BindingContext();
               
               mcb_pers5.DisplayMember = "SearchName";
               mcb_pers5.ValueMember = "persoon_nr";
               mcb_pers5.DataSource = persoontable;
               
            //   mcb_pers5.Text = "";
               
               //betrokken intern
               //offerte
               mcb_pers6.BindingContext = new BindingContext();
               
               mcb_pers6.DisplayMember = "SearchName";
               mcb_pers6.ValueMember = "persoon_nr";
               mcb_pers6.DataSource = persoontable;
               
             //  mcb_pers6.Text = "";
               
               //0offerte
               mcb_pers7.BindingContext = new BindingContext();
               
               mcb_pers7.DisplayMember = "SearchName";
               mcb_pers7.ValueMember = "persoon_nr";
               mcb_pers7.DataSource = persoontable;
               
            //   mcb_pers7.Text = "";
               
               //meerwerk
               mcb_pers8.BindingContext = new BindingContext();
               
               mcb_pers8.DisplayMember = "SearchName";
               mcb_pers8.ValueMember = "persoon_nr";
               mcb_pers8.DataSource = persoontable;
               
            //   mcb_pers8.Text = "";
               
               //operationeel
               mcb_pers9.BindingContext = new BindingContext();
               
               mcb_pers9.DisplayMember = "SearchName";
               mcb_pers9.ValueMember = "persoon_nr";
               mcb_pers9.DataSource = persoontable;
               
            //   mcb_pers9.Text = "";
               
               //uitvoerder
               mcb_pers10.BindingContext = new BindingContext();
               
               mcb_pers10.DisplayMember = "SearchName";
               mcb_pers10.ValueMember = "persoon_nr";
               mcb_pers10.DataSource = persoontable;
               
           //    mcb_pers10.Text = "";
               
               //tegelzetter
               mcb_pers11.BindingContext = new BindingContext();
               
               mcb_pers11.DisplayMember = "SearchName";
               mcb_pers11.ValueMember = "persoon_nr";
               mcb_pers11.DataSource = persoontable;
               
            //   mcb_pers11.Text = "";
               
               //bedrijfsbureau
               //tegels_pers
               mcb_pers12.BindingContext = new BindingContext();
               
               mcb_pers12.DisplayMember = "SearchName";
               mcb_pers12.ValueMember = "persoon_nr";
               mcb_pers12.DataSource = persoontable;
               
           //   mcb_pers12.Text = "";
               
               //sanitair_pers
               mcb_pers13.BindingContext = new BindingContext();
              
               mcb_pers13.DisplayMember = "SearchName";
               mcb_pers13.ValueMember = "persoon_nr";
               mcb_pers13.DataSource = persoontable;
               
         //    mcb_pers13.Text = "";
               
               //keukens_pers
               mcb_pers14.BindingContext = new BindingContext();
               
               mcb_pers14.DisplayMember = "SearchName";
               mcb_pers14.ValueMember = "persoon_nr";
               mcb_pers14.DataSource = persoontable;
               
        //    mcb_pers14.Text = "";
               
               //kopersbegeleiding_pers
               mcb_pers15.BindingContext = new BindingContext();
               
               mcb_pers15.DisplayMember = "SearchName";
               mcb_pers15.ValueMember = "persoon_nr";
               mcb_pers15.DataSource = persoontable;
               
          //    mcb_pers15.Text = "";
               

               //bdr
               //opdrachtgever
               mcb_bdr1.BindingContext = new BindingContext();
               
               mcb_bdr1.DisplayMember = "zoeknaam";
               mcb_bdr1.ValueMember = "bedrijf_nr";
               mcb_bdr1.DataSource = bedrijftable;
               
         //    mcb_bdr1.Text = "";
               
               //bouwbedrijf
               mcb_bdr2.BindingContext = new BindingContext();
               
               mcb_bdr2.DisplayMember = "zoeknaam";
               mcb_bdr2.ValueMember = "bedrijf_nr";
               mcb_bdr2.DataSource = bedrijftable;
               
         //   mcb_bdr2.Text = "";
               
               //projectontwikkelaar
               mcb_bdr3.BindingContext = new BindingContext();
              
               mcb_bdr3.DisplayMember = "zoeknaam";
               mcb_bdr3.ValueMember = "bedrijf_nr";
               mcb_bdr3.DataSource = bedrijftable;
               
          //   mcb_bdr3.Text = "";
               
               //tegels_bdr
               mcb_bdr4.BindingContext = new BindingContext();
               
               mcb_bdr4.DisplayMember = "zoeknaam";
               mcb_bdr4.ValueMember = "bedrijf_nr";
               mcb_bdr4.DataSource = bedrijftable;
               
         //   mcb_bdr4.Text = "";
               
               //sanitair_bdr
               mcb_bdr5.BindingContext = new BindingContext();
               
               mcb_bdr5.DisplayMember = "zoeknaam";
               mcb_bdr5.ValueMember = "bedrijf_nr";
               mcb_bdr5.DataSource = bedrijftable;
               
            // mcb_bdr5.Text = "";
               
               //keukens_bdr
               mcb_bdr6.BindingContext = new BindingContext();
              
               mcb_bdr6.DisplayMember = "zoeknaam";
               mcb_bdr6.ValueMember = "bedrijf_nr";
               mcb_bdr6.DataSource = bedrijftable;
               
           //   mcb_bdr6.Text = "";
               
               //kopersbegeleiding_bdr
               mcb_bdr7.BindingContext = new BindingContext();
               
               mcb_bdr7.DisplayMember = "zoeknaam";
               mcb_bdr7.ValueMember = "bedrijf_nr";
               mcb_bdr7.DataSource = bedrijftable;
               
           //   mcb_bdr7.Text = "";
               
               //facturatie_bdr
               mcb_bdr8.BindingContext = new BindingContext();
               
               mcb_bdr8.DisplayMember = "zoeknaam";
               mcb_bdr8.ValueMember = "bedrijf_nr";
               mcb_bdr8.DataSource = bedrijftable;
               
           //   mcb_bdr8.Text = "";
               
               //betalings_bdr
               mcb_bdr9.BindingContext = new BindingContext();
               
               mcb_bdr9.DisplayMember = "zoeknaam";
               mcb_bdr9.ValueMember = "bedrijf_nr";
               mcb_bdr9.DataSource = bedrijftable;
               
           //    mcb_bdr9.Text = "";
               

               comboBox31.BindingContext = new BindingContext();
               
               comboBox31.DisplayMember = "omschrijving";
               comboBox31.ValueMember = "omschrijving_nr";
               comboBox31.DataSource = statustable;
          //    comboBox31.Text = "";
               

               comboBox33.BindingContext = new BindingContext();
               
               comboBox33.DisplayMember = "verrekenmethodeOMSCHRIJVING";
               comboBox33.ValueMember = "verrekenmethodeNR";
               comboBox33.DataSource = methodetable;
          //    comboBox33.Text = "";
               
               if (tmp)
               {
                   mcb_pers1.SelectedIndex = -1;
                   mcb_pers2.SelectedIndex = -1;
                   mcb_pers3.SelectedIndex = -1;
                   mcb_pers4.SelectedIndex = -1;
                   mcb_pers5.SelectedIndex = -1;
                   mcb_pers6.SelectedIndex = -1;
                   mcb_pers7.SelectedIndex = -1;
                   mcb_pers8.SelectedIndex = -1;
                   mcb_pers9.SelectedIndex = -1;
                   mcb_pers10.SelectedIndex = -1;
                   mcb_pers11.SelectedIndex = -1;
                   mcb_pers12.SelectedIndex = -1;
                   mcb_pers13.SelectedIndex = -1;
                   mcb_pers14.SelectedIndex = -1;
                   mcb_pers15.SelectedIndex = -1;
                   mcb_bdr1.SelectedIndex = -1;
                   mcb_bdr2.SelectedIndex = -1;
                   mcb_bdr3.SelectedIndex = -1;
                   mcb_bdr4.SelectedIndex = -1;
                   mcb_bdr5.SelectedIndex = -1;
                   mcb_bdr6.SelectedIndex = -1;
                   mcb_bdr7.SelectedIndex = -1;
                   mcb_bdr8.SelectedIndex = -1;
                   mcb_bdr9.SelectedIndex = -1;
                   comboBox31.SelectedIndex = -1;
                   comboBox33.SelectedIndex = -1;
               }
           }
            

            
            
        }
        private void show_all_containers(Control C)
        {
            foreach (Control SC in C.Controls)
            {
                if (SC is SplitContainer)
                {
                    SC.Show();
                }
                if (SC.Controls.Count > 0)
                {
                    show_all_containers(SC);
                }
            }
            
        }
        private void no_edit(bool nav)
        {
            using (Panel tmp_P = new Panel())
            {
                tmp_P.Location = panel5.Location;
                tmp_P.Size = panel5.Size;
                tmp_P.BackColor = System.Drawing.SystemColors.ControlLightLight;
                tmp_P.BorderStyle = BorderStyle.Fixed3D;
                this.Controls.Add(tmp_P);
                tmp_P.Show();
                tmp_P.BringToFront();

                if (nav)
                {
                    clear_text_lbl();
                    Clear_text_tb(this);
                    NavigateRecord();
                }

                switch (tab)
                {
                    case 3:
                        //tab 3
                        label184.Hide();
                        label185.Hide();
                        label186.Hide();
                        label187.Hide();
                        label188.Hide();
                        label189.Hide();
                        label190.Hide();
                        label191.Hide();
                        //button16.Hide();
                        //button17.Hide();

                        // label1.Hide();
                        // label3.Hide();
                        // linkLabel25.Show();
                        // linkLabel26.Show();
                        textBox61.Hide();
                        textBox63.Hide();
                        dateTimePicker5.Hide();
                        dateTimePicker1.Hide();
                        dateTimePicker6.Hide();
                        dateTimePicker7.Hide();
                        textBox13.Hide();
                        textBox14.Hide();

                        checkBox1.Hide();
                        dateTimePicker8.Hide();
                        dateTimePicker2.Hide();
                        dateTimePicker9.Hide();
                        dateTimePicker10.Hide();
                        textBox23.Hide();
                        textBox18.Hide();
                        textBox26.Hide();
                        textBox30.Hide();
                        textBox31.Hide();
                        textBox27.Hide();
                        textBox37.Hide();
                        textBox38.Hide();
                        textBox36.Hide();
                        textBox40.Hide();
                        textBox41.Hide();
                        textBox39.Hide();
                        textBox43.Hide();
                        textBox44.Hide();
                        textBox42.Hide();
                        textBox51.Hide();
                        textBox52.Hide();
                        textBox50.Hide();
                        textBox54.Hide();
                        textBox56.Hide();
                        textBox53.Hide();
                        textBox58.Hide();
                        textBox59.Hide();
                        textBox57.Hide();
                        label89.Hide();
                        label91.Hide();
                        label118.Hide();
                        label158.Hide();
                        label157.Hide();
                        label119.Hide();
                        label162.Hide();
                        label161.Hide();
                        label160.Hide();
                        label165.Hide();
                        label164.Hide();
                        label163.Hide();
                        label171.Hide();
                        label167.Hide();
                        label166.Hide();
                        label174.Hide();
                        label173.Hide();
                        label172.Hide();
                        label177.Hide();
                        label176.Hide();
                        label175.Hide();
                        label180.Hide();
                        label179.Hide();
                        label178.Hide();
                        label144.Show();
                        label145.Show();
                        label146.Show();
                        label147.Show();
                        label149.Show();
                        label150.Show();
                        label151.Show();
                        label152.Show();
                        label153.Show();
                        label154.Show();
                        label155.Show();


                        dateTimePicker1.Enabled = false;
                        dateTimePicker2.Enabled = false;
                        dateTimePicker5.Enabled = false;
                        dateTimePicker6.Enabled = false;
                        dateTimePicker7.Enabled = false;
                        dateTimePicker8.Enabled = false;
                        dateTimePicker9.Enabled = false;
                        dateTimePicker10.Enabled = false;
                        break;
                    case 2:
                        //tab 2
                        // meerwerk
                        radioplusbtn.Hide(); radiominbtn.Hide();
                     //   button15.Hide();
                       // button22.Hide();
                        //button3.Hide();
                        //button21.Hide();
                        //button23.Hide();
                        mcb_pers12.Hide();
                        mcb_pers13.Hide();
                        mcb_pers14.Hide();
                        mcb_pers15.Hide();
                        mcb_bdr4.Hide();
                        mcb_bdr5.Hide();
                        mcb_bdr6.Hide();
                        mcb_bdr7.Hide();
                        mcb_bdr8.Hide();
                        mcb_bdr9.Hide();
                        textBox66.Hide();
                        textBox65.Hide();
                        comboBox33.Hide();
                        //button24.Hide();
                        //button13.Hide();
                        //button25.Hide();
                        //button14.Hide();
                        //button26.Hide();
                        textBox2.Hide();
                        textBox1.Hide();
                        textBox3.Hide();
                        textBox8.Hide();
                        textBox17.Hide();
                        textBox21.Hide();
                        textBox24.Hide();
                        textBox22.Hide();
                        radioButton20.Hide(); radioButton21.Hide();
                        radioButton12.Hide(); radioButton13.Hide();
                        radioButton14.Hide(); radioButton15.Hide();
                        radioButton10.Hide(); radioButton11.Hide();
                        textBox20.Hide();
                        textBox19.Hide();
                        textBox55.Hide();
                        textBox32.Hide();
                        textBox33.Hide();
                        textBox34.Hide();
                        textBox35.Hide();
                        textBox47.Hide();
                        radioButton1.Hide();
                        radioButton2.Hide();
                        radioButton16.Hide(); radioButton17.Hide();
                        radioButton18.Hide(); radioButton19.Hide();
                        radioButton3.Hide();
                        radioButton4.Hide();
                        radioButton5.Hide();
                        radioButton6.Hide();
                        radioButton7.Hide();
                        radioButton8.Hide();
                        radioButton9.Hide();
                        label124.Show();
                        label125.Show();
                        label131.Show();
                        label205.Show();
                        label206.Show();
                        label207.Show();
                        label208.Show();
                        label209.Show();
                        label210.Show();
                        label211.Show();
                        label212.Show();
                        label126.Show();
                        label127.Show();
                        label128.Show();
                        label129.Show();
                        label130.Show();
                        label135.Show();
                        label131.Show();
                        label132.Show();
                        label133.Show();
                        label134.Show();
                        label135.Show();
                        label136.Show();
                        label137.Show();
                        label138.Show();
                        label139.Show();
                        label213.Show();
                        label214.Show();
                        label170.Show();
                        label140.Show();
                        label141.Show();
                        label140.Show();
                        label141.Show();
                        label142.Show();
                        label143.Show();
                        label142.Show();
                        label143.Show();
                        label218.Show();
                        //leestekens
                        label8.Hide();
                        label2.Hide();
                        label24.Hide();
                        label17.Hide();
                        label26.Hide();
                        label27.Hide();
                        label28.Hide();
                        label29.Hide();
                        label30.Hide();
                        label38.Hide();
                        label31.Hide();
                        label40.Hide();
                        label216.Hide();
                        break;
                    case 1:


                        //tab1
                        // button29.Hide();
                        //  label192.Hide();
                        //  linkLabel27.Show();
                        textBox7.Hide();
                        linkLabel28.Show();
                        textBox60.Hide();
                        label181.Hide();
                        label182.Hide();
                        label183.Hide();
                        label58.Hide();
                        label59.Hide();
                        label74.Hide();
                        textBox12.Hide();
                        textBox15.Hide();
                        textBox16.Hide();
                        label83.Hide();
                        label84.Hide();
                        label85.Hide();
                        label86.Hide();
                        label17.Hide();
                        label24.Hide();
                        label26.Hide();
                        label27.Hide();
                        label28.Hide();
                        label29.Hide();
                        label30.Hide();
                        label38.Hide();
                        label31.Hide();
                        label40.Hide();
                        textBox6.Hide();
                        label2.Hide();
                        label8.Hide();
                   //     button27.Hide();
                  //      button20.Hide();
                  //      button19.Hide();
                  //      button1.Hide();
                   //     button2.Hide();
                   //     button4.Hide();
                   //     button5.Hide();
                   //     button6.Hide();
                        textBox9.Hide();
                        comboBox31.Hide();
                        textBox10.Hide();
                        textBox28.Hide();
                        textBox29.Hide();
                        textBox11.Hide();
                        textBox45.Hide();
                        dateTimePicker3.Hide();
                        dateTimePicker4.Hide();
                        dateTimePicker11.Hide();
                        //button7.Hide();
                        //button8.Hide();
                        //button9.Hide();
                        //button10.Hide();
                        //button11.Hide();
                        //button12.Hide();
                        textBox5.Hide();
                        textBox62.Hide();
                        textBox4.Hide();
                        textBox67.Hide();
                        textBox25.Hide();
                        mcb_pers1.Hide();
                        mcb_pers2.Hide();
                        mcb_pers3.Hide();
                        mcb_pers4.Hide();
                        mcb_pers5.Hide();
                        mcb_pers6.Hide();
                        mcb_pers7.Hide();
                        mcb_pers8.Hide();
                        mcb_pers9.Hide();
                        mcb_pers10.Hide();
                        mcb_pers11.Hide();
                        mcb_bdr1.Hide();
                        mcb_bdr2.Hide();
                        mcb_bdr3.Hide();

                        label168.Show();
                        label42.Show();
                        label69.Show();
                        label70.Show();
                        label100.Show();
                        label101.Show();
                        label102.Show();
                        label104.Show();
                        label105.Show();
                        label106.Show();
                        label90.Show();
                        label92.Show();
                        label93.Show();
                        label94.Show();
                        label95.Show();
                        label107.Show();
                        label123.Show();
                        label201.Show();
                        label202.Show();
                        label203.Show();
                        label204.Show();
                        label96.Show();
                        label97.Show();
                        label98.Show();
                        label99.Show();
                        label103.Show();
                        dateTimePicker4.Enabled = false;
                        dateTimePicker3.Enabled = false;
                        dateTimePicker11.Enabled = false;
                        button33.Hide();

                        break;
                }
                btn_verwijder.Hide();
                
                if (enum_nr != 4)
                {
                    this.btnwijzig.Show();
                    this.btnopslaan.Hide();
                }
                OrganizePanels(tab);
            }
        }
        private void do_edit()
        {
            using (Panel tmp_P = new Panel())
            {
                tmp_P.Location = panel5.Location;
                tmp_P.Size = panel5.Size;
                tmp_P.BackColor = System.Drawing.SystemColors.ControlLightLight;
                tmp_P.BorderStyle = BorderStyle.Fixed3D;
                this.Controls.Add(tmp_P);
                tmp_P.Show();
                tmp_P.BringToFront();

                switch (tab)
                {
                    case 3:
                        //tab 3
                        show_all_containers(panel7);
                        label184.Show();
                        label185.Show();
                        label186.Show();
                        label187.Show();
                        label188.Show();
                        label189.Show();
                        label190.Show();
                        label191.Show();
                        
                        label3.Show();
                        label1.Show();

                        linkLabel26.Hide();
                        linkLabel25.Hide();
                        textBox61.Show();
                        textBox63.Show();
                        dateTimePicker5.Show();
                        dateTimePicker1.Show();
                        dateTimePicker6.Show();
                        dateTimePicker7.Show();
                        textBox13.Show();
                        textBox14.Show();

                        checkBox1.Show();
                        dateTimePicker8.Show();
                        dateTimePicker2.Show();
                        dateTimePicker9.Show();
                        dateTimePicker10.Show();
                        textBox23.Show();
                        textBox18.Show();
                        textBox26.Show();
                        textBox30.Show();
                        textBox31.Show();
                        textBox27.Show();
                        textBox37.Show();
                        textBox38.Show();
                        textBox36.Show();
                        textBox40.Show();
                        textBox41.Show();
                        textBox39.Show();
                        textBox43.Show();
                        textBox44.Show();
                        textBox42.Show();
                        textBox51.Show();
                        textBox52.Show();
                        textBox50.Show();
                        textBox54.Show();
                        textBox56.Show();
                        textBox53.Show();
                        textBox58.Show();
                        textBox59.Show();
                        textBox57.Show();
                        label89.Show();
                        label91.Show();
                        label118.Show();
                        label158.Show();
                        label157.Show();
                        label119.Show();
                        label162.Show();
                        label161.Show();
                        label160.Show();
                        label165.Show();
                        label164.Show();
                        label163.Show();
                        label171.Show();
                        label167.Show();
                        label166.Show();
                        label174.Show();
                        label173.Show();
                        label172.Show();
                        label177.Show();
                        label176.Show();
                        label175.Show();
                        label180.Show();
                        label179.Show();
                        label178.Show();
                        label144.Hide();
                        label145.Hide();
                        label146.Hide();
                        label147.Hide();

                        label149.Hide();
                        label150.Hide();
                        label151.Hide();
                        label152.Hide();
                        label153.Hide();
                        label154.Hide();
                        label155.Hide();


                        dateTimePicker1.Enabled = true;
                        dateTimePicker2.Enabled = true;
                        dateTimePicker5.Enabled = true;
                        dateTimePicker6.Enabled = true;
                        dateTimePicker7.Enabled = true;
                        dateTimePicker8.Enabled = true;
                        dateTimePicker9.Enabled = true;
                        dateTimePicker10.Enabled = true;

                        break;
                    case 2:
                        //tab 2
                        show_all_containers(panel6);
                        // meerwerk
                        radiominbtn.Show(); radioplusbtn.Show();
                        button15.Show();
                        button22.Show();
                        button3.Show();
                        button21.Show();
                        button23.Show();
                        mcb_pers12.Show();
                        mcb_pers13.Show();
                        mcb_pers14.Show();
                        mcb_pers15.Show();
                        mcb_bdr4.Show();
                        mcb_bdr5.Show();
                        mcb_bdr6.Show();
                        mcb_bdr7.Show();
                        mcb_bdr8.Show();
                        mcb_bdr9.Show();
                        textBox66.Show();
                        textBox65.Show();
                        comboBox33.Show();
                        button24.Show();
                        button13.Show();
                        button25.Show();
                        button14.Show();
                        button26.Show();
                        textBox2.Show();
                        textBox1.Show();
                        textBox3.Show();
                        textBox8.Show();
                        textBox17.Show();
                        textBox21.Show();
                        textBox24.Show();
                        textBox22.Show();
                        radioButton20.Show(); radioButton21.Show();
                        radioButton12.Show(); radioButton13.Show();
                        radioButton14.Show(); radioButton15.Show();
                        radioButton10.Show(); radioButton11.Show();
                        textBox20.Show();
                        textBox19.Show();
                        textBox55.Show();
                        textBox32.Show();
                        textBox33.Show();
                        textBox34.Show();
                        textBox35.Show();
                        textBox47.Show();
                        radioButton1.Show();
                        radioButton2.Show();
                        radioButton16.Show(); radioButton17.Show();
                        radioButton18.Show(); radioButton19.Show();
                        radioButton3.Show();
                        radioButton4.Show();
                        radioButton5.Show();
                        radioButton6.Show();
                        radioButton7.Show();
                        radioButton8.Show();
                        radioButton9.Show();
                        label124.Hide();
                        label125.Hide();
                        label131.Hide();
                        label205.Hide();
                        label206.Hide();
                        label207.Hide();
                        label208.Hide();
                        label209.Hide();
                        label210.Hide();
                        label211.Hide();
                        label212.Hide();
                        label126.Hide();
                        label127.Hide();
                        label128.Hide();
                        label129.Hide();
                        label130.Hide();
                        label135.Hide();
                        label131.Hide();
                        label132.Hide();
                        label133.Hide();
                        label134.Hide();
                        label135.Hide();
                        label136.Text = "€";
                        label137.Text = "€";
                        label138.Hide();
                        label139.Hide();
                        label213.Hide();
                        label214.Hide();
                        label170.Hide();
                        label140.Hide();
                        label141.Hide();
                        label140.Hide();
                        label141.Hide();
                        label142.Hide();
                        label143.Hide();
                        label142.Hide();
                        label143.Hide();
                        label218.Hide();
                        label17.Show();
                        label24.Show();
                        label26.Show();
                        label27.Show();
                        label28.Show();
                        label29.Show();
                        label30.Show();
                        label38.Show();
                        label31.Show();
                        label40.Show();
                        label216.Show();
                        label2.Show();
                        label8.Show();

                        break;
                    case 1:
                        //tab1
                        show_all_containers(panel5);
                        elementHost1.Visible = true;
                        elementHost2.Visible = true;
                        textBox7.Show();
                        linkLabel28.Hide();
                        textBox60.Show();
                        label181.Show();
                        label182.Show();
                        label183.Show();
                        label58.Show();
                        label59.Show();
                        label74.Show();
                        textBox12.Show();
                        textBox15.Show();
                        textBox16.Show();
                        label83.Show();
                        label84.Show();
                        label85.Show();
                        label86.Show();
                        textBox6.Show();
                        button27.Show();
                        button20.Show();
                        button19.Show();
                        button1.Show();
                        button2.Show();
                        button4.Show();
                        button5.Show();
                        button6.Show();
                        textBox9.Show();
                        comboBox31.Show();
                        textBox10.Show();
                        textBox28.Show();
                        textBox29.Show();
                        textBox11.Show();
                        textBox45.Show();
                        dateTimePicker3.Show();
                        dateTimePicker4.Show();
                        dateTimePicker11.Show();
                        button7.Show();
                        button8.Show();
                        button9.Show();
                        button10.Show();
                        button11.Show();
                        button12.Show();
                        textBox5.Show();
                        textBox62.Show();
                        textBox4.Show();
                        textBox67.Show();
                        textBox25.Show();
                        mcb_pers1.Show();
                        mcb_pers2.Show();
                        mcb_pers3.Show();
                        mcb_pers4.Show();
                        mcb_pers5.Show();
                        mcb_pers6.Show();
                        mcb_pers7.Show();
                        mcb_pers8.Show();
                        mcb_pers9.Show();
                        mcb_pers10.Show();
                        mcb_pers11.Show();
                        mcb_bdr1.Show();
                        mcb_bdr2.Show();
                        mcb_bdr3.Show();

                        label168.Hide();
                        label42.Hide();
                        label69.Hide();
                        label70.Hide();
                        label100.Hide();
                        label101.Hide();
                        label102.Hide();
                        label104.Hide();
                        label105.Hide();
                        label106.Hide();
                        label90.Hide();
                        label92.Hide();
                        label93.Hide();
                        label94.Hide();
                        label95.Hide();
                        label107.Hide();
                        label123.Hide();
                        label201.Hide();
                        label202.Hide();
                        label203.Hide();
                        label204.Hide();
                        label96.Hide();
                        label97.Hide();
                        label98.Hide();
                        label99.Hide();
                        label103.Hide();
                        dateTimePicker4.Enabled = true;
                        dateTimePicker3.Enabled = true;
                        dateTimePicker11.Enabled = true;
                        if (!elementHost5.Visible || !elementHost4.Visible || !elementHost6.Visible)
                        {
                            button33.Show();
                        }
                        telefoonnummer_user_control1.Api_get_areacode(adres_user_control1.Postcode_cijfers + adres_user_control1.Postcode_letters);
                        telefoonnummer_user_control2.Api_get_areacode(adres_user_control1.Postcode_cijfers + adres_user_control1.Postcode_letters);
                        telefoonnummer_user_control3.Api_get_areacode(adres_user_control1.Postcode_cijfers + adres_user_control1.Postcode_letters);
                        break;
                }
                btn_verwijder.Show();
                this.btnwijzig.Hide();
                this.btnopslaan.Show();
                OrganizePanels(tab);
            }
        }
        static DateTime FirstDateOfWeek(int year, int weekNum)// van jaar en weeknummer naar datum
        {
            DateTime firstMonday = DateTime.Now;
           
            DateTime jan1 = new DateTime(year, 1, 1);
            if((int)jan1.DayOfWeek <= 1)
            {
              firstMonday = jan1.AddDays(1 - (int)jan1.DayOfWeek);
            }
            else if ((int)jan1.DayOfWeek > 1)
            {
               firstMonday =  jan1.AddDays(7 -((int)jan1.DayOfWeek - 1)); 

            }
            int[] ar =GetWeekNumber(firstMonday);
           
            return firstMonday.AddDays((weekNum - ar[0])*7);
        }
        public static int[] GetWeekNumber(DateTime dt)
        {
            Calendar cal = CultureInfo.InvariantCulture.Calendar;
            DayOfWeek day = cal.GetDayOfWeek(dt);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                dt = dt.AddDays(3);
            }
            int weeknum, jaar;
            weeknum = cal.GetWeekOfYear(dt, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            jaar = dt.Year;
            if ((weeknum >= 52) && (dt.Month < 12)) { jaar--; }
            int[] ar = new int[] { weeknum, jaar };
           
            return ar;
          
        }
        private void Select_Value_Mcombobox(MultiColumnComboBoxDemo.MultiColumnComboBox mcb, Label LL, int tv, int vtype)
        {
            int testvalue = tv;
            bool done = false;
            foreach (DataRow dr in ((DataTable)mcb.DataSource).Rows)
            {
                if (vtype == 2)
                {
                    if ((int)dr[0] == testvalue)
                    {
                        mcb.SelectedValue = testvalue;
                        DataRowView drv = (DataRowView)mcb.SelectedItem;
                        LL.Text = drv.Row.ItemArray[4].ToString();
                        done = true;
                    }
                }
                else if (vtype == 3)
                {
                    if ((int)dr[0] == testvalue)
                    {
                        mcb.SelectedValue = testvalue;
                        DataRowView drv = (DataRowView)mcb.SelectedItem;
                        LL.Text = drv.Row.ItemArray[2].ToString();
                        done = true;

                    }
                }
            }
            if (!done)
            {
                LL.Text = "-";
            }
        }
        private void set_telnr_type(string settingss)
        {
            string[] types = settingss.Split(',');
            if (types.Length > 0)
            {
                telefoonnummer_user_control1.set_naam(int.Parse(types[0]), false);
            }
            if (types.Length > 1)
            {
                telefoonnummer_user_control2.set_naam(int.Parse(types[1]), false);
            }
            if (types.Length > 2)
            {
                telefoonnummer_user_control3.set_naam(int.Parse(types[2]), false);
            }
        }
        private void NavigateRecord()
        {

              try
              {

                  init_comboNToolTip(true);

                  DateTime tempd = new DateTime();



                 
                  table = adapter.GetData(int.Parse(id));
                  testrow = (testset.projectRow)table.Rows[0];
                  ValidateRow = table.NewprojectRow();
                  ValidateRow.ItemArray = testrow.ItemArray;
                  indexId= testrow.project_ID;
                  //tab1
                  //project informatie
                  //projectnaam
                  if (!testrow.Isnaam_projectNull())
                  {
                      string naam = String.Empty;
                      if (testrow.naam_project.Contains(Environment.NewLine))
                      {
                          naam = testrow.naam_project.Replace(Environment.NewLine, " ");
                      }
                      else
                      {
                          naam = testrow.naam_project;
                      }
                      label42.Text = naam;
                      textBox6.Text = naam;
                      lbl_bot.Text = " ID " + id + " - " + naam;
                      vrijgeefstring = naam;
                      variabele_set1.set_projnm(vrijgeefstring);
                      variabele_set2.set_projnm(vrijgeefstring);
                  }
                  else
                  {
                      label42.Parent.Parent.Hide();
                      label42.Text = "-";
                  }
                  testsetTableAdapters.new_del_record_logTableAdapter NdrL_adapt = new testsetTableAdapters.new_del_record_logTableAdapter();
                  NdrL_adapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
                  testset.new_del_record_logDataTable NdrL_dt = NdrL_adapt.GetData(int.Parse(id), "project");
                  if (NdrL_dt.Rows.Count > 0)
                  {
                      testset.new_del_record_logRow NdrL_row = (testset.new_del_record_logRow)NdrL_dt.Rows[0];
                      if (!NdrL_row.IsnaamNull() && !NdrL_row.Isdatum_tijdNull())
                      {
                          label193.Text = "Dit project is aangemaakt door: " + NdrL_row.naam + " op: " + NdrL_row.datum_tijd.ToString();
                      }
                      else
                      {
                          label193.Text = "Het is niet bekend wanneer en door wie dit project is ingevoerd";
                      }
                  }
                  else
                  {
                      label193.Text = "Het is niet bekend wanneer en door wie dit project is ingevoerd";
                  }
                  //projectnaam specificatie
                  if (memoadapt.Exists(int.Parse(id), 1, 1) > 0)
                  {
                      memotabl = memoadapt.GetData(int.Parse(id), 1, 1);
                      memoRow = (testset.memosRow)memotabl.Rows[0];
                      textBox60.Text = memoRow.inhoud;
                      linkLabel27.Text = memoRow.inhoud;
                      label192.Text = memoRow.inhoud;
                      show_mem_btn(textBox60, button29);
                  }
                  else
                  {
                      label192.Parent.Parent.Hide();
                      show_mem_btn(textBox60, button29);
                  }
                  //opdrachtgever
                  if (!testrow.IsopdrachtgeverZEEBREGTS_nrNull() && testrow.opdrachtgeverZEEBREGTS_nr != 0)
                  {
                      Select_Value_Mcombobox(mcb_bdr1, label69, testrow.opdrachtgeverZEEBREGTS_nr, 2);
                      // mcb_bdr1.SelectedValue = testrow.opdrachtgeverZEEBREGTS_nr;
                      // DataRowView drv = (DataRowView)mcb_bdr1.SelectedItem;
                      // linkLabel1.Text = drv.Row.ItemArray[4].ToString();
                  }
                  else
                  {
                      label69.Parent.Parent.Hide();
                      label69.Text = "-";
                  }
                  

                  //bouwbedrijf
                  if (!testrow.Isbouwbedrijf_nrNull() && testrow.bouwbedrijf_nr != 0)
                  {
                      Select_Value_Mcombobox(mcb_bdr2,label70, testrow.bouwbedrijf_nr, 2);
                     // mcb_bdr2.SelectedValue = testrow.bouwbedrijf_nr;
                      //DataRowView drv = (DataRowView)mcb_bdr2.SelectedItem;
                     // linkLabel2.Text = drv.Row.ItemArray[4].ToString();

                  }
                  else
                  {
                      label70.Parent.Parent.Hide();
                      label70.Text = "-";
                  }
                  //projectontwikkelaar
                  if (!testrow.Isprojectontwikkelaar_nrNull() && testrow.projectontwikkelaar_nr != 0)
                  {
                      Select_Value_Mcombobox(mcb_bdr3,label100, testrow.projectontwikkelaar_nr, 2);
                      //mcb_bdr3.SelectedValue = testrow.projectontwikkelaar_nr;
                     // DataRowView drv = (DataRowView)mcb_bdr3.SelectedItem;
                     // linkLabel3.Text = drv.Row.ItemArray[4].ToString();

                  }
                  else
                  {
                      label100.Parent.Parent.Hide();
                      label100.Text = "-";
                  }
                  //inkoper
                  if (!testrow.Isinkoper_nrNull() && testrow.inkoper_nr != 0)
                  {
                      Select_Value_Mcombobox(mcb_pers1,label101, testrow.inkoper_nr, 3);
                      //mcb_pers1.SelectedValue = testrow.inkoper_nr;
                     // DataRowView drv = (DataRowView)mcb_pers1.SelectedItem;
                     // linkLabel4.Text = drv.Row.ItemArray[2].ToString();
                  }
                  else
                  {
                      label101.Parent.Parent.Hide();
                      label101.Text = "-";
                  }
                  //werkvoorbereider
                  if (!testrow.Iswerkvoorbereider_nrNull() && testrow.werkvoorbereider_nr != 0)
                  {
                      Select_Value_Mcombobox(mcb_pers2,label102, testrow.werkvoorbereider_nr, 3);
                      //mcb_pers2.SelectedValue = testrow.werkvoorbereider_nr;
                      //DataRowView drv = (DataRowView)mcb_pers2.SelectedItem;
                      //linkLabel5.Text = drv.Row.ItemArray[2].ToString();
                  }
                  else
                  {
                      label102.Parent.Parent.Hide();
                      label102.Text = "-";
                  }
                  //projectleider
                  if (!testrow.Isprojectleider_nrNull() && testrow.projectleider_nr != 0)
                  {
                      Select_Value_Mcombobox(mcb_pers3,label104, testrow.projectleider_nr, 3);
                      //mcb_pers3.SelectedValue = testrow.projectleider_nr;
                      //DataRowView drv = (DataRowView)mcb_pers3.SelectedItem;
                      //linkLabel6.Text = drv.Row.ItemArray[2].ToString();
                  }
                  else
                  {
                      label104.Parent.Parent.Hide();
                      label104.Text = "-";
                  }
                  //uitvoerder
                  if (!testrow.Isuitvoerder_nrNull() && testrow.uitvoerder_nr != 0)
                  {
                      Select_Value_Mcombobox(mcb_pers4,label105, testrow.uitvoerder_nr, 3);
                      //mcb_pers4.SelectedValue = testrow.uitvoerder_nr;
                      //DataRowView drv = (DataRowView)mcb_pers4.SelectedItem;
                      //linkLabel7.Text = drv.Row.ItemArray[2].ToString();
                  }
                  else
                  {
                      label105.Parent.Parent.Hide();
                      label105.Text = "-";
                  }
                  //opzichter
                  if (!testrow.Is_opzichter_persoon_nrNull() && testrow._opzichter_persoon_nr != 0)
                  {
                      Select_Value_Mcombobox(mcb_pers5,label106, testrow._opzichter_persoon_nr, 3);
                      //mcb_pers5.SelectedValue = testrow._opzichter_persoon_nr;
                      //DataRowView drv = (DataRowView)mcb_pers5.SelectedItem;
                      //linkLabel8.Text = drv.Row.ItemArray[2].ToString();
                  }
                  else
                  {
                      label106.Parent.Parent.Hide();
                      label106.Text = "-";
                  }

                  //contract
                  //projectnr extern
                  if (!testrow.Isaannemer_projectnummerNull())
                  {
                      label96.Text = testrow.aannemer_projectnummer;
                      textBox5.Text = testrow.aannemer_projectnummer;
                  }
                  else
                  {
                      label96.Parent.Parent.Hide();
                      label96.Text = "-";
                  }
                  //contractnr
                  if (!testrow.Isaannemer_contractnummerNull())
                  {
                      label97.Text = testrow.aannemer_contractnummer;
                      textBox62.Text = testrow.aannemer_contractnummer;
                  }
                  else
                  {
                      label97.Parent.Parent.Hide();
                      label97.Text = "-";
                  }
                  //nacalculatiecode
                  if (!testrow.IsnacalculatiecodeNull())
                  {
                      label98.Text = testrow.nacalculatiecode;
                      textBox4.Text = testrow.nacalculatiecode;
                  }
                  else
                  {
                      label98.Parent.Parent.Hide();
                      label98.Text = "-";
                  }

                 //telnrs settings
                  if (!testrow.Istelefoon_nr_settingsNull())
                  {
                      set_telnr_type(testrow.telefoon_nr_settings);
                  }
                  //tel1
                  if (!testrow.Istelefoon_nr_1Null())
                  {
                      elementHost5.Show();
                      telefoonnummer_user_control1.set_Nummer(testrow.telefoon_nr_1, this);
                  }
                  else
                  {
                      telefoonnummer_user_control1.set_Nummer("", this);
                      elementHost5.Hide();
                  }
                  if (!testrow.Istelefoon_nr_2Null())
                  {
                      elementHost4.Show();
                      telefoonnummer_user_control2.set_Nummer(testrow.telefoon_nr_2, this);
                  }
                  else
                  {
                      telefoonnummer_user_control2.set_Nummer("", this);
                      elementHost4.Hide();
                  }
                  if (!testrow.Istelefoon_nr_3Null())
                  {
                      elementHost6.Show();
                      telefoonnummer_user_control3.set_Nummer(testrow.telefoon_nr_3, this);
                  }
                  else
                  {
                      telefoonnummer_user_control3.set_Nummer("", this);
                      elementHost6.Hide();
                  }
                  //naw nieuwe stijl
                  Navigate_adres();
                  //website
                  if (linkLabel28.Links.Count > 0)
                  {
                      linkLabel28.Links.Remove(linkLabel28.Links[0]);
                  }
                  if (!testrow.IswebsiteNull())
                  {
                      linkLabel28.Text = testrow.website;
                      textBox7.Text = testrow.website;
                      linkLabel28.Links.Add(0, testrow.website.Length, testrow.website);
                  }
                  else
                  {
                      linkLabel28.Parent.Parent.Hide(); 
                      linkLabel28.Text = "-";
                      textBox7.Text = String.Empty;
                  }
                  //factuur informatie
                  //aanhef
                  if (!testrow.Isfactuur_aanhefNull())
                  {
                      label99.Text = testrow.factuur_aanhef;
                      textBox67.Text = testrow.factuur_aanhef;
                  }
                  else
                  {
                      label99.Parent.Parent.Hide();
                      label99.Text = "-";
                  }
                  //adres
                  //betalingstermijn

                  if (!testrow.IsbetalingstermijnNull() && testrow.betalingstermijn != 0)
                  {
                      label103.Text = testrow.betalingstermijn.ToString();
                      textBox25.Text = testrow.betalingstermijn.ToString();
                  }
                  else
                  {
                      label103.Parent.Parent.Hide();
                      label103.Text = "-";
                  }
                  // factuur nieuwe stijl
                  
                  //
                  //planning
                  //status 22
                  if (!testrow.IsstatusNull() && testrow.status != 0)
                  {
                      comboBox31.SelectedValue = testrow.status;
                      label90.Text = comboBox31.Text;
                  }
                  else
                  {
                      label90.Parent.Parent.Hide();
                      label90.Text = "-";
                  }
                  //start
                  if (!testrow.Isplanning_start_weekNull() && testrow.planning_start_week != 0)
                  {
                      int week = testrow.planning_start_week;
                      int jaar = testrow.planning_start_jaar;
                      if (week >= 1 || jaar >= 1900)
                      { tempd = FirstDateOfWeek(jaar, week); }
                      textBox10.Text = jaar.ToString();
                      textBox28.Text = week.ToString();
                      label92.Text = "week: " + week + " jaar: " + jaar;
                  }
                  else
                  {
                      label92.Parent.Parent.Hide();
                      textBox10.Text = "";
                      label92.Text = "-";
                  }
                  //eind
                  if (!testrow.Isplanning_eind_weekNull() && testrow.planning_eind_week != 0)
                  {
                      int week = testrow.planning_eind_week;
                      int jaar = testrow.planning_eind_jaar;
                      if (week >= 1 || jaar >= 1900)
                      { tempd = FirstDateOfWeek(jaar, week); }
                      textBox11.Text = jaar.ToString();
                      textBox29.Text = week.ToString();
                      label93.Text = "week: " + week + " jaar: " + jaar;
                  }
                  else
                  {
                      label93.Parent.Parent.Hide();
                      textBox11.Text = "";
                      label93.Text = "-";
                  }
                  //bouwsnelheid
                  if (!testrow.Isbouw_snelheidNull() && testrow.bouw_snelheid != 0)
                  {
                      label94.Text = testrow.bouw_snelheid.ToString();
                      textBox45.Text = testrow.bouw_snelheid.ToString();
                  }
                  else
                  {
                      label94.Parent.Parent.Hide();
                      label94.Text = "-";
                  }
                  //bevestiging(startcheck)38 !!!!!!!!!!
                  if (!testrow.Isstart_checkNull())
                  {
                      dateTimePicker11.Value = testrow.start_check;
                      textBox16.Text = dateTimePicker11.Value.Day.ToString();
                      textBox15.Text = dateTimePicker11.Value.Month.ToString();
                      textBox12.Text = dateTimePicker11.Value.Year.ToString();
                      label95.Text = dateTimePicker11.Value.ToLongDateString();
                  }
                  else
                  {
                      label95.Parent.Parent.Hide();
                      label95.Text = "-"; }
                  //betrokken intern
                  //offerte
                  if (!testrow.Isofferte_persoon_nrNull() && testrow.offerte_persoon_nr != 0)
                  {
                      Select_Value_Mcombobox(mcb_pers6,label107, testrow.offerte_persoon_nr, 3);
                     // mcb_pers6.SelectedValue = testrow.offerte_persoon_nr;
                      //DataRowView drv = (DataRowView)mcb_pers6.SelectedItem;
                      //linkLabel9.Text = drv.Row.ItemArray[2].ToString();
                  }
                  else
                  {
                      label107.Parent.Parent.Hide();
                      label107.Text = "-";
                  }
                  //0offerte
                  if (!testrow.Is_0offerte_persoon_nrNull() && testrow._0offerte_persoon_nr != 0)
                  {
                      Select_Value_Mcombobox(mcb_pers7,label123, testrow._0offerte_persoon_nr, 3);
                      //mcb_pers7.SelectedValue = testrow._0offerte_persoon_nr;
                      //DataRowView drv = (DataRowView)mcb_pers7.SelectedItem;
                      //linkLabel10.Text = drv.Row.ItemArray[2].ToString();
                  }
                  else
                  {
                      label123.Parent.Parent.Hide();
                      label123.Text = "-";
                  }
                  //meerwerk
                  if (!testrow.Iskoperofferte_persoon_nrNull() && testrow.koperofferte_persoon_nr != 0)
                  {
                      Select_Value_Mcombobox(mcb_pers8,label201, testrow.koperofferte_persoon_nr, 3);
                      //mcb_pers8.SelectedValue = testrow.koperofferte_persoon_nr;
                      //DataRowView drv = (DataRowView)mcb_pers8.SelectedItem;
                      //linkLabel11.Text = drv.Row.ItemArray[2].ToString();
                  }
                  else
                  {
                      label201.Parent.Parent.Hide();
                      label201.Text = "-";
                  }
                  //operationeel
                  if (!testrow.IsuitvoerderAfbouw_persoon_nrNull() && testrow.uitvoerderAfbouw_persoon_nr != 0)
                  {
                      Select_Value_Mcombobox(mcb_pers9,label202, testrow.uitvoerderAfbouw_persoon_nr, 3);
                      //mcb_pers9.SelectedValue = testrow.uitvoerderAfbouw_persoon_nr;
                      //DataRowView drv = (DataRowView)mcb_pers9.SelectedItem;
                      //linkLabel12.Text = drv.Row.ItemArray[2].ToString();
                  }
                  else
                  {
                      label202.Parent.Parent.Hide();
                      label202.Text = "-";
                  }
                  //uitvoerder
                  if (!testrow.IsuitvoerderZeebregts_persoon_nrNull() && testrow.uitvoerderZeebregts_persoon_nr != 0)
                  {
                      Select_Value_Mcombobox(mcb_pers10,label203, testrow.uitvoerderZeebregts_persoon_nr, 3);
                      //mcb_pers10.SelectedValue = testrow.uitvoerderZeebregts_persoon_nr;
                      //DataRowView drv = (DataRowView)mcb_pers10.SelectedItem;
                      //linkLabel13.Text = drv.Row.ItemArray[2].ToString();
                  }
                  else
                  {
                      label203.Parent.Parent.Hide();
                      label203.Text = "-";
                  }

                  //tegelzetter
                  if (!testrow.Istegelzetter_persoon_nrNull() && testrow.tegelzetter_persoon_nr != 0)
                  {
                      Select_Value_Mcombobox(mcb_pers11,label204, testrow.tegelzetter_persoon_nr, 3);
                      //mcb_pers11.SelectedValue = testrow.tegelzetter_persoon_nr;
                      //DataRowView drv = (DataRowView)mcb_pers11.SelectedItem;
                      //linkLabel14.Text = drv.Row.ItemArray[2].ToString(); ;
                  }
                  else
                  {
                      label204.Parent.Parent.Hide();
                      label204.Text = "-";
                  }


                  //administratienr
                  if (!testrow.IsprojectNRadminZeebregtsNull())
                  {
                      label168.Text = testrow.projectNRadminZeebregts;
                      textBox9.Text = testrow.projectNRadminZeebregts;
                  }
                  else
                  {
                      label168.Parent.Parent.Hide();
                      label168.Text = "-";
                  }

                  //tab2 bedrijfsbureau
                  //meerwerk

                  //omschrijving
                  if (!testrow.IsomschrijvingKoopwoningenNull())
                  {
                      label124.Text = testrow.omschrijvingKoopwoningen;
                      textBox66.Text = testrow.omschrijvingKoopwoningen;
                  }
                  else
                  {
                      label124.Parent.Parent.Hide();
                      label124.Text = "-";
                  }
                  //aantal woningen
                  if (!testrow.Isaantal_koopwoningenNull() && testrow.aantal_koopwoningen != 0)
                  {
                      label125.Text = testrow.aantal_koopwoningen.ToString();
                      textBox65.Text = testrow.aantal_koopwoningen.ToString();
                  }
                  else
                  {
                      label125.Parent.Parent.Hide();
                      label125.Text = "-";
                  }
                  //tegels
                  if (!testrow.Istegelshowroom_nrNull() && testrow.tegelshowroom_nr != 0)
                  {
                      Select_Value_Mcombobox(mcb_bdr4,label205, testrow.tegelshowroom_nr, 2);
                      //mcb_bdr4.SelectedValue = testrow.tegelshowroom_nr;
                      //DataRowView drv = (DataRowView)mcb_bdr4.SelectedItem;
                      //linkLabel15.Text = drv.Row.ItemArray[4].ToString();

                  }
                  else
                  {
                      label205.Parent.Parent.Hide();
                      label205.Text = "-";
                  }
                  //tegels persoon
                  if (!testrow.IscontactpersoonTegelshowroom_nrNull() && testrow.contactpersoonTegelshowroom_nr != 0)
                  {
                      Select_Value_Mcombobox(mcb_pers12,label206, testrow.contactpersoonTegelshowroom_nr, 3);
                      //mcb_pers12.SelectedValue = testrow.contactpersoonTegelshowroom_nr;
                      //DataRowView drv = (DataRowView)mcb_pers12.SelectedItem;
                      //linkLabel16.Text = drv.Row.ItemArray[2].ToString();
                  }
                  else
                  {
                      label206.Parent.Parent.Hide();
                      label206.Text = "-";
                  }
                  //sanitair
                  if (!testrow.Issanitairshowroom_nrNull() && testrow.sanitairshowroom_nr != 0)
                  {
                      Select_Value_Mcombobox(mcb_bdr5,label207, testrow.sanitairshowroom_nr, 2);
                      //mcb_bdr5.SelectedValue = testrow.sanitairshowroom_nr;
                      //DataRowView drv = (DataRowView)mcb_bdr5.SelectedItem;
                      //linkLabel17.Text = drv.Row.ItemArray[4].ToString();

                  }
                  else
                  {
                      label207.Parent.Parent.Hide();
                      label207.Text = "-";
                  }
                  //sanitair persoon
                  if (!testrow.IscontactpersoonSANITAIRshowroom_nrNull() && testrow.contactpersoonSANITAIRshowroom_nr != 0)
                  {
                      Select_Value_Mcombobox(mcb_pers13,label208, testrow.contactpersoonSANITAIRshowroom_nr, 3);
                      //mcb_pers13.SelectedValue = testrow.contactpersoonSANITAIRshowroom_nr;
                     // DataRowView drv = (DataRowView)mcb_pers13.SelectedItem;
                      //linkLabel18.Text = drv.Row.ItemArray[2].ToString();
                  }
                  else
                  {
                      label208.Parent.Parent.Hide();
                      label208.Text = "-";
                  }
                  //keukens
                  if (!testrow.Iskeukenshowroom_nrNull() && testrow.keukenshowroom_nr != 0)
                  {
                      Select_Value_Mcombobox(mcb_bdr6,label209, testrow.keukenshowroom_nr, 2);
                      //mcb_bdr6.SelectedValue = testrow.keukenshowroom_nr;
                      //DataRowView drv = (DataRowView)mcb_bdr6.SelectedItem;
                      //linkLabel19.Text = drv.Row.ItemArray[4].ToString();

                  }
                  else
                  {
                      label209.Parent.Parent.Hide();
                      label209.Text = "-";
                  }
                  //keukens persoon
                  if (!testrow.IscontactpersoonKEUKENshowroom_nrNull() && testrow.contactpersoonKEUKENshowroom_nr != 0)
                  {
                      Select_Value_Mcombobox(mcb_pers14,label210, testrow.contactpersoonKEUKENshowroom_nr, 3);
                      //mcb_pers14.SelectedValue = testrow.contactpersoonKEUKENshowroom_nr;
                      //DataRowView drv = (DataRowView)mcb_pers14.SelectedItem;
                      //linkLabel20.Text = drv.Row.ItemArray[2].ToString();
                  }
                  else
                  {
                      label210.Parent.Parent.Hide();
                      label210.Text = "-";
                  }
                  //begeleiding
                  if (!testrow.Iskopersbegeleidingbedrijf_nrNull() && testrow.kopersbegeleidingbedrijf_nr != 0)
                  {
                      Select_Value_Mcombobox(mcb_bdr7,label211, testrow.kopersbegeleidingbedrijf_nr, 2);
                      //mcb_bdr7.SelectedValue = testrow.kopersbegeleidingbedrijf_nr;
                      //DataRowView drv = (DataRowView)mcb_bdr7.SelectedItem;
                      //linkLabel21.Text = drv.Row.ItemArray[4].ToString();

                  }
                  else
                  {
                      label211.Parent.Parent.Hide();
                      label211.Text = "-";
                  }
                  //begeleiding persoon
                  if (!testrow.Iskopersbegeleider_nrNull() && testrow.kopersbegeleider_nr != 0)
                  {
                      Select_Value_Mcombobox(mcb_pers15,label212, testrow.kopersbegeleider_nr, 3);
                      //mcb_pers15.SelectedValue = testrow.kopersbegeleider_nr;
                     // DataRowView drv = (DataRowView)mcb_pers15.SelectedItem;
                      //linkLabel22.Text = drv.Row.ItemArray[2].ToString();
                  }
                  else
                  {
                      label212.Parent.Parent.Hide();
                      label212.Text = "-";
                  }

                  //ontvangen gegevens
                  //verkoopbrochure
                  if (!testrow.verkoopbrochure)
                      {
                          label128.Text = "niet ontvangen";
                          radioButton13.Checked = false; radioButton12.Checked = true;
                      }
                      else
                      {

                          label128.Text = "ontvangen";
                          radioButton13.Checked = true; radioButton12.Checked = false;
                      }
                  //NAW lijst
                  if (!testrow.NAWgegevens)
                      {
                          label129.Text = "niet ontvangen";
                          radioButton15.Checked = false; radioButton14.Checked = true;
                      }
                      else
                      {

                          label129.Text = "ontvangen";
                          radioButton15.Checked = true; radioButton14.Checked = false;
                      }
                  //tekeningen
                  if (!testrow._0_tekeningen)
                      {
                          label130.Text = "niet ontvangen";
                          radioButton10.Checked = true; radioButton11.Checked = false;
                      }
                      else
                      {

                          label130.Text = "ontvangen";
                          radioButton10.Checked = false; radioButton11.Checked = true;
                      }
                  
                  //instellingen-meerwerk
                  //methode
                  if (!testrow.IsverekenmethodeNull() && testrow.verekenmethode != 0)
                  {
                      comboBox33.SelectedValue = testrow.verekenmethode;
                      label131.Text = comboBox33.Text;
                  }
                  else
                  {
                      label131.Parent.Parent.Hide();
                      label131.Text = "-";
                  }
                  //rendement 1
                  if (!testrow.Is_prov_vast_aan_meerwerk_Null())
                  {
                      String prcnt = testrow._prov_vast_aan_meerwerk_.ToString("F", CultureInfo.CreateSpecificCulture("nl-NL"));
                       String[] prcnta = prcnt.Split(',');
                      label132.Text = prcnt + "%";
                      if (prcnta.Length > 0)
                      {
                          textBox20.Text = prcnta[0];
                          if (prcnta.Length > 1)
                          {
                              textBox3.Text = prcnta[1];
                          }
                      }
                      else
                      {
                          prcnta = prcnt.Split('.');
                          if (prcnta.Length > 0)
                          {
                              textBox20.Text = prcnta[0];
                              if (prcnta.Length > 1)
                              {
                                  textBox3.Text = prcnta[1];
                              }
                          }
                      }
                      
                  }
                  else
                  {
                      label132.Parent.Parent.Hide();
                      label132.Text = "-";
                  }
                  //rendement 2
                  if (!testrow.Is_prov_vast_aan_Null())
                  {
                      String prcnt = testrow._prov_vast_aan_.ToString("F", CultureInfo.CreateSpecificCulture("nl-NL"));
                      prcnt = prcnt.Replace('.', ',');
                      String[] prcnta = prcnt.Split(',');
                      label133.Text = prcnt + "%";
                      if (prcnta.Length > 0)
                      {
                          textBox19.Text = prcnta[0];
                          if (prcnta.Length > 1)
                          {
                              textBox8.Text = prcnta[1];
                          }
                      }
                      else
                      {
                          prcnta = prcnt.Split('.');
                          if (prcnta.Length > 0)
                          {
                              textBox19.Text = prcnta[0];
                              if (prcnta.Length > 1)
                              {
                                  textBox8.Text = prcnta[1];
                              }
                          }
                      }
                  }
                  else
                  {
                      label133.Parent.Parent.Hide();
                      label133.Text = "-";
                  }
                  //coordinatiekosten
                  if (!testrow.Is_coordiatiekostenbouwbedrijf_Null())
                  {
                      String prcnt = testrow._coordiatiekostenbouwbedrijf_.ToString("F",CultureInfo.CreateSpecificCulture("nl-NL"));
                      String[] prcnta = prcnt.Split(',');
                      label134.Text = prcnt + "%";
                      textBox55.Text = prcnta[0];
                      if (int.Parse(textBox55.Text) > -0.01)
                      {
                          radioplusbtn.Checked = true;
                      }
                      else
                      {
                          radiominbtn.Checked = true;
                          textBox55.Text = (int.Parse(textBox55.Text) * -1).ToString();
                      }
                      textBox17.Text = prcnta[1];
                  }
                  else
                  {
                      label134.Parent.Parent.Hide();
                      label134.Text = "-";
                  }
                  //NA werk
                  if (!testrow.IsFACTORnietAW_ZeebregtsNull())
                  {
                      if (testrow.FACTORnietAW_Zeebregts == 1)
                      {
                          label135.Text = "ja";
                          radioButton1.Checked = true;
                          radioButton2.Checked = false;
                      }
                      else
                      {
                          label135.Text = "nee";
                          radioButton1.Checked = false;
                          radioButton2.Checked = true;
                      }
                  }
                  //stelpost wand
                  if (!testrow.IsstelpostwandNull())
                  {
                      String spw = testrow.stelpostwand.ToString("F", CultureInfo.CreateSpecificCulture("nl-NL"));
                      spw = spw.Replace('.', ',');
                      String[] spwa = spw.Split(',');
                      label136.Text = "€ " + spw;
                      textBox32.Text = spwa[0];
                      textBox1.Text = spwa[1];
                  }
                  else
                  {
                      label136.Parent.Parent.Hide();
                      label136.Text = "-";
                  }
                  //stelpost vloer
                  if (!testrow.IsstelpostvloerNull())
                  {
                      String spv = testrow.stelpostvloer.ToString("F", CultureInfo.CreateSpecificCulture("nl-NL"));
                      spv = spv.Replace('.', ',');
                      String[] spva = spv.Split(',');
                      label137.Text = "€ " + spv;
                      textBox33.Text = spva[0];
                      textBox2.Text = spva[1];
                  }
                  else
                  {
                      label137.Parent.Parent.Hide();
                      label137.Text = "-";
                  }
                  //opslagprijs
                  if (!testrow.Isopslagprijs_koperNull())
                  {
                      double tmp = testrow.opslagprijs_koper * 100;
                      String prcnt = tmp.ToString("F", CultureInfo.CreateSpecificCulture("nl-NL"));
                      prcnt = prcnt.Replace('.', ',');
                      String[] prcnta = prcnt.Split(',');
                      label138.Text = prcnt + "%";
                      textBox34.Text = prcnta[0];
                      textBox21.Text = prcnta[1];
                  }
                  else
                  {
                      label138.Parent.Parent.Hide();
                      label138.Text = "-";
                  }
                  //opslag meters
                  if (!testrow.Isopslagmeters_koperNull())
                  {
                      double tmp = testrow.opslagmeters_koper * 100;
                      String prcnt = tmp.ToString("F", CultureInfo.CreateSpecificCulture("nl-NL"));
                      prcnt = prcnt.Replace('.', ',');
                      String[] prcnta = prcnt.Split(',');
                      label139.Text = prcnt + "%";
                      textBox35.Text = prcnta[0];
                      textBox24.Text = prcnta[1];
                  }
                  else
                  {
                      label139.Parent.Parent.Hide();
                      label139.Text = "-";
                  }
                  //facturatie
                  if (!testrow.IsfacturatieMEERWERKbedrijf_nrNull() && testrow.facturatieMEERWERKbedrijf_nr != 0)
                  {
                      Select_Value_Mcombobox(mcb_bdr8,label213, testrow.facturatieMEERWERKbedrijf_nr, 2);
                      //mcb_bdr8.SelectedValue = testrow.facturatieMEERWERKbedrijf_nr;
                     // DataRowView drv = (DataRowView)mcb_bdr8.SelectedItem;
                      //linkLabel24.Text = drv.Row.ItemArray[4].ToString();

                  }
                  else
                  {
                      label213.Parent.Parent.Hide();
                      label213.Text = "-";
                  }
                  //betalingbedrijf
                  if (!testrow.IsbetalingbedrijfNull() && testrow.betalingbedrijf != 0)
                  {
                      Select_Value_Mcombobox(mcb_bdr9,label214, testrow.betalingbedrijf, 2);
                      //mcb_bdr9.SelectedValue = testrow.betalingbedrijf;
                      //DataRowView drv = (DataRowView)mcb_bdr9.SelectedItem;
                      //linkLabel23.Text = drv.Row.ItemArray[4].ToString();

                  }
                  else
                  {
                      label214.Parent.Parent.Hide();
                      label214.Text = "-";
                  }
                  //verpakkingstoeslag
                  if (!testrow.IsverpakkingstoeslagUpgradeNull())
                  {
                      if (!testrow.verpakkingstoeslagUpgrade)
                      {
                          label170.Text = "nee";
                          radioButton17.Checked = false; radioButton16.Checked = true;
                      }


                      else
                      {

                          label170.Text = "ja";
                          radioButton17.Checked = true; radioButton16.Checked = false;
                      }
                  }
                  else
                  {
                      label170.Parent.Parent.Hide();
                      label170.Text = "-";
                  }
                  //BTW
                  label218.Text = testrow.btw + "%";
                  textBox47.Text = testrow.btw.ToString("F",CultureInfo.CreateSpecificCulture("nl-NL"));
                  //instellingen - 0 offerte
                  //switchcontract
                  if (!testrow.IsswitchcontractNull())
                  {
                      if (testrow.switchcontract == 1)
                      {
                          label140.Text = "via type";
                          radioButton4.Checked = true;
                          radioButton3.Checked = false;
                      }
                      else
                      {
                          label140.Text = "via woning";
                          radioButton4.Checked = false;
                          radioButton3.Checked = true;
                      }
                  }
                  else
                  {
                      label140.Parent.Parent.Hide();
                      label140.Text = "-";
                  }
                  //uitsplitsing
                  if (!testrow.UITvia0offerte)
                      {
                          label141.Text = "via meerwerk";
                          radioButton6.Checked = true;
                          radioButton5.Checked = false;
                      }
                      else
                      {
                          label141.Text = "via 0 offerte";
                          radioButton6.Checked = false;
                          radioButton5.Checked = true;
                      }
                  
                  //instellingen - kwaliteit
                  //STABU groep
                  if (!testrow.IsSTABU_groepNull() && testrow.STABU_groep != 0)
                  {
                      if (testrow.STABU_groep == 1)
                      {
                          label142.Text = "groep 1";
                          radioButton7.Checked = true;
                          radioButton8.Checked = false;
                          radioButton9.Checked = false;
                      }
                      else if (testrow.STABU_groep == 2)
                      {
                          label142.Text = "groep 2";
                          radioButton7.Checked = false;
                          radioButton8.Checked = true;
                          radioButton9.Checked = false;
                      }
                      else if (testrow.STABU_groep == 3)
                      {
                          label142.Text = "groep 3";
                          radioButton7.Checked = false;
                          radioButton8.Checked = false;
                          radioButton9.Checked = true;
                      }
                  }
                  else
                  {
                      label142.Parent.Parent.Hide();
                      label142.Text = "-";
                  }
                  //instelling STABU
                  if (!testrow.Is_STABU_groep_ja_neeNull())
                  {
                      if (!testrow._STABU_groep_ja_nee)
                      {

                          label143.Text = "nee";
                          radioButton19.Checked = false; radioButton18.Checked = true;
                      }

                      else
                      {

                          label143.Text = "ja";
                          radioButton19.Checked = true; radioButton18.Checked = false;
                      }
                  }
                  else
                  {
                      label143.Parent.Parent.Hide();
                      label143.Text = "-";
                  }
                  //instellingen - algemeen
                  //master
                  if (!testrow.Isexel_masterNull())
                  {
                      label126.Text = testrow.exel_master.ToString().Replace(",", ".");
                      textBox22.Text = testrow.exel_master.ToString().Replace(",", ".");
                  }
                  else
                  {
                      label126.Parent.Parent.Hide();
                      label126.Text = "-";
                  }
                  //statusoverzicht
                  if (!testrow.IsINafbouwCONTROLNull())
                  {
                      if (!testrow.INafbouwCONTROL)
                      {
                          label127.Text = "nee";
                          radioButton21.Checked = false; radioButton20.Checked = true;
                      }

                      else
                      {

                          label127.Text = "ja";
                          radioButton21.Checked = true; radioButton20.Checked = false;
                      }
                  }
                  else
                  {
                      label127.Parent.Parent.Hide();
                      label127.Text = "-";
                  }
                  //tab3 offerte
                  //offerte
                  //aanvraag

                  if (!testrow.Isoff_aanvraagNull())
                  {
                      dateTimePicker5.Value = testrow.off_aanvraag;
                      textBox26.Text = dateTimePicker5.Value.Day.ToString();
                      textBox18.Text = dateTimePicker5.Value.Month.ToString();
                      textBox23.Text = dateTimePicker5.Value.Year.ToString();
                      label144.Text = dateTimePicker5.Value.ToLongDateString();
                  }
                  else
                  {
                      label144.Parent.Parent.Hide();
                      label144.Text = "-"; }
                  //deadline
                  if (!testrow.Isdeadline_offNull())
                  {
                      dateTimePicker1.Value = testrow.deadline_off;
                      textBox27.Text = dateTimePicker1.Value.Day.ToString();
                      textBox31.Text = dateTimePicker1.Value.Month.ToString();
                      textBox30.Text = dateTimePicker1.Value.Year.ToString();
                      label145.Text = dateTimePicker1.Value.ToLongDateString();
                  }
                  else
                  {
                      label145.Parent.Parent.Hide();
                      label145.Text = "-"; }
                  //verstuurd
                  if (!testrow.Isoff_verstuurdNull())
                  {
                      dateTimePicker6.Value = testrow.off_verstuurd;
                      label146.Text = dateTimePicker6.Value.ToLongDateString();
                      textBox36.Text = dateTimePicker6.Value.Day.ToString();
                      textBox38.Text = dateTimePicker6.Value.Month.ToString();
                      textBox37.Text = dateTimePicker6.Value.Year.ToString();
                  }
                  else
                  {
                      label146.Parent.Parent.Hide();
                      label146.Text = "-"; }
                  //definitief
                  if (!testrow.Isoff_defNull())
                  {
                      dateTimePicker7.Value = testrow.off_def;
                      label147.Text = dateTimePicker7.Value.ToLongDateString();
                      textBox39.Text = dateTimePicker7.Value.Day.ToString();
                      textBox41.Text = dateTimePicker7.Value.Month.ToString();
                      textBox40.Text = dateTimePicker7.Value.Year.ToString();
                  }
                  else
                  {
                      label147.Parent.Parent.Hide();
                      label147.Text = "-"; }
                  //memo
                  if (memoadapt.Exists(int.Parse(id),1,2) > 0)
                  {
                      memotabl = memoadapt.GetData(int.Parse(id), 1, 2);
                      memoRow = (testset.memosRow)memotabl.Rows[0];
                      textBox61.Text = memoRow.inhoud;
                      linkLabel25.Text = memoRow.inhoud;
                      show_mem_btn(textBox61, button16);

                  }
                  else
                  {
                      textBox61.Text = String.Empty;
                      show_mem_btn(textBox61, button16);
                      linkLabel25.Text = String.Empty;
                      linkLabel25.Parent.Parent.Hide();
                  }
                  //0 offerte
                  //aanvraag
                  if (!testrow.Is_0off_aanvraagNull())
                  {
                      dateTimePicker8.Value = testrow._0off_aanvraag;
                      label149.Text = dateTimePicker8.Value.ToLongDateString();
                      textBox42.Text = dateTimePicker8.Value.Day.ToString();
                      textBox44.Text = dateTimePicker8.Value.Month.ToString();
                      textBox43.Text = dateTimePicker8.Value.Year.ToString();
                  }
                  else
                  {
                      label149.Parent.Parent.Hide();
                      label149.Text = "-"; }
                  //deadline
                  if (!testrow.Isdeadline_0offNull())
                  {
                      dateTimePicker2.Value = testrow.deadline_0off;
                      label150.Text = dateTimePicker2.Value.ToLongDateString();
                      textBox50.Text = dateTimePicker2.Value.Day.ToString();
                      textBox52.Text = dateTimePicker2.Value.Month.ToString();
                      textBox51.Text = dateTimePicker2.Value.Year.ToString();
                  }
                  else
                  {
                      label150.Parent.Parent.Hide();
                      label150.Text = "-"; }
                  //volgorde
                  if (!testrow.Is_0off_volgNull())
                  {
                      label151.Text = testrow._0off_volg.ToString();
                      textBox13.Text = testrow._0off_volg.ToString();
                  }
                  else
                  {
                      label151.Parent.Parent.Hide();
                      label151.Text = "-";
                  }

                  //versie
                  if (!testrow.Is_0off_versieNull())
                  {
                      label152.Text = testrow._0off_versie.ToString();
                      textBox14.Text = testrow._0off_versie.ToString();
                  }
                  else
                  {
                      label152.Parent.Parent.Hide();
                      label152.Text = "-";
                  }
                  //verstuurd
                  if (!testrow.Is_0off_verstuurdNull())
                  {

                      dateTimePicker9.Value = testrow._0off_verstuurd;
                      label153.Text = dateTimePicker9.Value.ToLongDateString();
                      textBox53.Text = dateTimePicker9.Value.Day.ToString();
                      textBox56.Text = dateTimePicker9.Value.Month.ToString();
                      textBox54.Text = dateTimePicker9.Value.Year.ToString();
                  }
                  else
                  {
                      label153.Parent.Parent.Hide();
                      label153.Text = "-"; }
                  //definitief
                  if (!testrow.Is_0off_defNull())
                  {

                      dateTimePicker10.Value = testrow._0off_def;
                      label154.Text = dateTimePicker10.Value.ToLongDateString();
                      textBox57.Text = dateTimePicker10.Value.Day.ToString();
                      textBox59.Text = dateTimePicker10.Value.Month.ToString();
                      textBox58.Text = dateTimePicker10.Value.Year.ToString();
                  }
                  else
                  {
                      label154.Parent.Parent.Hide();
                      label154.Text = "-"; }
                  //instelling
                   if (!testrow._0off_aanv)
                      {
                          label155.Text = "nee";
                          checkBox1.Checked = false;
                      }
                   else
                      {

                          label155.Text = "ja";
                          checkBox1.Checked = true;
                      }
                  
                  //memo
                   if (memoadapt.Exists(int.Parse(id),1,3)>0)
                   {
                       memotabl = memoadapt.GetData(int.Parse(id), 1, 3);
                       memoRow = (testset.memosRow)memotabl.Rows[0];
                       textBox63.Text = memoRow.inhoud;
                       linkLabel26.Text = memoRow.inhoud;
                       show_mem_btn(textBox63, button17);

                   }
                   else
                   {
                       show_mem_btn(textBox63, button17);
                       textBox63.Text = String.Empty;
                       linkLabel26.Text = String.Empty;
                       linkLabel26.Parent.Parent.Hide();
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
              }


              

        }
        private void Navigate_adres()
        {
           // table = adapter.GetData(int.Parse(id));

          //  testrow = (testset.projectRow)table.Rows[0];
            if (!testrow.Isadres_id_bouwNull())
            {
                elementHost1.Show();
                aTA.FillBy(aDT, testrow.adres_id_bouw);

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
                        if (aRow.postcode_cijfers > 999)
                        {
                            adres_user_control1.Postcode_cijfers = aRow.postcode_cijfers;
                        }
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
            else
            {
                elementHost1.Hide();
            }
            if (!testrow.Isadres_id_factuurNull())
            {
                elementHost2.Show();
                aDT.Clear();
                aTA.FillBy(aDT, testrow.adres_id_factuur);

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
                        if (aRow.postcode_cijfers > 999)
                        {
                            adres_user_control2.Postcode_cijfers = aRow.postcode_cijfers;
                        }
                    }
                    if (!aRow.Ispostcode_lettersNull())
                    {
                        adres_user_control2.Postcode_letters = aRow.postcode_letters;
                    }
                    if (!aRow.Isstraat_2Null())
                    {
                        adres_user_control2.Straat2 = aRow.straat_2;
                    }

                }
            }
            else
            {
                elementHost2.Hide();
            }
        }
        private void MultiLineLabel(Label ml)
        {
            if (ml.Text.Length > 34)
            {
                string tmp = ml.Text;
                int regels = ml.Text.Length / 34;
                ml.Text = string.Empty;
                
                for (int i = 0; i < regels; i++)
                {
                    string tmp2 = tmp.Substring((i * 34), 34);
                    ml.Text += tmp2 + "\r\n";
                }
              
            }
           

        }
        private void btnterug_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            using (Panel tmp_P = new Panel())
            {
                tmp_P.Location = panel5.Location;
                tmp_P.Size = panel5.Size;
                tmp_P.BackColor = System.Drawing.SystemColors.ControlLightLight;
                tmp_P.BorderStyle = BorderStyle.Fixed3D;
                this.Controls.Add(tmp_P);
                tmp_P.Show();
                tmp_P.BringToFront();
                antw = Is_ingebruik(int.Parse(id), 1);
                if (antw.Length > 1 && antw == Global.username)
                {
                    if (!Verwijder_ingebruik(int.Parse(id), 1))
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
                if (upsub == true)
                {
                    //  SuspendUpdate_R.Suspend(this);
                    adres_user_control1.Wijzigstand = false;
                    adres_user_control2.Wijzigstand = false;
                    telefoonnummer_user_control1.Annuleer();
                    telefoonnummer_user_control2.Annuleer();
                    telefoonnummer_user_control3.Annuleer();
                    telefoonnummer_user_control1.Wijzigstand(false);
                    telefoonnummer_user_control2.Wijzigstand(false);
                    telefoonnummer_user_control3.Wijzigstand(false);

                    //NavigateRecord();

                    if (van_zoek)
                    {
                        btnterug.Image = null;
                        btnterug.Image = Properties.Resources.ZoekProject2;
                    }
                    else
                    {
                        btnterug.Image = null;
                        btnterug.Image = Properties.Resources.Alleprojecten2;
                    }

                    variabele_set1.annuleer();
                    variabele_set2.annuleer();
                    this.wijzigstand = false;

                    Navigate_adres();
                    btn_verwijder.Hide();
                    btn_verwijder_annuleer.Hide();
                    btn_verwijder_bevestig.Hide();
                    upsub = false;
                    no_edit(true);
                    //SuspendUpdate_R.Resume(this);
                    switch (tab)
                    {
                        case 1:
                            panel5.Focus();
                            break;
                        case 2:
                            panel6.Focus();
                            break;
                        case 3:
                            panel7.Focus();
                            break;
                    }

                }
                else
                {
                    if (start_parent != null)
                    {
                        if (!start_parent.IsDisposed)
                        {
                            if (start_parent is overview1)
                            { (start_parent as overview1).refresh_zoek(); }
                            start_parent.herlaad();
                        }
                        else
                        {
                            FormManager.GetMenu().herlaad();
                        }
                    }

                    this.sluit();
                    Close();
                }
            }
                Cursor.Current = Cursors.Default;
        }
        private bool VerplichtCheck()
        {
            bool compleet = true;
            if (textBox6.TextLength < 1)
            {
                textBox6.BackColor = Color.Crimson;
                textBox6.ForeColor = Color.White;
                compleet = false;
            }
            else
            {
                textBox6.BackColor = Color.White;
                textBox6.ForeColor = Color.Black;
            }
            if (mcb_bdr1.SelectedIndex < 0)
            {
                mcb_bdr1.BackColor = Color.Crimson;
                mcb_bdr1.ForeColor = Color.White;
                compleet = false;
            }
            else
            {
                mcb_bdr1.BackColor = Color.White;
                mcb_bdr1.ForeColor = Color.Black;
            }
            if (comboBox31.SelectedIndex < 0)
            {
                comboBox31.BackColor = Color.Crimson;
                comboBox31.ForeColor = Color.White;
                compleet = false;
            }
            else
            {
                comboBox31.BackColor = Color.White;
                comboBox31.ForeColor = Color.Black;
            }
            return compleet;

        }
        private void Vergelijk_rows()
        {
           for(int i = 0;i < testrow.ItemArray.Length;i++)
            {
                if (i == 6 || (i >= 73 && i <= 95))
                {
                    if (testrow.ItemArray[i].ToString() != ValidateRow.ItemArray[i].ToString())
                    {
                        int id_oud = 0;
                        int.TryParse(ValidateRow.ItemArray[i].ToString(), out id_oud);
                        int id_new = 0;
                        int.TryParse(testrow.ItemArray[i].ToString(), out id_new);
                        int type = 0;
                        if (i == 6 || (i >= 88 && i <= 95))//bdr
                        {
                            type = 2;
                        }
                        else if (i >= 73 && i <= 87)//pers
                        {
                            type = 3;
                        }
                        //calc afgl funct
                        taak_bepaler tb = new taak_bepaler();
						if (id_oud > 0) { tb.Recalc_function(id_oud, type); }
						if (id_new > 0) { tb.Recalc_function(id_new, type); }
                        
                    }
                }
            }
            
        }
        private void Recalc_bij_del()
        {
            for (int i = 0; i < testrow.ItemArray.Length; i++)
            {
                int r_id = 0;
                int.TryParse(testrow.ItemArray[i].ToString(), out r_id);
                int type = 0;
                if (i == 6 || (i >= 88 && i <= 95))//bdr
                {
                    type = 2;
                }
                else if (i >= 73 && i <= 87)//pers
                {
                    type = 3;
                }
                taak_bepaler tb = new taak_bepaler();
                tb.Recalc_function(r_id, type);
            }
        }
        private void btnopslaan_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            using (Panel tmp_P = new Panel())
            {
                tmp_P.Location = panel5.Location;
                tmp_P.Size = panel5.Size;
                tmp_P.BackColor = System.Drawing.SystemColors.ControlLightLight;
                tmp_P.BorderStyle = BorderStyle.Fixed3D;
                this.Controls.Add(tmp_P);
                tmp_P.Show();
                tmp_P.BringToFront();

                bool fout_in_nummer = false;
                string mem_memoaanvr = String.Empty;
                string mem_naamspec = String.Empty;
                string mem_0offannv = String.Empty;
                //SuspendUpdate_R.Suspend(this);
                if (!VerplichtCheck() || !variabele_set1.VerplichtCheck() || !variabele_set2.VerplichtCheck())
                {
                    MessageBox.Show("Niet alle verplichte velden zijn ingevuld.");
                    //SuspendUpdate_R.Resume(this);
                }
                else
                {
                    if (upsub == true)//update
                    {
                        try
                        {
                            adres_user_control1.Wijzigstand = false;
                            adres_user_control2.Wijzigstand = false;
                            //tab1
                            //project informatie
                            //projectnaam

                            if (textBox6.Text != "")
                            { testrow.naam_project = textBox6.Text; }
                            else { testrow.naam_project = null; }
                            // project naam specificatie

                            if (textBox60.Text != String.Empty && textBox60.Text!= "...")
                            {
                                testrow.naam_specificatie = mem_naamspec = textBox60.Text.Trim();
                            }
                            else
                            {
                                testrow.naam_specificatie = String.Empty;
                            }
                            //opdrachtgever
                            if (mcb_bdr1.Text != "")
                            {
                                testrow.opdrachtgeverZEEBREGTS_nr = int.Parse(mcb_bdr1.SelectedValue.ToString());
                            }
                            else { testrow.SetopdrachtgeverZEEBREGTS_nrNull(); }

                            //bouwbedrijf
                            if (mcb_bdr2.Text != "")
                            { testrow.bouwbedrijf_nr = int.Parse(mcb_bdr2.SelectedValue.ToString()); }
                            else { testrow.Setbouwbedrijf_nrNull(); }

                            //projectontwikkelaar
                            if (mcb_bdr3.Text != "")
                            { testrow.projectontwikkelaar_nr = int.Parse(mcb_bdr3.SelectedValue.ToString()); }
                            else { testrow.Setprojectontwikkelaar_nrNull(); }
                            //inkoper
                            if (mcb_pers1.Text != "")
                            { testrow.inkoper_nr = int.Parse(mcb_pers1.SelectedValue.ToString()); }
                            else { testrow.Setinkoper_nrNull(); }
                            //werkvoorbereider
                            if (mcb_pers2.Text != "")
                            { testrow.werkvoorbereider_nr = int.Parse(mcb_pers2.SelectedValue.ToString()); }
                            else { testrow.Setwerkvoorbereider_nrNull(); }
                            //projectleider
                            if (mcb_pers3.Text != "")
                            {
                                int o;
                                int.TryParse(mcb_pers3.SelectedValue.ToString(), out o);

                                testrow.projectleider_nr = o;
                            }
                            else { testrow.Setprojectleider_nrNull(); }
                            //uitvoerder
                            if (mcb_pers4.Text != "")
                            { testrow.uitvoerder_nr = int.Parse(mcb_pers4.SelectedValue.ToString()); }
                            else { testrow.Setuitvoerder_nrNull(); }
                            //opzichter
                            if (mcb_pers5.Text != "")
                            { testrow._opzichter_persoon_nr = int.Parse(mcb_pers5.SelectedValue.ToString()); }
                            else { testrow.Set_opzichter_persoon_nrNull(); }
                            //contract
                            //projectnr extern
                            if (textBox5.Text != "")
                            {
                                testrow.aannemer_projectnummer = textBox5.Text;
                            }
                            else { testrow.aannemer_projectnummer = null; }
                            //contractnr
                            if (textBox62.Text != "")
                            {
                                testrow.aannemer_contractnummer = textBox62.Text;
                            }
                            else { testrow.aannemer_contractnummer = null; }
                            //nacalculatiecode
                            if (textBox4.Text != "")
                            {
                                testrow.nacalculatiecode = textBox4.Text;
                            }
                            else { testrow.nacalculatiecode = null; }

                            //NAW gegevens
                            //bouwstraat

                            //tel1
                            KeyValuePair<string, bool> nummerantw1 = telefoonnummer_user_control1.Get_Nummer();
                            if (nummerantw1.Value == true)
                            {
                                testrow.telefoon_nr_1 = nummerantw1.Key;
                            }
                            else
                            {
                                fout_in_nummer = true;
                                goto NUMBERFAULT;
                            }
                            ////////////
                            //tel2
                            KeyValuePair<string, bool> nummerantw2 = telefoonnummer_user_control2.Get_Nummer();
                            if (nummerantw2.Value == true)
                            {
                                testrow.telefoon_nr_2 = nummerantw2.Key;
                            }
                            else
                            {
                                fout_in_nummer = true;
                                goto NUMBERFAULT;
                            }
                            //tel3
                            KeyValuePair<string, bool> nummerantw3 = telefoonnummer_user_control3.Get_Nummer();
                            if (nummerantw3.Value == true)
                            {
                                testrow.telefoon_nr_3 = nummerantw3.Key;
                            }
                            else
                            {
                                fout_in_nummer = true;
                                goto NUMBERFAULT;
                            }
                            testrow.telefoon_nr_settings = telefoonnummer_user_control1.type.ToString() + "," + telefoonnummer_user_control2.type.ToString() + "," + telefoonnummer_user_control3.type.ToString();
                            ///////////////
                            bool tel_mob_done = false;
                            bool tel_vast_done = false;
                            bool tel_fax_done = false;
                            string mobiel_ipv_vast = String.Empty;
                            if (telefoonnummer_user_control1.type > -1 && nummerantw1.Value == true)
                            {
                                switch (telefoonnummer_user_control1.type)
                                {
                                    case 0: case 3: case 4: case 5://vast
                                        if (!tel_vast_done)
                                        {
                                            testrow.bouw_tel = nummerantw1.Key;
                                            if (nummerantw1.Key != null)
                                            {
                                                tel_vast_done = true;
                                            }
                                        }
                                        break;
                                    case 1://mobiel
                                        if (!tel_mob_done)
                                        {
                                            mobiel_ipv_vast = nummerantw1.Key;
                                            if (nummerantw1.Key != null)
                                            {
                                                tel_mob_done = true;
                                            }
                                        }
                                        break;
                                    case 2://fax
                                        if (!tel_fax_done)
                                        {
                                            testrow.bouw_fax = nummerantw1.Key;
                                            if (nummerantw1.Key != null)
                                            {
                                                tel_fax_done = true;
                                            }
                                        }
                                        break;
                                }
                            }
                            if (telefoonnummer_user_control2.type > -1 && nummerantw2.Value == true)
                            {
                                switch (telefoonnummer_user_control2.type)
                                {
                                    case 0: case 3: case 4: case 5://vast
                                        if (!tel_vast_done)
                                        {
                                            testrow.bouw_tel = nummerantw2.Key;
                                            if (nummerantw2.Key != null)
                                            {
                                                tel_vast_done = true;
                                            }
                                        }
                                        break;
                                    case 1://mobiel
                                        if (!tel_mob_done)
                                        {
                                            mobiel_ipv_vast = nummerantw2.Key;
                                            if (nummerantw2.Key != null)
                                            {
                                                tel_mob_done = true;
                                            }
                                        }
                                        break;
                                    case 2://fax
                                        if (!tel_fax_done)
                                        {
                                            testrow.bouw_fax = nummerantw2.Key;
                                            if (nummerantw2.Key != null)
                                            {
                                                tel_fax_done = true;
                                            }
                                        }
                                        break;
                                }
                            }
                            if (telefoonnummer_user_control3.type > -1)
                            {
                                switch (telefoonnummer_user_control3.type)
                                {
                                    case 0: case 3: case 4: case 5://vast
                                        if (!tel_vast_done)
                                        {
                                            testrow.bouw_tel = nummerantw3.Key;
                                            if (nummerantw3.Key != null)
                                            {
                                                tel_vast_done = true;
                                            }
                                        }
                                        break;
                                    case 1://mobiel
                                        if (!tel_mob_done)
                                        {
                                            mobiel_ipv_vast = nummerantw3.Key;
                                            if (nummerantw3.Key != null)
                                            {
                                                tel_mob_done = true;
                                            }
                                        }
                                        break;
                                    case 2://fax
                                        if (!tel_fax_done)
                                        {
                                            testrow.bouw_fax = nummerantw3.Key;
                                            if (nummerantw3.Key != null)
                                            {
                                                tel_fax_done = true;
                                            }
                                        }
                                        break;
                                }
                            }
                            if (!tel_vast_done)
                            {
                                testrow.Setbouw_telNull();
                            }
                            if (!tel_fax_done)
                            {
                                testrow.Setbouw_faxNull();
                            }
                            if (tel_mob_done && !tel_vast_done)
                            {
                                testrow.bouw_tel = mobiel_ipv_vast;
                            }
                            //////////////
                            //naw nieuwe stijl
                            if (!testrow.Isadres_id_bouwNull())
                            {

                                aTA.FillBy(aDT, testrow.adres_id_bouw);
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
                                MessageBox.Show("geef een plaatsnaam op");
                                adres_user_control1.Wijzigstand = true;
                                adres_user_control2.Wijzigstand = true;
                                telefoonnummer_user_control1.Wijzigstand(true);
                                telefoonnummer_user_control2.Wijzigstand(true);
                                telefoonnummer_user_control3.Wijzigstand(true);
                                OrganizePanels(tab);
                                OrganizePanels(tab);

                                return;
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
                            int? pc_cfrs1;
                            if (adres_user_control1.Postcode_cijfers > 999)
                            {
                                pc_cfrs1 = adres_user_control1.Postcode_cijfers;

                                //aRow.postcode_cijfers = adres_user_control1.Postcode_cijfers;
                            }
                            else
                            {
                                pc_cfrs1 = null;
                                //aRow.postcode_cijfers = pc_cfrs;
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
                            if (!testrow.Isadres_id_bouwNull())
                            {
                                aTA.adres_set(aRow.adres_id,
                                              aRow.land,
                                              aRow.plaats,
                                              aRow.straat_1,
                                              aRow.straat_2,
                                              pc_cfrs1,
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
                                    pc_cfrs1,
                                    aRow.postcode_letters,
                                    aRow.huis_postbus_nummer,
                                    aRow.huisnummer_toevoeging,
                                    _valid,
                                    adres_user_control1.ViaPostcode,
                                    adres_user_control1._City_key);
                                testrow.adres_id_bouw = n_id;
                            }

                            //website
                            if (textBox7.Text != String.Empty)
                            {
                                testrow.website = textBox7.Text;
                            }
                            else
                            {
                                testrow.SetwebsiteNull();
                            }
                            //factuur informatie
                            //aanhef
                            if (textBox67.Text != "")
                            {
                                testrow.factuur_aanhef = textBox67.Text;
                            }
                            else { testrow.factuur_aanhef = null; }

                            //betalingstermijn
                            if (textBox25.Text != "")
                            {
                                testrow.betalingstermijn = int.Parse(textBox25.Text.ToString());
                            }
                            else { testrow.SetbetalingstermijnNull(); }
                            //factuur nieuwe stijl

                            if (!testrow.Isadres_id_factuurNull())
                            {
                                aDT.Clear();
                                aTA.FillBy(aDT, testrow.adres_id_factuur);
                                aRow = aDT.Rows[0] as AdresDataSet.adressenRow;
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
                                aRow.land = String.Empty;
                                _valid = false;
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
                                aRow.straat_1 = String.Empty;
                                _valid = false;
                            }
                            int? pc_cfrs2;
                            if (adres_user_control2.Postcode_cijfers > 999)
                            {
                                pc_cfrs2 = adres_user_control2.Postcode_cijfers;
                                //aRow.postcode_cijfers = 
                            }
                            else
                            {
                                pc_cfrs2 = null;
                                // aRow.postcode_cijfers = 0;
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
                            if (!testrow.Isadres_id_factuurNull())
                            {
                                aTA.adres_set(aRow.adres_id,
                                              aRow.land,
                                              aRow.plaats,
                                              aRow.straat_1,
                                              aRow.straat_2,
                                              pc_cfrs2,
                                              aRow.postcode_letters,
                                              aRow.huis_postbus_nummer,
                                              aRow.huisnummer_toevoeging,
                                              _valid,
                                              adres_user_control2.ViaPostcode,
                                              adres_user_control2._City_key);
                            }
                            else
                            {
                                int n_id = (int)aTA.adres_max_id() + 1;
                                aTA.adres_new(n_id,
                                              aRow.land,
                                              aRow.plaats,
                                              aRow.straat_1,
                                              aRow.straat_2,
                                              pc_cfrs2,
                                              aRow.postcode_letters,
                                              aRow.huis_postbus_nummer,
                                              aRow.huisnummer_toevoeging,
                                              _valid,
                                              adres_user_control2.ViaPostcode,
                                              adres_user_control2._City_key);
                                testrow.adres_id_factuur = n_id;
                            }
                            //
                            //planning
                            //status 22
                            if (comboBox31.Text != "")
                            {
                                testrow.status = int.Parse(comboBox31.SelectedValue.ToString());
                            }
                            else { testrow.SetstatusNull(); }

                            //start
                            if (textBox10.Text != "" || textBox28.Text != "")
                            {
                                int week = int.Parse(textBox28.Text.ToString());
                                int jaar = int.Parse(textBox10.Text.ToString());
                                testrow.planning_start_week = week;
                                testrow.planning_start_jaar = jaar;

                            }
                            else { testrow.Setplanning_start_jaarNull(); testrow.Setplanning_start_weekNull(); }
                            //eind
                            if (textBox11.Text != "" || textBox29.Text != "")
                            {
                                int week = int.Parse(textBox29.Text.ToString());
                                int jaar = int.Parse(textBox11.Text.ToString());
                                testrow.planning_eind_week = week;
                                testrow.planning_eind_jaar = jaar;

                            }
                            else { testrow.Setplanning_eind_jaarNull(); testrow.Setplanning_eind_weekNull(); }
                            //bouwsnelheid
                            if (textBox45.Text != "")
                            {
                                testrow.bouw_snelheid = int.Parse(textBox45.Text);
                            }
                            else { testrow.Setbouw_snelheidNull(); }
                            //bevestiging(startcheck)
                            if (dates[11] && dateTimePicker11.Value != DateTimePicker.MinimumDateTime)
                            {
                                testrow.start_check = dateTimePicker11.Value;
                                dates[11] = false;
                            }
                            else if (dateTimePicker11.Value == DateTimePicker.MinimumDateTime)
                            {
                                testrow.Setstart_checkNull();
                            }

                            //betrokken intern
                            //offerte
                            if (mcb_pers6.Text != "")
                            { testrow.offerte_persoon_nr = int.Parse(mcb_pers6.SelectedValue.ToString()); }
                            else { testrow.Setofferte_persoon_nrNull(); }
                            //0offerte
                            if (mcb_pers7.Text != "")
                            { testrow._0offerte_persoon_nr = int.Parse(mcb_pers7.SelectedValue.ToString()); }
                            else { testrow.Set_0offerte_persoon_nrNull(); }
                            //meerwerk
                            if (mcb_pers8.Text != "")
                            {
                                testrow.koperofferte_persoon_nr = int.Parse(mcb_pers8.SelectedValue.ToString());
                            }
                            else { testrow.Setkoperofferte_persoon_nrNull(); }
                            //operationeel
                            if (mcb_pers9.Text != "")
                            { testrow.uitvoerderAfbouw_persoon_nr = int.Parse(mcb_pers9.SelectedValue.ToString()); }
                            else { testrow.SetuitvoerderAfbouw_persoon_nrNull(); }
                            //uitvoerder
                            if (mcb_pers10.Text != "")
                            {
                                testrow.uitvoerderZeebregts_persoon_nr = int.Parse(mcb_pers10.SelectedValue.ToString());
                            }
                            else { testrow.SetuitvoerderZeebregts_persoon_nrNull(); }
                            //tegelzetter
                            if (mcb_pers11.Text != "")
                            {
                                testrow.tegelzetter_persoon_nr = int.Parse(mcb_pers11.SelectedValue.ToString());
                            }
                            else { testrow.Settegelzetter_persoon_nrNull(); }
                            //administratienr
                            if (textBox9.Text != "")
                            {
                                testrow.projectNRadminZeebregts = textBox9.Text;
                            }
                            else { testrow.projectNRadminZeebregts = null; }

                            //tab2 bedrijfsbureau
                            //meerwerk

                            //omschrijving
                            if (textBox66.Text != "")
                            {
                                testrow.omschrijvingKoopwoningen = textBox66.Text;
                            }
                            else { testrow.omschrijvingKoopwoningen = null; }
                            //aantal woningen
                            if (textBox65.Text != "")
                            {
                                testrow.aantal_koopwoningen = int.Parse(textBox65.Text);
                            }
                            else { testrow.Setaantal_koopwoningenNull(); }
                            //tegels
                            if (mcb_bdr4.Text != "")
                            {
                                testrow.tegelshowroom_nr = int.Parse(mcb_bdr4.SelectedValue.ToString());
                            }
                            else { testrow.Settegelshowroom_nrNull(); }
                            //tegels persoon
                            if (mcb_pers12.Text != "")
                            {
                                testrow.contactpersoonTegelshowroom_nr = int.Parse(mcb_pers12.SelectedValue.ToString());
                            }
                            else { testrow.SetcontactpersoonTegelshowroom_nrNull(); }
                            //sanitair
                            if (mcb_bdr5.Text != "")
                            {
                                testrow.sanitairshowroom_nr = int.Parse(mcb_bdr5.SelectedValue.ToString());
                            }
                            else { testrow.Setsanitairshowroom_nrNull(); }
                            //sanitair persoon
                            if (mcb_pers13.Text != "")
                            {
                                testrow.contactpersoonSANITAIRshowroom_nr = int.Parse(mcb_pers13.SelectedValue.ToString());
                            }
                            else { testrow.SetcontactpersoonSANITAIRshowroom_nrNull(); }
                            //keukens
                            if (mcb_bdr6.Text != "")
                            {
                                testrow.keukenshowroom_nr = int.Parse(mcb_bdr6.SelectedValue.ToString());
                            }
                            else { testrow.Setkeukenshowroom_nrNull(); }
                            //keukens persoon
                            if (mcb_pers14.Text != "")
                            {
                                testrow.contactpersoonKEUKENshowroom_nr = int.Parse(mcb_pers14.SelectedValue.ToString());
                            }
                            else { testrow.SetcontactpersoonKEUKENshowroom_nrNull(); }
                            //begeleiding
                            if (mcb_bdr7.Text != "")
                            {
                                testrow.kopersbegeleidingbedrijf_nr = int.Parse(mcb_bdr7.SelectedValue.ToString());
                            }
                            else { testrow.Setkopersbegeleidingbedrijf_nrNull(); }
                            //begeleiding persoon
                            if (mcb_pers15.Text != "")
                            {
                                testrow.kopersbegeleider_nr = int.Parse(mcb_pers15.SelectedValue.ToString());
                            }
                            else { testrow.Setkopersbegeleider_nrNull(); }
                            //ontvangen gegevens
                            //verkoopbrochure
                            if (radioButton13.Checked == false)
                            {
                                testrow.verkoopbrochure = false;
                            }
                            else
                            {
                                testrow.verkoopbrochure = true;
                            }
                            //NAW lijst
                            if (radioButton15.Checked == false)
                            {

                                testrow.NAWgegevens = false;
                            }
                            else
                            {

                                testrow.NAWgegevens = true;
                            }
                            //tekeningen
                            if (radioButton11.Checked == false)
                            {
                                testrow._0_tekeningen = false;
                            }
                            else
                            {
                                testrow._0_tekeningen = true;

                            }
                            //instellingen-meerwerk
                            //methode
                            if (comboBox33.Text != "")
                            {
                                testrow.verekenmethode = int.Parse(comboBox33.SelectedValue.ToString());
                            }
                            else { testrow.SetverekenmethodeNull(); }
                            //rendement 1
                            if (textBox20.Text != "")
                            {
                                Double outp;
                                String conv;
                                if (textBox3.Text != "")
                                {
                                    conv = textBox20.Text + "," + textBox3.Text;
                                    Double.TryParse(conv, System.Globalization.NumberStyles.Float,CultureInfo.CreateSpecificCulture("nl-NL"),out outp);
                                    testrow._prov_vast_aan_meerwerk_ = outp;
                                }
                                else
                                {
                                    conv = textBox20.Text + ",00";
                                    Double.TryParse(conv, System.Globalization.NumberStyles.Float, CultureInfo.CreateSpecificCulture("nl-NL"), out outp);
                                    testrow._prov_vast_aan_meerwerk_ = outp;
                                }

                            }
                            else { testrow.Set_prov_vast_aan_meerwerk_Null(); }
                            //rendement 2
                            if (textBox19.Text != "")
                            {
                                Double outp;
                                String conv;
                                if (textBox8.Text != "")
                                {
                                    conv = textBox19.Text + "," + textBox8.Text;
                                    Double.TryParse(conv, System.Globalization.NumberStyles.Float, CultureInfo.CreateSpecificCulture("nl-NL"), out outp);
                                    testrow._prov_vast_aan_ = outp;
                                }
                                else
                                {
                                    conv = textBox19.Text + ",00";
                                    Double.TryParse(conv, System.Globalization.NumberStyles.Float, CultureInfo.CreateSpecificCulture("nl-NL"), out outp);
                                    testrow._prov_vast_aan_ = outp;
                                }

                            }
                            else { testrow.Set_prov_vast_aan_Null(); }

                            //coordinatiekosten
                            if (textBox55.Text != "")
                            {
                                Double outp;
                                String conv;
                                if (textBox17.Text != "")
                                {
                                    conv = textBox55.Text + "," + textBox17.Text;
                                    Double.TryParse(conv, System.Globalization.NumberStyles.Float, CultureInfo.CreateSpecificCulture("nl-NL"), out outp);
                                    if (radiominbtn.Checked)
                                    {
                                        outp = outp * -1;
                                    }
                                    testrow._coordiatiekostenbouwbedrijf_ = outp;
                                }
                                else
                                {
                                    conv = textBox55.Text + ",00";
                                    Double.TryParse(conv, System.Globalization.NumberStyles.Float, CultureInfo.CreateSpecificCulture("nl-NL"), out outp);
                                    testrow._coordiatiekostenbouwbedrijf_ = outp;
                                }

                            }
                            else { testrow.Set_coordiatiekostenbouwbedrijf_Null(); }
                            //NA werk
                            if (radioButton1.Checked == false)
                            {
                                testrow.FACTORnietAW_Zeebregts = 0;
                            }
                            else
                            {
                                testrow.FACTORnietAW_Zeebregts = 1;
                            }
                            //stelpost wand
                            if (textBox32.Text != "")
                            {
                                Decimal outp;
                                String stelpostw;
                                if (textBox1.Text != "")
                                {
                                    stelpostw = textBox32.Text;
                                    stelpostw += "," + textBox1.Text;

                                    Decimal.TryParse(stelpostw, System.Globalization.NumberStyles.Float, CultureInfo.CreateSpecificCulture("nl-NL"), out outp);
                                    testrow.stelpostwand = outp;

                                }
                                else
                                {
                                    stelpostw = textBox32.Text;
                                    stelpostw += ",00";

                                    Decimal.TryParse(stelpostw, System.Globalization.NumberStyles.Float, CultureInfo.CreateSpecificCulture("nl-NL"), out outp);
                                    testrow.stelpostwand = outp;
                                }
                            }
                            else { testrow.SetstelpostwandNull(); }
                            //stelpost vloer
                            if (textBox33.Text != "")
                            {

                                Decimal outp;
                                String stelpostv;
                                if (textBox2.Text != "")
                                {
                                    stelpostv = textBox33.Text;
                                    stelpostv += "," + textBox2.Text;

                                    Decimal.TryParse(stelpostv, System.Globalization.NumberStyles.Float, CultureInfo.CreateSpecificCulture("nl-NL"), out outp);
                                    testrow.stelpostvloer = outp;

                                }
                                else
                                {
                                    stelpostv = textBox33.Text;
                                    stelpostv += ",00";

                                    Decimal.TryParse(stelpostv, System.Globalization.NumberStyles.Float, CultureInfo.CreateSpecificCulture("nl-NL"), out outp);
                                    testrow.stelpostvloer = outp;
                                }
                            }
                            else { testrow.SetstelpostvloerNull(); }
                            //opslagprijs
                            if (textBox34.Text != "")
                            {
                                Double outp;
                                String conv;
                                if (textBox21.Text != "")
                                {
                                    conv = textBox34.Text + "," + textBox21.Text;
                                    Double.TryParse(conv, System.Globalization.NumberStyles.Currency, CultureInfo.CreateSpecificCulture("nl-NL"), out outp);
                                    outp = outp / 100;
                                    testrow.opslagprijs_koper = outp;
                                }
                                else
                                {
                                    conv = textBox34.Text + ",00";
                                    Double.TryParse(conv, System.Globalization.NumberStyles.Currency, CultureInfo.CreateSpecificCulture("nl-NL"), out outp);
                                    outp = outp / 100;
                                    testrow.opslagprijs_koper = outp;
                                }
                            }
                            else { testrow.Setopslagprijs_koperNull(); }
                            //opslag meters
                            if (textBox35.Text != "")
                            {
                                Double outp;
                                String conv;
                                if (textBox24.Text != "")
                                {
                                    conv = textBox35.Text + "," + textBox24.Text;
                                    Double.TryParse(conv, System.Globalization.NumberStyles.Float, CultureInfo.CreateSpecificCulture("nl-NL"), out outp);
                                    outp = outp / 100;
                                    testrow.opslagmeters_koper = outp;
                                }
                                else
                                {
                                    conv = textBox35.Text + ",00";
                                    Double.TryParse(conv, System.Globalization.NumberStyles.Float, CultureInfo.CreateSpecificCulture("nl-NL"), out outp);
                                    outp = outp / 100;
                                    testrow.opslagmeters_koper = outp;
                                }
                            }
                            else { testrow.Setopslagmeters_koperNull(); }
                            //facturatie
                            if (mcb_bdr8.Text != "")
                            {
                                testrow.facturatieMEERWERKbedrijf_nr = int.Parse(mcb_bdr8.SelectedValue.ToString());
                            }
                            else { testrow.SetfacturatieMEERWERKbedrijf_nrNull(); }
                            //betalingbedrijf
                            if (mcb_bdr9.Text != "")
                            {
                                testrow.betalingbedrijf = int.Parse(mcb_bdr9.SelectedValue.ToString());
                            }
                            else { testrow.SetbetalingbedrijfNull(); }
                            //verpakkingstoeslag
                            if (radioButton17.Checked == false)
                            {
                                testrow.verpakkingstoeslagUpgrade = false;
                            }
                            else
                            {
                                testrow.verpakkingstoeslagUpgrade = true;
                            }

                            //btw
                            var outBtw = 0.0;
                            var btwval = textBox47.Text;
                            if (Double.TryParse(btwval, System.Globalization.NumberStyles.Float, CultureInfo.CreateSpecificCulture("nl-NL"), out outBtw))
                            {
                                if (outBtw < 100)
                                {
                                    testrow.btw = (decimal)outBtw;
                                }
                                else
                                {
                                    testrow.btw = (decimal)0.21;
                                }
                            }
                            else
                            {
                                testrow.btw = (decimal)0.21;
                                
                            }
                                   
                            
                            
                            //var btwval = new decimal();
                            //if (Global.isRdp)
                            //{
                            //    textBox47.Text = textBox47.Text.Replace(".", ",");
                            //}
                            //else
                            //{
                            //    textBox47.Text = textBox47.Text.Replace(",", ".");
                            //}
                            
                            
                            //decimal.TryParse(textBox47.Text.ToString(CultureInfo.CreateSpecificCulture("nl-NL")), out btwval);
                            //testrow.btw = btwval;
                            //instellingen - 0 offerte
                            //switchcontract
                            if (radioButton4.Checked == true)
                            {
                                testrow.switchcontract = 1;
                            }
                            else
                            {
                                testrow.switchcontract = 2;
                            }
                            //uitsplitsing
                            if (radioButton6.Checked == true)
                            {
                                testrow.UITvia0offerte = false;
                            }
                            else
                            {
                                testrow.UITvia0offerte = true;
                            }
                            //instellingen - kwaliteit
                            //STABU groep
                            if (radioButton7.Checked == true)
                            {
                                testrow.STABU_groep = 1;
                            }
                            else if (radioButton8.Checked == true)
                            {
                                testrow.STABU_groep = 2;
                            }
                            else if (radioButton9.Checked == true)
                            {
                                testrow.STABU_groep = 3;
                            }
                            //instelling STABU
                            if (radioButton19.Checked == false)
                            {
                                testrow._STABU_groep_ja_nee = false;
                            }
                            else
                            {
                                testrow._STABU_groep_ja_nee = true;
                            }
                            //instellingen - algemeen
                            //master
                            if (textBox22.Text != "")
                            {
                                testrow.exel_master = float.Parse(textBox22.Text.Replace(".", ","));
                            }
                            else { testrow.Setexel_masterNull(); }
                            //statusoverzicht
                            if (radioButton21.Checked == false)
                            {
                                testrow.INafbouwCONTROL = false;
                            }
                            else
                            {
                                testrow.INafbouwCONTROL = true;
                            }
                            //tab3 offerte
                            //offerte
                            //aanvraag
                            if (dates[5] && dateTimePicker5.Value != DateTimePicker.MinimumDateTime)
                            {
                                testrow.off_aanvraag = dateTimePicker5.Value;
                                dates[5] = false;
                            }
                            else if (dateTimePicker5.Value == DateTimePicker.MinimumDateTime)
                            {
                                testrow.Setoff_aanvraagNull();
                            }
                            //deadline
                            if (dates[1] && dateTimePicker1.Value != DateTimePicker.MinimumDateTime)
                            {
                                testrow.deadline_off = dateTimePicker1.Value;
                                dates[1] = false;
                            }
                            else if (dateTimePicker1.Value == DateTimePicker.MinimumDateTime)
                            {
                                testrow.Setdeadline_offNull();
                            }
                            //verstuurd
                            if (dates[6] && dateTimePicker6.Value != DateTimePicker.MinimumDateTime)
                            {
                                testrow.off_verstuurd = dateTimePicker6.Value;
                                dates[6] = false;
                            }
                            else if (dateTimePicker6.Value == DateTimePicker.MinimumDateTime)
                            {
                                testrow.Setoff_verstuurdNull();
                            }
                            //definitief
                            if (dates[7] && dateTimePicker7.Value != DateTimePicker.MinimumDateTime)
                            {
                                testrow.off_def = dateTimePicker7.Value;
                                dates[7] = false;
                            }
                            else if (dateTimePicker7.Value == DateTimePicker.MinimumDateTime)
                            {
                                testrow.Setoff_defNull();
                            }
                            //memo

                            if (textBox61.Text != String.Empty && textBox61.Text != "...")
                            {
                                testrow.memo_aanvraag = mem_memoaanvr = textBox61.Text;
                            }
                            else { testrow.Setmemo_aanvraagNull(); }
                            //0 offerte
                            //aanvraag
                            if (dates[8] && dateTimePicker8.Value != DateTimePicker.MinimumDateTime)
                            {
                                testrow._0off_aanvraag = dateTimePicker8.Value;
                                dates[8] = false;
                            }
                            else if (dateTimePicker8.Value == DateTimePicker.MinimumDateTime)
                            {
                                testrow.Set_0off_aanvraagNull();
                            }
                            //deadline
                            if (dates[2] && dateTimePicker2.Value != DateTimePicker.MinimumDateTime)
                            {
                                testrow.deadline_0off = dateTimePicker2.Value;
                                dates[2] = false;
                            }
                            else if (dateTimePicker2.Value == DateTimePicker.MinimumDateTime)
                            {
                                testrow.Setdeadline_0offNull();
                            }
                            //volgorde
                            if (textBox13.Text != "")
                            {
                                testrow._0off_volg = int.Parse(textBox13.Text);
                            }
                            else { testrow.Set_0off_volgNull(); }
                            //versie
                            if (textBox14.Text != "")
                            {
                                testrow._0off_versie = int.Parse(textBox14.Text);
                            }
                            else { testrow.Set_0off_versieNull(); }
                            //verstuurd
                            if (dates[9] && dateTimePicker9.Value != DateTimePicker.MinimumDateTime)
                            {
                                testrow._0off_verstuurd = dateTimePicker9.Value;
                                dates[9] = false;
                            }
                            else if (dateTimePicker9.Value == DateTimePicker.MinimumDateTime)
                            {
                                testrow.Set_0off_verstuurdNull();
                            }
                            //definitief
                            if (dates[10] && dateTimePicker10.Value != DateTimePicker.MinimumDateTime)
                            {
                                testrow._0off_def = dateTimePicker10.Value;
                                dates[10] = false;
                            }
                            else if (dateTimePicker10.Value == DateTimePicker.MinimumDateTime)
                            {
                                testrow.Set_0off_defNull();
                            }
                            //instelling
                            if (checkBox1.Checked == false)
                            {
                                testrow._0off_aanv = false;
                            }
                            else
                            {
                                testrow._0off_aanv = true;
                            }
                            //memo
                            if (textBox63.Text != String.Empty && textBox63.Text != "...")
                            {
                                testrow._0off_annv_omschrijving = mem_0offannv = textBox63.Text;
                            }
                            else { testrow.Set_0off_annv_omschrijvingNull(); }

                            variabele_set1.save();
                            variabele_set2.save();
                        }
                        catch (Exception e3)
                        {
                            MessageBox.Show("opslaan mislukt, controleer alle waardes");
                            String log_line = "crash program @ " + DateTime.Now.ToString() + "error: " + e3;
                            System.IO.StreamWriter file = new System.IO.StreamWriter(Global.log_file_path, true);
                            file.WriteLine(log_line);
                            file.Close();

                            //SuspendUpdate_R.Resume(this);
                            return;
                        }
                        try
                        {
                            int i_id = int.Parse(id);
                            Vergelijk_rows();
                            adapter.Update(testrow);
                            //memos//
                            if (mem_naamspec != String.Empty)
                            {
                                if (memoadapt.Exists(i_id, 1, 1) > 0)
                                {
                                    memoadapt.Update_memos(mem_naamspec, i_id, 1, 1);
                                }
                                else
                                {
                                    memoadapt.new_memo(i_id, 1, 1, mem_naamspec);
                                }
                            }
                            else
                            {
                                if (memoadapt.Exists(i_id, 1, 1) > 0)
                                {
                                    memoadapt.DeleteMemo(i_id, 1, 1);
                                }
                            }
                            if (mem_memoaanvr != String.Empty)
                            {
                                if (memoadapt.Exists(i_id, 1, 2) > 0)
                                {
                                    memoadapt.Update_memos(mem_memoaanvr, i_id, 1, 2);
                                }
                                else
                                {
                                    memoadapt.new_memo(i_id, 1, 2, mem_memoaanvr);
                                }
                            }
                            else
                            {
                                if (memoadapt.Exists(i_id, 1, 2) > 0)
                                {
                                    memoadapt.DeleteMemo(i_id, 1, 2);
                                }
                            }
                            if (mem_0offannv != String.Empty)
                            {
                                if (memoadapt.Exists(i_id, 1, 3) > 0)
                                {
                                    memoadapt.Update_memos(mem_0offannv, i_id, 1, 3);
                                }
                                else
                                {
                                    memoadapt.new_memo(i_id, 1, 3, mem_0offannv);
                                }
                            }
                            else
                            {
                                if (memoadapt.Exists(i_id, 1, 3) > 0)
                                {
                                    memoadapt.DeleteMemo(i_id, 1, 3);
                                }
                            }
                            ////////
                            if (adres_user_control1.Plaats != string.Empty)
                            {
                                string straat_bezoek_oud = adres_user_control1.Straat + " " + adres_user_control1.Huisnummer + adres_user_control1.Huisnummer_toevoeging + " (" + adres_user_control1.Plaats.ToString() + ") " + adres_user_control1.Straat2;
                                string pc_cijfers;
                                if (adres_user_control1.Postcode_cijfers > 999)
                                {
                                    pc_cijfers = adres_user_control1.Postcode_cijfers.ToString();
                                }
                                else
                                {
                                    pc_cijfers = " ";

                                }
                                adapter.Update_bouw_adres_oud(adres_user_control1.Plaats, straat_bezoek_oud, pc_cijfers + adres_user_control1.Postcode_letters, testrow.project_NR);
                            }
                            string straat_bezoek_oud2 = adres_user_control2.Straat + " " + adres_user_control2.Huisnummer.ToString();
                            if (adres_user_control2.Huisnummer_toevoeging != string.Empty)
                            {
                                straat_bezoek_oud2 += adres_user_control2.Huisnummer_toevoeging;
                            }
                            string pc_cijfrs;
                            if (adres_user_control2.Postcode_cijfers > 999)
                            {
                                pc_cijfrs = adres_user_control2.Postcode_cijfers.ToString();
                            }
                            else
                            {
                                pc_cijfrs = " ";
                            }
                            adapter.Update_factuur_adres_oud(adres_user_control2.Plaats, straat_bezoek_oud2, pc_cijfrs + adres_user_control2.Postcode_letters, testrow.project_NR);

                        }
                        catch (Exception e6)
                        {
                            MessageBox.Show("opslaan mislukt, controleer netwerkverbinding");
                            String log_line = "crash program @ " + DateTime.Now.ToString() + "error in projectform: " + e6;
                            System.IO.StreamWriter file = new System.IO.StreamWriter(Global.log_file_path, true);
                            file.WriteLine(log_line);
                            file.Close();
                            adres_user_control1.Wijzigstand = true;
                            adres_user_control2.Wijzigstand = true;
                            telefoonnummer_user_control1.Wijzigstand(true);
                            telefoonnummer_user_control2.Wijzigstand(true);
                            telefoonnummer_user_control3.Wijzigstand(true);
                            // SuspendUpdate_R.Resume(this);
                            return;
                        }
                        finally
                        {
                            antw = Is_ingebruik(int.Parse(id), 1);
                            if (antw.Length > 1 && antw == Global.username)
                            {
                                if (Verwijder_ingebruik(int.Parse(id), 1))
                                {
                                }
                                else
                                {
                                    MessageBox.Show("fout bij vrijgeven opslaan " + antw);

                                }
                            }
                            if (upsub == true)
                            {
                                handelingen_logger.log_handeling(int.Parse(id), 1, 26);
                                if (van_zoek)
                                {
                                    btnterug.Image = null;
                                    btnterug.Image = Properties.Resources.ZoekProject2;
                                }
                                else
                                {
                                    btnterug.Image = null;
                                    btnterug.Image = Properties.Resources.Alleprojecten2;
                                }

                                variabele_set1.kies_stand(1);
                                variabele_set2.kies_stand(1);
                                this.wijzigstand = false;
                                telefoonnummer_user_control1.Wijzigstand(wijzigstand);
                                telefoonnummer_user_control2.Wijzigstand(wijzigstand);
                                telefoonnummer_user_control3.Wijzigstand(wijzigstand);
                            }
                            no_edit(true);
                            btnterug.Show();
                            upsub = false;
                            this.project_form_Resize(this, e);
                        }

                    }
                }

            //SuspendUpdate_R.Resume(this);
            NUMBERFAULT: if (fout_in_nummer)
                {
                    //SuspendUpdate_R.Resume(this);
                    MessageBox.Show("Een telefoonnummer is fout ingevoerd. \nGooi leeg met het kruisje of pas het nummer aan.");
                }
                switch (tab)
                {
                    case 1:
                        panel5.Focus();
                        break;
                    case 2:
                        panel6.Focus();
                        break;
                    case 3:
                        panel7.Focus();
                        break;
                }
            }
                Cursor.Current = Cursors.Default;  
            
        }
        private void btnwijzig_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            using (Panel tmp_P = new Panel())
            {
                tmp_P.Location = panel5.Location;
                tmp_P.Size = panel5.Size;
                tmp_P.BackColor = System.Drawing.SystemColors.ControlLightLight;
                tmp_P.BorderStyle = BorderStyle.Fixed3D;
                this.Controls.Add(tmp_P);
                tmp_P.Show();
                tmp_P.BringToFront();
                Button btn = sender as Button;
                btn.BackColor = Color.AliceBlue;
                // SuspendUpdate_R.Suspend(this);
                antw = Is_ingebruik(int.Parse(id), 1);
                bezetnaam = antw;
                if (antw.Length > 1)
                {
                    MessageBox.Show("Het wijzigen van het project " + vrijgeefstring + " is momenteel niet mogelijk.\n Dit formulier is reeds in gebruik door " + antw + ".\n U ontvangt een melding zodra dit formulier wordt vrijgegeven.");
                    btnwijzig.Enabled = false;
                    INUSE_BackgroundWorker.RunWorkerAsync();
                }
                else
                {
                    if (Neem_ingebruik(int.Parse(id), 1, Global.username))
                    {
                        upsub = true;
                        this.wijzigstand = true;
                        adres_user_control1.Wijzigstand = wijzigstand;
                        adres_user_control2.Wijzigstand = wijzigstand;
                        telefoonnummer_user_control1.Wijzigstand(wijzigstand);
                        telefoonnummer_user_control2.Wijzigstand(wijzigstand);
                        telefoonnummer_user_control3.Wijzigstand(wijzigstand);
                        btnterug.Image = null;
                        btnterug.Image = Properties.Resources.Annuleer2;
                        btn_verwijder.Show();
                        variabele_set1.kies_stand(2);
                        variabele_set2.kies_stand(2);
                        NavigateRecord();
                        do_edit();
                    }
                    else
                    {

                        MessageBox.Show("reservering failed");
                    }
                }
                //SuspendUpdate_R.Resume(this);
                switch (tab)
                {
                    case 1:
                        panel5.Focus();
                        break;
                    case 2:
                        panel6.Focus();
                        break;
                    case 3:
                        panel7.Focus();
                        break;
                }
                btn.BackColor = System.Drawing.SystemColors.Control;
            }
            Cursor.Current = Cursors.Default;
        }
        Panel verw_P;
        private void ScreenCapture()
        {
            button30.Hide();
            button31.Hide();
            button32.Hide();
            button18.Hide();
            label57.Hide();
            label54.Hide();
            label44.Hide();
            verw_P = new Panel();
            verw_P.Location = panel5.Location;
            verw_P.Size = panel5.Size;
            verw_P.BackColor = System.Drawing.SystemColors.ControlLightLight;
            verw_P.BorderStyle = BorderStyle.Fixed3D;
            this.Controls.Add(verw_P);
            verw_P.Show();
            verw_P.BringToFront();

            btnopslaan.Enabled = false;
            btnwijzig.Enabled = false;
            btnterug.Enabled = false;
            
            foreach (Control c in panel1.Controls)
            {
                c.Enabled = false;
            }
          
            foreach (Control c in panel7.Controls)
            {
                c.Enabled = false;
            }
            foreach (Control c in panel5.Controls)
            {
                c.Enabled = false;
            }
            foreach (Control c in panel6.Controls)
            {
                c.Enabled = false;
            }
           
            if (tab == 1)
            {
                panel5.Focus();
            }
            else if (tab == 2)
            {
                panel6.Focus();
            }
            else if (tab == 3)
            {
                panel7.Focus();
            }
        }
        private void ScreenCapture_off()
        {
            btnopslaan.Enabled = true;
            btnwijzig.Enabled = true;
            btnterug.Enabled = true;
            foreach (Control c in panel1.Controls)
            {
                c.Enabled = true;
            }
          
            foreach (Control c in panel7.Controls)
            {
                c.Enabled = true;
            }
            foreach (Control c in panel5.Controls)
            {
                c.Enabled = true;
            }
            foreach (Control c in panel6.Controls)
            {
                c.Enabled = true;
            }
            button30.Show();

            button31.Show();

            button32.Show();
            if (Global.UserLevel < 3)
            {
                button18.Show();
            }
            label57.Show();
            label54.Show();
            label44.Show();
            
            if (tab == 1)
            {
                panel5.Focus();
            }
            else if (tab == 2)
            {
                panel6.Focus();
            }
            else if (tab == 3)
            {
                panel7.Focus();
            }
            verw_P.Dispose();
        }
        private void Save_cbb_settings(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is MultiColumnComboBoxDemo.MultiColumnComboBox)
                {
                    if ((c as MultiColumnComboBoxDemo.MultiColumnComboBox).SelectedIndex < 0)
                    {
                        cbb.Add((c as MultiColumnComboBoxDemo.MultiColumnComboBox), -1);
                       
                    }
                    else
                    {
                        int a;
                        int.TryParse((c as MultiColumnComboBoxDemo.MultiColumnComboBox).SelectedValue.ToString(), out a);
                        cbb.Add((c as MultiColumnComboBoxDemo.MultiColumnComboBox), a);
                        
                    }
                   
                }
                if (c.Controls.Count > 0)
                {
                    Save_cbb_settings(c);
                }
            }
        }

        private void OrganizePanels(int tab_nr)
        {
            if (this.Visible)
            {
             

                   // SuspendUpdate_R.Suspend(this);
                   //SuspendUpdate_R.Resume(this);
                    int p_x_links = panel5.Width / 2 - 436;
                    int p_x_rechts = panel5.Width / 2 + 4;
                    Point scrollpos;
                    switch (tab_nr)
                    {
                        case 1:
                            /*      links      :   rechts
                             * #1   p_projinfo  p_betrokene intern  
                             * #2   p_planning  p_contract
                             * #3   p_factuur   p_telnrs
                             * #4   varset1     p_NAW
                             * #5               varset2
                             */
                            //links
                            scrollpos = panel5.AutoScrollPosition;
                            panel5.AutoScrollPosition = new Point(0, 0);
                            p_projinfo.Location = new Point(p_x_links, 10);
                            p_projinfo.Size = bereken_panel_size(p_projinfo);
                            p_planning.Location = new Point(p_x_links, p_projinfo.Height + p_projinfo.Location.Y + 8);
                            p_planning.Size = bereken_panel_size(p_planning);
                            p_factuur_info.Location = new Point(p_x_links, p_planning.Height + p_planning.Location.Y + 8);
                            p_factuur_info.Size = bereken_panel_size(p_factuur_info);
                            variabele_set1.Location = new Point(p_x_links, p_factuur_info.Height + p_factuur_info.Location.Y + 8);
                            //rechts
                            p_betrokken_intern.Location = new Point(p_x_rechts, 10);
                            p_betrokken_intern.Size = bereken_panel_size(p_betrokken_intern);
                            p_contract.Location = new Point(p_x_rechts, p_betrokken_intern.Height + p_betrokken_intern.Location.Y + 8);
                            p_contract.Size = bereken_panel_size(p_contract);
                            p_telnrs.Location = new Point(p_x_rechts, p_contract.Height + p_contract.Location.Y + 8);
                            p_telnrs.Size = bereken_panel_size(p_telnrs);
                            p_NAW.Location = new Point(p_x_rechts, p_telnrs.Height + p_telnrs.Location.Y + 8);
                            p_NAW.Size = bereken_panel_size(p_NAW);
                            variabele_set2.Location = new Point(p_x_rechts, p_NAW.Height + p_NAW.Location.Y + 8);
                            scrollpos.X *= -1;
                            scrollpos.Y *= -1;
                            panel5.AutoScrollPosition = scrollpos;
                            break;
                        case 2:
                            /*          links       :       rechts
                             * #1   p_meerwer           p_instellingen
                             * #2   p_ontvangen
                             */
                            //L
                            scrollpos = panel6.AutoScrollPosition;
                            panel6.AutoScrollPosition = new Point(0, 0);
                            p_meerwerk.Location = new Point(p_x_links, 10);
                            p_meerwerk.Size = bereken_panel_size(p_meerwerk);
                            p_ontvangen_gegevens.Location = new Point(p_x_links, p_meerwerk.Height + p_meerwerk.Location.Y + 8);
                            p_ontvangen_gegevens.Size = bereken_panel_size(p_ontvangen_gegevens);
                            //R
                            p_instellingen.Location = new Point(p_x_rechts, 10);
                            p_instellingen.Size = bereken_panel_size(p_instellingen);
                            scrollpos.X *= -1;
                            scrollpos.Y *= -1;
                            panel6.AutoScrollPosition = scrollpos;
                            break;
                        case 3:
                            scrollpos = panel7.AutoScrollPosition;
                            panel7.AutoScrollPosition = new Point(0, 0);
                            p_offerte.Location = new Point(p_x_links, 10);
                            p_offerte.Size = bereken_panel_size(p_offerte);
                            p_0offerte.Location = new Point(p_x_rechts, 10);
                            p_0offerte.Size = bereken_panel_size(p_0offerte);
                            scrollpos.X *= -1;
                            scrollpos.Y *= -1;
                            panel7.AutoScrollPosition = scrollpos;
                            break;
                    }
                }
            
        }
        private Size bereken_panel_size(Panel p)
        {
            Size Sz = new Size();
            int tmp_H = 42;
            int n_showed = 0;
            foreach (Control C in p.Controls )
            {
                if (C is SplitContainer )
                {
                    if (C.Visible)
                    {
                        C.Location = new Point(10, 25 * (n_showed+1) + 5);
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
                else if (C is Button && p == p_telnrs)
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
        private void Load_cbb_settings()
        {
            foreach (MultiColumnComboBoxDemo.MultiColumnComboBox mcb in cbb.Keys)
            {
                
                int b;
               cbb.TryGetValue(mcb, out b);
               if (b == -1)
               {
                   mcb.SelectedIndex = -1;
                        
               }
               else
               {
                   mcb.SelectedValue = b;
                   
               }
               mcb.Invalidate();
               
            }
            
        }
        private void dateTimePicker3_ValueChanged(object sender, EventArgs e)
        {
            if (!from_tb)
            {
                if (sender == dateTimePicker3 && dateTimePicker3.Value != DateTimePicker.MinimumDateTime)
                {
                    changed = true;
                    int[] ar = GetWeekNumber(dateTimePicker3.Value);
                    startWeekNum = ar[0];
                    startJaar = ar[1];
                    textBox10.Text = startJaar.ToString();
                    textBox28.Text = startWeekNum.ToString();
                }

                if (sender == dateTimePicker4 && dateTimePicker4.Value != DateTimePicker.MinimumDateTime)
                {
                    changed = true;
                    int[] ar = GetWeekNumber(dateTimePicker4.Value);
                    eindWeekNum = ar[0];
                    eindJaar = ar[1];
                    textBox11.Text = eindJaar.ToString();
                    textBox29.Text = eindWeekNum.ToString();
                }

                if (sender == dateTimePicker11)
                {
                    
                    if (dateTimePicker11.Value != DateTimePicker.MinimumDateTime)
                    {
                        changed = true;
                        dates[11] = true;
                        textBox16.Text = dateTimePicker11.Value.Day.ToString();
                        textBox15.Text = dateTimePicker11.Value.Month.ToString();
                        textBox12.Text = dateTimePicker11.Value.Year.ToString();
                    }

                }
                if (sender == dateTimePicker5)
                {
                    
                    if (dateTimePicker5.Value != DateTimePicker.MinimumDateTime)
                    {
                        dates[5] = true; changed = true;
                        textBox26.Text = dateTimePicker5.Value.Day.ToString();
                        textBox18.Text = dateTimePicker5.Value.Month.ToString();
                        textBox23.Text = dateTimePicker5.Value.Year.ToString();
                    }
                }
                if (sender == dateTimePicker6)
                {
                    
                    if (dateTimePicker6.Value != DateTimePicker.MinimumDateTime)
                    {
                        dates[6] = true; changed = true;
                        textBox36.Text = dateTimePicker6.Value.Day.ToString();
                        textBox38.Text = dateTimePicker6.Value.Month.ToString();
                        textBox37.Text = dateTimePicker6.Value.Year.ToString();
                    }
                }
                if (sender == dateTimePicker7)
                {
                    
                    if (dateTimePicker7.Value != DateTimePicker.MinimumDateTime)
                    {
                        dates[7] = true; changed = true;
                        textBox39.Text = dateTimePicker7.Value.Day.ToString();
                        textBox41.Text = dateTimePicker7.Value.Month.ToString();
                        textBox40.Text = dateTimePicker7.Value.Year.ToString();
                    }
                }
                if (sender == dateTimePicker1)
                {
                    
                    if (dateTimePicker1.Value != DateTimePicker.MinimumDateTime)
                    {
                        dates[1] = true; changed = true;
                        textBox27.Text = dateTimePicker1.Value.Day.ToString();
                        textBox31.Text = dateTimePicker1.Value.Month.ToString();
                        textBox30.Text = dateTimePicker1.Value.Year.ToString();
                    }
                }
                if (sender == dateTimePicker8)
                {
                    
                    if (dateTimePicker8.Value != DateTimePicker.MinimumDateTime)
                    {
                        dates[8] = true; changed = true;
                        textBox42.Text = dateTimePicker8.Value.Day.ToString();
                        textBox44.Text = dateTimePicker8.Value.Month.ToString();
                        textBox43.Text = dateTimePicker8.Value.Year.ToString();
                    }
                }
                if (sender == dateTimePicker2)
                {
                    
                    if (dateTimePicker2.Value != DateTimePicker.MinimumDateTime)
                    {
                        dates[2] = true; changed = true;
                        textBox50.Text = dateTimePicker2.Value.Day.ToString();
                        textBox52.Text = dateTimePicker2.Value.Month.ToString();
                        textBox51.Text = dateTimePicker2.Value.Year.ToString();
                    }
                }
                if (sender == dateTimePicker9)
                {
                    
                    if (dateTimePicker9.Value != DateTimePicker.MinimumDateTime)
                    {
                        dates[9] = true; changed = true;
                        textBox53.Text = dateTimePicker9.Value.Day.ToString();
                        textBox56.Text = dateTimePicker9.Value.Month.ToString();
                        textBox54.Text = dateTimePicker9.Value.Year.ToString();
                    }
                }
                if (sender == dateTimePicker10)
                {
                    
                    if (dateTimePicker10.Value != DateTimePicker.MinimumDateTime)
                    {
                        dates[10] = true; changed = true;
                        textBox57.Text = dateTimePicker10.Value.Day.ToString();
                        textBox59.Text = dateTimePicker10.Value.Month.ToString();
                        textBox58.Text = dateTimePicker10.Value.Year.ToString();
                    }
                }
            }
            else
            {
                from_tb = false;
            }
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
        public override void herlaad()
        {
            if (!wijzigstand)
            {
                
                NavigateRecord();
                
            }
            base.herlaad();
           
        }
        public override void save()
        {
            btnopslaan.PerformClick();
        }
        private void Clear_text_tb(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is TextBox)
                {
                    (c as TextBox).Clear();
                }

                if (c.Controls.Count > 0) Clear_text_tb(c);
            }
        }
        private Point convarPos = new Point();
        private Point bdrvarPos = new Point();
        public void hold_autosize(Variabele_set vs)
        {
            /* if (vs == variabele_set1)
             {
               //  convarPos = Con_var_box.Location;
                 convarH = variabele_set1.Height;
                 variabele_set1.AutoSize = false;
                 variabele_set1.Height = convarH;
                 variabele_set1.Location = new Point(variabele_set1.Location.X, variabele_set2.Location.Y);
             }
             else if (vs == variabele_set2)
             {
                // bdrvarPos = Bdr_var_box.Location;
                 bdrvarH = variabele_set2.Height;
                 variabele_set2.AutoSize = false;
                 variabele_set2.Height = bdrvarH;
                 variabele_set2.Location = new Point(variabele_set2.Location.X, variabele_set1.Location.Y);
             }*/
        }
        public void resume_autosize(Variabele_set vs)
        {
            /*  if (vs == variabele_set1)
              {
                  variabele_set1.AutoSize = true;
                 // Con_var_box.Location = convarPos;
              }
              else if (vs == variabele_set2)
              {
                  variabele_set2.AutoSize = true;
                  //Bdr_var_box.Location = bdrvarPos;
              }*/
        }
        public void scrolldown(Variabele_set vs)
        {
            panel5.VerticalScroll.Value = panel5.VerticalScroll.Value + 25;
        }
        public void Hide_tel_ctrl(telefoonnummer_user_control tuc)
        {
            if (tuc == telefoonnummer_user_control1)
            {
                elementHost5.Hide();
            }
            else if (tuc == telefoonnummer_user_control2)
            {
                elementHost4.Hide();
            }
            else if (tuc == telefoonnummer_user_control3)
            {
                elementHost6.Hide();
            }
            button33.Show();
            OrganizePanels(1);
        }
        private void project_form_Resize(object sender, EventArgs e)
        {

            Point pt1 = new Point(panel1.Width / 2 - lbl_top.Width / 2, 5);
            Point pt2 = new Point(panel1.Width / 2 - lbl_mid.Width / 2, 28);
            Point pt3 = new Point(panel1.Width / 2 - lbl_bot.Width / 2, panel1.Height - 18);
            lbl_top.Location = pt1;
            lbl_mid.Location = pt2;
            lbl_bot.Location = pt3;

            Point pb1 = new Point(panel4.Width / 2 - 200, 0);
            Point pb2 = new Point(panel4.Width / 2 - 128, 0);
            Point pb3 = new Point(panel4.Width / 2 - 33, 0);
            Point pb4 = new Point(panel4.Width / 2 + 62, 0);
            Point pb5 = new Point(panel4.Width / 2 + 162, 0);
			Point pb6 = new Point(panel4.Width / 2 + 262, 0);
            //  Point pb5 = new Point(panel4.Width / 2 + 150, 0);
            //btn_verwijder.Location = pb1;
            //btn_verwijder_annuleer.Location = pb1;
            button35.Location = pb1;
            button30.Location = pb2;
            button31.Location = pb3;
            button32.Location = pb4;
            button18.Location = pb5;
			button34.Location = pb6;
            // btn_verwijder_bevestig.Location = pb5;
            Point pl1 = new Point(pb2.X + 10, pb2.Y + 70);
            Point pl2 = new Point(pb3.X - 3, pb3.Y + 70);
            Point pl3 = new Point(pb4.X + 15, pb4.Y + 70);
            label44.Location = pl1;
            label54.Location = pl2;
            label57.Location = pl3;
            elementHost3.Location = new Point(panel2.Width / 2 - elementHost3.Width / 2, elementHost3.Location.Y);
            Point PLinks = new Point((panel1.Width / 2 - p_projinfo.Width) - 5, p_projinfo.Location.Y);
            p_projinfo.Location = PLinks;

            Point PRechts = new Point((panel1.Width / 2) + 5, p_betrokken_intern.Location.Y);
            p_betrokken_intern.Location = PRechts;

            PLinks.Y = p_planning.Location.Y;
            p_planning.Location = PLinks;

            PRechts.Y = p_contract.Location.Y;
            p_contract.Location = PRechts;

            PLinks.Y = p_factuur_info.Location.Y;
            p_factuur_info.Location = PLinks;

            PLinks.Y = variabele_set1.Location.Y;
            variabele_set1.Location = PLinks;

            PRechts.Y = p_NAW.Location.Y;
            p_NAW.Location = PRechts;

            PRechts.Y = variabele_set2.Location.Y;
            variabele_set2.Location = PRechts;

            PLinks.Y = p_ontvangen_gegevens.Location.Y;
            p_ontvangen_gegevens.Location = PLinks;

            PLinks.Y = p_meerwerk.Location.Y;
            p_meerwerk.Location = PLinks;

            PRechts.Y = p_instellingen.Location.Y;
            p_instellingen.Location = PRechts;

            PLinks.Y = p_offerte.Location.Y;
            p_offerte.Location = PLinks;


            PRechts.Y = p_0offerte.Location.Y;
            p_0offerte.Location = PRechts;

            PRechts.Y = p_telnrs.Location.Y;
            p_telnrs.Location = PRechts;

            label193.Location = new Point(panel2.Width / 2 - label193.Width / 2, label193.Location.Y);

        }
        private void project_form_FormClosing(object sender, FormClosingEventArgs e)
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
                antw = Is_ingebruik(int.Parse(id), 1);
                if (antw.Length > 1 && antw == Global.username)
                {
                    if (!Verwijder_ingebruik(int.Parse(id), 1))
                    {
                        MessageBox.Show("fout bij vrijgeven closing " + antw);
                    }
                }
            }
        }

        private void button30_Click(object sender, EventArgs e)
        {
            SuspendLayout();
            button30.Image = zeebregtsCs.Properties.Resources.Iftab;
            button31.Image = zeebregtsCs.Properties.Resources.bdtapgray;
            button32.Image = zeebregtsCs.Properties.Resources.oftabgray;
            button18.Image = zeebregtsCs.Properties.Resources.haxorgray;
			panel5.Show(); tab = 1; if (upsub) { do_edit(); } else { no_edit(false); }
            panel5.Focus();
            panel6.Hide();
            panel7.Hide();
            panel2.Hide();
            ResumeLayout();
            panel5.Focus();
        }
        private void button31_Click(object sender, EventArgs e)
        {
            SuspendLayout();
            button30.Image = zeebregtsCs.Properties.Resources.iftabgray;
            button31.Image = zeebregtsCs.Properties.Resources.Bdtab;
            button32.Image = zeebregtsCs.Properties.Resources.oftabgray;
            button18.Image = zeebregtsCs.Properties.Resources.haxorgray;
            panel5.Hide();
			panel6.Show(); tab = 2; if (upsub) { do_edit(); } else { no_edit(false); }
            panel6.Focus();
            panel7.Hide();
            panel2.Hide();
            ResumeLayout();
            panel6.Focus();
        }
        private void button32_Click(object sender, EventArgs e)
        {
            SuspendLayout();
            button30.Image = zeebregtsCs.Properties.Resources.iftabgray;
            button31.Image = zeebregtsCs.Properties.Resources.bdtapgray;
            button32.Image = zeebregtsCs.Properties.Resources.Oftab;
            button18.Image = zeebregtsCs.Properties.Resources.haxorgray;
            panel5.Hide();
            panel6.Hide();
			panel7.Show(); tab = 3; if (upsub) { do_edit(); } else { no_edit(false); }
            panel7.Focus();
            panel2.Hide();
            ResumeLayout();
            panel7.Focus();
        }

		private void button34_Click(object sender, EventArgs e)//Exceltab
		{

            //ExcelViewerMDI EVM = new ExcelViewerMDI(int.Parse(id));
            //EVM.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            //EVM.WindowState = Global.windowstate;
            //if (Global.windowstate != System.Windows.Forms.FormWindowState.Maximized)
            //{
            //    EVM.Location = Global.position;
            //    EVM.Size = Global.size;
            //}
            //EVM.Show(this);
            //this.Hide();
		}
		private void button18_Click(object sender, EventArgs e)//Haxortab
		{
			SuspendLayout();
			string naam_proj = String.Empty;
			string plaats = String.Empty;
			int opdgeef = 0;
			if (!testrow.Isnaam_projectNull())
			{
				naam_proj = testrow.naam_project;
			}
			if (!testrow.IsplaatsNull())
			{
				plaats = testrow.plaats;
			}
			if (!testrow.IsopdrachtgeverZEEBREGTS_nrNull())
			{
				opdgeef = testrow.opdrachtgeverZEEBREGTS_nr;
			}
			haxor_panel1.start_loading_proj(naam_proj, plaats, opdgeef);
			button18.Image = zeebregtsCs.Properties.Resources.haxor;
			button30.Image = zeebregtsCs.Properties.Resources.iftabgray;
			button31.Image = zeebregtsCs.Properties.Resources.bdtapgray;
			button32.Image = zeebregtsCs.Properties.Resources.oftabgray;
			panel5.Hide();
			panel6.Hide();
			panel7.Hide();
			panel2.Show();
			ResumeLayout();
			panel2.Focus();
		}
        
		private void btn_verwijder_Click(object sender, EventArgs e)
        {
            btn_verwijder_annuleer.Show();
            btn_verwijder.Hide();
            btn_verwijder_bevestig.Show();
            ScreenCapture();
        }
        private void btn_verwijder_annuleer_Click(object sender, EventArgs e)
        {
            btn_verwijder.Show();
            btn_verwijder_annuleer.Hide();
            btn_verwijder_bevestig.Hide();
            ScreenCapture_off();
        }
        private void btn_verwijder_bevestig_Click(object sender, EventArgs e)
        {

            var mdrCount = CheckMdrGebruik();
            if (mdrCount > 0)
            {
                MessageBox.Show("Dit project komt " + mdrCount + " keer voor in de Mandagen Registratie.\n Hierdoor kan dit project niet worden verwijderd.");
                btn_verwijder_annuleer.PerformClick();
                btnterug.PerformClick();
                return;
            }
            else if(mdrCount == -1)
            {
                MessageBox.Show("Het gebruik van dit project in de Mandagen Registratie kan niet worden geverifieerd. \nHierdoor is het verwijderen van projecten niet mogelijk");
                btn_verwijder_annuleer.PerformClick();
                btnterug.PerformClick();
                return;
            }


            var con = new SqlConnection();
            con.ConnectionString = Global.ConnectionString_fileserver;
            con.Open();
            string stopro = "delete_project";
            SqlCommand command = new SqlCommand(stopro, con);
            SqlParameter projid = command.Parameters.Add("@projid", SqlDbType.Int); 
            projid.Value = int.Parse(id);
            projid.Direction = ParameterDirection.Input;
            command.CommandType = CommandType.StoredProcedure;
            int ok = command.ExecuteNonQuery();
            if (ok > 0)
            {
                Recalc_bij_del();
                variabele_set1.recalc_bij_del();
                variabele_set2.recalc_bij_del();
                MessageBox.Show("succesvol verwijderd");
                handelingen_logger.log_handeling(int.Parse(id), 1, 21);
                this.wijzigstand = false;
                telefoonnummer_user_control1.Wijzigstand(wijzigstand);
                telefoonnummer_user_control2.Wijzigstand(wijzigstand);
                telefoonnummer_user_control3.Wijzigstand(wijzigstand);
                Verwijder_ingebruik(int.Parse(id), 1);
                if (start_parent != null)
                {
                    if (!start_parent.IsDisposed)
                    {
                        if (start_parent is overview1)
                        { (start_parent as overview1).refresh_zoek(); }
                        start_parent.herlaad();
                    }
                    else
                    {
                        FormManager.GetMenu().herlaad();
                    }
                }

                this.sluit();
                Close();
            }
             
        }
        private int CheckMdrGebruik()
        {
            var result = -1;
            var mdrcon = new System.Data.SqlClient.SqlConnection();
            mdrcon.ConnectionString = Global.ConnectionString_Mdr;
            string stopro = "p_CountDependableObjectsVoorProjectNr";
            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = stopro;
            command.Connection = mdrcon;
            SqlParameter ProjectNr = command.Parameters.Add("@ProjectNr", SqlDbType.Int);
            SqlParameter Count = command.Parameters.Add("@Count", SqlDbType.Int);
            ProjectNr.Value = id ;
            ProjectNr.Direction = ParameterDirection.Input;
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



        private void textBox_numonly_KeyPress(object sender, KeyPressEventArgs e)
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
        private void splitContainer11_Panel2_Leave(object sender, EventArgs e)
        {
            Panel p = (Panel)sender;
            int w, j;
            String tmpr = "";
            if (textBox10.TextLength == 1)
            {
                tmpr = "200" + textBox10.Text;
                textBox10.Text = tmpr;
            }
            else if (textBox10.TextLength == 2)
            {
                tmpr = "20" + textBox10.Text;
                textBox10.Text = tmpr;
            }
            else if (textBox10.TextLength == 3)
            {
                tmpr = "2" + textBox10.Text;
                textBox10.Text = tmpr;
            }

            if (textBox28.TextLength == 1)
            {
                tmpr = "0" + textBox28.Text;
                textBox28.Text = tmpr;
            }
            int.TryParse(textBox28.Text, out w);
            int.TryParse(textBox10.Text, out j);

            if( j > 0 )
            {
                
                    
                if (w > 0)
                {

                    if (j > 1950 && j < 2100)
                    {
                        DateTime dt = new DateTime(j, 12, 31);
                        
                        if (w > 0 && w <= 53)// niet kleiner dan 1
                        {
                            DateTime date = FirstDateOfWeek(j, w);
                            
                            dateTimePicker3.Value = date;
                            
                            
                        }
                        else
                        {
                            MessageBox.Show("ongeldig weeknummer ");
                            textBox28.Focus();
                            return ;
                        }
                    }
                    else
                    {
                        MessageBox.Show("geef een geldig jaar op");
                        textBox10.Focus();
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Geef een weeknummer op");
                    textBox28.Focus();
                    return;
                }
            }
            else if (w > 0)
            {
                MessageBox.Show("geef ook een geldig jaar op");

                textBox10.Focus();
                return;
            }
            else
            {
                
            }
        }
        private void splitContainer15_Panel2_Leave(object sender, EventArgs e)
        {
            Panel p = (Panel)sender;
            int w, j;
            String tmpr = "";
            if (textBox11.TextLength == 1)
            {
                tmpr = "200" + textBox11.Text;
                textBox11.Text = tmpr;
            }
            else if (textBox11.TextLength == 2)
            {
                tmpr = "20" + textBox11.Text;
                textBox11.Text = tmpr;
            }
            else if (textBox11.TextLength == 3)
            {
                tmpr = "2" + textBox11.Text;
                textBox11.Text = tmpr;
            }

            if (textBox29.TextLength == 1)
            {
                tmpr = "0" + textBox29.Text;
                textBox29.Text = tmpr;
            }
            int.TryParse(textBox29.Text, out w);
            int.TryParse(textBox11.Text, out j);
            if(j > 0 )
            {
                if (w > 0)
                {

                    if (j > 1950 && j < 2100)
                    {
                        DateTime dt = new DateTime(j, 12, 31);
                       
                        if (w > 0 && w <= 53)// niet kleiner dan 1
                        {
                            DateTime date = FirstDateOfWeek(j, w);
                            
                            dateTimePicker4.Value = date;
                        }
                        else
                        {
                            MessageBox.Show("ongeldig weeknummer ");
                            textBox29.Focus();
                            return;
                
                        }
                    }
                    else
                    {
                        MessageBox.Show("geef een geldig jaar op");
                        textBox11.Focus();
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Geef een weeknummer op");
                    textBox29.Focus();
                    return;
                }
            }
            else if (w > 0)
            {
                MessageBox.Show("geef ook een geldig jaar op");

                textBox11.Focus();
                return;
            }
            else
            {
                //bijde leeg
            }
        }
        private void splitContainer17_Panel2_Leave(object sender, EventArgs e)//start_Check
        {
            if (textBox12.Text == "" && textBox15.Text == "" && textBox16.Text == "")
            {
                dates[11] = false;
                dateTimePicker11.Value = DateTimePicker.MinimumDateTime;
            }
            else
            {
                if (textBox16.Text == "")
                {
                    MessageBox.Show("Geef ook een waarde voor dag op.");
                    textBox16.Focus();
                    return;
                }
                else if (textBox15.Text == "")
                {
                    MessageBox.Show("Geef ook een waarde voor maand op.");
                    textBox15.Focus();
                    return;
                }
                else if (textBox12.Text == "")
                {
                    MessageBox.Show("Geef ook een waarde voor jaar op.");
                    textBox12.Focus();
                    return;
                }

                string date = "";
                if (textBox12.TextLength == 4)
                { date = textBox12.Text + "/" + textBox15.Text + "/" + textBox16.Text; }
                else if (textBox12.TextLength == 3)
                { date = "2" + textBox12.Text + "/" + textBox15.Text + "/" + textBox16.Text; }
                else if (textBox12.TextLength == 2)
                { date = "20" + textBox12.Text + "/" + textBox15.Text + "/" + textBox16.Text; }
                else if (textBox12.TextLength == 1)
                { date = "200" + textBox12.Text + "/" + textBox15.Text + "/" + textBox16.Text; }
                DateTime testdt;
                if (DateTime.TryParse(date, out testdt))
                {
                    dateTimePicker11.Value = testdt;
                    textBox16.Text = dateTimePicker11.Value.Day.ToString();
                    textBox15.Text = dateTimePicker11.Value.Month.ToString();
                    textBox12.Text = dateTimePicker11.Value.Year.ToString();

                }
                else
                {
                    MessageBox.Show("ongeldige datum");
                    textBox16.Focus();
                    return;
                }
            }
            
        }
        private void splitContainer23_Panel2_Leave(object sender, EventArgs e)//offaanvraag
        {
            if (textBox23.Text == "" && textBox18.Text == "" && textBox26.Text == "")
            {
                dates[5] = false;
                dateTimePicker5.Value = DateTimePicker.MinimumDateTime;
            }
            else
            {
                if (textBox26.Text == "")
                {
                    MessageBox.Show("Geef ook een waarde voor dag op.");
                    textBox26.Focus();
                    return;
                }
                else if (textBox18.Text == "")
                {
                    MessageBox.Show("Geef ook een waarde voor maand op.");
                    textBox18.Focus();
                    return;
                }
                else if (textBox23.Text == "")
                {
                    MessageBox.Show("Geef ook een waarde voor jaar op.");
                    textBox23.Focus();
                    return;
                }

                string date = "";
                if (textBox23.TextLength == 4)
                { date = textBox23.Text + "/" + textBox18.Text + "/" + textBox26.Text; }
                else if (textBox23.TextLength == 3)
                { date = "2" + textBox23.Text + "/" + textBox18.Text + "/" + textBox26.Text; }
                else if (textBox23.TextLength == 2)
                { date = "20" + textBox23.Text + "/" + textBox18.Text + "/" + textBox26.Text; }
                else if (textBox23.TextLength == 1)
                { date = "200" + textBox23.Text + "/" + textBox18.Text + "/" + textBox26.Text; }
                DateTime testdt;
                if (DateTime.TryParse(date, out testdt))
                {
                    dateTimePicker5.Value = testdt;
                    textBox26.Text = dateTimePicker5.Value.Day.ToString();
                    textBox18.Text = dateTimePicker5.Value.Month.ToString();
                    textBox23.Text = dateTimePicker5.Value.Year.ToString();

                }
                else
                {
                    MessageBox.Show("ongeldige datum");
                    textBox26.Focus();
                    return;
                }
            }
            
        }
        private void splitContainer22_Panel2_Leave(object sender, EventArgs e)//offdeadline
        {
            if (textBox30.Text == "" && textBox31.Text == "" && textBox27.Text == "")
            {
                dates[1] = false;
                dateTimePicker1.Value = DateTimePicker.MinimumDateTime;
            }
            else
            {
                if (textBox27.Text == "")
                {
                    MessageBox.Show("Geef ook een waarde voor dag op.");
                    textBox27.Focus();
                    return;
                }
                else if (textBox31.Text == "")
                {
                    MessageBox.Show("Geef ook een waarde voor maand op.");
                    textBox31.Focus();
                    return;
                }
                else if (textBox30.Text == "")
                {
                    MessageBox.Show("Geef ook een waarde voor jaar op.");
                    textBox30.Focus();
                    return;
                }

                string date = "";
                if (textBox30.TextLength == 4)
                { date = textBox30.Text + "/" + textBox31.Text + "/" + textBox27.Text; }
                else if (textBox30.TextLength == 3)
                { date = "2" + textBox30.Text + "/" + textBox31.Text + "/" + textBox27.Text; }
                else if (textBox30.TextLength == 2)
                { date = "20" + textBox30.Text + "/" + textBox31.Text + "/" + textBox27.Text; }
                else if (textBox30.TextLength == 1)
                { date = "200" + textBox30.Text + "/" + textBox31.Text + "/" + textBox27.Text; }
                DateTime testdt;
                if (DateTime.TryParse(date, out testdt))
                {
                    dateTimePicker1.Value = testdt;
                    textBox27.Text = dateTimePicker1.Value.Day.ToString();
                    textBox31.Text = dateTimePicker1.Value.Month.ToString();
                    textBox30.Text = dateTimePicker1.Value.Year.ToString();

                }
                else
                {
                    MessageBox.Show("ongeldige datum");
                    textBox27.Focus();
                    return;
                }
            }
            
        }
        private void splitContainer2_Panel2_Leave(object sender, EventArgs e)//offverstuurd
        {
            if (textBox37.Text == "" && textBox38.Text == "" && textBox36.Text == "")
            {
                dates[6] = false;
                dateTimePicker6.Value = DateTimePicker.MinimumDateTime;
            }
            else
            {
                if (textBox36.Text == "")
                {
                    MessageBox.Show("Geef ook een waarde voor dag op.");
                    textBox36.Focus();
                    return;
                }
                else if (textBox38.Text == "")
                {
                    MessageBox.Show("Geef ook een waarde voor maand op.");
                    textBox38.Focus();
                    return;
                }
                else if (textBox37.Text == "")
                {
                    MessageBox.Show("Geef ook een waarde voor jaar op.");
                    textBox37.Focus();
                    return;
                }

                string date = "";
                if (textBox37.TextLength == 4)
                { date = textBox37.Text + "/" + textBox38.Text + "/" + textBox36.Text; }
                else if (textBox37.TextLength == 3)
                { date = "2" + textBox37.Text + "/" + textBox38.Text + "/" + textBox36.Text; }
                else if (textBox37.TextLength == 2)
                { date = "20" + textBox37.Text + "/" + textBox38.Text + "/" + textBox36.Text; }
                else if (textBox37.TextLength == 1)
                { date = "200" + textBox37.Text + "/" + textBox38.Text + "/" + textBox36.Text; }
                DateTime testdt;
                if (DateTime.TryParse(date, out testdt))
                {
                    dateTimePicker6.Value = testdt;
                    textBox36.Text = dateTimePicker6.Value.Day.ToString();
                    textBox38.Text = dateTimePicker6.Value.Month.ToString();
                    textBox37.Text = dateTimePicker6.Value.Year.ToString();

                }
                else
                {
                    MessageBox.Show("ongeldige datum");
                    textBox36.Focus();
                    return;
                }
            }
            
        }
        private void splitContainer1_Panel2_Leave(object sender, EventArgs e)//offdefinitief
        {
            if (textBox40.Text == "" && textBox41.Text == "" && textBox39.Text == "")
            {
                dates[7] = false;
                dateTimePicker7.Value = DateTimePicker.MinimumDateTime;
            }
            else
            {
                if (textBox39.Text == "")
                {
                    MessageBox.Show("Geef ook een waarde voor dag op.");
                    textBox39.Focus();
                    return;
                }
                else if (textBox41.Text == "")
                {
                    MessageBox.Show("Geef ook een waarde voor maand op.");
                    textBox41.Focus();
                    return;
                }
                else if (textBox40.Text == "")
                {
                    MessageBox.Show("Geef ook een waarde voor jaar op.");
                    textBox40.Focus();
                    return;
                }

                string date = "";
                if (textBox40.TextLength == 4)
                { date = textBox40.Text + "/" + textBox41.Text + "/" + textBox39.Text; }
                else if (textBox40.TextLength == 3)
                { date = "2" + textBox40.Text + "/" + textBox41.Text + "/" + textBox39.Text; }
                else if (textBox40.TextLength == 2)
                { date = "20" + textBox40.Text + "/" + textBox41.Text + "/" + textBox39.Text; }
                else if (textBox40.TextLength == 1)
                { date = "200" + textBox40.Text + "/" + textBox41.Text + "/" + textBox39.Text; }
                DateTime testdt;
                if (DateTime.TryParse(date, out testdt))
                {
                    dateTimePicker7.Value = testdt;
                    textBox39.Text = dateTimePicker7.Value.Day.ToString();
                    textBox41.Text = dateTimePicker7.Value.Month.ToString();
                    textBox40.Text = dateTimePicker7.Value.Year.ToString();

                }
                else
                {
                    MessageBox.Show("ongeldige datum");
                    textBox39.Focus();
                    return;
                }
            }
            
        }
        private void splitContainer3_Panel2_Leave(object sender, EventArgs e)//0offaanvraag
        {
            if (textBox43.Text == "" && textBox44.Text == "" && textBox42.Text == "")
            {
                dates[8] = false;
                dateTimePicker8.Value = DateTimePicker.MinimumDateTime;
            }
            else
            {
                if (textBox42.Text == "")
                {
                    MessageBox.Show("Geef ook een waarde voor dag op.");
                    textBox42.Focus();
                    return;
                }
                else if (textBox44.Text == "")
                {
                    MessageBox.Show("Geef ook een waarde voor maand op.");
                    textBox44.Focus();
                    return;
                }
                else if (textBox43.Text == "")
                {
                    MessageBox.Show("Geef ook een waarde voor jaar op.");
                    textBox43.Focus();
                    return;
                }

                string date = "";
                if (textBox43.TextLength == 4)
                { date = textBox43.Text + "/" + textBox44.Text + "/" + textBox42.Text; }
                else if (textBox43.TextLength == 3)
                { date = "2" + textBox43.Text + "/" + textBox44.Text + "/" + textBox42.Text; }
                else if (textBox43.TextLength == 2)
                { date = "20" + textBox43.Text + "/" + textBox44.Text + "/" + textBox42.Text; }
                else if (textBox43.TextLength == 1)
                { date = "200" + textBox43.Text + "/" + textBox44.Text + "/" + textBox42.Text; }
                DateTime testdt;
                if (DateTime.TryParse(date, out testdt))
                {
                    dateTimePicker8.Value = testdt;
                    textBox42.Text = dateTimePicker8.Value.Day.ToString();
                    textBox44.Text = dateTimePicker8.Value.Month.ToString();
                    textBox43.Text = dateTimePicker8.Value.Year.ToString();

                }
                else
                {
                    MessageBox.Show("ongeldige datum");
                    textBox42.Focus();
                    return;
                }
            }
            
        }
        private void splitContainer6_Panel2_Leave(object sender, EventArgs e)//0offdeadline
        {
            if (textBox51.Text == "" && textBox52.Text == "" && textBox50.Text == "")
            {
                dates[2] = false;
                dateTimePicker2.Value = DateTimePicker.MinimumDateTime;
            }
            else
            {
                if (textBox50.Text == "")
                {
                    MessageBox.Show("Geef ook een waarde voor dag op.");
                    textBox50.Focus();
                    return;
                }
                else if (textBox52.Text == "")
                {
                    MessageBox.Show("Geef ook een waarde voor maand op.");
                    textBox52.Focus();
                    return;
                }
                else if (textBox51.Text == "")
                {
                    MessageBox.Show("Geef ook een waarde voor jaar op.");
                    textBox51.Focus();
                    return;
                }

                string date = "";
                if (textBox51.TextLength == 4)
                { date = textBox51.Text + "/" + textBox52.Text + "/" + textBox50.Text; }
                else if (textBox51.TextLength == 3)
                { date = "2" + textBox51.Text + "/" + textBox52.Text + "/" + textBox50.Text; }
                else if (textBox51.TextLength == 2)
                { date = "20" + textBox51.Text + "/" + textBox52.Text + "/" + textBox50.Text; }
                else if (textBox51.TextLength == 1)
                { date = "200" + textBox51.Text + "/" + textBox52.Text + "/" + textBox50.Text; }
                DateTime testdt;
                if (DateTime.TryParse(date, out testdt))
                {
                    dateTimePicker2.Value = testdt;
                    textBox50.Text = dateTimePicker2.Value.Day.ToString();
                    textBox52.Text = dateTimePicker2.Value.Month.ToString();
                    textBox51.Text = dateTimePicker2.Value.Year.ToString();

                }
                else
                {
                    MessageBox.Show("ongeldige datum");
                    textBox50.Focus();
                    return;
                }
            }
            
        }
        private void splitContainer7_Panel2_Leave(object sender, EventArgs e)//0offverstuurd
        {
            if (textBox54.Text == "" && textBox56.Text == "" && textBox53.Text == "")
            {
                dates[9] = false;
                dateTimePicker9.Value = DateTimePicker.MinimumDateTime;
            }
            else
            {
                if (textBox53.Text == "")
                {
                    MessageBox.Show("Geef ook een waarde voor dag op.");
                    textBox53.Focus();
                    return;
                }
                else if (textBox56.Text == "")
                {
                    MessageBox.Show("Geef ook een waarde voor maand op.");
                    textBox56.Focus();
                    return;
                }
                else if (textBox54.Text == "")
                {
                    MessageBox.Show("Geef ook een waarde voor jaar op.");
                    textBox54.Focus();
                    return;
                }

                string date = "";
                if (textBox54.TextLength == 4)
                { date = textBox54.Text + "/" + textBox56.Text + "/" + textBox53.Text; }
                else if (textBox54.TextLength == 3)
                { date = "2" + textBox54.Text + "/" + textBox56.Text + "/" + textBox53.Text; }
                else if (textBox54.TextLength == 2)
                { date = "20" + textBox54.Text + "/" + textBox56.Text + "/" + textBox53.Text; }
                else if (textBox54.TextLength == 1)
                { date = "200" + textBox54.Text + "/" + textBox56.Text + "/" + textBox53.Text; }
                DateTime testdt;
                if (DateTime.TryParse(date, out testdt))
                {
                    dateTimePicker9.Value = testdt;
                    textBox53.Text = dateTimePicker9.Value.Day.ToString();
                    textBox56.Text = dateTimePicker9.Value.Month.ToString();
                    textBox54.Text = dateTimePicker9.Value.Year.ToString();

                }
                else
                {
                    MessageBox.Show("ongeldige datum");
                    textBox53.Focus();
                    return;
                }
            }
            
        }
        private void splitContainer10_Panel2_Leave(object sender, EventArgs e)//0offdefinitief
        {
            if (textBox58.Text == "" && textBox59.Text == "" && textBox57.Text == "")
            {
                dates[10] = false;
                dateTimePicker10.Value = DateTimePicker.MinimumDateTime;
            }
            else
            {
                if (textBox57.Text == "")
                {
                    MessageBox.Show("Geef ook een waarde voor dag op.");
                    textBox57.Focus();
                    return;
                }
                else if (textBox59.Text == "")
                {
                    MessageBox.Show("Geef ook een waarde voor maand op.");
                    textBox59.Focus();
                    return;
                }
                else if (textBox58.Text == "")
                {
                    MessageBox.Show("Geef ook een waarde voor jaar op.");
                    textBox58.Focus();
                    return;
                }

                string date = "";
                if (textBox58.TextLength == 4)
                { date = textBox58.Text + "/" + textBox59.Text + "/" + textBox57.Text; }
                else if (textBox58.TextLength == 3)
                { date = "2" + textBox58.Text + "/" + textBox59.Text + "/" + textBox57.Text; }
                else if (textBox58.TextLength == 2)
                { date = "20" + textBox58.Text + "/" + textBox59.Text + "/" + textBox57.Text; }
                else if (textBox58.TextLength == 1)
                { date = "200" + textBox58.Text + "/" + textBox59.Text + "/" + textBox57.Text; }
                DateTime testdt;
                if (DateTime.TryParse(date, out testdt))
                {
                    dateTimePicker10.Value = testdt;
                    textBox57.Text = dateTimePicker10.Value.Day.ToString();
                    textBox59.Text = dateTimePicker10.Value.Month.ToString();
                    textBox58.Text = dateTimePicker10.Value.Year.ToString();

                }
                else
                {
                    MessageBox.Show("ongeldige datum");
                    textBox57.Focus();
                    return;
                }
            }
            
        }
        private void splitContainer_Panel2_Leave(object sender, EventArgs e)
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
                    if ((c as TextBox).TextLength < 1)
                    {
                        MessageBox.Show("Vul waarde in, dit is een verplicht veld");

                        c.Focus();
                    }
                }
            }
        }
       
        private void splitContainer_Panel1_MouseHover(object sender, EventArgs e)
        {
            ToolTip tt_tmp;
            string tt_text;
            Control c = (Control)sender;
            if (c is SplitterPanel)
            {
                ttd.TryGetValue((c.Parent as SplitContainer), out tt_tmp);
                if (tt_tmp != null)
                {
                    tooltiptexts.TryGetValue(tt_tmp, out tt_text);
                    tt_tmp.Show(tt_text, (c.Parent as SplitContainer).Panel1, 0, -40);
                }
            }
            else if (c is Label || c is Button)
            {
                ttd.TryGetValue((c.Parent.Parent as SplitContainer), out tt_tmp);
                if (tt_tmp != null)
                {
                    tooltiptexts.TryGetValue(tt_tmp, out tt_text);
                    tt_tmp.Show(tt_text, (c.Parent.Parent as SplitContainer).Panel1, 0, -40);
                }
            }


        }
        private void splitContainer_Panel1_MouseLeave(object sender, EventArgs e)
        {
            ToolTip tt_tmp;
            Control c = (Control)sender;
            if (c is SplitterPanel)
            {
                ttd.TryGetValue((c.Parent as SplitContainer), out tt_tmp);
                try { tt_tmp.Hide((c.Parent as SplitContainer).Panel1); }
                catch (Exception ex)
                {
                    log_exception(ex);
                }
            }
            else if (c is Label || c is Button)
            {
                ttd.TryGetValue((c.Parent.Parent as SplitContainer), out tt_tmp);
                try { tt_tmp.Hide((c.Parent.Parent as SplitContainer).Panel1); }
                catch (Exception ex)
                {
                    log_exception(ex);
                }
            }


        }

        bool hold = false;
        private void label184_Click(object sender, EventArgs e)
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
        bool drop = false;
        bool changed = false;
        private void dateTimePicker_DropDown(object sender, EventArgs e)
        {
            if (!hold)
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
            else
            {
                return;
            }
            

        }
        private void dateTimePicker_CloseUp(object sender, EventArgs e)
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

        string bezetnaam = "";
        private void INUSE_BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string result = Is_ingebruik(int.Parse(id), 1);
            while (result.Length > 1)
            {
                result = Is_ingebruik(int.Parse(id), 1);
                Thread.Sleep(5000);
                
            }
            
        }
        private void INUSE_BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Het project " + vrijgeefstring + " is zojuist vrijgegeven door " + bezetnaam + ".\n U kunt dit formulier nu wijzigen.");
            btnwijzig.Enabled = true;
        }

        private void textBoxNumOnly_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (tb.TextLength > 0)
            {
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
            else { tb.BackColor = Color.White; }
        }
        private void linkLabel25_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)//memo offerte
        {
            String tmpveldnaam = "memo offerte ";
            int tmpProjectID = int.Parse(id);
            String tmpownrnm = testrow.naam_project;
            if (!wijzigstand)
            {
                TextBrowser tb = new TextBrowser(linkLabel25.Text, linkLabel25, wijzigstand, tmpveldnaam, tmpownrnm, tmpProjectID,this);
                tb.ShowDialog();
            }
        }
        private void linkLabel26_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)//memo 0offerte
        {
            String tmpveldnaam = "memo 0 offerte ";
            int tmpProjectID = int.Parse(id);
            String tmpownrnm = testrow.naam_project;
            if (!wijzigstand)
            {
                TextBrowser tb = new TextBrowser(linkLabel26.Text, linkLabel26, wijzigstand, tmpveldnaam, tmpownrnm, tmpProjectID,this);
                tb.ShowDialog();
            }
        }
        private void button16_Click(object sender, EventArgs e)//memo offerte
        {
            String tmpveldnaam = "memo offerte ";
            int tmpProjectID = int.Parse(id);
            String tmpownrnm = testrow.naam_project;
            Global.windowstate = this.WindowState;
            if (this.WindowState != FormWindowState.Maximized)
            {
                Global.position = this.Location;
                Global.size = this.Size;
            }
            
            TextBrowser tb = new TextBrowser(textBox61.Text, textBox61, wijzigstand, tmpveldnaam, tmpownrnm, tmpProjectID,this);
            tb.Show();
            this.Hide();
            panel7.Focus();
        }
        private void button17_Click(object sender, EventArgs e)//memo 0 offerte
        {
            String tmpveldnaam = "memo 0 offerte ";
            int tmpProjectID = int.Parse(id);
            String tmpownrnm = testrow.naam_project;
            Global.windowstate = this.WindowState;
            if (this.WindowState != FormWindowState.Maximized)
            {
                Global.position = this.Location;
                Global.size = this.Size;
            }
            
            TextBrowser tb = new TextBrowser(textBox63.Text, textBox63, wijzigstand, tmpveldnaam, tmpownrnm, tmpProjectID,this);
                tb.Show();
                this.Hide();
                panel7.Focus();
        }
        private void linkLabel25_TextChanged(object sender, EventArgs e)
        {
            if (linkLabel25.Text.Length < 3 || linkLabel25.Text == "...")
            {
                linkLabel25.Text = "";
            }
            label1.Text = linkLabel25.Text;
        }
        private void linkLabel26_TextChanged(object sender, EventArgs e)
        {
            if (linkLabel26.Text.Length < 3|| linkLabel26.Text == "...")
            {
                linkLabel26.Text = "";
            }
            label3.Text = linkLabel26.Text;
        }
        private void textBox60_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 43 || e.KeyChar == 45 || e.KeyChar == 8 || e.KeyChar == 46)
            {

            }
            else
            {
                e.Handled = true;
            }
        }
        private void button28_Click(object sender, EventArgs e)
        {
            string summary = String.Empty;
            string copy_naam = String.Empty;
            string copy_nummer = String.Empty;
            string copy_opdrachtgever = String.Empty;
            string copy_aannemer = String.Empty;
            string copy_tel = String.Empty;
            string copy_adres = String.Empty;
            if (!testrow.Isnaam_projectNull())
            {
                copy_naam = testrow.naam_project;
            }
            if (!testrow.Isproject_NRNull())
            {
                copy_nummer = testrow.project_NR.ToString();
            }
            copy_opdrachtgever = label69.Text;
            
            if (copy_aannemer.Length > 1)
            {
                copy_aannemer ="\nAannemer: "+ label70.Text;
            }
            if (!testrow.Isbouw_telNull() && testrow.bouw_tel.Length > 1)
            {
                copy_tel = "\nTelefoonnummer: "+testrow.bouw_tel;
            }
            /*if (adres_user_control1.Postcode_cijfers.ToString().Length > 1)
            {
                copy_adres = "Postcode: " + adres_user_control1.Postcode_cijfers + adres_user_control1.Postcode_letters + "\n";
            }
            if (adres_user_control1.Straat.ToString().Length > 1)
            {
                copy_adres += "Straat: " + adres_user_control1.Straat +" "+ adres_user_control1.Huisnummer+adres_user_control1.Huisnummer_toevoeging+ "\n";
            }*/
            if (adres_user_control1.Straat.ToString().Length > 3)
            {
                copy_adres += "Adres: " + adres_user_control1.Straat;
                if (adres_user_control1.Huisnummer.Length > 0)
                {
                    copy_adres += " " + adres_user_control1.Huisnummer + adres_user_control1.Huisnummer_toevoeging;
                }
                if (adres_user_control1.Postcode_cijfers.ToString().Length > 0)
                {
                    copy_adres += ", " + adres_user_control1.Postcode_cijfers + adres_user_control1.Postcode_letters;
                }
                if (adres_user_control1.Plaats.Length > 0)
                {
                    copy_adres += " " + adres_user_control1.Plaats + "\n";
                }
            }
            else if (adres_user_control1.Plaats.ToString().Length > 1)
            {
                copy_adres += "Plaats: " + adres_user_control1.Plaats + "\n";
            }
            summary = "Projectnummer: " + copy_nummer + "\nProjectnaam: " + copy_naam + "\nOpdrachtgever: " + copy_opdrachtgever + copy_tel + "\n" + copy_adres;
            System.Windows.Forms.Clipboard.SetDataObject(summary, true);
            handelingen_logger.log_handeling(int.Parse(id), 1, 18);
        }
        //private int convarH = 0;
        //private int bdrvarH = 0;
        
        private void panel5_Click(object sender, EventArgs e)
        {
            Panel p = sender as Panel;
            p.Focus();
        }
        private void button29_Click(object sender, EventArgs e)///naam spec btnclick
        {
            String tmpveldnaam = "project specificatie ";
            int tmpProjectID = int.Parse(id);
            String tmpownrnm = testrow.naam_project;
            Global.windowstate = this.WindowState;
            if (this.WindowState != FormWindowState.Maximized)
            {
                Global.position = this.Location;
                Global.size = this.Size;
            }
            
            TextBrowser tb = new TextBrowser(textBox60.Text, textBox60, wijzigstand, tmpveldnaam, tmpownrnm, tmpProjectID,this);
            tb.Show();
            this.Hide();
            panel5.Focus();
            
        }
        private void linkLabel27_Click(object sender, EventArgs e)//naam spec linkclicked
        {
            String tmpveldnaam = "project specificatie ";
            int tmpProjectID = int.Parse(id);
            String tmpownrnm = testrow.naam_project;
            if (!wijzigstand)
            {
                TextBrowser tb = new TextBrowser(linkLabel27.Text, linkLabel27, wijzigstand, tmpveldnaam, tmpownrnm, tmpProjectID,this);
                tb.ShowDialog();
            }
        }
        private void linkLabel27_TextChanged(object sender, EventArgs e)//naam spec textchanged
        {
            if (linkLabel27.Text.Length < 3 || linkLabel27.Text == "...")
            {
                linkLabel27.Text = "";
            }
            label192.Text = linkLabel27.Text;
        }

        private void show_mem_btn(TextBox tb, Button B)
        {
            Bitmap b = new Bitmap(tb.Width - 10, tb.Height - 10);
            Graphics g = tb.CreateGraphics();
            Size s = g.MeasureString(tb.Text, tb.Font, b.Width).ToSize();
            if (wijzigstand)
            {
                //if (s.Height > tb.Height - 8)
                //{
                //   tb.ScrollBars = ScrollBars.Vertical;
                //    //tb.ScrollToCaret();
                //    //tb.Select(tb.Text.Length - 1, 0);
                //}
                //else
                //{
                //    tb.ScrollBars = ScrollBars.None; // tb.ReadOnly = false;
                //}
                if (tb.Height / 4 * 3 < s.Height)
                {
                    B.Show();
                }
                else
                {
                    B.Hide();
                }
            }
            else
            {
                if (s.Height > tb.Height - 8)
                {
                    //tb.ScrollBars = ScrollBars.Vertical;
                    //tb.ScrollToCaret();
                   // tb.Select(tb.Text.Length - 1, 0);
                    B.Show();
                }
                else
                {
                   // tb.ScrollBars = ScrollBars.None; // tb.ReadOnly = false;
                    B.Hide();
                }
            }
        }
        private void textBox61_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = sender as TextBox;
            LinkLabel LL = new LinkLabel();
            Button B = new Button();
            if (tb == textBox60)
            {
                LL = linkLabel27;
                B = button29;
            }
            else if (tb == textBox61)
            {
                LL = linkLabel25;
                B = button16;
            }
            else if (tb == textBox63)
            {
                LL = linkLabel26;
                B = button17;
            }
            //show_mem_btn(tb, B);
            
            LL.Text = wrap_text(tb.Text);
        }
        private void textBox60_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb == textBox60)
            {
                show_mem_btn(tb, button29);
            }
            else if (tb == textBox61)
            {
                show_mem_btn(tb, button16);
            }
            else if (tb == textBox63)
            {
                show_mem_btn(tb, button17);
            }
        }
        private void memo_tooltip_MouseHover(object sender, EventArgs e)
        {
            
            ToolTip_settings(tt_memo);
            string tt_text = "Gebruik de pijl om over te schakelen naar een uitgebreider invoerscherm";
            Control c = (Control)sender;
            if (c is SplitterPanel)
            {
                if ((c.Parent as SplitContainer) == splitContainer80)
                {
                    if (button29.Visible)
                    {
                        tt_memo.Show(tt_text, (c.Parent as SplitContainer).Panel1, 0, -40);
                    }
                }
                if ((c.Parent as SplitContainer) == splitContainer4)
                {
                    if (button16.Visible)
                    {
                        tt_memo.Show(tt_text, (c.Parent as SplitContainer).Panel1, 0, -40);
                    }
                } if ((c.Parent as SplitContainer) == splitContainer14)
                {
                    if (button17.Visible)
                    {
                        tt_memo.Show(tt_text, (c.Parent as SplitContainer).Panel1, 0, -40);
                    }
                }
            }
            else if (c is Label || c is Button)
            {
                if ((c.Parent.Parent as SplitContainer) == splitContainer80)
                {
                    if (button29.Visible)
                    {
                        tt_memo.Show(tt_text, (c.Parent.Parent as SplitContainer).Panel1, 0, -40);
                    }
                }
                if ((c.Parent.Parent as SplitContainer) == splitContainer4)
                {
                    if (button16.Visible)
                    {
                        tt_memo.Show(tt_text, (c.Parent.Parent as SplitContainer).Panel1, 0, -40);
                    }
                } if ((c.Parent.Parent as SplitContainer) == splitContainer14)
                {
                    if (button17.Visible)
                    {
                        tt_memo.Show(tt_text, (c.Parent.Parent as SplitContainer).Panel1, 0, -40);
                    }
                }
            }
        }
        private void memo_tooltip_MouseLeave(object sender, EventArgs e)
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
                (c as Button).FlatStyle = FlatStyle.Flat;
            }
        }
        private void linkLabel28_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)//launch website
        {
            ProcessStartInfo sInfo = new ProcessStartInfo(e.Link.LinkData.ToString());
            Process.Start(sInfo);
            handelingen_logger.log_handeling(int.Parse(id), 1, 20);
        }

        private void project_form_Shown(object sender, EventArgs e)
        {
			OrganizePanels(1);
	    }
        private void button27_MouseEnter(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            btn.FlatStyle = FlatStyle.Standard;
        }
        private void button27_MouseLeave(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            btn.FlatStyle = FlatStyle.Flat;
        }
        private void bdr_button_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            Button btn = sender as Button;
            if (wijzigstand)//rood voorm btn
            {
				combox = 0;
                cbb.Clear();
                Save_cbb_settings(this);
                if (this.WindowState == FormWindowState.Normal)
                {
                    Global.size = this.Size;
                    Global.position = this.Location;
                }
                Global.windowstate = this.WindowState;
                String tmpveldnaam = btn.Tag.ToString();
                String tmpownrnm = testrow.naam_project;
                int tmpprojid = int.Parse(id);
               // handelingen_logger.log_handeling(tmpprojid, 1, 6);
                foreach (Control c in btn.Parent.Controls)
                {
                    if (c is MultiColumnComboBoxDemo.MultiColumnComboBox)
                    {
                        cbb_to_fill = c as MultiColumnComboBoxDemo.MultiColumnComboBox;
                    }
                }
                Global.overzicht_type = 2;
                Global.give_return = true;
                Proj_form_helper.Start_route(2, 3, this, this, int.Parse(id), tmpveldnaam, tmpownrnm);
                this.Hide();
                Global.overzicht_type = 0;
                Global.give_return = false;

            }
            else//groen voorm linkl
            {
                String tmpveldnaam = btn.Tag.ToString();
                int tmpProjectID = int.Parse(id);
                String tmpownrnm = testrow.naam_project;
                if (this.WindowState == FormWindowState.Normal)
                {
                    Global.size = this.Size;
                    Global.position = this.Location;
                }
                Global.windowstate = this.WindowState;
               // handelingen_logger.log_handeling(tmpProjectID, 1, 5);
                foreach (Control c in btn.Parent.Controls)
                {
                    if (c is MultiColumnComboBoxDemo.MultiColumnComboBox)
                    {
                        cbb_to_fill = c as MultiColumnComboBoxDemo.MultiColumnComboBox;
                    }
                }
                string temp = cbb_to_fill.SelectedValue.ToString();
                Global.edit_form_bedr = false;
                Global.overzicht_select = temp;
                Proj_form_helper.Start_route(2, 2, this, this, tmpProjectID, tmpveldnaam, tmpownrnm);
                this.Hide();
                handelingen_logger.log_handeling(int.Parse(temp), 1, 10);

            }
            Cursor.Current = Cursors.Default;
        }
        private void cont_button_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            Button btn = sender as Button;
            if (wijzigstand)//rood voorm btn
            {
                cbb.Clear();
				combox = 0;
                Save_cbb_settings(this);
                if (this.WindowState == FormWindowState.Normal)
                {
                    Global.size = this.Size;
                    Global.position = this.Location;
                }
                Global.windowstate = this.WindowState;
                String tmpveldnaam = btn.Tag.ToString();
                String tmpownrnm = testrow.naam_project;
                int tmpprojid = int.Parse(id);
               // handelingen_logger.log_handeling(tmpprojid, 1, 6);
                foreach (Control c in btn.Parent.Controls)
                {
                    if (c is MultiColumnComboBoxDemo.MultiColumnComboBox)
                    {
                        cbb_to_fill = c as MultiColumnComboBoxDemo.MultiColumnComboBox;
                    }
                }
                Global.overzicht_type = 3;
                Global.give_return = true;
                Proj_form_helper.Start_route(3, 3, this, this, int.Parse(id), tmpveldnaam, tmpownrnm);
                this.Hide();
                Global.overzicht_type = 0;
                Global.give_return = false;
            }
            else//groen voorm linkl
            {
                String tmpveldnaam = btn.Tag.ToString();
                int tmpProjectID = int.Parse(id);
                String tmpownrnm = testrow.naam_project;
                if (this.WindowState == FormWindowState.Normal)
                {
                    Global.size = this.Size;
                    Global.position = this.Location;
                }
                Global.windowstate = this.WindowState;
              //  handelingen_logger.log_handeling(tmpProjectID, 1, 5);
                foreach (Control c in btn.Parent.Controls)
                {
                    if (c is MultiColumnComboBoxDemo.MultiColumnComboBox)
                    {
                        cbb_to_fill = c as MultiColumnComboBoxDemo.MultiColumnComboBox;
                    }
                }
                string temp = cbb_to_fill.SelectedValue.ToString();
                Global.edit_form_pers = false;
                Global.overzicht_select = temp;
                Proj_form_helper.Start_route(3, 2, this, this, tmpProjectID, tmpveldnaam, tmpownrnm);
                this.Hide();
                handelingen_logger.log_handeling(int.Parse(temp), 1, 9);
            }
            Cursor.Current = Cursors.Default;
        }

        private void button33_Click(object sender, EventArgs e)//telnrs add
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
            p_telnrs.Controls.Add(sender as Button);
            OrganizePanels(1);
        }

        private void textBox47_Enter(object sender, EventArgs e)
        {
            if (textBox47.Text.Contains(","))
            {
                textBox47.Text = textBox47.Text.Split(',')[0];
            }
            else
            {
                textBox47.Text = textBox47.Text.Split('.')[0];
            }

        }

        private void button35_Click(object sender, EventArgs e)
        {
           
            //MessageBox.Show("ProjNum is " + curItem.ProjNum );


            string myDocspath = @"\\PROLIANT\bedrijfsdata\Bestanden\ProjectFolders\" + id;
            if (Directory.Exists(myDocspath))
            {
                string windir = Environment.GetEnvironmentVariable("WINDIR");
                System.Diagnostics.Process prc = new System.Diagnostics.Process();
                prc.StartInfo.FileName = windir + @"\explorer.exe";
                prc.StartInfo.Arguments = myDocspath;
                prc.Start();
            }
            else
            {
                var result = MessageBox.Show("Directory bestaat nog niet. Wilt u deze nu aanmaken?", "Map Bestaat Niet", MessageBoxButtons.OKCancel);
                if (result == DialogResult.OK)
                {
                    Directory.CreateDirectory(myDocspath);
                    string windir = Environment.GetEnvironmentVariable("WINDIR");
                    System.Diagnostics.Process prc = new System.Diagnostics.Process();
                    prc.StartInfo.FileName = windir + @"\explorer.exe";
                    prc.StartInfo.Arguments = myDocspath;
                    prc.Start();
                }

            }
        }


}
}
