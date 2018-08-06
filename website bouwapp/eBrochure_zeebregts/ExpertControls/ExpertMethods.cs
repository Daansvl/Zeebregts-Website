using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace eBrochure_zeebregts.ExpertControls
{
    public static class ExpertMethods
    {
        public static double instapPrijsCalc(string id, string oms, Dictionary<string, int> checklist, double prijs,Dictionary<string, double> HeleDozenLijst)
        {
            var retval = 0.0;
            if (checklist.ContainsKey(id) == true)
            {
                checklist[id] += 1;
            }
            else
            {
                retval = prijs;
                checklist.Add(id, 1);
                HeleDozenLijst.Add(oms, prijs);
            }
            return retval;
        }

        public static double CultureDeliminatorConverter(string sourcestring)
        {
            double outputdouble = 0.0;
            if (!String.IsNullOrEmpty(sourcestring))
            {
                sourcestring = sourcestring.Replace(",", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                sourcestring = sourcestring.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                double.TryParse(sourcestring, out outputdouble);
            }
            return outputdouble;
        }
		
    }
}
