using System.Windows;
using System.Data;
using System;
using System.Net;
using System.IO;
using System.Xml;
using System.Windows.Controls;
using System.Windows.Media;

namespace Adres_Validator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string[] blackList = { "--", ";--", ";", "/*", "*/", "@@", "@" };
        private bool isProject = false;
        private int bdr_rownr = 0;
        private int proj_rownr = 0;
        private int done_rownr = 0;
        database_tabellen.validator_set_1TableAdapters.bedrijfTableAdapter bta;
        database_tabellen.validator_set_1TableAdapters.projectTableAdapter pta;
        database_tabellen.validator_set_1.bedrijfDataTable bdt = new database_tabellen.validator_set_1.bedrijfDataTable();
        database_tabellen.validator_set_1.projectDataTable pdt = new database_tabellen.validator_set_1.projectDataTable();
        database_tabellen.validator_set_1TableAdapters.adressenTableAdapter ata = new database_tabellen.validator_set_1TableAdapters.adressenTableAdapter();
        database_tabellen.validator_set_1.adressenDataTable adt_prevnxt = new database_tabellen.validator_set_1.adressenDataTable();
        database_tabellen.validator_set_1.adressenRow aRow;
        database_tabellen.validator_set_1.bedrijfRow bRow;
        database_tabellen.validator_set_1.projectRow pRow;
        public MainWindow()
        {
            InitializeComponent();
            initialiseer();
            proj_rownr = Properties.Settings.Default.proj_rownum;
        }
        private void initialiseer()
        {
            project_btn.IsChecked = true;
            bdt.Constraints.Clear();  
            pdt.Constraints.Clear();
            bta = new database_tabellen.validator_set_1TableAdapters.bedrijfTableAdapter();
            pta = new database_tabellen.validator_set_1TableAdapters.projectTableAdapter();
            bta.Fill(bdt);
            pta.Fill(pdt);
           // adt_prevnxt.Clear();
            //ata.Fill(adt_prevnxt);
            //done_rownr = adt_prevnxt.Rows.Count;
            
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
       
        
        
        private void Next_btn_Click(object sender, RoutedEventArgs e)
        {

            CKB_zelfdeadres.IsChecked = false;
            TB_plaats_1.Clear();
            TB_plaats_1.Background = Brushes.White;
            TB_plaats_2.Clear();
            TB_plaats_2.Background = Brushes.White;
            TB_postcode_1.Clear();
            TB_postcode_1.Background = Brushes.White;
            TB_postcode_2.Clear();
            TB_postcode_2.Background = Brushes.White;
            TB_straat_1.Clear();
            TB_straat_1.Background = Brushes.White;
            TB_straat_2.Clear();
            TB_straat_2.Background = Brushes.White;
            bevestig_btn.IsEnabled = true;
            Next_btn.IsEnabled = false;
            if (isProject)
            {
                pRow = (database_tabellen.validator_set_1.projectRow)pdt.Rows[proj_rownr];
                proj_rownr += 1;
                recordnaam.Content ="project naam: "+ pRow.naam_project;
                nogtegaan.Content = proj_rownr+ " van "+ pdt.Rows.Count.ToString();
                projnum.Content = "project nummer: "+pRow.project_NR;
                inkoperpers.Content ="inkoper: "+ pRow.inkoper_voornaam + " " + pRow.inkoper_achternaam;
                offertepers.Content ="offerte: "+ pRow.offerte_voornaam +" "+ pRow.offerte_achternaam;
                adres1.Land = "Nederland";
                adres2.Land = "Nederland";
                if (!pRow.IsplaatsNull())
                {
                    TB_plaats_1.Text = pRow.plaats;
                }
                else
                {
                    TB_plaats_1.Background = Brushes.Red;
                }
                if (!pRow.Isbouw_straatNull())
                {
                    TB_straat_1.Text = pRow.bouw_straat;
                }
                else
                {
                    TB_straat_1.Background = Brushes.Red;
                }
                if (!pRow.Isbouw_postcodeNull())
                {
                    TB_postcode_1.Text = pRow.bouw_postcode;
                }
                else
                {
                    TB_postcode_1.Background = Brushes.Red;
                }
                if (!pRow.Isfactuur_plaatsNull())
                {
                    TB_plaats_2.Text = pRow.factuur_plaats;
                }
                else
                {
                    TB_plaats_2.Background = Brushes.Red;
                }
                if (!pRow.Isfactuur_straatNull())
                {
                    TB_straat_2.Text = pRow.factuur_straat;
                }
                else
                {
                    TB_straat_2.Background = Brushes.Red;
                }
                if (!pRow.Isfactuur_postcodeNull())
                {
                    TB_postcode_2.Text = pRow.factuur_postcode;
                }
                else
                {
                    TB_postcode_2.Background = Brushes.Red;
                }
                adres1.load_data(TB_plaats_1.Text, TB_straat_1.Text, TB_postcode_1.Text);
                adres2.load_data(TB_plaats_2.Text, TB_straat_2.Text, TB_postcode_2.Text);
            }
            else
            {
                
                bRow = (database_tabellen.validator_set_1.bedrijfRow)bdt.Rows[bdr_rownr];
                recordnaam.Content = bRow.zoeknaam;
                bdr_rownr += 1;
                int cntr = 0;
                nogtegaan.Content = bdr_rownr + " van " + bdt.Rows.Count.ToString();
                if (!bRow.IsplaatsNull())
                {
                    cntr++;
                    TB_plaats_1.Text = bRow.plaats;
                }
                else
                {
                    TB_plaats_1.Background = Brushes.Red;
                }
                if (!bRow.IsstraatNull())
                {
                    cntr++;
                    TB_straat_1.Text = bRow.straat;
                }
                else
                {
                    TB_straat_1.Background = Brushes.Red;
                }
                if (!bRow.IspostcodeNull())
                {
                    cntr++;
                    TB_postcode_1.Text = bRow.postcode;
                }
                else
                {
                    TB_postcode_1.Background = Brushes.Red;
                }
                if (!bRow.IspostPLAATSNull())
                {
                    cntr++;
                    TB_plaats_2.Text = bRow.postPLAATS;
                }
                else
                {
                    TB_plaats_2.Background = Brushes.Red;
                }
                if (!bRow.IspostSTRAATNull())
                {
                    cntr++;
                    TB_straat_2.Text = bRow.postSTRAAT;
                }
                else
                {
                    TB_straat_2.Background = Brushes.Red;
                }
                if (!bRow.IspostPOSTCODENull())
                {
                    cntr++;
                    TB_postcode_2.Text = bRow.postPOSTCODE;
                }
                else
                {
                    TB_postcode_2.Background = Brushes.Red;
                }
                if (cntr == 6)
                {
                    if (bRow.plaats == bRow.postPLAATS && bRow.straat == bRow.postSTRAAT && bRow.postcode == bRow.postPOSTCODE)
                    {
                        CKB_zelfdeadres.IsChecked = true;
                    }
                    else
                    {
                        CKB_zelfdeadres.IsChecked = false;
                    }
                }
                adres1.load_data(TB_plaats_1.Text, TB_straat_1.Text, TB_postcode_1.Text);
                adres2.load_data(TB_plaats_2.Text, TB_straat_2.Text, TB_postcode_2.Text);
            }
        }
        private bool alles_ingevuld_Check()
        {
            string error = "";
            bool retval = true;
            if (adres1.Land == String.Empty)
            {
                retval = false;
                error += " land1,";
            }
            if (adres1.Plaats == String.Empty)
            {
                retval = false;
                error += " plaats1,";
            }
            if (adres1.Straat == String.Empty)
            {
                retval = false;
                error += " straat1,";
            }
            if (adres1.Postcode_cijfers < 1 || adres1.Postcode_letters == String.Empty)
            {
                retval = false;
                error += " postcode1,";
            }

            if (adres2.Land == String.Empty)
            {
                retval = false;
                error += " land2,";
            }
            if (adres2.Plaats == String.Empty)
            {
                retval = false;
                error += " plaats2,";
            }
            if (adres2.Straat == String.Empty)
            {
                retval = false;
                error += " straat2,";
            }
            if (adres2.Postcode_cijfers < 1 || adres2.Postcode_letters == String.Empty)
            {
                retval = false;
                error += " postcode2,";
            }
            if (!retval)
            {
                MessageBox.Show("Niets ingevuld bij:" + error);
            }
            return retval;
        }
        private void bevestig_btn_Click(object sender, RoutedEventArgs e)
        {
            
                
                    if (isProject)
                    {
                        Next_btn.IsEnabled = true;
                        int? max_id;
                        if ((int?)ata.adres_max_id() != null)
                        {
                            max_id = (int?)ata.adres_max_id();
                        }
                        else
                        {
                            max_id = 0;
                        }

                        max_id += 1;
                        int? postcode_cijfers = null;
                        int? huisnummer = null;
                        if (adres1.Postcode_cijfers.ToString() != String.Empty)
                        {
                             postcode_cijfers = adres1.Postcode_cijfers;
                        }
                        if (adres1.Huisnummer.ToString() != String.Empty)
                        {
                            huisnummer = int.Parse(adres1.Huisnummer);
                        }
                        bool? via_postcode = adres1.ViaPostcode;
                        var result = ata.adres_new(max_id, adres1.Land, adres1.Plaats, adres1.Straat, adres1.Straat2, postcode_cijfers, adres1.Postcode_letters, huisnummer.ToString(), adres1.Huisnummer_toevoeging, true, via_postcode);
                        
                        int? max_id2;
                        if ((int?)ata.adres_max_id() != null)
                        {
                            max_id2 = (int?)ata.adres_max_id();
                        }
                        else
                        {
                            max_id2 = 0;
                        }
                        max_id2 += 1;
                        int? postcode_cijfers2 = null;
                        int? huisnummer2 = null;
                        if (adres2.Plaats != String.Empty)
                        {
                            if (adres2.Postcode_cijfers.ToString() != String.Empty)
                            {
                                postcode_cijfers2 = adres2.Postcode_cijfers;
                            }
                            if (adres2.Huisnummer.ToString() != String.Empty)
                            {
                                huisnummer2 = int.Parse(adres2.Huisnummer);
                            }
                            var result2 = ata.adres_new(max_id2, adres2.Land, adres2.Plaats, adres2.Straat, adres2.Straat2, postcode_cijfers2, adres2.Postcode_letters, huisnummer2.ToString(), adres2.Huisnummer_toevoeging, true, adres2.ViaPostcode);
                            pta.Update_adres_ids(max_id, max_id2, pRow.project_ID);
                        }
                        else
                        {
                            pta.Update_adres_ids(max_id, null, pRow.project_ID);
                        }
                        if (adres1.Straat != string.Empty)
                        {
                            string straat_bezoek_oud = adres1.Straat + " (" + adres1.Plaats.ToString() + ")";
                            pta.Update_oud_bouw_adres(adres1.Plaats, straat_bezoek_oud, adres1.Postcode_cijfers.ToString() + adres1.Postcode_letters, pRow.project_ID);

                        }
                        if (adres2.Plaats != String.Empty)
                        {
                            string straat_bezoek_oud2 = adres2.Straat + " " + adres2.Huisnummer.ToString();
                            if (adres2.Huisnummer_toevoeging != string.Empty)
                            {
                                straat_bezoek_oud2 += adres2.Huisnummer_toevoeging;
                            }
                            pta.Update_factuur_adres_oud(adres2.Plaats, straat_bezoek_oud2, adres2.Postcode_cijfers.ToString() + adres2.Postcode_letters, pRow.project_ID);
                        }
                    }
                    else if (alles_ingevuld_Check())
                    {
                        Next_btn.IsEnabled = true;
                        int? max_id;
                        if ((int?)ata.adres_max_id() != null)
                        {
                            max_id = (int?)ata.adres_max_id();
                        }
                        else
                        {
                            max_id = 0;
                        }
                    
                        max_id += 1;
                        int? postcode_cijfers = null;
                        int? huisnummer = null;
                        if (adres1.Postcode_cijfers.ToString() != String.Empty)
                        {
                            postcode_cijfers = adres1.Postcode_cijfers;
                        }
                        if (adres1.Huisnummer.ToString() != String.Empty)
                        {
                            huisnummer = int.Parse(adres1.Huisnummer);
                        }
                        bool? via_postcode = adres1.ViaPostcode;
                        var result = ata.adres_new(max_id, adres1.Land, adres1.Plaats, adres1.Straat, adres1.Straat2, postcode_cijfers, adres1.Postcode_letters, huisnummer.ToString(), adres1.Huisnummer_toevoeging, true,via_postcode);
                        int? max_id2;
                        if ((int?)ata.adres_max_id() != null)
                        {
                            max_id2 = (int?)ata.adres_max_id();
                        }
                        else
                        {
                            max_id2 = 0;
                        }
                        if (!CKB_zelfdeadres.IsChecked.Value)
                        {
                            if ((int?)ata.adres_max_id() != null)
                            {
                                max_id2 = (int?)ata.adres_max_id();
                            }
                            else
                            {
                                max_id2 = 0;
                            }
                            max_id2 += 1;
                            int? postcode_cijfers2 = null;
                            int? huisnummer2 = null;
                            if (adres2.Postcode_cijfers.ToString() != String.Empty)
                            {
                                postcode_cijfers2 = adres2.Postcode_cijfers;
                            }
                            if (adres2.Huisnummer.ToString() != String.Empty)
                            {
                                huisnummer2 = int.Parse(adres2.Huisnummer);
                            }
                            var result2 = ata.adres_new(max_id2, adres2.Land, adres2.Plaats, adres2.Straat, adres2.Straat2, postcode_cijfers2, adres2.Postcode_letters, huisnummer2.ToString(), adres2.Huisnummer_toevoeging, true, adres2.ViaPostcode);
                            bta.save_adres_ids(max_id, max_id2, bRow.bedrijf_ID);
                        }
                        else
                        {
                            bta.save_adres_ids(max_id, max_id, bRow.bedrijf_ID);
                        }
                        string straat_bezoek_oud = adres1.Straat + " " + adres1.Huisnummer.ToString();
                        if (adres1.Huisnummer_toevoeging != string.Empty)
                        {
                            straat_bezoek_oud += adres1.Huisnummer_toevoeging;
                        }
                        string straat_bezoek_oud2 = adres2.Straat + " " + adres2.Huisnummer.ToString();
                        if (adres2.Huisnummer_toevoeging != string.Empty)
                        {
                            straat_bezoek_oud2 += adres2.Huisnummer_toevoeging;
                        }
                        bta.Update_oud_adres(straat_bezoek_oud, adres1.Postcode_cijfers.ToString() + adres1.Postcode_letters, adres1.Plaats, straat_bezoek_oud2, adres2.Postcode_cijfers.ToString() + adres2.Postcode_letters, adres2.Plaats, bRow.bedrijf_ID);
                }



            
            bevestig_btn.IsEnabled = false;
          //  adt_prevnxt.Clear();
            //ata.Fill(adt_prevnxt);
            //done_rownr = adt_prevnxt.Rows.Count;
            
        }

        private void bdrproj_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            switch (rb.Name)
            {
                case"bedrijf_btn":
                    isProject = false;
                    CKB_zelfdeadres.Visibility = Visibility.Visible;
                    adres1.initialiseer(true, false);
                    adres2.initialiseer(true, false);
                    break;
                case "project_btn":
                    isProject = true;
                    CKB_zelfdeadres.Visibility = Visibility.Collapsed;
                    adres1.initialiseer(true, true);
                    adres2.initialiseer(true, false);
                    break;
            }
            hide_show_2ehelft();
        }
        private void hide_show_2ehelft()
        {
            if (isProject)
            {
                adres2.Visibility = Visibility.Collapsed;
                check_info.Visibility = Visibility.Collapsed;
                CKB_zelfdeadres.Visibility = Visibility.Collapsed;
                post_adres_lbl.Visibility = Visibility.Collapsed;
                adres2_oud_r1.Visibility = Visibility.Collapsed;
                adres2_oud_r2.Visibility = Visibility.Collapsed;
                adres2_oud_r3.Visibility = Visibility.Collapsed;
            }
            else
            {
                adres2.Visibility = Visibility.Visible;
                check_info.Visibility = Visibility.Visible;
                CKB_zelfdeadres.Visibility = Visibility.Visible;
                post_adres_lbl.Visibility = Visibility.Visible;
                adres2_oud_r1.Visibility = Visibility.Visible;
                adres2_oud_r2.Visibility = Visibility.Visible;
                adres2_oud_r3.Visibility = Visibility.Visible;
            }
        }
        private void CKB_zelfdeadres_Checked(object sender, RoutedEventArgs e)
        {
            adres2.Wijzigstand = false;
            TB_plaats_2.Text = TB_plaats_1.Text;
            TB_straat_2.Text = TB_straat_1.Text;
            TB_postcode_2.Text = TB_postcode_1.Text;
            TB_plaats_2.IsReadOnly = true;
            TB_straat_2.IsReadOnly = true;
            TB_postcode_2.IsReadOnly = true;
            adres2.Land = adres1.Land;
            adres2.Plaats =adres1.Plaats;
            adres2.Straat = adres1.Straat;
            adres2.Huisnummer = adres1.Huisnummer;
            adres2.Huisnummer_toevoeging = adres1.Huisnummer_toevoeging;
            adres2.Postcode_cijfers = adres1.Postcode_cijfers;
            adres2.Postcode_letters = adres1.Postcode_letters;
            
        }

        private void CKB_zelfdeadres_Unchecked(object sender, RoutedEventArgs e)
        {
            TB_plaats_2.IsReadOnly = false;
            TB_straat_2.IsReadOnly = false;
            TB_postcode_2.IsReadOnly = false;
            adres2.Wijzigstand = true;
        }

        private void prev_btn_Click(object sender, RoutedEventArgs e)
        {
            vlg_btn.IsEnabled = true;
            done_rownr--;
            aRow = adt_prevnxt.Rows[done_rownr] as database_tabellen.validator_set_1.adressenRow;
            adres2.Land = aRow.land;
            adres2.Plaats = aRow.plaats;
            adres2.Straat = aRow.straat_1;
            adres2.Postcode_cijfers = aRow.postcode_cijfers;
            adres2.Postcode_letters = aRow.postcode_letters;
            adres2.Huisnummer = aRow.huis_postbus_nummer.ToString();
            adres2.Huisnummer_toevoeging = aRow.huisnummer_toevoeging;
            adres2.Straat2 = aRow.straat_2;
            done_rownr--;
            aRow = adt_prevnxt.Rows[done_rownr] as database_tabellen.validator_set_1.adressenRow;
            adres1.Land = aRow.land;
            adres1.Plaats = aRow.plaats;
            adres1.Straat = aRow.straat_1;
            adres1.Postcode_cijfers = aRow.postcode_cijfers;
            adres1.Postcode_letters = aRow.postcode_letters;
            adres1.Huisnummer = aRow.huis_postbus_nummer.ToString();
            adres1.Huisnummer_toevoeging = aRow.huisnummer_toevoeging;
            adres1.Straat2 = aRow.straat_2;
            if (done_rownr == 0)
            {
                prev_btn.IsEnabled = false;
            }

        }

        private void vlg_btn_Click(object sender, RoutedEventArgs e)
        {
            prev_btn.IsEnabled = true;
            done_rownr++;
            aRow = adt_prevnxt.Rows[done_rownr] as database_tabellen.validator_set_1.adressenRow;
            adres2.Land = aRow.land;
            adres2.Plaats = aRow.plaats;
            adres2.Straat = aRow.straat_1;
            adres2.Postcode_cijfers = aRow.postcode_cijfers;
            adres2.Postcode_letters = aRow.postcode_letters;
            adres2.Huisnummer = aRow.huis_postbus_nummer.ToString();
            adres2.Huisnummer_toevoeging = aRow.huisnummer_toevoeging;
            adres2.Straat2 = aRow.straat_2;
            done_rownr++;
            aRow = adt_prevnxt.Rows[done_rownr] as database_tabellen.validator_set_1.adressenRow;
            adres1.Land = aRow.land;
            adres1.Plaats = aRow.plaats;
            adres1.Straat = aRow.straat_1;
            adres1.Postcode_cijfers = aRow.postcode_cijfers;
            adres1.Postcode_letters = aRow.postcode_letters;
            adres1.Huisnummer = aRow.huis_postbus_nummer.ToString();
            adres1.Huisnummer_toevoeging = aRow.huisnummer_toevoeging;
            adres1.Straat2 = aRow.straat_2;
            if (done_rownr == adt_prevnxt.Rows.Count - 1)
            {
                vlg_btn.IsEnabled = false;
            }
        }

        private void skip_btn_Click(object sender, RoutedEventArgs e)
        {
            CKB_zelfdeadres.IsChecked = false;
            TB_plaats_1.Clear();
            TB_plaats_1.Background = Brushes.White;
            TB_plaats_2.Clear();
            TB_plaats_2.Background = Brushes.White;
            TB_postcode_1.Clear();
            TB_postcode_1.Background = Brushes.White;
            TB_postcode_2.Clear();
            TB_postcode_2.Background = Brushes.White;
            TB_straat_1.Clear();
            TB_straat_1.Background = Brushes.White;
            TB_straat_2.Clear();
            TB_straat_2.Background = Brushes.White;
            bevestig_btn.IsEnabled = true;
            Next_btn.IsEnabled = false;
            if (isProject)
            {
                pRow = (database_tabellen.validator_set_1.projectRow)pdt.Rows[proj_rownr];
                proj_rownr += 1;
                recordnaam.Content = pRow.naam_project;
                nogtegaan.Content = proj_rownr + " van " + pdt.Rows.Count.ToString();
                projnum.Content = pRow.project_NR;
                inkoperpers.Content = "inkoper: " + pRow.inkoper_voornaam + " " + pRow.inkoper_achternaam;
                offertepers.Content = "offerte: " + pRow.offerte_voornaam + " " + pRow.offerte_achternaam;
                
               
                if (!pRow.IsplaatsNull())
                {
                    TB_plaats_1.Text = pRow.plaats;
                }
                else
                {
                    TB_plaats_1.Background = Brushes.Red;
                }
                if (!pRow.Isbouw_straatNull())
                {
                    TB_straat_1.Text = pRow.bouw_straat;
                }
                else
                {
                    TB_straat_1.Background = Brushes.Red;
                }
                if (!pRow.Isbouw_postcodeNull())
                {
                    TB_postcode_1.Text = pRow.bouw_postcode;
                }
                else
                {
                    TB_postcode_1.Background = Brushes.Red;
                }
                if (!pRow.Isfactuur_plaatsNull())
                {
                    TB_plaats_2.Text = pRow.factuur_plaats;
                }
                else
                {
                    TB_plaats_2.Background = Brushes.Red;
                }
                if (!pRow.Isfactuur_straatNull())
                {
                    TB_straat_2.Text = pRow.factuur_straat;
                }
                else
                {
                    TB_straat_2.Background = Brushes.Red;
                }
                if (!pRow.Isfactuur_postcodeNull())
                {
                    TB_postcode_2.Text = pRow.factuur_postcode;
                }
                else
                {
                    TB_postcode_2.Background = Brushes.Red;
                }
                adres1.load_data(TB_plaats_1.Text, TB_straat_1.Text, TB_postcode_1.Text);
                adres2.load_data(TB_plaats_2.Text, TB_straat_2.Text, TB_postcode_2.Text);
            }
            else
            {

                bRow = (database_tabellen.validator_set_1.bedrijfRow)bdt.Rows[bdr_rownr];
                recordnaam.Content = bRow.zoeknaam;
                bdr_rownr += 1;
                int cntr = 0;
                nogtegaan.Content = bdr_rownr + " van " + bdt.Rows.Count.ToString();
                if (!bRow.IsplaatsNull())
                {
                    cntr++;
                    TB_plaats_1.Text = bRow.plaats;
                }
                else
                {
                    TB_plaats_1.Background = Brushes.Red;
                }
                if (!bRow.IsstraatNull())
                {
                    cntr++;
                    TB_straat_1.Text = bRow.straat;
                }
                else
                {
                    TB_straat_1.Background = Brushes.Red;
                }
                if (!bRow.IspostcodeNull())
                {
                    cntr++;
                    TB_postcode_1.Text = bRow.postcode;
                }
                else
                {
                    TB_postcode_1.Background = Brushes.Red;
                }
                if (!bRow.IspostPLAATSNull())
                {
                    cntr++;
                    TB_plaats_2.Text = bRow.postPLAATS;
                }
                else
                {
                    TB_plaats_2.Background = Brushes.Red;
                }
                if (!bRow.IspostSTRAATNull())
                {
                    cntr++;
                    TB_straat_2.Text = bRow.postSTRAAT;
                }
                else
                {
                    TB_straat_2.Background = Brushes.Red;
                }
                if (!bRow.IspostPOSTCODENull())
                {
                    cntr++;
                    TB_postcode_2.Text = bRow.postPOSTCODE;
                }
                else
                {
                    TB_postcode_2.Background = Brushes.Red;
                }
                if (cntr == 6)
                {
                    if (bRow.plaats == bRow.postPLAATS && bRow.straat == bRow.postSTRAAT && bRow.postcode == bRow.postPOSTCODE)
                    {
                        CKB_zelfdeadres.IsChecked = true;
                    }
                    else
                    {
                        CKB_zelfdeadres.IsChecked = false;
                    }
                }
                adres1.load_data(TB_plaats_1.Text, TB_straat_1.Text, TB_postcode_1.Text);
                adres2.load_data(TB_plaats_2.Text, TB_straat_2.Text, TB_postcode_2.Text);
            }
        }

        private void telnrbtn_Click(object sender, RoutedEventArgs e)
        {
            telnrs_window tw = new telnrs_window();
            tw.Show();
            this.Hide();
        }

        private void funtbtn_Click(object sender, RoutedEventArgs e)
        {
            afgeleide_functies af = new afgeleide_functies();
            af.Show();
            this.Hide();
        }

        private void start_bij_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.proj_rownum = proj_rownr;
            Properties.Settings.Default.Save();
        }
    }
}
