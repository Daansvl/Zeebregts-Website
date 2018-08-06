using System;
using System.Collections.Generic;
using System.Configuration;
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

namespace MandagenRegistratie.controls.Admin
{
    /// <summary>
    /// Interaction logic for Edit.xaml
    /// </summary>
    public partial class Edit : MenuControl
    {
        public Edit()
        {
            InitializeComponent();
            #region Pagina specifieke informatie
            PageTitle = "Wijzigen";
            PageSubtitle = "Gebruikers wijzigen";

            if (Rechten.IsAdmin || ApplicationState.GetValue<bool>(ApplicationVariables.blnControlMDR))
            {
                PageGereedButtonVisibility = System.Windows.Visibility.Visible;
            }
            else
            {
                PageGereedButtonVisibility = System.Windows.Visibility.Hidden;
            }


            PageBackButtonText = "ANNULEER";
            PageOKButtonText = "OPSLAAN";
            #endregion

            this.Reloaded += Load;
            this.OkClick += Save;

            Load();
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

                dgGebruikers.ItemsSource = dbrep.datacontext.Gebruikers;
                //listGebruikers = dbrep.datacontext.Gebruikers.ToList();
                //listProjectleiders = dbrep.datacontext.Projectleiders.ToList();



                // chatstatus
                if (ApplicationState.GetValue<bool>(ApplicationVariables.blnChatStatus))
                {
                    lblChatStatus.Content = "Actief: verbonden met " + ApplicationState.GetValue<string>(ApplicationVariables.strEndpoint);
                    lblResetChat.Visibility = System.Windows.Visibility.Collapsed;
                    btnResetChat.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    lblResetChat.Visibility = System.Windows.Visibility.Visible;
                    btnResetChat.Visibility = System.Windows.Visibility.Visible;
                    lblChatStatus.Content = "Niet actief: kon niet verbinden met " + ApplicationState.GetValue<string>(ApplicationVariables.strEndpoint) + ". Controleer poort :8089";
                }

                cbWeekviewLeesstand.IsChecked = Global.useWeekviewLeesstand; // Convert.ToBoolean(ConfigurationManager.AppSettings["WeekviewLeesstand"]);

            }
            catch (Exception ex) 
            {
            
            }
            finally { Mouse.OverrideCursor = null; }

        }


        private void btnResetChat_Click(object sender, RoutedEventArgs e)
        {
            PageContainer pc = Tools.FindVisualParent<PageContainer>(this);
            //pc.LoadCommunicationChannel(ApplicationState.GetValue<string>(ApplicationVariables.strEndpoint));
            pc.LoadCommunicationChannel();

            Load();
        }


