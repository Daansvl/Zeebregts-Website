using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MandagenRegistratie.controls.Projecten.Weekview;
using MandagenRegistratie.controls.Vakmannen.Overzicht;
using MandagenRegistratie.controls.Projecten.Overzicht;
using ZeebregtsLogic;
using MandagenRegistratie.controls.Projectleiders.Overzicht;
using System.Windows.Threading;

namespace MandagenRegistratie.controls.Navigatie
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : UserControl
    {

        public MainMenu()
        {
            InitializeComponent();

            SetDefaults();
        }

        public void SetDefaults()
        {
            DateTime nu = DateTime.Now;

            ApplicationState.SetValue(ApplicationVariables.dtSelectedDay, Tools.CalculateWeekstart(new DateTime(nu.Year,nu.Month,nu.Day,0,0,0)));

        }

        private void btnProjectenOverview_Click(object sender, RoutedEventArgs e)
        {


            // create the page and load all values
            MandagenRegistratie.controls.Projecten.Overzicht.Projecten pvo = new MandagenRegistratie.controls.Projecten.Overzicht.Projecten();
            pvo.LoadView();

            //// add the item to the navigation history
            //ApplicationState.GetValue<List<Control>>(ApplicationVariables.listNavigationHistory).Insert(0, vdg);

            //// load the correct navigation
            //((PageContainer)((StackPanel)((ContentControl)Parent).Parent).Parent).LoadNavigatie();

            // load the page into the contentcontrol
            //((PageContainer)((DockPanel)((ContentControl)Parent).Parent).Parent).LoadControl(pvo);
            Tools.FindVisualParent<PageContainer>(this).LoadControl(pvo);

        }

        private void btnVakmannenOverview_Click(object sender, RoutedEventArgs e)
        {
            // create the page and load all values
            VakmannenOverview vov = new VakmannenOverview();
            vov.LoadView();

            // load the page into the contentcontrol
            //((PageContainer)((DockPanel)((ContentControl)Parent).Parent).Parent).LoadControl(vov);
            Tools.FindVisualParent<PageContainer>(this).LoadControl(vov);

        }

        private void btnSql2Excel_Click(object sender, RoutedEventArgs e)
        {
            Excel2SqlControl.Scherm1 scherm1 = new Excel2SqlControl.Scherm1();


            // load the page into the contentcontrol

            //TEST test = new TEST();
            //test.HHH = ((PageContainer)((StackPanel)((ContentControl)Parent).Parent).Parent).wfhTest;

            // load the page into the contentcontrol
            //((PageContainer)((DockPanel)((ContentControl)Parent).Parent).Parent).LoadControl(scherm1);
            Tools.FindVisualParent<PageContainer>(this).LoadControl(scherm1);
        }


        int aantalKliks = 0;

        /// <summary>
        /// Test knopje om mn nichtje van 12 te leren programmeren :-)
        /// Ik laat het voor de grap in het programma staan
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnIshara_Click(object sender, RoutedEventArgs e)
        {
            aantalKliks = aantalKliks + 1;

            //MessageBox.Show("Ik heb op Ishara geklikt, en Lucia kijkt, en Igna kijkt een film");
            MessageBox.Show("Ik heb " + aantalKliks.ToString() + " keer op Ishara geklikt");

            if (aantalKliks == 1)
            {
                MessageBox.Show("Ik heb 1x op Ishara geklikt");

            }
            else if (aantalKliks == 2)
            {
                MessageBox.Show("Dit is een prijsvraag, hoe meer je klikt, hoe meer je kans maakt op een prijs.");

            }
            else if (aantalKliks == 3)
            {
                MessageBox.Show("Je bent nu steeds dichter bij de prijs, klap 2x in je handen om de prijs te krijgen");

            }
            else if (aantalKliks == 4)
            {
                MessageBox.Show("Wow je bent nu echt eng dichtbij, kijk eerst iets beter naar je pc en knip 2x in je vingers");
            }
            else if (aantalKliks == 5)
            {
                MessageBox.Show("Om de prijs te winnen moet je een liedje van je favoriete artiest zingen");
            }
            else if (aantalKliks == 6)
            {
                MessageBox.Show("Je hebt alle opdrachten goed voldaan, wat zou de prijs zijn...");
            }
            else if (aantalKliks == 7)
            {
                MessageBox.Show("De prijs is...");
            }
            else if (aantalKliks > 7)
            {
                MessageBox.Show("Je hebt al gewonnen, je zou nu al moeten weten wat de prijs is.. of begin je weer opnieuw?");
                aantalKliks = 0;
            }

        }

        private void btnProjectleidersOverview_Click(object sender, RoutedEventArgs e)
        {
            // create the page and load all values
            ProjectleidersOverzicht plv = new ProjectleidersOverzicht();

            // load the page into the contentcontrol
            //((PageContainer)((DockPanel)((ContentControl)Parent).Parent).Parent).LoadControl(plv);
            Tools.FindVisualParent<PageContainer>(this).LoadControl(plv);
        }
    }
}
