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

namespace MandagenRegistratie.controls.Projecten.Overzicht
{
    /// <summary>
    /// Interaction logic for PerProjectleiderHeader.xaml
    /// </summary>
    public partial class PerProjectleiderHeader : UserControl
    {
        public Gebruiker projectleider { get; set; }
        public DateTime startdatum { get; set; }
        public DateTime einddatum { get; set; }
        public List<Project> listProjecten { get; set; }

        public PerProjectleiderHeader()
        {
            InitializeComponent();
        }

        public void RecalcHeaderTexts(double width)
        {
            foreach (UIElement ui in spProjects.Children)
            {
                if (ui.GetType() == typeof(StackPanel))
                {
                    foreach (UIElement ui2 in ((StackPanel)ui).Children)
                    {
                        if (ui2.GetType() == typeof(Label))
                        {
                            if (((Label)ui2).ToolTip != null)
                            {
                                ((Label)ui2).Content = ((Label)ui2).ToolTip.ToString().ToString((width-50) * 1.75);
                            }
                        }
                    }

                    //((TextBlock)ui).Width = this.Width;
                    //((TextBlock)ui).Text = ((TextBlock)ui).ToolTip.ToString().ToString(width * 1.8);
                }
            }
        }

        public void Reload(List<int> listVakmannen, DateTime datum)
        {
            // hier even niks doen
        }


        public void Load()
        {

            dbOriginalRepository dbrepOriginal = new dbOriginalRepository();
            MDRpersoon pp = dbrepOriginal.GetContact(projectleider.ContactIdOrigineel);
            lblProjectleider.Content = ToonNaam(pp);

            foreach (Project project in listProjecten)
            {
                MDRproject po = dbrepOriginal.GetProject((int)project.ProjectNr, true);

                StackPanel spProjectNaam = new StackPanel();
                spProjectNaam.Orientation = Orientation.Horizontal;

                Label lblProjectNr = new Label();
                lblProjectNr.Width = 40;
                lblProjectNr.Height = 19;
                lblProjectNr.Margin = new Thickness(10, 2, 0, 0);

                lblProjectNr.FontSize = 11;
                lblProjectNr.Padding = new Thickness(0, 2, 0, 0);

                if (po != null)
                {
                    lblProjectNr.Content = po.project_NR.ToString();
                }

                Label lbl = new Label();
                lbl.Content = project.Naam.ToString(300);
                lbl.ToolTip = project.Naam;
                lbl.Height = 19;
                lbl.Margin = new Thickness(0, 2, 0, 0);

                lbl.FontSize = 11;
                lbl.Padding = new Thickness(0, 2, 0, 0);
                //lbl.Background = new SolidColorBrush(Colors.Bisque);

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
