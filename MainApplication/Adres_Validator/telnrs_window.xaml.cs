using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Data;

namespace Adres_Validator
{
    /// <summary>
    /// Interaction logic for telnrs_window.xaml
    /// </summary>
    public partial class telnrs_window : Window
    {
        //database_tabellen.validator_set_1.bedrijf_telnrsDataTable bdt = new database_tabellen.validator_set_1.bedrijf_telnrsDataTable();
        //database_tabellen.validator_set_1TableAdapters.bedrijf_telnrsTableAdapter bta = new database_tabellen.validator_set_1TableAdapters.bedrijf_telnrsTableAdapter();
        //database_tabellen.validator_set_1.persoonDataTable pdt = new database_tabellen.validator_set_1.persoonDataTable();
        //database_tabellen.validator_set_1TableAdapters.persoonTableAdapter pta = new database_tabellen.validator_set_1TableAdapters.persoonTableAdapter();
        database_tabellen.validator_set_1TableAdapters.project_tel_nrsTableAdapter proj_nrs_ta = new database_tabellen.validator_set_1TableAdapters.project_tel_nrsTableAdapter();
        database_tabellen.validator_set_1.project_tel_nrsDataTable proj_nrs_dt = new database_tabellen.validator_set_1.project_tel_nrsDataTable();
        public telnrs_window()
        {
            InitializeComponent();
            proj_nrs_ta.Fill(proj_nrs_dt);
            telnrsgrid_oud.ItemsSource =  Load_oude_nummers();
            telnrsgrid_niew.ItemsSource = Load_nieuwe_nummers();

        }
        private ObservableCollection<telefoon_nummers> Load_oude_nummers()
        {
            ObservableCollection<telefoon_nummers> tel_nrs_oud_list = new ObservableCollection<telefoon_nummers>();
            foreach (database_tabellen.validator_set_1.project_tel_nrsRow projRow in proj_nrs_dt)
            {
                tel_nrs_oud_list.Add(new telefoon_nummers()
                {
                    ID = projRow.project_ID,
                    Tel = projRow.bouw_tel,
                    Fax = projRow.bouw_fax,
                    
                });
            }
            return tel_nrs_oud_list;
        }
        private ObservableCollection<telefoon_nummers> Load_nieuwe_nummers()
        {
            ObservableCollection<telefoon_nummers> tel_nrs_nieuw_list = new ObservableCollection<telefoon_nummers>();
           foreach (database_tabellen.validator_set_1.project_tel_nrsRow projRow in proj_nrs_dt)
            {
                tel_nrs_nieuw_list.Add(new telefoon_nummers()
                {
                    ID = projRow.project_ID,
                    Tel = convert(projRow.bouw_tel),
                    Fax = convert(projRow.bouw_fax)
                });
            }
            return tel_nrs_nieuw_list;
        }
        private string convert(string nummer)
        {
            string retnr = String.Empty;
            if (nummer != String.Empty && nummer != null)
            {
                retnr = "+31";
                nummer = Regex.Replace(nummer, @"^0", @"(0)");
                nummer = Regex.Replace(nummer, @"\s+", @"");
                nummer = Regex.Replace(nummer, @"\-", " ");
                retnr += nummer;
            }
            return retnr;
        }

