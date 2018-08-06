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

namespace MandagenRegistratie.controls.Admin
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
            PageTitle = "Instellingen";
            PageSubtitle = "Gebruikers";

            if (Rechten.IsAdmin || ApplicationState.GetValue<bool>(ApplicationVariables.blnControlMDR))
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
                }
                else
                {
                    lblChatStatus.Content = "Niet actief: kon niet verbinden met " + ApplicationState.GetValue<string>(ApplicationVariables.strEndpoint) + ". Controleer poort :8089";
                }
            }
            catch { }
            finally { Mouse.OverrideCursor = null; }

        }


        public void Wijzigen()
        {
            Edit edit = new Edit();
            edit.Load();
            PageGoToPage(edit);
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


    }
}
