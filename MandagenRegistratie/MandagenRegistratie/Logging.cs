using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MandagenRegistratieDomain;
using ZeebregtsLogic;

namespace MandagenRegistratie
{
    public class Logging
    {
        protected dbRepository dbrep = new dbRepository();

        public void Log(string logregel)
        {
            MandagenRegistratieDomain.Loggen log = new MandagenRegistratieDomain.Loggen();
            
            log.Ipaddress = string.IsNullOrWhiteSpace(ApplicationState.GetValue<string>(ApplicationVariables.strIpAddress)) ? "-" : ApplicationState.GetValue<string>(ApplicationVariables.strIpAddress);
            log.Message = logregel;
            log.Datum = DateTime.Now;

            dbrep.datacontext.Loggens.InsertOnSubmit(log);
            dbrep.datacontext.SubmitChanges();
            // mm:ss.fff
        }

        public void Log(string ipaddress, string logregel)
        {
            MandagenRegistratieDomain.Loggen log = new MandagenRegistratieDomain.Loggen();

            log.Ipaddress = ipaddress;
            log.Message = logregel;
            log.Datum = DateTime.Now;

            dbrep.datacontext.Loggens.InsertOnSubmit(log);
            dbrep.datacontext.SubmitChanges();
            // mm:ss.fff
        }

    }
}
