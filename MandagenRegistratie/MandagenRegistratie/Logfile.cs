using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeebregtsLogic;

namespace MandagenRegistratie
{
    public class Logfile
    {

        public static void Log(string regel)
        {

            string currentLog = ApplicationState.GetValue<string>("strSessionLog");
            long startTicks = ApplicationState.GetValue<long>("intStartTime");

            long interval = DateTime.Now.Ticks - startTicks;

            TimeSpan timespan = new TimeSpan(interval);

            StringBuilder temp = new StringBuilder(currentLog);
            temp = temp.AppendLine(timespan.TotalMilliseconds.ToString() + "\t" + regel);

            ApplicationState.SetValue("strSessionLog", temp.ToString());

        }

        public static void Clear()
        {

            ApplicationState.SetValue("strSessionLog", string.Empty);



        }

        public static void ResetTimer()
        {
            ApplicationState.SetValue("intStartTime", DateTime.Now.Ticks);

        }

        public static void Save()
        {

            File.WriteAllText(System.Environment.CurrentDirectory + "\\LogFile.txt", ApplicationState.GetValue<string>("strSessionLog"));

        }
    }
}
