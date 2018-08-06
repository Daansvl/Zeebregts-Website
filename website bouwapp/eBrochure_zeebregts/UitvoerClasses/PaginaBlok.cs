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

namespace eBrochure_zeebregts.UitvoerClasses
{
    public class PaginaBlok
    {
        public int BlokHoogte { get; set; }
        public string LinkedRuimteId { get; set; }
        public List<string> KantlijnRegels { get; private set; }
        public List<string> TextRegels { get; private set; }

        public List<bool> BoolRegels { get; private set; }
        public int RegelHoogte { get; set; }

        public PaginaBlok(int rglH)
        {
            RegelHoogte = rglH;
            KantlijnRegels = new List<string>();
            TextRegels = new List<string>();
            BoolRegels = new List<bool>();
        }
        public void AddRegel(string kantlijn, string text, bool isbold)
        {
            KantlijnRegels.Add((String.IsNullOrEmpty(kantlijn) ? "" : kantlijn));
            TextRegels.Add((String.IsNullOrEmpty(text) ? "" : text));
            BoolRegels.Add(isbold);
            //KantlijnRegels.Add(Environment.NewLine);
            //TextRegels.Add(Environment.NewLine);

            BlokHoogte += RegelHoogte;
        }
    }
}