        private void opslaan_Click(object sender, RoutedEventArgs e)
        {
            /*foreach(database_tabellen.validator_set_1.persoonRow prow in pdt)
            {
                if (prow.telefoon_nr_settings != null)
                {
                    string settings = prow.telefoon_nr_settings;
                    string[] sepp_sett = settings.Split(',');
                    int[] tel_types = new int[3];
                    tel_types[0] = int.Parse(sepp_sett[0]);
                    tel_types[1] = int.Parse(sepp_sett[1]);
                    tel_types[2] = int.Parse(sepp_sett[2]);
                    bool tel_mob_done = false;
                    bool tel_vast_done = false;
                    bool tel_fax_done = false;
                    string vast = String.Empty;
                    string mobiel = String.Empty;
                    string fax = String.Empty;
                    if (tel_types[0] > -1)
                    {
                        switch (tel_types[0])
                        {
                            case 0://vast
                                if (!tel_vast_done)
                                {
                                    vast = prow.telefoon_nr_1;
                                    tel_vast_done = true;
                                }
                                break;
                            case 1://mobiel
                                if (!tel_mob_done)
                                {
                                    mobiel = prow.telefoon_nr_1;
                                    tel_mob_done = true;
                                }
                                break;
                            case 2://fax
                                if (!tel_fax_done)
                                {
                                    fax = prow.telefoon_nr_1;
                                    tel_fax_done = true;
                                }
                                break;
                            case 3://voip
                                if (!tel_vast_done)
                                {
                                    vast = prow.telefoon_nr_1;
                                    tel_vast_done = true;
                                }
                                break;
                            case 4://skype
                                if (!tel_vast_done)
                                {
                                    vast = prow.telefoon_nr_1;
                                    tel_vast_done = true;
                                }
                                break;
                            case 5://bedrijfsnummer
                                if (!tel_vast_done)
                                {
                                    vast = prow.telefoon_nr_1;
                                    tel_vast_done = true;
                                }
                                break;
                        }
                    }
                    if (tel_types[1] > -1)
                    {
                        switch (tel_types[1])
                        {
                            case 0://vast
                                if (!tel_vast_done)
                                {
                                    vast = prow.telefoon_nr_2;
                                    tel_vast_done = true;
                                }
                                break;
                            case 1://mobiel
                                if (!tel_mob_done)
                                {
                                    mobiel = prow.telefoon_nr_2;
                                    tel_mob_done = true;
                                }
                                break;
                            case 2://fax
                                if (!tel_fax_done)
                                {
                                    fax = prow.telefoon_nr_2;
                                    tel_fax_done = true;
                                }
                                break;
                            case 3://voip
                                if (!tel_vast_done)
                                {
                                    vast = prow.telefoon_nr_2;
                                    tel_vast_done = true;
                                }
                                break;
                            case 4://skype
                                if (!tel_vast_done)
                                {
                                    vast = prow.telefoon_nr_2;
                                    tel_vast_done = true;
                                }
                                break;
                            case 5://bedrijfsnummer
                                if (!tel_vast_done)
                                {
                                    vast = prow.telefoon_nr_2;
                                    tel_vast_done = true;
                                }
                                break;
                        }
                    }
                    if (tel_types[2] > -1)
                    {
                        switch (tel_types[2])
                        {
                            case 0://vast
                                if (!tel_vast_done)
                                {
                                    vast = prow.telefoon_nr_3;
                                    tel_vast_done = true;
                                }
                                break;
                            case 1://mobiel
                                if (!tel_mob_done)
                                {
                                    mobiel = prow.telefoon_nr_3;
                                    tel_mob_done = true;
                                }
                                break;
                            case 2://fax
                                if (!tel_fax_done)
                                {
                                    fax = prow.telefoon_nr_3;
                                    tel_fax_done = true;
                                }
                                break;
                            case 3://voip
                                if (!tel_vast_done)
                                {
                                    vast = prow.telefoon_nr_3;
                                    tel_vast_done = true;
                                }
                                break;
                            case 4://skype
                                if (!tel_vast_done)
                                {
                                    vast = prow.telefoon_nr_3;
                                    tel_vast_done = true;
                                }
                                break;
                            case 5://bedrijfsnummer
                                if (!tel_vast_done)
                                {
                                    vast = prow.telefoon_nr_3;
                                    tel_vast_done = true;
                                }
                                break;
                        }
                    }
                   pta.verhuis_nummers(vast,mobiel,fax,prow.persoon_ID);
                }
              
            }
            MessageBox.Show("klaar");*/
          foreach (telefoon_nummers tlnr in telnrsgrid_niew.ItemsSource)
            {
                proj_nrs_ta.Update_tel_nrs(tlnr.Tel, tlnr.Fax, tlnr.ID);
               //bta.Update_telnrs(tlnr.Tel, tlnr.Mob, tlnr.Fax, tlnr.ID);
            }
        }
    }
    public class telefoon_nummers
    {
        public int ID { get; set; }
        public string Tel { get; set; }
        public string Mob { get; set; }
        public string Fax { get; set; }
       
    }
}