        private void Save()
        {
            List<int> lstBewaren = new List<int>();

            dbRepository dbrep = new dbRepository();
            dbOriginalRepository dbrepOriginal = new dbOriginalRepository();
            foreach (object obj in dgGebruikers.Items)
            {
                if (obj.GetType() == typeof(Gebruiker))
                {
                    Gebruiker gb = (Gebruiker)obj;
                    bool IsInserting = false;

                    Gebruiker ge = dbrep.GetGebruiker(gb.GebruikerId);

                    if (ge == null)
                    {
                        ge = new Gebruiker();
                        IsInserting = true;
                    }

                    ge.IsAdministrator = gb.IsAdministrator;
                    ge.CanLoginAsProjectleider = gb.CanLoginAsProjectleider;
                    ge.Gebruikersnaam = gb.Gebruikersnaam;
                    ge.Gebruikersrol = IsInserting ? 1 : gb.Gebruikersrol;
                    ge.ProjectleiderId = gb.ProjectleiderId;
                    ge.IsProjectleider = gb.IsProjectleider;
                    ge.Windowsidentity = gb.Windowsidentity;
                    ge.PersoonNrOrigineel = gb.PersoonNrOrigineel;
                    ge.CanPrint = gb.CanPrint;
                    ge.IsManager = gb.IsManager;

                    MDRpersoon pp = dbrepOriginal.datacontext.MDRpersoons.Where(p => p.persoon_nr == gb.PersoonNrOrigineel).FirstOrDefault();
                    if (pp != null)
                    {
                        ge.ContactIdOrigineel = pp.persoon_ID;
                    }

                    if (IsInserting)
                    {
                        dbrep.datacontext.Gebruikers.InsertOnSubmit(ge);
                        dbrep.datacontext.SubmitChanges();

                        ge.ProjectleiderId = 100 + ge.GebruikerId;
                        dbrep.datacontext.SubmitChanges();

                    }

                    // opnieuw inloggen als deze gebruiker als ik het was, zodat de rechten ververst worden
                    if (ge.GebruikerId == ApplicationState.GetValue<Gebruiker>(ApplicationVariables.objGebruiker).GebruikerId)
                    {
                        dbrep.datacontext.SubmitChanges();

                        Login.Edit.LoginAls(ge.GebruikerId, true, this);
                    }

                    // deze bewaren
                    lstBewaren.Add(ge.GebruikerId);

                }

            }

            dbrep.datacontext.SubmitChanges();


            foreach (Gebruiker geb in dbrep.datacontext.Gebruikers)
            {
                // als deze was verwijderd uit de lijst.... verwijderen uit database
                if (!lstBewaren.Contains(geb.GebruikerId))
                {

                    if (dbrep.GetProjectsByProjectleiderId(geb.ProjectleiderId).Count == 0)
                    {
                        dbrep.datacontext.Gebruikers.DeleteOnSubmit(geb);

                    }
                    else
                    {
                        MessageBox.Show("Kan geen gebruikers deleten met bijbehorende projecten. Gebruiker: [ " + geb.Gebruikersnaam + " ]");
                    }
                }
            }

            dbrep.datacontext.SubmitChanges();

            //Load();

            //ConfigurationManager.AppSettings.Remove("WeekviewLeesstand");
            //ConfigurationManager.AppSettings.Add("WeekviewLeesstand", cbWeekviewLeesstand.IsChecked == true ? "true" : "false");
            
            //SetAppsettings("WeekviewLeesstand", cbWeekviewLeesstand.IsChecked == true ? "true" : "false");

            Global.useWeekviewLeesstand = cbWeekviewLeesstand.IsChecked == true;

            PageGoBack();
        }

        private void SetAppsettings(string key, string value)
        {
            string exePath = System.IO.Path.Combine(Environment.CurrentDirectory, "MandagenRegistratie.exe");
            //Configuration config = ConfigurationManager.OpenExeConfiguration("~/App.config");
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(exePath);
            var setting = config.AppSettings.Settings[key];
            if (setting != null)
            {
                setting.Value = value;
            }
            else
            {
                config.AppSettings.Settings.Add(key, value);
            }

            config.Save();
        }

        private void dgGebruikers_AutoGeneratingColumn_1(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "GebruikerId")
            {
                e.Cancel = true;
            }

            if (e.PropertyName == "Gebruikersrol")
            {
                e.Cancel = true;
            }

            if (e.PropertyName == "ContactIdOrigineel")
            {
                e.Cancel = true;
            }

            if (e.PropertyName == "ProjectleiderId")
            {
                //e.Cancel = true;
            }

            if (e.PropertyName == "Gebruikersnaam")
            {
                e.Column.Width = 200;
            }

