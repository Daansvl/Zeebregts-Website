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
using System.Data;


namespace Adres_Validator
{
    /// <summary>
    /// Interaction logic for afgeleide_functies.xaml
    /// </summary>
    public partial class afgeleide_functies : Window
    {
        private delegate void UpdateProgressBarDelegate(System.Windows.DependencyProperty dp, Object value);
        UpdateProgressBarDelegate updatePbDelegate;
        private delegate void UpdateLayoutDelegate(System.Windows.DependencyProperty dp, Object value);
        UpdateLayoutDelegate updateLDelegate;
        database_tabellen.validator_set_1TableAdapters.bedrijf_voor_functTableAdapter bvfta = new database_tabellen.validator_set_1TableAdapters.bedrijf_voor_functTableAdapter();
        database_tabellen.validator_set_1.bedrijf_voor_functDataTable bvfdt = new database_tabellen.validator_set_1.bedrijf_voor_functDataTable();
        database_tabellen.validator_set_1TableAdapters.persoon_voor_functTableAdapter pvfta = new database_tabellen.validator_set_1TableAdapters.persoon_voor_functTableAdapter();
        database_tabellen.validator_set_1.persoon_voor_functDataTable pvfdt = new database_tabellen.validator_set_1.persoon_voor_functDataTable();
        
        
        public afgeleide_functies()
        {
            InitializeComponent();
           // bvfta.Connection.ConnectionString = zeebregtsCs.Global.ConnectionString_fileserver;
            updatePbDelegate = new UpdateProgressBarDelegate(pro_bar.SetValue);
            updateLDelegate = new UpdateLayoutDelegate(dg_nieuw.SetValue);
        }

        
        private void btn_inladen_Click(object sender, RoutedEventArgs e)
        {
            if (rb_bedrijven.IsChecked == true)
            {
                bvfta.Fill(bvfdt);
                dg_oud.ItemsSource = bvfdt;
                dg_oud.Columns[0].Visibility = Visibility.Collapsed;
            }
            else if (rb_contacten.IsChecked == true)
            {
                pvfta.Fill(pvfdt);
                dg_oud.ItemsSource = pvfdt;
                dg_oud.Columns[0].Visibility = Visibility.Collapsed;
            }
            else
            {
                lbl_info.Content = "kies type";
            }
        }

        private void btn_starten_Click(object sender, RoutedEventArgs e)
        {
            double waarde = 0;
            pro_bar.Minimum = 0;
            if (rb_bedrijven.IsChecked == true)
            {
                pro_bar.Maximum = bvfdt.Count;
                pro_bar.Value = 0;
                List<zeebregtsCs.RefItem> LRI = new List<zeebregtsCs.RefItem>();
                foreach (database_tabellen.validator_set_1.bedrijf_voor_functRow brow in bvfdt)
                {
                    waarde++;
                    Dispatcher.Invoke(updatePbDelegate, System.Windows.Threading.DispatcherPriority.Background, new object[] { ProgressBar.ValueProperty, waarde });
                    zeebregtsCs.RefItem RI = convert_bdr(brow);
                    if (RI != null)
                    {
                       LRI.Add(RI);
                    }
                    
                }
                Dispatcher.Invoke(updateLDelegate, System.Windows.Threading.DispatcherPriority.Background, new object[] { ListBox.ItemsSourceProperty, LRI });
                
            }
            else if (rb_contacten.IsChecked == true)
            {
                pro_bar.Maximum = pvfdt.Count;
                pro_bar.Value = 0;
                List<zeebregtsCs.RefItem> LRI = new List<zeebregtsCs.RefItem>();
                foreach (database_tabellen.validator_set_1.persoon_voor_functRow prow in pvfdt)
                {
                    waarde++;
                    Dispatcher.Invoke(updatePbDelegate, System.Windows.Threading.DispatcherPriority.Background, new object[] { ProgressBar.ValueProperty, waarde });
                    zeebregtsCs.RefItem RI = convert_pers(prow);
                    if (RI != null)
                    {
                        LRI.Add(RI);
                    }
                    
                }
                Dispatcher.Invoke(updateLDelegate, System.Windows.Threading.DispatcherPriority.Background, new object[] { ListBox.ItemsSourceProperty, LRI });
            }
        }

