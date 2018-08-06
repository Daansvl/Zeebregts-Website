using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Threading;
namespace zeebregtsCs
{
   
    static class Global
    {
        private static int m_overzicht_type = 0;
        private static string m_6PPAUTH_KEY = "zYQZgaosidH8UtcE";
        private static string m_overzicht_select = "";
        private static bool _m_edit_form_proj = false;
        private static int m_userlevel = 0;
        private static bool _m_edit_form_pers = false;
        private static bool _m_edit_form_bedr = false;
        private static bool _m_retval = false;
        private static bool m_givereturn = false;
        private static int m_max_id_pers = 0;
        private static int m_max_id_bedr = 0;
        private static int m_max_id_proj = 0;
        private static string m_username = "";
        private static System.Drawing.Size m_size;
        private static System.Drawing.Point m_position;
        private static System.Windows.Forms.FormWindowState m_windowstate;
        private static bool m_van_zoek_proj = false;
        private static bool m_van_zoek_bdr = false;
        private static bool m_van_zoek_pers = false;
        private static string m_logfile = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)+"\\log.txt";
        private static Dictionary<base_form, base_form> FW = new Dictionary<base_form, base_form>();
        private static bool m_NoConnection = false;
        private static string m_WTitle = "";
        private static string m_dub_bdr_naam = "";
        private static string m_dub_bdr_zoeknaam = "";
        private static string m_dub_bdr_postcode = "";
        private static string m_dub_proj_naamproj = "";
        private static string m_dub_proj_plaats = "";
        private static int m_dub_proj_opdrachtgever = 0;
        private static string m_dub_pers_voornaam = "";
        private static string m_dub_pers_voorletters = "";
        private static string m_dub_pers_achternaam = "";
        private static int m_dub_pers_bedrijf_nr = 0;
        private static bool m_dub_pers_man = false;
        private static bool m_Dubbel_inv_check = true;
        private static bool m_dubbel_is_bevestigd = false;
        private static System.Windows.Window LW;
        private static bool m_dubb_van_zoek = false;
        private static bool m_IsRdp = false;
        public static bool isRdp
        {
            get { return m_IsRdp; }
            set { m_IsRdp = value; }
        }
      //  private static ExchangeService.Service1Client m_exservice = new ExchangeService.Service1Client() {  };

