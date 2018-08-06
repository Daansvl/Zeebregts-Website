using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeebregtsLogic
{
    public class Logging
    {

        public static string connectionString = ConfigurationManager.ConnectionStrings["MandagenRegistratieConnectionString"].ConnectionString;
        public dbDataContext datacontext = new dbDataContext(connectionString);

        public void Log(string logregel)
        {
            ZeebregtsLogic.Loggen log = new ZeebregtsLogic.Loggen();

            log.Ipaddress = string.IsNullOrWhiteSpace(ApplicationState.GetValue<string>(ApplicationVariables.strIpAddress)) ? "-" : ApplicationState.GetValue<string>(ApplicationVariables.strIpAddress);
            log.Message = logregel;
            log.Datum = DateTime.Now;

            datacontext.Loggens.InsertOnSubmit(log);
            datacontext.SubmitChanges();
            // mm:ss.fff
        }

        public void Log(string ipaddress, string logregel)
        {
            ZeebregtsLogic.Loggen log = new ZeebregtsLogic.Loggen();

            log.Ipaddress = ipaddress;
            log.Message = logregel;
            log.Datum = DateTime.Now;

            datacontext.Loggens.InsertOnSubmit(log);
            datacontext.SubmitChanges();
            // mm:ss.fff
        }

    }
}
