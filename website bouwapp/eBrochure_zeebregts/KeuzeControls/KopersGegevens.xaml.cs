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
using System.ComponentModel;

namespace eBrochure_zeebregts.KeuzeControls
{
    public partial class KopersGegevens : UserControl ,IBaseControl
    {
        
        public KopersGegevens()
        { 
            InitializeComponent();
            Acumulator.Instance().BB.ShowWijzigBtn(true);
            LoadData();
            SwitchWijzig(true);
        }
        public void LoadData()
        {
            var kavelinfo = (from k in Acumulator.Instance().ctx.Bouwnummers
                             where k.B_ID == Acumulator.Instance().Bouwnr
                             select k.Omschrijving).FirstOrDefault().ToString();
            var proj = (from p in Acumulator.Instance().ctx.PRojects
                        join b in Acumulator.Instance().ctx.Bouwnummers on p.PR_ID equals b.PR_NR
                        where b.B_ID == Acumulator.Instance().Bouwnr
                        select p.Omschrijving).FirstOrDefault();
            var koper = (from b in Acumulator.Instance().ctx.Bouwnummers
                         where b.B_ID == Acumulator.Instance().Bouwnr
                         select b).FirstOrDefault();
            InputNaam.Text = koper.KlantNaam ?? "";
            InputStraat.Text = koper.adres_straat ?? "";
            InputPlaats.Text = koper.adres_plaats ?? "";
            InputEmail.Text = koper.email ?? "";
            InputTel1.Text = koper.Telefoon1 ?? "";
            InputTel2.Text = koper.Telefoon2 ?? "";
            TBproject.Text = proj ?? "";
            TBbouwnr.Text = kavelinfo;
            TBRuimte.Text = "";
            foreach (var r in Acumulator.Instance().OTracker.offerteRuimte_.Children)
            {
                var spiegeld = "";
                var sp = (from rpt in Acumulator.Instance().ctx.RuimtesPerTypes
                          where rpt.R_NR == (r as Ruimte).RuimteID && rpt.T_NR == Acumulator.Instance().Type
                          select rpt.Spiegel).FirstOrDefault();
                if (sp??false)
                {
                    spiegeld = " (Gespiegeld)";
                    
                }
                TBRuimte.Text += (r as Ruimte).Omschrijving +spiegeld + Environment.NewLine;
            }
            var opgeslagenOfferteLean = Acumulator.Instance().oOL;
            if (opgeslagenOfferteLean != null)
            {
                statusBox.SetDataSaved();
               // SavedBy.Text = opgeslagenOfferteLean.gebruiker;
                //SavedAt.Text = opgeslagenOfferteLean.Datum.ToString("dd-MM-yyyy  HH:mm:ss");
                //VersieNr.Text = opgeslagenOfferteLean.VersieFull.ToString() + "." + opgeslagenOfferteLean.VersiePartial.ToString();
            }
            else
            {
                statusBox.SetDateVoid();
                //SavedAt.Text = "-";
                //SavedBy.Text = "-";
                //VersieNr.Text = "-";
            }
            statusBox.InitOverzicht();
            if (Acumulator.Instance().HuidigGebruiker.Rol == UserRole.Admin)
            {
                HiddenBtn.Visibility = System.Windows.Visibility.Visible;
            }
           
        }
        public void WijzigPressed()
        {
            SwitchWijzig(false);
        }
        public void Clear4Submit()
        {
        }
        public bool SubmitPressed()
        {
            var koper = (from b in Acumulator.Instance().ctx.Bouwnummers
                         where b.B_ID == Acumulator.Instance().Bouwnr
                         select b).FirstOrDefault();
            koper.KlantNaam = InputNaam.Text;
            koper.adres_straat = InputStraat.Text;
            koper.adres_plaats = InputPlaats.Text;
            koper.email = InputEmail.Text;
            koper.Telefoon1 = InputTel1.Text;
            koper.Telefoon2 = InputTel2.Text;
            Acumulator.Instance().ctx.SubmitChanges();
            return true;
        }
        private void SwitchWijzig(bool leesalleen)
        {
            if (leesalleen)
            {
                InputNaam.Visibility = System.Windows.Visibility.Collapsed;
                InputStraat.Visibility = System.Windows.Visibility.Collapsed;
                InputPlaats.Visibility = System.Windows.Visibility.Collapsed;
                InputEmail.Visibility = System.Windows.Visibility.Collapsed;
                InputTel1.Visibility = System.Windows.Visibility.Collapsed;
                InputTel2.Visibility = System.Windows.Visibility.Collapsed;

                LblNaam.Visibility = Visibility.Visible;
                LblPlaats.Visibility = Visibility.Visible;
                LblStraat.Visibility = Visibility.Visible;
                LblEmail.Visibility = Visibility.Visible;
                LblTel1.Visibility = Visibility.Visible;
                LblTel2.Visibility = Visibility.Visible;
            }
            else
            {
                InputNaam.Visibility = System.Windows.Visibility.Visible;
                InputStraat.Visibility = System.Windows.Visibility.Visible;
                InputPlaats.Visibility = System.Windows.Visibility.Visible;
                InputEmail.Visibility = System.Windows.Visibility.Visible;
                InputTel1.Visibility = System.Windows.Visibility.Visible;
                InputTel2.Visibility = System.Windows.Visibility.Visible;

                LblNaam.Visibility = Visibility.Collapsed;
                LblPlaats.Visibility = Visibility.Collapsed;
                LblStraat.Visibility = Visibility.Collapsed;
                LblEmail.Visibility = Visibility.Collapsed;
                LblTel1.Visibility = Visibility.Collapsed;
                LblTel2.Visibility = Visibility.Collapsed;
            }
        }

        private void HiddenBtn_Click_1(object sender, RoutedEventArgs e)
        {
            var xc = new Helpers.XmlChecker();
            xc.CheckAllSaves();
        }
    
    
    }
}