        private zeebregtsCs.RefItem convert_bdr(database_tabellen.validator_set_1.bedrijf_voor_functRow brow)
        {
            List<zeebregtsCs.RefItem> tmplst = new List<zeebregtsCs.RefItem>();
            ///////////////////
            int bid = brow.bedrijf_nr;
            int result = 0;
            string column = "";
            string table = "";
           // zeebregtsCs.bedrijfformdatasetTableAdapters.QueriesTableAdapter qtadapt = new zeebregtsCs.bedrijfformdatasetTableAdapters.QueriesTableAdapter();
            zeebregtsCs.bedrijfformdatasetTableAdapters.bedrijfTableAdapter adapter = new zeebregtsCs.bedrijfformdatasetTableAdapters.bedrijfTableAdapter();
            
            zeebregtsCs.bedrijfformdatasetTableAdapters.bedrijf_nr_locatiesTableAdapter locaties_adapt = new zeebregtsCs.bedrijfformdatasetTableAdapters.bedrijf_nr_locatiesTableAdapter();
            zeebregtsCs.bedrijfformdataset.bedrijf_nr_locatiesDataTable locaties_table = locaties_adapt.GetData();
            zeebregtsCs.bedrijfformdatasetTableAdapters.verwijder_get_varbedrijf_functnaamTableAdapter vrgfadapt = new zeebregtsCs.bedrijfformdatasetTableAdapters.verwijder_get_varbedrijf_functnaamTableAdapter();
            zeebregtsCs.bedrijfformdataset.verwijder_get_varbedrijf_functnaamDataTable varfuntdt = vrgfadapt.GetData(bid);
            foreach (zeebregtsCs.bedrijfformdataset.bedrijf_nr_locatiesRow row in locaties_table.Rows)
            {
                if (row.tabel == "bedrijvensetvariabel")
                {
                    foreach (zeebregtsCs.bedrijfformdataset.verwijder_get_varbedrijf_functnaamRow dr in varfuntdt.Rows)
                    {
                        if (dr.functie_ID != 11 && dr.functie_ID != 1)
                        {
                            tmplst.Add(new zeebregtsCs.RefItem { funct_naam = dr.naam+brow.bedrijf_nr.ToString(), funct_count = dr.aantal, functie_id = dr.functie_ID, uit_var = true });
                        }
                    }
                }
                else
                {
                    column = row.column.ToString();
                    table = row.tabel.ToString();
                    
                    result = (int)adapter.Check_del_bdr(bid, column, table);
                    if (result > 0)
                    {
                        bool voegtoe = true;
                        foreach (zeebregtsCs.RefItem RI in tmplst)
                        {
                            if (RI.funct_naam == row.weergavenaam)
                            {
                                RI.funct_count += result;
                                RI.functie_id = row.regel_nr;
                                RI.uit_var = false;
                                voegtoe = false;
                            }
                        }
                        if (voegtoe)
                        {
                            if (row.regel_nr != 7 && row.regel_nr != 6 && row.regel_nr != 3 && row.regel_nr != 2)
                            {
                                tmplst.Add(new zeebregtsCs.RefItem
                                {
                                    funct_naam = row.weergavenaam + brow.bedrijf_nr.ToString(),
                                    funct_count = result,
                                    functie_id = row.regel_nr,
                                    uit_var = false
                                });
                            }
                        }
                    }
                }
            }
            //qtadapt.Dispose();
            locaties_adapt.Dispose();
            locaties_table.Dispose();
            //////////////////
            tmplst = tmplst.OrderByDescending(RefItem => RefItem.funct_count).ToList<zeebregtsCs.RefItem>();
            //////////////

            if (tmplst.Count > 0)
            { return tmplst[0]; }
            else
            { return null; }
            //database_tabellen.validator_set_1TableAdapters.QueriesTableAdapter qta = new database_tabellen.validator_set_1TableAdapters.QueriesTableAdapter();
            //qta.new_agfl_funct(brow.bedrijf_nr, 2, tmplst[0].functie_id, tmplst[0].uit_var);
           

        }
        private zeebregtsCs.RefItem convert_pers(database_tabellen.validator_set_1.persoon_voor_functRow prow)
        {
            List<zeebregtsCs.RefItem> tmplst = new List<zeebregtsCs.RefItem>();
            ////////////////////
            int pid = prow.persoon_nr;
            int result = 0;
            string column = "";
            string table = "";
            zeebregtsCs.persoonsformdatasetTableAdapters.persoonTableAdapter adapter = new zeebregtsCs.persoonsformdatasetTableAdapters.persoonTableAdapter();
            zeebregtsCs.persoonsformdatasetTableAdapters.persoon_nr_locatiesTableAdapter locaties_adapt = new zeebregtsCs.persoonsformdatasetTableAdapters.persoon_nr_locatiesTableAdapter();
            zeebregtsCs.persoonsformdataset.persoon_nr_locatiesDataTable locaties_table = locaties_adapt.GetData();
            zeebregtsCs.persoonsformdatasetTableAdapters.var_pers_funct_countTableAdapter varcnttadapt = new zeebregtsCs.persoonsformdatasetTableAdapters.var_pers_funct_countTableAdapter();
            zeebregtsCs.persoonsformdataset.var_pers_funct_countDataTable varcntdt = varcnttadapt.GetData(pid);
            foreach (zeebregtsCs.persoonsformdataset.persoon_nr_locatiesRow row in locaties_table.Rows)
            {
                if (row.column == "persoon_nr")
                {
                    foreach (zeebregtsCs.persoonsformdataset.var_pers_funct_countRow dr in varcntdt.Rows)
                    {
                        tmplst.Add(new zeebregtsCs.RefItem { funct_count = dr.aantal, funct_naam = dr.naam+prow.persoon_nr.ToString(), functie_id = dr.functie_id, uit_var = true });
                    }
                }
                else
                {
                    column = row.column.ToString();
                    table = row.tabel.ToString();
                    result = (int)adapter.Check_del_pers(pid, column, table);

                    if (result > 0)
                    {
                        bool voegtoe = true;
                        foreach (zeebregtsCs.RefItem RI in tmplst)
                        {
                            if (RI.funct_naam == row.weergavenaam)
                            {
                                RI.funct_count += result;
                                RI.uit_var = false;
                                RI.functie_id = row.regel_nr;
                                voegtoe = false;
                            }
                        }
                        if (voegtoe)
                        {
                            tmplst.Add(new zeebregtsCs.RefItem
                            {
                                funct_naam = row.weergavenaam+prow.persoon_nr,
                                funct_count = result,
                                functie_id = row.regel_nr,
                                uit_var = false
                            });
                        }
                    }
                }
            }
            adapter.Dispose();
            locaties_adapt.Dispose();
            locaties_table.Dispose();
            ///////////////////
            tmplst = tmplst.OrderByDescending(RefItem => RefItem.funct_count).ToList<zeebregtsCs.RefItem>();
            if (tmplst.Count > 0)
            {
                return tmplst[0];
            }
            else
            {
                return null;
            }
        }

