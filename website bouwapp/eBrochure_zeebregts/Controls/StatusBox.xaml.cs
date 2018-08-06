using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace eBrochure_zeebregts.Controls
{
    public partial class StatusBox : UserControl
    {
        public StatusBox()
        {
            InitializeComponent();
        }
        public void InitOverzicht()
        {
            NameTB.Text = "Status:";
       
        }
        public void SetDateVoid()
        {
            SavedAt.Text = "-";
            SavedBy.Text = "-";
            VersieNr.Text = "-";
            Uitwerking.Text = "-";
            Prijs.Text = "-";
        }
        public void SetDataSaved()
        {
            NameTB.Text = "Laatst opgeslagen offerte:";
            var opgeslagenOfferteLean = Acumulator.Instance().oOL;
            if (opgeslagenOfferteLean != null)
            {
                SavedBy.Text = opgeslagenOfferteLean.gebruiker;
                SavedAt.Text = opgeslagenOfferteLean.Datum.ToString("dd-MM-yyyy  HH:mm:ss");
                OpgeslagenopTB.Text = "Opgeslagen op:";
                VersieNr.Text = opgeslagenOfferteLean.VersieFull.ToString() + "." + opgeslagenOfferteLean.VersiePartial.ToString();
                if (opgeslagenOfferteLean.VersiePartial == 0)
                {
                    Uitwerking.Text = "volledig";
                }
                else
                {
                    Uitwerking.Text = "onvolledig";
                }
                Prijs.Text = opgeslagenOfferteLean.Prijs;
            }
            else
            {
                SavedAt.Text = "-";
                SavedBy.Text = "-";
                VersieNr.Text = "-";
            }
        }
        public void SetDataNew(DateTime savetime,bool volledig)
        {
            NameTB.Text = "Nieuw uitgewerkte offerte:";
            SavedAt.Text = savetime.ToString("dd-MM-yyyy HH:mm:ss");
            SavedBy.Text =Acumulator.Instance().HuidigGebruiker.GebruikersNaam;
            OpgeslagenopTB.Text = "Uitgewerkt op:";
            if (volledig)
            {
                if (Acumulator.Instance().oOL != null)
                {
                    VersieNr.Text = Acumulator.Instance().oOL.VersieFull + 1 + ".0";
                }
                else
                {
                    VersieNr.Text = "1.0";
                }
                Uitwerking.Text = "volledig";
            }
            else
            {
                if (Acumulator.Instance().oOL != null)
                {
                    VersieNr.Text = Acumulator.Instance().oOL.VersieFull + "." + (Acumulator.Instance().oOL.VersiePartial + 1).ToString();
                }
                else
                {
                    VersieNr.Text = "0.1";
                }
                Uitwerking.Text = "onvolledig";
            }
            if (String.IsNullOrEmpty(Acumulator.Instance().InfoBar.Bon.GetTotal())== false) { Prijs.Text = Acumulator.Instance().InfoBar.Bon.GetTotal(); }
        }
    }
}