        //public static ExchangeService.Service1Client ExService
        //{
        //    get 
        //    {
        //        if (m_exservice.ClientCredentials.Windows.AllowedImpersonationLevel != System.Security.Principal.TokenImpersonationLevel.Impersonation)
        //        {
        //            m_exservice.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
        //        }
        //        return m_exservice; 
        //    }
        //}
        public static bool Dubb_van_zoek
        {
            get { return m_dubb_van_zoek; }
            set { m_dubb_van_zoek = value; }
        }
        public static System.Windows.Window LodWin
        {
            get { return LW;}
            set {LW = value ;}
        }
        public static bool Dubbel_is_bevestigd
        {
            get{return m_dubbel_is_bevestigd;}
            set{ m_dubbel_is_bevestigd = value;}
        }
        /*public static void FW_add(base_form huidig, base_form close_naar)
        {
            FW.Add(huidig, close_naar);
            String log_line ="add @ "+ DateTime.Now.ToString() + ": huidig: " + huidig.ToString() + ": close_naar: " + close_naar.ToString();
            System.IO.StreamWriter file = new System.IO.StreamWriter(Global.log_file_path, true);
            file.WriteLine(log_line);
            file.Close();
        }*/
        /*public static void FW_remove(base_form cl)
        {
            base_form tmp;
            if (FW.TryGetValue(cl,out tmp))
            {
                String log_line = "remove @ " + DateTime.Now.ToString() + " form removed: " + cl.ToString() + " target: " + tmp.ToString();
                System.IO.StreamWriter file = new System.IO.StreamWriter(Global.log_file_path, true);
                file.WriteLine(log_line);
                file.Close();
            }
            FW.Remove(cl);
        }*/
        public static string ZES_PP_AUTH_KEY
        {
            get { return m_6PPAUTH_KEY; }
        }
        public static bool NoConnection
        {
            get { return m_NoConnection; }
            set { m_NoConnection = value; }
        }
       /* public static base_form FW_menu()
        {
            List<base_form> keys = new List<base_form>(FW.Keys);
            return keys[0];
        }*/
        public static string username
        {
            get { return m_username;}
            set { m_username = value;}
        }
        public static string WTitle
        {
            get { return m_WTitle; }
            set { m_WTitle = value; }
        }
        /*public static bool Close_Clicked(base_form f)
        {
            List<base_form> keys= new List<base_form>(FW.Keys);
            int i = 0;
            bool retval = false;
            try
            {
               
                Stack<base_form> stack = new Stack<base_form>();
                if (f.kan_sluiten())
                {
                    retval = true;
                    
                    for ( i = FW.Count - 1; i >= 0; i--)
                    {


                        if (keys[i] == FW[f] || keys[i] == Global.FW_menu())
                        {
                           
                            keys[i].herlaad();
                            
                           stack.Reverse<base_form>();
                            for (int j = 0; j < stack.Count; j++)
                            {
                                base_form tmp = stack.Pop();
                                FW_remove(tmp);
                                tmp.sluit();
                                tmp.Close();
                                tmp.Dispose();
                            }
                            return retval;
                        }
                        else
                        {
                            if (keys[i].kan_sluiten())
                            {
                                stack.Push(keys[i]);

                            }
                            else
                            {
                                DialogResult dr;
                                dr = MessageBox.Show("uw wijzigingen gaan verloren. Doorgaan?", "sluiten", MessageBoxButtons.OKCancel);
                                if (dr == DialogResult.Cancel)
                                {

                                    keys[i].herlaad();
                                    stack.Reverse<base_form>();
                                    for (int j = 0; j < stack.Count; j++)
                                    {
                                        base_form tmp = stack.Pop();
                                        FW_remove(tmp);
                                        tmp.sluit();
                                        tmp.Close();
                                        tmp.Dispose();
                                    }
                                    return retval;


                                }
                                else if (dr == DialogResult.OK)
                                {
                                    stack.Push(keys[i]);
                                }
                            }
                        }

                    }
                }
                else
                {
                    DialogResult dr;
                    dr = MessageBox.Show("uw wijzigingen gaan verloren. Doorgaan?", "sluiten", MessageBoxButtons.OKCancel);
                    if (dr == DialogResult.Cancel)
                    {

                        f.Show();

                    }
                    else if (dr == DialogResult.OK)
                    {
                        f.reclose();
                        
                    }
                }

                
            }
            catch (Exception e)
            {
                MessageBox.Show(FW.Values.ToString() +" "+ keys[i].ToString() +" "+ i +" "+ e);
                Global.FW_menu().Show();
                String log_line = "crash closing@ " + DateTime.Now.ToString() + "error: " + e;
                System.IO.StreamWriter file = new System.IO.StreamWriter(Global.log_file_path, true);
                file.WriteLine(log_line);
                file.Close();
                
            }
            return retval;
        }
        */
        private static string m_returnID = "";
       // private static string m_ConnectionString_notebook = "Server=NOTEBOOK-SERVER\\SQLEXPRESS;Database=zeebregtsdb;Trusted_connection=Yes;MultipleActiveResultSets=True;";
        private static string m_ConnectionString_fileserver = Properties.Settings.Default.zeebregtsdbConnectionStringRemote;//"Data Source=SQL-SERVER;Initial Catalog=zeebregtsdb;MultipleActiveResultSets=True;Integrated Security=SSPI";//"Data Source=FILE-SERVER;Initial Catalog=zeebregtsdb;Persist Security Info=True;User ID=daan;Password=bl22s"; //"Server=FILE-SERVER;Database=zeebregtsdb;Trusted_Connection=Yes;Integrated Security=SSPI";
        private static string m_ConnectionString_Mdr = "Data Source=SQL-SERVER; Initial Catalog=MandagenRegistratieBeta; MultipleActiveResultSets=True;User ID=sa;Password=Zeebregts2013##;";
        public static int UserLevel
        {
            get { return m_userlevel; }
            set { m_userlevel = value; }
        }
        public static string ConnectionString_fileserver
        {
            get { return m_ConnectionString_fileserver; }
            set { m_ConnectionString_fileserver = value; }
        }
        public static string ConnectionString_Mdr
        {
            get { return m_ConnectionString_Mdr; }
            set { m_ConnectionString_Mdr = value; }
        }
        public static string log_file_path
        {
            get { return m_logfile; }
            set { m_logfile = value; }
        }
        public static int overzicht_type
        {
            get { return m_overzicht_type; }
            set { m_overzicht_type = value; }
        }
        public static string overzicht_select
        {
            get { return m_overzicht_select; }
            set { m_overzicht_select = value; }
        }
        public static bool edit_form_proj
        {
            get { return _m_edit_form_proj; }
            set { _m_edit_form_proj = value; }
        }
        public static bool retval
        {
            get { return _m_retval; }
            set { _m_retval = value; }
        }
        public static bool edit_form_pers
        {
            get { return _m_edit_form_pers; }
            set { _m_edit_form_pers = value; }
        }
        public static bool edit_form_bedr
        {
            get { return _m_edit_form_bedr; }
            set { _m_edit_form_bedr = value; }
        }
        public static bool give_return
        {
            get { return m_givereturn; }
            set { m_givereturn = value; }
        }
        public static int max_id_pers
        {
            get { return m_max_id_pers; }
            set { m_max_id_pers = value; }
        }
        public static int max_id_bedr
        {
            get { return m_max_id_bedr; }
            set { m_max_id_bedr = value; }
        }
        public static int max_id_proj
        {
            get { return m_max_id_proj; }
            set { m_max_id_proj = value; }
        }
        public static string return_id
        {
            get { return m_returnID; }
            set { m_returnID = value; }
        }
        public static System.Drawing.Size size
        {
            get { return m_size;}
            set { m_size = value; }

        }
        public static System.Drawing.Point position
        {
            get { return m_position; }
            set { m_position = value; }
        }
        public static System.Windows.Forms.FormWindowState windowstate
        {
            get { return m_windowstate; }
            set { m_windowstate = value; }
        }
        public static bool van_zoek_proj
        {
            get { return m_van_zoek_proj; }
            set { m_van_zoek_proj = value; }
        }
        public static bool van_zoek_bdr
        {
            get { return m_van_zoek_bdr; }
            set { m_van_zoek_bdr = value; }
        }
        public static bool van_zoek_pers
        {
            get { return m_van_zoek_pers; }
            set { m_van_zoek_pers = value; }
        }
        public static bool Dubbel_inv_check
        {
            get { return m_Dubbel_inv_check; }
            set { m_Dubbel_inv_check = value; }
        }
        public static string dub_bdr_naam
        {
            get { return m_dub_bdr_naam; }
            set { m_dub_bdr_naam = value; }
        }
        public static string dub_bdr_zoeknaam
        {
            get { return m_dub_bdr_zoeknaam; }
            set { m_dub_bdr_zoeknaam = value; }
        }
        public static string dub_bdr_postcode
        {
            get { return m_dub_bdr_postcode; }
            set { m_dub_bdr_postcode = value; }
        }
        public static string dub_proj_naamproj
        {
            get { return m_dub_proj_naamproj; }
            set { m_dub_proj_naamproj = value; }
        }
        public static string dub_proj_plaats
        {
            get { return m_dub_proj_plaats; }
            set { m_dub_proj_plaats = value; }
        }
        public static int dub_proj_opdrachtgever
        {
            get { return m_dub_proj_opdrachtgever; }
            set { m_dub_proj_opdrachtgever = value; }
        }
        public static string dub_pers_voornaam
        {
            get { return m_dub_pers_voornaam; }
            set { m_dub_pers_voornaam = value; }
        }
        public static string dub_pers_voorletters
        {
            get { return m_dub_pers_voorletters; }
            set { m_dub_pers_voorletters = value; }
        }
        public static string dub_pers_achternaam
        {
            get { return m_dub_pers_achternaam; }
            set { m_dub_pers_achternaam = value; }
        }
        public static int dub_pers_bedrijf_nr
        {
            get { return m_dub_pers_bedrijf_nr; }
            set { m_dub_pers_bedrijf_nr = value; }
        }
        public static bool dub_pers_man
        {
            get { return m_dub_pers_man; }
            set { m_dub_pers_man = value; }
        }
    }
}
