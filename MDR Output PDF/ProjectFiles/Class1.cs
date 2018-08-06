using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDR2PDF
{
    public partial class MarioPDFSettings
    {
        public Gebruiker Gebruiker = new Gebruiker();
        //public bool Ismanager = false;
        public string Omgeving = "";
        public classLijstgegevens Lijstgegevens = new classLijstgegevens();
        // Schermgegevens
        public int schermH = 0;
        public int schermW = 0;
    }

    public partial class classSubLijstGegevens
    {
        public string SubCode = string.Empty;
        public string SubTitel = string.Empty;
        public bool Managers = true; // Only for Managers!
        public bool ProjectLeiders = true;
    }

    public partial class classLijstgegevens
    {
        //public bool OnlyManagers = true; // Only for Managers!
        public string Code = string.Empty;
        public string Titel = string.Empty;
        //public int SublijstKeuze = -1;
        public List<classSubLijstGegevens> SubLijst = new List<classSubLijstGegevens>();
        //public List<string> Subtitels = new List<string>();
        //public List<bool> SubtitelsOnlyManagers = new List<bool> { true, true, true, true, true, true, true, true, true, true };
    }


    public partial class LijstOmschrijvingen
    {
        List<string> xxx = new List<string>();

    }
    public class GlobaleVars
    {
        public static string strApplicationState = "MSZ_PDF_Output";
    }

    class Class1
    {
        public class Globals
        {
            public static string MandagenRegistratieConnectionString = "";
            //public static string ZeebregtsDBConnectionString = ConfigurationManager.ConnectionStrings["ZeebregtsDBConnectionString"].ConnectionString;
            public static string ZeebregtsDBConnectionString = "jkhjk";
            public static string test = "13";


            private static bool _expired;
            public static bool Expired
            {
                get
                {
                    // Reads are usually simple
                    return _expired;
                }
                set
                {
                    // You can add logic here for race conditions,
                    // or other measurements
                    _expired = value;
                }
            }
            // Perhaps extend this to have Read-Modify-Write static methods
            // for data integrity during concurrency? Situational.
        }
    }
}
