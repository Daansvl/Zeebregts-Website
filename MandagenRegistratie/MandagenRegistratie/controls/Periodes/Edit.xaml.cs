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

namespace MandagenRegistratie.controls.Periodes
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
                //dbOriginalRepository dbrepOriginal = new dbOriginalRepository();

              


            }
            catch { }
            finally { Mouse.OverrideCursor = null; }

        }


        private void btnResetChat_Click(object sender, RoutedEventArgs e)
        {
            PageContainer pc = Tools.FindVisualParent<PageContainer>(this);
            //pc.LoadCommunicationChannel(ApplicationState.GetValue<string>(ApplicationVariables.strEndpoint));
            pc.LoadCommunicationChannel();

            Load();
        }



        private void SetAppsettings(string key, string value)
        {
            string exePath = System.IO.Path.Combine(Environment.CurrentDirectory, "MandagenRegistratie.exe");
            //Configuration config = ConfigurationManager.OpenExeConfiguration("~/App.config");
            Configuration config = ConfigurationManager.OpenExeConfiguration(exePath);
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
                    if (v.Bedrijf.BedrijfIdOrigineel == 1)
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

            foreach (Gebruiker gebruiker in dbrep.datacontext.Gebruikers)
            {
                if (gebruiker.ProjectleiderId < 50)
                {
                    Projectleider pl = dbrep.datacontext.Projectleiders.Where(p => p.ProjectleiderId == gebruiker.ProjectleiderId).FirstOrDefault();

                    gebruiker.ProjectleiderId = 100 + gebruiker.GebruikerId;
                    gebruiker.ContactIdOrigineel = pl.ContactIdOrigineel;


                    foreach (Project project in dbrep.datacontext.Projects.Where(p => p.ProjectleiderId == pl.ProjectleiderId))
                    {
                        project.ProjectleiderId = gebruiker.ProjectleiderId;
                    }

                    foreach (Mandagen mandag in dbrep.datacontext.Mandagens.Where(m => m.ProjectleiderId == pl.ProjectleiderId))
                    {
                        mandag.ProjectleiderId = gebruiker.ProjectleiderId;
                        mandag.MutatieDoorProjectleiderId = gebruiker.ProjectleiderId;
                    }

                    dbrep.datacontext.SubmitChanges();


                }
            }

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
                    persoon pp = dbrepOriginal.datacontext.persoons.Where(p => p.persoon_ID == gebruiker.ContactIdOrigineel).FirstOrDefault();

                    gebruiker.PersoonNrOrigineel = pp.persoon_nr == null ? -1 : (int)pp.persoon_nr;

                    dbrep.datacontext.SubmitChanges();


                }
            }

            Load();

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
