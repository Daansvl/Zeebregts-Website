using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace MDR2PDF
{

    class GlobaleVars2
    {
        public class Globals
        {

            //public static string WeekLijst = @ConfigurationManager.AppSettings["Weeklijst"];
            //public static string MandagenRegistratieConnectionString = ConfigurationManager.ConnectionStrings["MandagenRegistratieConnectionString"].ConnectionString;
            //public static string ZeebregtsDBConnectionString = ConfigurationManager.ConnectionStrings["ZeebregtsDBConnectionString"].ConnectionString;
            
            public static string test = "13";

            // TESTTESTTEST
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
            public static string TESTTEST = "HOIOI";
        }



    }


}
