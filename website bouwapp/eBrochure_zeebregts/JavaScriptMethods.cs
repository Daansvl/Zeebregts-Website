using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Browser;
using eBrochure_zeebregts.Classes;

namespace eBrochure_zeebregts
{
    public class JavaScriptMethods
    {
        [ScriptableMember]
        public void ClosingPage()
        {
            /*if (Acumulator.Instance().HuidigGebruiker != null && Acumulator.Instance().HuidigGebruiker.GebruikersNaam != null)
            {
                LogHelper.SendLog("eBrochure Closed by user: " + Acumulator.Instance().HuidigGebruiker.GebruikersNaam, LogType.status);
            }*/
           // MessageBox.Show("Closing");

        }
    }
}
