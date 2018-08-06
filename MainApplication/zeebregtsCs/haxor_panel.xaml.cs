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
    /// Interaction logic for haxor_panel.xaml
    /// </summary>
    public partial class haxor_panel : UserControl
    {
        public haxor_panel()
        {
            InitializeComponent();
        }
        public void start_loading_proj(string naam, string plaats, int opdgeef)
        {
            Dubbel_invoer_viewer.type = 1;
            Dubbel_invoer_viewer.proj_naamproj = naam;
            Dubbel_invoer_viewer.proj_plaats = plaats;
            Dubbel_invoer_viewer.proj_opdgeef = opdgeef;
            doubleinvcheck_view.ItemsSource = Dubbel_invoer_viewer.Load_DI_list();
        }
        public void start_loading_bdr(string naam, string zoeknaam, string postcode)
        {
            Dubbel_invoer_viewer.type = 2;
            Dubbel_invoer_viewer.bdr_naam = naam;
            Dubbel_invoer_viewer.bdr_zoeknaam = zoeknaam;
            Dubbel_invoer_viewer.bdr_postcode = postcode;
            doubleinvcheck_view.ItemsSource = Dubbel_invoer_viewer.Load_DI_list();
        }
        public void start_loading_pers(string voornaam, string voorletters, string achternaam, int bdrnr, bool man)
        {
            Dubbel_invoer_viewer.type = 3;
            Dubbel_invoer_viewer.pers_voornaam = voornaam;
            Dubbel_invoer_viewer.pers_voorletters = voorletters;
            Dubbel_invoer_viewer.pers_achternaam = achternaam;
            Dubbel_invoer_viewer.pers_bdrnr = bdrnr;
            Dubbel_invoer_viewer.pers_man = man;
            doubleinvcheck_view.ItemsSource = Dubbel_invoer_viewer.Load_DI_list();
        }
    }
    public static class Dubbel_invoer_viewer
    {
        public static int type { get; set; }
        public static string proj_naamproj { get; set; }
        public static string proj_plaats { get; set; }
        public static int proj_opdgeef { get; set; }

        public static string bdr_naam { get; set; }
        public static string bdr_zoeknaam { get; set; }
        public static string bdr_postcode { get; set; }

        public static string pers_voornaam { get; set; }
        public static string pers_voorletters { get; set; }
        public static string pers_achternaam { get; set; }
        public static int    pers_bdrnr { get; set; }
        public static bool   pers_man { get; set; }
       private static  haxor_datasetTableAdapters.Dubbel_inv_debugTableAdapter DiD_ta;
       private static haxor_dataset.Dubbel_inv_debugDataTable DiD_dt;
        public static List<DU_Item> Load_DI_list()
       {
           List<DU_Item> De_list = new List<DU_Item>();
            try
            {
                DiD_ta = new haxor_datasetTableAdapters.Dubbel_inv_debugTableAdapter();
                DiD_ta.Connection.ConnectionString = Global.ConnectionString_fileserver;
                DiD_dt = new haxor_dataset.Dubbel_inv_debugDataTable();
                switch (type)
                {
                    case 1:
                       De_list = dubblinv_linq.Get_doublicates_hxzr_proj(proj_naamproj, proj_plaats, proj_opdgeef);
                        break;
                    case 2:
                        De_list = dubblinv_linq.Get_doublicates_hxzr_bdr(bdr_naam, bdr_zoeknaam, bdr_postcode);
                        break;
                    case 3:
                        DiD_dt = DiD_ta.GetData_pers(pers_voornaam, pers_voorletters, pers_achternaam, pers_bdrnr, pers_man);
                        foreach (haxor_dataset.Dubbel_inv_debugRow DiD_row in DiD_dt.Rows)
                        {
                            De_list.Add(new DU_Item { ID = DiD_row.ID, zoek_col1 = DiD_row.zoek_col1, zoek_col2 = DiD_row.zoek_col2, zoek_col3 = DiD_row.zoek_col3 });
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                String log_line = "Exception occurred @ " + DateTime.Now.ToString() + " error: " + e;
                System.IO.StreamWriter file = new System.IO.StreamWriter(Global.log_file_path, true);
                file.WriteLine(log_line);
                file.Close();
                if (DiD_dt.HasErrors)
                {
                    foreach (DataRow x in DiD_dt.GetErrors())
                    {
                        string error = x.RowError;
                    }
                }
                
            }

            
            

            return De_list;
        }
    }

    public class DU_Item
    {
        public int    ID        { get; set; }
        public string zoek_col1 { get; set; }
        public string zoek_col2 { get; set; }
        public string zoek_col3 { get; set; }
    }
}