        private void btn_opslaan_Click(object sender, RoutedEventArgs e)
        {
            double waarde = 0;
            pro_bar.Minimum = 0;
            if (rb_bedrijven.IsChecked == true)
            {
                pro_bar.Maximum = bvfdt.Count;
                pro_bar.Value = 0;
                List<zeebregtsCs.RefItem> LRI = new List<zeebregtsCs.RefItem>();
                foreach (database_tabellen.validator_set_1.bedrijf_voor_functRow brow in bvfdt)
                {
                    waarde++;
                    Dispatcher.Invoke(updatePbDelegate, System.Windows.Threading.DispatcherPriority.Background, new object[] { ProgressBar.ValueProperty, waarde });
                    zeebregtsCs.RefItem RI = convert_bdr_nsave(brow);
                    if (RI != null)
                    {
                        LRI.Add(RI);
                        
                    }
                    
                }
                Dispatcher.Invoke(updateLDelegate, System.Windows.Threading.DispatcherPriority.Background, new object[] { ListBox.ItemsSourceProperty, LRI });
                
            }
            else if (rb_contacten.IsChecked == true)
            {
                pro_bar.Maximum = pvfdt.Count;
                pro_bar.Value = 0;
                List<zeebregtsCs.RefItem> LRI = new List<zeebregtsCs.RefItem>();
                foreach (database_tabellen.validator_set_1.persoon_voor_functRow prow in pvfdt)
                {
                    waarde++;
                    Dispatcher.Invoke(updatePbDelegate, System.Windows.Threading.DispatcherPriority.Background, new object[] { ProgressBar.ValueProperty, waarde });
                    zeebregtsCs.RefItem RI = convert_pers_nsave(prow);
                    if (RI != null)
                    {
                        LRI.Add(RI);
                    }
                    
                }
                Dispatcher.Invoke(updateLDelegate, System.Windows.Threading.DispatcherPriority.Background, new object[] { ListBox.ItemsSourceProperty, LRI });
                
            }
        }
        private zeebregtsCs.RefItem convert_bdr_nsave(database_tabellen.validator_set_1.bedrijf_voor_functRow brow)
        {
            List<zeebregtsCs.RefItem> tmplst = new List<zeebregtsCs.RefItem>();
            ///////////////////
            int bid = brow.bedrijf_nr;
            int result = 0;
            string column = "";
            string table = "";
            zeebregtsCs.bedrijfformdatasetTableAdapters.bedrijfTableAdapter qtadapt = new zeebregtsCs.bedrijfformdatasetTableAdapters.bedrijfTableAdapter();
            zeebregtsCs.bedrijfformdatasetTableAdapters.bedrijf_nr_locatiesTableAdapter locaties_adapt = new zeebregtsCs.bedrijfformdatasetTableAdapters.bedrijf_nr_locatiesTableAdapter();
            zeebregtsCs.bedrijfformdataset.bedrijf_nr_locatiesDataTable locaties_table = locaties_adapt.GetData();
            zeebregtsCs.bedrijfformdatasetTableAdapters.verwijder_get_varbedrijf_functnaamTableAdapter vrgfadapt = new zeebregtsCs.bedrijfformdatasetTableAdapters.verwijder_get_varbedrijf_functnaamTableAdapter();
            zeebregtsCs.bedrijfformdataset.verwijder_get_varbedrijf_functnaamDataTable varfuntdt = vrgfadapt.GetData(bid);
            foreach (zeebregtsCs.bedrijfformdataset.bedrijf_nr_locatiesRow row in locaties_table.Rows)
            {
                if (row.tabel == "bedrijvensetvariabel")
                {
                    foreach (zeebregtsCs.bedrijfformdataset.verwijder_get_varbedrijf_functnaamRow dr in varfuntdt.Rows)
                    {
                        if (dr.functie_ID != 11 && dr.functie_ID != 1)
                        {
                            tmplst.Add(new zeebregtsCs.RefItem { funct_naam = dr.naam, funct_count = dr.aantal, functie_id = dr.functie_ID, uit_var = true });
                        }
                    }
                }
                else
                {
                    column = row.column.ToString();
                    table = row.tabel.ToString();
                    result = (int)qtadapt.Check_del_bdr(bid, column, table);
                    if (result > 0)
                    {
                        bool voegtoe = true;
                        foreach (zeebregtsCs.RefItem RI in tmplst)
                        {
                            if (RI.funct_naam == row.weergavenaam)
                            {
                                RI.funct_count += result;
                                RI.functie_id = row.regel_nr;
                                RI.uit_var = false;
                                voegtoe = false;
                            }
                        }
                        if (voegtoe)
                        {
                            if (row.regel_nr != 7 && row.regel_nr != 6 && row.regel_nr != 3 && row.regel_nr != 2)
                            {
                                tmplst.Add(new zeebregtsCs.RefItem
                                {
                                    funct_naam = row.weergavenaam,
                                    funct_count = result,
                                    functie_id = row.regel_nr,
                                    uit_var = false
                                });
                            }
                        }
                    }
                }
            }
            qtadapt.Dispose();
            locaties_adapt.Dispose();
            locaties_table.Dispose();
            //////////////////
            tmplst = tmplst.OrderByDescending(RefItem => RefItem.funct_count).ToList<zeebregtsCs.RefItem>();
            //////////////
            database_tabellen.validator_set_1TableAdapters.QueriesTableAdapter qta = new database_tabellen.validator_set_1TableAdapters.QueriesTableAdapter();
            
            if (tmplst.Count > 0)
            {
                qta.new_agfl_funct(brow.bedrijf_nr, 2, tmplst[0].functie_id, tmplst[0].uit_var);
                return tmplst[0]; }
            else
            { return null; }
            

        }
        private zeebregtsCs.RefItem convert_pers_nsave(database_tabellen.validator_set_1.persoon_voor_functRow prow)
        {
            List<zeebregtsCs.RefItem> tmplst = new List<zeebregtsCs.RefItem>();
            ////////////////////
            int pid = prow.persoon_nr;
            int result = 0;
            string column = "";
            string table = "";
            zeebregtsCs.persoonsformdatasetTableAdapters.persoonTableAdapter qtadapt = new zeebregtsCs.persoonsformdatasetTableAdapters.persoonTableAdapter();
            zeebregtsCs.persoonsformdatasetTableAdapters.persoon_nr_locatiesTableAdapter locaties_adapt = new zeebregtsCs.persoonsformdatasetTableAdapters.persoon_nr_locatiesTableAdapter();
            zeebregtsCs.persoonsformdataset.persoon_nr_locatiesDataTable locaties_table = locaties_adapt.GetData();
            zeebregtsCs.persoonsformdatasetTableAdapters.var_pers_funct_countTableAdapter varcnttadapt = new zeebregtsCs.persoonsformdatasetTableAdapters.var_pers_funct_countTableAdapter();
            zeebregtsCs.persoonsformdataset.var_pers_funct_countDataTable varcntdt = varcnttadapt.GetData(pid);
            foreach (zeebregtsCs.persoonsformdataset.persoon_nr_locatiesRow row in locaties_table.Rows)
            {
                if (row.column == "persoon_nr")
                {
                    foreach (zeebregtsCs.persoonsformdataset.var_pers_funct_countRow dr in varcntdt.Rows)
                    {
                        tmplst.Add(new zeebregtsCs.RefItem { funct_count = dr.aantal, funct_naam = dr.naam, functie_id = dr.functie_id, uit_var = true });
                    }
                }
                else
                {
                    column = row.column.ToString();
                    table = row.tabel.ToString();
                    result = (int)qtadapt.Check_del_pers(pid, column, table);

                    if (result > 0)
                    {
                        bool voegtoe = true;
                        foreach (zeebregtsCs.RefItem RI in tmplst)
                        {
                            if (RI.funct_naam == row.weergavenaam)
                            {
                                RI.funct_count += result;
                                RI.uit_var = false;
                                RI.functie_id = row.regel_nr;
                                voegtoe = false;
                            }
                        }
                        if (voegtoe)
                        {
                            tmplst.Add(new zeebregtsCs.RefItem
                            {
                                funct_naam = row.weergavenaam,
                                funct_count = result,
                                functie_id = row.regel_nr,
                                uit_var = false
                            });
                        }
                    }
                }
            }
            qtadapt.Dispose();
            locaties_adapt.Dispose();
            locaties_table.Dispose();
            ///////////////////
            tmplst = tmplst.OrderByDescending(RefItem => RefItem.funct_count).ToList<zeebregtsCs.RefItem>();
            database_tabellen.validator_set_1TableAdapters.QueriesTableAdapter qta = new database_tabellen.validator_set_1TableAdapters.QueriesTableAdapter();
            
            if (tmplst.Count > 0)
            {
                qta.new_agfl_funct(prow.persoon_nr, 3, tmplst[0].functie_id, tmplst[0].uit_var);
                return tmplst[0];
            }
            else
            {
                return null;
            }
        }
    }
}
