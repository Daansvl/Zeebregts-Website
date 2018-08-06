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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;

namespace zeebregtsCs
{
    /// <summary>
    /// Interaction logic for verwijder_panel.xaml
    /// </summary>
    public partial class verwijder_panel : UserControl
    {
        bedrijfformdatasetTableAdapters.Get_afgl_functTableAdapter Get_afgl_ta = new bedrijfformdatasetTableAdapters.Get_afgl_functTableAdapter();
        bedrijfformdatasetTableAdapters.bedrijfTableAdapter adapter = new bedrijfformdatasetTableAdapters.bedrijfTableAdapter();
        public verwijder_panel()
        {
            adapter.Connection.ConnectionString = Global.ConnectionString_fileserver;
            Get_afgl_ta.Connection.ConnectionString = Global.ConnectionString_fileserver;
            InitializeComponent();
        }
        public void start_init(int tp, int id)
        {
            LoadFunctList._type = tp;
            LoadFunctList._record_id = id;
           catagory_list.ItemsSource =  LoadFunctList.load_list();
            
        }
        private void Calc_function(bool exist)
        {
            switch (LoadFunctList._type)
            {
                case 2:
                    Calc_bdr(!exist);
                    break;
                case 3:
                    Calc_pers(!exist);
                    break;
            }
        }
        private void Calc_bdr(bool nieuw)
        {
            List<zeebregtsCs.RefItem> tmplst = new List<zeebregtsCs.RefItem>();
            ///////////////////
            int bid = LoadFunctList._record_id;
            int result = 0;
            string column = "";
            string table = "";
           // zeebregtsCs.bedrijfformdatasetTableAdapters.QueriesTableAdapter qtadapt = new zeebregtsCs.bedrijfformdatasetTableAdapters.QueriesTableAdapter();
            zeebregtsCs.bedrijfformdatasetTableAdapters.bedrijf_nr_locatiesTableAdapter locaties_adapt = new zeebregtsCs.bedrijfformdatasetTableAdapters.bedrijf_nr_locatiesTableAdapter();
            locaties_adapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
            zeebregtsCs.bedrijfformdataset.bedrijf_nr_locatiesDataTable locaties_table = locaties_adapt.GetData();
            zeebregtsCs.bedrijfformdatasetTableAdapters.verwijder_get_varbedrijf_functnaamTableAdapter vrgfadapt = new zeebregtsCs.bedrijfformdatasetTableAdapters.verwijder_get_varbedrijf_functnaamTableAdapter();
            vrgfadapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
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
            locaties_adapt.Dispose();
            locaties_table.Dispose();
            //////////////////
            tmplst = tmplst.OrderByDescending(RefItem => RefItem.funct_count).ToList<zeebregtsCs.RefItem>();
            //////////////
            if (tmplst.Count > 0)
            {
                if (nieuw)
                {
                    Get_afgl_ta.new_agfl_funct(LoadFunctList._record_id, 2, tmplst[0].functie_id, tmplst[0].uit_var);
                }
                else
                {
                    Get_afgl_ta.update_agfl_funct(LoadFunctList._record_id, 2, tmplst[0].functie_id, tmplst[0].uit_var);
                }
            }
            
        }
        private void Calc_pers(bool nieuw)
        {
            List<zeebregtsCs.RefItem> tmplst = new List<zeebregtsCs.RefItem>();
            ////////////////////
            int pid = LoadFunctList._record_id;
            int result = 0;
            string column = "";
            string table = "";
            //zeebregtsCs.persoonsformdatasetTableAdapters.QueriesTableAdapter qtadapt = new zeebregtsCs.persoonsformdatasetTableAdapters.QueriesTableAdapter();
            zeebregtsCs.persoonsformdatasetTableAdapters.persoon_nr_locatiesTableAdapter locaties_adapt = new zeebregtsCs.persoonsformdatasetTableAdapters.persoon_nr_locatiesTableAdapter();
            locaties_adapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
            zeebregtsCs.persoonsformdataset.persoon_nr_locatiesDataTable locaties_table = locaties_adapt.GetData();
            zeebregtsCs.persoonsformdatasetTableAdapters.var_pers_funct_countTableAdapter varcnttadapt = new zeebregtsCs.persoonsformdatasetTableAdapters.var_pers_funct_countTableAdapter();
            varcnttadapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
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
                                funct_naam = row.weergavenaam,
                                funct_count = result,
                                functie_id = row.regel_nr,
                                uit_var = false
                            });
                        }
                    }
                }
            }
            locaties_adapt.Dispose();
            locaties_table.Dispose();
            ///////////////////
            tmplst = tmplst.OrderByDescending(RefItem => RefItem.funct_count).ToList<zeebregtsCs.RefItem>();
            if (tmplst.Count > 0)
            {
                if (nieuw)
                {
                    Get_afgl_ta.new_agfl_funct(LoadFunctList._record_id, 3, tmplst[0].functie_id, tmplst[0].uit_var);
                }
                else
                {
                    Get_afgl_ta.update_agfl_funct(LoadFunctList._record_id, 3, tmplst[0].functie_id, tmplst[0].uit_var);
                }
                
            }
        }
        public void Save_function(string vrij)
        {
            bool exists = false;
            int rslt = (int)Get_afgl_ta.Check_afgl_exists(LoadFunctList._record_id, LoadFunctList._type);
            if (rslt > 0)
            {
                exists = true;
            }
           if (exists)
                {
                    Get_afgl_ta.update_afgl_vrij(LoadFunctList._record_id, LoadFunctList._type, vrij);
                }
                else
                {
                    Get_afgl_ta.new_afgl_vrij(LoadFunctList._record_id, LoadFunctList._type, vrij);
                }
            
           rslt = (int)Get_afgl_ta.Check_afgl_exists(LoadFunctList._record_id, LoadFunctList._type);
           if (rslt > 0)
           {
               exists = true;
           }
            Calc_function(exists);
        }
        public String[] Get_function()
        {
            string omsch_vrij = String.Empty;
            string omsch_afgl = String.Empty;
            
            bedrijfformdataset.Get_afgl_functDataTable afgl_dt =  Get_afgl_ta.GetData(LoadFunctList._record_id, LoadFunctList._type, ref omsch_vrij, ref omsch_afgl);
            if (afgl_dt.Rows.Count > 0)
            {
                bedrijfformdataset.Get_afgl_functRow afgl_row = (bedrijfformdataset.Get_afgl_functRow)afgl_dt.Rows[0];

                if (!afgl_row.Isomschrijving_vrijNull())
                {
                    omsch_vrij = afgl_row.omschrijving_vrij;
                }
                if (!afgl_row.Isafgeleide_omschrijvingNull())
                {
                    omsch_afgl = afgl_row.afgeleide_omschrijving;
                }
            }
            string[] results = { omsch_vrij, omsch_afgl};
            return results;
        }
    }

    public static class LoadFunctList
    {
        public static int _type = 0;
        public static int _record_id = 0;
        public static bedrijfformdatasetTableAdapters.bedrijfTableAdapter adapter = new bedrijfformdatasetTableAdapters.bedrijfTableAdapter();
        
        public static List<RefItem> load_list()
        {
            List<RefItem> functlist = new List<RefItem>();
            switch (_type)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    functlist = bdr_list();
                    break;
                case 3:
                    functlist = pers_list();
                    break;
            }


            return functlist;
        }
        private static List<RefItem> pers_list()
        {
            List<RefItem> tmplst = new List<RefItem>();
            ////////////////////
            int pid = _record_id;
            int result = 0;
            string column = "";
            string table = "";
            //persoonsformdatasetTableAdapters.QueriesTableAdapter qtadapt = new persoonsformdatasetTableAdapters.QueriesTableAdapter();
            adapter.Connection.ConnectionString = Global.ConnectionString_fileserver;
            persoonsformdatasetTableAdapters.persoon_nr_locatiesTableAdapter locaties_adapt = new persoonsformdatasetTableAdapters.persoon_nr_locatiesTableAdapter();
            locaties_adapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
            persoonsformdataset.persoon_nr_locatiesDataTable locaties_table = locaties_adapt.GetData();
            persoonsformdatasetTableAdapters.var_pers_funct_countTableAdapter varcnttadapt = new persoonsformdatasetTableAdapters.var_pers_funct_countTableAdapter();
            varcnttadapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
            persoonsformdataset.var_pers_funct_countDataTable varcntdt = varcnttadapt.GetData(pid);
            foreach (persoonsformdataset.persoon_nr_locatiesRow row in locaties_table.Rows)
            {
                if (row.column == "persoon_nr")
                {
                    foreach (persoonsformdataset.var_pers_funct_countRow dr in varcntdt.Rows)
                    {
                        tmplst.Add(new RefItem { funct_count = dr.aantal, funct_naam = dr.naam, functie_id = dr.functie_id, uit_var =true});
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
                        foreach (RefItem RI in tmplst)
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
                            tmplst.Add(new RefItem
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
            locaties_adapt.Dispose();
            locaties_table.Dispose();
            ///////////////////
            tmplst = tmplst.OrderByDescending(RefItem => RefItem.funct_count).ToList<RefItem>();
            return tmplst;
        }
        private static List<RefItem> bdr_list()
        {
            List<RefItem> tmplst = new List<RefItem>();
            ///////////////////
            int bid = _record_id;
            int result = 0;
            string column = "";
            string table = "";
            adapter.Connection.ConnectionString = Global.ConnectionString_fileserver;
            //bedrijfformdatasetTableAdapters.QueriesTableAdapter qtadapt = new bedrijfformdatasetTableAdapters.QueriesTableAdapter();
            bedrijfformdatasetTableAdapters.bedrijf_nr_locatiesTableAdapter locaties_adapt = new bedrijfformdatasetTableAdapters.bedrijf_nr_locatiesTableAdapter();
            locaties_adapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
            bedrijfformdataset.bedrijf_nr_locatiesDataTable locaties_table = locaties_adapt.GetData();
            bedrijfformdatasetTableAdapters.verwijder_get_varbedrijf_functnaamTableAdapter vrgfadapt = new bedrijfformdatasetTableAdapters.verwijder_get_varbedrijf_functnaamTableAdapter();
            vrgfadapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
            bedrijfformdataset.verwijder_get_varbedrijf_functnaamDataTable varfuntdt = vrgfadapt.GetData(bid);
            foreach (bedrijfformdataset.bedrijf_nr_locatiesRow row in locaties_table.Rows)
            {
                if (row.tabel == "bedrijvensetvariabel")
                {
                    foreach (bedrijfformdataset.verwijder_get_varbedrijf_functnaamRow dr in varfuntdt.Rows)
                    {
                        tmplst.Add(new RefItem {funct_naam = dr.naam, funct_count = dr.aantal, functie_id = dr.functie_ID, uit_var = true });
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
                        foreach (RefItem RI in tmplst)
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
                            tmplst.Add(new RefItem
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
            locaties_adapt.Dispose();
            locaties_table.Dispose();
            //////////////////
            tmplst = tmplst.OrderByDescending(RefItem => RefItem.funct_count).ToList<RefItem>();
            return tmplst;
        }
    }
    public class RefItem
    {
        public string funct_naam { get; set; }
        public int funct_count { get; set; }
        public int functie_id { get; set; }
        public bool uit_var { get; set; }
    }
}
