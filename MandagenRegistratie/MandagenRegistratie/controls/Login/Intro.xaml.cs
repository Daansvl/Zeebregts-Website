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
using MandagenRegistratieDomain;
using ZeebregtsLogic;

namespace MandagenRegistratie.controls.Login
{
    /// <summary>
    /// Interaction logic for ProjectleidersOverzicht.xaml
    /// </summary>
    public partial class Intro : MenuControl
    {
        public Intro()
        {
            InitializeComponent();
            #region Pagina specifieke informatie
            PageTitle = "Inloggen als...";
            PageSubtitle = "Ingelogd als " + ApplicationState.GetValue<Gebruiker>(ApplicationVariables.objGebruiker).Gebruikersnaam;

            if (Rechten.CanLoginAsProjectleider || ApplicationState.GetValue<bool>(ApplicationVariables.blnControlMDR))
            {
                PageGereedButtonVisibility = System.Windows.Visibility.Visible;
            }
            else
            {
                PageGereedButtonVisibility = System.Windows.Visibility.Hidden;
            }


            PageBackButtonText = "TERUG";
            PageOKButtonText = "WIJZIGEN";
            #endregion

            this.Reloaded += Load;
            this.OkClick += Wijzigen;

            Load();
        }

        class PL
        {
            public PL()
            {

            }


            public string Naam { get; set; }
            public int GebruikerId { get; set; }
            public int ContactIdOrigineel { get; set; }
        }


        /// 
        /// </summary>
        public void Load()
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                // intialize
                dbRepository dbrep = new dbRepository();
                dbOriginalRepository dbrepOriginal = new dbOriginalRepository();

                List<PL> listPL = new List<PL>();

                // admin gebruiker kan iedereen zien/worden
                foreach (Gebruiker plitem in dbrep.GetProjectleiders().Where(g => g.IsProjectleider || Rechten.IsAdmin))
                {
                    MDRpersoon tt = dbrepOriginal.GetContact(plitem.ContactIdOrigineel);

                    PL pl = new PL();
                    //pl.Naam = plitem.ProjectleiderId.ToString() + ": " + (tt.voornaam + " " + tt.tussenvoegsel + " " + tt.achternaam).ToStringTrimmed();
                    pl.Naam = plitem.Gebruikersnaam;
                    pl.GebruikerId = plitem.GebruikerId;
                    pl.ContactIdOrigineel = plitem.ContactIdOrigineel;

                    listPL.Add(pl);
                }

                cbProjectleiders.ItemsSource = listPL;
                cbProjectleiders.DisplayMemberPath = "Naam";
                cbProjectleiders.SelectedValuePath = "GebruikerId";

                cbProjectleiders.SelectedValue = ApplicationState.GetValue<Gebruiker>(ApplicationVariables.objGebruiker).GebruikerId;
            }
            catch { }
            finally
            {
                Mouse.OverrideCursor = null;
            }

        }

        //public static void LoginAls(int gebruikerId, bool isFirstLogin)
        //{
        //    dbRepository dbrep = new dbRepository();
        //    Gebruiker gebruiker = dbrep.GetGebruiker((int)gebruikerId);

        //    if (gebruiker != null)
        //    {

        //        // set gebruiker in sessie
        //        ApplicationState.SetValue(ApplicationVariables.intGebruikerId, gebruiker.GebruikerId);
        //        ApplicationState.SetValue(ApplicationVariables.objGebruiker, gebruiker);

        //        if (isFirstLogin)
        //        {
        //            ApplicationState.SetValue(ApplicationVariables.objGebruikerFirst, gebruiker);
        //        }

        //        Setting setting = dbrep.datacontext.Settings.Where(s => s.GebruikerId == ApplicationState.GetValue<int>(ApplicationVariables.intGebruikerId) && s.SettingsNaam == ApplicationVariables.strVakmanIdsAdding).FirstOrDefault();

        //        if (setting != null)
        //        {
        //            ApplicationState.SetValue(ApplicationVariables.listSelectedVakmanIdsAdding, setting.SettingsValue.FromStringToList());
        //        }

        //        Setting setting2 = dbrep.datacontext.Settings.Where(s => s.GebruikerId == ApplicationState.GetValue<int>(ApplicationVariables.intGebruikerId) && s.SettingsNaam == ApplicationVariables.strVakmanIdsDeleting).FirstOrDefault();

        //        if (setting != null)
        //        {
        //            ApplicationState.SetValue(ApplicationVariables.listSelectedVakmanIdsDeleting, setting2.SettingsValue.FromStringToList());
        //        }



        //        try
        //        {
        //            // set de projectleiderID in de sessie
        //            ApplicationState.SetValue(ApplicationVariables.intProjectleider, gebruiker.ProjectleiderId);

        //            // haal de projectleider op
        //            //Projectleider projectleider = dbrep.GetProjectleider((int)gebruiker.ProjectleiderId);

        //            // haal bijbehorende contact op uit database daan
        //            dbOriginalRepository dbrepOr = new dbOriginalRepository();
        //            persoon contactpersoon = dbrepOr.GetContact(gebruiker.ContactIdOrigineel);

        //            // informeer gebruiker dat ie automatisch ingelogd is
        //            //MessageBox.Show("Welkom " + gebruiker.Gebruikersnaam + ", u bent automatisch ingelogd als projectleider: " + (contactpersoon.voornaam + " " + contactpersoon.tussenvoegsel + " " + contactpersoon.achternaam).ToStringTrimmed());
        //            //MessageBox.Show("Welkom " + gebruiker.Gebruikersnaam + ", u bent automatisch ingelogd als projectleider: " + gebruiker.Gebruikersnaam);
        //        }
        //        catch
        //        {
        //            // informeer gebruiker dat er geen projectleider gekoppeld is, en de gebruiker alleen lees rechten heeft
        //            //MessageBox.Show("Welkom " + gebruiker.Gebruikersnaam + ", er is nog geen account aan uw windows identiteit gekoppeld. Neem contact op met een systeembeheerder om een account aan uw windows identiteit te koppelen. U heeft voorlopig alleen lees-rechten.");

        //        }

        //    }

        //}


        public void Wijzigen()
        {


            //LoginAls((int)cbProjectleiders.SelectedValue, false);

            // na het saven terug in history

            Edit edit = new Edit();
            edit.Load();
            PageGoToPage(edit);
            
            // PageGoBack();

        }

        private void btnResetChat_Click(object sender, RoutedEventArgs e)
        {
            PageContainer pc = Tools.FindVisualParent<PageContainer>(this);
            //pc.LoadCommunicationChannel(ApplicationState.GetValue<string>(ApplicationVariables.strEndpoint));
            pc.LoadCommunicationChannel();

            Load();
        }

    }
}
