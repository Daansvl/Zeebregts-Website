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
using MandagenRegistratie.controls.Projecten.Dagview;
using MandagenRegistratie.controls.Vakmannen.Lijst;
using MandagenRegistratie.controls.Projecten.Lijst;
using ZeebregtsLogic;
using MandagenRegistratie.controls.Admin;
using System.Windows.Threading;
using MandagenRegistratieDomain;
using MandagenRegistratie.controls.Projecten.Overzicht;
using MandagenRegistratie.controls.Projecten;

namespace MandagenRegistratie.controls.Navigatie
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : NavigationControl
    {

        public MainMenu()
        {
            try
            {
                InitializeComponent();

                Reloaded += SetButtons;

                // log de gebruiker in op basis van WindowsIdentity
                SetDefaults();
                
                // juiste buttons tonen op basis van de tijdens SetDefaults() verworven rechten
                SetButtons();

                Loaded += MainMenu_Loaded;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void MainMenu_Loaded(object sender, RoutedEventArgs e)
        {
            Tools.FindVisualParent<PageContainer>(this).PreviewKeyDown += NavigationControl_PreviewKeyDown_1;
            Tools.FindVisualParent<PageContainer>(this).PreviewKeyUp += NavigationControl_PreviewKeyUp_1;

        }

        public void SetDefaults()
        {
            DateTime nu = DateTime.Now;


            ApplicationState.SetValue(ApplicationVariables.dtSelectedDay, Tools.CalculateWeekstart(new DateTime(nu.Year,nu.Month,nu.Day,0,0,0)));

            dbOriginalRepository dborep = new dbOriginalRepository();
            ApplicationState.SetValue(ApplicationVariables.listMDRPersoons, dborep.datacontext.MDRpersoons.ToList());
            ApplicationState.SetValue(ApplicationVariables.listMDRProjecten, dborep.datacontext.MDRprojects.ToList());

            // find the currently logged in user
            string strWindowsUser;
            strWindowsUser = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
            string[] strWindowsUserArray = strWindowsUser.Split('\\');
            string strCleanUsername = strWindowsUserArray[strWindowsUserArray.Length - 1];

            dbRepository dbrep = new dbRepository();
            Gebruiker gebruiker = dbrep.GetGebruiker(strCleanUsername);


            if (gebruiker != null)
            {
                Login.Edit.LoginAls(gebruiker.GebruikerId, true, this);

                // set de rechten op de knoppen
                SetButtons();

            }
            else
            {

                // NIEUWE GEBRUIKER

                // gebruiker == null, geen gebruiker gevonden
                //MessageBox.Show("Geen gebruiker gevonden, er is geen gekoppelde projectleider automatisch ingelogd. Neem contact op met een systeembeheerder en probeer het opnieuw.");


                gebruiker = new Gebruiker();
                gebruiker.IsAdministrator = false;
                gebruiker.CanLoginAsProjectleider = false;
                gebruiker.Gebruikersnaam = strCleanUsername;
                gebruiker.Gebruikersrol = 1;
                gebruiker.ProjectleiderId = -1;
                gebruiker.IsProjectleider = false;
                gebruiker.Windowsidentity = strWindowsUser;
                gebruiker.ContactIdOrigineel = -1;
                gebruiker.CanPrint = false;

                dbrep.datacontext.Gebruikers.InsertOnSubmit(gebruiker);

                dbrep.datacontext.SubmitChanges();

                // geef deze gebruiker een automatisch gegenereerd projectleiderId
                gebruiker.ProjectleiderId = 100 + gebruiker.GebruikerId;
                dbrep.datacontext.SubmitChanges();

                // set gebruiker in sessie
                ApplicationState.SetValue(ApplicationVariables.intGebruikerId, gebruiker.GebruikerId);
                ApplicationState.SetValue(ApplicationVariables.objGebruiker, gebruiker);
                // set de projectleiderID in de sessie
                ApplicationState.SetValue(ApplicationVariables.intProjectleider, gebruiker.ProjectleiderId);

                // informeer gebruiker dat er geen projectleider gekoppeld is, en de gebruiker alleen lees rechten heeft
                //MessageBox.Show("Welkom " + gebruiker.Gebruikersnaam + ", er is nog geen account aan uw windows identiteit gekoppeld. Neem contact op met een systeembeheerder om een account aan uw windows identiteit te koppelen. U heeft voorlopig alleen lees-rechten.");

            }

        }

        public void SetButtons()
        {
            //  set rechten
            if (Rechten.IsAdmin)
            {
                btnAdmin.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                btnAdmin.Visibility = System.Windows.Visibility.Collapsed;
            }

            //  set rechten
            if (Rechten.CanLoginAsProjectleider)
            {
                btnLogin.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                btnLogin.Visibility = System.Windows.Visibility.Collapsed;
            }

            if (Rechten.CanPrint)
            {
                btnMDR2Excel.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                btnMDR2Excel.Visibility = System.Windows.Visibility.Collapsed;
            }

        }

        private void btnProjecten_Click(object sender, RoutedEventArgs e)
        {

            Projecten.Intro intro = new Projecten.Intro();
            //intro.Load();

            // load the page into the contentcontrol
            //((PageContainer)((DockPanel)((ContentControl)Parent).Parent).Parent).LoadControl(vov);
            Tools.FindVisualParent<PageContainer>(this).LoadControl(intro, false);

        }

        private void btnVakmannen_Click(object sender, RoutedEventArgs e)
        {
            //// create the page and load all values
            //VakmannenOverview vov = new VakmannenOverview(false);
            ////vov.LoadView();

            //// load the page into the contentcontrol
            ////((PageContainer)((DockPanel)((ContentControl)Parent).Parent).Parent).LoadControl(vov);
            //Tools.FindVisualParent<PageContainer>(this).LoadControl(vov, false);

            Vakmannen.Intro intro = new Vakmannen.Intro();
            //intro.Load();

            // load the page into the contentcontrol
            //((PageContainer)((DockPanel)((ContentControl)Parent).Parent).Parent).LoadControl(vov);
            Tools.FindVisualParent<PageContainer>(this).LoadControl(intro, false);


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

        private void btnAdmin_Click(object sender, RoutedEventArgs e)
        {
            //WindowsFormsControlLibrary1.UserControl1 test2 = new WindowsFormsControlLibrary1.UserControl1();
            ////WindowsFormsApplication3.UserControl2 test = new WindowsFormsApplication3.UserControl2();
            //Tools.FindVisualParent<PageContainer>(this).LoadControl(test2);

            // create the page and load all values
            Admin.Intro plv = new Admin.Intro();

            // load the page into the contentcontrol
            //((PageContainer)((DockPanel)((ContentControl)Parent).Parent).Parent).LoadControl(plv);
            Tools.FindVisualParent<PageContainer>(this).LoadControl(plv, false);
        }

        private void btnLogin_Click_1(object sender, RoutedEventArgs e)
        {
            // create the page and load all values
            Login.Intro plv = new Login.Intro();

            // load the page into the contentcontrol
            //((PageContainer)((DockPanel)((ContentControl)Parent).Parent).Parent).LoadControl(plv);
            Tools.FindVisualParent<PageContainer>(this).LoadControl(plv, false);

            //WindowsFormsControlLibrary1.UserControl1 test2 = new WindowsFormsControlLibrary1.UserControl1();
            ////WindowsFormsApplication3.UserControl2 test = new WindowsFormsApplication3.UserControl2();
            //Tools.FindVisualParent<PageContainer>(this).LoadControl(test2);

        }

        private bool IsControlKeyDown = false;
        private bool keyM = false;
        private bool keyD = false;
        private bool keyR = false;

        private void NavigationControl_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl)
            {
                IsControlKeyDown = true;

            }

            if (e.Key == Key.M)
            {
                keyM = true;
            }

            if (e.Key == Key.D)
            {
                keyD = true;
            }

            if (e.Key == Key.R)
            {
                keyR = true;
            }

            if (IsControlKeyDown && keyM && keyD && keyR)
            {

                btnAdmin.Visibility = System.Windows.Visibility.Visible;
                btnLogin.Visibility = System.Windows.Visibility.Visible;

                ApplicationState.SetValue(ApplicationVariables.blnControlMDR, true);
            }

        }

        private void NavigationControl_PreviewKeyUp_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl)
            {
                IsControlKeyDown = false;
                keyM = false;
                keyD = false;
                keyR = false;
            }
        }

        private void btnMDR2Excel_Click(object sender, RoutedEventArgs e)
        {
            // create the page and load all values
            try
            {
                MDR2PDF.USMarioStart mdr2excel = new MDR2PDF.USMarioStart();

                // load the page into the contentcontrol
                //((PageContainer)((DockPanel)((ContentControl)Parent).Parent).Parent).LoadControl(plv);
                mdr2excel.PageGereedButtonText = "BEVESTIG";
                mdr2excel.PageTitle = "Printlijst";
                mdr2excel.PageSubtitle = "Kies een rapport in de lijst";
                //mdr2excel.OkClick += mdr2excel.MakeExcel();
                //.MakeExcel;
                mdr2excel.PageGereedButtonVisibility = System.Windows.Visibility.Visible;

                Tools.FindVisualParent<PageContainer>(this).LoadControl(mdr2excel);
            }
            catch (Exception ex)
            {
                Logging log = new Logging();
                log.Log("marino", ex.Message);

            }
        }

        private void btnPrintTest_Click(object sender, RoutedEventArgs e)
        {
            Printen.Intro printtest = new Printen.Intro();
            printtest.PageGereedButtonVisibility = System.Windows.Visibility.Hidden;
            printtest.PageTitle = "Printen van html naar pdf";

            Tools.FindVisualParent<PageContainer>(this).LoadControl(printtest, false);

        }

    }
}