            if (e.PropertyName == "Windowsidentity")
            {
                e.Column.Width = 250;
            }

        }

        private void btnSetLoondienst_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                dbRepository dbrep = new dbRepository();

                foreach (Vakman v in dbrep.GetVakmannenAll())
                {
                    if (v.BedrijfIdOrigineel == 1)
                    {
                        v.ZZP = false;
                    }
                    else
                    {
                        v.ZZP = true;
                    }
                }

                dbrep.datacontext.SubmitChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnResetProjectleiders_Click(object sender, RoutedEventArgs e)
        {
            dbRepository dbrep = new dbRepository();

            //foreach (Gebruiker gebruiker in dbrep.datacontext.Gebruikers)
            //{
            //    if (gebruiker.ProjectleiderId < 50)
            //    {
            //        Projectleider pl = dbrep.datacontext.Projectleiders.Where(p => p.ProjectleiderId == gebruiker.ProjectleiderId).FirstOrDefault();

            //        gebruiker.ProjectleiderId = 100 + gebruiker.GebruikerId;
            //        gebruiker.ContactIdOrigineel = pl.ContactIdOrigineel;


            //        foreach (Project project in dbrep.datacontext.Projects.Where(p => p.ProjectleiderId == pl.ProjectleiderId))
            //        {
            //            project.ProjectleiderId = gebruiker.ProjectleiderId;
            //        }

            //        foreach (Mandagen mandag in dbrep.datacontext.Mandagens.Where(m => m.ProjectleiderId == pl.ProjectleiderId))
            //        {
            //            mandag.ProjectleiderId = gebruiker.ProjectleiderId;
            //            mandag.MutatieDoorProjectleiderId = gebruiker.ProjectleiderId;
            //        }

            //        dbrep.datacontext.SubmitChanges();


            //    }
            //}

            Load();

        }

        private void btnPersoonNrs_Click(object sender, RoutedEventArgs e)
        {
            dbRepository dbrep = new dbRepository();
            dbOriginalRepository dbrepOriginal = new dbOriginalRepository();

            foreach (Gebruiker gebruiker in dbrep.datacontext.Gebruikers)
            {
                if (gebruiker.PersoonNrOrigineel < 0)
                {
                    MDRpersoon pp = dbrepOriginal.datacontext.MDRpersoons.Where(p => p.persoon_ID == gebruiker.ContactIdOrigineel).FirstOrDefault();

                    gebruiker.PersoonNrOrigineel = pp.persoon_nr == null ? -1 : (int)pp.persoon_nr;

                    dbrep.datacontext.SubmitChanges();


                }
            }

            Load();

        }

        private void btnCleanProjects_Click(object sender, RoutedEventArgs e)
        {
            dbRepository db = new dbRepository();
            int countProjects = 0;
            int countMandagen = 0;

            foreach(Project p in db.GetProjects())
            {
                bool deleteProject = true;

                foreach (Mandagen mandag in p.Mandagens)
                {
                    if(mandag.Begintijd != mandag.Eindtijd)
                    {
                        deleteProject = false;
                    }

                }

                if(deleteProject)
                {

                    foreach(Mandagen m in p.Mandagens)
                    {
                        countMandagen ++;
                        db.datacontext.Mandagens.DeleteOnSubmit(m);
                    }

                    countProjects ++;
                    db.datacontext.Projects.DeleteOnSubmit(p);
                }

                db.datacontext.SubmitChanges();

            }

            lblResultMessage.Content = countProjects.ToString() + " projects deleted.";
            lblResultMessage.Content += Environment.NewLine;
            lblResultMessage.Content += countMandagen.ToString() + " mandagen records deleted.";

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            dbRepository dbrep = new dbRepository();
            int countMandagen = 0;
            int countEdits = 0;

            foreach (Mandagen mandag in dbrep.datacontext.Mandagens)
            {
                long lnMandagTicks = (mandag.Eindtijd.Ticks - mandag.Begintijd.Ticks);
                TimeSpan tsMandag = TimeSpan.FromTicks(lnMandagTicks);
                if (tsMandag.Hours != mandag.Uren || tsMandag.Minutes != mandag.Minuten)
                {
                    mandag.Uren = tsMandag.Hours;
                    mandag.Minuten = tsMandag.Minutes;

                    countEdits++;
                }

                countMandagen++;
            }

            lblResultMessage.Content = countMandagen.ToString() + " mandagen records inspected.";
            lblResultMessage.Content += Environment.NewLine;
            lblResultMessage.Content += countEdits.ToString() + " mandagen records edited.";

            dbrep.datacontext.SubmitChanges();
        }

        private void btnComparison_Click(object sender, RoutedEventArgs e)
        {
            Admin.Comparison intro = new Admin.Comparison();
            intro.Load();

            // load the page into the contentcontrol
            //((PageContainer)((DockPanel)((ContentControl)Parent).Parent).Parent).LoadControl(vov);
            Tools.FindVisualParent<PageContainer>(this).LoadControl(intro, false);


        }

        private void btnLogboek_Click_1(object sender, RoutedEventArgs e)
        {
            Admin.LoggingPage intro = new Admin.LoggingPage();
            intro.Load();

            // load the page into the contentcontrol
            //((PageContainer)((DockPanel)((ContentControl)Parent).Parent).Parent).LoadControl(vov);
            Tools.FindVisualParent<PageContainer>(this).LoadControl(intro, false);
        }
    }
}
