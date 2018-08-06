using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MandagenRegistratieDomain;
using ZeebregtsLogic;

namespace MandagenRegistratie.controls.Vakmannen.Overzicht
{
    /// <summary>
    /// Interaction logic for PerProjectleiderHeader.xaml
    /// </summary>
    public partial class OverzichtHeader : UserControl
    {
        public Gebruiker projectleider { get; set; }
        public DateTime startdatum { get; set; }
        public DateTime einddatum { get; set; }
        public List<vwVakman> listVwVakmannen { get; set; }
        public List<Vakman> listVakmannen { get; set; }
        public List<MDRpersoon> listMDRpersoons { get; set; }

        public OverzichtHeader()
        {
            InitializeComponent();
        }

        public void RecalcHeaderTexts(double width)
        {
            foreach (UIElement ui in spProjects.Children)
            {
                if (ui.GetType() == typeof(TextBlock))
                {
                    ((TextBlock)ui).Width = this.Width;
                    ((TextBlock)ui).Text = ((TextBlock)ui).ToolTip.ToString().ToString(width * 2);
                }
            }
        }

        public void Reload(List<int> objListVakmannen, DateTime datum)
        {
            // hier even niks doen
        }

        public void Load()
        {

            dbOriginalRepository dbrepOriginal = new dbOriginalRepository();

            foreach (vwVakman vakman in listVwVakmannen)
            {
                MDRpersoon po = listMDRpersoons.FirstOrDefault(p => p.persoon_ID == vakman.ContactIdOrigineel);

                StackPanel spProjectNaam = new StackPanel();
                spProjectNaam.Orientation = Orientation.Horizontal;

                TextBlock lblProjectNr = new TextBlock();
                lblProjectNr.Width = 40;
                lblProjectNr.Height = 19;
                lblProjectNr.Margin = new Thickness(10, 2, 0, 0);

                lblProjectNr.FontSize = 11;
                lblProjectNr.Padding = new Thickness(0, 2, 0, 0);
                lblProjectNr.Text = po.persoon_nr.ToString();

                TextBlock lbl = new TextBlock();
                lbl.Text = ToonNaam(po).ToString(300);
                lbl.ToolTip = ToonNaam(po);
                lbl.Height = 19;
                lbl.Margin = new Thickness(0, 2, 0, 0);

                lbl.FontSize = 11;
                lbl.Padding = new Thickness(0, 2, 0, 0);

                //lbl.FontWeight = FontWeights.ExtraBold;
                //lbl.BorderThickness = new Thickness(0);

                spProjectNaam.Children.Add(lblProjectNr);
                spProjectNaam.Children.Add(lbl);

                spProjects.Children.Add(spProjectNaam);

            }


        }

        public string ToonNaam(MDRpersoon objPersoon)
        {
            if (objPersoon != null)
            {
                return (objPersoon.voornaam + " " + objPersoon.tussenvoegsel + " " + objPersoon.achternaam).ToStringTrimmed();
            }
            else
            {
                return "";
            }


        }



    }
}
